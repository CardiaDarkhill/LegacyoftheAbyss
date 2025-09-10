using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D10 RID: 3344
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByNameV3 : FsmStateAction
	{
		// Token: 0x060062D1 RID: 25297 RVA: 0x001F3A9A File Offset: 0x001F1C9A
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.delay = null;
			this.everyFrame = false;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x001F3ACC File Offset: 0x001F1CCC
		public override void OnEnter()
		{
			if (!this.activeBool.Value || this.activeBool.IsNone)
			{
				base.Finish();
				return;
			}
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

		// Token: 0x060062D3 RID: 25299 RVA: 0x001F3B69 File Offset: 0x001F1D69
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

		// Token: 0x04006137 RID: 24887
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04006138 RID: 24888
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		// Token: 0x04006139 RID: 24889
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		// Token: 0x0400613A RID: 24890
		[Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
		public bool everyFrame;

		// Token: 0x0400613B RID: 24891
		[Tooltip("Event will only be sent if this bool is true - note that the bool must be true on state entry for a delayed event to be sent!")]
		public FsmBool activeBool;

		// Token: 0x0400613C RID: 24892
		private DelayedEvent delayedEvent;
	}
}
