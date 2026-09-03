#nullable disable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LegacyHelper
{
    public class ShadeProjectile : MonoBehaviour
    {
        public int damage = 20;
        public Transform hornetRoot;
        public bool destroyOnTerrain;
        public float maxRange;
        public float lifeSeconds;
        public Sprite[] animFrames;
        public float animFrameTime = 0.1f;

        /// <summary>
        /// Whether the projectile turns to point along its velocity. Off for the bolts, which are
        /// drawn flat and only ever thrown along the ground; on for anything following an arc,
        /// where a sprite held flat reads as the wrong art rather than as aim.
        /// </summary>
        public bool faceVelocity;

        /// <summary>
        /// How far the bolt may shift its height, in total, to stay clear of ground that is not
        /// flat. Zero leaves it bursting on the first thing it touches.
        /// <para>
        /// Hollow Knight's fireball does not steer around terrain - it holds its line and rides up
        /// or down to clear what it is scraping, which is why it crosses a room whose floor rises a
        /// little instead of dying at the first bump. Most of Silksong's floors are not flat, so
        /// without this a Vengeful Spirit rarely reached anything.
        /// </para>
        /// <para>
        /// A budget rather than a per-frame allowance, and spent for good: the clamp is what stops
        /// the same trick walking the bolt up the face of a wall and out of the top of the room.
        /// </para>
        /// </summary>
        public float terrainRide;

        private SpriteRenderer sr;
        private int animFrameIndex;
        private float animTimer;
        private Vector2 spawnPos;
        private HashSet<Collider2D> hitSet;
        private int terrainLayer;
        private int terrainMask;
        private Vector2 rideProbeSize;
        private Vector2 rideProbeOffset;

        /// <summary>
        /// How far the bolt currently sits from the line it was thrown along, signed. Held as a
        /// displacement rather than as a spend so that riding a bump and settling back afterwards
        /// costs nothing - it is how far the bolt has strayed that has to be clamped, not how much
        /// it has moved.
        /// </summary>
        private float rideOffset;

        private Rigidbody2D body;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            hitSet = new HashSet<Collider2D>();
            terrainLayer = LayerMask.NameToLayer("Terrain");
            // No damage multiplier here. The only spawner (SpawnProjectile) assigns damage
            // straight after AddComponent, i.e. after this Awake has already run, so this only
            // ever scaled the field's placeholder default - and now that the Shade's nail and
            // its spells scale separately, applying the nail's multiplier to a fireball would
            // be wrong even if it did reach anything. ComputeSpellDamageMultiplier owns it.
        }

        void Start()
        {
            spawnPos = transform.position;
            if (lifeSeconds > 0f)
                Destroy(gameObject, lifeSeconds);

            MeasureRideProbe();
        }

        /// <summary>
        /// The box the ride test asks about: everything this projectile collides with, taken once.
        /// <para>
        /// Kept as a size and an offset from the transform rather than as world bounds, because the
        /// probe has to be asked about positions the projectile is not standing in yet - the whole
        /// point is to find out whether a height would be clear before moving to it.
        /// </para>
        /// </summary>
        private void MeasureRideProbe()
        {
            terrainMask = terrainLayer >= 0 ? 1 << terrainLayer : 0;
            if (terrainMask == 0 || terrainRide <= 0f)
            {
                return;
            }

            bool measured = false;
            Bounds union = default;

            foreach (var collider in GetComponents<Collider2D>())
            {
                if (collider == null)
                {
                    continue;
                }

                if (!measured)
                {
                    union = collider.bounds;
                    measured = true;
                }
                else
                {
                    union.Encapsulate(collider.bounds);
                }
            }

            if (!measured || union.size.x <= 0.0001f || union.size.y <= 0.0001f)
            {
                terrainRide = 0f;
                return;
            }

            // A little under the drawn hitbox. The probe is asking "would this height be clear",
            // and a box the exact size of the collider answers no while merely brushing a surface
            // the bolt is already flying along.
            rideProbeSize = (Vector2)union.size * 0.85f;
            rideProbeOffset = (Vector2)union.center - (Vector2)transform.position;
        }

        /// <summary>How far the bolt tries at a time when looking for a height that is clear.</summary>
        private const float RideStep = 0.1f;

        /// <summary>
        /// How much further the bolt may go in <paramref name="direction"/> without straying more
        /// than <paramref name="budget"/> from the line it was thrown along.
        /// <para>
        /// Measured from where it currently sits, so a bolt that has ridden all the way up may
        /// still come back down the other side - the clamp is on how far it has strayed, not on how
        /// far it has moved. That is what makes a floor of successive bumps survivable while a wall
        /// still stops it.
        /// </para>
        /// </summary>
        internal static float RideLimit(float budget, float offset, int direction)
            => Mathf.Max(0f, budget - (direction * offset));

        /// <summary>
        /// Holds the bolt's line and lifts it over what it is scraping, or bursts it when nothing
        /// within the budget is clear - which is what a wall looks like from here.
        /// <para>
        /// Every height is tested before the bolt is moved to it, so a wall costs no visible drift
        /// at all: the bolt bursts where it arrived rather than climbing the face first.
        /// </para>
        /// </summary>
        private void FixedUpdate()
        {
            if (terrainRide <= 0f || terrainMask == 0)
            {
                return;
            }

            Vector2 centre = (Vector2)transform.position + rideProbeOffset;
            if (!Physics2D.OverlapBox(centre, rideProbeSize, 0f, terrainMask))
            {
                return;
            }

            int direction = ResolveRideDirection(centre);
            float limit = RideLimit(terrainRide, rideOffset, direction);

            for (float lift = RideStep; lift <= limit; lift += RideStep)
            {
                if (Physics2D.OverlapBox(centre + new Vector2(0f, direction * lift), rideProbeSize, 0f, terrainMask))
                {
                    continue;
                }

                float shift = direction * lift;
                if (body != null)
                {
                    body.position += new Vector2(0f, shift);
                }
                else
                {
                    transform.position += new Vector3(0f, shift, 0f);
                }

                rideOffset += shift;
                return;
            }

            if (destroyOnTerrain)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Which way out of the ground to try, taken from the surface rather than assumed.
        /// <para>
        /// Up for a floor and down for a ceiling, read off which side of the probe is clear. A bump
        /// in the floor and a low beam are the same problem and want opposite answers, and guessing
        /// up for both means the bolt tries to climb through the beam and bursts on it.
        /// </para>
        /// </summary>
        private int ResolveRideDirection(Vector2 centre)
        {
            bool clearAbove = !Physics2D.OverlapBox(centre + new Vector2(0f, RideStep), rideProbeSize, 0f, terrainMask);
            bool clearBelow = !Physics2D.OverlapBox(centre - new Vector2(0f, RideStep), rideProbeSize, 0f, terrainMask);

            if (clearAbove == clearBelow)
            {
                // Buried either way, or free either way. Up, which is the floor - the case this
                // exists for.
                return 1;
            }

            return clearAbove ? 1 : -1;
        }

        void Update()
        {
            if (sr != null && animFrames != null && animFrames.Length > 1)
            {
                animTimer += Time.deltaTime;
                if (animTimer >= animFrameTime)
                {
                    animTimer -= animFrameTime;
                    animFrameIndex = (animFrameIndex + 1) % animFrames.Length;
                    sr.sprite = animFrames[animFrameIndex];
                }
            }

            if (faceVelocity)
            {
                Vector2 velocity = body != null ? body.linearVelocity : Vector2.zero;
                if (velocity.sqrMagnitude > 0.01f)
                {
                    // Re-read every frame, not set once: under gravity the direction of travel
                    // keeps changing, which is the whole point of showing it.
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
                }
            }

            if (maxRange > 0f && Vector2.Distance(spawnPos, transform.position) >= maxRange)
                Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;
            if (hitSet.Contains(other)) return;
            hitSet.Add(other);

            Vector2 vel = body ? body.linearVelocity : (Vector2)(other.transform.position - transform.position);
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle += 360f;

            var hit = new HitInstance
            {
                Source = gameObject,
                AttackType = AttackTypes.Spell,
                DamageDealt = damage,
                Direction = angle,
                MagnitudeMultiplier = 1f,
                Multiplier = 1f,
                IsHeroDamage = true,
                IsFirstHit = true
            };

            // Once, not twice. HitTaker.Hit already walks up to three parents collecting every
            // IHitResponder and hits each of them, and an enemy's HealthManager *is* one of those -
            // TryGetHealthManager below it is the identical walk, so anything it finds has just been
            // hit. The extra call was therefore a second full application of the same HitInstance,
            // and every spell the Shade has ever cast landed for double its stated damage. That is
            // the Howling Wraiths report: 14 on paper, 28 in the enemy.
            HitTaker.Hit(other.gameObject, hit);

            // Terrain is left to the ride above when there is a budget for it, because bursting
            // here would be doing it on contact - before anything has asked whether a slightly
            // different height would have cleared the obstacle.
            if (destroyOnTerrain && terrainRide <= 0f && terrainLayer >= 0 && other.gameObject.layer == terrainLayer)
                Destroy(gameObject);
        }
    }

    public class ShadeAoE : MonoBehaviour
    {
        public int damage = 20;
        public Transform hornetRoot;
        public float lifeSeconds = 0.25f;
        public GameObject sourceOverride;
        public AttackTypes attackType = AttackTypes.Spell;
        public float direction = 90f;
        public float magnitudeMultiplier = 1f;
        public float multiplier = 1f;
        public bool isHeroDamage = true;
        public bool isFirstHit = true;

        /// <summary>
        /// Seconds before the same target can be hit again. Zero is the original behaviour, one hit
        /// per target for the volume's whole life, which is what a burst wants; a lingering cloud
        /// wants its damage spread over the time it stands, so it sets an interval instead.
        /// </summary>
        public float hitIntervalSeconds;

        private Dictionary<Collider2D, float> nextHitTimes;

        void Awake()
        {
            nextHitTimes = new Dictionary<Collider2D, float>();
        }

        public void ConfigureDamage(int amount, bool applyDamageMultiplier)
        {
            if (applyDamageMultiplier)
            {
                damage = Mathf.Max(0, Mathf.RoundToInt(amount * ModConfig.Instance.shadeDamageMultiplier));
            }
            else
            {
                damage = Mathf.Max(0, amount);
            }
        }

        void Start()
        {
            if (lifeSeconds > 0f)
                Destroy(gameObject, lifeSeconds);
        }

        void OnTriggerEnter2D(Collider2D other) => TryHit(other);

        // Only does anything for a volume with an interval set; a plain burst records a next-hit
        // time it can never reach, so repeats cost one dictionary lookup and stop there.
        void OnTriggerStay2D(Collider2D other) => TryHit(other);

        private void TryHit(Collider2D other)
        {
            if (other == null) return;
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;
            if (nextHitTimes.TryGetValue(other, out float nextAllowed) && Time.time < nextAllowed) return;
            nextHitTimes[other] = hitIntervalSeconds > 0f
                ? Time.time + hitIntervalSeconds
                : float.PositiveInfinity;

            var hit = new HitInstance
            {
                Source = sourceOverride ? sourceOverride : gameObject,
                AttackType = attackType,
                DamageDealt = damage,
                Direction = direction,
                MagnitudeMultiplier = magnitudeMultiplier,
                Multiplier = multiplier,
                IsHeroDamage = isHeroDamage,
                IsFirstHit = isFirstHit
            };

            // Once, not twice. HitTaker.Hit already walks up to three parents collecting every
            // IHitResponder and hits each of them, and an enemy's HealthManager *is* one of those -
            // TryGetHealthManager below it is the identical walk, so anything it finds has just been
            // hit. The extra call was therefore a second full application of the same HitInstance,
            // and every spell the Shade has ever cast landed for double its stated damage. That is
            // the Howling Wraiths report: 14 on paper, 28 in the enemy.
            HitTaker.Hit(other.gameObject, hit);
        }
    }
}

#nullable restore
