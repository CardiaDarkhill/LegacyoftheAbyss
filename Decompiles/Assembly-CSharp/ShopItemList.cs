using System;
using UnityEngine;

// Token: 0x0200071F RID: 1823
[CreateAssetMenu(menuName = "Hornet/Shop Item List")]
public class ShopItemList : ScriptableObject
{
	// Token: 0x1700076B RID: 1899
	// (get) Token: 0x060040E5 RID: 16613 RVA: 0x0011CFEF File Offset: 0x0011B1EF
	public ShopItem[] ShopItems
	{
		get
		{
			return this.shopItems;
		}
	}

	// Token: 0x04004261 RID: 16993
	[SerializeField]
	private ShopItem[] shopItems;
}
