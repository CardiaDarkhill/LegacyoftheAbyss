using System;
using UnityEngine;

// Token: 0x020003F2 RID: 1010
public class JitterFixPosition : MonoBehaviour
{
	// Token: 0x06002276 RID: 8822 RVA: 0x0009EA14 File Offset: 0x0009CC14
	private void Start()
	{
		Transform transform = base.transform;
		this.localPosition = transform.localPosition;
		this.localRotation = transform.localRotation;
		this.jitter.PositionRestored += this.OnPositionRestored;
		this.nextFixTime = Time.timeAsDouble + (double)this.cooldown;
		base.enabled = false;
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x0009EA71 File Offset: 0x0009CC71
	private void OnDestroy()
	{
		if (this.jitter)
		{
			this.jitter.PositionRestored -= this.OnPositionRestored;
		}
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x0009EA98 File Offset: 0x0009CC98
	private void LateUpdate()
	{
		if (this.isLerping)
		{
			this.timer += Time.deltaTime * this.lerpMultiplier;
			Transform transform = base.transform;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.localPosition, Time.deltaTime * this.lerpMultiplier);
			if (!this.ignoreRotation)
			{
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.localRotation, Time.deltaTime * this.rotationRate);
			}
			if (this.timer >= 1f)
			{
				this.isLerping = false;
				base.enabled = false;
				return;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x0009EB44 File Offset: 0x0009CD44
	private void OnPositionRestored()
	{
		if (Time.timeAsDouble < this.nextFixTime)
		{
			return;
		}
		this.nextFixTime = Time.timeAsDouble + (double)this.cooldown;
		Transform transform = base.transform;
		if (this.useLerp)
		{
			if (!this.isLerping)
			{
				this.timer = 0f;
				float num = (this.lerpDuration > 0f) ? this.lerpDuration : 1f;
				this.lerpMultiplier = 1f / num;
				this.rotationRate = 360f / num;
				this.isLerping = true;
			}
			base.enabled = true;
			return;
		}
		transform.localPosition = this.localPosition;
		if (!this.ignoreRotation)
		{
			transform.localRotation = this.localRotation;
		}
	}

	// Token: 0x04002143 RID: 8515
	[SerializeField]
	private JitterSelf jitter;

	// Token: 0x04002144 RID: 8516
	[SerializeField]
	private bool ignoreRotation;

	// Token: 0x04002145 RID: 8517
	[SerializeField]
	private float cooldown;

	// Token: 0x04002146 RID: 8518
	[SerializeField]
	private bool useLerp;

	// Token: 0x04002147 RID: 8519
	[SerializeField]
	private float lerpDuration = 0.25f;

	// Token: 0x04002148 RID: 8520
	private Vector3 localPosition;

	// Token: 0x04002149 RID: 8521
	private Quaternion localRotation;

	// Token: 0x0400214A RID: 8522
	private double nextFixTime;

	// Token: 0x0400214B RID: 8523
	private float lerpMultiplier;

	// Token: 0x0400214C RID: 8524
	private float rotationRate;

	// Token: 0x0400214D RID: 8525
	private float lerpRate;

	// Token: 0x0400214E RID: 8526
	private float timer;

	// Token: 0x0400214F RID: 8527
	private bool isLerping;
}
