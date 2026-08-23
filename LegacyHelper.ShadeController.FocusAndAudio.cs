#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using GlobalEnums;

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

                // Show/update aura
                EnsureFocusAura();
                try
                {
                    if (focusAuraRenderer)
                    {
                        focusAuraRenderer.enabled = true;
                        var t = focusAuraRenderer.transform;
                        float pulse = 1f + 0.25f * Mathf.Sin(Time.time * 15f);
                        float size = focusAuraBaseSize * pulse;
                        t.localScale = new Vector3(size, size, 1f);
                    }
                }
                catch { }

                // Drain soul over time while channeling
                float drainRate = Mathf.Max(0.01f, (float)focusSoulCost / Mathf.Max(0.05f, focusChannelTime)); // soul per second
                focusSoulAccumulator += drainRate * Time.deltaTime;
                int drainThisFrame = Mathf.FloorToInt(focusSoulAccumulator);
                if (drainThisFrame > 0)
                {
                    focusSoulAccumulator -= drainThisFrame;
                    int beforeSoul = shadeSoul;
                    shadeSoul = Mathf.Max(0, shadeSoul - drainThisFrame);
                    if (shadeSoul != beforeSoul)
                    {
                        PushSoulToHud();
                    }
                    if (shadeSoul <= 0)
                    {
                        // Ran out of soul mid-channel; cancel with no benefit
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

                    // Heal Hornet if close
                    try
                    {
                        var hc = HeroController.instance;
                        if (hc != null && hc.transform != null)
                        {
                            float dist = Vector2.Distance(hc.transform.position, transform.position);
                            if (dist <= focusHealRange)
                            {
                                // Avoid exceeding max via AddHealth handling
                                int hornetHeal = GetHornetFocusHealAmount();
                                if (hornetHeal > 0)
                                    hc.AddHealth(hornetHeal);
                            }
                        }
                    }
                    catch { }

                    // Play complete SFX
                    TryPlayFocusCompleteSfx();
                }

                // End channel regardless of success
                isFocusing = false;
                isCastingSpell = false;
                focusDamageShieldAbsorbedThisChannel = false;
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
                try { if (focusAuraRenderer) focusAuraRenderer.enabled = false; } catch { }
                StopFocusChargeSfx();
                focusSoulAccumulator = 0f;
                PersistIfChanged();
                RefreshBaldurShellFocusState();
                return;
            }

            // Start focus when holding key with enough soul and missing HP
            if (!ShadeInput.IsActionHeld(ShadeAction.Focus)) return;
            if (isCastingSpell || isChannelingTeleport || inHardLeash || isInactive) return;
            if (shadeHP >= shadeMaxHP) return; // already full
            if (shadeSoul < focusSoulCost) return; // not enough soul
            if (focusHealingDisabled) return;

            isFocusing = true;
            isCastingSpell = true;
            focusDamageShieldAbsorbedThisChannel = false;
            focusTimer = Mathf.Max(0.05f, focusChannelTime);
            try { if (sr) { var c = sr.color; c.a = focusAlphaWhileChannel; sr.color = c; } } catch { }
            focusSoulAccumulator = 0f;
            EnsureFocusAura();
            try { if (focusAuraRenderer) focusAuraRenderer.enabled = true; } catch { }
            StartFocusChargeSfx();
            RefreshBaldurShellFocusState();
        }

        private void CancelFocus()
        {
            if (!isFocusing) return;
            isFocusing = false;
            isCastingSpell = false;
            try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            try { if (focusAuraRenderer) focusAuraRenderer.enabled = false; } catch { }
            StopFocusChargeSfx();
            focusSoulAccumulator = 0f;
            focusDamageShieldAbsorbedThisChannel = false;
            RefreshBaldurShellFocusState();
        }

        private void EnsureFocusAura()
        {
            try
            {
                if (focusAuraRenderer && focusAuraRenderer.gameObject)
                    return;
                // Create aura
                var go = new GameObject("ShadeFocusAura");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                EnsureSimpleLightResources();
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = s_simpleQuadMesh;
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = s_simpleAdditiveMat;
                try { mr.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.8f)); } catch { }
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                var shadeSR = GetComponent<SpriteRenderer>();
                mr.sortingLayerID = shadeSR ? shadeSR.sortingLayerID : 0;
                mr.sortingOrder = shadeSR ? (shadeSR.sortingOrder - 2) : -2;
                go.transform.localScale = new Vector3(focusAuraBaseSize, focusAuraBaseSize, 1f);
                focusAuraRenderer = mr;
                mr.enabled = false;
            }
            catch { }
        }

        private void EnsureBaldurShellRenderer()
        {
            try
            {
                if (baldurShellRenderer && baldurShellRenderer.gameObject)
                {
                    if (sr)
                    {
                        baldurShellRenderer.sortingLayerID = sr.sortingLayerID;
                        baldurShellRenderer.sortingOrder = sr.sortingOrder + 1;
                    }
                    return;
                }

                var go = new GameObject("ShadeBaldurShell");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.enabled = false;
                renderer.sprite = null;
                if (sr)
                {
                    renderer.sortingLayerID = sr.sortingLayerID;
                    renderer.sortingOrder = sr.sortingOrder + 1;
                }
                renderer.color = Color.white;
                baldurShellRenderer = renderer;
            }
            catch { }
        }

        private void RefreshBaldurShellFocusState(bool immediate = false)
        {
            bool hasFrames = baldurShellFocusAnimFrames != null && baldurShellFocusAnimFrames.Length > 0;
            bool shouldShow = focusDamageShieldEnabled && isFocusing && hasFrames;

            if (shouldShow)
            {
                EnsureBaldurShellRenderer();
                if (!baldurShellRenderer)
                {
                    return;
                }

                try
                {
                    if (sr)
                    {
                        baldurShellRenderer.sortingLayerID = sr.sortingLayerID;
                        baldurShellRenderer.sortingOrder = sr.sortingOrder + 1;
                    }
                }
                catch { }

                if (baldurShellActive)
                {
                    try
                    {
                        baldurShellRenderer.enabled = true;
                        int index = Mathf.Clamp(baldurShellFrameIndex, 0, baldurShellFocusAnimFrames.Length - 1);
                        baldurShellRenderer.sprite = baldurShellFocusAnimFrames[index];
                    }
                    catch { }
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
                try { StopCoroutine(baldurShellRoutine); } catch { }
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
                    try
                    {
                        baldurShellRenderer.enabled = true;
                        baldurShellRenderer.sprite = frames[i];
                    }
                    catch { }
                    if (i < length - 1)
                    {
                        yield return new WaitForSeconds(BaldurShellFrameTime);
                    }
                }
                baldurShellActive = true;
            }
            else
            {
                int start = Mathf.Clamp(baldurShellFrameIndex, 0, length - 1);
                for (int i = start; i >= 0; i--)
                {
                    baldurShellFrameIndex = i;
                    try
                    {
                        baldurShellRenderer.enabled = true;
                        baldurShellRenderer.sprite = frames[i];
                    }
                    catch { }
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
                try
                {
                    baldurShellRenderer.enabled = false;
                    baldurShellRenderer.sprite = null;
                }
                catch { }
            }
        }

        private void EnsureFocusSfx()
        {
            try
            {
                if (focusSfx == null)
                {
                    var go = new GameObject("ShadeFocusSFX");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    focusSfx = go.AddComponent<AudioSource>();
                    focusSfx.playOnAwake = false;
                    focusSfx.spatialBlend = 0f; // 2D; set to small 3D if desired
                    focusSfx.volume = Mathf.Clamp01(GetEffectiveSfxVolume());
                }

                // Clip resolution runs at most once per session: it probes the filesystem
                // and then walks every loaded object, neither of which should repeat per cast.
                if (searchedFocusSfx) return;
                searchedFocusSfx = true;

                // Prefer HK1 SFX dropped into the mod Assets folder (wav)
                // Primary (per your filenames), with fallback aliases
                if (sfxFocusCharge == null)
                    sfxFocusCharge = TryLoadAudioFromAssets("focus_health_charging.wav") ?? TryLoadAudioFromAssets("focus_charge.wav");
                if (sfxFocusComplete == null)
                    sfxFocusComplete = TryLoadAudioFromAssets("focus_health_heal.wav") ?? TryLoadAudioFromAssets("focus_complete.wav");
                if (sfxFocusReady == null)
                    sfxFocusReady = TryLoadAudioFromAssets("focus_ready.wav");

                if (sfxFocusCharge == null || sfxFocusComplete == null || sfxFocusReady == null)
                {
                    var all = Resources.FindObjectsOfTypeAll<AudioClip>();
                    AudioClip bestCharge = null; int bestChargeScore = int.MinValue;
                    AudioClip bestComplete = null; int bestCompleteScore = int.MinValue;
                    AudioClip bestReady = null; int bestReadyScore = int.MinValue;
                    foreach (var c in all)
                    {
                        if (!c) continue; string n = c.name ?? string.Empty; string lname = n.ToLowerInvariant();
                        int chargeScore = 0;
                        if (lname.Contains("focus")) chargeScore += 5;
                        if (lname.Contains("charge") || lname.Contains("loop") || lname.Contains("start")) chargeScore += 3;
                        if (lname.Contains("spell")) chargeScore += 1;
                        if (lname.Contains("bind")) chargeScore += 1; // fallback to Silksong bind if no focus
                        if (chargeScore > bestChargeScore) { bestChargeScore = chargeScore; bestCharge = c; }

                        int completeScore = 0;
                        if (lname.Contains("focus")) completeScore += 4;
                        if (lname.Contains("heal") || lname.Contains("end") || lname.Contains("complete") || lname.Contains("release")) completeScore += 4;
                        if (lname.Contains("spell")) completeScore += 1;
                        if (lname.Contains("bind")) completeScore += 1; // fallback
                        if (completeScore > bestCompleteScore) { bestCompleteScore = completeScore; bestComplete = c; }

                        int readyScore = 0;
                        if (lname.Contains("focus")) readyScore += 3;
                        if (lname.Contains("ready") || lname.Contains("available") || lname.Contains("charge_complete") || lname.Contains("full")) readyScore += 3;
                        if (lname.Contains("bind")) readyScore += 1;
                        if (readyScore > bestReadyScore) { bestReadyScore = readyScore; bestReady = c; }
                    }
                    if (bestChargeScore > 0 && sfxFocusCharge == null) sfxFocusCharge = bestCharge;
                    if (bestCompleteScore > 0 && sfxFocusComplete == null) sfxFocusComplete = bestComplete;
                    if (bestReadyScore > 0 && sfxFocusReady == null) sfxFocusReady = bestReady;
                }
            }
            catch { }
        }

        private static AudioClip TryLoadAudioFromAssets(string fileName)
        {
            try
            {
                if (!ModPaths.TryGetAssetPath(out var path, fileName))
                {
                    return null;
                }

                if (!path.EndsWith(".wav", System.StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return LoadPcmWav(path);
            }
            catch { }
            return null;
        }

        private static AudioClip LoadPcmWav(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                using (var br = new BinaryReader(fs))
                {
                    // RIFF header
                    if (new string(br.ReadChars(4)) != "RIFF") return null;
                    br.ReadInt32(); // Chunk size
                    if (new string(br.ReadChars(4)) != "WAVE") return null;

                    int channels = 1;
                    int sampleRate = 44100;
                    int bitsPerSample = 16;
                    int dataSize = 0;
                    long dataPos = 0;

                    // Read chunks
                    while (br.BaseStream.Position + 8 <= br.BaseStream.Length)
                    {
                        string chunkId = new string(br.ReadChars(4));
                        int chunkSize = br.ReadInt32();
                        long next = br.BaseStream.Position + chunkSize;

                        if (chunkId == "fmt ")
                        {
                            int audioFormat = br.ReadInt16();
                            channels = br.ReadInt16();
                            sampleRate = br.ReadInt32();
                            br.ReadInt32(); // byteRate
                            br.ReadInt16(); // blockAlign
                            bitsPerSample = br.ReadInt16();
                            if (chunkSize > 16)
                            {
                                // skip any extra bytes in fmt chunk
                                br.BaseStream.Position = next;
                            }
                            if (audioFormat != 1) return null; // PCM only
                        }
                        else if (chunkId == "data")
                        {
                            dataPos = br.BaseStream.Position;
                            dataSize = chunkSize;
                            br.BaseStream.Position = next;
                        }
                        else
                        {
                            // Skip unknown chunks
                            br.BaseStream.Position = next;
                        }
                    }

                    if (dataPos == 0 || dataSize <= 0) return null;

                    // Read samples
                    fs.Position = dataPos;
                    int bytesPerSample = bitsPerSample / 8;
                    int totalSamples = dataSize / bytesPerSample;
                    int sampleCountPerChannel = totalSamples / channels;
                    float[] data = new float[sampleCountPerChannel * channels];

                    if (bitsPerSample == 16)
                    {
                        for (int i = 0; i < totalSamples; i++)
                        {
                            short s = br.ReadInt16();
                            data[i] = Mathf.Clamp(s / 32768f, -1f, 1f);
                        }
                    }
                    else if (bitsPerSample == 8)
                    {
                        for (int i = 0; i < totalSamples; i++)
                        {
                            byte b = br.ReadByte();
                            data[i] = (b - 128) / 128f;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    // Avoid SetData to keep compatibility with some UnityEngine builds.
                    // Use streaming clip with a PCM reader callback.
                    string name = Path.GetFileNameWithoutExtension(path);
                    int pos = 0;
                    var clip = AudioClip.Create(name, sampleCountPerChannel, channels, sampleRate, true,
                        (float[] outData) =>
                        {
                            int len = outData.Length;
                            for (int i = 0; i < len; i++)
                            {
                                outData[i] = (pos < data.Length) ? data[pos++] : 0f;
                            }
                        },
                        (int newPosition) =>
                        {
                            // newPosition is per-channel sample index
                            pos = Mathf.Clamp(newPosition * channels, 0, data.Length);
                        }
                    );
                    return clip;
                }
            }
            catch { }
            return null;
        }

        private void StartFocusChargeSfx()
        {
            try
            {
                EnsureFocusSfx();
                if (focusSfx != null && sfxFocusCharge != null)
                {
                    focusSfx.loop = true;
                    focusSfx.clip = sfxFocusCharge;
                    focusSfx.volume = Mathf.Clamp01(GetEffectiveSfxVolume());
                    focusSfx.Play();
                }
            }
            catch { }
        }

        private void StopFocusChargeSfx()
        {
            try
            {
                if (focusSfx != null)
                {
                    focusSfx.loop = false;
                    focusSfx.Stop();
                }
            }
            catch { }
        }

        private void TryPlayFocusCompleteSfx()
        {
            try
            {
                EnsureFocusSfx();
                if (focusSfx != null && sfxFocusComplete != null)
                {
                    focusSfx.PlayOneShot(sfxFocusComplete, Mathf.Clamp01(GetEffectiveSfxVolume()));
                }
            }
            catch { }
        }

        private void CheckFocusReadySfx()
        {
            try
            {
                if (lastSoulForReady < 0) { lastSoulForReady = shadeSoul; return; }
                if (lastSoulForReady < focusSoulCost && shadeSoul >= focusSoulCost)
                {
                    EnsureFocusSfx();
                    if (focusSfx != null && sfxFocusReady != null)
                        focusSfx.PlayOneShot(sfxFocusReady, Mathf.Clamp01(GetEffectiveSfxVolume()));
                }
                lastSoulForReady = shadeSoul;
            }
            catch { }
        }

        private void UpdateSfxVolumes()
        {
            float volume = Mathf.Clamp01(GetEffectiveSfxVolume());
            try { if (focusSfx != null) focusSfx.volume = volume; } catch { }
            try { if (spellSfx != null) spellSfx.volume = volume; } catch { }
        }

        // ========== Spell SFX (Projectile, Shriek, Quake) ==========
        private AudioSource spellSfx;
        private AudioClip sfxFireball;
        private AudioClip sfxQuakePrepare;
        private AudioClip sfxQuakeImpact;
        private AudioClip sfxVoidQuakeImpact;
        private AudioClip sfxScream;
        private AudioClip sfxVoidScream;

        private void EnsureSpellSfx()
        {
            try
            {
                if (spellSfx == null)
                {
                    var go = new GameObject("ShadeSpellSFX");
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = Vector3.zero;
                    spellSfx = go.AddComponent<AudioSource>();
                    spellSfx.playOnAwake = false;
                    spellSfx.spatialBlend = 0f;
                    spellSfx.volume = Mathf.Clamp01(GetEffectiveSfxVolume());
                }
                // As with focus SFX: probing the Assets folder and walking every loaded
                // object are both one-time costs, not per-cast costs.
                if (searchedSpellSfx) return;
                searchedSpellSfx = true;

                if (sfxFireball == null) sfxFireball = TryLoadAudioFromAssets("hero_fireball.wav");
                if (sfxQuakePrepare == null) sfxQuakePrepare = TryLoadAudioFromAssets("hero_quake_spell_prepare.wav");
                if (sfxQuakeImpact == null) sfxQuakeImpact = TryLoadAudioFromAssets("hero_quake_spell_impact.wav");
                if (sfxVoidQuakeImpact == null) sfxVoidQuakeImpact = TryLoadAudioFromAssets("hero_void_quake_impact.wav");
                if (sfxScream == null) sfxScream = TryLoadAudioFromAssets("hero_scream_spell.wav");
                if (sfxVoidScream == null) sfxVoidScream = TryLoadAudioFromAssets("hero_void_scream_spell.wav");

                if (sfxFireball == null || sfxQuakePrepare == null || sfxQuakeImpact == null || sfxVoidQuakeImpact == null || sfxScream == null || sfxVoidScream == null)
                {
                    ResolveSpellSfxByName();
                }
            }
            catch { }
        }

        private static readonly string[] FireballKeys = { "fireball", "vengeful", "spirit", "spell" };
        private static readonly string[] QuakePrepareKeys = { "quake", "prepare", "start", "spell" };
        private static readonly string[] QuakeImpactKeys = { "quake", "impact", "spell" };
        private static readonly string[] VoidQuakeKeys = { "void", "quake", "impact" };
        private static readonly string[] ScreamKeys = { "scream", "wraith", "howl", "spell" };
        private static readonly string[] VoidScreamKeys = { "void", "scream", "abyss" };

        /// <summary>
        /// Picks the best-matching clip for each unresolved spell sound in a single pass.
        /// The previous version ran one full pass per sound -- six walks over every loaded
        /// AudioClip, each lowercasing every clip name again.
        /// </summary>
        private void ResolveSpellSfxByName()
        {
            var all = Resources.FindObjectsOfTypeAll<AudioClip>();
            if (all == null || all.Length == 0) return;

            AudioClip bestFireball = null, bestQuakePrepare = null, bestQuakeImpact = null;
            AudioClip bestVoidQuake = null, bestScream = null, bestVoidScream = null;
            int sFireball = int.MinValue, sQuakePrepare = int.MinValue, sQuakeImpact = int.MinValue;
            int sVoidQuake = int.MinValue, sScream = int.MinValue, sVoidScream = int.MinValue;

            static int Score(string name, string[] keys)
            {
                int sc = 0;
                foreach (var k in keys)
                {
                    if (name.Contains(k, StringComparison.Ordinal)) sc += 2; // favour multiple matches
                }
                return sc;
            }

            foreach (var c in all)
            {
                if (!c) continue;
                // Lowercased once per clip rather than once per clip per target.
                string n = (c.name ?? string.Empty).ToLowerInvariant();

                int score = Score(n, FireballKeys);
                if (score > sFireball) { sFireball = score; bestFireball = c; }

                score = Score(n, QuakePrepareKeys);
                if (score > sQuakePrepare) { sQuakePrepare = score; bestQuakePrepare = c; }

                score = Score(n, QuakeImpactKeys);
                if (score > sQuakeImpact) { sQuakeImpact = score; bestQuakeImpact = c; }

                score = Score(n, VoidQuakeKeys);
                if (score > sVoidQuake) { sVoidQuake = score; bestVoidQuake = c; }

                score = Score(n, ScreamKeys);
                if (score > sScream) { sScream = score; bestScream = c; }

                score = Score(n, VoidScreamKeys);
                if (score > sVoidScream) { sVoidScream = score; bestVoidScream = c; }
            }

            sfxFireball ??= bestFireball;
            sfxQuakePrepare ??= bestQuakePrepare;
            sfxQuakeImpact ??= bestQuakeImpact;
            sfxVoidQuakeImpact ??= bestVoidQuake;
            sfxScream ??= bestScream;
            sfxVoidScream ??= bestVoidScream;
        }

        private void TryPlayFireballSfx()
        {
            try
            {
                EnsureSpellSfx();
                if (spellSfx != null && sfxFireball != null) spellSfx.PlayOneShot(sfxFireball, Mathf.Clamp01(GetEffectiveSfxVolume()));
            }
            catch { }
        }

        private void TryPlayShriekSfx(bool upgraded)
        {
            try
            {
                EnsureSpellSfx();
                var clip = upgraded ? sfxVoidScream : sfxScream;
                if (spellSfx != null && clip != null) spellSfx.PlayOneShot(clip, Mathf.Clamp01(GetEffectiveSfxVolume()));
            }
            catch { }
        }

        private void TryPlayQuakePrepareSfx()
        {
            try
            {
                EnsureSpellSfx();
                if (spellSfx != null && sfxQuakePrepare != null) spellSfx.PlayOneShot(sfxQuakePrepare, Mathf.Clamp01(GetEffectiveSfxVolume()));
            }
            catch { }
        }

        private void TryPlayQuakeImpactSfx(bool upgraded)
        {
            try
            {
                EnsureSpellSfx();
                var clip = upgraded ? sfxVoidQuakeImpact : sfxQuakeImpact;
                if (spellSfx != null && clip != null) spellSfx.PlayOneShot(clip, Mathf.Clamp01(GetEffectiveSfxVolume()));
            }
            catch { }
        }

        private void SetupShadeLight()
        {
            try
            {
                var lightGO = new GameObject("ShadeLightSimple");
                lightGO.transform.SetParent(transform, false);
                lightGO.transform.localPosition = Vector3.zero;
                lightGO.transform.localRotation = Quaternion.identity;
                EnsureSimpleLightResources();

                var mf = lightGO.AddComponent<MeshFilter>();
                mf.sharedMesh = s_simpleQuadMesh;
                var mr = lightGO.AddComponent<MeshRenderer>();
                mr.sharedMaterial = s_simpleAdditiveMat;
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                var shadeSR = GetComponent<SpriteRenderer>();
                mr.sortingLayerID = shadeSR ? shadeSR.sortingLayerID : 0;
                mr.sortingOrder = shadeSR ? (shadeSR.sortingOrder - 1) : -1;
                lightGO.transform.localScale = new Vector3(simpleLightSize, simpleLightSize, 1f);
                shadeLightRenderers = new Renderer[] { mr };
            }
            catch { }
        }

        private void SyncShadeLight()
        {
            try
            {
                if (shadeLightRenderers == null) return;
                var shadeSR = GetComponent<SpriteRenderer>();
                int baseLayer = shadeSR ? shadeSR.sortingLayerID : 0;
                int baseOrder = shadeSR ? shadeSR.sortingOrder : 0;
                foreach (var r in shadeLightRenderers)
                {
                    if (!r) continue;
                    // Re-asserted every frame, so the scripted-hold check has to live here rather
                    // than in ApplyScriptedHoldVisibility - a one-shot disable would be undone on
                    // the very next tick and the Shade's glow would sit there through the cutscene.
                    r.enabled = !hiddenForScriptedHold;
                    r.sortingLayerID = baseLayer;
                    r.sortingOrder = baseOrder - 1;
                }
                // Keep focus aura sorted just below the shade sprite as well
                if (focusAuraRenderer)
                {
                    focusAuraRenderer.sortingLayerID = baseLayer;
                    focusAuraRenderer.sortingOrder = baseOrder - 2;
                }
            }
            catch { }
        }

        private static void EnsureSimpleLightResources()
        {
            try
            {
                if (s_simpleQuadMesh == null)
                {
                    s_simpleQuadMesh = new Mesh();
                    s_simpleQuadMesh.name = "ShadeLightQuad";
                    s_simpleQuadMesh.vertices = new Vector3[]
                    {
                        new Vector3(-0.5f, -0.5f, 0f),
                        new Vector3( 0.5f, -0.5f, 0f),
                        new Vector3(-0.5f,  0.5f, 0f),
                        new Vector3( 0.5f,  0.5f, 0f)
                    };
                    s_simpleQuadMesh.uv = new Vector2[] {
                        new Vector2(0,0), new Vector2(1,0), new Vector2(0,1), new Vector2(1,1)
                    };
                    s_simpleQuadMesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
                    s_simpleQuadMesh.RecalculateNormals();
                }
                if (s_simpleLightTex == null)
                {
                    int size = 128;
                    s_simpleLightTex = new Texture2D(size, size, TextureFormat.ARGB32, false);
                    s_simpleLightTex.filterMode = FilterMode.Bilinear;
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            float nx = (x + 0.5f) / size * 2f - 1f;
                            float ny = (y + 0.5f) / size * 2f - 1f;
                            float r = Mathf.Sqrt(nx * nx + ny * ny);
                            float a = Mathf.Clamp01(1f - r);
                            a = Mathf.Pow(a, 3.5f) * 0.55f;
                            s_simpleLightTex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
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
                    try { s_simpleAdditiveMat.SetColor("_Color", new Color(1f, 1f, 1f, 0.35f)); } catch { }
                }
            }
            catch { }
        }

        private static bool IsTerrainHazard(GlobalEnums.HazardType hz)
        {
            switch (hz)
            {
                case GlobalEnums.HazardType.SPIKES:
                case GlobalEnums.HazardType.ACID:
                case GlobalEnums.HazardType.LAVA:
                case GlobalEnums.HazardType.PIT:
                case GlobalEnums.HazardType.COAL:
                case GlobalEnums.HazardType.ZAP:
                case GlobalEnums.HazardType.SINK:
                case GlobalEnums.HazardType.STEAM:
                case GlobalEnums.HazardType.COAL_SPIKES:
                case GlobalEnums.HazardType.RESPAWN_PIT:
                    return true;
                default:
                    return false;
            }
        }

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

            // Visual hint: fade sprite slightly while channeling
            try
            {
                if (sr)
                {
                    var c = sr.color; c.a = 0.6f; sr.color = c;
                }
            }
            catch { }

            teleportChannelTimer -= Time.deltaTime;
            if (teleportChannelTimer <= 0f)
            {
                TeleportToHornet();
                teleportCooldownTimer = teleportCooldown;
                isChannelingTeleport = false;
                // restore sprite alpha
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            }

            // Cancel on movement or attack input
            if (Input.GetKeyDown(KeyCode.Escape) ||
                ShadeInput.WasActionPressed(ShadeAction.Nail) ||
                ShadeInput.WasActionPressed(ShadeAction.NailUp) ||
                ShadeInput.WasActionPressed(ShadeAction.NailDown) ||
                ShadeInput.WasActionPressed(ShadeAction.Fire))
            {
                isChannelingTeleport = false;
                try { if (sr) { var c = sr.color; c.a = 0.9f; sr.color = c; } } catch { }
            }
        }

        internal sealed class AggroProxyTracker : MonoBehaviour, ITrackTriggerObject
        {
            private ShadeController owner;
            private Collider2D proxyCollider;
            private readonly HashSet<Remasker> remaskersInside = new HashSet<Remasker>();
            private static readonly List<Remasker> RemaskerBuffer = new List<Remasker>();

            /// <summary>How long the same collider is ignored for after being recorded once.</summary>
            private const float ProxyEntryThrottleSeconds = 1f;

            /// <summary>Ancestors named in a recorded path. Enough to identify the boss, not the whole scene.</summary>
            private const int ProxyEntryPathDepth = 2;

            private readonly Dictionary<Collider2D, float> _lastProxyEntryTimes = new Dictionary<Collider2D, float>();

            internal void Attach(ShadeController shade, Collider2D collider)
            {
                owner = shade;
                proxyCollider = collider;
                remaskersInside.Clear();
            }

            internal bool IsEligibleForAggro => owner != null && owner.IsAggroEligible;

            internal bool TryGetOwner(out ShadeController shade)
            {
                shade = owner;
                return shade != null;
            }

            internal bool TryGetTargetPoint(out Vector2 target)
            {
                target = transform.position;
                if (!IsEligibleForAggro)
                {
                    return false;
                }

                if (!proxyCollider || !proxyCollider.enabled || !proxyCollider.gameObject.activeInHierarchy)
                {
                    return false;
                }

                try
                {
                    target = proxyCollider.bounds.center;
                }
                catch
                {
                    target = transform.position;
                }

                return true;
            }

            public void OnTrackTriggerEntered(TrackTriggerObjects enteredRange)
            {
                ShadeAggroTracker.NotifyEntered(this, enteredRange);
            }

            public void OnTrackTriggerExited(TrackTriggerObjects exitedRange)
            {
                ShadeAggroTracker.NotifyExited(this, exitedRange);
            }

            private void OnDisable()
            {
                ForceExitTrackedRemaskers();
                ShadeAggroTracker.NotifyDisabled(this);
            }

            private void OnDestroy()
            {
                ForceExitTrackedRemaskers();
                ShadeAggroTracker.NotifyDisabled(this);
            }

            private void OnTriggerEnter2D(Collider2D other)
            {
                RecordProxyEntry(other);
                TrackRemasker(other, entering: true);
            }

            /// <summary>
            /// Notes every trigger the proxy walks into, for the bug report event ring.
            /// <para>
            /// The proxy exists to look exactly like Hornet to enemy detection, which means it also
            /// looks like Hornet to anything else that tests for her - including boss attacks that,
            /// once triggered, go on to act on <c>HeroController.instance</c> rather than on whatever
            /// actually tripped them. When that happens the visible symptom lands on Hornet and no
            /// artefact in a report names the object responsible. This is the line that names it.
            /// </para>
            /// <para>
            /// Enters only, and throttled per object: exits are not what starts an attack, and a
            /// region the Shade is hovering in and out of would otherwise flush the ring.
            /// </para>
            /// </summary>
            private void RecordProxyEntry(Collider2D other)
            {
                if (!other)
                {
                    return;
                }

                try
                {
                    float now = Time.realtimeSinceStartup;
                    if (_lastProxyEntryTimes.TryGetValue(other, out float previous) &&
                        now - previous < ProxyEntryThrottleSeconds)
                    {
                        return;
                    }

                    _lastProxyEntryTimes[other] = now;

                    // Self and ancestor are reported separately, and the distinction matters: an
                    // ancestor DamageHero is just "this belongs to something that can hurt you",
                    // which is true of every collider on an enemy including its harmless detection
                    // ranges. Only a DamageHero on the collider's own object means "this collider is
                    // the thing that hurts". Reporting the two as one flag made an attack hitbox and
                    // a battle range look identical in the first report that used this.
                    bool ownFsm = other.GetComponent<PlayMakerFSM>() != null;
                    bool ownDamageHero = other.GetComponent<DamageHero>() != null;
                    bool parentFsm = !ownFsm && other.GetComponentInParent<PlayMakerFSM>() != null;
                    bool parentDamageHero = !ownDamageHero && other.GetComponentInParent<DamageHero>() != null;

                    LegacyoftheAbyss.Diagnostics.BugReportSystem.RecordEvent(
                        "shade-proxy-entered",
                        LegacyHelper.DescribeHierarchy(other.transform, ProxyEntryPathDepth),
                        FormattableString.Invariant(
                            $"layer={LayerMask.LayerToName(other.gameObject.layer)} tag={other.gameObject.tag} trigger={other.isTrigger} fsm={(ownFsm ? "self" : parentFsm ? "parent" : "none")} damageHero={(ownDamageHero ? "self" : parentDamageHero ? "parent" : "none")}"));
                }
                catch
                {
                }
            }

            private void TrackRemasker(Collider2D other, bool entering)
            {
                if (!other)
                {
                    return;
                }

                Remasker remasker = null;
                try
                {
                    remasker = other.GetComponent<Remasker>();
                    if (!remasker)
                    {
                        remasker = other.GetComponentInParent<Remasker>();
                    }
                }
                catch
                {
                    remasker = null;
                }

                if (!remasker)
                {
                    return;
                }

                if (entering)
                {
                    remaskersInside.Add(remasker);
                }
                else
                {
                    remaskersInside.Remove(remasker);
                }
            }

            internal void ForceExitTrackedRemaskers()
            {
                if (remaskersInside.Count == 0)
                {
                    return;
                }

                RemaskerBuffer.Clear();
                RemaskerBuffer.AddRange(remaskersInside);
                foreach (var remasker in RemaskerBuffer)
                {
                    if (!remasker)
                    {
                        continue;
                    }

                    try
                    {
                        remasker.Exited(true);
                    }
                    catch
                    {
                    }
                }

                RemaskerBuffer.Clear();
                remaskersInside.Clear();
            }

            internal void NotifyRemaskerIgnored(Remasker remasker)
            {
                if (!remasker)
                {
                    return;
                }

                remaskersInside.Remove(remasker);
            }
        }

        private int GetHornetNailDamage()
        {
            try
            {
                var gm = GameManager.instance;
                var pd = gm != null ? gm.playerData : null;
                if (pd == null) return 5;
                int baseDmg = Mathf.Max(1, pd.nailDamage);
                bool bound = false;
                try { bound = BossSequenceController.BoundNail; } catch { bound = false; }
                if (bound)
                {
                    int boundVal = 0;
                    try { boundVal = BossSequenceController.BoundNailDamage; } catch { boundVal = baseDmg; }
                    return Mathf.Min(baseDmg, Mathf.Max(1, boundVal));
                }
                return baseDmg;
            }
            catch { return 5; }
        }
    }
}
#nullable restore
