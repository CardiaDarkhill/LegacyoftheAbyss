#nullable disable
using System.Collections.Generic;
using LegacyoftheAbyss.Shade;
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

        /// <summary>
        /// How close this minion has to be to actually land a hit, in world units. Matched to the
        /// trigger it carries rather than relied on through the physics callback: the collision
        /// matrix decides whether a trigger on this object ever meets an enemy's collider, and the
        /// chase already knows exactly where its quarry is.
        /// </summary>
        public float contactRadius = 0.45f;

        /// <summary>
        /// Whether the difficulty multiplier applies. Off for the charms specified as an exact
        /// number of damage - Weaversong's 3, a hatchling's 9 - which are the charm's figure and
        /// not a share of the nail's.
        /// </summary>
        public bool scaleWithDamageMultiplier = true;

        /// <summary>
        /// Whether this minion walks instead of flying. A weaverling is a spiderling: it runs along
        /// the floor and cannot cross a gap, which is most of what makes it read as a creature
        /// rather than as an icon drifting over the terrain.
        /// </summary>
        public bool groundBound;

        /// <summary>
        /// How much of the orbit's radius is spent vertically. The flock charms sit on a flattened
        /// ellipse so they read as circling in front of the bearer; Dreamshield is a real circle,
        /// because the shield is meant to cover every side equally.
        /// </summary>
        public float orbitVerticalScale = 0.6f;

        /// <summary>
        /// Whether the minion turns so its own right-hand side points away from the bearer. The
        /// Dreamshield's point does; a weaverling has no orientation worth keeping.
        /// </summary>
        public bool faceOutward;

        /// <summary>
        /// Whether an idle minion drifts around near the bearer instead of sitting on them. Without
        /// it a flock with nothing to chase piles up in one spot.
        /// </summary>
        public bool wanders;

        /// <summary>How long a minion may fail to close on its target before it is sent home.</summary>
        private const float StuckSeconds = 3f;

        /// <summary>How much closer it has to get in that time to count as making progress.</summary>
        private const float StuckProgressEpsilon = 0.75f;

        private const float WanderRepickSeconds = 1.6f;
        private const float WanderRadius = 3.2f;

        private float stuckTimer;
        private float bestTargetDistance = float.MaxValue;
        private float wanderTimer;
        private Vector2 wanderPoint;
        private bool hasWanderPoint;

        /// <summary>Gravity for a grounded minion, matched to the Knight so they fall alike.</summary>
        private const float Gravity = 60f;

        private const float MaxFallSpeed = 26f;

        /// <summary>How hard a grounded minion hops when its quarry is above it.</summary>
        private const float HopSpeed = 13f;

        private const float BodyRadius = 0.3f;
        private const float GroundProbe = 0.14f;

        private float verticalVelocity;
        private bool grounded;

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
                    var previous = target;
                    target = FindNearestEnemy(owner.position, seekRange);
                    if (target != previous)
                    {
                        ResetStuckTracking();
                    }
                }

                UpdateStuckTracking(dt);
            }

            if (groundBound)
            {
                MoveAlongGround(dt);
            }
            else if (target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, seekSpeed * dt);
            }
            else
            {
                angle += orbitSpeed * dt;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians) * orbitVerticalScale, 0f) * orbitRadius;
                transform.position = Vector3.Lerp(transform.position, owner.position + offset, 1f - Mathf.Exp(-12f * dt));

                if (faceOutward)
                {
                    // Turned by where it is rather than by where it is going: the point should aim
                    // away from the bearer at every position on the circle, which is the offset's
                    // own direction. Reading it from the offset also keeps it exact at the top and
                    // bottom of the orbit, where the direction of travel is horizontal.
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg);
                }
            }

            // Only on contact. Damaging the chase target unconditionally meant a minion hit
            // whatever it had picked out from anywhere inside its whole seek range - which is
            // what "it just randomly hits things nearby" was: a hit with no strike behind it.
            if (target != null && WithinContact(target))
            {
                TryDamage(target.gameObject);
            }

            ExpireHitCooldowns(dt);
        }

        /// <summary>
        /// One step of a spiderling: run at whatever it is chasing, or trail its owner when it has
        /// nothing, and fall the rest of the time. It hops when its quarry is above it and it has
        /// something to push off, which is as much climbing as a minion this small needs.
        /// </summary>
        private void MoveAlongGround(float dt)
        {
            Vector2 position = transform.position;
            Vector2 goal = target != null ? (Vector2)target.position : ResolveIdleGoal(dt, position);

            float toGoalX = goal.x - position.x;
            float horizontal = Mathf.Abs(toGoalX) > 0.15f ? Mathf.Sign(toGoalX) * seekSpeed : 0f;

            ProbeGround(position);

            if (grounded)
            {
                verticalVelocity = 0f;
                if (goal.y - position.y > 1.2f)
                {
                    verticalVelocity = HopSpeed;
                    grounded = false;
                }
            }
            else
            {
                verticalVelocity = Mathf.Max(verticalVelocity - Gravity * dt, -MaxFallSpeed);
            }

            Vector2 step = position + new Vector2(horizontal, verticalVelocity) * dt;
            transform.position = ResolveTerrain(position, step);
        }

        /// <summary>
        /// Where a minion with nothing to chase is heading: a point it picks near its bearer and
        /// re-picks every so often, rather than the bearer itself. Walking at the bearer directly is
        /// what made the whole flock stack into one spot.
        /// </summary>
        private Vector2 ResolveIdleGoal(float dt, Vector2 position)
        {
            Vector2 ownerPos = owner.position;
            if (!wanders)
            {
                return ownerPos;
            }

            wanderTimer -= dt;
            bool arrived = hasWanderPoint && Mathf.Abs(wanderPoint.x - position.x) < 0.4f;
            if (!hasWanderPoint || arrived || wanderTimer <= 0f)
            {
                hasWanderPoint = true;
                wanderTimer = WanderRepickSeconds;
                wanderPoint = ownerPos + new Vector2(Random.Range(-WanderRadius, WanderRadius), 0f);
            }

            // Never further from the bearer than the wander radius, whatever it wandered into.
            if (Mathf.Abs(wanderPoint.x - ownerPos.x) > WanderRadius)
            {
                wanderPoint.x = ownerPos.x + Mathf.Sign(wanderPoint.x - ownerPos.x) * WanderRadius;
            }

            return new Vector2(wanderPoint.x, ownerPos.y);
        }

        /// <summary>
        /// Sends a minion back to its bearer when it has spent <see cref="StuckSeconds"/> unable to
        /// get any closer to what it is chasing - a ledge, a wall, a pit. Re-homed rather than
        /// destroyed and re-summoned: the flock is a fixed set, and putting this one back at the
        /// bearer is the same thing to look at without churning the set it belongs to.
        /// </summary>
        private void UpdateStuckTracking(float dt)
        {
            if (target == null)
            {
                ResetStuckTracking();
                return;
            }

            float distance = Vector2.Distance(transform.position, target.position);
            if (distance < bestTargetDistance - StuckProgressEpsilon)
            {
                bestTargetDistance = distance;
                stuckTimer = 0f;
                return;
            }

            stuckTimer += dt;
            if (stuckTimer < StuckSeconds)
            {
                return;
            }

            transform.position = owner.position;
            verticalVelocity = 0f;
            hasWanderPoint = false;
            ResetStuckTracking();
        }

        private void ResetStuckTracking()
        {
            stuckTimer = 0f;
            bestTargetDistance = float.MaxValue;
        }

        private void ProbeGround(Vector2 position)
        {
            int mask = ShadeController.TerrainMask();
            grounded = verticalVelocity <= 0.01f
                && Physics2D.OverlapCircle(position + Vector2.down * (BodyRadius + GroundProbe * 0.5f), GroundProbe, mask) != null;
        }

        /// <summary>
        /// Stops the body at terrain rather than sliding through it, swept per axis so a spiderling
        /// runs along a wall instead of sticking to it.
        /// </summary>
        private static Vector2 ResolveTerrain(Vector2 current, Vector2 target)
            => ShadeCharmSummons.ResolveTerrain(current, target, BodyRadius);

        /// <summary>
        /// Whether the minion is close enough to the target to be touching it. Measured against
        /// the target's own collider where it has one, because a HealthManager's transform origin
        /// is routinely nowhere near the body it belongs to.
        /// </summary>
        private bool WithinContact(Transform other)
        {
            if (other == null)
            {
                return false;
            }

            var collider = other.GetComponent<Collider2D>();
            Vector3 closest = collider != null
                ? (Vector3)collider.ClosestPoint(transform.position)
                : other.position;

            return (closest - transform.position).sqrMagnitude <= contactRadius * contactRadius;
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
                DamageDealt = scaleWithDamageMultiplier
                    ? Mathf.Max(1, Mathf.RoundToInt(contactDamage * ModConfig.Instance.shadeDamageMultiplier))
                    : Mathf.Max(1, contactDamage),
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
