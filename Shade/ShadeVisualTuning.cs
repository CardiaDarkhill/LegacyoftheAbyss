#nullable enable

using System;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// The arithmetic behind the Shade's rendering options, kept free of Unity object access so
    /// it can be exercised without a running engine. The Unity-side plumbing that consumes these
    /// values lives in <c>LegacyHelper.ShadeRendering.cs</c> (sorting/material) and
    /// <c>LegacyHelper.ShadeController.ShadowParticles.cs</c> (the emitter).
    /// </summary>
    internal static class ShadeVisualTuning
    {
        /// <summary>Unity's implicit first sorting layer; every project has it, so it is the last resort.</summary>
        internal const string UnityDefaultSortingLayer = "Default";

        /// <summary>
        /// Particles emitted per second at 0% SOUL, before the player's intensity multiplier.
        /// Deliberately non-trivial: the brief is that the wisps are already noticeable on an
        /// empty soul meter rather than fading out to nothing. Sized against the emitter's spawn
        /// box (see ShadowSpawnWidth/Height) - spreading the same rate over the whole body reads as
        /// noticeably sparser than emitting it all from one point did.
        /// </summary>
        internal const float BaseEmissionRate = 24f;

        /// <summary>
        /// Emission is <see cref="BaseEmissionRate"/> x (1 + soulFraction), i.e. exactly double at
        /// full SOUL. Alpha and size add a smaller lift on top so a full meter also reads as
        /// *thicker* smoke, not just more of it.
        /// </summary>
        internal const float FullSoulAlphaLift = 0.18f;

        internal const float FullSoulSizeLift = 0.15f;

        /// <summary>How fast the emitter chases a change in SOUL, in soul-fractions per second.</summary>
        internal const float IntensitySlewPerSecond = 1.6f;

        /// <summary>
        /// Picks the sorting layer the Shade's sprite renderer should use.
        /// <para>
        /// <paramref name="configured"/> is <see cref="ModConfig.shadeSortingLayer"/>. A blank value
        /// (or a layer name this game build does not define, e.g. after a Unity upgrade reshuffles
        /// them) falls through to Hornet's own layer, then to Unity's "Default".
        /// </para>
        /// </summary>
        internal static string ResolveSortingLayerName(string? configured, Func<string, bool> layerExists, string? heroLayerName)
        {
            if (layerExists == null)
            {
                throw new ArgumentNullException(nameof(layerExists));
            }

            if (!string.IsNullOrWhiteSpace(configured) && SafeExists(layerExists, configured!))
            {
                return configured!;
            }

            if (!string.IsNullOrWhiteSpace(heroLayerName) && SafeExists(layerExists, heroLayerName!))
            {
                return heroLayerName!;
            }

            if (SafeExists(layerExists, ModConfig.DefaultShadeSortingLayer))
            {
                return ModConfig.DefaultShadeSortingLayer;
            }

            return UnityDefaultSortingLayer;
        }

        /// <summary>
        /// Draw order within the resolved layer. Sharing Hornet's layer means the order only has
        /// meaning relative to hers, so the configured value is applied as an offset; on any other
        /// layer the layer itself already decides who occludes whom, so it is used as-is.
        /// </summary>
        internal static int ResolveSortingOrder(int heroSortingOrder, int configuredOffset, bool sharesHeroLayer)
        {
            return sharesHeroLayer ? heroSortingOrder + configuredOffset : configuredOffset;
        }

        /// <summary>Current SOUL as 0..1. Guards the "no capacity yet" case seen before charms resolve.</summary>
        internal static float SoulFraction(int soul, int soulMax)
        {
            if (soulMax <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01((float)soul / soulMax);
        }

        /// <summary>Particles per second for a given SOUL fill and player intensity multiplier.</summary>
        internal static float EmissionRate(float soulFraction, float userIntensity)
        {
            float fraction = Mathf.Clamp01(soulFraction);
            float intensity = Mathf.Clamp(userIntensity, 0f, ModConfig.MaxShadowParticleIntensity);
            return BaseEmissionRate * (1f + fraction) * intensity;
        }

        /// <summary>Multiplier applied to the wisps' start alpha.</summary>
        internal static float AlphaScale(float soulFraction, float userIntensity)
        {
            float fraction = Mathf.Clamp01(soulFraction);
            float intensity = Mathf.Clamp(userIntensity, 0f, ModConfig.MaxShadowParticleIntensity);
            // Intensity above 1 thickens the smoke, but only halfway - doubling both the count and
            // the opacity turns the Shade into a solid black blob.
            float intensityLift = intensity <= 1f ? intensity : 1f + (intensity - 1f) * 0.5f;
            return (1f + FullSoulAlphaLift * fraction) * intensityLift;
        }

        /// <summary>Multiplier applied to the wisps' start size.</summary>
        internal static float SizeScale(float soulFraction)
        {
            return 1f + FullSoulSizeLift * Mathf.Clamp01(soulFraction);
        }

        private static bool SafeExists(Func<string, bool> layerExists, string name)
        {
            try
            {
                return layerExists(name);
            }
            catch
            {
                return false;
            }
        }
    }
}
