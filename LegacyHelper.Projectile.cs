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
        private SpriteRenderer sr;
        private int animFrameIndex;
        private float animTimer;
        private Vector2 spawnPos;
        private HashSet<Collider2D> hitSet;
        private int terrainLayer;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
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
                var body = GetComponent<Rigidbody2D>();
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

            var rb = GetComponent<Rigidbody2D>();
            Vector2 vel = rb ? rb.linearVelocity : (other.transform.position - transform.position);
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

            HitTaker.Hit(other.gameObject, hit);
            if (HitTaker.TryGetHealthManager(other.gameObject, out var hm))
                hm.Hit(hit);

            if (destroyOnTerrain && terrainLayer >= 0 && other.gameObject.layer == terrainLayer)
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

            HitTaker.Hit(other.gameObject, hit);
            if (HitTaker.TryGetHealthManager(other.gameObject, out var hm))
                hm.Hit(hit);
        }
    }
}

#nullable restore
