using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AA RID: 4266
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event.")]
	public class SendRandomEvent : FsmStateAction
	{
		// Token: 0x060073E1 RID: 29665 RVA: 0x002386C0 File Offset: 0x002368C0
		public override void Reset()
		{
			this.events = new FsmEvent[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.delay = null;
		}

		// Token: 0x060073E2 RID: 29666 RVA: 0x00238714 File Offset: 0x00236914
		public override void OnEnter()
		{
			if (this.events.Length != 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					if (this.delay.Value < 0.001f)
					{
						base.Fsm.Event(this.events[randomWeightedIndex]);
						base.Finish();
						return;
					}
					this.delayedEvent = base.Fsm.DelayedEvent(this.events[randomWeightedIndex], this.delay.Value);
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x060073E3 RID: 29667 RVA: 0x00238791 File Offset: 0x00236991
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x040073F4 RID: 29684
		[CompoundArray("Events", "Event", "Weight")]
		[Tooltip("A possible Event choice.")]
		public FsmEvent[] events;

		// Token: 0x040073F5 RID: 29685
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this Event being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x040073F6 RID: 29686
		[Tooltip("Optional delay in seconds before sending the event.")]
		public FsmFloat delay;

		// Token: 0x040073F7 RID: 29687
		private DelayedEvent delayedEvent;
	}
}
