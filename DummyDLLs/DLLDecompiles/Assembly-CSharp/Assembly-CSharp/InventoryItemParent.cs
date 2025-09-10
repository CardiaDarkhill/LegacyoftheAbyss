using System;
using System.Collections.Generic;

// Token: 0x0200069E RID: 1694
public class InventoryItemParent : InventoryItemSelectableDirectional, IInventorySelectionParent
{
	// Token: 0x170006D9 RID: 1753
	// (get) Token: 0x06003C6C RID: 15468 RVA: 0x00109CF0 File Offset: 0x00107EF0
	public override string DisplayName
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006DA RID: 1754
	// (get) Token: 0x06003C6D RID: 15469 RVA: 0x00109CF7 File Offset: 0x00107EF7
	public override string Description
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006DB RID: 1755
	// (get) Token: 0x06003C6E RID: 15470 RVA: 0x00109CFE File Offset: 0x00107EFE
	protected override bool IsAutoNavSelectable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x00109D01 File Offset: 0x00107F01
	protected override void Awake()
	{
		this.manager = base.GetComponentInParent<InventoryItemManager>();
		this.childSelectables = new List<InventoryItemSelectable>(base.transform.childCount);
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x00109D28 File Offset: 0x00107F28
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		this.childSelectables.Clear();
		base.GetComponentsInChildren<InventoryItemSelectable>(this.childSelectables);
		this.childSelectables.Remove(this);
		if (this.childSelectables.Count <= 0)
		{
			return null;
		}
		if (direction != null)
		{
			return InventoryItemNavigationHelper.GetClosestOnAxis<InventoryItemSelectable>(direction.Value, this.manager.CurrentSelected, this.childSelectables);
		}
		return this.childSelectables[0];
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x00109D9C File Offset: 0x00107F9C
	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable source, InventoryItemManager.SelectionDirection? direction)
	{
		InventoryItemSelectable inventoryItemSelectable = (direction == null) ? null : this.GetNextSelectable(direction.Value);
		if (inventoryItemSelectable == source)
		{
			inventoryItemSelectable = null;
		}
		return inventoryItemSelectable;
	}

	// Token: 0x04003E56 RID: 15958
	private InventoryItemManager manager;

	// Token: 0x04003E57 RID: 15959
	private List<InventoryItemSelectable> childSelectables;
}
