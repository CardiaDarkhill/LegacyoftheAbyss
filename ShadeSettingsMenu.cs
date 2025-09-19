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

public static class ShadeSettingsMenu
{
    private static GameObject screen;
    private static bool built;
    private static UIManager builtFor;
    private static MenuScreen mainScreen;
    private static MenuScreen difficultyScreen;
    private static MenuScreen controlsScreen;
    private static MenuScreen loggingScreen;
    private static MenuScreen activeScreen;
    private static readonly List<MenuScreen> allScreens = new();
    private static readonly Dictionary<MenuScreen, MenuSelectable> screenFirstSelectables = new();
    private static GameObject templateSource;
    private static bool templateSourceWasActive;
    private static bool pauseMenuWasActive;
    private static bool optionsMenuWasActive;
    private static bool gameOptionsMenuWasActive;
    private static bool storedGameOptionsCanvasState;
    private static float storedGameOptionsAlpha;
    private static bool storedGameOptionsInteractable;
    private static bool storedGameOptionsBlocksRaycasts;
    private static readonly ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("ShadeSettingsMenu");
    private static bool loggedBuildAttempt;
    private static bool loggedMissingOptionsMenu;
    private static bool loggedMissingSliderTemplate;
    private static bool loggedNullUI;
    private static bool loggedNoPauseMenu;
    private static bool loggedButtonAlreadyPresent;
    private static bool loggedNoPauseButtonTemplates;
    private static bool loggedNoMenuButtonList;
    private static bool loggedNullEntries;
    private const float FractionalSliderStep = 0.1f;
    private const float SliderRowHeight = 96f;
    private const float ToggleRowHeight = 84f;
    private const float ButtonRowHeight = 88f;
    private const float ContentSpacing = 64f;
    private const float LabelColumnWidth = 420f;
    private const float ValueColumnWidth = 140f;
    private const float MenuFontScale = 1.5f;
    private static readonly Color ButtonNormalColor = new Color(1f, 1f, 1f, 0f);
    private static readonly Color ButtonHighlightColor = new Color(1f, 0.95f, 0.78f, 0.35f);
    private static readonly Color ButtonPressedColor = new Color(0.95f, 0.9f, 0.8f, 0.45f);
    private static readonly Color ButtonDisabledColor = new Color(1f, 1f, 1f, 0.15f);
    private static bool consumeNextToggle;
    private static readonly List<BindingMenuDriver> bindingDrivers = new();
    private static ShadeToggleDriver shadeToggleDriver;

    private static string GetShadeToggleLabel() => $"Shade Enabled: {(ModConfig.Instance.shadeEnabled ? "On" : "Off")}";

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
    private static Sprite fallbackSlicedSprite;
    private static Sprite fallbackKnobSprite;
    private static Sprite fallbackCheckSprite;

    private static void LogMenu(LogLevel level, string message)
    {
        if (!ModConfig.Instance.logMenu)
            return;
        log.Log(level, message);
    }

    private static void LogMenuDebug(string message) => LogMenu(LogLevel.Debug, message);
    private static void LogMenuInfo(string message) => LogMenu(LogLevel.Info, message);
    private static void LogMenuWarning(string message) => LogMenu(LogLevel.Warning, message);
    private static void LogMenuError(string message) => LogMenu(LogLevel.Error, message);

    private enum CancelTarget
    {
        PauseMenu,
        ShadeMain
    }

    private class CancelRouter : MonoBehaviour, ICancelHandler
    {
        public CancelTarget target;

        public void OnCancel(BaseEventData eventData)
        {
            eventData?.Use();
            if (target == CancelTarget.ShadeMain)
            {
                ShowMainMenu();
            }
            else
            {
                var ui = builtFor ?? UIManager.instance;
                if (ui != null)
                {
                    bool consumeToggle = activeScreen != null && activeScreen != mainScreen;
                    HideImmediate(ui, consumeToggle);
                }
            }
        }
    }

    internal static bool IsShowing => activeScreen != null && activeScreen.gameObject != null && activeScreen.gameObject.activeSelf;

    private sealed class SliderMenuDriver : MonoBehaviour, IMoveHandler, ISubmitHandler
    {
        public Slider slider;
        public bool wholeNumbers;

        public void Initialize(Slider s, bool whole)
        {
            slider = s;
            wholeNumbers = whole;
        }

        private void Step(float direction)
        {
            if (slider == null)
                return;
            float delta = wholeNumbers ? 1f : FractionalSliderStep;
            float target = slider.value + delta * direction;
            float snapped = SnapSliderValue(target, slider.minValue, slider.maxValue, wholeNumbers);
            if (!Mathf.Approximately(snapped, slider.value))
            {
                slider.value = snapped;
            }
        }

        public void OnMove(AxisEventData eventData)
        {
            if (slider == null || eventData == null)
                return;
            if (eventData.moveDir == MoveDirection.Left)
            {
                Step(-1f);
                eventData.Use();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                Step(1f);
                eventData.Use();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (slider == null)
                return;
            Step(1f);
            eventData?.Use();
        }
    }

    private sealed class ToggleMenuDriver : MonoBehaviour, IMoveHandler, ISubmitHandler
    {
        public Toggle toggle;

        public void Initialize(Toggle t)
        {
            toggle = t;
        }

        public void OnMove(AxisEventData eventData)
        {
            if (toggle == null || eventData == null)
                return;
            if (eventData.moveDir == MoveDirection.Left)
            {
                if (toggle.isOn)
                    toggle.isOn = false;
                eventData.Use();
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                if (!toggle.isOn)
                    toggle.isOn = true;
                eventData.Use();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (toggle == null)
                return;
            toggle.isOn = !toggle.isOn;
            eventData?.Use();
        }
    }

    private sealed class BindingMenuDriver : MonoBehaviour
    {
        private MenuButton button;
        private ShadeAction action;
        private bool secondary;
        private string labelPrefix;
        private Text uiText;
        private Component tmpTextComponent;
        private PropertyInfo tmpTextProperty;
        private bool capturing;

        public void Initialize(MenuButton menuButton, ShadeAction targetAction, bool isSecondary, string label)
        {
            button = menuButton;
            action = targetAction;
            secondary = isSecondary;
            labelPrefix = label;
            uiText = button.GetComponentInChildren<Text>(true);
            var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                tmpTextComponent = button.GetComponentInChildren(tmpType, true);
                tmpTextProperty = tmpType.GetProperty("text");
            }

            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(BeginCapture);
            RegisterBindingDriver(this);
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            string bindingText = ShadeInput.DescribeBindingOption(ShadeInput.GetBindingOption(action, secondary));
            SetButtonText($"{labelPrefix}: {bindingText}");
        }

        private void SetButtonText(string value)
        {
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }
            if (tmpTextComponent != null && tmpTextProperty != null)
            {
                tmpTextProperty.SetValue(tmpTextComponent, value);
            }
        }

        private void BeginCapture()
        {
            if (!capturing)
                StartCoroutine(CaptureRoutine());
        }

        private System.Collections.IEnumerator CaptureRoutine()
        {
            capturing = true;
            SetButtonText($"{labelPrefix}: Press a binding... (Esc cancels, Backspace clears)");
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(KeyCode.Escape))
                    break;
                if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.None());
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
                if (ShadeInput.TryCaptureKey(out var key))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.FromKey(key));
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
                if (ShadeInput.TryCaptureControl(out var control, out int deviceIndex))
                {
                    ShadeInput.SetBindingOption(action, secondary, ShadeBindingOption.FromControl(control, deviceIndex));
                    ShadeInput.EnsureControllerIndex(deviceIndex);
                    ModConfig.Save();
                    NotifyBindingChanged();
                    break;
                }
            }
            capturing = false;
            UpdateLabel();
        }

        private void OnDestroy()
        {
            UnregisterBindingDriver(this);
        }
    }

    private sealed class ShadeToggleDriver : MonoBehaviour
    {
        private MenuButton button;
        private Text uiText;
        private Component tmpTextComponent;
        private PropertyInfo tmpTextProperty;

        public void Initialize(MenuButton menuButton)
        {
            button = menuButton;
            uiText = button.GetComponentInChildren<Text>(true);
            var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (tmpType != null)
            {
                tmpTextComponent = button.GetComponentInChildren(tmpType, true);
                tmpTextProperty = tmpType.GetProperty("text");
            }

            button.OnSubmitPressed.RemoveAllListeners();
            button.OnSubmitPressed.AddListener(ToggleShade);
            shadeToggleDriver = this;
            UpdateLabel();
        }

        private void OnEnable()
        {
            if (shadeToggleDriver == null)
                shadeToggleDriver = this;
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            SetButtonText(GetShadeToggleLabel());
        }

        private void ToggleShade()
        {
            LegacyHelper.SetShadeEnabled(!ModConfig.Instance.shadeEnabled);
        }

        private void SetButtonText(string value)
        {
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }

            if (tmpTextComponent != null && tmpTextProperty != null)
            {
                tmpTextProperty.SetValue(tmpTextComponent, value);
            }
        }

        private void OnDestroy()
        {
            if (shadeToggleDriver == this)
                shadeToggleDriver = null;
        }
    }

    private static void RegisterBindingDriver(BindingMenuDriver driver)
    {
        if (driver != null && !bindingDrivers.Contains(driver))
            bindingDrivers.Add(driver);
    }

    private static void UnregisterBindingDriver(BindingMenuDriver driver)
    {
        if (driver == null)
            return;
        bindingDrivers.Remove(driver);
    }

    internal static void NotifyBindingChanged()
    {
        for (int i = bindingDrivers.Count - 1; i >= 0; i--)
        {
            var driver = bindingDrivers[i];
            if (driver == null)
            {
                bindingDrivers.RemoveAt(i);
                continue;
            }
            driver.UpdateLabel();
        }
    }

    internal static void NotifyShadeToggleChanged()
    {
        shadeToggleDriver?.UpdateLabel();
    }

    private static void ApplyDefaultPreset()
    {
        ShadeInput.Config.ResetToDefaults();
        HornetInput.ApplyControllerDefaults();
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyDualControllerPresetOption()
    {
        ShadeInput.Config.ApplyDualControllerPreset();
        HornetInput.ApplyControllerDefaults();
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyKeyboardOnlyPresetOption()
    {
        ShadeInput.Config.ApplyKeyboardOnlyPreset();
        HornetInput.ApplyKeyboardDefaults(true);
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private static void ApplyShadeControllerPresetOption()
    {
        ShadeInput.Config.ApplyShadeControllerPreset();
        HornetInput.ApplyKeyboardDefaults(true);
        ModConfig.Save();
        NotifyBindingChanged();
    }

    private sealed class RowHighlightDriver : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject highlight;
        private readonly List<Animator> animators = new();
        private static readonly int ShowTrigger = Animator.StringToHash("show");
        private static readonly int HideTrigger = Animator.StringToHash("hide");

        public void Initialize(GameObject highlightGo, IEnumerable<Animator> highlightAnimators)
        {
            highlight = highlightGo;
            animators.Clear();
            if (highlightAnimators != null)
            {
                foreach (var animator in highlightAnimators)
                {
                    if (animator != null)
                        animators.Add(animator);
                }
            }
            SetActive(false, true);
        }

        private void SetActive(bool active, bool instant = false)
        {
            bool useFallback = highlight != null && animators.Count == 0;
            if (useFallback && highlight.activeSelf != active)
                highlight.SetActive(active);

            if (animators.Count == 0)
                return;

            foreach (var animator in animators)
            {
                if (animator == null)
                    continue;
                try
                {
                    if (active)
                    {
                        animator.ResetTrigger(HideTrigger);
                        animator.SetTrigger(ShowTrigger);
                    }
                    else
                    {
                        animator.ResetTrigger(ShowTrigger);
                        animator.SetTrigger(HideTrigger);
                    }
                    if (instant)
                        animator.Update(0f);
                }
                catch
                {
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);
            SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                return;
            SetActive(false);
        }

        private void OnDisable()
        {
            SetActive(false, true);
        }
    }

    private sealed class MenuFocusDriver : MonoBehaviour
    {
        public MenuScreen screen;

        private void Update()
        {
            if (screen == null || !screen.gameObject.activeInHierarchy)
                return;

            var eventSystem = EventSystem.current;
            if (eventSystem == null)
                return;

            var current = eventSystem.currentSelectedGameObject;
            if (current != null && current.transform.IsChildOf(screen.transform))
                return;

            var highlight = GetPreferredHighlight(screen);
            if (highlight == null)
                return;

            var selectable = highlight.GetFirstInteractable();
            if (selectable == null)
                return;

            eventSystem.SetSelectedGameObject(selectable.gameObject);
            UIManager.HighlightSelectableNoSound(selectable);
        }
    }

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
            if (comp is CancelRouter || comp is SliderMenuDriver || comp is ToggleMenuDriver)
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

    private static MenuSelectable CreateToggle(Transform parent, MenuSelectable template, MenuButton buttonTemplate, string label, bool value, System.Action<bool> onChange, CancelTarget cancelTarget)
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
        ApplyTextStyle(labelTxt, toggleLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        labelTxt.raycastTarget = false;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = LabelColumnWidth;
        labelLe.preferredWidth = LabelColumnWidth;
        labelLe.flexibleWidth = 0f;

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

        SanitizeSelectableHierarchy(go);

        var toggle = go.GetComponentInChildren<Toggle>(true);
        if (toggle == null)
        {
            LogMenuError($"Created toggle '{label}' missing Toggle component");
            Object.DestroyImmediate(row);
            return null;
        }
        Object.DestroyImmediate(toggle.GetComponent<MenuPreventDeselect>());
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var toggleLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        toggleLe.minWidth = 80f;
        toggleLe.preferredWidth = 80f;
        toggleLe.flexibleWidth = 0f;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.isOn = value;
        toggle.interactable = true;
        toggle.enabled = true;
        toggle.onValueChanged.AddListener(onChange.Invoke);

        SetAutomaticNavigation(toggle);

        var rowLe = row.AddComponent<LayoutElement>();
        float baseHeight = 0f;
        if (rect != null)
        {
            baseHeight = rect.rect.height;
            if (baseHeight <= 0f)
                baseHeight = rect.sizeDelta.y;
        }
        if (baseHeight <= 0f)
            baseHeight = ToggleRowHeight;
        else
            baseHeight = Mathf.Max(baseHeight, ToggleRowHeight);
        rowLe.preferredHeight = baseHeight;
        rowLe.minHeight = baseHeight;
        rowRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        if (rect != null)
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);

        var selectable = go.GetComponent<MenuSelectable>();
        if (selectable == null)
        {
            LogMenuError($"Created toggle '{label}' missing Selectable component");
            Object.Destroy(row);
            return null;
        }
        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.DoNothing;
        var router = go.GetComponent<CancelRouter>() ?? go.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        var driver = go.GetComponent<ToggleMenuDriver>() ?? go.AddComponent<ToggleMenuDriver>();
        driver.Initialize(toggle);

        var highlight = CreateRowHighlight(row.transform, buttonTemplate, baseHeight, label, out var leftCursor, out var rightCursor, out var selectHighlight);
        var highlightDriver = go.GetComponent<RowHighlightDriver>() ?? go.AddComponent<RowHighlightDriver>();
        highlightDriver.Initialize(highlight, new[] { leftCursor, rightCursor, selectHighlight });
        selectable.leftCursor = leftCursor;
        selectable.rightCursor = rightCursor;
        selectable.selectHighlight = selectHighlight;
        SetAutomaticNavigation(selectable);
        return selectable;
    }

    private static void DestroyScreens()
    {
        foreach (var ms in allScreens)
        {
            if (ms != null)
                Object.Destroy(ms.gameObject);
        }
        allScreens.Clear();
        screenFirstSelectables.Clear();
        mainScreen = null;
        difficultyScreen = null;
        controlsScreen = null;
        loggingScreen = null;
        activeScreen = null;
        screen = null;
        templateSource = null;
        templateSourceWasActive = false;
        pauseMenuWasActive = false;
        optionsMenuWasActive = false;
        gameOptionsMenuWasActive = false;
        consumeNextToggle = false;
    }

    private static void StripTemplateComponents(MenuScreen ms)
    {
        if (ms == null)
            return;

        foreach (var comp in ms.GetComponents<MonoBehaviour>())
        {
            if (comp == null)
                continue;
            var type = comp.GetType();
            if (type == typeof(MenuScreen) || type == typeof(CanvasGroup) || type == typeof(MenuButtonList) || type == typeof(Animator) || type == typeof(GraphicRaycaster) || comp is CancelRouter)
                continue;
            var ns = type.Namespace ?? string.Empty;
            if (ns.StartsWith("UnityEngine"))
                continue;
            Object.DestroyImmediate(comp);
        }

        foreach (var comp in ms.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (comp == null)
                continue;
            if (ms.backButton != null && comp.gameObject == ms.backButton.gameObject)
                continue;
            if (comp is CancelRouter || comp is SliderMenuDriver || comp is ToggleMenuDriver)
                continue;
            var type = comp.GetType();
            string fullName = type.FullName ?? string.Empty;
            bool shouldDestroy = false;
            if (fullName.Contains("MenuOptions") || fullName.Contains("MenuOption") || fullName.Contains("PauseMenu"))
                shouldDestroy = true;
            if (!shouldDestroy && (type.GetInterface("HKMenu.IMenuOptionLayout") != null || type.GetInterface("IMenuOptionLayout") != null))
                shouldDestroy = true;
            if (shouldDestroy)
                Object.DestroyImmediate(comp);
        }
    }

    private static void InitializeScreen(MenuScreen ms)
    {
        if (ms == null)
            return;
        var canvasGroup = ms.ScreenCanvasGroup;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        var rt = ms.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        ms.transform.SetAsLastSibling();
        StripTemplateComponents(ms);

        var focusDriver = ms.gameObject.GetComponent<MenuFocusDriver>() ?? ms.gameObject.AddComponent<MenuFocusDriver>();
        focusDriver.screen = ms;
    }

    private static RectTransform CreateContentRoot(MenuScreen ms)
    {
        if (ms == null)
            return null;
        foreach (Transform child in ms.transform)
        {
            if (ms.backButton != null && child.gameObject == ms.backButton.gameObject)
                continue;
            Object.DestroyImmediate(child.gameObject);
        }

        var content = new GameObject("Content");
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.SetParent(ms.transform, false);
        contentRect.anchorMin = new Vector2(0f, 0f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.offsetMin = new Vector2(60f, 80f);
        contentRect.offsetMax = new Vector2(-60f, -70f);
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = ContentSpacing;
        layout.padding = new RectOffset(0, 0, 10, 10);
        layout.childAlignment = TextAnchor.UpperLeft;
        return contentRect;
    }

    private static void ConfigureBackButton(MenuScreen ms, CancelTarget cancelTarget, UIManager ui)
    {
        if (ms?.backButton == null)
            return;
        ms.backButton.OnSubmitPressed.RemoveAllListeners();
        if (cancelTarget == CancelTarget.PauseMenu)
        {
            if (ui != null)
                ms.backButton.OnSubmitPressed.AddListener(() => ui.StartCoroutine(Hide(ui)));
        }
        else
        {
            ms.backButton.OnSubmitPressed.AddListener(ShowMainMenu);
        }
        foreach (var cond in ms.backButton.GetComponents<MenuButtonListCondition>())
            Object.DestroyImmediate(cond);
        var pauseMenuComponent = ms.backButton.GetComponent<PauseMenuButton>();
        if (pauseMenuComponent != null)
            Object.DestroyImmediate(pauseMenuComponent);
        ms.backButton.cancelAction = CancelAction.DoNothing;
        var router = ms.backButton.gameObject.GetComponent<CancelRouter>() ?? ms.backButton.gameObject.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        var backLayout = ms.backButton.GetComponent<LayoutElement>() ?? ms.backButton.gameObject.AddComponent<LayoutElement>();
        backLayout.minHeight = ButtonRowHeight;
        backLayout.preferredHeight = ButtonRowHeight;
        backLayout.flexibleHeight = 0f;
        backLayout.minWidth = 0f;
        backLayout.preferredWidth = 0f;
        backLayout.flexibleWidth = 1f;
        var backRect = ms.backButton.GetComponent<RectTransform>();
        if (backRect != null)
        {
            backRect.anchorMin = new Vector2(0f, 0.5f);
            backRect.anchorMax = new Vector2(1f, 0.5f);
            backRect.pivot = new Vector2(0.5f, 0.5f);
            backRect.offsetMin = new Vector2(0f, backRect.offsetMin.y);
            backRect.offsetMax = new Vector2(0f, backRect.offsetMax.y);
            backRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ButtonRowHeight);
        }
        var backText = ms.backButton.GetComponentInChildren<Text>(true);
        if (backText != null)
        {
            string existing = backText.text;
            ApplyTextStyle(backText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            backText.text = existing;
        }
    }

    private static void SetupButtonList(MenuScreen ms, List<MenuSelectable> selectables)
    {
        if (ms == null)
            return;
        var mbl = ms.GetComponent<MenuButtonList>() ?? ms.gameObject.AddComponent<MenuButtonList>();
        var topField = typeof(MenuButtonList).GetField("isTopLevelMenu", BindingFlags.NonPublic | BindingFlags.Instance);
        topField?.SetValue(mbl, false);
        var skipField = typeof(MenuButtonList).GetField("skipDisabled", BindingFlags.NonPublic | BindingFlags.Instance);
        skipField?.SetValue(mbl, false);
        mbl.ClearLastSelected();
        var entryField = typeof(MenuButtonList).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
        if (entryField == null)
        {
            LogMenuWarning("MenuButtonList entries field null");
            return;
        }
        var entryType = entryField.FieldType.GetElementType();
        if (entryType == null)
        {
            LogMenuWarning("MenuButtonList entry type null");
            return;
        }
        var arr = Array.CreateInstance(entryType, selectables.Count);
        for (int i = 0; i < selectables.Count; i++)
        {
            var e = Activator.CreateInstance(entryType);
            entryType.GetField("selectable", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(e, selectables[i]);
            arr.SetValue(e, i);
        }
        entryField.SetValue(mbl, arr);

        foreach (var selectable in selectables)
        {
            if (selectable is Selectable unitySelectable)
                SetAutomaticNavigation(unitySelectable);
        }

        if (ms.backButton != null)
            SetAutomaticNavigation(ms.backButton);

        mbl.SetupActive();
    }

    private static MenuSelectable GetPreferredHighlight(MenuScreen ms)
    {
        if (ms == null)
            return null;
        if (screenFirstSelectables.TryGetValue(ms, out var selectable) && selectable != null)
            return selectable;
        if (ms.backButton != null)
            return ms.backButton;
        return null;
    }

    private static MenuSelectable CreateMenuButton(Transform parent, MenuButton template, string label, System.Action onSubmit, CancelTarget cancelTarget)
    {
        if (template == null)
            return null;
        var templateImage = template.targetGraphic as Image;
        var go = Object.Instantiate(template.gameObject, parent, false);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;
        go.name = label.Replace(" ", string.Empty) + "Button";
        var goRect = go.GetComponent<RectTransform>();
        if (goRect != null)
        {
            goRect.anchorMin = new Vector2(0f, 0.5f);
            goRect.anchorMax = new Vector2(1f, 0.5f);
            goRect.pivot = new Vector2(0.5f, 0.5f);
            goRect.offsetMin = new Vector2(0f, goRect.offsetMin.y);
            goRect.offsetMax = new Vector2(0f, goRect.offsetMax.y);
            goRect.sizeDelta = new Vector2(0f, goRect.sizeDelta.y);
        }
        foreach (var auto in go.GetComponentsInChildren<AutoLocalizeTextUI>(true))
            Object.DestroyImmediate(auto);
        bool hasLabel = false;
        var text = go.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            text.gameObject.SetActive(true);
            ApplyTextStyle(text, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            text.text = label;
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
                    tmpType.GetProperty("text")?.SetValue(tmp, label);
                    tmpType.GetProperty("color")?.SetValue(tmp, Color.white);
                    if (tmp is Component tmpComp)
                    {
                        tmpComp.gameObject.SetActive(true);
                        var enabledProp = tmpType.GetProperty("enabled");
                        enabledProp?.SetValue(tmp, true);
                    }
                    hasLabel = true;
                }
            }
        }
        if (!hasLabel)
        {
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(go.transform, false);
            var fallback = labelObj.AddComponent<Text>();
            ApplyTextStyle(fallback, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            fallback.text = label;
            fallback.raycastTarget = false;
        }
        foreach (var cond in go.GetComponents<MenuButtonListCondition>())
            Object.DestroyImmediate(cond);
        var pauseMenuButton = go.GetComponent<PauseMenuButton>();
        if (pauseMenuButton != null)
            Object.DestroyImmediate(pauseMenuButton);
        SanitizeSelectableHierarchy(go);
        var btn = go.GetComponent<MenuButton>();
        if (btn == null)
        {
            Object.Destroy(go);
            return null;
        }
        if (goRect != null)
            goRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ButtonRowHeight);
        var image = go.GetComponent<Image>();
        if (image == null)
        {
            image = go.AddComponent<Image>();
        }
        if (image != null)
        {
            image.enabled = true;
            image.raycastTarget = true;
            if (templateImage != null && templateImage.sprite != null)
            {
                image.sprite = templateImage.sprite;
                image.type = templateImage.type;
                image.pixelsPerUnitMultiplier = templateImage.pixelsPerUnitMultiplier;
                image.preserveAspect = templateImage.preserveAspect;
                image.fillCenter = templateImage.fillCenter;
                image.maskable = templateImage.maskable;
                image.material = templateImage.material;
                image.useSpriteMesh = templateImage.useSpriteMesh;
                image.alphaHitTestMinimumThreshold = templateImage.alphaHitTestMinimumThreshold;
            }
            else if (image.sprite == null)
            {
                image.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsButtonBg", true);
                image.type = Image.Type.Sliced;
            }
            image.color = ButtonNormalColor;
        }
        if (btn != null)
        {
            btn.targetGraphic = image;
            ApplyButtonColors(btn);
        }
        btn.OnSubmitPressed.RemoveAllListeners();
        if (onSubmit != null)
            btn.OnSubmitPressed.AddListener(() => onSubmit());
        btn.cancelAction = CancelAction.DoNothing;
        var router = go.GetComponent<CancelRouter>() ?? go.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        btn.DontPlaySelectSound = true;
        var layout = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        layout.minHeight = ButtonRowHeight;
        layout.preferredHeight = ButtonRowHeight;
        layout.flexibleHeight = 0f;
        layout.minWidth = 0f;
        layout.preferredWidth = 0f;
        layout.flexibleWidth = 1f;
        SetAutomaticNavigation(btn);
        return btn;
    }

    private static MenuButton CreateDefaultMenuButtonTemplate()
    {
        var root = new GameObject("DefaultMenuButton");
        root.hideFlags = HideFlags.HideAndDontSave;
        var rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(460f, ButtonRowHeight);
        var image = root.AddComponent<Image>();
        var sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsButtonBg", true);
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = ButtonNormalColor;
        image.raycastTarget = true;
        var button = root.AddComponent<MenuButton>();
        button.targetGraphic = image;
        ApplyButtonColors(button);
        var layout = root.AddComponent<LayoutElement>();
        layout.minHeight = ButtonRowHeight;
        layout.preferredHeight = ButtonRowHeight;
        layout.flexibleHeight = 0f;
        layout.minWidth = 460f;
        layout.preferredWidth = 460f;
        layout.flexibleWidth = 0f;
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(root.transform, false);
        var text = labelObj.AddComponent<Text>();
        ApplyTextStyle(text, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        text.text = "Button";
        root.SetActive(false);
        return button;
    }

    private static void BuildMainMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;
        var selectables = new List<MenuSelectable>();
        var shadeToggle = CreateMenuButton(content, buttonTemplate, GetShadeToggleLabel(), null, CancelTarget.PauseMenu);
        if (shadeToggle is MenuButton toggleButton)
        {
            var driver = toggleButton.gameObject.AddComponent<ShadeToggleDriver>();
            driver.Initialize(toggleButton);
            selectables.Add(toggleButton);
        }
        else if (shadeToggle != null)
        {
            selectables.Add(shadeToggle);
        }
        if (difficultyScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Difficulty", () => ShowScreen(difficultyScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        if (controlsScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Controls", () => ShowScreen(controlsScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        if (loggingScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Logging", () => ShowScreen(loggingScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        SetupButtonList(ms, selectables);
        if (selectables.Count > 0)
        {
            var first = selectables[0];
            screenFirstSelectables[ms] = first;
            ms.defaultHighlight = first;
        }
        else if (ms.backButton != null)
        {
            screenFirstSelectables[ms] = ms.backButton;
            ms.defaultHighlight = ms.backButton;
        }
        ConfigureBackButton(ms, CancelTarget.PauseMenu, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private static void BuildDifficultyMenu(UIManager ui, MenuScreen ms, MenuSelectable sliderTemplate, MenuButton buttonTemplate)
    {
        if (ms == null || sliderTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;
        var selectables = new List<MenuSelectable>();
        void AddSlider(string label, float min, float max, float value, System.Action<float> onChange, bool whole = false)
        {
            var s = CreateSlider(content, sliderTemplate, buttonTemplate, label, min, max, value, onChange, CancelTarget.ShadeMain, whole);
            if (s != null) selectables.Add(s);
        }
        AddSlider("Hornet Damage", 0.2f, 2f, ModConfig.Instance.hornetDamageMultiplier, v => ModConfig.Instance.hornetDamageMultiplier = v);
        AddSlider("Shade Damage", 0.2f, 2f, ModConfig.Instance.shadeDamageMultiplier, v => ModConfig.Instance.shadeDamageMultiplier = v);
        AddSlider("Shade Heal (Bind)", 0f, 6f, ModConfig.Instance.bindShadeHeal, v => ModConfig.Instance.bindShadeHeal = Mathf.RoundToInt(v), true);
        AddSlider("Hornet Heal (Bind)", 0f, 6f, ModConfig.Instance.bindHornetHeal, v => ModConfig.Instance.bindHornetHeal = Mathf.RoundToInt(v), true);
        AddSlider("Shade Focus Heal", 0f, 6f, ModConfig.Instance.focusShadeHeal, v => ModConfig.Instance.focusShadeHeal = Mathf.RoundToInt(v), true);
        AddSlider("Hornet Focus Heal", 0f, 6f, ModConfig.Instance.focusHornetHeal, v => ModConfig.Instance.focusHornetHeal = Mathf.RoundToInt(v), true);
        SetupButtonList(ms, selectables);
        if (selectables.Count > 0)
        {
            var first = selectables[0];
            screenFirstSelectables[ms] = first;
            ms.defaultHighlight = first;
        }
        else if (ms.backButton != null)
        {
            screenFirstSelectables[ms] = ms.backButton;
            ms.defaultHighlight = ms.backButton;
        }
        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private static void BuildControlsMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        bindingDrivers.Clear();

        float horizontalMargin = Mathf.Clamp(Screen.width * 0.035f, 48f, 140f);
        float bottomMargin = Mathf.Clamp(Screen.height * 0.08f, 56f, 132f);
        float topMargin = Mathf.Clamp(Screen.height * 0.115f, 72f, 168f);

        if (content != null)
        {
            content.offsetMin = new Vector2(horizontalMargin, bottomMargin);
            content.offsetMax = new Vector2(-horizontalMargin, -topMargin);
            var anchored = content.anchoredPosition;
            anchored.y = Mathf.Clamp(Screen.height * 0.04f, 28f, 64f);
            content.anchoredPosition = anchored;
        }

        var info = new GameObject("ControlsInfo");
        info.transform.SetParent(content, false);
        var infoText = info.AddComponent<Text>();
        ApplyTextStyle(infoText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        infoText.text = "Select a binding to change it. Press Backspace to clear or press a controller button to bind.";
        ScaleTextElements(info, 0.85f);
        var infoLayout = info.AddComponent<LayoutElement>();
        infoLayout.preferredHeight = 48f;

        var selectables = new List<MenuSelectable>();
        var presetButtons = new List<MenuButton>();

        var presetRow = new GameObject("PresetOptions");
        var presetRect = presetRow.AddComponent<RectTransform>();
        presetRect.SetParent(content, false);
        presetRect.anchorMin = new Vector2(0f, 1f);
        presetRect.anchorMax = new Vector2(1f, 1f);
        presetRect.pivot = new Vector2(0.5f, 1f);
        presetRect.offsetMin = Vector2.zero;
        presetRect.offsetMax = Vector2.zero;
        var presetLayout = presetRow.AddComponent<HorizontalLayoutGroup>();
        float presetSpacing = Mathf.Clamp(Screen.width * 0.035f, 32f, 90f);
        int sidePadding = Mathf.RoundToInt(Mathf.Clamp(Screen.width * 0.04f, 36f, -80f));
        float presetCardPreferredWidth = Mathf.Clamp(Screen.width * 0.22f, 260f, 430f);
        float presetCardMinWidth = Mathf.Clamp(Screen.width * 0.16f, 200f, presetCardPreferredWidth);
        presetLayout.spacing = presetSpacing;
        presetLayout.padding = new RectOffset(sidePadding, sidePadding, 0, 0);
        presetLayout.childControlWidth = true;
        presetLayout.childControlHeight = false;
        presetLayout.childForceExpandWidth = true;
        presetLayout.childForceExpandHeight = false;
        presetLayout.childAlignment = TextAnchor.UpperCenter;
        var presetLayoutElement = presetRow.AddComponent<LayoutElement>();
        presetLayoutElement.minHeight = ButtonRowHeight * 1.75f;
        presetLayoutElement.preferredHeight = 0f;
        presetLayoutElement.flexibleHeight = 1f;

        void AddPresetOption(string label, string description, System.Action onSubmit)
        {
            var optionRoot = new GameObject(label.Replace(' ', '_'));
            var optionRect = optionRoot.AddComponent<RectTransform>();
            optionRect.SetParent(presetRow.transform, false);
            optionRect.anchorMin = new Vector2(0f, 1f);
            optionRect.anchorMax = new Vector2(1f, 1f);
            optionRect.pivot = new Vector2(0.5f, 1f);

            var optionLayout = optionRoot.AddComponent<VerticalLayoutGroup>();
            optionLayout.spacing = 18f;
            optionLayout.padding = new RectOffset(12, 12, 0, 0);
            optionLayout.childControlWidth = true;
            optionLayout.childControlHeight = false;
            optionLayout.childForceExpandWidth = true;
            optionLayout.childForceExpandHeight = false;
            optionLayout.childAlignment = TextAnchor.UpperCenter;

            var optionLayoutElement = optionRoot.AddComponent<LayoutElement>();
            optionLayoutElement.minWidth = presetCardMinWidth;
            optionLayoutElement.preferredWidth = presetCardPreferredWidth;
            optionLayoutElement.flexibleWidth = 1f;
            optionLayoutElement.flexibleHeight = 1f;

            var selectable = CreateMenuButton(optionRoot.transform, buttonTemplate, label, onSubmit, CancelTarget.ShadeMain);
            if (selectable is MenuButton button)
            {
                var layout = button.GetComponent<LayoutElement>();
                if (layout != null)
                {
                    float buttonPadding = optionLayout.padding.left + optionLayout.padding.right;
                    float buttonMinWidth = Mathf.Max(0f, presetCardMinWidth - buttonPadding);
                    float buttonPreferredWidth = Mathf.Max(buttonMinWidth, presetCardPreferredWidth - buttonPadding);
                    layout.minWidth = buttonMinWidth;
                    layout.preferredWidth = buttonPreferredWidth;
                    layout.flexibleWidth = 1f;
                }
                selectables.Add(button);
                presetButtons.Add(button);
            }
            else if (selectable != null)
            {
                selectables.Add(selectable);
            }

            var descriptionObject = new GameObject("Description");
            var descriptionRect = descriptionObject.AddComponent<RectTransform>();
            descriptionRect.SetParent(optionRoot.transform, false);
            descriptionRect.anchorMin = new Vector2(0f, 1f);
            descriptionRect.anchorMax = new Vector2(1f, 1f);
            descriptionRect.pivot = new Vector2(0.5f, 1f);
            var descriptionText = descriptionObject.AddComponent<Text>();
            ApplyTextStyle(descriptionText, sliderLabelStyle, TextAnchor.UpperCenter, Color.white);
            descriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descriptionText.verticalOverflow = VerticalWrapMode.Overflow;
            descriptionText.text = description;
            var descriptionLayout = descriptionObject.AddComponent<LayoutElement>();
            descriptionLayout.minWidth = presetCardMinWidth;
            descriptionLayout.preferredWidth = presetCardPreferredWidth;
            descriptionLayout.flexibleWidth = 0f;
            descriptionLayout.minHeight = 0f;
            descriptionLayout.preferredHeight = 0f;
            descriptionLayout.flexibleHeight = 1f;
            var fitter = descriptionObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScaleTextElements(descriptionObject, 0.64f);
        }

        AddPresetOption("Default", "Shade keeps the original keyboard layout. Hornet stays on controller and keyboard hotkeys stay disabled.", ApplyDefaultPreset);
        AddPresetOption("Two Controllers", "Shade uses the second controller with dedicated buttons while Hornet remains on the first controller.", ApplyDualControllerPresetOption);
        AddPresetOption("Keyboard Only", "Shade moves to the keypad while Hornet's controls jump to the left side of the keyboard. Controllers are disabled.", ApplyKeyboardOnlyPresetOption);
        AddPresetOption("Shade Controller", "Shade uses the first controller layout and Hornet swaps to left-side keyboard hotkeys with the controller disabled.", ApplyShadeControllerPresetOption);

        var bindingsContainer = new GameObject("BindingColumns");
        var bindingsRect = bindingsContainer.AddComponent<RectTransform>();
        bindingsRect.SetParent(content, false);
        var bindingsLayout = bindingsContainer.AddComponent<HorizontalLayoutGroup>();
        bindingsLayout.spacing = 32f;
        bindingsLayout.childControlWidth = true;
        bindingsLayout.childControlHeight = true;
        bindingsLayout.childForceExpandWidth = true;
        bindingsLayout.childForceExpandHeight = false;
        bindingsLayout.childAlignment = TextAnchor.UpperLeft;
        var bindingsLayoutElement = bindingsContainer.AddComponent<LayoutElement>();
        bindingsLayoutElement.minHeight = 0f;
        bindingsLayoutElement.preferredHeight = 0f;
        bindingsLayoutElement.flexibleHeight = 1f;

        RectTransform CreateBindingColumn(string name)
        {
            var column = new GameObject(name);
            var rect = column.AddComponent<RectTransform>();
            rect.SetParent(bindingsContainer.transform, false);
            var layout = column.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = ContentSpacing * 0.5f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            var columnLayout = column.AddComponent<LayoutElement>();
            columnLayout.minWidth = 0f;
            columnLayout.preferredWidth = 0f;
            columnLayout.flexibleWidth = 1f;
            return rect;
        }

        var leftColumn = CreateBindingColumn("LeftColumn");
        var rightColumn = CreateBindingColumn("RightColumn");

        void ConfigureBindingButton(MenuButton btn)
        {
            if (btn == null)
                return;
            ScaleTextElements(btn.gameObject, 0.85f);
            var layout = btn.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.minHeight = 70f;
                layout.preferredHeight = 70f;
            }
            var rect = btn.GetComponent<RectTransform>();
            if (rect != null)
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70f);
        }

        void AddBindingButton(Transform parent, ShadeAction action, string label, bool secondary)
        {
            var selectable = CreateMenuButton(parent, buttonTemplate, string.Empty, null, CancelTarget.ShadeMain);
            if (selectable is MenuButton btn)
            {
                var driver = btn.gameObject.AddComponent<BindingMenuDriver>();
                driver.Initialize(btn, action, secondary, label);
                ConfigureBindingButton(btn);
                selectables.Add(btn);
            }
        }

        void AddBindingRow(Transform parent, ShadeAction action, string label)
        {
            string primaryLabel = action == ShadeAction.Nail ? label + " (Primary)" : label;
            AddBindingButton(parent, action, primaryLabel, false);
            if (action == ShadeAction.Nail)
            {
                AddBindingButton(parent, action, label + " (Alt)", true);
            }
        }

        var bindingRows = new (ShadeAction action, string label)[]
        {
            (ShadeAction.MoveLeft, "Move Left"),
            (ShadeAction.MoveRight, "Move Right"),
            (ShadeAction.MoveUp, "Move Up"),
            (ShadeAction.MoveDown, "Move Down"),
            (ShadeAction.Nail, "Side Slash"),
            (ShadeAction.NailUp, "Up Slash"),
            (ShadeAction.NailDown, "Down Slash"),
            (ShadeAction.Fire, "Spellcast"),
            (ShadeAction.Teleport, "Teleport"),
            (ShadeAction.Focus, "Focus"),
            (ShadeAction.Sprint, "Sprint"),
            (ShadeAction.AssistMode, "Assist Mode")
        };

        int leftCount = (bindingRows.Length + 1) / 2;
        for (int i = 0; i < bindingRows.Length; i++)
        {
            var parent = i < leftCount ? leftColumn.transform : rightColumn.transform;
            AddBindingRow(parent, bindingRows[i].action, bindingRows[i].label);
        }

        SetupButtonList(ms, selectables);
        ConfigureHorizontalNavigation(presetButtons);
        if (selectables.Count > 0)
        {
            var first = selectables[0];
            screenFirstSelectables[ms] = first;
            ms.defaultHighlight = first;
        }
        else if (ms.backButton != null)
        {
            screenFirstSelectables[ms] = ms.backButton;
            ms.defaultHighlight = ms.backButton;
        }
        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private static void BuildLoggingMenu(UIManager ui, MenuScreen ms, MenuSelectable toggleTemplate, MenuButton buttonTemplate)
    {
        if (ms == null || toggleTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;
        var selectables = new List<MenuSelectable>();
        void AddToggle(string label, bool value, System.Action<bool> onChange)
        {
            var t = CreateToggle(content, toggleTemplate, buttonTemplate, label, value, onChange, CancelTarget.ShadeMain);
            if (t != null) selectables.Add(t);
        }
        AddToggle("General Logs", ModConfig.Instance.logGeneral, v => ModConfig.Instance.logGeneral = v);
        AddToggle("Menu Logs", ModConfig.Instance.logMenu, v => ModConfig.Instance.logMenu = v);
        AddToggle("Shade Debug Logs", ModConfig.Instance.logShade, v => ModConfig.Instance.logShade = v);
        AddToggle("HUD Debug Logs", ModConfig.Instance.logHud, v => ModConfig.Instance.logHud = v);
        AddToggle("Damage Summary File", ModConfig.Instance.logDamage, v => ModConfig.Instance.logDamage = v);
        SetupButtonList(ms, selectables);
        if (selectables.Count > 0)
        {
            var first = selectables[0];
            screenFirstSelectables[ms] = first;
            ms.defaultHighlight = first;
        }
        else if (ms.backButton != null)
        {
            screenFirstSelectables[ms] = ms.backButton;
            ms.defaultHighlight = ms.backButton;
        }
        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private static void EnsureBaseMenusHidden()
    {
        var ui = builtFor ?? UIManager.instance;
        if (ui == null)
            return;
        if (ui.pauseMenuScreen != null)
            ui.pauseMenuScreen.gameObject.SetActive(false);
        if (ui.optionsMenuScreen != null)
            ui.optionsMenuScreen.gameObject.SetActive(false);
        if (ui.gameOptionsMenuScreen != null)
            ui.gameOptionsMenuScreen.gameObject.SetActive(false);
    }

    private static void ShowScreen(MenuScreen target)
    {
        if (target == null)
            return;
        EnsureBaseMenusHidden();
        target.transform.SetAsLastSibling();
        var previous = activeScreen;
        foreach (var ms in allScreens)
        {
            if (ms == null)
                continue;
            bool show = ms == target;
            ms.gameObject.SetActive(show);
        }
        activeScreen = target;
        if (target == mainScreen)
        {
            if (previous != null && previous != mainScreen)
                consumeNextToggle = true;
            NotifyShadeToggleChanged();
        }
        else if (target != null && target != mainScreen)
        {
            consumeNextToggle = false;
        }
        var highlight = GetPreferredHighlight(target);
        if (highlight != null)
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(highlight.gameObject);
            UIManager.HighlightSelectableNoSound(highlight.GetFirstInteractable());
        }
    }

    private static void ShowMainMenu()
    {
        ShowScreen(mainScreen);
    }

    internal static bool HandlePauseToggle(UIManager ui)
    {
        if (consumeNextToggle)
        {
            consumeNextToggle = false;
            return true;
        }

        if (!IsShowing)
            return false;

        if (activeScreen != null && mainScreen != null && activeScreen != mainScreen)
        {
            ShowMainMenu();
            return true;
        }

        HideImmediate(ui, consumeToggle: false);
        return false;
    }

    private static void Build(UIManager ui)
    {
        if (!loggedBuildAttempt)
        {
            LogMenuInfo("Attempting to build Shade settings page");
            loggedBuildAttempt = true;
        }
        if (built && mainScreen != null && builtFor == ui)
        {
            LogMenuDebug("Settings page already built for this UI");
            return;
        }

        if ((mainScreen != null || allScreens.Count > 0) && builtFor != ui)
        {
            LogMenuDebug("UIManager changed, destroying previous settings page");
            DestroyScreens();
        }

        built = false;
        builtFor = ui;
        screenFirstSelectables.Clear();
        allScreens.Clear();
        activeScreen = null;

        var optionsScreen = ui.optionsMenuScreen;
        if (optionsScreen == null && !loggedMissingOptionsMenu)
        {
            LogMenuWarning("optionsMenuScreen not yet available; using pause menu as template");
            loggedMissingOptionsMenu = true;
        }
        var pauseScreen = ui.pauseMenuScreen;
        templateSource = optionsScreen != null ? optionsScreen.gameObject : (pauseScreen != null ? pauseScreen.gameObject : null);
        templateSourceWasActive = templateSource != null && templateSource.activeSelf;

        GameObject screenTemplate = pauseScreen != null ? pauseScreen.gameObject : templateSource;
        if (screenTemplate == null)
        {
            LogMenuWarning("Template screen not available; aborting build");
            return;
        }

        MenuSelectable sliderTemplate = null;
        if (optionsScreen != null)
        {
            foreach (var cand in optionsScreen.GetComponentsInChildren<MenuSelectable>(true))
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
                LogMenuWarning("slider template not found in options menu; using default");
                loggedMissingSliderTemplate = true;
            }
            sliderTemplate = CreateDefaultSliderTemplate();
            createdSliderTemplate = true;
        }

        MenuSelectable toggleTemplate = null;
        if (optionsScreen != null)
        {
            foreach (var cand in optionsScreen.GetComponentsInChildren<MenuSelectable>(true))
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
            toggleTemplate = CreateDefaultToggleTemplate();
            createdToggleTemplate = true;
        }

        CacheTextStyles(sliderTemplate, toggleTemplate);

        MenuButton buttonTemplate = null;
        bool createdButtonTemplate = false;
        if (optionsScreen != null)
        {
            foreach (var cand in optionsScreen.GetComponentsInChildren<MenuButton>(true))
            {
                if (optionsScreen.backButton != null && cand == optionsScreen.backButton)
                    continue;
                buttonTemplate = Object.Instantiate(cand.gameObject).GetComponent<MenuButton>();
                createdButtonTemplate = true;
                break;
            }
        }
        if (buttonTemplate == null)
        {
            var templateMenuScreen = screenTemplate.GetComponent<MenuScreen>();
            if (templateMenuScreen != null && templateMenuScreen.backButton != null)
            {
                buttonTemplate = Object.Instantiate(templateMenuScreen.backButton.gameObject).GetComponent<MenuButton>();
                createdButtonTemplate = true;
            }
        }
        if (buttonTemplate == null)
        {
            buttonTemplate = CreateDefaultMenuButtonTemplate();
            createdButtonTemplate = true;
        }
        if (buttonTemplate != null)
        {
            buttonTemplate.gameObject.hideFlags = HideFlags.HideAndDontSave;
            buttonTemplate.gameObject.SetActive(false);
        }

        Font preferredFont = FindFontInObject(buttonTemplate != null ? buttonTemplate.gameObject : null);
        if (preferredFont == null && pauseScreen != null)
            preferredFont = FindFontInObject(pauseScreen.gameObject);
        ApplyPreferredFont(preferredFont);

        mainScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
        difficultyScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
        controlsScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
        loggingScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();

        if (mainScreen != null)
        {
            mainScreen.gameObject.name = "ShadeSettingsMain";
            mainScreen.gameObject.SetActive(false);
            InitializeScreen(mainScreen);
            allScreens.Add(mainScreen);
        }
        if (difficultyScreen != null)
        {
            difficultyScreen.gameObject.name = "ShadeSettingsDifficulty";
            difficultyScreen.gameObject.SetActive(false);
            InitializeScreen(difficultyScreen);
            allScreens.Add(difficultyScreen);
        }
        if (controlsScreen != null)
        {
            controlsScreen.gameObject.name = "ShadeSettingsControls";
            controlsScreen.gameObject.SetActive(false);
            InitializeScreen(controlsScreen);
            allScreens.Add(controlsScreen);
        }
        if (loggingScreen != null)
        {
            loggingScreen.gameObject.name = "ShadeSettingsLogging";
            loggingScreen.gameObject.SetActive(false);
            InitializeScreen(loggingScreen);
            allScreens.Add(loggingScreen);
        }

        screen = mainScreen != null ? mainScreen.gameObject : null;

        BuildMainMenu(ui, mainScreen, buttonTemplate);
        BuildDifficultyMenu(ui, difficultyScreen, sliderTemplate, buttonTemplate);
        BuildControlsMenu(ui, controlsScreen, buttonTemplate);
        BuildLoggingMenu(ui, loggingScreen, toggleTemplate, buttonTemplate);

        if (createdSliderTemplate && sliderTemplate != null)
            Object.Destroy(sliderTemplate.gameObject);
        if (createdToggleTemplate && toggleTemplate != null)
            Object.Destroy(toggleTemplate.gameObject);
        if (createdButtonTemplate && buttonTemplate != null)
            Object.Destroy(buttonTemplate.gameObject);

        built = true;
        LogMenuInfo("Shade settings page built");
    }

    internal static void Inject(UIManager ui)
    {
        if (ui == null)
        {
            if (!loggedNullUI)
            {
                LogMenuWarning("Inject called with null UIManager");
                loggedNullUI = true;
            }
            return;
        }
        if (ui.pauseMenuScreen == null)
        {
            if (!loggedNoPauseMenu)
            {
                LogMenuWarning("pauseMenuScreen not yet available");
                loggedNoPauseMenu = true;
            }
            return;
        }

        // Ensure a screen exists for this UI
        Build(ui);
        if (mainScreen == null)
            return;

        // Avoid duplicate buttons by scanning entire hierarchy
        foreach (Transform child in ui.pauseMenuScreen.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "ShadeSettingsButton")
            {
                if (!loggedButtonAlreadyPresent)
                {
                    LogMenuInfo("ShadeSettingsButton already present; skipping injection");
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
                LogMenuWarning("No PauseMenuButton templates found");
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
                LogMenuWarning("MenuButtonList not found on template parent");
                loggedNoMenuButtonList = true;
            }
            return;
        }
        var templateTargetImage = template.targetGraphic as Image;
        var field = typeof(MenuButtonList).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
        var entries = (Array)field.GetValue(list);
        if (entries == null)
        {
            if (!loggedNullEntries)
            {
                LogMenuWarning("MenuButtonList entries field null");
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
            ApplyTextStyle(txt, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
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
            ApplyTextStyle(t, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            t.text = "Legacy of the Abyss";
            t.color = Color.white;
        }

        var background = go.GetComponent<Image>();
        if (background == null)
            background = go.AddComponent<Image>();
        if (background != null)
        {
            background.enabled = true;
            background.raycastTarget = true;
            if (templateTargetImage != null && templateTargetImage.sprite != null)
            {
                background.sprite = templateTargetImage.sprite;
                background.type = templateTargetImage.type;
                background.pixelsPerUnitMultiplier = templateTargetImage.pixelsPerUnitMultiplier;
                background.preserveAspect = templateTargetImage.preserveAspect;
                background.fillCenter = templateTargetImage.fillCenter;
                background.maskable = templateTargetImage.maskable;
                background.material = templateTargetImage.material;
                background.useSpriteMesh = templateTargetImage.useSpriteMesh;
                background.alphaHitTestMinimumThreshold = templateTargetImage.alphaHitTestMinimumThreshold;
            }
            else if (background.sprite == null)
            {
                background.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsButtonBg", true);
                background.type = Image.Type.Sliced;
            }
            background.color = ButtonNormalColor;
        }

        var goLayout = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        goLayout.minHeight = ButtonRowHeight;
        goLayout.preferredHeight = ButtonRowHeight;
        goLayout.flexibleHeight = 0f;
        goLayout.flexibleWidth = 0f;
        var goRect = go.GetComponent<RectTransform>();
        if (goRect != null)
            goRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ButtonRowHeight);

        var pauseBtn = go.GetComponent<PauseMenuButton>();

        var pauseSelectable = go.GetComponent<Selectable>();
        if (pauseSelectable != null)
        {
            pauseSelectable.targetGraphic = background;
            ApplyButtonColors(pauseSelectable);
            SetAutomaticNavigation(pauseSelectable);
        }

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
        LogMenuInfo("Injected ShadeSettingsButton into pause menu");
    }

    internal static IEnumerator Show(UIManager ui)
    {
        Build(ui);
        consumeNextToggle = false;
        if (mainScreen == null)
        {
            LogMenuWarning("Show called but main screen is null");
            yield break;
        }

        LogMenuInfo("Showing Shade settings page");
        bool templateWasActive = templateSource != null && templateSource.activeSelf;
        if (ui.pauseMenuScreen != null)
        {
            pauseMenuWasActive = ui.pauseMenuScreen.gameObject.activeSelf;
            ui.pauseMenuScreen.gameObject.SetActive(false);
        }
        if (ui.optionsMenuScreen != null)
        {
            optionsMenuWasActive = ui.optionsMenuScreen.gameObject.activeSelf;
            ui.optionsMenuScreen.gameObject.SetActive(false);
        }
        if (ui.gameOptionsMenuScreen != null)
        {
            gameOptionsMenuWasActive = ui.gameOptionsMenuScreen.gameObject.activeSelf;
            var cg = ui.gameOptionsMenuScreen.ScreenCanvasGroup;
            if (cg != null)
            {
                storedGameOptionsCanvasState = true;
                storedGameOptionsAlpha = cg.alpha;
                storedGameOptionsInteractable = cg.interactable;
                storedGameOptionsBlocksRaycasts = cg.blocksRaycasts;
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            else
            {
                storedGameOptionsCanvasState = false;
            }
            ui.gameOptionsMenuScreen.gameObject.SetActive(false);
        }
        else
        {
            storedGameOptionsCanvasState = false;
        }
        if (templateSource != null)
        {
            templateSourceWasActive = templateWasActive;
            templateSource.SetActive(false);
        }
        ShowScreen(mainScreen);
        yield break;
    }

    internal static void HideImmediate(UIManager ui, bool consumeToggle = true)
    {
        consumeNextToggle = consumeToggle;
        if (allScreens.Count == 0)
            return;
        LogMenuInfo("Hiding Shade settings page");
        foreach (var ms in allScreens)
        {
            if (ms != null)
                ms.gameObject.SetActive(false);
        }
        activeScreen = null;
        var targetUi = ui ?? UIManager.instance;
        if (targetUi != null)
        {
            if (targetUi.pauseMenuScreen != null)
                targetUi.pauseMenuScreen.gameObject.SetActive(pauseMenuWasActive);
            if (targetUi.optionsMenuScreen != null)
                targetUi.optionsMenuScreen.gameObject.SetActive(optionsMenuWasActive);
            if (targetUi.gameOptionsMenuScreen != null)
            {
                targetUi.gameOptionsMenuScreen.gameObject.SetActive(gameOptionsMenuWasActive);
                if (storedGameOptionsCanvasState)
                {
                    var cg = targetUi.gameOptionsMenuScreen.ScreenCanvasGroup;
                    if (cg != null)
                    {
                        cg.alpha = storedGameOptionsAlpha;
                        cg.interactable = storedGameOptionsInteractable;
                        cg.blocksRaycasts = storedGameOptionsBlocksRaycasts;
                    }
                }
            }
            if (templateSource != null)
                templateSource.SetActive(templateSourceWasActive);
            try
            {
                targetUi.UIGoToPauseMenu();
            }
            catch (Exception e)
            {
                LogMenuWarning($"Failed to navigate back to pause menu: {e}");
            }
        }
        pauseMenuWasActive = false;
        optionsMenuWasActive = false;
        gameOptionsMenuWasActive = false;
        storedGameOptionsCanvasState = false;
        ModConfig.Save();
    }

    internal static IEnumerator Hide(UIManager ui)
    {
        HideImmediate(ui);
        yield break;
    }

    internal static void Clear()
    {
        DestroyScreens();
        built = false;
        builtFor = null;
        sliderLabelStyle = null;
        sliderValueStyle = null;
        toggleLabelStyle = null;
        fallbackFont = null;
        storedGameOptionsCanvasState = false;
    }
}
#nullable restore
