using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class SpeedRampAnimator : SpeedChanger
{
	// Token: 0x0600053F RID: 1343 RVA: 0x0001AB9F File Offset: 0x00018D9F
	public void StartSpeedRamp()
	{
		this.StopSpeedRamp();
		this.animationRoutine = this.StartTimerRoutine(0f, this.duration, delegate(float time)
		{
			time = this.curve.Evaluate(time);
			float lerpedValue = this.speedRange.GetLerpedValue(time);
			base.CallSpeedChangedEvent(lerpedValue);
		}, null, null, false);
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001ABCD File Offset: 0x00018DCD
	public void StopSpeedRamp()
	{
		if (this.animationRoutine != null)
		{
			base.StopCoroutine(this.animationRoutine);
		}
	}

	// Token: 0x04000513 RID: 1299
	[SerializeField]
	private MinMaxFloat speedRange;

	// Token: 0x04000514 RID: 1300
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000515 RID: 1301
	[SerializeField]
	private float duration;

	// Token: 0x04000516 RID: 1302
	private Coroutine animationRoutine;
}
