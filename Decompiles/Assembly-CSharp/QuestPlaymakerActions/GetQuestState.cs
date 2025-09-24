using System;
using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088D RID: 2189
	[ActionCategory("Quests")]
	public class GetQuestState : QuestFsmAction
	{
		// Token: 0x06004C24 RID: 19492 RVA: 0x001671A2 File Offset: 0x001653A2
		public override void Reset()
		{
			base.Reset();
			this.IsAccepted = null;
			this.IsCompleted = null;
		}

		// Token: 0x06004C25 RID: 19493 RVA: 0x001671B8 File Offset: 0x001653B8
		protected override void DoQuestAction(FullQuestBase quest)
		{
			this.IsAccepted.Value = QuestManager.GetAcceptedQuests().Contains(quest);
			this.IsCompleted.Value = quest.IsCompleted;
		}

		// Token: 0x04004D7F RID: 19839
		[UIHint(UIHint.Variable)]
		public FsmBool IsAccepted;

		// Token: 0x04004D80 RID: 19840
		[UIHint(UIHint.Variable)]
		public FsmBool IsCompleted;
	}
}
