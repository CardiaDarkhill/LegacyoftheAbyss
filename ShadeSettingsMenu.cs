using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using BepInEx.Logging;

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

    private static Slider CreateDefaultSliderTemplate()
    {
        var go = new GameObject("DefaultSlider");
        go.hideFlags = HideFlags.HideAndDontSave;
        var rt = go.AddComponent<RectTransform>();

        var background = new GameObject("Background");
        background.transform.SetParent(go.transform, false);
        var bgImage = background.AddComponent<Image>();
        bgImage.color = Color.white;

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRt.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRt.sizeDelta = new Vector2(-20f, 0f);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        // leave sprites null to avoid resource load errors

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        var handleAreaRt = handleArea.AddComponent<RectTransform>();
        handleAreaRt.anchorMin = new Vector2(0f, 0f);
        handleAreaRt.anchorMax = new Vector2(1f, 1f);
        handleAreaRt.sizeDelta = new Vector2(-20f, 0f);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        // leave knob sprite null

        var slider = go.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;

        go.AddComponent<MenuSelectable>();
        go.SetActive(false);
        return slider;
    }

    private static Toggle CreateDefaultToggleTemplate()
    {
        var go = new GameObject("DefaultToggle");
        go.hideFlags = HideFlags.HideAndDontSave;
        var rt = go.AddComponent<RectTransform>();

        var background = new GameObject("Background");
        background.transform.SetParent(go.transform, false);
        var bgImage = background.AddComponent<Image>();
        // background sprite left null

        var checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        var checkImg = checkmark.AddComponent<Image>();
        // no checkmark sprite

        var toggle = go.AddComponent<Toggle>();
        toggle.graphic = checkImg;
        toggle.targetGraphic = bgImage;

        go.AddComponent<MenuSelectable>();
        go.SetActive(false);
        return toggle;
    }

    private static MenuSelectable CreateSlider(Transform parent, Slider template, string label, float min, float max, float value, System.Action<float> onChange, bool whole = false)
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
        hLayout.childControlWidth = false;
        hLayout.childForceExpandHeight = false;
        hLayout.childForceExpandWidth = false;
        hLayout.spacing = 20f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        labelTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelTxt.text = label;
        labelTxt.color = Color.white;
        labelTxt.alignment = TextAnchor.MiddleLeft;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.preferredWidth = 250f;

        // slider instance
        var go = Object.Instantiate(template.gameObject, row.transform, false);
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

        var slider = go.GetComponentInChildren<Slider>(true);
        Object.DestroyImmediate(slider.GetComponent<MenuAudioSlider>());
        Object.DestroyImmediate(slider.GetComponent<MenuPreventDeselect>());
        slider.onValueChanged.RemoveAllListeners();
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var sliderLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        sliderLe.preferredWidth = 400f;

        // value text to the right of slider
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(row.transform, false);
        var valueTxt = valueObj.AddComponent<Text>();
        valueTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        valueTxt.color = Color.white;
        valueTxt.alignment = TextAnchor.MiddleRight;
        var valueLe = valueObj.AddComponent<LayoutElement>();
        valueLe.preferredWidth = 80f;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = whole;
        slider.SetValueWithoutNotify(value);
        valueTxt.text = whole ? Mathf.RoundToInt(value).ToString() : value.ToString("0.00");
        slider.onValueChanged.AddListener(v =>
        {
            onChange.Invoke(v);
            valueTxt.text = whole ? Mathf.RoundToInt(v).ToString() : v.ToString("0.00");
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
        return selectable;
    }

    private static MenuSelectable CreateToggle(Transform parent, Toggle template, string label, bool value, System.Action<bool> onChange)
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
        hLayout.childControlWidth = false;
        hLayout.childForceExpandHeight = false;
        hLayout.childForceExpandWidth = false;
        hLayout.spacing = 20f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        labelTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelTxt.text = label;
        labelTxt.color = Color.white;
        labelTxt.alignment = TextAnchor.MiddleLeft;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.preferredWidth = 250f;

        // toggle instance
        var go = Object.Instantiate(template.gameObject, row.transform, false);
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

        var toggle = go.GetComponentInChildren<Toggle>(true);
        Object.DestroyImmediate(toggle.GetComponent<MenuPreventDeselect>());
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var toggleLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        toggleLe.preferredWidth = 60f;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.isOn = value;
        toggle.onValueChanged.AddListener(onChange.Invoke);

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

        var sliderTemplate = optionsScreen != null ? optionsScreen.GetComponentInChildren<Slider>(true) : null;
        bool createdSliderTemplate = false;
        if (sliderTemplate == null || sliderTemplate.GetComponent<MenuSelectable>() == null)
        {
            if (!loggedMissingSliderTemplate)
            {
                log.LogWarning("slider template not found in options menu; using default");
                loggedMissingSliderTemplate = true;
            }
            sliderTemplate = CreateDefaultSliderTemplate();
            createdSliderTemplate = true;
        }

        var toggleTemplate = optionsScreen != null ? optionsScreen.GetComponentInChildren<Toggle>(true) : null;
        bool createdToggleTemplate = false;
        if (toggleTemplate == null || toggleTemplate.GetComponent<MenuSelectable>() == null)
        {
            if (!loggedMissingToggleTemplate)
            {
                log.LogWarning("toggle template not found in options menu; using default");
                loggedMissingToggleTemplate = true;
            }
            toggleTemplate = CreateDefaultToggleTemplate();
            createdToggleTemplate = true;
        }

        screen = Object.Instantiate(templateScreen, templateScreen.transform.parent);
        screen.name = "ShadeSettingsPage";
        screen.SetActive(false);
        log.LogDebug("Instantiated ShadeSettingsPage");

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
        layout.spacing = 15f;

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
            firstSelectable = selectables[0];

        if (ms.backButton != null)
        {
            log.LogDebug("Wiring back button");
            ms.backButton.OnSubmitPressed.AddListener(() => ui.StartCoroutine(Hide(ui)));
        }

        var mbl = screen.GetComponent<MenuButtonList>() ?? screen.AddComponent<MenuButtonList>();
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
        var txt = go.GetComponentInChildren<Text>(true);
        if (txt != null)
        {
            txt.text = "Legacy of the Abyss";
            txt.color = Color.white;
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
                    return;
                }
            }

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
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }
        yield break;
    }

    internal static IEnumerator Hide(UIManager ui)
    {
        log.LogInfo("Hiding Shade settings page");
        screen.SetActive(false);
        ui.pauseMenuScreen.gameObject.SetActive(true);
        ModConfig.Save();
        yield break;
    }
}
