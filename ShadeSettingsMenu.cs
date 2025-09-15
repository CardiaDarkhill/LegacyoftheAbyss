using System;
using System.Collections;
using System.Reflection;
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
    private static Selectable firstSelectable;
    private static readonly ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("ShadeSettingsMenu");
    private static bool loggedBuildAttempt;
    private static bool loggedMissingOptionsMenu;
    private static bool loggedMissingSliderTemplate;
    private static bool loggedMissingToggleTemplate;
    private static bool loggedMissingTemplates;
    private static bool loggedNullUI;
    private static bool loggedNoPauseMenu;
    private static bool loggedButtonAlreadyPresent;
    private static bool loggedNoPauseButtonTemplates;
    private static bool loggedNoMenuButtonList;
    private static bool loggedNullEntries;

    private static Slider CreateSlider(Transform parent, Slider template, string label, float min, float max, float value, System.Action<float> onChange, bool whole = false)
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
            Object.DestroyImmediate(t.gameObject);
        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType != null)
        {
            var tmps = go.GetComponentsInChildren(tmpType, true);
            foreach (var tmp in tmps)
                Object.DestroyImmediate(tmp.gameObject);
        }

        var slider = go.GetComponentInChildren<Slider>(true);
        Object.DestroyImmediate(slider.GetComponent<MenuAudioSlider>());
        Object.DestroyImmediate(slider.GetComponent<MenuPreventDeselect>());
        slider.onValueChanged.RemoveAllListeners();
        Object.DestroyImmediate(go.GetComponent<MenuButton>());
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

        return slider;
    }

    private static Toggle CreateToggle(Transform parent, Toggle template, string label, bool value, System.Action<bool> onChange)
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
            Object.DestroyImmediate(t.gameObject);
        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType != null)
        {
            var tmps = go.GetComponentsInChildren(tmpType, true);
            foreach (var tmp in tmps)
                Object.DestroyImmediate(tmp.gameObject);
        }

        var toggle = go.GetComponentInChildren<Toggle>(true);
        Object.DestroyImmediate(toggle.GetComponent<MenuPreventDeselect>());
        Object.DestroyImmediate(go.GetComponent<MenuButton>());
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

        return toggle;
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
        var templateScreen = optionsScreen != null ? optionsScreen.gameObject : ui.pauseMenuScreen?.gameObject;
        if (templateScreen == null)
        {
            if (!loggedMissingOptionsMenu)
            {
                log.LogWarning("optionsMenuScreen missing and no pauseMenuScreen available");
                loggedMissingOptionsMenu = true;
            }
            return;
        }

        if (optionsScreen == null && !loggedMissingOptionsMenu)
        {
            log.LogWarning("optionsMenuScreen missing; using pause menu as template");
            loggedMissingOptionsMenu = true;
        }

        var sliderTemplate = optionsScreen != null ? optionsScreen.GetComponentInChildren<Slider>(true) : null;
        if (sliderTemplate == null)
        {
            if (!loggedMissingSliderTemplate)
            {
                log.LogWarning("slider template not found in options menu; using default");
                loggedMissingSliderTemplate = true;
            }
            var sliders = Resources.FindObjectsOfTypeAll<Slider>();
            if (sliders != null && sliders.Length > 0)
                sliderTemplate = sliders[0];
            if (sliderTemplate == null)
            {
                var sliderGO = DefaultControls.CreateSlider(new DefaultControls.Resources());
                sliderGO.SetActive(false);
                sliderTemplate = sliderGO.GetComponent<Slider>();
            }
        }

        var toggleTemplate = optionsScreen != null ? optionsScreen.GetComponentInChildren<Toggle>(true) : null;
        if (toggleTemplate == null)
        {
            if (!loggedMissingToggleTemplate)
            {
                log.LogWarning("toggle template not found in options menu; using default");
                loggedMissingToggleTemplate = true;
            }
            var toggles = Resources.FindObjectsOfTypeAll<Toggle>();
            if (toggles != null && toggles.Length > 0)
                toggleTemplate = toggles[0];
            if (toggleTemplate == null)
            {
                var toggleGO = DefaultControls.CreateToggle(new DefaultControls.Resources());
                toggleGO.SetActive(false);
                toggleTemplate = toggleGO.GetComponent<Toggle>();
            }
        }

        if (sliderTemplate == null || toggleTemplate == null)
        {
            if (!loggedMissingTemplates)
            {
                log.LogError("required templates missing; cannot build settings page");
                loggedMissingTemplates = true;
            }
            return;
        }

        screen = Object.Instantiate(templateScreen, templateScreen.transform.parent);
        screen.name = "ShadeSettingsPage";
        screen.SetActive(false);
        log.LogDebug("Instantiated ShadeSettingsPage");

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

        firstSelectable = CreateSlider(content.transform, sliderTemplate, "Hornet Damage", 0.2f, 2f, ModConfig.Instance.hornetDamageMultiplier, v => ModConfig.Instance.hornetDamageMultiplier = v);
        CreateSlider(content.transform, sliderTemplate, "Shade Damage", 0.2f, 2f, ModConfig.Instance.shadeDamageMultiplier, v => ModConfig.Instance.shadeDamageMultiplier = v);
        CreateSlider(content.transform, sliderTemplate, "Shade Heal (Bind)", 0f, 6f, ModConfig.Instance.bindShadeHeal, v => ModConfig.Instance.bindShadeHeal = Mathf.RoundToInt(v), true);
        CreateSlider(content.transform, sliderTemplate, "Hornet Heal (Bind)", 0f, 6f, ModConfig.Instance.bindHornetHeal, v => ModConfig.Instance.bindHornetHeal = Mathf.RoundToInt(v), true);
        CreateSlider(content.transform, sliderTemplate, "Shade Focus Heal", 0f, 6f, ModConfig.Instance.focusShadeHeal, v => ModConfig.Instance.focusShadeHeal = Mathf.RoundToInt(v), true);
        CreateSlider(content.transform, sliderTemplate, "Hornet Focus Heal", 0f, 6f, ModConfig.Instance.focusHornetHeal, v => ModConfig.Instance.focusHornetHeal = Mathf.RoundToInt(v), true);
        CreateToggle(content.transform, toggleTemplate, "Damage Logging", ModConfig.Instance.logDamage, v => ModConfig.Instance.logDamage = v);

        if (ms.backButton != null)
        {
            log.LogDebug("Wiring back button");
            ms.backButton.OnSubmitPressed.AddListener(() => ui.StartCoroutine(Hide(ui)));
        }

        var mbl = screen.GetComponent<MenuButtonList>();
        if (mbl != null)
        {
            log.LogDebug("Removing inherited MenuButtonList from settings page");
            Object.DestroyImmediate(mbl);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

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
