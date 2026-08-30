#nullable enable

namespace LegacyoftheAbyss.Shade.Knight
{
    /// <summary>
    /// The clips holding Hollow Knight's HUD art in the bundle.
    /// <para>
    /// Named here rather than at the point of use because they are a contract with an asset bundle
    /// this project does not build: a wrong name costs the HUD its art and says nothing about why.
    /// Verified against the bundle, with the sprite each clip's first frame resolves to and the size
    /// of that region, so a future change can be checked rather than guessed at:
    /// </para>
    /// <list type="bullet">
    /// <item><c>Health Idle</c> -> <c>idle_v020000</c>, 70x57</item>
    /// <item><c>Health Empty</c> -> <c>health_backboard</c>, 57x153</item>
    /// <item><c>HUD Frame Idle</c> -> <c>HUD_frame_v020005</c>, 144x239</item>
    /// <item><c>Soul Orb Glow</c> -> <c>soul_orb_glow0000</c>, 129x125</item>
    /// <item><c>Health Refill</c> -> <c>refill0000</c>, the orb's filled interior</item>
    /// </list>
    /// </summary>
    internal static class KnightHud
    {
        internal const string MaskClip = "Health Idle";
        internal const string MaskBackboardClip = "Health Empty";
        internal const string FrameClip = "HUD Frame Idle";
        internal const string SoulOrbClip = "Soul Orb Glow";
        /// <summary>
        /// The orb's filled interior. A standalone texture rather than a clip - see
        /// <c>KnightAssets.TryBuildSpriteFromTexture</c>.
        /// </summary>
        internal const string SoulOrbFillTexture = "soul_orb_full_v020000";

        /// <summary>
        /// Baldur Shell's five HUD states, in order from an unbroken shell to a spent one.
        /// <para>
        /// Each is the <em>last</em> frame of its clip: these are break animations, so the frame
        /// that shows the state they leave behind is the one at the end, not the one at the start.
        /// <c>KnightAssets.TryBuildSprite</c> clamps its frame index, so asking past the end is how
        /// a caller says "the last one" without knowing the clip's length.
        /// </para>
        /// </summary>
        internal static readonly string[] BaldurShellStageClips =
        {
            "UI Appear",
            "UI Break 1",
            "UI Break 2",
            "UI Break 3",
            "UI Break 4"
        };
    }
}
