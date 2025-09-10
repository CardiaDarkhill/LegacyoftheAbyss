using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class ChainLinkBreak : MonoBehaviour
{
	// Token: 0x06001455 RID: 5205 RVA: 0x0005B670 File Offset: 0x00059870
	public void Break()
	{
		this.breakEffects.transform.parent = null;
		this.breakEffects.SetActive(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001292 RID: 4754
	public GameObject breakEffects;
}
