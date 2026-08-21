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
            foreach (var binding in action.Bindings)
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
            // Hornet is moving to the controller, which frees the keyboard for the Shade -- but
            // ResetDefaultControllerButtonBindings() below only rebuilds the *controller* side of
            // the 5 inventory-open actions explicitly; their keyboard side is restored from
            // whatever gm.gameSettings currently holds (InputHandler.MapKeyboardLayoutFromGameSettings,
            // called internally). Writing our Key1-5 order into those settings first is what makes
            // the Shade's keyboard shortcuts survive the switch, without touching the controller
            // binding that same call is about to add for Hornet.
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

            handler.ResetDefaultControllerButtonBindings();

            LogBindings("ApplyControllerDefaults: OpenInventory", handler.inputActions?.OpenInventory);
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
        // whatever controller binding OpenInventory had. Restore one -- Back/Select are the same
        // buttons GetShadeControllerBackValue() already reads elsewhere in this mod, so this matches
        // the button a player on that controller would instinctively try.
        try
        {
            actions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Back));
            actions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Select));
        }
        catch
        {
        }

        LogBindings("ApplyKeyboardDefaults: OpenInventory", actions.OpenInventory);
    }
}
