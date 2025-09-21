using System;
using UnityEngine;

// Token: 0x02000759 RID: 1881
public class CopyPosition : MonoBehaviour
{
	// Token: 0x060042AD RID: 17069 RVA: 0x00125E4B File Offset: 0x0012404B
	private void Start()
	{
		this.hasCopyTarget = this.copyTarget;
		this.DoCopyPosition();
		if (!this.everyFrame)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060042AE RID: 17070 RVA: 0x00125E73 File Offset: 0x00124073
	private void Update()
	{
		this.DoCopyPosition();
	}

	// Token: 0x060042AF RID: 17071 RVA: 0x00125E7B File Offset: 0x0012407B
	private void DoCopyPosition()
	{
		if (!this.hasCopyTarget)
		{
			return;
		}
		if (this.useWorldSpace)
		{
			base.transform.position = this.copyTarget.position;
			return;
		}
		base.transform.localPosition = this.copyTarget.localPosition;
	}

	// Token: 0x04004506 RID: 17670
	[SerializeField]
	private Transform copyTarget;

	// Token: 0x04004507 RID: 17671
	[SerializeField]
	private bool useWorldSpace;

	// Token: 0x04004508 RID: 17672
	[SerializeField]
	private bool everyFrame;

	// Token: 0x04004509 RID: 17673
	private bool hasCopyTarget;
}
