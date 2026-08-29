#nullable disable
using System.Collections.Generic;
using UnityEngine;

public partial class LegacyHelper
{
    /// <summary>
    /// The small companion the summoning charms put in the air beside their bearer: Weaversong's
    /// weaverlings, Glowing Womb's hatchlings, Grimmchild and Dreamshield. They differ only in
    /// whether they orbit or hunt, how hard they hit, and how long they last.
    /// <para>
    /// Owner-agnostic on purpose - it follows a transform, so a Shade and a Knight summon the same
    /// minions from the same charm.
    /// </para>
    /// </summary>
    public class ShadeCharmMinion : MonoBehaviour
    {
        public Transform owner;
        public int contactDamage = 5;

        /// <summary>Radius of the idle orbit around the owner.</summary>
        public float orbitRadius = 1.8f;
        public float orbitSpeed = 180f;
        public float orbitPhase;

        /// <summary>Distance at which this minion breaks orbit to chase. Zero never chases.</summary>
        public float seekRange;
        public float seekSpeed = 9f;

        /// <summary>Seconds before this minion expires. Zero lives until dismissed.</summary>
        public float lifeSeconds;

        /// <summary>Whether hitting something ends this minion, as a hatchling's charge does.</summary>
        public bool expiresOnHit;

        private float angle;
        private float life;
        private Transform target;
        private float retargetTimer;
        private readonly Dictionary<GameObject, float> hitCooldowns = new();
        private readonly List<GameObject> scratchKeys = new();

        private const float HitCooldownSeconds = 0.45f;
        private const float RetargetSeconds = 0.35f;

        private void Awake()
        {
            angle = orbitPhase;
        }

        private void Update()
        {
            if (owner == null)
            {
                Destroy(gameObject);
                return;
            }

            float dt = Time.deltaTime;

            if (lifeSeconds > 0f)
            {
                life += dt;
                if (life >= lifeSeconds)
                {
                    Destroy(gameObject);
                    return;
                }
            }

            if (seekRange > 0f)
            {
                retargetTimer -= dt;
                if (retargetTimer <= 0f)
                {
                    retargetTimer = RetargetSeconds;
                    target = FindNearestEnemy(owner.position, seekRange);
                }
            }

            if (target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, seekSpeed * dt);
                TryDamage(target.gameObject);
            }
            else
            {
                angle += orbitSpeed * dt;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians) * 0.6f, 0f) * orbitRadius;
                transform.position = Vector3.Lerp(transform.position, owner.position + offset, 1f - Mathf.Exp(-12f * dt));
            }

            ExpireHitCooldowns(dt);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other != null)
            {
                TryDamage(other.gameObject);
            }
        }

        private void TryDamage(GameObject victim)
        {
            if (victim == null || contactDamage <= 0)
            {
                return;
            }

            if (owner != null && victim.transform.IsChildOf(owner))
            {
                return;
            }

            if (!HitTaker.TryGetHealthManager(victim, out var health) || health == null)
            {
                return;
            }

            var root = health.gameObject;
            if (hitCooldowns.ContainsKey(root))
            {
                return;
            }

            float direction = Mathf.Atan2(
                root.transform.position.y - transform.position.y,
                root.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            if (direction < 0f) direction += 360f;

            var hit = new HitInstance
            {
                Source = gameObject,
                AttackType = AttackTypes.Spell,
                DamageDealt = Mathf.Max(1, Mathf.RoundToInt(contactDamage * ModConfig.Instance.shadeDamageMultiplier)),
                Direction = direction,
                MagnitudeMultiplier = 1f,
                Multiplier = 1f,
                IsHeroDamage = true,
                IsFirstHit = true
            };

            health.Hit(hit);
            hitCooldowns[root] = HitCooldownSeconds;

            if (expiresOnHit)
            {
                Destroy(gameObject);
            }
        }

        private void ExpireHitCooldowns(float dt)
        {
            if (hitCooldowns.Count == 0)
            {
                return;
            }

            // Snapshot the keys: the dictionary is written inside the loop.
            scratchKeys.Clear();
            scratchKeys.AddRange(hitCooldowns.Keys);

            foreach (var key in scratchKeys)
            {
                float remaining = hitCooldowns[key] - dt;
                if (remaining <= 0f || key == null)
                {
                    hitCooldowns.Remove(key);
                }
                else
                {
                    hitCooldowns[key] = remaining;
                }
            }
        }

        /// <summary>
        /// Nearest living enemy to <paramref name="origin"/>, or null. Scans HealthManagers, which
        /// is the same handle the Shade's own targeting uses.
        /// </summary>
        internal static Transform FindNearestEnemy(Vector3 origin, float range)
        {
            var candidates = UnityEngine.Object.FindObjectsByType<HealthManager>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            Transform best = null;
            float bestSqr = range * range;

            for (int i = 0; i < candidates.Length; i++)
            {
                var candidate = candidates[i];
                if (candidate == null || !candidate.isActiveAndEnabled)
                {
                    continue;
                }

                float sqr = (candidate.transform.position - origin).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = candidate.transform;
                }
            }

            return best;
        }

    }
}
