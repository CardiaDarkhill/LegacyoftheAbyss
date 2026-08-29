#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

// The Controls screen: the binding rows and the navigation wiring between their columns.
public static partial class ShadeSettingsMenu
{
    private static void BuildControlsMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        // This screen positions info/presetRow/scrollWrapper explicitly rather than through
        // content's VerticalLayoutGroup, which does not reliably size this screen's direct children:
        // presetRow lands at height 0 despite a correct LayoutElement, and scrollWrapper never grows
        // past RectTransform's 100x100 default, even after ForceRebuildLayoutImmediate. The group
        // itself stays attached - every other screen shares it through CreateContentRoot - but is
        // disabled here so it cannot fight the explicit values.
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
            // No upward nudge: the margins are proportional, so one only adds dead space at the bottom.
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
        // Keep the max above the min: an inverted Mathf.Clamp returns the max for every width, and a
        // negative one hangs the preset row past both edges of the content it sits in.
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
        // Real height is computed below, once every card's wrapped-text height is known - see
        // AddPresetOption for why Unity's own layout computation cannot be left to do it. Must stay
        // at zero flexible height as well: content's VerticalLayoutGroup splits leftover height
        // between every child claiming a share, so a share here leaves the binding scroll view below
        // showing about two rows instead of filling the space.
        presetLayoutElement.flexibleHeight = 0f;
        var presetCardHeights = new List<float>();

        // One button per preset, with its description in the shared footer for whichever row is
        // highlighted - the way every other screen here explains itself. Keep it there rather than
        // under each card: an inline description has to be measured at an assumed width and stamped
        // as an explicit height, and wrapped lines pile up as soon as the cards are wider than the
        // guess.

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
            (ShadeAction.Sprint, "Sprint / Dash"),
            (ShadeAction.Jump, "Jump (Knight)"),
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

        // The settled rect of every band on this screen, for when it looks wrong and a screenshot
        // cannot say by how much. Behind the Menu Logs setting: this builds seconds after launch, so
        // left on it would push the whole line out of the report's log ring before anyone reads it.
        // Goes through this file's ManualLogSource rather than Debug.Log, which some BepInEx installs
        // never capture ("Unable to start Unity log writer" in LogOutput.log).
        if (ModConfig.Instance.logMenu)
        {
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
}
#nullable restore
