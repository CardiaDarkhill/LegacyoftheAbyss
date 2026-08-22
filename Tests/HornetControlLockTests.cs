using System;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalEnums;
using InControl;
using Xunit;

/// <summary>
/// Covers the shared "Hornet's controls are locked" flag that the Shade's bench/cutscene docking,
/// its combat gate, and the Shade HUD all read, plus the InControl binding-visibility rule that the
/// shade-owned-device menu access depends on.
/// <para>
/// Scope note matches <see cref="InputDeviceBlockerTests"/>: anything needing a live Unity player
/// loop is out. <c>HeroController.instance</c> in particular reaches for <c>FindObjectOfType</c> and
/// <c>CustomPlayerLoop</c>, both extern calls that throw in a plain test host - which is exactly why
/// the cases here are the ones that resolve before <c>HornetControlsLocked</c> ever looks at it.
/// </para>
/// </summary>
[Collection(ShadeRuntimeCollection.Name)]
public class HornetControlLockTests
{
    [Fact]
    public void SittingAtABenchLocksHornetsControls()
    {
        using var environment = new HeroStateEnvironment();

        environment.SetAtBench(true);

        Assert.True(LegacyHelper.ShadeController.HornetControlsLocked());
    }

    [Fact]
    public void OrdinaryGameplayDoesNotLockHornetsControls()
    {
        using var environment = new HeroStateEnvironment();

        environment.SetAtBench(false);

        // With no HeroController reachable there is no scripted control loss to report either, so
        // the Shade stays under player control.
        Assert.False(LegacyHelper.ShadeController.HornetControlsLocked());
    }

    [Fact]
    public void MissingGameManagerDoesNotLockHornetsControls()
    {
        using var environment = new HeroStateEnvironment();

        environment.ClearGameManager();

        Assert.False(LegacyHelper.ShadeController.HornetControlsLocked());
    }

    [Fact]
    public void OrdinaryGameplayIsNotAControlLoss()
    {
        Assert.False(LegacyHelper.ShadeController.EvaluateControlsLocked(OrdinaryGameplay()));
    }

    /// <summary>
    /// Menus take <c>acceptingInput</c> away as well, and always have - the Shade stays under player
    /// control there so its own charm tab is reachable.
    /// </summary>
    [Fact]
    public void AnOpenMenuIsNotAControlLoss()
    {
        var state = OrdinaryGameplay();
        state.AcceptingInput = false;
        state.Paused = true;

        Assert.False(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// The three the Shade is meant to dock for, each named outright by the game rather than inferred
    /// from Hornet having lost her controls.
    /// </summary>
    [Theory]
    [InlineData("AtBench")]
    [InlineData("HeldByInteraction")]
    [InlineData("InCutscene")]
    public void TheScriptedHoldsLockHornetsControls(string scriptedField)
    {
        var state = OrdinaryGameplay();
        SetControlStateField(ref state, scriptedField, true);

        Assert.True(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// A scripted hold locks even though Hornet still nominally has her controls - a conversation
    /// starts by parking the interactable, and the relinquish follows a frame later.
    /// </summary>
    [Fact]
    public void AScriptedHoldLocksBeforeControlIsTakenAway()
    {
        var state = OrdinaryGameplay();
        state.HeldByInteraction = true;

        Assert.True(state.AcceptingInput);
        Assert.True(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// The reported bug, and the whole reason the rule cannot be built on <c>controlReqlinquished</c>:
    /// the Drifter's Cloak on an updraft, the air dash, the Needolin, silk skills, tools and the quick
    /// map all take Hornet's controls away for their duration while the player is still driving. Each
    /// arrives here looking exactly like this, and the game's HUD staying up is what says otherwise.
    /// </summary>
    [Fact]
    public void AnActionOfHornetsOwnIsNotAControlLoss()
    {
        var state = OrdinaryGameplay();
        state.ControlRelinquished = true;
        state.AcceptingInput = false;
        state.GameHudHidden = false;

        Assert.False(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// The same control loss, but the game has taken its own HUD away too. Nothing the player asks
    /// Hornet to do does that, so this is a scripted sequence that did not identify itself.
    /// </summary>
    [Fact]
    public void AControlLossWithTheGameHudGoneLocks()
    {
        var state = OrdinaryGameplay();
        state.ControlRelinquished = true;
        state.AcceptingInput = false;
        state.GameHudHidden = true;

        Assert.True(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// The HUD being gone is only ever a tiebreaker for a control loss. On its own - the reward
    /// popups and boss-door panels hide it while Hornet keeps playing - it decides nothing.
    /// </summary>
    [Fact]
    public void TheGameHudAloneDoesNotLock()
    {
        var state = OrdinaryGameplay();
        state.GameHudHidden = true;

        Assert.False(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>
    /// Scene changes and death keep locking. Neither is on the user-facing list, but the player has
    /// no control in either and the screen is going dark regardless.
    /// </summary>
    [Theory]
    [InlineData("Transitioning")]
    [InlineData("Downed")]
    public void SceneChangesAndDeathStillLock(string scriptedField)
    {
        var state = OrdinaryGameplay();
        state.ControlRelinquished = true;
        state.AcceptingInput = false;
        SetControlStateField(ref state, scriptedField, true);

        Assert.True(LegacyHelper.ShadeController.EvaluateControlsLocked(state));
    }

    /// <summary>Hornet on the ground with her controls, which is what everything else deviates from.</summary>
    private static LegacyHelper.ShadeController.HornetControlState OrdinaryGameplay()
    {
        return new LegacyHelper.ShadeController.HornetControlState { AcceptingInput = true };
    }

    private static void SetControlStateField(ref LegacyHelper.ShadeController.HornetControlState state, string name, bool value)
    {
        var field = typeof(LegacyHelper.ShadeController.HornetControlState)
            .GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.NotNull(field);

        // Boxing is the only way to reflect onto a struct field; unbox back over the original.
        object boxed = state;
        field.SetValue(boxed, value);
        state = (LegacyHelper.ShadeController.HornetControlState)boxed;
    }

    /// <summary>
    /// The rule <c>InputDeviceBlocker.IsDrivingAllowedHeroAction</c> exists to work around: a device
    /// binding for a control the action's current device does not have is invisible in
    /// <c>PlayerAction.Bindings</c>, while still being live in <c>UnfilteredBindings</c>.
    /// <para>
    /// Every control the shade-owned pad needs for menus - Back and Select (open inventory), Options
    /// and Menu (pause) - sits outside InControl's "standard" control range, so with no active device
    /// (which is the state at every boot, before anything has been touched) none of them are visible.
    /// Reading <c>Bindings</c> there is what made pause and the inventory unreachable from the
    /// Shade's pad until a preset was re-applied. If this test starts failing because the bindings
    /// have become visible, the workaround is no longer needed - it is not that the workaround broke.
    /// </para>
    /// </summary>
    [Theory]
    [InlineData(InputControlType.Back)]
    [InlineData(InputControlType.Select)]
    [InlineData(InputControlType.Options)]
    [InlineData(InputControlType.Menu)]
    public void MenuControlBindingsAreInvisibleWithoutAnActiveDevice(InputControlType control)
    {
        var actions = new HeroActions();
        try
        {
            var action = actions.OpenInventory;
            Assert.True(action.AddBinding(new DeviceBindingSource(control)));

            Assert.DoesNotContain(action.Bindings, binding => IsDeviceBindingFor(binding, control));
            Assert.Contains(action.UnfilteredBindings, binding => IsDeviceBindingFor(binding, control));
        }
        finally
        {
            actions.Destroy();
        }
    }

    /// <summary>
    /// The complement of the case above: a control inside the standard range stays visible even with
    /// no device, which is why only the pause/menu binds were ever affected and nothing else was.
    /// </summary>
    [Theory]
    [InlineData(InputControlType.Action1)]
    [InlineData(InputControlType.LeftStickUp)]
    public void StandardControlBindingsStayVisibleWithoutAnActiveDevice(InputControlType control)
    {
        var actions = new HeroActions();
        try
        {
            var action = actions.Jump;
            Assert.True(action.AddBinding(new DeviceBindingSource(control)));

            Assert.Contains(action.Bindings, binding => IsDeviceBindingFor(binding, control));
        }
        finally
        {
            actions.Destroy();
        }
    }

    private static bool IsDeviceBindingFor(BindingSource binding, InputControlType control)
    {
        return binding is DeviceBindingSource device && device.Control == control;
    }

    private sealed class HeroStateEnvironment : IDisposable
    {
        private readonly object originalGameManager;
        private readonly object originalPlayerData;
        private readonly GameManager gm;
        private readonly PlayerData playerData;

        internal HeroStateEnvironment()
        {
            originalGameManager = GetStaticField(typeof(GameManager), "_instance");
            originalPlayerData = GetStaticField(typeof(PlayerData), "_instance");

            gm = (GameManager)FormatterServices.GetUninitializedObject(typeof(GameManager));
            playerData = (PlayerData)FormatterServices.GetUninitializedObject(typeof(PlayerData));

            SetProperty(gm, "GameState", GameState.PLAYING);
            gm.playerData = playerData;
            playerData.atBench = false;

            SetStaticField(typeof(GameManager), "_instance", gm);
            SetStaticField(typeof(PlayerData), "_instance", playerData);
        }

        internal void SetAtBench(bool value) => playerData.atBench = value;

        internal void ClearGameManager() => SetStaticField(typeof(GameManager), "_instance", null);

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
