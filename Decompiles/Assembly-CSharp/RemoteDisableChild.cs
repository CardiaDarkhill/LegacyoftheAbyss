using System;
using UnityEngine;

// Token: 0x02000700 RID: 1792
public class RemoteDisableChild : MonoBehaviour
{
	// Token: 0x0600400E RID: 16398 RVA: 0x0011A5E2 File Offset: 0x001187E2
	public void RemoteDisableObject()
	{
		this.child.SetActive(false);
	}

	// Token: 0x0600400F RID: 16399 RVA: 0x0011A5F0 File Offset: 0x001187F0
	public void RemoteEnableObject()
	{
		this.child.SetActive(true);
	}

	// Token: 0x040041BE RID: 16830
	public GameObject child;
}
