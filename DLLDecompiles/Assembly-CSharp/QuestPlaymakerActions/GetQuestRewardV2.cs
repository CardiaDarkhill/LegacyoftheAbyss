using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000893 RID: 2195
	[ActionCategory("Quests")]
	public class GetQuestRewardV2 : QuestFsmAction
	{
		// Token: 0x06004C37 RID: 19511 RVA: 0x001674BD File Offset: 0x001656BD
		public override void Reset()
		{
			base.Reset();
			this.StoreReward = null;
			this.StoreAmount = null;
		}

		// Token: 0x06004C38 RID: 19512 RVA: 0x001674D3 File Offset: 0x001656D3
		protected override void DoQuestAction(FullQuestBase quest)
		{
			this.StoreReward.Value = quest.RewardItem;
			this.StoreAmount.Value = quest.RewardCount;
		}

		// Token: 0x04004D8F RID: 19855
		[ObjectType(typeof(SavedItem))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreReward;

		// Token: 0x04004D90 RID: 19856
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;
	}
}
