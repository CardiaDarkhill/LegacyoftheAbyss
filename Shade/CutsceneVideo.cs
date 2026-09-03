#nullable enable

using UnityEngine;
using UnityEngine.Video;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Whether a pre-rendered cutscene is on screen right now.
    /// <para>
    /// The companion has to stand aside for these, and neither obvious way of knowing works. Naming
    /// the scene does not: <c>Memory_Needolin</c> is a jumping puzzle first and a cutscene second,
    /// so hiding the companion for the scene takes it away for the part the player is still
    /// playing. Watching for Hornet's control being relinquished does not either: half the game's
    /// scripted moments do that without a frame of video, and the companion has business during
    /// those - the bench rest is one.
    /// </para>
    /// <para>
    /// What is actually being asked is whether a video is covering the screen, so that is what is
    /// asked. Silksong plays them through <c>CinematicPlayer</c>, which drives a Unity
    /// <see cref="VideoPlayer"/>; its <c>isPlaying</c> is public, needs no reflection, and is true
    /// for exactly the stretch the film is up.
    /// </para>
    /// </summary>
    internal static class CutsceneVideo
    {
        /// <summary>
        /// How often the scene is swept for video players. They are rare, they are created with the
        /// scene rather than at the moment of playing, and <c>FindObjectsByType</c> is far too
        /// expensive to run per frame - so the list is cached and only the cheap <c>isPlaying</c>
        /// check happens every time.
        /// </summary>
        private const float RescanSeconds = 0.5f;

        private static VideoPlayer[]? s_players;
        private static float s_nextScan = -1f;

        internal static bool IsPlaying
        {
            get
            {
                try
                {
                    float now = Time.unscaledTime;
                    if (s_players == null || now >= s_nextScan)
                    {
                        s_nextScan = now + RescanSeconds;
                        s_players = Object.FindObjectsByType<VideoPlayer>(
                            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                    }

                    var players = s_players;
                    for (int i = 0; i < players.Length; i++)
                    {
                        var player = players[i];

                        // A destroyed player is the usual reason the cache is stale, and it is worth
                        // re-sweeping straight away rather than waiting out the interval: it means
                        // the scene has changed under us.
                        if (player == null)
                        {
                            s_players = null;
                            s_nextScan = -1f;
                            return false;
                        }

                        if (player.isPlaying)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                }

                return false;
            }
        }
    }
}
