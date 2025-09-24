using System;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class DeactivateRandomly : MonoBehaviour
{
	// Token: 0x06001370 RID: 4976 RVA: 0x00058A97 File Offset: 0x00056C97
	private void Start()
	{
		if ((float)Random.Range(1, 100) <= this.deactivationChance)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011DC RID: 4572
	public float deactivationChance = 50f;
}
