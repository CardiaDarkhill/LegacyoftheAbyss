using Xunit;

/// <summary>
/// The clamp on how far a bolt may shift its height to clear uneven ground.
/// <para>
/// Whether a given height is clear needs the physics scene and is not testable here. What is
/// testable is the rule that keeps the trick honest: a bolt that meets a wall must not climb it.
/// </para>
/// </summary>
public class ProjectileTerrainRideTests
{
    private const float Budget = 1.5f;

    [Fact]
    public void OnItsOriginalLineTheWholeBudgetIsAvailableEitherWay()
    {
        Assert.Equal(Budget, LegacyHelper.ShadeProjectile.RideLimit(Budget, 0f, 1), 3);
        Assert.Equal(Budget, LegacyHelper.ShadeProjectile.RideLimit(Budget, 0f, -1), 3);
    }

    [Fact]
    public void ABoltThatHasRidenAllTheWayUpCannotGoHigher()
    {
        // The wall case. Without this it climbs the face a step at a time for as long as the wall
        // lasts, which is the "migrates up the screen" the clamp exists to prevent.
        Assert.Equal(0f, LegacyHelper.ShadeProjectile.RideLimit(Budget, Budget, 1), 3);
    }

    [Fact]
    public void ItCanAlwaysComeBackDownTheWayItCameUp()
    {
        // Measured from where it sits, not from what it has spent: at the top of its budget it may
        // descend the full span to the bottom of it, which is what lets a floor of successive bumps
        // be survivable rather than draining the allowance one lip at a time.
        Assert.Equal(Budget * 2f, LegacyHelper.ShadeProjectile.RideLimit(Budget, Budget, -1), 3);
    }

    [Theory]
    [InlineData(0f, 1)]
    [InlineData(0f, -1)]
    [InlineData(1.5f, -1)]
    [InlineData(-1.5f, 1)]
    [InlineData(0.7f, 1)]
    [InlineData(-0.7f, -1)]
    public void TakingTheWholeLimitNeverStraysFurtherThanTheBudget(float offset, int direction)
    {
        float limit = LegacyHelper.ShadeProjectile.RideLimit(Budget, offset, direction);
        float moved = offset + (direction * limit);

        Assert.InRange(moved, -Budget - 0.001f, Budget + 0.001f);
    }

    [Fact]
    public void ALimitIsNeverNegative()
    {
        // A bolt somehow past its budget asks for nothing rather than for a move back inward, which
        // would read as the terrain pushing it about.
        Assert.Equal(0f, LegacyHelper.ShadeProjectile.RideLimit(Budget, Budget * 3f, 1), 3);
    }
}
