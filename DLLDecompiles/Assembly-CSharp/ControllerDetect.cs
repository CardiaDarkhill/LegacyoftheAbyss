using System;
using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200061C RID: 1564
[RequireComponent(typeof(Image))]
public class ControllerDetect : MonoBehaviour
{
	// Token: 0x060037A1 RID: 14241 RVA: 0x000F5338 File Offset: 0x000F3538
	private void Awake()
	{
		this.gm = GameManager.instance;
		this.ih = this.gm.inputHandler;
		this.ui = UIManager.instance;
		this.controllerImage = base.GetComponent<Image>();
		this.profileYPos = base.GetComponent<RectTransform>().anchoredPosition.y;
		this.menuButtonList = base.GetComponentInParent<MenuButtonList>();
	}

	// Token: 0x060037A2 RID: 14242 RVA: 0x000F539A File Offset: 0x000F359A
	private void OnEnable()
	{
		this.LookForActiveController();
		InputManager.OnActiveDeviceChanged += this.ControllerActivated;
		InputManager.OnDeviceAttached += this.ControllerAttached;
		InputManager.OnDeviceDetached += this.ControllerDetached;
	}

	// Token: 0x060037A3 RID: 14243 RVA: 0x000F53D5 File Offset: 0x000F35D5
	private void OnDisable()
	{
		InputManager.OnActiveDeviceChanged -= this.ControllerActivated;
		InputManager.OnDeviceAttached -= this.ControllerAttached;
		InputManager.OnDeviceDetached -= this.ControllerDetached;
	}

	// Token: 0x060037A4 RID: 14244 RVA: 0x000F540A File Offset: 0x000F360A
	private void ControllerActivated(InputDevice inputDevice)
	{
		this.LookForActiveController();
	}

	// Token: 0x060037A5 RID: 14245 RVA: 0x000F5412 File Offset: 0x000F3612
	private void ControllerAttached(InputDevice inputDevice)
	{
		this.LookForActiveController();
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x000F541A File Offset: 0x000F361A
	private void ControllerDetached(InputDevice inputDevice)
	{
		this.LookForActiveController();
		if (EventSystem.current != this.applyButton)
		{
			this.applyButton.Select();
		}
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x000F5440 File Offset: 0x000F3640
	private void ShowController(GamepadType gamepadType)
	{
		gamepadType = Platform.Current.OverrideGamepadDisplay(gamepadType);
		GamepadType gamepadType2;
		if (gamepadType == GamepadType.PS3_WIN)
		{
			gamepadType2 = GamepadType.PS4;
		}
		else
		{
			gamepadType2 = gamepadType;
		}
		foreach (ControllerImage controllerImage in this.controllerImages)
		{
			if (controllerImage.buttonPositions != null)
			{
				controllerImage.buttonPositions.gameObject.SetActive(false);
			}
		}
		float num = DemoHelper.IsDemoMode ? this.remapHiddenOffsetY : 0f;
		foreach (ControllerImage controllerImage2 in this.controllerImages)
		{
			if (controllerImage2.gamepadType == gamepadType2)
			{
				this.controllerImage.sprite = controllerImage2.sprite;
				if (controllerImage2.buttonPositions != null)
				{
					controllerImage2.buttonPositions.gameObject.SetActive(true);
				}
				base.transform.localScale = new Vector3(controllerImage2.displayScale, controllerImage2.displayScale, 1f);
				RectTransform component = base.GetComponent<RectTransform>();
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.y = this.profileYPos + controllerImage2.offsetY + num;
				component.anchoredPosition = anchoredPosition;
				return;
			}
		}
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x000F5564 File Offset: 0x000F3764
	private void HideButtonLabels()
	{
		foreach (ControllerImage controllerImage in this.controllerImages)
		{
			if (controllerImage.buttonPositions != null)
			{
				controllerImage.buttonPositions.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x000F55AC File Offset: 0x000F37AC
	private void LookForActiveController()
	{
		if (this.ih.gamepadState == GamepadState.DETACHED)
		{
			this.HideButtonLabels();
			this.controllerImage.sprite = this.controllerImages[0].sprite;
			this.ui.ShowCanvasGroup(this.controllerPrompt);
			this.remapButton.gameObject.SetActive(false);
		}
		else if (this.ih.activeGamepadType != GamepadType.NONE)
		{
			this.ui.HideCanvasGroup(this.controllerPrompt);
			this.remapButton.gameObject.SetActive(true);
			this.ShowController((this.ih.ActiveGamepadAlias != GamepadType.NONE) ? this.ih.ActiveGamepadAlias : this.ih.activeGamepadType);
		}
		if (this.menuButtonList)
		{
			this.menuButtonList.SetupActive();
		}
	}

	// Token: 0x04003AA3 RID: 15011
	private GameManager gm;

	// Token: 0x04003AA4 RID: 15012
	private UIManager ui;

	// Token: 0x04003AA5 RID: 15013
	private InputHandler ih;

	// Token: 0x04003AA6 RID: 15014
	private Image controllerImage;

	// Token: 0x04003AA7 RID: 15015
	[Header("Controller Menu Items")]
	public CanvasGroup controllerPrompt;

	// Token: 0x04003AA8 RID: 15016
	public CanvasGroup remapDialog;

	// Token: 0x04003AA9 RID: 15017
	public CanvasGroup menuControls;

	// Token: 0x04003AAA RID: 15018
	public CanvasGroup remapControls;

	// Token: 0x04003AAB RID: 15019
	[Header("Controller Menu Preselect")]
	public Selectable controllerMenuPreselect;

	// Token: 0x04003AAC RID: 15020
	public Selectable remapMenuPreselect;

	// Token: 0x04003AAD RID: 15021
	[Header("Remap Menu Controls")]
	public MenuSelectable remapApplyButton;

	// Token: 0x04003AAE RID: 15022
	public MenuSelectable defaultsButton;

	// Token: 0x04003AAF RID: 15023
	[Header("Controller Menu Controls")]
	public MenuButton applyButton;

	// Token: 0x04003AB0 RID: 15024
	public MenuButton remapButton;

	// Token: 0x04003AB1 RID: 15025
	public float remapHiddenOffsetY;

	// Token: 0x04003AB2 RID: 15026
	[SerializeField]
	public ControllerImage[] controllerImages;

	// Token: 0x04003AB3 RID: 15027
	private float profileYPos;

	// Token: 0x04003AB4 RID: 15028
	private MenuButtonList menuButtonList;
}
