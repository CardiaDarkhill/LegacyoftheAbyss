#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalEnums;
using LegacyoftheAbyss.Shade;
using Object = UnityEngine.Object;

/// <summary>
/// The screen shown when a new game is started: what to do with the shade progress already on that
/// save slot, which difficulty to play at, and which character to play as.
/// <para>
/// Starting a new game used to do nothing at all for this mod - not even clear the previous run's
/// charms off the slot - so a fresh file began with the last one's loadout and whatever difficulty
/// the settings menu happened to be left on. The three questions are asked once, at the only moment
/// the answers are actually a decision rather than a setting.
/// </para>
/// <para>
/// A sibling of the pause settings screens rather than its own thing: it is the same toolkit -
/// <see cref="CreateMenuButton"/>, <see cref="LabeledStepperDriver"/>, the description footer - and
/// splitting it out would mean either duplicating that or widening it. It keeps its own screen and
/// its own lifecycle though, and deliberately does not touch <c>mainScreen</c>, <c>allScreens</c> or
/// <c>built</c>: the pause menu's build hides the pause menu behind it, which is not something to be
/// doing in the middle of the title screen.
/// </para>
/// </summary>
public static partial class ShadeSettingsMenu
{
    private static MenuScreen newGameScreen;
    private static DifficultyMenuController newGameController;
    private static UIManager newGameBuiltFor;

    /// <summary>The save slot the questions are being asked about, from <c>GameManager.profileID</c>.</summary>
    private static int newGameSlot;

    /// <summary>
    /// Set while re-entering <c>UIManager.StartNewGame</c> after the player has answered, so the
    /// patch lets that one call through instead of showing the screen again.
    /// </summary>
    private static bool newGameAnswered;

    /// <summary>
    /// The flags <c>StartNewGame</c> was called with, held so Begin can hand them straight back.
    /// Silksong carries its play mode in these, and losing them would silently start a normal run
    /// where a Steel Soul one was asked for.
    /// </summary>
    private static bool newGamePermaDeath;
    private static bool newGameBossRush;

    private static bool newGameResetProgress = true;
    private static int newGameDifficultyIndex;
    private static ShadeCharacterId newGameCharacter = ShadeCharacterId.Shade;

    /// <summary>Whether the slot being started over holds anything the reset question could erase.</summary>
    private static bool newGameSlotHasProgress;

    /// <summary>
    /// Which screen the player came from, so backing out returns them to it.
    /// <para>
    /// There are two routes in, and the game chooses between them in <c>SaveSlotButton</c>: an empty
    /// slot goes straight to <c>StartNewGame</c> normally, but once the player has finished a Steel
    /// Soul or boss rush run it stops at Mode Select first and starts the game from there. This
    /// screen sits after both, which is the right place for it - the play mode is Silksong's
    /// question and this is ours - but "back" means a different screen in each case.
    /// </para>
    /// </summary>
    private static MainMenuState newGameEnteredFrom = MainMenuState.SAVE_PROFILES;

    private const string ResetHelp =
        "Would you like to reset the charm and charm notch progress for this save slot?";

    private const string DifficultyHelp =
        "Easy is vanilla Silksong, Normal should be about as hard as the base game, Hard will significantly test you. Abyss is entirely unfair.";

    private const string CharacterHelp =
        "Would you like to play as the Shade or the Knight? (Special thanks to Shownyoung for their work on the Knight in Silksong mod and their help with this feature!) WARNING: The Knight may struggle with some platforming sections.";

    private const string AssignDevicesHelp =
        "Say which device each player is holding by pressing a button on it. Two players cannot share one device, and either may use the keyboard.";

    /// <summary>
    /// Called by the <c>UIManager.StartNewGame</c> patch. Returns true when the questions still
    /// need asking, in which case the original call is skipped and picked up again on Begin.
    /// </summary>
    internal static bool InterceptNewGame(UIManager ui, bool permaDeath, bool bossRush)
    {
        if (ui == null || !ModConfig.Instance.shadeNewGameOptionsEnabled)
        {
            return false;
        }

        // The answered pass. Cleared here rather than on Begin so that an abandoned attempt - a
        // player who backs out of the brightness prompt StartNewGame can divert to - asks again.
        if (newGameAnswered)
        {
            newGameAnswered = false;
            return false;
        }

        // The game diverts to the overscan and brightness prompts on a first run and calls
        // StartNewGame again afterwards. Those passes are not the moment to ask anything, so this
        // waits for the one that is actually going to start the game.
        if (ui.gs == null || ui.gs.overscanAdjusted != 1 || ui.gs.brightnessAdjusted != 1)
        {
            return false;
        }

        try
        {
            newGamePermaDeath = permaDeath;
            newGameBossRush = bossRush;
            newGameEnteredFrom = ui.menuState;
            newGameSlot = ResolveNewGameSlot();
            newGameSlotHasProgress = ShadeRuntime.SlotHasShadeProgress(newGameSlot);
            newGameResetProgress = newGameSlotHasProgress;
            newGameDifficultyIndex = DefaultDifficultyIndex();
            newGameCharacter = ShadeCharacterId.Shade;

            if (!BuildNewGameScreen(ui))
            {
                return false;
            }

            ui.StartCoroutine(ShowNewGameScreen(ui));
            return true;
        }
        catch (Exception e)
        {
            // A menu that throws here would strand the player on a faded-out save slot screen with
            // no way forward, so a failure hands the new game straight back to the game.
            LogMenuWarning($"New game options could not be shown; starting the game as normal: {e}");
            return false;
        }
    }

    /// <summary>
    /// Which slot the new game is being started on. <c>GameManager.profileID</c> is set by the save
    /// slot button before <c>StartNewGame</c> runs, so it is the answer here even though no save
    /// file exists yet.
    /// </summary>
    private static int ResolveNewGameSlot()
    {
        try
        {
            // Asked of the runtime rather than worked out here: the game numbers its profiles from
            // one and the shade's slots index from zero, and a second copy of that conversion is a
            // way to erase the neighbouring save file's charms.
            return ShadeRuntime.ResolveSlotIndex(GameManager.instance);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>Normal, or whichever preset is nearest to it if the presets are ever reordered.</summary>
    private static int DefaultDifficultyIndex()
    {
        var all = DifficultyPreset.All;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].Name == DifficultyPreset.Normal)
            {
                return i;
            }
        }

        return 0;
    }

    private static IEnumerator ShowNewGameScreen(UIManager ui)
    {
        if (ui.ih != null)
        {
            ui.ih.StopUIInput();
        }

        // The save profile screen is not one of the states HideCurrentMenu knows how to hide, so
        // it needs its own call; Mode Select is, and would otherwise still be standing behind this.
        if (ui.menuState == MainMenuState.SAVE_PROFILES)
        {
            yield return ui.StartCoroutine(ui.HideSaveProfileMenu(true));
        }
        else
        {
            yield return ui.StartCoroutine(ui.HideCurrentMenu());
        }

        // The menu state is deliberately left alone. It is an enum this mod cannot add to, and
        // every value in it names a screen the game would then try to hide on its own behalf.
        newGameController?.RefreshAll();
        yield return ui.StartCoroutine(ui.ShowMenu(newGameScreen));

        if (newGameScreen != null)
        {
            newGameScreen.HighlightDefault();
        }

        if (ui.ih != null)
        {
            ui.ih.StartUIInput();
        }
    }

    private static IEnumerator BeginNewGame(UIManager ui)
    {
        ApplyNewGameChoices();

        if (newGameScreen != null)
        {
            yield return ui.StartCoroutine(ui.HideMenu(newGameScreen));
        }

        newGameAnswered = true;
        ui.StartNewGame(newGamePermaDeath, newGameBossRush);
    }

    private static IEnumerator CancelNewGame(UIManager ui)
    {
        if (newGameScreen != null)
        {
            yield return ui.StartCoroutine(ui.HideMenu(newGameScreen));
        }

        if (newGameEnteredFrom == MainMenuState.PLAY_MODE_MENU)
        {
            ui.UIGoToPlayModeMenu();
            yield break;
        }

        ui.UIGoBackToSaveProfiles();
    }

    /// <summary>
    /// Writes the three answers down. Nothing here happens while the player is stepping through the
    /// rows: backing out of this screen has to leave the settings exactly as it found them.
    /// </summary>
    private static void ApplyNewGameChoices()
    {
        try
        {
            // The reset first, so nothing below is written into a slot that is about to be wiped.
            if (newGameResetProgress && newGameSlotHasProgress)
            {
                ShadeRuntime.ResetSlotProgress(newGameSlot);
            }

            var preset = DifficultyPreset.All[Mathf.Clamp(newGameDifficultyIndex, 0, DifficultyPreset.All.Length - 1)];
            preset.ApplyTo(ModConfig.Instance);
            ModConfig.Save();

            // Difficulty belongs to the save slot, so it is written to the one being started rather
            // than only to the live config - which the slot would otherwise overwrite the moment it
            // loads and finds a difficulty of its own.
            ShadeRuntime.SetSlotDifficulty(newGameSlot, ShadeDifficultySettings.CaptureFrom(ModConfig.Instance));

            // The primary companion's, which is the one a new game starts with.
            ShadeCharacterManager.Select(0, newGameCharacter);

            LogMenuInfo($"New game on slot {newGameSlot}: difficulty={preset.Name} character={newGameCharacter} reset={newGameResetProgress && newGameSlotHasProgress}");
        }
        catch (Exception e)
        {
            LogMenuWarning($"New game choices could not all be applied: {e}");
        }
    }

    /// <summary>
    /// Builds the screen, once per <see cref="UIManager"/>. Rebuilt when that changes because the
    /// menu scene is torn down and remade between the title screen and the game.
    /// </summary>
    private static bool BuildNewGameScreen(UIManager ui)
    {
        if (newGameScreen != null && newGameBuiltFor == ui)
        {
            // The reset row appears only for a slot that has something to reset, and which slot is
            // being started on changes between attempts.
            return RebuildIfSlotShapeChanged(ui);
        }

        DestroyNewGameScreen();

        // The Options screen, which is what every other screen in this menu is built from and the
        // only one proven to come out of CreateContentRoot clean.
        //
        // Not the Mode Select screen, despite it being the closest thing the game has to this one.
        // Its rows are composites - a title, a description and a glow - so a row cloned from one
        // arrived carrying "Normal play mode." under whichever label it was given, and the screen
        // itself kept enough of its own furniture that "Mode Select", "Classic" and "Steel Soul"
        // drew straight through ours. It is also only reachable once the game has been beaten (see
        // SaveSlotButton), so it is the wrong thing to depend on twice over.
        var template = ui.optionsMenuScreen != null ? ui.optionsMenuScreen
            : (ui.gameOptionsMenuScreen != null ? ui.gameOptionsMenuScreen : ui.playModeMenuScreen);
        if (template == null)
        {
            LogMenuWarning("No main-menu screen to clone the new game options from; starting the game as normal.");
            return false;
        }

        newGameScreen = Object.Instantiate(template.gameObject, template.transform.parent).GetComponent<MenuScreen>();
        if (newGameScreen == null)
        {
            return false;
        }

        newGameScreen.name = "ShadeNewGameOptions";
        newGameBuiltFor = ui;
        InitializeScreen(newGameScreen);
        newGameScreen.gameObject.SetActive(false);

        // Clear whatever the template screen brought with it - the play mode screen arrives with its
        // own Steel Soul buttons, and the options screen with its category list.
        var content = CreateContentRoot(newGameScreen);
        if (content == null)
        {
            DestroyNewGameScreen();
            return false;
        }

        var buttonTemplate = CreateNewGameButtonTemplate(ui, template);
        if (buttonTemplate == null)
        {
            DestroyNewGameScreen();
            return false;
        }

        // Logged unconditionally, as the slider template is, and for the same reason: which
        // screen and which button this was cloned from is the whole difference between a screen
        // that reads correctly and one wearing Mode Select's furniture, and it is not something a
        // screenshot can tell you.
        log.LogInfo($"New game screen: cloned '{template.name}', rows from '{buttonTemplate.name}', "
            + $"slot {newGameSlot} {(newGameSlotHasProgress ? "has" : "has no")} progress");

        ApplyPreferredFont(FindFontInObject(buttonTemplate.gameObject));
        PopulateNewGameScreen(content, buttonTemplate);
        ConfigureNewGameBackButton();

        Object.Destroy(buttonTemplate.gameObject);
        newGameBuiltSlotHadProgress = newGameSlotHasProgress;
        return true;
    }

    /// <summary>What <see cref="newGameSlotHasProgress"/> was when the rows were laid out.</summary>
    private static bool newGameBuiltSlotHadProgress;

    private static bool RebuildIfSlotShapeChanged(UIManager ui)
    {
        if (newGameBuiltSlotHadProgress == newGameSlotHasProgress)
        {
            newGameController?.RefreshAll();
            return true;
        }

        DestroyNewGameScreen();
        return BuildNewGameScreen(ui);
    }

    private static void DestroyNewGameScreen()
    {
        if (newGameScreen != null)
        {
            Object.Destroy(newGameScreen.gameObject);
        }

        newGameScreen = null;
        newGameController = null;
        newGameBuiltFor = null;
    }

    /// <summary>
    /// A menu button to clone the rows from - the Options screen's, which is what the pause settings
    /// screens use and the shape this screen's rows actually are: one label, one line.
    /// <para>
    /// Deliberately not the Mode Select screen's, even when that is what is being cloned. Those rows
    /// carry a title <em>and</em> a description, and <see cref="CreateMenuButton"/> writes the label
    /// into the first Text it finds - so every row came out reading "Normal play mode." under a
    /// label that had gone somewhere else.
    /// </para>
    /// </summary>
    private static MenuButton CreateNewGameButtonTemplate(UIManager ui, MenuScreen template)
    {
        foreach (var source in new[] { ui.optionsMenuScreen, ui.gameOptionsMenuScreen, template })
        {
            if (source == null)
            {
                continue;
            }

            foreach (var candidate in source.GetComponentsInChildren<MenuButton>(true))
            {
                if (candidate == null || (source.backButton != null && candidate == source.backButton))
                {
                    continue;
                }

                var clone = Object.Instantiate(candidate.gameObject).GetComponent<MenuButton>();
                if (clone != null)
                {
                    clone.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    clone.gameObject.SetActive(false);
                    return clone;
                }
            }
        }

        var fallback = CreateDefaultMenuButtonTemplate();
        if (fallback != null)
        {
            fallback.gameObject.hideFlags = HideFlags.HideAndDontSave;
            fallback.gameObject.SetActive(false);
        }

        return fallback;
    }

    private static void PopulateNewGameScreen(RectTransform content, MenuButton buttonTemplate)
    {
        var contentLayout = content.GetComponent<VerticalLayoutGroup>();
        if (contentLayout != null)
        {
            contentLayout.enabled = false;
        }

        newGameController = newGameScreen.gameObject.GetComponent<DifficultyMenuController>()
            ?? newGameScreen.gameObject.AddComponent<DifficultyMenuController>();

        var msRect = (RectTransform)newGameScreen.transform;
        float canvasWidth = msRect.rect.width;
        float canvasHeight = msRect.rect.height;

        float horizontalMargin = Mathf.Max(48f, canvasWidth * 0.18f);
        content.offsetMin = new Vector2(horizontalMargin, canvasHeight * ListBottomMarginFraction);
        content.offsetMax = new Vector2(-horizontalMargin, -(canvasHeight * ListTopMarginFraction));

        var selectables = new List<MenuSelectable>();
        var descriptions = new List<KeyValuePair<MenuSelectable, string>>();
        const float RowSpacing = 24f;
        float cursorY = 0f;

        CreateNewGameLabel(content, "Legacy of the Abyss", cursorY, 72f, TextAnchor.MiddleCenter, Color.white, sliderLabelStyle);
        cursorY += 72f;

        var subtitle = CreateNewGameLabel(content,
            "Difficulty and Character selection may be changed later from the pause menu",
            cursorY, 52f, TextAnchor.UpperCenter, DescriptionColor, toggleLabelStyle);
        if (subtitle != null)
        {
            subtitle.fontSize = Mathf.Max(12, Mathf.RoundToInt(subtitle.fontSize * 0.78f));
        }

        cursorY += 52f + RowSpacing;

        MenuSelectable AddRow(string label, string help, Func<string> value, Action<int> step)
        {
            var selectable = CreateMenuButton(content, buttonTemplate, label, null, CancelTarget.ShadeNewGame);
            if (selectable is not MenuButton button)
            {
                return selectable;
            }

            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -cursorY);
            rect.sizeDelta = new Vector2(0f, ButtonRowHeight);
            cursorY += ButtonRowHeight + RowSpacing;

            var driver = button.gameObject.AddComponent<LabeledStepperDriver>();
            driver.Initialize(button, label, value, step);
            newGameController.RegisterStepper(driver);

            selectables.Add(button);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(button, help));
            return button;
        }

        // Only offered for a slot that has something to erase, per the roadmap - there is no sense
        // asking whether to clear progress that is not there.
        if (newGameSlotHasProgress)
        {
            AddRow("Reset Shade Progress", ResetHelp,
                () => newGameResetProgress ? "Yes" : "No",
                _ => newGameResetProgress = !newGameResetProgress);
        }

        AddRow("Difficulty", DifficultyHelp,
            () => DifficultyPreset.All[Mathf.Clamp(newGameDifficultyIndex, 0, DifficultyPreset.All.Length - 1)].Name,
            direction => newGameDifficultyIndex = StepIndex(newGameDifficultyIndex, direction, DifficultyPreset.All.Length));

        AddRow("Character", CharacterHelp,
            () => newGameCharacter == ShadeCharacterId.Knight ? "Knight" : "Shade",
            _ => newGameCharacter = newGameCharacter == ShadeCharacterId.Knight ? ShadeCharacterId.Shade : ShadeCharacterId.Knight);

        // The Controls screen's row, offered here as well: which player is holding what is a
        // question worth answering before the first room rather than after it, and a second player
        // who cannot move is not obviously a device problem from inside the game.
        var assign = CreateMenuButton(content, buttonTemplate, "Assign Devices", null, CancelTarget.ShadeNewGame);
        if (assign is MenuButton assignButton)
        {
            var assignRect = assignButton.GetComponent<RectTransform>();
            assignRect.anchorMin = new Vector2(0f, 1f);
            assignRect.anchorMax = new Vector2(1f, 1f);
            assignRect.pivot = new Vector2(0.5f, 1f);
            assignRect.anchoredPosition = new Vector2(0f, -cursorY);
            assignRect.sizeDelta = new Vector2(0f, ButtonRowHeight);
            cursorY += ButtonRowHeight + RowSpacing;

            assignButton.gameObject.AddComponent<ControllerAssignmentDriver>().Initialize(assignButton);
            selectables.Add(assignButton);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(assignButton, AssignDevicesHelp));
        }

        cursorY += RowSpacing;

        var begin = CreateMenuButton(content, buttonTemplate, "Begin", null, CancelTarget.ShadeNewGame);
        if (begin is MenuButton beginButton)
        {
            var rect = beginButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -cursorY);
            rect.sizeDelta = new Vector2(0f, ButtonRowHeight);
            cursorY += ButtonRowHeight + RowSpacing;

            beginButton.OnSubmitPressed.RemoveAllListeners();
            beginButton.OnSubmitPressed.AddListener(() =>
            {
                var manager = newGameBuiltFor ?? UIManager.instance;
                if (manager != null)
                {
                    manager.StartCoroutine(BeginNewGame(manager));
                }
            });

            selectables.Add(beginButton);
            descriptions.Add(new KeyValuePair<MenuSelectable, string>(beginButton,
                "Start the game with these settings. Difficulty and character can still be changed later from the pause menu."));
        }

        // --- description footer, exactly as the Difficulty screen's -----------------------------
        var footer = new GameObject("Description");
        var footerRect = footer.AddComponent<RectTransform>();
        footerRect.SetParent(content, false);
        footerRect.anchorMin = new Vector2(0f, 0f);
        footerRect.anchorMax = new Vector2(1f, 0f);
        footerRect.pivot = new Vector2(0.5f, 0f);
        footerRect.anchoredPosition = Vector2.zero;
        footerRect.sizeDelta = new Vector2(0f, DescriptionRowHeight * 1.6f);
        CreateDescriptionFooter(footer, TextAnchor.UpperCenter, descriptions);

        SetupButtonList(newGameScreen, selectables);

        MenuSelectable first = selectables.Count > 0 ? selectables[0] : null;
        if (first != null)
        {
            screenFirstSelectables[newGameScreen] = first;
            newGameScreen.defaultHighlight = first;
        }

    }

    /// <summary>
    /// Sends the template's own back button back to the save slots, and puts a cancel router on the
    /// screen so Escape does the same from anywhere on it. Left where the template placed it rather
    /// than run through <see cref="ConfigureBackButton"/>, which lays a back button out as the last
    /// row of a list screen's column - this screen positions its rows itself.
    /// </summary>
    private static void ConfigureNewGameBackButton()
    {
        var router = newGameScreen.gameObject.GetComponent<CancelRouter>() ?? newGameScreen.gameObject.AddComponent<CancelRouter>();
        router.target = CancelTarget.ShadeNewGame;

        var back = newGameScreen.backButton;
        if (back == null)
        {
            return;
        }

        back.OnSubmitPressed.RemoveAllListeners();
        back.cancelAction = CancelAction.DoNothing;
        StripBorrowedEventTriggers(back.gameObject);
        back.OnSubmitPressed.AddListener(() =>
        {
            var manager = newGameBuiltFor ?? UIManager.instance;
            if (manager != null)
            {
                manager.StartCoroutine(CancelNewGame(manager));
            }
        });

        foreach (var condition in back.GetComponents<MenuButtonListCondition>())
        {
            Object.DestroyImmediate(condition);
        }

        var backRouter = back.gameObject.GetComponent<CancelRouter>() ?? back.gameObject.AddComponent<CancelRouter>();
        backRouter.target = CancelTarget.ShadeNewGame;
    }

    private static int StepIndex(int current, int direction, int count)
    {
        if (count <= 0)
        {
            return 0;
        }

        int step = direction >= 0 ? 1 : -1;
        return ((current + step) % count + count) % count;
    }

    private static Text CreateNewGameLabel(RectTransform parent, string label, float cursorY, float height, TextAnchor alignment, Color color, UiTextStyle? style)
    {
        var go = new GameObject("Heading");
        var rect = go.AddComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -cursorY);
        rect.sizeDelta = new Vector2(0f, height);

        var text = go.AddComponent<Text>();
        ApplyTextStyle(text, style, alignment, color);
        text.text = label;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }
}
#nullable restore
