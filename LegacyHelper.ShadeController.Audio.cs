#nullable disable
using System;
using System.IO;
using LegacyoftheAbyss.Shade;
using UnityEngine;

// Every sound the Shade makes. Clips are preferred from the mod's own Assets folder and otherwise
// matched by name against whatever the game has loaded, because the mod ships no audio of its own.
public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        /// <summary>A 2D source parented to the Shade. Both SFX channels are built the same way.</summary>
        private AudioSource CreateSfxSource(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = Mathf.Clamp01(GetEffectiveSfxVolume());
            return source;
        }

        private void EnsureFocusSfx()
        {
            focusSfx ??= CreateSfxSource("ShadeFocusSFX");

            // Clip resolution runs at most once per session: it probes the filesystem and then walks
            // every loaded object, neither of which should repeat per cast.
            if (searchedFocusSfx) return;
            searchedFocusSfx = true;

            // Hollow Knight's own SFX dropped into the mod's Assets folder win, under either name.
            sfxFocusCharge ??= TryLoadAudioFromAssets("focus_health_charging.wav") ?? TryLoadAudioFromAssets("focus_charge.wav");
            sfxFocusComplete ??= TryLoadAudioFromAssets("focus_health_heal.wav") ?? TryLoadAudioFromAssets("focus_complete.wav");
            sfxFocusReady ??= TryLoadAudioFromAssets("focus_ready.wav");

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
            EnsureFocusSfx();
            if (focusSfx == null || sfxFocusCharge == null) return;

            focusSfx.loop = true;
            focusSfx.clip = sfxFocusCharge;
            focusSfx.volume = Mathf.Clamp01(GetEffectiveSfxVolume());
            focusSfx.Play();
        }

        private void StopFocusChargeSfx()
        {
            if (focusSfx == null) return;
            focusSfx.loop = false;
            focusSfx.Stop();
        }

        /// <summary>
        /// Plays a focus sound. The clip must come from one of the accessors below and never from
        /// the field directly - see <see cref="FocusCompleteSfx"/> for why.
        /// </summary>
        private void PlayFocusSfx(AudioClip clip)
        {
            EnsureFocusSfx();
            if (focusSfx != null && clip != null)
                focusSfx.PlayOneShot(clip, Mathf.Clamp01(GetEffectiveSfxVolume()));
        }

        /// <summary>
        /// The focus-complete clip, resolved if it has not been already.
        /// <para>
        /// An accessor rather than the bare field because the clips are found lazily, and the order
        /// C# evaluates arguments in made that a trap: <c>PlayFocusSfx(sfxFocusComplete)</c> reads
        /// the field <em>before</em> the call that resolves it, so it passed the null it was still
        /// holding. The sound was silent the first time and correct every time after, once per
        /// scene, because the companion and its clips are built anew in each one. That was reported
        /// as most Shade sounds failing the first time they are used in a room.
        /// </para>
        /// </summary>
        private AudioClip FocusCompleteSfx
        {
            get { EnsureFocusSfx(); return sfxFocusComplete; }
        }

        /// <summary>The focus-ready chime, resolved if it has not been already. See <see cref="FocusCompleteSfx"/>.</summary>
        private AudioClip FocusReadySfx
        {
            get { EnsureFocusSfx(); return sfxFocusReady; }
        }

        private void TryPlayFocusCompleteSfx() => PlayFocusSfx(FocusCompleteSfx);

        /// <summary>Chimes the first frame SOUL crosses the Focus cost, not while it sits above it.</summary>
        private void CheckFocusReadySfx()
        {
            if (lastSoulForReady < 0)
            {
                lastSoulForReady = shadeSoul;
                return;
            }

            if (lastSoulForReady < focusSoulCost && shadeSoul >= focusSoulCost)
                PlayFocusSfx(FocusReadySfx);

            lastSoulForReady = shadeSoul;
        }

        private void UpdateSfxVolumes()
        {
            float volume = Mathf.Clamp01(GetEffectiveSfxVolume());
            if (focusSfx != null) focusSfx.volume = volume;
            if (spellSfx != null) spellSfx.volume = volume;
            if (knightSfx != null) knightSfx.volume = volume;
        }

        // ========== Knight movement SFX (dash, wings, Shade Cloak) ==========
        // Its own source, parented to this companion, so the Knight's sounds come from where the
        // Knight is rather than from Hornet - the two are routinely a screen apart.
        private AudioSource knightSfx;

        private AudioSource EnsureKnightSfx() => knightSfx ??= CreateSfxSource("KnightSFX");

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
            spellSfx ??= CreateSfxSource("ShadeSpellSFX");

            if (searchedSpellSfx) return;
            searchedSpellSfx = true;

            sfxFireball ??= TryLoadAudioFromAssets("hero_fireball.wav");
            sfxQuakePrepare ??= TryLoadAudioFromAssets("hero_quake_spell_prepare.wav");
            sfxQuakeImpact ??= TryLoadAudioFromAssets("hero_quake_spell_impact.wav");
            sfxVoidQuakeImpact ??= TryLoadAudioFromAssets("hero_void_quake_impact.wav");
            sfxScream ??= TryLoadAudioFromAssets("hero_scream_spell.wav");
            sfxVoidScream ??= TryLoadAudioFromAssets("hero_void_scream_spell.wav");

            if (sfxFireball == null || sfxQuakePrepare == null || sfxQuakeImpact == null
                || sfxVoidQuakeImpact == null || sfxScream == null || sfxVoidScream == null)
            {
                ResolveSpellSfxByName();
            }
        }

        private static readonly string[] FireballKeys = { "fireball", "vengeful", "spirit", "spell" };
        private static readonly string[] QuakePrepareKeys = { "quake", "prepare", "start", "spell" };
        private static readonly string[] QuakeImpactKeys = { "quake", "impact", "spell" };
        private static readonly string[] VoidQuakeKeys = { "void", "quake", "impact" };
        private static readonly string[] ScreamKeys = { "scream", "wraith", "howl", "spell" };
        private static readonly string[] VoidScreamKeys = { "void", "scream", "abyss" };

        /// <summary>
        /// Picks the best-matching clip for each unresolved spell sound in one pass over the loaded
        /// clips. Scoring all six together keeps this to a single walk and a single lowercasing.
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

        /// <summary>
        /// Plays a spell sound. The clip must come from one of the accessors below and never from
        /// the field directly - see <see cref="FocusCompleteSfx"/> for why.
        /// </summary>
        private void PlaySpellSfx(AudioClip clip)
        {
            EnsureSpellSfx();
            if (spellSfx != null && clip != null)
                spellSfx.PlayOneShot(clip, Mathf.Clamp01(GetEffectiveSfxVolume()));
        }

        // Resolved on read, for the reason given on FocusCompleteSfx: reading one of these fields
        // as an argument reads it before the call that fills it in, which cost every one of these
        // sounds its first use in each room.
        private AudioClip FireballSfx { get { EnsureSpellSfx(); return sfxFireball; } }
        private AudioClip ScreamSfx { get { EnsureSpellSfx(); return sfxScream; } }
        private AudioClip VoidScreamSfx { get { EnsureSpellSfx(); return sfxVoidScream; } }
        private AudioClip QuakePrepareSfx { get { EnsureSpellSfx(); return sfxQuakePrepare; } }
        private AudioClip QuakeImpactSfx { get { EnsureSpellSfx(); return sfxQuakeImpact; } }
        private AudioClip VoidQuakeImpactSfx { get { EnsureSpellSfx(); return sfxVoidQuakeImpact; } }

        private void TryPlayFireballSfx() => PlaySpellSfx(FireballSfx);

        private void TryPlayShriekSfx(bool upgraded) => PlaySpellSfx(upgraded ? VoidScreamSfx : ScreamSfx);

        private void TryPlayQuakePrepareSfx() => PlaySpellSfx(QuakePrepareSfx);

        private void TryPlayQuakeImpactSfx(bool upgraded) => PlaySpellSfx(upgraded ? VoidQuakeImpactSfx : QuakeImpactSfx);

    }
}
#nullable restore
