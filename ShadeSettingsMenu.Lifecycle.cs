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
    private static void Build(UIManager ui)
    {
        if (!loggedBuildAttempt)
        {
            LogMenuInfo("Attempting to build Shade settings page");
            loggedBuildAttempt = true;
        }
        bool debugKeysEnabled = ModConfig.Instance.debugKeysEnabled;
        if (built && mainScreen != null && builtFor == ui && lastBuiltDebugKeysEnabled == debugKeysEnabled)
        {
            LogMenuDebug("Settings page already built for this UI");
            return;
        }

        if (mainScreen != null || allScreens.Count > 0)
        {
            LogMenuDebug(builtFor != ui
                ? "UIManager changed, destroying previous settings page"
                : "Debug keys setting changed, rebuilding settings page");
            DestroyScreens();
        }

        built = false;
        builtFor = ui;
        lastBuiltDebugKeysEnabled = debugKeysEnabled;
        screenFirstSelectables.Clear();
        screenLastSelectables.Clear();
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
        if (IncludeLegacyCharmMenu)
        {
            charmsScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
        }
        else
        {
            charmsScreen = null;
        }
        skinsScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
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
        if (charmsScreen != null)
        {
            charmsScreen.gameObject.name = "ShadeSettingsCharms";
            charmsScreen.gameObject.SetActive(false);
            InitializeScreen(charmsScreen);
            allScreens.Add(charmsScreen);
        }
        if (skinsScreen != null)
        {
            skinsScreen.gameObject.name = "ShadeSettingsSkins";
            skinsScreen.gameObject.SetActive(false);
            InitializeScreen(skinsScreen);
            allScreens.Add(skinsScreen);
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
        if (IncludeLegacyCharmMenu && charmsScreen != null)
            BuildCharmsMenu(ui, charmsScreen, buttonTemplate);
        BuildSkinsMenu(ui, skinsScreen, buttonTemplate);
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
        // Fast path: already injected into this UIManager. Inject is polled every frame, and
        // everything below it allocates (hierarchy scans, Unity's .name getter), so bail first.
        if (ui != null && injectedFor == ui)
            return;

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
                injectedFor = ui;
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
        injectedFor = ui;
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
        injectedFor = null;
        sliderLabelStyle = null;
        sliderValueStyle = null;
        toggleLabelStyle = null;
        fallbackFont = null;
        fallbackCharmSprite = null;
        charmsController = null;
        storedGameOptionsCanvasState = false;
    }
}
#nullable restore
