#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// Every distance and threshold the brain reasons with, passed in rather than read from
    /// <see cref="ModConfig"/> inside the decision so tests can drive the brain with chosen values.
    /// <para>
    /// Geometry numbers are deliberately tighter than the hitboxes they stand in for: Shriek spawns
    /// a 12-unit, 95-degree cone, so asking for 11 at 84 keeps a counted enemy inside the cone by
    /// the time the cast animation finishes and the collider appears.
    /// </para>
    /// </summary>
    internal readonly struct ShadeAiTuning
    {
        /// <summary>How far from the Shade an enemy has to be before it is worth walking to.</summary>
        internal float EngageRadius { get; init; }

        /// <summary>
        /// Fraction of the soft leash the brain will use. Standing exactly on it means Hornet pulls
        /// the Shade backwards while it closes, which reads as the Shade fighting its own movement.
        /// </summary>
        internal float LeashUsableFraction { get; init; }

        /// <summary>Gap left between the Shade and the enemy's collider edge at the strike point.</summary>
        internal float StrikeStandoff { get; init; }

        /// <summary>Height above the enemy centre the strike point sits at, so the Shade floats rather than clips.</summary>
        internal float StrikeVerticalOffset { get; init; }

        /// <summary>Inside this of the strike point the Shade stops steering, so it does not jitter in place.</summary>
        internal float ArriveDeadzone { get; init; }

        /// <summary>Beyond this from the strike point the Shade sprints.</summary>
        internal float SprintDistance { get; init; }

        /// <summary>Horizontal reach of a side slash, measured from the Shade to the enemy's collider edge.</summary>
        internal float NailReach { get; init; }

        /// <summary>
        /// How far off the Shade's own height an enemy may be and still be hit by a side slash.
        /// Sized against the slash prefab, which comes out around 2.1 units tall in world scale, so
        /// the blade covers roughly a unit either side of centre.
        /// </summary>
        internal float NailVerticalTolerance { get; init; }

        /// <summary>How far to the side an enemy may be and still be hit by an up or down slash.</summary>
        internal float NailHorizontalTolerance { get; init; }

        /// <summary>Below this the Shade slashes instead - a fireball fired point blank wastes SOUL.</summary>
        internal float FireballMinRange { get; init; }

        internal float FireballMaxRange { get; init; }

        /// <summary>The projectile flies flat, so the target has to be close to level to be worth one.</summary>
        internal float FireballVerticalTolerance { get; init; }

        internal float ShriekConeHeight { get; init; }

        internal float ShriekConeHalfAngleDegrees { get; init; }

        /// <summary>Horizontal half-width of the ground slam Descending Dark lands.</summary>
        internal float QuakeRadius { get; init; }

        /// <summary>How far below the Shade the slam still counts as landing on an enemy.</summary>
        internal float QuakeBelowReach { get; init; }

        /// <summary>How far above the Shade an enemy may be and still be caught by the slam.</summary>
        internal float QuakeAboveReach { get; init; }

        /// <summary>How many enemies count as a cluster worth spending a spell on.</summary>
        internal int MinClusterForSpell { get; init; }

        /// <summary>How long a target commitment stands before another candidate may take it.</summary>
        internal float RetargetInterval { get; init; }

        /// <summary>
        /// How much better a candidate must be before the Shade drops what it is fighting, as a
        /// fraction of the committed target's score. Same dead band as
        /// <see cref="ShadeAggroTargeting.PreferShade"/>: without it an enemy pair at near-equal
        /// range swaps every interval and the Shade walks back and forth hitting neither.
        /// </summary>
        internal float TargetSwitchMargin { get; init; }

        /// <summary>Score bonus applied to a boss, as a fraction. Bosses are worth crossing a room for.</summary>
        internal float BossPreference { get; init; }

        /// <summary>
        /// How many of the Shade's own nail hits an enemy has to survive before a single-target spell
        /// is worth the SOUL. Counted in hits rather than hit points so it scales with the Shade's
        /// damage and charms; the game assembly exposes no per-enemy boss flag to use instead, as
        /// <c>BossSceneController.bosses</c> is routinely left empty.
        /// </summary>
        internal int BossNailHits { get; init; }

        /// <summary>
        /// How far from the edge of a damaging volume the Shade tries to stay once it can be hurt.
        /// Must stay below <see cref="StrikeStandoff"/>: an enemy body is itself a <c>DamageHero</c>
        /// volume, so a wider standoff makes every melee target its own no-go zone.
        /// </summary>
        internal float ThreatStandoff { get; init; }

        /// <summary>How far to Hornet's side the Shade waits when it has nothing to fight.</summary>
        internal float EscortOffsetX { get; init; }

        /// <summary>How far above her it waits while she is on the ground.</summary>
        internal float EscortHeightAbove { get; init; }

        /// <summary>
        /// How far below her it drops while she is airborne. Sized against the pogo target, a
        /// 1.45-tall capsule centred on the Shade (<c>EnsurePogoTarget</c>) whose top face sits about
        /// 0.7 above it: too far below and the pogo is unreachable, too close and the Shade is inside her.
        /// </summary>
        internal float EscortHeightBelow { get; init; }

        /// <summary>Health fraction below which the Shade holds SOUL back to heal itself.</summary>
        internal float SelfHealHealthFraction { get; init; }

        /// <summary>Health fraction below which it holds SOUL back to heal Hornet.</summary>
        internal float HornetHealHealthFraction { get; init; }

        internal static ShadeAiTuning Default => new ShadeAiTuning
        {
            EngageRadius = 14f,
            LeashUsableFraction = 0.85f,
            StrikeStandoff = 1.4f,
            StrikeVerticalOffset = 0.2f,
            ArriveDeadzone = 0.35f,
            SprintDistance = 6f,
            NailReach = 2.3f,
            NailVerticalTolerance = 1.1f,
            NailHorizontalTolerance = 1.1f,
            FireballMinRange = 3f,
            FireballMaxRange = 16f,
            FireballVerticalTolerance = 1.2f,
            ShriekConeHeight = 11f,
            ShriekConeHalfAngleDegrees = 42f,
            QuakeRadius = 5.5f,
            QuakeBelowReach = 9f,
            QuakeAboveReach = 1.5f,
            MinClusterForSpell = 2,
            RetargetInterval = 0.6f,
            TargetSwitchMargin = 0.25f,
            BossPreference = 0.35f,
            BossNailHits = 20,
            ThreatStandoff = 0.6f,
            EscortOffsetX = 1.9f,
            EscortHeightAbove = 1.9f,
            EscortHeightBelow = 2f,
            SelfHealHealthFraction = 0.5f,
            HornetHealHealthFraction = 0.4f
        };

        /// <summary>
        /// The player-facing subset. Everything else is a geometry constant tracking a hitbox we do
        /// not control, so exposing it would only let config drift out of sync with that collider.
        /// </summary>
        internal static ShadeAiTuning FromConfig(ModConfig? config)
        {
            var tuning = Default;
            if (config == null)
            {
                return tuning;
            }

            return tuning with
            {
                EngageRadius = Mathf.Clamp(config.shadeAiEngageRadius, 2f, 40f),
                MinClusterForSpell = Mathf.Clamp(config.shadeAiSpellClusterSize, 1, 8),
                BossNailHits = Mathf.Clamp(config.shadeAiSpellWorthNailHits, 1, 100),
                SelfHealHealthFraction = Mathf.Clamp01(config.shadeAiSelfHealBelow),
                HornetHealHealthFraction = Mathf.Clamp01(config.shadeAiHornetHealBelow)
            };
        }
    }
}
