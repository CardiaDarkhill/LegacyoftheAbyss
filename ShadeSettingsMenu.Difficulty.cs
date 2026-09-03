#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using GlobalEnums;
using LegacyoftheAbyss.Shade;

// The Difficulty screen and the settings it steps through.
public static partial class ShadeSettingsMenu
{
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
        var footerDriver = CreateDescriptionFooter(footer, TextAnchor.UpperLeft, descriptions);
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
        PersistDifficultyChange();
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
        PersistDifficultyChange();
    }

    /// <summary>
    /// Resizes the Shade standing in the scene right now rather than waiting for it to respawn.
    /// Nothing is saved here - the menu writes the config out when it closes.
    /// </summary>
    private static void ApplyShadeMaskFractionToLiveShade()
    {
        try
        {
            foreach (var shade in LegacyHelper.ShadeController.ActiveInstances)
            {
                if (shade != null)
                    shade.RefreshDerivedMaskCount();
            }
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not apply the Shade mask setting to the live Shades: {e}");
        }
    }

    private static bool GetShadeAssistMode()
    {
        try
        {
            // Assist mode is one setting shared by every Shade, so the primary answers for all.
            var shade = LegacyHelper.ShadeController.PrimaryInstance;
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
            var shades = LegacyHelper.ShadeController.ActiveInstances;
            if (shades.Count > 0)
            {
                foreach (var shade in shades)
                {
                    if (shade != null)
                        shade.SetAssistMode(enabled);
                }
            }
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
        PersistDifficultyChange();
    }

    /// <summary>
    /// Puts a difficulty change where it belongs: <c>config.json</c>, which is the default for a
    /// save that has no difficulty of its own, and the save slot in play, which is where difficulty
    /// actually lives now.
    /// <para>
    /// Called from every write on this screen rather than from <c>DifficultyMenuController</c>,
    /// which the new-game screen shares - that screen holds its choices locally until Begin, and a
    /// refresh there must not push the current values onto whichever slot happens to be active.
    /// </para>
    /// </summary>
    private static void PersistDifficultyChange()
    {
        try
        {
            ModConfig.Save();
            ShadeRuntime.PersistDifficultyToActiveSlot();
        }
        catch (Exception e)
        {
            LogMenuWarning($"Difficulty change could not be saved: {e}");
        }
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
}
#nullable restore
