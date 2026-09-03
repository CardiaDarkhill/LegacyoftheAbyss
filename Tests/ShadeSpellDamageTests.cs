using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// The spells against Hollow Knight's own numbers.
/// <para>
/// Every case here is a figure from the wiki. The pieces matter as much as the totals: several of
/// these spells land in more than one hit, and the per-hit figure being mistaken for the whole
/// spell is what produced a report of Howling Wraiths hitting for twice what the log claimed.
/// </para>
/// </summary>
public class ShadeSpellDamageTests
{
    private static int Hit(int hollowKnightDamage) => ShadeSpellDamage.PerHit(hollowKnightDamage, 1f, 1f);

    [Fact]
    public void VengefulSpiritAndShadeSoulMatchTheWiki()
    {
        Assert.Equal(15, Hit(ShadeSpellDamage.VengefulSpirit));
        Assert.Equal(30, Hit(ShadeSpellDamage.ShadeSoul));
    }

    [Fact]
    public void HowlingWraithsIsThirteenThreeTimes()
    {
        Assert.Equal(13, Hit(ShadeSpellDamage.HowlingWraithsPerHit));
        Assert.Equal(3, ShadeSpellDamage.HowlingWraithsHits);
        Assert.Equal(39, Hit(ShadeSpellDamage.HowlingWraithsPerHit) * ShadeSpellDamage.HowlingWraithsHits);
    }

    [Fact]
    public void AbyssShriekIsTwentyFourTimes()
    {
        Assert.Equal(20, Hit(ShadeSpellDamage.AbyssShriekPerHit));
        Assert.Equal(4, ShadeSpellDamage.AbyssShriekHits);
        Assert.Equal(80, Hit(ShadeSpellDamage.AbyssShriekPerHit) * ShadeSpellDamage.AbyssShriekHits);
    }

    [Fact]
    public void DesolateDiveIsFifteenPlusTwenty()
    {
        int dive = Hit(ShadeSpellDamage.QuakeDive);
        int shockwave = Hit(ShadeSpellDamage.DesolateDiveShockwave);

        Assert.Equal(15, dive);
        Assert.Equal(20, shockwave);
        Assert.Equal(35, dive + shockwave);
    }

    [Fact]
    public void DescendingDarkLandsInsideItsRange()
    {
        // The wiki gives 60 to 65 depending on which side of the first burst connects. The bursts
        // are taken together rather than split by side, so the total is one figure in that band.
        int total = Hit(ShadeSpellDamage.QuakeDive) + Hit(ShadeSpellDamage.DescendingDarkBursts);

        Assert.InRange(total, 60, 65);
    }

    [Fact]
    public void AnUpgradedSpellIsWorthMoreThanTheOneItReplaces()
    {
        Assert.True(ShadeSpellDamage.ShadeSoul > ShadeSpellDamage.VengefulSpirit);
        Assert.True(
            ShadeSpellDamage.AbyssShriekPerHit * ShadeSpellDamage.AbyssShriekHits >
            ShadeSpellDamage.HowlingWraithsPerHit * ShadeSpellDamage.HowlingWraithsHits);
        Assert.True(
            ShadeSpellDamage.QuakeDive + ShadeSpellDamage.DescendingDarkBursts >
            ShadeSpellDamage.QuakeDive + ShadeSpellDamage.DesolateDiveShockwave);
    }

    [Fact]
    public void NothingHereScalesWithTheNail()
    {
        // The regression this file exists for. These were once stated as multiples of Hornet's
        // needle, on the reasoning that a fixed figure falls behind her upgrades - but the Knight's
        // spells upgrade in their own right and its spell charms are far stronger than her
        // equivalents, so scaling compounded all three. At a late-game needle it put Abyss Shriek
        // high enough to kill the final boss in three casts. PerHit takes no nail at all now, and
        // this asserts the whole spell stays inside what Hollow Knight itself deals.
        int shriek = Hit(ShadeSpellDamage.AbyssShriekPerHit) * ShadeSpellDamage.AbyssShriekHits;

        Assert.Equal(80, shriek);
    }

    [Theory]
    [InlineData(2f, 1f, 30)]
    [InlineData(1f, 0.5f, 8)]
    [InlineData(1.3f, 1f, 20)]
    public void CharmAndDifficultyMultipliersBothApply(float charm, float config, int expected)
    {
        // Shaman Stone and the difficulty presets are meant to move these. It is only the nail that
        // does not enter into it - a 1.3 charm on Vengeful Spirit's 15 is Hollow Knight's own step
        // from 15 to about 20.
        Assert.Equal(expected, ShadeSpellDamage.PerHit(ShadeSpellDamage.VengefulSpirit, charm, config));
    }

    [Fact]
    public void ASpellNeverDealsNothing()
    {
        // A difficulty preset that scales spells right down still has to leave an attack that does
        // something, or the spell reads as broken rather than weak.
        Assert.Equal(1, ShadeSpellDamage.PerHit(ShadeSpellDamage.VengefulSpirit, 0f, 0f));
        Assert.Equal(1, ShadeSpellDamage.PerHit(ShadeSpellDamage.VengefulSpirit, 1f, 0.001f));
    }
}
