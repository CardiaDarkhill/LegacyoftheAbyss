using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000887 RID: 2183
	[ActionCategory("Quests")]
	[Tooltip("Force ends a quest - assumes something like QuestCompleteYesNo was previously used to check if quest can complete.")]
	public class EndQuestV2 : QuestFsmAction
	{
		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06004C11 RID: 19473 RVA: 0x00166F16 File Offset: 0x00165116
		protected override bool CustomFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004C12 RID: 19474 RVA: 0x00166F19 File Offset: 0x00165119
		public override void Reset()
		{
			base.Reset();
			this.ConsumeCurrency = null;
			this.ShowPrompt = true;
		}

		// Token: 0x06004C13 RID: 19475 RVA: 0x00166F34 File Offset: 0x00165134
		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.TryEndQuest(new Action(base.Finish), this.ConsumeCurrency.Value, true, this.ShowPrompt.Value);
		}

		// Token: 0x04004D6D RID: 19821
		public FsmBool ConsumeCurrency;

		// Token: 0x04004D6E RID: 19822
		public FsmBool ShowPrompt;
	}
}
