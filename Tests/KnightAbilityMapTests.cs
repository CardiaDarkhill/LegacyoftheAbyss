#nullable enable

using LegacyoftheAbyss.Shade.Knight;
using Xunit;

/// <summary>
/// The Hornet-to-Knight unlock table, which follows Knight in Silksong's own sync config. These
/// pin the gates the brief named explicitly, so a retune cannot quietly move them.
/// </summary>
public class KnightAbilityMapTests
{
    private static KnightAbilities Map(HornetProgressSnapshot snapshot)
        => KnightAbilityMap.FromHornet(snapshot);

    [Fact]
    public void NothingUnlockedByDefault()
    {
        var abilities = Map(default);

        Assert.False(abilities.MothwingCloak);
        Assert.False(abilities.MantisClaw);
        Assert.False(abilities.DoubleJump);
        Assert.False(abilities.ShadeCloak);
        Assert.False(abilities.CanDash);
        Assert.Equal(0, abilities.FireballLevel);
        Assert.Equal(0, abilities.QuakeLevel);
        Assert.Equal(0, abilities.ScreamLevel);
    }

    [Fact]
    public void MothwingCloakArrivesWithHornetsSprint()
    {
        var abilities = Map(new HornetProgressSnapshot { HasDash = true });

        Assert.True(abilities.MothwingCloak);
        Assert.True(abilities.CanDash);
    }

    [Fact]
    public void MantisClawArrivesWithHornetsWallClimb()
    {
        Assert.True(Map(new HornetProgressSnapshot { HasWalljump = true }).MantisClaw);
        Assert.False(Map(new HornetProgressSnapshot { HasDash = true }).MantisClaw);
    }

    [Fact]
    public void DoubleJumpArrivesWithHornets()
    {
        Assert.True(Map(new HornetProgressSnapshot { HasDoubleJump = true }).DoubleJump);
    }

    /// <summary>
    /// Shade Cloak follows Harpoon Dash rather than the start of Act 3 - the decision recorded for
    /// this branch, and what Knight in Silksong itself does.
    /// </summary>
    [Fact]
    public void ShadeCloakFollowsHarpoonDashNotSprint()
    {
        Assert.False(Map(new HornetProgressSnapshot { HasDash = true }).ShadeCloak);

        var withHarpoon = Map(new HornetProgressSnapshot { HasHarpoonDash = true });
        Assert.True(withHarpoon.ShadeCloak);
        Assert.True(withHarpoon.CanDash);
    }

    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 2)]
    [InlineData(true, true, 2)]
    public void FireballLevelsFromNeedleThrowThenSilkCharge(bool needleThrow, bool silkCharge, int expected)
    {
        var abilities = Map(new HornetProgressSnapshot
        {
            HasNeedleThrow = needleThrow,
            HasSilkCharge = silkCharge,
        });

        Assert.Equal(expected, abilities.FireballLevel);
    }

    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 2)]
    [InlineData(true, true, 2)]
    public void QuakeLevelsFromParryThenBossNeedle(bool parry, bool bossNeedle, int expected)
    {
        var abilities = Map(new HornetProgressSnapshot
        {
            HasParry = parry,
            HasSilkBossNeedle = bossNeedle,
        });

        Assert.Equal(expected, abilities.QuakeLevel);
    }

    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(true, false, 1)]
    [InlineData(false, true, 2)]
    [InlineData(true, true, 2)]
    public void ScreamLevelsFromThreadSphereThenSilkBomb(bool threadSphere, bool silkBomb, int expected)
    {
        var abilities = Map(new HornetProgressSnapshot
        {
            HasThreadSphere = threadSphere,
            HasSilkBomb = silkBomb,
        });

        Assert.Equal(expected, abilities.ScreamLevel);
    }

    /// <summary>A fully-progressed Hornet must leave nothing behind on the Knight.</summary>
    [Fact]
    public void EverythingUnlockedWhenHornetHasEverything()
    {
        var abilities = Map(new HornetProgressSnapshot
        {
            HasDash = true,
            HasWalljump = true,
            HasDoubleJump = true,
            HasHarpoonDash = true,
            HasNeedleThrow = true,
            HasSilkCharge = true,
            HasParry = true,
            HasSilkBossNeedle = true,
            HasThreadSphere = true,
            HasSilkBomb = true,
        });

        Assert.True(abilities.MothwingCloak);
        Assert.True(abilities.MantisClaw);
        Assert.True(abilities.DoubleJump);
        Assert.True(abilities.ShadeCloak);
        Assert.Equal(2, abilities.FireballLevel);
        Assert.Equal(2, abilities.QuakeLevel);
        Assert.Equal(2, abilities.ScreamLevel);
    }
}
