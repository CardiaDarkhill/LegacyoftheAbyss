using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000883 RID: 2179
	[ActionCategory("Quests")]
	public class IncrementQuestCounter : QuestFsmAction
	{
		// Token: 0x06004C03 RID: 19459 RVA: 0x00166DDE File Offset: 0x00164FDE
		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.IncrementQuestCounter();
		}
	}
}
