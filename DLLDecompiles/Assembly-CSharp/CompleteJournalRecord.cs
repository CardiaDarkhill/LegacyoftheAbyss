using System;
using HutongGames.PlayMaker;

// Token: 0x020002FA RID: 762
public class CompleteJournalRecord : FsmStateAction
{
	// Token: 0x06001B3A RID: 6970 RVA: 0x0007E81A File Offset: 0x0007CA1A
	public override void Reset()
	{
		this.Record = null;
		this.ShowPopup = true;
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x0007E830 File Offset: 0x0007CA30
	public override void OnEnter()
	{
		if (!this.Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = this.Record.Value as EnemyJournalRecord;
			if (enemyJournalRecord)
			{
				while (enemyJournalRecord.KillCount < enemyJournalRecord.KillsRequired - 1)
				{
					EnemyJournalManager.RecordKill(enemyJournalRecord, false);
				}
				EnemyJournalManager.RecordKill(enemyJournalRecord, this.ShowPopup.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04001A2A RID: 6698
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	// Token: 0x04001A2B RID: 6699
	public FsmBool ShowPopup;
}
