using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000882 RID: 2178
	[ActionCategory("Quests")]
	public class BeginQuestV2 : QuestFsmAction
	{
		// Token: 0x06004BFF RID: 19455 RVA: 0x00166D8A File Offset: 0x00164F8A
		public override void Reset()
		{
			base.Reset();
			this.ShowPrompt = true;
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06004C00 RID: 19456 RVA: 0x00166D9E File Offset: 0x00164F9E
		protected override bool CustomFinish
		{
			get
			{
				return this.ShowPrompt.Value;
			}
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x00166DAB File Offset: 0x00164FAB
		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (this.ShowPrompt.Value)
			{
				quest.BeginQuest(new Action(base.Finish), true);
				return;
			}
			quest.BeginQuest(null, false);
		}

		// Token: 0x04004D66 RID: 19814
		public FsmBool ShowPrompt;
	}
}
