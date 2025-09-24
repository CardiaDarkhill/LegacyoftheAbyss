using System;
using System.Collections;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000442 RID: 1090
public class UIManager : MonoBehaviour
{
	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x060025BA RID: 9658 RVA: 0x000ABBA1 File Offset: 0x000A9DA1
	public bool IsFadingMenu
	{
		get
		{
			return this.isFadingMenu || Time.timeAsDouble < this.startMenuTime;
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x060025BB RID: 9659 RVA: 0x000ABBBA File Offset: 0x000A9DBA
	// (set) Token: 0x060025BC RID: 9660 RVA: 0x000ABBC1 File Offset: 0x000A9DC1
	public static bool IsSelectingProfile { get; private set; }

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x060025BD RID: 9661 RVA: 0x000ABBC9 File Offset: 0x000A9DC9
	// (set) Token: 0x060025BE RID: 9662 RVA: 0x000ABBD0 File Offset: 0x000A9DD0
	public static bool IsSaveProfileMenu { get; private set; }

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x060025BF RID: 9663 RVA: 0x000ABBD8 File Offset: 0x000A9DD8
	public static UIManager instance
	{
		get
		{
			if (UIManager._instance == null)
			{
				UIManager._instance = Object.FindObjectOfType<UIManager>();
				if (UIManager._instance == null)
				{
					Debug.LogError("Couldn't find a UIManager, make sure one exists in the scene.");
					return null;
				}
				if (Application.isPlaying)
				{
					Object.DontDestroyOnLoad(UIManager._instance.gameObject);
				}
			}
			return UIManager._instance;
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000ABC30 File Offset: 0x000A9E30
	private void Awake()
	{
		if (UIManager._instance == null)
		{
			UIManager._instance = this;
			Object.DontDestroyOnLoad(this);
		}
		else if (this != UIManager._instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.graphicRaycaster = base.GetComponentInChildren<GraphicRaycaster>();
		this.AudioGoToGameplay(0f);
		if (this.saveProfileScreen)
		{
			this.saveProfileScreen.gameObject.SetActive(true);
			this.saveProfileScreen.alpha = 0f;
			this.saveProfileScreen.blocksRaycasts = false;
			this.saveProfileScreen.interactable = false;
		}
		this.InitBlackThread();
	}

	// Token: 0x060025C1 RID: 9665 RVA: 0x000ABCD4 File Offset: 0x000A9ED4
	public void SceneInit()
	{
		if (this == UIManager._instance)
		{
			this.SetupRefs();
		}
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000ABCEC File Offset: 0x000A9EEC
	private void Start()
	{
		if (this != UIManager._instance)
		{
			return;
		}
		this.SetupRefs();
		if (this.gm.IsMenuScene())
		{
			this.SetScreenBlankerAlpha(1f);
			float num = this.FadeScreenIn();
			if (Platform.Current.IsSharedDataMounted)
			{
				this.LoadGameSettings();
			}
			this.startMenuTime = Time.timeAsDouble + (double)num;
			this.ConfigureMenu();
			if (DemoHelper.IsDemoMode)
			{
				this.slotOne.Prepare(this.gm, false, true, false);
				this.slotTwo.Prepare(this.gm, false, true, false);
				this.slotThree.Prepare(this.gm, false, true, false);
				this.slotFour.Prepare(this.gm, false, true, false);
			}
			else if (Platform.Current.WillPreloadSaveFiles && Platform.Current.IsSaveStoreMounted)
			{
				this.slotOne.PreloadSave(this.gm);
				this.slotTwo.PreloadSave(this.gm);
				this.slotThree.PreloadSave(this.gm);
				this.slotFour.PreloadSave(this.gm);
			}
			if (DemoHelper.IsDemoMode)
			{
				Navigation navigation = this.slotOne.navigation;
				Navigation navigation2 = this.slotTwo.navigation;
				Navigation navigation3 = this.slotThree.navigation;
				Navigation navigation4 = this.slotFour.navigation;
				navigation.selectOnDown = navigation.selectOnDown.navigation.selectOnDown;
				navigation2.selectOnDown = navigation2.selectOnDown.navigation.selectOnDown;
				navigation3.selectOnDown = navigation3.selectOnDown.navigation.selectOnDown;
				navigation4.selectOnDown = navigation4.selectOnDown.navigation.selectOnDown;
				navigation.selectOnRight = this.GetNavigationRightRecursive(navigation.selectOnRight);
				navigation2.selectOnLeft = this.GetNavigationLeftRecursive(navigation2.selectOnLeft);
				navigation2.selectOnRight = this.GetNavigationRightRecursive(navigation2.selectOnRight);
				navigation3.selectOnLeft = this.GetNavigationLeftRecursive(navigation3.selectOnLeft);
				navigation3.selectOnRight = this.GetNavigationRightRecursive(navigation3.selectOnRight);
				navigation4.selectOnLeft = this.GetNavigationLeftRecursive(navigation4.selectOnLeft);
				this.slotOne.navigation = navigation;
				this.slotTwo.navigation = navigation2;
				this.slotThree.navigation = navigation3;
				this.slotFour.navigation = navigation4;
				Selectable selectOnDown = navigation.selectOnDown;
				Navigation navigation5 = selectOnDown.navigation;
				navigation5.selectOnUp = this.slotOne;
				selectOnDown.navigation = navigation5;
			}
			if (this.menuAchievementsList)
			{
				if (Platform.Current.AreAchievementsFetched)
				{
					this.menuAchievementsList.PreInit();
				}
				else
				{
					Platform.AchievementsFetched += this.menuAchievementsList.PreInit;
				}
			}
		}
		else if (this.gm.startedOnThisScene && Platform.Current.IsSharedDataMounted)
		{
			this.LoadGameSettings();
		}
		this.RegisterSaveStoreChangedEvent();
		if (this.graphicRaycaster && ManagerSingleton<InputHandler>.Instance)
		{
			ManagerSingleton<InputHandler>.Instance.OnCursorVisibilityChange += delegate(bool isVisible)
			{
				this.graphicRaycaster.enabled = isVisible;
			};
		}
		int value = StaticVariableList.GetValue<int>("ExhibitionModeProfileId", 0);
		if (value > 0)
		{
			base.StopAllCoroutines();
			this.uiState = UIState.MAIN_MENU_HOME;
			this.ih.StopUIInput();
			this.saveSlot = value;
			this.gm.LoadGameFromUI(value);
			StaticVariableList.SetValue("ExhibitionModeProfileId", 0, 0);
		}
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000AC064 File Offset: 0x000AA264
	private void OnDestroy()
	{
		if (UIManager._instance == this)
		{
			UIManager._instance = null;
		}
		if (this.menuAchievementsList && !Platform.Current.AreAchievementsFetched)
		{
			Platform.AchievementsFetched -= this.menuAchievementsList.PreInit;
		}
		this.UnregisterSaveStoreChangedEvent();
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000AC0B9 File Offset: 0x000AA2B9
	[ContextMenu("Test Save Reset")]
	private void TestSaveReset()
	{
		this.OnSaveStoreStateChanged(true);
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000AC0C4 File Offset: 0x000AA2C4
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.LoadGameSettings();
			bool doAnimate = this.menuState == MainMenuState.SAVE_PROFILES || UIManager.IsSaveProfileMenu;
			if (this.slotOne)
			{
				this.slotOne.ResetButton(this.gm, doAnimate, true);
			}
			if (this.slotTwo)
			{
				this.slotTwo.ResetButton(this.gm, doAnimate, true);
			}
			if (this.slotThree)
			{
				this.slotThree.ResetButton(this.gm, doAnimate, true);
			}
			if (this.slotFour)
			{
				this.slotFour.ResetButton(this.gm, doAnimate, true);
			}
		}
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000AC16F File Offset: 0x000AA36F
	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			this.lastSelectionBeforeFocusLoss = EventSystem.current.currentSelectedGameObject;
			return;
		}
		if (this.lastSelectionBeforeFocusLoss != null)
		{
			EventSystem.current.SetSelectedGameObject(this.lastSelectionBeforeFocusLoss);
			this.lastSelectionBeforeFocusLoss = null;
		}
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000AC1AC File Offset: 0x000AA3AC
	private Selectable GetNavigationRightRecursive(Selectable selectOnRight)
	{
		if (selectOnRight == null)
		{
			return null;
		}
		if (selectOnRight.gameObject.activeSelf)
		{
			return selectOnRight;
		}
		return this.GetNavigationRightRecursive(selectOnRight.navigation.selectOnRight);
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000AC1E8 File Offset: 0x000AA3E8
	private Selectable GetNavigationLeftRecursive(Selectable selectOnLeft)
	{
		if (selectOnLeft == null)
		{
			return null;
		}
		if (selectOnLeft.gameObject.activeSelf)
		{
			return selectOnLeft;
		}
		return this.GetNavigationLeftRecursive(selectOnLeft.navigation.selectOnLeft);
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000AC224 File Offset: 0x000AA424
	public void SetState(UIState newState)
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (newState != this.uiState)
		{
			if (this.uiState == UIState.PAUSED && newState == UIState.PLAYING)
			{
				this.UIClosePauseMenu();
			}
			else if (this.uiState == UIState.PLAYING && newState == UIState.PAUSED)
			{
				this.UIGoToPauseMenu();
			}
			else if (newState == UIState.INACTIVE)
			{
				this.DisableScreens();
			}
			else if (newState == UIState.MAIN_MENU_HOME)
			{
				if (Platform.Current.EngagementState == Platform.EngagementStates.Engaged)
				{
					this.didLeaveEngageMenu = true;
					this.UIGoToMainMenu();
				}
				else
				{
					this.UIGoToEngageMenu();
				}
			}
			else if (newState == UIState.LOADING)
			{
				this.DisableScreens();
			}
			else if (newState == UIState.PLAYING)
			{
				this.DisableScreens();
			}
			else if (newState == UIState.CUTSCENE)
			{
				this.DisableScreens();
			}
			this.uiState = newState;
			return;
		}
		if (newState == UIState.MAIN_MENU_HOME)
		{
			this.UIGoToMainMenu();
		}
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000AC2E7 File Offset: 0x000AA4E7
	private void SetMenuState(MainMenuState newState)
	{
		this.menuState = newState;
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000AC2F0 File Offset: 0x000AA4F0
	private void SetupRefs()
	{
		this.gm = GameManager.instance;
		this.gs = this.gm.gameSettings;
		this.ih = this.gm.inputHandler;
		if (this.gm.IsMenuScene() && this.gameTitle == null)
		{
			this.gameTitle = GameObject.Find("LogoTitle").GetComponent<SpriteRenderer>();
		}
		if (this.UICanvas.worldCamera == null)
		{
			this.UICanvas.worldCamera = GameCameras.instance.mainCamera;
		}
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000AC382 File Offset: 0x000AA582
	public void SetUIStartState(GameState gameState)
	{
		if (gameState == GameState.MAIN_MENU)
		{
			this.SetState(UIState.MAIN_MENU_HOME);
			return;
		}
		if (gameState == GameState.LOADING)
		{
			this.SetState(UIState.LOADING);
			return;
		}
		if (gameState == GameState.ENTERING_LEVEL)
		{
			this.SetState(UIState.PLAYING);
			return;
		}
		if (gameState == GameState.PLAYING)
		{
			this.SetState(UIState.PLAYING);
			return;
		}
		if (gameState == GameState.CUTSCENE)
		{
			this.SetState(UIState.CUTSCENE);
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000AC3C0 File Offset: 0x000AA5C0
	private void LoadGameSettings()
	{
		LanguageCode languageCode = Language.CurrentLanguage();
		this.LoadGameOptionsSettings();
		this.LoadStoredSettings();
		Language.LoadLanguage();
		LanguageCode languageCode2 = Language.CurrentLanguage();
		if (languageCode != languageCode2)
		{
			this.gm.RefreshLocalization();
			ChangeFontByLanguage[] array = Object.FindObjectsByType<ChangeFontByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFont();
			}
			ChangePositionByLanguage[] array2 = Object.FindObjectsByType<ChangePositionByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].DoOffset();
			}
			ActivatePerLanguage[] array3 = Object.FindObjectsByType<ActivatePerLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].UpdateLanguage();
			}
			ChangeByLanguageBase[] array4 = Object.FindObjectsByType<ChangeByLanguageBase>(FindObjectsSortMode.None);
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].DoUpdate();
			}
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000AC474 File Offset: 0x000AA674
	private void RegisterSaveStoreChangedEvent()
	{
		if (!this.registeredSaveStoreChangedEvent)
		{
			this.registeredSaveStoreChangedEvent = true;
			Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
		}
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000AC496 File Offset: 0x000AA696
	private void UnregisterSaveStoreChangedEvent()
	{
		if (this.registeredSaveStoreChangedEvent)
		{
			this.registeredSaveStoreChangedEvent = false;
			Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
		}
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000AC4B8 File Offset: 0x000AA6B8
	public bool UIGoBack()
	{
		if (UIManager.IsSelectingProfile)
		{
			this.UIGoBackToSaveProfiles();
			return true;
		}
		switch (this.menuState)
		{
		case MainMenuState.OPTIONS_MENU:
			return this.optionsMenuScreen.GoBack();
		case MainMenuState.GAMEPAD_MENU:
			return this.gamepadMenuScreen.GoBack();
		case MainMenuState.KEYBOARD_MENU:
			return this.keyboardMenuScreen.GoBack();
		case MainMenuState.AUDIO_MENU:
			return this.audioMenuScreen.GoBack();
		case MainMenuState.VIDEO_MENU:
			return this.videoMenuScreen.GoBack();
		case MainMenuState.OVERSCAN_MENU:
			return this.overscanMenuScreen.GoBack();
		case MainMenuState.GAME_OPTIONS_MENU:
			return this.gameOptionsMenuScreen.GoBack();
		case MainMenuState.ACHIEVEMENTS_MENU:
			return this.achievementsMenuScreen.GoBack();
		case MainMenuState.RESOLUTION_PROMPT:
			return this.resolutionPrompt.GoBack();
		case MainMenuState.BRIGHTNESS_MENU:
			return this.brightnessMenuScreen.GoBack();
		case MainMenuState.PAUSE_MENU:
			return this.pauseMenuScreen.GoBack();
		case MainMenuState.PLAY_MODE_MENU:
			return this.playModeMenuScreen.GoBack();
		case MainMenuState.EXTRAS_MENU:
			return this.extrasMenuScreen.GoBack();
		case MainMenuState.REMAP_GAMEPAD_MENU:
			return this.remapGamepadMenuScreen.GoBack();
		case MainMenuState.EXTRAS_CONTENT_MENU:
			return this.extrasContentMenuScreen.GoBack();
		case MainMenuState.ENGAGE_MENU:
			return this.engageMenuScreen.GoBack();
		}
		return false;
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000AC5F5 File Offset: 0x000AA7F5
	public void UIGoToOptionsMenu()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000AC604 File Offset: 0x000AA804
	public void UILeaveOptionsMenu()
	{
		if (this.uiState == UIState.PAUSED)
		{
			this.UIGoToPauseMenu();
			return;
		}
		this.UIGoToMainMenu();
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000AC61C File Offset: 0x000AA81C
	public void ClearSaveCache()
	{
		this.slotOne.ClearCache();
		this.slotTwo.ClearCache();
		this.slotThree.ClearCache();
		this.slotFour.ClearCache();
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000AC64A File Offset: 0x000AA84A
	public void UIExplicitSwitchUser()
	{
		this.ClearSaveCache();
		this.UIGoToEngageMenu();
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000AC658 File Offset: 0x000AA858
	public void UIGoToEngageMenu()
	{
		base.StartCoroutine(this.GoToEngageMenu());
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000AC667 File Offset: 0x000AA867
	public void UIGoToMainMenu()
	{
		base.StartCoroutine(this.GoToMainMenu());
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000AC676 File Offset: 0x000AA876
	public void UIGoToProfileMenu()
	{
		base.StartCoroutine(this.GoToProfileMenu());
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000AC685 File Offset: 0x000AA885
	public void UIGoToControllerMenu()
	{
		base.StartCoroutine(this.GoToControllerMenu());
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x000AC694 File Offset: 0x000AA894
	public void UIGoToRemapControllerMenu()
	{
		base.StartCoroutine(this.GoToRemapControllerMenu());
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000AC6A3 File Offset: 0x000AA8A3
	public void UIGoToKeyboardMenu()
	{
		base.StartCoroutine(this.GoToKeyboardMenu());
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000AC6B2 File Offset: 0x000AA8B2
	public void UIGoToAudioMenu()
	{
		base.StartCoroutine(this.GoToAudioMenu());
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000AC6C1 File Offset: 0x000AA8C1
	public void UIGoToVideoMenu(bool rollbackRes = false)
	{
		base.StartCoroutine(this.GoToVideoMenu(rollbackRes));
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000AC6D1 File Offset: 0x000AA8D1
	public void UIGoToPauseMenu()
	{
		base.StartCoroutine(this.GoToPauseMenu());
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000AC6E0 File Offset: 0x000AA8E0
	public void UIClosePauseMenu()
	{
		this.ih.StopUIInput();
		base.StartCoroutine(this.HideCurrentMenu());
		base.StartCoroutine(this.FadeOutCanvasGroup(this.modalDimmer, true, false));
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000AC70F File Offset: 0x000AA90F
	public void UIClearPauseMenu()
	{
		this.pauseMenuAnimator.SetBool(UIManager._clearProp, true);
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000AC722 File Offset: 0x000AA922
	public void UnClearPauseMenu()
	{
		this.pauseMenuAnimator.SetBool(UIManager._clearProp, false);
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000AC735 File Offset: 0x000AA935
	public void UIGoToOverscanMenu()
	{
		base.StartCoroutine(this.GoToOverscanMenu());
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x000AC744 File Offset: 0x000AA944
	public void UIGoToBrightnessMenu()
	{
		base.StartCoroutine(this.GoToBrightnessMenu());
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x000AC753 File Offset: 0x000AA953
	public void UIGoToGameOptionsMenu()
	{
		base.StartCoroutine(this.GoToGameOptionsMenu());
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x000AC762 File Offset: 0x000AA962
	public void UIGoToAchievementsMenu()
	{
		base.StartCoroutine(this.GoToAchievementsMenu());
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x000AC771 File Offset: 0x000AA971
	public void UIGoToExtrasMenu()
	{
		base.StartCoroutine(this.GoToExtrasMenu());
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000AC780 File Offset: 0x000AA980
	public void UIGoToExtrasContentMenu()
	{
		base.StartCoroutine(this.GoToExtrasContentMenu());
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000AC78F File Offset: 0x000AA98F
	public void UIShowQuitGamePrompt()
	{
		base.StartCoroutine(this.GoToQuitGamePrompt());
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x000AC79E File Offset: 0x000AA99E
	public void UIShowReturnMenuPrompt()
	{
		base.StartCoroutine(this.GoToReturnMenuPrompt());
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x000AC7AD File Offset: 0x000AA9AD
	public void UIShowResolutionPrompt(bool startTimer = false)
	{
		base.StartCoroutine(this.GoToResolutionPrompt(startTimer));
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000AC7BD File Offset: 0x000AA9BD
	public void UILeaveExitToMenuPrompt()
	{
		base.StartCoroutine(this.LeaveExitToMenuPrompt());
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000AC7CC File Offset: 0x000AA9CC
	public void UIGoToPlayModeMenu()
	{
		base.StartCoroutine(this.GoToPlayModeMenu());
	}

	// Token: 0x060025EC RID: 9708 RVA: 0x000AC7DB File Offset: 0x000AA9DB
	public void UIReturnToMainMenu()
	{
		base.StartCoroutine(this.ReturnToMainMenu());
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000AC7EA File Offset: 0x000AA9EA
	public void UIGoToMenuCredits()
	{
		base.StartCoroutine(this.GoToMenuCredits());
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000AC7F9 File Offset: 0x000AA9F9
	public void UIStartNewGame()
	{
		this.StartNewGame(false, false);
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000AC803 File Offset: 0x000AAA03
	public void UIStartNewGameContinue()
	{
		if (this.isStartingNewGame)
		{
			this.StartNewGame(this.permaDeath, this.bossRush);
			return;
		}
		this.UIContinueGame(this.saveSlot);
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000AC82C File Offset: 0x000AAA2C
	public void UIGoBackToSaveProfiles()
	{
		base.StartCoroutine(this.GoBackToSaveProfiles());
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000AC83C File Offset: 0x000AAA3C
	public void StartNewGame(bool permaDeath = false, bool bossRush = false)
	{
		this.permaDeath = permaDeath;
		this.bossRush = bossRush;
		this.ih.StopUIInput();
		UIManager.IsSelectingProfile = true;
		this.isStartingNewGame = true;
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 1)
		{
			UIManager.IsSelectingProfile = false;
			this.isStartingNewGame = false;
			this.gm.EnsureSaveSlotSpace(delegate(bool hasSpace)
			{
				if (hasSpace)
				{
					if (this.menuState == MainMenuState.SAVE_PROFILES)
					{
						this.StartCoroutine(this.HideSaveProfileMenu(false));
					}
					else
					{
						this.StartCoroutine(this.HideCurrentMenu());
					}
					this.uiAudioPlayer.PlayStartGame();
					if (MenuStyles.Instance)
					{
						MenuStyles.Instance.StopAudio();
					}
					this.gm.StartNewGame(permaDeath, bossRush);
					return;
				}
				this.ih.StartUIInput();
				SaveSlotButton saveSlotButton;
				switch (this.gm.profileID)
				{
				default:
					saveSlotButton = this.slotOne;
					break;
				case 2:
					saveSlotButton = this.slotTwo;
					break;
				case 3:
					saveSlotButton = this.slotThree;
					break;
				case 4:
					saveSlotButton = this.slotFour;
					break;
				}
				saveSlotButton.Select();
				Debug.LogError("Insufficient space for new save profile", this);
			});
			return;
		}
		if (this.gs.overscanAdjusted == 0)
		{
			this.UIGoToOverscanMenu();
			return;
		}
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 0)
		{
			this.UIGoToBrightnessMenu();
		}
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000AC90C File Offset: 0x000AAB0C
	public void UIContinueGame(int slot)
	{
		this.ih.StopUIInput();
		UIManager.IsSelectingProfile = true;
		this.isStartingNewGame = false;
		this.saveSlot = slot;
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 1)
		{
			UIManager.IsSelectingProfile = false;
			this.gm.LoadGameFromUI(slot);
			return;
		}
		if (this.gs.overscanAdjusted == 0)
		{
			this.UIGoToOverscanMenu();
			return;
		}
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 0)
		{
			this.UIGoToBrightnessMenu();
		}
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000AC99C File Offset: 0x000AAB9C
	public void UIContinueGame(int slot, SaveGameData saveGameData)
	{
		this.ih.StopUIInput();
		UIManager.IsSelectingProfile = true;
		this.isStartingNewGame = false;
		this.saveSlot = slot;
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 1)
		{
			UIManager.IsSelectingProfile = false;
			this.gm.LoadGameFromUI(slot, saveGameData);
			return;
		}
		if (this.gs.overscanAdjusted == 0)
		{
			this.UIGoToOverscanMenu();
			return;
		}
		if (this.gs.overscanAdjusted == 1 && this.gs.brightnessAdjusted == 0)
		{
			this.UIGoToBrightnessMenu();
		}
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000ACA30 File Offset: 0x000AAC30
	public void ContinueGame()
	{
		this.ih.StopUIInput();
		if (MenuStyles.Instance)
		{
			MenuStyles.Instance.StopAudio();
		}
		if (StaticVariableList.GetValue<int>("ExhibitionModeProfileId", 0) > 0)
		{
			return;
		}
		this.uiAudioPlayer.PlayStartGame();
		if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			base.StartCoroutine(this.HideSaveProfileMenu(false));
		}
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000ACA8F File Offset: 0x000AAC8F
	public void PrepareContinueGame()
	{
		this.ih.StopUIInput();
		if (MenuStyles.Instance)
		{
			MenuStyles.Instance.StopAudio();
		}
		if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			base.StartCoroutine(this.HideSaveProfileMenu(false));
		}
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000ACACC File Offset: 0x000AACCC
	public static void HighlightSelectableNoSound(Selectable selectable)
	{
		IPlaySelectSound component = selectable.GetComponent<IPlaySelectSound>();
		if (component != null)
		{
			component.DontPlaySelectSound = true;
			selectable.Select();
			component.DontPlaySelectSound = false;
			return;
		}
		selectable.Select();
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000ACAFE File Offset: 0x000AACFE
	public IEnumerator GoToEngageMenu()
	{
		if (this.ih == null)
		{
			this.ih = this.gm.inputHandler;
		}
		this.ih.StopUIInput();
		this.didLeaveEngageMenu = false;
		Platform.Current.ClearEngagement();
		Platform.Current.BeginEngagement();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			this.mainMenuScreen.interactable = false;
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return base.StartCoroutine(this.HideSaveProfileMenu(true));
		}
		else
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		this.ih.StopUIInput();
		this.gameTitle.gameObject.SetActive(true);
		this.gameTitle.GetComponent<LogoLanguage>().SetSprite();
		this.engageMenuScreen.gameObject.SetActive(true);
		base.StartCoroutine(this.FadeInSprite(this.gameTitle));
		this.subtitleFSM.SendEvent("FADE IN");
		this.engageMenuScreen.topFleur.ResetTrigger(UIManager._hideProp);
		this.engageMenuScreen.topFleur.SetTrigger(UIManager._showProp);
		this.engageMenuScreen.bottomFleur.ResetTrigger(UIManager._hideProp);
		this.engageMenuScreen.bottomFleur.SetTrigger(UIManager._showProp);
		yield return base.StartCoroutine(this.FadeInCanvasGroup(this.engageMenuScreen.GetComponent<CanvasGroup>(), false));
		yield return null;
		this.SetMenuState(MainMenuState.ENGAGE_MENU);
		yield break;
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000ACB0D File Offset: 0x000AAD0D
	public IEnumerator GoToMainMenu()
	{
		if (this.ih == null)
		{
			this.ih = this.gm.inputHandler;
		}
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.OPTIONS_MENU || this.menuState == MainMenuState.ACHIEVEMENTS_MENU || this.menuState == MainMenuState.QUIT_GAME_PROMPT || this.menuState == MainMenuState.EXTRAS_MENU || this.menuState == MainMenuState.ENGAGE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		else if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return base.StartCoroutine(this.HideSaveProfileMenu(true));
		}
		else
		{
			yield return null;
		}
		this.ih.StopUIInput();
		this.gameTitle.gameObject.SetActive(true);
		this.gameTitle.GetComponent<LogoLanguage>().SetSprite();
		base.StartCoroutine(this.FadeInSprite(this.gameTitle));
		this.subtitleFSM.SendEvent("FADE IN");
		float num = (float)(this.startMenuTime - Time.timeAsDouble);
		if (num > 0f)
		{
			yield return new WaitForSeconds(num);
		}
		this.mainMenuScreen.gameObject.SetActive(true);
		yield return base.StartCoroutine(this.FadeInCanvasGroup(this.mainMenuScreen, false));
		yield return null;
		this.mainMenuScreen.interactable = true;
		this.ih.StartUIInput();
		this.mainMenuButtons.HighlightDefault(false);
		this.SetMenuState(MainMenuState.MAIN_MENU);
		yield break;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000ACB1C File Offset: 0x000AAD1C
	public IEnumerator GoToProfileMenu()
	{
		UIManager.IsSelectingProfile = false;
		this.isStartingNewGame = false;
		UIManager.IsSaveProfileMenu = true;
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			base.StartCoroutine(this.FadeOutSprite(this.gameTitle));
			this.subtitleFSM.SendEvent("FADE OUT");
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else if (this.menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.ih.StopUIInput();
		}
		SaveSlotButton saveSlotButton;
		switch (DemoHelper.IsDemoMode ? 0 : Platform.Current.LocalSharedData.GetInt("lastProfileIndex", 0))
		{
		case 2:
			saveSlotButton = this.slotTwo;
			break;
		case 3:
			saveSlotButton = this.slotThree;
			break;
		case 4:
			saveSlotButton = this.slotFour;
			break;
		default:
			saveSlotButton = this.slotOne;
			break;
		}
		SaveSlotButton itemToHighlight = saveSlotButton;
		this.saveProfilePreselect.itemToHighlight = itemToHighlight;
		base.StartCoroutine(this.FadeInCanvasGroup(this.saveProfileScreen, true));
		base.StartCoroutine(this.FadeInCanvasGroup(this.saveProfileTitle, false));
		this.saveProfileTopFleur.ResetTrigger(UIManager._hideProp);
		this.saveProfileTopFleur.SetTrigger(UIManager._showProp);
		bool hasPreload = this.slotOne.HasPreloaded;
		if (hasPreload)
		{
			yield return new WaitForSeconds(0.165f);
		}
		base.StartCoroutine(this.PrepareSaveFilesInOrder());
		if (!hasPreload)
		{
			yield return new WaitForSeconds(0.165f);
		}
		this.uiAudioPlayer.PlayOpenProfileSelect();
		SaveSlotButton[] array = new SaveSlotButton[]
		{
			this.slotOne,
			this.slotTwo,
			this.slotThree,
			this.slotFour
		};
		foreach (SaveSlotButton saveSlotButton2 in array)
		{
			if (saveSlotButton2 && saveSlotButton2.gameObject.activeSelf)
			{
				saveSlotButton2.UpdateSaveFileState();
				saveSlotButton2.ShowRelevantModeForSaveFileState();
				yield return new WaitForSeconds(0.165f);
			}
		}
		SaveSlotButton[] array2 = null;
		yield return new WaitForSeconds(0.165f);
		base.StartCoroutine(this.FadeInCanvasGroup(this.saveProfileControls, false));
		yield return null;
		this.ih.StartUIInput();
		this.saveSlots.HighlightDefault(false);
		this.SetMenuState(MainMenuState.SAVE_PROFILES);
		UIManager.IsSaveProfileMenu = false;
		yield break;
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000ACB2C File Offset: 0x000AAD2C
	private void InitBlackThread()
	{
		if (this.initBlackThreadComponents)
		{
			return;
		}
		this.initBlackThreadComponents = true;
		if (this.blackThreadLoopAudio == null)
		{
			return;
		}
		this.blackThreadAudioFader = this.blackThreadLoopAudio.GetComponent<AudioSourceFadeControl>();
		this.hasBlackThreadAudioFader = (this.blackThreadAudioFader != null);
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000ACB7C File Offset: 0x000AAD7C
	public void UpdateBlackThreadAudio()
	{
		if (!this.blackThreadLoopAudio)
		{
			return;
		}
		this.InitBlackThread();
		if (this.slotOne.IsBlackThreaded || this.slotTwo.IsBlackThreaded || this.slotThree.IsBlackThreaded || this.slotFour.IsBlackThreaded)
		{
			if (this.hasBlackThreadAudioFader)
			{
				this.blackThreadAudioFader.FadeUp();
				if (!this.blackThreadLoopAudio.activeSelf)
				{
					this.blackThreadLoopAudio.SetActive(true);
				}
			}
			else
			{
				this.blackThreadLoopAudio.SetActive(true);
			}
			this.blackThreadMusicSnapshot.TransitionTo(this.musicSnapshotTransitionTime);
			return;
		}
		if (this.hasBlackThreadAudioFader)
		{
			this.blackThreadAudioFader.FadeDown();
		}
		else
		{
			this.blackThreadLoopAudio.SetActive(false);
		}
		this.defaultMusicSnapshot.TransitionTo(this.musicSnapshotTransitionTime);
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000ACC52 File Offset: 0x000AAE52
	public void FadeOutBlackThreadLoop()
	{
		if (!this.blackThreadLoopAudio)
		{
			return;
		}
		if (this.hasBlackThreadAudioFader)
		{
			this.blackThreadAudioFader.FadeDown();
			return;
		}
		this.blackThreadLoopAudio.SetActive(false);
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000ACC82 File Offset: 0x000AAE82
	protected IEnumerator PrepareSaveFilesInOrder()
	{
		foreach (SaveSlotButton saveSlotButton in new SaveSlotButton[]
		{
			this.slotOne,
			this.slotTwo,
			this.slotThree,
			this.slotFour
		})
		{
			if (saveSlotButton && saveSlotButton.saveFileState == SaveSlotButton.SaveFileStates.NotStarted)
			{
				saveSlotButton.PreloadSave(this.gm);
			}
		}
		SaveSlotButton[] slotButtons;
		int num;
		for (int i = 0; i < slotButtons.Length; i = num)
		{
			SaveSlotButton slotButton = slotButtons[i];
			if (slotButton)
			{
				if (slotButton.saveFileState == SaveSlotButton.SaveFileStates.NotStarted)
				{
					slotButton.Prepare(this.gm, false, true, false);
					float waitTime = 0.165f;
					while (slotButton.saveFileState == SaveSlotButton.SaveFileStates.OperationInProgress)
					{
						yield return null;
						waitTime -= Time.deltaTime;
					}
					if (waitTime > 0f)
					{
						yield return new WaitForSeconds(waitTime);
					}
				}
				slotButton = null;
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000ACC91 File Offset: 0x000AAE91
	public IEnumerator GoToOptionsMenu()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			base.StartCoroutine(this.FadeOutSprite(this.gameTitle));
			this.subtitleFSM.SendEvent("FADE OUT");
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else if (this.menuState == MainMenuState.AUDIO_MENU || this.menuState == MainMenuState.VIDEO_MENU || this.menuState == MainMenuState.GAMEPAD_MENU || this.menuState == MainMenuState.GAME_OPTIONS_MENU || this.menuState == MainMenuState.PAUSE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		else if (this.menuState == MainMenuState.KEYBOARD_MENU)
		{
			if (this.uiButtonSkins.listeningKey != null)
			{
				this.uiButtonSkins.listeningKey.StopActionListening();
				this.uiButtonSkins.listeningKey.AbortRebind();
			}
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		yield return base.StartCoroutine(this.ShowMenu(this.optionsMenuScreen));
		this.SetMenuState(MainMenuState.OPTIONS_MENU);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000ACCA0 File Offset: 0x000AAEA0
	public IEnumerator GoToControllerMenu()
	{
		if (this.menuState == MainMenuState.OPTIONS_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		else if (this.menuState == MainMenuState.REMAP_GAMEPAD_MENU)
		{
			if (this.uiButtonSkins.listeningButton != null)
			{
				this.uiButtonSkins.listeningButton.StopActionListening();
				this.uiButtonSkins.listeningButton.AbortRebind();
			}
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		yield return base.StartCoroutine(this.ShowMenu(this.gamepadMenuScreen));
		this.SetMenuState(MainMenuState.GAMEPAD_MENU);
		yield break;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000ACCAF File Offset: 0x000AAEAF
	public IEnumerator GoToRemapControllerMenu()
	{
		yield return base.StartCoroutine(this.HideCurrentMenu());
		base.StartCoroutine(this.ShowMenu(this.remapGamepadMenuScreen));
		yield return base.StartCoroutine(this.uiButtonSkins.ShowCurrentButtonMappings());
		this.SetMenuState(MainMenuState.REMAP_GAMEPAD_MENU);
		yield break;
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000ACCBE File Offset: 0x000AAEBE
	public IEnumerator GoToKeyboardMenu()
	{
		yield return base.StartCoroutine(this.HideCurrentMenu());
		base.StartCoroutine(this.ShowMenu(this.keyboardMenuScreen));
		this.uiButtonSkins.ShowCurrentKeyboardMappings();
		this.SetMenuState(MainMenuState.KEYBOARD_MENU);
		yield break;
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000ACCCD File Offset: 0x000AAECD
	public IEnumerator GoToAudioMenu()
	{
		yield return base.StartCoroutine(this.HideCurrentMenu());
		yield return base.StartCoroutine(this.ShowMenu(this.audioMenuScreen));
		this.SetMenuState(MainMenuState.AUDIO_MENU);
		yield break;
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000ACCDC File Offset: 0x000AAEDC
	public IEnumerator GoToVideoMenu(bool rollbackRes = false)
	{
		if (this.menuState == MainMenuState.OPTIONS_MENU || this.menuState == MainMenuState.OVERSCAN_MENU || this.menuState == MainMenuState.BRIGHTNESS_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		else if (this.menuState == MainMenuState.RESOLUTION_PROMPT)
		{
			if (rollbackRes)
			{
				this.HideMenuInstant(this.resolutionPrompt);
				this.videoMenuScreen.gameObject.SetActive(true);
				this.eventSystem.SetSelectedGameObject(null);
				this.resolutionOption.RollbackResolution();
			}
			else
			{
				yield return base.StartCoroutine(this.HideCurrentMenu());
			}
		}
		yield return base.StartCoroutine(this.ShowMenu(this.videoMenuScreen));
		this.SetMenuState(MainMenuState.VIDEO_MENU);
		yield break;
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000ACCF2 File Offset: 0x000AAEF2
	public IEnumerator GoToOverscanMenu()
	{
		if (this.menuState == MainMenuState.VIDEO_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.overscanSetting.NormalMode();
		}
		else if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return base.StartCoroutine(this.HideSaveProfileMenu(true));
			this.overscanSetting.DoneMode();
		}
		else if (this.menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.overscanSetting.DoneMode();
		}
		yield return base.StartCoroutine(this.ShowMenu(this.overscanMenuScreen));
		this.SetMenuState(MainMenuState.OVERSCAN_MENU);
		yield break;
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000ACD01 File Offset: 0x000AAF01
	public IEnumerator GoToBrightnessMenu()
	{
		if (this.menuState == MainMenuState.VIDEO_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.brightnessSetting.NormalMode();
		}
		else if (this.menuState == MainMenuState.OVERSCAN_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.brightnessSetting.DoneMode();
		}
		else if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return base.StartCoroutine(this.HideSaveProfileMenu(true));
			this.brightnessSetting.DoneMode();
		}
		else if (this.menuState == MainMenuState.PLAY_MODE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
			this.brightnessSetting.DoneMode();
		}
		yield return base.StartCoroutine(this.ShowMenu(this.brightnessMenuScreen));
		this.SetMenuState(MainMenuState.BRIGHTNESS_MENU);
		yield break;
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000ACD10 File Offset: 0x000AAF10
	public IEnumerator GoToGameOptionsMenu()
	{
		yield return base.StartCoroutine(this.HideCurrentMenu());
		yield return base.StartCoroutine(this.ShowMenu(this.gameOptionsMenuScreen));
		this.SetMenuState(MainMenuState.GAME_OPTIONS_MENU);
		yield break;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000ACD1F File Offset: 0x000AAF1F
	public IEnumerator GoToAchievementsMenu()
	{
		if (Platform.Current.HasNativeAchievementsDialog)
		{
			Platform.Current.ShowNativeAchievementsDialog();
			yield return null;
			this.mainMenuButtons.achievementsButton.Select();
			yield break;
		}
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			base.StartCoroutine(this.FadeOutSprite(this.gameTitle));
			this.subtitleFSM.SendEvent("FADE OUT");
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.achievementsMenuScreen));
		this.SetMenuState(MainMenuState.ACHIEVEMENTS_MENU);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000ACD2E File Offset: 0x000AAF2E
	public IEnumerator GoToExtrasMenu()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			base.StartCoroutine(this.FadeOutSprite(this.gameTitle));
			this.subtitleFSM.SendEvent("FADE OUT");
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else if (this.menuState == MainMenuState.EXTRAS_CONTENT_MENU)
		{
			yield return base.StartCoroutine(this.HideMenu(this.extrasContentMenuScreen, true));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.extrasMenuScreen));
		this.SetMenuState(MainMenuState.EXTRAS_MENU);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x000ACD3D File Offset: 0x000AAF3D
	public IEnumerator GoToExtrasContentMenu()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.EXTRAS_MENU)
		{
			yield return base.StartCoroutine(this.HideMenu(this.extrasMenuScreen, true));
		}
		else
		{
			Debug.LogError("Entering from this menu not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.extrasContentMenuScreen));
		this.SetMenuState(MainMenuState.EXTRAS_CONTENT_MENU);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000ACD4C File Offset: 0x000AAF4C
	public IEnumerator GoToQuitGamePrompt()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.MAIN_MENU)
		{
			base.StartCoroutine(this.FadeOutSprite(this.gameTitle));
			this.subtitleFSM.SendEvent("FADE OUT");
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.mainMenuScreen, true, false));
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.quitGamePrompt));
		this.SetMenuState(MainMenuState.QUIT_GAME_PROMPT);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000ACD5B File Offset: 0x000AAF5B
	public IEnumerator GoToReturnMenuPrompt()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.PAUSE_MENU)
		{
			yield return base.StartCoroutine(this.HideCurrentMenu());
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.returnMainMenuPrompt));
		this.SetMenuState(MainMenuState.EXIT_PROMPT);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x000ACD6A File Offset: 0x000AAF6A
	public IEnumerator GoToResolutionPrompt(bool startTimer = false)
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.VIDEO_MENU)
		{
			yield return base.StartCoroutine(this.HideMenu(this.videoMenuScreen, true));
		}
		else
		{
			Debug.LogError("Switching between these menus is not implemented.");
		}
		yield return base.StartCoroutine(this.ShowMenu(this.resolutionPrompt));
		this.SetMenuState(MainMenuState.RESOLUTION_PROMPT);
		if (startTimer)
		{
			this.countdownTimer.StartTimer();
		}
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x000ACD80 File Offset: 0x000AAF80
	public IEnumerator LeaveExitToMenuPrompt()
	{
		yield return base.StartCoroutine(this.HideCurrentMenu());
		if (this.uiState == UIState.PAUSED)
		{
			this.UnClearPauseMenu();
		}
		yield break;
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x000ACD8F File Offset: 0x000AAF8F
	public IEnumerator GoToPlayModeMenu()
	{
		this.ih.StopUIInput();
		if (this.menuState == MainMenuState.SAVE_PROFILES)
		{
			yield return base.StartCoroutine(this.HideSaveProfileMenu(true));
		}
		yield return base.StartCoroutine(this.ShowMenu(this.playModeMenuScreen));
		this.SetMenuState(MainMenuState.PLAY_MODE_MENU);
		this.ih.StartUIInput();
		yield break;
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000ACD9E File Offset: 0x000AAF9E
	public IEnumerator ReturnToMainMenu()
	{
		this.ih.StopUIInput();
		bool calledBack = false;
		base.StartCoroutine(this.gm.ReturnToMainMenu(true, delegate(bool willComplete)
		{
			calledBack = true;
			if (!willComplete)
			{
				this.ih.StartUIInput();
				this.returnMainMenuPrompt.HighlightDefault();
				return;
			}
			this.StartCoroutine(this.HideCurrentMenu());
		}, false, false));
		while (!calledBack)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000ACDAD File Offset: 0x000AAFAD
	public IEnumerator EmergencyReturnToMainMenu()
	{
		this.ih.StopUIInput();
		bool calledBack = false;
		PlayerData playerData = this.gm.playerData;
		bool flag;
		if ((!InteractManager.BlockingInteractable || playerData.atBench) && !playerData.disablePause && !playerData.disableSaveQuit && this.ih.PauseAllowed)
		{
			GameState gameState = this.gm.GameState;
			if (gameState == GameState.PLAYING || gameState == GameState.PAUSED)
			{
				flag = !ScenePreloader.HasPendingOperations;
				goto IL_B9;
			}
		}
		flag = false;
		IL_B9:
		bool willSave = flag;
		base.StartCoroutine(this.gm.ReturnToMainMenu(willSave, delegate(bool willComplete)
		{
			calledBack = true;
			if (!willComplete)
			{
				this.ih.StartUIInput();
				this.returnMainMenuPrompt.HighlightDefault();
				return;
			}
			this.ih.StartUIInput();
			this.StartCoroutine(this.HideCurrentMenu());
		}, false, true));
		while (!calledBack)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x000ACDBC File Offset: 0x000AAFBC
	public IEnumerator GoToPauseMenu()
	{
		this.ih.StopUIInput();
		this.ignoreUnpause = true;
		if (this.uiState == UIState.PAUSED)
		{
			if (this.menuState == MainMenuState.OPTIONS_MENU || this.menuState == MainMenuState.EXIT_PROMPT)
			{
				yield return base.StartCoroutine(this.HideCurrentMenu());
			}
		}
		else
		{
			base.StartCoroutine(this.FadeInCanvasGroupAlpha(this.modalDimmer, 0.8f));
		}
		yield return base.StartCoroutine(this.ShowMenu(this.pauseMenuScreen));
		this.SetMenuState(MainMenuState.PAUSE_MENU);
		this.ih.StartUIInput();
		this.ignoreUnpause = false;
		yield break;
	}

	// Token: 0x06002612 RID: 9746 RVA: 0x000ACDCB File Offset: 0x000AAFCB
	public IEnumerator GoToMenuCredits()
	{
		this.ih.StopUIInput();
		yield return base.StartCoroutine(this.HideCurrentMenu());
		float num = this.FadeScreenOut();
		if (num > 0.25f)
		{
			yield return new WaitForSeconds(num - 0.25f);
			TransitionAudioFader.FadeOutAllFaders();
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			TransitionAudioFader.FadeOutAllFaders();
			yield return new WaitForSeconds(num);
		}
		this.gm.LoadScene("Menu_Credits");
		yield break;
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x000ACDDA File Offset: 0x000AAFDA
	private IEnumerator GoBackToSaveProfiles()
	{
		this.ih.StopAcceptingInput();
		this.isGoingBack = true;
		yield return this.HideCurrentMenu();
		yield return this.GoToProfileMenu();
		this.isGoingBack = false;
		yield break;
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000ACDEC File Offset: 0x000AAFEC
	public void ReloadSaves()
	{
		bool doAnimate = this.menuState == MainMenuState.SAVE_PROFILES;
		this.slotOne.ResetButton(this.gm, doAnimate, true);
		this.slotTwo.ResetButton(this.gm, doAnimate, true);
		this.slotThree.ResetButton(this.gm, doAnimate, true);
		this.slotFour.ResetButton(this.gm, doAnimate, true);
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000ACE4F File Offset: 0x000AB04F
	public void ShowCutscenePrompt(CinematicSkipPopup.Texts text)
	{
		this.cinematicSkipPopup.Show(text);
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000ACE5D File Offset: 0x000AB05D
	public void HideCutscenePrompt(bool isInstant, Action onEnd = null)
	{
		this.cinematicSkipPopup.Hide(isInstant, onEnd);
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000ACE6C File Offset: 0x000AB06C
	public void ApplyAudioMenuSettings()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000ACE7B File Offset: 0x000AB07B
	public void ApplyVideoMenuSettings()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000ACE8A File Offset: 0x000AB08A
	public void ApplyControllerMenuSettings()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x000ACE99 File Offset: 0x000AB099
	public void ApplyRemapGamepadMenuSettings()
	{
		base.StartCoroutine(this.GoToControllerMenu());
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x000ACEA8 File Offset: 0x000AB0A8
	public void ApplyKeyboardMenuSettings()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x000ACEB7 File Offset: 0x000AB0B7
	public void ApplyOverscanSettings(bool goToBrightness = false)
	{
		Debug.LogError("This function is now deprecated");
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x000ACEC3 File Offset: 0x000AB0C3
	public void ApplyBrightnessSettings()
	{
		base.StartCoroutine(this.GoToVideoMenu(false));
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x000ACED3 File Offset: 0x000AB0D3
	public void ApplyGameMenuSettings()
	{
		base.StartCoroutine(this.GoToOptionsMenu());
	}

	// Token: 0x0600261F RID: 9759 RVA: 0x000ACEE2 File Offset: 0x000AB0E2
	public void SaveOverscanSettings()
	{
		this.gs.SaveOverscanSettings();
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x000ACEEF File Offset: 0x000AB0EF
	public void SaveBrightnessSettings()
	{
		this.gs.SaveBrightnessSettings();
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x000ACEFC File Offset: 0x000AB0FC
	public void DefaultAudioMenuSettings()
	{
		this.gs.ResetAudioSettings();
		this.RefreshAudioControls();
	}

	// Token: 0x06002622 RID: 9762 RVA: 0x000ACF10 File Offset: 0x000AB110
	public void DefaultVideoMenuSettings()
	{
		this.gs.ResetVideoSettings();
		Platform.Current.AdjustGraphicsSettings(this.gs);
		this.resolutionOption.ResetToDefaultResolution();
		this.fullscreenOption.UpdateSetting(this.gs.fullScreen);
		if (this.fullscreenMenuOption != null)
		{
			this.fullscreenMenuOption.RefreshMenuControls();
			this.fullscreenMenuOption.UpdateApplyButton();
		}
		this.vsyncOption.UpdateSetting(this.gs.vSync);
		this.shadersOption.UpdateSetting((int)this.gs.shaderQuality);
		this.RefreshVideoControls();
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x000ACFAF File Offset: 0x000AB1AF
	public void DefaultGamepadMenuSettings()
	{
		this.ih.ResetDefaultControllerButtonBindings();
		this.uiButtonSkins.RefreshButtonMappings();
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000ACFC7 File Offset: 0x000AB1C7
	public void DefaultKeyboardMenuSettings()
	{
		this.ih.ResetDefaultKeyBindings();
		this.uiButtonSkins.RefreshKeyMappings();
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x000ACFDF File Offset: 0x000AB1DF
	public void DefaultGameMenuSettings()
	{
		this.gs.ResetGameOptionsSettings();
		Platform.Current.AdjustGameSettings(this.gs);
		this.RefreshGameOptionsControls();
	}

	// Token: 0x06002626 RID: 9766 RVA: 0x000AD002 File Offset: 0x000AB202
	public void LoadStoredSettings()
	{
		this.gs.LoadOverscanConfigured();
		this.gs.LoadBrightnessConfigured();
		this.LoadAudioSettings();
		this.LoadVideoSettings();
		this.LoadGameOptionsSettings();
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x000AD02C File Offset: 0x000AB22C
	private void LoadAudioSettings()
	{
		this.gs.LoadAudioSettings();
		this.RefreshAudioControls();
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x000AD03F File Offset: 0x000AB23F
	private void LoadVideoSettings()
	{
		this.gs.LoadVideoSettings();
		this.gs.LoadOverscanSettings();
		this.gs.LoadBrightnessSettings();
		Platform.Current.AdjustGraphicsSettings(this.gs);
		this.RefreshVideoControls();
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x000AD078 File Offset: 0x000AB278
	private void LoadGameOptionsSettings()
	{
		this.gs.LoadGameOptionsSettings();
		Platform.Current.AdjustGameSettings(this.gs);
		this.RefreshGameOptionsControls();
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x000AD09B File Offset: 0x000AB29B
	private void LoadControllerSettings()
	{
		Debug.LogError("Not yet implemented.");
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000AD0A7 File Offset: 0x000AB2A7
	private void RefreshAudioControls()
	{
		this.masterSlider.RefreshValueFromSettings();
		this.musicSlider.RefreshValueFromSettings();
		this.soundSlider.RefreshValueFromSettings();
		this.playerVoiceSetting.RefreshValueFromGameSettings(false);
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x000AD0D8 File Offset: 0x000AB2D8
	private void RefreshVideoControls()
	{
		this.resolutionOption.RefreshControls();
		this.fullscreenOption.RefreshValueFromGameSettings(false);
		this.vsyncOption.RefreshValueFromGameSettings(true);
		this.overscanSetting.RefreshValueFromSettings();
		this.brightnessSetting.RefreshValueFromSettings();
		this.displayOption.RefreshControls();
		this.frameCapOption.RefreshValueFromGameSettings();
		this.particlesOption.RefreshValueFromGameSettings(false);
		this.shadersOption.RefreshValueFromGameSettings(false);
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x000AD14C File Offset: 0x000AB34C
	public void DisableFrameCapSetting()
	{
		if (this.frameCapOption)
		{
			this.frameCapOption.DisableFrameCapSetting();
		}
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000AD166 File Offset: 0x000AB366
	public void DisableVsyncSetting()
	{
		if (this.vsyncOption)
		{
			this.vsyncOption.UpdateSetting(0);
			this.vsyncOption.RefreshValueFromGameSettings(false);
		}
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000AD18D File Offset: 0x000AB38D
	private void RefreshKeyboardControls()
	{
		this.uiButtonSkins.RefreshKeyMappings();
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000AD19C File Offset: 0x000AB39C
	private void RefreshGameOptionsControls()
	{
		this.languageSetting.RefreshControls();
		this.backerCreditsSetting.RefreshValueFromGameSettings(false);
		this.nativeAchievementsSetting.RefreshValueFromGameSettings(false);
		this.controllerRumbleSetting.RefreshValueFromGameSettings(true);
		this.cameraShakeSetting.RefreshValueFromGameSettings(true);
		this.hudSetting.RefreshValueFromGameSettings(true);
		this.switchFrameCapSetting.RefreshValueFromGameSettings(true);
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000AD1FC File Offset: 0x000AB3FC
	public void TogglePauseGame()
	{
		if (!this.ignoreUnpause)
		{
			base.StartCoroutine(this.gm.PauseGameToggleByMenu());
		}
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000AD218 File Offset: 0x000AB418
	public void QuitGame()
	{
		this.ih.StopUIInput();
		base.StartCoroutine(this.gm.QuitGame());
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000AD237 File Offset: 0x000AB437
	public void FadeOutMenuAudio(float duration)
	{
		this.menuSilenceSnapshot.TransitionToSafe(duration);
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000AD245 File Offset: 0x000AB445
	public void AudioGoToPauseMenu(float duration)
	{
		this.menuPauseSnapshot.TransitionToSafe(duration);
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000AD253 File Offset: 0x000AB453
	public void AudioGoToGameplay(float duration)
	{
		this.gameplaySnapshot.TransitionToSafe(duration);
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000AD264 File Offset: 0x000AB464
	public void ConfigureMenu()
	{
		if (this.mainMenuButtons != null)
		{
			this.mainMenuButtons.ConfigureNavigation();
		}
		if (this.gameMenuOptions != null)
		{
			this.gameMenuOptions.ConfigureNavigation();
		}
		if (this.videoMenuOptions != null)
		{
			this.videoMenuOptions.ConfigureNavigation();
		}
		UIState uistate = this.uiState;
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000AD2C5 File Offset: 0x000AB4C5
	public IEnumerator HideCurrentMenu()
	{
		this.isFadingMenu = true;
		float disableAfterDelay = 0f;
		MenuScreen menu;
		switch (this.menuState)
		{
		case MainMenuState.OPTIONS_MENU:
			menu = this.optionsMenuScreen;
			goto IL_26C;
		case MainMenuState.GAMEPAD_MENU:
			menu = this.gamepadMenuScreen;
			this.gs.SaveGameOptionsSettings();
			goto IL_26C;
		case MainMenuState.KEYBOARD_MENU:
			menu = this.keyboardMenuScreen;
			this.ih.SendKeyBindingsToGameSettings();
			this.gs.SaveKeyboardSettings();
			goto IL_26C;
		case MainMenuState.AUDIO_MENU:
			menu = this.audioMenuScreen;
			this.gs.SaveAudioSettings();
			goto IL_26C;
		case MainMenuState.VIDEO_MENU:
			menu = this.videoMenuScreen;
			this.gs.SaveVideoSettings();
			goto IL_26C;
		case MainMenuState.EXIT_PROMPT:
			menu = this.returnMainMenuPrompt;
			goto IL_26C;
		case MainMenuState.OVERSCAN_MENU:
			menu = this.overscanMenuScreen;
			goto IL_26C;
		case MainMenuState.GAME_OPTIONS_MENU:
			menu = this.gameOptionsMenuScreen;
			this.gs.SaveGameOptionsSettings();
			goto IL_26C;
		case MainMenuState.ACHIEVEMENTS_MENU:
			menu = this.achievementsMenuScreen;
			goto IL_26C;
		case MainMenuState.QUIT_GAME_PROMPT:
			menu = this.quitGamePrompt;
			goto IL_26C;
		case MainMenuState.RESOLUTION_PROMPT:
			menu = this.resolutionPrompt;
			goto IL_26C;
		case MainMenuState.BRIGHTNESS_MENU:
			menu = this.brightnessMenuScreen;
			if (!this.isGoingBack)
			{
				this.gs.SaveBrightnessSettings();
				goto IL_26C;
			}
			goto IL_26C;
		case MainMenuState.PAUSE_MENU:
			menu = this.pauseMenuScreen;
			goto IL_26C;
		case MainMenuState.PLAY_MODE_MENU:
			menu = this.playModeMenuScreen;
			disableAfterDelay = 0.5f;
			goto IL_26C;
		case MainMenuState.EXTRAS_MENU:
			menu = this.extrasMenuScreen;
			goto IL_26C;
		case MainMenuState.REMAP_GAMEPAD_MENU:
			menu = this.remapGamepadMenuScreen;
			if (this.uiButtonSkins.listeningButton != null)
			{
				this.uiButtonSkins.listeningButton.StopActionListening();
				this.uiButtonSkins.listeningButton.AbortRebind();
			}
			this.ih.SendButtonBindingsToGameSettings();
			this.gs.SaveGamepadSettings(this.ih.activeGamepadType);
			goto IL_26C;
		case MainMenuState.ENGAGE_MENU:
			menu = this.engageMenuScreen;
			goto IL_26C;
		}
		yield break;
		IL_26C:
		yield return base.StartCoroutine(this.HideMenu(menu, disableAfterDelay <= 0f));
		if (disableAfterDelay > 0f)
		{
			yield return new WaitForSeconds(disableAfterDelay);
			menu.gameObject.SetActive(false);
		}
		this.isFadingMenu = false;
		yield break;
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000AD2D4 File Offset: 0x000AB4D4
	public IEnumerator ShowMenu(MenuScreen menu)
	{
		this.isFadingMenu = true;
		this.ih.StopUIInput();
		this.StopCurrentFade(menu);
		this.fadingMenuScreen = menu;
		if (menu.ScreenCanvasGroup != null)
		{
			this.fadingRoutine = base.StartCoroutine(this.FadeInCanvasGroup(menu.ScreenCanvasGroup, false));
		}
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(UIManager._hideProp);
			menu.topFleur.SetTrigger(UIManager._showProp);
		}
		if (menu.bottomFleur != null)
		{
			menu.bottomFleur.ResetTrigger(UIManager._hideProp);
			menu.bottomFleur.SetTrigger(UIManager._showProp);
		}
		yield return null;
		MenuButtonList component = menu.GetComponent<MenuButtonList>();
		if (component)
		{
			component.SetupActive();
		}
		if (menu.HighlightBehaviour == MenuScreen.HighlightDefaultBehaviours.BeforeFade)
		{
			menu.HighlightDefault();
		}
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.1f));
		this.ih.StartUIInput();
		yield return null;
		if (menu.HighlightBehaviour == MenuScreen.HighlightDefaultBehaviours.AfterFade)
		{
			menu.HighlightDefault();
		}
		this.isFadingMenu = false;
		yield break;
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000AD2EA File Offset: 0x000AB4EA
	private void StopCurrentFade(MenuScreen menu)
	{
		if (this.fadingMenuScreen == menu && this.fadingRoutine != null)
		{
			base.StopCoroutine(this.fadingRoutine);
			this.fadingRoutine = null;
		}
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x000AD315 File Offset: 0x000AB515
	public IEnumerator HideMenu(MenuScreen menu, bool disable = true)
	{
		this.isFadingMenu = true;
		this.ih.StopUIInput();
		this.StopCurrentFade(menu);
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(UIManager._showProp);
			menu.topFleur.SetTrigger(UIManager._hideProp);
		}
		if (menu.bottomFleur != null)
		{
			menu.bottomFleur.ResetTrigger(UIManager._showProp);
			menu.bottomFleur.SetTrigger(UIManager._hideProp);
		}
		if (menu.ScreenCanvasGroup != null)
		{
			yield return base.StartCoroutine(this.FadeOutCanvasGroup(menu.ScreenCanvasGroup, disable, false));
		}
		this.ih.StartUIInput();
		this.isFadingMenu = false;
		yield break;
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x000AD334 File Offset: 0x000AB534
	public void HideMenuInstant(MenuScreen menu)
	{
		this.ih.StopUIInput();
		if (menu.topFleur != null)
		{
			menu.topFleur.ResetTrigger(UIManager._showProp);
			menu.topFleur.SetTrigger(UIManager._hideProp);
		}
		this.HideCanvasGroup(menu.ScreenCanvasGroup);
		this.ih.StartUIInput();
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x000AD391 File Offset: 0x000AB591
	public IEnumerator HideSaveProfileMenu(bool updateBlackThread)
	{
		base.StartCoroutine(this.FadeOutCanvasGroup(this.saveProfileTitle, true, false));
		this.saveProfileTopFleur.ResetTrigger(UIManager._showProp);
		this.saveProfileTopFleur.SetTrigger(UIManager._hideProp);
		base.StartCoroutine(this.FadeOutCanvasGroup(this.saveProfileControls, true, false));
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		this.slotOne.HideSaveSlot(updateBlackThread);
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		this.slotTwo.HideSaveSlot(updateBlackThread);
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		this.slotThree.HideSaveSlot(updateBlackThread);
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.165f));
		this.slotFour.HideSaveSlot(updateBlackThread);
		yield return base.StartCoroutine(this.gm.timeTool.TimeScaleIndependentWaitForSeconds(0.33f));
		yield return base.StartCoroutine(this.FadeOutCanvasGroup(this.saveProfileScreen, false, true));
		yield break;
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000AD3A8 File Offset: 0x000AB5A8
	private void DisableScreens()
	{
		for (int i = 0; i < this.UICanvas.transform.childCount; i++)
		{
			if (!(this.UICanvas.transform.GetChild(i).name == "PauseMenuScreen"))
			{
				this.UICanvas.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		if (this.achievementsPopupPanel)
		{
			this.achievementsPopupPanel.SetActive(true);
		}
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000AD427 File Offset: 0x000AB627
	private void ShowCanvas(Canvas canvas)
	{
		canvas.gameObject.SetActive(true);
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x000AD435 File Offset: 0x000AB635
	private void HideCanvas(Canvas canvas)
	{
		canvas.gameObject.SetActive(false);
	}

	// Token: 0x06002640 RID: 9792 RVA: 0x000AD443 File Offset: 0x000AB643
	public void ShowCanvasGroup(CanvasGroup cg)
	{
		cg.gameObject.SetActive(true);
		cg.interactable = true;
		cg.alpha = 1f;
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000AD463 File Offset: 0x000AB663
	public void HideCanvasGroup(CanvasGroup cg)
	{
		cg.interactable = false;
		cg.alpha = 0f;
		cg.gameObject.SetActive(false);
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000AD483 File Offset: 0x000AB683
	public IEnumerator FadeInCanvasGroup(CanvasGroup cg, bool alwaysActive = false)
	{
		if (cg == this.mainMenuScreen)
		{
			MenuStyles.Instance.SetInSubMenu(false);
		}
		float loopFailsafe = 0f;
		cg.alpha = 0f;
		if (alwaysActive)
		{
			cg.blocksRaycasts = false;
		}
		else
		{
			cg.gameObject.SetActive(true);
		}
		while (cg.alpha < 1f)
		{
			cg.alpha += Time.unscaledDeltaTime * this.MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha >= 0.95f)
			{
				cg.alpha = 1f;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = 1f;
		cg.interactable = true;
		if (alwaysActive)
		{
			cg.blocksRaycasts = true;
		}
		yield return null;
		yield break;
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000AD4A0 File Offset: 0x000AB6A0
	public IEnumerator FadeInCanvasGroupAlpha(CanvasGroup cg, float endAlpha)
	{
		float loopFailsafe = 0f;
		if (endAlpha > 1f)
		{
			endAlpha = 1f;
		}
		cg.alpha = 0f;
		cg.gameObject.SetActive(true);
		while (cg.alpha < endAlpha - 0.05f)
		{
			cg.alpha += Time.unscaledDeltaTime * this.MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha >= endAlpha - 0.05f)
			{
				cg.alpha = endAlpha;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = endAlpha;
		cg.interactable = true;
		cg.gameObject.SetActive(true);
		yield return null;
		yield break;
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x000AD4BD File Offset: 0x000AB6BD
	public IEnumerator FadeOutCanvasGroup(CanvasGroup cg, bool disable = true, bool stopBlocking = false)
	{
		if (cg == this.mainMenuScreen)
		{
			MenuStyles.Instance.SetInSubMenu(true);
		}
		float loopFailsafe = 0f;
		cg.interactable = false;
		while (cg.alpha > 0.05f)
		{
			cg.alpha -= Time.unscaledDeltaTime * this.MENU_FADE_SPEED;
			loopFailsafe += Time.unscaledDeltaTime;
			if (cg.alpha <= 0.05f || loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		cg.alpha = 0f;
		if (disable)
		{
			cg.gameObject.SetActive(false);
		}
		if (stopBlocking)
		{
			cg.blocksRaycasts = false;
		}
		yield return null;
		yield break;
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x000AD4E1 File Offset: 0x000AB6E1
	public static void FadeOutCanvasGroupInstant(CanvasGroup cg, bool disable = true, bool stopBlocking = false)
	{
		cg.interactable = false;
		cg.alpha = 0f;
		if (disable)
		{
			cg.gameObject.SetActive(false);
		}
		if (stopBlocking)
		{
			cg.blocksRaycasts = false;
		}
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x000AD50E File Offset: 0x000AB70E
	private IEnumerator FadeInSprite(SpriteRenderer sprite)
	{
		while (sprite.color.a < 1f)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + Time.unscaledDeltaTime * this.MENU_FADE_SPEED);
			yield return null;
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
		yield return null;
		yield break;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000AD524 File Offset: 0x000AB724
	private IEnumerator FadeOutSprite(SpriteRenderer sprite)
	{
		while (sprite.color.a > 0f)
		{
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.unscaledDeltaTime * this.MENU_FADE_SPEED);
			yield return null;
		}
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
		yield return null;
		yield break;
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000AD53C File Offset: 0x000AB73C
	public void MakeMenuLean()
	{
		Debug.Log("Making UI menu lean.");
		if (this.saveProfileScreen)
		{
			Object.Destroy(this.saveProfileScreen.gameObject);
			this.saveProfileScreen = null;
			this.saveProfilePreselect = null;
			this.saveProfileTitle = null;
			this.saveProfileControls = null;
			this.saveProfileTopFleur = null;
			this.saveSlots = null;
			this.slotOne = null;
			this.slotTwo = null;
			this.slotThree = null;
			this.slotFour = null;
			this.blackThreadLoopAudio = null;
		}
		if (this.achievementsMenuScreen)
		{
			Object.Destroy(this.achievementsMenuScreen.gameObject);
			this.achievementsMenuScreen = null;
		}
		if (Platform.Current.WillDisplayGraphicsSettings)
		{
			return;
		}
		if (this.videoMenuScreen)
		{
			Object.Destroy(this.videoMenuScreen.gameObject);
			this.videoMenuScreen = null;
		}
		if (this.brightnessMenuScreen)
		{
			Object.Destroy(this.brightnessMenuScreen.gameObject);
			this.brightnessMenuScreen = null;
		}
		if (this.overscanMenuScreen)
		{
			Object.Destroy(this.overscanMenuScreen.gameObject);
			this.overscanMenuScreen = null;
		}
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000AD65C File Offset: 0x000AB85C
	public float FadeScreenIn()
	{
		float num = Platform.Current.EnterSceneWait;
		if (CheatManager.SceneEntryWait >= 0f)
		{
			num = CheatManager.SceneEntryWait;
		}
		this.FadeScreenBlankerTo(0f, this.screenFadeTime, num);
		return this.screenFadeTime + num;
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000AD6A0 File Offset: 0x000AB8A0
	public float FadeScreenOut()
	{
		this.FadeScreenBlankerTo(1f, this.screenFadeTime, 0f);
		return this.screenFadeTime;
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000AD6C0 File Offset: 0x000AB8C0
	private void FadeScreenBlankerTo(float alpha, float duration, float delay)
	{
		if (!this.screenFader)
		{
			return;
		}
		if (this.screenFadeRoutine != null)
		{
			base.StopCoroutine(this.screenFadeRoutine);
		}
		float startAlpha = this.screenFader.alpha;
		this.screenFadeRoutine = this.StartTimerRoutine(delay, duration, delegate(float time)
		{
			this.screenFader.alpha = Mathf.Lerp(startAlpha, alpha, time);
		}, null, null, false);
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000AD731 File Offset: 0x000AB931
	private void SetScreenBlankerAlpha(float alpha)
	{
		if (!this.screenFader)
		{
			return;
		}
		this.screenFader.alpha = alpha;
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000AD74D File Offset: 0x000AB94D
	public void BlankScreen(bool value)
	{
		if (!this.screenFader)
		{
			return;
		}
		this.screenFader.gameObject.SetActive(true);
		this.screenFader.alpha = (value ? 1f : 0f);
	}

	// Token: 0x0400233C RID: 9020
	private GameManager gm;

	// Token: 0x0400233D RID: 9021
	private GameSettings gs;

	// Token: 0x0400233E RID: 9022
	private InputHandler ih;

	// Token: 0x0400233F RID: 9023
	public MenuAudioController uiAudioPlayer;

	// Token: 0x04002340 RID: 9024
	public HollowKnightInputModule inputModule;

	// Token: 0x04002341 RID: 9025
	[Space]
	[SerializeField]
	private CanvasGroup screenFader;

	// Token: 0x04002342 RID: 9026
	[SerializeField]
	private float screenFadeTime = 1f;

	// Token: 0x04002343 RID: 9027
	private Coroutine screenFadeRoutine;

	// Token: 0x04002344 RID: 9028
	[Space]
	public float MENU_FADE_SPEED = 3.2f;

	// Token: 0x04002345 RID: 9029
	private const float MENU_FADE_DELAY = 0.1f;

	// Token: 0x04002346 RID: 9030
	private const float MENU_MODAL_DIMMER_ALPHA = 0.8f;

	// Token: 0x04002347 RID: 9031
	public const float MENU_FADE_ALPHA_TOLERANCE = 0.05f;

	// Token: 0x04002348 RID: 9032
	public const float MENU_FADE_FAILSAFE = 2f;

	// Token: 0x04002349 RID: 9033
	[Header("State")]
	[Space(6f)]
	public UIState uiState;

	// Token: 0x0400234A RID: 9034
	public MainMenuState menuState;

	// Token: 0x0400234B RID: 9035
	[Header("Event System")]
	[Space(6f)]
	public EventSystem eventSystem;

	// Token: 0x0400234C RID: 9036
	[Header("Main Elements")]
	[Space(6f)]
	public Canvas UICanvas;

	// Token: 0x0400234D RID: 9037
	public CanvasGroup modalDimmer;

	// Token: 0x0400234E RID: 9038
	public CanvasScaler canvasScaler;

	// Token: 0x0400234F RID: 9039
	public Canvas GenericMessageCanvas;

	// Token: 0x04002350 RID: 9040
	[Header("Menu Audio")]
	[Space(6f)]
	public AudioMixerSnapshot gameplaySnapshot;

	// Token: 0x04002351 RID: 9041
	public AudioMixerSnapshot menuSilenceSnapshot;

	// Token: 0x04002352 RID: 9042
	public AudioMixerSnapshot menuPauseSnapshot;

	// Token: 0x04002353 RID: 9043
	[Space]
	public AudioMixerSnapshot defaultMusicSnapshot;

	// Token: 0x04002354 RID: 9044
	public AudioMixerSnapshot blackThreadMusicSnapshot;

	// Token: 0x04002355 RID: 9045
	public float musicSnapshotTransitionTime;

	// Token: 0x04002356 RID: 9046
	[Header("Main Menu")]
	[Space(6f)]
	public CanvasGroup mainMenuScreen;

	// Token: 0x04002357 RID: 9047
	public MainMenuOptions mainMenuButtons;

	// Token: 0x04002358 RID: 9048
	public SpriteRenderer gameTitle;

	// Token: 0x04002359 RID: 9049
	public PlayMakerFSM subtitleFSM;

	// Token: 0x0400235A RID: 9050
	[Header("Save Profile Menu")]
	[Space(6f)]
	[SerializeField]
	private PreselectOption saveProfilePreselect;

	// Token: 0x0400235B RID: 9051
	public CanvasGroup saveProfileScreen;

	// Token: 0x0400235C RID: 9052
	public CanvasGroup saveProfileTitle;

	// Token: 0x0400235D RID: 9053
	public CanvasGroup saveProfileControls;

	// Token: 0x0400235E RID: 9054
	public Animator saveProfileTopFleur;

	// Token: 0x0400235F RID: 9055
	public PreselectOption saveSlots;

	// Token: 0x04002360 RID: 9056
	public SaveSlotButton slotOne;

	// Token: 0x04002361 RID: 9057
	public SaveSlotButton slotTwo;

	// Token: 0x04002362 RID: 9058
	public SaveSlotButton slotThree;

	// Token: 0x04002363 RID: 9059
	public SaveSlotButton slotFour;

	// Token: 0x04002364 RID: 9060
	public CheckpointSprite checkpointSprite;

	// Token: 0x04002365 RID: 9061
	public GameObject blackThreadLoopAudio;

	// Token: 0x04002366 RID: 9062
	[Header("Options Menu")]
	[Space(6f)]
	public MenuScreen optionsMenuScreen;

	// Token: 0x04002367 RID: 9063
	[Header("Game Options Menu")]
	[Space(6f)]
	public MenuScreen gameOptionsMenuScreen;

	// Token: 0x04002368 RID: 9064
	public GameMenuOptions gameMenuOptions;

	// Token: 0x04002369 RID: 9065
	public MenuLanguageSetting languageSetting;

	// Token: 0x0400236A RID: 9066
	public MenuSetting backerCreditsSetting;

	// Token: 0x0400236B RID: 9067
	public MenuSetting nativeAchievementsSetting;

	// Token: 0x0400236C RID: 9068
	public MenuSetting controllerRumbleSetting;

	// Token: 0x0400236D RID: 9069
	public MenuSetting hudSetting;

	// Token: 0x0400236E RID: 9070
	public MenuSetting cameraShakeSetting;

	// Token: 0x0400236F RID: 9071
	public MenuSetting switchFrameCapSetting;

	// Token: 0x04002370 RID: 9072
	[Header("Audio Menu")]
	[Space(6f)]
	public MenuScreen audioMenuScreen;

	// Token: 0x04002371 RID: 9073
	public MenuAudioSlider masterSlider;

	// Token: 0x04002372 RID: 9074
	public MenuAudioSlider musicSlider;

	// Token: 0x04002373 RID: 9075
	public MenuAudioSlider soundSlider;

	// Token: 0x04002374 RID: 9076
	public MenuSetting playerVoiceSetting;

	// Token: 0x04002375 RID: 9077
	[Header("Video Menu")]
	[Space(6f)]
	public MenuScreen videoMenuScreen;

	// Token: 0x04002376 RID: 9078
	public VideoMenuOptions videoMenuOptions;

	// Token: 0x04002377 RID: 9079
	public MenuResolutionSetting resolutionOption;

	// Token: 0x04002378 RID: 9080
	public ResolutionCountdownTimer countdownTimer;

	// Token: 0x04002379 RID: 9081
	public MenuOptionHorizontal fullscreenMenuOption;

	// Token: 0x0400237A RID: 9082
	public MenuSetting fullscreenOption;

	// Token: 0x0400237B RID: 9083
	public MenuSetting vsyncOption;

	// Token: 0x0400237C RID: 9084
	public MenuSetting particlesOption;

	// Token: 0x0400237D RID: 9085
	public MenuSetting shadersOption;

	// Token: 0x0400237E RID: 9086
	public MenuDisplaySetting displayOption;

	// Token: 0x0400237F RID: 9087
	public MenuFrameCapSetting frameCapOption;

	// Token: 0x04002380 RID: 9088
	[Header("Controller Options Menu")]
	[Space(6f)]
	public MenuScreen gamepadMenuScreen;

	// Token: 0x04002381 RID: 9089
	public ControllerDetect controllerDetect;

	// Token: 0x04002382 RID: 9090
	[Header("Controller Remap Menu")]
	[Space(6f)]
	public MenuScreen remapGamepadMenuScreen;

	// Token: 0x04002383 RID: 9091
	[Header("Keyboard Options Menu")]
	[Space(6f)]
	public MenuScreen keyboardMenuScreen;

	// Token: 0x04002384 RID: 9092
	[Header("Overscan Setting Menu")]
	[Space(6f)]
	public MenuScreen overscanMenuScreen;

	// Token: 0x04002385 RID: 9093
	public OverscanSetting overscanSetting;

	// Token: 0x04002386 RID: 9094
	[Header("Brightness Setting Menu")]
	[Space(6f)]
	public MenuScreen brightnessMenuScreen;

	// Token: 0x04002387 RID: 9095
	public BrightnessSetting brightnessSetting;

	// Token: 0x04002388 RID: 9096
	[Header("Achievements Menu")]
	[Space(6f)]
	public MenuScreen achievementsMenuScreen;

	// Token: 0x04002389 RID: 9097
	public MenuAchievementsList menuAchievementsList;

	// Token: 0x0400238A RID: 9098
	public GameObject achievementsPopupPanel;

	// Token: 0x0400238B RID: 9099
	[Header("Extras Menu")]
	[Space(6f)]
	public MenuScreen extrasMenuScreen;

	// Token: 0x0400238C RID: 9100
	public MenuScreen extrasContentMenuScreen;

	// Token: 0x0400238D RID: 9101
	[Header("Play Mode Menu")]
	[Space(6f)]
	public MenuScreen playModeMenuScreen;

	// Token: 0x0400238E RID: 9102
	[Header("Pause Menu")]
	[Space(6f)]
	public Animator pauseMenuAnimator;

	// Token: 0x0400238F RID: 9103
	public MenuScreen pauseMenuScreen;

	// Token: 0x04002390 RID: 9104
	[Header("Engage Menu")]
	[Space(6f)]
	public MenuScreen engageMenuScreen;

	// Token: 0x04002391 RID: 9105
	public bool didLeaveEngageMenu;

	// Token: 0x04002392 RID: 9106
	[Header("Prompts")]
	[Space(6f)]
	public MenuScreen quitGamePrompt;

	// Token: 0x04002393 RID: 9107
	public MenuScreen returnMainMenuPrompt;

	// Token: 0x04002394 RID: 9108
	public MenuScreen resolutionPrompt;

	// Token: 0x04002395 RID: 9109
	[Header("Cinematics")]
	[SerializeField]
	private CinematicSkipPopup cinematicSkipPopup;

	// Token: 0x04002396 RID: 9110
	[Header("Button Skins")]
	[Space(6f)]
	public UIButtonSkins uiButtonSkins;

	// Token: 0x04002397 RID: 9111
	[Header("Menu Vibrations")]
	public VibrationDataAsset menuSubmitVibration;

	// Token: 0x04002398 RID: 9112
	public VibrationDataAsset menuCancelVibration;

	// Token: 0x04002399 RID: 9113
	private bool clearSaveFile;

	// Token: 0x0400239A RID: 9114
	private Selectable lastSelected;

	// Token: 0x0400239B RID: 9115
	private bool lastSubmitWasMouse;

	// Token: 0x0400239C RID: 9116
	private bool ignoreUnpause;

	// Token: 0x0400239D RID: 9117
	private bool isFadingMenu;

	// Token: 0x0400239E RID: 9118
	private double startMenuTime;

	// Token: 0x0400239F RID: 9119
	private MenuScreen fadingMenuScreen;

	// Token: 0x040023A0 RID: 9120
	private Coroutine fadingRoutine;

	// Token: 0x040023A1 RID: 9121
	private GraphicRaycaster graphicRaycaster;

	// Token: 0x040023A2 RID: 9122
	private bool permaDeath;

	// Token: 0x040023A3 RID: 9123
	private bool bossRush;

	// Token: 0x040023A4 RID: 9124
	private static readonly int _showProp = Animator.StringToHash("show");

	// Token: 0x040023A5 RID: 9125
	private static readonly int _hideProp = Animator.StringToHash("hide");

	// Token: 0x040023A6 RID: 9126
	private static readonly int _clearProp = Animator.StringToHash("clear");

	// Token: 0x040023A9 RID: 9129
	private bool isStartingNewGame;

	// Token: 0x040023AA RID: 9130
	private bool isGoingBack;

	// Token: 0x040023AB RID: 9131
	private int saveSlot;

	// Token: 0x040023AC RID: 9132
	private AudioSourceFadeControl blackThreadAudioFader;

	// Token: 0x040023AD RID: 9133
	private bool hasBlackThreadAudioFader;

	// Token: 0x040023AE RID: 9134
	private bool initBlackThreadComponents;

	// Token: 0x040023AF RID: 9135
	private bool registeredSaveStoreChangedEvent;

	// Token: 0x040023B0 RID: 9136
	private static UIManager _instance;

	// Token: 0x040023B1 RID: 9137
	private GameObject lastSelectionBeforeFocusLoss;
}
