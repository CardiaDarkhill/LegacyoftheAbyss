#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// Borrows a single prefab out of the Knight bundle as a piece of art.
    /// <para>
    /// Most of the charms carried over from Hollow Knight have their own object in that bundle -
    /// the weaverlings, the hatchlings, the dung cloud, the spore cloud, the orbit shield, the
    /// flukes - and standing an icon in for them was what made half of them read as unimplemented.
    /// This is the one place that turns such a prefab into something safe to put in a Silksong
    /// scene, so no charm has to work out the stripping for itself.
    /// </para>
    /// <para>
    /// Art only. The prefab supplies what the charm looks like; the damage, the lifetime and the
    /// movement stay with the charm's own code, because the prefab's versions of those are wired
    /// to Hollow Knight's hero and would not survive being pulled out of it.
    /// </para>
    /// </summary>
    internal static class KnightEffects
    {
        /// <summary>Bundle prefab names, kept together so a rename is one edit and one search.</summary>
        internal const string Weaverling = "Weaverling";
        internal const string Hatchling = "Knight Hatchling";
        internal const string OrbitShield = "Orbit Shield";
        internal const string DungCloud = "Knight Dung Cloud";
        internal const string SporeCloud = "Knight Spore Cloud";
        /// <summary>
        /// Thorns of Agony's vines. A clip on the Knight's own rig rather than a prefab: the
        /// bundle's "Charm Thorn Counter" object is the charm's inventory icon and a trigger box,
        /// which is what was being drawn as stray green lines.
        /// </summary>
        internal const string ThornAttackClip = "Thorn Attack";

        /// <summary>The clip's own rate, so the vines burst at the speed they were drawn at.</summary>
        internal const float ThornAttackFps = 20f;
        internal const string SpellFluke = "Spell Fluke";

        /// <summary>
        /// Grubberfly's Elegy beams, one prefab per direction: the art is drawn facing its way
        /// rather than rotated, so the direction picks the prefab instead of an angle.
        /// <para>
        /// Each has an "R" twin in the bundle - Hollow Knight's pooled copy of the same object.
        /// Either draws, so <see cref="TrySpawnFirst"/> takes whichever is present.
        /// </para>
        /// </summary>
        internal static readonly string[] GrubberflyBeamRight = { "Grubberfly BeamR", "Grubberfly BeamR R" };

        internal static readonly string[] GrubberflyBeamLeft = { "Grubberfly BeamL", "Grubberfly BeamL R" };

        internal static readonly string[] GrubberflyBeamUp = { "Grubberfly BeamU", "Grubberfly BeamU R" };

        internal static readonly string[] GrubberflyBeamDown = { "Grubberfly BeamD", "Grubberfly BeamD R" };

        /// <summary>
        /// Instantiates <paramref name="prefabName"/> under <paramref name="parent"/>, stripped of
        /// everything that is not art, and returns it. Null when the bundle or the prefab is
        /// missing, which every caller is expected to handle by keeping whatever it drew before.
        /// </summary>
        internal static GameObject? TrySpawn(string prefabName, Transform parent, float scale = 1f)
        {
            if (string.IsNullOrEmpty(prefabName) || parent == null)
            {
                return null;
            }

            if (!KnightAssets.TryLoad())
            {
                return null;
            }

            var prefab = KnightAssets.FindPrefab(prefabName);
            if (prefab == null)
            {
                return null;
            }

            KnightAssets.ApplyShaders();

            // Staged inactive, exactly as the Knight rig is: Instantiate wakes everything it copies,
            // and one frame is enough for a Hollow Knight FSM to cast a spell or play a sound.
            var stage = new GameObject("KnightEffectStage");
            stage.SetActive(false);

            GameObject instance;
            try
            {
                instance = Object.Instantiate(prefab, stage.transform, worldPositionStays: false);
                instance.name = prefabName;
                instance.SetActive(true);
                Strip(instance);
            }
            catch
            {
                Object.Destroy(stage);
                return null;
            }

            instance.transform.SetParent(parent, worldPositionStays: false);
            instance.transform.localPosition = Vector3.zero;

            // The prefab's own rotation is kept for the same reason its scale is: the up and down
            // Grubberfly beams are the sideways one turned a quarter, and resetting to identity
            // laid them both flat.
            instance.transform.localRotation = prefab.transform.localRotation;

            // Multiplied into the prefab's own scale rather than replacing it. Hollow Knight
            // mirrors a lot of its art with a negative x scale on the prefab - the left-facing and
            // right-facing beams are the same sprite turned around that way - so overwriting it
            // pointed every direction the same way.
            Vector3 prefabScale = prefab.transform.localScale;
            instance.transform.localScale = new Vector3(
                prefabScale.x * scale,
                prefabScale.y * scale,
                prefabScale.z == 0f ? scale : prefabScale.z * scale);

            Object.Destroy(stage);
            return instance;
        }

        /// <summary>
        /// As <see cref="TrySpawn"/>, and additionally draws the effect where the companion draws.
        /// A borrowed prefab keeps Hollow Knight's sorting layers, which do not all exist here, so
        /// an effect left alone routinely lands behind the room.
        /// </summary>
        internal static GameObject? TrySpawnSorted(string prefabName, Transform parent, Renderer? sortLike, float scale = 1f, int sortingOffset = 1, float alpha = 1f)
        {
            var instance = TrySpawn(prefabName, parent, scale);
            if (instance == null)
            {
                return null;
            }

            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                {
                    continue;
                }

                if (sortLike != null)
                {
                    renderer.sortingLayerID = sortLike.sortingLayerID;
                    renderer.sortingOrder = sortLike.sortingOrder + sortingOffset;
                }

                if (alpha < 0.999f)
                {
                    ApplyAlpha(renderer, alpha);
                }
            }

            ApplyParticleTuning(instance, scale, alpha);
            return instance;
        }

        /// <summary>
        /// A bundled animation clip played once on a plain renderer, for art that lives only as an
        /// animation rather than as a prefab of its own - Thorns of Agony's vines are six frames of
        /// "Thorn Attack" on the Knight's own body, with no object in the bundle to borrow.
        /// </summary>
        internal static GameObject? TrySpawnClip(string clipName, float fps, Transform parent, Renderer? sortLike, float scale = 1f)
        {
            if (string.IsNullOrEmpty(clipName) || parent == null || !KnightAssets.TryLoad())
            {
                return null;
            }

            int frameCount = KnightAssets.GetClipFrameCount(clipName);
            if (frameCount <= 0)
            {
                LegacyHelper.LogWarning($"Knight clip '{clipName}' is not in the bundle; whatever asked for it draws nothing.");
                return null;
            }

            var frames = new Sprite?[frameCount];
            bool any = false;
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = KnightAssets.TryBuildSprite(clipName, i);
                any |= frames[i] != null;
            }

            if (!any)
            {
                LegacyHelper.LogWarning($"Knight clip '{clipName}' has {frameCount} frames but none could be cut from the atlas.");
                return null;
            }

            var go = new GameObject("KnightClip_" + clipName);
            go.transform.SetParent(parent, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * scale;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = frames[0];
            if (sortLike != null)
            {
                renderer.sortingLayerID = sortLike.sortingLayerID;
                renderer.sortingOrder = sortLike.sortingOrder + 1;
            }

            // tk2d packs frames rotated to save atlas space and records it on the definition; a
            // Unity Sprite has nowhere to carry that, so it has to be turned back here.
            if (KnightAssets.IsSpriteRotated(clipName, 0))
            {
                go.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            }

            var player = go.AddComponent<KnightClipPlayer>();
            player.Play(renderer, frames, fps);
            return go;
        }

        /// <summary>
        /// Fades a borrowed renderer. Written on the instanced material rather than the shared one:
        /// these materials come out of the bundle and are shared by every copy of the effect, so
        /// tinting the shared material would fade the art everywhere it is used, for the session.
        /// </summary>
        private static void ApplyAlpha(Renderer renderer, float alpha)
        {
            try
            {
                if (renderer is SpriteRenderer sprite)
                {
                    var colour = sprite.color;
                    colour.a *= alpha;
                    sprite.color = colour;
                    return;
                }

                var material = renderer.material;
                if (material != null && material.HasProperty("_Color"))
                {
                    var colour = material.color;
                    colour.a *= alpha;
                    material.color = colour;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Makes a particle effect answer to the scale and opacity it was spawned with.
        /// <para>
        /// A particle system ignores its transform's scale by default, which is why halving the
        /// scale of a cloud that is entirely particles changed nothing about how large it drew.
        /// Its colour is on the system rather than on the renderer's material, for the same reason.
        /// </para>
        /// </summary>
        private static void ApplyParticleTuning(GameObject instance, float scale, float alpha)
        {
            foreach (var system in instance.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (system == null)
                {
                    continue;
                }

                try
                {
                    var main = system.main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;

                    if (alpha < 0.999f)
                    {
                        var start = main.startColor;
                        var colour = start.color;
                        colour.a *= alpha;
                        start.color = colour;
                        main.startColor = start;
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// The first of <paramref name="prefabNames"/> the bundle actually carries, spawned and
        /// sorted. For art that ships under more than one name, so a caller states its preference
        /// as an order rather than probing for each one itself.
        /// </summary>
        internal static GameObject? TrySpawnFirst(string[] prefabNames, Transform parent, Renderer? sortLike, float scale = 1f, int sortingOffset = 1)
        {
            if (prefabNames == null)
            {
                return null;
            }

            foreach (var name in prefabNames)
            {
                var instance = TrySpawnSorted(name, parent, sortLike, scale, sortingOffset);
                if (instance != null)
                {
                    return instance;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes everything that would act rather than draw: the hero's own scripts and FSMs, the
        /// prefab's colliders and bodies (the charm brings its own hitbox), and its audio sources.
        /// <c>DestroyImmediate</c> throughout - <c>Destroy</c> defers to the end of the frame, and
        /// one frame is all a copied FSM needs.
        /// </summary>
        private static void Strip(GameObject instance)
        {
            foreach (var behaviour in instance.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour != null && !IsArtBehaviour(behaviour))
                {
                    Object.DestroyImmediate(behaviour);
                }
            }

            foreach (var collider in instance.GetComponentsInChildren<Collider2D>(true))
            {
                if (collider != null)
                {
                    Object.DestroyImmediate(collider);
                }
            }

            // After the colliders: a Rigidbody2D still carrying colliders is refused.
            foreach (var body in instance.GetComponentsInChildren<Rigidbody2D>(true))
            {
                if (body != null)
                {
                    Object.DestroyImmediate(body);
                }
            }

            foreach (var source in instance.GetComponentsInChildren<AudioSource>(true))
            {
                if (source != null)
                {
                    Object.DestroyImmediate(source);
                }
            }

            // The clouds are a collider, an FSM and a bag of children: a few particle systems
            // that are the cloud, and a row of "Impact Lines" one-shots the FSM was going to fire
            // outward. Without the FSM those just sit there as static streaks - the orange lines
            // reported around Defender's Crest - so they go with it.
            foreach (var child in instance.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && child != instance.transform
                    && child.name.StartsWith("Impact Lines", System.StringComparison.Ordinal))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            // Trails and line renderers draw between points something else is supposed to be
            // moving, and that something is one of the scripts above. Left behind with nothing
            // driving them they stretch a stray streak from wherever the effect was created.
            foreach (var trail in instance.GetComponentsInChildren<TrailRenderer>(true))
            {
                if (trail != null)
                {
                    Object.DestroyImmediate(trail);
                }
            }

            foreach (var line in instance.GetComponentsInChildren<LineRenderer>(true))
            {
                if (line != null)
                {
                    Object.DestroyImmediate(line);
                }
            }
        }

        /// <summary>
        /// What is kept: the tk2d animator and sprite that do the drawing, and the component that
        /// hides a one-shot effect when its clip ends, which is the behaviour a burst wants.
        /// </summary>
        private static bool IsArtBehaviour(MonoBehaviour behaviour)
        {
            // ParticleSystem is not a MonoBehaviour, so it is never in the set this filters and
            // survives the strip on its own.
            return behaviour is tk2dSpriteAnimator
                || behaviour is tk2dBaseSprite
                || behaviour is DeactivateAfter2dtkAnimation;
        }
    }
}
