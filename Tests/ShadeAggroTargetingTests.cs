using Xunit;

/// <summary>
/// Covers the "should this enemy come after the Shade instead of Hornet?" comparison.
/// <para>
/// Only <see cref="ShadeAggroTargeting.PreferShade"/> is exercised here - the surrounding service
/// needs <c>Time.time</c>, a live <c>HeroController</c> and a <c>ConditionalWeakTable</c> keyed on
/// real GameObjects, none of which exist in a plain test host. The comparison is where the actual
/// behaviour lives, though: the latch around it only controls how often it is asked.
/// </para>
/// </summary>
public class ShadeAggroTargetingTests
{
    private const float Margin = 0.2f;

    [Fact]
    public void TakesTheShadeWhenItIsClearlyCloser()
    {
        Assert.True(ShadeAggroTargeting.PreferShade(
            hornetDistance: 10f, shadeDistance: 7f, currentlyTargetingShade: false, switchMargin: Margin));
    }

    [Fact]
    public void IgnoresTheShadeWhenHornetIsCloser()
    {
        Assert.False(ShadeAggroTargeting.PreferShade(
            hornetDistance: 7f, shadeDistance: 10f, currentlyTargetingShade: false, switchMargin: Margin));
    }

    /// <summary>
    /// The dead band. An enemy standing almost equidistant between the two must not start chasing the
    /// Shade on a marginal difference, or it will hand the decision straight back the moment either
    /// of them moves.
    /// </summary>
    [Theory]
    [InlineData(10f, 9f)]
    [InlineData(10f, 8.5f)]
    [InlineData(10f, 10f)]
    public void DoesNotSwitchToTheShadeOnAMarginalDifference(float hornetDistance, float shadeDistance)
    {
        Assert.False(ShadeAggroTargeting.PreferShade(
            hornetDistance, shadeDistance, currentlyTargetingShade: false, switchMargin: Margin));
    }

    /// <summary>
    /// The other side of the dead band: once committed to the Shade, an enemy holds that target
    /// through the Shade drifting somewhat further away than Hornet, so the two states are not
    /// symmetrical and cannot oscillate.
    /// </summary>
    [Theory]
    [InlineData(10f, 11f)]
    [InlineData(10f, 12f)]
    [InlineData(10f, 10f)]
    public void KeepsTheShadeWhileItIsOnlySlightlyFurther(float hornetDistance, float shadeDistance)
    {
        Assert.True(ShadeAggroTargeting.PreferShade(
            hornetDistance, shadeDistance, currentlyTargetingShade: true, switchMargin: Margin));
    }

    [Fact]
    public void GivesTheShadeUpOnceItIsClearlyFurther()
    {
        Assert.False(ShadeAggroTargeting.PreferShade(
            hornetDistance: 10f, shadeDistance: 13f, currentlyTargetingShade: true, switchMargin: Margin));
    }

    /// <summary>
    /// Whatever the distances, the two directions must never both say "switch" - that is exactly the
    /// per-frame flip-flop the hysteresis exists to prevent.
    /// </summary>
    [Theory]
    [InlineData(10f, 1f)]
    [InlineData(10f, 8f)]
    [InlineData(10f, 10f)]
    [InlineData(10f, 12f)]
    [InlineData(10f, 40f)]
    [InlineData(0f, 0f)]
    public void DecisionIsStableForAnyDistancePair(float hornetDistance, float shadeDistance)
    {
        bool takesIt = ShadeAggroTargeting.PreferShade(hornetDistance, shadeDistance, false, Margin);
        bool keepsIt = ShadeAggroTargeting.PreferShade(hornetDistance, shadeDistance, true, Margin);

        // "Would start chasing it" must imply "would carry on chasing it".
        Assert.True(!takesIt || keepsIt);
    }
}
