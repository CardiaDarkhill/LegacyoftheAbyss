using System;
using System.Collections.Generic;
using LegacyoftheAbyss.Shade;
using Xunit;

/// <summary>
/// Covers the arithmetic the Shade's rendering options are built on. The Unity-side plumbing
/// (sorting layer assignment, the particle emitter itself) needs a running engine and is not
/// exercised here; what these tests protect is the decision-making that feeds it.
/// </summary>
public class ShadeVisualTuningTests
{
    /// <summary>Silksong's real sorting-layer set, in draw order, read out of globalgamemanagers.</summary>
    private static readonly string[] SilksongLayers =
    {
        "Default", "Far BG 2", "Far BG 1", "Mid BG", "Immediate BG", "Actors", "Player", "Tiles",
        "MID Dressing", "Immediate FG", "Scene Border", "Far FG", "Vignette", "Over", "HUD", "Inventory"
    };

    private static Func<string, bool> Layers(params string[] names)
    {
        var set = new HashSet<string>(names, StringComparer.Ordinal);
        return set.Contains;
    }

    [Fact]
    public void ConfiguredSortingLayerWinsWhenItExists()
    {
        string resolved = ShadeVisualTuning.ResolveSortingLayerName("Actors", Layers(SilksongLayers), "Player");
        Assert.Equal("Actors", resolved);
    }

    [Fact]
    public void BlankSortingLayerFallsBackToHornetsOwnLayer()
    {
        string resolved = ShadeVisualTuning.ResolveSortingLayerName("   ", Layers(SilksongLayers), "Player");
        Assert.Equal("Player", resolved);
    }

    [Fact]
    public void UnknownSortingLayerFallsBackToHornetsOwnLayer()
    {
        // A game update that renames or drops a layer must not leave the Shade on layer 0, which
        // draws behind every piece of scenery.
        string resolved = ShadeVisualTuning.ResolveSortingLayerName("Companions", Layers(SilksongLayers), "Player");
        Assert.Equal("Player", resolved);
    }

    [Fact]
    public void FallsBackToACharacterLayerWhenHornetIsUnavailable()
    {
        // Mid scene load, before her renderer can be read. Better over the scenery than under it.
        string resolved = ShadeVisualTuning.ResolveSortingLayerName(null, Layers(SilksongLayers), null);
        Assert.Equal(ShadeVisualTuning.HeroFallbackSortingLayer, resolved);
    }

    [Fact]
    public void TheShippedDefaultIsToMatchHornetRatherThanToNameALayer()
    {
        // The regression this file gained a case for. "Player" was shipped here, and it sorts above
        // everything on the layer Silksong actually draws Hornet on - so the companion was ordered
        // against the scenery by a different rule than she was, and stood in front of grass she
        // stood behind. Blank is the only value that has the world treat the two alike.
        Assert.Equal(
            "Default",
            ShadeVisualTuning.ResolveSortingLayerName(
                ModConfig.DefaultShadeSortingLayer,
                Layers(SilksongLayers),
                heroLayerName: "Default"));
    }

    [Fact]
    public void FallsBackToUnityDefaultWhenNothingElseResolves()
    {
        string resolved = ShadeVisualTuning.ResolveSortingLayerName("Companions", Layers("Default"), "Player");
        Assert.Equal(ShadeVisualTuning.UnityDefaultSortingLayer, resolved);
    }

    [Fact]
    public void LayerLookupFailureIsTreatedAsMissingRatherThanThrowing()
    {
        string resolved = ShadeVisualTuning.ResolveSortingLayerName(
            "Player",
            _ => throw new InvalidOperationException("SortingLayer unavailable"),
            "Player");
        Assert.Equal(ShadeVisualTuning.UnityDefaultSortingLayer, resolved);
    }

    [Fact]
    public void SortingOrderIsRelativeToHornetOnHerOwnLayer()
    {
        Assert.Equal(6, ShadeVisualTuning.ResolveSortingOrder(5, 1, sharesHeroLayer: true));
        Assert.Equal(4, ShadeVisualTuning.ResolveSortingOrder(5, -1, sharesHeroLayer: true));
    }

    [Fact]
    public void SortingOrderIsAbsoluteOnADifferentLayer()
    {
        // Hornet's order says nothing about ordering within another layer, so the configured value
        // is used as-is rather than being added to an unrelated number.
        Assert.Equal(1, ShadeVisualTuning.ResolveSortingOrder(5, 1, sharesHeroLayer: false));
    }

    [Fact]
    public void SoulFractionClampsAndHandlesZeroCapacity()
    {
        Assert.Equal(0f, ShadeVisualTuning.SoulFraction(0, 99), 4);
        Assert.Equal(0.5f, ShadeVisualTuning.SoulFraction(50, 100), 4);
        Assert.Equal(1f, ShadeVisualTuning.SoulFraction(200, 100), 4);
        Assert.Equal(0f, ShadeVisualTuning.SoulFraction(10, 0), 4);
    }

    [Fact]
    public void EmissionIsNonZeroAtEmptySoulAndDoubledAtFull()
    {
        float empty = ShadeVisualTuning.EmissionRate(0f, 1f);
        float full = ShadeVisualTuning.EmissionRate(1f, 1f);

        // The brief: clearly visible on an empty meter, about twice as intense on a full one.
        Assert.Equal(ShadeVisualTuning.BaseEmissionRate, empty, 4);
        Assert.True(empty > 0f);
        Assert.Equal(empty * 2f, full, 4);
    }

    [Fact]
    public void EmissionScalesLinearlyWithTheUserIntensity()
    {
        Assert.Equal(0f, ShadeVisualTuning.EmissionRate(1f, 0f), 4);
        Assert.Equal(
            ShadeVisualTuning.EmissionRate(0.5f, 1f) * 2f,
            ShadeVisualTuning.EmissionRate(0.5f, 2f),
            4);
    }

    [Fact]
    public void EmissionIntensityIsClampedToTheConfiguredMaximum()
    {
        Assert.Equal(
            ShadeVisualTuning.EmissionRate(0.5f, ModConfig.MaxShadowParticleIntensity),
            ShadeVisualTuning.EmissionRate(0.5f, ModConfig.MaxShadowParticleIntensity + 5f),
            4);
    }

    [Fact]
    public void AlphaAndSizeLiftWithSoulButFarLessThanEmissionDoes()
    {
        float alphaEmpty = ShadeVisualTuning.AlphaScale(0f, 1f);
        float alphaFull = ShadeVisualTuning.AlphaScale(1f, 1f);
        Assert.Equal(1f, alphaEmpty, 4);
        Assert.True(alphaFull > alphaEmpty);
        // Doubling opacity as well as count would turn the Shade into a solid black blob.
        Assert.True(alphaFull < 1.5f);

        Assert.Equal(1f, ShadeVisualTuning.SizeScale(0f), 4);
        Assert.True(ShadeVisualTuning.SizeScale(1f) > 1f);
        Assert.True(ShadeVisualTuning.SizeScale(1f) < 1.5f);
    }

    [Fact]
    public void AlphaRespondsToUserIntensityAtHalfRate()
    {
        // Below 1 the multiplier applies directly (turning it down should genuinely fade the
        // wisps); above 1 only half of the extra is applied, so max intensity thickens rather
        // than saturates.
        Assert.Equal(0.5f, ShadeVisualTuning.AlphaScale(0f, 0.5f), 4);
        Assert.Equal(1.5f, ShadeVisualTuning.AlphaScale(0f, 2f), 4);
    }
}
