using System;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class FadeOutByAngle : MonoBehaviour
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000398 RID: 920 RVA: 0x0001266F File Offset: 0x0001086F
	public MinMaxFloat AngleLimitsNearest
	{
		get
		{
			return new MinMaxFloat(Mathf.DeltaAngle(0f, this.angleLimits.Start), Mathf.DeltaAngle(0f, this.angleLimits.End));
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000126A0 File Offset: 0x000108A0
	private void OnDrawGizmosSelected()
	{
		MinMaxFloat angleLimitsNearest = this.AngleLimitsNearest;
		HandleHelper.Draw2DAngle(base.transform.position, angleLimitsNearest.Start, angleLimitsNearest.End, new float?(1f));
	}

	// Token: 0x0600039A RID: 922 RVA: 0x000126DC File Offset: 0x000108DC
	private void Update()
	{
		if (!this.fadeTarget)
		{
			return;
		}
		MinMaxFloat angleLimitsNearest = this.AngleLimitsNearest;
		float num = Vector3.SignedAngle(Vector3.right, base.transform.right, Vector3.forward);
		bool flag = num >= angleLimitsNearest.Start && num <= angleLimitsNearest.End;
		if (flag)
		{
			if (!this.wasWithinLimits)
			{
				this.fadeInDelayTimeLeft = this.fadeInDelay;
			}
			else if (this.fadeInDelayTimeLeft > 0f)
			{
				this.fadeInDelayTimeLeft -= Time.deltaTime;
				if (this.fadeInDelayTimeLeft <= 0f)
				{
					this.fadeTarget.FadeTo(1f, this.fadeInDuration, null, false, null);
				}
			}
		}
		else if (this.wasWithinLimits)
		{
			this.SetExitedLimits();
		}
		this.wasWithinLimits = flag;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x000127A8 File Offset: 0x000109A8
	public void SetExitedLimits()
	{
		this.fadeTarget.FadeTo(0f, this.fadeOutDuration, null, false, null);
		this.fadeInDelayTimeLeft = 0f;
		this.wasWithinLimits = false;
	}

	// Token: 0x04000340 RID: 832
	[SerializeField]
	private MinMaxFloat angleLimits;

	// Token: 0x04000341 RID: 833
	[SerializeField]
	private NestedFadeGroupBase fadeTarget;

	// Token: 0x04000342 RID: 834
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04000343 RID: 835
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04000344 RID: 836
	[SerializeField]
	private float fadeInDelay;

	// Token: 0x04000345 RID: 837
	private bool wasWithinLimits;

	// Token: 0x04000346 RID: 838
	private float fadeInDelayTimeLeft;
}
