#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Ai
{
    /// <summary>
    /// Turns the enemies loaded in the scene into the flat <see cref="ShadeAiTarget"/> list the
    /// brain reasons over.
    /// <para>
    /// Split in two on purpose. Finding enemies walks every <c>HealthManager</c> in the scene, which
    /// is the one genuinely expensive thing the AI does, so it runs on an interval
    /// (<see cref="ModConfig.shadeAiScanIntervalSeconds"/>) and the result is cached. Reading where
    /// those enemies are is a bounds lookup per entry, so that happens every frame - otherwise the
    /// Shade would swing at where a moving enemy was a third of a second ago.
    /// </para>
    /// </summary>
    internal sealed class ShadeAiTargetScanner
    {
        /// <summary>Radius for an enemy with no usable collider. Small enough to close to real nail range.</summary>
        private const float FallbackRadius = 0.5f;

        private const float MinimumRadius = 0.25f;

        /// <summary>Bosses have colliders covering half a room; without this the strike point ends up inside them.</summary>
        private const float MaximumRadius = 4f;

        /// <summary>
        /// Upper bound on how many enemies reach the brain, nearest first. Each costs a line-of-sight
        /// raycast per frame.
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

            /// <summary>Set only for gauntlet enemies; null for everything else.</summary>
            internal BattleScene? Battle;
            internal int WaveIndex;
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
        internal IReadOnlyList<ShadeAiTarget> Collect(Vector2 origin, float maxDistance, int spellWorthHealth, float time, float scanInterval, int minSpellTargetHealth)
        {
            if (time >= nextScanTime)
            {
                nextScanTime = time + Mathf.Max(0.05f, scanInterval);
                Rescan(spellWorthHealth);
            }

            targets.Clear();
            candidates.Clear();
            stats.Tracked = 0;
            stats.Dormant = 0;
            stats.OutOfRange = 0;
            stats.Returned = 0;
            stats.Blocked = 0;
            stats.NotWorthASpell = 0;
            stats.NotDrawn = 0;
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

                // Skipped rather than removed - the wave this enemy belongs to is going to start.
                if (entry.Battle != null && !ShadeAiBattleScenes.IsWaveLive(entry.Battle, entry.WaveIndex))
                {
                    stats.Dormant++;
                    continue;
                }

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

                bool worthASpell = IsWorthASpell(entry.Health, minSpellTargetHealth);
                if (!worthASpell)
                {
                    stats.NotWorthASpell++;
                }

                if (!IsDrawingAnything(entry.Health))
                {
                    stats.NotDrawn++;
                }

                targets.Add(new ShadeAiTarget(entry.Id, position, radius, entry.IsBoss, visible, worthASpell));
            }

            stats.Returned = targets.Count;
            return targets;
        }

        /// <summary>
        /// Whether solid terrain sits between the Shade and an enemy.
        /// <para>
        /// Tests the <c>Terrain</c> layer, which is what destroys the projectile in flight
        /// (<c>LegacyHelper.Projectile.cs</c>) and what <see cref="ShadeAiTerrain"/> steers by, so
        /// seeing, reaching and hitting cannot disagree about where the walls are.
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

        /// <summary>The four sides <c>HealthManager.IsBlockingByDirection</c> distinguishes.</summary>
        private const int CardinalDirections = 4;

        private void Rescan(int spellWorthHealth)
        {
            entries.Clear();

            var found = Object.FindObjectsByType<HealthManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var declaredBosses = TryGetDeclaredBosses();

            foreach (var health in found)
            {
                if (!health || health.isDead || health.hp <= 0 || !health.isActiveAndEnabled)
                {
                    continue;
                }

                var transform = health.transform;
                var entry = new Entry
                {
                    Health = health,
                    Body = ResolveBodyCollider(health),
                    Transform = transform,
                    Id = health.GetInstanceID(),
                    IsBoss = IsSpellWorthy(health, declaredBosses, spellWorthHealth)
                };

                if (ShadeAiBattleScenes.TryResolveWave(transform, out var battle, out int waveIndex))
                {
                    entry.Battle = battle;
                    entry.WaveIndex = waveIndex;
                }

                entries.Add(entry);
            }

            stats.Found = entries.Count;
        }

        /// <summary>
        /// Whether an enemy is still worth tracking: alive, and present in the scene. Deliberately
        /// only that - <c>IsInvincible</c> is raised routinely mid-attack, a disabled body collider
        /// is routine and <see cref="ResolvePlacement"/> already falls back to the transform, and
        /// <c>Renderer.isVisible</c> is a camera-culling flag; filtering on them leaves the Shade
        /// attacking nothing. Terrain is <see cref="HasLineOfSight"/>'s job and an unstarted gauntlet
        /// wave is <see cref="Collect"/>'s. Do not add filters here without
        /// <see cref="ShadeAiScanStats"/> evidence.
        /// </summary>
        private static bool IsAttackable(Entry entry)
        {
            return entry.Health
                && !entry.Health.isDead
                && entry.Health.hp > 0
                && entry.Health.isActiveAndEnabled;
        }

        /// <summary>
        /// Whether spending SOUL on this enemy is worth it - which is a different question from
        /// whether to attack it, and answered separately for that reason.
        /// <para>
        /// Two enemies in a cast's area used to be enough to buy one, whatever they were, so a pair
        /// of harmless birds cost a third of the meter. And an enemy no spell can currently touch
        /// takes the whole cast for nothing, which is what a boss does through its pre-fight
        /// dialogue: present, at full health, and blocking everything until the conversation ends.
        /// </para>
        /// </summary>
        private static bool IsWorthASpell(HealthManager health, int minSpellTargetHealth)
        {
            if (health == null)
            {
                return false;
            }

            if (minSpellTargetHealth > 0 && health.hp < minSpellTargetHealth)
            {
                return false;
            }

            return CanASpellLandOn(health);
        }

        /// <summary>
        /// Whether a spell could land on this enemy from <em>any</em> direction.
        /// <para>
        /// Emphatically not <c>IsInvincible</c>. That flag is the master switch for the whole
        /// blocking system rather than a statement that the enemy cannot be hurt: an armoured enemy
        /// - the helmeted ones that fill the game's second region, which are meant to be hit from
        /// the sides or below - sets it <em>and</em> sets <c>InvincibleFromDirection</c> to say which
        /// way its armour faces. Reading the flag alone would have written off every armoured enemy
        /// in the game as never worth a spell.
        /// </para>
        /// <para>
        /// So the question is put to the game's own answer instead, once per cardinal direction: if
        /// there is a side a spell could come in from, the enemy is worth casting at, because the
        /// Shade can move. Only something blocking all four - a boss that is simply switched off -
        /// falls through. Asking <c>IsBlockingByDirection</c> also picks up the two exemptions it
        /// knows about and this code should not have to: the <c>Spell Vulnerable</c> tag, and the
        /// enemies that armour cannot stop a piercing hit on.
        /// </para>
        /// </summary>
        private static bool CanASpellLandOn(HealthManager health)
        {
            try
            {
                // Cheap for the ordinary case: the method returns false at once for anything that is
                // not blocking at all, so a normal enemy costs one call.
                for (int direction = 0; direction < CardinalDirections; direction++)
                {
                    if (!health.IsBlockingByDirection(direction, AttackTypes.Spell))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                // Never let a lookup failure cost the Shade its spells.
                return true;
            }
        }

        /// <summary>
        /// Whether anything of this enemy is actually being drawn. Recorded for the diagnostics
        /// only - see <c>ShadeAiScanStats.NotDrawn</c> - and deliberately not <c>isVisible</c>,
        /// which is a camera-culling flag that goes false whenever an enemy is off screen.
        /// </summary>
        private static bool IsDrawingAnything(HealthManager health)
        {
            if (health == null)
            {
                return false;
            }

            try
            {
                foreach (var renderer in health.GetComponentsInChildren<Renderer>(false))
                {
                    if (renderer != null && renderer.enabled)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// An enemy sits at the centre of its body collider, not at its transform - pivots here are
        /// routinely at the feet or off the sprite entirely.
        /// </summary>
        private static void ResolvePlacement(Entry entry, out Vector2 position, out float radius)
        {
            var body = entry.Body;
            if (body != null && body.enabled)
            {
                var bounds = body.bounds;
                position = bounds.center;
                radius = Mathf.Clamp(Mathf.Max(bounds.extents.x, bounds.extents.y), MinimumRadius, MaximumRadius);
                return;
            }

            position = entry.Transform != null ? (Vector2)entry.Transform.position : Vector2.zero;
            radius = FallbackRadius;
        }

        private static Collider2D? ResolveBodyCollider(HealthManager health)
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

            // Plenty of enemies are trigger-only. A trigger still describes where the body is.
            return firstTrigger;
        }

        /// <summary>
        /// Whether one enemy on its own justifies a spell. There is no per-enemy boss flag in the
        /// game assembly, so this asks whether it would survive a long stretch of ordinary nail hits.
        /// The threshold is derived in hits rather than hit points so it tracks the Shade's damage as
        /// charms change it; a flat HP figure classifies ordinary enemies as bosses.
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
            var controller = BossSceneController.Instance;
            return controller != null ? controller.bosses : null;
        }
    }
}
