using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088C RID: 2188
	[ActionCategory("Quests")]
	public class CheckQuestStateV2 : QuestFsmAction
	{
		// Token: 0x06004C21 RID: 19489 RVA: 0x0016712A File Offset: 0x0016532A
		public override void Reset()
		{
			base.Reset();
			this.NotTrackedEvent = null;
			this.IncompleteEvent = null;
			this.CompletedEvent = null;
		}

		// Token: 0x06004C22 RID: 19490 RVA: 0x00167148 File Offset: 0x00165348
		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (quest.IsCompleted)
			{
				base.Fsm.Event(this.CompletedEvent);
				return;
			}
			if (quest.IsAccepted)
			{
				base.Fsm.Event(this.IncompleteEvent);
				return;
			}
			base.Fsm.Event(this.NotTrackedEvent);
		}

		// Token: 0x04004D7C RID: 19836
		public FsmEvent NotTrackedEvent;

		// Token: 0x04004D7D RID: 19837
		public FsmEvent IncompleteEvent;

		// Token: 0x04004D7E RID: 19838
		public FsmEvent CompletedEvent;
	}
}
