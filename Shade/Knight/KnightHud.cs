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
        /// A Soul Vessel's five resting states, from empty to full.
        /// <para>
        /// Each is the <em>last</em> frame of the clip that fills to that level, not the clip named
        /// after the level. The named ones cannot be used: <c>V_Half</c> is a single frame and that
        /// frame is the <em>empty</em> vessel, byte for byte what <c>V_Empty</c> draws, so a half
        /// vessel taken from it would read as an empty one and look exactly like the soul never
        /// arriving. The fill animations are correct throughout, and the frame they come to rest on
        /// is the state they leave behind - which is the frame wanted here.
        /// </para>
        /// <para>
        /// Verified against the bundle, with the sprite each resolves to. Note the sizes: a full
        /// vessel is drawn half as big again as the others because it carries its glow, so whoever
        /// draws these must size them against one another rather than to a fixed box.
        /// </para>
        /// <list type="bullet">
        /// <item><c>V_Empty</c> -> <c>appear0006 1</c>, 37x37</item>
        /// <item><c>V_UpToQuarter</c> -> <c>level_010005</c>, 37x37</item>
        /// <item><c>V_UpToHalf</c> -> <c>level_020005</c>, 37x38, packed turned</item>
        /// <item><c>V_UpTo3Quarter</c> -> <c>level_030005</c>, 37x37</item>
        /// <item><c>V_UpToFull</c> -> <c>full0009</c>, 62x60</item>
        /// </list>
        /// </summary>
        internal static readonly string[] SoulVesselStageClips =
        {
            "V_Empty",
            "V_UpToQuarter",
            "V_UpToHalf",
            "V_UpTo3Quarter",
            "V_UpToFull"
        };

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
