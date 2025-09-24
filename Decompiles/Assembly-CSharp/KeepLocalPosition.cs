using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class KeepLocalPosition : MonoBehaviour
{
	// Token: 0x06001533 RID: 5427 RVA: 0x000601B8 File Offset: 0x0005E3B8
	private void OnEnable()
	{
		this.initialLocalPos = base.transform.localPosition;
		this.initialLocalScale = base.transform.localScale;
		this.lastLossyScale = base.transform.lossyScale;
		if (this.getPositionOnEnable)
		{
			this.xPosition = this.initialLocalPos.x;
			this.yPosition = this.initialLocalPos.y;
		}
	}

	// Token: 0x06001534 RID: 5428 RVA: 0x00060222 File Offset: 0x0005E422
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			base.transform.localPosition = this.initialLocalPos;
			base.transform.localScale = this.initialLocalScale;
		}
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x00060250 File Offset: 0x0005E450
	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (this.keepX)
		{
			localPosition.x = this.xPosition;
		}
		if (this.keepY)
		{
			localPosition.y = this.yPosition;
		}
		base.transform.localPosition = localPosition;
		if (this.keepScaleX || this.keepScaleY)
		{
			Vector3 vector = base.transform.localScale;
			Vector3 lossyScale = base.transform.lossyScale;
			if (this.keepScaleX && Mathf.Sign(lossyScale.x) != Mathf.Sign(this.lastLossyScale.x))
			{
				vector.x *= -1f;
			}
			if (this.keepScaleY && Mathf.Sign(lossyScale.y) != Mathf.Sign(this.lastLossyScale.y))
			{
				vector *= -1f;
			}
			base.transform.localScale = vector;
			this.lastLossyScale = base.transform.lossyScale;
		}
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x0006034D File Offset: 0x0005E54D
	public void SetKeepWorldPosition(bool value)
	{
		base.enabled = value;
	}

	// Token: 0x040013D0 RID: 5072
	public bool keepX;

	// Token: 0x040013D1 RID: 5073
	public float xPosition;

	// Token: 0x040013D2 RID: 5074
	public bool keepY;

	// Token: 0x040013D3 RID: 5075
	public float yPosition;

	// Token: 0x040013D4 RID: 5076
	[Space]
	public bool getPositionOnEnable;

	// Token: 0x040013D5 RID: 5077
	[Space]
	public bool keepScaleX;

	// Token: 0x040013D6 RID: 5078
	public bool keepScaleY;

	// Token: 0x040013D7 RID: 5079
	[Space]
	public bool resetOnDisable;

	// Token: 0x040013D8 RID: 5080
	private Vector3 initialLocalPos;

	// Token: 0x040013D9 RID: 5081
	private Vector3 initialLocalScale;

	// Token: 0x040013DA RID: 5082
	private Vector3 lastLossyScale;
}
