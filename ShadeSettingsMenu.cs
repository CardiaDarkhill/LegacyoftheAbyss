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

public static class ShadeSettingsMenu
{
    private static GameObject screen;
    private static bool built;
    private static UIManager builtFor;
    private static MenuSelectable firstSelectable;
    private static readonly ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("ShadeSettingsMenu");
    private static bool loggedBuildAttempt;
    private static bool loggedMissingOptionsMenu;
    private static bool loggedMissingSliderTemplate;
    private static bool loggedMissingToggleTemplate;
    private static bool loggedNullUI;
    private static bool loggedNoPauseMenu;
    private static bool loggedButtonAlreadyPresent;
    private static bool loggedNoPauseButtonTemplates;
    private static bool loggedNoMenuButtonList;
    private static bool loggedNullEntries;
    private const float FractionalSliderStep = 0.1f;

    private struct ShadowStyle
    {
        public Type Type;
        public Color EffectColor;
        public Vector2 EffectDistance;
        public bool UseGraphicAlpha;
    }

    private struct TextStyle
    {
        public Font Font;
        public int FontSize;
        public FontStyle FontStyle;
        public TextAnchor Alignment;
        public Color Color;
        public bool RichText;
        public bool BestFit;
        public int BestFitMin;
        public int BestFitMax;
        public float LineSpacing;
        public bool AlignByGeometry;
        public HorizontalWrapMode HorizontalOverflow;
        public VerticalWrapMode VerticalOverflow;
        public List<ShadowStyle> Shadows;
    }

    private static TextStyle? sliderLabelStyle;
    private static TextStyle? sliderValueStyle;
    private static TextStyle? toggleLabelStyle;
    private static Font fallbackFont;

    private class CancelToPause : MonoBehaviour, ICancelHandler
    {
        public void OnCancel(BaseEventData eventData)
        {
            if (builtFor != null)
                HideImmediate(builtFor);
        }
    }

    private static IEnumerator FocusSelectableNextFrame(Selectable target, MenuSelectable wrapper)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(target.gameObject);
        target.navigation = wrapper.navigation;
    }

    internal static bool IsShowing => screen != null && screen.activeSelf;

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
        var uiSprite = Resources.GetBuiltinResource<Sprite>("UISprite.psd");
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
        var knobSprite = Resources.GetBuiltinResource<Sprite>("Knob.psd");
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
        var uiSprite = Resources.GetBuiltinResource<Sprite>("UISprite.psd");
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
        var checkSprite = Resources.GetBuiltinResource<Sprite>("Checkmark.psd");
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

        ClearAndApplyShadows(text, hasStyle ? resolved.Shadows : null);
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

    private static MenuSelectable CreateSlider(Transform parent, MenuSelectable template, string label, float min, float max, float value, System.Action<float> onChange, bool whole = false)
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
        hLayout.spacing = 36f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        ApplyTextStyle(labelTxt, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = 340f;
        labelLe.preferredWidth = 340f;

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

        var slider = go.GetComponentInChildren<Slider>(true);
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
        var valueLe = valueObj.AddComponent<LayoutElement>();
        valueLe.minWidth = 110f;
        valueLe.preferredWidth = 110f;

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
                log.LogWarning($"Error normalizing slider '{label}' value: {e}");
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

        var rowLe = row.AddComponent<LayoutElement>();
        rowLe.preferredHeight = rect.sizeDelta.y;
        rowLe.minHeight = rect.sizeDelta.y;

        // return whichever Selectable component exists (MenuSelectable if present)
        var selectable = go.GetComponent<MenuSelectable>();
        if (selectable == null)
        {
            log.LogError($"Created slider '{label}' missing Selectable component");
            Object.Destroy(row);
            return null;
        }
        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.GoToPauseMenu;
        slider.gameObject.AddComponent<CancelToPause>();
        selectable.OnSelected += _ =>
        {
            selectable.StartCoroutine(FocusSelectableNextFrame(slider, selectable));
        };
        return selectable;
    }

    private static MenuSelectable CreateToggle(Transform parent, MenuSelectable template, string label, bool value, System.Action<bool> onChange)
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
        hLayout.spacing = 36f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        ApplyTextStyle(labelTxt, toggleLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = 340f;
        labelLe.preferredWidth = 340f;

        // toggle instance
        var go = Object.Instantiate(template.gameObject, row.transform, false);
        go.SetActive(true);
        go.name = label + "Toggle";
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

        var toggle = go.GetComponentInChildren<Toggle>(true);
        Object.DestroyImmediate(toggle.GetComponent<MenuPreventDeselect>());
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var toggleLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        toggleLe.minWidth = 60f;
        toggleLe.preferredWidth = 60f;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.isOn = value;
        toggle.interactable = true;
        toggle.enabled = true;
        toggle.onValueChanged.AddListener(onChange.Invoke);
        toggle.gameObject.AddComponent<CancelToPause>();

        var rowLe = row.AddComponent<LayoutElement>();
        rowLe.preferredHeight = rect.sizeDelta.y;
        rowLe.minHeight = rect.sizeDelta.y;

        var selectable = go.GetComponent<MenuSelectable>();
        if (selectable == null)
        {
            log.LogError($"Created toggle '{label}' missing Selectable component");
            Object.Destroy(row);
            return null;
        }
        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.GoToPauseMenu;
        selectable.OnSelected += _ =>
        {
            selectable.StartCoroutine(FocusSelectableNextFrame(toggle, selectable));
        };
        return selectable;
    }

    private static void Build(UIManager ui)
    {
        if (!loggedBuildAttempt)
        {
            log.LogInfo("Attempting to build Shade settings page");
            loggedBuildAttempt = true;
        }
        if (built && screen != null && builtFor == ui)
        {
            log.LogDebug("Settings page already built for this UI");
            return;
        }

        if (screen != null && builtFor != ui)
        {
            log.LogDebug("UIManager changed, destroying previous settings page");
            Object.Destroy(screen);
            screen = null;
        }

        built = false;
        builtFor = ui;

        // Need an options menu screen to clone for consistent styling
        var optionsScreen = ui.optionsMenuScreen;
        GameObject templateScreen;
        if (optionsScreen == null)
        {
            if (!loggedMissingOptionsMenu)
            {
                log.LogWarning("optionsMenuScreen not yet available; using pause menu as template");
                loggedMissingOptionsMenu = true;
            }
            templateScreen = ui.pauseMenuScreen.gameObject;
        }
        else
        {
            templateScreen = optionsScreen.gameObject;
        }

        MenuSelectable sliderTemplate = null;
        if (optionsScreen != null)
        {
            var candidates = optionsScreen.GetComponentsInChildren<MenuSelectable>(true);
            foreach (var cand in candidates)
            {
                if (cand.GetComponentInChildren<Slider>(true) != null)
                {
                    sliderTemplate = cand;
                    break;
                }
            }
        }
        bool createdSliderTemplate = false;
        if (sliderTemplate == null)
        {
            if (!loggedMissingSliderTemplate)
            {
                log.LogWarning("slider template not found in options menu; using default");
                loggedMissingSliderTemplate = true;
            }
            sliderTemplate = CreateDefaultSliderTemplate();
            createdSliderTemplate = true;
        }

        MenuSelectable toggleTemplate = null;
        if (optionsScreen != null)
        {
            var candidates = optionsScreen.GetComponentsInChildren<MenuSelectable>(true);
            foreach (var cand in candidates)
            {
                if (cand.GetComponentInChildren<Toggle>(true) != null)
                {
                    toggleTemplate = cand;
                    break;
                }
            }
        }
        bool createdToggleTemplate = false;
        if (toggleTemplate == null)
        {
            if (!loggedMissingToggleTemplate)
            {
                log.LogWarning("toggle template not found in options menu; using default");
                loggedMissingToggleTemplate = true;
            }
            toggleTemplate = CreateDefaultToggleTemplate();
            createdToggleTemplate = true;
        }

        CacheTextStyles(sliderTemplate, toggleTemplate);

        screen = Object.Instantiate(templateScreen, templateScreen.transform.parent);
        screen.name = "ShadeSettingsPage";
        screen.SetActive(false);
        log.LogDebug("Instantiated ShadeSettingsPage");

        var canvasGroup = screen.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        var rt = screen.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var ms = screen.GetComponent<MenuScreen>();

        // remove existing children except back button
        foreach (Transform child in ms.transform)
        {
            if (ms.backButton != null && child.gameObject == ms.backButton.gameObject)
                continue;
            Object.Destroy(child.gameObject);
        }

        var content = new GameObject("Content");
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.SetParent(ms.transform, false);
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 24f;
        layout.padding = new RectOffset(60, 60, 40, 60);

        var selectables = new List<MenuSelectable>();
        MenuSelectable s;
        s = CreateSlider(content.transform, sliderTemplate, "Hornet Damage", 0.2f, 2f, ModConfig.Instance.hornetDamageMultiplier, v => ModConfig.Instance.hornetDamageMultiplier = v);
        if (s != null) selectables.Add(s);
        s = CreateSlider(content.transform, sliderTemplate, "Shade Damage", 0.2f, 2f, ModConfig.Instance.shadeDamageMultiplier, v => ModConfig.Instance.shadeDamageMultiplier = v);
        if (s != null) selectables.Add(s);
        s = CreateSlider(content.transform, sliderTemplate, "Shade Heal (Bind)", 0f, 6f, ModConfig.Instance.bindShadeHeal, v => ModConfig.Instance.bindShadeHeal = Mathf.RoundToInt(v), true);
        if (s != null) selectables.Add(s);
        s = CreateSlider(content.transform, sliderTemplate, "Hornet Heal (Bind)", 0f, 6f, ModConfig.Instance.bindHornetHeal, v => ModConfig.Instance.bindHornetHeal = Mathf.RoundToInt(v), true);
        if (s != null) selectables.Add(s);
        s = CreateSlider(content.transform, sliderTemplate, "Shade Focus Heal", 0f, 6f, ModConfig.Instance.focusShadeHeal, v => ModConfig.Instance.focusShadeHeal = Mathf.RoundToInt(v), true);
        if (s != null) selectables.Add(s);
        s = CreateSlider(content.transform, sliderTemplate, "Hornet Focus Heal", 0f, 6f, ModConfig.Instance.focusHornetHeal, v => ModConfig.Instance.focusHornetHeal = Mathf.RoundToInt(v), true);
        if (s != null) selectables.Add(s);
        s = CreateToggle(content.transform, toggleTemplate, "Damage Logging", ModConfig.Instance.logDamage, v => ModConfig.Instance.logDamage = v);
        if (s != null) selectables.Add(s);
        if (selectables.Count > 0)
        {
            firstSelectable = selectables[0];
            if (ms != null)
                ms.defaultHighlight = firstSelectable;
        }

        if (ms.backButton != null)
        {
            log.LogDebug("Wiring back button");
            ms.backButton.OnSubmitPressed.AddListener(() => ui.StartCoroutine(Hide(ui)));
        }

        var mbl = screen.GetComponent<MenuButtonList>() ?? screen.AddComponent<MenuButtonList>();
        mbl.ClearLastSelected();
        var entryField = typeof(MenuButtonList).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
        var entryType = entryField.FieldType.GetElementType();
        var arr = Array.CreateInstance(entryType, selectables.Count);
        for (int i = 0; i < selectables.Count; i++)
        {
            var e = Activator.CreateInstance(entryType);
            entryType.GetField("selectable", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(e, selectables[i]);
            arr.SetValue(e, i);
        }
        entryField.SetValue(mbl, arr);
        mbl.SetupActive();

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        if (createdSliderTemplate)
            Object.Destroy(sliderTemplate.gameObject);
        if (createdToggleTemplate)
            Object.Destroy(toggleTemplate.gameObject);

        built = true;
        log.LogInfo("Shade settings page built");
    }

    internal static void Inject(UIManager ui)
    {
        if (ui == null)
        {
            if (!loggedNullUI)
            {
                log.LogWarning("Inject called with null UIManager");
                loggedNullUI = true;
            }
            return;
        }
        if (ui.pauseMenuScreen == null)
        {
            if (!loggedNoPauseMenu)
            {
                log.LogWarning("pauseMenuScreen not yet available");
                loggedNoPauseMenu = true;
            }
            return;
        }

        // Ensure a screen exists for this UI
        Build(ui);
        if (screen == null)
            return;

        // Avoid duplicate buttons by scanning entire hierarchy
        foreach (Transform child in ui.pauseMenuScreen.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "ShadeSettingsButton")
            {
                if (!loggedButtonAlreadyPresent)
                {
                    log.LogInfo("ShadeSettingsButton already present; skipping injection");
                    loggedButtonAlreadyPresent = true;
                }
                return;
            }
        }

        var buttons = ui.pauseMenuScreen.GetComponentsInChildren<PauseMenuButton>(true);
        if (buttons.Length == 0)
        {
            if (!loggedNoPauseButtonTemplates)
            {
                log.LogWarning("No PauseMenuButton templates found");
                loggedNoPauseButtonTemplates = true;
            }
            return;
        }

        PauseMenuButton template = null;
        MenuButtonList list = null;
        foreach (var b in buttons)
        {
            var l = b.GetComponentInParent<MenuButtonList>(true);
            if (l != null)
            {
                template = b;
                list = l;
                break;
            }
        }
        if (template == null || list == null)
        {
            if (!loggedNoMenuButtonList)
            {
                log.LogWarning("MenuButtonList not found on template parent");
                loggedNoMenuButtonList = true;
            }
            return;
        }
        var field = typeof(MenuButtonList).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
        var entries = (Array)field.GetValue(list);
        if (entries == null)
        {
            if (!loggedNullEntries)
            {
                log.LogWarning("MenuButtonList entries field null");
                loggedNullEntries = true;
            }
            return;
        }

        var go = Object.Instantiate(template.gameObject, template.transform.parent);
        go.name = "ShadeSettingsButton";
        Object.DestroyImmediate(go.GetComponentInChildren<AutoLocalizeTextUI>());
        bool hasLabel = false;
        var txt = go.GetComponentInChildren<Text>(true);
        if (txt != null)
        {
            txt.text = "Legacy of the Abyss";
            txt.color = Color.white;
            hasLabel = true;
        }
        else
        {
            var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                var tmp = go.GetComponentInChildren(tmpType, true);
                if (tmp != null)
                {
                    tmpType.GetProperty("text")?.SetValue(tmp, "Legacy of the Abyss");
                    tmpType.GetProperty("color")?.SetValue(tmp, Color.white);
                    hasLabel = true;
                }
            }
        }

        if (!hasLabel)
        {
            var textObj = new GameObject("Label");
            textObj.transform.SetParent(go.transform, false);
            var t = textObj.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.text = "Legacy of the Abyss";
            t.color = Color.white;
        }

        var pauseBtn = go.GetComponent<PauseMenuButton>();

        foreach (var cond in go.GetComponents<MenuButtonListCondition>())
            Object.DestroyImmediate(cond);

        var entryType = entries.GetType().GetElementType();
        var newEntry = Activator.CreateInstance(entryType);
        var selField = entryType.GetField("selectable", BindingFlags.NonPublic | BindingFlags.Instance);
        selField.SetValue(newEntry, pauseBtn);
        var arr = Array.CreateInstance(entryType, entries.Length + 1);
        entries.CopyTo(arr, 0);
        arr.SetValue(newEntry, entries.Length);
        field.SetValue(list, arr);

        var dirtyField = typeof(MenuButtonList).GetField("isDirty", BindingFlags.NonPublic | BindingFlags.Instance);
        dirtyField?.SetValue(list, true);

        list.SetupActive();
        log.LogInfo("Injected ShadeSettingsButton into pause menu");
    }

    internal static IEnumerator Show(UIManager ui)
    {
        Build(ui);
        if (screen == null)
        {
            log.LogWarning("Show called but screen is null");
            yield break;
        }

        log.LogInfo("Showing Shade settings page");
        ui.pauseMenuScreen.gameObject.SetActive(false);
        screen.SetActive(true);
        if (firstSelectable != null)
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
            UIManager.HighlightSelectableNoSound(firstSelectable.GetFirstInteractable());
        }
        yield break;
    }

    internal static void HideImmediate(UIManager ui)
    {
        if (screen == null)
            return;
        log.LogInfo("Hiding Shade settings page");
        screen.SetActive(false);
        var targetUi = ui ?? UIManager.instance;
        if (targetUi != null)
        {
            if (targetUi.pauseMenuScreen != null)
                targetUi.pauseMenuScreen.gameObject.SetActive(true);
            try
            {
                targetUi.UIGoToPauseMenu();
            }
            catch (Exception e)
            {
                log.LogWarning($"Failed to navigate back to pause menu: {e}");
            }
        }
        ModConfig.Save();
    }

    internal static IEnumerator Hide(UIManager ui)
    {
        HideImmediate(ui);
        yield break;
    }

    internal static void Clear()
    {
        if (screen != null)
        {
            Object.Destroy(screen);
            screen = null;
        }
        built = false;
        builtFor = null;
        firstSelectable = null;
        sliderLabelStyle = null;
        sliderValueStyle = null;
        toggleLabelStyle = null;
        fallbackFont = null;
    }
}
