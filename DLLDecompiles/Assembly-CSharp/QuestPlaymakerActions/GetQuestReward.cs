using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000892 RID: 2194
	[ActionCategory("Quests")]
	public class GetQuestReward : QuestFsmAction
	{
		// Token: 0x06004C34 RID: 19508 RVA: 0x00167493 File Offset: 0x00165693
		public override void Reset()
		{
			base.Reset();
			this.StoreReward = null;
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x001674A2 File Offset: 0x001656A2
		protected override void DoQuestAction(FullQuestBase quest)
		{
			this.StoreReward.Value = quest.RewardItem;
		}

		// Token: 0x04004D8E RID: 19854
		[ObjectType(typeof(SavedItem))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreReward;
	}
}
