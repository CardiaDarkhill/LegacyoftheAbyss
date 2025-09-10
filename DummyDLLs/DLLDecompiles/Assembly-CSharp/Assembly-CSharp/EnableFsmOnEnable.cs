using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class EnableFsmOnEnable : MonoBehaviour
{
	// Token: 0x060016ED RID: 5869 RVA: 0x00067CF8 File Offset: 0x00065EF8
	private void Reset()
	{
		this.fsm = base.GetComponent<PlayMakerFSM>();
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x00067D06 File Offset: 0x00065F06
	private void OnEnable()
	{
		this.fsm.enabled = true;
	}

	// Token: 0x04001585 RID: 5509
	[SerializeField]
	private PlayMakerFSM fsm;
}
