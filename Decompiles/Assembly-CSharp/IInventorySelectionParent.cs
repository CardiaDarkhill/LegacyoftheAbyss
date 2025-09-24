using System;

// Token: 0x0200069F RID: 1695
public interface IInventorySelectionParent
{
	// Token: 0x06003C73 RID: 15475
	InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction);
}
