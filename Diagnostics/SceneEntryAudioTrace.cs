#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Names every sound that starts in the first couple of seconds of a new room.
    /// <para>
    /// "A Shade Soul goes off on room entry" has two completely different explanations - the Shade
    /// actually casting, or a clip being played by something that merely got loaded or instantiated -
    /// and from the player's chair they are the same noise. Nothing else in a report can tell them
    /// apart: the flight recorder does not see audio, and the thing that plays a clip is often not
    /// the thing that owns it.
    /// </para>
    /// <para>
    /// Deliberately bounded rather than always on. The window opens on a scene load and shuts a
    /// couple of seconds later, and the entry cap stops a room full of ambience from spending the
    /// event ring. Outside the window the prefix is one float comparison.
    /// </para>
    /// </summary>
    internal static class SceneEntryAudioTrace
    {
        // Long enough to cover the companion's own arrival, which is the part that matters and is
        // not the same moment as the scene load: the first attempt used 2.5s and shut roughly a
        // second before the Shade was placed and the Knight's 54 MB bundle finished loading, so it
        // recorded a room's ambience and nothing else.
        private const float WindowSeconds = 8f;
        private const int MaxEntriesPerScene = 60;

        private const int MaxBundleStacks = 6;

        private static float windowClosesAt = -1f;
        private static int recorded;
        private static int bundleStacksTaken;
        private static bool installed;

        /// <summary>The first few frames of a stack, without the noise either side of them.</summary>
        private static string Trim(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                return "<no stack>";
            }

            var lines = stackTrace.Split('\n');
            var kept = new List<string>();
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.Contains("SceneEntryAudioTrace") || trimmed.Contains("System.Environment"))
                {
                    continue;
                }

                kept.Add(trimmed);

                // Deep enough to get past PlayMaker. Six frames stopped at FsmState.OnEnter, which
                // named the action but not what had woken the object it was on - and for a pooled
                // object sitting parked, that caller is the whole question.
                if (kept.Count >= 18)
                {
                    break;
                }
            }

            return string.Join(" <- ", kept);
        }

        internal static void Install()
        {
            if (installed)
            {
                return;
            }

            installed = true;
            SceneManager.sceneLoaded += (scene, mode) => Open();
        }

        /// <summary>Reopens the window. Called on a scene load and again when a companion is placed.</summary>
        internal static void Open()
        {
            windowClosesAt = Time.realtimeSinceStartup + WindowSeconds;
            recorded = 0;
            bundleStacksTaken = 0;
        }

        /// <summary>The FSM whose one-shot action is running right now, for the next Record call.</summary>
        private static string pendingFsm = string.Empty;

        internal static void NoteFsmOneShot(HutongGames.PlayMaker.Fsm? fsm)
        {
            if (fsm == null)
            {
                pendingFsm = string.Empty;
                return;
            }

            var owner = fsm.GameObject;
            pendingFsm = " by FSM '" + (fsm.Name ?? "?") + "' on " + (owner != null ? Path(owner.transform) : "<no owner>");
        }

        internal static void Record(AudioSource? source, AudioClip? clip, string how)
        {
            if (Time.realtimeSinceStartup > windowClosesAt || recorded >= MaxEntriesPerScene)
            {
                return;
            }

            recorded++;

            // A clip name proves nothing on its own - Silksong reuses much of Hollow Knight's audio
            // library - but the bundle's own instance of one could only have been played by
            // something we brought in. The check only reaches clips the bundle's prefabs hold on an
            // AudioSource, which is 39 of its 162, so a "no" here is not an acquittal.
            bool fromBundle = clip != null && LegacyoftheAbyss.Shade.Knight.KnightAssets.IsBundleAudio(clip);

            // The game's own one-shot spawner. Whoever is playing the stray sound on room entry is
            // going through it, and naming the caller is the only thing left that will settle it.
            bool oneShotSpawner = how == "PlayOneShot"
                && source != null
                && source.gameObject.name.StartsWith("Audio Player", StringComparison.Ordinal);

            string origin = fromBundle ? " [KNIGHT BUNDLE]" : string.Empty;

            // Capped, and only for those two: a stack trace is expensive and the point is to name a
            // caller, not to narrate the room.
            string stack = string.Empty;
            if ((fromBundle || oneShotSpawner) && bundleStacksTaken < MaxBundleStacks)
            {
                bundleStacksTaken++;
                stack = " via " + Trim(Environment.StackTrace);
            }

            string clipName = clip != null ? clip.name : "<no clip>";
            string owner = source != null ? Path(source.transform) : "<no source>";
            string position = source != null
                ? FormattableString.Invariant($"({source.transform.position.x:F1}, {source.transform.position.y:F1})")
                : "?";

            string culprit = pendingFsm;
            pendingFsm = string.Empty;

            BugReportSystem.RecordEvent(
                "scene-audio",
                clipName,
                string.Format(CultureInfo.InvariantCulture, "{0} from {1} at {2}{3}{4}{5}", how, owner, position, culprit, origin, stack));
        }

        private static string Path(Transform node)
        {
            var builder = new StringBuilder(node.name);
            for (var parent = node.parent; parent != null; parent = parent.parent)
            {
                builder.Insert(0, '/').Insert(0, parent.name);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// The action the stray room-entry sound comes through.
    /// <para>
    /// The captured stack ended at <c>FsmState.OnEnter</c>, which says a PlayMaker state was entered
    /// and played a clip but not <i>whose</i> - PlayMaker's own frames carry no FSM identity. This
    /// reads it off the action itself, which is the last thing needed to name the culprit.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    internal static class AudioPlayerOneShotSingle_OnEnter_SceneEntryTrace
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.DeclaredMethod(
                typeof(HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle), "OnEnter");
            if (method == null)
            {
                LegacyHelper.LogWarning("Scene-entry audio trace: AudioPlayerOneShotSingle.OnEnter not found.");
                yield break;
            }

            yield return method;
        }

        private static void Prefix(HutongGames.PlayMaker.Actions.AudioPlayerOneShotSingle __instance)
            => SceneEntryAudioTrace.NoteFsmOneShot(__instance?.Fsm);
    }

    /// <summary>
    /// <c>PlayOneShot</c> resolved by shape rather than named: it is overloaded, and naming an
    /// overload in the attribute throws rather than returning nothing.
    /// </summary>
    [HarmonyPatch]
    internal static class AudioSource_PlayOneShot_SceneEntryTrace
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            // The one-argument overload forwards to this one, so patching it alone covers both.
            var method = AccessTools.Method(typeof(AudioSource), "PlayOneShot", new[] { typeof(AudioClip), typeof(float) });
            if (method == null)
            {
                LegacyHelper.LogWarning("Scene-entry audio trace disabled: AudioSource.PlayOneShot(AudioClip, float) not found.");
                yield break;
            }

            yield return method;
        }

        private static void Prefix(AudioSource __instance, AudioClip clip)
            => SceneEntryAudioTrace.Record(__instance, clip, "PlayOneShot");
    }

    [HarmonyPatch]
    internal static class AudioSource_Play_SceneEntryTrace
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var method = AccessTools.Method(typeof(AudioSource), "Play", Type.EmptyTypes);
            if (method == null)
            {
                LegacyHelper.LogWarning("Scene-entry audio trace disabled: AudioSource.Play() not found.");
                yield break;
            }

            yield return method;
        }

        private static void Prefix(AudioSource __instance)
            => SceneEntryAudioTrace.Record(__instance, __instance != null ? __instance.clip : null, "Play");
    }
}
