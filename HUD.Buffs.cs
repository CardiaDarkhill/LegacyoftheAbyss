#nullable disable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The companion's buff bar: a row of small status icons under the mask row.
/// <para>
/// Deliberately a row of anonymous slots fed from a list rather than a place per charm. Baldur
/// Shell is the first thing to want one and will not be the last, so adding the next buff is
/// appending to what <c>CollectHudBuffs</c> gathers, not touching the HUD at all.
/// </para>
/// <para>
/// Placement is config-driven and re-applied every frame by <c>HUD.Tuning.cs</c>, for the reason
/// every other piece of this HUD is: a row of icons cannot be positioned from sprite dimensions,
/// and Ctrl+F5 turns a guess-and-rebuild loop into a few seconds against the running screen.
/// </para>
/// </summary>
public partial class SimpleHUD
{
    /// <summary>One icon in the bar. Sprites are cached by their source, so this is cheap to build.</summary>
    internal readonly struct BuffIcon
    {
        internal BuffIcon(string id, Sprite sprite, bool rotated)
        {
            Id = id;
            Sprite = sprite;
            Rotated = rotated;
        }

        /// <summary>Stable per buff, so the bar can tell "changed" from "same icon again".</summary>
        internal string Id { get; }

        internal Sprite Sprite { get; }

        /// <summary>Whether the atlas stored this frame turned, as the masks are.</summary>
        internal bool Rotated { get; }
    }

    private RectTransform buffBarRoot;
    private readonly List<Image> buffIconImages = new List<Image>();
    private readonly List<BuffIcon> buffIcons = new List<BuffIcon>();

    /// <summary>The unscaled icon box the bar was built from, so a knob can rescale it.</summary>
    private Vector2 tuningBuffIconSize;

    /// <summary>How many slots the pool keeps. More than the bar is ever likely to show at once.</summary>
    private const int MaxBuffIcons = 8;

    private static Sprite[] baldurStageSprites;
    private static bool[] baldurStageRotated;

    /// <summary>
    /// Baldur Shell's five HUD states, cut from the bundle once. Index 0 is an unbroken shell and
    /// index 4 a spent one, matching <c>KnightHud.BaldurShellStageClips</c>.
    /// <para>
    /// Null entries are left in place rather than filled: a missing clip should cost that one state
    /// its icon, not the whole bar, and <see cref="SetBuffIcons"/> drops icons with no sprite.
    /// </para>
    /// </summary>
    private static void EnsureBaldurStageSprites()
    {
        if (baldurStageSprites != null)
        {
            return;
        }

        bool any = TryResolveStageSprites(
            LegacyoftheAbyss.Shade.Knight.KnightHud.BaldurShellStageClips,
            out var sprites,
            out var rotated);

        // The flags are kept either way. A charm can be worn before the bundle lands, so a miss
        // here is usually "not yet" rather than "not there", and the sprites stay uncached so the
        // next call asks again.
        baldurStageRotated = rotated;
        if (!any)
        {
            return;
        }
        baldurStageSprites = sprites;
    }

    /// <summary>
    /// The buff icon for a shell with <paramref name="charges"/> blows left, or an empty icon when
    /// the art is unavailable. <paramref name="maxCharges"/> is the full shell.
    /// </summary>
    internal static BuffIcon BuildBaldurShellIcon(int charges, int maxCharges)
    {
        EnsureBaldurStageSprites();
        if (baldurStageSprites == null)
        {
            return default;
        }

        int stages = baldurStageSprites.Length;
        int spent = Mathf.Clamp(maxCharges - charges, 0, stages - 1);
        return new BuffIcon("baldur_shell", baldurStageSprites[spent], baldurStageRotated[spent]);
    }

    private void BuildBuffBar(Canvas canvas, float uiScale)
    {
        if (canvas == null)
        {
            return;
        }

        var go = new GameObject("ShadeBuffBar");
        go.transform.SetParent(canvas.transform, false);

        buffBarRoot = go.AddComponent<RectTransform>();
        buffBarRoot.anchorMin = buffBarRoot.anchorMax = new Vector2(1f, 1f);
        buffBarRoot.pivot = new Vector2(1f, 1f);
        buffBarRoot.anchoredPosition = Vector2.zero;
        buffBarRoot.sizeDelta = Vector2.zero;

        tuningBuffIconSize = new Vector2(ModConfig.Instance.hudBuffIconSize, ModConfig.Instance.hudBuffIconSize) * uiScale;

        buffIconImages.Clear();
        for (int i = 0; i < MaxBuffIcons; i++)
        {
            var slot = new GameObject($"Buff{i}");
            slot.transform.SetParent(buffBarRoot, false);

            var rect = slot.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = tuningBuffIconSize;

            var image = CreateSlotArt(slot);
            image.raycastTarget = false;
            image.enabled = false;

            buffIconImages.Add(image);
            slot.SetActive(false);
        }
    }

    /// <summary>
    /// Replaces what the bar shows. Called every frame, so it compares against what is already up
    /// and does nothing when that matches - and compares in place rather than by building a key,
    /// because a per-frame string would be garbage generated purely to say "no change".
    /// </summary>
    internal void SetBuffIcons(IReadOnlyList<BuffIcon> icons)
    {
        if (MatchesCurrentBuffs(icons))
        {
            return;
        }

        buffIcons.Clear();
        if (icons != null)
        {
            for (int i = 0; i < icons.Count && buffIcons.Count < MaxBuffIcons; i++)
            {
                if (icons[i].Sprite != null)
                {
                    buffIcons.Add(icons[i]);
                }
            }
        }

        RefreshBuffIcons();
    }

    private bool MatchesCurrentBuffs(IReadOnlyList<BuffIcon> icons)
    {
        int shown = 0;
        if (icons != null)
        {
            for (int i = 0; i < icons.Count && shown < MaxBuffIcons; i++)
            {
                var icon = icons[i];
                if (icon.Sprite == null)
                {
                    continue;
                }

                if (shown >= buffIcons.Count
                    || buffIcons[shown].Id != icon.Id
                    || buffIcons[shown].Sprite != icon.Sprite)
                {
                    return false;
                }

                shown++;
            }
        }

        return shown == buffIcons.Count;
    }

    private void RefreshBuffIcons()
    {
        for (int i = 0; i < buffIconImages.Count; i++)
        {
            var image = buffIconImages[i];
            if (image == null)
            {
                continue;
            }

            bool used = i < buffIcons.Count;
            var slot = image.rectTransform.parent as RectTransform;
            if (slot != null && slot.gameObject.activeSelf != used)
            {
                slot.gameObject.SetActive(used);
            }

            if (!used)
            {
                image.enabled = false;
                continue;
            }

            image.sprite = buffIcons[i].Sprite;
            image.enabled = true;
        }
    }

    /// <summary>
    /// Lays the bar out from the config every frame, exactly as the mask row is. The row runs
    /// leftward from its anchor so it sits under the masks, which run the same way.
    /// </summary>
    private void ApplyBuffBarTuning(ModConfig config, float uiScale, Vector2 orbCentre)
    {
        if (buffBarRoot == null || soulOrbRoot == null)
        {
            return;
        }

        bool enabled = config.hudBuffBarEnabled && buffIcons.Count > 0;
        if (buffBarRoot.gameObject.activeSelf != enabled)
        {
            buffBarRoot.gameObject.SetActive(enabled);
        }

        if (!enabled)
        {
            return;
        }

        float size = Mathf.Max(1f, config.hudBuffIconSize) * uiScale * Mathf.Max(0.05f, config.hudBuffIconScale);
        Vector2 iconSize = new Vector2(size, size);
        tuningBuffIconSize = iconSize;

        buffBarRoot.anchoredPosition = new Vector2(
            soulOrbRoot.anchoredPosition.x + (config.hudBuffBarOffsetX * uiScale),
            orbCentre.y + (config.hudBuffBarOffsetY * uiScale));

        float spacing = config.hudBuffIconSpacing * uiScale;
        float x = 0f;

        for (int i = 0; i < buffIconImages.Count; i++)
        {
            var image = buffIconImages[i];
            if (image == null || i >= buffIcons.Count)
            {
                continue;
            }

            var slot = image.rectTransform.parent as RectTransform;
            if (slot == null)
            {
                continue;
            }

            slot.sizeDelta = iconSize;
            slot.anchoredPosition = new Vector2(-x, 0f);
            x += iconSize.x + spacing;

            bool rotated = buffIcons[i].Rotated;
            image.rectTransform.sizeDelta = rotated ? new Vector2(iconSize.y, iconSize.x) : iconSize;
            image.rectTransform.localEulerAngles = rotated ? new Vector3(0f, 0f, 90f) : Vector3.zero;
        }
    }
}
