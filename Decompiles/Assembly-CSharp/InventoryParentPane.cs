using System;
using UnityEngine;

// Token: 0x020006B5 RID: 1717
public class InventoryParentPane : InventoryPane
{
	// Token: 0x17000719 RID: 1817
	// (get) Token: 0x06003DE8 RID: 15848 RVA: 0x0010FF30 File Offset: 0x0010E130
	public override bool IsAvailable
	{
		get
		{
			if (!base.IsAvailable)
			{
				return false;
			}
			InventoryPane[] array = this.subPanes;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAvailable)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x06003DE9 RID: 15849 RVA: 0x0010FF6C File Offset: 0x0010E16C
	protected override void Awake()
	{
		base.Awake();
		InventoryPane[] array = this.subPanes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RootPane = this;
		}
	}

	// Token: 0x06003DEA RID: 15850 RVA: 0x0010FFA0 File Offset: 0x0010E1A0
	public override InventoryPane Get()
	{
		int num = 0;
		InventoryPane result = null;
		foreach (InventoryPane inventoryPane in this.subPanes)
		{
			if (inventoryPane.IsAvailable)
			{
				num++;
				result = inventoryPane;
			}
		}
		if (num == 0)
		{
			return base.Get();
		}
		if (num <= 1)
		{
			return result;
		}
		return base.Get();
	}

	// Token: 0x04003F85 RID: 16261
	[Space]
	[SerializeField]
	private InventoryPane[] subPanes;
}
