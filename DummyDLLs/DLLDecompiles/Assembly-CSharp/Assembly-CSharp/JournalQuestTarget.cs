using System;
using UnityEngine;

// Token: 0x020001C7 RID: 455
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Journal")]
public class JournalQuestTarget : QuestTargetCounter
{
	// Token: 0x060011D6 RID: 4566 RVA: 0x00053887 File Offset: 0x00051A87
	public override Sprite GetQuestCounterSprite(int index)
	{
		return this.counterSprite;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x0005388F File Offset: 0x00051A8F
	public override bool CanGetMore()
	{
		return false;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x00053894 File Offset: 0x00051A94
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		JournalQuestTarget.CheckTypes checkTypes = this.checkType;
		if (checkTypes == JournalQuestTarget.CheckTypes.Seen)
		{
			return EnemyJournalManager.GetKilledEnemies().Count;
		}
		if (checkTypes != JournalQuestTarget.CheckTypes.Completed)
		{
			throw new ArgumentOutOfRangeException();
		}
		return EnemyJournalManager.GetCompletedEnemies().Count;
	}

	// Token: 0x040010C0 RID: 4288
	[SerializeField]
	private Sprite counterSprite;

	// Token: 0x040010C1 RID: 4289
	[SerializeField]
	private JournalQuestTarget.CheckTypes checkType;

	// Token: 0x0200150A RID: 5386
	private enum CheckTypes
	{
		// Token: 0x040085AF RID: 34223
		Seen,
		// Token: 0x040085B0 RID: 34224
		Completed
	}
}
