using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020006A1 RID: 1697
public class InventoryItemSelectableButtonEvent : InventoryItemSelectableDirectional
{
	// Token: 0x170006E2 RID: 1762
	// (get) Token: 0x06003C90 RID: 15504 RVA: 0x0010A0A2 File Offset: 0x001082A2
	// (set) Token: 0x06003C91 RID: 15505 RVA: 0x0010A0AA File Offset: 0x001082AA
	public LocalisedString InteractionText
	{
		get
		{
			return this.interactionText;
		}
		set
		{
			this.interactionText = value;
			if (this.isSelected)
			{
				this.UpdateInteractionText();
			}
		}
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x0010A0C1 File Offset: 0x001082C1
	public override bool Submit()
	{
		if (this.ButtonActivated != null)
		{
			this.ButtonActivated();
			return true;
		}
		return base.Submit();
	}

	// Token: 0x06003C93 RID: 15507 RVA: 0x0010A0DE File Offset: 0x001082DE
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		this.isSelected = true;
		this.UpdateInteractionText();
	}

	// Token: 0x06003C94 RID: 15508 RVA: 0x0010A0F4 File Offset: 0x001082F4
	public override void Deselect()
	{
		base.Deselect();
		this.isSelected = false;
		if (this.interactionDisplayText)
		{
			this.interactionDisplayText.Text = this.previousInteractionText;
		}
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x0010A121 File Offset: 0x00108321
	private void UpdateInteractionText()
	{
		if (this.interactionDisplayText)
		{
			this.previousInteractionText = this.interactionDisplayText.Text;
			this.interactionDisplayText.Text = this.interactionText;
		}
	}

	// Token: 0x04003E5E RID: 15966
	public Action ButtonActivated;

	// Token: 0x04003E5F RID: 15967
	[SerializeField]
	private SetTextMeshProGameText interactionDisplayText;

	// Token: 0x04003E60 RID: 15968
	[SerializeField]
	private LocalisedString interactionText;

	// Token: 0x04003E61 RID: 15969
	private LocalisedString previousInteractionText;

	// Token: 0x04003E62 RID: 15970
	private bool isSelected;
}
