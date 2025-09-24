using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000888 RID: 2184
	[ActionCategory("Quests")]
	public class CanEndQuest : QuestFsmAction
	{
		// Token: 0x06004C15 RID: 19477 RVA: 0x00166F68 File Offset: 0x00165168
		public override void Reset()
		{
			base.Reset();
			this.CannotEndEvent = null;
			this.CanEndEvent = null;
			this.StoreValue = null;
		}

		// Token: 0x06004C16 RID: 19478 RVA: 0x00166F88 File Offset: 0x00165188
		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool canComplete = quest.CanComplete;
			this.StoreValue.Value = canComplete;
			base.Fsm.Event(canComplete ? this.CanEndEvent : this.CannotEndEvent);
		}

		// Token: 0x04004D6F RID: 19823
		public FsmEvent CannotEndEvent;

		// Token: 0x04004D70 RID: 19824
		public FsmEvent CanEndEvent;

		// Token: 0x04004D71 RID: 19825
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;
	}
}
