#nullable enable

using GlobalEnums;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// The Knight's sounds, taken from Hollow Knight's own set where the bundle can reach them.
    /// <para>
    /// The bundle does ship audio - 162 clips - even though <c>LoadAllAssets&lt;AudioClip&gt;()</c>
    /// returns none of them; see <see cref="KnightAssets.FindAudioClip"/>. What that reaches depends
    /// on how each clip is referenced: the ones hanging off the rig's own <c>Sounds</c> children are
    /// certain, the ones held in Hollow Knight's <c>HeroController</c> fields only load if that
    /// script bound to a real type here. So every sound below names its Hollow Knight clip first and
    /// a Silksong equivalent second, and <see cref="Report"/> says which one actually played.
    /// </para>
    /// </summary>
    internal static class KnightAudio
    {
        // Hollow Knight's clip names, as they appear in the bundle.
        private const string ClipDash = "hero_dash";
        private const string ClipWings = "hero_wings";
        private const string ClipShadeCloak = "hero_shade_dash_1";

        private static string s_report = "no sound played yet";

        /// <summary>Where the Knight's sounds came from, for <c>BugReportState</c>.</summary>
        internal static string Report => s_report;

        internal static void PlayDash(AudioSource? source, float volume)
            => Play(source, volume, ClipDash, "dash", HeroSounds.DASH);

        /// <summary>
        /// Monarch Wings. Hornet's own double jump is Faedown Cloak, which is the right stand-in and
        /// the one that was asked for, so it takes over rather than the dash sound if the Hollow
        /// Knight clip is out of reach.
        /// </summary>
        internal static void PlayWings(AudioSource? source, float volume)
        {
            if (TryPlayBundleClip(source, volume, ClipWings, "wings"))
            {
                return;
            }

            var hero = HeroController.UnsafeInstance;
            var clip = hero != null ? hero.doubleJumpClip : null;
            if (clip != null && source != null)
            {
                source.PlayOneShot(clip, Mathf.Clamp01(volume));
                s_report = "wings=Hornet's doubleJumpClip (Faedown Cloak)";
                return;
            }

            // Deliberately no third fallback: the dash sound reads as a dash, not as a jump.
            s_report = "wings=MISSING";
        }

        internal static void PlayShadeCloak(AudioSource? source, float volume)
            => Play(source, volume, ClipShadeCloak, "shade cloak", HeroSounds.DASH);

        private static void Play(AudioSource? source, float volume, string clipName, string label, HeroSounds fallback)
        {
            if (TryPlayBundleClip(source, volume, clipName, label))
            {
                return;
            }

            var hero = HeroController.UnsafeInstance;
            var audio = hero != null ? hero.AudioCtrl : null;
            if (audio == null)
            {
                s_report = $"{label}=MISSING";
                return;
            }

            audio.PlaySound(fallback);
            s_report = $"{label}=Silksong {fallback}";
        }

        private static bool TryPlayBundleClip(AudioSource? source, float volume, string clipName, string label)
        {
            if (source == null)
            {
                return false;
            }

            var clip = KnightAssets.FindAudioClip(clipName);
            if (clip == null)
            {
                return false;
            }

            source.PlayOneShot(clip, Mathf.Clamp01(volume));
            s_report = $"{label}={clipName} (bundle)";
            return true;
        }
    }
}
