#nullable disable
using System.Collections;
using UnityEngine;

// The Shade's Focus channel: the heal itself, the aura and Baldur Shell visuals around it, and the
// teleport channel that shares its "busy casting" state. Sound effects live in
// LegacyHelper.ShadeController.Audio.cs.
public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private void HandleFocus()
        {
            // Already channeling
            if (isFocusing)
            {
                // Cancel if key released or interrupted by teleport
                if (!ShadeInput.IsActionHeld(ShadeAction.Focus) || isChannelingTeleport || inHardLeash || isInactive)
                {
                    CancelFocus();
                    return;
                }

                EnsureFocusAura();
                if (focusAuraRenderer)
                {
                    focusAuraRenderer.enabled = true;
                    float pulse = 1f + 0.25f * Mathf.Sin(Time.time * 15f);
                    float size = focusAuraBaseSize * pulse;
                    focusAuraRenderer.transform.localScale = new Vector3(size, size, 1f);
                }

                // Drain soul over time while channeling
                float drainRate = Mathf.Max(0.01f, (float)focusSoulCost / Mathf.Max(0.05f, focusChannelTime)); // soul per second
                focusSoulAccumulator += drainRate * Time.deltaTime;
                int drainThisFrame = Mathf.FloorToInt(focusSoulAccumulator);
                if (drainThisFrame > 0)
                {
                    focusSoulAccumulator -= drainThisFrame;
                    int beforeSoul = shadeSoul;
                    shadeSoul = Mathf.Max(0, shadeSoul - drainThisFrame);
                    focusSoulDrainedThisChannel += beforeSoul - shadeSoul;
                    if (shadeSoul != beforeSoul)
                    {
                        PushSoulToHud();
                    }

                    // Only a channel that has *not* yet paid its own cost can run out: the meter
                    // holds an exact multiple of it, so the last heal a full meter affords always
                    // ends on zero and cancelling there is what cost the third heal.
                    if (shadeSoul <= 0 && focusSoulDrainedThisChannel < focusSoulCost)
                    {
                        CancelFocus();
                        return;
                    }
                }

                focusTimer -= Time.deltaTime;
                if (focusTimer > 0f) return;

                // Complete focus
                int healAmt = GetFocusHealAmount();
                int canHeal = (shadeHP < shadeMaxHP && healAmt > 0) ? Mathf.Min(healAmt, shadeMaxHP - shadeHP) : 0;
                if (canHeal > 0)
                {
                    int before = shadeHP;
                    shadeHP = Mathf.Min(shadeHP + canHeal, shadeMaxHP);
                    if (shadeHP != before)
                    {
                        if (GetTotalCurrentHealth() > 0)
                        {
                            isInactive = false;
                            CancelDeathAnimation();
                        }
                        PushShadeStatsToHud(suppressDamageAudio: true);
                    }

                    // Healing Hornet is a side effect of the Shade healing itself nearby, not
                    // something Focus can be aimed at her on its own.
                    var hero = HeroController.instance;
                    if (hero != null && Vector2.Distance(hero.transform.position, transform.position) <= focusHealRange)
                    {
                        int hornetHeal = GetHornetFocusHealAmount();
                        if (hornetHeal > 0)
                            hero.AddHealth(hornetHeal);
                    }

                    TryPlayFocusCompleteSfx();
                }

                // Spore Shroom bursts on the channel completing, whether or not it healed.
                OnFocusCompletedCharmEffects();

                // End channel regardless of success.
                EndFocusChannel();
                PersistIfChanged();
                return;
            }

            // Start focus when holding key with enough soul and missing HP
            if (!ShadeInput.IsActionHeld(ShadeAction.Focus)) return;
            if (isCastingSpell || isChannelingTeleport || inHardLeash || isInactive) return;
            // Full masks normally refuses the channel, matching Hornet's own rule. "Full Masks
            // Focus" lifts that so the Shade can spend SOUL purely to heal her - the Hornet heal
            // is a side effect of the Shade healing itself, so without this it can never help her
            // while it is undamaged.
            if (shadeHP >= shadeMaxHP && !ModConfig.Instance.shadeFocusAtFullMasks) return;
            if (shadeSoul < focusSoulCost) return; // not enough soul
            if (focusHealingDisabled) return;

            isFocusing = true;
            isCastingSpell = true;
            focusDamageShieldAbsorbedThisChannel = false;
            focusTimer = Mathf.Max(0.05f, focusChannelTime);
            SetSpriteAlpha(focusAlphaWhileChannel);
            focusSoulAccumulator = 0f;
            focusSoulDrainedThisChannel = 0;
            EnsureFocusAura();
            if (focusAuraRenderer) focusAuraRenderer.enabled = true;
            StartFocusChargeSfx();
            RefreshBaldurShellFocusState();
        }

        /// <summary>
        /// Puts the companion back to idle after a channel, however it ended. The completion path
        /// and the cancel path had grown separate copies of the same nine statements, differing
        /// only in the order two of them ran in.
        /// </summary>
        private void EndFocusChannel()
        {
            isFocusing = false;
            isCastingSpell = false;
            focusDamageShieldAbsorbedThisChannel = false;
            SetSpriteAlpha(SpriteAlphaIdle);
            if (focusAuraRenderer) focusAuraRenderer.enabled = false;
            StopFocusChargeSfx();
            focusSoulAccumulator = 0f;
            focusSoulDrainedThisChannel = 0;
            RefreshBaldurShellFocusState();
        }

        private void CancelFocus()
        {
            if (!isFocusing) return;
            EndFocusChannel();
        }

        /// <summary>Opacity the Shade sits at whenever it is not channelling something.</summary>
        private const float SpriteAlphaIdle = 0.9f;

        private void SetSpriteAlpha(float alpha)
        {
            if (!sr) return;
            var c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        private void EnsureFocusAura()
        {
            if (focusAuraRenderer && focusAuraRenderer.gameObject)
                return;

            var go = new GameObject("ShadeFocusAura");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(focusAuraBaseSize, focusAuraBaseSize, 1f);

            EnsureSimpleLightResources();
            go.AddComponent<MeshFilter>().sharedMesh = s_simpleQuadMesh;

            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = s_simpleAdditiveMat;
            mr.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.8f));
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.sortingLayerID = sr ? sr.sortingLayerID : 0;
            mr.sortingOrder = sr ? sr.sortingOrder - 2 : -2;
            mr.enabled = false;

            focusAuraRenderer = mr;
        }

        private void EnsureBaldurShellRenderer()
        {
            if (!baldurShellRenderer || !baldurShellRenderer.gameObject)
            {
                var go = new GameObject("ShadeBaldurShell");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;

                baldurShellRenderer = go.AddComponent<SpriteRenderer>();
                baldurShellRenderer.enabled = false;
                baldurShellRenderer.sprite = null;
                baldurShellRenderer.color = Color.white;
            }

            // Re-read every time: the Shade's own sorting moves with its skin and scene layer.
            if (sr)
            {
                baldurShellRenderer.sortingLayerID = sr.sortingLayerID;
                baldurShellRenderer.sortingOrder = sr.sortingOrder + 1;
            }
        }

        private void RefreshBaldurShellFocusState(bool immediate = false)
        {
            bool hasFrames = baldurShellFocusAnimFrames != null && baldurShellFocusAnimFrames.Length > 0;

            // A spent shell draws nothing: the charm is still equipped and still worth its notches
            // once a bench mends it, but there is no shell there to curl up right now.
            bool hasShell = (OwnCharms?.BaldurShellCharges ?? 0) > 0;
            bool shouldShow = focusDamageShieldEnabled && isFocusing && hasFrames && hasShell;

            if (shouldShow)
            {
                EnsureBaldurShellRenderer();
                if (!baldurShellRenderer)
                {
                    return;
                }

                if (baldurShellActive)
                {
                    baldurShellRenderer.enabled = true;
                    int index = Mathf.Clamp(baldurShellFrameIndex, 0, baldurShellFocusAnimFrames.Length - 1);
                    baldurShellRenderer.sprite = baldurShellFocusAnimFrames[index];
                    return;
                }

                if (baldurShellRoutine == null)
                {
                    baldurShellRoutine = StartCoroutine(PlayBaldurShellAnimation(true));
                }

                return;
            }

            if (baldurShellRoutine != null)
            {
                StopCoroutine(baldurShellRoutine);
                baldurShellRoutine = null;
            }

            if (baldurShellActive && !immediate && hasFrames && baldurShellRenderer)
            {
                baldurShellRoutine = StartCoroutine(PlayBaldurShellAnimation(false));
            }
            else
            {
                HideBaldurShell();
            }
        }

        private IEnumerator PlayBaldurShellAnimation(bool forward)
        {
            if (baldurShellRenderer == null || baldurShellFocusAnimFrames == null || baldurShellFocusAnimFrames.Length == 0)
            {
                baldurShellRoutine = null;
                yield break;
            }

            var frames = baldurShellFocusAnimFrames;
            int length = frames.Length;

            if (forward)
            {
                baldurShellActive = false;
                for (int i = 0; i < length; i++)
                {
                    baldurShellFrameIndex = i;
                    baldurShellRenderer.enabled = true;
                    baldurShellRenderer.sprite = frames[i];
                    if (i < length - 1)
                    {
                        yield return new WaitForSeconds(BaldurShellFrameTime);
                    }
                }
                baldurShellActive = true;
            }
            else
            {
                for (int i = Mathf.Clamp(baldurShellFrameIndex, 0, length - 1); i >= 0; i--)
                {
                    baldurShellFrameIndex = i;
                    baldurShellRenderer.enabled = true;
                    baldurShellRenderer.sprite = frames[i];
                    if (i > 0)
                    {
                        yield return new WaitForSeconds(BaldurShellFrameTime);
                    }
                }
                HideBaldurShell();
            }

            baldurShellRoutine = null;
        }

        private void HideBaldurShell()
        {
            baldurShellActive = false;
            baldurShellFrameIndex = 0;
            if (baldurShellRenderer)
            {
                baldurShellRenderer.enabled = false;
                baldurShellRenderer.sprite = null;
            }
        }

        /// <summary>Radial-falloff quad shared by the focus aura. Built once, on first use.</summary>
        private static void EnsureSimpleLightResources()
        {
            if (s_simpleQuadMesh == null)
            {
                s_simpleQuadMesh = new Mesh
                {
                    name = "ShadeLightQuad",
                    vertices = new Vector3[]
                    {
                        new Vector3(-0.5f, -0.5f, 0f),
                        new Vector3( 0.5f, -0.5f, 0f),
                        new Vector3(-0.5f,  0.5f, 0f),
                        new Vector3( 0.5f,  0.5f, 0f)
                    },
                    uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) },
                    triangles = new int[] { 0, 2, 1, 2, 3, 1 }
                };
                s_simpleQuadMesh.RecalculateNormals();
            }

            if (s_simpleLightTex == null)
            {
                const int size = 128;
                s_simpleLightTex = new Texture2D(size, size, TextureFormat.ARGB32, false) { filterMode = FilterMode.Bilinear };
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float nx = (x + 0.5f) / size * 2f - 1f;
                        float ny = (y + 0.5f) / size * 2f - 1f;
                        float a = Mathf.Clamp01(1f - Mathf.Sqrt(nx * nx + ny * ny));
                        s_simpleLightTex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Pow(a, 3.5f) * 0.55f));
                    }
                }
                s_simpleLightTex.Apply();
            }

            if (s_simpleAdditiveMat == null)
            {
                var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent");
                s_simpleAdditiveMat = new Material(shader)
                {
                    name = "ShadeLightAdditiveMat",
                    mainTexture = s_simpleLightTex,
                    renderQueue = 3000
                };
                s_simpleAdditiveMat.SetColor("_Color", new Color(1f, 1f, 1f, 0.35f));
            }
        }

        /// <summary>Opacity while the teleport channel is winding up.</summary>
        private const float SpriteAlphaChannellingTeleport = 0.6f;

        private void HandleTeleportChannel()
        {

            teleportCooldownTimer = Mathf.Max(0f, teleportCooldownTimer - Time.deltaTime);

            // Start channel
            if (!isChannelingTeleport && teleportCooldownTimer <= 0f && ShadeInput.WasActionPressed(ShadeAction.Teleport))
            {
                isChannelingTeleport = true;
                teleportChannelTimer = teleportChannelTime;
            }

            if (!isChannelingTeleport) return;

            SetSpriteAlpha(SpriteAlphaChannellingTeleport);

            teleportChannelTimer -= Time.deltaTime;
            if (teleportChannelTimer <= 0f)
            {
                TeleportToHornet();
                teleportCooldownTimer = teleportCooldown;
                isChannelingTeleport = false;
                SetSpriteAlpha(SpriteAlphaIdle);
            }

            // Cancel on movement or attack input
            if (Input.GetKeyDown(KeyCode.Escape) ||
                ShadeInput.WasActionPressed(ShadeAction.Nail) ||
                ShadeInput.WasActionPressed(ShadeAction.NailUp) ||
                ShadeInput.WasActionPressed(ShadeAction.NailDown) ||
                ShadeInput.WasActionPressed(ShadeAction.Fire))
            {
                isChannelingTeleport = false;
                SetSpriteAlpha(SpriteAlphaIdle);
            }
        }

    }
}
#nullable restore
