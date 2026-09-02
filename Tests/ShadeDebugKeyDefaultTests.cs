using UnityEngine;
using Xunit;

/// <summary>
/// The debug keys' defaults, and the two rules about not disturbing anyone who has already set
/// their controls up: an action that is bound keeps what it has, and a default whose key is in use
/// elsewhere is dropped rather than applied.
/// </summary>
public class ShadeDebugKeyDefaultTests
{
    private static ShadeInputConfig Fresh() => ShadeInputConfig.CreateDefault();

    [Theory]
    [InlineData(ShadeAction.DebugSoulIncrease, KeyCode.Equals)]
    [InlineData(ShadeAction.DebugSoulDecrease, KeyCode.Minus)]
    [InlineData(ShadeAction.DebugDamageShade, KeyCode.LeftBracket)]
    [InlineData(ShadeAction.DebugHealShade, KeyCode.RightBracket)]
    [InlineData(ShadeAction.DebugSoulReset, KeyCode.Backslash)]
    public void DefaultsAreTheRowRightOfTheLetters(ShadeAction action, KeyCode expected)
    {
        var binding = Fresh().GetBinding(action);

        Assert.Equal(ShadeBindingOptionType.Key, binding.primary.type);
        Assert.Equal(expected, binding.primary.key);
    }

    [Fact]
    public void UnboundDebugActionsAreFilledInOnLoad()
    {
        // A config saved before the debug keys had defaults: the actions exist and hold nothing.
        var config = Fresh();
        foreach (var action in new[]
                 {
                     ShadeAction.DebugSoulIncrease, ShadeAction.DebugSoulDecrease,
                     ShadeAction.DebugDamageShade, ShadeAction.DebugHealShade,
                     ShadeAction.DebugSoulReset
                 })
        {
            config.SetBinding(action, new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None()));
        }

        Assert.Equal(5, config.ApplyMissingDebugDefaults());
        Assert.Equal(KeyCode.Equals, config.GetBinding(ShadeAction.DebugSoulIncrease).primary.key);
        Assert.Equal(KeyCode.Backslash, config.GetBinding(ShadeAction.DebugSoulReset).primary.key);

        // Idempotent: a second load must not report changes it did not make.
        Assert.Equal(0, config.ApplyMissingDebugDefaults());
    }

    [Fact]
    public void ADebugKeyThePlayerHasAlreadyChosenIsLeftAlone()
    {
        var config = Fresh();
        config.SetBinding(
            ShadeAction.DebugSoulIncrease,
            new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.F9), ShadeBindingOption.None()));

        config.ApplyMissingDebugDefaults();

        Assert.Equal(KeyCode.F9, config.GetBinding(ShadeAction.DebugSoulIncrease).primary.key);
    }

    [Fact]
    public void ADefaultIsDroppedWhenThatKeyIsAlreadyUsedForSomethingElse()
    {
        // The case the rule exists for: someone has the Shade's nail on '-'. Switching the debug
        // keys on must not quietly turn their attack button into a soul drain.
        var config = Fresh();
        config.SetBinding(
            ShadeAction.Nail,
            new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.Minus), ShadeBindingOption.None()));
        config.SetBinding(
            ShadeAction.DebugSoulDecrease,
            new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None()));

        config.ApplyMissingDebugDefaults();

        Assert.Equal(ShadeBindingOptionType.None, config.GetBinding(ShadeAction.DebugSoulDecrease).primary.type);
        Assert.Equal(KeyCode.Minus, config.GetBinding(ShadeAction.Nail).primary.key);
    }

    [Fact]
    public void ASecondaryBindingCountsAsUsingTheKey()
    {
        var config = Fresh();
        config.SetBinding(
            ShadeAction.Focus,
            new ShadeBinding(ShadeBindingOption.FromKey(KeyCode.H), ShadeBindingOption.FromKey(KeyCode.Equals)));
        config.SetBinding(
            ShadeAction.DebugSoulIncrease,
            new ShadeBinding(ShadeBindingOption.None(), ShadeBindingOption.None()));

        config.ApplyMissingDebugDefaults();

        Assert.Equal(ShadeBindingOptionType.None, config.GetBinding(ShadeAction.DebugSoulIncrease).primary.type);
    }
}
