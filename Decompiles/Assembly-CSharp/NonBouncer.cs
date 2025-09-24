using System;
using UnityEngine;

// Token: 0x02000446 RID: 1094
public class NonBouncer : MonoBehaviour
{
	// Token: 0x06002680 RID: 9856 RVA: 0x000AE57A File Offset: 0x000AC77A
	public void SetActive(bool set_active)
	{
		this.active = set_active;
	}

	// Token: 0x040023DD RID: 9181
	public bool active = true;
}
