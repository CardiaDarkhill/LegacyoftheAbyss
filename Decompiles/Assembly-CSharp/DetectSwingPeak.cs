using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200000B RID: 11
public class DetectSwingPeak : MonoBehaviour
{
	// Token: 0x0600005C RID: 92 RVA: 0x00003D8C File Offset: 0x00001F8C
	private void OnEnable()
	{
		this.didGetPrevious = false;
		this.currentSign = 0f;
		this.hasPeaked = false;
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003DA8 File Offset: 0x00001FA8
	private void Update()
	{
		float num = base.transform.localEulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num < -180f)
		{
			num += 360f;
		}
		float num2 = num - this.previousAngleZ;
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		if (this.didGetPrevious && Mathf.Abs(num2) > Mathf.Epsilon)
		{
			this.stillTimeElapsed = 0f;
			float num3 = Mathf.Sign(num2);
			if (((num3 < 0f && this.currentSign > 0f) || (num3 > 0f && this.currentSign < 0f)) && (!this.hasPeaked || Mathf.Abs(this.lastPeakSign - Mathf.Sign(num)) > 1E-45f))
			{
				this.OnRangeExceeded.Invoke();
				this.lastPeakSign = Mathf.Sign(num);
				this.hasPeaked = true;
			}
			this.currentSign = num3;
		}
		else if (this.stillTimeElapsed < 1f)
		{
			this.stillTimeElapsed += Time.deltaTime;
			if (this.stillTimeElapsed >= 1f)
			{
				this.OnRangeExceeded.Invoke();
				this.stillTimeElapsed = 0f;
			}
		}
		this.previousAngleZ = num;
		this.didGetPrevious = true;
	}

	// Token: 0x04000049 RID: 73
	public UnityEvent OnRangeExceeded;

	// Token: 0x0400004A RID: 74
	private const float STILL_TIME = 1f;

	// Token: 0x0400004B RID: 75
	private bool didGetPrevious;

	// Token: 0x0400004C RID: 76
	private float previousAngleZ;

	// Token: 0x0400004D RID: 77
	private float currentSign;

	// Token: 0x0400004E RID: 78
	private float stillTimeElapsed;

	// Token: 0x0400004F RID: 79
	private float lastPeakSign;

	// Token: 0x04000050 RID: 80
	private bool hasPeaked;
}
