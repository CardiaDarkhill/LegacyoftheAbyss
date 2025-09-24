using System;
using GlobalEnums;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class InventoryItemComboButtonPromptDisplay : MonoBehaviour, InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder
{
	// Token: 0x170006C0 RID: 1728
	// (get) Token: 0x06003BBD RID: 15293 RVA: 0x00106AF5 File Offset: 0x00104CF5
	// (set) Token: 0x06003BBE RID: 15294 RVA: 0x00106AFD File Offset: 0x00104CFD
	public int order { get; set; }

	// Token: 0x06003BBF RID: 15295 RVA: 0x00106B08 File Offset: 0x00104D08
	public void Show(InventoryItemComboButtonPromptDisplay.Display display)
	{
		base.gameObject.SetActive(true);
		ActionButtonIcon actionButtonIcon;
		switch (display.DirectionModifier)
		{
		case AttackToolBinding.Neutral:
			this.parentWithModifier.SetActive(false);
			this.parentWithoutModifier.SetActive(true);
			actionButtonIcon = this.actionButtonWithoutModifier;
			break;
		case AttackToolBinding.Up:
			this.parentWithModifier.SetActive(true);
			this.parentWithoutModifier.SetActive(false);
			this.modifierUp.SetActive(true);
			this.modifierDown.SetActive(false);
			actionButtonIcon = this.actionButtonWithModifier;
			break;
		case AttackToolBinding.Down:
			this.parentWithModifier.SetActive(true);
			this.parentWithoutModifier.SetActive(false);
			this.modifierUp.SetActive(false);
			this.modifierDown.SetActive(true);
			actionButtonIcon = this.actionButtonWithModifier;
			break;
		default:
			throw new NotImplementedException();
		}
		actionButtonIcon.SetAction(display.ActionButton);
		this.promptText.text = display.PromptText;
		SetTextMeshProGameText component = this.promptText.GetComponent<SetTextMeshProGameText>();
		if (component != null)
		{
			component.Text = display.PromptText;
		}
		if (this.modifierHoldPrompt)
		{
			this.modifierHoldPrompt.SetActive(display.ShowHold);
		}
	}

	// Token: 0x06003BC0 RID: 15296 RVA: 0x00106C39 File Offset: 0x00104E39
	public void Hide()
	{
		this.parentWithModifier.SetActive(false);
		this.parentWithoutModifier.SetActive(false);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003BC2 RID: 15298 RVA: 0x00106C67 File Offset: 0x00104E67
	Transform InventoryItemButtonPromptDisplayList.IPromptDisplayListOrder.get_transform()
	{
		return base.transform;
	}

	// Token: 0x04003DEA RID: 15850
	[SerializeField]
	private GameObject parentWithoutModifier;

	// Token: 0x04003DEB RID: 15851
	[SerializeField]
	private ActionButtonIcon actionButtonWithoutModifier;

	// Token: 0x04003DEC RID: 15852
	[SerializeField]
	private GameObject parentWithModifier;

	// Token: 0x04003DED RID: 15853
	[SerializeField]
	private ActionButtonIcon actionButtonWithModifier;

	// Token: 0x04003DEE RID: 15854
	[SerializeField]
	private GameObject modifierHoldPrompt;

	// Token: 0x04003DEF RID: 15855
	[SerializeField]
	private GameObject modifierUp;

	// Token: 0x04003DF0 RID: 15856
	[SerializeField]
	private GameObject modifierDown;

	// Token: 0x04003DF1 RID: 15857
	[SerializeField]
	private TMP_Text promptText;

	// Token: 0x0200198F RID: 6543
	[Serializable]
	public struct Display
	{
		// Token: 0x04009642 RID: 38466
		public HeroActionButton ActionButton;

		// Token: 0x04009643 RID: 38467
		public AttackToolBinding DirectionModifier;

		// Token: 0x04009644 RID: 38468
		public LocalisedString PromptText;

		// Token: 0x04009645 RID: 38469
		public bool ShowHold;
	}
}
