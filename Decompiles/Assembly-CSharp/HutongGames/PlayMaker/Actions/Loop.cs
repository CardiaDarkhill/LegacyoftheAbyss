using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A1 RID: 4257
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends the Loop Event when the action runs. It loops the specified number of times then sends the Finish Event. ")]
	public class Loop : FsmStateAction
	{
		// Token: 0x060073A7 RID: 29607 RVA: 0x00237C70 File Offset: 0x00235E70
		public override void OnEnter()
		{
			this.storeCurrentLoop.Value = this.loopedCount;
			this.loopedCount++;
			if (this.loopedCount > this.loops.Value)
			{
				this.loopedCount = 0;
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			base.Fsm.Event(this.loopEvent);
		}

		// Token: 0x040073CE RID: 29646
		[RequiredField]
		[Tooltip("How many times to loop.")]
		public FsmInt loops;

		// Token: 0x040073CF RID: 29647
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current loop count. Starts at 0. Useful for iterating through arrays.")]
		public FsmInt storeCurrentLoop;

		// Token: 0x040073D0 RID: 29648
		[Tooltip("Event that starts a loop.")]
		public FsmEvent loopEvent;

		// Token: 0x040073D1 RID: 29649
		[Tooltip("Event to send when the loops have finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040073D2 RID: 29650
		private int loopedCount;
	}
}
