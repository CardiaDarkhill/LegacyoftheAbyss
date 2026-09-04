#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LegacyoftheAbyss.Shade;

// The individual option screens that are short enough to sit together: the root menu, the
// Charms screen, Logging, and the two Shade AI screens. Difficulty and Controls are large
// enough to have files of their own.
public static partial class ShadeSettingsMenu
{
    private static void BuildMainMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null)
            return;
        var content = CreateContentRoot(ms);
        if (content == null)
            return;
        var selectables = new List<MenuSelectable>();
        // "Shade Enabled" lives in Debug Options rather than here: with the AI in, turning the Shade
        // off outright is a testing tool, not a front-page choice for someone who installed this mod.
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
            var s = CreateMenuButton(content, buttonTemplate, "Characters", () => ShowScreen(skinsScreen), CancelTarget.PauseMenu);
            if (s != null) selectables.Add(s);
        }
        // A toggle among the navigation rows rather than a screen of its own: it is one switch, and
        // it belongs next to Characters because it only does anything with a second player out.
        {
            var s = CreateToggle(content, buttonTemplate, "Co-op Camera", ModConfig.Instance.companionCameraBiasEnabled,
                v =>
                {
                    ModConfig.Instance.companionCameraBiasEnabled = v;
                    if (!v)
                        LegacyHelper.CompanionCameraBias.Reset();
                }, CancelTarget.PauseMenu);
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
        SetScreenFirstSelectable(ms, selectables);
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
        SetScreenFirstSelectable(ms, selectables);

        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        charmsController?.Initialize(notchText, statusText, detailTitleText, detailDescriptionText, navigationText, equipButton, unequipButton);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
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
        // A surveying tool: with this on the map shows every pickup rather than only the ones still
        // out there, which is what placing a new charm well actually needs to see.
        AddToggle("Map Shows Collected Pickups", ModConfig.Instance.debugShowCollectedPickupsOnMap,
            v => ModConfig.Instance.debugShowCollectedPickupsOnMap = v);
        SetupButtonList(ms, selectables);
        SetScreenFirstSelectable(ms, selectables);
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

        var footerLayout = footer.AddComponent<LayoutElement>();
        footerLayout.minHeight = DescriptionRowHeight;
        footerLayout.preferredHeight = DescriptionRowHeight;
        footerLayout.flexibleHeight = 0f;

        // Appearance and wiring belong to the overload; only where the box goes is this screen's
        // business. Rows are registered by the caller once they exist.
        return CreateDescriptionFooter(footer, TextAnchor.UpperLeft, descriptions: null);
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
        SetScreenFirstSelectable(ms, selectables);

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

        // Re-asked on every show rather than captured here: the character can change on another
        // screen, and this row is built once for the session.
        static bool AiUnavailableForCharacter() => ShadeCharacterManager
            .GetSelected(ShadeCompanionRegistry.PrimaryId).Id == ShadeCharacterId.Knight;

        Add(CreateToggle(content, buttonTemplate, "Shade AI", ModConfig.Instance.shadeAiEnabled, v =>
            {
                ModConfig.Instance.shadeAiEnabled = v;
                // Apply to the Shade standing in the scene right now rather than waiting for a
                // respawn. persist:false because this menu owns the value and saves it on close.
                foreach (var shade in LegacyHelper.ShadeController.ActiveInstances)
                {
                    if (shade != null)
                        shade.SetShadeAiEnabled(v, persist: false);
                }
            }, CancelTarget.ShadeMain, AiUnavailableForCharacter),
            "Let the Shade fight by itself. It picks targets, attacks, steps out of danger and heals you both. It can be killed, so you will need to revive it. Unavailable while the Knight is equipped.");

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
}
#nullable restore
