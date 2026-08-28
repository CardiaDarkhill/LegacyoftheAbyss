#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using InControl;
using UnityEngine;
using GlobalEnums;

// Letting the Shade's reserved device drive menus. MenuInputBridge adds the Shade's bindings to the
// game's own menu actions and takes them back out again before anything saves them to GameSettings;
// the patches around it are the InputHandler entry points that read or write those bindings.
// The device reservation itself is in LegacyHelper.Patches.InputDevices.cs.
public partial class LegacyHelper
{
    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.OnAwake))]

    private class InputHandler_OnAwake_MenuInputBridge
    {
        private static void Postfix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.Initialize(__instance);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.OnDestroy))]
    private class InputHandler_OnDestroy_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.OnDestroyed(__instance);
            }
            catch
            {
            }
        }
    }

    private static class MenuInputBridge
    {
        private const int KeyboardUpBindingId = 1;
        private const int KeyboardDownBindingId = 2;
        private const int KeyboardLeftBindingId = 3;
        private const int KeyboardRightBindingId = 4;
        private const int ControllerUpBindingId = 5;
        private const int ControllerDownBindingId = 6;
        private const int ControllerLeftBindingId = 7;
        private const int ControllerRightBindingId = 8;
        private const int KeyboardCancelBindingId = 9;
        private const int ControllerInventoryBindingId = 10;
        private const int KeyboardConfirmBindingId = 11;

        private static InputHandler handler;
        private static bool subscribed;
        private static BindingSourceType? pendingSimulatedBindingSource;
        private static int pendingSimulatedBindingFrame = -1;

        internal static void Initialize(InputHandler instance)
        {
            handler = instance;
            EnsureBindings(instance?.inputActions);
            if (!subscribed)
            {
                InputHandler.OnUpdateHeroActions += HandleActionsUpdated;
                subscribed = true;
            }
        }

        internal static void OnDestroyed(InputHandler instance)
        {
            if (ReferenceEquals(handler, instance))
            {
                handler = null;
            }

            if (subscribed)
            {
                InputHandler.OnUpdateHeroActions -= HandleActionsUpdated;
                subscribed = false;
            }
        }

        private static void HandleActionsUpdated(HeroActions actions)
        {
            try
            {
                EnsureBindings(actions);
            }
            catch
            {
            }
        }

        private static void EnsureBindings(HeroActions actions)
        {
            if (actions == null)
            {
                return;
            }

            RemoveSavedPlaceholders(actions);

            AddBinding(actions.Up, new ShadeKeyboardMovementBinding(KeyboardUpBindingId, ShadeAction.MoveUp));
            AddBinding(actions.Down, new ShadeKeyboardMovementBinding(KeyboardDownBindingId, ShadeAction.MoveDown));
            AddBinding(actions.Left, new ShadeKeyboardMovementBinding(KeyboardLeftBindingId, ShadeAction.MoveLeft));
            AddBinding(actions.Right, new ShadeKeyboardMovementBinding(KeyboardRightBindingId, ShadeAction.MoveRight));

            AddBinding(actions.Up, new ShadeControllerMovementBinding(ControllerUpBindingId, ShadeAction.MoveUp));
            AddBinding(actions.Down, new ShadeControllerMovementBinding(ControllerDownBindingId, ShadeAction.MoveDown));
            AddBinding(actions.Left, new ShadeControllerMovementBinding(ControllerLeftBindingId, ShadeAction.MoveLeft));
            AddBinding(actions.Right, new ShadeControllerMovementBinding(ControllerRightBindingId, ShadeAction.MoveRight));

            AddBinding(actions.MenuCancel, new ShadeKeyboardBackBinding(KeyboardCancelBindingId));
            AddBinding(actions.OpenInventory, new ShadeControllerInventoryBinding(ControllerInventoryBindingId));
            AddBinding(actions.MenuSubmit, new ShadeKeyboardConfirmBinding(KeyboardConfirmBindingId));

            // Belongs here, not only in HornetInput.ApplyKeyboardDefaults/ApplyControllerDefaults:
            // those two only ever run when a player explicitly clicks a preset button in the mod's
            // settings menu, never automatically. A fresh game launch loads Hornet's own bindings
            // (keyboard vs controller) correctly from persisted settings without either of those
            // methods ever running - but the *Shade's* extra binding on the inventory-open actions
            // lived only inside them, so on a fresh boot the Shade's device had no way to open the
            // inventory at all until the player re-clicked a preset this session. EnsureBindings runs
            // both at InputHandler.OnAwake and on every OnUpdateHeroActions event after, which is what
            // makes it actually automatic.
            HornetInput.EnsureShadeInventoryBindings(actions);
            HornetInput.LogDeviceSnapshotOnce(actions);
        }

        private static void AddBinding(PlayerAction action, ShadeMenuBindingSourceBase binding)
        {
            if (action == null)
            {
                return;
            }

            foreach (var existing in action.Bindings)
            {
                if (existing is ShadeMenuBindingSourceBase other && other.Equals(binding))
                {
                    return;
                }
            }

            action.AddBinding(binding);
        }

        private static void PrunePlaceholderBindings(PlayerAction action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                var unfiltered = action.UnfilteredBindings;
                if (unfiltered == null || unfiltered.Count == 0)
                    return;

                for (int i = unfiltered.Count - 1; i >= 0; i--)
                {
                    var binding = unfiltered[i];
                    if (binding == null)
                        continue;

                    if (IsPlaceholderBinding(binding))
                    {
                        action.RemoveBinding(binding);
                    }
                }
            }
            catch
            {
            }
        }

        internal static void RemoveSavedPlaceholders(HeroActions actions)
        {
            if (actions == null)
            {
                return;
            }

            PrunePlaceholderBindings(actions.Up);
            PrunePlaceholderBindings(actions.Down);
            PrunePlaceholderBindings(actions.Left);
            PrunePlaceholderBindings(actions.Right);
            PrunePlaceholderBindings(actions.MenuCancel);
            PrunePlaceholderBindings(actions.OpenInventory);
            PrunePlaceholderBindings(actions.MenuSubmit);
        }

        internal static bool IsShadePlaceholderBinding(BindingSource binding)
        {
            if (binding is ShadeMenuBindingSourceBase)
            {
                return true;
            }

            return IsPlaceholderBinding(binding);
        }

        private static bool IsPlaceholderBinding(BindingSource binding)
        {
            if (binding == null)
                return false;

            try
            {
                if (binding is DeviceBindingSource deviceBinding)
                {
                    return deviceBinding.Control == InputControlType.None;
                }

                if (binding is KeyBindingSource keyBinding)
                {
                    var combo = keyBinding.Control;
                    return combo.IncludeCount == 0 && combo.ExcludeCount == 0;
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsMenuActive()
        {
            try
            {
                return MenuStateUtility.IsMenuActive();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// True while the inventory surfaces (inventory, tools, quests, journal, map, and the Shade's
        /// charm tab) are up, as distinct from the pause/options menus that
        /// <see cref="IsMenuActive"/> also covers. The number keys mean "switch tab" there and
        /// nothing anywhere else, which is the distinction <see cref="ShadeKeyboardBackBinding"/>
        /// needs.
        /// </summary>
        private static bool IsInventoryOpen()
        {
            try
            {
                var playerData = MenuStateUtility.TryGetPlayerData();
                return !ReferenceEquals(playerData, null) && playerData.isInventoryOpen;
            }
            catch
            {
                return false;
            }
        }

        private static InputHandler TryGetHandler()
        {
            if (handler != null)
                return handler;
            try
            {
                return InputHandler.UnsafeInstance;
            }
            catch
            {
                return null;
            }
        }

        private static bool HornetControllerBindingsEnabled() => HornetInput.EffectiveControllerEnabled();

        private static bool HornetKeyboardBindingsEnabled() => HornetInput.EffectiveKeyboardEnabled();

        private static void RegisterSimulatedBinding(BindingSourceType sourceType)
        {
            pendingSimulatedBindingSource = sourceType;
            pendingSimulatedBindingFrame = Time.frameCount;
        }

        private static bool ShadeInventoryKeyPressed()
        {
            var current = TryGetHandler();
            if (current == null)
                return false;

            var actions = current.inputActions;
            if (actions == null)
                return false;

            return IsActionKeyboardBindingPressed(actions.OpenInventory)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryMap)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryJournal)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryTools)
                || IsActionKeyboardBindingPressed(actions.OpenInventoryQuests);
        }

        private static bool IsActionKeyboardBindingPressed(PlayerAction action)
        {
            if (action == null)
                return false;

            foreach (var binding in action.Bindings)
            {
                if (binding == null || binding is ShadeMenuBindingSourceBase)
                    continue;

                if (binding is KeyBindingSource keyBinding)
                {
                    if (IsKeyComboPressed(keyBinding.Control))
                        return true;
                    continue;
                }

                if (binding is MouseBindingSource mouseBinding)
                {
                    if (IsMousePressed(mouseBinding.Control))
                        return true;
                }
            }

            return false;
        }

        private static bool IsKeyComboPressed(KeyCombo combo)
        {
            if (combo.IncludeCount == 0)
                return false;

            for (int i = 0; i < combo.IncludeCount; i++)
            {
                if (!IsKeyPressed(combo.GetInclude(i)))
                    return false;
            }

            return true;
        }

        private static bool IsKeyPressed(Key key)
        {
            if (key == Key.None)
                return false;

            try
            {
                var mappings = UnityKeyboardProvider.KeyMappings;
                int index = (int)key;
                if (index >= 0 && index < mappings.Length)
                {
                    return mappings[index].IsPressed;
                }
            }
            catch
            {
            }

            return false;
        }

        private static bool IsMousePressed(Mouse mouse)
        {
            if (mouse == Mouse.None)
                return false;

            try
            {
                return mouse switch
                {
                    Mouse.LeftButton => Input.GetMouseButton(0),
                    Mouse.RightButton => Input.GetMouseButton(1),
                    Mouse.MiddleButton => Input.GetMouseButton(2),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        private static float GetShadeControllerDirectionalValue(ShadeAction action)
        {
            float value = ShadeInput.GetActionValue(action, ShadeBindingOptionType.Controller);
            float dpad = GetShadeControllerDPadValue(action);
            return Mathf.Max(value, dpad);
        }

        private static float GetShadeControllerDPadValue(ShadeAction action)
        {
            var device = TryGetShadeController();
            if (device == null)
                return 0f;

            try
            {
                return action switch
                {
                    ShadeAction.MoveUp => GetDpadDirectionValue(device, InputControlType.DPadUp, InputControlType.DPadY, true),
                    ShadeAction.MoveDown => GetDpadDirectionValue(device, InputControlType.DPadDown, InputControlType.DPadY, false),
                    ShadeAction.MoveLeft => GetDpadDirectionValue(device, InputControlType.DPadLeft, InputControlType.DPadX, false),
                    ShadeAction.MoveRight => GetDpadDirectionValue(device, InputControlType.DPadRight, InputControlType.DPadX, true),
                    _ => 0f
                };
            }
            catch
            {
                return 0f;
            }
        }

        private static float GetDpadDirectionValue(InputDevice device, InputControlType buttonControl, InputControlType axisControl, bool positiveAxis)
        {
            if (device == null || device == InputDevice.Null)
                return 0f;

            if (IsControlPressed(device, buttonControl))
                return 1f;

            float axisContribution = 0f;
            try
            {
                var axis = device.GetControl(axisControl);
                if (axis != null && axis != InputControl.Null)
                {
                    float value = Mathf.Clamp(axis.Value, -1f, 1f);
                    axisContribution = positiveAxis ? Mathf.Max(0f, value) : Mathf.Max(0f, -value);
                }
            }
            catch
            {
            }

            if (axisContribution > 0f)
                return Mathf.Clamp01(axisContribution);

            try
            {
                var dpad = device.DPad;
                if (dpad != null && dpad != TwoAxisInputControl.Null)
                {
                    float composite = axisControl switch
                    {
                        InputControlType.DPadX => positiveAxis ? Mathf.Max(0f, dpad.X) : Mathf.Max(0f, -dpad.X),
                        InputControlType.DPadY => positiveAxis ? Mathf.Max(0f, dpad.Y) : Mathf.Max(0f, -dpad.Y),
                        _ => 0f
                    };
                    if (composite > 0f)
                        return Mathf.Clamp01(composite);
                }
            }
            catch
            {
            }

            return 0f;
        }

        private static float GetShadeControllerBackValue()
        {
            var device = TryGetShadeController();
            if (device == null)
                return 0f;

            try
            {
                if (IsControlPressed(device, InputControlType.Action6))
                    return 1f;
                if (IsControlPressed(device, InputControlType.Back))
                    return 1f;
                if (IsControlPressed(device, InputControlType.Select))
                    return 1f;
            }
            catch
            {
            }

            return 0f;
        }

        private static bool IsControlPressed(InputDevice device, InputControlType controlType)
        {
            try
            {
                var control = device.GetControl(controlType);
                return control != null && control != InputControl.Null && control.IsPressed;
            }
            catch
            {
                return false;
            }
        }

        private static InputDevice TryGetShadeController()
        {
            try
            {
                var cfg = ShadeInput.Config;
                if (cfg == null)
                    return null;
                int index = Mathf.Max(-1, cfg.controllerDeviceIndex);
                if (index < 0)
                    return null;
                var devices = InputManager.Devices;
                if (devices == null || devices.Count == 0)
                    return null;
                if (index >= devices.Count)
                    index = devices.Count - 1;
                return devices[index];
            }
            catch
            {
                return null;
            }
        }

        internal static bool ShouldBypassActiveControllerUpdate(InputHandler instance)
        {
            if (!pendingSimulatedBindingSource.HasValue)
                return false;

            if (Time.frameCount != pendingSimulatedBindingFrame)
            {
                pendingSimulatedBindingSource = null;
                return false;
            }

            try
            {
                var actions = instance?.inputActions;
                if (actions == null)
                {
                    pendingSimulatedBindingSource = null;
                    return false;
                }

                if (actions.LastInputType == pendingSimulatedBindingSource.Value)
                {
                    pendingSimulatedBindingSource = null;
                    return true;
                }
            }
            catch
            {
                pendingSimulatedBindingSource = null;
            }

            return false;
        }

        private abstract class ShadeMenuBindingSourceBase : BindingSource
        {
            private readonly int id;

            protected ShadeMenuBindingSourceBase(int id)
            {
                this.id = id;
            }

            protected abstract float ComputeValue();
            protected virtual bool ShouldActivate() => true;
            protected abstract BindingSourceType SourceType { get; }
            protected abstract InputDeviceClass SourceClass { get; }
            protected abstract string SourceName { get; }
            protected abstract string SourceDeviceName { get; }

            public override float GetValue(InputDevice inputDevice)
            {
                if (!ShouldActivate())
                    return 0f;
                float value = Mathf.Clamp01(ComputeValue());
                if (value > 0f)
                    RegisterSimulatedBinding(SourceType);
                return value;
            }

            public override bool GetState(InputDevice inputDevice) => GetValue(inputDevice) >= 0.5f;

            public override BindingSourceType BindingSourceType => SourceType;

            public override string Name => SourceName;

            public override string DeviceName => SourceDeviceName;

            public override InputDeviceClass DeviceClass => SourceClass;

            public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

            public override bool IsValid => true;

            public override bool Equals(BindingSource other)
            {
                return other is ShadeMenuBindingSourceBase binding && binding.id == id && binding.GetType() == GetType();
            }

            public override bool Equals(object obj)
            {
                return obj is ShadeMenuBindingSourceBase binding && binding.id == id && binding.GetType() == GetType();
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (GetType().GetHashCode() * 397) ^ id;
                }
            }

            public override void Save(BinaryWriter writer)
            {
                try
                {
                    WritePlaceholderData(writer);
                }
                catch
                {
                }
            }

            public override void Load(BinaryReader reader, ushort dataFormatVersion)
            {
                try
                {
                    ReadPlaceholderData(reader);
                }
                catch
                {
                }
            }

            private void WritePlaceholderData(BinaryWriter writer)
            {
                if (writer == null)
                    return;

                switch (SourceType)
                {
                    case BindingSourceType.DeviceBindingSource:
                    case BindingSourceType.UnknownDeviceBindingSource:
                        writer.Write((int)InputControlType.None);
                        break;
                    case BindingSourceType.KeyBindingSource:
                        writer.Write(0);
                        writer.Write(0UL);
                        writer.Write(0);
                        writer.Write(0UL);
                        break;
                    case BindingSourceType.MouseBindingSource:
                        writer.Write((int)Mouse.None);
                        break;
                    default:
                        writer.Write(0);
                        break;
                }
            }

            private void ReadPlaceholderData(BinaryReader reader)
            {
                if (reader == null)
                    return;

                switch (SourceType)
                {
                    case BindingSourceType.DeviceBindingSource:
                    case BindingSourceType.UnknownDeviceBindingSource:
                        _ = reader.ReadInt32();
                        break;
                    case BindingSourceType.KeyBindingSource:
                        _ = reader.ReadInt32();
                        _ = reader.ReadUInt64();
                        _ = reader.ReadInt32();
                        _ = reader.ReadUInt64();
                        break;
                    case BindingSourceType.MouseBindingSource:
                        _ = reader.ReadInt32();
                        break;
                    default:
                        break;
                }
            }
        }

        private sealed class ShadeKeyboardMovementBinding : ShadeMenuBindingSourceBase
        {
            private readonly ShadeAction action;

            public ShadeKeyboardMovementBinding(int id, ShadeAction action) : base(id)
            {
                this.action = action;
            }

            protected override float ComputeValue()
            {
                return ShadeInput.GetActionValue(action, ShadeBindingOptionType.Key);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => $"Shade {action} Keyboard";
            protected override string SourceDeviceName => "Shade Keyboard";
        }

        private sealed class ShadeControllerMovementBinding : ShadeMenuBindingSourceBase
        {
            private readonly ShadeAction action;

            public ShadeControllerMovementBinding(int id, ShadeAction action) : base(id)
            {
                this.action = action;
            }

            protected override float ComputeValue()
            {
                return GetShadeControllerDirectionalValue(action);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetKeyboardBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.KeyBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Keyboard;
            protected override string SourceName => $"Shade {action} Controller";
            protected override string SourceDeviceName => "Shade Controller";
        }

        /// <summary>
        /// Lets the Shade's keyboard back out of menus, by making its inventory keys drive
        /// <c>MenuCancel</c>. The Shade's player has no other cancel key of their own.
        /// <para>
        /// Explicitly inert while the inventory itself is open, and that exclusion is the whole of
        /// bug 4b. <c>ComputeValue</c> fires on <i>any</i> of the five inventory keys, and
        /// <c>InventoryPaneInput.Update</c> tests <c>MenuActions.Cancel</c> in its very first switch -
        /// before it ever reaches the shortcut handling - so with this binding live every number key
        /// closed the whole inventory instead of switching tabs, on every tab. It looked like it was
        /// caused by having visited the Shade tab, and it looked like it was about <c>paneControl</c>;
        /// it was neither. It tracked <c>hornetControllerEnabled</c>, which is what
        /// <see cref="HornetControllerBindingsEnabled"/> below reads: true exactly when the Shade owns
        /// the keyboard, which is the configuration the bug showed up in.
        /// </para>
        /// <para>
        /// Inside the inventory the number keys already have a job - native pane switching, and
        /// closing when you press the current pane's own key - and the Shade holds real bindings on
        /// all five of them, so nothing is lost by standing down here. Every other menu surface
        /// (pause, options) leaves those keys unused, so cancel remains the right meaning there.
        /// </para>
        /// </summary>
        private sealed class ShadeKeyboardBackBinding : ShadeMenuBindingSourceBase
        {
            public ShadeKeyboardBackBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return ShadeInventoryKeyPressed() ? 1f : 0f;
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled() && !IsInventoryOpen();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => "Shade Inventory Shortcut";
            protected override string SourceDeviceName => "Shade Keyboard";
        }

        private sealed class ShadeControllerInventoryBinding : ShadeMenuBindingSourceBase
        {
            public ShadeControllerInventoryBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return GetShadeControllerBackValue();
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetKeyboardBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Keyboard;
            protected override string SourceName => "Shade Controller Back";
            protected override string SourceDeviceName => "Shade Controller";
        }

        private sealed class ShadeKeyboardConfirmBinding : ShadeMenuBindingSourceBase
        {
            public ShadeKeyboardConfirmBinding(int id) : base(id)
            {
            }

            protected override float ComputeValue()
            {
                return ShadeInput.GetActionValue(ShadeAction.Fire, ShadeBindingOptionType.Key);
            }

            protected override bool ShouldActivate()
            {
                return IsMenuActive() && HornetControllerBindingsEnabled();
            }

            protected override BindingSourceType SourceType => BindingSourceType.DeviceBindingSource;
            protected override InputDeviceClass SourceClass => InputDeviceClass.Controller;
            protected override string SourceName => "Shade Confirm Keyboard";
            protected override string SourceDeviceName => "Shade Keyboard";
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.GetButtonBindingForAction))]
    private class InputHandler_GetButtonBindingForAction_MenuInputBridge
    {
        private static void Postfix(PlayerAction action, ref InputControlType __result)
        {
            if (__result != InputControlType.None)
                return;

            try
            {
                if (action == null)
                    return;

                foreach (var binding in action.Bindings)
                {
                    if (binding is DeviceBindingSource deviceBinding)
                    {
                        if (MenuInputBridge.IsShadePlaceholderBinding(binding))
                            continue;

                        if (deviceBinding.Control != InputControlType.None)
                        {
                            __result = deviceBinding.Control;
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), "GetKeyBindingForActionBinding")]
    private class InputHandler_GetKeyBindingForActionBinding_MenuInputBridge
    {
        private static bool Prefix(PlayerAction action, BindingSource bindingSource, ref InputHandler.KeyOrMouseBinding __result)
        {
            try
            {
                if (MenuInputBridge.IsShadePlaceholderBinding(bindingSource))
                {
                    __result = new InputHandler.KeyOrMouseBinding(InControl.Key.None);
                    return false;
                }
            }
            catch
            {
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.GetKeyBindingForAction))]
    private class InputHandler_GetKeyBindingForAction_MenuInputBridge
    {
        private static void Postfix(InputHandler __instance, PlayerAction action, ref InputHandler.KeyOrMouseBinding __result)
        {
            try
            {
                if (!InputHandler.KeyOrMouseBinding.IsNone(__result))
                    return;

                if (__instance == null || action == null)
                    return;

                var actions = __instance.inputActions;
                if (actions == null || !actions.Actions.Contains(action))
                    return;

                foreach (var binding in action.Bindings)
                {
                    if (MenuInputBridge.IsShadePlaceholderBinding(binding))
                        continue;

                    if (binding is KeyBindingSource keyBinding)
                    {
                        var combo = keyBinding.Control;
                        if (combo.IncludeCount == 0)
                            continue;

                        if (combo.IncludeCount == 1)
                        {
                            __result = new InputHandler.KeyOrMouseBinding(combo.GetInclude(0));
                            return;
                        }

                        continue;
                    }

                    if (binding is MouseBindingSource mouseBinding)
                    {
                        __result = new InputHandler.KeyOrMouseBinding(mouseBinding.Control);
                        return;
                    }
                }
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SendKeyBindingsToGameSettings))]
    private class InputHandler_SendKeyBindingsToGameSettings_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.RemoveSavedPlaceholders(__instance?.inputActions);
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(typeof(InputHandler), nameof(InputHandler.SendButtonBindingsToGameSettings))]
    private class InputHandler_SendButtonBindingsToGameSettings_MenuInputBridge
    {
        private static void Prefix(InputHandler __instance)
        {
            try
            {
                MenuInputBridge.RemoveSavedPlaceholders(__instance?.inputActions);
            }
            catch
            {
            }
        }
    }
}
#nullable restore
