using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088E RID: 2190
	[ActionCategory("Quests")]
	public class GetQuestInfo : QuestFsmAction
	{
		// Token: 0x06004C27 RID: 19495 RVA: 0x001671E9 File Offset: 0x001653E9
		public override void Reset()
		{
			base.Reset();
			this.TargetCount = null;
			this.CurrentCount = null;
		}

		// Token: 0x06004C28 RID: 19496 RVA: 0x00167200 File Offset: 0x00165400
		protected override void DoQuestAction(FullQuestBase quest)
		{
			int num = 0;
			int num2 = 0;
			foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in quest.TargetsAndCounters)
			{
				FullQuestBase.QuestTarget item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				num += item.Count;
				num2 += item2;
			}
			this.TargetCount.Value = num;
			this.CurrentCount.Value = num2;
		}

		// Token: 0x04004D81 RID: 19841
		[UIHint(UIHint.Variable)]
		public FsmInt TargetCount;

		// Token: 0x04004D82 RID: 19842
		[UIHint(UIHint.Variable)]
		public FsmInt CurrentCount;
	}
}
