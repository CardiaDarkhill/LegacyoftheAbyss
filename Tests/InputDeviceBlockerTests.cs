using System;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalEnums;
using Xunit;

/// <summary>
/// Covers the "should a shade-owned device be kept away from Hornet's actions?" decision.
/// <para>
/// Scope note: anything that needs a live Unity player loop is deliberately out. Constructing a
/// <c>UIManager</c> runs a static initializer that calls <c>Animator.StringToHash</c>, and
/// <c>new GameObject(...)</c> / <c>Time.timeScale</c> are extern calls into the engine - all of
/// them throw <c>SecurityException: ECall methods must be packaged into a system module</c> in a
/// plain test host. The uiState/menuState half of the decision is therefore covered through
/// <see cref="MenuStateUtility.IsMenuStateName"/>, which is where the actual classification lives.
/// </para>
/// </summary>
[Collection(ShadeRuntimeCollection.Name)]
public class InputDeviceBlockerTests
{
    [Fact]
    public void BlocksShadeDeviceDuringOrdinaryGameplay()
    {
        using var environment = new InputBlockerEnvironment();

        environment.SetGameState(GameState.PLAYING);
        environment.SetPaused(false);
        environment.SetInventoryOpen(false);

        Assert.True(LegacyHelper.InputDeviceBlocker.EvaluateShouldBlockShadeDeviceInput());
    }

    [Fact]
    public void AllowsShadeDeviceWhilePaused()
    {
        using var environment = new InputBlockerEnvironment();

        environment.SetGameState(GameState.PLAYING);
        environment.SetInventoryOpen(false);
        environment.SetPaused(true);

        Assert.False(LegacyHelper.InputDeviceBlocker.EvaluateShouldBlockShadeDeviceInput());
    }

    [Fact]
    public void AllowsShadeDeviceWhileInventoryIsOpen()
    {
        using var environment = new InputBlockerEnvironment();

        environment.SetGameState(GameState.PLAYING);
        environment.SetPaused(false);
        environment.SetInventoryOpen(true);

        Assert.False(LegacyHelper.InputDeviceBlocker.EvaluateShouldBlockShadeDeviceInput());
    }

    [Theory]
    [InlineData(GameState.CUTSCENE)]
    [InlineData(GameState.PAUSED)]
    [InlineData(GameState.MAIN_MENU)]
    [InlineData(GameState.LOADING)]
    public void AllowsShadeDeviceOutsideActiveGameplay(GameState state)
    {
        using var environment = new InputBlockerEnvironment();

        environment.SetPaused(false);
        environment.SetInventoryOpen(false);
        environment.SetGameState(state);

        Assert.False(LegacyHelper.InputDeviceBlocker.EvaluateShouldBlockShadeDeviceInput());
    }

    [Theory]
    [InlineData("PAUSE_MENU")]
    [InlineData("inventory_overlay")]
    [InlineData("MapScreen")]
    [InlineData("JOURNAL")]
    [InlineData("SHOP")]
    [InlineData("OPTIONS_MENU")]
    public void MenuStateNamesAreTreatedAsMenus(string stateName)
    {
        Assert.True(MenuStateUtility.IsMenuStateName(stateName));
    }

    [Theory]
    [InlineData("PLAYING")]
    [InlineData("GAMEPLAY")]
    [InlineData("")]
    [InlineData(null)]
    public void GameplayStateNamesAreNotTreatedAsMenus(string stateName)
    {
        Assert.False(MenuStateUtility.IsMenuStateName(stateName));
    }

    /// <summary>
    /// Pause and the inventory shortcuts have to survive the shade owning a controller, otherwise
    /// the player cannot open the pause menu from that pad at all (the shade's device never becomes
    /// InControl's ActiveDevice during gameplay, so HeroActions only ever polls Hornet's device).
    /// These names must match <c>HeroActions</c> exactly - the set is compared ordinally.
    /// </summary>
    [Theory]
    [InlineData("Pause")]
    [InlineData("openInventory")]
    [InlineData("openInventoryMap")]
    [InlineData("openInventoryJournal")]
    [InlineData("openInventoryTools")]
    [InlineData("openInventoryQuests")]
    [InlineData("Quick Map")]
    public void AllowedHeroActionNamesMatchHeroActions(string actionName)
    {
        Assert.Contains(actionName, LegacyHelper.InputDeviceBlocker.AllowedHeroActionNames);
    }

    [Theory]
    [InlineData("Jump")]
    [InlineData("Attack")]
    [InlineData("Dash")]
    [InlineData("Cast")]
    public void GameplayHeroActionNamesAreNotAllowed(string actionName)
    {
        Assert.DoesNotContain(actionName, LegacyHelper.InputDeviceBlocker.AllowedHeroActionNames);
    }

    private sealed class InputBlockerEnvironment : IDisposable
    {
        private readonly object originalGameManager;
        private readonly object originalPlayerData;
        private readonly GameManager gm;
        private readonly PlayerData playerData;

        internal InputBlockerEnvironment()
        {
            originalGameManager = GetStaticField(typeof(GameManager), "_instance");
            originalPlayerData = GetStaticField(typeof(PlayerData), "_instance");

            gm = (GameManager)FormatterServices.GetUninitializedObject(typeof(GameManager));
            playerData = (PlayerData)FormatterServices.GetUninitializedObject(typeof(PlayerData));

            SetProperty(gm, "GameState", GameState.PLAYING);
            playerData.isInventoryOpen = false;

            SetStaticField(typeof(GameManager), "_instance", gm);
            SetStaticField(typeof(PlayerData), "_instance", playerData);
        }

        internal void SetGameState(GameState state) => SetProperty(gm, "GameState", state);

        internal void SetPaused(bool value) => gm.isPaused = value;

        internal void SetInventoryOpen(bool value) => playerData.isInventoryOpen = value;

        public void Dispose()
        {
            SetStaticField(typeof(GameManager), "_instance", originalGameManager);
            SetStaticField(typeof(PlayerData), "_instance", originalPlayerData);
        }

        private static object GetStaticField(Type type, string name)
        {
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return field?.GetValue(null);
        }

        private static void SetStaticField(Type type, string name, object value)
        {
            var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            field?.SetValue(null, value);
        }

        private static void SetProperty(object target, string name, object value)
        {
            if (target == null)
            {
                return;
            }

            var property = target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            property?.SetValue(target, value, null);
        }
    }
}
