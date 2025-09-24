using System;
using System.Linq;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088B RID: 2187
	[ActionCategory("Quests")]
	public class CheckQuestState : QuestFsmAction
	{
		// Token: 0x06004C1E RID: 19486 RVA: 0x001670AC File Offset: 0x001652AC
		public override void Reset()
		{
			base.Reset();
			this.NotTrackedEvent = null;
			this.TrackedEvent = null;
			this.CompletedEvent = null;
		}

		// Token: 0x06004C1F RID: 19487 RVA: 0x001670CC File Offset: 0x001652CC
		protected override void DoQuestAction(FullQuestBase quest)
		{
			if (QuestManager.GetAcceptedQuests().Contains(quest))
			{
				if (quest.IsCompleted)
				{
					base.Fsm.Event(this.CompletedEvent);
				}
				base.Fsm.Event(this.TrackedEvent);
				return;
			}
			base.Fsm.Event(this.NotTrackedEvent);
		}

		// Token: 0x04004D79 RID: 19833
		public FsmEvent NotTrackedEvent;

		// Token: 0x04004D7A RID: 19834
		public FsmEvent TrackedEvent;

		// Token: 0x04004D7B RID: 19835
		public FsmEvent CompletedEvent;
	}
}
