using System;

// Token: 0x02000685 RID: 1669
public class InventoryItemButtonPrompt : InventoryItemButtonPromptBase<InventoryItemButtonPromptData>
{
	// Token: 0x06003BA0 RID: 15264 RVA: 0x00106523 File Offset: 0x00104723
	protected override void OnShow(InventoryItemButtonPromptDisplayList displayList, InventoryItemButtonPromptData data)
	{
		displayList.Append(data, false, this.order);
	}
}
