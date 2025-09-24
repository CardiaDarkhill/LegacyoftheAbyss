using System;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public class KeepWorldScalePositiveLate : MonoBehaviour
{
	// Token: 0x060022BA RID: 8890 RVA: 0x0009F8E0 File Offset: 0x0009DAE0
	private void LateUpdate()
	{
		if (base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	// Token: 0x0400218E RID: 8590
	public bool x;

	// Token: 0x0400218F RID: 8591
	public bool y;
}
