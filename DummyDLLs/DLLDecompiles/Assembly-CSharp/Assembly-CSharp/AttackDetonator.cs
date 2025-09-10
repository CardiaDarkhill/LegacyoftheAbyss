using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class AttackDetonator : MonoBehaviour
{
	// Token: 0x060007C4 RID: 1988 RVA: 0x00025536 File Offset: 0x00023736
	public void SetActive(bool set_active)
	{
		this.active = set_active;
	}

	// Token: 0x0400078A RID: 1930
	public bool active = true;
}
