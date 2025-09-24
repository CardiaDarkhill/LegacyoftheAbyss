using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200107F RID: 4223
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Forwards all event received by this FSM to another target. Optionally specify a list of events to ignore.")]
	public class ForwardAllEvents : FsmStateAction
	{
		// Token: 0x06007316 RID: 29462 RVA: 0x00235FC3 File Offset: 0x002341C3
		public override void Reset()
		{
			this.forwardTo = new FsmEventTarget
			{
				target = FsmEventTarget.EventTarget.FSMComponent
			};
			this.exceptThese = new FsmEvent[]
			{
				FsmEvent.Finished
			};
			this.eatEvents = true;
		}

		// Token: 0x06007317 RID: 29463 RVA: 0x00235FF2 File Offset: 0x002341F2
		public override void Awake()
		{
			base.HandlesOnEvent = true;
		}

		// Token: 0x06007318 RID: 29464 RVA: 0x00235FFC File Offset: 0x002341FC
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.exceptThese != null)
			{
				FsmEvent[] array = this.exceptThese;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == fsmEvent)
					{
						return false;
					}
				}
			}
			base.Fsm.Event(this.forwardTo, fsmEvent);
			return this.eatEvents;
		}

		// Token: 0x0400731A RID: 29466
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		// Token: 0x0400731B RID: 29467
		[Tooltip("Don't forward these events.")]
		public FsmEvent[] exceptThese;

		// Token: 0x0400731C RID: 29468
		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;
	}
}
