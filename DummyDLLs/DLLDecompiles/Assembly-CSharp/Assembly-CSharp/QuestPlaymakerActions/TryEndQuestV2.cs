using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000885 RID: 2181
	[ActionCategory("Quests")]
	public class TryEndQuestV2 : QuestFsmAction
	{
		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06004C09 RID: 19465 RVA: 0x00166E5C File Offset: 0x0016505C
		protected override bool CustomFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004C0A RID: 19466 RVA: 0x00166E5F File Offset: 0x0016505F
		public override void Reset()
		{
			base.Reset();
			this.ConsumeCurrency = null;
			this.FailEvent = null;
			this.SuccessEvent = null;
		}

		// Token: 0x06004C0B RID: 19467 RVA: 0x00166E7C File Offset: 0x0016507C
		protected override void DoQuestAction(FullQuestBase quest)
		{
			FsmEvent fsmEvent = quest.CanComplete ? this.SuccessEvent : this.FailEvent;
			quest.TryEndQuest(delegate
			{
				this.Fsm.Event(fsmEvent);
				this.Finish();
			}, this.ConsumeCurrency.Value, false, true);
		}

		// Token: 0x04004D69 RID: 19817
		public FsmBool ConsumeCurrency;

		// Token: 0x04004D6A RID: 19818
		public FsmEvent FailEvent;

		// Token: 0x04004D6B RID: 19819
		public FsmEvent SuccessEvent;
	}
}
