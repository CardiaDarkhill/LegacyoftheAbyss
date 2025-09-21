using System;
using UnityEngine;

// Token: 0x020001BB RID: 443
public struct UIMsgDisplay : ICollectableUIMsgItem, IUIMsgPopupItem
{
	// Token: 0x06001138 RID: 4408 RVA: 0x00050E2C File Offset: 0x0004F02C
	public float GetUIMsgIconScale()
	{
		return this.IconScale;
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x00050E34 File Offset: 0x0004F034
	public bool HasUpgradeIcon()
	{
		return false;
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x00050E37 File Offset: 0x0004F037
	public string GetUIMsgName()
	{
		return this.Name;
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x00050E3F File Offset: 0x0004F03F
	public Sprite GetUIMsgSprite()
	{
		return this.Icon;
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x00050E47 File Offset: 0x0004F047
	public Object GetRepresentingObject()
	{
		return this.RepresentingObject;
	}

	// Token: 0x04001040 RID: 4160
	public string Name;

	// Token: 0x04001041 RID: 4161
	public Sprite Icon;

	// Token: 0x04001042 RID: 4162
	public float IconScale;

	// Token: 0x04001043 RID: 4163
	public Object RepresentingObject;
}
