using System;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class DeparentAndFollow : MonoBehaviour
{
	// Token: 0x0600138F RID: 5007 RVA: 0x00059400 File Offset: 0x00057600
	private void Start()
	{
		this.parent = base.transform.parent;
		this.offset = base.transform.localPosition;
		base.transform.parent = null;
		if (this.parent == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x00059450 File Offset: 0x00057650
	private void Update()
	{
		if (this.parent == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.transform.position = this.parent.position + this.offset;
	}

	// Token: 0x040011F6 RID: 4598
	private Transform parent;

	// Token: 0x040011F7 RID: 4599
	private Vector3 offset;
}
