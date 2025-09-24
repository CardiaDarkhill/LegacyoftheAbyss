using System;
using HutongGames.PlayMaker;

// Token: 0x020002F8 RID: 760
public class RecordJournalKill : FsmStateAction
{
	// Token: 0x06001B34 RID: 6964 RVA: 0x0007E75E File Offset: 0x0007C95E
	public override void Reset()
	{
		this.Record = null;
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x0007E768 File Offset: 0x0007C968
	public override void OnEnter()
	{
		if (!this.Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = this.Record.Value as EnemyJournalRecord;
			if (enemyJournalRecord)
			{
				EnemyJournalManager.RecordKill(enemyJournalRecord, true);
			}
		}
		base.Finish();
	}

	// Token: 0x04001A27 RID: 6695
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;
}
