using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleHUD
{
    private void CreateUI()
    {
        // Canvas
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
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

        BuildMasks(hRect, uiScale);
    }

    private void RefreshHealth()
    {
        int cur = shadeHealth;
        if (maskImages == null) return;

        if (cur < previousShadeHealth)
        {
            int lost = previousShadeHealth - cur;
            if (suppressNextDamageSound)
            {
                suppressNextDamageSound = false;
            }
            else
            {
                TryPlayPinnedHurtSfx(lost);
            }
            for (int i = cur; i < previousShadeHealth && i < maskImages.Length; i++)
                StartCoroutine(LoseHealth(maskImages[i]));
        }
        else if (suppressNextDamageSound)
        {
            suppressNextDamageSound = false;
        }
        for (int i = 0; i < cur && i < maskImages.Length; i++)
        {
            maskImages[i].sprite = maskSprite != null ? maskSprite : maskImages[i].sprite;
            maskImages[i].color = Color.white;
        }
        for (int i = cur; i < maskImages.Length; i++)
            if (i >= previousShadeHealth) maskImages[i].color = missingMaskColor;
        previousShadeHealth = cur;
    }

    private IEnumerator LoseHealth(Image img)
    {
        if (img == null) yield break;
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white; yield return new WaitForSeconds(0.05f);
            img.color = Color.red; yield return new WaitForSeconds(0.05f);
        }
        img.color = missingMaskColor;
    }

    private void RefreshSoul()
    {
        if (soulOrbRoot == null) return;
        float max = shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax);
        float silkVal = shadeSoulOverride ? Mathf.Clamp(shadeSoul, 0f, max) : (debugUseCustomSilk ? debugSilk : playerData.silk);
        float fill = Mathf.Clamp01(silkVal / max);
        if (soulImage != null) soulImage.fillAmount = fill;
    }

    private void RebuildMasks()
    {
        if (healthContainer == null) return;
        foreach (var img in maskImages ?? Array.Empty<Image>()) if (img != null) Destroy(img.gameObject);
        BuildMasks(healthContainer.GetComponent<RectTransform>(), GetUIScale());
    }

    private void BuildMasks(RectTransform container, float uiScale)
    {
        maskImages = new Image[shadeMax];
        Vector2 maskPixels = maskSprite != null
            ? new Vector2(maskSprite.rect.width, maskSprite.rect.height)
            : new Vector2(33, 41);
        Vector2 maskSize = maskPixels * uiScale * MaskScale;

        float spacing = 6f * uiScale;
        float x = 0f;
        for (int i = 0; i < shadeMax; i++)
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
    }
}


