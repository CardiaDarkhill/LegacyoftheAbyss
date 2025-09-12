using System.Collections;
using UnityEngine;

public partial class LegacyHelper
{
    public class ShadeProjectile : MonoBehaviour
    {
        public int damage = 20;
        public Transform hornetRoot;
        public float lifeSeconds = 1.5f;
        bool hasHit;

        void Start() => Destroy(gameObject, lifeSeconds);

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit) return;
            if (other == null) return;
            if (hornetRoot != null && other.transform.IsChildOf(hornetRoot)) return;
            if (other.transform == transform || other.transform.IsChildOf(transform)) return;
            hasHit = true;

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

            StartCoroutine(DestroyNextFrame());
        }

        IEnumerator DestroyNextFrame()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}

