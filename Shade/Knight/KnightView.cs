#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// The Knight's visuals: the bundled prefab instantiated under a companion body, cut down to
    /// art, and driven by clip name. The prefab ships as Hollow Knight's whole hero rig, so
    /// everything that would try to *be* a hero is stripped on the way in — what is left is the
    /// sprite animator and its renderers.
    /// </summary>
    internal sealed class KnightView : MonoBehaviour
    {
        // Clip names as they exist in the bundled tk2d library.
        internal const string ClipIdle = "Idle";
        internal const string ClipRun = "Run";

        /// <summary>The Sprintmaster walk cycle, played in place of <see cref="ClipRun"/>.</summary>
        internal const string ClipSprint = "Sprint";

        /// <summary>The ordinary focus pose, held for the length of the channel.</summary>
        internal const string ClipFocus = "Focus";

        /// <summary>
        /// Shape of Unn's focus set: the Knight takes a slug's form and can crawl while channelling.
        /// <para>
        /// The bundle also carries "B", "S" and "BS" suffixed variants of both of these, which are
        /// the Baldur Shell and Spore Shroom combinations. They are not wired up - which suffix is
        /// which has not been confirmed against the art, and a wrong guess draws the wrong charm.
        /// </para>
        /// </summary>
        internal const string ClipSlugIdle = "Slug Idle";

        /// <summary>The crawl half of Shape of Unn's focus set. See <see cref="ClipSlugIdle"/>.</summary>
        internal const string ClipSlugWalk = "Slug Walk";
        internal const string ClipAirborne = "Airborne";
        internal const string ClipLand = "Land";
        internal const string ClipDash = "Dash";
        internal const string ClipWallSlide = "Wall Slide";
        internal const string ClipDoubleJump = "Double Jump";
        internal const string ClipCollect = "Collect Normal 1";
        internal const string ClipShadeCloak = "Shadow Dash";

        /// <summary>Shade Cloak with Sharp Shadow worn - the body sharpens into the dash.</summary>
        internal const string ClipShadeCloakSharp = "Shadow Dash Sharp";
        internal const string ClipShadeCloakReady = "Shadow Recharge";
        internal const string ClipMap = "Map Open";
        internal const string ClipSit = "Sit";
        internal const string ClipSitIdle = "Sit Idle";

        // The spells. Each has a plain and an upgraded pose, as Hollow Knight's own do.
        internal const string ClipFireball = "Fireball1 Cast";
        internal const string ClipFireballUpgraded = "Fireball2 Cast";
        internal const string ClipScream = "Scream";
        internal const string ClipScreamUpgraded = "Scream 2";
        internal const string ClipQuakeAntic = "Quake Antic";
        internal const string ClipQuakeFall = "Quake Fall";
        internal const string ClipQuakeFallUpgraded = "Quake Fall 2";
        internal const string ClipQuakeLand = "Quake Land";
        internal const string ClipQuakeLandUpgraded = "Quake Land 2";

        // The wings are their own object under Effects, sharing the rig's single 214-clip library.
        internal const string ClipMonarchWings = "Double Jump Wings 2";

        private static readonly System.Collections.Generic.HashSet<string> s_missingClips = new();

        private tk2dSpriteAnimator? animator;
        private Transform? rig;
        private string? currentClip;
        private int facing;
        private float baseScaleX = 1f;
        private bool footAligned;

        /// <summary>
        /// Builds the Knight rig under <paramref name="body"/>. Returns null when the bundle is
        /// unavailable, so the caller can fall back rather than spawn an invisible companion.
        /// </summary>
        internal static KnightView? Attach(GameObject body)
        {
            if (!KnightAssets.TryLoad())
            {
                return null;
            }

            KnightAssets.ApplyShaders();

            var prefab = KnightAssets.KnightPrefab;
            if (prefab == null)
            {
                return null;
            }

            var view = body.AddComponent<KnightView>();
            if (!view.Build(prefab))
            {
                Destroy(view);
                return null;
            }

            return view;
        }

        private bool Build(GameObject prefab)
        {
            // Staged under an inactive holder so that nothing on the prefab ever gets to run.
            // Instantiate wakes everything it copies, and this prefab is Hollow Knight's entire hero
            // - FSMs included. One frame is all they need: the two hero_fireball clips and the
            // explosion that played on every room entry and every character swap were the Knight's
            // own spell FSMs casting in the frame before StripHeroBehaviour could reach them.
            var stage = new GameObject("KnightRigStage");
            stage.SetActive(false);

            var instance = Instantiate(prefab, stage.transform, worldPositionStays: false);
            instance.name = "KnightRig";
            instance.transform.localRotation = Quaternion.identity;

            // Active within an inactive parent, so still nothing awakens.
            instance.SetActive(true);
            rig = instance.transform;

            StripHeroBehaviour(instance);

            animator = instance.GetComponent<tk2dSpriteAnimator>()
                ?? instance.GetComponentInChildren<tk2dSpriteAnimator>(true);

            if (animator == null)
            {
                LegacyHelper.LogWarning("Knight rig has no tk2dSpriteAnimator; the Knight cannot animate.");
                Destroy(stage);
                rig = null;
                return false;
            }

            // The prefab ships its animator switched off - it expects Hollow Knight's own
            // HeroController to drive it. Without this the clips resolve and Play is accepted, but
            // tk2d never ticks them on, so the Knight holds whatever frame it was left on and reads
            // as having no animation at all.
            animator.enabled = true;
            if (!animator.gameObject.activeSelf)
            {
                animator.gameObject.SetActive(true);
            }

            DisableStrayRenderers();

            // Only now does anything on the rig wake, and by this point it is art and nothing else.
            instance.transform.SetParent(transform, worldPositionStays: false);
            Destroy(stage);

            // The body transform is already scaled and the rig is its child, so the prefab's own
            // scale is the baseline - scaling by the body's own factor again multiplied the two and
            // produced a Knight several times the intended size. What the prefab ships at still
            // stands nearly as tall as Hornet, though, so knightScale brings it down to her.
            instance.transform.localScale *= Mathf.Max(0.01f, ModConfig.Instance.knightScale);
            baseScaleX = Mathf.Abs(instance.transform.localScale.x);

            instance.transform.localPosition = Vector3.zero;
            return true;
        }

        /// <summary>
        /// Leaves only the animator's own renderer drawing. The prefab carries Hollow Knight's hero
        /// glow and vignette pieces, and once they are forced onto the companion's sorting layer by
        /// <see cref="ApplySorting"/> they draw over the room and take the scene's lighting with them.
        /// </summary>
        private void DisableStrayRenderers()
        {
            if (rig == null || animator == null)
            {
                return;
            }

            var keep = animator.GetComponent<Renderer>();
            foreach (var renderer in rig.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer != null && renderer != keep)
                {
                    renderer.enabled = false;
                }
            }

            // The whole object, immediately, and not the component: ParticleSystemRenderer depends on
            // ParticleSystem so removing one alone is refused, and a deferred Destroy would leave it
            // emitting until the end of the frame - which is after the rig goes live.
            foreach (var particles in rig.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (particles != null)
                {
                    DestroyImmediate(particles.gameObject);
                }
            }
        }

        /// <summary>
        /// Behaviours the rig keeps. Everything else on it is Hollow Knight's hero, and this is an
        /// allow-list rather than a list of things to remove because the ways that rig can act on
        /// its own kept turning out to be one wider than whatever had just been removed: first its
        /// FSMs casting spells, then a <c>PersonalObjectPool</c> warming Hollow Knight's effect
        /// prefabs into Silksong's global pool and setting one off every time a room loaded.
        /// </summary>
        private static bool IsArtBehaviour(MonoBehaviour behaviour)
        {
            return behaviour is tk2dSpriteAnimator
                || behaviour is tk2dBaseSprite
                || behaviour is DeactivateAfter2dtkAnimation;
        }

        /// <summary>
        /// Removes everything on the rig that would act on its own. The prefab carries Hollow
        /// Knight's whole hero - FSMs, physics, hitboxes, pooling - and left in place they fight
        /// the companion body this rig is parented to, or worse act on the world around it.
        /// <para>
        /// <see cref="DestroyImmediate"/> throughout, and that is load-bearing rather than a style
        /// choice. <c>Destroy</c> defers to the end of the frame, so with the staged build the FSMs
        /// were still attached at the moment the rig was reparented and woke up - and they cast.
        /// The first version of the staging fix therefore turned two stray sounds into a Shade Soul
        /// and a Vengeful Spirit flying across the room, because by then the effects were live under
        /// the companion rather than orphaned. Nothing here has awoken yet, so immediate removal is
        /// safe: Unity does not call <c>OnDestroy</c> on a component whose <c>Awake</c> never ran.
        /// </para>
        /// </summary>
        private static void StripHeroBehaviour(GameObject instance)
        {
            // MonoBehaviours first, and SetParticleScale is the reason the order matters: it holds
            // the Rigidbody2D and calls IsAwake every frame, so removing the body from under it
            // threw a NullReferenceException per frame for the rest of the session.
            foreach (var behaviour in instance.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour != null && !IsArtBehaviour(behaviour))
                {
                    DestroyImmediate(behaviour);
                }
            }

            foreach (var collider in instance.GetComponentsInChildren<Collider2D>(true))
            {
                if (collider != null)
                {
                    DestroyImmediate(collider);
                }
            }

            // After the colliders: a Rigidbody2D with colliders still attached to it is refused.
            foreach (var body in instance.GetComponentsInChildren<Rigidbody2D>(true))
            {
                if (body != null)
                {
                    DestroyImmediate(body);
                }
            }

            // Audio comes from the companion's own audio path; the rig's sources would double it.
            foreach (var source in instance.GetComponentsInChildren<AudioSource>(true))
            {
                if (source != null)
                {
                    DestroyImmediate(source);
                }
            }
        }

        /// <summary>Copies the companion's sorting so the Knight draws where a Shade would.</summary>
        internal void ApplySorting(int sortingLayerId, int sortingOrder)
        {
            if (rig == null)
            {
                return;
            }

            foreach (var renderer in rig.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sortingLayerID = sortingLayerId;
                renderer.sortingOrder = sortingOrder;
            }
        }

        /// <summary>Whether the rig is currently drawing. The Shade answers this with its renderer.</summary>
        internal bool IsVisible => rig != null && rig.gameObject.activeSelf;

        // The rig's one-shot effects live as their own children under "Effects", each with its own
        // animator sharing the body's clip library. Names are the prefab's, verified against the
        // bundle: guessing at them ("Wings", "Monarch Wings") found nothing and cost the flourish
        // silently twice.
        internal const string WingsObjectName = "Double J Wings";
        internal const string ShadeCloakReadyObjectName = "Shadow Recharge";

        private readonly System.Collections.Generic.Dictionary<string, EffectObject> effects = new();

        private sealed class EffectObject
        {
            internal Transform? Root;
            internal tk2dSpriteAnimator? Animator;
            internal float HideAt;
        }

        /// <summary>Shows the Monarch Wings for the length of the double jump.</summary>
        internal void FlashMonarchWings(float seconds = 0.45f)
            => FlashEffect(WingsObjectName, ClipMonarchWings, seconds);

        /// <summary>Shows the burst Hollow Knight plays the moment Shade Cloak comes back.</summary>
        internal void FlashShadeCloakReady(float seconds = 0.6f)
            => FlashEffect(ShadeCloakReadyObjectName, ClipShadeCloakReady, seconds);

        /// <summary>
        /// Plays one of the rig's own effect objects. These are switched off twice over on the way
        /// in - <see cref="DisableStrayRenderers"/> takes their renderers and the prefab ships their
        /// animators disabled - so all three have to be put back, and the parent chain with them:
        /// activating a child of an inactive parent draws nothing.
        /// </summary>
        private void FlashEffect(string objectName, string clipName, float seconds)
        {
            var effect = ResolveEffect(objectName);
            if (effect?.Root == null)
            {
                return;
            }

            foreach (var renderer in effect.Root.GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
            }

            for (var node = effect.Root; node != null && node != rig; node = node.parent)
            {
                node.gameObject.SetActive(true);
            }

            if (effect.Animator != null)
            {
                effect.Animator.enabled = true;
                var clip = effect.Animator.Library != null ? effect.Animator.Library.GetClipByName(clipName) : null;
                if (clip != null)
                {
                    effect.Animator.Play(clip);
                }
                else if (s_missingClips.Add(clipName))
                {
                    LegacyHelper.LogWarning($"Knight effect clip '{clipName}' is not in the bundled clip library.");
                }
            }

            effect.HideAt = Time.time + seconds;
        }

        private EffectObject? ResolveEffect(string objectName)
        {
            if (effects.TryGetValue(objectName, out var cached))
            {
                return cached;
            }

            var resolved = new EffectObject();
            effects[objectName] = resolved;

            if (rig == null)
            {
                return resolved;
            }

            foreach (var child in rig.GetComponentsInChildren<Transform>(true))
            {
                if (string.Equals(child.name, objectName, System.StringComparison.Ordinal))
                {
                    resolved.Root = child;
                    resolved.Animator = child.GetComponent<tk2dSpriteAnimator>()
                        ?? child.GetComponentInChildren<tk2dSpriteAnimator>(true);
                    return resolved;
                }
            }

            // Naming the children in the warning rather than pointing at the bug report: the last two
            // attempts at this both failed on the name alone, with nothing on hand to correct it from.
            var names = new System.Collections.Generic.List<string>();
            foreach (var child in rig.GetComponentsInChildren<Transform>(true))
            {
                names.Add(child.name);
            }

            LegacyHelper.LogWarning(
                $"Knight effect object '{objectName}' is not in the rig. It carries: {string.Join(", ", names)}");
            return resolved;
        }

        private void Update()
        {
            foreach (var effect in effects.Values)
            {
                if (effect.Root == null || effect.HideAt <= 0f || Time.time < effect.HideAt)
                {
                    continue;
                }

                effect.HideAt = 0f;
                foreach (var renderer in effect.Root.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.enabled = false;
                }
            }
        }

        internal void SetVisible(bool visible)
        {
            if (rig != null && rig.gameObject.activeSelf != visible)
            {
                rig.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Puts the rig's rendered bottom on the body's foot line. Measured rather than assumed:
        /// the prefab's pivot is not ours to control, and guessing at it left the Knight drawn half
        /// a body into the floor. Deferred until the animator has a frame, because the renderer
        /// reports empty bounds until then, and done once - after that the rig rides the body.
        /// </summary>
        internal void AlignFeetTo(float targetBottomWorldY)
        {
            if (footAligned || rig == null || animator == null)
            {
                return;
            }

            var renderer = animator.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }

            var bounds = renderer.bounds;
            if (bounds.size.y <= 0.0001f)
            {
                return;
            }

            rig.position += new Vector3(0f, targetBottomWorldY - bounds.min.y, 0f);
            footAligned = true;
        }

        private float appliedLift;

        /// <summary>
        /// What the Knight actually occupies on screen, or false when the animator has no frame yet.
        /// Used for placing it against another character's silhouette, which is the only way to say
        /// "just touching" - transform origins sit wherever each rig's author put them.
        /// </summary>
        internal bool TryGetRenderedBounds(out Bounds bounds)
        {
            bounds = default;
            var renderer = animator != null ? animator.GetComponent<Renderer>() : null;
            if (renderer == null)
            {
                return false;
            }

            bounds = renderer.bounds;
            return bounds.size.x > 0.0001f;
        }

        /// <summary>
        /// Raises the drawn rig off its feet without moving the body.
        /// <para>
        /// For sitting. The seat is a few inches above where the Knight would stand, and the honest
        /// way to say that is with the sprite rather than with the collider: pinning the body above
        /// the floor meant abandoning gravity and the ground probes, and because Hornet's transform
        /// origin sits near her middle rather than at her feet, an offset measured from her put the
        /// Knight most of a body-height into the air.
        /// </para>
        /// </summary>
        internal void SetLift(float lift)
        {
            if (rig == null || Mathf.Approximately(lift, appliedLift))
            {
                return;
            }

            // World space, matching AlignFeetTo: the body this rides may be scaled, and a local
            // offset would then mean a different distance than it says.
            rig.position += new Vector3(0f, lift - appliedLift, 0f);
            appliedLift = lift;
        }

        /// <summary>
        /// The rig is authored facing left, as the Shade's own sheets are, so facing right is the
        /// mirrored one. Taking Abs of the prefab's scale and treating positive as right had the
        /// Knight looking the way it was walking away from.
        /// </summary>
        internal void SetFacing(int newFacing)
        {
            if (rig == null || newFacing == 0 || facing == newFacing)
            {
                return;
            }

            facing = newFacing;
            var scale = rig.localScale;
            scale.x = baseScaleX * (newFacing >= 0 ? -1f : 1f);
            rig.localScale = scale;
        }

        /// <summary>
        /// Plays a clip, ignoring a repeat of the one already running so a per-frame caller does
        /// not restart it. A clip name the bundle does not carry costs one animation rather than
        /// the companion, but is reported once - the clip names are a contract with an asset bundle
        /// we do not build, and a silent miss here looks exactly like a movement bug.
        /// </summary>
        /// <summary>
        /// Whether the bundle actually carries a clip. <see cref="Play"/> leaves the current
        /// animation running when asked for one it cannot find, so a caller with a fallback has to
        /// ask first or an optional clip freezes the Knight on whatever it was doing.
        /// </summary>
        internal bool HasClip(string clipName)
        {
            if (animator == null || animator.Library == null || string.IsNullOrEmpty(clipName))
            {
                return false;
            }

            return animator.Library.GetClipByName(clipName) != null;
        }

        internal void Play(string clipName, bool restart = false)
        {
            if (animator == null || string.IsNullOrEmpty(clipName))
            {
                return;
            }

            // Guarded on what was last asked for rather than on IsPlaying: if the animator ever
            // reports not-playing while holding the clip, an IsPlaying guard re-Plays every frame
            // and the animation restarts from frame zero forever, which looks like a frozen sprite.
            if (!restart && currentClip == clipName)
            {
                return;
            }

            var clip = animator.Library != null ? animator.Library.GetClipByName(clipName) : null;
            if (clip == null)
            {
                if (s_missingClips.Add(clipName))
                {
                    LegacyHelper.LogWarning($"Knight animation '{clipName}' is not in the bundled clip library; that state will not animate.");
                }

                return;
            }

            currentClip = clipName;
            animator.Play(clip);
        }

    }
}
