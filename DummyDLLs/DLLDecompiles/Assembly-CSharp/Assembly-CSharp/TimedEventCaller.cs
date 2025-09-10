using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200036C RID: 876
public class TimedEventCaller : EventBase
{
	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001E18 RID: 7704 RVA: 0x0008B167 File Offset: 0x00089367
	public override string InspectorInfo
	{
		get
		{
			return string.Format("{0}-{1} seconds", this.randomDelay.Start, this.randomDelay.End);
		}
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x0008B193 File Offset: 0x00089393
	private void OnValidate()
	{
		if (this.delay > 0f)
		{
			this.randomDelay = new MinMaxFloat(this.delay, this.delay);
			this.delay = 0f;
		}
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x0008B1C4 File Offset: 0x000893C4
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		if (!this.runOnEnable && this.runOnTrigger)
		{
			this.runOnTrigger.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
			{
				this.StartCallRoutine();
			};
			this.runOnTrigger.OnTriggerExited += delegate(Collider2D col, GameObject sender)
			{
				this.Stop();
			};
		}
	}

	// Token: 0x06001E1B RID: 7707 RVA: 0x0008B220 File Offset: 0x00089420
	private void OnEnable()
	{
		if (!this.runOnEnable)
		{
			return;
		}
		this.Stop();
		this.StartCallRoutine();
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x0008B237 File Offset: 0x00089437
	public void StartCallRoutine()
	{
		if (this.callRoutine == null)
		{
			this.callRoutine = base.StartCoroutine(this.CallRoutine());
		}
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x0008B253 File Offset: 0x00089453
	public void Stop()
	{
		if (this.callRoutine != null)
		{
			base.StopCoroutine(this.callRoutine);
			this.callRoutine = null;
		}
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x0008B270 File Offset: 0x00089470
	private IEnumerator CallRoutine()
	{
		WaitForSeconds wait = null;
		float oldDelay = 0f;
		do
		{
			float randomValue = this.randomDelay.GetRandomValue();
			if (wait == null || Math.Abs(randomValue - oldDelay) > Mathf.Epsilon)
			{
				wait = ((randomValue > 0f) ? new WaitForSeconds(randomValue) : null);
				oldDelay = randomValue;
			}
			if (wait != null)
			{
				yield return wait;
			}
			if (this.OnCall != null)
			{
				this.OnCall.Invoke();
			}
			base.CallReceivedEvent();
		}
		while (this.repeat);
		this.callRoutine = null;
		yield break;
	}

	// Token: 0x04001D35 RID: 7477
	[SerializeField]
	private bool runOnEnable = true;

	// Token: 0x04001D36 RID: 7478
	[SerializeField]
	[ModifiableProperty]
	[Conditional("runOnEnable", false, false, false)]
	private TriggerEnterEvent runOnTrigger;

	// Token: 0x04001D37 RID: 7479
	[SerializeField]
	private bool repeat = true;

	// Token: 0x04001D38 RID: 7480
	[HideInInspector]
	[SerializeField]
	[FormerlySerializedAs("repeatDelay")]
	[Obsolete]
	private float delay = 1f;

	// Token: 0x04001D39 RID: 7481
	[SerializeField]
	private MinMaxFloat randomDelay;

	// Token: 0x04001D3A RID: 7482
	[Space]
	public UnityEvent OnCall;

	// Token: 0x04001D3B RID: 7483
	private Coroutine callRoutine;
}
