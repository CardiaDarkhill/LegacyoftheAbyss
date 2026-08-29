#nullable enable

using GlobalEnums;
using UnityEngine;

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// The Knight's sounds.
    /// <para>
    /// <b>Not from the bundle.</b> The Knight asset pack carries 468 animation clips and zero
    /// AudioClips - the bug report's Knight bundle row reads <c>audio (0)</c> - so Hollow Knight's
    /// own dash and wings sounds are simply not there to use. These play Silksong's equivalents
    /// through Hornet's audio controller instead, which is also what keeps them mixed and
    /// attenuated like every other sound in the game.
    /// </para>
    /// </summary>
    internal static class KnightAudio
    {
        private static string s_report = "no sound played yet";

        /// <summary>Where the Knight's sounds came from, for <c>BugReportState</c>.</summary>
        internal static string Report => s_report;

        internal static void PlayDash() => Play(HeroSounds.DASH, "dash");

        /// <summary>
        /// Monarch Wings. Silksong has no wings of its own, so the dash sound stands in - it is the
        /// closest thing in the game's own set to a burst of movement.
        /// </summary>
        internal static void PlayWings() => Play(HeroSounds.DASH, "wings (dash sound, no wings in Silksong's set)");

        internal static void PlayShadeCloak() => Play(HeroSounds.DASH, "shade cloak (dash sound)");

        private static void Play(HeroSounds sound, string label)
        {
            var hero = HeroController.UnsafeInstance;
            var audio = hero != null ? hero.AudioCtrl : null;
            if (audio == null)
            {
                s_report = $"{label}=no hero audio controller";
                return;
            }

            audio.PlaySound(sound);
            s_report = $"{label}={sound}";
        }
    }
}
