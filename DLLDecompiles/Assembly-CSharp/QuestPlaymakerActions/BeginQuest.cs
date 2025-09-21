using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000881 RID: 2177
	[ActionCategory("Quests")]
	public class BeginQuest : QuestFsmAction
	{
		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06004BFC RID: 19452 RVA: 0x00166D6A File Offset: 0x00164F6A
		protected override bool CustomFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004BFD RID: 19453 RVA: 0x00166D6D File Offset: 0x00164F6D
		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.BeginQuest(new Action(base.Finish), true);
		}
	}
}
