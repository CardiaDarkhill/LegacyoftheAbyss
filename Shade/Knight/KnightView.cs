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
        internal const string ClipCollect = "Collect Normal";

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

            if (!restart && currentClip == clipName && animator.IsPlaying(clipName))
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
