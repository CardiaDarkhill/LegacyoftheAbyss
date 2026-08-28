#nullable disable
using System;
using System.Reflection;
using GlobalEnums;

/// <summary>
/// "Is the player sitting in a menu?" - asked from several places that must not act while one is
/// open, and answered from whichever of the game's own notions of menu state can be reached.
/// <para>
/// Every accessor here reads a plain managed field first and only then falls back to a singleton
/// accessor that scans the scene. That ordering is not just about cost: <c>FindObjectOfType</c> is
/// an extern call, so the fallbacks throw <c>SecurityException</c> outside a Unity player loop, and
/// the pure-managed tests covering this reach the answer without ever touching them. The remaining
/// catches exist for that boundary - degrade to "no menu open" rather than take a test host down.
/// </para>
/// </summary>
internal static class MenuStateUtility
{
    internal static GameManager TryGetGameManager()
    {
        // UnsafeInstance is a bare read of the backing field; SilentInstance scans when that is
        // empty, and is preferred over `instance`, which logs an error every call besides.
        var gm = GameManager.UnsafeInstance;
        if (!ReferenceEquals(gm, null))
        {
            return gm;
        }

        try
        {
            gm = GameManager.SilentInstance;
            return ReferenceEquals(gm, null) ? null : gm;
        }
        catch
        {
            return null;
        }
    }

    private static FieldInfo s_uiManagerInstanceField;
    private static bool s_uiManagerInstanceFieldResolved;

    /// <summary>
    /// The <c>UIManager</c>, preferring the one the <c>GameManager</c> already holds. Unlike
    /// <c>GameManager</c> it offers no silent accessor, so the backing field is read reflectively
    /// before falling back to the one that scans and logs.
    /// <para>
    /// Both fallbacks sit inside the guard, not just the accessor: touching any static on
    /// <c>UIManager</c> runs its type initializer, which calls <c>Animator.StringToHash</c> and so
    /// throws outside a player loop even for the reflective read.
    /// </para>
    /// </summary>
    internal static UIManager TryGetUiManager(GameManager gm)
    {
        if (!ReferenceEquals(gm, null) && !ReferenceEquals(gm.ui, null))
        {
            return gm.ui;
        }

        try
        {
            if (!s_uiManagerInstanceFieldResolved)
            {
                s_uiManagerInstanceFieldResolved = true;
                s_uiManagerInstanceField = typeof(UIManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic);
            }

            if (s_uiManagerInstanceField?.GetValue(null) is UIManager cached && !ReferenceEquals(cached, null))
            {
                return cached;
            }

            var ui = UIManager.instance;
            return ReferenceEquals(ui, null) ? null : ui;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Deliberately not <c>PlayerData.instance</c>, which deserializes a blank singleton when none
    /// exists. A question about menu state must not create save data as a side effect.
    /// </summary>
    internal static PlayerData TryGetPlayerData()
    {
        return PlayerData.HasInstance ? PlayerData.instance : null;
    }

    internal static bool IsMenuActive(GameManager gm = null, UIManager ui = null)
    {
        if (ShadeSettingsMenu.IsShowing)
        {
            return true;
        }

        gm ??= TryGetGameManager();

        if (!ReferenceEquals(gm, null))
        {
            if (gm.isPaused || gm.IsGamePaused())
            {
                return true;
            }

            // The inventory is a PlayMaker FSM, and its own state name knows a pane is opening
            // before the UI state does.
            var inventoryFsm = gm.inventoryFSM;
            if (inventoryFsm != null && IsMenuStateName(inventoryFsm.ActiveStateName))
            {
                return true;
            }
        }

        var playerData = TryGetPlayerData();
        if (!ReferenceEquals(playerData, null) && playerData.isInventoryOpen)
        {
            return true;
        }

        ui ??= TryGetUiManager(gm);
        return !ReferenceEquals(ui, null) && IsMenuState(ui.uiState);
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

        return stateName.Contains("PAUSE", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("MENU", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("INVENTORY", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("MAP", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("JOURNAL", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("SHOP", StringComparison.OrdinalIgnoreCase) ||
               stateName.Contains("OPTION", StringComparison.OrdinalIgnoreCase);
    }
}
