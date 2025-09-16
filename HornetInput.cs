using UnityEngine;

public static class HornetInput
{
    private static InputHandler FindHandler()
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
}
