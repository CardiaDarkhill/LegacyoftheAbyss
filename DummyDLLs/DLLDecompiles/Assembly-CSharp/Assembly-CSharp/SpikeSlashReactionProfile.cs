using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200055C RID: 1372
[CreateAssetMenu(menuName = "Profiles/Spike Slash Reaction")]
public sealed class SpikeSlashReactionProfile : ScriptableObject
{
	// Token: 0x0600310E RID: 12558 RVA: 0x000D9A5C File Offset: 0x000D7C5C
	public void SpawnEffect(Vector3 position, Quaternion rotation)
	{
		this.spawnedEffects.RemoveAll((GameObject o) => o == null);
		foreach (GameObject prefab in this.spawnedEffects)
		{
			prefab.Spawn(position, rotation);
		}
	}

	// Token: 0x04003459 RID: 13401
	[SerializeField]
	private List<GameObject> spawnedEffects = new List<GameObject>();
}
