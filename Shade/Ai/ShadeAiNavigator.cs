#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// Turns "I want to be over there" into "so head this way", going around terrain instead of into
    /// it. The Shade is a solid body - non-trigger collider on a layer that collides with the level
    /// (<c>SetupPhysics</c>) - so steering straight at a destination grinds along any wall between.
    /// <para>
    /// Local steering, not a path search: it sweeps the direct line and, if blocked, fans outward
    /// until it finds a heading that is not. Enough for corners, pillars and doorways; <b>not</b>
    /// enough for a concave trap, since nothing here searches beyond what one sweep can see. A
    /// dead-end alcove facing away from the target will still hold it.
    /// </para>
    /// </summary>
    internal sealed class ShadeAiNavigator
    {
        /// <summary>How far ahead to look. Beyond this the detour is guesswork anyway.</summary>
        private const float MaxProbeDistance = 7f;

        /// <summary>
        /// How far a detour has to be clear before it is worth taking. Must stay well below
        /// <see cref="MaxProbeDistance"/>: a detour only has to clear the corner in front of it, and
        /// the heading is re-derived every frame. Probing the full distance rejects every shallow
        /// heading near anything wide and settles on the perpendicular one, routing around a ledge
        /// as though it were a building.
        /// </summary>
        private const float DetourProbeDistance = 2.5f;

        /// <summary>How far off the direct heading each fan step turns.</summary>
        private const float FanStepDegrees = 22.5f;

        /// <summary>
        /// How far round the fan may reach. Ninety degrees - along the face of whatever is in the way
        /// - and no further: past that the Shade heads away from where it wants to be, which is
        /// <see cref="TrackProgress"/>'s call once the shallow detours have been shown not to work.
        /// </summary>
        private const int FanSteps = 4;

        /// <summary>
        /// How long the direct line has to stay clear before a detour is dropped. Long enough to
        /// ignore a single clear frame at a ledge lip, short enough not to sail past the gap. Not
        /// <see cref="SideCommitSeconds"/>, which answers a different question.
        /// </summary>
        private const float DirectClearSeconds = 0.15f;

        /// <summary>
        /// How long a chosen way round an obstacle stands before the other side may be considered.
        /// Without it the Shade alternates left and right every frame at any obstacle whose two
        /// detours are near equal, vibrating on the spot instead of going round.
        /// </summary>
        private const float SideCommitSeconds = 0.8f;

        /// <summary>
        /// How long the Shade may make no progress toward its destination before the navigator
        /// decides the way it chose is a dead end and tries the other one.
        /// </summary>
        private const float StuckSeconds = 1.1f;

        /// <summary>Progress smaller than this over that window is not progress.</summary>
        private const float StuckProgressEpsilon = 0.4f;

        private int committedSide;
        private float sideCommitUntil;

        // The chosen detour is latched, not just the side it was on: at a ledge lip the direct line
        // flickers between blocked and clear, and recomputing the fan on each flip makes it bounce.
        private Vector2 committedHeading;
        private float headingCommitUntil;

        private float progressDistance = float.MaxValue;
        private float progressCheckedAt;

        /// <summary>When the direct line last became clear, or 0 while it is blocked.</summary>
        private float directClearSince;

        /// <summary>True when the last call had to route around something. Diagnostics only.</summary>
        internal bool LastPathBlocked { get; private set; }

        /// <summary>True when it has stopped making headway and is trying the other way round.</summary>
        internal bool LastPathStuck { get; private set; }

        internal void Reset()
        {
            committedSide = 0;
            sideCommitUntil = 0f;
            committedHeading = Vector2.zero;
            headingCommitUntil = 0f;
            progressDistance = float.MaxValue;
            progressCheckedAt = 0f;
            directClearSince = 0f;
            LastPathBlocked = false;
            LastPathStuck = false;
        }

        /// <summary>
        /// A unit heading toward <paramref name="desired"/> that is clear of terrain, or
        /// <see cref="Vector2.zero"/> when there is nowhere to go. Returns the direct heading when
        /// nothing is in the way, and when everything is - a Shade that is fully boxed in should
        /// press against the wall it wants to be past rather than stand still, because the level
        /// moves and it does not.
        /// </summary>
        internal Vector2 Steer(Vector2 origin, Vector2 desired, float bodyRadius, float time)
        {
            return Steer(origin, desired, bodyRadius, time, null, 0f);
        }

        /// <param name="threats">
        /// Damaging volumes to route around, treated exactly like walls. Without this the Shade walks
        /// through an enemy to reach a spot behind it and eats the contact damage; with it, it goes
        /// round. Threats already containing <paramref name="origin"/> are ignored - steering cannot
        /// avoid something the Shade is already inside, and pretending otherwise blocks every heading
        /// including the ones that lead out.
        /// </param>
        internal Vector2 Steer(Vector2 origin, Vector2 desired, float bodyRadius, float time, IReadOnlyList<ShadeAiThreat>? threats, float threatStandoff)
        {
            LastPathBlocked = false;

            Vector2 toDesired = desired - origin;
            float distance = toDesired.magnitude;
            if (distance <= 0.0001f)
            {
                Reset();
                return Vector2.zero;
            }

            Vector2 direct = toDesired / distance;
            float probe = Mathf.Min(distance, MaxProbeDistance);
            float detourProbe = Mathf.Min(distance, DetourProbeDistance);

            TrackProgress(distance, time);

            if (!Blocked(origin, direct, probe, bodyRadius, threats, threatStandoff))
            {
                // Only drop the detour once the direct line has been open long enough to trust.
                if (directClearSince <= 0f)
                {
                    directClearSince = time;
                }

                if (time - directClearSince >= DirectClearSeconds)
                {
                    committedSide = 0;
                    committedHeading = Vector2.zero;
                    headingCommitUntil = 0f;
                    return direct;
                }
            }
            else
            {
                directClearSince = 0f;
            }

            LastPathBlocked = true;

            // Keep following a detour that is still viable rather than re-deriving one.
            if (committedHeading.sqrMagnitude > 0.0001f
                && time < headingCommitUntil
                && !Blocked(origin, committedHeading, detourProbe, bodyRadius, threats, threatStandoff))
            {
                return committedHeading;
            }

            int preferred = ResolvePreferredSide(origin, direct, detourProbe, bodyRadius, time, threats, threatStandoff);

            for (int step = 1; step <= FanSteps; step++)
            {
                float angle = step * FanStepDegrees;

                // Preferred side first at each width, so the two sides stay interleaved and the
                // Shade takes the shallowest detour rather than the first side that happens to work.
                for (int i = 0; i < 2; i++)
                {
                    int side = i == 0 ? preferred : -preferred;
                    Vector2 candidate = Rotate(direct, angle * side);
                    if (Blocked(origin, candidate, detourProbe, bodyRadius, threats, threatStandoff))
                    {
                        continue;
                    }

                    committedSide = side;
                    sideCommitUntil = time + SideCommitSeconds;
                    committedHeading = candidate;
                    headingCommitUntil = time + SideCommitSeconds;
                    return candidate;
                }
            }

            committedHeading = Vector2.zero;
            return direct;
        }

        /// <summary>
        /// Watches whether the destination is actually getting closer. Local steering cannot know a
        /// detour leads nowhere, only notice afterwards that it is no nearer - the wrong way round a
        /// ledge is not blocked, just endless - so a stalled side is dropped for the other one.
        /// </summary>
        private void TrackProgress(float distance, float time)
        {
            LastPathStuck = false;

            if (distance < progressDistance - StuckProgressEpsilon || progressCheckedAt <= 0f)
            {
                progressDistance = distance;
                progressCheckedAt = time;
                return;
            }

            if (time - progressCheckedAt < StuckSeconds)
            {
                return;
            }

            LastPathStuck = true;
            progressDistance = distance;
            progressCheckedAt = time;

            // Force the other way round, and hold it long enough to actually get somewhere.
            committedSide = committedSide == 0 ? 1 : -committedSide;
            sideCommitUntil = time + (SideCommitSeconds * 2f);
            committedHeading = Vector2.zero;
            headingCommitUntil = 0f;
        }

        /// <summary>
        /// Which way to try first. A commitment that has not expired stands; otherwise pick the side
        /// with more room, so the Shade goes the short way round a corner rather than the long way.
        /// </summary>
        private int ResolvePreferredSide(Vector2 origin, Vector2 direct, float probe, float bodyRadius, float time, IReadOnlyList<ShadeAiThreat>? threats, float threatStandoff)
        {
            if (committedSide != 0 && time < sideCommitUntil)
            {
                return committedSide;
            }

            Vector2 left = Rotate(direct, 90f);
            bool leftBlocked = Blocked(origin, left, probe, bodyRadius, threats, threatStandoff);
            bool rightBlocked = Blocked(origin, -left, probe, bodyRadius, threats, threatStandoff);

            if (leftBlocked != rightBlocked)
            {
                return leftBlocked ? -1 : 1;
            }

            // Equally open or equally boxed in: keep whatever was chosen last rather than flipping.
            return committedSide != 0 ? committedSide : 1;
        }

        private static bool Blocked(Vector2 origin, Vector2 direction, float distance, float bodyRadius, IReadOnlyList<ShadeAiThreat>? threats, float threatStandoff)
        {
            return ShadeAiTerrain.SweepBlocked(origin, direction, distance, bodyRadius)
                || ThreatBlocks(threats, origin, direction, distance, bodyRadius, threatStandoff);
        }

        /// <summary>
        /// Whether travelling along a heading would put the Shade inside a damaging volume. Pure
        /// geometry - a circle against a swept segment - so unlike the terrain half it can be tested
        /// without an engine.
        /// </summary>
        internal static bool ThreatBlocks(IReadOnlyList<ShadeAiThreat>? threats, Vector2 origin, Vector2 direction, float distance, float bodyRadius, float threatStandoff)
        {
            if (threats == null || threats.Count == 0 || distance <= 0f || direction.sqrMagnitude <= 0.0001f)
            {
                return false;
            }

            Vector2 end = origin + (direction.normalized * distance);
            for (int i = 0; i < threats.Count; i++)
            {
                var threat = threats[i];
                float clearance = threat.Radius + threatStandoff + bodyRadius;

                // Already inside it: no heading avoids this one, and treating it as blocking would
                // reject the headings that lead back out.
                if (Vector2.Distance(threat.Position, origin) <= clearance)
                {
                    continue;
                }

                if (DistanceToSegment(threat.Position, origin, end) < clearance)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Walks a destination back toward the Shade until it is somewhere the Shade could stand, or
        /// gives up and returns the origin. Threat avoidance pushes destinations out of hitboxes with
        /// no idea where the walls are, so in a closed boss arena it can land one inside the wall.
        /// <para>
        /// Pulls back along the line rather than searching outward, so the destination stays in the
        /// direction the brain wanted: a target round a corner is not <i>inside</i> anything and is
        /// left alone, and only genuinely unstandable points move.
        /// </para>
        /// </summary>

        internal static Vector2 PullBackToStandable(Vector2 origin, Vector2 desired, Func<Vector2, bool> blocked, int steps = 5)
        {
            if (blocked == null || !blocked(desired))
            {
                return desired;
            }

            for (int i = 1; i <= steps; i++)
            {
                float t = 1f - (i / (float)(steps + 1));
                Vector2 candidate = Vector2.Lerp(origin, desired, t);
                if (!blocked(candidate))
                {
                    return candidate;
                }
            }

            // Everything between here and there is solid. Standing still beats pressing into a wall.
            return origin;
        }

        internal static float DistanceToSegment(Vector2 point, Vector2 from, Vector2 to)
        {
            Vector2 span = to - from;
            float lengthSqr = span.sqrMagnitude;
            if (lengthSqr <= 0.0001f)
            {
                return Vector2.Distance(point, from);
            }

            float t = Mathf.Clamp01(Vector2.Dot(point - from, span) / lengthSqr);
            return Vector2.Distance(point, from + (span * t));
        }

        internal static Vector2 Rotate(Vector2 value, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2((value.x * cos) - (value.y * sin), (value.x * sin) + (value.y * cos));
        }
    }
}
