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
    private static GameObject screen;
    private static bool built;
    private static UIManager builtFor;
    // Tracks the debugKeysEnabled value the Controls menu was last built with, so toggling
    // it in Debug Options forces a rebuild (adding/removing the debug binding rows) the next
    // time the settings menu is opened, rather than requiring built to be reset by hand.
    private static bool lastBuiltDebugKeysEnabled;
    // Set once the settings button has been injected into a given UIManager's pause menu.
    // Guards Inject's hierarchy scan, which would otherwise re-walk the whole pause menu every frame.
    private static UIManager injectedFor;
    private static MenuScreen mainScreen;
    private static MenuScreen difficultyScreen;
    private static MenuScreen controlsScreen;
    private static MenuScreen loggingScreen;
    private static MenuScreen shadeAiScreen;
    private static MenuScreen shadeAiAdvancedScreen;

    /// <summary>Muted colour for the explanation line under each options screen.</summary>
    private static readonly Color DescriptionColor = new Color(0.84f, 0.82f, 0.74f, 0.92f);

    /// <summary>Height reserved for that line. Two rows of wrapped text at the reduced size.</summary>
    private const float DescriptionRowHeight = 92f;
    private static MenuScreen charmsScreen;
    private static MenuScreen skinsScreen;
    private static MenuScreen activeScreen;
    private static readonly List<MenuScreen> allScreens = new();
    private static readonly Dictionary<MenuScreen, MenuSelectable> screenFirstSelectables = new();

    /// <summary>
    /// Where the player was on each screen the last time they left it, so backing out of a sub-menu
    /// returns the highlight to the row that opened it instead of snapping to the top of the list.
    /// See <c>ShowScreen</c> for why this has to exist rather than leaning on the screen's own
    /// <c>MenuButtonList</c>.
    /// </summary>
    private static readonly Dictionary<MenuScreen, MenuSelectable> screenLastSelectables = new();
    private static GameObject templateSource;
    private static bool templateSourceWasActive;
    private static bool pauseMenuWasActive;
    private static bool optionsMenuWasActive;
    private static bool gameOptionsMenuWasActive;
    private static bool storedGameOptionsCanvasState;
    private static float storedGameOptionsAlpha;
    private static bool storedGameOptionsInteractable;
    private static bool storedGameOptionsBlocksRaycasts;
    private static readonly ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("ShadeSettingsMenu");
    private static bool loggedBuildAttempt;
    private static bool loggedMissingOptionsMenu;
    private static bool loggedNullUI;
    private static bool loggedNoPauseMenu;
    private static bool loggedButtonAlreadyPresent;
    private static bool loggedNoPauseButtonTemplates;
    private static bool loggedNoMenuButtonList;
    private static bool loggedNullEntries;
    private const float FractionalSliderStep = 0.1f;
    private const float SliderRowHeight = 96f;
    private const float ToggleRowHeight = 84f;
    private const float ButtonRowHeight = 88f;
    private const float ContentSpacing = 64f;
    private const float LabelColumnWidth = 420f;
    private const float ValueColumnWidth = 140f;
    private const float MenuFontScale = 1.5f;

    /// <summary>Share of the screen width one of the plain list screens uses for its column.</summary>
    private const float ListColumnWidthFraction = 0.66f;

    /// <summary>Band left clear above a list screen's first row, as a share of the screen height.</summary>
    private const float ListTopMarginFraction = 0.19f;

    /// <summary>The matching band below its last row.</summary>
    private const float ListBottomMarginFraction = 0.12f;

    /// <summary>
    /// How tall a shoulder-button prompt is drawn, as a share of the panel heading's height.
    /// </summary>
    private const float PanePromptHeightFraction = 1f;

    /// <summary>
    /// How large these screens are drawn relative to the canvas, measured by
    /// <c>StretchScreenOverCanvas</c> when each one is set up. The pause menu this menu is cloned
    /// from sits in a part of the hierarchy that is scaled down - about two thirds on the install
    /// this was measured on - so a local unit here is not a local unit on the game's own option
    /// screens.
    /// </summary>
    private static float screenCanvasScale = 1f;

    /// <summary>The same figure for the game's slider, so the two can be reconciled.</summary>
    private static float gameSliderCanvasScale = 1f;

    /// <summary>Whether the slider rows are the game's own, and so want that reconciling.</summary>
    private static bool sliderTemplateIsGameClone;
    private static readonly Color ButtonNormalColor = new Color(1f, 1f, 1f, 0f);
    private static readonly Color ButtonHighlightColor = new Color(1f, 0.95f, 0.78f, 0.35f);
    private static readonly Color ButtonPressedColor = new Color(0.95f, 0.9f, 0.8f, 0.45f);
    private static readonly Color ButtonDisabledColor = new Color(1f, 1f, 1f, 0.15f);
    private static readonly bool IncludeLegacyCharmMenu = false;
    private static bool consumeNextToggle;

    /// <summary>
    /// The frame this menu last acted on a press of Back.
    /// <para>
    /// One press of Escape reaches these screens twice: once as the EventSystem's Cancel, delivered
    /// to whichever row is selected, and once as the game's own pause toggle. Either can arrive
    /// first, and whichever does takes the step back - so the other then finds itself on the screen
    /// the first one just opened and steps back again from there, which walks out of the pause menu
    /// from anywhere in two levels of this menu in a single keypress.
    /// </para>
    /// <para>
    /// So a back press is claimed rather than handled twice. The first arrival claims the frame and
    /// acts; the second finds it taken and does nothing at all.
    /// </para>
    /// </summary>
    private static int backNavigationFrame = -1;

    /// <summary>
    /// How many rows are waiting for the player to press the control they want bound. While any is,
    /// Escape belongs to that row - it is how the prompt says to cancel - and neither the pause
    /// toggle nor a Cancel may act on it.
    /// </summary>
    private static int captureDepth;

    /// <summary>Whether a row is asking for a keypress, and so owns Escape.</summary>
    internal static bool IsCapturingBinding => captureDepth > 0;

    /// <summary>
    /// Gives up a capture. Clamped at zero and re-zeroed whenever the screens go away, because a
    /// row destroyed mid-prompt - a rebuild, or the screen closing - takes its coroutine with it
    /// and never reaches the release. A count stuck above zero would leave Escape doing nothing at
    /// all for the rest of the session.
    /// </summary>
    private static void ReleaseCapture()
    {
        captureDepth = captureDepth > 0 ? captureDepth - 1 : 0;
    }

    /// <summary>
    /// Takes this frame's back press, or reports that something else already has. Also claimed - and
    /// never released - by a capture that cancels itself on Escape, so the same press cannot then
    /// close the screen the row lives on.
    /// </summary>
    private static bool ClaimBackNavigation()
    {
        int frame = Time.frameCount;
        if (backNavigationFrame == frame)
        {
            return false;
        }

        backNavigationFrame = frame;
        return true;
    }
    private static readonly List<BindingMenuDriver> bindingDrivers = new();
    private static ShadeToggleDriver shadeToggleDriver;

    /// <summary>
    /// Which row the sliders were cloned from the last time the menu was built, or why none was
    /// found. Read by the bug reporter - see BugReportState.MenuSliderTemplate.
    /// </summary>
    internal static string LastSliderTemplateDescription { get; private set; } = "not built yet";

    private static string GetShadeToggleLabel() => $"Shade Enabled: {(ModConfig.Instance.shadeEnabled ? "On" : "Off")}";

    private static UiTextStyle? sliderLabelStyle;
    private static UiTextStyle? sliderValueStyle;
    private static UiTextStyle? toggleLabelStyle;
    private static Font fallbackFont;
    private static Sprite fallbackSlicedSprite;
    private static Sprite fallbackKnobSprite;
    private static Sprite fallbackCheckSprite;
    private static Sprite fallbackCharmSprite;
    private static CharmMenuController charmsController;
    private static SkinMenuController skinsController;
    private static DifficultyMenuController difficultyController;

    private static void LogMenu(LogLevel level, string message)
    {
        if (!ModConfig.Instance.logMenu)
            return;
        log.Log(level, message);
    }

    private static void LogMenuDebug(string message) => LogMenu(LogLevel.Debug, message);
    private static void LogMenuInfo(string message) => LogMenu(LogLevel.Info, message);
    private static void LogMenuWarning(string message) => LogMenu(LogLevel.Warning, message);
    private static void LogMenuError(string message) => LogMenu(LogLevel.Error, message);

}
#nullable restore
