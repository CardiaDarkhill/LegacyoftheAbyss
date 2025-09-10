using System;
using HutongGames.PlayMaker;

// Token: 0x020002F7 RID: 759
public class GetJournalKillData : FsmStateAction
{
	// Token: 0x06001B31 RID: 6961 RVA: 0x0007E671 File Offset: 0x0007C871
	public override void Reset()
	{
		this.Record = null;
		this.KillCount = null;
		this.IsKilled = null;
		this.IsCompleted = null;
		this.KilledEvent = null;
		this.CompletedEvent = null;
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x0007E6A0 File Offset: 0x0007C8A0
	public override void OnEnter()
	{
		if (!this.Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = (EnemyJournalRecord)this.Record.Value;
			int killCount = enemyJournalRecord.KillCount;
			int killsRequired = enemyJournalRecord.KillsRequired;
			if (!this.KillCount.IsNone)
			{
				this.KillCount.Value = killCount;
			}
			if (!this.IsCompleted.IsNone)
			{
				this.IsCompleted.Value = (killCount >= killsRequired);
				base.Fsm.Event(this.CompletedEvent);
			}
			if (!this.IsKilled.IsNone)
			{
				this.IsKilled.Value = (killCount > 0);
				base.Fsm.Event(this.KilledEvent);
			}
		}
		base.Finish();
	}

	// Token: 0x04001A21 RID: 6689
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	// Token: 0x04001A22 RID: 6690
	[UIHint(UIHint.Variable)]
	public FsmInt KillCount;

	// Token: 0x04001A23 RID: 6691
	[UIHint(UIHint.Variable)]
	public FsmBool IsKilled;

	// Token: 0x04001A24 RID: 6692
	[UIHint(UIHint.Variable)]
	public FsmBool IsCompleted;

	// Token: 0x04001A25 RID: 6693
	public FsmEvent KilledEvent;

	// Token: 0x04001A26 RID: 6694
	public FsmEvent CompletedEvent;
}
