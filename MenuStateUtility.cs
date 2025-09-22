#nullable disable
using System;
using System.Reflection;
using GlobalEnums;
using UnityEngine;

internal static class MenuStateUtility
{
    internal static GameManager TryGetGameManager()
    {
        try
        {
            var gm = GameManager.UnsafeInstance;
            if (!ReferenceEquals(gm, null))
            {
                return gm;
            }
        }
        catch
        {
        }

        try
        {
            var field = typeof(GameManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                var value = field.GetValue(null) as GameManager;
                if (!ReferenceEquals(value, null))
                {
                    return value;
                }
            }
        }
        catch
        {
        }

        try
        {
            var gm = GameManager.instance;
            if (!ReferenceEquals(gm, null))
            {
                return gm;
            }
        }
        catch
        {
        }

        return null;
    }

    internal static UIManager TryGetUiManager(GameManager gm)
    {
        try
        {
            if (!ReferenceEquals(gm, null))
            {
                var manager = gm.ui;
                if (!ReferenceEquals(manager, null))
                {
                    return manager;
                }
            }
        }
        catch
        {
        }

        try
        {
            var field = typeof(UIManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                var value = field.GetValue(null) as UIManager;
                if (!ReferenceEquals(value, null))
                {
                    return value;
                }
            }
        }
        catch
        {
        }

        try
        {
            var ui = UIManager.instance;
            if (!ReferenceEquals(ui, null))
            {
                return ui;
            }
        }
        catch
        {
        }

        return null;
    }

    internal static bool IsMenuActive(GameManager gm = null, UIManager ui = null)
    {
        try
        {
            if (ShadeSettingsMenu.IsShowing)
            {
                return true;
            }
        }
        catch
        {
        }

        if (ReferenceEquals(gm, null))
        {
            gm = TryGetGameManager();
        }

        if (!ReferenceEquals(gm, null))
        {
            try
            {
                if (gm.IsGamePaused())
                {
                    return true;
                }
            }
            catch
            {
            }
        }

        if (Time.timeScale <= 0.0001f)
        {
            return true;
        }

        if (ReferenceEquals(ui, null))
        {
            ui = TryGetUiManager(gm);
        }

        string stateName = TryGetUiStateName(ui);
        if (IsMenuStateName(stateName))
        {
            return true;
        }

        return false;
    }

    internal static string TryGetUiStateName(UIManager ui)
    {
        if (ReferenceEquals(ui, null))
        {
            return null;
        }

        try
        {
            var type = ui.GetType();
            while (type != null)
            {
                var field = type.GetField("uiState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    return ConvertStateValueToName(field.GetValue(ui));
                }
                type = type.BaseType;
            }
        }
        catch
        {
        }

        try
        {
            var type = ui.GetType();
            while (type != null)
            {
                var property = type.GetProperty("uiState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (property != null && property.GetIndexParameters().Length == 0)
                {
                    return ConvertStateValueToName(property.GetValue(ui, null));
                }
                type = type.BaseType;
            }
        }
        catch
        {
        }

        return null;
    }

    private static string ConvertStateValueToName(object value)
    {
        if (value == null)
        {
            return null;
        }

        try
        {
            if (value is string str)
            {
                return str;
            }

            if (value is UIState state)
            {
                return state.ToString();
            }

            if (value is Enum enumValue)
            {
                return enumValue.ToString();
            }

            if (value is int raw)
            {
                return ((UIState)raw).ToString();
            }
        }
        catch
        {
        }

        return value.ToString();
    }

    internal static bool IsMenuState(UIState state)
    {
        return IsMenuStateName(state.ToString());
    }

    internal static bool IsMenuStateName(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return false;
        }

        if (string.Equals(stateName, "PLAYING", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(stateName, "GAMEPLAY", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return stateName.IndexOf("PAUSE", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("MENU", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("INVENTORY", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("MAP", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("JOURNAL", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("SHOP", StringComparison.OrdinalIgnoreCase) >= 0 ||
               stateName.IndexOf("OPTION", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
