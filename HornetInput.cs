using InControl;
using UnityEngine;

public static class HornetInput
{
    private static InputHandler? FindHandler()
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
            settings.inventoryKey = Key.Key1.ToString();
            settings.inventoryMapKey = Key.Key2.ToString();
            settings.inventoryJournalKey = Key.Key3.ToString();
            settings.inventoryToolsKey = Key.Key4.ToString();
            settings.inventoryQuestsKey = Key.Key5.ToString();
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
        Bind(actions.OpenInventoryMap, Key.Key2);
        Bind(actions.OpenInventoryJournal, Key.Key3);
        Bind(actions.OpenInventoryTools, Key.Key4);
        Bind(actions.OpenInventoryQuests, Key.Key5);
        Bind(actions.QuickCast, Key.G);
        Bind(actions.Taunt, Key.C);
        Bind(actions.Up, Key.W);
        Bind(actions.Down, Key.S);
        Bind(actions.Left, Key.A);
        Bind(actions.Right, Key.D);
    }
}
