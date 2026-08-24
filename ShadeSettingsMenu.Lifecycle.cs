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
    /// <summary>
    /// Finds the row to clone for a slider: one of the base game's volume sliders.
    /// <para>
    /// Two earlier attempts at this both came back empty and left every slider in the mod as the
    /// bare Unity fallback. The first searched <c>optionsMenuScreen</c>, which is the list of
    /// category buttons and carries no slider at all. The second added the Audio/Video/Brightness
    /// screens but still went looking for a <c>MenuSelectable</c> with a <c>Slider</c> underneath it,
    /// which assumes a row shape nothing here had verified.
    /// </para>
    /// <para>
    /// So this asks the question the other way round and starts from the component that can only
    /// exist on a volume slider. <see cref="Resources.FindObjectsOfTypeAll{T}"/> sees inactive
    /// objects, which every one of these is while the game is not on that screen, and does not care
    /// which <c>UIManager</c> field happens to be wired. The result is logged unconditionally,
    /// because "the slider template was not found" is otherwise indistinguishable from "it was found
    /// and still looks wrong".
    /// </para>
    /// </summary>
    private static GameObject FindGameSliderTemplate(UIManager ui)
    {
        try
        {
            // Scene instances only. FindObjectsOfTypeAll also returns prefab assets, and one of
            // those - a generic options row carrying its own label, its own value text and a
            // checkbox - is what got cloned when the ordering happened to put it first. Every mod
            // slider then drew "Master Volume", "10" and a tick box on top of its own label. A
            // prefab is not a row on a screen; only something in a loaded scene is.
            foreach (var audioSlider in Resources.FindObjectsOfTypeAll<MenuAudioSlider>())
            {
                if (audioSlider == null || !audioSlider.gameObject.scene.IsValid())
                {
                    continue;
                }

                var root = ResolveSliderTemplateRoot(audioSlider.gameObject);
                if (root == null)
                {
                    continue;
                }

                RecordSliderTemplate($"{DescribeHierarchyPath(root.transform)} (from MenuAudioSlider)");
                return root;
            }
        }
        catch (Exception e)
        {
            LogMenuWarning($"MenuAudioSlider scan failed: {e}");
        }

        // Fallback: any row on a screen that draws sliders. Keeps working if a future build renames
        // or drops MenuAudioSlider.
        if (ui != null)
        {
            var screens = new[]
            {
                ui.audioMenuScreen,
                ui.brightnessMenuScreen,
                ui.videoMenuScreen,
                ui.gameOptionsMenuScreen,
                ui.optionsMenuScreen
            };

            foreach (var screen in screens)
            {
                if (screen == null)
                {
                    continue;
                }

                foreach (var slider in screen.GetComponentsInChildren<Slider>(true))
                {
                    if (slider == null || !slider.gameObject.scene.IsValid())
                    {
                        continue;
                    }

                    var root = ResolveSliderTemplateRoot(slider.gameObject);
                    if (root == null)
                    {
                        continue;
                    }

                    RecordSliderTemplate($"{DescribeHierarchyPath(root.transform)} (from screen '{screen.name}')");
                    return root;
                }
            }
        }

        RecordSliderTemplate("none found - using the plain fallback");
        return null;
    }

    /// <summary>
    /// Remembers and announces which row the sliders were cloned from. Goes to the log *and* to
    /// <see cref="LastSliderTemplateDescription"/>, which the bug reporter puts in every snapshot -
    /// the menu is built seconds after launch, so this line is always long gone from the log ring by
    /// the time anyone files a report about how the sliders look.
    /// </summary>
    private static void RecordSliderTemplate(string description)
    {
        LastSliderTemplateDescription = description;
        log.LogInfo("Slider template: " + description);
    }

    /// <summary>
    /// Which shoulder-button glyph to draw: the pad's when one is plugged in, the keyboard's cap
    /// otherwise.
    /// <para>
    /// Not <c>GetButtonSkinFor</c>, which picks by <c>lastActiveController</c>. On a machine with
    /// both, that shows whichever device was touched last - so this menu drew the keyboard's square
    /// bracket caps for a player who wanted the LB/RB the inventory shows them. It also flaps as the
    /// player switches hands, and two people can be on these screens at once.
    /// </para>
    /// </summary>
    internal static ButtonSkin ResolvePaneButtonSkin(HeroActionButton action)
    {
        try
        {
            var skins = UIManager.instance != null ? UIManager.instance.uiButtonSkins : null;
            var handler = HornetInput.FindHandler();
            if (skins == null || handler == null)
            {
                return null;
            }

            var playerAction = handler.ActionButtonToPlayerAction(action);
            if (playerAction == null)
            {
                return null;
            }

            return AnyControllerAttached()
                ? skins.GetControllerButtonSkinFor(playerAction)
                : skins.GetKeyboardSkinFor(playerAction);
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not resolve the button skin for {action}: {e}");
            return null;
        }
    }

    private static bool AnyControllerAttached()
    {
        try
        {
            var devices = InControl.InputManager.Devices;
            if (devices == null)
            {
                return false;
            }

            foreach (var device in devices)
            {
                if (device != null && device != InControl.InputDevice.Null && device.IsAttached)
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    /// Walks up from the object carrying the Slider to the row that should be cloned: the nearest
    /// ancestor with a <see cref="MenuSelectable"/> (which is what carries the selection fleurs), or
    /// the slider's own object when the row has none. Stops before leaving the row - an ancestor
    /// holding a second Slider is a list of rows, not a row.
    /// </summary>
    private static GameObject ResolveSliderTemplateRoot(GameObject sliderObject)
    {
        if (sliderObject == null)
        {
            return null;
        }

        GameObject best = sliderObject;
        var cursor = sliderObject.transform.parent;
        for (int depth = 0; depth < 4 && cursor != null; depth++)
        {
            var candidate = cursor.gameObject;
            if (candidate.GetComponentsInChildren<Slider>(true).Length > 1)
            {
                break;
            }

            if (candidate.GetComponent<MenuSelectable>() != null)
            {
                best = candidate;
                break;
            }

            cursor = cursor.parent;
        }

        return best;
    }

    private static string DescribeHierarchyPath(Transform transform)
    {
        if (transform == null)
        {
            return "<null>";
        }

        var parts = new List<string>();
        var cursor = transform;
        while (cursor != null && parts.Count < 8)
        {
            parts.Insert(0, cursor.name);
            cursor = cursor.parent;
        }

        return string.Join("/", parts);
    }

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

        // The game's slider is read for its look, never cloned. See SliderSkin for why: its rows
        // carry their own label, value and layout group, and three separate attempts at deleting
        // those from a clone afterwards each missed something.
        var gameSlider = FindGameSliderTemplate(ui);
        gameSliderSkin = CaptureSliderSkin(gameSlider, screenTemplate);
        RecordSliderTemplate(LastSliderTemplateDescription + " | " + gameSliderSkin.Describe());
        GameObject sliderTemplate = CreateDefaultSliderTemplate();

        // Only supplies a text style (toggle rows themselves are drawn as menu buttons), but the
        // same reasoning as the slider applies: the screen that actually carries toggles is Game
        // Options, not the category list.
        MenuSelectable toggleTemplate = null;
        foreach (var candidateScreen in new[] { ui.gameOptionsMenuScreen, ui.videoMenuScreen, optionsScreen })
        {
            if (candidateScreen == null)
                continue;
            foreach (var cand in candidateScreen.GetComponentsInChildren<MenuSelectable>(true))
            {
                if (cand.GetComponentInChildren<Toggle>(true) != null)
                {
                    toggleTemplate = cand;
                    break;
                }
            }
            if (toggleTemplate != null)
                break;
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
        shadeAiScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();
        shadeAiAdvancedScreen = Object.Instantiate(screenTemplate, screenTemplate.transform.parent).GetComponent<MenuScreen>();

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
        if (shadeAiScreen != null)
        {
            shadeAiScreen.gameObject.name = "ShadeSettingsShadeAi";
            shadeAiScreen.gameObject.SetActive(false);
            InitializeScreen(shadeAiScreen);
            allScreens.Add(shadeAiScreen);
        }
        if (shadeAiAdvancedScreen != null)
        {
            shadeAiAdvancedScreen.gameObject.name = "ShadeSettingsShadeAiAdvanced";
            shadeAiAdvancedScreen.gameObject.SetActive(false);
            InitializeScreen(shadeAiAdvancedScreen);
            allScreens.Add(shadeAiAdvancedScreen);
        }

        screen = mainScreen != null ? mainScreen.gameObject : null;

        BuildMainMenu(ui, mainScreen, buttonTemplate);
        BuildDifficultyMenu(ui, difficultyScreen, sliderTemplate, buttonTemplate);
        if (IncludeLegacyCharmMenu && charmsScreen != null)
            BuildCharmsMenu(ui, charmsScreen, buttonTemplate);
        BuildSkinsMenu(ui, skinsScreen, buttonTemplate);
        BuildControlsMenu(ui, controlsScreen, buttonTemplate);
        BuildLoggingMenu(ui, loggingScreen, buttonTemplate);
        BuildShadeAiMenu(ui, shadeAiScreen, sliderTemplate, buttonTemplate);
        BuildShadeAiAdvancedMenu(ui, shadeAiAdvancedScreen, sliderTemplate, buttonTemplate);

        // Always ours, so always disposed once every screen has cloned it.
        if (sliderTemplate != null)
            Object.Destroy(sliderTemplate);
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
