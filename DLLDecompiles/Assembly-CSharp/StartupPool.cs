using System;
using UnityEngine;

// Token: 0x0200035F RID: 863
[Serializable]
public struct StartupPool
{
	// Token: 0x06001DE5 RID: 7653 RVA: 0x0008A5E8 File Offset: 0x000887E8
	public StartupPool(int size, GameObject prefab, bool initialiseSpawnedObjects = false, bool shared = false)
	{
		this.size = size;
		this.prefab = prefab;
		this.SpawnedCount = 0;
		this.initialiseSpawnedObjects = initialiseSpawnedObjects;
		this.shared = shared;
	}

	// Token: 0x04001D09 RID: 7433
	public int size;

	// Token: 0x04001D0A RID: 7434
	public GameObject prefab;

	// Token: 0x04001D0B RID: 7435
	[NonSerialized]
	public int SpawnedCount;

	// Token: 0x04001D0C RID: 7436
	public bool initialiseSpawnedObjects;

	// Token: 0x04001D0D RID: 7437
	public bool shared;
}
