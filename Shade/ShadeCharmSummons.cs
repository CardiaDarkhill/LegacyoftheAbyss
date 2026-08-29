#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Spawning and dismissal for the charms that put something in the air beside their bearer.
    /// Keyed per controller, so two companions wearing Weaversong each keep their own weaverlings.
    /// </summary>
    internal static class ShadeCharmSummons
    {
        private sealed class SummonSet
        {
            internal readonly List<GameObject> Members = new();
            internal float Timer;
        }

        private static readonly Dictionary<(LegacyHelper.ShadeController, ShadeCharmId), SummonSet> s_sets = new();

        /// <summary>
        /// Creates <paramref name="count"/> minions orbiting the bearer, replacing any this charm
        /// already had out. Safe to call on re-equip.
        /// </summary>
        internal static void Spawn(
            LegacyHelper.ShadeController controller,
            ShadeCharmId charm,
            int count,
            int damage,
            float orbitRadius,
            float seekRange,
            float lifeSeconds = 0f,
            bool expiresOnHit = false)
        {
            if (controller == null)
            {
                return;
            }

            Dismiss(controller, charm);
            var set = GetSet(controller, charm);

            for (int i = 0; i < count; i++)
            {
                var go = new GameObject($"ShadeCharmMinion_{charm}_{i}");
                go.transform.position = controller.transform.position;

                var minion = go.AddComponent<LegacyHelper.ShadeCharmMinion>();
                minion.owner = controller.transform;
                minion.contactDamage = damage;
                minion.orbitRadius = orbitRadius;
                minion.orbitPhase = count > 0 ? 360f / count * i : 0f;
                minion.seekRange = seekRange;
                minion.lifeSeconds = lifeSeconds;
                minion.expiresOnHit = expiresOnHit;

                ApplyVisual(go, charm);

                var trigger = go.AddComponent<CircleCollider2D>();
                trigger.isTrigger = true;
                trigger.radius = 0.45f;

                set.Members.Add(go);
            }
        }

        /// <summary>
        /// Adds one more minion without disturbing those already out - Glowing Womb's trickle,
        /// as opposed to Weaversong's fixed set. Caps the flock so a long fight cannot flood it.
        /// </summary>
        internal static void AddOne(
            LegacyHelper.ShadeController controller,
            ShadeCharmId charm,
            int maxAlive,
            int damage,
            float seekRange,
            float lifeSeconds,
            bool expiresOnHit)
        {
            if (controller == null)
            {
                return;
            }

            var set = GetSet(controller, charm);
            set.Members.RemoveAll(m => m == null);
            if (set.Members.Count >= maxAlive)
            {
                return;
            }

            var go = new GameObject($"ShadeCharmMinion_{charm}");
            go.transform.position = controller.transform.position;

            var minion = go.AddComponent<LegacyHelper.ShadeCharmMinion>();
            minion.owner = controller.transform;
            minion.contactDamage = damage;
            minion.orbitRadius = 1.4f;
            minion.orbitPhase = Random.Range(0f, 360f);
            minion.seekRange = seekRange;
            minion.lifeSeconds = lifeSeconds;
            minion.expiresOnHit = expiresOnHit;

            ApplyVisual(go, charm);

            var trigger = go.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = 0.4f;

            set.Members.Add(go);
        }

        /// <summary>Ticks a spawn timer and reports when it has come round. For trickle charms.</summary>
        internal static bool TickSpawnTimer(
            LegacyHelper.ShadeController controller,
            ShadeCharmId charm,
            float delta,
            float interval)
        {
            if (controller == null || interval <= 0f)
            {
                return false;
            }

            var set = GetSet(controller, charm);
            set.Timer += Mathf.Max(0f, delta);
            if (set.Timer < interval)
            {
                return false;
            }

            set.Timer -= interval;
            return true;
        }

        internal static void Dismiss(LegacyHelper.ShadeController controller, ShadeCharmId charm)
        {
            if (controller == null || !s_sets.TryGetValue((controller, charm), out var set))
            {
                return;
            }

            foreach (var member in set.Members)
            {
                if (member != null)
                {
                    Object.Destroy(member);
                }
            }

            set.Members.Clear();
            set.Timer = 0f;
            s_sets.Remove((controller, charm));
        }

        /// <summary>
        /// Dismisses everything one controller has summoned, and drops bookkeeping left by any
        /// controller that has already gone. Called when a companion despawns.
        /// </summary>
        internal static void DismissAll(LegacyHelper.ShadeController controller)
        {
            var stale = new List<(LegacyHelper.ShadeController, ShadeCharmId)>();
            foreach (var pair in s_sets)
            {
                if (pair.Key.Item1 == null || ReferenceEquals(pair.Key.Item1, controller))
                {
                    foreach (var member in pair.Value.Members)
                    {
                        if (member != null)
                        {
                            Object.Destroy(member);
                        }
                    }

                    stale.Add(pair.Key);
                }
            }

            foreach (var key in stale)
            {
                s_sets.Remove(key);
            }
        }

        private static SummonSet GetSet(LegacyHelper.ShadeController controller, ShadeCharmId charm)
        {
            var key = (controller, charm);
            if (!s_sets.TryGetValue(key, out var set))
            {
                set = new SummonSet();
                s_sets[key] = set;
            }

            return set;
        }

        /// <summary>
        /// Gives the minion something to look at. The charm's own icon stands in rather than the
        /// bundled Hollow Knight prefabs: those live in the ~54 MB Knight bundle, and pulling it in
        /// because someone equipped a charm would be a steep cost for a sprite this small.
        /// </summary>
        private static void ApplyVisual(GameObject go, ShadeCharmId charm)
        {
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = ShadeCharmIconLoader.TryLoadIcon(null, charm.ToString());
            renderer.color = new Color(1f, 1f, 1f, 0.9f);
            go.transform.localScale = Vector3.one * 0.5f;

            var owner = LegacyHelper.ShadeController.PrimaryInstance;
            var ownerRenderer = owner != null ? owner.GetComponent<SpriteRenderer>() : null;
            if (ownerRenderer != null)
            {
                renderer.sortingLayerID = ownerRenderer.sortingLayerID;
                renderer.sortingOrder = ownerRenderer.sortingOrder + 1;
            }
        }
    }
}
