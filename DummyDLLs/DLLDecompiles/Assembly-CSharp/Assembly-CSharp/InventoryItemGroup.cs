using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class InventoryItemGroup : InventoryItemSelectable
{
	// Token: 0x170006C8 RID: 1736
	// (get) Token: 0x06003BEC RID: 15340 RVA: 0x00107EE2 File Offset: 0x001060E2
	public override string DisplayName
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006C9 RID: 1737
	// (get) Token: 0x06003BED RID: 15341 RVA: 0x00107EE9 File Offset: 0x001060E9
	public override string Description
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x06003BEE RID: 15342 RVA: 0x00107EF0 File Offset: 0x001060F0
	protected void Awake()
	{
		this.itemManager = base.GetComponentInParent<InventoryItemManager>();
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x00107F00 File Offset: 0x00106100
	private void UpdateChildItems()
	{
		this.childItems.Clear();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.activeInHierarchy)
			{
				InventoryItemGroup component = transform.GetComponent<InventoryItemGroup>();
				if (component)
				{
					component.UpdateChildItems();
					this.childItems.AddRange(component.childItems);
				}
				else
				{
					InventoryItemSelectable component2 = transform.GetComponent<InventoryItemSelectable>();
					if (component2)
					{
						this.childItems.Add(component2);
					}
				}
			}
		}
	}

	// Token: 0x06003BF0 RID: 15344 RVA: 0x00107FB0 File Offset: 0x001061B0
	public override InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		this.UpdateChildItems();
		if (this.childItems.Count <= 0)
		{
			return null;
		}
		if (direction != null && this.itemManager && this.itemManager.CurrentSelected)
		{
			InventoryItemSelectable closestOnAxis = InventoryItemNavigationHelper.GetClosestOnAxis<InventoryItemSelectable>(direction.Value, this.itemManager.CurrentSelected, this.childItems);
			if (closestOnAxis)
			{
				return closestOnAxis.Get(direction);
			}
		}
		return this.childItems[0].Get(direction);
	}

	// Token: 0x06003BF1 RID: 15345 RVA: 0x0010803B File Offset: 0x0010623B
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		return this;
	}

	// Token: 0x04003E0D RID: 15885
	private readonly List<InventoryItemSelectable> childItems = new List<InventoryItemSelectable>();

	// Token: 0x04003E0E RID: 15886
	private InventoryItemManager itemManager;
}
