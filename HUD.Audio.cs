using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public partial class SimpleHUD
{
    private AudioSource sfxSource;
    private List<AudioClip> shadeHurtCandidates;
    private int shadeHurtIdx;
    private AudioClip pinnedHurtSingle;
    private AudioClip pinnedHurtDouble;

    private void TryPlayPinnedHurtSfx(int lost)
    {
        try
        {
            if (sfxSource == null)
            {
                var go = new GameObject("ShadeHUD_SFX");
                go.transform.SetParent(this.transform, false);
                sfxSource = go.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false; sfxSource.spatialBlend = 0f; sfxSource.volume = 1f;
            }
            if (pinnedHurtSingle == null || pinnedHurtDouble == null)
            {
                AudioClip[] all = Resources.FindObjectsOfTypeAll<AudioClip>();
                if (all != null)
                {
                    foreach (var c in all)
                    {
                        if (c == null) continue; string n = c.name ?? string.Empty;
                        if (pinnedHurtSingle == null && string.Equals(n, "hero_damage", StringComparison.OrdinalIgnoreCase)) pinnedHurtSingle = c;
                        if (pinnedHurtDouble == null && string.Equals(n, "hero_double_damage", StringComparison.OrdinalIgnoreCase)) pinnedHurtDouble = c;
                        if (pinnedHurtSingle != null && pinnedHurtDouble != null) break;
                    }
                    if (pinnedHurtSingle == null)
                        foreach (var c in all) { var n = c != null ? c.name : null; if (!string.IsNullOrEmpty(n) && n.IndexOf("hero_damage", StringComparison.OrdinalIgnoreCase) >= 0) { pinnedHurtSingle = c; break; } }
                    if (pinnedHurtDouble == null)
                        foreach (var c in all) { var n = c != null ? c.name : null; if (!string.IsNullOrEmpty(n) && n.IndexOf("hero_double_damage", StringComparison.OrdinalIgnoreCase) >= 0) { pinnedHurtDouble = c; break; } }
                }
            }
            AudioClip clip = (lost >= 2 && pinnedHurtDouble != null) ? pinnedHurtDouble : pinnedHurtSingle;
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
                return;
            }
        }
        catch { }
        TryPlayDamageSfx();
    }

    private void TryPlayDamageSfx()
    {
        try
        {
            if (sfxSource == null)
            {
                var go = new GameObject("ShadeHUD_SFX");
                go.transform.SetParent(this.transform, false);
                sfxSource = go.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false; sfxSource.spatialBlend = 0f; sfxSource.volume = 1f;
            }
            if (shadeHurtCandidates == null || shadeHurtCandidates.Count == 0)
            {
                shadeHurtCandidates = BuildShadeHurtCandidates();
                shadeHurtIdx = 0;
            }
            if (shadeHurtCandidates != null && shadeHurtCandidates.Count > 0)
            {
                var clip = shadeHurtCandidates[shadeHurtIdx % shadeHurtCandidates.Count]; shadeHurtIdx++;
                if (clip != null) sfxSource.PlayOneShot(clip);
            }
        }
        catch { }
    }

    private List<AudioClip> BuildShadeHurtCandidates()
    {
        var list = new List<AudioClip>(); var seen = new HashSet<AudioClip>();
        try { var fsmClips = FindHurtClipsFromHornetFSM(); if (fsmClips != null) foreach (var c in fsmClips) if (c != null && seen.Add(c)) list.Add(c); } catch { }
        try {
            var all = Resources.FindObjectsOfTypeAll<AudioClip>();
            if (all != null && all.Length > 0)
            {
                var ordered = all.Where(c => c != null).Select(c => new { clip = c, n = (c.name ?? string.Empty) })
                    .Where(x => x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("take_hit", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0
                             || x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(x => (x.n.IndexOf("take", StringComparison.OrdinalIgnoreCase) >= 0 && x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0))
                    .ThenByDescending(x => x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ThenByDescending(x => x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ThenByDescending(x => x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(x => x.clip);
                foreach (var c in ordered) if (c != null && seen.Add(c)) list.Add(c);
            }
        } catch { }
        return list;
    }

    private List<AudioClip> FindHurtClipsFromHornetFSM()
    {
        var result = new List<AudioClip>();
        try
        {
            var hc = HeroController.instance;
            if (hc != null)
            {
                var fsmField = hc.GetType().GetField("HeroFSM", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var fsmVal = fsmField?.GetValue(hc);
                if (fsmVal != null)
                {
                    var statesProp = fsmVal.GetType().GetProperty("States", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var states = (statesProp != null) ? (statesProp.GetValue(fsmVal, null) as System.Collections.IEnumerable) : null;
                    if (states != null)
                    {
                        foreach (var s in states)
                        {
                            var actionsProp = s.GetType().GetProperty("Actions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            var actions = (actionsProp != null) ? (actionsProp.GetValue(s, null) as System.Collections.IEnumerable) : null;
                            if (actions == null) continue;
                            foreach (var action in actions)
                            {
                                if (action == null) continue;
                                try
                                {
                                    var at = action.GetType();
                                    var props = at.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                    foreach (var p in props)
                                    {
                                        if (p.PropertyType == typeof(AudioClip))
                                        {
                                            var clip2 = p.GetValue(action, null) as AudioClip;
                                            if (clip2 != null && !result.Contains(clip2)) result.Add(clip2);
                                        }
                                        var pt = p.PropertyType;
                                        if (pt != null && pt.FullName != null && pt.FullName.Contains("UnityEngine.AudioSource"))
                                        {
                                            var src = p.GetValue(action, null) as AudioSource;
                                            if (src != null && src.clip != null && !result.Contains(src.clip)) result.Add(src.clip);
                                        }
                                        if (pt != null && pt.FullName != null && pt.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
                                        {
                                            var pobj = p.GetValue(action, null);
                                            if (pobj != null)
                                            {
                                                var vprop = pobj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
                                                var v = (vprop != null) ? vprop.GetValue(pobj, null) : null;
                                                var clip3 = v as AudioClip;
                                                if (clip3 != null && !result.Contains(clip3)) result.Add(clip3);
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }
        catch { }
        return result;
    }
}


