using System;
using UnityEngine;

// Token: 0x020001BA RID: 442
public interface ICollectableUIMsgItem : IUIMsgPopupItem
{
	// Token: 0x06001134 RID: 4404
	Sprite GetUIMsgSprite();

	// Token: 0x06001135 RID: 4405
	string GetUIMsgName();

	// Token: 0x06001136 RID: 4406
	float GetUIMsgIconScale();

	// Token: 0x06001137 RID: 4407
	bool HasUpgradeIcon();
}
