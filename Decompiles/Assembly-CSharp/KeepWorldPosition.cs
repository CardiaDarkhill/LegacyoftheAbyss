using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class KeepWorldPosition : MonoBehaviour
{
	// Token: 0x0600153E RID: 5438 RVA: 0x000604E2 File Offset: 0x0005E6E2
	private void Start()
	{
		this.Initialise();
		this.started = true;
	}

	// Token: 0x0600153F RID: 5439 RVA: 0x000604F1 File Offset: 0x0005E6F1
	private void OnEnable()
	{
		if (this.started)
		{
			this.Initialise();
		}
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x00060504 File Offset: 0x0005E704
	private void Initialise()
	{
		Transform transform = base.transform;
		this.initialLocalPos = transform.localPosition;
		this.initialLocalScale = transform.localScale;
		this.lastLossyScale = transform.lossyScale;
		if (this.getPositionOnEnable)
		{
			Vector3 position = transform.position;
			this.xPosition = position.x;
			this.yPosition = position.y;
		}
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x00060563 File Offset: 0x0005E763
	private void OnDisable()
	{
		if (!this.resetOnDisable || !this.started)
		{
			return;
		}
		Transform transform = base.transform;
		transform.localPosition = this.initialLocalPos;
		transform.localScale = this.initialLocalScale;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x00060594 File Offset: 0x0005E794
	private void Update()
	{
		Transform transform = base.transform;
		Vector3 position = transform.position;
		if (this.keepX)
		{
			position.x = this.xPosition;
		}
		if (this.keepY)
		{
			position.y = this.yPosition;
		}
		transform.position = position;
		if (this.keepScaleX || this.keepScaleY || this.deactivateIfFlippedOnX)
		{
			Vector3 vector = transform.localScale;
			Vector3 lossyScale = transform.lossyScale;
			if (Math.Abs(Mathf.Sign(lossyScale.x) - Mathf.Sign(this.lastLossyScale.x)) > Mathf.Epsilon)
			{
				if (this.keepScaleX)
				{
					vector.x *= -1f;
				}
				if (this.deactivateIfFlippedOnX)
				{
					base.gameObject.SetActive(false);
				}
			}
			if (this.keepScaleY && Math.Abs(Mathf.Sign(lossyScale.y) - Mathf.Sign(this.lastLossyScale.y)) > Mathf.Epsilon)
			{
				vector *= -1f;
			}
			transform.localScale = vector;
			this.lastLossyScale = transform.lossyScale;
		}
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000606AC File Offset: 0x0005E8AC
	public void SetKeepWorldPosition(bool value)
	{
		base.enabled = value;
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000606B5 File Offset: 0x0005E8B5
	public void ForceUpdate()
	{
		this.Update();
	}

	// Token: 0x040013DF RID: 5087
	public bool keepX;

	// Token: 0x040013E0 RID: 5088
	public float xPosition;

	// Token: 0x040013E1 RID: 5089
	public bool keepY;

	// Token: 0x040013E2 RID: 5090
	public float yPosition;

	// Token: 0x040013E3 RID: 5091
	[Space]
	public bool getPositionOnEnable;

	// Token: 0x040013E4 RID: 5092
	[Space]
	public bool keepScaleX;

	// Token: 0x040013E5 RID: 5093
	public bool keepScaleY;

	// Token: 0x040013E6 RID: 5094
	public bool deactivateIfFlippedOnX;

	// Token: 0x040013E7 RID: 5095
	[Space]
	public bool resetOnDisable;

	// Token: 0x040013E8 RID: 5096
	private Vector3 initialLocalPos;

	// Token: 0x040013E9 RID: 5097
	private Vector3 initialLocalScale;

	// Token: 0x040013EA RID: 5098
	private Vector3 lastLossyScale;

	// Token: 0x040013EB RID: 5099
	private bool started;
}
