#nullable disable
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        // Where the Knight's body meets the world: what it is standing on, what it is holding, and
        // where a step is allowed to end.
        //
        // Hornet is moved by Unity's physics, which rides her up over small irregularities without
        // anyone asking it to. The Knight is moved by a swept cast, which has no such generosity,
        // and much of Silksong's ground is a few centimetres uneven or seamed between two colliders
        // - so everything here exists to do deliberately what she gets for free.

        private const float KnightGroundProbe = 0.12f;

        /// <summary>
        /// Lifts the body clear when it is already inside terrain. The swept collision below only
        /// stops motion *into* geometry; a Knight spawned or teleported into the floor starts
        /// overlapping and would otherwise stay buried, because every sweep out of it begins in
        /// contact. Uses <c>Collider2D.Distance</c>, which is pure geometry rather than filtered by
        /// the layer collision matrix the Knight and terrain do not share.
        /// </summary>
        private void PushKnightOutOfTerrain()
        {
            if (bodyCol == null)
            {
                return;
            }

            var bounds = bodyCol.bounds;
            var overlaps = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f, KnightTerrainMask());

            for (int i = 0; i < overlaps.Length; i++)
            {
                var other = overlaps[i];
                if (other == null || other == bodyCol || other.transform.IsChildOf(transform))
                {
                    continue;
                }

                var distance = bodyCol.Distance(other);
                if (!distance.isValid || !distance.isOverlapped)
                {
                    continue;
                }

                // The normal runs from this collider toward the other, so separating means moving
                // back along it by the penetration depth.
                float depth = Mathf.Abs(distance.distance);
                transform.position -= (Vector3)(distance.normal * depth);

                if (knightVerticalVelocity < 0f)
                {
                    knightVerticalVelocity = 0f;
                }
            }
        }

        private void ProbeKnightSurroundings()
        {
            knightWasGrounded = knightGrounded;

            var bounds = bodyCol != null ? bodyCol.bounds : new Bounds(transform.position, Vector3.one);
            int mask = KnightTerrainMask();

            Vector2 footCentre = new Vector2(bounds.center.x, bounds.min.y);
            knightGrounded = knightVerticalVelocity <= 0.01f
                && Physics2D.OverlapBox(footCentre, new Vector2(bounds.size.x * 0.85f, KnightGroundProbe), 0f, mask) != null;

            knightWallDirection = 0;
            if (!knightGrounded && knightAbilities.MantisClaw)
            {
                Vector2 side = new Vector2(bounds.size.x * 0.5f + 0.06f, 0f);
                Vector2 centre = bounds.center;
                var probe = new Vector2(0.08f, bounds.size.y * 0.7f);

                if (Physics2D.OverlapBox(centre + side, probe, 0f, mask) != null)
                    knightWallDirection = 1;
                else if (Physics2D.OverlapBox(centre - side, probe, 0f, mask) != null)
                    knightWallDirection = -1;
            }

            if (knightGrounded)
            {
                knightAirJumpSpent = false;
                knightDashSpentInAir = false;
                if (!knightWasGrounded)
                {
                    knightLandTimer = 0.12f;
                }
            }
            else if (knightWallDirection != 0)
            {
                // A wall refunds what the ground refunds. UpdateKnightDash has always said "until
                // the ground or a wall gives it back" and only the ground ever did, so a Knight
                // climbing a shaft had one dash and one air jump for the whole climb. The double
                // jump was refunded by *jumping off* a wall but not by holding one, which is the
                // same omission a step earlier.
                knightAirJumpSpent = false;
                knightDashSpentInAir = false;
            }

            // Momentum is killed the instant the wall is caught, not once the rise has decayed on
            // its own. Jumping into a wall used to carry on upward for several frames before the
            // cling took hold, which is the delay before a wall jump would answer.
            bool clinging = !knightGrounded && knightWallDirection != 0 && knightAbilities.MantisClaw;
            if (clinging && !knightWasClinging && knightVerticalVelocity > 0f)
            {
                knightVerticalVelocity = 0f;
                knightJumpHoldTimer = 0f;
            }

            knightWasClinging = clinging;
        }

        /// <summary>Terrain and soft terrain, for anything of ours that must not pass through walls.</summary>
        internal static int TerrainMask() => KnightTerrainMask();

        private static int KnightTerrainMask()
        {
            int terrain = LayerMask.NameToLayer("Terrain");
            int mask = terrain >= 0 ? 1 << terrain : 0;

            int soft = LayerMask.NameToLayer("Soft Terrain");
            if (soft >= 0)
            {
                mask |= 1 << soft;
            }

            return mask != 0 ? mask : Physics2D.AllLayers;
        }

        /// <summary>How far a sweep stops short of what it hits, so the body never rests inside it.</summary>
        private const float KnightSweepSkin = 0.01f;

        /// <summary>
        /// Sweeps the shape the body actually is.
        /// <para>
        /// The body is a capsule and this used to cast a box over it, which threw away the rounded
        /// bottom corners - and those corners are the whole reason a capsule rides over a small lip
        /// instead of catching on its edge. Sized off the collider's own bounds, as the box was, so
        /// the reach is unchanged.
        /// </para>
        /// </summary>
        private RaycastHit2D KnightSweep(Vector2 origin, Vector2 direction, float distance, int mask)
        {
            Vector2 size = (Vector2)bodyCol.bounds.size * 0.9f;

            if (bodyCol is CapsuleCollider2D capsule)
            {
                return Physics2D.CapsuleCast(origin, size, capsule.direction, 0f, direction, distance, mask);
            }

            return Physics2D.BoxCast(origin, size, 0f, direction, distance, mask);
        }

        /// <summary>
        /// Stops the body at terrain rather than letting MovePosition push it through. Each axis is
        /// swept separately so sliding along a wall or a ceiling still works.
        /// <para>
        /// Hornet is moved by Unity's physics, which rides her up over small irregularities without
        /// anyone asking it to. The Knight is moved by a swept cast, which has no such generosity:
        /// it stops at whatever the sweep touches, and much of Silksong's ground is a few
        /// centimetres uneven or seamed between two colliders. So the step is tried again from
        /// slightly higher up before it is called a wall, and the body is settled back down after -
        /// which is the same thing physics does for her, done deliberately.
        /// </para>
        /// </summary>
        private Vector2 ResolveKnightCollision(Vector2 current, Vector2 target)
        {
            if (bodyCol == null)
            {
                return target;
            }

            int mask = KnightTerrainMask();
            float stepHeight = KnightStepHeight;

            Vector2 resolved = current;

            float dx = target.x - current.x;
            if (Mathf.Abs(dx) > 0.0001f)
            {
                Vector2 direction = new Vector2(Mathf.Sign(dx), 0f);
                float distance = Mathf.Abs(dx);
                var hit = KnightSweep(resolved, direction, distance, mask);

                if (hit.collider == null)
                {
                    resolved.x = target.x;
                }
                else if (!TryStepOver(ref resolved, direction, distance, stepHeight, mask))
                {
                    resolved.x += direction.x * Mathf.Max(0f, hit.distance - KnightSweepSkin);
                }
            }

            float dy = target.y - current.y;
            if (Mathf.Abs(dy) > 0.0001f)
            {
                Vector2 direction = new Vector2(0f, Mathf.Sign(dy));
                var hit = KnightSweep(resolved, direction, Mathf.Abs(dy), mask);
                if (hit.collider != null)
                {
                    resolved.y += direction.y * Mathf.Max(0f, hit.distance - KnightSweepSkin);
                    // Landing or hitting a ceiling both kill vertical speed.
                    knightVerticalVelocity = 0f;
                }
                else
                {
                    resolved.y = target.y;
                }
            }

            SettleKnightOntoGround(ref resolved, current, stepHeight, mask);
            return resolved;
        }

        /// <summary>How high a lip the Knight steps over, in world units, or zero when switched off.</summary>
        private float KnightStepHeight
        {
            get
            {
                if (bodyCol == null)
                {
                    return 0f;
                }

                float share = Mathf.Clamp(ModConfig.Instance.knightStepHeight, 0f, 0.9f);
                return share <= 0f ? 0f : bodyCol.bounds.size.y * share;
            }
        }

        /// <summary>
        /// Retries a blocked horizontal step from <paramref name="stepHeight"/> higher up, and
        /// settles the body back down onto whatever it lands on. Returns false when the obstruction
        /// is a real wall rather than a lip, in which case nothing has been moved.
        /// </summary>
        private bool TryStepOver(ref Vector2 resolved, Vector2 direction, float distance, float stepHeight, int mask)
        {
            // Only from the ground, and only when not already on the way up: stepping mid-jump would
            // let the Knight climb a wall a step at a time.
            if (stepHeight <= 0f || !knightGrounded || knightVerticalVelocity > 0.01f)
            {
                return false;
            }

            // However much headroom there is, up to a step. A low ceiling makes this a wall again.
            var above = KnightSweep(resolved, Vector2.up, stepHeight, mask);
            float lift = above.collider != null ? Mathf.Max(0f, above.distance - KnightSweepSkin) : stepHeight;
            if (lift <= KnightSweepSkin)
            {
                return false;
            }

            Vector2 raised = new Vector2(resolved.x, resolved.y + lift);
            if (KnightSweep(raised, direction, distance, mask).collider != null)
            {
                return false;
            }

            Vector2 crossed = new Vector2(raised.x + direction.x * distance, raised.y);

            // Back down onto the surface that was stepped onto. Falling the whole lift again means
            // there was nothing there after all, which is fine - the vertical pass takes it from
            // here.
            var below = KnightSweep(crossed, Vector2.down, lift, mask);
            float drop = below.collider != null ? Mathf.Max(0f, below.distance - KnightSweepSkin) : lift;

            resolved = new Vector2(crossed.x, crossed.y - drop);
            return true;
        }

        /// <summary>
        /// Keeps a walking Knight on the ground over a small drop.
        /// <para>
        /// Without this the ground falling away by a centimetre leaves the Knight airborne for a
        /// frame, which costs it its grounded state, its coyote time and its walk animation - a
        /// stutter every few steps on ground that looks flat. Only ever pulls the body down onto
        /// something within a step, so walking off an actual ledge still falls.
        /// </para>
        /// </summary>
        private void SettleKnightOntoGround(ref Vector2 resolved, Vector2 current, float stepHeight, int mask)
        {
            if (stepHeight <= 0f || !knightGrounded || knightVerticalVelocity > 0.01f)
            {
                return;
            }

            // Only when the move was a walk. A dash or a knockback should carry the Knight off a
            // lip rather than being pinned to it.
            if (Mathf.Abs(resolved.x - current.x) <= 0.0001f || knightDashTimer > 0f)
            {
                return;
            }

            var ground = KnightSweep(resolved, Vector2.down, stepHeight, mask);
            if (ground.collider == null || ground.distance <= KnightSweepSkin)
            {
                return;
            }

            resolved.y -= Mathf.Max(0f, ground.distance - KnightSweepSkin);
        }
    }
}
