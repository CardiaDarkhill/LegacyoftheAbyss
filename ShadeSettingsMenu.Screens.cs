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
        charmsScreen = null;
        charmsController = null;
        skinsScreen = null;
        skinsController = null;
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
        if (charmsScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Charms", () => ShowScreen(charmsScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        if (skinsScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Skins", () => ShowScreen(skinsScreen), CancelTarget.PauseMenu);
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

    private static void BuildCharmsMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;

        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        charmsController = ms.gameObject.GetComponent<CharmMenuController>() ?? ms.gameObject.AddComponent<CharmMenuController>();

        var inventory = ShadeRuntime.Charms;
        if (inventory == null)
        {
            var fallbackMessage = new GameObject("CharmUnavailable");
            fallbackMessage.transform.SetParent(content, false);
            var messageText = fallbackMessage.AddComponent<Text>();
            ApplyTextStyle(messageText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            messageText.text = "Charm inventory unavailable.";
            var msgLayout = fallbackMessage.AddComponent<LayoutElement>();
            msgLayout.minHeight = 120f;
            msgLayout.preferredHeight = 120f;
            return;
        }

        float horizontalMargin = Mathf.Clamp(Screen.width * 0.04f, 48f, 140f);
        float bottomMargin = Mathf.Clamp(Screen.height * 0.08f, 56f, 132f);
        float topMargin = Mathf.Clamp(Screen.height * 0.11f, 72f, 164f);
        content.offsetMin = new Vector2(horizontalMargin, bottomMargin);
        content.offsetMax = new Vector2(-horizontalMargin, -topMargin);

        var selectables = new List<MenuSelectable>();
        var actionButtons = new List<MenuButton>();

        var notchObj = new GameObject("NotchMeter");
        var notchRect = notchObj.AddComponent<RectTransform>();
        notchRect.SetParent(content, false);
        var notchText = notchObj.AddComponent<Text>();
        ApplyTextStyle(notchText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        notchText.text = $"Notches Used: {inventory.UsedNotches}/{inventory.NotchCapacity}";
        var notchLayout = notchObj.AddComponent<LayoutElement>();
        notchLayout.minHeight = 52f;
        notchLayout.preferredHeight = 52f;

        var navObj = new GameObject("NavigationHint");
        var navRect = navObj.AddComponent<RectTransform>();
        navRect.SetParent(content, false);
        var navigationText = navObj.AddComponent<Text>();
        ApplyTextStyle(navigationText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        ScaleTextElements(navObj, 0.85f);
        var navLayout = navObj.AddComponent<LayoutElement>();
        navLayout.minHeight = 44f;
        navLayout.preferredHeight = 44f;

        var gridRoot = new GameObject("CharmGrid");
        var gridRect = gridRoot.AddComponent<RectTransform>();
        gridRect.SetParent(content, false);
        gridRect.anchorMin = new Vector2(0f, 1f);
        gridRect.anchorMax = new Vector2(1f, 1f);
        gridRect.pivot = new Vector2(0.5f, 1f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;
        var gridLayout = gridRoot.AddComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        int columnCount = Screen.width >= 1800 ? 3 : 2;
        gridLayout.constraintCount = Mathf.Max(2, columnCount);
        float cellWidth = Mathf.Clamp(Screen.width * 0.22f, 260f, 360f);
        float cellHeight = Mathf.Clamp(Screen.height * 0.24f, 220f, 320f);
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        float horizontalSpacing = Mathf.Clamp(cellWidth * 0.08f, 18f, 42f);
        float verticalSpacing = Mathf.Clamp(cellHeight * 0.12f, 18f, 46f);
        gridLayout.spacing = new Vector2(horizontalSpacing, verticalSpacing);
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        var gridLayoutElement = gridRoot.AddComponent<LayoutElement>();
        gridLayoutElement.minHeight = cellHeight * 2.4f;
        gridLayoutElement.flexibleHeight = 1f;

        var detailRoot = new GameObject("CharmDetails");
        var detailRect = detailRoot.AddComponent<RectTransform>();
        detailRect.SetParent(content, false);
        var detailLayout = detailRoot.AddComponent<VerticalLayoutGroup>();
        detailLayout.spacing = 12f;
        detailLayout.padding = new RectOffset(36, 36, 0, 0);
        detailLayout.childControlHeight = false;
        detailLayout.childForceExpandHeight = false;
        detailLayout.childControlWidth = true;
        detailLayout.childForceExpandWidth = true;
        detailLayout.childAlignment = TextAnchor.UpperLeft;
        var detailLayoutElement = detailRoot.AddComponent<LayoutElement>();
        detailLayoutElement.minHeight = 180f;
        detailLayoutElement.preferredHeight = 0f;

        var detailTitleObj = new GameObject("DetailTitle");
        detailTitleObj.transform.SetParent(detailRoot.transform, false);
        var detailTitleText = detailTitleObj.AddComponent<Text>();
        ApplyTextStyle(detailTitleText, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        ScaleTextElements(detailTitleObj, 1.05f);
        var detailTitleLayout = detailTitleObj.AddComponent<LayoutElement>();
        detailTitleLayout.minHeight = 42f;
        detailTitleLayout.preferredHeight = 42f;

        var detailDescriptionObj = new GameObject("DetailDescription");
        detailDescriptionObj.transform.SetParent(detailRoot.transform, false);
        var detailDescriptionText = detailDescriptionObj.AddComponent<Text>();
        ApplyTextStyle(detailDescriptionText, sliderValueStyle ?? sliderLabelStyle, TextAnchor.UpperLeft, Color.white);
        detailDescriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        detailDescriptionText.verticalOverflow = VerticalWrapMode.Overflow;
        var detailDescLayout = detailDescriptionObj.AddComponent<LayoutElement>();
        detailDescLayout.minHeight = 72f;
        detailDescLayout.preferredHeight = 0f;

        var statusObj = new GameObject("StatusMessage");
        statusObj.transform.SetParent(detailRoot.transform, false);
        var statusText = statusObj.AddComponent<Text>();
        ApplyTextStyle(statusText, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        ScaleTextElements(statusObj, 0.9f);
        var statusLayout = statusObj.AddComponent<LayoutElement>();
        statusLayout.minHeight = 40f;
        statusLayout.preferredHeight = 40f;
        statusText.text = "Select a charm to view details.";

        var actionRow = new GameObject("CharmActions");
        var actionRect = actionRow.AddComponent<RectTransform>();
        actionRect.SetParent(content, false);
        var actionLayout = actionRow.AddComponent<HorizontalLayoutGroup>();
        actionLayout.spacing = Mathf.Clamp(Screen.width * 0.035f, 32f, 84f);
        actionLayout.childAlignment = TextAnchor.MiddleCenter;
        actionLayout.childControlWidth = true;
        actionLayout.childForceExpandWidth = false;
        actionLayout.childControlHeight = false;
        actionLayout.childForceExpandHeight = false;
        var actionLayoutElement = actionRow.AddComponent<LayoutElement>();
        actionLayoutElement.minHeight = ButtonRowHeight * 1.2f;
        actionLayoutElement.preferredHeight = ButtonRowHeight * 1.2f;

        MenuButton equipButton = null;
        MenuButton unequipButton = null;

        var equipSelectable = CreateMenuButton(actionRow.transform, buttonTemplate, "Equip", null, CancelTarget.ShadeMain);
        if (equipSelectable is MenuButton equipMenuButton)
        {
            equipButton = equipMenuButton;
            actionButtons.Add(equipMenuButton);
        }

        var unequipSelectable = CreateMenuButton(actionRow.transform, buttonTemplate, "Unequip", null, CancelTarget.ShadeMain);
        if (unequipSelectable is MenuButton unequipMenuButton)
        {
            unequipButton = unequipMenuButton;
            actionButtons.Add(unequipMenuButton);
        }

        ConfigureHorizontalNavigation(actionButtons);

        var iconSprite = GetFallbackSprite(ref fallbackCharmSprite, "ShadeSettingsCharmIcon", false);

        foreach (var definition in inventory.AllCharms)
        {
            var selectable = CreateMenuButton(gridRoot.transform, buttonTemplate, definition.DisplayName, null, CancelTarget.ShadeMain);
            if (selectable is MenuButton menuButton)
            {
                selectables.Add(menuButton);

                var layout = menuButton.GetComponent<LayoutElement>();
                if (layout != null)
                {
                    layout.minHeight = 0f;
                    layout.preferredHeight = 0f;
                    layout.flexibleHeight = 1f;
                    layout.minWidth = 0f;
                    layout.preferredWidth = 0f;
                    layout.flexibleWidth = 1f;
                }

                var buttonRect = menuButton.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
                    buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
                    buttonRect.pivot = new Vector2(0.5f, 0.5f);
                    buttonRect.sizeDelta = gridLayout.cellSize;
                }

                var contentRoot = new GameObject("CharmContent");
                var contentRect = contentRoot.AddComponent<RectTransform>();
                contentRect.SetParent(menuButton.transform, false);
                contentRect.anchorMin = Vector2.zero;
                contentRect.anchorMax = Vector2.one;
                contentRect.offsetMin = new Vector2(16f, 18f);
                contentRect.offsetMax = new Vector2(-16f, -18f);
                var contentLayout = contentRoot.AddComponent<VerticalLayoutGroup>();
                contentLayout.spacing = 8f;
                contentLayout.childAlignment = TextAnchor.UpperCenter;
                contentLayout.childControlHeight = false;
                contentLayout.childForceExpandHeight = false;
                contentLayout.childControlWidth = true;
                contentLayout.childForceExpandWidth = true;

                var iconObj = new GameObject("Icon");
                var iconRect = iconObj.AddComponent<RectTransform>();
                iconRect.SetParent(contentRoot.transform, false);
                var iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = iconSprite;
                iconImage.type = Image.Type.Simple;
                iconImage.preserveAspect = true;
                var iconLayout = iconObj.AddComponent<LayoutElement>();
                iconLayout.minHeight = cellHeight * 0.55f;
                iconLayout.preferredHeight = cellHeight * 0.55f;

                var existingLabel = menuButton.GetComponentInChildren<Text>(true);
                if (existingLabel != null)
                {
                    existingLabel.transform.SetParent(contentRoot.transform, false);
                    ApplyTextStyle(existingLabel, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
                    var nameLayout = existingLabel.gameObject.GetComponent<LayoutElement>() ?? existingLabel.gameObject.AddComponent<LayoutElement>();
                    nameLayout.minHeight = 36f;
                    nameLayout.preferredHeight = 36f;
                }

                var notchObjLocal = new GameObject("NotchCost");
                notchObjLocal.transform.SetParent(contentRoot.transform, false);
                var notchCostText = notchObjLocal.AddComponent<Text>();
                ApplyTextStyle(notchCostText, sliderValueStyle ?? sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
                ScaleTextElements(notchObjLocal, 0.9f);
                var notchCostLayout = notchObjLocal.AddComponent<LayoutElement>();
                notchCostLayout.minHeight = 30f;
                notchCostLayout.preferredHeight = 30f;

                var statusObjLocal = new GameObject("StatusLabel");
                statusObjLocal.transform.SetParent(contentRoot.transform, false);
                var statusLabel = statusObjLocal.AddComponent<Text>();
                ApplyTextStyle(statusLabel, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
                ScaleTextElements(statusObjLocal, 0.85f);
                var statusLocalLayout = statusObjLocal.AddComponent<LayoutElement>();
                statusLocalLayout.minHeight = 26f;
                statusLocalLayout.preferredHeight = 26f;

                var driver = menuButton.gameObject.AddComponent<CharmButtonDriver>();
                driver.Initialize(charmsController, definition, menuButton, iconImage, existingLabel, notchCostText, statusLabel, iconSprite);
            }
        }

        if (equipButton != null)
            selectables.Add(equipButton);
        if (unequipButton != null)
            selectables.Add(unequipButton);

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
        charmsController?.Initialize(notchText, statusText, detailTitleText, detailDescriptionText, navigationText, equipButton, unequipButton);
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
        else if (target == charmsScreen)
        {
            charmsController?.HandleScreenShown();
        }
        else if (target == skinsScreen)
        {
            consumeNextToggle = false;
            skinsController?.HandleScreenShown();
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

}
#nullable restore
