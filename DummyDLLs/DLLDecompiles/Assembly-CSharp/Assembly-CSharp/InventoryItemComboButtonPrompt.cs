using System;

// Token: 0x02000689 RID: 1673
public class InventoryItemComboButtonPrompt : InventoryItemButtonPromptBase<InventoryItemComboButtonPromptDisplay.Display>
{
	// Token: 0x06003BBB RID: 15291 RVA: 0x00106ADE File Offset: 0x00104CDE
	protected override void OnShow(InventoryItemButtonPromptDisplayList displayList, InventoryItemComboButtonPromptDisplay.Display data)
	{
		displayList.Append(data, this.order);
	}
}
