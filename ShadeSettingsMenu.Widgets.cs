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

    private static MenuSelectable CreateDefaultSliderTemplate()
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
        bgImage.sprite = uiSprite;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
        var backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.anchorMin = Vector2.zero;
        backgroundRt.anchorMax = Vector2.one;
        backgroundRt.offsetMin = Vector2.zero;
        backgroundRt.offsetMax = Vector2.zero;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRt.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRt.sizeDelta = new Vector2(-20f, 0f);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.sprite = uiSprite;
        fillImg.type = Image.Type.Sliced;
        fillImg.color = new Color(0.75f, 0.75f, 0.78f, 0.95f);
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
        handleAreaRt.sizeDelta = new Vector2(-20f, 0f);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        var knobSprite = GetFallbackSprite(ref fallbackKnobSprite, "ShadeSettingsSliderKnob", false);
        handleImg.sprite = knobSprite;
        handleImg.color = Color.white;
        var handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(20f, 20f);

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
        return selectable;
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

        if (MenuFontScale > 1f)
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

    private static void CacheTextStyles(MenuSelectable sliderTemplate, MenuSelectable toggleTemplate)
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

    private static MenuSelectable CreateSlider(Transform parent, MenuSelectable template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, bool whole = false)
    {
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
        hLayout.spacing = 64f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        ApplyTextStyle(labelTxt, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        labelTxt.raycastTarget = false;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = LabelColumnWidth;
        labelLe.preferredWidth = LabelColumnWidth;
        labelLe.flexibleWidth = 0f;

        // slider instance
        var go = Object.Instantiate(template.gameObject, row.transform, false);
        go.SetActive(true);
        go.name = label + "Slider";
        foreach (var t in go.GetComponentsInChildren<Text>(true))
            Object.DestroyImmediate(t);
        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType != null)
        {
            var tmps = go.GetComponentsInChildren(tmpType, true);
            foreach (var tmp in tmps)
                Object.DestroyImmediate(tmp);
        }
        foreach (var auto in go.GetComponentsInChildren<AutoLocalizeTextUI>(true))
            Object.DestroyImmediate(auto);

        SanitizeSelectableHierarchy(go);

        var slider = go.GetComponentInChildren<Slider>(true);
        if (slider == null)
        {
            LogMenuError($"Created slider '{label}' missing Slider component");
            Object.DestroyImmediate(row);
            return null;
        }
        Object.DestroyImmediate(slider.GetComponent<MenuAudioSlider>());
        Object.DestroyImmediate(slider.GetComponent<MenuPreventDeselect>());
        slider.onValueChanged.RemoveAllListeners();
        slider.interactable = true;
        slider.enabled = true;
        if (slider.GetComponent<SliderRightStickInput>() == null)
            slider.gameObject.AddComponent<SliderRightStickInput>();
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var sliderLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        sliderLe.minWidth = 800f;
        sliderLe.preferredWidth = 800f;
        sliderLe.flexibleWidth = 1f;
        var sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800f);

        // value text to the right of slider
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(row.transform, false);
        var valueTxt = valueObj.AddComponent<Text>();
        ApplyTextStyle(valueTxt, sliderValueStyle, TextAnchor.MiddleRight, Color.white);
        valueTxt.raycastTarget = false;
        var valueLe = valueObj.AddComponent<LayoutElement>();
        valueLe.minWidth = ValueColumnWidth;
        valueLe.preferredWidth = ValueColumnWidth;
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
        if (baseHeight <= 0f)
            baseHeight = SliderRowHeight;
        else
            baseHeight = Mathf.Max(baseHeight, SliderRowHeight);
        rowLe.preferredHeight = baseHeight;
        rowLe.minHeight = baseHeight;
        rowRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        if (rect != null)
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);

        // return whichever Selectable component exists (MenuSelectable if present)
        var selectable = go.GetComponent<MenuSelectable>();
        if (selectable == null)
        {
            LogMenuError($"Created slider '{label}' missing Selectable component");
            Object.Destroy(row);
            return null;
        }
        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.DoNothing;
        var router = go.GetComponent<CancelRouter>() ?? go.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        var driver = go.GetComponent<SliderMenuDriver>() ?? go.AddComponent<SliderMenuDriver>();
        driver.Initialize(slider, whole);

        var highlight = CreateRowHighlight(row.transform, buttonTemplate, baseHeight, label, out var leftCursor, out var rightCursor, out var selectHighlight);
        var highlightDriver = go.GetComponent<RowHighlightDriver>() ?? go.AddComponent<RowHighlightDriver>();
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
