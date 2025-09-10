using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F3 RID: 499
[CreateAssetMenu(menuName = "Hornet/Scene Object Pool")]
public class SceneObjectPool : ScriptableObject
{
	// Token: 0x06001339 RID: 4921 RVA: 0x00058280 File Offset: 0x00056480
	public void SpawnPool(GameObject owner)
	{
		if (this.gameObjectPool == null || this.gameObjectPool.Length == 0)
		{
			return;
		}
		owner.AddComponent<PersonalObjectPool>().startupPool = new List<StartupPool>(this.gameObjectPool);
	}

	// Token: 0x040011B0 RID: 4528
	[SerializeField]
	private StartupPool[] gameObjectPool;

	// Token: 0x040011B1 RID: 4529
	[Space]
	[SerializeField]
	private Object[] holdReferences;
}
