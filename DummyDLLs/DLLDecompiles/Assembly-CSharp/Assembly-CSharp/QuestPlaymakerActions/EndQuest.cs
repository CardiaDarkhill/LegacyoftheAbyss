using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000886 RID: 2182
	[ActionCategory("Quests")]
	[Tooltip("Force ends a quest - assumes something like QuestCompleteYesNo was previously used to check if quest can complete.")]
	public class EndQuest : QuestFsmAction
	{
		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06004C0D RID: 19469 RVA: 0x00166EDA File Offset: 0x001650DA
		protected override bool CustomFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004C0E RID: 19470 RVA: 0x00166EDD File Offset: 0x001650DD
		public override void Reset()
		{
			base.Reset();
			this.ConsumeCurrency = null;
		}

		// Token: 0x06004C0F RID: 19471 RVA: 0x00166EEC File Offset: 0x001650EC
		protected override void DoQuestAction(FullQuestBase quest)
		{
			quest.TryEndQuest(new Action(base.Finish), this.ConsumeCurrency.Value, true, true);
		}

		// Token: 0x04004D6C RID: 19820
		public FsmBool ConsumeCurrency;
	}
}
