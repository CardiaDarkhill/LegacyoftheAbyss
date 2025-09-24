using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x0200051F RID: 1311
public class NeedolinAppearTimeline : MonoBehaviour
{
	// Token: 0x06002F29 RID: 12073 RVA: 0x000D01F0 File Offset: 0x000CE3F0
	private void Update()
	{
		if (this.isFinished)
		{
			return;
		}
		if (this.pauseTimeLeft > 0f)
		{
			this.pauseTimeLeft -= Time.deltaTime;
			return;
		}
		if (this.isAppearing)
		{
			this.currentSpeed += this.appearAcceleration * Time.deltaTime;
			if (this.currentSpeed > this.appearSpeed)
			{
				this.currentSpeed = this.appearSpeed;
			}
			this.currentTime += this.currentSpeed * Time.deltaTime;
			if (this.currentTime > this.maxTime)
			{
				this.currentTime = this.maxTime;
			}
		}
		else
		{
			this.currentSpeed += this.disappearAcceleration * Time.deltaTime;
			if (this.currentSpeed > this.disappearSpeed)
			{
				this.currentSpeed = this.disappearSpeed;
			}
			bool flag = this.currentTime > Mathf.Epsilon;
			this.currentTime -= this.currentSpeed * Time.deltaTime;
			if (this.currentTime < 0f)
			{
				this.currentTime = 0f;
			}
			if (flag && this.currentTime <= Mathf.Epsilon)
			{
				this.OnDisappeared.Invoke();
			}
		}
		float num = Mathf.Clamp01(this.currentTime / this.maxTime);
		num = this.curve.Evaluate(num);
		this.director.time = (double)(this.maxTime * num);
		this.director.Evaluate();
		if (num >= 1f - Mathf.Epsilon)
		{
			this.isFinished = true;
			this.OnFinish.Invoke();
		}
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x000D0386 File Offset: 0x000CE586
	public void StartAppear()
	{
		this.isAppearing = true;
		this.currentSpeed = ((this.currentTime > this.minTime) ? 0f : this.appearSpeed);
		this.pauseTimeLeft = 0f;
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x000D03BC File Offset: 0x000CE5BC
	public void CancelAppear()
	{
		this.isAppearing = false;
		this.isFinished = false;
		if (this.currentTime > this.minTime)
		{
			this.currentSpeed = 0f;
			this.pauseTimeLeft = this.pauseTime;
			return;
		}
		this.currentSpeed = this.disappearSpeed;
		this.pauseTimeLeft = 0f;
	}

	// Token: 0x040031EA RID: 12778
	[SerializeField]
	private PlayableDirector director;

	// Token: 0x040031EB RID: 12779
	[SerializeField]
	private float maxTime;

	// Token: 0x040031EC RID: 12780
	[SerializeField]
	private float minTime;

	// Token: 0x040031ED RID: 12781
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040031EE RID: 12782
	[SerializeField]
	private float appearSpeed;

	// Token: 0x040031EF RID: 12783
	[SerializeField]
	private float appearAcceleration;

	// Token: 0x040031F0 RID: 12784
	[SerializeField]
	private float pauseTime;

	// Token: 0x040031F1 RID: 12785
	[SerializeField]
	private float disappearSpeed;

	// Token: 0x040031F2 RID: 12786
	[SerializeField]
	private float disappearAcceleration;

	// Token: 0x040031F3 RID: 12787
	[Space]
	public UnityEvent OnFinish;

	// Token: 0x040031F4 RID: 12788
	[Space]
	public UnityEvent OnDisappeared;

	// Token: 0x040031F5 RID: 12789
	private bool isAppearing;

	// Token: 0x040031F6 RID: 12790
	private float currentTime;

	// Token: 0x040031F7 RID: 12791
	private float currentSpeed;

	// Token: 0x040031F8 RID: 12792
	private float pauseTimeLeft;

	// Token: 0x040031F9 RID: 12793
	private bool isFinished;
}
