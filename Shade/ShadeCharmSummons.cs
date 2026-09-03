#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// Stops a summoned body at terrain rather than letting it drift through, swept per axis so
        /// it slides along a wall instead of sticking to it - which is what lets one follow its
        /// bearer round a corner rather than pressing into the corner itself.
        /// </summary>
        internal static Vector2 ResolveTerrain(Vector2 current, Vector2 target, float bodyRadius)
        {
            int mask = LegacyHelper.ShadeController.TerrainMask();
            Vector2 resolved = current;

            float dx = target.x - current.x;
            if (Mathf.Abs(dx) > 0.0001f)
            {
                var hit = Physics2D.CircleCast(resolved, bodyRadius, new Vector2(Mathf.Sign(dx), 0f), Mathf.Abs(dx), mask);
                resolved.x = hit.collider != null
                    ? resolved.x + Mathf.Sign(dx) * Mathf.Max(0f, hit.distance - 0.01f)
                    : target.x;
            }

            float dy = target.y - current.y;
            if (Mathf.Abs(dy) > 0.0001f)
            {
                var hit = Physics2D.CircleCast(resolved, bodyRadius, new Vector2(0f, Mathf.Sign(dy)), Mathf.Abs(dy), mask);
                resolved.y = hit.collider != null
                    ? resolved.y + Mathf.Sign(dy) * Mathf.Max(0f, hit.distance - 0.01f)
                    : target.y;
            }

            return resolved;
        }

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
            bool expiresOnHit = false,
            bool scaleWithDamageMultiplier = true,
            bool groundBound = false,
            bool wanders = false,
            bool faceOutward = false,
            float orbitVerticalScale = 0.6f,
            float orbitSpeed = 0f,
            float visualScale = 1f)
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
                minion.scaleWithDamageMultiplier = scaleWithDamageMultiplier;
                minion.groundBound = groundBound;
                minion.wanders = wanders;
                minion.faceOutward = faceOutward;
                minion.orbitVerticalScale = orbitVerticalScale;
                if (orbitSpeed > 0f)
                {
                    minion.orbitSpeed = orbitSpeed;
                }

                minion.contactRadius = 0.45f;

                ApplyVisual(go, controller, charm, visualScale);

                var trigger = go.AddComponent<CircleCollider2D>();
                trigger.isTrigger = true;
                trigger.radius = minion.contactRadius;

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
            bool expiresOnHit,
            bool scaleWithDamageMultiplier = true)
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
            minion.scaleWithDamageMultiplier = scaleWithDamageMultiplier;
            minion.contactRadius = 0.4f;

            ApplyVisual(go, controller, charm, 1f);

            var trigger = go.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = minion.contactRadius;

            set.Members.Add(go);
        }

        /// <summary>
        /// Grimmchild, which is not one of the generic orbiting minions: it flies, aims and shoots.
        /// Registered in the same set so dismissal and companion teardown reach it unchanged.
        /// </summary>
        internal static void SpawnGrimmchild(LegacyHelper.ShadeController controller)
        {
            if (controller == null)
            {
                return;
            }

            Dismiss(controller, ShadeCharmId.Grimmchild);

            var go = LegacyHelper.ShadeCharmGrimmchild.Create(controller);
            if (go != null)
            {
                GetSet(controller, ShadeCharmId.Grimmchild).Members.Add(go);
            }
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

            // A downed companion summons nothing. Gated here rather than in each charm because it is
            // true of all of them: the bearer is what the summons come from, and one that is waiting
            // to be revived should not still be birthing hatchlings. The timer is held rather than
            // advanced, so reviving does not immediately spend the whole time spent dead.
            if (controller.IsInactive)
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

        /// <summary>
        /// The bundle prefab that is this charm's summon, or null when it has none and the charm
        /// icon has to stand in. Grimmchild is absent on purpose: it is drawn from the Grimmchild
        /// III skin, which the developer asked for by name.
        /// </summary>
        private static string? BundlePrefabFor(ShadeCharmId charm)
        {
            switch (charm)
            {
                case ShadeCharmId.Weaversong:
                    return Knight.KnightEffects.Weaverling;
                case ShadeCharmId.GlowingWomb:
                    return Knight.KnightEffects.Hatchling;
                case ShadeCharmId.Dreamshield:
                    return Knight.KnightEffects.OrbitShield;
                default:
                    return null;
            }
        }

        /// <summary>The Shade skin whose sheets Grimmchild is drawn from. Named by the developer.</summary>
        private const string GrimmchildSkinId = "Grimmchild Phase 3";

        private const string GrimmchildSheetName = "Shade_Idle_Sheet.png";

        /// <summary>
        /// What a summon is drawn as.
        /// <para>
        /// The charm definition's own <c>Icon</c>, which it resolved from the file name on disk.
        /// Asking the icon loader for the enum name instead - which is what this did - found
        /// nothing at all: the files are named "shade_charm_glowingwomb0009charmhatchling.png",
        /// not "GlowingWomb.png". That is why the summons were invisible rather than merely plain,
        /// and why they were reported as possibly not spawning.
        /// </para>
        /// </summary>
        private static Sprite? ResolveMinionSprite(LegacyHelper.ShadeController? controller, ShadeCharmId charm)
        {
            if (charm == ShadeCharmId.Grimmchild)
            {
                var grimmchild = TryLoadGrimmchildSprite();
                if (grimmchild != null)
                {
                    return grimmchild;
                }
            }

            try
            {
                var inventory = controller != null ? controller.Companion?.Charms : ShadeRuntime.Charms;
                return inventory?.GetDefinition(charm).Icon;
            }
            catch
            {
                return null;
            }
        }

        private static Sprite? s_grimmchildSprite;
        private static bool s_grimmchildSpriteResolved;

        /// <summary>
        /// Grimmchild's own art, taken from the Grimmchild III shade skin rather than from its
        /// charm icon. Cached including the failure, so a missing skin folder is not re-probed on
        /// every spawn.
        /// </summary>
        private static Sprite? TryLoadGrimmchildSprite()
        {
            if (s_grimmchildSpriteResolved)
            {
                return s_grimmchildSprite;
            }

            s_grimmchildSpriteResolved = true;

            try
            {
                ShadeSkinManager.EnsureLoaded();
                var skin = ShadeSkinManager.Skins?.FirstOrDefault(s => s != null && s.Matches(GrimmchildSkinId));
                if (skin == null)
                {
                    return null;
                }

                string path = ShadeSkinManager.ResolveSpritePath(skin, GrimmchildSheetName);
                if (!File.Exists(path))
                {
                    return null;
                }

                var bytes = File.ReadAllBytes(path);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                if (!texture.LoadImage(bytes))
                {
                    return null;
                }

                // The idle sheet is a horizontal strip; the first frame is the pose to hold.
                int frameWidth = Mathf.Max(1, texture.height);
                int width = Mathf.Min(frameWidth, texture.width);
                s_grimmchildSprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
            }
            catch
            {
                s_grimmchildSprite = null;
            }

            return s_grimmchildSprite;
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
        private static void ApplyVisual(GameObject go, LegacyHelper.ShadeController controller, ShadeCharmId charm, float visualScale)
        {
            var renderer = go.AddComponent<SpriteRenderer>();
            var ownerRenderer = controller != null ? controller.GetComponent<SpriteRenderer>() : null;

            // Hollow Knight's own object for this charm where the bundle has one - a weaverling, a
            // hatchling, the orbit shield. The charm icon standing in for them is what made these
            // read as unimplemented, and it is only the fallback now.
            string? prefabName = BundlePrefabFor(charm);
            if (prefabName != null
                && Knight.KnightEffects.TrySpawnSorted(prefabName, go.transform, ownerRenderer, visualScale) != null)
            {
                // The prefab draws; this renderer only carries the sorting the fallback would use.
                renderer.enabled = false;
                return;
            }

            renderer.sprite = ResolveMinionSprite(controller, charm);
            renderer.color = new Color(1f, 1f, 1f, 0.9f);
            go.transform.localScale = Vector3.one * 0.5f;

            // This companion's renderer, not the primary's: two companions can each be wearing the
            // charm, and sorting a second one's summons against the first draws them in the wrong
            // layer the moment the two are in different scenes' worth of sorting.
            if (ownerRenderer != null)
            {
                renderer.sortingLayerID = ownerRenderer.sortingLayerID;
                renderer.sortingOrder = ownerRenderer.sortingOrder + 1;
            }
        }
    }
}
