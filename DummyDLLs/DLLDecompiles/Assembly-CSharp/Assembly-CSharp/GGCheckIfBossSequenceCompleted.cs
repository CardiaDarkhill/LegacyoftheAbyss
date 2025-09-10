using System;
using HutongGames.PlayMaker;

// Token: 0x0200038A RID: 906
[ActionCategory("Hollow Knight/GG")]
public class GGCheckIfBossSequenceCompleted : FsmStateAction
{
	// Token: 0x06001EDA RID: 7898 RVA: 0x0008D38C File Offset: 0x0008B58C
	public override void Reset()
	{
		this.completedEvent = null;
		this.notCompletedEvent = null;
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x0008D39C File Offset: 0x0008B59C
	public override void OnEnter()
	{
		if (BossSequenceController.WasCompleted)
		{
			base.Fsm.Event(this.completedEvent);
		}
		else
		{
			base.Fsm.Event(this.notCompletedEvent);
		}
		base.Finish();
	}

	// Token: 0x04001DAF RID: 7599
	public FsmEvent completedEvent;

	// Token: 0x04001DB0 RID: 7600
	public FsmEvent notCompletedEvent;
}
