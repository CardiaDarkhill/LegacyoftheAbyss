#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// Turns the enemies actually loaded in the scene into the flat <see cref="ShadeAiTarget"/> list
    /// the brain reasons over.
    /// <para>
    /// Split in two on purpose. Finding enemies means walking every <c>HealthManager</c> in the
    /// scene, which is the one genuinely expensive thing the AI does, so that happens on an interval
    /// (<see cref="ModConfig.shadeAiScanIntervalSeconds"/>) and the result is cached. Reading where
    /// those enemies are is a bounds lookup per entry, so that happens every frame - otherwise the
    /// Shade would swing at where a moving enemy was a third of a second ago.
    /// </para>
    /// </summary>
    internal sealed class ShadeAiTargetScanner
    {
        /// <summary>
        /// Radius used for an enemy with no usable collider. Small enough that the Shade closes to
        /// genuine nail range on it rather than swinging from a body-width away.
        /// </summary>
        private const float FallbackRadius = 0.5f;

        private const float MinimumRadius = 0.25f;

        /// <summary>
        /// Bosses have colliders that cover half a room. Letting the measured radius run away would
        /// have the Shade treat "inside the boss" as its strike point.
        /// </summary>
        private const float MaximumRadius = 4f;

        /// <summary>
        /// Upper bound on how many enemies reach the brain, nearest first. Every one of them costs a
        /// line-of-sight raycast per frame, and a Shade fighting sixteen things at once is not going
        /// to make a better decision with the seventeenth.
        /// </summary>
        private const int MaxTrackedTargets = 16;

        private sealed class Entry
        {
            internal HealthManager Health = null!;
            internal Collider2D? Body;
            internal Transform Transform = null!;
            internal int Id;
            internal bool IsBoss;
            internal float SortDistance;
        }

        private readonly List<Entry> entries = new List<Entry>();
        private readonly List<Entry> candidates = new List<Entry>();
        private readonly List<ShadeAiTarget> targets = new List<ShadeAiTarget>();
        private float nextScanTime;
        private ShadeAiScanStats stats;

        /// <summary>What the last <see cref="Collect"/> kept and threw away. Diagnostics only.</summary>
        internal ShadeAiScanStats Stats => stats;

        /// <summary>Drops the cached scene contents. Call on scene change or when the AI is switched off.</summary>
        internal void Reset()
        {
            entries.Clear();
            candidates.Clear();
            targets.Clear();
            nextScanTime = 0f;
        }

        /// <summary>
        /// The enemies worth considering this frame. The returned list is reused between calls - read
        /// it before the next call, do not keep it.
        /// </summary>
        /// <param name="spellWorthHealth">
        /// HP at or above which a single enemy is worth a spell on its own. The driver derives this
        /// from the Shade's own nail damage; see <see cref="ShadeAiTuning.BossNailHits"/>.
        /// </param>
        internal IReadOnlyList<ShadeAiTarget> Collect(Vector2 origin, float maxDistance, int spellWorthHealth, float time, float scanInterval)
        {
            if (time >= nextScanTime)
            {
                nextScanTime = time + Mathf.Max(0.05f, scanInterval);
                Rescan(spellWorthHealth);
            }

            targets.Clear();
            candidates.Clear();
            stats.Tracked = 0;
            stats.OutOfRange = 0;
            stats.Returned = 0;
            stats.Blocked = 0;
            float limit = Mathf.Max(1f, maxDistance);
            float limitSqr = limit * limit;

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var entry = entries[i];
                if (!IsAttackable(entry))
                {
                    entries.RemoveAt(i);
                    continue;
                }

                stats.Tracked++;

                ResolvePlacement(entry, out Vector2 position, out _);
                Vector2 offset = position - origin;
                float sqr = offset.sqrMagnitude;
                if (sqr > limitSqr)
                {
                    stats.OutOfRange++;
                    continue;
                }

                entry.SortDistance = sqr;
                candidates.Add(entry);
            }

            candidates.Sort(static (a, b) => a.SortDistance.CompareTo(b.SortDistance));

            int taken = Mathf.Min(candidates.Count, MaxTrackedTargets);
            for (int i = 0; i < taken; i++)
            {
                var entry = candidates[i];
                ResolvePlacement(entry, out Vector2 position, out float radius);
                bool visible = HasLineOfSight(origin, position, radius);
                if (!visible)
                {
                    stats.Blocked++;
                }

                targets.Add(new ShadeAiTarget(entry.Id, position, radius, entry.Health.hp, entry.IsBoss, visible));
            }

            stats.Returned = targets.Count;
            return targets;
        }

        /// <summary>
        /// Whether solid terrain sits between the Shade and an enemy.
        /// <para>
        /// This is the fix for two shipped bugs at once: the Shade spending twenty seconds slashing a
        /// wall with an enemy behind it, and three fireballs spent on something the projectile could
        /// never reach. The projectile is destroyed by the <c>Terrain</c> layer
        /// (<c>LegacyHelper.Projectile.cs</c>), so testing that same layer asks exactly the question
        /// the spell answers in flight - and <see cref="ShadeAiTerrain"/> is also what the navigator
        /// steers by, so seeing and reaching cannot disagree about where the walls are.
        /// </para>
        /// </summary>
        private static bool HasLineOfSight(Vector2 origin, Vector2 targetPosition, float targetRadius)
        {
            // Aim at the near edge rather than the centre, so a large enemy whose middle is buried in
            // the floor still reads as visible.
            Vector2 toTarget = targetPosition - origin;
            float distance = toTarget.magnitude;
            if (distance <= 0.0001f)
            {
                return true;
            }

            float trimmed = Mathf.Max(0.05f, distance - Mathf.Max(0f, targetRadius));
            Vector2 endPoint = origin + (toTarget / distance * trimmed);
            return !ShadeAiTerrain.LineBlocked(origin, endPoint);
        }

        private void Rescan(int spellWorthHealth)
        {
            entries.Clear();

            HealthManager[] found;
            try
            {
                found = Object.FindObjectsByType<HealthManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            }
            catch
            {
                return;
            }

            if (found == null)
            {
                return;
            }

            var declaredBosses = TryGetDeclaredBosses();

            foreach (var health in found)
            {
                if (!health || health.isDead || health.hp <= 0)
                {
                    continue;
                }

                var transform = health.transform;
                if (transform == null || !health.isActiveAndEnabled)
                {
                    continue;
                }

                entries.Add(new Entry
                {
                    Health = health,
                    Body = ResolveBodyCollider(health),
                    Transform = transform,
                    Id = health.GetInstanceID(),
                    IsBoss = IsSpellWorthy(health, declaredBosses, spellWorthHealth)
                });
            }

            stats.Found = entries.Count;
        }

        /// <summary>
        /// Whether an enemy is still worth tracking: alive, and present in the scene.
        /// <para>
        /// Deliberately only that. An earlier version also rejected on <c>IsInvincible</c>, on a
        /// disabled body collider and on <c>Renderer.isVisible</c>, all three added on speculation to
        /// stop the Shade slashing a wall - and together they stopped it attacking anything at all.
        /// Every one of them was unsound:
        /// </para>
        /// <list type="bullet">
        /// <item><description><c>IsInvincible</c> is not "cannot be hurt". Enemies raise it during
        /// attacks and phase changes, and armoured ones pair it with <c>invincibleFromDirection</c>
        /// to mean "blocks from this side" - so it reads true on plenty of enemies that are being
        /// damaged perfectly well.</description></item>
        /// <item><description>A disabled body collider is routine mid-attack, and the enemy is still
        /// there; <see cref="ResolvePlacement"/> already falls back to the transform.</description></item>
        /// <item><description><c>isVisible</c> is a camera-culling flag, false for any renderer whose
        /// bounds leave the frustum and for sprites swapped out for a frame.</description></item>
        /// </list>
        /// <para>
        /// The wall the Shade was slashing is handled by <see cref="HasLineOfSight"/>, which asks the
        /// actual question. Do not add filters here without evidence from
        /// <see cref="ShadeAiScanStats"/> that they are needed.
        /// </para>
        /// </summary>
        private static bool IsAttackable(Entry entry)
        {
            try
            {
                return entry.Health
                    && !entry.Health.isDead
                    && entry.Health.hp > 0
                    && entry.Health.isActiveAndEnabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// An enemy sits at the centre of its body collider, not at its transform - pivots here are
        /// routinely at the feet or off the sprite entirely, and a strike point derived from one of
        /// those puts the Shade below the thing it is trying to hit.
        /// </summary>
        private static void ResolvePlacement(Entry entry, out Vector2 position, out float radius)
        {
            try
            {
                var body = entry.Body;
                if (body != null && body && body.enabled)
                {
                    var bounds = body.bounds;
                    position = bounds.center;
                    radius = Mathf.Clamp(Mathf.Max(bounds.extents.x, bounds.extents.y), MinimumRadius, MaximumRadius);
                    return;
                }
            }
            catch
            {
            }

            position = entry.Transform != null ? (Vector2)entry.Transform.position : Vector2.zero;
            radius = FallbackRadius;
        }

        private static Collider2D? ResolveBodyCollider(HealthManager health)
        {
            try
            {
                var own = health.GetComponent<Collider2D>();
                if (own != null && !own.isTrigger)
                {
                    return own;
                }

                Collider2D? firstTrigger = own;
                foreach (var candidate in health.GetComponentsInChildren<Collider2D>(false))
                {
                    if (candidate == null)
                    {
                        continue;
                    }

                    if (!candidate.isTrigger)
                    {
                        return candidate;
                    }

                    firstTrigger ??= candidate;
                }

                // Plenty of enemies are trigger-only. A trigger still describes where the body is,
                // which is all this is being asked for.
                return firstTrigger;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Whether one enemy on its own justifies a spell.
        /// <para>
        /// There is no per-enemy boss flag anywhere in the game assembly -
        /// <c>HealthManager.EnemyTypes</c> is Regular/Shade/Armoured, the journal record types are
        /// Enemy/Other, and <c>BossSceneController.bosses</c> is only filled in when a scene wants to
        /// end on those deaths. So this asks the question that actually matters instead: would this
        /// enemy survive a long stretch of ordinary nail hits? Measuring in hits rather than hit
        /// points is what keeps it honest as the Shade's damage changes with charms.
        /// </para>
        /// <para>
        /// The flat 200 HP test this replaced classified ordinary Ant enemies as bosses and burned a
        /// full SOUL meter on three fireballs at one of them.
        /// </para>
        /// </summary>
        private static bool IsSpellWorthy(HealthManager health, HealthManager[]? declaredBosses, int spellWorthHealth)
        {
            if (declaredBosses != null)
            {
                foreach (var declared in declaredBosses)
                {
                    if (declared == health)
                    {
                        return true;
                    }
                }
            }

            return spellWorthHealth > 0 && health.hp >= spellWorthHealth;
        }

        private static HealthManager[]? TryGetDeclaredBosses()
        {
            try
            {
                var controller = BossSceneController.Instance;
                return controller != null ? controller.bosses : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
