#nullable disable
using System;
using System.Collections;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleHUD
{
    private void SubscribeToCharmInventory()
    {
        var inventory = ShadeRuntime.Charms;
        if (inventory == null)
        {
            UnsubscribeFromCharmInventory();
            return;
        }

        if (subscribedCharmInventory == inventory)
        {
            return;
        }

        UnsubscribeFromCharmInventory();

        subscribedCharmInventory = inventory;
        try
        {
            subscribedCharmInventory.StateChanged += HandleCharmInventoryChanged;
            charmInventoryDirty = true;
        }
        catch
        {
            subscribedCharmInventory = null;
        }
    }

    private void UnsubscribeFromCharmInventory()
    {
        if (subscribedCharmInventory == null)
        {
            return;
        }

        try
        {
            subscribedCharmInventory.StateChanged -= HandleCharmInventoryChanged;
        }
        catch
        {
        }

        subscribedCharmInventory = null;
    }

    private void HandleCharmInventoryChanged()
    {
        charmInventoryDirty = true;
    }

    private void CreateUI()
    {
        // Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = -500;
        scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f; // height bias
        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Soul orb root
        soulOrbRoot = new GameObject("SoulOrb").AddComponent<RectTransform>();
        soulOrbRoot.SetParent(canvas.transform, false);
        soulOrbRoot.anchorMin = soulOrbRoot.anchorMax = new Vector2(1, 1);
        soulOrbRoot.pivot = new Vector2(1, 1);
        Vector2 orbPixels = soulOrbSprite != null
            ? new Vector2(soulOrbSprite.rect.width, soulOrbSprite.rect.height)
            : new Vector2(96, 96);
        float uiScale = GetUIScale();
        soulOrbRoot.sizeDelta = orbPixels * uiScale * 0.85f;
        soulOrbRoot.anchoredPosition = new Vector2(-200f * uiScale, -20f * uiScale);
        soulOrbRoot.localScale = new Vector3(-1f, 1f, 1f); // mirror like previous frame
        orbGameplayScale = soulOrbRoot.localScale;
        orbMenuScale = orbGameplayScale;

        // Background
        var soulBgGO = new GameObject("SoulBackground");
        soulBgGO.transform.SetParent(soulOrbRoot, false);
        soulBgImage = soulBgGO.AddComponent<Image>();
        soulBgImage.sprite = soulOrbSprite != null ? soulOrbSprite : BuildCircleSprite();
        soulBgImage.preserveAspect = true;
        var soulBgRect = soulBgImage.rectTransform;
        soulBgRect.anchorMin = new Vector2(0, 0);
        soulBgRect.anchorMax = new Vector2(1, 1);
        soulBgRect.pivot = new Vector2(0.5f, 0.5f);
        soulBgRect.anchoredPosition = Vector2.zero;
        soulBgRect.sizeDelta = Vector2.zero;
        soulBgImage.color = new Color(0.6f, 0.6f, 0.6f, 0.35f);

        // Reveal mask (for potential future use)
        var maskGO = new GameObject("SoulRevealMask");
        maskGO.transform.SetParent(soulOrbRoot, false);
        soulRevealMask = maskGO.AddComponent<RectTransform>();
        soulRevealMask.anchorMin = new Vector2(0, 0);
        soulRevealMask.anchorMax = new Vector2(1, 0);
        soulRevealMask.pivot = new Vector2(0.5f, 0f);
        soulRevealMask.anchoredPosition = Vector2.zero;
        soulRevealMask.sizeDelta = new Vector2(0f, 0f);
        maskGO.AddComponent<RectMask2D>();

        // Inner orb image
        var soulImgGO = new GameObject("SoulImage");
        soulImgGO.transform.SetParent(soulOrbRoot, false);
        soulImage = soulImgGO.AddComponent<Image>();
        soulImage.sprite = soulOrbSprite != null ? soulOrbSprite : BuildCircleSprite();
        soulImage.preserveAspect = true;
        soulImage.maskable = true;
        soulImage.raycastTarget = false;
        soulImage.color = Color.white;
        soulImage.type = Image.Type.Filled;
        soulImage.fillMethod = Image.FillMethod.Vertical;
        soulImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        soulImage.fillAmount = 0f;
        var soulImgRect = soulImage.rectTransform;
        soulImgRect.anchorMin = new Vector2(0.5f, 0f);
        soulImgRect.anchorMax = new Vector2(0.5f, 0f);
        soulImgRect.pivot = new Vector2(0.5f, 0f);
        soulImgRect.anchoredPosition = Vector2.zero;
        soulImgRect.sizeDelta = soulOrbRoot.sizeDelta;

        // Health masks container
        healthContainer = new GameObject("HealthContainer");
        healthContainer.transform.SetParent(canvas.transform, false);
        var hRect = healthContainer.AddComponent<RectTransform>();
        hRect.anchorMin = hRect.anchorMax = new Vector2(1, 1);
        hRect.pivot = new Vector2(1, 1);
        Vector2 maskPixelsRef = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33, 41);
        float maskHeight = maskPixelsRef.y * uiScale * MaskScale;
        float orbCenterY = soulOrbRoot.anchoredPosition.y - (soulOrbRoot.sizeDelta.y * 0.5f);
        hRect.anchoredPosition = new Vector2(
            soulOrbRoot.anchoredPosition.x,
            orbCenterY + (maskHeight * 0.5f)
        );
        healthGameplayScale = hRect.localScale;
        healthMenuScale = healthGameplayScale;

        BuildMasks(hRect, uiScale);

        unlockPopup = gameObject.GetComponent<ShadeUnlockPopup>();
        if (unlockPopup == null)
        {
            unlockPopup = gameObject.AddComponent<ShadeUnlockPopup>();
        }
        unlockPopup.Initialize(canvas, GetUIScale);
        UpdateMenuOrientation(ShouldTreatAsMenu());
    }

    private void RefreshHealth()
    {
        HandleAssistVisibilityChange();

        var charms = ShadeRuntime.Charms;
        hivebloodEquipped = charms != null && charms.IsEquipped(ShadeCharmId.Hiveblood);

        if (shadeAssistModeActive)
        {
            int total = Mathf.Max(0, shadeMax) + Mathf.Max(0, shadeLifebloodMax);
            previousShadeTotalHealth = Mathf.Clamp(shadeHealth + shadeLifeblood, 0, total);
            HideHivebloodPreview();
            return;
        }

        if (pendingMaskRefresh && healthContainer != null)
        {
            RebuildMasks();
        }

        if (maskImages == null)
        {
            pendingMaskRefresh = true;
            HideHivebloodPreview();
            return;
        }

        int normalMax = Mathf.Max(0, shadeMax);
        int lifebloodMax = Mathf.Max(0, shadeLifebloodMax);
        int totalMax = normalMax + lifebloodMax;

        if (maskImages.Length != totalMax)
        {
            RebuildMasks();
            if (maskImages == null) return;
        }

        int currentNormal = Mathf.Clamp(shadeHealth, 0, normalMax);
        int currentLifeblood = Mathf.Clamp(shadeLifeblood, 0, lifebloodMax);
        int totalCurrent = currentNormal + currentLifeblood;

        if (totalCurrent < previousShadeTotalHealth)
        {
            int lost = previousShadeTotalHealth - totalCurrent;
            if (suppressNextDamageSound)
            {
                suppressNextDamageSound = false;
            }
            else if (lost > 0)
            {
                TryPlayPinnedHurtSfx(lost);
            }

            int previousClamped = Mathf.Clamp(previousShadeTotalHealth, 0, maskImages.Length);
            int currentClamped = Mathf.Clamp(totalCurrent, 0, maskImages.Length);
            for (int i = previousClamped - 1; i >= currentClamped; i--)
            {
                if (i < 0 || i >= maskImages.Length) continue;
                bool lifeblood = i >= normalMax;
                StartCoroutine(LoseHealth(maskImages[i], shadeOvercharmed, lifeblood, hivebloodEquipped));
            }
        }
        else if (suppressNextDamageSound)
        {
            suppressNextDamageSound = false;
        }

        Color filledColor = GetNormalMaskFilledColor();
        for (int i = 0; i < normalMax && i < maskImages.Length; i++)
        {
            var img = maskImages[i];
            if (img == null) continue;
            img.sprite = maskSprite != null ? maskSprite : img.sprite;
            img.color = i < currentNormal ? filledColor : missingMaskColor;
        }

        for (int i = 0; i < lifebloodMax; i++)
        {
            int index = normalMax + i;
            if (index >= maskImages.Length) break;
            var img = maskImages[index];
            if (img == null) continue;
            img.sprite = maskSprite != null ? maskSprite : img.sprite;
            bool filled = i < currentLifeblood;
            if (filled)
            {
                if (!img.gameObject.activeSelf)
                {
                    img.gameObject.SetActive(true);
                }
                img.color = lifebloodMaskColor;
            }
            else if (!animatingMaskImages.Contains(img))
            {
                if (img.gameObject.activeSelf)
                {
                    img.gameObject.SetActive(false);
                }
            }
        }

        RefreshHivebloodPreview();
        previousShadeTotalHealth = totalCurrent;
    }

    private Color GetNormalMaskFilledColor()
    {
        if (shadeOvercharmed)
        {
            return overcharmMaskColor;
        }

        return hivebloodEquipped ? hivebloodMaskColor : Color.white;
    }

    private void RefreshHivebloodPreview()
    {
        if (hivebloodPreviewMask == null && maskImages == null)
        {
            return;
        }

        if (shadeAssistModeActive)
        {
            HideHivebloodPreview();
            return;
        }

        var charms = ShadeRuntime.Charms;
        if (charms == null || !charms.HivebloodMaskRegenerating)
        {
            HideHivebloodPreview();
            return;
        }

        float elapsed = Mathf.Clamp(charms.HivebloodRegenTimer, 0f, charms.HivebloodRegenDuration);
        if (elapsed < HivebloodPreviewFirstStageSeconds)
        {
            HideHivebloodPreview();
            return;
        }

        if (maskImages == null || maskImages.Length == 0)
        {
            HideHivebloodPreview();
            return;
        }

        int normalMax = Mathf.Max(0, shadeMax);
        if (normalMax <= 0 || shadeHealth < 0 || shadeHealth >= normalMax)
        {
            HideHivebloodPreview();
            return;
        }

        if (shadeHealth >= maskImages.Length)
        {
            HideHivebloodPreview();
            return;
        }

        var targetImage = maskImages[shadeHealth];
        if (targetImage == null)
        {
            HideHivebloodPreview();
            return;
        }

        var previewImage = EnsureHivebloodPreviewMask();
        var previewRect = previewImage.rectTransform;
        var targetRect = targetImage.rectTransform;

        previewRect.SetParent(targetRect, false);
        previewRect.anchorMin = previewRect.anchorMax = new Vector2(0.5f, 0.5f);
        previewRect.pivot = new Vector2(0.5f, 0.5f);
        previewRect.anchoredPosition = Vector2.zero;
        previewRect.localScale = Vector3.one;
        previewRect.localRotation = Quaternion.identity;
        previewRect.SetAsLastSibling();

        float scale = elapsed >= HivebloodPreviewSecondStageSeconds ? (2f / 3f) : (1f / 3f);
        Vector2 baseSize = targetRect.sizeDelta;
        previewRect.sizeDelta = baseSize * scale;

        previewImage.sprite = maskSprite != null ? maskSprite : targetImage.sprite;
        previewImage.color = GetNormalMaskFilledColor();
        previewImage.enabled = true;
        var previewGO = previewImage.gameObject;
        if (!previewGO.activeSelf)
        {
            previewGO.SetActive(true);
        }
    }

    private Image EnsureHivebloodPreviewMask()
    {
        if (hivebloodPreviewMask != null)
        {
            return hivebloodPreviewMask;
        }

        var go = new GameObject("HivebloodRegenPreview");
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.SetParent(transform, false);
        go.SetActive(false);

        hivebloodPreviewMask = go.AddComponent<Image>();
        hivebloodPreviewMask.preserveAspect = true;
        hivebloodPreviewMask.raycastTarget = false;
        return hivebloodPreviewMask;
    }

    private void HideHivebloodPreview()
    {
        if (hivebloodPreviewMask == null)
        {
            return;
        }

        var rect = hivebloodPreviewMask.rectTransform;
        if (rect != null && rect.parent != transform)
        {
            rect.SetParent(transform, false);
        }

        var go = hivebloodPreviewMask.gameObject;
        if (go.activeSelf)
        {
            go.SetActive(false);
        }
    }

    private IEnumerator LoseHealth(Image img, bool wasOvercharmed, bool wasLifeblood, bool hivebloodActive)
    {
        if (img == null) yield break;

        animatingMaskImages.Add(img);

        Color filledColor;
        Color flickerColor;
        Color emptyColor;

        if (wasLifeblood)
        {
            filledColor = lifebloodMaskColor;
            flickerColor = new Color(lifebloodMaskColor.r, lifebloodMaskColor.g, lifebloodMaskColor.b, Mathf.Clamp01(lifebloodMaskColor.a * 0.6f));
            emptyColor = lifebloodMissingColor;
        }
        else
        {
            filledColor = wasOvercharmed ? overcharmMaskColor : (hivebloodActive ? hivebloodMaskColor : Color.white);
            flickerColor = wasOvercharmed ? filledColor : Color.red;
            emptyColor = missingMaskColor;
        }

        for (int i = 0; i < 2; i++)
        {
            img.color = filledColor; yield return new WaitForSeconds(0.05f);
            img.color = flickerColor; yield return new WaitForSeconds(0.05f);
        }

        img.color = emptyColor;

        if (wasLifeblood)
        {
            HideLifebloodMaskIfDepleted(img);
        }

        animatingMaskImages.Remove(img);
    }

    private void HideLifebloodMaskIfDepleted(Image img)
    {
        if (img == null || maskImages == null)
        {
            return;
        }

        int index = Array.IndexOf(maskImages, img);
        if (index < 0)
        {
            return;
        }

        int normalMax = Mathf.Max(0, shadeMax);
        int lifebloodIndex = index - normalMax;
        if (lifebloodIndex < 0)
        {
            return;
        }

        int currentLifeblood = Mathf.Clamp(shadeLifeblood, 0, Mathf.Max(0, shadeLifebloodMax));
        if (lifebloodIndex >= currentLifeblood && img.gameObject.activeSelf)
        {
            img.gameObject.SetActive(false);
        }
    }

    private void RefreshSoul()
    {
        if (soulOrbRoot == null) return;
        float max = shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax);
        float silkVal = shadeSoulOverride ? Mathf.Clamp(shadeSoul, 0f, max) : (debugUseCustomSilk ? debugSilk : playerData.silk);
        float fill = Mathf.Clamp01(silkVal / max);
        if (soulImage != null) soulImage.fillAmount = fill;
    }

    private void HandleAssistVisibilityChange()
    {
        if (healthContainer == null)
        {
            return;
        }

        bool hasHealthToShow = (Mathf.Max(0, shadeMax) + Mathf.Max(0, shadeLifebloodMax)) > 0;
        bool shouldShow = !shadeAssistModeActive && hasHealthToShow;
        bool currentlyActive = healthContainer.activeSelf;
        if (currentlyActive != shouldShow)
        {
            healthContainer.SetActive(shouldShow);
            if (shouldShow)
            {
                pendingMaskRefresh = true;
            }
        }

        if (overcharmBackdrop != null)
        {
            bool showOvercharm = shouldShow && shadeOvercharmed;
            overcharmBackdrop.enabled = showOvercharm;
            overcharmBackdrop.gameObject.SetActive(showOvercharm);
        }
    }

    private void RebuildMasks()
    {
        if (shadeAssistModeActive)
        {
            pendingMaskRefresh = true;
            return;
        }

        if (healthContainer == null)
        {
            pendingMaskRefresh = true;
            return;
        }
        HideHivebloodPreview();
        foreach (var img in maskImages ?? Array.Empty<Image>()) if (img != null) Destroy(img.gameObject);
        animatingMaskImages.Clear();
        BuildMasks(healthContainer.GetComponent<RectTransform>(), GetUIScale());
        pendingMaskRefresh = false;
    }

    private void BuildMasks(RectTransform container, float uiScale)
    {
        int totalMasks = Mathf.Max(0, shadeMax) + Mathf.Max(0, shadeLifebloodMax);
        maskImages = new Image[totalMasks];
        Vector2 maskPixels = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33, 41);
        Vector2 maskSize = maskPixels * uiScale * MaskScale;

        float spacing = 6f * uiScale;
        overcharmMaskSize = maskSize;
        overcharmMaskSpacing = spacing;
        EnsureOvercharmBackdrop(container);
        float x = 0f;
        for (int i = 0; i < totalMasks; i++)
        {
            var m = new GameObject($"Mask{i}");
            m.transform.SetParent(container, false);
            var r = m.AddComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(1, 1);
            r.pivot = new Vector2(1, 1);
            r.sizeDelta = maskSize;
            r.anchoredPosition = new Vector2(-x, 0f);
            x += maskSize.x + spacing;

            var img = m.AddComponent<Image>();
            img.preserveAspect = true;
            img.sprite = maskSprite != null ? maskSprite : BuildMaskSprite();
            img.color = Color.white;
            maskImages[i] = img;
        }

        RefreshOvercharmBackdrop();
    }

    private void EnsureOvercharmBackdrop(RectTransform container)
    {
        if (container == null)
            return;

        if (overcharmBackdrop == null)
        {
            var go = new GameObject("OvercharmBackdrop");
            go.transform.SetParent(container, false);
            overcharmBackdrop = go.AddComponent<Image>();
            overcharmBackdrop.raycastTarget = false;
            overcharmBackdrop.preserveAspect = true;
        }

        if (overcharmBackdropSprite != null)
        {
            overcharmBackdrop.sprite = overcharmBackdropSprite;
            overcharmBackdrop.color = overcharmBackdropSpriteColor;
        }
        else if (overcharmBackdrop.sprite == null)
        {
            overcharmBackdrop.sprite = BuildMaskSprite();
            overcharmBackdrop.color = overcharmBackdropColor;
        }
        else
        {
            overcharmBackdrop.color = overcharmBackdropColor;
        }

        var rect = overcharmBackdrop.rectTransform;
        rect.localScale = new Vector3(OvercharmBackdropScale, OvercharmBackdropScale, 1f);
        rect.localRotation = Quaternion.Euler(0f, 0f, OvercharmBackdropRotation);
        overcharmBackdrop.transform.SetAsFirstSibling();
    }

    private void RefreshOvercharmBackdrop()
    {
        if (shadeAssistModeActive)
        {
            if (overcharmBackdrop != null)
            {
                overcharmBackdrop.enabled = false;
                overcharmBackdrop.gameObject.SetActive(false);
            }
            return;
        }

        if (healthContainer == null)
            return;

        EnsureOvercharmBackdrop(healthContainer.GetComponent<RectTransform>());
        if (overcharmBackdrop == null)
            return;

        int totalMasks = Mathf.Max(0, shadeMax) + Mathf.Max(0, shadeLifebloodMax);
        if (totalMasks <= 0 || maskImages == null || maskImages.Length == 0)
        {
            overcharmBackdrop.enabled = false;
            overcharmBackdrop.gameObject.SetActive(false);
            return;
        }

        Vector2 minBounds;
        Vector2 maxBounds;
        if (!TryCalculateMaskBounds(out minBounds, out maxBounds))
        {
            overcharmBackdrop.enabled = false;
            overcharmBackdrop.gameObject.SetActive(false);
            return;
        }

        float measuredWidth = Mathf.Max(0f, maxBounds.x - minBounds.x);
        float measuredHeight = Mathf.Max(0f, maxBounds.y - minBounds.y);
        float width = measuredWidth;
        float height = measuredHeight;

        if (overcharmMaskSize.x > 0f && OvercharmBackdropReferenceMaskCount > 0)
        {
            float constantWidth = overcharmMaskSize.x * OvercharmBackdropReferenceMaskCount;
            if (OvercharmBackdropReferenceMaskCount > 1 && overcharmMaskSpacing > 0f)
            {
                constantWidth += overcharmMaskSpacing * (OvercharmBackdropReferenceMaskCount - 1);
            }
            width = constantWidth;
        }

        if (overcharmMaskSize.y > 0f)
        {
            height = overcharmMaskSize.y;
        }

        if (width <= 0f || height <= 0f)
        {
            width = measuredWidth;
            height = measuredHeight;
        }

        if (width <= 0f || height <= 0f)
        {
            overcharmBackdrop.enabled = false;
            overcharmBackdrop.gameObject.SetActive(false);
            return;
        }

        var rect = overcharmBackdrop.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(width, height);

        float scale = OvercharmBackdropScale;
        rect.localScale = new Vector3(scale, scale, 1f);
        rect.localRotation = Quaternion.Euler(0f, 0f, OvercharmBackdropRotation);

        float rightEdge = Mathf.Min(0f, maxBounds.x);
        float topEdge = maxBounds.y;

        float horizontalOffset = width * OvercharmBackdropHorizontalOffsetFraction;
        if (horizontalOffset <= 0f && overcharmMaskSize.x > 0f)
        {
            horizontalOffset = overcharmMaskSize.x * 0.55f;
        }

        float verticalOffset = height * OvercharmBackdropVerticalOffsetFraction;
        if (verticalOffset <= 0f && overcharmMaskSize.y > 0f)
        {
            verticalOffset = overcharmMaskSize.y * 0.65f;
        }

        rect.anchoredPosition = new Vector2(rightEdge - horizontalOffset, topEdge - verticalOffset);
        if (overcharmBackdropSprite != null)
        {
            overcharmBackdrop.sprite = overcharmBackdropSprite;
            overcharmBackdrop.color = overcharmBackdropSpriteColor;
        }
        else if (overcharmBackdrop.sprite == null)
        {
            overcharmBackdrop.sprite = BuildMaskSprite();
            overcharmBackdrop.color = overcharmBackdropColor;
        }
        else
        {
            overcharmBackdrop.color = overcharmBackdropColor;
        }

        overcharmBackdrop.enabled = shadeOvercharmed;
        overcharmBackdrop.gameObject.SetActive(shadeOvercharmed);
        overcharmBackdrop.transform.SetAsFirstSibling();
    }

    private bool TryCalculateMaskBounds(out Vector2 min, out Vector2 max)
    {
        min = Vector2.zero;
        max = Vector2.zero;

        if (maskImages == null || maskImages.Length == 0)
        {
            return false;
        }

        bool hasMask = false;
        foreach (var image in maskImages)
        {
            if (image == null)
            {
                continue;
            }

            var rect = image.rectTransform;
            if (rect == null)
            {
                continue;
            }

            Vector2 anchored = rect.anchoredPosition;
            Vector2 size = rect.rect.size;
            Vector2 pivot = rect.pivot;

            float left = anchored.x - size.x * pivot.x;
            float right = anchored.x + size.x * (1f - pivot.x);
            float bottom = anchored.y - size.y * pivot.y;
            float top = anchored.y + size.y * (1f - pivot.y);

            var currentMin = new Vector2(left, bottom);
            var currentMax = new Vector2(right, top);

            if (!hasMask)
            {
                min = currentMin;
                max = currentMax;
                hasMask = true;
            }
            else
            {
                min = new Vector2(Mathf.Min(min.x, currentMin.x), Mathf.Min(min.y, currentMin.y));
                max = new Vector2(Mathf.Max(max.x, currentMax.x), Mathf.Max(max.y, currentMax.y));
            }
        }

        if (!hasMask)
        {
            return false;
        }

        return true;
    }
}


#nullable restore
