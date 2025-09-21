using System;
using HutongGames.PlayMaker;

// Token: 0x02000389 RID: 905
[ActionCategory("Hollow Knight/GG")]
public class GGCheckBoundSoul : FsmStateAction
{
	// Token: 0x06001ED7 RID: 7895 RVA: 0x0008D319 File Offset: 0x0008B519
	public override void Reset()
	{
		this.boundEvent = null;
		this.unboundEvent = null;
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x0008D32C File Offset: 0x0008B52C
	public override void OnEnter()
	{
		if (BossSequenceController.IsInSequence)
		{
			if (BossSequenceController.BoundSoul)
			{
				base.Fsm.Event(this.boundEvent);
			}
			else
			{
				base.Fsm.Event(this.unboundEvent);
			}
		}
		else
		{
			base.Fsm.Event(this.unboundEvent);
		}
		base.Finish();
	}

	// Token: 0x04001DAD RID: 7597
	public FsmEvent boundEvent;

	// Token: 0x04001DAE RID: 7598
	public FsmEvent unboundEvent;
}
