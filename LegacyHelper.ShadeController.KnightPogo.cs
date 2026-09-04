#nullable disable
using GlobalEnums;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        // The down slash's bounce, and the launch a balloon gives instead of one.
        //
        // What may be bounced off is read from Hornet's own rule in HeroDownAttack rather than
        // invented - see ClassifyKnightPogoSurface - because the game's verticality is built out of
        // bouncers, tinkable fixtures and breakables that carry no HealthManager at all.

        private const float KnightPogoSpeed = 16f;

        /// <summary>
        /// Bounces the Knight upward off whatever it just down-slashed, and gives back the air jump
        /// and air dash exactly as landing would.
        /// </summary>
        internal void ApplyKnightPogoBounce()
        {
            knightVerticalVelocity = KnightPogoSpeed;
            knightJumpHoldTimer = 0f;
            knightAirJumpSpent = false;
            knightDashSpentInAir = false;
        }

        /// <summary>
        /// The balloon's own launch, read off <c>BounceBalloon</c>: 18 units a second for half a
        /// second, gravity held off throughout - a nine unit rise, where an ordinary jump is about
        /// two. Hornet gets this from the balloon itself; the Knight cannot, because every line of
        /// that routine drives <c>HeroController</c> directly, which is why a balloon did nothing
        /// for it at all.
        /// </summary>
        private const float KnightBalloonRiseSpeed = 18f;

        private const float KnightBalloonRiseSeconds = 0.5f;

        private float knightBalloonRiseTimer;

        /// <summary>
        /// Starts the balloon launch. Controls are held for its duration, as they are for Hornet,
        /// and the air moves come back at the top, so the rise can be carried on from with a jump
        /// or a dash exactly as hers can.
        /// </summary>
        internal void BeginKnightBalloonLaunch()
        {
            if (!UsesGroundedMovement)
            {
                return;
            }

            knightBalloonRiseTimer = KnightBalloonRiseSeconds;
            knightDashTimer = 0f;
            knockbackVelocity = Vector2.zero;
            knockbackTimer = 0f;
            EndKnightCastFreeze();
        }

        /// <summary>
        /// Drives the launch. Returns true while it owns the Knight, in which case nothing else
        /// this frame may move it.
        /// </summary>
        private bool UpdateKnightBalloonLaunch(float dt)
        {
            if (knightBalloonRiseTimer <= 0f)
            {
                return false;
            }

            knightBalloonRiseTimer -= dt;
            knightVerticalVelocity = KnightBalloonRiseSpeed;
            knightJumpHoldTimer = 0f;
            knightJumpBufferTimer = 0f;
            knightDashPressLatched = false;

            if (knightBalloonRiseTimer <= 0f)
            {
                knightBalloonRiseTimer = 0f;
                knightAirJumpSpent = false;
                knightDashSpentInAir = false;
            }

            return true;
        }

        /// <summary>How far below the Knight a down slash looks for something to bounce off.</summary>
        private const float KnightPogoProbeDepth = 0.9f;

        /// <summary>
        /// How far it looks for Hornet, which is a good deal further.
        /// <para>
        /// Her collider is nothing like her silhouette - most of her head carries none of it - so a
        /// probe sized for the drawing missed her repeatedly from heights that plainly looked like a
        /// hit. She is the one pogo target the player aims at deliberately and the one the Knight's
        /// verticality depends on, so she gets a reach that forgives the difference.
        /// </para>
        /// </summary>
        private const float KnightHornetPogoProbeDepth = 2.6f;

        /// <summary>
        /// A down slash looking for a surface to bounce off. Hornet counts, which is the point:
        /// without her as a platform most of the game's verticality is closed to the Knight.
        /// Called after the slash is thrown, so the bounce and the swing stay in step.
        /// </summary>
        private bool TryKnightPogo(GameObject slash)
        {
            if (!UsesGroundedMovement || knightGrounded)
            {
                return false;
            }

            var bounds = bodyCol != null ? bodyCol.bounds : new Bounds(transform.position, Vector3.one);
            var probeCentre = new Vector2(bounds.center.x, bounds.min.y - KnightPogoProbeDepth * 0.5f);
            var probeSize = new Vector2(bounds.size.x * 1.1f, KnightPogoProbeDepth);

            // Widened to whatever the swing itself covers below the Knight. The nail's hitbox and
            // this probe were sized independently and the nail was the larger of the two, which is
            // the worst way round it could be: the object flashes, registers the hit and gives no
            // height back, which reads as the pogo being broken rather than as two numbers
            // disagreeing.
            ExpandProbeToSlashReach(ref probeCentre, ref probeSize, bounds, slash);

            if (TryKnightPogoIn(probeCentre, probeSize, hornetOnly: false))
            {
                return true;
            }

            // A second, deeper pass that will only accept Hornet. Kept separate rather than simply
            // probing deeper, so the extra reach cannot pick up an enemy or a ledge the player was
            // nowhere near aiming at.
            var deepCentre = new Vector2(bounds.center.x, bounds.min.y - KnightHornetPogoProbeDepth * 0.5f);
            var deepSize = new Vector2(bounds.size.x * 1.1f, KnightHornetPogoProbeDepth);
            return TryKnightPogoIn(deepCentre, deepSize, hornetOnly: true);
        }

        private bool TryKnightPogoIn(Vector2 probeCentre, Vector2 probeSize, bool hornetOnly)
        {
            // An explicit mask keeps this a geometry query rather than one filtered by the layer
            // collision matrix, which the Knight and Hornet do not share.
            var hits = Physics2D.OverlapBoxAll(probeCentre, probeSize, 0f, Physics2D.AllLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit == null || hit.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (hornetOnly && !IsHornetCollider(hit))
                {
                    continue;
                }

                switch (ClassifyKnightPogoSurface(hit))
                {
                    case KnightPogoKind.Launch:
                        BeginKnightBalloonLaunch();
                        return true;
                    case KnightPogoKind.Bounce:
                        ApplyKnightPogoBounce();
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Grows the pogo probe to cover the part of the swing that is below the Knight.
        /// <para>
        /// Only downward: a down slash's collider reaches out to the sides as well, and a pogo is
        /// about what is underneath. Only when the colliders measure as something - a slash caught
        /// before its damager is switched on reports empty bounds, and in that case the probe is
        /// left at its own size rather than being collapsed to nothing.
        /// </para>
        /// </summary>
        private static void ExpandProbeToSlashReach(
            ref Vector2 probeCentre, ref Vector2 probeSize, Bounds body, GameObject slash)
        {
            if (slash == null)
            {
                return;
            }

            bool measured = false;
            Bounds swing = default;

            foreach (var collider in slash.GetComponentsInChildren<Collider2D>(true))
            {
                if (collider == null)
                {
                    continue;
                }

                var colliderBounds = collider.bounds;
                if (colliderBounds.size.x <= 0.0001f || colliderBounds.size.y <= 0.0001f)
                {
                    continue;
                }

                if (!measured)
                {
                    swing = colliderBounds;
                    measured = true;
                }
                else
                {
                    swing.Encapsulate(colliderBounds);
                }
            }

            if (!measured)
            {
                return;
            }

            float top = Mathf.Min(swing.max.y, body.min.y);
            float bottom = Mathf.Min(swing.min.y, top);
            if (top - bottom <= 0.0001f)
            {
                return;
            }

            var probe = new Bounds(
                new Vector3(probeCentre.x, probeCentre.y, 0f),
                new Vector3(probeSize.x, probeSize.y, 1f));

            probe.Encapsulate(new Vector3(swing.min.x, bottom, 0f));
            probe.Encapsulate(new Vector3(swing.max.x, top, 0f));

            probeCentre = new Vector2(probe.center.x, probe.center.y);
            probeSize = new Vector2(probe.size.x, probe.size.y);
        }

        private static bool IsHornetCollider(Collider2D collider)
        {
            var hero = HeroController.UnsafeInstance;
            return hero != null
                && hero.transform != null
                && collider.transform != null
                && collider.transform.IsChildOf(hero.transform);
        }

        /// <summary>What a down slash gets from a surface, if anything.</summary>
        private enum KnightPogoKind
        {
            None,
            Bounce,
            Launch
        }

        /// <summary>
        /// Whether a down slash bounces off this, read off Hornet's own rule in
        /// <c>HeroDownAttack</c> rather than invented.
        /// <para>
        /// This used to be "an enemy, or Hornet". That is most of what a pogo is aimed at and none
        /// of what the game's verticality is built from: the bouncers, tinkable fixtures, levers and
        /// breakables Hornet chains together carry no <c>HealthManager</c> and sit on the interactive
        /// and bouncer layers, so environment pogoing did not exist for the Knight at all.
        /// </para>
        /// <para>
        /// The hero plane test is the other half, and it is what a report about pogoing "this
        /// specific background object" turned out to be. Silksong's background scenery is the same
        /// prop as the foreground one, pushed back in z with its colliders intact - the game tells
        /// the two apart with <c>Extensions.IsOnHeroPlane</c>, and <c>Breakable</c> uses exactly that
        /// to decide whether to switch itself off. A geometry probe sees straight through the
        /// distinction unless it asks.
        /// </para>
        /// </summary>
        private static KnightPogoKind ClassifyKnightPogoSurface(Collider2D collider)
        {
            if (IsHornetCollider(collider))
            {
                return KnightPogoKind.Bounce;
            }

            var transform = collider.transform;
            if (transform == null || !transform.IsOnHeroPlane())
            {
                return KnightPogoKind.None;
            }

            var go = collider.gameObject;
            var layer = (PhysLayers)go.layer;

            // Ignore Raycast is the game's switched-off layer - HeroController moves itself there on
            // death and enemies are parked there once a fight is over. The damage path already
            // refuses it; without the same refusal here the Knight bounces off a corpse's collider,
            // because the HealthManager test below does not care that the fight has ended.
            if (layer == PhysLayers.IGNORE_RAYCAST)
            {
                return KnightPogoKind.None;
            }

            // The bouncer family is asked about before NonBouncer, because they carry one - a
            // BouncePod adds one to itself in Awake, and HeroDownAttack names BounceBalloon
            // alongside NonBouncer in its own refusal. That is not "do not bounce off this", it is
            // "the object will handle the bounce", and every line of the handling drives
            // HeroController directly. So the Knight has to do it here or get nothing at all, which
            // is what a balloon and a hanging pod were both giving it.
            if (go.GetComponentInParent<BounceBalloon>() != null)
            {
                return KnightPogoKind.Launch;
            }

            if (go.GetComponentInParent<BouncePod>() != null)
            {
                // A pod is an ordinary bounce even for Hornet: DoBounceOff calls DownspikeBounce,
                // where the balloon runs a scripted updraft instead.
                return KnightPogoKind.Bounce;
            }

            // Terrain is what a pogo happens above, never off, and it is the layer most of the
            // world is on - so it is refused before anything else is asked.
            if (layer == PhysLayers.TERRAIN || layer == PhysLayers.SOFT_TERRAIN)
            {
                return KnightPogoKind.None;
            }

            // The game's own opt-out, honoured wherever it is set. A breakable adds one to itself
            // when it is made inert, which is part of why background props must not be bounced off.
            var nonBouncer = go.GetComponentInParent<NonBouncer>();
            if (nonBouncer != null && nonBouncer.active)
            {
                return KnightPogoKind.None;
            }

            if (collider.GetComponentInParent<HealthManager>() != null)
            {
                return KnightPogoKind.Bounce;
            }

            switch (layer)
            {
                case PhysLayers.ENEMIES:
                case PhysLayers.INTERACTIVE_OBJECT:
                case PhysLayers.HERO_ATTACK:
                case PhysLayers.BOUNCER:
                    return KnightPogoKind.Bounce;
            }

            // A tink or a spike-slash reaction is the game saying the nail stops here, which is
            // exactly the surface a pogo wants.
            bool nailStopsHere = go.GetComponentInParent<TinkEffect>() != null
                || go.GetComponentInParent<SpikeSlashReaction>() != null;

            return nailStopsHere ? KnightPogoKind.Bounce : KnightPogoKind.None;
        }
    }
}
