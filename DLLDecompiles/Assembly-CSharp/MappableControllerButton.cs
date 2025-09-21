using System;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006D3 RID: 1747
public class MappableControllerButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ICancelHandler
{
	// Token: 0x06003F11 RID: 16145 RVA: 0x0011631A File Offset: 0x0011451A
	private new void Start()
	{
		if (Application.isPlaying)
		{
			this.active = true;
			this.SetupRefs();
		}
	}

	// Token: 0x06003F12 RID: 16146 RVA: 0x00116330 File Offset: 0x00114530
	private new void OnEnable()
	{
		if (Application.isPlaying)
		{
			if (!this.active)
			{
				this.Start();
			}
			this.GetBinding();
			Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
		}
	}

	// Token: 0x06003F13 RID: 16147 RVA: 0x0011635E File Offset: 0x0011455E
	protected override void OnDisable()
	{
		base.OnDisable();
		this.ClearSwapCache();
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003F14 RID: 16148 RVA: 0x0011637D File Offset: 0x0011457D
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.ShowCurrentBinding();
		}
	}

	// Token: 0x06003F15 RID: 16149 RVA: 0x00116388 File Offset: 0x00114588
	private void GetBinding()
	{
		this.currentBinding = this.ih.GetButtonBindingForAction(this.playerAction);
	}

	// Token: 0x06003F16 RID: 16150 RVA: 0x001163A1 File Offset: 0x001145A1
	private void ClearSwapCache()
	{
		this.bindingToSwap = null;
		this.actionToSwap = null;
	}

	// Token: 0x06003F17 RID: 16151 RVA: 0x001163B4 File Offset: 0x001145B4
	public void ListenForNewButton()
	{
		this.buttonmapSprite.sprite = this.uibs.blankKey;
		this.buttonmapText.text = Language.Get("CTRL_PRESSBUTTON", "MainMenu");
		base.interactable = false;
		this.ClearSwapCache();
		this.SetupBindingListenOptions();
		this.isListening = true;
		this.uibs.ListeningForButtonRebind(this);
		this.playerAction.ListenForBinding();
	}

	// Token: 0x06003F18 RID: 16152 RVA: 0x00116424 File Offset: 0x00114624
	public void ShowCurrentBinding()
	{
		if (!this.active)
		{
			this.Start();
		}
		this.GetBinding();
		if (this.currentBinding == InputControlType.None)
		{
			this.buttonmapSprite.sprite = this.uibs.blankKey;
			this.buttonmapText.text = Language.Get("CTRL_UNMAPPED", "MainMenu");
		}
		else
		{
			ButtonSkin controllerButtonSkinFor = this.uibs.GetControllerButtonSkinFor(this.playerAction);
			this.buttonmapSprite.sprite = controllerButtonSkinFor.sprite;
			this.buttonmapText.text = "";
			if (this.buttonmapSprite.sprite == null)
			{
				Debug.LogError("Could not find a suitable skin for the new button map: " + this.currentBinding.ToString());
				this.buttonmapSprite.sprite = this.uibs.blankKey;
			}
		}
		base.interactable = true;
	}

	// Token: 0x06003F19 RID: 16153 RVA: 0x00116502 File Offset: 0x00114702
	public void AbortRebind()
	{
		if (this.isListening)
		{
			base.interactable = true;
			this.isListening = false;
			this.ClearSwapCache();
		}
	}

	// Token: 0x06003F1A RID: 16154 RVA: 0x00116520 File Offset: 0x00114720
	public void StopActionListening()
	{
		this.playerAction.StopListeningForBinding();
	}

	// Token: 0x06003F1B RID: 16155 RVA: 0x00116530 File Offset: 0x00114730
	public bool OnBindingFound(PlayerAction action, BindingSource binding)
	{
		DeviceBindingSource deviceBindingSource = (DeviceBindingSource)binding;
		if (this.unmappableButtons.Contains(binding as DeviceBindingSource))
		{
			this.uibs.FinishedListeningForButton();
			action.StopListeningForBinding();
			this.AbortRebind();
			if (this.verboseMode)
			{
				Debug.LogFormat("Cancelled new {0} button binding (Not allowed to bind {1})", new object[]
				{
					action.Name,
					deviceBindingSource.Control
				});
			}
			return false;
		}
		if (binding != null)
		{
			foreach (PlayerAction playerAction in this.ih.MappableControllerActions)
			{
				if (playerAction != this.playerAction && playerAction.Bindings.Contains(binding))
				{
					this.actionToSwap = playerAction;
					break;
				}
			}
			if (this.actionToSwap != null)
			{
				foreach (BindingSource bindingSource in action.Bindings)
				{
					if (bindingSource.BindingSourceType == binding.BindingSourceType || bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource)
					{
						this.bindingToSwap = bindingSource;
						break;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06003F1C RID: 16156 RVA: 0x00116674 File Offset: 0x00114874
	public void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		DeviceBindingSource deviceBindingSource = (DeviceBindingSource)binding;
		if (this.verboseMode)
		{
			Debug.Log("New binding added for " + action.Name + ": " + deviceBindingSource.Control.ToString());
		}
		if (this.actionToSwap != null && this.bindingToSwap != null)
		{
			this.actionToSwap.AddBinding(this.bindingToSwap);
		}
		this.uibs.FinishedListeningForButton();
		this.isListening = false;
		base.interactable = true;
		this.ih.RemapUiButtons();
		this.ClearSwapCache();
	}

	// Token: 0x06003F1D RID: 16157 RVA: 0x00116710 File Offset: 0x00114910
	public void OnBindingRejected(PlayerAction action, BindingSource binding, BindingSourceRejectionType rejection)
	{
		DeviceBindingSource deviceBindingSource = (DeviceBindingSource)binding;
		if (rejection == BindingSourceRejectionType.DuplicateBindingOnAction)
		{
			if (this.verboseMode)
			{
				Debug.LogFormat("{0}->{1} is already bound to {2}, cancelling rebind", new object[]
				{
					deviceBindingSource.DeviceName,
					deviceBindingSource.Control,
					action.Name
				});
			}
			this.uibs.FinishedListeningForButton();
			this.AbortRebind();
			action.StopListeningForBinding();
			this.isListening = false;
		}
		else if (rejection == BindingSourceRejectionType.DuplicateBindingOnActionSet)
		{
			if (this.verboseMode)
			{
				string text = " |";
				for (int i = 0; i < action.Bindings.Count; i++)
				{
					text = text + action.Bindings[i].Name + "|";
				}
				text += "|";
				Debug.LogErrorFormat("{0}->{1} is already bound to another button: {2}", new object[]
				{
					deviceBindingSource.DeviceName,
					deviceBindingSource.Control,
					text
				});
			}
		}
		else
		{
			if (this.verboseMode)
			{
				Debug.Log("Binding rejected for " + action.Name + ": " + rejection.ToString());
			}
			this.uibs.FinishedListeningForButton();
			this.AbortRebind();
			action.StopListeningForBinding();
			this.isListening = false;
		}
		this.ClearSwapCache();
	}

	// Token: 0x06003F1E RID: 16158 RVA: 0x00116857 File Offset: 0x00114A57
	public new void OnSubmit(BaseEventData eventData)
	{
		if (!this.isListening)
		{
			this.ListenForNewButton();
		}
	}

	// Token: 0x06003F1F RID: 16159 RVA: 0x00116867 File Offset: 0x00114A67
	public new void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	// Token: 0x06003F20 RID: 16160 RVA: 0x00116870 File Offset: 0x00114A70
	public new void OnCancel(BaseEventData eventData)
	{
		if (this.isListening)
		{
			if (this.ih.lastActiveController == BindingSourceType.KeyBindingSource)
			{
				this.StopListeningForNewButton();
				return;
			}
		}
		else
		{
			base.OnCancel(eventData);
		}
	}

	// Token: 0x06003F21 RID: 16161 RVA: 0x00116896 File Offset: 0x00114A96
	private void StopListeningForNewButton()
	{
		this.uibs.FinishedListeningForButton();
		this.StopActionListening();
		this.AbortRebind();
	}

	// Token: 0x06003F22 RID: 16162 RVA: 0x001168B0 File Offset: 0x00114AB0
	private void SetupUnmappableButtons()
	{
		this.unmappableButtons = new List<DeviceBindingSource>();
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.DPadUp));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.DPadDown));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.DPadLeft));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.DPadRight));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.LeftStickUp));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.LeftStickDown));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.LeftStickLeft));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.LeftStickRight));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.RightStickUp));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.RightStickDown));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.RightStickLeft));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.RightStickRight));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.LeftStickButton));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.RightStickButton));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Start));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Select));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Command));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Back));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Menu));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Options));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.TouchPadButton));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Options));
		this.unmappableButtons.Add(new DeviceBindingSource(InputControlType.Share));
	}

	// Token: 0x06003F23 RID: 16163 RVA: 0x00116A64 File Offset: 0x00114C64
	private void SetupBindingListenOptions()
	{
		BindingListenOptions bindingListenOptions = new BindingListenOptions();
		bindingListenOptions.IncludeControllers = true;
		bindingListenOptions.IncludeNonStandardControls = false;
		bindingListenOptions.IncludeMouseButtons = false;
		bindingListenOptions.IncludeKeys = false;
		bindingListenOptions.IncludeModifiersAsFirstClassKeys = false;
		bindingListenOptions.IncludeUnknownControllers = false;
		bindingListenOptions.MaxAllowedBindingsPerType = 1U;
		bindingListenOptions.OnBindingFound = new Func<PlayerAction, BindingSource, bool>(this.OnBindingFound);
		bindingListenOptions.OnBindingAdded = new Action<PlayerAction, BindingSource>(this.OnBindingAdded);
		bindingListenOptions.OnBindingRejected = new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected);
		bindingListenOptions.UnsetDuplicateBindingsOnSet = true;
		this.ih.inputActions.ListenOptions = bindingListenOptions;
	}

	// Token: 0x06003F24 RID: 16164 RVA: 0x00116AF8 File Offset: 0x00114CF8
	private void SetupRefs()
	{
		this.gm = GameManager.instance;
		this.ui = this.gm.ui;
		this.uibs = this.ui.uiButtonSkins;
		this.ih = this.gm.inputHandler;
		this.playerAction = this.ih.ActionButtonToPlayerAction(this.actionButtonType);
		base.HookUpAudioPlayer();
		base.HookUpEventTrigger();
		this.SetupUnmappableButtons();
	}

	// Token: 0x040040D7 RID: 16599
	private bool verboseMode;

	// Token: 0x040040D8 RID: 16600
	private GameManager gm;

	// Token: 0x040040D9 RID: 16601
	private InputHandler ih;

	// Token: 0x040040DA RID: 16602
	private UIManager ui;

	// Token: 0x040040DB RID: 16603
	private UIButtonSkins uibs;

	// Token: 0x040040DC RID: 16604
	private PlayerAction playerAction;

	// Token: 0x040040DD RID: 16605
	private PlayerAction actionToSwap;

	// Token: 0x040040DE RID: 16606
	private BindingSource bindingToSwap;

	// Token: 0x040040DF RID: 16607
	private bool active;

	// Token: 0x040040E0 RID: 16608
	private bool isListening;

	// Token: 0x040040E1 RID: 16609
	private InputControlType currentBinding;

	// Token: 0x040040E2 RID: 16610
	private List<DeviceBindingSource> unmappableButtons;

	// Token: 0x040040E3 RID: 16611
	[Space(6f)]
	[Header("Button Mapping")]
	public HeroActionButton actionButtonType;

	// Token: 0x040040E4 RID: 16612
	public Text buttonmapText;

	// Token: 0x040040E5 RID: 16613
	public Image buttonmapSprite;
}
