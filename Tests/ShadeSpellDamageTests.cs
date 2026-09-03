using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// The spells against Hollow Knight's own numbers, bare and under Shaman Stone.
/// <para>
/// Every case here is a figure from the wiki. The pieces matter as much as the totals: several of
/// these spells land in more than one hit, and the per-hit figure being mistaken for the whole
/// spell is what produced a report of Howling Wraiths hitting for twice what the log claimed.
/// </para>
/// </summary>
public class ShadeSpellDamageTests
{
    private static int Bare(ShadeSpellDamage.SpellHit hit) => hit.Resolve(shamanStoneEquipped: false, configMultiplier: 1f);

    private static int Shaman(ShadeSpellDamage.SpellHit hit) => hit.Resolve(shamanStoneEquipped: true, configMultiplier: 1f);

    [Fact]
    public void VengefulSpiritAndShadeSoulMatchTheWiki()
    {
        Assert.Equal(15, Bare(ShadeSpellDamage.VengefulSpirit));
        Assert.Equal(30, Bare(ShadeSpellDamage.ShadeSoul));
    }

    [Fact]
    public void HowlingWraithsIsThirteenThreeTimes()
    {
        Assert.Equal(13, Bare(ShadeSpellDamage.HowlingWraiths));
        Assert.Equal(3, ShadeSpellDamage.HowlingWraithsHits);
        Assert.Equal(39, Bare(ShadeSpellDamage.HowlingWraiths) * ShadeSpellDamage.HowlingWraithsHits);
    }

    [Fact]
    public void AbyssShriekIsTwentyFourTimes()
    {
        Assert.Equal(20, Bare(ShadeSpellDamage.AbyssShriek));
        Assert.Equal(4, ShadeSpellDamage.AbyssShriekHits);
        Assert.Equal(80, Bare(ShadeSpellDamage.AbyssShriek) * ShadeSpellDamage.AbyssShriekHits);
    }

    [Fact]
    public void DesolateDiveIsFifteenPlusTwenty()
    {
        Assert.Equal(15, Bare(ShadeSpellDamage.DesolateDiveImpact));
        Assert.Equal(20, Bare(ShadeSpellDamage.DesolateDiveShockwave));
        Assert.Equal(35, Bare(ShadeSpellDamage.DesolateDiveImpact) + Bare(ShadeSpellDamage.DesolateDiveShockwave));
    }

    [Fact]
    public void DescendingDarkLandsInsideItsRange()
    {
        // The wiki gives 60 to 65 depending on which side of the first burst connects. The bursts
        // are taken together rather than split by side, so the total is one figure in that band.
        int total = Bare(ShadeSpellDamage.DescendingDarkImpact) + Bare(ShadeSpellDamage.DescendingDarkBursts);

        Assert.InRange(total, 60, 65);
    }

    // --- Shaman Stone, which Hollow Knight sets per spell rather than once --------------------

    [Fact]
    public void ShamanStoneAddsAThirdToTheProjectiles()
    {
        Assert.Equal(20, Shaman(ShadeSpellDamage.VengefulSpirit));
        Assert.Equal(40, Shaman(ShadeSpellDamage.ShadeSoul));
    }

    [Fact]
    public void ShamanStoneAddsAHalfToTheScreams()
    {
        Assert.Equal(60, Shaman(ShadeSpellDamage.HowlingWraiths) * ShadeSpellDamage.HowlingWraithsHits);
        Assert.Equal(120, Shaman(ShadeSpellDamage.AbyssShriek) * ShadeSpellDamage.AbyssShriekHits);
    }

    [Fact]
    public void ShamanStoneTakesDesolateDiveToFiftyThree()
    {
        // The one figure the wiki gives outright for a quake spell with the charm on.
        Assert.Equal(
            53,
            Shaman(ShadeSpellDamage.DesolateDiveImpact) + Shaman(ShadeSpellDamage.DesolateDiveShockwave));
    }

    [Fact]
    public void ShamanStoneAddsFortySevenPercentToDescendingDark()
    {
        int bare = Bare(ShadeSpellDamage.DescendingDarkImpact) + Bare(ShadeSpellDamage.DescendingDarkBursts);
        int shaman = Shaman(ShadeSpellDamage.DescendingDarkImpact) + Shaman(ShadeSpellDamage.DescendingDarkBursts);

        // The wiki states the increase rather than the total, so this checks the increase.
        Assert.InRange(shaman / (float)bare, 1.46f, 1.48f);
    }

    [Fact]
    public void TheChargeIsNotOneFigureForEverySpell()
    {
        // The regression this guards: a single shared multiplier. Hollow Knight's increase differs
        // per spell, and the projectiles get the smallest of them - so if these ever come out equal,
        // something has gone back to multiplying every spell by the same number.
        float projectiles = Shaman(ShadeSpellDamage.VengefulSpirit) / (float)Bare(ShadeSpellDamage.VengefulSpirit);
        float screams = Shaman(ShadeSpellDamage.AbyssShriek) / (float)Bare(ShadeSpellDamage.AbyssShriek);

        Assert.True(projectiles < screams);
    }

    [Fact]
    public void AnUpgradedSpellIsWorthMoreThanTheOneItReplaces()
    {
        Assert.True(Bare(ShadeSpellDamage.ShadeSoul) > Bare(ShadeSpellDamage.VengefulSpirit));
        Assert.True(
            Bare(ShadeSpellDamage.AbyssShriek) * ShadeSpellDamage.AbyssShriekHits >
            Bare(ShadeSpellDamage.HowlingWraiths) * ShadeSpellDamage.HowlingWraithsHits);
        Assert.True(
            Bare(ShadeSpellDamage.DescendingDarkImpact) + Bare(ShadeSpellDamage.DescendingDarkBursts) >
            Bare(ShadeSpellDamage.DesolateDiveImpact) + Bare(ShadeSpellDamage.DesolateDiveShockwave));
    }

    [Fact]
    public void NothingHereScalesWithTheNail()
    {
        // The regression this file exists for. These were once stated as multiples of Hornet's
        // needle, on the reasoning that a fixed figure falls behind her upgrades - but the Knight's
        // spells upgrade in their own right and its spell charms are far stronger than her
        // equivalents, so scaling compounded all three. At a late-game needle it put Abyss Shriek
        // high enough to kill the final boss in three casts. Resolve takes no nail at all now.
        Assert.Equal(80, Bare(ShadeSpellDamage.AbyssShriek) * ShadeSpellDamage.AbyssShriekHits);
    }

    [Theory]
    [InlineData(0.5f, 8)]
    [InlineData(1f, 15)]
    [InlineData(2f, 30)]
    public void TheDifficultyMultiplierStillScalesASpell(float config, int expected)
    {
        Assert.Equal(expected, ShadeSpellDamage.VengefulSpirit.Resolve(shamanStoneEquipped: false, config));
    }

    [Fact]
    public void ASpellNeverDealsNothing()
    {
        // A difficulty preset that scales spells right down still has to leave an attack that does
        // something, or the spell reads as broken rather than weak.
        Assert.Equal(1, ShadeSpellDamage.VengefulSpirit.Resolve(shamanStoneEquipped: false, 0f));
        Assert.Equal(1, ShadeSpellDamage.VengefulSpirit.Resolve(shamanStoneEquipped: false, 0.001f));
    }
}
