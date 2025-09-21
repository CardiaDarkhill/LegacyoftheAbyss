using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlobalEnums;
using InControl;
using SharpDX.DirectInput;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020003C7 RID: 967
[RequireComponent(typeof(GameManager))]
public class InputHandler : ManagerSingleton<InputHandler>
{
	// Token: 0x14000066 RID: 102
	// (add) Token: 0x060020AE RID: 8366 RVA: 0x00096300 File Offset: 0x00094500
	// (remove) Token: 0x060020AF RID: 8367 RVA: 0x00096334 File Offset: 0x00094534
	public static event Action<HeroActions> OnUpdateHeroActions;

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x060020B0 RID: 8368 RVA: 0x00096368 File Offset: 0x00094568
	// (remove) Token: 0x060020B1 RID: 8369 RVA: 0x000963A0 File Offset: 0x000945A0
	public event InputHandler.CursorVisibilityChange OnCursorVisibilityChange;

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x060020B2 RID: 8370 RVA: 0x000963D5 File Offset: 0x000945D5
	// (set) Token: 0x060020B3 RID: 8371 RVA: 0x000963DD File Offset: 0x000945DD
	public GamepadType ActiveGamepadAlias { get; set; }

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x060020B4 RID: 8372 RVA: 0x000963E6 File Offset: 0x000945E6
	// (set) Token: 0x060020B5 RID: 8373 RVA: 0x000963EE File Offset: 0x000945EE
	public List<PlayerAction> MappableControllerActions { get; set; }

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x060020B6 RID: 8374 RVA: 0x000963F7 File Offset: 0x000945F7
	// (set) Token: 0x060020B7 RID: 8375 RVA: 0x000963FF File Offset: 0x000945FF
	public List<PlayerAction> MappableKeyboardActions { get; set; }

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x060020B8 RID: 8376 RVA: 0x00096408 File Offset: 0x00094608
	// (set) Token: 0x060020B9 RID: 8377 RVA: 0x00096410 File Offset: 0x00094610
	public bool PauseAllowed { get; private set; }

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x060020BA RID: 8378 RVA: 0x00096419 File Offset: 0x00094619
	// (set) Token: 0x060020BB RID: 8379 RVA: 0x00096421 File Offset: 0x00094621
	public SkipPromptMode SkipMode { get; private set; }

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x060020BC RID: 8380 RVA: 0x0009642C File Offset: 0x0009462C
	// (remove) Token: 0x060020BD RID: 8381 RVA: 0x00096464 File Offset: 0x00094664
	public event InputHandler.ActiveControllerSwitch RefreshActiveControllerEvent;

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x060020BE RID: 8382 RVA: 0x0009649C File Offset: 0x0009469C
	public bool WasSkipButtonPressed
	{
		get
		{
			HeroActions heroActions = this.inputActions;
			return Platform.Current.GetMenuAction(heroActions, false, false) != Platform.MenuActions.None || heroActions.Pause.WasPressed || InventoryPaneInput.IsInventoryButtonPressed(heroActions) || heroActions.QuickCast.WasPressed || heroActions.SuperDash.WasPressed || heroActions.Dash.WasPressed || heroActions.QuickMap.WasPressed;
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x060020BF RID: 8383 RVA: 0x00096508 File Offset: 0x00094708
	// (set) Token: 0x060020C0 RID: 8384 RVA: 0x00096510 File Offset: 0x00094710
	public bool ForceDreamNailRePress { get; set; }

	// Token: 0x060020C1 RID: 8385 RVA: 0x0009651C File Offset: 0x0009471C
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.gm = base.GetComponent<GameManager>();
		this.gs = this.gm.gameSettings;
		this.inputActions = new HeroActions();
		this.acceptingInput = true;
		this.PauseAllowed = true;
		this.SkipMode = SkipPromptMode.NOT_SKIPPABLE;
		SaveDataUpgradeHandler.UpgradeSystemData<InputHandler>(this);
		this.buttonQueueTimers = new float[ArrayForEnumAttribute.GetArrayLength(typeof(HeroActionButton))];
		return true;
	}

	// Token: 0x060020C2 RID: 8386 RVA: 0x00096598 File Offset: 0x00094798
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		this.playerData = this.gm.playerData;
		if (!InputManager.IsSetup)
		{
			InputManager.OnSetupCompleted += this.<OnStart>g__Setup|67_0;
		}
		else
		{
			this.<OnStart>g__Setup|67_0();
		}
		InputManager.OnDeviceAttached += this.ControllerAttached;
		InputManager.OnActiveDeviceChanged += this.ControllerActivated;
		InputManager.OnDeviceDetached += this.ControllerDetached;
		Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
		return true;
	}

	// Token: 0x060020C3 RID: 8387 RVA: 0x00096633 File Offset: 0x00094833
	protected override void Awake()
	{
		base.Awake();
		this.OnAwake();
	}

	// Token: 0x060020C4 RID: 8388 RVA: 0x00096642 File Offset: 0x00094842
	public void Start()
	{
		this.OnStart();
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x0009664C File Offset: 0x0009484C
	protected override void OnDestroy()
	{
		base.OnDestroy();
		InputManager.OnDeviceAttached -= this.ControllerAttached;
		InputManager.OnActiveDeviceChanged -= this.ControllerActivated;
		InputManager.OnDeviceDetached -= this.ControllerDetached;
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
		this.inputActions.Destroy();
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000966B0 File Offset: 0x000948B0
	public void SceneInit()
	{
		this.isTitleScreenScene = this.gm.IsTitleScreenScene();
		this.isMenuScene = this.gm.IsMenuScene();
		if (this.gm.IsStagTravelScene())
		{
			this.isStagTravelScene = true;
			this.stagLockoutActive = true;
			base.Invoke("UnlockStagInput", 1.2f);
			return;
		}
		this.isStagTravelScene = false;
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x00096712 File Offset: 0x00094912
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.LoadSavedInputBindings();
		}
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x00096720 File Offset: 0x00094920
	private void LoadSavedInputBindings()
	{
		if (!this.hasSetup)
		{
			return;
		}
		try
		{
			this.doingSoftReset = true;
			foreach (PlayerAction playerAction in this.inputActions.Actions)
			{
				playerAction.ClearBindings();
			}
			this.SetupNonMappableBindings();
			this.gs.LoadKeyboardSettings();
			this.MapKeyboardLayoutFromGameSettings();
			if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.IsAttached)
			{
				this.ControllerActivated(InputManager.ActiveDevice);
			}
			this.SetupMappableKeyboardBindingsList();
		}
		finally
		{
			this.doingSoftReset = false;
			Action<HeroActions> onUpdateHeroActions = InputHandler.OnUpdateHeroActions;
			if (onUpdateHeroActions != null)
			{
				onUpdateHeroActions(this.inputActions);
			}
		}
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000967E8 File Offset: 0x000949E8
	private void SetCursorVisible(bool value)
	{
		InputHandler.SetCursorEnabled(value);
		if (this.OnCursorVisibilityChange != null)
		{
			this.OnCursorVisibilityChange(value);
		}
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x00096804 File Offset: 0x00094A04
	private static void SetCursorEnabled(bool isEnabled)
	{
		if (isEnabled && Platform.Current.IsMouseSupported && !DemoHelper.IsExhibitionMode)
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			return;
		}
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x00096838 File Offset: 0x00094A38
	private void Update()
	{
		if (this.isTitleScreenScene)
		{
			this.SetCursorVisible(false);
		}
		else if (!this.isMenuScene)
		{
			if (!this.gm.isPaused)
			{
				this.SetCursorVisible(false);
			}
			else
			{
				this.SetCursorVisible(!this.controllerPressed);
			}
		}
		else
		{
			this.SetCursorVisible(!this.controllerPressed);
		}
		this.UpdateActiveController();
		this.UpdateButtonQueueing();
		if (this.acceptingInput)
		{
			if (this.gm.GameState == GameState.PLAYING)
			{
				this.PlayingInput();
			}
			else if (this.gm.GameState == GameState.CUTSCENE)
			{
				if (this.isStagTravelScene)
				{
					if (!this.stagLockoutActive)
					{
						this.StagCutsceneInput();
					}
				}
				else
				{
					this.CutsceneInput();
				}
			}
			if (this.inputActions.Pause.WasPressed && this.PauseAllowed && !this.playerData.disablePause)
			{
				GameState gameState = this.gm.GameState;
				if (gameState == GameState.PLAYING || gameState == GameState.PAUSED)
				{
					base.StartCoroutine(this.gm.PauseGameToggle(true));
				}
			}
		}
		if (this.controllerPressed)
		{
			if (Mathf.Abs(Input.GetAxisRaw("mouse x")) > 0.1f)
			{
				this.controllerPressed = false;
				return;
			}
		}
		else if (this.inputActions.ActiveDevice.AnyButtonIsPressed || this.inputActions.MoveVector.WasPressed)
		{
			this.controllerPressed = true;
		}
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x0009698C File Offset: 0x00094B8C
	private void UpdateButtonQueueing()
	{
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		for (int i = 0; i < this.buttonQueueTimers.Length; i++)
		{
			HeroActionButton actionButtonType = (HeroActionButton)i;
			if (this.ActionButtonToPlayerAction(actionButtonType).WasPressed)
			{
				this.buttonQueueTimers[i] = 0.1f;
			}
			else
			{
				float num = this.buttonQueueTimers[i];
				if (num > 0f)
				{
					num -= Time.unscaledDeltaTime;
					this.buttonQueueTimers[i] = num;
				}
			}
		}
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000969FC File Offset: 0x00094BFC
	public bool GetWasButtonPressedQueued(HeroActionButton heroAction, bool consume)
	{
		if (this.buttonQueueTimers[(int)heroAction] <= 0f)
		{
			return false;
		}
		if (consume)
		{
			this.buttonQueueTimers[(int)heroAction] = 0f;
		}
		return true;
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x00096A2D File Offset: 0x00094C2D
	private void ControllerAttached(InputDevice inputDevice)
	{
		if (inputDevice.IsUnknown)
		{
			return;
		}
		this.gamepadState = GamepadState.ATTACHED;
		this.gameController = inputDevice;
		Debug.LogFormat("Game controller {0} attached", new object[]
		{
			inputDevice.Name
		});
		this.SetActiveGamepadType(inputDevice);
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x00096A66 File Offset: 0x00094C66
	private void ControllerActivated(InputDevice inputDevice)
	{
		if (inputDevice.IsUnknown)
		{
			return;
		}
		this.gamepadState = GamepadState.ACTIVATED;
		this.gameController = inputDevice;
		this.SetActiveGamepadType(inputDevice);
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x00096A88 File Offset: 0x00094C88
	private void ControllerDetached(InputDevice inputDevice)
	{
		if (this.gameController != inputDevice)
		{
			return;
		}
		this.gamepadState = GamepadState.DETACHED;
		this.activeGamepadType = GamepadType.NONE;
		this.ActiveGamepadAlias = GamepadType.NONE;
		this.gameController = InputDevice.Null;
		Debug.LogFormat("Game controller {0} detached.", new object[]
		{
			inputDevice.Name
		});
		UIManager instance = UIManager.instance;
		if (instance.uiButtonSkins.listeningButton != null)
		{
			instance.uiButtonSkins.listeningButton.StopActionListening();
			instance.uiButtonSkins.listeningButton.AbortRebind();
			instance.uiButtonSkins.RefreshButtonMappings();
		}
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x00096B1C File Offset: 0x00094D1C
	private void PlayingInput()
	{
		if (CheatManager.IsOpen)
		{
			return;
		}
		if (this.ForceDreamNailRePress && !this.inputActions.DreamNail.IsPressed)
		{
			this.ForceDreamNailRePress = false;
		}
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x00096B48 File Offset: 0x00094D48
	private void CutsceneInput()
	{
		if (this.skippingCutscene)
		{
			return;
		}
		if (!Input.anyKeyDown && (this.gameController == null || !this.gameController.AnyButton.WasPressed))
		{
			return;
		}
		switch (this.SkipMode)
		{
		case SkipPromptMode.SKIP_PROMPT:
			if (!this.readyToSkipCutscene)
			{
				this.gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Skip);
				this.readyToSkipCutscene = true;
				base.CancelInvoke("StopCutsceneInput");
				base.Invoke("StopCutsceneInput", 3f * Time.timeScale);
				this.skipCooldownTime = Time.timeAsDouble + 0.30000001192092896;
				return;
			}
			if (Time.timeAsDouble < this.skipCooldownTime)
			{
				return;
			}
			base.CancelInvoke("StopCutsceneInput");
			this.readyToSkipCutscene = false;
			this.skippingCutscene = true;
			this.gm.SkipCutscene();
			return;
		case SkipPromptMode.SKIP_INSTANT:
			this.skippingCutscene = true;
			this.gm.SkipCutscene();
			return;
		case SkipPromptMode.NOT_SKIPPABLE:
			return;
		case SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING:
			this.gm.ui.ShowCutscenePrompt(CinematicSkipPopup.Texts.Loading);
			base.CancelInvoke("StopCutsceneInput");
			base.Invoke("StopCutsceneInput", 3f * Time.timeScale);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x00096C78 File Offset: 0x00094E78
	private void StagCutsceneInput()
	{
		if (Input.anyKeyDown || this.gameController.AnyButton.WasPressed)
		{
			this.gm.SkipCutscene();
		}
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x00096C9E File Offset: 0x00094E9E
	public void AttachHeroController(HeroController heroController)
	{
		this.heroCtrl = heroController;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x00096CA7 File Offset: 0x00094EA7
	public void StopAcceptingInput()
	{
		this.acceptingInput = false;
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x00096CB0 File Offset: 0x00094EB0
	public void StartAcceptingInput()
	{
		this.acceptingInput = true;
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x00096CB9 File Offset: 0x00094EB9
	public void PreventPause()
	{
		this.PauseAllowed = false;
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x00096CC2 File Offset: 0x00094EC2
	public void AllowPause()
	{
		this.PauseAllowed = true;
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x00096CCC File Offset: 0x00094ECC
	public void UpdateActiveController()
	{
		if (this.lastActiveController != this.inputActions.LastInputType || this.lastInputDeviceStyle != this.inputActions.LastDeviceStyle)
		{
			this.lastActiveController = this.inputActions.LastInputType;
			this.lastInputDeviceStyle = this.inputActions.LastDeviceStyle;
			if (this.RefreshActiveControllerEvent != null)
			{
				this.RefreshActiveControllerEvent();
			}
		}
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x00096D34 File Offset: 0x00094F34
	public void StopUIInput()
	{
		this.acceptingInput = false;
		EventSystem.current.sendNavigationEvents = false;
		UIManager.instance.inputModule.allowMouseInput = false;
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x00096D58 File Offset: 0x00094F58
	public void StartUIInput()
	{
		this.acceptingInput = true;
		EventSystem.current.sendNavigationEvents = true;
		UIManager.instance.inputModule.allowMouseInput = true;
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x00096D7C File Offset: 0x00094F7C
	public void StopMouseInput()
	{
		UIManager.instance.inputModule.allowMouseInput = false;
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x00096D8E File Offset: 0x00094F8E
	public void EnableMouseInput()
	{
		UIManager.instance.inputModule.allowMouseInput = true;
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x00096DA0 File Offset: 0x00094FA0
	public void SetSkipMode(SkipPromptMode newMode)
	{
		if (newMode == SkipPromptMode.NOT_SKIPPABLE)
		{
			this.StopAcceptingInput();
		}
		else if (newMode == SkipPromptMode.SKIP_PROMPT)
		{
			this.readyToSkipCutscene = false;
			this.StartAcceptingInput();
		}
		else if (newMode == SkipPromptMode.SKIP_INSTANT)
		{
			this.StartAcceptingInput();
		}
		else if (newMode == SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING)
		{
			this.readyToSkipCutscene = false;
			this.StartAcceptingInput();
		}
		this.SkipMode = newMode;
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x00096DEF File Offset: 0x00094FEF
	public void RefreshPlayerData()
	{
		this.playerData = PlayerData.instance;
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x00096DFC File Offset: 0x00094FFC
	public void ResetDefaultKeyBindings()
	{
		this.inputActions.Jump.ClearBindings();
		this.inputActions.Attack.ClearBindings();
		this.inputActions.Dash.ClearBindings();
		this.inputActions.Cast.ClearBindings();
		this.inputActions.SuperDash.ClearBindings();
		this.inputActions.DreamNail.ClearBindings();
		this.inputActions.QuickMap.ClearBindings();
		this.inputActions.OpenInventory.ClearBindings();
		this.inputActions.OpenInventoryMap.ClearBindings();
		this.inputActions.OpenInventoryJournal.ClearBindings();
		this.inputActions.OpenInventoryTools.ClearBindings();
		this.inputActions.OpenInventoryQuests.ClearBindings();
		this.inputActions.QuickCast.ClearBindings();
		this.inputActions.Up.ClearBindings();
		this.inputActions.Down.ClearBindings();
		this.inputActions.Left.ClearBindings();
		this.inputActions.Right.ClearBindings();
		this.inputActions.Taunt.ClearBindings();
		this.MapDefaultKeyboardLayout();
		this.gs.jumpKey = InControl.Key.Z.ToString();
		this.gs.attackKey = InControl.Key.X.ToString();
		this.gs.dashKey = InControl.Key.C.ToString();
		this.gs.castKey = InControl.Key.A.ToString();
		this.gs.superDashKey = InControl.Key.S.ToString();
		this.gs.dreamNailKey = InControl.Key.D.ToString();
		this.gs.quickMapKey = InControl.Key.Tab.ToString();
		this.gs.inventoryKey = InControl.Key.I.ToString();
		this.gs.inventoryMapKey = InControl.Key.M.ToString();
		this.gs.inventoryToolsKey = InControl.Key.Q.ToString();
		this.gs.inventoryJournalKey = InControl.Key.J.ToString();
		this.gs.inventoryQuestsKey = InControl.Key.T.ToString();
		this.gs.quickCastKey = InControl.Key.F.ToString();
		this.gs.tauntKey = InControl.Key.V.ToString();
		this.gs.upKey = InControl.Key.UpArrow.ToString();
		this.gs.downKey = InControl.Key.DownArrow.ToString();
		this.gs.leftKey = InControl.Key.LeftArrow.ToString();
		this.gs.rightKey = InControl.Key.RightArrow.ToString();
		this.gs.SaveKeyboardSettings();
		if (this.gameController != InputDevice.Null)
		{
			this.SetActiveGamepadType(this.gameController);
		}
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x0009713C File Offset: 0x0009533C
	public void ResetDefaultControllerButtonBindings()
	{
		this.inputActions.Jump.ClearBindings();
		this.inputActions.Attack.ClearBindings();
		this.inputActions.Dash.ClearBindings();
		this.inputActions.Cast.ClearBindings();
		this.inputActions.SuperDash.ClearBindings();
		this.inputActions.DreamNail.ClearBindings();
		this.inputActions.QuickMap.ClearBindings();
		this.inputActions.QuickCast.ClearBindings();
		this.inputActions.Taunt.ClearBindings();
		this.MapKeyboardLayoutFromGameSettings();
		this.gs.ResetGamepadSettings(this.activeGamepadType);
		this.gs.SaveGamepadSettings(this.activeGamepadType);
		this.MapControllerButtons(this.activeGamepadType);
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x00097210 File Offset: 0x00095410
	public void ResetAllControllerButtonBindings()
	{
		int num = Enum.GetNames(typeof(GamepadType)).Length;
		for (int i = 0; i < num; i++)
		{
			GamepadType gamepadType = (GamepadType)i;
			if (this.gs.LoadGamepadSettings(gamepadType))
			{
				this.gs.ResetGamepadSettings(gamepadType);
				this.gs.SaveGamepadSettings(gamepadType);
			}
		}
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x00097264 File Offset: 0x00095464
	public void SendKeyBindingsToGameSettings()
	{
		this.gs.jumpKey = this.GetKeyBindingForAction(this.inputActions.Jump).ToString();
		this.gs.attackKey = this.GetKeyBindingForAction(this.inputActions.Attack).ToString();
		this.gs.dashKey = this.GetKeyBindingForAction(this.inputActions.Dash).ToString();
		this.gs.castKey = this.GetKeyBindingForAction(this.inputActions.Cast).ToString();
		this.gs.superDashKey = this.GetKeyBindingForAction(this.inputActions.SuperDash).ToString();
		this.gs.dreamNailKey = this.GetKeyBindingForAction(this.inputActions.DreamNail).ToString();
		this.gs.quickMapKey = this.GetKeyBindingForAction(this.inputActions.QuickMap).ToString();
		this.gs.inventoryKey = this.GetKeyBindingForAction(this.inputActions.OpenInventory).ToString();
		this.gs.inventoryMapKey = this.GetKeyBindingForAction(this.inputActions.OpenInventoryMap).ToString();
		this.gs.inventoryJournalKey = this.GetKeyBindingForAction(this.inputActions.OpenInventoryJournal).ToString();
		this.gs.inventoryToolsKey = this.GetKeyBindingForAction(this.inputActions.OpenInventoryTools).ToString();
		this.gs.inventoryQuestsKey = this.GetKeyBindingForAction(this.inputActions.OpenInventoryQuests).ToString();
		this.gs.upKey = this.GetKeyBindingForAction(this.inputActions.Up).ToString();
		this.gs.downKey = this.GetKeyBindingForAction(this.inputActions.Down).ToString();
		this.gs.leftKey = this.GetKeyBindingForAction(this.inputActions.Left).ToString();
		this.gs.rightKey = this.GetKeyBindingForAction(this.inputActions.Right).ToString();
		this.gs.quickCastKey = this.GetKeyBindingForAction(this.inputActions.QuickCast).ToString();
		this.gs.tauntKey = this.GetKeyBindingForAction(this.inputActions.Taunt).ToString();
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x00097568 File Offset: 0x00095768
	public void SendButtonBindingsToGameSettings()
	{
		this.gs.controllerMapping.jump = this.GetButtonBindingForAction(this.inputActions.Jump);
		this.gs.controllerMapping.attack = this.GetButtonBindingForAction(this.inputActions.Attack);
		this.gs.controllerMapping.dash = this.GetButtonBindingForAction(this.inputActions.Dash);
		this.gs.controllerMapping.cast = this.GetButtonBindingForAction(this.inputActions.Cast);
		this.gs.controllerMapping.superDash = this.GetButtonBindingForAction(this.inputActions.SuperDash);
		this.gs.controllerMapping.dreamNail = this.GetButtonBindingForAction(this.inputActions.DreamNail);
		this.gs.controllerMapping.quickMap = this.GetButtonBindingForAction(this.inputActions.QuickMap);
		this.gs.controllerMapping.quickCast = this.GetButtonBindingForAction(this.inputActions.QuickCast);
		this.gs.controllerMapping.taunt = this.GetButtonBindingForAction(this.inputActions.Taunt);
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000976A0 File Offset: 0x000958A0
	public void MapControllerButtons(GamepadType gamePadType)
	{
		this.inputActions.Reset();
		this.MapKeyboardLayoutFromGameSettings();
		if (!this.gs.LoadGamepadSettings(gamePadType))
		{
			this.gs.ResetGamepadSettings(gamePadType);
		}
		this.inputActions.Jump.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.jump));
		this.inputActions.Attack.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.attack));
		this.inputActions.Dash.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.dash));
		this.inputActions.Cast.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.cast));
		this.inputActions.SuperDash.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.superDash));
		this.inputActions.DreamNail.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.dreamNail));
		this.inputActions.QuickMap.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.quickMap));
		this.inputActions.QuickCast.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.quickCast));
		this.inputActions.Taunt.AddBinding(new DeviceBindingSource(this.gs.controllerMapping.taunt));
		if (gamePadType == GamepadType.XBOX_360)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Back));
			return;
		}
		if (gamePadType == GamepadType.PS4)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.TouchPadButton));
			this.inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Options));
			this.inputActions.SwipeInventoryMap.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Up));
			this.inputActions.SwipeInventoryJournal.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Down));
			this.inputActions.SwipeInventoryTools.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Left));
			this.inputActions.SwipeInventoryQuests.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Right));
			return;
		}
		if (gamePadType == GamepadType.PS5)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.TouchPadButton));
			this.inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Options));
			this.inputActions.SwipeInventoryMap.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Up));
			this.inputActions.SwipeInventoryJournal.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Down));
			this.inputActions.SwipeInventoryTools.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Left));
			this.inputActions.SwipeInventoryQuests.AddBinding(new PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe.Right));
			return;
		}
		GamepadType gamepadType = this.activeGamepadType;
		if (gamepadType == GamepadType.XBOX_ONE || gamepadType == GamepadType.XBOX_SERIES_X)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.View));
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Back));
			this.inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Menu));
			return;
		}
		if (gamePadType == GamepadType.PS3_WIN)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Select));
			return;
		}
		gamepadType = this.activeGamepadType;
		if (gamepadType == GamepadType.SWITCH_JOYCON_DUAL || gamepadType == GamepadType.SWITCH_PRO_CONTROLLER || gamepadType == GamepadType.SWITCH2_JOYCON_DUAL || gamepadType == GamepadType.SWITCH2_PRO_CONTROLLER)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Minus));
			this.inputActions.Pause.AddDefaultBinding(new DeviceBindingSource(InputControlType.Plus));
			return;
		}
		if (gamePadType == GamepadType.UNKNOWN)
		{
			this.inputActions.OpenInventory.AddBinding(new DeviceBindingSource(InputControlType.Select));
		}
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x00097A54 File Offset: 0x00095C54
	public void RemapUiButtons()
	{
		this.inputActions.MenuSubmit.ResetBindings();
		this.inputActions.MenuCancel.ResetBindings();
		this.inputActions.MenuExtra.ResetBindings();
		this.inputActions.MenuSuper.ResetBindings();
		this.inputActions.PaneLeft.ResetBindings();
		this.inputActions.PaneRight.ResetBindings();
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x00097AC4 File Offset: 0x00095CC4
	public PlayerAction ActionButtonToPlayerAction(HeroActionButton actionButtonType)
	{
		this.OnStart();
		switch (actionButtonType)
		{
		case HeroActionButton.JUMP:
			return this.inputActions.Jump;
		case HeroActionButton.ATTACK:
			return this.inputActions.Attack;
		case HeroActionButton.DASH:
			return this.inputActions.Dash;
		case HeroActionButton.SUPER_DASH:
			return this.inputActions.SuperDash;
		case HeroActionButton.CAST:
			return this.inputActions.Cast;
		case HeroActionButton.TAUNT:
			return this.inputActions.Taunt;
		case HeroActionButton.QUICK_MAP:
			return this.inputActions.QuickMap;
		case HeroActionButton.INVENTORY:
			return this.inputActions.OpenInventory;
		case HeroActionButton.MENU_SUBMIT:
			return this.inputActions.MenuSubmit;
		case HeroActionButton.MENU_CANCEL:
			return this.inputActions.MenuCancel;
		case HeroActionButton.DREAM_NAIL:
			return this.inputActions.DreamNail;
		case HeroActionButton.UP:
			return this.inputActions.Up;
		case HeroActionButton.DOWN:
			return this.inputActions.Down;
		case HeroActionButton.LEFT:
			return this.inputActions.Left;
		case HeroActionButton.RIGHT:
			return this.inputActions.Right;
		case HeroActionButton.QUICK_CAST:
			return this.inputActions.QuickCast;
		case HeroActionButton.MENU_PANE_LEFT:
			return this.inputActions.PaneLeft;
		case HeroActionButton.MENU_PANE_RIGHT:
			return this.inputActions.PaneRight;
		case HeroActionButton.MENU_EXTRA:
			return this.inputActions.MenuExtra;
		case HeroActionButton.MENU_SUPER:
			return this.inputActions.MenuSuper;
		case HeroActionButton.INVENTORY_MAP:
			return this.inputActions.OpenInventoryMap;
		case HeroActionButton.INVENTORY_JOURNAL:
			return this.inputActions.OpenInventoryJournal;
		case HeroActionButton.INVENTORY_TOOLS:
			return this.inputActions.OpenInventoryTools;
		case HeroActionButton.INVENTORY_QUESTS:
			return this.inputActions.OpenInventoryQuests;
		default:
			Debug.Log("No PlayerAction could be matched to HeroActionButton: " + actionButtonType.ToString());
			return null;
		}
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x00097C80 File Offset: 0x00095E80
	public InputHandler.KeyOrMouseBinding GetKeyBindingForAction(PlayerAction action)
	{
		if (!this.inputActions.Actions.Contains(action))
		{
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
		int count = action.Bindings.Count;
		if (count == 0)
		{
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
		if (count == 1)
		{
			BindingSource bindingSource = action.Bindings[0];
			if (bindingSource.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource.BindingSourceType == BindingSourceType.MouseBindingSource)
			{
				return this.GetKeyBindingForActionBinding(action, action.Bindings[0]);
			}
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
		else
		{
			if (count > 1)
			{
				foreach (BindingSource bindingSource2 in action.Bindings)
				{
					if (bindingSource2.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource2.BindingSourceType == BindingSourceType.MouseBindingSource)
					{
						InputHandler.KeyOrMouseBinding keyBindingForActionBinding = this.GetKeyBindingForActionBinding(action, bindingSource2);
						if (!InputHandler.KeyOrMouseBinding.IsNone(keyBindingForActionBinding))
						{
							return keyBindingForActionBinding;
						}
					}
				}
				return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
			}
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x00097D74 File Offset: 0x00095F74
	private InputHandler.KeyOrMouseBinding GetKeyBindingForActionBinding(PlayerAction action, BindingSource bindingSource)
	{
		KeyBindingSource keyBindingSource = bindingSource as KeyBindingSource;
		if (keyBindingSource != null)
		{
			if (keyBindingSource.Control.IncludeCount == 0)
			{
				Debug.LogErrorFormat("This action has no key mapped but registered a key binding. ({0})", new object[]
				{
					action.Name
				});
				return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
			}
			if (keyBindingSource.Control.IncludeCount == 1)
			{
				return new InputHandler.KeyOrMouseBinding(keyBindingSource.Control.GetInclude(0));
			}
			int includeCount = keyBindingSource.Control.IncludeCount;
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
		else
		{
			MouseBindingSource mouseBindingSource = bindingSource as MouseBindingSource;
			if (mouseBindingSource != null)
			{
				return new InputHandler.KeyOrMouseBinding(mouseBindingSource.Control);
			}
			Debug.LogErrorFormat("Keybinding Error - Action: {0} returned a null binding.", new object[]
			{
				action.Name
			});
			return new InputHandler.KeyOrMouseBinding(InControl.Key.None);
		}
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x00097E3C File Offset: 0x0009603C
	public InputControlType GetButtonBindingForAction(PlayerAction action)
	{
		if (!this.inputActions.Actions.Contains(action))
		{
			return InputControlType.None;
		}
		if (action.Bindings.Count > 0)
		{
			foreach (BindingSource bindingSource in action.Bindings)
			{
				if (bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource)
				{
					DeviceBindingSource deviceBindingSource = bindingSource as DeviceBindingSource;
					if (deviceBindingSource != null)
					{
						return deviceBindingSource.Control;
					}
				}
			}
			return InputControlType.None;
		}
		return InputControlType.None;
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x00097ECC File Offset: 0x000960CC
	public PlayerAction GetActionForMappableControllerButton(InputControlType button)
	{
		foreach (PlayerAction playerAction in this.MappableControllerActions)
		{
			if (this.GetButtonBindingForAction(playerAction) == button)
			{
				return playerAction;
			}
		}
		return null;
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x00097F2C File Offset: 0x0009612C
	public PlayerAction GetActionForDefaultControllerButton(InputControlType button)
	{
		InputDevice activeDevice = InputManager.ActiveDevice;
		if (activeDevice != null)
		{
			activeDevice.GetControl(button);
		}
		return null;
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x00097F44 File Offset: 0x00096144
	public void PrintMappings(PlayerAction action)
	{
		if (this.inputActions.Actions.Contains(action))
		{
			using (IEnumerator<BindingSource> enumerator = action.Bindings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BindingSource bindingSource = enumerator.Current;
					if (bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource)
					{
						DeviceBindingSource deviceBindingSource = (DeviceBindingSource)bindingSource;
						Debug.LogFormat("{0} : {1} of type {2}", new object[]
						{
							action.Name,
							deviceBindingSource.Control,
							bindingSource.BindingSourceType
						});
					}
					else
					{
						Debug.LogFormat("{0} : {1} of type {2}", new object[]
						{
							action.Name,
							bindingSource.Name,
							bindingSource.BindingSourceType
						});
					}
				}
				return;
			}
		}
		Debug.Log("Action Not Found");
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x00098024 File Offset: 0x00096224
	public string ActionButtonLocalizedKey(PlayerAction action)
	{
		return this.ActionButtonLocalizedKey(action.Name);
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x00098034 File Offset: 0x00096234
	public string ActionButtonLocalizedKey(string actionName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(actionName);
		if (num <= 1750693397U)
		{
			if (num <= 709991534U)
			{
				if (num != 236909357U)
				{
					if (num != 434653995U)
					{
						if (num == 709991534U)
						{
							if (actionName == "Cast")
							{
								return "BUTTON_CAST";
							}
						}
					}
					else if (actionName == "Quick Cast")
					{
						return "BUTTON_QCAST";
					}
				}
				else if (actionName == "Jump")
				{
					return "BUTTON_JUMP";
				}
			}
			else if (num != 1157218093U)
			{
				if (num != 1589474080U)
				{
					if (num == 1750693397U)
					{
						if (actionName == "Dash")
						{
							return "BUTTON_DASH";
						}
					}
				}
				else if (actionName == "Super Dash")
				{
					return "BUTTON_SUPER_DASH";
				}
			}
			else if (actionName == "Pause")
			{
				return "BUTTON_PAUSE";
			}
		}
		else if (num <= 2343121693U)
		{
			if (num != 1771441078U)
			{
				if (num != 2133600820U)
				{
					if (num == 2343121693U)
					{
						if (actionName == "Attack")
						{
							return "BUTTON_ATTACK";
						}
					}
				}
				else if (actionName == "Move")
				{
					return "BUTTON_MOVE";
				}
			}
			else if (actionName == "Look")
			{
				return "BUTTON_LOOK";
			}
		}
		else if (num != 3332570256U)
		{
			if (num != 3369262303U)
			{
				if (num == 4076831930U)
				{
					if (actionName == "Quick Map")
					{
						return "BUTTON_MAP";
					}
				}
			}
			else if (actionName == "Inventory")
			{
				return "BUTTON_INVENTORY";
			}
		}
		else if (actionName == "Dream Nail")
		{
			return "BUTTON_DREAM_NAIL";
		}
		Debug.Log("IH Unknown Key for action: " + actionName);
		return "unknownkey";
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x0009822A File Offset: 0x0009642A
	private void StopCutsceneInput()
	{
		this.gm.ui.HideCutscenePrompt(false, delegate
		{
			this.readyToSkipCutscene = false;
		});
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x00098249 File Offset: 0x00096449
	private void UnlockStagInput()
	{
		this.stagLockoutActive = false;
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x00098252 File Offset: 0x00096452
	private IEnumerator SetupGamepadUIInputActions()
	{
		if (this.gm.ui.menuState == MainMenuState.GAMEPAD_MENU)
		{
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			yield return new WaitForEndOfFrame();
		}
		Platform.AcceptRejectInputStyles acceptRejectInputStyle = Platform.Current.AcceptRejectInputStyle;
		if (acceptRejectInputStyle != Platform.AcceptRejectInputStyles.NonJapaneseStyle)
		{
			if (acceptRejectInputStyle != Platform.AcceptRejectInputStyles.JapaneseStyle)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.inputActions.MenuSubmit.AddDefaultBinding(InputControlType.Action2);
			this.inputActions.MenuCancel.AddDefaultBinding(InputControlType.Action1);
		}
		else
		{
			this.inputActions.MenuSubmit.AddDefaultBinding(InputControlType.Action1);
			this.inputActions.MenuCancel.AddDefaultBinding(InputControlType.Action2);
		}
		this.inputActions.MenuExtra.AddDefaultBinding(InputControlType.Action3);
		this.inputActions.MenuSuper.AddDefaultBinding(InputControlType.Action4);
		yield break;
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x00098264 File Offset: 0x00096464
	private void RemoveGamepadUiInputActions()
	{
		this.inputActions.MenuSubmit.RemoveBinding(new DeviceBindingSource(InputControlType.Action1));
		this.inputActions.MenuSubmit.RemoveBinding(new DeviceBindingSource(InputControlType.Action2));
		this.inputActions.MenuCancel.RemoveBinding(new DeviceBindingSource(InputControlType.Action1));
		this.inputActions.MenuCancel.RemoveBinding(new DeviceBindingSource(InputControlType.Action2));
		this.inputActions.MenuExtra.RemoveBinding(new DeviceBindingSource(InputControlType.Action3));
		this.inputActions.MenuSuper.RemoveBinding(new DeviceBindingSource(InputControlType.Action4));
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000982FB File Offset: 0x000964FB
	private void DestroyCurrentActionSet()
	{
		this.inputActions.Destroy();
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x00098308 File Offset: 0x00096508
	public void SetActiveGamepadType(InputDevice inputDevice)
	{
		this.ActiveGamepadAlias = GamepadType.NONE;
		if (this.gamepadState != GamepadState.DETACHED)
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
			{
				InputDeviceStyle deviceStyle = inputDevice.DeviceStyle;
				switch (deviceStyle)
				{
				case InputDeviceStyle.Xbox360:
					this.activeGamepadType = GamepadType.XBOX_360;
					goto IL_17D;
				case InputDeviceStyle.XboxOne:
				{
					GamepadType gamepadType = GamepadType.XBOX_ONE;
					if (InputManager.EnableXInput || InputManager.NativeInputEnableXInput)
					{
						int num = InputManager.Devices.IndexOf(inputDevice);
						List<DeviceInstance> devices = DirectInputManager.GetDevices();
						if (devices.Count > 0 && devices[num % devices.Count].InstanceName.Contains("xbox 360", StringComparison.InvariantCultureIgnoreCase))
						{
							this.ActiveGamepadAlias = GamepadType.XBOX_360;
						}
					}
					this.activeGamepadType = gamepadType;
					goto IL_17D;
				}
				case InputDeviceStyle.XboxSeriesX:
					this.activeGamepadType = GamepadType.XBOX_SERIES_X;
					goto IL_17D;
				case InputDeviceStyle.PlayStation2:
					break;
				case InputDeviceStyle.PlayStation3:
					this.activeGamepadType = GamepadType.PS3_WIN;
					goto IL_17D;
				case InputDeviceStyle.PlayStation4:
					this.activeGamepadType = GamepadType.PS4;
					goto IL_17D;
				case InputDeviceStyle.PlayStation5:
					this.activeGamepadType = GamepadType.PS5;
					goto IL_17D;
				default:
					if (deviceStyle == InputDeviceStyle.NintendoSwitch)
					{
						this.activeGamepadType = GamepadType.SWITCH_PRO_CONTROLLER;
						goto IL_17D;
					}
					break;
				}
				Debug.LogError("Unable to match controller of name (" + inputDevice.Name + "), will attempt default mapping set." + inputDevice.DeviceStyle.ToString());
				this.activeGamepadType = GamepadType.XBOX_360;
			}
			else
			{
				Debug.LogError("Unsupported platform for InputHander " + Application.platform.ToString());
				this.activeGamepadType = GamepadType.XBOX_360;
			}
			IL_17D:
			this.MapControllerButtons(this.activeGamepadType);
			this.UpdateActiveController();
			this.SetupMappableControllerBindingsList();
			base.StartCoroutine(this.SetupGamepadUIInputActions());
		}
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000984B8 File Offset: 0x000966B8
	private void MapDefaultKeyboardLayout()
	{
		this.inputActions.Jump.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.Z
		}));
		this.inputActions.Attack.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.X
		}));
		this.inputActions.Dash.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.C
		}));
		this.inputActions.Cast.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.A
		}));
		this.inputActions.SuperDash.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.S
		}));
		this.inputActions.DreamNail.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.D
		}));
		this.inputActions.QuickMap.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.Tab
		}));
		this.inputActions.OpenInventory.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.I
		}));
		this.inputActions.OpenInventoryMap.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.M
		}));
		this.inputActions.OpenInventoryJournal.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.J
		}));
		this.inputActions.OpenInventoryTools.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.Q
		}));
		this.inputActions.OpenInventoryQuests.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.T
		}));
		this.inputActions.QuickCast.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.F
		}));
		this.inputActions.Taunt.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.V
		}));
		this.inputActions.Up.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.UpArrow
		}));
		this.inputActions.Down.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.DownArrow
		}));
		this.inputActions.Left.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.LeftArrow
		}));
		this.inputActions.Right.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			InControl.Key.RightArrow
		}));
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x00098718 File Offset: 0x00096918
	private void MapKeyboardLayoutFromGameSettings()
	{
		InputHandler.AddKeyBinding(this.inputActions.Jump, this.gs.jumpKey);
		InputHandler.AddKeyBinding(this.inputActions.Attack, this.gs.attackKey);
		InputHandler.AddKeyBinding(this.inputActions.Dash, this.gs.dashKey);
		InputHandler.AddKeyBinding(this.inputActions.Cast, this.gs.castKey);
		InputHandler.AddKeyBinding(this.inputActions.SuperDash, this.gs.superDashKey);
		InputHandler.AddKeyBinding(this.inputActions.DreamNail, this.gs.dreamNailKey);
		InputHandler.AddKeyBinding(this.inputActions.QuickMap, this.gs.quickMapKey);
		InputHandler.AddKeyBinding(this.inputActions.OpenInventory, this.gs.inventoryKey);
		InputHandler.AddKeyBinding(this.inputActions.OpenInventoryMap, this.gs.inventoryMapKey);
		InputHandler.AddKeyBinding(this.inputActions.OpenInventoryJournal, this.gs.inventoryJournalKey);
		InputHandler.AddKeyBinding(this.inputActions.OpenInventoryTools, this.gs.inventoryToolsKey);
		InputHandler.AddKeyBinding(this.inputActions.OpenInventoryQuests, this.gs.inventoryQuestsKey);
		InputHandler.AddKeyBinding(this.inputActions.QuickCast, this.gs.quickCastKey);
		InputHandler.AddKeyBinding(this.inputActions.Taunt, this.gs.tauntKey);
		InputHandler.AddKeyBinding(this.inputActions.Up, this.gs.upKey);
		InputHandler.AddKeyBinding(this.inputActions.Down, this.gs.downKey);
		InputHandler.AddKeyBinding(this.inputActions.Left, this.gs.leftKey);
		InputHandler.AddKeyBinding(this.inputActions.Right, this.gs.rightKey);
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x0009890C File Offset: 0x00096B0C
	private static void AddKeyBinding(PlayerAction action, string savedBinding)
	{
		InControl.Mouse mouse = InControl.Mouse.None;
		InControl.Key key;
		if (!Enum.TryParse<InControl.Key>(savedBinding, out key) && !Enum.TryParse<InControl.Mouse>(savedBinding, out mouse))
		{
			return;
		}
		if (mouse != InControl.Mouse.None)
		{
			action.AddBinding(new MouseBindingSource(mouse));
			return;
		}
		action.AddBinding(new KeyBindingSource(new InControl.Key[]
		{
			key
		}));
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x00098958 File Offset: 0x00096B58
	private void SetupNonMappableBindings()
	{
		if (!this.doingSoftReset)
		{
			this.inputActions = new HeroActions();
		}
		this.inputActions.MenuSubmit.AddDefaultBinding(new InControl.Key[]
		{
			InControl.Key.Return
		});
		this.inputActions.MenuCancel.AddDefaultBinding(new InControl.Key[]
		{
			InControl.Key.Escape
		});
		this.inputActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		this.inputActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.inputActions.Right.AddDefaultBinding(InputControlType.DPadRight);
		this.inputActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		this.inputActions.Up.AddDefaultBinding(InputControlType.DPadUp);
		this.inputActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		this.inputActions.Down.AddDefaultBinding(InputControlType.DPadDown);
		this.inputActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		this.inputActions.RsUp.AddDefaultBinding(InputControlType.RightStickUp);
		this.inputActions.RsDown.AddDefaultBinding(InputControlType.RightStickDown);
		this.inputActions.RsLeft.AddDefaultBinding(InputControlType.RightStickLeft);
		this.inputActions.RsRight.AddDefaultBinding(InputControlType.RightStickRight);
		this.inputActions.Pause.AddDefaultBinding(new InControl.Key[]
		{
			InControl.Key.Escape
		});
		this.inputActions.Pause.AddDefaultBinding(InputControlType.Start);
		this.inputActions.PaneRight.AddDefaultBinding(new InControl.Key[]
		{
			InControl.Key.RightBracket
		});
		this.inputActions.PaneRight.AddDefaultBinding(InputControlType.RightBumper);
		this.inputActions.PaneRight.AddDefaultBinding(InputControlType.RightTrigger);
		this.inputActions.PaneLeft.AddDefaultBinding(new InControl.Key[]
		{
			InControl.Key.LeftBracket
		});
		this.inputActions.PaneLeft.AddDefaultBinding(InputControlType.LeftBumper);
		this.inputActions.PaneLeft.AddDefaultBinding(InputControlType.LeftTrigger);
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x00098B2C File Offset: 0x00096D2C
	private void SetupMappableControllerBindingsList()
	{
		this.MappableControllerActions = new List<PlayerAction>
		{
			this.inputActions.Jump,
			this.inputActions.Attack,
			this.inputActions.Dash,
			this.inputActions.Cast,
			this.inputActions.SuperDash,
			this.inputActions.DreamNail,
			this.inputActions.QuickMap,
			this.inputActions.QuickCast,
			this.inputActions.Taunt,
			this.inputActions.OpenInventory,
			this.inputActions.Up,
			this.inputActions.Down,
			this.inputActions.Left,
			this.inputActions.Right
		};
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x00098C34 File Offset: 0x00096E34
	private void SetupMappableKeyboardBindingsList()
	{
		this.MappableKeyboardActions = new List<PlayerAction>
		{
			this.inputActions.Jump,
			this.inputActions.Attack,
			this.inputActions.Dash,
			this.inputActions.Cast,
			this.inputActions.SuperDash,
			this.inputActions.DreamNail,
			this.inputActions.QuickMap,
			this.inputActions.QuickCast,
			this.inputActions.Taunt,
			this.inputActions.OpenInventory,
			this.inputActions.Up,
			this.inputActions.Down,
			this.inputActions.Left,
			this.inputActions.Right,
			this.inputActions.Up,
			this.inputActions.Down,
			this.inputActions.Left,
			this.inputActions.Right,
			this.inputActions.Taunt,
			this.inputActions.OpenInventoryJournal,
			this.inputActions.OpenInventoryMap,
			this.inputActions.OpenInventoryTools,
			this.inputActions.OpenInventoryQuests
		};
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x00098DD4 File Offset: 0x00096FD4
	public Vector2 GetSticksInput(out bool isRightStick)
	{
		Vector2 value = this.inputActions.MoveVector.Value;
		Vector2 value2 = this.inputActions.RightStick.Value;
		Vector2 result;
		if (value2.magnitude > value.magnitude)
		{
			result = value2;
			isRightStick = true;
		}
		else
		{
			result = value;
			isRightStick = false;
		}
		if (result.magnitude > 1f)
		{
			result.Normalize();
		}
		return result;
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x00098E40 File Offset: 0x00097040
	[CompilerGenerated]
	private void <OnStart>g__Setup|67_0()
	{
		InputManager.OnSetupCompleted -= this.<OnStart>g__Setup|67_0;
		this.doingSoftReset = false;
		this.SetupNonMappableBindings();
		this.gs.LoadKeyboardSettings();
		this.MapKeyboardLayoutFromGameSettings();
		if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.IsAttached)
		{
			this.ControllerActivated(InputManager.ActiveDevice);
		}
		else
		{
			this.gameController = InputDevice.Null;
		}
		this.lastActiveController = BindingSourceType.None;
		this.SetupMappableKeyboardBindingsList();
		Action<HeroActions> onUpdateHeroActions = InputHandler.OnUpdateHeroActions;
		if (onUpdateHeroActions != null)
		{
			onUpdateHeroActions(this.inputActions);
		}
		this.hasSetup = true;
	}

	// Token: 0x04001FEE RID: 8174
	private const float BUTTON_QUEUE_TIME = 0.1f;

	// Token: 0x04001FEF RID: 8175
	private const float STAG_LOCKOUT_DURATION = 1.2f;

	// Token: 0x04001FF2 RID: 8178
	private GameManager gm;

	// Token: 0x04001FF3 RID: 8179
	private GameSettings gs;

	// Token: 0x04001FF4 RID: 8180
	public InputDevice gameController;

	// Token: 0x04001FF5 RID: 8181
	public HeroActions inputActions;

	// Token: 0x04001FF6 RID: 8182
	public BindingSourceType lastActiveController;

	// Token: 0x04001FF7 RID: 8183
	public InputDeviceStyle lastInputDeviceStyle;

	// Token: 0x04001FF8 RID: 8184
	public GamepadType activeGamepadType;

	// Token: 0x04001FFA RID: 8186
	public GamepadState gamepadState;

	// Token: 0x04001FFB RID: 8187
	private HeroController heroCtrl;

	// Token: 0x04001FFC RID: 8188
	private PlayerData playerData;

	// Token: 0x04001FFF RID: 8191
	public bool acceptingInput;

	// Token: 0x04002000 RID: 8192
	public bool skippingCutscene;

	// Token: 0x04002002 RID: 8194
	private bool readyToSkipCutscene;

	// Token: 0x04002004 RID: 8196
	private bool controllerDetected;

	// Token: 0x04002005 RID: 8197
	private ControllerProfile currentControllerProfile;

	// Token: 0x04002006 RID: 8198
	private bool isTitleScreenScene;

	// Token: 0x04002007 RID: 8199
	private bool isMenuScene;

	// Token: 0x04002008 RID: 8200
	private bool isStagTravelScene;

	// Token: 0x04002009 RID: 8201
	private bool stagLockoutActive;

	// Token: 0x0400200A RID: 8202
	private double skipCooldownTime;

	// Token: 0x0400200B RID: 8203
	private bool controllerPressed;

	// Token: 0x0400200C RID: 8204
	private float[] buttonQueueTimers;

	// Token: 0x0400200F RID: 8207
	private bool hasSetup;

	// Token: 0x04002010 RID: 8208
	private bool hasAwaken;

	// Token: 0x04002011 RID: 8209
	private bool hasStarted;

	// Token: 0x04002012 RID: 8210
	[NonSerialized]
	private bool doingSoftReset;

	// Token: 0x0200167F RID: 5759
	public readonly struct KeyOrMouseBinding
	{
		// Token: 0x06008A39 RID: 35385 RVA: 0x0027F551 File Offset: 0x0027D751
		public KeyOrMouseBinding(InControl.Key key)
		{
			this.Key = key;
			this.Mouse = InControl.Mouse.None;
		}

		// Token: 0x06008A3A RID: 35386 RVA: 0x0027F561 File Offset: 0x0027D761
		public KeyOrMouseBinding(InControl.Mouse mouse)
		{
			this.Key = InControl.Key.None;
			this.Mouse = mouse;
		}

		// Token: 0x06008A3B RID: 35387 RVA: 0x0027F571 File Offset: 0x0027D771
		public static bool IsNone(InputHandler.KeyOrMouseBinding val)
		{
			return val.Key == InControl.Key.None && val.Mouse == InControl.Mouse.None;
		}

		// Token: 0x06008A3C RID: 35388 RVA: 0x0027F586 File Offset: 0x0027D786
		public override string ToString()
		{
			if (this.Mouse == InControl.Mouse.None)
			{
				return this.Key.ToString();
			}
			return this.Mouse.ToString();
		}

		// Token: 0x04008B07 RID: 35591
		public readonly InControl.Key Key;

		// Token: 0x04008B08 RID: 35592
		public readonly InControl.Mouse Mouse;
	}

	// Token: 0x02001680 RID: 5760
	// (Invoke) Token: 0x06008A3E RID: 35390
	public delegate void CursorVisibilityChange(bool isVisible);

	// Token: 0x02001681 RID: 5761
	// (Invoke) Token: 0x06008A42 RID: 35394
	public delegate void ActiveControllerSwitch();
}
