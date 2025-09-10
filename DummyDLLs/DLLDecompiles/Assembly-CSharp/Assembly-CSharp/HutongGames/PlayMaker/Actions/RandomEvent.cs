using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A4 RID: 4260
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random State Event after an optional delay. Use this to transition to a random state connected to the current state.")]
	public class RandomEvent : FsmStateAction
	{
		// Token: 0x060073AF RID: 29615 RVA: 0x00237DA9 File Offset: 0x00235FA9
		public override void Reset()
		{
			this.delay = null;
			this.noRepeat = false;
		}

		// Token: 0x060073B0 RID: 29616 RVA: 0x00237DC0 File Offset: 0x00235FC0
		public override void OnEnter()
		{
			if (base.State.Transitions.Length == 0)
			{
				return;
			}
			if (this.lastEventIndex == -1)
			{
				this.lastEventIndex = Random.Range(0, base.State.Transitions.Length);
			}
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.GetRandomEvent());
				base.Finish();
				return;
			}
			this.delayedEvent = base.Fsm.DelayedEvent(this.GetRandomEvent(), this.delay.Value);
		}

		// Token: 0x060073B1 RID: 29617 RVA: 0x00237E4A File Offset: 0x0023604A
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x060073B2 RID: 29618 RVA: 0x00237E60 File Offset: 0x00236060
		private FsmEvent GetRandomEvent()
		{
			do
			{
				this.randomEventIndex = Random.Range(0, base.State.Transitions.Length);
			}
			while (this.noRepeat.Value && base.State.Transitions.Length > 1 && this.randomEventIndex == this.lastEventIndex);
			this.lastEventIndex = this.randomEventIndex;
			return base.State.Transitions[this.randomEventIndex].FsmEvent;
		}

		// Token: 0x040073D9 RID: 29657
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay before sending the event (seconds).")]
		public FsmFloat delay;

		// Token: 0x040073DA RID: 29658
		[Tooltip("Don't repeat the same event twice in a row.")]
		public FsmBool noRepeat;

		// Token: 0x040073DB RID: 29659
		private DelayedEvent delayedEvent;

		// Token: 0x040073DC RID: 29660
		private int randomEventIndex;

		// Token: 0x040073DD RID: 29661
		private int lastEventIndex = -1;
	}
}
