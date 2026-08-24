#nullable disable
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using BepInEx.Logging;
using GlobalEnums;
using LegacyoftheAbyss.Shade;

public static partial class ShadeSettingsMenu
{
    private static Sprite GetFallbackSprite(ref Sprite cache, string spriteName, bool sliced)
    {
        if (cache != null)
            return cache;

        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var colors = new Color32[size * size];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(colors);
        tex.Apply();
        tex.name = spriteName + "Tex";
        tex.hideFlags = HideFlags.HideAndDontSave;

        Vector4 border = sliced ? new Vector4(6f, 6f, 6f, 6f) : Vector4.zero;
        cache = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect, border);
        cache.name = spriteName;
        cache.hideFlags = HideFlags.HideAndDontSave;
        return cache;
    }

    /// <summary>
    /// The row every slider in this menu is built from: this file's structure, painted with the
    /// game's sprites where <see cref="CaptureSliderSkin"/> found them and with a hairline fallback
    /// where it did not.
    /// </summary>
    /// <summary>
    /// The look of the game's slider, lifted off one of its rows: which sprites it draws its track,
    /// its filled portion and its knob with, and how big they are.
    /// <para>
    /// This exists because cloning the row itself did not work, three attempts running. The row is
    /// not just a slider - it carries the game's own label and value readout, a layout group
    /// arranging them, and on some builds a toggle - and every attempt to delete those afterwards
    /// missed something, because each depended on a guess about the row's shape or about which of
    /// the game's two text stacks it uses. Taking the four sprites and rebuilding the row here needs
    /// no such guess: the structure is this file's, so nothing can come along for the ride.
    /// </para>
    /// </summary>
    private struct SliderSkin
    {
        public bool Valid;
        public bool HasTrack;
        public Sprite TrackSprite;
        public Image.Type TrackType;
        public Color TrackColor;
        public float TrackHeight;
        public bool HasFill;
        public Sprite FillSprite;
        public Image.Type FillType;
        public Color FillColor;
        public Sprite HandleSprite;
        public Color HandleColor;
        public Vector2 HandleSize;

        /// <summary>
        /// How far the game turns its handle sprite. The art is authored pointing sideways and the
        /// row rotates it to point down at the track - copy the sprite without this and you get a
        /// left-pointing arrowhead sitting in the line instead of a chevron above it.
        /// </summary>
        public float HandleRotationZ;

        /// <summary>
        /// The game's own handle rect, already converted into this menu's units.
        /// <para>
        /// Sized from the rect rather than from the sprite's proportions, because the sprite is
        /// mostly transparent padding: the visible chevron is about 15x17 inside a 58x122 rect. Fit
        /// the rect to the art's aspect and the art comes out a fraction of the size it should be,
        /// which is what "extremely small" meant.
        /// </para>
        /// </summary>
        public float ScaleRatio;

        public string Describe()
        {
            if (!Valid)
            {
                return "no game slider skin";
            }

            return FormattableString.Invariant(
                $"track={SpriteName(TrackSprite)}@{TrackHeight:0.#}/{(HasTrack ? "solid" : "fallback")} fill={SpriteName(FillSprite)}/{(HasFill ? "solid" : "fallback")} handle={SpriteName(HandleSprite)}@{HandleSize.x:0.#}x{HandleSize.y:0.#} rot={HandleRotationZ:0.#} scale={ScaleRatio:0.000}");
        }

        private static string SpriteName(Sprite sprite) => sprite != null ? sprite.name : "<none>";
    }

    private static SliderSkin gameSliderSkin;

    /// <summary>
    /// How thick the game draws its slider line, in its own units. Used only when the track's own
    /// rect cannot be measured - an inactive screen may never have had a layout pass.
    /// </summary>
    private const float GameTrackHeight = 4f;

    /// <summary>
    /// Reads <see cref="SliderSkin"/> off a game slider row without instantiating anything. Every
    /// piece is optional - whatever is missing keeps the hairline fallback this file draws.
    /// </summary>
    private static SliderSkin CaptureSliderSkin(GameObject template, GameObject ourScreen)
    {
        var skin = new SliderSkin
        {
            TrackType = Image.Type.Sliced,
            TrackColor = new Color(0.86f, 0.85f, 0.80f, 0.5f),
            TrackHeight = 3f,
            FillType = Image.Type.Sliced,
            FillColor = new Color(0.96f, 0.95f, 0.90f, 0.95f),
            HandleColor = new Color(1f, 0.98f, 0.92f, 1f),
            HandleSize = new Vector2(8f, 26f),
            ScaleRatio = 1f
        };

        if (template == null)
        {
            return skin;
        }

        try
        {
            var slider = template.GetComponentInChildren<Slider>(true);
            if (slider == null)
            {
                return skin;
            }

            // The game's option screens and this mod's clones of the pause menu are drawn at
            // different scales, so a size copied straight across renders at the wrong size. Measure
            // the ratio between the two rather than assuming either.
            skin.ScaleRatio = MeasureScaleRatio(slider.transform, ourScreen);
            skin.TrackHeight = GameTrackHeight * skin.ScaleRatio;

            var fillImage = slider.fillRect != null ? slider.fillRect.GetComponent<Image>() : null;
            var handleImage = slider.handleRect != null ? slider.handleRect.GetComponent<Image>() : null;

            // The track has no property on Slider to read it from, so it is found by elimination:
            // an Image under the slider that is neither the fill nor the handle, nor an ancestor of
            // either (those are the Fill Area / Handle Slide Area containers).
            Image trackImage = null;
            foreach (var image in slider.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image == fillImage || image == handleImage)
                {
                    continue;
                }

                if (slider.fillRect != null && slider.fillRect.IsChildOf(image.transform))
                {
                    continue;
                }

                if (slider.handleRect != null && slider.handleRect.IsChildOf(image.transform))
                {
                    continue;
                }

                trackImage = image;
                break;
            }

            // A null sprite is not a miss here: the game draws both the track and the filled part
            // of it as plain white quads, which is an Image with no sprite at all. Rejecting those
            // is why two rounds of reports came back saying "track=<none> fill=<none>" while the
            // game's own line was plainly visible.
            if (trackImage != null)
            {
                skin.HasTrack = true;
                skin.TrackSprite = trackImage.sprite;
                skin.TrackType = trackImage.type;
                skin.TrackColor = trackImage.color;
                float height = MeasuredHeight(trackImage.rectTransform) * skin.ScaleRatio;
                if (height > 0.5f)
                {
                    skin.TrackHeight = height;
                }
            }

            if (fillImage != null)
            {
                skin.HasFill = true;
                skin.FillSprite = fillImage.sprite;
                skin.FillType = fillImage.type;
                skin.FillColor = fillImage.color;
            }

            if (handleImage != null)
            {
                skin.HandleSprite = handleImage.sprite;
                skin.HandleColor = handleImage.color;
                skin.HandleRotationZ = handleImage.rectTransform.localEulerAngles.z;

                var size = MeasuredSize(handleImage.rectTransform) * skin.ScaleRatio;
                if (size.x > 0.5f && size.y > 0.5f)
                {
                    skin.HandleSize = size;
                }
            }

            skin.Valid = skin.HasTrack || skin.HasFill || skin.HandleSprite != null;
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not read the game slider's look: {e}");
        }

        return skin;
    }

    /// <summary>
    /// How much bigger something has to be drawn here to look the size it does over there. One when
    /// either transform cannot be read, so a failure leaves sizes alone rather than collapsing them.
    /// </summary>
    private static float MeasureScaleRatio(Transform theirs, GameObject ourScreen)
    {
        try
        {
            if (theirs == null || ourScreen == null)
            {
                return 1f;
            }

            float theirScale = Mathf.Abs(theirs.lossyScale.x);
            float ourScale = Mathf.Abs(ourScreen.transform.lossyScale.x);
            if (theirScale < 0.0001f || ourScale < 0.0001f)
            {
                return 1f;
            }

            return Mathf.Clamp(theirScale / ourScale, 0.1f, 10f);
        }
        catch
        {
            return 1f;
        }
    }

    /// <summary>
    /// A RectTransform's height, preferring its laid-out rect and falling back to sizeDelta. An
    /// inactive screen may never have had a layout pass, in which case rect is zero and sizeDelta is
    /// the only thing that carries the authored size.
    /// </summary>
    private static float MeasuredHeight(RectTransform rect)
    {
        if (rect == null)
        {
            return 0f;
        }

        float height = rect.rect.height;
        return height > 0.5f ? height : rect.sizeDelta.y;
    }

    private static Vector2 MeasuredSize(RectTransform rect)
    {
        if (rect == null)
        {
            return Vector2.zero;
        }

        var size = rect.rect.size;
        if (size.x <= 0.5f || size.y <= 0.5f)
        {
            size = rect.sizeDelta;
        }

        return size;
    }

    private static GameObject CreateDefaultSliderTemplate()
    {
        var root = new GameObject("DefaultSlider");
        root.hideFlags = HideFlags.HideAndDontSave;
        root.AddComponent<RectTransform>();
        var selectable = root.AddComponent<MenuSelectable>();

        var sliderGo = new GameObject("Slider");
        sliderGo.transform.SetParent(root.transform, false);
        var sliderRt = sliderGo.AddComponent<RectTransform>();
        sliderRt.sizeDelta = new Vector2(160f, 20f);

        var background = new GameObject("Background");
        background.transform.SetParent(sliderGo.transform, false);
        var bgImage = background.AddComponent<Image>();
        var uiSprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsSliderBg", true);
        var skin = gameSliderSkin;
        float trackHalf = Mathf.Max(1f, skin.TrackHeight * 0.5f);
        bgImage.sprite = skin.TrackSprite != null ? skin.TrackSprite : uiSprite;
        bgImage.type = skin.TrackSprite != null ? skin.TrackType : Image.Type.Sliced;
        // A hairline centred in the row, not a full-height plate.
        bgImage.color = skin.TrackColor;
        var backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.anchorMin = new Vector2(0f, 0.5f);
        backgroundRt.anchorMax = new Vector2(1f, 0.5f);
        backgroundRt.offsetMin = new Vector2(0f, -trackHalf);
        backgroundRt.offsetMax = new Vector2(0f, trackHalf);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0f, 0.5f);
        fillAreaRt.anchorMax = new Vector2(1f, 0.5f);
        float knobWidth = skin.HandleSize.x;
        float knobHeight = skin.HandleSize.y;
        // A quarter turn either way swaps what the knob occupies horizontally and vertically.
        bool knobTurned = Mathf.Abs(Mathf.Sin(skin.HandleRotationZ * Mathf.Deg2Rad)) > 0.5f;
        float knobFootprint = knobTurned ? knobHeight : knobWidth;
        float knobHalf = knobFootprint * 0.5f;

        // Inset by half a knob at each end, matching the range the handle's centre actually travels.
        // Insetting only the right, and by a whole knob, is why the chevron did not line up with the
        // end of the filled part of the track.
        fillAreaRt.offsetMin = new Vector2(knobHalf, -trackHalf);
        fillAreaRt.offsetMax = new Vector2(-knobHalf, trackHalf);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.sprite = skin.FillSprite != null ? skin.FillSprite : uiSprite;
        fillImg.type = skin.FillSprite != null ? skin.FillType : Image.Type.Sliced;
        fillImg.color = skin.FillColor;
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(1f, 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderGo.transform, false);
        var handleAreaRt = handleArea.AddComponent<RectTransform>();
        handleAreaRt.anchorMin = new Vector2(0f, 0f);
        handleAreaRt.anchorMax = new Vector2(1f, 1f);
        handleAreaRt.sizeDelta = new Vector2(-knobFootprint, 0f);
        handleAreaRt.anchoredPosition = Vector2.zero;

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        var knobSprite = GetFallbackSprite(ref fallbackKnobSprite, "ShadeSettingsSliderKnob", false);
        handleImg.sprite = skin.HandleSprite != null ? skin.HandleSprite : knobSprite;
        handleImg.color = skin.HandleColor;
        var handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(knobWidth, knobHeight);
        handleRt.localRotation = Quaternion.Euler(0f, 0f, skin.HandleRotationZ);
        // Centred on the track, exactly as the game has it. The chevron appears above the line
        // because the art sits near one end of a mostly-empty sprite, not because the rect is
        // offset - nudging the rect as well pushed it into space.
        handleRt.anchoredPosition = Vector2.zero;

        var slider = sliderGo.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.transition = Selectable.Transition.ColorTint;
        var colors = slider.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        slider.colors = colors;

        root.SetActive(false);
        return root;
    }

    private static MenuSelectable CreateDefaultToggleTemplate()
    {
        var root = new GameObject("DefaultToggle");
        root.hideFlags = HideFlags.HideAndDontSave;
        root.AddComponent<RectTransform>();
        var selectable = root.AddComponent<MenuSelectable>();

        var toggleGo = new GameObject("Toggle");
        toggleGo.transform.SetParent(root.transform, false);
        var toggleRt = toggleGo.AddComponent<RectTransform>();
        toggleRt.sizeDelta = new Vector2(20f, 20f);

        var background = new GameObject("Background");
        background.transform.SetParent(toggleGo.transform, false);
        var bgImage = background.AddComponent<Image>();
        var uiSprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsToggleBg", true);
        bgImage.sprite = uiSprite;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
        var backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.anchorMin = Vector2.zero;
        backgroundRt.anchorMax = Vector2.one;
        backgroundRt.offsetMin = Vector2.zero;
        backgroundRt.offsetMax = Vector2.zero;

        var checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        var checkImg = checkmark.AddComponent<Image>();
        var checkSprite = GetFallbackSprite(ref fallbackCheckSprite, "ShadeSettingsToggleCheck", false);
        checkImg.sprite = checkSprite;
        checkImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        var checkRt = checkmark.GetComponent<RectTransform>();
        checkRt.anchorMin = new Vector2(0.2f, 0.2f);
        checkRt.anchorMax = new Vector2(0.8f, 0.8f);
        checkRt.offsetMin = Vector2.zero;
        checkRt.offsetMax = Vector2.zero;

        var toggle = toggleGo.AddComponent<Toggle>();
        toggle.graphic = checkImg;
        toggle.targetGraphic = bgImage;
        toggle.transition = Selectable.Transition.ColorTint;
        var colors = toggle.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        toggle.colors = colors;

        root.SetActive(false);
        return selectable;
    }

    private static Animator CloneAnimator(Animator source, Transform parent, string nameSuffix)
    {
        if (source == null || parent == null)
            return null;

        var clone = Object.Instantiate(source.gameObject, parent, false);
        clone.name = string.IsNullOrEmpty(nameSuffix) ? source.gameObject.name : nameSuffix;
        var layout = clone.GetComponent<LayoutElement>() ?? clone.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;
        layout.minWidth = 0f;
        layout.preferredWidth = 0f;
        layout.flexibleWidth = 0f;
        layout.minHeight = 0f;
        layout.preferredHeight = 0f;
        layout.flexibleHeight = 0f;

        var sourceRect = source.GetComponent<RectTransform>();
        var cloneRect = clone.GetComponent<RectTransform>();
        if (sourceRect != null && cloneRect != null)
        {
            cloneRect.anchorMin = sourceRect.anchorMin;
            cloneRect.anchorMax = sourceRect.anchorMax;
            cloneRect.pivot = sourceRect.pivot;
            cloneRect.sizeDelta = sourceRect.sizeDelta;
            cloneRect.anchoredPosition = sourceRect.anchoredPosition;
            cloneRect.anchoredPosition3D = sourceRect.anchoredPosition3D;
            cloneRect.localScale = sourceRect.localScale;
            cloneRect.localRotation = sourceRect.localRotation;
        }

        foreach (var graphic in clone.GetComponentsInChildren<Graphic>(true))
        {
            if (graphic != null)
                graphic.raycastTarget = false;
        }

        var animator = clone.GetComponent<Animator>();
        if (animator != null)
        {
            try
            {
                animator.ResetTrigger("show");
                animator.ResetTrigger("hide");
                animator.Update(0f);
            }
            catch
            {
            }
        }
        return animator;
    }

    private static GameObject CreateRowHighlight(Transform parent, MenuButton buttonTemplate, float height, string label, out Animator leftCursor, out Animator rightCursor, out Animator selectHighlight)
    {
        leftCursor = null;
        rightCursor = null;
        selectHighlight = null;

        if (parent == null)
            return null;

        string baseName = string.IsNullOrEmpty(label) ? "Highlight" : label.Replace(" ", string.Empty) + "Highlight";
        var highlight = new GameObject(baseName);
        var rect = highlight.AddComponent<RectTransform>();
        highlight.transform.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.anchoredPosition3D = Vector3.zero;

        var layout = highlight.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;
        layout.minHeight = height;
        layout.preferredHeight = height;
        layout.flexibleHeight = 0f;
        layout.minWidth = 0f;
        layout.preferredWidth = 0f;
        layout.flexibleWidth = 0f;

        if (buttonTemplate != null)
        {
            leftCursor = CloneAnimator(buttonTemplate.leftCursor, highlight.transform, baseName + "Left");
            rightCursor = CloneAnimator(buttonTemplate.rightCursor, highlight.transform, baseName + "Right");
            selectHighlight = CloneAnimator(buttonTemplate.selectHighlight, highlight.transform, baseName + "Center");
        }

        if (leftCursor == null && rightCursor == null && selectHighlight == null)
        {
            var image = highlight.AddComponent<Image>();
            image.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsButtonBg", true);
            image.type = Image.Type.Sliced;
            image.color = ButtonHighlightColor;
            image.raycastTarget = false;
            highlight.SetActive(false);
        }

        highlight.transform.SetAsFirstSibling();
        return highlight;
    }

    private static List<ShadowStyle> CaptureShadowStyles(Graphic graphic)
    {
        var list = new List<ShadowStyle>();
        foreach (var shadow in graphic.GetComponents<Shadow>())
        {
            list.Add(new ShadowStyle
            {
                Type = shadow.GetType(),
                EffectColor = shadow.effectColor,
                EffectDistance = shadow.effectDistance,
                UseGraphicAlpha = shadow.useGraphicAlpha
            });
        }
        return list;
    }

    private static TextStyle CaptureTextStyle(Text text)
    {
        return new TextStyle
        {
            Font = text.font,
            FontSize = text.fontSize,
            FontStyle = text.fontStyle,
            Alignment = text.alignment,
            Color = text.color,
            RichText = text.supportRichText,
            BestFit = text.resizeTextForBestFit,
            BestFitMin = text.resizeTextMinSize,
            BestFitMax = text.resizeTextMaxSize,
            LineSpacing = text.lineSpacing,
            AlignByGeometry = text.alignByGeometry,
            HorizontalOverflow = text.horizontalOverflow,
            VerticalOverflow = text.verticalOverflow,
            Shadows = CaptureShadowStyles(text)
        };
    }

    private static void ClearAndApplyShadows(Graphic graphic, List<ShadowStyle> styles)
    {
        foreach (var shadow in graphic.GetComponents<Shadow>())
            Object.DestroyImmediate(shadow);

        if (styles == null)
            return;

        foreach (var style in styles)
        {
            if (style.Type == null)
                continue;
            if (!(graphic.gameObject.AddComponent(style.Type) is Shadow newShadow))
                continue;
            newShadow.effectColor = style.EffectColor;
            newShadow.effectDistance = style.EffectDistance;
            newShadow.useGraphicAlpha = style.UseGraphicAlpha;
        }
    }

    private static void ApplyTextStyle(Text text, TextStyle? style, TextAnchor defaultAlignment, Color defaultColor)
    {
        var resolved = style.GetValueOrDefault();
        bool hasStyle = style.HasValue;

        var fontToUse = resolved.Font != null ? resolved.Font : fallbackFont;
        if (fontToUse == null)
            fontToUse = Resources.GetBuiltinResource<Font>("Arial.ttf");

        text.font = fontToUse;
        text.color = hasStyle ? resolved.Color : defaultColor;
        text.enabled = true;
        text.raycastTarget = false;
        text.alignment = hasStyle ? resolved.Alignment : defaultAlignment;
        text.fontSize = hasStyle && resolved.FontSize > 0 ? resolved.FontSize : 24;
        text.fontStyle = hasStyle ? resolved.FontStyle : FontStyle.Normal;
        text.supportRichText = hasStyle ? resolved.RichText : true;
        text.lineSpacing = hasStyle ? resolved.LineSpacing : 1f;
        text.resizeTextForBestFit = hasStyle && resolved.BestFit;
        text.resizeTextMinSize = hasStyle && resolved.BestFit ? resolved.BestFitMin : 10;
        text.resizeTextMaxSize = hasStyle && resolved.BestFit ? resolved.BestFitMax : 40;
        text.alignByGeometry = hasStyle ? resolved.AlignByGeometry : false;
        text.horizontalOverflow = hasStyle ? resolved.HorizontalOverflow : HorizontalWrapMode.Overflow;
        text.verticalOverflow = hasStyle ? resolved.VerticalOverflow : VerticalWrapMode.Overflow;

        // Only the fallback size gets scaled up. MenuFontScale exists to lift this file's own
        // default (24) to something readable at the size these screens draw at; a size captured from
        // a real game text is already right for this canvas, and multiplying it again is how every
        // label on the Difficulty screen ended up three times too large the moment a game slider
        // became the template.
        if (!hasStyle && MenuFontScale > 1f)
        {
            text.fontSize = Mathf.Max(1, Mathf.RoundToInt(text.fontSize * MenuFontScale));
            if (text.resizeTextForBestFit)
            {
                int originalMin = text.resizeTextMinSize;
                int originalMax = text.resizeTextMaxSize;
                text.resizeTextMinSize = Mathf.Max(1, Mathf.RoundToInt(originalMin * MenuFontScale));
                text.resizeTextMaxSize = Mathf.Max(text.resizeTextMinSize, Mathf.RoundToInt(originalMax * MenuFontScale));
            }
        }

        if (text.color.a <= 0.01f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        ClearAndApplyShadows(text, hasStyle ? resolved.Shadows : null);
    }

    private static void ScaleUnityText(Text text, float scale)
    {
        if (text == null || scale <= 0f || Mathf.Approximately(scale, 1f))
            return;

        int adjustedSize = Mathf.Max(1, Mathf.RoundToInt(text.fontSize * scale));
        text.fontSize = adjustedSize;

        if (text.resizeTextForBestFit)
        {
            int min = Mathf.Max(1, Mathf.RoundToInt(text.resizeTextMinSize * scale));
            int max = Mathf.Max(min, Mathf.RoundToInt(text.resizeTextMaxSize * scale));
            text.resizeTextMinSize = min;
            text.resizeTextMaxSize = max;
        }
    }

    private static void ScaleTextElements(GameObject root, float scale)
    {
        if (root == null || scale <= 0f || Mathf.Approximately(scale, 1f))
            return;

        foreach (var text in root.GetComponentsInChildren<Text>(true))
        {
            ScaleUnityText(text, scale);
        }

        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType == null)
            return;

        foreach (var tmp in root.GetComponentsInChildren(tmpType, true))
        {
            try
            {
                var fontSizeProp = tmpType.GetProperty("fontSize");
                if (fontSizeProp != null)
                {
                    float currentSize = Convert.ToSingle(fontSizeProp.GetValue(tmp, null));
                    fontSizeProp.SetValue(tmp, currentSize * scale, null);
                }

                var autoSizeProp = tmpType.GetProperty("enableAutoSizing");
                if (autoSizeProp != null && autoSizeProp.GetValue(tmp, null) is bool autoSize && autoSize)
                {
                    var minProp = tmpType.GetProperty("fontSizeMin");
                    var maxProp = tmpType.GetProperty("fontSizeMax");
                    if (minProp != null)
                    {
                        float min = Convert.ToSingle(minProp.GetValue(tmp, null));
                        minProp.SetValue(tmp, min * scale, null);
                    }
                    if (maxProp != null)
                    {
                        float max = Convert.ToSingle(maxProp.GetValue(tmp, null));
                        maxProp.SetValue(tmp, max * scale, null);
                    }
                }
            }
            catch
            {
            }
        }
    }

    private static void ApplyButtonColors(Selectable selectable)
    {
        if (selectable == null)
            return;

        selectable.transition = Selectable.Transition.ColorTint;
        var colors = selectable.colors;
        colors.normalColor = ButtonNormalColor;
        colors.highlightedColor = ButtonHighlightColor;
        colors.selectedColor = ButtonHighlightColor;
        colors.pressedColor = ButtonPressedColor;
        colors.disabledColor = ButtonDisabledColor;
        colors.colorMultiplier = 1f;
        selectable.colors = colors;

        if (selectable.targetGraphic != null)
        {
            selectable.targetGraphic.color = ButtonNormalColor;
            selectable.targetGraphic.raycastTarget = true;
        }
    }

    private static void CacheTextStyles(GameObject sliderTemplate, MenuSelectable toggleTemplate)
    {
        sliderLabelStyle = null;
        sliderValueStyle = null;
        toggleLabelStyle = null;
        fallbackFont = null;

        if (sliderTemplate != null)
        {
            foreach (var text in sliderTemplate.GetComponentsInChildren<Text>(true))
            {
                if (text == null)
                    continue;
                var hasAuto = text.GetComponent<AutoLocalizeTextUI>() != null;
                if (hasAuto)
                {
                    if (!sliderLabelStyle.HasValue)
                    {
                        sliderLabelStyle = CaptureTextStyle(text);
                        if (text.font != null)
                            fallbackFont ??= text.font;
                    }
                }
                else
                {
                    if (!sliderValueStyle.HasValue)
                    {
                        sliderValueStyle = CaptureTextStyle(text);
                        if (text.font != null)
                            fallbackFont ??= text.font;
                    }
                }
            }
        }

        if (toggleTemplate != null)
        {
            foreach (var text in toggleTemplate.GetComponentsInChildren<Text>(true))
            {
                if (text == null)
                    continue;
                if (!toggleLabelStyle.HasValue)
                {
                    toggleLabelStyle = CaptureTextStyle(text);
                    if (text.font != null)
                        fallbackFont ??= text.font;
                }
            }
        }

        if (fallbackFont == null)
            fallbackFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void SanitizeSelectableHierarchy(GameObject root)
    {
        if (root == null)
            return;

        foreach (var comp in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (comp == null)
                continue;
            if (comp is CancelRouter || comp is SliderMenuDriver)
                continue;
            var type = comp.GetType();
            string ns = type.Namespace ?? string.Empty;
            if (ns.StartsWith("UnityEngine"))
                continue;
            Object.DestroyImmediate(comp);
        }

        foreach (var group in root.GetComponentsInChildren<CanvasGroup>(true))
        {
            if (group == null)
                continue;
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        var selectable = root.GetComponent<MenuSelectable>();
        if (selectable != null && selectable.targetGraphic == null)
        {
            var graphic = root.GetComponent<Graphic>();
            if (graphic == null)
                graphic = root.GetComponentInChildren<Graphic>(true);
            if (graphic != null)
            {
                selectable.targetGraphic = graphic;
                graphic.raycastTarget = true;
            }
        }
    }

    private static void SetAutomaticNavigation(Selectable selectable)
    {
        if (selectable == null)
            return;

        var navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        navigation.wrapAround = false;
        selectable.navigation = navigation;
    }

    private static void ConfigureHorizontalNavigation(IList<MenuButton> buttons)
    {
        if (buttons == null || buttons.Count == 0)
            return;

        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            if (button == null)
                continue;

            var navigation = button.navigation;
            var up = navigation.selectOnUp;
            var down = navigation.selectOnDown;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.wrapAround = false;
            navigation.selectOnLeft = i > 0 ? buttons[i - 1] : navigation.selectOnLeft;
            navigation.selectOnRight = i < buttons.Count - 1 ? buttons[i + 1] : navigation.selectOnRight;
            navigation.selectOnUp = up;
            navigation.selectOnDown = down;
            button.navigation = navigation;
        }
    }

    private static Font FindFontInObject(GameObject root)
    {
        if (root == null)
            return null;
        foreach (var text in root.GetComponentsInChildren<Text>(true))
        {
            if (text != null && text.font != null)
                return text.font;
        }
        return null;
    }

    private static void ApplyPreferredFont(Font font)
    {
        if (font == null)
            return;
        fallbackFont = font;
        if (sliderLabelStyle.HasValue)
        {
            var style = sliderLabelStyle.Value;
            style.Font = font;
            sliderLabelStyle = style;
        }
        if (sliderValueStyle.HasValue)
        {
            var style = sliderValueStyle.Value;
            style.Font = font;
            sliderValueStyle = style;
        }
        if (toggleLabelStyle.HasValue)
        {
            var style = toggleLabelStyle.Value;
            style.Font = font;
            toggleLabelStyle = style;
        }
    }

    private static float SnapSliderValue(float value, float min, float max, bool whole)
    {
        value = Mathf.Clamp(value, min, max);
        if (whole)
        {
            var rounded = Mathf.Round(value);
            if (rounded < min)
                rounded = min;
            if (rounded > max)
                rounded = max;
            return rounded;
        }

        float snapped = Mathf.Round((value - min) / FractionalSliderStep) * FractionalSliderStep + min;
        snapped = Mathf.Clamp(snapped, min, max);
        float multiplier = 1f / FractionalSliderStep;
        snapped = Mathf.Round(snapped * multiplier) / multiplier;
        return snapped;
    }

    private static string FormatSliderValue(float value, bool whole)
    {
        return whole ? Mathf.RoundToInt(value).ToString() : value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Column widths for one slider row. The Difficulty screen puts sliders inside half-width
    /// panels, where the full-width defaults do not fit, so the three columns are parameters rather
    /// than the constants they used to be.
    /// </summary>
    private struct SliderRowMetrics
    {
        public float LabelWidth;
        public float SliderWidth;
        public float ValueWidth;
        public float Spacing;
        public float Height;

        /// <summary>
        /// True when <see cref="Height"/> is the row's height rather than its minimum. The default
        /// metrics take the taller of the cloned template and <see cref="SliderRowHeight"/>, which is
        /// right for a screen that stacks rows down a list, and wrong for the Difficulty screen's
        /// panels, where the rows sit at computed offsets and one taller than its slot overlaps the
        /// row below it.
        /// </summary>
        public bool FixedHeight;

        public static SliderRowMetrics Default => new SliderRowMetrics
        {
            LabelWidth = LabelColumnWidth,
            SliderWidth = 800f,
            ValueWidth = ValueColumnWidth,
            Spacing = 64f,
            Height = SliderRowHeight
        };

        /// <summary>Proportional columns for a row that has to live inside <paramref name="rowWidth"/>.</summary>
        public static SliderRowMetrics ForWidth(float rowWidth, float height)
        {
            float spacing = Mathf.Clamp(rowWidth * 0.04f, 16f, 48f);
            float label = Mathf.Clamp(rowWidth * 0.36f, 160f, 460f);
            float value = Mathf.Clamp(rowWidth * 0.11f, 64f, ValueColumnWidth);
            float slider = Mathf.Max(80f, rowWidth - label - value - spacing * 2f);
            return new SliderRowMetrics
            {
                LabelWidth = label,
                SliderWidth = slider,
                ValueWidth = value,
                Spacing = spacing,
                Height = height,
                FixedHeight = true
            };
        }
    }

    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, bool whole = false)
        => CreateSlider(parent, template, buttonTemplate, label, min, max, value, onChange, cancelTarget, SliderRowMetrics.Default, out _, whole);

    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, SliderRowMetrics metrics, bool whole = false)
        => CreateSlider(parent, template, buttonTemplate, label, min, max, value, onChange, cancelTarget, metrics, out _, whole);

    /// <param name="rowTransform">
    /// The row this built, for callers that position rows themselves. Handed back rather than left
    /// to be derived from the returned selectable's parent: where the selectable lives depends on
    /// whether the cloned template carried a MenuSelectable of its own, and a caller that assumed
    /// one arrangement moved the whole panel instead of the row when it turned out to be the other.
    /// </param>
    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, SliderRowMetrics metrics, out RectTransform rowTransform, bool whole = false)
    {
        rowTransform = null;
        // container row stretching full width
        var row = new GameObject(label + "Row");
        var rowRect = row.AddComponent<RectTransform>();
        rowRect.SetParent(parent, false);
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        var hLayout = row.AddComponent<HorizontalLayoutGroup>();
        hLayout.childControlHeight = true;
        hLayout.childControlWidth = true;
        hLayout.childForceExpandHeight = false;
        hLayout.childForceExpandWidth = false;
        hLayout.spacing = metrics.Spacing;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        ApplyTextStyle(labelTxt, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        labelTxt.raycastTarget = false;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = metrics.LabelWidth;
        labelLe.preferredWidth = metrics.LabelWidth;
        labelLe.flexibleWidth = 0f;

        // slider instance
        var go = Object.Instantiate(template, row.transform, false);
        go.SetActive(true);
        // The template is hidden and flagged DontSave so it does not clutter the hierarchy or get
        // unloaded mid-build; a row cloned from it wants neither.
        go.hideFlags = HideFlags.None;
        go.name = label + "Slider";

        var slider = go.GetComponentInChildren<Slider>(true);
        if (slider == null)
        {
            LogMenuError($"Created slider '{label}' missing Slider component");
            Object.DestroyImmediate(row);
            return null;
        }

        SanitizeSelectableHierarchy(go);
        rowTransform = rowRect;
        Object.DestroyImmediate(slider.GetComponent<MenuAudioSlider>());
        Object.DestroyImmediate(slider.GetComponent<MenuPreventDeselect>());
        slider.onValueChanged.RemoveAllListeners();
        slider.interactable = true;
        slider.enabled = true;
        // No SliderRightStickInput. The game wires that component's slider reference in the editor
        // through OnValidate, which never runs at runtime, so one added here has a null slider and
        // throws the moment it decides the stick is pushed. It is also not wanted: it snaps the
        // value to its minimum or maximum rather than stepping, and SliderMenuDriver already handles
        // left/right on the row.
        Object.DestroyImmediate(slider.GetComponent<SliderRightStickInput>());
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var sliderLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        sliderLe.minWidth = metrics.SliderWidth;
        sliderLe.preferredWidth = metrics.SliderWidth;
        sliderLe.flexibleWidth = 1f;
        var sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, metrics.SliderWidth);

        // value text to the right of slider
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(row.transform, false);
        var valueTxt = valueObj.AddComponent<Text>();
        ApplyTextStyle(valueTxt, sliderValueStyle, TextAnchor.MiddleRight, Color.white);
        valueTxt.raycastTarget = false;
        var valueLe = valueObj.AddComponent<LayoutElement>();
        valueLe.minWidth = metrics.ValueWidth;
        valueLe.preferredWidth = metrics.ValueWidth;
        valueLe.flexibleWidth = 0f;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = whole;
        float initialValue = SnapSliderValue(value, min, max, whole);
        slider.SetValueWithoutNotify(initialValue);
        valueTxt.text = FormatSliderValue(initialValue, whole);
        if (!Mathf.Approximately(initialValue, value))
        {
            try
            {
                onChange.Invoke(initialValue);
            }
            catch (Exception e)
            {
                LogMenuWarning($"Error normalizing slider '{label}' value: {e}");
            }
        }
        slider.onValueChanged.AddListener(v =>
        {
            var snapped = SnapSliderValue(v, min, max, whole);
            if (!Mathf.Approximately(snapped, v))
                slider.SetValueWithoutNotify(snapped);
            onChange.Invoke(snapped);
            valueTxt.text = FormatSliderValue(snapped, whole);
        });

        SetAutomaticNavigation(slider);

        var rowLe = row.AddComponent<LayoutElement>();
        float baseHeight = 0f;
        if (rect != null)
        {
            baseHeight = rect.rect.height;
            if (baseHeight <= 0f)
                baseHeight = rect.sizeDelta.y;
        }
        if (metrics.FixedHeight || baseHeight <= 0f)
            baseHeight = metrics.Height;
        else
            baseHeight = Mathf.Max(baseHeight, metrics.Height);
        rowLe.preferredHeight = baseHeight;
        rowLe.minHeight = baseHeight;
        rowRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        if (rect != null)
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        // Told to the LayoutElement as well as stamped on the RectTransform. hLayout has
        // childControlHeight on, so it recomputes this child's height on every rebuild from
        // whatever the LayoutElement reports - and with no height reported that is zero, which
        // silently undoes the size set on the line above the first time anything triggers one.
        sliderLe.minHeight = baseHeight;
        sliderLe.preferredHeight = baseHeight;

        // The row's selectable is normally the one the cloned template carries. The game's volume
        // rows put the Slider itself on the object we clone and have no MenuSelectable of their own,
        // so one goes on the row instead - not on the clone, because a second Selectable beside the
        // Slider makes both reachable and the highlight lands on whichever the EventSystem picks.
        var selectable = go.GetComponent<MenuSelectable>();
        GameObject driverHost;
        if (selectable != null)
        {
            driverHost = go;
        }
        else
        {
            selectable = row.AddComponent<MenuSelectable>();
            driverHost = row;
            // The Slider stays interactive for the mouse but is taken out of keyboard/controller
            // navigation, so Up/Down cannot stop on it instead of on the row.
            var sliderNav = slider.navigation;
            sliderNav.mode = Navigation.Mode.None;
            slider.navigation = sliderNav;
        }

        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.DoNothing;

        // A slider row is a label, a track and a number, none of which fills the row - so without a
        // raycast target of its own only the few pixels of track were hoverable. Fully transparent,
        // and behind the slider's own graphics in the hierarchy, so dragging the handle still goes
        // to the slider rather than to this.
        // On the row itself, not a child: children of a UI object draw and raycast above it, so the
        // slider's own graphics still receive the drag. (Reordering siblings here would reorder the
        // row within its list, which is not what a raycast target should do.)
        var rowRaycast = row.GetComponent<Image>() ?? row.AddComponent<Image>();
        rowRaycast.color = Color.clear;
        rowRaycast.raycastTarget = true;

        var rowPointerSelect = driverHost.GetComponent<PointerSelectDriver>() ?? driverHost.AddComponent<PointerSelectDriver>();
        rowPointerSelect.target = selectable;
        var rowSurfacePointerSelect = row.GetComponent<PointerSelectDriver>() ?? row.AddComponent<PointerSelectDriver>();
        rowSurfacePointerSelect.target = selectable;
        if (selectable.targetGraphic == null)
        {
            selectable.targetGraphic = slider.targetGraphic ?? go.GetComponentInChildren<Graphic>(true);
        }
        var router = driverHost.GetComponent<CancelRouter>() ?? driverHost.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        var driver = driverHost.GetComponent<SliderMenuDriver>() ?? driverHost.AddComponent<SliderMenuDriver>();
        driver.Initialize(slider, whole);

        var highlight = CreateRowHighlight(row.transform, buttonTemplate, baseHeight, label, out var leftCursor, out var rightCursor, out var selectHighlight);
        var highlightDriver = driverHost.GetComponent<RowHighlightDriver>() ?? driverHost.AddComponent<RowHighlightDriver>();
        highlightDriver.Initialize(highlight, new[] { leftCursor, rightCursor, selectHighlight });
        selectable.leftCursor = leftCursor;
        selectable.rightCursor = rightCursor;
        selectable.selectHighlight = selectHighlight;
        SetAutomaticNavigation(selectable);
        return selectable;
    }

    /// <summary>
    /// A yes/no row. Rendered as an ordinary menu button whose label reads
    /// <c>"Something: On"</c>, rather than as a checkbox square.
    /// <para>
    /// It used to clone the game's Toggle prefab and sit a little square next to a label. Two
    /// problems with that: it was a second visual language for the same idea the Shade Enabled row
    /// already expressed in words, and a Toggle clone carries none of the selection fleurs a
    /// MenuButton clone does, so those rows were also the ones that looked unselected.
    /// </para>
    /// </summary>
    private static MenuSelectable CreateToggle(Transform parent, MenuButton buttonTemplate, string label, bool value, System.Action<bool> onChange, CancelTarget cancelTarget)
    {
        var selectable = CreateMenuButton(parent, buttonTemplate, label, null, cancelTarget);
        if (selectable is not MenuButton button)
        {
            if (selectable == null)
                LogMenuError($"Could not create toggle row '{label}'");
            return selectable;
        }

        var driver = button.gameObject.AddComponent<LabeledToggleDriver>();
        driver.Initialize(button, label, value, onChange);
        return button;
    }

}
#nullable restore
