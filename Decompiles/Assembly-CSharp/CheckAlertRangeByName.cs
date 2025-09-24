using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000294 RID: 660
[ActionCategory("Hollow Knight")]
public class CheckAlertRangeByName : FsmStateAction
{
	// Token: 0x06001714 RID: 5908 RVA: 0x0006832B File Offset: 0x0006652B
	public override void Reset()
	{
		this.eventTarget = null;
		this.storeResult = new FsmBool();
		this.sendEvent = null;
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x00068348 File Offset: 0x00066548
	public override void OnEnter()
	{
		this.source = AlertRange.Find(base.Owner, this.alertRangeName);
		if (this.source != null)
		{
			this.alertRange_go = this.source.gameObject;
		}
		this.Apply();
		if (!this.everyFrame)
		{
			base.Finish();
		}
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x0006839F File Offset: 0x0006659F
	public override void OnUpdate()
	{
		this.Apply();
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x000683A8 File Offset: 0x000665A8
	private void Apply()
	{
		if (this.source != null && this.source.gameObject.activeSelf)
		{
			if (!this.storeResult.IsNone)
			{
				this.storeResult.Value = this.source.IsHeroInRange();
			}
			if (!this.sendEvent.IsNone && this.source.IsHeroInRange())
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
			}
			if (!this.outOfRangeEvent.IsNone && !this.source.IsHeroInRange())
			{
				base.Fsm.Event(this.eventTarget, this.outOfRangeEvent.Value);
				return;
			}
		}
		else if (!this.storeResult.IsNone)
		{
			this.storeResult.Value = false;
		}
	}

	// Token: 0x040015A0 RID: 5536
	[UIHint(UIHint.Variable)]
	public FsmEventTarget eventTarget;

	// Token: 0x040015A1 RID: 5537
	public string alertRangeName;

	// Token: 0x040015A2 RID: 5538
	public FsmBool storeResult;

	// Token: 0x040015A3 RID: 5539
	public FsmString sendEvent;

	// Token: 0x040015A4 RID: 5540
	public FsmString outOfRangeEvent;

	// Token: 0x040015A5 RID: 5541
	public bool everyFrame;

	// Token: 0x040015A6 RID: 5542
	private GameObject alertRange_go;

	// Token: 0x040015A7 RID: 5543
	private AlertRange source;
}
