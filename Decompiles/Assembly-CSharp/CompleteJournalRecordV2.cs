using System;
using HutongGames.PlayMaker;

// Token: 0x020002FB RID: 763
public class CompleteJournalRecordV2 : FsmStateAction
{
	// Token: 0x06001B3D RID: 6973 RVA: 0x0007E89B File Offset: 0x0007CA9B
	public override void Reset()
	{
		this.Record = null;
		this.ShowPopup = true;
		this.ForcePopup = null;
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x0007E8B8 File Offset: 0x0007CAB8
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
				EnemyJournalManager.RecordKill(enemyJournalRecord, this.ShowPopup.Value, this.ForcePopup.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04001A2C RID: 6700
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	// Token: 0x04001A2D RID: 6701
	public FsmBool ShowPopup;

	// Token: 0x04001A2E RID: 6702
	public FsmBool ForcePopup;
}
