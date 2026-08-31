#nullable disable
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GlobalEnums;
using Object = UnityEngine.Object;

// Screen scaffolding shared by every settings screen - building one from the game's own menu
// prefabs, stripping what the clone brings with it, button rows, selection memory - plus the
// show/hide navigation between them. The screens themselves live in the sibling partials.
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

        StripBorrowedEventTriggers(ms.gameObject);

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
        // Snapshotted before anything is destroyed. Transform's enumerator walks by index and
        // DestroyImmediate removes the child there and then, so destroying inside the loop shifts
        // every later child down one and skips it. The screens cloned from the pause menu happen to
        // survive that; a screen with more children than that would not.
        var templateChildren = new List<Transform>();
        foreach (Transform child in ms.transform)
            templateChildren.Add(child);

        foreach (var child in templateChildren)
        {
            if (child == null)
                continue;
            // By ancestry, not identity: on some screens the back button sits inside a container,
            // and destroying the container takes the screen's only way out with it.
            if (ms.backButton != null && ms.backButton.transform.IsChildOf(child))
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

        // Insets are a share of the screen, not fixed pixels: StretchScreenOverCanvas sizes this rect
        // to the whole display, so fixed margins against it leave lists hard against the top edge and
        // rows running the full width. The game's own option screens sit in a narrow centred column
        // under a clear band, which these match - about a fifth of the height above, two thirds of
        // the width across.
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
        // Skipped by StripTemplateComponents so that it survives at all, which also means it keeps
        // the game's own way out of the screen it was borrowed from.
        StripBorrowedEventTriggers(ms.backButton.gameObject);
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
