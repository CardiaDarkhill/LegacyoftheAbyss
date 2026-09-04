using System;
using LegacyoftheAbyss.Shade;
using Xunit;

public class ShadeCharmLoadoutTests
{
    [Fact]
    public void BuildSnapshotUsesBaselineWhenLoadoutEmpty()
    {
        var baseline = ShadeCharmStatBaseline.CreateDefault();

        var snapshot = ShadeCharmCalculator.BuildSnapshot(baseline, Array.Empty<ShadeCharmDefinition>());

        Assert.Equal(baseline.MoveSpeed, snapshot.MoveSpeed);
        Assert.Equal(baseline.SprintMultiplier, snapshot.SprintMultiplier);
        Assert.Equal(baseline.FireCooldown, snapshot.FireCooldown);
        Assert.Equal(baseline.ProjectileSoulCost, snapshot.ProjectileSoulCost);
        Assert.Equal(baseline.ShadeSoulCapacity, snapshot.ShadeSoulCapacity);
        Assert.Empty(snapshot.Definitions);
        Assert.Null(snapshot.AbilityOverrides.EnableProjectile);
        Assert.Null(snapshot.AbilityOverrides.EnableShriek);
    }

    /// <summary>
    /// Hollow Knight's own nail figures, which are the point of the two-timer gate: without a
    /// separate duration, Quick Slash could only shorten one of them and the charm read as inert
    /// against a stopwatch.
    /// </summary>
    [Fact]
    public void TheNailKeepsHollowKnightsTwoTimings()
    {
        var baseline = ShadeCharmStatBaseline.CreateDefault();

        Assert.Equal(0.41f, baseline.NailCooldown, 3);
        Assert.Equal(0.35f, baseline.NailDuration, 3);

        var quickSlash = new ShadeCharmDefinition(
            "quickSlash",
            statModifiers: new ShadeCharmStatModifiers
            {
                NailCooldownMultiplier = 0.25f / 0.41f,
                NailDurationMultiplier = 0.28f / 0.35f
            });

        var snapshot = ShadeCharmCalculator.BuildSnapshot(baseline, new[] { quickSlash });

        Assert.Equal(0.25f, snapshot.NailCooldown, 3);
        Assert.Equal(0.28f, snapshot.NailDuration, 3);

        // The strike is refused until both have run out, so the rate is set by the longer one:
        // 0.41s unaided, 0.28s with the charm - a bit over a third faster.
        float unaided = MathF.Max(baseline.NailCooldown, baseline.NailDuration);
        float aided = MathF.Max(snapshot.NailCooldown, snapshot.NailDuration);
        Assert.Equal(0.41f, unaided, 3);
        Assert.Equal(0.28f, aided, 3);
    }

    [Fact]
    public void StatModifiersStackMultiplicativelyAndAdditively()
    {
        var baseline = ShadeCharmStatBaseline.CreateDefault();

        var speedCharm = new ShadeCharmDefinition(
            "speed",
            new ShadeCharmStatModifiers
            {
                MoveSpeedMultiplier = 1.2f,
                MoveSpeedFlatBonus = 1f,
                SprintSpeedFlatBonus = 0.4f,
                FireCooldownFlatDelta = -0.05f,
                ProjectileSoulCostFlatDelta = -3,
                ShadeSoulCapacityFlatBonus = 10
            });

        var cooldownCharm = new ShadeCharmDefinition(
            "cooldowns",
            new ShadeCharmStatModifiers
            {
                FireCooldownMultiplier = 0.5f,
                ProjectileSoulCostMultiplier = 0.5f,
                SprintDashCooldownMultiplier = 0.8f,
                SprintDashCooldownFlatDelta = -0.1f,
                FocusSoulCostMultiplier = 0.5f
            });

        var snapshot = ShadeCharmCalculator.BuildSnapshot(baseline, new[] { speedCharm, cooldownCharm });

        Assert.Equal(13f, snapshot.MoveSpeed, 3);
        Assert.Equal(2.9f, snapshot.SprintMultiplier, 3);
        Assert.Equal(0.7f, snapshot.SprintDashCooldown, 3);
        Assert.Equal(0.075f, snapshot.FireCooldown, 3);
        Assert.Equal(14, snapshot.ProjectileSoulCost);
        Assert.Equal(17, snapshot.FocusSoulCost);
        Assert.Equal(109, snapshot.ShadeSoulCapacity);
    }

    [Fact]
    public void AbilityTogglesRespectLatestOverride()
    {
        var baseline = ShadeCharmStatBaseline.CreateDefault();

        var disableCharm = new ShadeCharmDefinition(
            "disable",
            abilityToggles: new ShadeCharmAbilityToggles
            {
                EnableProjectile = false,
                EnableShriek = false
            });

        var enableShriek = new ShadeCharmDefinition(
            "enable",
            abilityToggles: new ShadeCharmAbilityToggles
            {
                EnableShriek = true
            });

        var snapshot = ShadeCharmCalculator.BuildSnapshot(baseline, new[] { disableCharm, enableShriek });

        Assert.Equal(2, snapshot.Definitions.Count);
        Assert.True(snapshot.AbilityOverrides.EnableProjectile.HasValue);
        Assert.False(snapshot.AbilityOverrides.EnableProjectile!.Value);
        Assert.True(snapshot.AbilityOverrides.EnableShriek.HasValue);
        Assert.True(snapshot.AbilityOverrides.EnableShriek!.Value);
    }

    /// <summary>
    /// Stepping the mask fraction option resizes the live Shade at every step, and the option's
    /// list wraps through "Always 1". Without a paused baseline to refill from, a player cycling
    /// back to the setting they started on was left permanently on 1 health.
    /// </summary>
    [Theory]
    // Unpaused: a resize never heals, whatever the Shade has lost.
    [InlineData(1, -1, 0)]
    [InlineData(5, -1, 0)]
    // Paused: put back exactly what the earlier steps of this menu visit clamped away.
    [InlineData(1, 5, 4)]
    [InlineData(4, 5, 1)]
    // Nothing to put back, and never a negative fill when the Shade is above its baseline.
    [InlineData(5, 5, 0)]
    [InlineData(6, 5, 0)]
    public void AResizeOnlyRefillsWhatThePauseMenuTookAway(int currentHealth, int pausedBaseline, int expected)
    {
        Assert.Equal(
            expected,
            LegacyHelper.ShadeController.ResolveResizeRefill(currentHealth, pausedBaseline));
    }

    /// <summary>
    /// Fragile Heart healed the companion in full on every room transition.
    /// <para>
    /// The charm's loadout is rebuilt from baseline on every charm change <em>and</em> every scene
    /// change, and the companion is respawned on every scene change too - so its OnApplied runs
    /// again with the bonus back at zero and a controller that has never seen the charm before.
    /// Nothing on the controller can tell that from the player putting the charm on. The fill was
    /// "top up to the new maximum", so every one of those rebuilds restored the companion to full:
    /// a one-notch charm made it unkillable to anything that could not empty its health inside a
    /// single room.
    /// </para>
    /// <para>
    /// The tie is broken by the maximum the companion already has, which is restored from the save
    /// with the charm's masks already counted - so the answer does not depend on spotting a fresh
    /// equip at all.
    /// </para>
    /// </summary>
    [Theory]
    // Put on mid-run: the maximum standing is the one without the charm, so its two masks arrive
    // filled - and nothing beyond them does.
    [InlineData(5, 7, 5, 2)]
    // The rebuild after a scene change or a reload. The restored maximum already counts the charm,
    // so there is nothing left to hand back however many times this runs.
    [InlineData(5, 7, 7, 0)]
    // Taken off. The maximum falls; ApplyCharmHealthModifiers clamps into it and no fill is owed.
    [InlineData(7, 5, 7, 0)]
    // Joni's converts the maximum to lifeblood, so the same call arrives carrying capacities.
    [InlineData(7, 10, 7, 3)]
    [InlineData(7, 10, 10, 0)]
    public void AMaxHealthCharmOnlyFillsWhatTheStandingMaximumDoesNotCover(
        int previousMax, int newMax, int currentMax, int expected)
    {
        Assert.Equal(
            expected,
            LegacyHelper.ShadeController.ResolveMaxHpFill(previousMax, newMax, currentMax));
    }
}
