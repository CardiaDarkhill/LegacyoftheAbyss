using System;
using HutongGames.PlayMaker;

// Token: 0x020002F9 RID: 761
public class RecordJournalKillV2 : FsmStateAction
{
	// Token: 0x06001B37 RID: 6967 RVA: 0x0007E7B0 File Offset: 0x0007C9B0
	public override void Reset()
	{
		this.Record = null;
		this.ShowPopup = true;
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x0007E7C8 File Offset: 0x0007C9C8
	public override void OnEnter()
	{
		if (!this.Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = this.Record.Value as EnemyJournalRecord;
			if (enemyJournalRecord)
			{
				EnemyJournalManager.RecordKill(enemyJournalRecord, this.ShowPopup.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04001A28 RID: 6696
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	// Token: 0x04001A29 RID: 6697
	public FsmBool ShowPopup;
}
