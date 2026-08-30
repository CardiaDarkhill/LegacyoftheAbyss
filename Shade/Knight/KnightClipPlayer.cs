#nullable enable

using UnityEngine;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// Steps a bundled animation's frames once and then removes itself.
    /// <para>
    /// The Knight's rig plays its clips through a tk2d animator, but some of the art a charm wants
    /// is only a clip on that rig and has no object of its own to borrow - Thorns of Agony's vines
    /// are six frames of "Thorn Attack" and nothing else. Cutting those frames to sprites and
    /// running them on a plain renderer is cheaper than instantiating the whole rig to play one
    /// animation, and it puts the effect where the charm fired rather than on the body.
    /// </para>
    /// </summary>
    internal sealed class KnightClipPlayer : MonoBehaviour
    {
        private SpriteRenderer? renderer;
        private Sprite?[]? frames;
        private float frameTime;
        private float timer;
        private int index;

        internal void Play(SpriteRenderer target, Sprite?[] clipFrames, float fps)
        {
            renderer = target;
            frames = clipFrames;
            frameTime = fps > 0.01f ? 1f / fps : 0.05f;
            timer = 0f;
            index = 0;

            if (renderer != null && frames != null && frames.Length > 0)
            {
                renderer.sprite = frames[0];
            }
        }

        private void Update()
        {
            if (frames == null || frames.Length == 0 || renderer == null)
            {
                Destroy(gameObject);
                return;
            }

            timer += Time.deltaTime;
            while (timer >= frameTime)
            {
                timer -= frameTime;
                index++;

                if (index >= frames.Length)
                {
                    // Played once. The burst that spawned this owns its own lifetime, but the art
                    // must not sit on its last frame waiting for it.
                    Destroy(gameObject);
                    return;
                }

                if (frames[index] != null)
                {
                    renderer.sprite = frames[index];
                }
            }
        }
    }
}
