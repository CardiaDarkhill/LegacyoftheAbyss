using System;
using HutongGames.PlayMaker;

namespace QuestPlaymakerActions
{
	// Token: 0x02000889 RID: 2185
	[ActionCategory("Quests")]
	public class CanEndQuestV2 : QuestFsmAction
	{
		// Token: 0x06004C18 RID: 19480 RVA: 0x00166FCC File Offset: 0x001651CC
		public override void Reset()
		{
			base.Reset();
			this.RequireActive = null;
			this.CannotEndEvent = null;
			this.CanEndEvent = null;
			this.StoreValue = null;
		}

		// Token: 0x06004C19 RID: 19481 RVA: 0x00166FF0 File Offset: 0x001651F0
		protected override void DoQuestAction(FullQuestBase quest)
		{
			bool flag = this.RequireActive.Value ? quest.GetIsReadyToTurnIn(false) : quest.CanComplete;
			this.StoreValue.Value = flag;
			base.Fsm.Event(flag ? this.CanEndEvent : this.CannotEndEvent);
		}

		// Token: 0x04004D72 RID: 19826
		public FsmBool RequireActive;

		// Token: 0x04004D73 RID: 19827
		public FsmEvent CannotEndEvent;

		// Token: 0x04004D74 RID: 19828
		public FsmEvent CanEndEvent;

		// Token: 0x04004D75 RID: 19829
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;
	}
}
