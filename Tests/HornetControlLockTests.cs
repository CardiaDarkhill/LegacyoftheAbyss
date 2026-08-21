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
