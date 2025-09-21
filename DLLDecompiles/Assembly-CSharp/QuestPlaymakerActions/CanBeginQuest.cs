using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x0200088A RID: 2186
	[ActionCategory("Quests")]
	public class CanBeginQuest : QuestFsmAction
	{
		// Token: 0x06004C1B RID: 19483 RVA: 0x0016704A File Offset: 0x0016524A
		public override void Reset()
		{
			base.Reset();
			this.CannotBeginEvent = null;
			this.CanBeginEvent = null;
			this.StoreValue = null;
		}

		// Token: 0x06004C1C RID: 19484 RVA: 0x00167068 File Offset: 0x00165268
		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool isAvailable = quest.IsAvailable;
			this.StoreValue.Value = isAvailable;
			base.Fsm.Event(isAvailable ? this.CanBeginEvent : this.CannotBeginEvent);
		}

		// Token: 0x04004D76 RID: 19830
		public FsmEvent CannotBeginEvent;

		// Token: 0x04004D77 RID: 19831
		public FsmEvent CanBeginEvent;

		// Token: 0x04004D78 RID: 19832
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;
	}
}
