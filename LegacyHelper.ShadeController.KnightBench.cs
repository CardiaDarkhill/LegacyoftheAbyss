#nullable disable
using UnityEngine;
using LegacyoftheAbyss.Shade.Knight;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>
        /// How far the two rendered boxes are made to overlap.
        /// <para>
        /// Large, and it has to be: tk2d bounds are the sprite's mesh, not its ink, and both of
        /// these carry roughly half a unit of empty padding each side. Placing them edge to edge
        /// therefore left a full unit of daylight - the same amount on the left as on the right,
        /// which is what confirms it is padding rather than anything about the seat. The bounds
        /// still do the useful part, which is scaling with whatever pose each character is in; this
        /// takes the padding back out.
        /// </para>
        /// </summary>
        private const float KnightBenchShoulderGap = -1f;

        /// <summary>
        /// Fallback spacing, used only when either character's rendered size cannot be measured.
        /// Centre to centre, which is why it is no use as the main rule: it says nothing about how
        /// wide either of them is, and a seat set by it landed the Knight on top of her.
        /// </summary>
        private const float KnightBenchSideOffset = 1.1f;

        /// <summary>
        /// How far the drawn Knight is raised while seated, so it reads as sitting on the bench
        /// rather than a shade under it. Deliberately small - the Knight still stands on the same
        /// floor Hornet's bench does, and this only makes up the few inches of seat.
        /// </summary>
        private const float KnightBenchSeatLift = 0.18f;

        private const float KnightBenchArriveTolerance = 0.2f;

        /// <summary>
        /// How far off Hornet's level the Knight may be before the seat is given a height as well
        /// as a side. Generous, because the two origins mean different things - hers sits near her
        /// middle and the Knight's at its feet - so a unit of difference is what standing on the
        /// same floor looks like.
        /// </summary>
        private const float KnightBenchElevationTolerance = 1.5f;

        /// <summary>How far below Hornet's level the seat looks for a floor, and from how far above it.</summary>
        private const float KnightBenchGroundProbe = 14f;

        private const float KnightBenchGroundProbeLift = 2f;

        /// <summary>
        /// How far along the bench the seated Knight may be before it walks back. Generous: it only
        /// has to catch a Knight that is no longer at the bench at all, not one settling into it.
        /// </summary>
        private const float KnightBenchDriftTolerance = 3f;

        /// <summary>
        /// How far above or below Hornet the seated Knight may be before it walks back.
        /// <para>
        /// Measured against Hornet rather than against the seat, and loosely, because the two
        /// origins mean different things - hers sits near her middle, the Knight's at its feet -
        /// so a unit or two of difference is what sitting together looks like. Only a Knight that
        /// has genuinely left the bench clears this.
        /// </para>
        /// </summary>
        private const float KnightBenchVerticalDriftTolerance = 5f;

        /// <summary>
        /// How long the Knight is given to walk there before being put there instead. The approach
        /// is a straight walk and nothing more, so a gap, a step or a closed door would otherwise
        /// leave it jogging into scenery for as long as Hornet rests.
        /// </summary>
        private const float KnightBenchWalkSeconds = 2f;

        /// <summary>The lead-in before the sitting loop, matching the length of the Sit clip.</summary>
        private const float KnightBenchSitAnticSeconds = 0.45f;

        private bool knightBenchActive;
        private bool knightBenchSeated;
        private float knightBenchWalkTimer;
        private float knightBenchSeatedAt;
        private float knightBenchTargetX;

        /// <summary>Which side of Hornet, chosen once when she sits and held for the whole rest.</summary>
        private int knightBenchSide;

        /// <summary>
        /// Sends the Knight to sit beside Hornet whenever she rests.
        /// <para>
        /// Runs after the control lock has zeroed the captured input, and writes over it: a bench is
        /// the one scripted hold the companion has something of its own to do. The Shade keeps the
        /// old behaviour and stands aside, because it has no sitting animation to play.
        /// </para>
        /// </summary>
        private void UpdateKnightBench(bool atBench)
        {
            if (!UsesGroundedMovement || knightView == null)
            {
                knightBenchActive = false;
                knightBenchSeated = false;
                return;
            }

            if (!atBench)
            {
                if (knightBenchSeated)
                {
                    knightView.SetLift(0f);
                }

                knightBenchActive = false;
                knightBenchSeated = false;
                return;
            }

            if (!knightBenchActive)
            {
                knightBenchActive = true;
                knightBenchSeated = false;
                knightBenchWalkTimer = KnightBenchWalkSeconds;
                knightBenchSide = ChooseBenchSide();
            }

            capturedMoveInput = Vector2.zero;
            capturedSprintHeld = false;
            capturedHorizontalInput = 0f;

            // The approach is scripted, so drop anything the player queued: a dash or a jump landing
            // mid-walk would carry the Knight past the seat it is heading for.
            knightJumpPressLatched = false;
            knightDashPressLatched = false;
            knightJumpHeld = false;

            knightView.SetLift(knightBenchSeated ? KnightBenchSeatLift : 0f);

            if (knightBenchSeated)
            {
                // The seat is held for as long as Hornet rests, and things happen in that time:
                // the player can teleport the Knight away, and a seat placed over a gap used to
                // drop it. Neither un-seated it, so the hold below kept it wherever it had ended
                // up for the rest of the rest. Re-checked every frame instead, and walked back.
                if (HasDriftedFromBenchSeat())
                {
                    knightBenchSeated = false;
                    knightBenchWalkTimer = KnightBenchWalkSeconds;
                    knightView.SetLift(0f);
                }
                else
                {
                    return;
                }
            }

            // Recomputed every frame of the approach: both silhouettes change as the Knight walks,
            // and Hornet is still settling into her own sitting pose while it does.
            knightBenchTargetX = BenchSeatPosition().x;

            float delta = knightBenchTargetX - transform.position.x;
            if (Mathf.Abs(delta) <= KnightBenchArriveTolerance)
            {
                SeatKnightAtBench(snap: true);
                return;
            }

            knightBenchWalkTimer -= Time.deltaTime;
            if (knightBenchWalkTimer <= 0f)
            {
                SeatKnightAtBench(snap: true);
                return;
            }

            capturedHorizontalInput = Mathf.Sign(delta);
        }

        /// <summary>
        /// Whether the seated Knight has left its seat - teleported away, or dropped out of the
        /// room. Checked every frame while it is seated, because the seat is held for as long as
        /// Hornet rests and nothing else would ever put the Knight back.
        /// </summary>
        private bool HasDriftedFromBenchSeat()
        {
            if (hornetTransform == null)
            {
                return false;
            }

            if (Mathf.Abs(transform.position.x - BenchSeatPosition().x) > KnightBenchDriftTolerance)
            {
                return true;
            }

            return Mathf.Abs(transform.position.y - hornetTransform.position.y) > KnightBenchVerticalDriftTolerance;
        }

        /// <summary>
        /// The side Hornet is facing away from, so the Knight never sits in front of her.
        /// <para>
        /// Chosen from her facing rather than from wherever the Knight happened to be standing.
        /// That earlier rule put the Knight on a different side of the same bench depending on
        /// which way it had walked in from, which is the inconsistency that was reported: the seat
        /// should be a property of Hornet, not of the approach.
        /// </para>
        /// </summary>
        private int ChooseBenchSide()
        {
            var hero = HeroController.UnsafeInstance;
            var heroState = hero != null ? hero.cState : null;
            return heroState != null && heroState.facingRight ? -1 : 1;
        }

        /// <summary>
        /// Where the Knight sits, measured between the two <em>silhouettes</em> rather than between
        /// their transforms.
        /// <para>
        /// Neither origin is where its character is drawn - Hornet's sits near her middle, the
        /// Knight's at its feet, and her seated pose moves her sprite again. A seat set by transform
        /// distance read 0.62 units apart on paper and drew one on top of the other. Renderer bounds
        /// are the only measure that means the same thing to both of them.
        /// </para>
        /// <para>
        /// Horizontal only: the Knight keeps its own gravity and settles on the same ground the
        /// bench stands on, and the height of the seat is made up in the drawing by
        /// <c>KnightView.SetLift</c>.
        /// </para>
        /// </summary>
        private Vector3 BenchSeatPosition()
        {
            if (hornetTransform == null)
            {
                return transform.position;
            }

            float targetX = hornetTransform.position.x + (knightBenchSide * KnightBenchSideOffset);

            if (TryGetHornetBounds(out var hornetBounds)
                && knightView != null
                && knightView.TryGetRenderedBounds(out var knightBounds))
            {
                float drawnX = hornetBounds.center.x
                    + (knightBenchSide * (hornetBounds.extents.x + knightBounds.extents.x + KnightBenchShoulderGap));

                // Moved by the difference rather than set to the value: the body is not the centre
                // of what it draws, and this carries that offset through whatever it happens to be.
                targetX = transform.position.x + (drawnX - knightBounds.center.x);
            }

            return new Vector3(targetX, transform.position.y, transform.position.z);
        }

        /// <summary>Hornet's drawn extent. Her own renderer, not the children - those are her lights.</summary>
        private static bool TryGetHornetBounds(out Bounds bounds)
        {
            bounds = default;
            var hero = HeroController.UnsafeInstance;
            var renderer = hero != null ? hero.GetComponent<Renderer>() : null;
            if (renderer == null)
            {
                return false;
            }

            bounds = renderer.bounds;
            return bounds.size.x > 0.0001f;
        }

        private void SeatKnightAtBench(bool snap)
        {
            if (snap)
            {
                TeleportToPosition(BenchSeatPlacement());
            }

            knightBenchSeated = true;
            knightBenchSeatedAt = Time.time;

            // Both facing the same way, as two people on a bench would - turning to face her reads
            // as an interruption rather than as sitting together. Taken from cState rather than
            // from her scale, which carries no promise about which sign means which way.
            var hero = HeroController.UnsafeInstance;
            var heroState = hero != null ? hero.cState : null;
            if (heroState != null)
            {
                facing = heroState.facingRight ? 1 : -1;
            }
        }

        /// <summary>
        /// Where the Knight is actually put down: the measured seat, and a height to go with it
        /// when the Knight is not already on Hornet's level.
        /// <para>
        /// Horizontal alone was not enough. The approach is a straight walk and nothing more, so a
        /// Knight that is on a ledge above the bench or on the floor below never arrives; the walk
        /// times out and the Knight is placed - and the placement kept its own y, which seated it
        /// in the air beside the bench or under the floor it had been standing on. That is the
        /// report. A seated Knight has its gravity switched off for the whole rest, so it cannot
        /// fall to the floor afterwards either: the height has to be right when it is set down.
        /// </para>
        /// <para>
        /// Still not Hornet's y directly - her origin sits near her middle and the Knight's at its
        /// feet, so matching them stands it at her shoulder. Her level is where the search starts,
        /// and the floor beneath it is where the feet go.
        /// </para>
        /// </summary>
        private Vector3 BenchSeatPlacement()
        {
            Vector3 seat = BenchSeatPosition();
            if (hornetTransform == null || bodyCol == null)
            {
                return seat;
            }

            float rise = hornetTransform.position.y - transform.position.y;
            if (Mathf.Abs(rise) <= KnightBenchElevationTolerance)
            {
                return seat;
            }

            seat.y += rise;

            // Cast from where the feet would be at her level, and move the body by however far
            // they fall. The transform is not the middle of the collider, so the two are measured
            // apart rather than assumed equal. Started a little above the feet, so a seat that
            // lands slightly inside the floor still finds its surface rather than the next one down.
            var bounds = bodyCol.bounds;
            var feet = new Vector2(
                seat.x + (bounds.center.x - transform.position.x),
                seat.y + (bounds.min.y - transform.position.y));

            var hit = Physics2D.Raycast(
                feet + new Vector2(0f, KnightBenchGroundProbeLift),
                Vector2.down,
                KnightBenchGroundProbe + KnightBenchGroundProbeLift,
                KnightTerrainMask());

            if (hit.collider != null)
            {
                seat.y += hit.point.y - feet.y;
            }

            return seat;
        }

        /// <summary>The clip the seated Knight should be holding, or null when it is not seated.</summary>
        private string KnightBenchClip()
        {
            if (!knightBenchSeated)
            {
                return null;
            }

            return Time.time < knightBenchSeatedAt + KnightBenchSitAnticSeconds
                ? KnightView.ClipSit
                : KnightView.ClipSitIdle;
        }
    }
}
