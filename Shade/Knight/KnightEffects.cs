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


        internal const string SpellFluke = "Spell Fluke";

        /// <summary>
        /// Grubberfly's Elegy's beam, as one prefab that the caller turns.
        /// <para>
        /// The bundle has four of these and their baked transforms do not mean what the names
        /// suggest: BeamD is byte-for-byte BeamL, and BeamU is BeamL turned +90 degrees, which
        /// points it down. Hollow Knight sets the real orientation from the FSM at spawn, and that
        /// is stripped out of anything borrowed here - so picking by name gave a beam facing down
        /// when fired up and left when fired down. BeamL is the clean base: no mirroring, no
        /// rotation, art pointing left. See <see cref="GrubberflyBeamArtAngle"/>.
        /// </para>
        /// </summary>
        internal static readonly string[] GrubberflyBeam = { "Grubberfly BeamL", "Grubberfly BeamL R" };

        /// <summary>Which way <see cref="GrubberflyBeam"/> is drawn, so a caller can turn it.</summary>
        internal const float GrubberflyBeamArtAngle = 180f;

        /// <summary>
        /// A small effect prefab off the Knight's shared clip library, used as a body to play a
        /// clip on. The whole rig shares one library, so any of its effect objects can play any of
        /// its clips - and borrowing one means the animation draws through the bundle's own
        /// tk2dSprite and material, which is the path every effect that works here already takes.
        /// </summary>
        internal const string ClipHost = "Shadow Burst";

        /// <summary>
        /// Instantiates <paramref name="prefabName"/> under <paramref name="parent"/>, stripped of
        /// everything that is not art, and returns it. Null when the bundle or the prefab is
        /// missing, which every caller is expected to handle by keeping whatever it drew before.
        /// </summary>
        internal static GameObject? TrySpawn(string prefabName, Transform parent, float scale = 1f, bool keepPrefabTransform = true)
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

            try
            {
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
                    return null;
                }

                WakeArt(instance);

                instance.transform.SetParent(parent, worldPositionStays: false);
                instance.transform.localPosition = Vector3.zero;

                // The prefab's own rotation is kept for the same reason its scale is: the up and
                // down Grubberfly beams are the sideways one turned a quarter, and resetting to
                // identity laid them both flat.
                // Kept when the prefab *is* the art, dropped when it is only a body to play someone
                // else's clip on - a host's own rotation and scale say nothing about the animation
                // being played on it, and Shadow Burst's baked 1.92 was what drew the thorns
                // oversized.
                if (!keepPrefabTransform)
                {
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = Vector3.one * scale;
                }
                else
                {
                    instance.transform.localRotation = prefab.transform.localRotation;

                    // Multiplied into the prefab's own scale rather than replacing it. Hollow Knight
                    // mirrors a lot of its art with a negative x scale on the prefab - the
                    // left-facing and right-facing beams are the same sprite turned around that way
                    // - so overwriting it pointed every direction the same way.
                    Vector3 prefabScale = prefab.transform.localScale;
                    instance.transform.localScale = new Vector3(
                        prefabScale.x * scale,
                        prefabScale.y * scale,
                        prefabScale.z == 0f ? scale : prefabScale.z * scale);
                }

                return instance;
            }
            finally
            {
                // Always, not only on the two paths that used to say so: anything thrown past the
                // inner catch would otherwise leave the stage - and the instance still parented to
                // it - in the scene for the rest of the session.
                Object.Destroy(stage);
            }
        }

        /// <summary>
        /// As <see cref="TrySpawn"/>, and additionally draws the effect where the companion draws.
        /// A borrowed prefab keeps Hollow Knight's sorting layers, which do not all exist here, so
        /// an effect left alone routinely lands behind the room.
        /// </summary>
        internal static GameObject? TrySpawnSorted(string prefabName, Transform parent, Renderer? sortLike, float scale = 1f, int sortingOffset = 1, float alpha = 1f, bool keepPrefabTransform = true)
        {
            var instance = TrySpawn(prefabName, parent, scale, keepPrefabTransform);
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

            ApplyParticleTuning(instance, alpha);
            return instance;
        }

        /// <summary>
        /// The radius Hollow Knight itself gave this effect, in world units, or 0 if it carries no
        /// circle to read.
        /// <para>
        /// The clouds are prefabs with a <c>CircleCollider2D</c> on the root, and that circle is the
        /// game's own statement of how big the cloud is - <c>Knight Spore Cloud</c> and
        /// <c>Knight Dung Cloud</c> both hold 6.06, the spore one at a prefab scale of 1.35, so 8.18
        /// units of reach. Read from the prefab rather than the instance, because
        /// <see cref="Strip"/> destroys every collider on the way in.
        /// </para>
        /// </summary>
        internal static float TryGetPrefabRadius(string prefabName)
        {
            try
            {
                if (string.IsNullOrEmpty(prefabName) || !KnightAssets.TryLoad())
                {
                    return 0f;
                }

                var prefab = KnightAssets.FindPrefab(prefabName);
                var circle = prefab != null ? prefab.GetComponent<CircleCollider2D>() : null;
                if (circle == null)
                {
                    return 0f;
                }

                var scale = prefab!.transform.localScale;
                return Mathf.Abs(circle.radius) * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>
        /// Scales a spawned cloud so it draws at <paramref name="worldRadius"/> rather than at the
        /// size Hollow Knight built it.
        /// <para>
        /// A charm cloud is two things that were never told about each other: a circle that does the
        /// damage, and a borrowed effect that does the drawing. Spore Shroom's circle is 3.4 units
        /// and its effect is authored for 8.18, so an enemy standing well inside the visible cloud
        /// was nowhere near the volume - reported, reasonably, as two separate bugs.
        /// </para>
        /// <para>
        /// The factor comes from the prefab's own collider and nothing else. An earlier attempt
        /// estimated the drawn reach from the particles - where they start plus how far they travel
        /// before they die - and was wrong by a factor of fifty, because every system in these
        /// clouds has a velocity clamp of zero sitting on top of a start speed of up to 48: they are
        /// authored to be flung outward and stopped dead. Nothing about a particle system's numbers
        /// tells you how big it looks. The collider does.
        /// </para>
        /// </summary>
        internal static void ScaleCloudToRadius(GameObject? instance, string prefabName, float worldRadius)
        {
            if (instance == null || worldRadius <= 0f)
            {
                return;
            }

            // The radius first: it is the call that loads the bundle, and FindPrefab answers null
            // until it has.
            float authored = TryGetPrefabRadius(prefabName);
            var prefab = KnightAssets.FindPrefab(prefabName);
            if (prefab == null || authored <= 0.0001f)
            {
                return;
            }

            // Set from the prefab's own scale rather than multiplied into whatever the instance is
            // already holding. A caller that also passed an effectScale would otherwise have both
            // applied - Defender's Crest asked for 0.65 and came out at 0.22 - and the result would
            // depend on the order the two happened to run in. Multiplying the prefab's scale rather
            // than replacing it keeps the mirroring Hollow Knight bakes into its art as a negative x.
            // x and y only: TrySpawn already sorted the z out, and a prefab authored with a zero z
            // scale would be scaled to nothing here.
            Vector3 scaled = prefab.transform.localScale * (worldRadius / authored);
            Vector3 held = instance.transform.localScale;
            instance.transform.localScale = new Vector3(scaled.x, scaled.y, held.z);
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
        private static void ApplyParticleTuning(GameObject instance, float alpha)
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
        /// Switches the art back on.
        /// <para>
        /// The bundle's effect objects ship inactive twice over - their renderers disabled and
        /// their animators disabled - because Hollow Knight turns them on from the FSM that fires
        /// them, and that FSM is the first thing stripped here. A borrowed effect that is never
        /// woken is instantiated, parented and positioned correctly, and draws nothing at all.
        /// The parent chain matters too: activating a child of an inactive object shows nothing.
        /// </para>
        /// </summary>
        private static void WakeArt(GameObject instance)
        {
            foreach (var child in instance.GetComponentsInChildren<Transform>(true))
            {
                if (child != null && !child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }

            foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }

            // Every Behaviour still here is art - Strip removed the rest - so they all go back on.
            // Enabling only the animator was not enough: a tk2dSprite ships disabled too, and a
            // disabled one never builds its mesh, so the animator sets frames on something that
            // draws nothing and no part of it reports a problem.
            // Renderer is not a Behaviour in Unity, so the loop above is the only thing that
            // reaches them and this one cannot double back over them.
            foreach (var behaviour in instance.GetComponentsInChildren<Behaviour>(true))
            {
                if (behaviour != null)
                {
                    behaviour.enabled = true;
                }
            }
        }

        /// <summary>
        /// Borrows <see cref="ClipHost"/> and plays <paramref name="clipName"/> on it, for art that
        /// exists only as an animation on the Knight's rig and has no object of its own - Thorns of
        /// Agony's vines being the case this was written for. Preferred over cutting the clip to
        /// sprites by hand, because it draws through the bundle's own material rather than through
        /// a renderer built here with whatever material happens to be the default.
        /// </summary>
        internal static GameObject? TrySpawnAnimatedClip(string clipName, Transform parent, Renderer? sortLike, float scale = 1f, float alpha = 1f, Bounds? fitToBody = null)
        {
            if (string.IsNullOrEmpty(clipName) || parent == null)
            {
                return null;
            }

            var host = TrySpawnSorted(
                ClipHost, parent, sortLike, scale, sortingOffset: 1, alpha: alpha, keepPrefabTransform: false);
            if (host == null)
            {
                LegacyHelper.LogWarning($"Knight clip '{clipName}' has no host to play on ('{ClipHost}' is not in the bundle).");
                return null;
            }

            var animator = host.GetComponent<tk2dSpriteAnimator>()
                ?? host.GetComponentInChildren<tk2dSpriteAnimator>(true);
            if (animator == null)
            {
                LegacyHelper.LogWarning($"Knight clip host '{ClipHost}' carries no animator, so '{clipName}' cannot play.");
                Object.Destroy(host);
                return null;
            }

            var clip = animator.Library != null ? animator.Library.GetClipByName(clipName) : null;
            if (clip == null)
            {
                LegacyHelper.LogWarning($"Knight clip '{clipName}' is not in the host's library.");
                Object.Destroy(host);
                return null;
            }

            animator.enabled = true;
            animator.Play(clip);

            // Sized and placed against the body it belongs to, measured rather than guessed.
            // Playing the clip builds the mesh, so by here the renderer knows how big the art is.
            if (fitToBody.HasValue)
            {
                FitToBody(host, fitToBody.Value, scale);
            }

            DescribeClipOnce(clipName, host, animator);
            return host;
        }

        /// <summary>
        /// Scales and places a borrowed clip so the character drawn inside it lines up with the
        /// character it is being played for.
        /// <para>
        /// Several of these clips are the Knight's <em>own body</em> doing something - Thorns of
        /// Agony is the Knight spewing thorns, mask and all - so the size that reads correctly is
        /// the one where that body matches the real one, not one derived from the damage volume.
        /// Measured on the first frame, which is the pose before the effect has grown out of it,
        /// and both axes are taken from the same factor so nothing is stretched.
        /// </para>
        /// </summary>
        private static void FitToBody(GameObject instance, Bounds body, float bias)
        {
            try
            {
                var renderer = instance.GetComponentInChildren<Renderer>(true);
                if (renderer == null || body.size.y <= 0.001f)
                {
                    return;
                }

                // The bias is re-applied on top, because matching the body cancels out whatever
                // scale the effect was spawned with - without this a caller asking for a slightly
                // larger or smaller burst would silently get neither.
                float drawnHeight = renderer.bounds.size.y;
                if (drawnHeight > 0.001f)
                {
                    instance.transform.localScale *= (body.size.y / drawnHeight) * Mathf.Max(0.05f, bias);
                }

                // Centred on the body rather than dropped at its transform: the companion's origin
                // is at its feet and the clip's is its middle, so placing one at the other drew the
                // whole burst half a body too low.
                Vector3 drift = body.center - renderer.bounds.center;
                instance.transform.position += drift;
            }
            catch
            {
            }
        }

        private static readonly System.Collections.Generic.HashSet<string> s_describedClips = new();

        /// <summary>
        /// Reports what a borrowed animation resolved to, the first time each clip is played.
        /// <para>
        /// "It draws nothing" and "it was never spawned" look identical from outside, and this one
        /// has now been diagnosed wrong several times from their absence. One line, once per clip:
        /// enough to tell a missing renderer from a transparent material from an object sitting at
        /// the origin, without adding a line per hit taken.
        /// </para>
        /// </summary>
        private static void DescribeClipOnce(string clipName, GameObject host, tk2dSpriteAnimator animator)
        {
            if (!s_describedClips.Add(clipName))
            {
                return;
            }

            try
            {
                var renderer = host.GetComponentInChildren<Renderer>(true);
                string material = renderer != null && renderer.sharedMaterial != null
                    ? renderer.sharedMaterial.name
                    : "none";
                string sortingLayer = renderer != null ? renderer.sortingLayerName : "?";
                int sortingOrder = renderer != null ? renderer.sortingOrder : 0;

                LegacyHelper.LogInfo(
                    $"Knight clip '{clipName}' playing on '{host.name}': active={host.activeInHierarchy} "
                    + $"renderer={(renderer != null ? renderer.enabled.ToString() : "missing")} "
                    + $"material={material} sorting={sortingLayer}:{sortingOrder} "
                    + $"pos={host.transform.position} scale={host.transform.lossyScale} "
                    + $"animatorEnabled={animator.enabled} playing={animator.Playing}");
            }
            catch
            {
            }
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
