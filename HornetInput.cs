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
            return Object.FindFirstObjectByType<InputHandler>();
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

            var shade = LegacyHelper.ShadeController.ActiveInstance;
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
            return (cfg != null && cfg.hornetKeyboardEnabled) || ShadeAiHoldsTheShade();
        }
        catch
        {
            return false;
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

    private static void ApplyLeftSideLayout(InputHandler handler)
    {
        if (handler == null)
            return;

        var gm = GameManager.instance;
        var settings = gm != null ? gm.gameSettings : null;
        if (settings != null)
        {
            settings.jumpKey = Key.Space.ToString();
            settings.attackKey = Key.F.ToString();
            settings.dashKey = Key.LeftShift.ToString();
            settings.castKey = Key.Q.ToString();
            settings.superDashKey = Key.E.ToString();
            settings.dreamNailKey = Key.R.ToString();
            settings.quickMapKey = Key.Tab.ToString();
            // Number-key order matches InventoryPaneList.PaneTypes (Inv, Tools, Quests, Journal,
            // Map), i.e. left-to-right tab order, not the order these fields happen to be declared
            // in. A player pressing key N expects the Nth visible tab to open; the previous ordering
            // (1=Inv, 2=Map, 3=Journal, 4=Tools, 5=Quests) silently scrambled that, so every shortcut
            // except 1 opened a different tab than its position on the number row suggested.
            settings.inventoryKey = Key.Key1.ToString();
            settings.inventoryToolsKey = Key.Key2.ToString();
            settings.inventoryQuestsKey = Key.Key3.ToString();
            settings.inventoryJournalKey = Key.Key4.ToString();
            settings.inventoryMapKey = Key.Key5.ToString();
            settings.quickCastKey = Key.G.ToString();
            settings.tauntKey = Key.C.ToString();
            settings.upKey = Key.W.ToString();
            settings.downKey = Key.S.ToString();
            settings.leftKey = Key.A.ToString();
            settings.rightKey = Key.D.ToString();
            try { settings.SaveKeyboardSettings(); } catch { }
        }

        var actions = handler.inputActions;
        if (actions == null)
            return;

        static void Bind(PlayerAction action, Key key)
        {
            if (action == null)
                return;
            action.ClearBindings();
            action.AddBinding(new KeyBindingSource(new[] { key }));
        }

        Bind(actions.Jump, Key.Space);
        Bind(actions.Attack, Key.F);
        Bind(actions.Dash, Key.LeftShift);
        Bind(actions.Cast, Key.Q);
        Bind(actions.SuperDash, Key.E);
        Bind(actions.DreamNail, Key.R);
        Bind(actions.QuickMap, Key.Tab);
        Bind(actions.OpenInventory, Key.Key1);
        Bind(actions.OpenInventoryTools, Key.Key2);
        Bind(actions.OpenInventoryQuests, Key.Key3);
        Bind(actions.OpenInventoryJournal, Key.Key4);
        Bind(actions.OpenInventoryMap, Key.Key5);
        Bind(actions.QuickCast, Key.G);
        Bind(actions.Taunt, Key.C);
        Bind(actions.Up, Key.W);
        Bind(actions.Down, Key.S);
        Bind(actions.Left, Key.A);
        Bind(actions.Right, Key.D);

        // The controller is fully free for the Shade in this preset (Hornet is on keyboard, and
        // hornetControllerEnabled was just set false above), but Bind()'s ClearBindings() just wiped
        // whatever controller binding OpenInventory had. Restore one.
        EnsureShadeInventoryBindings(actions);
        LogBindings("ApplyKeyboardDefaults: OpenInventory", actions.OpenInventory);
    }
}
