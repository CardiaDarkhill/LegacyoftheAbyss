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
    private static void LogBindings(string label, PlayerAction action)
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

    /// <summary>
    /// Resolves the game's InputHandler, preferring the singleton and falling back through
    /// GameManager and finally a scene scan. Shared with ShadeInventoryPane, which had a
    /// byte-identical private copy of this cascade.
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
            try
            {
                return Object.FindAnyObjectByType<InputHandler>();
            }
            catch
            {
            }
        }

        return null;
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

            // Best-effort only, for whatever UI reads these labels directly (e.g. a native rebind
            // screen). InputHandler.MapKeyboardLayoutFromGameSettings (called internally by the reset
            // above) is *supposed* to restore the keyboard side of the 5 inventory-open actions from
            // these same settings, but confirmed via logging that it doesn't actually pick this up -
            // seeding it and reading it back off the same object showed the correct value, yet the
            // resulting action still ended up with zero keyboard bindings. Not chasing that further:
            // EnsureShadeInventoryBindings below adds the real, functional keyboard binding directly.
            var gm = GameManager.instance;
            var settings = gm != null ? gm.gameSettings : null;
            if (settings != null)
            {
                settings.inventoryKey = Key.Key1.ToString();
                settings.inventoryToolsKey = Key.Key2.ToString();
                settings.inventoryQuestsKey = Key.Key3.ToString();
                settings.inventoryJournalKey = Key.Key4.ToString();
                settings.inventoryMapKey = Key.Key5.ToString();
                try { settings.SaveKeyboardSettings(); } catch { }
            }

            EnsureShadeInventoryBindings(handler.inputActions);
            LogBindings("ApplyControllerDefaults: OpenInventory", handler.inputActions?.OpenInventory);
        }
        catch
        {
        }
    }

    /// <summary>
    /// Idempotently ensures whichever device Hornet is *not* currently using has a working binding
    /// on the 5 inventory-open actions, for the Shade. Deliberately does not touch Hornet's own
    /// bindings at all - unlike <see cref="ApplyKeyboardDefaults"/>/<see cref="ApplyControllerDefaults"/>,
    /// which only ever run from an explicit preset-button click in the mod's settings menu, this is
    /// meant to be called from somewhere that runs automatically (see the call in
    /// <c>LegacyHelper.MenuInputBridge.EnsureBindings</c>, which fires both at
    /// <c>InputHandler.OnAwake</c> and on every subsequent <c>InputHandler.OnUpdateHeroActions</c>).
    /// <para>
    /// Without this, a fresh game launch that loads a persisted "Hornet on keyboard" (or controller)
    /// config correctly restores Hornet's own bindings through the base game's own settings-load path
    /// - but the Shade's *extra* binding on these same actions only ever lived inside the two preset
    /// methods above, so on a fresh boot the Shade's device had no way to open the inventory at all
    /// until the player re-clicked a preset that session.
    /// </para>
    /// <para>
    /// <c>PlayerAction.AddBinding</c> already no-ops when an equal binding is already present
    /// (<c>BindingSource.Equals</c> compares by value - the key or control, not by reference), so
    /// calling this repeatedly (every <c>OnUpdateHeroActions</c>) does not accumulate duplicates.
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
                actions.OpenInventory?.AddBinding(new DeviceBindingSource(InputControlType.Back));
                actions.OpenInventory?.AddBinding(new DeviceBindingSource(InputControlType.Select));
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
