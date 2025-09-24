using System;
using UnityEngine;

// Token: 0x02000778 RID: 1912
public static class ObjectPoolAuditor
{
	// Token: 0x06004406 RID: 17414 RVA: 0x0012A86E File Offset: 0x00128A6E
	public static void RecordPoolCreated(GameObject prefab, int initialPoolSize)
	{
	}

	// Token: 0x06004407 RID: 17415 RVA: 0x0012A870 File Offset: 0x00128A70
	public static void RecordSpawned(GameObject prefab, bool didInstantiate)
	{
	}

	// Token: 0x06004408 RID: 17416 RVA: 0x0012A872 File Offset: 0x00128A72
	public static void RecordDespawned(GameObject instanceOrPrefab, bool willReuse)
	{
	}
}
