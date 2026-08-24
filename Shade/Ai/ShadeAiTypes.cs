#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// The one thing the Shade does this frame. Exactly one, because the combat handlers in
    /// <c>Update</c> run in sequence and a frame that both slashes and casts would do both.
    /// </summary>
    internal enum ShadeAiAction
    {
        None,
        SlashSide,
        SlashUp,
        SlashDown,
        Fireball,
        Shriek,
        DescendingDark,
        /// <summary>Hold Focus. A channel, so this is republished every frame until it completes.</summary>
        Focus
    }

    /// <summary>
    /// Why the plan is what it is. Kept as an enum plus a count rather than a formatted string so
    /// the per-frame path allocates nothing - the driver only renders it into words when the
    /// decision actually changes and an event gets written.
    /// </summary>
    internal enum ShadeAiReason
    {
        /// <summary>The driver never ran the brain - see the driver's own gate for which reason.</summary>
        Suspended,
        /// <summary>Nothing alive and reachable to fight.</summary>
        NoTargets,
        /// <summary>A target exists but every candidate sits outside the leash Hornet allows.</summary>
        OutOfLeash,
        /// <summary>
        /// Candidates exist but nothing can be seen from here - they are behind terrain. Its own
        /// reason because it is the state that produced the "Shade is attacking the wall" report.
        /// </summary>
        NoLineOfSight,
        /// <summary>Closing on the committed target.</summary>
        Approaching,
        /// <summary>At the strike point; slashing.</summary>
        InRange,
        /// <summary>Moving to a spot a spell would land from, rather than swinging from here.</summary>
        RepositioningToCast,
        /// <summary>A spell was cast because it lands on a boss.</summary>
        BossSpell,
        /// <summary>A spell was cast because it lands on <see cref="ShadeAiPlan.ReasonCount"/> enemies at once.</summary>
        ClusterSpell,
        /// <summary>A spell was worth casting but the SOUL it needs is spoken for.</summary>
        SoulReserved,
        /// <summary>Backing out of an attack that would land on the Shade.</summary>
        Evading,
        /// <summary>Channelling Focus, for its own health or Hornet's.</summary>
        Healing,
        /// <summary>Holding position: in range but every attack is still on cooldown.</summary>
        Cooldown,
        /// <summary>Standing where the player pointed, rather than choosing for itself.</summary>
        Commanded
    }

    /// <summary>One enemy the Shade could hit, flattened out of its <c>HealthManager</c>.</summary>
    internal readonly struct ShadeAiTarget
    {
        internal ShadeAiTarget(int id, Vector2 position, float radius, int hp, bool isBoss, bool hasLineOfSight)
        {
            Id = id;
            Position = position;
            Radius = radius;
            Hp = hp;
            IsBoss = isBoss;
            HasLineOfSight = hasLineOfSight;
        }

        /// <summary>Stable per-enemy identity, so a commitment survives the scan list being rebuilt.</summary>
        internal int Id { get; }

        /// <summary>Centre of the enemy's damage collider, not its transform - pivots are routinely offset.</summary>
        internal Vector2 Position { get; }

        /// <summary>Half-extent of that collider, so reach checks account for a large enemy's body.</summary>
        internal float Radius { get; }

        internal int Hp { get; }

        internal bool IsBoss { get; }

        /// <summary>
        /// False when terrain sits between the Shade and this enemy.
        /// <para>
        /// Not a nicety. Two bug reports came out of its absence: the Shade slashing a wall for
        /// twenty seconds at an enemy on the other side of it, and three fireballs spent on
        /// something the projectile could never have reached. Nothing unseeable is a target, and
        /// nothing unseeable is counted toward a cast.
        /// </para>
        /// </summary>
        internal bool HasLineOfSight { get; }
    }

    /// <summary>
    /// A tally of what the last scan threw away and why.
    /// <para>
    /// Exists because a report could not answer "the Shade sees no enemies - which check dropped
    /// them?". Three filters were added on speculation to stop the Shade slashing a wall, two of
    /// them were wrong, and the report that caught it could only say <c>0 enemies in range</c>.
    /// That is exactly the ambiguity between "the code never ran", "it ran and chose not to act" and
    /// "the situation never arose" that costs a round trip every time.
    /// </para>
    /// </summary>
    internal struct ShadeAiScanStats
    {
        /// <summary>HealthManagers the last full rescan found in the scene.</summary>
        internal int Found;

        /// <summary>Of those, still alive and enabled this frame.</summary>
        internal int Tracked;

        /// <summary>Dropped this frame for being further away than the scan radius.</summary>
        internal int OutOfRange;

        /// <summary>Handed to the brain, whether or not they turned out to be visible.</summary>
        internal int Returned;

        /// <summary>Of those, behind terrain.</summary>
        internal int Blocked;
    }

    /// <summary>
    /// Something that would hurt the Shade if it stood there - one enabled <c>DamageHero</c> volume,
    /// reduced to a circle.
    /// </summary>
    internal readonly struct ShadeAiThreat
    {
        internal ShadeAiThreat(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        internal Vector2 Position { get; }

        internal float Radius { get; }
    }

    /// <summary>
    /// Everything the brain is allowed to see. Assembled once a frame by the driver
    /// (<c>LegacyHelper.ShadeController.Ai.cs</c>) so the decision itself touches no Unity object
    /// and can be exercised without a running engine.
    /// </summary>
    internal struct ShadeAiSnapshot
    {
        internal float Time;

        internal Vector2 ShadePosition;
        internal Vector2 HornetPosition;

        /// <summary>The Shade's own facing.</summary>
        internal int Facing;

        /// <summary>-1 or +1. Which way Hornet is looking, which is where she is about to go.</summary>
        internal int HornetFacing;

        /// <summary>
        /// Whether Hornet is off the ground. Held briefly past a landing by the driver, so a bumpy
        /// run of small hops does not have the Shade swapping corners several times a second.
        /// </summary>
        internal bool HornetAirborne;

        /// <summary>The Shade's radial soft leash. Past this Hornet starts dragging it home.</summary>
        internal float SoftLeashRadius;

        /// <summary>Units per second, so the brain can price a reposition in time rather than distance.</summary>
        internal float MoveSpeed;

        /// <summary>
        /// The gap the AI leaves between its own swings, after the attack-speed cap. Also the budget
        /// a reposition has to fit inside to be worth making instead of swinging.
        /// </summary>
        internal float NailInterval;

        internal int Soul;

        /// <summary>
        /// SOUL the brain may not spend on offence, because a heal is going to need it.
        /// </summary>
        internal int SoulReserve;

        internal int ProjectileSoulCost;
        internal int ShriekSoulCost;
        internal int QuakeSoulCost;
        internal int FocusSoulCost;

        internal bool ProjectileUnlocked;
        internal bool ShriekUnlocked;
        internal bool DescendingDarkUnlocked;

        internal bool NailReady;
        internal bool FireReady;
        internal bool ShriekReady;
        internal bool QuakeReady;

        /// <summary>False when Focus is unavailable - already at full health, or disabled by a charm.</summary>
        internal bool CanFocusHeal;

        /// <summary>
        /// Whether a Focus channel is already running. Focus drains SOUL as it channels and refunds
        /// nothing when cancelled, so a brain that re-decided from scratch every frame could bleed
        /// the meter dry starting and dropping channels without ever completing one.
        /// </summary>
        internal bool IsFocusing;

        /// <summary>How close the Shade has to be to Hornet for a completed Focus to heal her too.</summary>
        internal float FocusHealRange;

        /// <summary>0-1.</summary>
        internal float SelfHealthFraction;

        /// <summary>0-1.</summary>
        internal float HornetHealthFraction;

        /// <summary>False while the Shade is invincible, which turns off avoidance and self-healing.</summary>
        internal bool CanTakeDamage;

        /// <summary>
        /// True while the player has told the Shade to hold a spot. It goes there and stays,
        /// fighting only what comes within reach, until the order is lifted.
        /// </summary>
        internal bool HasCommand;

        /// <summary>Where that order points. Meaningless unless <see cref="HasCommand"/>.</summary>
        internal Vector2 CommandPoint;

        internal ShadeAiTuning Tuning;

        internal IReadOnlyList<ShadeAiTarget> Targets;

        internal IReadOnlyList<ShadeAiThreat> Threats;
    }

    /// <summary>What the driver should make the Shade do this frame.</summary>
    internal struct ShadeAiPlan
    {
        internal ShadeAiAction Action;

        /// <summary>
        /// Where the Shade wants to stand. The driver turns this into movement input; keeping it a
        /// position rather than a direction is what lets avoidance nudge it around a hitbox and a
        /// cast pull it to a better spot without either touching the combat decision.
        /// </summary>
        internal Vector2 DesiredPosition;

        /// <summary>Per-axis movement in -1..1, already deadzoned.</summary>
        internal Vector2 Move;

        internal bool Sprint;

        /// <summary>-1, 0 or +1. Set when the Shade must face a way its movement would not turn it.</summary>
        internal int FaceX;

        internal int TargetId;

        internal bool HasTarget;

        internal ShadeAiReason Reason;

        /// <summary>The discriminator behind <see cref="Reason"/> - a cluster size, usually.</summary>
        internal int ReasonCount;

        internal static ShadeAiPlan Idle(ShadeAiReason reason) => new ShadeAiPlan
        {
            Action = ShadeAiAction.None,
            Reason = reason
        };
    }
}
