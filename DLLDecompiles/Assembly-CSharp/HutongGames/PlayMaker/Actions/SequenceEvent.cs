using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AB RID: 4267
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends the next event on the state each time the state is entered.")]
	public class SequenceEvent : FsmStateAction
	{
		// Token: 0x060073E5 RID: 29669 RVA: 0x002387AE File Offset: 0x002369AE
		public override void Reset()
		{
			this.delay = null;
		}

		// Token: 0x060073E6 RID: 29670 RVA: 0x002387B8 File Offset: 0x002369B8
		public override void OnEnter()
		{
			if (this.reset.Value)
			{
				this.eventIndex = 0;
				this.reset.Value = false;
			}
			int num = base.State.Transitions.Length;
			if (num > 0)
			{
				FsmEvent fsmEvent = base.State.Transitions[this.eventIndex].FsmEvent;
				if (this.delay.Value < 0.001f)
				{
					base.Fsm.Event(fsmEvent);
					base.Finish();
				}
				else
				{
					this.delayedEvent = base.Fsm.DelayedEvent(fsmEvent, this.delay.Value);
				}
				this.eventIndex++;
				if (this.eventIndex == num)
				{
					this.eventIndex = 0;
				}
			}
		}

		// Token: 0x060073E7 RID: 29671 RVA: 0x00238870 File Offset: 0x00236A70
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x040073F8 RID: 29688
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay before sending the event in seconds.")]
		public FsmFloat delay;

		// Token: 0x040073F9 RID: 29689
		[UIHint(UIHint.Variable)]
		[Tooltip("Assign a variable to control reset. Set it to True to reset the sequence. Value is set to False after resetting.")]
		public FsmBool reset;

		// Token: 0x040073FA RID: 29690
		private DelayedEvent delayedEvent;

		// Token: 0x040073FB RID: 29691
		private int eventIndex;
	}
}
