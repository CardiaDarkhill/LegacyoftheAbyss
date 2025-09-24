using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000293 RID: 659
[ActionCategory("Hollow Knight")]
public class CheckAlertRange : FsmStateAction
{
	// Token: 0x0600170B RID: 5899 RVA: 0x00068161 File Offset: 0x00066361
	public bool HideInRangeDelay()
	{
		return !this.everyFrame || this.InRangeEvent == null;
	}

	// Token: 0x0600170C RID: 5900 RVA: 0x00068176 File Offset: 0x00066376
	public bool HideOutOfRangeDelay()
	{
		return !this.everyFrame || this.OutOfRangeEvent == null;
	}

	// Token: 0x0600170D RID: 5901 RVA: 0x0006818B File Offset: 0x0006638B
	public override void OnPreprocess()
	{
		base.Fsm.HandleFixedUpdate = true;
	}

	// Token: 0x0600170E RID: 5902 RVA: 0x0006819C File Offset: 0x0006639C
	public override void Reset()
	{
		this.alertRange = new FsmObject
		{
			UseVariable = true
		};
		this.storeResult = new FsmBool();
		this.InRangeEvent = null;
		this.InRangeDelay = new FsmFloat();
		this.OutOfRangeEvent = null;
		this.OutOfRangeDelay = new FsmFloat();
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x000681EA File Offset: 0x000663EA
	public override void OnEnter()
	{
		if (this.alertRange.Value == null || this.alertRange.IsNone)
		{
			base.Finish();
			return;
		}
		this.Apply(true);
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x00068228 File Offset: 0x00066428
	public override void OnUpdate()
	{
		this.Apply(false);
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x00068234 File Offset: 0x00066434
	private void Apply(bool isFirst)
	{
		if (this.alertRange.Value != null)
		{
			AlertRange alertRange = this.alertRange.Value as AlertRange;
			bool flag = this.isCurrentlyInRange;
			this.isCurrentlyInRange = (alertRange != null && alertRange.IsHeroInRange());
			if (this.isCurrentlyInRange != flag || isFirst)
			{
				this.currentRangeTimer = this.GetCurrentRangeDelay();
			}
			if (this.currentRangeTimer <= 0f || !this.everyFrame)
			{
				this.storeResult.Value = this.isCurrentlyInRange;
				base.Fsm.Event(this.storeResult.Value ? this.InRangeEvent : this.OutOfRangeEvent);
				return;
			}
			this.currentRangeTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x00068302 File Offset: 0x00066502
	private float GetCurrentRangeDelay()
	{
		if (!this.isCurrentlyInRange)
		{
			return this.OutOfRangeDelay.Value;
		}
		return this.InRangeDelay.Value;
	}

	// Token: 0x04001597 RID: 5527
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	// Token: 0x04001598 RID: 5528
	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	// Token: 0x04001599 RID: 5529
	public FsmEvent InRangeEvent;

	// Token: 0x0400159A RID: 5530
	[HideIf("HideInRangeDelay")]
	public FsmFloat InRangeDelay;

	// Token: 0x0400159B RID: 5531
	public FsmEvent OutOfRangeEvent;

	// Token: 0x0400159C RID: 5532
	[HideIf("HideOutOfRangeDelay")]
	public FsmFloat OutOfRangeDelay;

	// Token: 0x0400159D RID: 5533
	public bool everyFrame;

	// Token: 0x0400159E RID: 5534
	private bool isCurrentlyInRange;

	// Token: 0x0400159F RID: 5535
	private float currentRangeTimer;
}
