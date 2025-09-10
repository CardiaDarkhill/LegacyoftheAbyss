using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A2 RID: 4258
	[ActionCategory(ActionCategory.StateMachine)]
	[Note("Put this action at the end of a State to loop through all actions in a state the specified number of times.")]
	[Tooltip("Loops through the state the specified number of times then sends the Finish Event.")]
	public class LoopState : FsmStateAction
	{
		// Token: 0x060073A9 RID: 29609 RVA: 0x00237CE8 File Offset: 0x00235EE8
		public override void OnEnter()
		{
			this.storeCurrentLoop.Value = this.loopedCount;
			this.loopedCount++;
			if (this.loopedCount >= this.loops.Value)
			{
				this.loopedCount = 0;
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			base.Fsm.SwitchState(base.Fsm.ActiveState);
		}

		// Token: 0x040073D3 RID: 29651
		[RequiredField]
		[Tooltip("How many times to loop through the state.")]
		public FsmInt loops;

		// Token: 0x040073D4 RID: 29652
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current loop count. Starts at 0. Useful for iterating through arrays.")]
		public FsmInt storeCurrentLoop;

		// Token: 0x040073D5 RID: 29653
		[Tooltip("Event to send when the loops have finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040073D6 RID: 29654
		private int loopedCount;
	}
}
