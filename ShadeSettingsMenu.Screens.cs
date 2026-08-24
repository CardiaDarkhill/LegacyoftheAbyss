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
        screenLastSelectables.Clear();
        mainScreen = null;
        difficultyScreen = null;
        difficultyController = null;
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
            if (comp is CancelRouter || comp is SliderMenuDriver)
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
            StretchScreenOverCanvas(rt);
        }
        ms.transform.SetAsLastSibling();
        StripTemplateComponents(ms);

        var focusDriver = ms.gameObject.GetComponent<MenuFocusDriver>() ?? ms.gameObject.AddComponent<MenuFocusDriver>();
        focusDriver.screen = ms;
    }

    /// <summary>
    /// Grows a cloned screen's rect until it actually covers the canvas, and centres it there.
    /// <para>
    /// These screens are clones of the pause menu, parented where it sits, and that part of the
    /// hierarchy is scaled down - roughly 0.7 on the install this was measured on. Stretching to the
    /// parent therefore produced a rect that *reports* the full 1920x1080 in local units while
    /// drawing over about 70% of the display, in the upper part of the screen. Every layout
    /// calculation in this file works in those local units, so all of them were quietly budgeting
    /// for a third more space than the player could see: the Controls screen concluded its binding
    /// list did not fit and grew a scrollbar with a quarter of the display sitting empty below it.
    /// </para>
    /// <para>
    /// Compensating by resetting localScale would fix the coverage but resize every glyph with it.
    /// Growing the rect by the same factor instead leaves the rendered size of everything already on
    /// these screens untouched and simply hands the layout the room it thought it had. The scale is
    /// measured rather than assumed, so a build where the pause menu is not scaled gets a no-op.
    /// </para>
    /// </summary>
    private static void StretchScreenOverCanvas(RectTransform rt)
    {
        try
        {
            // includeInactive, because InitializeScreen runs on a screen that has just been
            // deactivated - the parameterless overload skips inactive objects and would find nothing.
            var canvas = rt.GetComponentInParent<Canvas>(true);
            if (canvas == null)
            {
                return;
            }

            var canvasRect = canvas.rootCanvas != null
                ? canvas.rootCanvas.transform as RectTransform
                : canvas.transform as RectTransform;
            if (canvasRect == null)
            {
                return;
            }

            Vector2 canvasSize = canvasRect.rect.size;
            if (canvasSize.x <= 1f || canvasSize.y <= 1f)
            {
                return;
            }

            Vector3 ourScale = rt.lossyScale;
            Vector3 canvasScale = canvasRect.lossyScale;
            if (Mathf.Abs(canvasScale.x) < 0.0001f || Mathf.Abs(canvasScale.y) < 0.0001f)
            {
                return;
            }

            float relativeX = ourScale.x / canvasScale.x;
            float relativeY = ourScale.y / canvasScale.y;

            // Recorded before the early return below, so a build where the pause menu is not scaled
            // records its 1 rather than leaving the last screen's figure standing. Anything copied
            // in from elsewhere on the canvas has to be resized by this to come out the size it is
            // drawn at over there - see SliderUnitScale.
            if (relativeX > 0.0001f)
            {
                screenCanvasScale = relativeX;
            }

            // Only ever grow. A screen already covering the canvas (or somehow larger than it) is
            // left exactly as the stretch left it, so this cannot shrink a layout that was fine.
            if (relativeX <= 0.0001f || relativeY <= 0.0001f || (relativeX >= 0.999f && relativeY >= 0.999f))
            {
                return;
            }

            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(canvasSize.x / Mathf.Min(1f, relativeX), canvasSize.y / Mathf.Min(1f, relativeY));
            // Centre on the canvas in world space - the parent is not necessarily centred on it
            // either, and on the measured install it sits noticeably high.
            rt.position = canvasRect.TransformPoint(canvasRect.rect.center);

            LogMenuDebug(FormattableString.Invariant(
                $"Screen '{rt.name}' stretched to {rt.sizeDelta.x:0}x{rt.sizeDelta.y:0} local units (scale {relativeX:0.000}x{relativeY:0.000} of canvas {canvasSize.x:0}x{canvasSize.y:0})"));
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not stretch screen '{rt.name}' over the canvas: {e}");
        }
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

        // Insets are a share of the screen rather than a fixed 60/80/70, because that rect is no
        // longer the size it used to be - StretchScreenOverCanvas grew it to cover the display, and
        // fixed margins against it left every list starting hard against the top edge with rows
        // running the full width. The game's own option screens sit in a narrow centred column with
        // a clear band above them, so these match that: about a fifth of the height reserved at the
        // top, and a column roughly two thirds of the width.
        // Screens that lay themselves out (Controls, Difficulty) overwrite these straight after.
        var screenRect = (RectTransform)ms.transform;
        float screenWidth = screenRect.rect.width;
        float screenHeight = screenRect.rect.height;
        if (screenWidth > 1f && screenHeight > 1f)
        {
            float columnWidth = Mathf.Clamp(screenWidth * ListColumnWidthFraction, 480f, screenWidth - 96f);
            float sideMargin = Mathf.Max(48f, (screenWidth - columnWidth) * 0.5f);
            contentRect.offsetMin = new Vector2(sideMargin, screenHeight * ListBottomMarginFraction);
            contentRect.offsetMax = new Vector2(-sideMargin, -(screenHeight * ListTopMarginFraction));
        }
        else
        {
            contentRect.offsetMin = new Vector2(60f, 80f);
            contentRect.offsetMax = new Vector2(-60f, -70f);
        }
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
        else if (cancelTarget == CancelTarget.ShadeAi)
        {
            ms.backButton.OnSubmitPressed.AddListener(ShowShadeAiMenu);
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

    /// <summary>
    /// Whether "put the highlight back where the player left it" applies to <paramref name="ms"/>.
    /// <para>
    /// The skins screen is excluded on purpose: <c>SkinMenuController.RefreshButtons</c> keeps that
    /// screen's entry in <see cref="screenFirstSelectables"/> pointed at whatever skin is currently
    /// equipped, so re-entering it lands on the active pick rather than on the last row browsed.
    /// That is a deliberate behaviour, and remembering a position would quietly undo it.
    /// </para>
    /// </summary>
    private static bool TracksLastSelection(MenuScreen ms)
    {
        return ms != null && !ReferenceEquals(ms, skinsScreen);
    }

    /// <summary>
    /// Records where the highlight currently sits on <paramref name="ms"/>, so the next
    /// <see cref="ShowScreen"/> of that screen can put it back.
    /// </summary>
    private static void RememberSelection(MenuScreen ms)
    {
        if (ms == null || !TracksLastSelection(ms))
            return;

        var current = EventSystem.current;
        var selectedGo = current != null ? current.currentSelectedGameObject : null;
        if (selectedGo == null)
            return;

        var selectable = selectedGo.GetComponentInParent<MenuSelectable>();
        if (selectable == null)
            return;

        // Only remember a selection that actually lives on this screen - the EventSystem's selection
        // can still be pointing at whatever was highlighted before this screen existed.
        if (!selectable.transform.IsChildOf(ms.transform))
            return;

        // The Back button is a way out, not a position - returning to it would be as wrong as
        // returning to the top of the list. MenuButtonList makes the same exclusion for its own
        // lastSelected.
        if (ms.backButton != null && ReferenceEquals(selectable, ms.backButton))
            return;

        screenLastSelectables[ms] = selectable;
    }

    /// <summary>
    /// The row to highlight when <paramref name="ms"/> is shown: wherever the player left it, or the
    /// screen's first row the first time it opens.
    /// </summary>
    private static MenuSelectable GetRestoreHighlight(MenuScreen ms)
    {
        if (ms == null)
            return null;
        if (TracksLastSelection(ms) &&
            screenLastSelectables.TryGetValue(ms, out var remembered) &&
            remembered != null &&
            remembered.interactable &&
            IsSelectableLiveUnder(remembered, ms))
        {
            return remembered;
        }

        screenLastSelectables.Remove(ms);
        return GetPreferredHighlight(ms);
    }

    /// <summary>
    /// True when <paramref name="selectable"/> is still a live row of <paramref name="ms"/>.
    /// <para>
    /// Deliberately walks <c>activeSelf</c> up to the screen rather than reading
    /// <c>activeInHierarchy</c>: this is called while the screen itself is still deactivated - that
    /// is the whole point, the answer has to be known before the screen goes live - so
    /// <c>activeInHierarchy</c> is false for every row on it and would reject them all.
    /// </para>
    /// </summary>
    private static bool IsSelectableLiveUnder(MenuSelectable selectable, MenuScreen ms)
    {
        if (selectable == null || ms == null)
            return false;

        var node = selectable.transform;
        while (node != null && node != ms.transform)
        {
            if (!node.gameObject.activeSelf)
                return false;
            node = node.parent;
        }

        return node == ms.transform;
    }

    /// <summary>
    /// Keeps the screen's own <c>MenuButtonList.lastSelected</c> in step with
    /// <see cref="screenLastSelectables"/>.
    /// <para>
    /// Both mechanisms fire on the same activation and each one overwrites the other's highlight:
    /// <c>MenuButtonList.OnEnable</c> starts a <c>SelectDelayed</c> coroutine for whatever it
    /// remembers, and <see cref="ShowScreen"/> highlights its own choice straight afterwards. That
    /// race is the visible bug - the correct row appears for a frame and is then replaced by the top
    /// of the list. Writing the same answer into both means it no longer matters which one lands
    /// last.
    /// </para>
    /// </summary>
    private static void SyncButtonListSelection(MenuScreen ms, MenuSelectable selectable)
    {
        if (ms == null || selectable == null)
            return;

        try
        {
            var mbl = ms.GetComponent<MenuButtonList>();
            if (mbl == null)
                return;

            var field = typeof(MenuButtonList).GetField("lastSelected", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(mbl, selectable);
        }
        catch (Exception ex)
        {
            LogMenuDebug($"Could not sync MenuButtonList selection: {ex.Message}");
        }
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
        // Activate, not Proceed. MenuButton.OnSubmit calls ForceDeselect() for every type except
        // Activate, which clears the EventSystem's selection - and MenuFocusDriver then puts the
        // highlight back on the screen's defaultHighlight. That is why pressing any toggle or
        // stepper row threw the cursor back to the top of the screen. It also cost ShowScreen its
        // RememberSelection, because by the time a Proceed row opened a sub-menu there was no
        // selection left to remember, so backing out never returned to the row that opened it.
        btn.buttonType = MenuButton.MenuButtonType.Activate;
        btn.cancelAction = CancelAction.DoNothing;
        var pointerSelect = go.GetComponent<PointerSelectDriver>() ?? go.AddComponent<PointerSelectDriver>();
        pointerSelect.target = btn;
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
        // "Shade Enabled" used to head this list. It now lives in Debug Options: with the Shade AI
        // in, removing the Shade outright is a testing tool rather than something a player who
        // installed this mod wants on the front page.
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
        if (shadeAiScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Shade AI", () => ShowScreen(shadeAiScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        if (controlsScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Controls", () => ShowScreen(controlsScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        if (loggingScreen != null)
        {
            var s = CreateMenuButton(content, buttonTemplate, "Debug Options", () => ShowScreen(loggingScreen), CancelTarget.PauseMenu);
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

    /// <summary>
    /// The Difficulty screen: a header row of three discrete settings, then Damage and Healing side
    /// by side, then the explanation line.
    /// <para>
    /// Positioned explicitly rather than through nested layout groups, for the same reason the
    /// Controls screen is - see the note at the top of <see cref="BuildControlsMenu"/>. The two
    /// panels have to be a known width before their slider rows can pick their column widths, and
    /// asking Unity's layout system for that number bottom-up is exactly the arrangement that
    /// produced zero-height rows there.
    /// </para>
    /// </summary>
    private static void BuildDifficultyMenu(UIManager ui, MenuScreen ms, GameObject sliderTemplate, MenuButton buttonTemplate)
    {
        if (ms == null || sliderTemplate == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        var contentLayout = content.GetComponent<VerticalLayoutGroup>();
        if (contentLayout != null)
            contentLayout.enabled = false;

        difficultyController = ms.gameObject.GetComponent<DifficultyMenuController>() ?? ms.gameObject.AddComponent<DifficultyMenuController>();

        var msRect = (RectTransform)ms.transform;
        float canvasWidth = msRect.rect.width;
        float canvasHeight = msRect.rect.height;

        // Wider than a list screen - two panels of rows need the room - but the same band above
        // and below, so this screen sits in the same place on the display as the others.
        float horizontalMargin = Mathf.Max(48f, canvasWidth * 0.06f);
        float bottomMargin = canvasHeight * ListBottomMarginFraction;
        float topMargin = canvasHeight * ListTopMarginFraction;
        content.offsetMin = new Vector2(horizontalMargin, bottomMargin);
        content.offsetMax = new Vector2(-horizontalMargin, -topMargin);

        float contentWidth = content.rect.width;
        float contentHeight = content.rect.height;
        const float SectionSpacing = 28f;

        var selectables = new List<MenuSelectable>();
        var descriptions = new List<KeyValuePair<MenuSelectable, string>>();
        var headerButtons = new List<MenuButton>();
        var damageRows = new List<MenuSelectable>();
        var healingRows = new List<MenuSelectable>();

        void Describe(MenuSelectable selectable, string description)
        {
            if (selectable == null)
                return;
            selectables.Add(selectable);
            if (!string.IsNullOrEmpty(description))
                descriptions.Add(new KeyValuePair<MenuSelectable, string>(selectable, description));
        }

        // --- header row: preset, assist mode, shade masks ------------------------------------
        float cursorY = 0f;
        var headerRow = new GameObject("DifficultyHeader");
        var headerRect = headerRow.AddComponent<RectTransform>();
        headerRect.SetParent(content, false);
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = new Vector2(0f, -cursorY);
        headerRect.sizeDelta = new Vector2(0f, ButtonRowHeight);

        float headerSpacing = Mathf.Clamp(contentWidth * 0.03f, 24f, 90f);
        float headerCellWidth = Mathf.Max(120f, (contentWidth - headerSpacing * 2f) / 3f);

        MenuButton AddHeaderCell(int column, string label, string description, Func<string> value, Action<int> step)
        {
            var selectable = CreateMenuButton(headerRect, buttonTemplate, label, null, CancelTarget.ShadeMain);
            if (selectable is not MenuButton button)
            {
                if (selectable != null)
                    Describe(selectable, description);
                return null;
            }

            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.offsetMin = new Vector2(rect.offsetMin.x, 0f);
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0f);
            rect.anchoredPosition = new Vector2(column * (headerCellWidth + headerSpacing), 0f);
            rect.sizeDelta = new Vector2(headerCellWidth, 0f);

            var driver = button.gameObject.AddComponent<LabeledStepperDriver>();
            driver.Initialize(button, label, value, step);
            difficultyController.RegisterStepper(driver);
            Describe(button, description);
            headerButtons.Add(button);
            return button;
        }

        // The preset row's explanation is whichever preset is currently selected, so unlike every
        // other row here it cannot be a fixed string. Registered as a live lookup further down.
        var presetButton = AddHeaderCell(0, "Difficulty",
            null,
            () => DifficultyPreset.IdentifyName(ModConfig.Instance),
            StepDifficultyPreset);
        MenuSelectable presetSelectable = presetButton;

        AddHeaderCell(1, "Assist Mode",
            "Press to switch. Makes the Shade invulnerable: it still fights and still heals, it simply cannot be killed or need reviving.",
            () => GetShadeAssistMode() ? "On" : "Off",
            _ => SetShadeAssistMode(!GetShadeAssistMode()));

        AddHeaderCell(2, "Shade Masks",
            "Press to step by 10%. How many masks the Shade carries, as a share of Hornet's, rounded up. Applies to the Shade in front of you at once.",
            DescribeShadeMaskSetting,
            StepShadeMaskFraction);

        cursorY += ButtonRowHeight + SectionSpacing;

        // --- the two panels ------------------------------------------------------------------
        float footerHeight = DescriptionRowHeight;
        float panelsHeight = Mathf.Max(160f, contentHeight - cursorY - footerHeight - SectionSpacing);
        float panelSpacing = Mathf.Clamp(contentWidth * 0.03f, 24f, 90f);
        float panelWidth = Mathf.Max(200f, (contentWidth - panelSpacing) / 2f);

        const float PanelHeaderHeight = 64f;
        const float PanelPadding = 24f;
        float panelRowWidth = Mathf.Max(120f, panelWidth - PanelPadding * 2f);

        RectTransform CreatePanel(int column, string title, HeroActionButton promptAction, out CanvasGroup promptGroup)
        {
            promptGroup = null;
            var panel = new GameObject(title + "Panel");
            var rect = panel.AddComponent<RectTransform>();
            rect.SetParent(content, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(column * (panelWidth + panelSpacing), -cursorY);
            rect.sizeDelta = new Vector2(panelWidth, panelsHeight);

            // A flat dark plate rather than the pause menu's own frame art: that art is nine-sliced
            // around a fixed aspect and does not survive being stretched to this shape. Deliberately
            // no RectMask2D on it either - one of those is what clipped the selection fleurs off the
            // AI screen, and every row in here has a pair sitting just outside its own rect.
            var background = panel.AddComponent<Image>();
            background.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsPanelBg", true);
            background.type = Image.Type.Sliced;
            background.color = new Color(0f, 0f, 0f, 0.35f);
            background.raycastTarget = false;

            // Heading and prompt sit in one centred row, so the glyph reads as part of the
            // heading rather than as something stranded in the panel's corner.
            var header = new GameObject("Title");
            var headerRowRect = header.AddComponent<RectTransform>();
            headerRowRect.SetParent(rect, false);
            headerRowRect.anchorMin = new Vector2(0f, 1f);
            headerRowRect.anchorMax = new Vector2(1f, 1f);
            headerRowRect.pivot = new Vector2(0.5f, 1f);
            headerRowRect.anchoredPosition = Vector2.zero;
            headerRowRect.sizeDelta = new Vector2(0f, PanelHeaderHeight);
            var headerLayout = header.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 14f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;

            // The glyph points at where pressing it takes you, so it leads on the right-hand panel
            // and trails on the left-hand one.
            if (promptAction == HeroActionButton.MENU_PANE_LEFT)
            {
                promptGroup = CreatePaneSwitchPrompt(headerRowRect, promptAction, PanelHeaderHeight);
            }

            var titleObject = new GameObject("TitleText");
            var titleRect = titleObject.AddComponent<RectTransform>();
            titleRect.SetParent(headerRowRect, false);
            var headerText = titleObject.AddComponent<Text>();
            ApplyTextStyle(headerText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
            headerText.text = title;
            headerText.raycastTarget = false;
            headerText.horizontalOverflow = HorizontalWrapMode.Overflow;
            var titleLayout = titleObject.AddComponent<LayoutElement>();
            titleLayout.preferredWidth = headerText.preferredWidth;
            titleLayout.preferredHeight = PanelHeaderHeight;
            titleLayout.flexibleWidth = 0f;

            if (promptAction == HeroActionButton.MENU_PANE_RIGHT)
            {
                promptGroup = CreatePaneSwitchPrompt(headerRowRect, promptAction, PanelHeaderHeight);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(headerRowRect);
            return rect;
        }

        var damagePanel = CreatePanel(0, "Damage", HeroActionButton.MENU_PANE_RIGHT, out var damagePromptGroup);
        var healingPanel = CreatePanel(1, "Healing", HeroActionButton.MENU_PANE_LEFT, out var healingPromptGroup);

        // Rows are spread through whatever height the panel has rather than stacked at a fixed
        // pitch, so both panels stay balanced whether or not the screen ended up scaled.
        float rowsAreaHeight = Mathf.Max(80f, panelsHeight - PanelHeaderHeight - PanelPadding);
        const int DamageRowCount = 4;
        const int HealingRowCount = 5;
        float damagePitch = rowsAreaHeight / DamageRowCount;
        float healingPitch = rowsAreaHeight / HealingRowCount;
        float damageRowHeight = Mathf.Clamp(damagePitch - 12f, 48f, SliderRowHeight);
        float healingRowHeight = Mathf.Clamp(healingPitch - 12f, 44f, SliderRowHeight);

        var damageMetrics = SliderRowMetrics.ForWidth(panelRowWidth, damageRowHeight);
        var healingMetrics = SliderRowMetrics.ForWidth(panelRowWidth, healingRowHeight);

        // CreateSlider and CreateToggle both leave their row at the top of whatever they were
        // parented to, and the panels deliberately have no layout group, so the vertical offset is
        // stamped here.
        void PlacePanelRow(RectTransform rect, int index, float pitch, float height)
        {
            if (rect == null)
                return;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(PanelPadding, rect.offsetMin.y);
            rect.offsetMax = new Vector2(-PanelPadding, rect.offsetMax.y);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            rect.anchoredPosition = new Vector2(0f, -(PanelHeaderHeight + index * pitch));
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        void AddPanelSlider(RectTransform panel, List<MenuSelectable> column, int index, float pitch,
            SliderRowMetrics metrics, string label, string description, float min, float max,
            Func<float> read, Action<float> write, bool whole = false)
        {
            var row = CreateSlider(panel, sliderTemplate, buttonTemplate, label, min, max, read(),
                v =>
                {
                    write(v);
                    // Any hand edit means the values no longer match whichever preset was chosen, so
                    // the header has to be re-read.
                    RefreshDifficultyHeader();
                },
                CancelTarget.ShadeMain, metrics, out var rowRect, whole);
            if (row == null)
                return;

            PlacePanelRow(rowRect, index, pitch, metrics.Height);
            column.Add(row);
            Describe(row, description);

            var slider = row.GetComponentInChildren<Slider>(true);
            if (slider != null)
            {
                // Re-read from config after a preset change. SetValueWithoutNotify would leave the
                // value text stale, so the notify path is used; the write it triggers is a harmless
                // no-op write of the value that is already there.
                difficultyController.RegisterSliderRefresh(() =>
                {
                    float current = read();
                    if (!Mathf.Approximately(slider.value, current))
                        slider.value = Mathf.Clamp(current, slider.minValue, slider.maxValue);
                });
            }
        }

        // Damage, in the order of the sketch: each character's melee first, then their casts.
        AddPanelSlider(damagePanel, damageRows, 0, damagePitch, damageMetrics,
            "Hornet Needle", "Multiplies the damage of Hornet's needle strikes. Her silk skills are on their own slider.",
            0.2f, 2f, () => ModConfig.Instance.hornetDamageMultiplier, v => ModConfig.Instance.hornetDamageMultiplier = v);
        AddPanelSlider(damagePanel, damageRows, 1, damagePitch, damageMetrics,
            "Shade Nail", "Multiplies the damage of the Shade's nail. Its spells are on their own slider.",
            0.2f, 2f, () => ModConfig.Instance.shadeDamageMultiplier, v => ModConfig.Instance.shadeDamageMultiplier = v);
        AddPanelSlider(damagePanel, damageRows, 2, damagePitch, damageMetrics,
            "Hornet Silk Skills", "Multiplies everything of Hornet's that is not a needle strike - silk skills, tools and thrown damage.",
            0.2f, 2f, () => ModConfig.Instance.hornetSilkSkillDamageMultiplier, v => ModConfig.Instance.hornetSilkSkillDamageMultiplier = v);
        AddPanelSlider(damagePanel, damageRows, 3, damagePitch, damageMetrics,
            "Shade Spells", "Multiplies all six of the Shade's spells. Its nail is on its own slider.",
            0.2f, 2f, () => ModConfig.Instance.shadeSpellDamageMultiplier, v => ModConfig.Instance.shadeSpellDamageMultiplier = v);

        AddPanelSlider(healingPanel, healingRows, 0, healingPitch, healingMetrics,
            "Hornet Bind", "Masks Hornet recovers from one Bind.",
            0f, 6f, () => ModConfig.Instance.bindHornetHeal, v => ModConfig.Instance.bindHornetHeal = Mathf.RoundToInt(v), true);
        AddPanelSlider(healingPanel, healingRows, 1, healingPitch, healingMetrics,
            "Shade Bind", "Masks the Shade recovers when Hornet Binds.",
            0f, 6f, () => ModConfig.Instance.bindShadeHeal, v => ModConfig.Instance.bindShadeHeal = Mathf.RoundToInt(v), true);
        AddPanelSlider(healingPanel, healingRows, 2, healingPitch, healingMetrics,
            "Hornet Focus", "Masks Hornet recovers when the Shade Focuses near her. Zero stops the Shade healing her at all.",
            0f, 6f, () => ModConfig.Instance.focusHornetHeal, v => ModConfig.Instance.focusHornetHeal = Mathf.RoundToInt(v), true);
        AddPanelSlider(healingPanel, healingRows, 3, healingPitch, healingMetrics,
            "Shade Focus", "Masks the Shade recovers from one of its own Focus channels.",
            0f, 6f, () => ModConfig.Instance.focusShadeHeal, v => ModConfig.Instance.focusShadeHeal = Mathf.RoundToInt(v), true);

        var fullMasksRow = CreateToggle(healingPanel, buttonTemplate, "Full Masks Focus",
            ModConfig.Instance.shadeFocusAtFullMasks,
            v =>
            {
                ModConfig.Instance.shadeFocusAtFullMasks = v;
                RefreshDifficultyHeader();
            },
            CancelTarget.ShadeMain);
        if (fullMasksRow != null)
        {
            PlacePanelRow(fullMasksRow.GetComponent<RectTransform>(), HealingRowCount - 1, healingPitch, healingRowHeight);
            healingRows.Add(fullMasksRow);
            Describe(fullMasksRow, "Whether the Shade may channel Focus while on full masks. Off matches Hornet's own rule; on lets it spend SOUL purely to heal her.");
        }

        cursorY += panelsHeight + SectionSpacing;

        // --- description footer ---------------------------------------------------------------
        var footer = new GameObject("Description");
        var footerRect = footer.AddComponent<RectTransform>();
        footerRect.SetParent(content, false);
        footerRect.anchorMin = new Vector2(0f, 1f);
        footerRect.anchorMax = new Vector2(1f, 1f);
        footerRect.pivot = new Vector2(0.5f, 1f);
        footerRect.anchoredPosition = new Vector2(0f, -cursorY);
        footerRect.sizeDelta = new Vector2(0f, footerHeight);
        var footerText = footer.AddComponent<Text>();
        ApplyTextStyle(footerText, toggleLabelStyle, TextAnchor.UpperLeft, DescriptionColor);
        footerText.text = string.Empty;
        footerText.raycastTarget = false;
        footerText.horizontalOverflow = HorizontalWrapMode.Wrap;
        footerText.verticalOverflow = VerticalWrapMode.Truncate;
        footerText.fontSize = Mathf.Max(12, Mathf.RoundToInt(footerText.fontSize * 0.78f));

        var footerDriver = footer.AddComponent<MenuDescriptionDriver>();
        footerDriver.target = footerText;
        foreach (var entry in descriptions)
            footerDriver.Register(entry.Key, entry.Value);
        if (presetSelectable != null)
            footerDriver.RegisterLive(presetSelectable, () => DifficultyPreset.IdentifyDescription(ModConfig.Instance));

        SetupButtonList(ms, selectables);
        ConfigureDifficultyMenuNavigation(headerButtons, damageRows, healingRows);
        var paneSwitch = ms.gameObject.GetComponent<PaneSwitchDriver>() ?? ms.gameObject.AddComponent<PaneSwitchDriver>();
        paneSwitch.leftColumn = damageRows;
        paneSwitch.rightColumn = healingRows;
        // Each prompt belongs to the column you have to be standing in for its button to do
        // anything: RB lives on Damage and moves you off it, LB lives on Healing and moves you back.
        paneSwitch.leftColumnPrompt = damagePromptGroup;
        paneSwitch.rightColumnPrompt = healingPromptGroup;
        // MenuButtonList.Start() flattens Explicit navigation back to a single up/down chain when
        // Unity gets round to running it, so the grid is reapplied afterwards. Same reason and same
        // mechanism as the Controls screen.
        var navigationReapply = ms.gameObject.AddComponent<DeferredNavigationReapplyDriver>();
        navigationReapply.Reapply = () => ConfigureDifficultyMenuNavigation(headerButtons, damageRows, healingRows);

        // The preset row, top-left, is where the highlight lands on entry - it is the setting that
        // decides every other one on the screen.
        MenuSelectable firstHighlight = presetSelectable;
        if (firstHighlight == null)
            firstHighlight = headerButtons.Count > 0 ? headerButtons[0] : null;
        if (firstHighlight == null)
            firstHighlight = selectables.Count > 0 ? selectables[0] : ms.backButton;
        if (firstHighlight != null)
        {
            screenFirstSelectables[ms] = firstHighlight;
            ms.defaultHighlight = firstHighlight;
        }

        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        difficultyController.RefreshAll();
    }

    /// <summary>
    /// Draws the game's own shoulder-button glyph beside a panel heading, so the way across to the
    /// other panel is visible rather than something the player has to guess.
    /// <para>
    /// Both halves of the prompt are built here unconditionally and left for
    /// <c>PanePromptGlyphDriver</c> to fill in, because which of them the current device needs is
    /// not settled at build time and does not stay settled afterwards.
    /// </para>
    /// </summary>
    private static CanvasGroup CreatePaneSwitchPrompt(RectTransform parent, HeroActionButton action, float headerHeight)
    {
        if (parent == null)
        {
            return null;
        }

        float size = headerHeight * PanePromptHeightFraction;

        var prompt = new GameObject(action == HeroActionButton.MENU_PANE_LEFT ? "PaneLeftPrompt" : "PaneRightPrompt");
        var rect = prompt.AddComponent<RectTransform>();
        rect.SetParent(parent, false);
        var layout = prompt.AddComponent<LayoutElement>();
        layout.preferredWidth = size;
        layout.preferredHeight = size;
        layout.minWidth = size;
        layout.minHeight = size;
        layout.flexibleWidth = 0f;

        // Faded rather than deactivated when it does not apply, so the heading beside it keeps the
        // same place on screen instead of sliding as the highlight moves between columns.
        var group = prompt.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        var image = prompt.AddComponent<Image>();
        image.preserveAspect = true;
        image.raycastTarget = false;

        // The symbol goes on its own child stretched over the cap, so it stays centred on the art
        // whatever size the cap ended up.
        var symbolObject = new GameObject("Symbol");
        var symbolRect = symbolObject.AddComponent<RectTransform>();
        symbolRect.SetParent(rect, false);
        symbolRect.anchorMin = Vector2.zero;
        symbolRect.anchorMax = Vector2.one;
        symbolRect.offsetMin = Vector2.zero;
        symbolRect.offsetMax = Vector2.zero;
        var symbolText = symbolObject.AddComponent<Text>();
        ApplyTextStyle(symbolText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        symbolText.alignment = TextAnchor.MiddleCenter;
        symbolText.raycastTarget = false;
        symbolText.horizontalOverflow = HorizontalWrapMode.Overflow;
        // Sized to the cap rather than to the menu's body size, which would overflow a key cap.
        symbolText.resizeTextForBestFit = true;
        symbolText.resizeTextMinSize = 8;
        symbolText.resizeTextMaxSize = Mathf.Max(9, Mathf.RoundToInt(size * 0.7f));

        var driver = prompt.AddComponent<PanePromptGlyphDriver>();
        driver.Initialize(action, image, symbolText, layout, size);

        return group;
    }

    /// <summary>Cycles to the next or previous difficulty preset and writes its whole value set.</summary>
    private static void StepDifficultyPreset(int direction)
    {
        var presets = DifficultyPreset.All;
        if (presets.Length == 0)
            return;

        var current = DifficultyPreset.Identify(ModConfig.Instance);
        int index = current != null ? Array.IndexOf(presets, current) : -1;

        // From a custom set, stepping either way lands on a real preset rather than needing two
        // presses to leave a state that is not in the list.
        int next = index < 0
            ? (direction >= 0 ? 0 : presets.Length - 1)
            : ((index + direction) % presets.Length + presets.Length) % presets.Length;

        presets[next].ApplyTo(ModConfig.Instance);
        ApplyShadeMaskFractionToLiveShade();
    }

    private static string DescribeShadeMaskSetting()
    {
        float fraction = Mathf.Clamp(ModConfig.Instance.shadeMaskFraction, ModConfig.MinShadeMaskFraction, 1f);
        if (fraction <= ModConfig.MinShadeMaskFraction + 0.001f)
            return "Always 1";
        return Mathf.RoundToInt(fraction * 100f).ToString(CultureInfo.InvariantCulture) + "% of Hornet";
    }

    private static void StepShadeMaskFraction(int direction)
    {
        float fraction = Mathf.Clamp(ModConfig.Instance.shadeMaskFraction, ModConfig.MinShadeMaskFraction, 1f);
        int minStep = Mathf.RoundToInt(ModConfig.MinShadeMaskFraction * 10f);
        int steps = Mathf.RoundToInt(fraction * 10f) + (direction >= 0 ? 1 : -1);
        if (steps > 10)
            steps = minStep;
        else if (steps < minStep)
            steps = 10;
        ModConfig.Instance.shadeMaskFraction = steps / 10f;
        ApplyShadeMaskFractionToLiveShade();
    }

    /// <summary>
    /// Resizes the Shade standing in the scene right now rather than waiting for it to respawn.
    /// Nothing is saved here - the menu writes the config out when it closes.
    /// </summary>
    private static void ApplyShadeMaskFractionToLiveShade()
    {
        try
        {
            var shade = LegacyHelper.ShadeController.ActiveInstance;
            if (shade != null)
                shade.RefreshMaskCountFromConfig();
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not apply the Shade mask setting to the live Shade: {e}");
        }
    }

    private static bool GetShadeAssistMode()
    {
        try
        {
            var shade = LegacyHelper.ShadeController.ActiveInstance;
            if (shade != null)
                return shade.GetAssistModeEnabled();

            // No Shade in the scene - it is switched off, or this is not a gameplay scene. The
            // persisted "can take damage" flag is the same state seen from the other side.
            return !ShadeRuntime.PersistentState.CanTakeDamage;
        }
        catch
        {
            return false;
        }
    }

    private static void SetShadeAssistMode(bool enabled)
    {
        try
        {
            var shade = LegacyHelper.ShadeController.ActiveInstance;
            if (shade != null)
                shade.SetAssistMode(enabled);
            else
                LogMenuWarning("Assist mode was changed with no Shade in the scene; nothing to apply it to.");
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not change assist mode: {e}");
        }
    }

    /// <summary>Re-reads the Difficulty screen's rows after something changed one of them.</summary>
    private static void RefreshDifficultyHeader()
    {
        difficultyController?.RefreshAll();
    }

    /// <summary>
    /// Explicit 2D navigation for the Difficulty screen: the header row moves Left/Right across
    /// itself and Down into the panel below it, each panel moves Up/Down within itself, and the top
    /// row of each panel moves Up back to the header. Same reasoning as the Controls screen - see
    /// ConfigureControlsMenuNavigation for why automatic navigation is not enough here.
    /// </summary>
    private static void ConfigureDifficultyMenuNavigation(
        List<MenuButton> headerButtons,
        List<MenuSelectable> damageRows,
        List<MenuSelectable> healingRows)
    {
        for (int i = 0; i < headerButtons.Count; i++)
        {
            var button = headerButtons[i];
            if (button == null)
                continue;
            var nav = button.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = i > 0 ? headerButtons[i - 1] : null;
            nav.selectOnRight = i < headerButtons.Count - 1 ? headerButtons[i + 1] : null;
            nav.selectOnUp = null;
            // The first two cells sit over the Damage panel and the third over Healing, so Down
            // lands under the cell rather than always in the left column.
            var target = i >= 2 ? FirstOrNull(healingRows) : FirstOrNull(damageRows);
            if (target == null)
                target = FirstOrNull(healingRows);
            if (target == null)
                target = FirstOrNull(damageRows);
            nav.selectOnDown = target;
            button.navigation = nav;
        }

        MenuSelectable headerLeft = headerButtons.Count > 0 ? headerButtons[0] : null;
        MenuSelectable headerRight = headerButtons.Count > 2 ? headerButtons[2] : headerLeft;

        WireDifficultyColumn(damageRows, healingRows, headerLeft, isLeftColumn: true);
        WireDifficultyColumn(healingRows, damageRows, headerRight, isLeftColumn: false);
    }

    private static MenuSelectable FirstOrNull(List<MenuSelectable> rows)
        => rows != null && rows.Count > 0 ? rows[0] : null;

    private static void WireDifficultyColumn(
        List<MenuSelectable> column,
        List<MenuSelectable> otherColumn,
        MenuSelectable topRowUpFallback,
        bool isLeftColumn)
    {
        for (int i = 0; i < column.Count; i++)
        {
            var row = column[i];
            if (row == null)
                continue;

            var up = i > 0 ? column[i - 1] : topRowUpFallback;
            var down = i < column.Count - 1 ? column[i + 1] : null;
            var across = otherColumn.Count > 0 ? otherColumn[Mathf.Min(i, otherColumn.Count - 1)] : null;

            var nav = row.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = up;
            nav.selectOnDown = down;
            // Left/Right on a slider row belongs to the slider - SliderMenuDriver consumes those
            // moves to step the value - so crossing between panels is only wired for rows that are
            // not sliders. Up/Down still reaches every row in both columns.
            bool ownsHorizontal = row.GetComponent<SliderMenuDriver>() != null;
            if (!ownsHorizontal)
            {
                if (isLeftColumn)
                    nav.selectOnRight = across;
                else
                    nav.selectOnLeft = across;
            }
            row.navigation = nav;
        }
    }

    /// <summary>
    /// A minimal vertical scrollbar, styled to match the sliders elsewhere in this menu
    /// (same fallback track/knob sprites). Sits flush against the right edge of
    /// <paramref name="parent"/>. ScrollRect manages its value and handle size live once
    /// assigned to ScrollRect.verticalScrollbar -- no need to set those up front here.
    /// </summary>
    private static Scrollbar CreateVerticalScrollbar(RectTransform parent, float width)
    {
        var root = new GameObject("Scrollbar");
        var rootRect = root.AddComponent<RectTransform>();
        rootRect.SetParent(parent, false);
        rootRect.anchorMin = new Vector2(1f, 0f);
        rootRect.anchorMax = new Vector2(1f, 1f);
        rootRect.pivot = new Vector2(1f, 0.5f);
        rootRect.sizeDelta = new Vector2(width, 0f);
        rootRect.anchoredPosition = Vector2.zero;

        var bgImage = root.AddComponent<Image>();
        bgImage.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsSliderBg", true);
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);

        var slidingArea = new GameObject("Sliding Area");
        var slidingRect = slidingArea.AddComponent<RectTransform>();
        slidingRect.SetParent(rootRect, false);
        slidingRect.anchorMin = Vector2.zero;
        slidingRect.anchorMax = Vector2.one;
        slidingRect.offsetMin = new Vector2(4f, 4f);
        slidingRect.offsetMax = new Vector2(-4f, -4f);

        var handle = new GameObject("Handle");
        var handleRect = handle.AddComponent<RectTransform>();
        handleRect.SetParent(slidingRect, false);
        handleRect.sizeDelta = Vector2.zero;
        var handleImage = handle.AddComponent<Image>();
        handleImage.sprite = GetFallbackSprite(ref fallbackKnobSprite, "ShadeSettingsSliderKnob", false);
        handleImage.color = new Color(0.85f, 0.85f, 0.88f, 0.95f);

        var scrollbar = root.AddComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleImage;
        scrollbar.transition = Selectable.Transition.ColorTint;
        var colors = scrollbar.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        scrollbar.colors = colors;

        return scrollbar;
    }

    private static void BuildControlsMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        // content's VerticalLayoutGroup (childControlHeight/Width=true) turned out not to
        // reliably apply computed sizes to this screen's direct children at all -- logged
        // diagnostics showed presetRow ending up at height 0 despite its LayoutElement
        // correctly holding the real measured value, and scrollWrapper never getting sized
        // beyond RectTransform's raw default (100x100), even after an explicit
        // ForceRebuildLayoutImmediate. Rather than continue chasing why (this is the third
        // distinct instance of Unity's automatic layout system not doing what its
        // documented behaviour says it should, just on this one screen), info/presetRow/
        // scrollWrapper are now positioned and sized explicitly in code below, the same way
        // bindingsRect and the preset cards already are. The layout group stays on
        // (untouched, shared by every other menu screen via CreateContentRoot) but is
        // disabled here so it can't fight with the explicit values.
        var contentLayout = content.GetComponent<VerticalLayoutGroup>();
        if (contentLayout != null)
        {
            contentLayout.enabled = false;
        }

        bindingDrivers.Clear();

        // Screen.width/height are the display's raw pixel resolution, not the UI canvas's
        // logical size -- on a 4K display running a 1080p-designed canvas (a 2x
        // CanvasScaler factor), Screen.height reports 2160 while everything actually
        // measuring this screen (content, ms.transform itself) is in ~1080-unit space.
        // Every one of these percentage-based margins was computing against a space twice
        // as large as the one it's applied in, which meant they clamped to their maximum
        // on this setup regardless of the intended proportion. ms.transform's own rect is
        // the actual coordinate space everything here is positioned in, so calculations
        // are based on that instead.
        var msRect = (RectTransform)ms.transform;
        float canvasWidth = msRect.rect.width;
        float canvasHeight = msRect.rect.height;

        float horizontalMargin = Mathf.Max(48f, canvasWidth * 0.05f);
        float bottomMargin = canvasHeight * ListBottomMarginFraction;
        // A little tighter than the other screens: this one carries the most rows, and its own
        // heading line already provides some of the band.
        float topMargin = canvasHeight * (ListTopMarginFraction - 0.03f);

        if (content != null)
        {
            content.offsetMin = new Vector2(horizontalMargin, bottomMargin);
            content.offsetMax = new Vector2(-horizontalMargin, -topMargin);
            // No upward nudge. It used to shove the whole screen up by a few percent of its height,
            // which is dead space at the bottom now that the margins are proportional.
        }

        // content's own size doesn't depend on its children or on contentLayout (now
        // disabled anyway) -- it comes purely from the anchors/offsets just set above,
        // relative to ms.transform, which InitializeScreen already sized before this
        // method ever runs. Safe to read right now, before anything below is built.
        float availableContentHeight = content.rect.height;
        const float SectionSpacing = 20f;
        float sectionCursorY = 0f;

        var info = new GameObject("ControlsInfo");
        var infoRect = info.AddComponent<RectTransform>();
        infoRect.SetParent(content, false);
        infoRect.anchorMin = new Vector2(0f, 1f);
        infoRect.anchorMax = new Vector2(1f, 1f);
        infoRect.pivot = new Vector2(0.5f, 1f);
        const float InfoHeight = 48f;
        infoRect.anchoredPosition = new Vector2(0f, -sectionCursorY);
        infoRect.sizeDelta = new Vector2(0f, InfoHeight);
        var infoText = info.AddComponent<Text>();
        ApplyTextStyle(infoText, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        infoText.text = "Select a binding to change it. Press Backspace to clear or press a controller button to bind.";
        ScaleTextElements(info, 0.85f);
        sectionCursorY += InfoHeight + SectionSpacing;

        var selectables = new List<MenuSelectable>();
        var presetButtons = new List<MenuButton>();

        var presetRow = new GameObject("PresetOptions");
        var presetRect = presetRow.AddComponent<RectTransform>();
        presetRect.SetParent(content, false);
        presetRect.anchorMin = new Vector2(0f, 1f);
        presetRect.anchorMax = new Vector2(1f, 1f);
        presetRect.pivot = new Vector2(0.5f, 1f);
        // Height (sizeDelta.y) is filled in further down once every card's actual
        // (wrapped-text-dependent) height is measured -- see AddPresetOption. Position can
        // be set now: it only depends on what's already been stacked above it.
        presetRect.anchoredPosition = new Vector2(0f, -sectionCursorY);
        presetRect.sizeDelta = Vector2.zero;
        var presetLayout = presetRow.AddComponent<HorizontalLayoutGroup>();
        float presetSpacing = Mathf.Clamp(canvasWidth * 0.035f, 32f, 90f);
        // The maximum here used to be -80, below the minimum, so Mathf.Clamp returned -80 for
        // every width and the preset row hung 80 units past each edge of the content it sits in.
        int sidePadding = Mathf.RoundToInt(Mathf.Clamp(canvasWidth * 0.04f, 36f, 120f));
        float presetCardPreferredWidth = Mathf.Clamp(canvasWidth * 0.22f, 260f, 430f);
        float presetCardMinWidth = Mathf.Clamp(canvasWidth * 0.16f, 200f, presetCardPreferredWidth);
        presetLayout.spacing = presetSpacing;
        presetLayout.padding = new RectOffset(sidePadding, sidePadding, 0, 0);
        presetLayout.childControlWidth = true;
        presetLayout.childControlHeight = false;
        presetLayout.childForceExpandWidth = true;
        presetLayout.childForceExpandHeight = false;
        presetLayout.childAlignment = TextAnchor.UpperCenter;
        var presetLayoutElement = presetRow.AddComponent<LayoutElement>();
        // Real value computed below, once every card's actual (wrapped-text-dependent)
        // height is known -- see the comment inside AddPresetOption for why this can't be
        // left to Unity's own layout computation. Must not compete for flexible space
        // either: content's VerticalLayoutGroup splits leftover height between every child
        // with flexibleHeight > 0, so if this claimed a share too, the binding scroll view
        // below only ever got roughly half of what was left over -- which is what made it
        // show ~2 rows instead of filling the available height.
        presetLayoutElement.flexibleHeight = 0f;
        var presetCardHeights = new List<float>();

        // One button per preset. The description that used to sit under each one is now shown in
        // the footer for whichever is highlighted, the way every other screen here explains itself.
        // That description was measured at presetCardPreferredWidth and stamped as an explicit
        // height, so once the cards grew wider than that guess the wrapped lines no longer matched
        // the space reserved and piled on top of each other. Nothing to measure, nothing to garble.
        var presetDescriptions = new List<KeyValuePair<MenuSelectable, string>>();

        void AddPresetOption(string label, string description, System.Action onSubmit)
        {
            var selectable = CreateMenuButton(presetRow.transform, buttonTemplate, label, onSubmit, CancelTarget.ShadeMain);
            if (selectable == null)
            {
                return;
            }

            var layout = selectable.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.minWidth = presetCardMinWidth;
                layout.preferredWidth = presetCardPreferredWidth;
                layout.flexibleWidth = 1f;
                layout.minHeight = ButtonRowHeight;
                layout.preferredHeight = ButtonRowHeight;
                layout.flexibleHeight = 0f;
            }

            var optionRect = selectable.GetComponent<RectTransform>();
            if (optionRect != null)
            {
                optionRect.sizeDelta = new Vector2(optionRect.sizeDelta.x, ButtonRowHeight);
            }

            selectables.Add(selectable);
            if (selectable is MenuButton button)
            {
                presetButtons.Add(button);
            }

            presetDescriptions.Add(new KeyValuePair<MenuSelectable, string>(selectable, description));
            presetCardHeights.Add(ButtonRowHeight);
        }

        AddPresetOption("Default", "Shade keeps the original keyboard layout. Hornet stays on controller and keyboard hotkeys stay disabled.", ApplyDefaultPreset);
        AddPresetOption("Two Controllers", "Shade uses the second controller with dedicated buttons while Hornet remains on the first controller.", ApplyDualControllerPresetOption);
        AddPresetOption("Keyboard Only", "Shade moves to the keypad while Hornet's controls jump to the left side of the keyboard. Controllers are disabled.", ApplyKeyboardOnlyPresetOption);
        AddPresetOption("Shade Controller", "Shade uses the first controller layout and Hornet swaps to left-side keyboard hotkeys with the controller disabled.", ApplyShadeControllerPresetOption);

        // content's VerticalLayoutGroup (unlike the two levels below presetRow) does have
        // childControlHeight=true, so it *will* correctly apply whatever height
        // presetLayoutElement reports here -- no need to also set presetRect.sizeDelta
        // directly the way the card and description RectTransforms needed above.
        float maxPresetCardHeight = presetCardHeights.Count > 0 ? Mathf.Max(presetCardHeights.ToArray()) : ButtonRowHeight;
        presetLayoutElement.minHeight = maxPresetCardHeight;
        presetLayoutElement.preferredHeight = maxPresetCardHeight;
        // presetLayout (HorizontalLayoutGroup, on this same row) has childControlHeight=
        // false, and contentLayout above is now disabled entirely -- neither pushes a
        // computed height onto this RectTransform, so it's set directly, same as every
        // card's optionRect already was.
        presetRect.sizeDelta = new Vector2(0f, maxPresetCardHeight);
        // A little more breathing room here specifically than elsewhere -- requested after
        // seeing the presets sit right on top of the binding list with no visual break.
        const float PresetToBindingsSpacing = 40f;
        sectionCursorY += maxPresetCardHeight + PresetToBindingsSpacing;

        // The binding list can be taller than the screen (worse once the debug rows are
        // added), so it lives in its own scroll view rather than as a direct child of
        // content -- otherwise it simply overflows the bottom edge with nothing able to
        // reach it, by mouse or otherwise.
        const float ScrollbarWidth = 26f;
        const float ScrollbarGap = 10f;
        // Both shrunk from 70/32 to fit noticeably more rows in the same space, per
        // feedback that the list should show more of itself at once. Still comfortably
        // large click targets.
        const float BindingRowHeight = 58f;
        float bindingRowSpacing = 18f;

        // Reserved before the scroll view takes the rest, so the explanation line always has room.
        float controlsFooterHeight = DescriptionRowHeight;

        var scrollWrapper = new GameObject("BindingScrollView");
        var scrollWrapperRect = scrollWrapper.AddComponent<RectTransform>();
        scrollWrapperRect.SetParent(content, false);
        scrollWrapperRect.anchorMin = new Vector2(0f, 1f);
        scrollWrapperRect.anchorMax = new Vector2(1f, 1f);
        scrollWrapperRect.pivot = new Vector2(0.5f, 1f);
        // Takes every bit of height content has left after info and the preset row -- this
        // is the section that actually benefits from more space, unlike the fixed header
        // above it.
        float scrollWrapperHeight = Mathf.Max(0f, availableContentHeight - sectionCursorY - controlsFooterHeight - SectionSpacing);
        scrollWrapperRect.anchoredPosition = new Vector2(0f, -sectionCursorY);
        scrollWrapperRect.sizeDelta = new Vector2(0f, scrollWrapperHeight);

        var viewportGo = new GameObject("Viewport");
        var viewportRect = viewportGo.AddComponent<RectTransform>();
        viewportRect.SetParent(scrollWrapperRect, false);
        viewportRect.anchorMin = new Vector2(0f, 0f);
        viewportRect.anchorMax = new Vector2(1f, 1f);
        viewportRect.pivot = new Vector2(0.5f, 0.5f);
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = new Vector2(-(ScrollbarWidth + ScrollbarGap), 0f);
        viewportGo.AddComponent<RectMask2D>();

        var bindingsContainer = new GameObject("BindingColumns");
        var bindingsRect = bindingsContainer.AddComponent<RectTransform>();
        bindingsRect.SetParent(viewportRect, false);
        bindingsRect.anchorMin = new Vector2(0f, 1f);
        bindingsRect.anchorMax = new Vector2(1f, 1f);
        bindingsRect.pivot = new Vector2(0.5f, 1f);
        bindingsRect.anchoredPosition = Vector2.zero;
        // X is fully stretched (anchorMin/Max.x = 0/1), so sizeDelta.x is an *offset* from
        // that stretch, not a size -- leaving it at RectTransform's default of 100 (never
        // set otherwise here) rendered this 100px wider than the viewport on every build.
        // Y is a point anchor (anchorMin/Max.y both = 1), so sizeDelta.y is the actual
        // height, which the row-count-based calculation after AddBindingButton fills in.
        bindingsRect.sizeDelta = new Vector2(0f, 0f);
        var bindingsLayout = bindingsContainer.AddComponent<HorizontalLayoutGroup>();
        bindingsLayout.spacing = 32f;
        bindingsLayout.childControlWidth = true;
        bindingsLayout.childControlHeight = true;
        bindingsLayout.childForceExpandWidth = true;
        bindingsLayout.childForceExpandHeight = false;
        bindingsLayout.childAlignment = TextAnchor.UpperLeft;
        // Deliberately not sized via ContentSizeFitter: as ScrollRect content nested three
        // layout groups deep (this HorizontalLayoutGroup -> each column's
        // VerticalLayoutGroup -> fixed-height row buttons), relying on Unity's bottom-up
        // preferred-size computation to settle correctly here proved unreliable in practice.
        // Every row is a fixed, known height, so the total is computed directly instead --
        // see the sizeDelta.y assignment after the physical rows are built below.

        var scrollRect = scrollWrapper.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.viewport = viewportRect;
        scrollRect.content = bindingsRect;
        scrollRect.scrollSensitivity = 24f;

        var scrollbar = CreateVerticalScrollbar(scrollWrapperRect, ScrollbarWidth);
        scrollRect.verticalScrollbar = scrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        var scrollIntoView = scrollWrapper.AddComponent<ScrollIntoViewDriver>();
        scrollIntoView.scrollRect = scrollRect;
        scrollIntoView.viewport = viewportRect;
        scrollIntoView.content = bindingsRect;

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
            layout.spacing = bindingRowSpacing;
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
                layout.minHeight = BindingRowHeight;
                layout.preferredHeight = BindingRowHeight;
            }
            var rect = btn.GetComponent<RectTransform>();
            if (rect != null)
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BindingRowHeight);
        }

        MenuButton AddBindingButton(Transform parent, ShadeAction action, string label, bool secondary)
        {
            var selectable = CreateMenuButton(parent, buttonTemplate, string.Empty, null, CancelTarget.ShadeMain);
            if (selectable is MenuButton btn)
            {
                var driver = btn.gameObject.AddComponent<BindingMenuDriver>();
                driver.Initialize(btn, action, secondary, label);
                ConfigureBindingButton(btn);
                selectables.Add(btn);
                return btn;
            }
            return null;
        }

        var bindingRowList = new List<(ShadeAction action, string label)>
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
            (ShadeAction.CommandShade, "Command Shade")
        };

        // Only surfaced while the "Debug Keys" toggle in Debug Options is on -- these bind
        // the developer HP/soul cheats in SimpleHUD.HandleDebugKeys, otherwise invisible.
        if (ModConfig.Instance.debugKeysEnabled)
        {
            bindingRowList.Add((ShadeAction.DebugDamageShade, "Debug: Damage Shade"));
            bindingRowList.Add((ShadeAction.DebugHealShade, "Debug: Heal Shade"));
            bindingRowList.Add((ShadeAction.DebugSoulIncrease, "Debug: Increase Soul"));
            bindingRowList.Add((ShadeAction.DebugSoulDecrease, "Debug: Decrease Soul"));
            bindingRowList.Add((ShadeAction.DebugSoulReset, "Debug: Reset Soul"));
        }

        // Expand into actual physical rows first -- Nail contributes two stacked rows
        // (Primary/Alt), not one, so splitting columns by *action* count (as before) could
        // silently produce mismatched, misaligned column heights even before the debug rows
        // existed. Splitting by physical row count keeps both columns the same height and
        // keeps row [k] in the left column level with row [k] in the right column, which is
        // what makes a sensible Left/Right grid (below) possible in the first place.
        var physicalRows = new List<(ShadeAction action, string label, bool secondary)>();
        foreach (var (action, label) in bindingRowList)
        {
            if (action == ShadeAction.Nail)
            {
                physicalRows.Add((action, label + " (Primary)", false));
                physicalRows.Add((action, label + " (Alt)", true));
            }
            else
            {
                physicalRows.Add((action, label, false));
            }
        }

        var leftColumnButtons = new List<MenuButton>();
        var rightColumnButtons = new List<MenuButton>();
        int leftPhysicalCount = (physicalRows.Count + 1) / 2;
        for (int i = 0; i < physicalRows.Count; i++)
        {
            var row = physicalRows[i];
            bool isLeft = i < leftPhysicalCount;
            var parent = isLeft ? leftColumn.transform : rightColumn.transform;
            var button = AddBindingButton(parent, row.action, row.label, row.secondary);
            if (button == null)
                continue;
            (isLeft ? leftColumnButtons : rightColumnButtons).Add(button);
        }

        // Every row is BindingRowHeight tall with bindingRowSpacing between rows, so the
        // content height is exactly derivable from whichever column has more rows -- no
        // need to ask Unity's layout system to work it out (see the note above).
        int tallestColumnRows = Mathf.Max(leftColumnButtons.Count, rightColumnButtons.Count);
        float contentHeight = tallestColumnRows > 0
            ? tallestColumnRows * BindingRowHeight + (tallestColumnRows - 1) * bindingRowSpacing
            : 0f;
        bindingsRect.sizeDelta = new Vector2(0f, contentHeight);

        // A scrollbar reserved beside a list that doesn't actually need to scroll just
        // reads as clutter. Hide it and let the viewport reclaim the width it was leaving
        // for it, but only decided now that the real content height is known -- with debug
        // keys off there may be few enough rows to fit without scrolling at all, while
        // turning them on can push it back over.
        bool needsScroll = contentHeight > scrollWrapperHeight + 0.5f;
        scrollbar.gameObject.SetActive(needsScroll);
        scrollRect.vertical = needsScroll;
        viewportRect.offsetMax = needsScroll
            ? new Vector2(-(ScrollbarWidth + ScrollbarGap), 0f)
            : Vector2.zero;

        var controlsFooter = new GameObject("Description");
        var controlsFooterRect = controlsFooter.AddComponent<RectTransform>();
        controlsFooterRect.SetParent(content, false);
        controlsFooterRect.anchorMin = new Vector2(0f, 1f);
        controlsFooterRect.anchorMax = new Vector2(1f, 1f);
        controlsFooterRect.pivot = new Vector2(0.5f, 1f);
        controlsFooterRect.anchoredPosition = new Vector2(0f, -(availableContentHeight - controlsFooterHeight));
        controlsFooterRect.sizeDelta = new Vector2(0f, controlsFooterHeight);
        var controlsFooterText = controlsFooter.AddComponent<Text>();
        ApplyTextStyle(controlsFooterText, toggleLabelStyle, TextAnchor.UpperCenter, DescriptionColor);
        controlsFooterText.text = string.Empty;
        controlsFooterText.raycastTarget = false;
        controlsFooterText.horizontalOverflow = HorizontalWrapMode.Wrap;
        controlsFooterText.verticalOverflow = VerticalWrapMode.Truncate;
        controlsFooterText.fontSize = Mathf.Max(12, Mathf.RoundToInt(controlsFooterText.fontSize * 0.78f));
        var controlsFooterDriver = controlsFooter.AddComponent<MenuDescriptionDriver>();
        controlsFooterDriver.target = controlsFooterText;
        foreach (var entry in presetDescriptions)
        {
            controlsFooterDriver.Register(entry.Key, entry.Value);
        }

        SetupButtonList(ms, selectables);
        ConfigureControlsMenuNavigation(presetButtons, leftColumnButtons, rightColumnButtons);
        // MenuButtonList.Start() -- fired later by Unity, not by the SetupButtonList call
        // above -- unconditionally reasserts flat-list Up/Down for every Explicit-mode
        // selectable, clobbering the 2D grid just configured. Reapply once after that has
        // had its chance to run. See DeferredNavigationReapplyDriver for why.
        var navigationReapply = ms.gameObject.AddComponent<DeferredNavigationReapplyDriver>();
        navigationReapply.Reapply = () => ConfigureControlsMenuNavigation(presetButtons, leftColumnButtons, rightColumnButtons);
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(bindingsRect);

        // Temporary diagnostic: the last two rounds of fixing this screen's vertical space
        // were both based on estimating actual sizes from a screenshot, and both
        // undershot. Logging the real, final, settled numbers here instead of guessing
        // again from another picture. Goes through this file's own BepInEx ManualLogSource
        // (like every other ShadeSettingsMenu log line) rather than LogMenuInfo, so it
        // fires regardless of the Menu Logs setting -- this is a one-off diagnostic, not
        // something a player should have to opt into. Plain Debug.Log was tried first and
        // silently went nowhere: this BepInEx install failed to hook Unity's own log
        // writer ("Unable to start Unity log writer" at the top of LogOutput.log), so
        // Debug.Log calls aren't captured at all here, only calls through a
        // ManualLogSource are. Safe to remove once the sizing is confirmed correct.
        log.LogInfo(
            $"Controls layout: screen={Screen.width}x{Screen.height} " +
            $"msScreen={((RectTransform)ms.transform).rect.width:0}x{((RectTransform)ms.transform).rect.height:0} " +
            $"content={content.rect.width:0}x{content.rect.height:0} " +
            $"topMargin={topMargin:0} bottomMargin={bottomMargin:0} horizontalMargin={horizontalMargin:0} " +
            $"info={infoRect.rect.width:0}x{infoRect.rect.height:0} " +
            $"presetRow={presetRect.rect.width:0}x{presetRect.rect.height:0} maxPresetCardHeight={maxPresetCardHeight:0} " +
            $"scrollWrapper={scrollWrapperRect.rect.width:0}x{scrollWrapperRect.rect.height:0} " +
            $"viewport={viewportRect.rect.width:0}x{viewportRect.rect.height:0} " +
            $"bindingsContent={bindingsRect.rect.width:0}x{bindingsRect.rect.height:0} " +
            $"leftRows={leftColumnButtons.Count} rightRows={rightColumnButtons.Count}");
    }

    /// <summary>
    /// Builds explicit 2D navigation for the Controls screen: the preset row moves with
    /// Left/Right across itself and Down into whichever column is nearest; each binding
    /// column moves Up/Down within itself and Left/Right across to the same row index in
    /// the other column (clamped to that column's last row if it's shorter); the top row of
    /// each column moves Up back to the preset row.
    ///
    /// This replaces relying on Unity's Navigation.Mode.Automatic (spatial nearest-neighbour
    /// guessing) for anything beyond the preset row: automatic mode has no notion of "this
    /// button belongs to the left column", so once the two columns can have a different
    /// number of rows (which they always could, once Nail's two stacked rows are accounted
    /// for -- see the physical-row split above) its guesses stop being reliable, which is
    /// what produced the "everything just goes up/down" behaviour this replaces.
    /// </summary>
    private static void ConfigureControlsMenuNavigation(
        List<MenuButton> presetButtons,
        List<MenuButton> leftColumnButtons,
        List<MenuButton> rightColumnButtons)
    {
        ConfigureHorizontalNavigation(presetButtons);

        // Presets: keep the left/right chain ConfigureHorizontalNavigation just set (and
        // leave Up alone -- nothing above the top row). Left half of the row drops Down into
        // the left column's first row, right half into the right column's.
        if (presetButtons.Count > 0)
        {
            MenuButton leftTarget = leftColumnButtons.Count > 0 ? leftColumnButtons[0]
                : (rightColumnButtons.Count > 0 ? rightColumnButtons[0] : null);
            MenuButton rightTarget = rightColumnButtons.Count > 0 ? rightColumnButtons[0]
                : (leftColumnButtons.Count > 0 ? leftColumnButtons[0] : null);

            int half = (presetButtons.Count + 1) / 2;
            for (int i = 0; i < presetButtons.Count; i++)
            {
                var preset = presetButtons[i];
                if (preset == null)
                    continue;
                var nav = preset.navigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnDown = i < half ? leftTarget : rightTarget;
                preset.navigation = nav;
            }
        }

        var leftTopFallback = presetButtons.Count > 0 ? presetButtons[0] : null;
        var rightTopFallback = presetButtons.Count > 0 ? presetButtons[presetButtons.Count - 1] : null;

        WireBindingColumn(leftColumnButtons, rightColumnButtons, leftTopFallback, isLeftColumn: true);
        WireBindingColumn(rightColumnButtons, leftColumnButtons, rightTopFallback, isLeftColumn: false);
    }

    /// <summary>
    /// Wires Up/Down within one binding column and Left/Right across to the row at the same
    /// index in the other column, clamped to that column's last row if it's shorter. Always
    /// switches to Explicit mode -- see ConfigureControlsMenuNavigation for why
    /// Navigation.Mode.Automatic's spatial guessing isn't reliable enough for this layout.
    /// </summary>
    private static void WireBindingColumn(
        List<MenuButton> column,
        List<MenuButton> otherColumn,
        MenuButton topRowUpFallback,
        bool isLeftColumn)
    {
        for (int i = 0; i < column.Count; i++)
        {
            var button = column[i];
            if (button == null)
                continue;

            var up = i > 0 ? column[i - 1] : topRowUpFallback;
            var down = i < column.Count - 1 ? column[i + 1] : null;
            var across = otherColumn.Count > 0 ? otherColumn[Mathf.Min(i, otherColumn.Count - 1)] : null;

            var nav = button.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = up;
            nav.selectOnDown = down;
            if (isLeftColumn)
                nav.selectOnRight = across;
            else
                nav.selectOnLeft = across;
            button.navigation = nav;
        }
    }

    private static void BuildLoggingMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;
        var selectables = new List<MenuSelectable>();
        void AddToggle(string label, bool value, System.Action<bool> onChange)
        {
            var t = CreateToggle(content, buttonTemplate, label, value, onChange, CancelTarget.ShadeMain);
            if (t != null) selectables.Add(t);
        }

        // Not a LabeledToggleDriver like the rows below it: turning the Shade off destroys the live
        // instance and turning it back on respawns one, so it needs ShadeToggleDriver's path through
        // LegacyHelper.SetShadeEnabled rather than a plain config write.
        var shadeToggle = CreateMenuButton(content, buttonTemplate, GetShadeToggleLabel(), null, CancelTarget.ShadeMain);
        if (shadeToggle is MenuButton shadeToggleButton)
        {
            var shadeDriver = shadeToggleButton.gameObject.AddComponent<ShadeToggleDriver>();
            shadeDriver.Initialize(shadeToggleButton);
            selectables.Add(shadeToggleButton);
        }
        else if (shadeToggle != null)
        {
            selectables.Add(shadeToggle);
        }

        AddToggle("General Logs", ModConfig.Instance.logGeneral, v => ModConfig.Instance.logGeneral = v);
        AddToggle("Menu Logs", ModConfig.Instance.logMenu, v => ModConfig.Instance.logMenu = v);
        AddToggle("Shade Debug Logs", ModConfig.Instance.logShade, v => ModConfig.Instance.logShade = v);
        AddToggle("HUD Debug Logs", ModConfig.Instance.logHud, v => ModConfig.Instance.logHud = v);
        AddToggle("Damage Summary File", ModConfig.Instance.logDamage, v => ModConfig.Instance.logDamage = v);
        AddToggle("Debug Keys (HP/Soul)", ModConfig.Instance.debugKeysEnabled, v => ModConfig.Instance.debugKeysEnabled = v);
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

    /// <summary>
    /// A line of explanation for whichever row is highlighted, pinned to the bottom of the screen.
    /// <para>
    /// The options these screens carry cannot be named descriptively enough to stand alone -
    /// "Spell Group Size" means nothing without a sentence - and this is cheaper than a tooltip
    /// system. The spacer above it takes the slack, so the line sits at the bottom rather than
    /// floating directly under the last row.
    /// </para>
    /// </summary>
    private static MenuDescriptionDriver CreateDescriptionFooter(RectTransform content)
    {
        var spacer = new GameObject("Spacer");
        var spacerRect = spacer.AddComponent<RectTransform>();
        spacerRect.SetParent(content, false);
        var spacerLayout = spacer.AddComponent<LayoutElement>();
        spacerLayout.minHeight = 0f;
        spacerLayout.preferredHeight = 0f;
        spacerLayout.flexibleHeight = 1f;

        var footer = new GameObject("Description");
        var footerRect = footer.AddComponent<RectTransform>();
        footerRect.SetParent(content, false);
        var text = footer.AddComponent<Text>();
        ApplyTextStyle(text, toggleLabelStyle, TextAnchor.UpperLeft, DescriptionColor);
        text.text = string.Empty;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.fontSize = Mathf.Max(12, Mathf.RoundToInt(text.fontSize * 0.78f));

        var footerLayout = footer.AddComponent<LayoutElement>();
        footerLayout.minHeight = DescriptionRowHeight;
        footerLayout.preferredHeight = DescriptionRowHeight;
        footerLayout.flexibleHeight = 0f;

        var driver = footer.AddComponent<MenuDescriptionDriver>();
        driver.target = text;
        return driver;
    }

    /// <summary>
    /// Finishes an options screen: the description footer, the navigation list, the default
    /// highlight and the back button. Shared by both Shade AI screens.
    /// </summary>
    private static void FinishOptionsScreen(UIManager ui, MenuScreen ms, RectTransform content, List<MenuSelectable> selectables, List<KeyValuePair<MenuSelectable, string>> descriptions, CancelTarget cancelTarget)
    {
        // Built after the rows so it is the last thing in the layout, then told about them.
        var footer = CreateDescriptionFooter(content);
        if (footer != null && descriptions != null)
        {
            foreach (var entry in descriptions)
            {
                footer.Register(entry.Key, entry.Value);
            }
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

        ConfigureBackButton(ms, cancelTarget, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    /// <summary>
    /// The Shade AI category: the three things a player actually decides, and a way through to the
    /// rest.
    /// <para>
    /// This screen scrolled when it carried ten rows, which was the only screen here that did, and
    /// the RectMask2D that scrolling needs clipped the selection fleurs - they are cloned from the
    /// button template with the template's own offsets, so they sit just outside each row and just
    /// outside the viewport with it. Splitting the list so neither screen needs to scroll fixes that
    /// and the "far too much detail" problem in the same move.
    /// </para>
    /// </summary>
    private static void BuildShadeAiMenu(UIManager ui, MenuScreen ms, GameObject sliderTemplate, MenuButton buttonTemplate)
    {
        if (ms == null || sliderTemplate == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        var selectables = new List<MenuSelectable>();
        var descriptions = new List<KeyValuePair<MenuSelectable, string>>();

        void Add(MenuSelectable selectable, string description)
        {
            if (selectable == null)
                return;
            selectables.Add(selectable);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(selectable, description));
        }

        Add(CreateToggle(content, buttonTemplate, "Shade AI", ModConfig.Instance.shadeAiEnabled, v =>
            {
                ModConfig.Instance.shadeAiEnabled = v;
                // Apply to the Shade standing in the scene right now rather than waiting for a
                // respawn. persist:false because this menu owns the value and saves it on close.
                try
                {
                    var shade = LegacyHelper.ShadeController.ActiveInstance;
                    if (shade != null)
                        shade.SetShadeAiEnabled(v, persist: false);
                }
                catch
                {
                }
            }, CancelTarget.ShadeMain),
            "Let the Shade fight by itself. It picks targets, attacks, steps out of danger and heals you both. It can be killed, so you will need to revive it.");

        Add(CreateSlider(content, sliderTemplate, buttonTemplate, "Attack Speed", 0.1f, 1f, ModConfig.Instance.shadeAiAttackSpeedFraction,
                v => ModConfig.Instance.shadeAiAttackSpeedFraction = v, CancelTarget.ShadeMain),
            "How fast the Shade swings, as a share of the fastest the game allows. Lower leaves more of the fight to you. Quick Slash still speeds it up.");

        if (shadeAiAdvancedScreen != null)
        {
            Add(CreateMenuButton(content, buttonTemplate, "Advanced AI Options", () => ShowScreen(shadeAiAdvancedScreen), CancelTarget.ShadeMain),
                "How the Shade dodges, when it stops to heal, and how far it will roam to reach an enemy.");
        }

        FinishOptionsScreen(ui, ms, content, selectables, descriptions, CancelTarget.ShadeMain);
    }

    /// <summary>
    /// The rest of the AI settings. Kept off the main AI screen because none of them are decisions a
    /// player needs to make to use the feature.
    /// <para>
    /// A few knobs are deliberately config-only rather than shown here - how tanky one enemy must be
    /// to be worth a spell, how long the AI stands down after you take over, and how often it rescans
    /// the scene. They are documented in <see cref="ModConfig"/>; putting them on screen would cost
    /// this screen its scrollbar-free layout for options nobody adjusts twice.
    /// </para>
    /// </summary>
    private static void BuildShadeAiAdvancedMenu(UIManager ui, MenuScreen ms, GameObject sliderTemplate, MenuButton buttonTemplate)
    {
        if (ms == null || sliderTemplate == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        var selectables = new List<MenuSelectable>();
        var descriptions = new List<KeyValuePair<MenuSelectable, string>>();

        void AddToggle(string label, string description, bool value, System.Action<bool> onChange)
        {
            var t = CreateToggle(content, buttonTemplate, label, value, onChange, CancelTarget.ShadeAi);
            if (t == null)
                return;
            selectables.Add(t);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(t, description));
        }

        void AddSlider(string label, string description, float min, float max, float value, System.Action<float> onChange, bool whole = false)
        {
            var s = CreateSlider(content, sliderTemplate, buttonTemplate, label, min, max, value, onChange, CancelTarget.ShadeAi, whole);
            if (s == null)
                return;
            selectables.Add(s);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(s, description));
        }

        AddToggle("Dodge Attacks",
            "Step out of enemy attacks and hazards instead of standing in them. Turn this off and the Shade will trade hits.",
            ModConfig.Instance.shadeAiAvoidAttacks,
            v => ModConfig.Instance.shadeAiAvoidAttacks = v);

        AddToggle("Heal When Low",
            "Save SOUL for healing rather than spells, and stop to channel Focus when someone needs it. Healing Hornet also heals the Shade, and needs it standing close.",
            ModConfig.Instance.shadeAiHealWhenLow,
            v => ModConfig.Instance.shadeAiHealWhenLow = v);

        AddSlider("Heal Shade Below",
            "How hurt the Shade has to be before it breaks off to heal itself.",
            0f, 1f, ModConfig.Instance.shadeAiSelfHealBelow,
            v => ModConfig.Instance.shadeAiSelfHealBelow = v);

        AddSlider("Heal Hornet Below",
            "How hurt you have to be before the Shade comes to you and heals instead of fighting.",
            0f, 1f, ModConfig.Instance.shadeAiHornetHealBelow,
            v => ModConfig.Instance.shadeAiHornetHealBelow = v);

        AddSlider("Engage Range",
            "How far the Shade will travel to reach an enemy. It never goes further than its leash on Hornet allows.",
            2f, 40f, ModConfig.Instance.shadeAiEngageRadius,
            v => ModConfig.Instance.shadeAiEngageRadius = v);

        FinishOptionsScreen(ui, ms, content, selectables, descriptions, CancelTarget.ShadeAi);
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

        // Capture where the player was on the outgoing screen before anything is deactivated - once
        // the screen is off, the EventSystem's selection is gone and there is nothing left to read.
        RememberSelection(activeScreen);

        target.transform.SetAsLastSibling();
        var previous = activeScreen;

        // Decide the highlight and hand it to the screen's MenuButtonList *before* activating it, so
        // the DoSelect its OnEnable kicks off already agrees with what this method is about to set.
        var highlight = GetRestoreHighlight(target);
        SyncButtonListSelection(target, highlight);

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
        }
        else if (target == loggingScreen)
        {
            // The Shade Enabled row lives here now, and SetShadeEnabled can also be called from
            // outside this menu, so its label is re-read whenever the screen comes up.
            consumeNextToggle = false;
            NotifyShadeToggleChanged();
        }
        else if (target == difficultyScreen)
        {
            consumeNextToggle = false;
            difficultyController?.HandleScreenShown();
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
        // The controllers above (charms/skins) can rebuild their rows on show, which can retire the
        // selectable chosen before activation; re-resolve rather than highlighting a dead row.
        if (highlight == null || !highlight.gameObject.activeInHierarchy)
        {
            highlight = GetRestoreHighlight(target);
            SyncButtonListSelection(target, highlight);
        }

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

    private static void ShowShadeAiMenu()
    {
        ShowScreen(shadeAiScreen);
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
