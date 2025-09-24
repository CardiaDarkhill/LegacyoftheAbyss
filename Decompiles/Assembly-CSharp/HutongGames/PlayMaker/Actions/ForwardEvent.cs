using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001080 RID: 4224
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Forward an event received by this FSM to another target.")]
	public class ForwardEvent : FsmStateAction
	{
		// Token: 0x0600731A RID: 29466 RVA: 0x0023604E File Offset: 0x0023424E
		public override void Reset()
		{
			this.forwardTo = new FsmEventTarget
			{
				target = FsmEventTarget.EventTarget.FSMComponent
			};
			this.eventsToForward = null;
			this.eatEvents = true;
		}

		// Token: 0x0600731B RID: 29467 RVA: 0x00236070 File Offset: 0x00234270
		public override void Awake()
		{
			base.HandlesOnEvent = true;
		}

		// Token: 0x0600731C RID: 29468 RVA: 0x0023607C File Offset: 0x0023427C
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.eventsToForward != null)
			{
				FsmEvent[] array = this.eventsToForward;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == fsmEvent)
					{
						base.Fsm.Event(this.forwardTo, fsmEvent);
						return this.eatEvents;
					}
				}
			}
			return false;
		}

		// Token: 0x0400731D RID: 29469
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		// Token: 0x0400731E RID: 29470
		[Tooltip("The events to forward.")]
		public FsmEvent[] eventsToForward;

		// Token: 0x0400731F RID: 29471
		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;
	}
}
