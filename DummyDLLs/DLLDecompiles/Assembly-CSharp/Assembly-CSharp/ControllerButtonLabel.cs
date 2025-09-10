using System;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200061A RID: 1562
[RequireComponent(typeof(Text))]
public class ControllerButtonLabel : MonoBehaviour
{
	// Token: 0x06003799 RID: 14233 RVA: 0x000F51C8 File Offset: 0x000F33C8
	private void Awake()
	{
		this.ih = GameManager.instance.inputHandler;
		this.ui = UIManager.instance;
		this.buttonText = base.GetComponent<Text>();
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x000F51F1 File Offset: 0x000F33F1
	private void OnEnable()
	{
		this.UpdateLanguage();
		Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x000F520A File Offset: 0x000F340A
	private void OnDisable()
	{
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
	}

	// Token: 0x0600379C RID: 14236 RVA: 0x000F521D File Offset: 0x000F341D
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.UpdateLanguage();
		}
	}

	// Token: 0x0600379D RID: 14237 RVA: 0x000F5228 File Offset: 0x000F3428
	private void UpdateLanguage()
	{
		if (!string.IsNullOrEmpty(this.overrideLabelKey))
		{
			this.buttonText.text = Language.Get(this.overrideLabelKey, "MainMenu");
			return;
		}
		this.ShowCurrentBinding();
	}

	// Token: 0x0600379E RID: 14238 RVA: 0x000F525C File Offset: 0x000F345C
	private void ShowCurrentBinding()
	{
		this.buttonText.text = "+";
		if (this.controllerButton == InputControlType.None)
		{
			this.buttonText.text = Language.Get("CTRL_UNMAPPED", "MainMenu");
			return;
		}
		PlayerAction playerAction = this.ih.GetActionForMappableControllerButton(this.controllerButton);
		if (playerAction != null)
		{
			this.buttonText.text = Language.Get(this.ih.ActionButtonLocalizedKey(playerAction), "MainMenu");
			return;
		}
		playerAction = this.ih.GetActionForDefaultControllerButton(this.controllerButton);
		if (playerAction != null)
		{
			this.buttonText.text = Language.Get(this.ih.ActionButtonLocalizedKey(playerAction), "MainMenu");
			return;
		}
		this.buttonText.text = Language.Get("CTRL_UNMAPPED", "MainMenu");
	}

	// Token: 0x04003A91 RID: 14993
	[Header("Button Text")]
	private Text buttonText;

	// Token: 0x04003A92 RID: 14994
	[Header("Button Label")]
	public string overrideLabelKey;

	// Token: 0x04003A93 RID: 14995
	[Header("Button Mapping")]
	public InputControlType controllerButton;

	// Token: 0x04003A94 RID: 14996
	private InputHandler ih;

	// Token: 0x04003A95 RID: 14997
	private UIManager ui;
}
