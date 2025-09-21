using System;
using UnityEngine;

// Token: 0x0200072A RID: 1834
public interface ISimpleShopItem
{
	// Token: 0x0600417F RID: 16767
	string GetDisplayName();

	// Token: 0x06004180 RID: 16768
	Sprite GetIcon();

	// Token: 0x06004181 RID: 16769
	int GetCost();

	// Token: 0x06004182 RID: 16770
	bool DelayPurchase();
}
