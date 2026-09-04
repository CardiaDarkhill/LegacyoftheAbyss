using System.Collections.Generic;
using UnityEngine;
using Xunit;

/// <summary>
/// The companion's keyboard defaults are Hollow Knight's own, so a player arriving from that game
/// already knows them. Two things are worth holding still.
/// <para>
/// One layout, not two. <c>ResetToDefaults</c> and <c>ApplyKeyboardLayout</c> used to carry separate
/// keyboard maps - one on the letters, one on the keypad - so which keys the companion answered to
/// depended on whether it had ever been on a pad.
/// </para>
/// <para>
/// And no key twice. Thirteen actions plus five debug keys share one board, and a duplicate is
/// silent: both actions fire, and it reads as the wrong one being bound.
/// </para>
/// </summary>
public class ShadeKeyboardDefaultsTests
{
    [Theory]
    [InlineData(ShadeAction.MoveLeft, KeyCode.LeftArrow)]
    [InlineData(ShadeAction.MoveRight, KeyCode.RightArrow)]
    [InlineData(ShadeAction.MoveUp, KeyCode.UpArrow)]
    [InlineData(ShadeAction.MoveDown, KeyCode.DownArrow)]
    [InlineData(ShadeAction.NailDown, KeyCode.Z)]   // Jump, and the Shade's down slash.
    [InlineData(ShadeAction.Nail, KeyCode.X)]       // Attack.
    [InlineData(ShadeAction.Focus, KeyCode.A)]      // Focus / Cast.
    [InlineData(ShadeAction.Fire, KeyCode.F)]       // Quick Cast.
    [InlineData(ShadeAction.Sprint, KeyCode.C)]     // Dash.
    [InlineData(ShadeAction.Teleport, KeyCode.D)]   // Dream Nail's key, which we warp on.
    [InlineData(ShadeAction.NailUp, KeyCode.S)]     // Super Dash's key, which we have no use for.
    [InlineData(ShadeAction.SwapCharacter, KeyCode.V)]
    public void DefaultsAreHollowKnightsOwnKeyboardTable(ShadeAction action, KeyCode expected)
    {
        var binding = ShadeInputConfig.CreateDefault().GetBinding(action);

        Assert.Equal(ShadeBindingOptionType.Key, binding.primary.type);
        Assert.Equal(expected, binding.primary.key);
    }

    [Fact]
    public void MovingOntoTheKeyboardGivesTheSameKeysAsAFreshConfig()
    {
        var fresh = ShadeInputConfig.CreateDefault();

        var moved = ShadeInputConfig.CreateDefault();
        moved.ApplyControllerLayout(deviceIndex: 1);
        moved.ApplyKeyboardLayout();

        foreach (var action in ShadeInputConfig.AllActions)
        {
            var expected = fresh.GetBinding(action).primary;
            var actual = moved.GetBinding(action).primary;

            if (expected.type != ShadeBindingOptionType.Key)
            {
                continue;
            }

            Assert.Equal(expected.type, actual.type);
            Assert.Equal(expected.key, actual.key);
        }
    }

    [Fact]
    public void NoKeyAnswersToTwoActions()
    {
        var config = ShadeInputConfig.CreateDefault();
        var seen = new Dictionary<KeyCode, ShadeAction>();

        foreach (var action in ShadeInputConfig.AllActions)
        {
            var binding = config.GetBinding(action);
            foreach (var option in new[] { binding.primary, binding.secondary })
            {
                if (option.type != ShadeBindingOptionType.Key)
                {
                    continue;
                }

                Assert.False(
                    seen.TryGetValue(option.key, out var owner),
                    $"{action} and {owner} both default to {option.key}.");
                seen[option.key] = action;
            }
        }
    }
}
