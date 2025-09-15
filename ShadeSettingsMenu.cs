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
        log.LogDebug("Build called");
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
        if (ui.optionsMenuScreen == null)
        {
            log.LogDebug("optionsMenuScreen missing");
            return;
        }

        var sliderTemplate = ui.optionsMenuScreen.GetComponentInChildren<Slider>(true);
        if (sliderTemplate == null)
        {
            log.LogDebug("slider template not found in options menu, searching globally");
            sliderTemplate = Object.FindObjectOfType<Slider>(true);
        }

        var toggleTemplate = ui.optionsMenuScreen.GetComponentInChildren<Toggle>(true);
        if (toggleTemplate == null)
        {
            log.LogDebug("toggle template not found in options menu, searching globally");
            toggleTemplate = Object.FindObjectOfType<Toggle>(true);
        }

        if (sliderTemplate == null || toggleTemplate == null)
        {
            log.LogDebug("required templates missing; cannot build settings page");
            return;
        }

        built = true;
        var templateScreen = ui.optionsMenuScreen.gameObject;
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
            log.LogDebug("Inject called with null UIManager");
            return;
        }
        if (ui.pauseMenuScreen == null)
        {
            log.LogDebug("pauseMenuScreen not yet available");
            return;
        }

        // Ensure a screen exists for this UI
        Build(ui);

        // Avoid duplicate buttons by scanning entire hierarchy
        foreach (Transform child in ui.pauseMenuScreen.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "ShadeSettingsButton")
            {
                log.LogDebug("ShadeSettingsButton already present; skipping injection");
                return;
            }
        }

        var buttons = ui.pauseMenuScreen.GetComponentsInChildren<PauseMenuButton>(true);
        if (buttons.Length == 0)
        {
            log.LogDebug("No PauseMenuButton templates found");
            return;
        }
        var template = buttons[buttons.Length - 1];

        var list = template.transform.parent.GetComponent<MenuButtonList>();
        if (list == null)
        {
            log.LogDebug("MenuButtonList not found on template parent");
            return;
        }
        var field = typeof(MenuButtonList).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
        var entries = (Array)field.GetValue(list);
        if (entries == null)
        {
            log.LogDebug("MenuButtonList entries field null");
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
        Animator flash = null;
        if (pauseBtn != null)
        {
            flash = pauseBtn.flashEffect;
            Object.DestroyImmediate(pauseBtn);
        }

        foreach (var cond in go.GetComponents<MenuButtonListCondition>())
            Object.Destroy(cond);

        var menuBtn = go.AddComponent<MenuButton>();
        if (flash != null)
            menuBtn.flashEffect = flash;
        // Ensure event exists before adding listener to avoid null reference
        if (menuBtn.OnSubmitPressed == null)
            menuBtn.OnSubmitPressed = new UnityEngine.Events.UnityEvent();
        menuBtn.OnSubmitPressed.AddListener(() => ui.StartCoroutine(Show(ui)));

        var entryType = entries.GetType().GetElementType();
        var newEntry = Activator.CreateInstance(entryType);
        var selField = entryType.GetField("selectable", BindingFlags.NonPublic | BindingFlags.Instance);
        selField.SetValue(newEntry, menuBtn);
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
            log.LogDebug("Show called but screen is null");
            yield break;
        }

        log.LogDebug("Showing Shade settings page");
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
        log.LogDebug("Hiding Shade settings page");
        screen.SetActive(false);
        ui.pauseMenuScreen.gameObject.SetActive(true);
        ModConfig.Save();
        yield break;
    }
}
