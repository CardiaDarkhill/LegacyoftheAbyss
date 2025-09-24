using System;
using UnityEngine;

// Token: 0x02000723 RID: 1827
public class ShopOwner : ShopOwnerBase
{
	// Token: 0x0600412A RID: 16682 RVA: 0x0011DFEE File Offset: 0x0011C1EE
	private void OnValidate()
	{
		if (this.stockList && this.stock != null && this.stock.Length != 0)
		{
			this.stock = new ShopItem[0];
		}
	}

	// Token: 0x17000772 RID: 1906
	// (get) Token: 0x0600412B RID: 16683 RVA: 0x0011E01A File Offset: 0x0011C21A
	public override ShopItem[] Stock
	{
		get
		{
			if (!this.stockList)
			{
				return this.stock;
			}
			return this.stockList.ShopItems;
		}
	}

	// Token: 0x04004285 RID: 17029
	[SerializeField]
	private ShopItemList stockList;

	// Token: 0x04004286 RID: 17030
	[SerializeField]
	private ShopItem[] stock;
}
