using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000295 RID: 661
[ActionCategory("Hollow Knight")]
public class UnalertTimer : FsmStateAction
{
	// Token: 0x06001719 RID: 5913 RVA: 0x0006848C File Offset: 0x0006668C
	public override void OnPreprocess()
	{
		base.Fsm.HandleFixedUpdate = true;
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x0006849A File Offset: 0x0006669A
	public override void Reset()
	{
		this.alertRange = new FsmObject
		{
			UseVariable = true
		};
		this.timerVariable = new FsmFloat
		{
			UseVariable = true
		};
		this.unalertTime = null;
		this.eventTarget = null;
		this.unalertEvent = null;
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x000684D5 File Offset: 0x000666D5
	public override void OnEnter()
	{
		if (this.alertRange.Value == null || this.alertRange.IsNone)
		{
			base.Finish();
		}
		if (this.resetOnStateEntry)
		{
			this.timerVariable = 0f;
		}
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x00068518 File Offset: 0x00066718
	public override void OnUpdate()
	{
		if (this.alertRange.Value != null)
		{
			if ((this.alertRange.Value as AlertRange).IsHeroInRange())
			{
				this.timerVariable.Value = 0f;
				return;
			}
			this.timerVariable.Value += Time.deltaTime;
			if (this.timerVariable.Value >= this.unalertTime.Value)
			{
				base.Fsm.Event(this.eventTarget, this.unalertEvent);
			}
		}
	}

	// Token: 0x040015A8 RID: 5544
	[ObjectType(typeof(AlertRange))]
	public FsmObject alertRange;

	// Token: 0x040015A9 RID: 5545
	[UIHint(UIHint.Variable)]
	public FsmFloat timerVariable;

	// Token: 0x040015AA RID: 5546
	public FsmFloat unalertTime;

	// Token: 0x040015AB RID: 5547
	public FsmEventTarget eventTarget;

	// Token: 0x040015AC RID: 5548
	public FsmEvent unalertEvent;

	// Token: 0x040015AD RID: 5549
	public bool resetOnStateEntry;
}
