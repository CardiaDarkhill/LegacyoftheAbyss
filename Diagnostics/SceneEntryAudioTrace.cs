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

        private static float windowClosesAt = -1f;
        private static int recorded;
        private static bool installed;

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
        }

        internal static void Record(AudioSource? source, AudioClip? clip, string how)
        {
            if (Time.realtimeSinceStartup > windowClosesAt || recorded >= MaxEntriesPerScene)
            {
                return;
            }

            recorded++;

            string clipName = clip != null ? clip.name : "<no clip>";
            string owner = source != null ? Path(source.transform) : "<no source>";
            string position = source != null
                ? FormattableString.Invariant($"({source.transform.position.x:F1}, {source.transform.position.y:F1})")
                : "?";

            BugReportSystem.RecordEvent(
                "scene-audio",
                clipName,
                string.Format(CultureInfo.InvariantCulture, "{0} from {1} at {2}", how, owner, position));
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
