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
        private HashSet<Collider2D> hitSet;

        void Awake()
        {
            hitSet = new HashSet<Collider2D>();
        }

        void Start()
        {
            Destroy(gameObject, lifeSeconds);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null) return;
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;
            if (hitSet.Contains(other)) return;
            hitSet.Add(other);

            var hit = new HitInstance
            {
                Source = gameObject,
                AttackType = AttackTypes.Spell,
                DamageDealt = damage,
                Direction = 90f,
                MagnitudeMultiplier = 1f,
                Multiplier = 1f,
                IsHeroDamage = true,
                IsFirstHit = true
            };

            HitTaker.Hit(other.gameObject, hit);
            if (HitTaker.TryGetHealthManager(other.gameObject, out var hm))
                hm.Hit(hit);
        }
    }
}

