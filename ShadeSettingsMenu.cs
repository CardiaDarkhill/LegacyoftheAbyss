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
        var go = Object.Instantiate(template.gameObject, parent);
        go.name = label + "Slider";
        var txt = go.GetComponentInChildren<Text>(true);
        if (txt != null)
        {
            txt.text = label;
            txt.color = Color.white;
        }
        var slider = go.GetComponentInChildren<Slider>(true);
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        slider.wholeNumbers = whole;
        slider.onValueChanged.AddListener(onChange.Invoke);
        return slider;
    }

    private static Toggle CreateToggle(Transform parent, Toggle template, string label, bool value, System.Action<bool> onChange)
    {
        var go = Object.Instantiate(template.gameObject, parent);
        go.name = label + "Toggle";
        var txt = go.GetComponentInChildren<Text>(true);
        if (txt != null)
        {
            txt.text = label;
            txt.color = Color.white;
        }
        var toggle = go.GetComponentInChildren<Toggle>(true);
        toggle.isOn = value;
        toggle.onValueChanged.AddListener(onChange.Invoke);
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
                log.LogWarning("slider template not found in options menu, searching globally");
                loggedMissingSliderTemplate = true;
            }
            var sliders = Resources.FindObjectsOfTypeAll<Slider>();
            if (sliders != null && sliders.Length > 0)
                sliderTemplate = sliders[0];
        }

        var toggleTemplate = optionsScreen != null ? optionsScreen.GetComponentInChildren<Toggle>(true) : null;
        if (toggleTemplate == null)
        {
            if (!loggedMissingToggleTemplate)
            {
                log.LogWarning("toggle template not found in options menu, searching globally");
                loggedMissingToggleTemplate = true;
            }
            var toggles = Resources.FindObjectsOfTypeAll<Toggle>();
            if (toggles != null && toggles.Length > 0)
                toggleTemplate = toggles[0];
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

        built = true;
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
        content.transform.SetParent(ms.transform, false);
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.spacing = 10f;

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
            Object.Destroy(mbl);
        }

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
        var template = buttons[buttons.Length - 1];

        var list = template.GetComponentInParent<MenuButtonList>();
        if (list == null)
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
        Object.Destroy(go.GetComponentInChildren<AutoLocalizeTextUI>());
        var txt = go.GetComponentInChildren<Text>(true);
        if (txt != null)
        {
            txt.text = "Legacy of the Abyss";
            txt.color = Color.white;
        }

        var pauseBtn = go.GetComponent<PauseMenuButton>();

        foreach (var cond in go.GetComponents<MenuButtonListCondition>())
            Object.Destroy(cond);

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
