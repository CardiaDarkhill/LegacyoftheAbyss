using InControl;
using UnityEngine;
using Xunit;

/// <summary>
/// The swap button's default, and the rule that keeps it from taking a control someone is using.
/// <para>
/// The action was added after the mod had shipped, so its default arrives on every config that
/// already exists - an absent JSON field keeps whatever the constructor gave it, and nobody chose
/// it. A player who had already bound R or the right stick would otherwise find that control
/// quietly doing two things.
/// </para>
/// </summary>
public class ShadeSwapCharacterBindingTests
{
    [Fact]
    public void TheDefaultIsRAndTheRightStickClick()
    {
        var config = ShadeInputConfig.CreateDefault();
        var binding = config.GetBinding(ShadeAction.SwapCharacter);

        Assert.Equal(ShadeBindingOptionType.Key, binding.primary.type);
        Assert.Equal(KeyCode.R, binding.primary.key);
        Assert.Equal(ShadeBindingOptionType.Controller, binding.secondary.type);
        Assert.Equal(InputControlType.RightStickButton, binding.secondary.control);
    }

    [Fact]
    public void AFreshConfigKeepsBothHalvesOfIt()
    {
        var config = ShadeInputConfig.CreateDefault();

        Assert.Equal(0, config.DropCollidingDefaults());
    }

    [Fact]
    public void AKeyTheresAlreadyASlashOnIsLeftToTheSlash()
    {
        var config = ShadeInputConfig.CreateDefault();
        config.SetBindingOption(ShadeAction.Nail, secondary: false, ShadeBindingOption.FromKey(KeyCode.R));

        Assert.Equal(1, config.DropCollidingDefaults());

        var swap = config.GetBinding(ShadeAction.SwapCharacter);
        Assert.Equal(ShadeBindingOptionType.None, swap.primary.type);

        // Only the half that collided. The pad half is still free, so it stays.
        Assert.Equal(ShadeBindingOptionType.Controller, swap.secondary.type);
        Assert.Equal(KeyCode.R, config.GetBinding(ShadeAction.Nail).primary.key);
    }

    [Fact]
    public void APadButtonTheresAlreadySomethingOnIsLeftAlone()
    {
        var config = ShadeInputConfig.CreateDefault();
        config.SetBindingOption(
            ShadeAction.Teleport,
            secondary: false,
            ShadeBindingOption.FromControl(InputControlType.RightStickButton));

        Assert.Equal(1, config.DropCollidingDefaults());
        Assert.Equal(ShadeBindingOptionType.None, config.GetBinding(ShadeAction.SwapCharacter).secondary.type);
    }

    [Fact]
    public void ADeviceAgnosticControlStillCountsAsTheSameButton()
    {
        // A rebound control remembers the pad it was captured on; the default names no pad and
        // means "whichever is the companion's". They are the same physical button.
        var config = ShadeInputConfig.CreateDefault();
        config.SetBindingOption(
            ShadeAction.Focus,
            secondary: false,
            ShadeBindingOption.FromControl(InputControlType.RightStickButton, 1));

        Assert.Equal(1, config.DropCollidingDefaults());
        Assert.Equal(ShadeBindingOptionType.None, config.GetBinding(ShadeAction.SwapCharacter).secondary.type);
    }

    [Fact]
    public void AControlThePlayerMovedItOntoIsNeverTakenAway()
    {
        // Once it is off its default it is the player's own doing, collision or not - otherwise the
        // pass would keep undoing a deliberate choice on every launch.
        var config = ShadeInputConfig.CreateDefault();
        config.SetBindingOption(ShadeAction.SwapCharacter, secondary: false, ShadeBindingOption.FromKey(KeyCode.T));
        config.SetBindingOption(ShadeAction.Nail, secondary: false, ShadeBindingOption.FromKey(KeyCode.T));

        Assert.Equal(0, config.DropCollidingDefaults());
        Assert.Equal(KeyCode.T, config.GetBinding(ShadeAction.SwapCharacter).primary.key);
    }

    [Fact]
    public void SwappingIsNotADebugActionAndSoStillReservesAPad()
    {
        // IsDebugAction is an ordering test against the debug block, so where the member sits in
        // the enum decides whether the companion's pad is kept away from Hornet.
        var config = ShadeInputConfig.CreateDefault();
        foreach (var action in ShadeInputConfig.AllActions)
        {
            config.SetBinding(action, new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None()));
        }

        config.SetBindingOption(
            ShadeAction.SwapCharacter,
            secondary: false,
            ShadeBindingOption.FromControl(InputControlType.RightStickButton, 1));

        Assert.True(config.ReservesControllerIndex(1));
    }
}
