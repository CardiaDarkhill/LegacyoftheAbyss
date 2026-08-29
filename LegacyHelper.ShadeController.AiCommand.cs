#nullable disable
using System;
using LegacyoftheAbyss.Shade.Ai;
using UnityEngine;

public partial class LegacyHelper
{
    public partial class ShadeController
    {
        /// <summary>
        /// Where the player is in the process of telling the Shade where to stand.
        /// <para>
        /// One button drives all of it, which is the whole point: tap it twice and the Shade holds
        /// where it already is, or tap, aim, tap and it goes where you pointed. Both end in the same
        /// state - the only difference is whether the reticle moved in between - so there is no
        /// second control to learn and no mode to get stuck in.
        /// </para>
        /// </summary>
        private enum ShadeAiCommandState
        {
            Inactive,
            Aiming,
            Holding
        }

        /// <summary>How fast the stick pushes the reticle, in world units per second.</summary>
        private const float AiCommandReticleSpeed = 16f;

        /// <summary>
        /// World units per unit of mouse movement. Mouse aiming is relative rather than absolute -
        /// see <see cref="ReadReticleMouseDelta"/> for why it has to be.
        /// </summary>
        private const float AiCommandMouseSpeed = 0.55f;

        /// <summary>Mouse movement below this is a twitch, not aiming.</summary>
        private const float AiCommandMouseDeadzone = 0.01f;

        /// <summary>Alpha the reticle drops to once the order is placed and it is just a marker.</summary>
        private const float AiCommandPlacedAlpha = 0.45f;

        /// <summary>
        /// Slack added to the distance an order was placed at, before the leash may pull the Shade
        /// off it.
        /// <para>
        /// An order is an explicit instruction, so the leash has to be able to honour the one that
        /// was actually given - including one placed past the screen edge, which the camera's own
        /// look-up/look-down makes reachable. Without the slack the order would be abandoned the
        /// instant Hornet drifted a hair further away, which reads as the Shade ignoring it.
        /// </para>
        /// </summary>
        private const float AiCommandLeashSlack = 6f;

        private static Sprite s_aiReticleSprite;

        internal bool HasShadeAiCommand => aiCommandState == ShadeAiCommandState.Holding;

        /// <summary>The point the Shade has been told to hold, if any.</summary>
        internal bool TryGetShadeAiCommand(out Vector2 point)
        {
            point = aiCommandPoint;
            return aiCommandState == ShadeAiCommandState.Holding;
        }

        /// <summary>
        /// Drives the reticle and the order behind it. Runs before <c>UpdateShadeAi</c> so the plan
        /// this frame already reflects an order given this frame.
        /// </summary>
        private void UpdateShadeAiCommand()
        {
            ModConfig config;
            try
            {
                config = ModConfig.Instance;
            }
            catch
            {
                return;
            }

            // Only meaningful while an AI is driving, and only when the player has left it enabled.
            // Anything else - a cutscene, the pause menu, a bug report being typed - is handled by
            // the same gate the driver uses, so the reticle cannot be opened behind a menu.
            if (!ShadeAiEnabled || !config.shadeAiCommandEnabled || hornetControlsLocked || isInactive || isDying || ShadeAiUiIsOpen())
            {
                ClearShadeAiCommand(null);
                return;
            }

            // Resolve it here as well as in the driver: the leash reads it from FixedUpdate, and an
            // order can be standing on a frame the driver itself never gets to run.
            UpdateAiHornetAirborne();

            bool pressed = ShadeInput.WasActionPressed(ShadeAction.CommandShade);
            Vector2 shadePosition = rb ? rb.position : (Vector2)transform.position;
            Vector2 hornetPosition = hornetTransform != null ? (Vector2)hornetTransform.position : shadePosition;

            switch (aiCommandState)
            {
                case ShadeAiCommandState.Inactive:
                    if (pressed)
                    {
                        BeginShadeAiAiming(shadePosition);
                    }

                    break;

                case ShadeAiCommandState.Aiming:
                    UpdateShadeAiReticle(hornetPosition);
                    if (pressed)
                    {
                        IssueShadeAiCommand();
                    }

                    break;

                case ShadeAiCommandState.Holding:
                    if (pressed)
                    {
                        // "Until I give the same command again" - the press lifts the order rather
                        // than reopening the reticle, so one button never means two things at once.
                        ClearShadeAiCommand("cancelled");
                        break;
                    }

                    // "Or move far enough away that the leash pulls you." The order cannot outrank
                    // the leash: past it Hornet is already dragging the Shade home, and a hold it
                    // physically cannot keep would just read as the order being ignored.
                    //
                    // The same widening the leash gets while she is airborne applies here, so the two
                    // thresholds move together. Without that the order would expire on distance while
                    // the Shade was still comfortably within the room it had been given, which is the
                    // recall-halfway-through-a-jump this was meant to stop.
                    float breakDistance = Mathf.Max(softLeashRadius * GetLeashRoomMultiplier(), aiCommandLeashFloor);
                    if (inHardLeash || Vector2.Distance(aiCommandPoint, hornetPosition) > breakDistance)
                    {
                        ClearShadeAiCommand(inHardLeash ? "leash pulled" : "out of range");
                    }

                    break;
            }

            SyncShadeAiReticle();
        }

        /// <summary>
        /// Opens the reticle on the Shade itself, which is what makes "tap twice to stay put" fall
        /// out of the same code path as aiming: confirming without moving confirms where it stands.
        /// </summary>
        private void BeginShadeAiAiming(Vector2 shadePosition)
        {
            aiCommandState = ShadeAiCommandState.Aiming;
            aiReticlePoint = shadePosition;
            aiReticleMoved = false;
            aiReticleMouseAnchor = Input.mousePosition;
        }

        /// <summary>
        /// How far the mouse moved this frame, in mouse units.
        /// <para>
        /// Relative, not absolute, and that is not a style choice. The game hides and locks the
        /// cursor during play (<c>InputHandler</c> sets <c>CursorLockMode.Locked</c>), which pins
        /// <c>Input.mousePosition</c> to the centre of the screen forever - the first version of this
        /// read that position and so the reticle could not be moved by the mouse at all. The locked
        /// axes still report movement, so those are what this reads, with the unlocked case kept as a
        /// fallback for anywhere the game hands the cursor back.
        /// </para>
        /// </summary>
        private Vector2 ReadReticleMouseDelta()
        {
            try
            {
                if (Cursor.lockState == CursorLockMode.Locked || !Cursor.visible)
                {
                    return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                }
            }
            catch
            {
                // A project without the legacy "Mouse X"/"Mouse Y" axes throws rather than returning
                // zero. Fall through to the position delta below.
            }

            try
            {
                Vector3 now = Input.mousePosition;
                Vector2 delta = now - aiReticleMouseAnchor;
                aiReticleMouseAnchor = now;

                // Pixels are a much larger unit than the axes report, so bring them into the same
                // scale rather than giving the unlocked path a wildly different sensitivity.
                return delta * 0.1f;
            }
            catch
            {
                return Vector2.zero;
            }
        }

        private void UpdateShadeAiReticle(Vector2 hornetPosition)
        {
            float delta = Time.unscaledDeltaTime;

            Vector2 mouse = ReadReticleMouseDelta();
            if (mouse.sqrMagnitude > AiCommandMouseDeadzone * AiCommandMouseDeadzone)
            {
                aiReticlePoint += mouse * AiCommandMouseSpeed;
                aiReticleMoved = true;
            }

            // The right stick, not the left. Left is Hornet's movement, and aiming the Shade should
            // not cost the player the ability to walk while they do it - which is also why the button
            // that opens this is left-stick click rather than right, since right-stick click is
            // already taunt.
            Vector2 stick = ShadeInput.GetActionStick(ShadeAction.CommandShade, rightStick: true);
            if (stick.sqrMagnitude > 0f)
            {
                aiReticlePoint += stick * (AiCommandReticleSpeed * delta);
                aiReticleMoved = true;
            }

            // Pointing somewhere the leash forbids would place an order the Shade could only break,
            // so the reticle simply cannot go there.
            float usable = Mathf.Max(1f, softLeashRadius * ShadeAiTuning.Default.LeashUsableFraction);
            aiReticlePoint = ShadeAiBrain.ClampToLeash(aiReticlePoint, hornetPosition, usable);
        }

        private void IssueShadeAiCommand()
        {
            aiCommandPoint = aiReticlePoint;
            aiCommandState = ShadeAiCommandState.Holding;

            // Fix the leash to what this order actually needs, measured now. Recomputing it as Hornet
            // moves would let it creep outward forever.
            Vector2 hornetNow = hornetTransform != null ? (Vector2)hornetTransform.position : aiCommandPoint;
            aiCommandLeashFloor = Vector2.Distance(aiCommandPoint, hornetNow) + AiCommandLeashSlack;
            aiStuckStreak = 0;

            try
            {
                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                    "shade-ai",
                    aiReticleMoved ? "ordered to a position" : "ordered to hold",
                    FormattableString.Invariant($"point=({aiCommandPoint.x:0.##}, {aiCommandPoint.y:0.##}) aimed={aiReticleMoved}"));
            }
            catch
            {
            }

            try
            {
                if (ModConfig.Instance.logShade)
                {
                    LegacyHelper.LogInfo(FormattableString.Invariant(
                        $"Shade AI ordered to ({aiCommandPoint.x:0.##}, {aiCommandPoint.y:0.##})."));
                }
            }
            catch
            {
            }
        }

        /// <summary>Drops any order and hides the reticle. Safe to call every frame.</summary>
        private void ClearShadeAiCommand(string reason)
        {
            if (aiCommandState == ShadeAiCommandState.Inactive)
            {
                HideShadeAiReticle();
                return;
            }

            bool wasHolding = aiCommandState == ShadeAiCommandState.Holding;
            aiCommandState = ShadeAiCommandState.Inactive;
            aiCommandLeashFloor = 0f;
            aiReticleMoved = false;
            HideShadeAiReticle();

            if (!wasHolding || string.IsNullOrEmpty(reason))
            {
                return;
            }

            try
            {
                LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent("shade-ai", "order lifted", reason);
            }
            catch
            {
            }
        }

        // --- Reticle -------------------------------------------------------------------

        private void SyncShadeAiReticle()
        {
            if (aiCommandState == ShadeAiCommandState.Inactive)
            {
                HideShadeAiReticle();
                return;
            }

            EnsureShadeAiReticle();
            if (aiReticleObject == null)
            {
                return;
            }

            bool aiming = aiCommandState == ShadeAiCommandState.Aiming;
            Vector2 position = aiming ? aiReticlePoint : aiCommandPoint;
            aiReticleObject.transform.position = new Vector3(position.x, position.y, transform.position.z);

            if (!aiReticleObject.activeSelf)
            {
                aiReticleObject.SetActive(true);
            }

            if (aiReticleRenderer != null)
            {
                // A slow pulse while aiming so it reads as live, steady and dimmer once placed so it
                // reads as a marker rather than something still waiting on input.
                float alpha = aiming ? 0.75f + (0.25f * Mathf.Sin(Time.unscaledTime * 8f)) : AiCommandPlacedAlpha;
                aiReticleRenderer.color = new Color(1f, 0.96f, 0.82f, alpha);

                float scale = aiming ? 1f + (0.06f * Mathf.Sin(Time.unscaledTime * 8f)) : 0.85f;
                aiReticleObject.transform.localScale = Vector3.one * scale;
            }
        }

        private void EnsureShadeAiReticle()
        {
            if (aiReticleObject != null)
            {
                return;
            }

            try
            {
                aiReticleObject = new GameObject("ShadeAiReticle");
                aiReticleRenderer = aiReticleObject.AddComponent<SpriteRenderer>();
                aiReticleRenderer.sprite = EnsureShadeAiReticleSprite();

                // Sit just in front of the Shade on whatever layer it resolved to, so the reticle is
                // occluded by the same weather and darkness the Shade is rather than floating over it.
                if (sr != null)
                {
                    aiReticleRenderer.sortingLayerID = sr.sortingLayerID;
                    aiReticleRenderer.sortingOrder = sr.sortingOrder + 2;
                }
            }
            catch
            {
                aiReticleObject = null;
                aiReticleRenderer = null;
            }
        }

        private void HideShadeAiReticle()
        {
            if (aiReticleObject != null && aiReticleObject.activeSelf)
            {
                aiReticleObject.SetActive(false);
            }
        }

        private void DestroyShadeAiReticle()
        {
            if (aiReticleObject == null)
            {
                return;
            }

            try
            {
                Destroy(aiReticleObject);
            }
            catch
            {
            }

            aiReticleObject = null;
            aiReticleRenderer = null;
        }

        /// <summary>
        /// A ring with four ticks, drawn once and shared. Generated rather than shipped as an asset
        /// so it cannot go missing from a partial install, the same reasoning as the light quad and
        /// the projectile dot.
        /// </summary>
        private static Sprite EnsureShadeAiReticleSprite()
        {
            if (s_aiReticleSprite != null)
            {
                return s_aiReticleSprite;
            }

            const int size = 64;
            const float outer = 30f;
            const float inner = 24f;

            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            Vector2 centre = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2 offset = new Vector2(x, y) - centre;
                    float distance = offset.magnitude;
                    float alpha = 0f;

                    if (distance <= outer && distance >= inner)
                    {
                        // Soften both edges so the ring does not alias into a jagged circle.
                        alpha = Mathf.Min(outer - distance, distance - inner);
                        alpha = Mathf.Clamp01(alpha);
                    }

                    // Four ticks reaching in toward the centre, so the exact spot is readable.
                    bool onAxis = Mathf.Abs(offset.x) <= 1f || Mathf.Abs(offset.y) <= 1f;
                    if (onAxis && distance <= inner && distance >= inner - 8f)
                    {
                        alpha = Mathf.Max(alpha, 1f);
                    }

                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            s_aiReticleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 32f);
            return s_aiReticleSprite;
        }
    }
}
#nullable restore
