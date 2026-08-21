using System;
using Xunit;

/// <summary>
/// The Shade carries a child collider that copies Hornet's layer and tag so enemies notice it, which
/// also makes it indistinguishable from Hornet to every <c>TrackTriggerObjects</c>-derived range. The
/// carve-out works through <c>IsCounted</c>, so the Shade stays registered in the range (aggro keeps
/// working) but stops being counted by <c>InsideCount</c>/<c>IsInside</c> - which is what the updraft
/// FSM, bench work ranges, pickup triggers and the rest actually read.
/// <para>
/// This pins down the one range type that must keep counting the Shade, and that everything else
/// does not.
/// </para>
/// </summary>
public class ShadeRegionExclusionTests
{
    private static bool CountsTheShade(Type regionType)
        => LegacyHelper.TrackTriggerObjects_IsCounted_IgnoreShade.CountsTheShade(regionType);

    /// <summary>
    /// AlertRange is the entire reason the Shade's aggro proxy exists, and enemy FSMs read it through
    /// <c>CheckTrackTriggerCount</c> -> <c>InsideCount</c>. If it ever stops counting the Shade,
    /// enemies stop noticing the companion - which is exactly what happened when an earlier version
    /// of this fix filtered the trigger callbacks instead.
    /// </summary>
    [Fact]
    public void AlertRangeStillCountsTheShade()
    {
        Assert.True(CountsTheShade(typeof(AlertRange)));
    }

    [Theory]
    [InlineData(typeof(TrackTriggerObjects))]
    [InlineData(typeof(TrackTriggerObjectsLineOfSight))]
    [InlineData(typeof(WindRegion))]
    [InlineData(typeof(UmbrellaWindRegion))]
    [InlineData(typeof(WindCameraRegion))]
    [InlineData(typeof(FrostRegion))]
    [InlineData(typeof(DarknessRegion))]
    [InlineData(typeof(AtmosRegion))]
    [InlineData(typeof(MusicRegion))]
    [InlineData(typeof(CameraLockArea))]
    [InlineData(typeof(NoClamberRegion))]
    [InlineData(typeof(NoWallClingRegion))]
    [InlineData(typeof(WorldRumbleArea))]
    [InlineData(typeof(CurrencyCounterAppearRegion))]
    [InlineData(typeof(TriggerActivateGameObject))]
    public void HeroStateRangesDoNotCountTheShade(Type regionType)
    {
        Assert.False(CountsTheShade(regionType));
    }

    /// <summary>
    /// A plain <c>TrackTriggerObjects</c> is the type the updraft FSM polls - it is not a
    /// <c>WindRegion</c>, which is why filtering by named region types never fixed the updraft.
    /// </summary>
    [Fact]
    public void PlainTrackTriggerObjectsDoesNotCountTheShade()
    {
        Assert.False(CountsTheShade(typeof(TrackTriggerObjects)));
    }

    [Fact]
    public void NullTypeCountsNothing()
    {
        Assert.False(CountsTheShade(null));
    }
}
