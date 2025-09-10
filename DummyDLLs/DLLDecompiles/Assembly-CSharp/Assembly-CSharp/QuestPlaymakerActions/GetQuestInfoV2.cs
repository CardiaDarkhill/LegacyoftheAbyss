using System;
using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088F RID: 2191
	[ActionCategory("Quests")]
	public class GetQuestInfoV2 : QuestFsmAction
	{
		// Token: 0x06004C2A RID: 19498 RVA: 0x00167288 File Offset: 0x00165488
		public override void Reset()
		{
			base.Reset();
			this.TargetCounts = null;
			this.CurrentCounts = null;
		}

		// Token: 0x06004C2B RID: 19499 RVA: 0x001672A0 File Offset: 0x001654A0
		protected override void DoQuestAction(FullQuestBase quest)
		{
			this.TargetCounts.intValues = (from target in quest.Targets
			select target.Count).ToArray<int>();
			this.CurrentCounts.intValues = quest.Counters.ToArray<int>();
		}

		// Token: 0x04004D83 RID: 19843
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray TargetCounts;

		// Token: 0x04004D84 RID: 19844
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Int, "", 0, 0, 65536)]
		public FsmArray CurrentCounts;
	}
}
