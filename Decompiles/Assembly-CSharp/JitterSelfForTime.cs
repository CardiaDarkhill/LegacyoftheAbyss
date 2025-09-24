using System;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
public class JitterSelfForTime : JitterSelf
{
	// Token: 0x0600229B RID: 8859 RVA: 0x0009F31C File Offset: 0x0009D51C
	public void StartTimedJitter()
	{
		if (this.duration <= 0f)
		{
			return;
		}
		if (this.delayedRoutine != null)
		{
			base.StopCoroutine(this.delayedRoutine);
		}
		base.InternalStopJitter(false);
		base.StartJitter();
		this.delayedRoutine = this.StartTimerRoutine(0f, this.duration, delegate(float t)
		{
			base.Multiplier = this.decayCurve.Evaluate(t);
		}, null, new Action(base.StopJitter), false);
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x0009F389 File Offset: 0x0009D589
	protected override void OnStopJitter()
	{
		if (this.delayedRoutine != null)
		{
			base.StopCoroutine(this.delayedRoutine);
			this.delayedRoutine = null;
		}
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x0009F3A8 File Offset: 0x0009D5A8
	public static JitterSelfForTime AddHandler(GameObject gameObject, Vector3 jitterAmount, float jitterDuration, float jitterFrequency)
	{
		JitterSelfForTime jitterSelfForTime = gameObject.AddComponent<JitterSelfForTime>();
		jitterSelfForTime.duration = jitterDuration;
		jitterSelfForTime.startInactive = true;
		JitterSelfConfig config = jitterSelfForTime.config;
		config.AmountMax = jitterAmount;
		config.AmountMin = jitterAmount;
		config.Frequency = jitterFrequency;
		config.UseCameraRenderHooks = true;
		jitterSelfForTime.config = config;
		return jitterSelfForTime;
	}

	// Token: 0x0400216E RID: 8558
	[Space]
	[SerializeField]
	private float duration;

	// Token: 0x0400216F RID: 8559
	[SerializeField]
	private AnimationCurve decayCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x04002170 RID: 8560
	private Coroutine delayedRoutine;
}
