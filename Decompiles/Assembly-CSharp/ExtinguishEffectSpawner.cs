using System;
using UnityEngine;

// Token: 0x0200022F RID: 559
public sealed class ExtinguishEffectSpawner : MonoBehaviour
{
	// Token: 0x06001497 RID: 5271 RVA: 0x0005CC83 File Offset: 0x0005AE83
	public void PlayEffect()
	{
		this.PlayEffect(base.transform.position);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x0005CC96 File Offset: 0x0005AE96
	public void PlayEffect(Vector3 position)
	{
		if (this.effectPrefab == null)
		{
			return;
		}
		this.effectPrefab.Spawn(position);
	}

	// Token: 0x040012F2 RID: 4850
	[SerializeField]
	private GameObject effectPrefab;
}
