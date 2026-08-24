#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// The Shade AI: pick an enemy, close on it, hit it, spend SOUL only where a spell earns it, stay
    /// out of what would hurt, and heal when it or Hornet is low.
    /// <para>
    /// Deliberately free of Unity object access. Everything it needs arrives in a
    /// <see cref="ShadeAiSnapshot"/> and everything it decides leaves in a <see cref="ShadeAiPlan"/>,
    /// so the interesting half of the feature is covered by <c>Tests/ShadeAiBrainTests.cs</c> rather
    /// than only by play sessions - which, for a mod that patches an assembly we do not control, is
    /// the difference between a five-second diagnosis and a lost evening.
    /// </para>
    /// <para>
    /// The only instance state is the target commitment. Everything else is derived per frame.
    /// </para>
    /// </summary>
    internal sealed class ShadeAiBrain
    {
        /// <summary>
        /// Where the Shriek cone apex sits relative to the Shade, matching the <c>localOffset</c>
        /// passed to <c>SpawnShriekCone</c>. If that moves, this moves with it.
        /// </summary>
        internal const float ShriekApexHeight = 0.8f;

        /// <summary>
        /// Extra clearance, on top of <see cref="ShadeAiTuning.ThreatStandoff"/>, that a spot needs
        /// before the Shade will stand still in it for a whole Focus channel.
        /// </summary>
        internal const float HealSafetyMargin = 1.5f;

        private int committedTargetId;
        private float nextRetargetTime;

        /// <summary>Drops the target commitment. Called whenever the AI is switched on or off.</summary>
        internal void Reset()
        {
            committedTargetId = 0;
            nextRetargetTime = 0f;
        }

        internal ShadeAiPlan Decide(in ShadeAiSnapshot snapshot)
        {
            var tuning = snapshot.Tuning;
            float leashRadius = Mathf.Max(1f, snapshot.SoftLeashRadius * tuning.LeashUsableFraction);

            // 1. Healing. Focus is a channel that pins the Shade in place, so it
            //    only starts somewhere with more clearance than simply standing would need.
            if (ShouldHeal(snapshot))
            {
                var healPlan = BuildHealPlan(snapshot, leashRadius, tuning);
                if (healPlan.HasValue)
                {
                    return healPlan.Value;
                }
            }

            // 3. An order from the player outranks the Shade's own choice of where to be. It still
            //    defends itself where it stands - a Shade told to hold a doorway that then refuses to
            //    swing at what walks into it would be worse than useless.
            if (snapshot.HasCommand)
            {
                return MarkEvading(snapshot, BuildCommandPlan(snapshot, leashRadius, tuning));
            }

            if (!TrySelectTarget(snapshot, leashRadius, out var target, out var noTargetReason))
            {
                return MarkEvading(snapshot, BuildEscortPlan(snapshot, leashRadius, tuning, noTargetReason));
            }

            return MarkEvading(snapshot, BuildCombatPlan(snapshot, leashRadius, tuning, target));
        }

        /// <summary>
        /// Notes that the Shade is standing in something that will hurt it, and stops it swinging
        /// while it is.
        /// <para>
        /// This used to be a branch of its own that replaced the destination with "straight back the
        /// way you came", and that was a bug with a very clear symptom: an enemy standing between the
        /// Shade and where it was told to go made the trip impossible. It would approach, enter the
        /// enemy's body hitbox, be sent back, approach again, and never arrive - and the moment the
        /// enemy died the same order completed fine.
        /// </para>
        /// <para>
        /// Every destination this brain produces has already been pushed clear of threats, so simply
        /// continuing toward it <i>is</i> the way out of one. Getting round the enemy on the way is
        /// the navigator's job, which treats a hitbox as an obstacle exactly like a wall.
        /// </para>
        /// </summary>
        private static ShadeAiPlan MarkEvading(in ShadeAiSnapshot snapshot, ShadeAiPlan plan)
        {
            if (!snapshot.CanTakeDamage || IsSafeAt(snapshot.ShadePosition, snapshot.Threats, snapshot.Tuning.ThreatStandoff))
            {
                return plan;
            }

            plan.Action = ShadeAiAction.None;
            plan.Reason = ShadeAiReason.Evading;
            plan.ReasonCount = snapshot.Threats != null ? snapshot.Threats.Count : 0;
            return plan;
        }

        /// <summary>
        /// Picks what to fight, or says why nothing qualifies. Shared by the ordinary combat path and
        /// by a commanded Shade deciding whether anything has wandered into its reach.
        /// </summary>
        private bool TrySelectTarget(in ShadeAiSnapshot snapshot, float leashRadius, out ShadeAiTarget target, out ShadeAiReason reason)
        {
            var tuning = snapshot.Tuning;
            target = default;
            reason = ShadeAiReason.NoTargets;

            var targets = snapshot.Targets;
            if (targets == null || targets.Count == 0)
            {
                Reset();
                return false;
            }

            int bestIndex = -1;
            float bestScore = float.MaxValue;
            int committedIndex = -1;
            float committedScore = float.MaxValue;
            bool sawOutOfLeash = false;
            bool sawBlocked = false;

            for (int i = 0; i < targets.Count; i++)
            {
                var candidate = targets[i];

                // Nothing behind terrain is a target. This is the whole of the "Shade is attacking
                // the wall over and over" report: an enemy on the far side of a wall was the nearest
                // thing in range, so the Shade walked up to the wall and swung at it forever.
                if (!candidate.HasLineOfSight)
                {
                    sawBlocked = true;
                    continue;
                }

                // Refusing a target the leash cannot reach is the difference between the Shade
                // fighting and the Shade being dragged backwards mid-swing.
                float fromHornet = Vector2.Distance(candidate.Position, snapshot.HornetPosition) - candidate.Radius;
                if (fromHornet > leashRadius + tuning.NailReach)
                {
                    sawOutOfLeash = true;
                    continue;
                }

                float fromShade = Mathf.Max(0f, Vector2.Distance(candidate.Position, snapshot.ShadePosition) - candidate.Radius);
                if (fromShade > tuning.EngageRadius)
                {
                    continue;
                }

                float score = Mathf.Max(0.01f, fromShade);
                if (candidate.IsBoss)
                {
                    score *= Mathf.Clamp01(1f - tuning.BossPreference);
                }

                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }

                if (candidate.Id == committedTargetId)
                {
                    committedIndex = i;
                    committedScore = score;
                }
            }

            if (bestIndex < 0)
            {
                Reset();
                reason = sawBlocked ? ShadeAiReason.NoLineOfSight
                    : sawOutOfLeash ? ShadeAiReason.OutOfLeash
                    : ShadeAiReason.NoTargets;
                return false;
            }

            int chosenIndex = ResolveCommitment(snapshot.Time, tuning, bestIndex, bestScore, committedIndex, committedScore);
            target = targets[chosenIndex];
            committedTargetId = target.Id;
            return true;
        }

        /// <summary>
        /// Holding a spot the player pointed at. The Shade walks there, stays, and takes whatever
        /// swings and casts land from where it is standing - but never steps off the spot to chase.
        /// </summary>
        private ShadeAiPlan BuildCommandPlan(in ShadeAiSnapshot snapshot, float leashRadius, in ShadeAiTuning tuning)
        {
            Vector2 point = ClampToLeash(snapshot.CommandPoint, snapshot.HornetPosition, leashRadius);
            if (snapshot.CanTakeDamage)
            {
                point = AvoidThreats(point, snapshot.Threats, tuning.ThreatStandoff);
            }

            var plan = BuildMovementPlan(snapshot, point, tuning);
            plan.Reason = ShadeAiReason.Commanded;

            if (!TrySelectTarget(snapshot, leashRadius, out var target, out _))
            {
                return plan;
            }

            plan.HasTarget = true;
            plan.TargetId = target.Id;
            plan.FaceX = FaceToward(snapshot.ShadePosition, target.Position);

            var spell = ChooseSpell(snapshot, snapshot.ShadePosition, target, out _, out int spellCount, out _);
            if (spell != ShadeAiAction.None)
            {
                plan.Action = spell;
                plan.ReasonCount = spellCount;
                return plan;
            }

            if (snapshot.NailReady)
            {
                var nail = ChooseNailDirection(target.Position - snapshot.ShadePosition, target.Radius, tuning);
                if (nail != ShadeAiAction.None)
                {
                    plan.Action = nail;
                }
            }

            return plan;
        }

        private ShadeAiPlan BuildCombatPlan(in ShadeAiSnapshot snapshot, float leashRadius, in ShadeAiTuning tuning, in ShadeAiTarget target)
        {
            // 3. A spell that already lands from here.
            var spell = ChooseSpell(snapshot, snapshot.ShadePosition, target, out var spellReason, out int spellCount, out bool soulBlocked);
            if (spell != ShadeAiAction.None)
            {
                var castPlan = BuildMovementPlan(snapshot, snapshot.ShadePosition, tuning);
                castPlan.HasTarget = true;
                castPlan.TargetId = target.Id;
                castPlan.Action = spell;
                castPlan.Reason = spellReason;
                castPlan.ReasonCount = spellCount;
                castPlan.FaceX = FaceToward(snapshot.ShadePosition, target.Position);
                return castPlan;
            }

            // 4. A spell that would land from somewhere the Shade can reach in less time than the
            //    swing it would otherwise be making. Stepping two units to catch three enemies in one
            //    cone beats hitting one of them with the nail.
            if (TryFindCastPosition(snapshot, target, leashRadius, out Vector2 castPosition, out int castCount))
            {
                var movePlan = BuildMovementPlan(snapshot, castPosition, tuning);
                movePlan.HasTarget = true;
                movePlan.TargetId = target.Id;
                movePlan.Reason = ShadeAiReason.RepositioningToCast;
                movePlan.ReasonCount = castCount;
                movePlan.FaceX = FaceToward(snapshot.ShadePosition, target.Position);
                return movePlan;
            }

            // 5. Otherwise close on it and swing.
            Vector2 strikePoint = ComputeStrikePoint(snapshot, target);
            strikePoint = ClampToLeash(strikePoint, snapshot.HornetPosition, leashRadius);
            if (snapshot.CanTakeDamage)
            {
                strikePoint = AvoidThreats(strikePoint, snapshot.Threats, tuning.ThreatStandoff);
            }

            var plan = BuildMovementPlan(snapshot, strikePoint, tuning);
            plan.HasTarget = true;
            plan.TargetId = target.Id;
            plan.FaceX = FaceToward(snapshot.ShadePosition, target.Position);

            Vector2 delta = target.Position - snapshot.ShadePosition;
            var nail = snapshot.NailReady ? ChooseNailDirection(delta, target.Radius, tuning) : ShadeAiAction.None;
            if (nail != ShadeAiAction.None)
            {
                plan.Action = nail;
                plan.Reason = ShadeAiReason.InRange;
                return plan;
            }

            float strikeDistance = Vector2.Distance(strikePoint, snapshot.ShadePosition);
            if (soulBlocked)
            {
                plan.Reason = ShadeAiReason.SoulReserved;
            }
            else if (strikeDistance <= tuning.ArriveDeadzone)
            {
                // In position with nothing off cooldown. Named separately from Approaching because
                // "the Shade is standing next to an enemy doing nothing" has two very different
                // causes and they look identical from outside.
                plan.Reason = ShadeAiReason.Cooldown;
            }
            else
            {
                plan.Reason = ShadeAiReason.Approaching;
            }

            return plan;
        }

        // --- Escorting ----------------------------------------------------------------------

        /// <summary>
        /// Where the Shade waits when it has nothing to fight.
        /// <para>
        /// On the ground it sits behind and above Hornet - out of the way of whatever she is walking
        /// into, and clear of her own nail arc. In the air that inverts to ahead and below, which is
        /// the useful half: a Shade under the far side of a jump is a platform, and Hornet can turn a
        /// gap she would not otherwise clear into a pogo off it.
        /// </para>
        /// </summary>
        internal static Vector2 ComputeEscortPoint(in ShadeAiSnapshot snapshot, int sideSign, int verticalSign)
        {
            var tuning = snapshot.Tuning;
            float height = verticalSign >= 0 ? tuning.EscortHeightAbove : -tuning.EscortHeightBelow;
            return snapshot.HornetPosition + new Vector2(sideSign * tuning.EscortOffsetX, height);
        }

        /// <summary>
        /// The escort point, moved if standing there would hurt.
        /// <para>
        /// Tries the three other corners before giving up and pushing clear, because a corner is a
        /// place the Shade is meant to be and an arbitrary point outside a spike field is not. The
        /// mirrored corners are ordered so the horizontal flip comes first: it keeps the Shade at the
        /// height it wanted, which is the half of the position that matters for a pogo.
        /// </para>
        /// </summary>
        internal static Vector2 ResolveEscortPoint(in ShadeAiSnapshot snapshot, float leashRadius)
        {
            int facing = snapshot.HornetFacing >= 0 ? 1 : -1;
            int side = snapshot.HornetAirborne ? facing : -facing;
            int vertical = snapshot.HornetAirborne ? -1 : 1;
            var tuning = snapshot.Tuning;

            Vector2 preferred = ClampToLeash(ComputeEscortPoint(snapshot, side, vertical), snapshot.HornetPosition, leashRadius);
            if (!snapshot.CanTakeDamage)
            {
                return preferred;
            }

            if (IsSafeAt(preferred, snapshot.Threats, tuning.ThreatStandoff))
            {
                return preferred;
            }

            for (int i = 1; i < 4; i++)
            {
                int candidateSide = (i & 1) != 0 ? -side : side;
                int candidateVertical = (i & 2) != 0 ? -vertical : vertical;
                Vector2 candidate = ClampToLeash(
                    ComputeEscortPoint(snapshot, candidateSide, candidateVertical),
                    snapshot.HornetPosition,
                    leashRadius);

                if (IsSafeAt(candidate, snapshot.Threats, tuning.ThreatStandoff))
                {
                    return candidate;
                }
            }

            return AvoidThreats(preferred, snapshot.Threats, tuning.ThreatStandoff);
        }

        private static ShadeAiPlan BuildEscortPlan(in ShadeAiSnapshot snapshot, float leashRadius, in ShadeAiTuning tuning, ShadeAiReason reason)
        {
            var plan = BuildMovementPlan(snapshot, ResolveEscortPoint(snapshot, leashRadius), tuning);
            plan.Reason = reason;
            // Look the way Hornet is looking. Nothing reads it while there is no target, but a Shade
            // that keeps the facing of its last kill while trailing her reads as broken.
            plan.FaceX = snapshot.HornetFacing >= 0 ? 1 : -1;
            return plan;
        }

        // --- Movement -----------------------------------------------------------------------

        private static ShadeAiPlan BuildMovementPlan(in ShadeAiSnapshot snapshot, Vector2 desired, in ShadeAiTuning tuning)
        {
            Vector2 toDesired = desired - snapshot.ShadePosition;
            float distance = toDesired.magnitude;

            return new ShadeAiPlan
            {
                DesiredPosition = desired,
                Move = SteerTo(toDesired, distance, tuning.ArriveDeadzone),
                Sprint = distance > tuning.SprintDistance
            };
        }

        private static int FaceToward(Vector2 from, Vector2 to)
        {
            float horizontal = to.x - from.x;
            return Mathf.Abs(horizontal) > 0.1f ? (horizontal > 0f ? 1 : -1) : 0;
        }

        private int ResolveCommitment(float time, in ShadeAiTuning tuning, int bestIndex, float bestScore, int committedIndex, float committedScore)
        {
            if (committedIndex < 0)
            {
                nextRetargetTime = time + tuning.RetargetInterval;
                return bestIndex;
            }

            if (time < nextRetargetTime)
            {
                return committedIndex;
            }

            nextRetargetTime = time + tuning.RetargetInterval;
            return PreferNewTarget(committedScore, bestScore, tuning.TargetSwitchMargin) ? bestIndex : committedIndex;
        }

        /// <summary>
        /// Whether a candidate is enough better than what the Shade is already fighting to be worth
        /// switching to. Lower scores are better. The margin is a dead band, not a tie-break: two
        /// enemies at almost the same range must not trade the Shade back and forth.
        /// </summary>
        internal static bool PreferNewTarget(float committedScore, float candidateScore, float switchMargin)
        {
            return candidateScore < committedScore * (1f - Mathf.Clamp01(switchMargin));
        }

        /// <summary>
        /// The spot beside an enemy the Shade wants to swing from: on whichever side it is already
        /// on, one standoff clear of the enemy collider, slightly above centre.
        /// </summary>
        internal static Vector2 ComputeStrikePoint(in ShadeAiSnapshot snapshot, in ShadeAiTarget target)
        {
            var tuning = snapshot.Tuning;
            float horizontal = snapshot.ShadePosition.x - target.Position.x;
            float side;
            if (Mathf.Abs(horizontal) > 0.05f)
            {
                side = horizontal > 0f ? 1f : -1f;
            }
            else
            {
                // Dead level with the enemy: keep the side the Shade already faces rather than
                // picking one arbitrarily, so it does not swap sides every frame it is overlapping.
                side = snapshot.Facing >= 0 ? -1f : 1f;
            }

            float offset = target.Radius + tuning.StrikeStandoff;
            return new Vector2(target.Position.x + (side * offset), target.Position.y + tuning.StrikeVerticalOffset);
        }

        internal static Vector2 ClampToLeash(Vector2 desired, Vector2 hornetPosition, float leashRadius)
        {
            Vector2 offset = desired - hornetPosition;
            float distance = offset.magnitude;
            if (distance <= leashRadius || distance <= 0.0001f)
            {
                return desired;
            }

            return hornetPosition + (offset / distance * leashRadius);
        }

        /// <summary>Whether a position is clear of every damaging volume, by at least the standoff.</summary>
        internal static bool IsSafeAt(Vector2 position, IReadOnlyList<ShadeAiThreat>? threats, float standoff)
        {
            if (threats == null)
            {
                return true;
            }

            for (int i = 0; i < threats.Count; i++)
            {
                var threat = threats[i];
                if (Vector2.Distance(position, threat.Position) < threat.Radius + standoff)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Pushes a position out of anything that would damage the Shade standing there. Iterated
        /// rather than solved: overlapping volumes are common and one pass out of the first can land
        /// inside the second.
        /// </summary>
        internal static Vector2 AvoidThreats(Vector2 desired, IReadOnlyList<ShadeAiThreat>? threats, float standoff)
        {
            if (threats == null || threats.Count == 0)
            {
                return desired;
            }

            Vector2 adjusted = desired;
            for (int pass = 0; pass < 2; pass++)
            {
                bool moved = false;
                for (int i = 0; i < threats.Count; i++)
                {
                    var threat = threats[i];
                    float clearance = threat.Radius + standoff;
                    Vector2 offset = adjusted - threat.Position;
                    float distance = offset.magnitude;
                    if (distance >= clearance)
                    {
                        continue;
                    }

                    Vector2 direction = distance > 0.0001f ? offset / distance : Vector2.up;
                    adjusted = threat.Position + (direction * clearance);
                    moved = true;
                }

                if (!moved)
                {
                    break;
                }
            }

            return adjusted;
        }

        /// <summary>
        /// Movement input for a given offset: full tilt while far, tapering in so the Shade settles
        /// on the strike point instead of oscillating across it, and nothing at all inside the
        /// deadzone.
        /// </summary>
        internal static Vector2 SteerTo(Vector2 toStrike, float distance, float arriveDeadzone)
        {
            if (distance <= arriveDeadzone || distance <= 0.0001f)
            {
                return Vector2.zero;
            }

            float taper = Mathf.Clamp01(distance / Mathf.Max(0.01f, arriveDeadzone * 3f));
            return toStrike / distance * taper;
        }

        // --- Healing ------------------------------------------------------------------------

        /// <summary>
        /// Whether a Focus is worth starting.
        /// <para>
        /// Note the Shade has to be damaged either way: <c>HandleFocus</c> refuses to channel at full
        /// health, and healing Hornet is a side effect of the Shade healing itself while she is
        /// nearby, not something Focus can be aimed at her on its own.
        /// </para>
        /// </summary>
        internal static bool ShouldHeal(in ShadeAiSnapshot snapshot)
        {
            if (!snapshot.CanFocusHeal)
            {
                return false;
            }

            // A channel already under way is seen through rather than re-argued. Focus drains SOUL
            // the whole time it channels and refunds none of it if it is dropped, so a brain that
            // reconsidered every frame would spend the meter on channels it never finished the
            // moment the health fraction sat near its own threshold.
            if (snapshot.IsFocusing)
            {
                return snapshot.Soul > 0;
            }

            if (snapshot.Soul < snapshot.FocusSoulCost)
            {
                return false;
            }

            var tuning = snapshot.Tuning;
            bool selfLow = snapshot.CanTakeDamage && snapshot.SelfHealthFraction <= tuning.SelfHealHealthFraction;
            bool hornetLow = snapshot.HornetHealthFraction <= tuning.HornetHealHealthFraction;
            return selfLow || hornetLow;
        }

        /// <summary>
        /// Where to heal from, and whether it is safe to start yet. Returns null when the Shade
        /// should get on with fighting instead - the caller falls through to combat.
        /// </summary>
        private static ShadeAiPlan? BuildHealPlan(in ShadeAiSnapshot snapshot, float leashRadius, in ShadeAiTuning tuning)
        {
            // Stand close enough that the completed channel reaches Hornet too. It costs nothing when
            // she is already near, and a heal that only covers the Shade is half a heal.
            Vector2 healSpot = snapshot.ShadePosition;
            float toHornet = Vector2.Distance(snapshot.ShadePosition, snapshot.HornetPosition);
            float wantedRange = Mathf.Max(0.5f, snapshot.FocusHealRange * 0.6f);
            if (snapshot.HornetHealthFraction <= tuning.HornetHealHealthFraction && toHornet > wantedRange)
            {
                Vector2 toward = (snapshot.HornetPosition - snapshot.ShadePosition).normalized;
                healSpot = snapshot.HornetPosition - (toward * wantedRange);
            }

            healSpot = ClampToLeash(healSpot, snapshot.HornetPosition, leashRadius);
            if (snapshot.CanTakeDamage)
            {
                healSpot = AvoidThreats(healSpot, snapshot.Threats, tuning.ThreatStandoff + HealSafetyMargin);
            }

            var plan = BuildMovementPlan(snapshot, healSpot, tuning);
            plan.Reason = ShadeAiReason.Healing;

            // Mid-channel the Shade is already standing where it chose to stand, and the movement
            // it would need to "arrive" is blocked by the channel anyway.
            bool arrived = snapshot.IsFocusing
                || Vector2.Distance(healSpot, snapshot.ShadePosition) <= tuning.ArriveDeadzone;

            // Starting a channel wants room to spare; continuing one only wants to not be standing
            // in something. Focus drains SOUL the whole time and refunds none of it, so abandoning a
            // channel at eighty percent costs eighty percent of a heal and buys nothing - which is
            // exactly what a threat drifting into the wider margin used to do. Once the meter is
            // being spent, only something that would actually land is worth stopping for.
            float margin = snapshot.IsFocusing ? tuning.ThreatStandoff : tuning.ThreatStandoff + HealSafetyMargin;
            bool clear = !snapshot.CanTakeDamage
                || IsSafeAt(snapshot.ShadePosition, snapshot.Threats, margin);

            if (arrived && clear)
            {
                plan.Action = ShadeAiAction.Focus;
                plan.Move = Vector2.zero;
                return plan;
            }

            // Not there yet, or not safe enough to stand still. Walking to the heal spot is still
            // the right thing to be doing, so commit to it rather than resuming an attack.
            return arrived ? null : plan;
        }

        // --- Slashes ------------------------------------------------------------------------

        /// <summary>
        /// Which slash, if any, reaches the enemy from where the Shade is standing.
        /// <para>
        /// The axis the enemy actually lies on decides first. An earlier version tested sideways
        /// before anything else and took it whenever the enemy was within the side slash's vertical
        /// band at all, so an enemy up and to the left got swung at sideways and missed - the band is
        /// how far the blade reaches vertically, not a claim that a horizontal swing covers the whole
        /// of it. Diagonal enemies were the visible symptom.
        /// </para>
        /// <para>
        /// When neither slash reaches, returning None is the point: the caller falls through to
        /// approaching, and the Shade lines itself up instead of swinging at air.
        /// </para>
        /// </summary>
        internal static ShadeAiAction ChooseNailDirection(Vector2 delta, float targetRadius, in ShadeAiTuning tuning)
        {
            float horizontal = Mathf.Abs(delta.x);
            float vertical = Mathf.Abs(delta.y);

            bool sideReaches = vertical <= tuning.NailVerticalTolerance + targetRadius
                && horizontal - targetRadius <= tuning.NailReach;
            bool verticalReaches = horizontal <= tuning.NailHorizontalTolerance + targetRadius
                && vertical - targetRadius <= tuning.NailReach;

            ShadeAiAction verticalSlash = delta.y >= 0f ? ShadeAiAction.SlashUp : ShadeAiAction.SlashDown;

            // Whichever way the enemy mostly is, tried first; the other only as a fallback.
            if (vertical > horizontal)
            {
                if (verticalReaches)
                {
                    return verticalSlash;
                }

                return sideReaches ? ShadeAiAction.SlashSide : ShadeAiAction.None;
            }

            if (sideReaches)
            {
                return ShadeAiAction.SlashSide;
            }

            return verticalReaches ? verticalSlash : ShadeAiAction.None;
        }

        // --- Spells -------------------------------------------------------------------------

        /// <summary>
        /// The spell rule in one place: cast only where the cast lands on a boss or on
        /// <see cref="ShadeAiTuning.MinClusterForSpell"/> enemies at once. Ordered by what the spell
        /// is worth - Shriek does 4x, Descending Dark 3x, the projectile 1x.
        /// </summary>
        internal static ShadeAiAction ChooseSpell(in ShadeAiSnapshot snapshot, Vector2 origin, in ShadeAiTarget target, out ShadeAiReason reason, out int count, out bool soulBlocked)
        {
            reason = ShadeAiReason.InRange;
            count = 0;
            soulBlocked = false;
            var tuning = snapshot.Tuning;
            var targets = snapshot.Targets;

            if (snapshot.ShriekUnlocked && snapshot.ShriekReady)
            {
                Vector2 apex = origin + new Vector2(0f, ShriekApexHeight);
                int inCone = CountInShriekCone(targets, apex, tuning.ShriekConeHeight, tuning.ShriekConeHalfAngleDegrees, out bool bossInCone);
                if (Qualifies(inCone, bossInCone, tuning))
                {
                    if (CanAfford(snapshot, snapshot.ShriekSoulCost))
                    {
                        reason = bossInCone ? ShadeAiReason.BossSpell : ShadeAiReason.ClusterSpell;
                        count = inCone;
                        return ShadeAiAction.Shriek;
                    }

                    soulBlocked = true;
                }
            }

            if (snapshot.DescendingDarkUnlocked && snapshot.QuakeReady)
            {
                int inSlam = CountInQuakeArea(targets, origin, tuning, out bool bossInSlam);
                if (Qualifies(inSlam, bossInSlam, tuning))
                {
                    if (CanAfford(snapshot, snapshot.QuakeSoulCost))
                    {
                        reason = bossInSlam ? ShadeAiReason.BossSpell : ShadeAiReason.ClusterSpell;
                        count = inSlam;
                        return ShadeAiAction.DescendingDark;
                    }

                    soulBlocked = true;
                }
            }

            if (snapshot.ProjectileUnlocked && snapshot.FireReady)
            {
                int facing = target.Position.x >= origin.x ? 1 : -1;
                int inLine = CountInProjectileLine(targets, origin, facing, tuning, out bool bossInLine);
                if (Qualifies(inLine, bossInLine, tuning))
                {
                    if (CanAfford(snapshot, snapshot.ProjectileSoulCost))
                    {
                        reason = bossInLine ? ShadeAiReason.BossSpell : ShadeAiReason.ClusterSpell;
                        count = inLine;
                        return ShadeAiAction.Fireball;
                    }

                    soulBlocked = true;
                }
            }

            return ShadeAiAction.None;
        }

        /// <summary>
        /// Looks for somewhere nearby a spell would land from, and returns it when the Shade can get
        /// there in less time than the swing it would otherwise be making. Two units to the left to
        /// catch three enemies in one cone is a better use of that time than one nail hit on one of
        /// them.
        /// <para>
        /// Candidates are derived from the enemies themselves - below a group for the upward cone,
        /// above it for the ground slam, level with it for the projectile - rather than swept blindly
        /// around the Shade, which keeps this to a handful of evaluations per frame.
        /// </para>
        /// <para>
        /// Line of sight is taken from where the Shade is standing now, not from the candidate: the
        /// candidate is by definition less than one attack away, and re-testing it would mean
        /// raycasting per probe per frame. Being slightly conservative here is the right side to err
        /// on - it can decline a cast that would have worked, never take one that could not.
        /// </para>
        /// </summary>
        internal bool TryFindCastPosition(in ShadeAiSnapshot snapshot, in ShadeAiTarget target, float leashRadius, out Vector2 position, out int count)
        {
            position = snapshot.ShadePosition;
            count = 0;

            var targets = snapshot.Targets;
            var tuning = snapshot.Tuning;
            if (targets == null || targets.Count == 0 || snapshot.NailInterval <= 0f || snapshot.MoveSpeed <= 0.01f)
            {
                return false;
            }

            bool anySpellReady = (snapshot.ShriekUnlocked && snapshot.ShriekReady)
                || (snapshot.DescendingDarkUnlocked && snapshot.QuakeReady)
                || (snapshot.ProjectileUnlocked && snapshot.FireReady);
            if (!anySpellReady)
            {
                return false;
            }

            float budget = snapshot.NailInterval * snapshot.MoveSpeed;
            int bestCount = 0;
            Vector2 best = snapshot.ShadePosition;

            for (int i = 0; i < targets.Count; i++)
            {
                var anchor = targets[i];
                if (!anchor.HasLineOfSight)
                {
                    continue;
                }

                EvaluateCastProbe(snapshot, target, anchor.Position - new Vector2(0f, tuning.ShriekConeHeight * 0.35f), leashRadius, budget, ref bestCount, ref best);
                EvaluateCastProbe(snapshot, target, anchor.Position + new Vector2(0f, tuning.QuakeBelowReach * 0.35f), leashRadius, budget, ref bestCount, ref best);
                EvaluateCastProbe(snapshot, target, new Vector2(snapshot.ShadePosition.x, anchor.Position.y), leashRadius, budget, ref bestCount, ref best);
            }

            if (bestCount <= 0)
            {
                return false;
            }

            position = best;
            count = bestCount;
            return true;
        }

        private static void EvaluateCastProbe(in ShadeAiSnapshot snapshot, in ShadeAiTarget target, Vector2 probe, float leashRadius, float budget, ref int bestCount, ref Vector2 best)
        {
            var tuning = snapshot.Tuning;
            probe = ClampToLeash(probe, snapshot.HornetPosition, leashRadius);
            if (snapshot.CanTakeDamage)
            {
                probe = AvoidThreats(probe, snapshot.Threats, tuning.ThreatStandoff);
                if (!IsSafeAt(probe, snapshot.Threats, tuning.ThreatStandoff))
                {
                    return;
                }
            }

            float travel = Vector2.Distance(probe, snapshot.ShadePosition);
            if (travel > budget || travel <= tuning.ArriveDeadzone)
            {
                return;
            }

            // Facing is taken from the committed target, so a probe that only works firing left is
            // still found.
            var spell = ChooseSpell(snapshot, probe, target, out _, out int probeCount, out _);
            if (spell == ShadeAiAction.None || probeCount <= bestCount)
            {
                return;
            }

            bestCount = probeCount;
            best = probe;
        }

        private static bool Qualifies(int hitCount, bool hitsBoss, in ShadeAiTuning tuning)
        {
            return hitCount > 0 && (hitsBoss || hitCount >= tuning.MinClusterForSpell);
        }

        private static bool CanAfford(in ShadeAiSnapshot snapshot, int cost)
        {
            return snapshot.Soul - cost >= snapshot.SoulReserve;
        }

        /// <summary>Enemies inside the upward wedge Shriek spawns.</summary>
        internal static int CountInShriekCone(IReadOnlyList<ShadeAiTarget>? targets, Vector2 apex, float height, float halfAngleDegrees, out bool hitsBoss)
        {
            hitsBoss = false;
            if (targets == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                var candidate = targets[i];
                if (!candidate.HasLineOfSight)
                {
                    continue;
                }

                Vector2 to = candidate.Position - apex;
                if (to.magnitude - candidate.Radius > height)
                {
                    continue;
                }

                // Straight overhead is inside every cone; the angle test is undefined there.
                if (to.sqrMagnitude > 0.0001f && Vector2.Angle(Vector2.up, to) > halfAngleDegrees)
                {
                    continue;
                }

                count++;
                hitsBoss |= candidate.IsBoss;
            }

            return count;
        }

        /// <summary>
        /// Enemies the Descending Dark slam would land on. The Shade dives to the ground beneath
        /// itself and slams there, so the area is a column below it rather than a disc around it.
        /// </summary>
        internal static int CountInQuakeArea(IReadOnlyList<ShadeAiTarget>? targets, Vector2 shadePosition, in ShadeAiTuning tuning, out bool hitsBoss)
        {
            hitsBoss = false;
            if (targets == null)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                var candidate = targets[i];
                if (!candidate.HasLineOfSight)
                {
                    continue;
                }

                if (Mathf.Abs(candidate.Position.x - shadePosition.x) - candidate.Radius > tuning.QuakeRadius)
                {
                    continue;
                }

                float verticalOffset = candidate.Position.y - shadePosition.y;
                if (verticalOffset - candidate.Radius > tuning.QuakeAboveReach)
                {
                    continue;
                }

                if (-verticalOffset - candidate.Radius > tuning.QuakeBelowReach)
                {
                    continue;
                }

                count++;
                hitsBoss |= candidate.IsBoss;
            }

            return count;
        }

        /// <summary>Enemies the flat-flying projectile would pass through on the given side.</summary>
        internal static int CountInProjectileLine(IReadOnlyList<ShadeAiTarget>? targets, Vector2 shadePosition, int facing, in ShadeAiTuning tuning, out bool hitsBoss)
        {
            hitsBoss = false;
            if (targets == null)
            {
                return 0;
            }

            float side = facing >= 0 ? 1f : -1f;
            int count = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                var candidate = targets[i];
                if (!candidate.HasLineOfSight)
                {
                    continue;
                }

                if (Mathf.Abs(candidate.Position.y - shadePosition.y) - candidate.Radius > tuning.FireballVerticalTolerance)
                {
                    continue;
                }

                float along = (candidate.Position.x - shadePosition.x) * side;
                if (along + candidate.Radius < tuning.FireballMinRange || along - candidate.Radius > tuning.FireballMaxRange)
                {
                    continue;
                }

                count++;
                hitsBoss |= candidate.IsBoss;
            }

            return count;
        }
    }
}
