using System;
using System.Collections;
using GlobalEnums;
using InControl;
using UnityEngine;

// Token: 0x0200073D RID: 1853
public class UIButtonSkins : MonoBehaviour
{
	// Token: 0x1700077F RID: 1919
	// (get) Token: 0x06004218 RID: 16920 RVA: 0x001229FA File Offset: 0x00120BFA
	// (set) Token: 0x06004219 RID: 16921 RVA: 0x00122A02 File Offset: 0x00120C02
	public MappableKey listeningKey { get; private set; }

	// Token: 0x17000780 RID: 1920
	// (get) Token: 0x0600421A RID: 16922 RVA: 0x00122A0B File Offset: 0x00120C0B
	// (set) Token: 0x0600421B RID: 16923 RVA: 0x00122A13 File Offset: 0x00120C13
	public MappableControllerButton listeningButton { get; private set; }

	// Token: 0x0600421C RID: 16924 RVA: 0x00122A1C File Offset: 0x00120C1C
	private void Start()
	{
		this.SetupRefs();
	}

	// Token: 0x0600421D RID: 16925 RVA: 0x00122A24 File Offset: 0x00120C24
	private void OnEnable()
	{
		if (!this.active)
		{
			this.SetupRefs();
		}
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x00122A34 File Offset: 0x00120C34
	private void OnDestroy()
	{
		if (this.ih != null)
		{
			this.ih.RefreshActiveControllerEvent -= this.OnControllerConnected;
		}
	}

	// Token: 0x0600421F RID: 16927 RVA: 0x00122A5C File Offset: 0x00120C5C
	public ButtonSkin GetButtonSkinFor(PlayerAction action)
	{
		switch (this.ih.lastActiveController)
		{
		case BindingSourceType.None:
		case BindingSourceType.KeyBindingSource:
		case BindingSourceType.MouseBindingSource:
			return this.GetButtonSkinFor(this.ih.GetKeyBindingForAction(action).ToString());
		case BindingSourceType.DeviceBindingSource:
		{
			InputControlType buttonBindingForAction = this.ih.GetButtonBindingForAction(action);
			return this.GetButtonSkinFor(buttonBindingForAction);
		}
		default:
			return null;
		}
	}

	// Token: 0x06004220 RID: 16928 RVA: 0x00122AC4 File Offset: 0x00120CC4
	public ButtonSkin GetKeyboardSkinFor(PlayerAction action)
	{
		return this.GetButtonSkinFor(this.ih.GetKeyBindingForAction(action).ToString());
	}

	// Token: 0x06004221 RID: 16929 RVA: 0x00122AF4 File Offset: 0x00120CF4
	public ButtonSkin GetControllerButtonSkinFor(PlayerAction action)
	{
		InputControlType buttonBindingForAction = this.ih.GetButtonBindingForAction(action);
		return this.GetButtonSkinFor(buttonBindingForAction);
	}

	// Token: 0x06004222 RID: 16930 RVA: 0x00122B18 File Offset: 0x00120D18
	public ButtonSkin GetButtonSkinFor(HeroActionButton actionButton)
	{
		if (this.ih == null)
		{
			Debug.LogWarning("Attempting to get button skins before the Input Handler is ready.", this);
			return this.GetButtonSkinFor(Key.None.ToString());
		}
		return this.GetButtonSkinFor(this.ih.ActionButtonToPlayerAction(actionButton));
	}

	// Token: 0x06004223 RID: 16931 RVA: 0x00122B68 File Offset: 0x00120D68
	public void ShowCurrentKeyboardMappings()
	{
		foreach (MappableKey mappableKey in this.mappableKeyboardButtons.GetComponentsInChildren<MappableKey>())
		{
			mappableKey.GetBinding();
			mappableKey.ShowCurrentBinding();
		}
	}

	// Token: 0x06004224 RID: 16932 RVA: 0x00122B9D File Offset: 0x00120D9D
	public IEnumerator ShowCurrentButtonMappings()
	{
		MappableControllerButton[] actionButtons = this.mappableControllerButtons.GetComponentsInChildren<MappableControllerButton>();
		int num;
		for (int i = 0; i < actionButtons.Length; i = num + 1)
		{
			actionButtons[i].ShowCurrentBinding();
			yield return null;
			num = i;
		}
		yield return null;
		yield break;
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x00122BAC File Offset: 0x00120DAC
	public IEnumerator InitialButtonMappingSetup()
	{
		MappableControllerButton[] actionButtons = this.mappableControllerButtons.GetComponentsInChildren<MappableControllerButton>(true);
		int num;
		for (int i = 0; i < actionButtons.Length; i = num + 1)
		{
			actionButtons[i].ShowCurrentBinding();
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x00122BBC File Offset: 0x00120DBC
	public void RefreshKeyMappings()
	{
		foreach (MappableKey mappableKey in this.mappableKeyboardButtons.GetComponentsInChildren<MappableKey>())
		{
			mappableKey.GetBinding();
			mappableKey.ShowCurrentBinding();
		}
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x00122BF4 File Offset: 0x00120DF4
	public void RefreshButtonMappings()
	{
		MappableControllerButton[] componentsInChildren = this.mappableControllerButtons.GetComponentsInChildren<MappableControllerButton>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ShowCurrentBinding();
		}
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x00122C23 File Offset: 0x00120E23
	public void ListeningForKeyRebind(MappableKey mappableKey)
	{
		if (this.listeningKey == null)
		{
			this.listeningKey = mappableKey;
			return;
		}
		this.listeningKey.AbortRebind();
		this.listeningKey = mappableKey;
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x00122C4D File Offset: 0x00120E4D
	public void ListeningForButtonRebind(MappableControllerButton mappableButton)
	{
		if (this.listeningButton != null)
		{
			this.listeningButton.AbortRebind();
		}
		this.listeningButton = mappableButton;
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x00122C6F File Offset: 0x00120E6F
	public void FinishedListeningForKey()
	{
		this.listeningKey = null;
		this.RefreshKeyMappings();
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x00122C7E File Offset: 0x00120E7E
	public void FinishedListeningForButton()
	{
		this.listeningButton = null;
		base.StartCoroutine(this.ShowCurrentButtonMappings());
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x00122C94 File Offset: 0x00120E94
	private ButtonSkin GetButtonSkinFor(InputControlType inputControlType)
	{
		ButtonSkin buttonSkin = new ButtonSkin(this.blankKey, "", ButtonSkinType.CONTROLLER);
		if (this.ih.activeGamepadType == GamepadType.PS4)
		{
			switch (inputControlType)
			{
			case InputControlType.DPadUp:
				buttonSkin.sprite = this.dpadUp;
				break;
			case InputControlType.DPadDown:
				buttonSkin.sprite = this.dpadDown;
				break;
			case InputControlType.DPadLeft:
				buttonSkin.sprite = this.dpadLeft;
				break;
			case InputControlType.DPadRight:
				buttonSkin.sprite = this.dpadRight;
				break;
			case InputControlType.LeftTrigger:
				buttonSkin.sprite = this.ps4lt;
				break;
			case InputControlType.RightTrigger:
				buttonSkin.sprite = this.ps4rt;
				break;
			case InputControlType.LeftBumper:
				buttonSkin.sprite = this.ps4lb;
				break;
			case InputControlType.RightBumper:
				buttonSkin.sprite = this.ps4rb;
				break;
			case InputControlType.Action1:
				buttonSkin.sprite = this.ps4x;
				break;
			case InputControlType.Action2:
				buttonSkin.sprite = this.ps4circle;
				break;
			case InputControlType.Action3:
				buttonSkin.sprite = this.ps4square;
				break;
			case InputControlType.Action4:
				buttonSkin.sprite = this.ps4triangle;
				break;
			default:
				switch (inputControlType)
				{
				case InputControlType.Start:
					buttonSkin.sprite = this.start;
					break;
				case InputControlType.Select:
					buttonSkin.sprite = this.select;
					break;
				case InputControlType.System:
				case InputControlType.Pause:
				case InputControlType.Menu:
					break;
				case InputControlType.Options:
					buttonSkin.sprite = this.options;
					break;
				case InputControlType.Share:
					buttonSkin.sprite = this.share;
					break;
				default:
					if (inputControlType == InputControlType.TouchPadButton)
					{
						buttonSkin.sprite = this.touchpadButton;
					}
					break;
				}
				break;
			}
		}
		else if (this.ih.activeGamepadType == GamepadType.PS5)
		{
			switch (inputControlType)
			{
			case InputControlType.DPadUp:
				buttonSkin.sprite = this.dpadUp;
				break;
			case InputControlType.DPadDown:
				buttonSkin.sprite = this.dpadDown;
				break;
			case InputControlType.DPadLeft:
				buttonSkin.sprite = this.dpadLeft;
				break;
			case InputControlType.DPadRight:
				buttonSkin.sprite = this.dpadRight;
				break;
			case InputControlType.LeftTrigger:
				buttonSkin.sprite = this.ps5.lt;
				break;
			case InputControlType.RightTrigger:
				buttonSkin.sprite = this.ps5.rt;
				break;
			case InputControlType.LeftBumper:
				buttonSkin.sprite = this.ps5.lb;
				break;
			case InputControlType.RightBumper:
				buttonSkin.sprite = this.ps5.rb;
				break;
			case InputControlType.Action1:
				buttonSkin.sprite = this.ps5.cross;
				break;
			case InputControlType.Action2:
				buttonSkin.sprite = this.ps5.circle;
				break;
			case InputControlType.Action3:
				buttonSkin.sprite = this.ps5.square;
				break;
			case InputControlType.Action4:
				buttonSkin.sprite = this.ps5.triangle;
				break;
			default:
				switch (inputControlType)
				{
				case InputControlType.Start:
					buttonSkin.sprite = this.start;
					break;
				case InputControlType.Select:
					buttonSkin.sprite = this.select;
					break;
				case InputControlType.System:
				case InputControlType.Pause:
				case InputControlType.Menu:
					break;
				case InputControlType.Options:
					buttonSkin.sprite = this.ps5.options;
					break;
				case InputControlType.Share:
					buttonSkin.sprite = this.ps5.share;
					break;
				default:
					if (inputControlType == InputControlType.TouchPadButton)
					{
						buttonSkin.sprite = this.ps5.touchpadButton;
					}
					break;
				}
				break;
			}
			if (buttonSkin.sprite == null)
			{
				this.GetSkin(buttonSkin, inputControlType);
			}
		}
		else
		{
			GamepadType activeGamepadType = this.ih.activeGamepadType;
			if (activeGamepadType == GamepadType.SWITCH_JOYCON_DUAL || activeGamepadType == GamepadType.SWITCH_PRO_CONTROLLER || activeGamepadType == GamepadType.SWITCH2_JOYCON_DUAL || activeGamepadType == GamepadType.SWITCH2_PRO_CONTROLLER)
			{
				switch (inputControlType)
				{
				case InputControlType.DPadUp:
					buttonSkin.sprite = this.switchHidDPadUp;
					break;
				case InputControlType.DPadDown:
					buttonSkin.sprite = this.switchHidDPadDown;
					break;
				case InputControlType.DPadLeft:
					buttonSkin.sprite = this.switchHidDPadLeft;
					break;
				case InputControlType.DPadRight:
					buttonSkin.sprite = this.switchHidDPadRight;
					break;
				case InputControlType.LeftTrigger:
					buttonSkin.sprite = this.switchHidLeftTrigger;
					break;
				case InputControlType.RightTrigger:
					buttonSkin.sprite = this.switchHidRightTrigger;
					break;
				case InputControlType.LeftBumper:
					buttonSkin.sprite = this.switchHidLeftBumper;
					break;
				case InputControlType.RightBumper:
					buttonSkin.sprite = this.switchHidRightBumper;
					break;
				case InputControlType.Action1:
					buttonSkin.sprite = this.switchHidB;
					break;
				case InputControlType.Action2:
					buttonSkin.sprite = this.switchHidA;
					break;
				case InputControlType.Action3:
					buttonSkin.sprite = this.switchHidY;
					break;
				case InputControlType.Action4:
					buttonSkin.sprite = this.switchHidX;
					break;
				default:
					if (inputControlType != InputControlType.Start)
					{
						if (inputControlType == InputControlType.Select)
						{
							buttonSkin.sprite = this.switchHidMinus;
						}
					}
					else
					{
						buttonSkin.sprite = this.switchHidPlus;
					}
					break;
				}
			}
			else
			{
				switch (inputControlType)
				{
				case InputControlType.DPadUp:
					buttonSkin.sprite = this.dpadUp;
					break;
				case InputControlType.DPadDown:
					buttonSkin.sprite = this.dpadDown;
					break;
				case InputControlType.DPadLeft:
					buttonSkin.sprite = this.dpadLeft;
					break;
				case InputControlType.DPadRight:
					buttonSkin.sprite = this.dpadRight;
					break;
				case InputControlType.LeftTrigger:
					buttonSkin.sprite = this.lt;
					break;
				case InputControlType.RightTrigger:
					buttonSkin.sprite = this.rt;
					break;
				case InputControlType.LeftBumper:
					buttonSkin.sprite = this.lb;
					break;
				case InputControlType.RightBumper:
					buttonSkin.sprite = this.rb;
					break;
				case InputControlType.Action1:
					buttonSkin.sprite = this.a;
					break;
				case InputControlType.Action2:
					buttonSkin.sprite = this.b;
					break;
				case InputControlType.Action3:
					buttonSkin.sprite = this.x;
					break;
				case InputControlType.Action4:
					buttonSkin.sprite = this.y;
					break;
				default:
					switch (inputControlType)
					{
					case InputControlType.Back:
						buttonSkin.sprite = this.select;
						break;
					case InputControlType.Start:
						buttonSkin.sprite = this.start;
						break;
					case InputControlType.Select:
						buttonSkin.sprite = this.select;
						break;
					case InputControlType.System:
					case InputControlType.Options:
					case InputControlType.Pause:
						break;
					case InputControlType.Menu:
						buttonSkin.sprite = this.menu;
						break;
					default:
						if (inputControlType == InputControlType.View)
						{
							buttonSkin.sprite = this.view;
						}
						break;
					}
					break;
				}
			}
		}
		return buttonSkin;
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x001232F8 File Offset: 0x001214F8
	private ButtonSkin GetButtonSkinFor(string buttonName)
	{
		ButtonSkin buttonSkin = new ButtonSkin(this.blankKey, buttonName, ButtonSkinType.BLANK);
		if (buttonName.Length == 1)
		{
			if (char.IsLetter(buttonName[0]))
			{
				buttonSkin.sprite = this.squareKey;
				buttonSkin.symbol = buttonName;
				buttonSkin.skinType = ButtonSkinType.SQUARE;
			}
		}
		else
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(buttonName);
			if (num <= 1044186795U)
			{
				if (num <= 450021021U)
				{
					if (num <= 355122396U)
					{
						if (num <= 215134355U)
						{
							if (num <= 75071339U)
							{
								if (num != 55056364U)
								{
									if (num == 75071339U)
									{
										if (buttonName == "LeftAlt")
										{
											buttonSkin.sprite = this.rectangleKey;
											buttonSkin.symbol = "L Alt";
											buttonSkin.skinType = ButtonSkinType.WIDE;
											return buttonSkin;
										}
									}
								}
								else if (buttonName == "MiddleButton")
								{
									buttonSkin.sprite = this.middleMouseButton;
									buttonSkin.symbol = "";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
							else if (num != 198356736U)
							{
								if (num == 215134355U)
								{
									if (buttonName == "F8")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "F9")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (num <= 257323198U)
						{
							if (num != 254900552U)
							{
								if (num == 257323198U)
								{
									if (buttonName == "Semicolon")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.symbol = ";";
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "Insert")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "Ins";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (num != 311208791U)
						{
							if (num != 332577688U)
							{
								if (num == 355122396U)
								{
									if (buttonName == "Key8")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.symbol = "8";
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "F1")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (buttonName == "PadDivide")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "/";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num <= 400677660U)
					{
						if (num <= 371900015U)
						{
							if (num != 366132926U)
							{
								if (num == 371900015U)
								{
									if (buttonName == "Key9")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.symbol = "9";
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "F3")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (num != 382910545U)
						{
							if (num != 399688164U)
							{
								if (num == 400677660U)
								{
									if (buttonName == "PadEnter")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.symbol = "En";
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "F5")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (buttonName == "F2")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num <= 422232872U)
					{
						if (num != 416465783U)
						{
							if (num == 422232872U)
							{
								if (buttonName == "Key4")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "4";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "F4")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 433243402U)
					{
						if (num != 439010491U)
						{
							if (num == 450021021U)
							{
								if (buttonName == "F6")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Key5")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "5";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (buttonName == "F7")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num <= 557853160U)
				{
					if (num <= 473965065U)
					{
						if (num <= 457187446U)
						{
							if (num != 455788110U)
							{
								if (num == 457187446U)
								{
									if (buttonName == "Pad8")
									{
										buttonSkin.sprite = this.squareKey;
										buttonSkin.symbol = "8";
										buttonSkin.skinType = ButtonSkinType.SQUARE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "Key6")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "6";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (num != 472565729U)
						{
							if (num == 473965065U)
							{
								if (buttonName == "Pad9")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "9";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Key7")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "7";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num <= 506120967U)
					{
						if (num != 489343348U)
						{
							if (num == 506120967U)
							{
								if (buttonName == "Key1")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "1";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Key0")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "0";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 522898586U)
					{
						if (num != 539676205U)
						{
							if (num == 557853160U)
							{
								if (buttonName == "Pad2")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "2";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Key3")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "3";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (buttonName == "Key2")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "2";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num <= 641741255U)
				{
					if (num <= 591408398U)
					{
						if (num != 574630779U)
						{
							if (num == 591408398U)
							{
								if (buttonName == "Pad0")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "0";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Pad3")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "3";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 608186017U)
					{
						if (num != 624963636U)
						{
							if (num == 641741255U)
							{
								if (buttonName == "Pad7")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "7";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Pad6")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "6";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (buttonName == "Pad1")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "1";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num <= 658518874U)
				{
					if (num != 651038163U)
					{
						if (num == 658518874U)
						{
							if (buttonName == "Pad4")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "4";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "RightBracket")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "]";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num != 675296493U)
				{
					if (num != 850645478U)
					{
						if (num == 1044186795U)
						{
							if (buttonName == "PageUp")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "PgUp";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "RightArrow")
					{
						buttonSkin.sprite = this.rightArrowKey;
						buttonSkin.symbol = "";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (buttonName == "Pad5")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.symbol = "5";
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (num <= 2118290244U)
			{
				if (num <= 1469573738U)
				{
					if (num <= 1391791790U)
					{
						if (num <= 1231278590U)
						{
							if (num != 1050238388U)
							{
								if (num == 1231278590U)
								{
									if (buttonName == "RightControl")
									{
										buttonSkin.sprite = this.rectangleKey;
										buttonSkin.symbol = "R Ctrl";
										buttonSkin.skinType = ButtonSkinType.WIDE;
										return buttonSkin;
									}
								}
							}
							else if (buttonName == "Equals")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "=";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
						else if (num != 1258159639U)
						{
							if (num == 1391791790U)
							{
								if (buttonName == "Home")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "Home";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Backslash")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "\\";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num <= 1428210068U)
					{
						if (num != 1406581478U)
						{
							if (num == 1428210068U)
							{
								if (buttonName == "LeftShift")
								{
									buttonSkin.sprite = this.rectangleKey;
									buttonSkin.symbol = "L Shift";
									buttonSkin.skinType = ButtonSkinType.WIDE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "PadMinus")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "-";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 1462265726U)
					{
						if (num != 1466336430U)
						{
							if (num == 1469573738U)
							{
								if (buttonName == "Delete")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = "Del";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "PadPlus")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "+";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (buttonName == "Backquote")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "~";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num <= 1706424088U)
				{
					if (num <= 1565561622U)
					{
						if (num != 1522423415U)
						{
							if (num == 1565561622U)
							{
								if (buttonName == "DownArrow")
								{
									buttonSkin.sprite = this.downArrowKey;
									buttonSkin.symbol = "";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Quote")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = "'";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 1629542348U)
					{
						if (num != 1682706001U)
						{
							if (num == 1706424088U)
							{
								if (buttonName == "Comma")
								{
									buttonSkin.sprite = this.squareKey;
									buttonSkin.symbol = ",";
									buttonSkin.skinType = ButtonSkinType.SQUARE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "RightButton")
						{
							buttonSkin.sprite = this.rightMouseButton;
							buttonSkin.symbol = "";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (buttonName == "Backspace")
					{
						buttonSkin.sprite = this.rectangleKey;
						buttonSkin.symbol = "Bksp";
						buttonSkin.skinType = ButtonSkinType.WIDE;
						return buttonSkin;
					}
				}
				else if (num <= 1898928778U)
				{
					if (num != 1732852044U)
					{
						if (num == 1898928778U)
						{
							if (buttonName == "Slash")
							{
								buttonSkin.sprite = this.squareKey;
								buttonSkin.symbol = "/";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "LeftBracket")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "[";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num != 1985520332U)
				{
					if (num != 2044664427U)
					{
						if (num == 2118290244U)
						{
							if (buttonName == "LeftButton")
							{
								buttonSkin.sprite = this.leftMouseButton;
								buttonSkin.symbol = "";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "PadPeriod")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = ".";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (buttonName == "Left Bracket")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.symbol = "[";
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (num <= 3250860581U)
			{
				if (num <= 2848837449U)
				{
					if (num <= 2434225852U)
					{
						if (num != 2267317284U)
						{
							if (num == 2434225852U)
							{
								if (buttonName == "RightAlt")
								{
									buttonSkin.sprite = this.rectangleKey;
									buttonSkin.symbol = "R Alt";
									buttonSkin.skinType = ButtonSkinType.WIDE;
									return buttonSkin;
								}
							}
						}
						else if (buttonName == "Period")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.symbol = ".";
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
					else if (num != 2769091631U)
					{
						if (num == 2848837449U)
						{
							if (buttonName == "LeftArrow")
							{
								buttonSkin.sprite = this.leftArrowKey;
								buttonSkin.symbol = "";
								buttonSkin.skinType = ButtonSkinType.SQUARE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "CapsLock")
					{
						buttonSkin.sprite = this.rectangleKey;
						buttonSkin.symbol = "CpsLk";
						buttonSkin.skinType = ButtonSkinType.WIDE;
						return buttonSkin;
					}
				}
				else if (num <= 3036628469U)
				{
					if (num != 2988002186U)
					{
						if (num == 3036628469U)
						{
							if (buttonName == "LeftControl")
							{
								buttonSkin.sprite = this.rectangleKey;
								buttonSkin.symbol = "L Ctrl";
								buttonSkin.skinType = ButtonSkinType.WIDE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "PadMultiply")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "*";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num != 3082514982U)
				{
					if (num != 3241480638U)
					{
						if (num == 3250860581U)
						{
							if (buttonName == "Space")
							{
								buttonSkin.sprite = this.rectangleKey;
								buttonSkin.skinType = ButtonSkinType.WIDE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "PageDown")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "PgDn";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (buttonName == "Escape")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.symbol = "Esc";
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (num <= 3592460967U)
			{
				if (num <= 3422663135U)
				{
					if (num != 3388260431U)
					{
						if (num == 3422663135U)
						{
							if (buttonName == "Return")
							{
								buttonSkin.sprite = this.rectangleKey;
								buttonSkin.symbol = "Enter";
								buttonSkin.skinType = ButtonSkinType.WIDE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "Minus")
					{
						buttonSkin.sprite = this.squareKey;
						buttonSkin.symbol = "-";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (num != 3482547786U)
				{
					if (num != 3583141561U)
					{
						if (num == 3592460967U)
						{
							if (buttonName == "RightShift")
							{
								buttonSkin.sprite = this.rectangleKey;
								buttonSkin.symbol = "R Shift";
								buttonSkin.skinType = ButtonSkinType.WIDE;
								return buttonSkin;
							}
						}
					}
					else if (buttonName == "UpArrow")
					{
						buttonSkin.sprite = this.upArrowKey;
						buttonSkin.symbol = "";
						buttonSkin.skinType = ButtonSkinType.SQUARE;
						return buttonSkin;
					}
				}
				else if (buttonName == "End")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.symbol = "End";
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (num <= 3720178443U)
			{
				if (num != 3703400824U)
				{
					if (num == 3720178443U)
					{
						if (buttonName == "F11")
						{
							buttonSkin.sprite = this.squareKey;
							buttonSkin.skinType = ButtonSkinType.SQUARE;
							return buttonSkin;
						}
					}
				}
				else if (buttonName == "F10")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (num != 3736956062U)
			{
				if (num != 3762169905U)
				{
					if (num == 4219689196U)
					{
						if (buttonName == "Tab")
						{
							buttonSkin.sprite = this.rectangleKey;
							buttonSkin.skinType = ButtonSkinType.WIDE;
							return buttonSkin;
						}
					}
				}
				else if (buttonName == "Right Bracket")
				{
					buttonSkin.sprite = this.squareKey;
					buttonSkin.symbol = "]";
					buttonSkin.skinType = ButtonSkinType.SQUARE;
					return buttonSkin;
				}
			}
			else if (buttonName == "F12")
			{
				buttonSkin.sprite = this.squareKey;
				buttonSkin.skinType = ButtonSkinType.SQUARE;
				return buttonSkin;
			}
			buttonSkin.skinType = ButtonSkinType.BLANK;
			buttonSkin.symbol = buttonName.Replace("Button", "Btn");
		}
		return buttonSkin;
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x00124859 File Offset: 0x00122A59
	private void SetupRefs()
	{
		this.ih = GameManager.instance.inputHandler;
		this.active = true;
		if (this.ih)
		{
			this.ih.RefreshActiveControllerEvent += this.OnControllerConnected;
		}
	}

	// Token: 0x0600422F RID: 16943 RVA: 0x00124896 File Offset: 0x00122A96
	private void OnControllerConnected()
	{
		if (this.ih != null)
		{
			this.ih.StartCoroutine(this.InitialButtonMappingSetup());
		}
	}

	// Token: 0x06004230 RID: 16944 RVA: 0x001248B8 File Offset: 0x00122AB8
	private void GetSkin(ButtonSkin skin, InputControlType inputControlType)
	{
		switch (inputControlType)
		{
		case InputControlType.DPadUp:
			skin.sprite = this.dpadUp;
			return;
		case InputControlType.DPadDown:
			skin.sprite = this.dpadDown;
			return;
		case InputControlType.DPadLeft:
			skin.sprite = this.dpadLeft;
			return;
		case InputControlType.DPadRight:
			skin.sprite = this.dpadRight;
			break;
		case InputControlType.LeftTrigger:
			skin.sprite = this.ps4lt;
			return;
		case InputControlType.RightTrigger:
			skin.sprite = this.ps4rt;
			return;
		case InputControlType.LeftBumper:
			skin.sprite = this.ps4lb;
			return;
		case InputControlType.RightBumper:
			skin.sprite = this.ps4rb;
			return;
		case InputControlType.Action1:
			skin.sprite = this.ps4x;
			return;
		case InputControlType.Action2:
			skin.sprite = this.ps4circle;
			return;
		case InputControlType.Action3:
			skin.sprite = this.ps4square;
			return;
		case InputControlType.Action4:
			skin.sprite = this.ps4triangle;
			return;
		default:
			switch (inputControlType)
			{
			case InputControlType.Start:
				skin.sprite = this.start;
				return;
			case InputControlType.Select:
				skin.sprite = this.select;
				return;
			case InputControlType.System:
			case InputControlType.Pause:
			case InputControlType.Menu:
				break;
			case InputControlType.Options:
				skin.sprite = this.options;
				return;
			case InputControlType.Share:
				skin.sprite = this.share;
				return;
			default:
				if (inputControlType != InputControlType.TouchPadButton)
				{
					return;
				}
				skin.sprite = this.touchpadButton;
				return;
			}
			break;
		}
	}

	// Token: 0x04004392 RID: 17298
	[Header("Empty Button")]
	public Sprite blankKey;

	// Token: 0x04004393 RID: 17299
	[Header("Keyboard Button Skins")]
	public Sprite squareKey;

	// Token: 0x04004394 RID: 17300
	public Sprite rectangleKey;

	// Token: 0x04004395 RID: 17301
	public Sprite upArrowKey;

	// Token: 0x04004396 RID: 17302
	public Sprite downArrowKey;

	// Token: 0x04004397 RID: 17303
	public Sprite leftArrowKey;

	// Token: 0x04004398 RID: 17304
	public Sprite rightArrowKey;

	// Token: 0x04004399 RID: 17305
	[Space]
	public Sprite leftMouseButton;

	// Token: 0x0400439A RID: 17306
	public Sprite rightMouseButton;

	// Token: 0x0400439B RID: 17307
	public Sprite middleMouseButton;

	// Token: 0x0400439C RID: 17308
	[Header("Default Font Settings")]
	public int labelFontSize;

	// Token: 0x0400439D RID: 17309
	public TextAnchor labelAlignment;

	// Token: 0x0400439E RID: 17310
	[Space(10f)]
	public int buttonFontSize;

	// Token: 0x0400439F RID: 17311
	public TextAnchor buttonAlignment;

	// Token: 0x040043A0 RID: 17312
	[Space(10f)]
	public int wideButtonFontSize;

	// Token: 0x040043A1 RID: 17313
	public TextAnchor wideButtonAlignment;

	// Token: 0x040043A2 RID: 17314
	[Header("Universal Controller Buttons")]
	public Sprite a;

	// Token: 0x040043A3 RID: 17315
	public Sprite b;

	// Token: 0x040043A4 RID: 17316
	public Sprite x;

	// Token: 0x040043A5 RID: 17317
	public Sprite y;

	// Token: 0x040043A6 RID: 17318
	public Sprite lb;

	// Token: 0x040043A7 RID: 17319
	public Sprite lt;

	// Token: 0x040043A8 RID: 17320
	public Sprite rb;

	// Token: 0x040043A9 RID: 17321
	public Sprite rt;

	// Token: 0x040043AA RID: 17322
	public Sprite start;

	// Token: 0x040043AB RID: 17323
	public Sprite select;

	// Token: 0x040043AC RID: 17324
	public Sprite dpadLeft;

	// Token: 0x040043AD RID: 17325
	public Sprite dpadRight;

	// Token: 0x040043AE RID: 17326
	public Sprite dpadUp;

	// Token: 0x040043AF RID: 17327
	public Sprite dpadDown;

	// Token: 0x040043B0 RID: 17328
	[Header("XBone Controller Buttons")]
	public Sprite view;

	// Token: 0x040043B1 RID: 17329
	public Sprite menu;

	// Token: 0x040043B2 RID: 17330
	[Header("PS4 Controller Buttons")]
	public Sprite options;

	// Token: 0x040043B3 RID: 17331
	public Sprite share;

	// Token: 0x040043B4 RID: 17332
	public Sprite touchpadButton;

	// Token: 0x040043B5 RID: 17333
	public Sprite ps4x;

	// Token: 0x040043B6 RID: 17334
	public Sprite ps4square;

	// Token: 0x040043B7 RID: 17335
	public Sprite ps4triangle;

	// Token: 0x040043B8 RID: 17336
	public Sprite ps4circle;

	// Token: 0x040043B9 RID: 17337
	public Sprite ps4lb;

	// Token: 0x040043BA RID: 17338
	public Sprite ps4lt;

	// Token: 0x040043BB RID: 17339
	public Sprite ps4rb;

	// Token: 0x040043BC RID: 17340
	public Sprite ps4rt;

	// Token: 0x040043BD RID: 17341
	[Header("PS5 Controller Buttons")]
	[SerializeField]
	private UIButtonSkins.SonySkin ps5;

	// Token: 0x040043BE RID: 17342
	[Header("Switch HID Buttons")]
	[SerializeField]
	private Sprite switchHidB;

	// Token: 0x040043BF RID: 17343
	[SerializeField]
	private Sprite switchHidA;

	// Token: 0x040043C0 RID: 17344
	[SerializeField]
	private Sprite switchHidY;

	// Token: 0x040043C1 RID: 17345
	[SerializeField]
	private Sprite switchHidX;

	// Token: 0x040043C2 RID: 17346
	[SerializeField]
	private Sprite switchHidLeftBumper;

	// Token: 0x040043C3 RID: 17347
	[SerializeField]
	private Sprite switchHidLeftTrigger;

	// Token: 0x040043C4 RID: 17348
	[SerializeField]
	private Sprite switchHidRightBumper;

	// Token: 0x040043C5 RID: 17349
	[SerializeField]
	private Sprite switchHidRightTrigger;

	// Token: 0x040043C6 RID: 17350
	[SerializeField]
	private Sprite switchHidMinus;

	// Token: 0x040043C7 RID: 17351
	[SerializeField]
	private Sprite switchHidPlus;

	// Token: 0x040043C8 RID: 17352
	[SerializeField]
	private Sprite switchHidDPadUp;

	// Token: 0x040043C9 RID: 17353
	[SerializeField]
	private Sprite switchHidDPadDown;

	// Token: 0x040043CA RID: 17354
	[SerializeField]
	private Sprite switchHidDPadLeft;

	// Token: 0x040043CB RID: 17355
	[SerializeField]
	private Sprite switchHidDPadRight;

	// Token: 0x040043CC RID: 17356
	private bool active;

	// Token: 0x040043CD RID: 17357
	private GameManager gm;

	// Token: 0x040043CE RID: 17358
	private InputHandler ih;

	// Token: 0x040043CF RID: 17359
	[Header("Keyboard Menu")]
	public RectTransform mappableKeyboardButtons;

	// Token: 0x040043D0 RID: 17360
	[Header("Controller Menu")]
	public RectTransform mappableControllerButtons;

	// Token: 0x02001A22 RID: 6690
	[Serializable]
	private class SonySkin
	{
		// Token: 0x040098AF RID: 39087
		public Sprite options;

		// Token: 0x040098B0 RID: 39088
		public Sprite share;

		// Token: 0x040098B1 RID: 39089
		public Sprite touchpadButton;

		// Token: 0x040098B2 RID: 39090
		public Sprite cross;

		// Token: 0x040098B3 RID: 39091
		public Sprite square;

		// Token: 0x040098B4 RID: 39092
		public Sprite triangle;

		// Token: 0x040098B5 RID: 39093
		public Sprite circle;

		// Token: 0x040098B6 RID: 39094
		public Sprite lb;

		// Token: 0x040098B7 RID: 39095
		public Sprite lt;

		// Token: 0x040098B8 RID: 39096
		public Sprite rb;

		// Token: 0x040098B9 RID: 39097
		public Sprite rt;
	}
}
