using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A8 RID: 4264
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByName : FsmStateAction
	{
		// Token: 0x060073D9 RID: 29657 RVA: 0x002384B2 File Offset: 0x002366B2
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.delay = null;
			this.everyFrame = false;
		}

		// Token: 0x060073DA RID: 29658 RVA: 0x002384D0 File Offset: 0x002366D0
		public override void OnEnter()
		{
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				if (!this.everyFrame)
				{
					base.Finish();
					return;
				}
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(this.eventTarget, FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
			}
		}

		// Token: 0x060073DB RID: 29659 RVA: 0x0023854C File Offset: 0x0023674C
		public override void OnUpdate()
		{
			if (!this.everyFrame)
			{
				if (DelayedEvent.WasSent(this.delayedEvent))
				{
					base.Finish();
					return;
				}
			}
			else
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
			}
		}

		// Token: 0x040073E8 RID: 29672
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040073E9 RID: 29673
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		// Token: 0x040073EA RID: 29674
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		// Token: 0x040073EB RID: 29675
		[Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
		public bool everyFrame;

		// Token: 0x040073EC RID: 29676
		private DelayedEvent delayedEvent;
	}
}
