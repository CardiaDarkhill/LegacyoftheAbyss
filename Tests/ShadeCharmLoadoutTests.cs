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
}
