using System;
using UnityEngine;

// Token: 0x0200074F RID: 1871
public interface IAssetLinker
{
	// Token: 0x0600428A RID: 17034
	Object GetAsset();

	// Token: 0x0600428B RID: 17035
	void SetAsset(Object asset);

	// Token: 0x0600428C RID: 17036
	Type GetAssetType();
}
