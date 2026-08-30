#nullable disable
using UnityEngine;

/// <summary>
/// Live layout knobs for the HUD.
/// <para>
/// Every HUD placement so far has been a guess made from sprite dimensions, shipped, screenshotted
/// and guessed again. These move that loop into the game: the values live in config.json, they are
/// re-applied every frame, and Ctrl+F5 rereads the file - so the numbers can be dialled in against
/// the actual screen in seconds instead of a build at a time.
/// </para>
/// <para>
/// Applied rather than baked so that nothing has to be torn down and rebuilt. The defaults were
/// dialled in against the running game, so leaving them alone is the shipped layout.
/// </para>
/// </summary>
public partial class SimpleHUD
{
    /// <summary>The unscaled sizes the built layout was derived from, kept so a knob can rescale them.</summary>
    private Vector2 tuningOrbPixels;

    private Vector2 tuningMaskSize;
    private Vector2 tuningFramePixels;
    private Vector2 tuningOrbBasePosition;

    private void ApplyHudTuning()
    {
        var config = ModConfig.Instance;
        if (config == null || soulOrbRoot == null)
        {
            return;
        }

        float uiScale = GetUIScale();

        soulOrbRoot.sizeDelta = tuningOrbPixels * uiScale * Mathf.Max(0.05f, config.hudOrbScale);
        soulOrbRoot.anchoredPosition = tuningOrbBasePosition
            + new Vector2(config.hudOrbOffsetX, config.hudOrbOffsetY) * uiScale;

        // The fill is a sibling of the background rather than a child of the root's own rect, so it
        // does not inherit a resize. Left behind, it draws the orb sprite stretched across whatever
        // size the build happened to give it, which is the hard-edged white block over the socket.
        if (soulImage != null)
        {
            soulImage.rectTransform.sizeDelta = soulOrbRoot.sizeDelta;
        }

        Vector2 orbCentre = soulOrbRoot.anchoredPosition
            + new Vector2(-soulOrbRoot.sizeDelta.x * 0.5f, -soulOrbRoot.sizeDelta.y * 0.5f);

        ApplyFrameTuning(config, uiScale, orbCentre);
        ApplyMaskTuning(config, uiScale, orbCentre);
        ApplyBuffBarTuning(config, uiScale, orbCentre);
    }

    private void ApplyFrameTuning(ModConfig config, float uiScale, Vector2 orbCentre)
    {
        if (hudFrameImage == null)
        {
            return;
        }

        var go = hudFrameImage.gameObject;
        if (go.activeSelf != config.hudFrameEnabled)
        {
            go.SetActive(config.hudFrameEnabled);
        }

        if (!config.hudFrameEnabled)
        {
            // The plate carries the orb's dark socket, so without it the flat disc has to come back.
            if (soulBgImage != null)
            {
                soulBgImage.enabled = true;
            }

            return;
        }

        if (soulBgImage != null)
        {
            soulBgImage.enabled = false;
        }

        float scale = uiScale * Mathf.Max(0.05f, config.hudFrameScale);
        var rect = hudFrameImage.rectTransform;
        rect.localEulerAngles = new Vector3(0f, 0f, config.hudFrameRotation);
        rect.localScale = new Vector3(config.hudFrameMirror ? -scale : scale, scale, 1f);

        // Placed by its socket, not its middle: the socket is off-centre in the plate, so aligning
        // the two centres leaves the orb sitting on the horn.
        var socketFromCentre = new Vector2(
            (config.hudFrameSocketX - 0.5f) * tuningFramePixels.x,
            -(config.hudFrameSocketY - 0.5f) * tuningFramePixels.y);

        rect.anchoredPosition = orbCentre
            - (socketFromCentre * scale)
            + (new Vector2(config.hudFrameOffsetX, config.hudFrameOffsetY) * uiScale);
    }

    private void ApplyMaskTuning(ModConfig config, float uiScale, Vector2 orbCentre)
    {
        if (maskImages == null || healthContainer == null)
        {
            return;
        }

        Vector2 maskSize = tuningMaskSize * Mathf.Max(0.05f, config.hudMaskScale);
        float spacing = config.hudMaskSpacing * uiScale;

        var containerRect = healthContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            containerRect.anchoredPosition = new Vector2(
                soulOrbRoot.anchoredPosition.x + (config.hudMaskRowOffsetX * uiScale),
                orbCentre.y + (maskSize.y * 0.5f) + (config.hudMaskRowOffsetY * uiScale));
        }

        float x = 0f;
        for (int i = 0; i < maskImages.Length; i++)
        {
            var img = maskImages[i];
            if (img == null)
            {
                continue;
            }

            // The art is a centred child of its slot; the slot is what the row lays out.
            var slot = img.rectTransform.parent as RectTransform;
            if (slot == null)
            {
                continue;
            }

            slot.sizeDelta = maskSize;
            slot.anchoredPosition = new Vector2(-x, 0f);
            x += maskSize.x + spacing;

            img.rectTransform.sizeDelta = maskSpriteRotated
                ? new Vector2(maskSize.y, maskSize.x)
                : maskSize;
        }

        overcharmMaskSize = maskSize;
        overcharmMaskSpacing = spacing;
    }
}
