using System;
using UnityEngine;

// Token: 0x0200074E RID: 1870
[Serializable]
public class AssetLinker<T> : IAssetLinker where T : Object
{
	// Token: 0x17000786 RID: 1926
	// (get) Token: 0x06004282 RID: 17026 RVA: 0x001258A4 File Offset: 0x00123AA4
	// (set) Token: 0x06004283 RID: 17027 RVA: 0x001258AC File Offset: 0x00123AAC
	public T Asset
	{
		get
		{
			return this.asset;
		}
		set
		{
			this.asset = value;
		}
	}

	// Token: 0x06004284 RID: 17028 RVA: 0x001258B5 File Offset: 0x00123AB5
	public Object GetAsset()
	{
		return this.Asset;
	}

	// Token: 0x06004285 RID: 17029 RVA: 0x001258C2 File Offset: 0x00123AC2
	public void SetAsset(Object asset)
	{
		this.Asset = (asset as T);
	}

	// Token: 0x06004286 RID: 17030 RVA: 0x001258D5 File Offset: 0x00123AD5
	public Type GetAssetType()
	{
		return typeof(T);
	}

	// Token: 0x06004287 RID: 17031 RVA: 0x001258E4 File Offset: 0x00123AE4
	public static implicit operator T(AssetLinker<T> assetLinker)
	{
		if (assetLinker != null)
		{
			return assetLinker.Asset;
		}
		return default(T);
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x00125904 File Offset: 0x00123B04
	public static implicit operator AssetLinker<T>(T asset)
	{
		return new AssetLinker<T>
		{
			Asset = asset
		};
	}

	// Token: 0x04004411 RID: 17425
	[SerializeField]
	private string guid;

	// Token: 0x04004412 RID: 17426
	[NonSerialized]
	private T asset;
}
