using System;
using System.Collections.Generic;
using System.Reflection;
using InControl;
using UnityEngine;

public static class HornetInput
{
    /// <summary>
    /// Logs every binding currently on <paramref name="action"/> at Info level (not gated behind
    /// logMenu -- this is a one-shot diagnostic call, not a per-frame trace). Added to track down a
    /// report that the Shade-side inventory-open bindings this file adds went missing again after
    /// re-applying a preset from the settings menu; the mechanism for that was never confirmed, so
    /// this replaces guessing with a direct dump of what's actually bound after each preset call.
    /// <para>
    /// Deliberately reads <see cref="PlayerAction.UnfilteredBindings"/>, not the public
    /// <c>Bindings</c> property. <c>Bindings</c> only surfaces bindings whose <c>IsValid</c> came
    /// back true *at the moment they were added* - for a <see cref="DeviceBindingSource"/> that means
    /// <c>action.Device.HasControl(...)</c>, checked against whatever device happened to be the
    /// action's current one right then, which need not be the controller the binding is actually
    /// meant for. A binding that fails that check is still added to the list
    /// <c>PlayerAction.UpdateBindings</c> actually evaluates every frame - it just never shows up in
    /// <c>Bindings</c>. The first version of this diagnostic used <c>Bindings</c> and could easily
    /// have reported "not there" for a binding that was working fine all along.
    /// </para>
    /// </summary>
    private static void LogBindings(string label, PlayerAction? action)
    {
        if (action == null)
        {
            return;
        }

        try
        {
            var parts = new System.Collections.Generic.List<string>();
            foreach (var binding in action.UnfilteredBindings)
            {
                if (binding == null)
                {
                    continue;
                }

                parts.Add(binding switch
                {
                    DeviceBindingSource device => $"Device:{device.Control}",
                    KeyBindingSource => "Key",
                    MouseBindingSource => "Mouse",
                    _ => binding.GetType().Name
                });
            }

            LegacyHelper.LogInfo(parts.Count == 0
                ? $"[HornetInput] {label}: <no bindings>"
                : $"[HornetInput] {label}: {string.Join(", ", parts)}");
        }
        catch
        {
        }
    }

    private static bool loggedDeviceSnapshot;

    /// <summary>
    /// One-shot dump, at Info, of what the attached pads actually expose and what the two actions
    /// that matter for menus are bound to.
    /// <para>
    /// Every round of "the pad can't pause / can't open the inventory" has turned on a fact about one
    /// of those two things - a binding wiped by <c>ClearBindings</c>, or a control the pad does not
    /// have (an XInput pad exposes <c>View</c> and <c>Menu</c>, and none of <c>Back</c>,
    /// <c>Select</c>, <c>Start</c> or <c>Options</c>). Printing both once removes the guesswork from
    /// the next one.
    /// </para>
    /// </summary>
    internal static void LogDeviceSnapshotOnce(HeroActions? actions)
    {
        if (loggedDeviceSnapshot || actions == null)
        {
            return;
        }

        try
        {
            var devices = InputManager.Devices;
            if (devices == null || devices.Count == 0)
            {
                // No pad yet; try again on a later EnsureBindings rather than logging "none".
                return;
            }

            loggedDeviceSnapshot = true;

            InputControlType[] interesting =
            {
                InputControlType.View, InputControlType.Menu, InputControlType.Back,
                InputControlType.Select, InputControlType.Start, InputControlType.Options,
                InputControlType.TouchPadButton, InputControlType.Minus
            };

            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                if (device == null || device == InputDevice.Null)
                {
                    continue;
                }

                var present = new System.Collections.Generic.List<string>();
                foreach (var control in interesting)
                {
                    if (device.HasControl(control))
                    {
                        present.Add(control.ToString());
                    }
                }

                LegacyHelper.LogInfo($"[HornetInput] Device[{i}] '{device.Name}' menu controls: " +
                    (present.Count == 0 ? "<none>" : string.Join(", ", present)));
            }

            LogBindings("Pause", actions.Pause);
            LogBindings("OpenInventory", actions.OpenInventory);
        }
        catch
        {
        }
    }

    /// <summary>
    /// The <see cref="InputHandler"/>, cheapest source first: the registered singleton, then the one
    /// the <c>GameManager</c> holds, then a scene scan. Shared with <c>ShadeInventoryPane</c>, which
    /// must not grow its own copy of the cascade.
    /// <para>
    /// Each step is guarded separately because every one of them touches a static on a game type,
    /// and those run type initializers that call into the engine - so they throw rather than return
    /// null outside a player loop. Falling through to "no handler" is the intended outcome there.
    /// </para>
    /// </summary>
    internal static InputHandler? FindHandler()
    {
        try
        {
            var singleton = ManagerSingleton<InputHandler>.UnsafeInstance;
            if (singleton != null)
                return singleton;
        }
        catch
        {
        }

        try
        {
            var gm = GameManager.instance;
            if (gm != null && gm.inputHandler != null)
                return gm.inputHandler;
        }
        catch
        {
        }

        try
        {
            return UnityEngine.Object.FindFirstObjectByType<InputHandler>();
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// Whether the Shade AI is currently standing in for the second player, in which case the
    /// two-player device split has nobody to serve.
    /// </summary>
    internal static bool ShadeAiHoldsTheShade()
    {
        try
        {
            var cfg = ModConfig.Instance;
            if (cfg == null || !cfg.shadeAiVanillaControls)
            {
                return false;
            }

            var shade = LegacyHelper.ShadeController.PrimaryInstance;
            return shade != null ? shade.ShadeAiEnabled : cfg.shadeAiEnabled;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Whether Hornet should answer to the keyboard right now.
    /// <para>
    /// The stored preference still says what the player chose for two-player; this is that answer
    /// widened while an AI is driving the Shade. Kept as a separate question rather than by writing
    /// <c>true</c> into the config, because the split has to come back exactly as it was the moment
    /// the AI is switched off.
    /// </para>
    /// </summary>
    internal static bool EffectiveKeyboardEnabled()
    {
        try
        {
            var cfg = ModConfig.Instance;
            // Open when the config cannot be read, exactly as the controller below is, and for a
            // sharper reason than symmetry. This answer gates the game's own keyboard mapping - a
            // false here does not merely decline to add bindings, it leaves Hornet with none at
            // all, because the game clears every binding immediately before remapping them (see
            // InputHandler.LoadSavedInputBindings). The worst an unreadable config can cost by
            // erring this way is that both players answer to the keyboard for a moment; erring the
            // other way costs Hornet her controls until the game is restarted.
            return cfg == null || cfg.hornetKeyboardEnabled || ShadeAiHoldsTheShade();
        }
        catch
        {
            return true;
        }
    }

    internal static bool EffectiveControllerEnabled()
    {
        try
        {
            var cfg = ModConfig.Instance;
            return cfg == null || cfg.hornetControllerEnabled || ShadeAiHoldsTheShade();
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// The gameplay actions the two mapping passes fill in - the same set
    /// <c>InputHandler.ResetDefaultKeyBindings</c> clears, minus the menu actions, which must keep
    /// their bindings or the pause menu stops answering.
    /// </summary>
    private static IEnumerable<PlayerAction> GameplayActions(HeroActions actions)
    {
        yield return actions.Jump;
        yield return actions.Attack;
        yield return actions.Dash;
        yield return actions.Cast;
        yield return actions.SuperDash;
        yield return actions.DreamNail;
        yield return actions.QuickMap;
        yield return actions.QuickCast;
        yield return actions.Taunt;
        yield return actions.Evade;
        yield return actions.Up;
        yield return actions.Down;
        yield return actions.Left;
        yield return actions.Right;
        yield return actions.OpenInventory;
        yield return actions.OpenInventoryMap;
        yield return actions.OpenInventoryJournal;
        yield return actions.OpenInventoryTools;
        yield return actions.OpenInventoryQuests;
    }

    private static MethodInfo? mapKeyboardFromSettings;
    private static bool mapKeyboardLookupDone;

    /// <summary>
    /// <c>InputHandler.MapKeyboardLayoutFromGameSettings</c>, which rebuilds the keyboard half from
    /// the player's own saved keys and only ever adds.
    /// <para>
    /// Reflected because it is private, and asserted in <c>Tests/GameApiContract.cs</c>. The public
    /// alternative, <c>ResetDefaultKeyBindings</c>, is not usable here: it overwrites every saved key
    /// with the hardcoded Z/X/C defaults and calls <c>SaveKeyboardSettings</c>, so using it to
    /// re-apply a device split would silently destroy the player's own layout.
    /// </para>
    /// </summary>
    private static MethodInfo? ResolveMapKeyboard()
    {
        if (mapKeyboardLookupDone)
        {
            return mapKeyboardFromSettings;
        }

        mapKeyboardLookupDone = true;
        try
        {
            mapKeyboardFromSettings = typeof(InputHandler).GetMethod(
                "MapKeyboardLayoutFromGameSettings",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
        catch
        {
            mapKeyboardFromSettings = null;
        }

        if (mapKeyboardFromSettings == null)
        {
            LegacyHelper.LogInfo(
                "[HornetInput] InputHandler.MapKeyboardLayoutFromGameSettings not found - Hornet's keyboard cannot be restored when the Shade AI takes over.");
        }

        return mapKeyboardFromSettings;
    }

    private static void RemoveBindings(PlayerAction action, bool keyboard)
    {
        if (action == null)
        {
            return;
        }

        try
        {
            var doomed = new List<BindingSource>();
            foreach (var binding in action.UnfilteredBindings)
            {
                if (binding == null)
                {
                    continue;
                }

                bool isKeyboard = binding is KeyBindingSource || binding is MouseBindingSource;
                if (isKeyboard == keyboard)
                {
                    doomed.Add(binding);
                }
            }

            foreach (var binding in doomed)
            {
                action.RemoveBinding(binding);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// Rebuilds Hornet's device split from <see cref="EffectiveKeyboardEnabled"/> and
    /// <see cref="EffectiveControllerEnabled"/>. Called when the Shade AI is switched on or off, so
    /// the change lands immediately rather than at the next scene load.
    /// <para>
    /// Removes only the bindings for a device that should not be answering and re-adds the ones that
    /// should, rather than clearing everything - the saved layout is the player's and this has no
    /// business rewriting it.
    /// </para>
    /// </summary>
    internal static void RefreshHornetDeviceBindings()
    {
        var handler = FindHandler();
        if (handler == null)
        {
            return;
        }

        var actions = handler.inputActions;
        if (actions == null)
        {
            return;
        }

        bool keyboard = EffectiveKeyboardEnabled();
        bool controller = EffectiveControllerEnabled();

        try
        {
            foreach (var action in GameplayActions(actions))
            {
                if (!keyboard)
                {
                    RemoveBindings(action, keyboard: true);
                }

                if (!controller)
                {
                    RemoveBindings(action, keyboard: false);
                }
            }
        }
        catch
        {
        }

        if (keyboard)
        {
            try { ResolveMapKeyboard()?.Invoke(handler, null); }
            catch { }
        }

        if (controller)
        {
            try { handler.MapControllerButtons(handler.activeGamepadType); }
            catch { }
        }

        try { EnsureShadeInventoryBindings(actions); }
        catch { }
    }

    /// <summary>How many times her keyboard has had to be put back this session, for the report.</summary>
    internal static int KeyboardRepairs { get; private set; }

    /// <summary>
    /// How long to wait before trying again after a repair that did not take.
    /// <para>
    /// The first attempt of an outage is immediate - that is the whole point of the check - but a
    /// remap can legitimately produce nothing (an empty saved layout parses to no bindings at all),
    /// and without this the check would then call the game's mapper on every frame for the rest of
    /// the session.
    /// </para>
    /// </summary>
    private const float KeyboardRepairRetrySeconds = 1f;

    private static float nextKeyboardRepairAttempt;
    private static bool loggedKeyboardRepair;

    /// <summary>
    /// Gives Hornet her keyboard back if she has been left without one.
    /// <para>
    /// The game rebuilds her bindings by clearing every one of them and then remapping from the
    /// saved layout (<c>InputHandler.LoadSavedInputBindings</c>, called whenever the save store
    /// mounts). This mod patches the remapping half, so anything that answers "no keyboard" at that
    /// instant - a config that could not be read, a flag read mid-load - does not skip a remap, it
    /// strips her. Twice now that has been reported as Hornet losing her controls on a room change,
    /// with her movement and map actions holding no key bindings at all afterwards and nothing to
    /// put them back before the game was restarted.
    /// </para>
    /// <para>
    /// So the invariant is checked rather than reasoned about: if she is meant to answer to the
    /// keyboard and none of her movement actions hold a key, the layout is mapped again. Checked
    /// every frame, because it costs two indexed walks of a binding list and a second of waiting is
    /// a second of Hornet standing still. Ours are <c>BindingSource</c> subclasses and the game's
    /// are <c>KeyBindingSource</c>, so the Shade's own additions cannot be mistaken for hers.
    /// </para>
    /// </summary>
    internal static void EnsureHornetKeyboardBindings()
    {
        try
        {
            if (!EffectiveKeyboardEnabled())
            {
                return;
            }

            var handler = FindHandler();
            var actions = handler != null ? handler.inputActions : null;
            if (actions == null)
            {
                return;
            }

            if (HasKeyBinding(actions.Left) || HasKeyBinding(actions.Right))
            {
                loggedKeyboardRepair = false;
                nextKeyboardRepairAttempt = 0f;
                return;
            }

            // On the frame it is noticed. An earlier version waited a second first, on the theory
            // that the game clears these bindings before remapping them and a repair might land in
            // the middle of that - but the clear and the remap are two statements of one synchronous
            // method, so no Update can ever observe the gap between them. All the wait achieved was
            // a second of Hornet standing still, which is the "her controls are locked after a room
            // transition" report. The retry below is the only wait left, and it only applies to an
            // attempt that has already failed once.
            float now = Time.unscaledTime;
            if (now < nextKeyboardRepairAttempt)
            {
                return;
            }

            nextKeyboardRepairAttempt = now + KeyboardRepairRetrySeconds;
            KeyboardRepairs++;

            if (!loggedKeyboardRepair)
            {
                loggedKeyboardRepair = true;
                Debug.LogWarning(
                    "[LegacyoftheAbyss] Hornet had no keyboard bindings while the keyboard is hers. "
                    + "Remapping her saved layout. This is the room-change control loss; if it "
                    + "recurs, the bug report's InputDevices line records who owned what at the time.");
            }

            try { ResolveMapKeyboard()?.Invoke(handler, null); }
            catch { }

            try { EnsureShadeInventoryBindings(actions); }
            catch { }
        }
        catch
        {
        }
    }

    private static bool HasKeyBinding(PlayerAction action)
    {
        if (action == null)
        {
            return false;
        }

        try
        {
            // Indexed rather than foreach: this runs every frame, and enumerating the collection
            // allocates where reading it by index does not.
            var bindings = action.Bindings;
            if (bindings == null)
            {
                return false;
            }

            for (int i = 0; i < bindings.Count; i++)
            {
                if (bindings[i] is KeyBindingSource)
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }

    public static void ApplyKeyboardDefaults(bool disableController)
    {
        var cfg = ModConfig.Instance;
        cfg.hornetKeyboardEnabled = true;
        cfg.hornetControllerEnabled = !disableController;

        var handler = FindHandler();
        if (handler == null)
            return;

        try
        {
            handler.ResetDefaultKeyBindings();
            ApplyLeftSideLayout(handler);
        }
        catch
        {
        }
    }

    public static void ApplyControllerDefaults()
    {
        var cfg = ModConfig.Instance;
        cfg.hornetKeyboardEnabled = false;
        cfg.hornetControllerEnabled = true;

        var handler = FindHandler();
        if (handler == null)
            return;

        try
        {
            handler.ResetDefaultControllerButtonBindings();

            // Deliberately does not touch gameSettings' keyboard keys. This preset puts Hornet on
            // the controller and leaves the keyboard to the Shade; the Shade's 1-5 come from
            // EnsureShadeInventoryBindings, on the actions directly, and it runs on every
            // InputHandler awake/update too - so the binding survives a fresh boot on its own.
            // Writing gameSettings here would persist 1-5 over the player's real keyboard inventory
            // keys via SaveKeyboardSettings, which outlives the mod.
            EnsureShadeInventoryBindings(handler.inputActions);
            LogBindings("ApplyControllerDefaults: OpenInventory", handler.inputActions?.OpenInventory);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Every control any supported pad uses for "open inventory", bound as a set rather than picked
    /// per platform.
    /// <para>
    /// <c>InputHandler.MapControllerButtons</c> binds exactly one of these per <c>GamepadType</c> -
    /// <c>Back</c> for a 360 pad, <c>View</c>+<c>Back</c> for Xbox One/Series, <c>TouchPadButton</c>
    /// for PS4/PS5, <c>Minus</c> for Switch, <c>Select</c> for PS3 and unknown pads - and
    /// <see cref="ApplyLeftSideLayout"/> destroys whichever it picked, because binding Key1 to
    /// <c>OpenInventory</c> goes through <c>ClearBindings()</c> first. Re-adding a subset leaves
    /// whichever pads use the missing control with no inventory binding and nothing in the log,
    /// because there is genuinely nothing to fire.
    /// </para>
    /// <para>
    /// Binding all of them is safe and cheaper than detecting the pad: a control the device does not
    /// have never reads as pressed (<c>InputDevice.GetControl</c> returns <c>InputControl.Null</c>),
    /// and <c>AddBinding</c> no-ops on an equal binding already present. Such a binding is invisible
    /// in <c>PlayerAction.Bindings</c> for the same reason - see
    /// <c>InputDeviceBlocker.IsDrivingAllowedHeroAction</c>, which is why nothing reads that list.
    /// </para>
    /// </summary>
    private static readonly InputControlType[] ShadeInventoryControls =
    {
        InputControlType.View,
        InputControlType.Back,
        InputControlType.Select,
        InputControlType.TouchPadButton,
        InputControlType.Minus
    };

    /// <summary>
    /// Idempotently gives whichever device Hornet is <i>not</i> using a working binding on the five
    /// inventory-open actions, for the Shade. Deliberately leaves Hornet's own bindings alone.
    /// <para>
    /// Safe to call automatically and repeatedly - <c>LegacyHelper.MenuInputBridge.EnsureBindings</c>
    /// fires it at <c>InputHandler.OnAwake</c> and on every <c>OnUpdateHeroActions</c> - because
    /// <c>PlayerAction.AddBinding</c> no-ops on an equal binding, comparing by value rather than
    /// reference. That automatic call is the point: <see cref="ApplyKeyboardDefaults"/> and
    /// <see cref="ApplyControllerDefaults"/> only run from a preset click in the settings menu, so a
    /// binding that lived only in them left the Shade's device unable to open the inventory on a
    /// fresh boot until the player clicked a preset.
    /// </para>
    /// </summary>
    internal static void EnsureShadeInventoryBindings(HeroActions? actions)
    {
        if (actions == null)
            return;

        try
        {
            var cfg = ModConfig.Instance;
            if (cfg == null)
                return;

            if (cfg.hornetKeyboardEnabled)
            {
                // Hornet's on keyboard, so the controller (if any) is free for the Shade.
                foreach (var control in ShadeInventoryControls)
                {
                    actions.OpenInventory?.AddBinding(new DeviceBindingSource(control));
                }
            }
            else
            {
                // Hornet's on controller (or neither device is claimed yet), so the keyboard is free.
                actions.OpenInventory?.AddBinding(new KeyBindingSource(new[] { Key.Key1 }));
                actions.OpenInventoryTools?.AddBinding(new KeyBindingSource(new[] { Key.Key2 }));
                actions.OpenInventoryQuests?.AddBinding(new KeyBindingSource(new[] { Key.Key3 }));
                actions.OpenInventoryJournal?.AddBinding(new KeyBindingSource(new[] { Key.Key4 }));
                actions.OpenInventoryMap?.AddBinding(new KeyBindingSource(new[] { Key.Key5 }));
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// One row of the left-side keyboard preset: the key, where it is saved, and the live action it
    /// drives.
    /// <para>
    /// A single table because these were two hand-written lists of the same eighteen keys - one
    /// writing <c>gameSettings</c>, one binding the actions - and the number row had already drifted
    /// between them once, so every inventory shortcut but the first opened a different tab from the
    /// one its position on the number row promised.
    /// </para>
    /// </summary>
    internal readonly struct KeyboardPresetRow
    {
        internal KeyboardPresetRow(Key key, Action<GameSettings, string> save, Func<HeroActions, PlayerAction?> resolve)
        {
            Key = key;
            Save = save;
            Resolve = resolve;
        }

        internal Key Key { get; }

        internal Action<GameSettings, string> Save { get; }

        internal Func<HeroActions, PlayerAction?> Resolve { get; }
    }

    /// <summary>
    /// The preset itself. The number keys are in <c>InventoryPaneList.PaneTypes</c> order - the
    /// left-to-right tab order a player pressing key N expects - rather than the order the settings
    /// fields happen to be declared in.
    /// </summary>
    internal static readonly KeyboardPresetRow[] LeftSideLayout =
    {
        new KeyboardPresetRow(Key.Space, (s, v) => s.jumpKey = v, a => a.Jump),
        new KeyboardPresetRow(Key.F, (s, v) => s.attackKey = v, a => a.Attack),
        new KeyboardPresetRow(Key.LeftShift, (s, v) => s.dashKey = v, a => a.Dash),
        new KeyboardPresetRow(Key.Q, (s, v) => s.castKey = v, a => a.Cast),
        new KeyboardPresetRow(Key.E, (s, v) => s.superDashKey = v, a => a.SuperDash),
        new KeyboardPresetRow(Key.R, (s, v) => s.dreamNailKey = v, a => a.DreamNail),
        new KeyboardPresetRow(Key.Tab, (s, v) => s.quickMapKey = v, a => a.QuickMap),
        new KeyboardPresetRow(Key.Key1, (s, v) => s.inventoryKey = v, a => a.OpenInventory),
        new KeyboardPresetRow(Key.Key2, (s, v) => s.inventoryToolsKey = v, a => a.OpenInventoryTools),
        new KeyboardPresetRow(Key.Key3, (s, v) => s.inventoryQuestsKey = v, a => a.OpenInventoryQuests),
        new KeyboardPresetRow(Key.Key4, (s, v) => s.inventoryJournalKey = v, a => a.OpenInventoryJournal),
        new KeyboardPresetRow(Key.Key5, (s, v) => s.inventoryMapKey = v, a => a.OpenInventoryMap),
        new KeyboardPresetRow(Key.G, (s, v) => s.quickCastKey = v, a => a.QuickCast),
        new KeyboardPresetRow(Key.C, (s, v) => s.tauntKey = v, a => a.Taunt),
        new KeyboardPresetRow(Key.W, (s, v) => s.upKey = v, a => a.Up),
        new KeyboardPresetRow(Key.S, (s, v) => s.downKey = v, a => a.Down),
        new KeyboardPresetRow(Key.A, (s, v) => s.leftKey = v, a => a.Left),
        new KeyboardPresetRow(Key.D, (s, v) => s.rightKey = v, a => a.Right),
    };

    private static void ApplyLeftSideLayout(InputHandler handler)
    {
        if (handler == null)
            return;

        var gm = GameManager.instance;
        var settings = gm != null ? gm.gameSettings : null;
        if (settings != null)
        {
            foreach (var row in LeftSideLayout)
            {
                row.Save(settings, row.Key.ToString());
            }

            try { settings.SaveKeyboardSettings(); } catch { }
        }

        var actions = handler.inputActions;
        if (actions == null)
            return;

        foreach (var row in LeftSideLayout)
        {
            var action = row.Resolve(actions);
            if (action == null)
            {
                continue;
            }

            action.ClearBindings();
            action.AddBinding(new KeyBindingSource(new[] { row.Key }));
        }

        // The controller is fully free for the Shade in this preset (Hornet is on keyboard, and
        // hornetControllerEnabled was just set false above), but the ClearBindings above just wiped
        // whatever controller binding OpenInventory had. Restore one.
        EnsureShadeInventoryBindings(actions);
        LogBindings("ApplyKeyboardDefaults: OpenInventory", actions.OpenInventory);
    }
}
