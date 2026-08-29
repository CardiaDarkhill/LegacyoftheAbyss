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
        internal const string ClipAirborne = "Airborne";
        internal const string ClipLand = "Land";
        internal const string ClipDash = "Dash";
        internal const string ClipWallSlide = "Wall Slide";
        internal const string ClipDoubleJump = "Double Jump";
        internal const string ClipCollect = "Collect Normal 1";
        internal const string ClipShadeCloak = "Shadow Dash";
        internal const string ClipShadeCloakReady = "Shadow Recharge";
        internal const string ClipMap = "Map Open";

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
            var instance = Instantiate(prefab, transform, worldPositionStays: false);
            instance.name = "KnightRig";
            instance.transform.localRotation = Quaternion.identity;
            instance.SetActive(true);
            rig = instance.transform;

            StripHeroBehaviour(instance);

            animator = instance.GetComponent<tk2dSpriteAnimator>()
                ?? instance.GetComponentInChildren<tk2dSpriteAnimator>(true);

            if (animator == null)
            {
                LegacyHelper.LogWarning("Knight rig has no tk2dSpriteAnimator; the Knight cannot animate.");
                Destroy(instance);
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

            // The body transform is already scaled and the rig is its child, so the prefab's own
            // scale is the baseline - scaling by the body's own factor again multiplied the two and
            // produced a Knight several times the intended size. What the prefab ships at still
            // stands nearly as tall as Hornet, though, so knightScale brings it down to her.
            instance.transform.localScale *= Mathf.Max(0.01f, ModConfig.Instance.knightScale);
            baseScaleX = Mathf.Abs(instance.transform.localScale.x);

            instance.transform.localPosition = Vector3.zero;

            DisableStrayRenderers();
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

            foreach (var particles in rig.GetComponentsInChildren<ParticleSystem>(true))
            {
                Destroy(particles);
            }
        }

        /// <summary>
        /// Removes everything on the rig that would act on its own. The prefab carries Hollow
        /// Knight's FSMs, physics and hitboxes, and left in place they fight the companion body
        /// this rig is parented to — its collider is the one that matters.
        /// </summary>
        private static void StripHeroBehaviour(GameObject instance)
        {
            foreach (var fsm in instance.GetComponentsInChildren<PlayMakerFSM>(true))
            {
                Destroy(fsm);
            }

            // Before the Rigidbody2D goes: this one holds a reference to it and calls IsAwake every
            // frame, so removing the body underneath it threw a NullReferenceException per frame
            // for the rest of the session.
            foreach (var particleScale in instance.GetComponentsInChildren<SetParticleScale>(true))
            {
                Destroy(particleScale);
            }

            foreach (var body in instance.GetComponentsInChildren<Rigidbody2D>(true))
            {
                Destroy(body);
            }

            foreach (var collider in instance.GetComponentsInChildren<Collider2D>(true))
            {
                Destroy(collider);
            }

            // Audio comes from the companion's own audio path; the rig's sources would double it.
            foreach (var source in instance.GetComponentsInChildren<AudioSource>(true))
            {
                Destroy(source);
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
