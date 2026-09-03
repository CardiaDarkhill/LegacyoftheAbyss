#nullable disable
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Soul Vessels: a short column of small orbs beside the soul meter, filling from the bottom.
/// <para>
/// Hollow Knight hangs them off the left of its soul orb. This HUD is that one mirrored into the
/// top-right corner, so they hang off the right here - which is the same side of the orb, not the
/// same side of the screen. Where exactly is a config knob, as everything else on this HUD is.
/// </para>
/// <para>
/// Each vessel has five drawn states rather than a continuous fill, because that is the art that
/// exists: the bundle carries five levels, and a partly-filled vessel is one of them rather than a
/// bar clipped to a fraction.
/// </para>
/// </summary>
public partial class SimpleHUD
{
    private RectTransform vesselColumnRoot;
    private Image[] vesselImages;

    /// <summary>The vessels' five states, cut from the bundle once. See <c>KnightHud</c>.</summary>
    private static Sprite[] vesselStageSprites;
    private static bool[] vesselStageRotated;

    /// <summary>
    /// The height the empty vessel is drawn at in the atlas, which the other four are measured
    /// against. A full vessel's art is half as big again because it carries its glow, so sizing
    /// every state to one box would shrink the full one's body to match the empty one's - the
    /// vessel would appear to shrink as it filled. Held to one factor instead, so they keep the
    /// proportions they were drawn with.
    /// </summary>
    private static float vesselReferenceHeight;

    /// <summary>Soul in the vessels and how many there are, pushed by the companion.</summary>
    private int shadeVesselSoul;
    private int shadeVesselCount;

    private static void EnsureVesselStageSprites()
    {
        if (vesselStageSprites != null)
        {
            return;
        }

        // See KnightHud.SoulVesselStageClips for why the clips named after the fill levels cannot
        // be used, and the resting frame of each fill animation is taken instead.
        bool any = TryResolveStageSprites(
            LegacyoftheAbyss.Shade.Knight.KnightHud.SoulVesselStageClips,
            out var sprites,
            out var rotated);

        vesselStageRotated = rotated;
        if (!any)
        {
            return;
        }

        vesselStageSprites = sprites;

        var empty = sprites[0];
        vesselReferenceHeight = empty != null ? empty.rect.height : 0f;
        if (vesselReferenceHeight <= 0f)
        {
            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    vesselReferenceHeight = sprite.rect.height;
                    break;
                }
            }
        }
    }

    private void BuildVesselColumn(Canvas canvas)
    {
        if (canvas == null)
        {
            return;
        }

        var go = new GameObject("ShadeSoulVessels");
        go.transform.SetParent(canvas.transform, false);

        vesselColumnRoot = go.AddComponent<RectTransform>();
        vesselColumnRoot.anchorMin = vesselColumnRoot.anchorMax = new Vector2(1f, 1f);
        vesselColumnRoot.pivot = new Vector2(0.5f, 0.5f);
        vesselColumnRoot.anchoredPosition = Vector2.zero;
        vesselColumnRoot.sizeDelta = Vector2.zero;

        vesselImages = new Image[LegacyHelper.ShadeController.MaxSoulVessels];
        for (int i = 0; i < vesselImages.Length; i++)
        {
            var slot = new GameObject($"Vessel{i + 1}");
            slot.transform.SetParent(vesselColumnRoot, false);

            var slotRect = slot.AddComponent<RectTransform>();
            slotRect.anchorMin = slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);

            var image = CreateSlotArt(slot);
            image.raycastTarget = false;
            image.enabled = false;

            vesselImages[i] = image;
            slot.SetActive(false);
        }
    }

    /// <summary>What the companion holds in reserve, and how many vessels it has to hold it in.</summary>
    internal void SetShadeVessels(int vesselSoul, int vesselCount)
    {
        shadeVesselCount = Mathf.Clamp(vesselCount, 0, LegacyHelper.ShadeController.MaxSoulVessels);
        shadeVesselSoul = Mathf.Clamp(vesselSoul, 0, LegacyoftheAbyss.Shade.ShadeSoulVessels.Capacity(shadeVesselCount));
    }

    private void RefreshVessels()
    {
        if (vesselImages == null)
        {
            return;
        }

        EnsureVesselStageSprites();

        bool enabled = ModConfig.Instance.hudVesselsEnabled && shadeVesselCount > 0 && vesselStageSprites != null;

        for (int i = 0; i < vesselImages.Length; i++)
        {
            var image = vesselImages[i];
            if (image == null)
            {
                continue;
            }

            var slot = image.rectTransform.parent as RectTransform;
            bool used = enabled && i < shadeVesselCount;
            if (slot != null && slot.gameObject.activeSelf != used)
            {
                slot.gameObject.SetActive(used);
            }

            if (!used)
            {
                image.enabled = false;
                continue;
            }

            // The reserve is one number; this is the only place it is read as three.
            int held = LegacyoftheAbyss.Shade.ShadeSoulVessels.HeldInVessel(shadeVesselSoul, i);
            int stage = LegacyoftheAbyss.Shade.ShadeSoulVessels.StageFor(held);

            var sprite = vesselStageSprites[stage];
            if (sprite == null)
            {
                image.enabled = false;
                continue;
            }

            image.sprite = sprite;
            image.enabled = true;
        }
    }

    /// <summary>
    /// Lays the column out from the config every frame, as the rest of this HUD is. Vessel one is
    /// at the bottom, because that is the one that fills first.
    /// </summary>
    private void ApplyVesselTuning(ModConfig config, float uiScale, Vector2 orbCentre)
    {
        if (vesselColumnRoot == null || vesselImages == null)
        {
            return;
        }

        bool enabled = config.hudVesselsEnabled && shadeVesselCount > 0;
        if (vesselColumnRoot.gameObject.activeSelf != enabled)
        {
            vesselColumnRoot.gameObject.SetActive(enabled);
        }

        if (!enabled)
        {
            return;
        }

        float size = Mathf.Max(1f, config.hudVesselSize) * uiScale * Mathf.Max(0.05f, config.hudVesselScale);
        float spacing = config.hudVesselSpacing * uiScale;
        float step = size + spacing;

        vesselColumnRoot.anchoredPosition = orbCentre
            + new Vector2(config.hudVesselOffsetX * uiScale, config.hudVesselOffsetY * uiScale);

        // Centred on the column rather than grown from one end, so gaining a vessel widens the
        // column about the orb instead of shifting the two already there.
        float bottom = -(shadeVesselCount - 1) * step * 0.5f;
        float reference = vesselReferenceHeight > 0f ? vesselReferenceHeight : 1f;

        for (int i = 0; i < vesselImages.Length; i++)
        {
            var image = vesselImages[i];
            if (image == null || i >= shadeVesselCount)
            {
                continue;
            }

            var slot = image.rectTransform.parent as RectTransform;
            if (slot == null)
            {
                continue;
            }

            slot.sizeDelta = new Vector2(size, size);
            slot.anchoredPosition = new Vector2(0f, bottom + (i * step));

            var sprite = image.sprite;
            if (sprite == null)
            {
                continue;
            }

            // Sized against the empty vessel rather than to the slot, so the states keep their
            // drawn proportions and a full one still carries its glow.
            float factor = size / reference;
            Vector2 drawn = new Vector2(sprite.rect.width * factor, sprite.rect.height * factor);

            int stage = System.Array.IndexOf(vesselStageSprites, sprite);
            bool rotated = stage >= 0 && vesselStageRotated != null && stage < vesselStageRotated.Length && vesselStageRotated[stage];

            image.rectTransform.sizeDelta = rotated ? new Vector2(drawn.y, drawn.x) : drawn;
            image.rectTransform.localEulerAngles = rotated ? new Vector3(0f, 0f, 90f) : Vector3.zero;
        }
    }
}
