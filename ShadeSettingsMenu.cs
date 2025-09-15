using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ShadeSettingsMenu
{
    private static GameObject screen;
    private static bool built;

    private static Slider CreateSlider(Transform parent, string label, float min, float max, float value, System.Action<float> onChange, bool whole = false)
    {
        var container = new GameObject(label + "Container");
        container.transform.SetParent(parent, false);
        var layout = container.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        var textGo = new GameObject(label + "Label");
        textGo.transform.SetParent(container.transform, false);
        var txt = textGo.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter;

        var sliderGo = new GameObject(label + "Slider");
        sliderGo.transform.SetParent(container.transform, false);
        var slider = sliderGo.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        slider.wholeNumbers = whole;
        slider.onValueChanged.AddListener(onChange.Invoke);
        return slider;
    }

    private static Toggle CreateToggle(Transform parent, string label, bool value, System.Action<bool> onChange)
    {
        var go = new GameObject(label + "Toggle");
        go.transform.SetParent(parent, false);
        var toggle = go.AddComponent<Toggle>();
        toggle.isOn = value;
        toggle.onValueChanged.AddListener(onChange.Invoke);
        var txt = go.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return toggle;
    }

    private static void Build(UIManager ui)
    {
        if (built) return;
        built = true;
        screen = new GameObject("ShadeSettingsPage");
        screen.transform.SetParent(ui.UICanvas.transform, false);
        screen.SetActive(false);
        var cg = screen.AddComponent<CanvasGroup>();
        cg.interactable = true;
        cg.blocksRaycasts = true;
        var layout = screen.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        CreateSlider(screen.transform, "Hornet Damage", 0.2f, 2f, ModConfig.Instance.hornetDamageMultiplier, v => ModConfig.Instance.hornetDamageMultiplier = v);
        CreateSlider(screen.transform, "Shade Damage", 0.2f, 2f, ModConfig.Instance.shadeDamageMultiplier, v => ModConfig.Instance.shadeDamageMultiplier = v);
        CreateSlider(screen.transform, "Shade Heal (Bind)", 0f, 6f, ModConfig.Instance.bindShadeHeal, v => ModConfig.Instance.bindShadeHeal = Mathf.RoundToInt(v), true);
        CreateSlider(screen.transform, "Hornet Heal (Bind)", 0f, 6f, ModConfig.Instance.bindHornetHeal, v => ModConfig.Instance.bindHornetHeal = Mathf.RoundToInt(v), true);
        CreateSlider(screen.transform, "Shade Focus Heal", 0f, 6f, ModConfig.Instance.focusShadeHeal, v => ModConfig.Instance.focusShadeHeal = Mathf.RoundToInt(v), true);
        CreateSlider(screen.transform, "Hornet Focus Heal", 0f, 6f, ModConfig.Instance.focusHornetHeal, v => ModConfig.Instance.focusHornetHeal = Mathf.RoundToInt(v), true);
        CreateToggle(screen.transform, "Damage Logging", ModConfig.Instance.logDamage, v => ModConfig.Instance.logDamage = v);

        var backGo = new GameObject("BackButton");
        backGo.transform.SetParent(screen.transform, false);
        var back = backGo.AddComponent<Button>();
        var backText = backGo.AddComponent<Text>();
        backText.text = "Back";
        backText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        back.onClick.AddListener(() => ui.StartCoroutine(Hide(ui)));
    }

    internal static void Inject(UIManager ui)
    {
        if (ui == null || ui.pauseMenuScreen == null) return;
        Build(ui);
        if (ui.pauseMenuScreen.GetComponentInChildren<PauseMenuButton>(true) == null) return;
        var buttons = ui.pauseMenuScreen.GetComponentsInChildren<PauseMenuButton>(true);
        var template = buttons[buttons.Length - 1];
        var go = Object.Instantiate(template.gameObject, template.transform.parent);
        go.name = "ShadeSettingsButton";
        var txt = go.GetComponentInChildren<Text>();
        if (txt) txt.text = "Shade Settings";
    }

    internal static IEnumerator Show(UIManager ui)
    {
        Build(ui);
        ui.pauseMenuScreen.gameObject.SetActive(false);
        screen.SetActive(true);
        yield break;
    }

    internal static IEnumerator Hide(UIManager ui)
    {
        screen.SetActive(false);
        ui.pauseMenuScreen.gameObject.SetActive(true);
        ModConfig.Save();
        yield break;
    }
}
