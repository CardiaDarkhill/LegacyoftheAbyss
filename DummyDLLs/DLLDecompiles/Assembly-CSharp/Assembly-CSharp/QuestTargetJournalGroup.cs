using System;
using UnityEngine;

// Token: 0x020005A8 RID: 1448
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Journal Group")]
public class QuestTargetJournalGroup : QuestTargetCounter
{
	// Token: 0x06003413 RID: 13331 RVA: 0x000E7E8B File Offset: 0x000E608B
	public override bool CanGetMore()
	{
		return false;
	}

	// Token: 0x06003414 RID: 13332 RVA: 0x000E7E90 File Offset: 0x000E6090
	public override bool ShouldIncrementQuestCounter(QuestTargetCounter eventSender)
	{
		foreach (EnemyJournalRecord y in this.enemies)
		{
			if (eventSender == y)
			{
				return true;
			}
		}
		return base.ShouldIncrementQuestCounter(eventSender);
	}

	// Token: 0x06003415 RID: 13333 RVA: 0x000E7EC8 File Offset: 0x000E60C8
	public override Sprite GetPopupIcon()
	{
		return this.icon;
	}

	// Token: 0x040037A7 RID: 14247
	[SerializeField]
	private Sprite icon;

	// Token: 0x040037A8 RID: 14248
	[Space]
	[SerializeField]
	private EnemyJournalRecord[] enemies;
}
