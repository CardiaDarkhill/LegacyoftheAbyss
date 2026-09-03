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
    /// <summary>
    /// Hornet is never handed the Shade player's pad back, and never handed a dead one.
    /// <para>
    /// This is what stops the active device sticking. Blocking is deliberately off during a scene
    /// load and while a menu is open, so a Shade player holding anything across one of those leaves
    /// their pad as the active device. The restore then has to have somewhere of Hornet's to put it,
    /// or she stays without input until the other player lets go - which is the bug, and which the
    /// quick map made reproducible on demand.
    /// </para>
    /// </summary>
    [Fact]
    public void TheShadePadIsNeverWhatHornetIsRestoredTo()
    {
        var pad = new InControl.InputDevice("Test Pad");

        Assert.False(
            LegacyHelper.InputDeviceBlocker.IsUsableHornetDevice(pad, shadeOwned: true),
            "The Shade player's pad must never be handed to Hornet.");

        // Not shade-owned: usable exactly when it is still plugged in. Stated against the device's
        // own answer so the test says what the rule is rather than assuming a construction detail.
        Assert.Equal(
            pad.IsAttached,
            LegacyHelper.InputDeviceBlocker.IsUsableHornetDevice(pad, shadeOwned: false));
    }

    /// <summary>
    /// Nothing is not a device. The restore leaves the active device alone rather than forcing it to
    /// null - an earlier attempt at this bug did force null, and took Hornet's controller with it.
    /// </summary>
    [Fact]
    public void NothingIsNotADeviceHornetCanUse()
    {
        Assert.False(LegacyHelper.InputDeviceBlocker.IsUsableHornetDevice(null, shadeOwned: false));
        Assert.False(LegacyHelper.InputDeviceBlocker.IsUsableHornetDevice(InControl.InputDevice.Null, shadeOwned: false));
    }

    /// <summary>
    /// The two-controller setup, which is the one people actually play: pad 1 is the Shade player's
    /// and must be kept away from Hornet, pad 0 is Hornet's own and must not be.
    /// <para>
    /// This is the shape of the bug it was written for. The shade-command binding sits on pad 0 on
    /// purpose - ordering the Shade about is Hornet's player's control - so asking "is any shade
    /// binding on this pad" answered yes for both. The Shade then looked like it had claimed every
    /// controller, which tripped the guard that stops Hornet being left with none, and *neither* pad
    /// was reserved. Hornet answered to both, so one stick drove both characters.
    /// </para>
    /// </summary>
    [Fact]
    public void TheDualControllerPresetReservesOnlyTheShadePad()
    {
        var config = new ShadeInputConfig();
        config.ApplyControllerLayout(deviceIndex: 1);

        Assert.True(config.ReservesControllerIndex(1), "Pad 1 is the Shade player's and must be reserved.");
        Assert.False(config.ReservesControllerIndex(0), "Pad 0 is Hornet's and must never be reserved.");

        // The literal question still answers yes for pad 0, which is exactly why reserving had to
        // stop asking it. Asserted so the two do not get quietly merged back together.
        Assert.True(config.IsControllerIndexInUse(0));
    }

    /// <summary>
    /// With two pads and the ordinary preset, the Shade has not claimed them all - so the escape
    /// hatch that would hand both back to Hornet must stay shut.
    /// </summary>
    [Fact]
    public void TwoPadsAreNotAllClaimedByTheShade()
    {
        var config = new ShadeInputConfig();
        config.ApplyControllerLayout(deviceIndex: 1);

        Assert.False(
            LegacyHelper.InputDeviceBlocker.ShadeUsesAllControllers(config, targetIndex: 1, deviceCount: 2),
            "Reserving pad 1 must not be waved off as 'the Shade has taken everything'.");
    }

    /// <summary>
    /// The guard still has to work when it is genuinely needed: a Shade configured to answer to
    /// every attached pad must not leave Hornet with nothing.
    /// </summary>
    [Fact]
    public void AShadeOnEveryPadStillLeavesHornetOne()
    {
        var config = new ShadeInputConfig();
        config.ApplyControllerLayout(deviceIndex: 1);

        // Move the Shade's own controls onto pad 0 as well, so it really does claim both.
        config.moveLeft.primary.controllerDevice = 0;
        config.moveRight.primary.controllerDevice = 0;

        Assert.True(config.ReservesControllerIndex(0));
        Assert.True(
            LegacyHelper.InputDeviceBlocker.ShadeUsesAllControllers(config, targetIndex: 1, deviceCount: 2),
            "A Shade genuinely on every pad must still trip the guard.");
    }

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

    private sealed class InputBlockerEnvironment : GameStaticsScope
    {
        internal InputBlockerEnvironment()
        {
            Data.isInventoryOpen = false;
        }

        internal void SetGameState(GameState state) => SetProperty(Gm, "GameState", state);

        internal void SetPaused(bool value) => Gm.isPaused = value;

        internal void SetInventoryOpen(bool value) => Data.isInventoryOpen = value;
    }
}
