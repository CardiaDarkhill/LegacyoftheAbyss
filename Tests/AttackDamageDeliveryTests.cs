using Xunit;

using Occupancy = LegacyHelper.ShadeGrabRetargeting.Occupancy;

/// <summary>
/// The two questions that decide where an attack's damage ends up, and the mistake each one guards.
/// <para>
/// Both have been got wrong on this project in opposite directions within a day. Attributing a
/// damager by walking up the hierarchy fed the Shade a boss's body-contact damage every time it
/// touched any child trigger, so telegraphs hurt it. Fixing that to match <c>HeroBox</c> then left the
/// Shade completely immune to attacks whose hitbox carries no damage component at all - Lace's cross
/// slash damages the hero by calling <c>HeroController</c> from an FSM, and its hitbox is a bare
/// trigger. The Shade had been receiving those only as a side effect of the first bug.
/// </para>
/// </summary>
public class AttackDamageDeliveryTests
{
    private static Occupancy Neither => new Occupancy(hornetInside: false, shadeInside: false);
    private static Occupancy HornetOnly => new Occupancy(hornetInside: true, shadeInside: false);
    private static Occupancy ShadeOnly => new Occupancy(hornetInside: false, shadeInside: true);
    private static Occupancy Both => new Occupancy(hornetInside: true, shadeInside: true);

    /// <summary>
    /// The Shade's share is decided by whether it is in the attack, and by nothing about Hornet.
    /// </summary>
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void TheShadeTakesAHitWheneverItIsInTheAttack(bool hornetInside, bool shadeInside, bool expected)
    {
        Assert.Equal(expected, LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(new Occupancy(hornetInside, shadeInside)));
    }

    /// <summary>
    /// The regression this file is named for: the Shade in an attack Hornet is not in must still be
    /// hit by it. Immunity is not an acceptable reading of "Hornet is spared".
    /// </summary>
    [Fact]
    public void SparingHornetDoesNotMakeTheShadeImmune()
    {
        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldSpareHornet(ShadeOnly));
        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(ShadeOnly));
    }

    [Fact]
    public void BothInTheAttackIsNotATie()
    {
        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(Both));
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldSpareHornet(Both));
    }

    [Fact]
    public void AnAttackNobodyIsStandingInHitsNobody()
    {
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(Neither));
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldMoveShadeInstead(Neither));
    }

    [Fact]
    public void HornetAloneKeepsVanillaBehaviour()
    {
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldSpareHornet(HornetOnly));
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(HornetOnly));
    }

    /// <summary>
    /// A reading that could not be taken is not a reading that she is clear of the attack.
    /// <para>
    /// Both of those arrive as <c>HornetInside == false</c>, and for as long as that was all there
    /// was to go on, a hurtbox that could not be found or was switched off spared her every hit the
    /// Shade was standing in. Failing towards her taking damage is the only acceptable direction:
    /// a hit she should have dodged is a moment, a hit she can never take is the game.
    /// </para>
    /// </summary>
    [Fact]
    public void AnUnmeasurableHornetIsNotASparedHornet()
    {
        var unmeasurable = new Occupancy(hornetInside: false, shadeInside: true, hornetMeasurable: false);

        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldSpareHornet(unmeasurable));
        Assert.False(LegacyHelper.ShadeGrabRetargeting.ShouldMoveShadeInstead(unmeasurable));

        // The Shade's own share is measured on its own and is unaffected by any of it.
        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldShadeTakeHit(unmeasurable));
    }

    /// <summary>
    /// A measured Hornet still behaves exactly as before, so the guard above cannot be mistaken for
    /// having switched the feature off.
    /// </summary>
    [Fact]
    public void AMeasuredHornetOutsideTheAttackIsStillSpared()
    {
        var measured = new Occupancy(hornetInside: false, shadeInside: true, hornetMeasurable: true);

        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldSpareHornet(measured));
        Assert.True(LegacyHelper.ShadeGrabRetargeting.ShouldMoveShadeInstead(measured));
    }

    /// <summary>
    /// A null object carries no damage of its own, so a hit from one is always the Shade's to be
    /// given by hand. Guards the branch that decides between "give it the hit" and "it already has
    /// one" - getting that backwards is double damage in one direction and immunity in the other.
    /// </summary>
    [Fact]
    public void NothingCarriesItsOwnDamageWhenThereIsNoObject()
    {
        Assert.False(LegacyHelper.ShadeController.CarriesItsOwnDamage(null));
    }

    /// <summary>
    /// An attack that names no amount still has to deal one, or intercepting it silently removes the
    /// hit altogether.
    /// </summary>
    [Fact]
    public void AnUnknownAttackStillDealsAMask()
    {
        Assert.Equal(1, LegacyHelper.ShadeController.ResolveAttackDamage(null));
    }
}
