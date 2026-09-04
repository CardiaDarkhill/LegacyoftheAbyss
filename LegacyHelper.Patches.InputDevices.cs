#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using InControl;
using UnityEngine;
using GlobalEnums;

// Keeping a device reserved for the Shade: which InputDevices and which HeroActions the game is
// allowed to see, and the rebinding paths that would otherwise hand a reserved device back.
// MenuInputBridge, which lets the reserved device drive menus, is in
// LegacyHelper.Patches.MenuInput.cs.
public partial class LegacyHelper
{
    [HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
    private class BlockKeyboardRebinding
    {
        private static bool Prefix()
        {
            // EffectiveKeyboardEnabled, not the raw config flag: while an AI drives the Shade there
            // is no second player to reserve a device for, so Hornet gets the keyboard back.
            return HornetInput.EffectiveKeyboardEnabled();
        }
    }

    [HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
    private class BlockDefaultKeyboardMap
    {
        private static bool Prefix()
        {
            return HornetInput.EffectiveKeyboardEnabled();
        }
    }

    [HarmonyPatch(typeof(InputHandler), "MapControllerButtons")]
    private class ControlControllerMapping
    {
        private static bool Prefix()
        {
            try
            {
                var cfg = ModConfig.Instance;
                if (cfg == null)
                    return true;

                if (HornetInput.EffectiveControllerEnabled())
                    return true;

                var shadeConfig = cfg.shadeInput;
                if (shadeConfig != null && shadeConfig.UsesControllerBindings())
                    return true;

                return false;
            }
            catch
            {
                return true;
            }
        }
    }

    internal static class InputDeviceBlocker
    {
        private static readonly HashSet<InputDevice> restrictedShadeDevices = new();

        /// <summary>
        /// Pads the Shade player owns, whether or not their input is being blocked this instant.
        /// <para>
        /// Distinct from <see cref="restrictedShadeDevices"/>, which means "shade-owned <em>and</em>
        /// blocked right now" and is therefore empty during a menu or a scene load - exactly the
        /// moments when it matters most to know whose pad a device is. Ownership does not change
        /// because the map is open.
        /// </para>
        /// </summary>
        private static readonly HashSet<InputDevice> shadeOwnedDevices = new();
        private static readonly List<InputDevice> cleanupList = new();
        // Names must match HeroActions.CreatePlayerAction(...) exactly; the lookup is ordinal.
        // "Quick Map" in particular is two words there - the old "QuickMap" spelling silently
        // matched nothing, so the quick-map bind was blocked along with everything else.
        private static readonly HashSet<string> AllowedHeroActions = new(StringComparer.Ordinal)
        {
            "Pause",
            "openInventory",
            "openInventoryMap",
            "openInventoryJournal",
            "openInventoryTools",
            "openInventoryQuests",
            "Quick Map"
        };

        /// <summary>
        /// Hero actions a shade-owned device is still allowed to drive. Pause and the menu-open
        /// binds belong to the player, not to whichever entity a pad happens to be assigned to.
        /// </summary>
        internal static IReadOnlyCollection<string> AllowedHeroActionNames => AllowedHeroActions;

        /// <summary>
        /// What the game's own inventory and map actions are currently bound to, for
        /// <c>BugReportState</c>. "The inventory will not open" has two very different causes -
        /// the key doing nothing because the action lost its binding, or the press being taken
        /// away from the player - and from outside they look identical.
        /// </summary>
        internal static string DescribeHeroMenuBindings()
        {
            try
            {
                var actions = InputHandler.UnsafeInstance?.inputActions;
                if (actions == null)
                {
                    return "no InputHandler";
                }

                var parts = new List<string>();
                foreach (var action in actions.Actions)
                {
                    if (action == null || !AllowedHeroActions.Contains(action.Name))
                    {
                        continue;
                    }

                    var bindings = action.UnfilteredBindings;
                    int count = bindings?.Count ?? 0;
                    var sources = new List<string>();
                    for (int i = 0; i < count && i < 8; i++)
                    {
                        var binding = bindings[i];
                        if (binding != null)
                        {
                            sources.Add(binding.Name ?? binding.BindingSourceType.ToString());
                        }
                    }

                    // The count matters as much as the names: several callers re-add these bindings
                    // on every hero-actions rebuild, so a list that keeps growing is its own bug and
                    // is invisible from a truncated sample.
                    string suffix = count > sources.Count ? $"/+{count - sources.Count}" : string.Empty;
                    parts.Add($"{action.Name}({count})=[{string.Join("/", sources)}{suffix}]");
                }

                return parts.Count == 0 ? "no menu actions found" : string.Join(", ", parts);
            }
            catch (System.Exception e)
            {
                return "unreadable: " + e.Message;
            }
        }

        /// <summary>
        /// Who owns which pad this instant, and what the game is being told about it.
        /// <para>
        /// Written for the bug report rather than the log. "Hornet cannot be controlled" is a
        /// report about the contents of three static sets and one property, none of which is
        /// visible from a screenshot - the first report of it arrived with no way to tell whether
        /// the active device had been left on the Shade's pad, whether a pad had been reserved that
        /// nobody was holding, or whether the input path was not involved at all.
        /// </para>
        /// </summary>
        internal static string DescribeDeviceOwnership()
        {
            try
            {
                var parts = new List<string>();

                var devices = InputManager.Devices;
                int count = devices?.Count ?? 0;
                if (count == 0)
                {
                    parts.Add("devices: none");
                }
                else
                {
                    var described = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        var device = devices[i];
                        if (device == null)
                        {
                            described.Add(i.ToString(CultureInfo.InvariantCulture) + ":<null>");
                            continue;
                        }

                        var flags = new List<string>();
                        if (IsShadeOwnedDevice(device)) flags.Add("shade");
                        if (IsRestrictedDevice(device)) flags.Add("restricted");
                        try
                        {
                            if (!device.IsAttached) flags.Add("detached");
                        }
                        catch
                        {
                            flags.Add("unreadable");
                        }

                        described.Add(FormattableString.Invariant(
                            $"{i}:{device.Name}[{string.Join("+", flags.ToArray())}]"));
                    }

                    parts.Add("devices: " + string.Join(", ", described.ToArray()));
                }

                var active = InputManager.ActiveDevice;
                parts.Add("active: " + (active == null || active == InputDevice.Null ? "none" : active.Name));

                var lastHornet = InputManager_UpdateActiveDevice_BlockShadeDevices.LastHornetDevice;
                parts.Add("lastHornet: "
                    + (lastHornet == null || lastHornet == InputDevice.Null ? "none" : lastHornet.Name));

                parts.Add("blocking: " + ShouldBlockShadeDeviceInput().ToString());
                parts.Add("hornetKeyboard: " + HornetInput.EffectiveKeyboardEnabled().ToString());

                // How often her keyboard has had to be put back. Non-zero means something is
                // clearing her bindings and not remapping them, which is still unidentified - the
                // repair hides the symptom rather than explaining it.
                parts.Add("keyboardRepairs: " + HornetInput.KeyboardRepairs.ToString(CultureInfo.InvariantCulture));
                parts.Add("hornetController: " + HornetInput.EffectiveControllerEnabled().ToString());
                parts.Add("shadeAi: " + HornetInput.ShadeAiHoldsTheShade().ToString());

                var handler = InputHandler.UnsafeInstance;
                if (handler != null)
                {
                    parts.Add("acceptingInput: " + handler.acceptingInput.ToString());
                    var actions = handler.inputActions;
                    if (actions != null)
                    {
                        var heroDevice = actions.Device;
                        parts.Add("heroDevice: "
                            + (heroDevice == null || heroDevice == InputDevice.Null ? "none" : heroDevice.Name));
                        parts.Add("heroLastInput: " + actions.LastInputType.ToString());
                    }
                }

                return string.Join(" | ", parts.ToArray());
            }
            catch (System.Exception e)
            {
                return "unreadable: " + e.Message;
            }
        }

        private static readonly Dictionary<InputDevice, bool> IgnoreDeviceCache = new();
        private static int ignoreDeviceCacheFrame = -1;
        private static int blockShadeCacheFrame = -1;
        private static bool blockShadeCacheValue;

        private static void SetDeviceRestricted(InputDevice device, bool restrict)
        {
            if (device == null || device == InputDevice.Null)
                return;

            if (restrict)
            {
                restrictedShadeDevices.Add(device);
            }
            else
            {
                restrictedShadeDevices.Remove(device);
            }
        }

        private static void ReleaseTrackedDevices()
        {
            shadeOwnedDevices.Clear();

            if (restrictedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            cleanupList.AddRange(restrictedShadeDevices);
            foreach (var device in cleanupList)
            {
                SetDeviceRestricted(device, false);
            }
            cleanupList.Clear();
        }

        private static void CleanupDetachedDevices(IList<InputDevice> devices)
        {
            if (restrictedShadeDevices.Count == 0)
                return;

            cleanupList.Clear();
            cleanupList.AddRange(restrictedShadeDevices);
            foreach (var device in cleanupList)
            {
                if (device == null || device == InputDevice.Null)
                {
                    SetDeviceRestricted(device, false);
                    continue;
                }

                if (devices == null || !ContainsDevice(devices, device))
                {
                    SetDeviceRestricted(device, false);
                }
            }

            cleanupList.Clear();
        }

        private static bool ContainsDevice(IList<InputDevice> list, InputDevice device)
        {
            if (list == null || device == null || device == InputDevice.Null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == device)
                    return true;
            }
            return false;
        }

        private static void EnsureIgnoreDeviceCache()
        {
            int frame = Time.frameCount;
            if (frame != ignoreDeviceCacheFrame)
            {
                IgnoreDeviceCache.Clear();
                ignoreDeviceCacheFrame = frame;
            }
        }

        internal static bool ShouldBlockShadeDeviceInput()
        {
            int frame;
            try
            {
                frame = Time.frameCount;
            }
            catch
            {
                // No Unity player loop (unit tests): skip the per-frame cache entirely.
                return EvaluateShouldBlockShadeDeviceInput();
            }

            if (frame == blockShadeCacheFrame)
            {
                return blockShadeCacheValue;
            }

            bool result = EvaluateShouldBlockShadeDeviceInput();

            blockShadeCacheFrame = frame;
            blockShadeCacheValue = result;
            return result;
        }

        /// <summary>
        /// The uncached decision: is the game in ordinary gameplay (so a shade-owned device must not
        /// drive Hornet), or is a menu/pause surface up (so it must)? Split out from the per-frame
        /// cache above so it can be exercised without a Unity player loop.
        /// </summary>
        internal static bool EvaluateShouldBlockShadeDeviceInput()
        {
            try
            {
                var gm = MenuStateUtility.TryGetGameManager();
                if (ReferenceEquals(gm, null))
                {
                    return false;
                }

                if (gm.GameState != GameState.PLAYING)
                {
                    return false;
                }

                var ui = MenuStateUtility.TryGetUiManager(gm);
                return !MenuStateUtility.IsMenuActive(gm, ui);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// True when <paramref name="device"/> is currently driving one of <see cref="AllowedHeroActions"/>.
        /// <para>
        /// InControl only polls <c>InputManager.ActiveDevice</c> for a <c>PlayerActionSet</c>, and
        /// this mod keeps a shade-owned pad from becoming the active device during gameplay - which
        /// would otherwise leave that pad unable to pause. Letting the frame through only while an
        /// allowed action is actually pressed restores pause and menu access without handing the pad
        /// any gameplay control; <c>PlayerAction_Update_BlockShadeGameplay</c> still nulls the device
        /// for everything outside the allow-list.
        /// </para>
        /// <para>
        /// <b>Must read <c>UnfilteredBindings</c>, never <c>Bindings</c>.</b> A binding reaches the
        /// visible <c>Bindings</c> list only if <c>IsValid</c> held when it was added, and for a
        /// <c>DeviceBindingSource</c> that is
        /// <c>BoundTo.Device.HasControl(control) || Utility.TargetIsStandard(control)</c>.
        /// <c>TargetIsStandard</c> covers only the stick/dpad/Action1-12 range, so every control this
        /// allow-list cares about - Back, Select, Options, Menu - fails it, and on a fresh boot
        /// <c>ActiveDevice</c> is null so <c>HasControl</c> is never consulted either.
        /// </para>
        /// <para>
        /// Reading <c>Bindings</c> therefore deadlocks: the pad may only become active while driving
        /// an allowed action, and that action's binding stays invisible until the pad has been active
        /// once. <c>GetState(device)</c> resolves against the device passed in rather than
        /// <c>BoundTo.Device</c>, so the unfiltered list is correct as well as sufficient.
        /// </para>
        /// </summary>

        internal static bool IsDrivingAllowedHeroAction(InputDevice device)
        {
            if (device == null || device == InputDevice.Null)
                return false;

            try
            {
                var actions = InputHandler.UnsafeInstance?.inputActions;
                if (actions == null)
                    return false;

                foreach (var action in actions.Actions)
                {
                    if (action == null || !AllowedHeroActions.Contains(action.Name))
                        continue;

                    var bindings = action.UnfilteredBindings;
                    if (bindings == null)
                        continue;

                    for (int i = 0; i < bindings.Count; i++)
                    {
                        var binding = bindings[i];
                        if (binding == null || binding.BindingSourceType != BindingSourceType.DeviceBindingSource)
                            continue;

                        if (binding.GetState(device))
                            return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// True when <paramref name="device"/> is holding down one of <paramref name="action"/>'s own
        /// device bindings right now.
        /// <para>
        /// Reads <c>UnfilteredBindings</c> for the reason spelled out on
        /// <see cref="IsDrivingAllowedHeroAction"/>: the menu/pause controls are all outside
        /// InControl's "standard" range, so they are absent from the visible <c>Bindings</c> list
        /// whenever the action's current device does not physically have them - which includes every
        /// frame before any device has become active at all.
        /// </para>
        /// <para>
        /// Only real <see cref="DeviceBindingSource"/>s count. The mod's own
        /// <c>ShadeMenuBindingSourceBase</c> bindings report a device binding source type but ignore
        /// the device they are handed entirely, so treating one as evidence that <i>this</i> device
        /// is pressed would be wrong.
        /// </para>
        /// </summary>
        internal static bool IsDeviceDrivingAction(PlayerAction action, InputDevice device)
        {
            if (action == null || device == null || device == InputDevice.Null)
                return false;

            try
            {
                var bindings = action.UnfilteredBindings;
                if (bindings == null)
                    return false;

                for (int i = 0; i < bindings.Count; i++)
                {
                    if (bindings[i] is DeviceBindingSource deviceBinding && deviceBinding.GetState(device))
                        return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Any attached device currently pressing one of <paramref name="action"/>'s own buttons, or
        /// null. Only ever called for <see cref="AllowedHeroActions"/>.
        /// <para>
        /// Deliberately does not ask which entity owns the device. That is the point of the
        /// allow-list: pause, the inventory-open binds and quick-map belong to the player, not to
        /// whichever entity a pad happens to be assigned to, so "some pad is pressing the pause
        /// button" is all this needs to know. Do not filter by the shade's configured controller
        /// index first: that assumes <c>shadeInput.controllerDeviceIndex</c> lines up with the
        /// device's position in <c>InputManager.Devices</c>, and that the shade is configured for a
        /// controller at all - either being wrong leaves the pad unable to open anything, silently.
        /// </para>
        /// </summary>
        internal static InputDevice FindDeviceDrivingAllowedAction(PlayerAction action)
        {
            if (action == null)
                return null;

            try
            {
                var devices = InputManager.Devices;
                if (devices == null || devices.Count == 0)
                    return null;

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    if (device == null || device == InputDevice.Null || device.IsUnknown)
                        continue;

                    if (IsDeviceDrivingAction(action, device))
                        return device;
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Whether the Shade has claimed every attached pad, in which case reserving them all would
        /// leave Hornet with nothing to play on.
        /// <para>
        /// Asks <c>ReservesControllerIndex</c> rather than <c>IsControllerIndexInUse</c>: the
        /// shade-command binding sits on Hornet's own pad by design, and counting it here made two
        /// pads look like two claimed pads. This escape hatch then fired on the ordinary
        /// two-controller setup and nothing was reserved at all - Hornet answered to both pads, so
        /// the Shade player's stick drove both characters.
        /// </para>
        /// </summary>
        internal static bool ShadeUsesAllControllers(ShadeInputConfig config, int targetIndex, int deviceCount)
        {
            if (config == null)
                return false;
            for (int i = 0; i < deviceCount; i++)
            {
                if (i == targetIndex)
                    continue;
                if (!config.ReservesControllerIndex(i))
                    return false;
            }
            return true;
        }

        internal static void RefreshShadeDevices()
        {
            if (!InputManager.IsSetup)
            {
                ReleaseTrackedDevices();
                return;
            }

            try
            {
                var cfg = ModConfig.Instance;
                if (cfg == null)
                {
                    ReleaseTrackedDevices();
                    return;
                }

                // Nobody is holding the Shade's controls while an AI drives it, so no device is
                // being reserved for a second player and every one of them belongs to Hornet.
                // Releasing rather than merely not-restricting matters: a device restricted before
                // the AI came on stays restricted until something lets it go.
                if (HornetInput.ShadeAiHoldsTheShade())
                {
                    ReleaseTrackedDevices();
                    return;
                }

                var shadeConfig = cfg.shadeInput;
                if (shadeConfig == null || !shadeConfig.ReservesAnyController())
                {
                    ReleaseTrackedDevices();
                    return;
                }

                var devices = InputManager.Devices;
                if (devices == null || devices.Count == 0)
                {
                    ReleaseTrackedDevices();
                    return;
                }

                for (int i = 0; i < devices.Count; i++)
                {
                    ShouldIgnoreDevice(devices[i]);
                }

                CleanupDetachedDevices(devices);
            }
            catch
            {
            }
        }

        internal static bool ShouldIgnoreDevice(InputDevice device)
        {
            EnsureIgnoreDeviceCache();

            bool canCache = device != null && device != InputDevice.Null;
            if (canCache && IgnoreDeviceCache.TryGetValue(device, out var cached))
            {
                return cached;
            }

            bool restrict = false;

            try
            {
                // Same reasoning as RefreshShadeDevices: with an AI on the Shade there is no second
                // player to reserve a pad for. Checked here too because this is called directly from
                // the per-device path as well as from the sweep.
                if (device != null && device != InputDevice.Null && !device.IsUnknown && !HornetInput.ShadeAiHoldsTheShade())
                {
                    var cfg = ModConfig.Instance;
                    if (cfg != null)
                    {
                        var shadeConfig = cfg.shadeInput;
                        if (shadeConfig != null && shadeConfig.ReservesAnyController())
                        {
                            var devices = InputManager.Devices;
                            if (devices != null && devices.Count > 0)
                            {
                                int index = devices.IndexOf(device);
                                if (index >= 0 && shadeConfig.ReservesControllerIndex(index))
                                {
                                    bool hornetWantsController = cfg.hornetControllerEnabled;
                                    bool shadeUsesAll = ShadeUsesAllControllers(shadeConfig, index, devices.Count);
                                    if (!shadeUsesAll || !hornetWantsController)
                                    {
                                        restrict = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                restrict = false;
            }

            if (device != null && device != InputDevice.Null)
            {
                if (restrict)
                {
                    shadeOwnedDevices.Add(device);
                }
                else
                {
                    shadeOwnedDevices.Remove(device);
                }
            }

            bool block = false;
            if (restrict)
            {
                block = ShouldBlockShadeDeviceInput();
            }

            SetDeviceRestricted(device, block);
            if (canCache)
            {
                IgnoreDeviceCache[device] = block;
            }
            return block;
        }

        internal static void ReleaseDevice(InputDevice device)
        {
            SetDeviceRestricted(device, false);
        }

        internal static bool IsRestrictedDevice(InputDevice device)
        {
            return device != null && device != InputDevice.Null && restrictedShadeDevices.Contains(device);
        }

        /// <summary>Whether this pad belongs to the Shade player, blocked this instant or not.</summary>
        internal static bool IsShadeOwnedDevice(InputDevice device)
        {
            return device != null && device != InputDevice.Null && shadeOwnedDevices.Contains(device);
        }

        /// <summary>
        /// A device Hornet may be handed back: real, still attached, and not the Shade player's.
        /// <para>
        /// Attachment rather than membership of <c>InputManager.Devices</c> on purpose. That list is
        /// controllers only, and Hornet is routinely on the keyboard while the Shade has the pad -
        /// requiring membership would refuse to hand her back her own input in exactly the setup the
        /// mod ships as its default.
        /// </para>
        /// </summary>
        internal static bool IsUsableHornetDevice(InputDevice device)
            => IsUsableHornetDevice(device, IsShadeOwnedDevice(device));

        /// <summary>
        /// The rule itself, with ownership handed in rather than looked up, so it can be exercised
        /// without an InputManager behind it.
        /// </summary>
        internal static bool IsUsableHornetDevice(InputDevice device, bool shadeOwned)
        {
            if (device == null || device == InputDevice.Null || device.IsUnknown)
                return false;

            if (shadeOwned)
                return false;

            try
            {
                return device.IsAttached;
            }
            catch
            {
                return false;
            }
        }

        internal static bool AllowsHeroAction(PlayerAction action)
        {
            try
            {
                if (action == null)
                    return false;
                string name = action.Name;
                if (string.IsNullOrEmpty(name))
                    return false;
                return AllowedHeroActions.Contains(name);
            }
            catch
            {
                return false;
            }
        }

        internal static void EnsureLastActiveController(InputHandler handler)
        {
            if (handler == null)
                return;
            try
            {
                var actions = handler.inputActions;
                if (actions == null)
                    return;
                if (handler.lastActiveController == BindingSourceType.None)
                {
                    handler.lastActiveController = actions.LastInputType;
                    handler.lastInputDeviceStyle = actions.LastDeviceStyle;
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(PlayerAction), nameof(PlayerAction.Update))]
    private class PlayerAction_Update_BlockShadeGameplay
    {
        private static void Prefix(PlayerAction __instance, ref InputDevice device)
        {
            try
            {
                if (__instance == null)
                    return;
                if (!(__instance.Owner is HeroActions))
                    return;

                if (InputDeviceBlocker.AllowsHeroAction(__instance))
                {
                    // Pause, the inventory-open binds and quick-map belong to the player, not to
                    // whichever entity owns the pad. InControl hands a PlayerActionSet exactly one
                    // device - InputManager.ActiveDevice - and this mod deliberately keeps a
                    // shade-owned pad out of that slot during gameplay, so those actions never saw
                    // the pad at all. Rather than fighting over the active-device slot (which
                    // reliably breaks input for whoever else is on a controller), hand the pad
                    // straight to this one action, on the frames it is actually pressing one of that
                    // action's own buttons.
                    bool incumbentIsPressing = device != null
                        && device != InputDevice.Null
                        && InputDeviceBlocker.IsDeviceDrivingAction(__instance, device);

                    if (!incumbentIsPressing)
                    {
                        var pressingDevice = InputDeviceBlocker.FindDeviceDrivingAllowedAction(__instance);
                        if (pressingDevice != null)
                        {
                            device = pressingDevice;

                            if (ModConfig.Instance.logMenu)
                            {
                                try
                                {
                                    LegacyHelper.LogInfo(FormattableString.Invariant(
                                        $"Handed '{pressingDevice.Name}' to hero action '{__instance.Name}' (it was pressing one of that action's buttons)"));
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    return;
                }

                if (device == null || device == InputDevice.Null)
                    return;

                if (InputDeviceBlocker.IsRestrictedDevice(device) && InputDeviceBlocker.ShouldBlockShadeDeviceInput())
                {
                    device = InputDevice.Null;
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputManager), "UpdateActiveDevice")]
    private class InputManager_UpdateActiveDevice_BlockShadeDevices
    {
        private static readonly MethodInfo ActiveDeviceSetter = AccessTools.PropertySetter(typeof(InputManager), nameof(InputManager.ActiveDevice));

        /// <summary>
        /// Where the postfix puts <c>InputManager.ActiveDevice</c> back to when a shade-owned pad
        /// grabs it during gameplay: simply whatever it was a frame ago.
        /// <para>
        /// An attempt to make this "the last device that was not shade-owned" instead - so the pad
        /// could not stay pinned as the active device once it had been active for two frames running
        /// - has been tried and reverted. It reduces to forcing <c>ActiveDevice</c> to
        /// <c>InputDevice.Null</c> on every gameplay frame whenever the only pad present is
        /// shade-owned, and since InControl feeds a <c>PlayerActionSet</c> exactly one device, that
        /// kills controller input for Hornet as well. Whatever replaces the stickiness has to leave
        /// <c>ActiveDevice</c> alone; see the device substitution in
        /// <see cref="PlayerAction_Update_BlockShadeGameplay"/>, which is how the shade's pad reaches
        /// the menu actions without this having to hand it the active-device slot at all.
        /// </para>
        /// </summary>
        private static InputDevice previousActiveDevice = InputDevice.Null;

        /// <summary>
        /// The most recent active device that was not the Shade player's, so there is something to
        /// hand Hornet back that is actually hers.
        /// <para>
        /// <see cref="previousActiveDevice"/> alone is not enough, and this is the bug it caused.
        /// Blocking is deliberately off during a scene load and while a menu is open, so if the
        /// Shade player is holding anything across one of those, their pad becomes the active device
        /// and stays there. When gameplay resumes, "whatever it was a frame ago" <em>is</em> the
        /// Shade's pad: the restore below finds nothing to change, the pad stays active for as long
        /// as it is held, and Hornet - whose actions are fed exactly one device, and that one nulled
        /// for being restricted - cannot move until the other player lets go. Opening the quick map
        /// made it reproducible on demand.
        /// </para>
        /// </summary>
        private static InputDevice lastHornetDevice = InputDevice.Null;

        /// <summary>The same, for the bug report. See <c>InputDeviceBlocker.DescribeDeviceOwnership</c>.</summary>
        internal static InputDevice LastHornetDevice => lastHornetDevice;

        private static void Prefix()
        {
            try
            {
                InputDeviceBlocker.RefreshShadeDevices();

                var active = InputManager.ActiveDevice ?? InputDevice.Null;
                previousActiveDevice = active;

                // Recorded by ownership rather than by the blocking state, which is off in exactly
                // the moments this needs to survive.
                if (InputDeviceBlocker.IsUsableHornetDevice(active))
                {
                    lastHornetDevice = active;
                }
            }
            catch
            {
                previousActiveDevice = InputDevice.Null;
            }
        }

        private static void Postfix()
        {
            try
            {
                if (!InputDeviceBlocker.ShouldBlockShadeDeviceInput())
                    return;

                var activeDevice = InputManager.ActiveDevice;
                if (!InputDeviceBlocker.IsRestrictedDevice(activeDevice))
                    return;

                // Pause / open-inventory / quick-map are never assignable away from the player, so
                // leave the shade's pad active for the frame it is pressing one of them.
                bool allowed = InputDeviceBlocker.IsDrivingAllowedHeroAction(activeDevice);

                // Diagnostic: only log the *allowed* case. A restricted (shade-owned) device becomes
                // InputManager.ActiveDevice - and this postfix runs - on every frame the player is
                // driving the Shade with that device at all (e.g. normal stick movement), so logging
                // every occurrence floods the console with routine, uninteresting "allowed=False"
                // lines. What actually answers "can the shade's device open the inventory" is whether
                // this ever flips to true while Select/Back is being pressed.
                if (allowed && ModConfig.Instance.logMenu)
                {
                    try
                    {
                        LegacyHelper.LogInfo("[InputDeviceBlocker] Restricted device allowed through this frame (driving an allowed hero action).");
                    }
                    catch
                    {
                    }
                }

                if (allowed)
                    return;

                if (ActiveDeviceSetter == null)
                    return;

                // Hornet's own pad first, then whatever was active a frame ago. Never
                // InputDevice.Null: forcing that on every gameplay frame is what the earlier attempt
                // at this reduced to, and it takes Hornet's controller with it. If there is nothing
                // of hers to restore, this leaves the active device alone exactly as before.
                var restoreDevice = InputDeviceBlocker.IsUsableHornetDevice(lastHornetDevice)
                    ? lastHornetDevice
                    : (previousActiveDevice ?? InputDevice.Null);

                if (restoreDevice == activeDevice || !InputDeviceBlocker.IsUsableHornetDevice(restoreDevice))
                    return;

                ActiveDeviceSetter.Invoke(null, new object[] { restoreDevice });
            }
            catch
            {
            }
            finally
            {
                previousActiveDevice = InputDevice.Null;
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.UpdateActiveController))]
    private class InputHandler_UpdateActiveController_BlockShadeDevice
    {
        private static bool Prefix(InputHandler __instance)
        {
            try
            {
                if (MenuInputBridge.ShouldBypassActiveControllerUpdate(__instance))
                    return false;

                if (!InputDeviceBlocker.ShouldIgnoreDevice(InputManager.ActiveDevice))
                    return true;
                InputDeviceBlocker.EnsureLastActiveController(__instance);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "ControllerActivated")]
    private class InputHandler_ControllerActivated_BlockShadeDevice
    {
        private static bool Prefix(InputDevice inputDevice)
        {
            try
            {
                if (InputDeviceBlocker.ShouldIgnoreDevice(inputDevice))
                    return false;
            }
            catch
            {
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(InputHandler), "ControllerDetached")]
    private class InputHandler_ControllerDetached_ReleaseShadeDevice
    {
        private static void Postfix(InputDevice inputDevice)
        {
            try
            {
                InputDeviceBlocker.ReleaseDevice(inputDevice);
            }
            catch
            {
            }
        }
    }
}
#nullable restore
