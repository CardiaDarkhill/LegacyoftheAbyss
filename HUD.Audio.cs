#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SimpleHUD
{
    private AudioSource sfxSource;
    private List<AudioClip> shadeHurtCandidates;
    private int shadeHurtIdx;
    private AudioClip pinnedHurtSingle;
    private AudioClip pinnedHurtDouble;
    // Negative cache: Resources.FindObjectsOfTypeAll walks every loaded object, so a
    // failed lookup must not re-run on the next hit.
    private bool searchedPinnedHurtClips;

    private AudioSource EnsureSfxSource()
    {
        if (sfxSource == null)
        {
            var go = new GameObject("ShadeHUD_SFX");
            go.transform.SetParent(transform, false);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }

        sfxSource.volume = Mathf.Clamp01(LegacyHelper.GetEffectiveSfxVolume());
        return sfxSource;
    }

    private void TryPlayPinnedHurtSfx(int lost)
    {
        try
        {
            var source = EnsureSfxSource();
            ResolvePinnedHurtClips();

            AudioClip clip = (lost >= 2 && pinnedHurtDouble != null) ? pinnedHurtDouble : pinnedHurtSingle;
            if (clip != null)
            {
                source.PlayOneShot(clip);
                return;
            }
        }
        catch { }

        TryPlayDamageSfx();
    }

    /// <summary>
    /// Locates the two pinned hero damage clips in a single pass over the loaded set.
    /// Exact name matches win; a substring match is kept as a fallback for each slot.
    /// </summary>
    private void ResolvePinnedHurtClips()
    {
        if (searchedPinnedHurtClips) return;
        searchedPinnedHurtClips = true;

        var all = Resources.FindObjectsOfTypeAll<AudioClip>();
        if (all == null) return;

        AudioClip singleFallback = null;
        AudioClip doubleFallback = null;

        foreach (var c in all)
        {
            if (c == null) continue;
            string n = c.name;
            if (string.IsNullOrEmpty(n)) continue;

            if (pinnedHurtSingle == null && string.Equals(n, "hero_damage", StringComparison.OrdinalIgnoreCase))
                pinnedHurtSingle = c;
            else if (singleFallback == null && n.Contains("hero_damage", StringComparison.OrdinalIgnoreCase))
                singleFallback = c;

            if (pinnedHurtDouble == null && string.Equals(n, "hero_double_damage", StringComparison.OrdinalIgnoreCase))
                pinnedHurtDouble = c;
            else if (doubleFallback == null && n.Contains("hero_double_damage", StringComparison.OrdinalIgnoreCase))
                doubleFallback = c;

            if (pinnedHurtSingle != null && pinnedHurtDouble != null) break;
        }

        pinnedHurtSingle ??= singleFallback;
        pinnedHurtDouble ??= doubleFallback;
    }

    private void TryPlayDamageSfx()
    {
        try
        {
            var source = EnsureSfxSource();

            if (shadeHurtCandidates == null || shadeHurtCandidates.Count == 0)
            {
                shadeHurtCandidates = BuildShadeHurtCandidates();
                shadeHurtIdx = 0;
            }

            if (shadeHurtCandidates.Count == 0) return;

            var clip = shadeHurtCandidates[shadeHurtIdx % shadeHurtCandidates.Count];
            shadeHurtIdx++;
            if (clip != null)
            {
                source.PlayOneShot(clip);
            }
        }
        catch { }
    }

    /// <summary>
    /// Ranks every loaded clip by how much it looks like a hero-damage sound.
    /// The score is computed once per clip; the previous LINQ chain re-ran up to nine
    /// substring searches per clip on every sort comparison.
    /// </summary>
    private static int ScoreHurtCandidate(string n)
    {
        bool take = n.Contains("take", StringComparison.OrdinalIgnoreCase);
        bool hit = n.Contains("hit", StringComparison.OrdinalIgnoreCase);
        bool hornet = n.Contains("hornet", StringComparison.OrdinalIgnoreCase);
        bool hurt = n.Contains("hurt", StringComparison.OrdinalIgnoreCase);
        bool damage = n.Contains("damage", StringComparison.OrdinalIgnoreCase);
        bool takeHit = n.Contains("take_hit", StringComparison.OrdinalIgnoreCase);

        if (!(hurt || takeHit || damage || hit || hornet)) return -1;

        // Bit weights preserve the original ThenByDescending precedence:
        // (take && hit) > hornet > hurt > damage.
        int score = 0;
        if (take && hit) score |= 8;
        if (hornet) score |= 4;
        if (hurt) score |= 2;
        if (damage) score |= 1;
        return score;
    }

    /// <summary>
    /// Every loaded clip that looks like a hero-damage sound, best first. The fallback for when the
    /// two clips <see cref="ResolvePinnedHurtClips"/> pins by name are not in this build.
    /// <para>
    /// This used to crawl Hornet's PlayMaker FSM first, through a field named <c>HeroFSM</c>. There
    /// is no such field - the game names them <c>damageEffectFSM</c>, <c>sprintFSM</c> and so on -
    /// so the crawl resolved to nothing on every build and this scan has always been what answered.
    /// </para>
    /// </summary>
    private List<AudioClip> BuildShadeHurtCandidates()
    {
        var list = new List<AudioClip>();
        var seen = new HashSet<AudioClip>();

        try
        {
            var all = Resources.FindObjectsOfTypeAll<AudioClip>();
            if (all != null && all.Length > 0)
            {
                var scored = new List<(AudioClip Clip, int Score)>();
                foreach (var c in all)
                {
                    if (c == null) continue;
                    string n = c.name ?? string.Empty;
                    int score = ScoreHurtCandidate(n);
                    if (score >= 0) scored.Add((c, score));
                }

                // OrderByDescending is a stable sort, matching the previous ordering
                // for clips that tie on score.
                foreach (var entry in scored.OrderByDescending(x => x.Score))
                {
                    if (seen.Add(entry.Clip)) list.Add(entry.Clip);
                }
            }
        }
        catch { }

        return list;
    }
}
#nullable restore
