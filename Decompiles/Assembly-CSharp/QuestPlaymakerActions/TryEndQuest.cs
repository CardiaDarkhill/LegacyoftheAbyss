using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000884 RID: 2180
	[ActionCategory("Quests")]
	public class TryEndQuest : QuestFsmAction
	{
		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06004C05 RID: 19461 RVA: 0x00166DEE File Offset: 0x00164FEE
		protected override bool CustomFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004C06 RID: 19462 RVA: 0x00166DF1 File Offset: 0x00164FF1
		public override void Reset()
		{
			base.Reset();
			this.FailEvent = null;
			this.SuccessEvent = null;
		}

		// Token: 0x06004C07 RID: 19463 RVA: 0x00166E08 File Offset: 0x00165008
		protected override void DoQuestAction(FullQuestBase quest)
		{
			FsmEvent fsmEvent = quest.CanComplete ? this.SuccessEvent : this.FailEvent;
			quest.TryEndQuest(delegate
			{
				this.Fsm.Event(fsmEvent);
				this.Finish();
			}, true, false, true);
		}

		// Token: 0x04004D67 RID: 19815
		public FsmEvent FailEvent;

		// Token: 0x04004D68 RID: 19816
		public FsmEvent SuccessEvent;
	}
}
