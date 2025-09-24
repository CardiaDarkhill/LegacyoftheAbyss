using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000364 RID: 868
public sealed class RandomiseEnabledChild : MonoBehaviour
{
	// Token: 0x06001DF7 RID: 7671 RVA: 0x0008A890 File Offset: 0x00088A90
	private void Awake()
	{
		this.targets.RemoveAll((GameObject o) => o == null);
	}

	// Token: 0x06001DF8 RID: 7672 RVA: 0x0008A8C0 File Offset: 0x00088AC0
	private void OnEnable()
	{
		int num = Random.Range(0, this.targets.Count);
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].SetActive(i == num);
		}
	}

	// Token: 0x04001D1C RID: 7452
	[SerializeField]
	private List<GameObject> targets = new List<GameObject>();
}
