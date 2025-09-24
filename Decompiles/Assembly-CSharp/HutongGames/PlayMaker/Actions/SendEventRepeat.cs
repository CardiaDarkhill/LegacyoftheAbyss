using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001064 RID: 4196
	[ActionCategory(ActionCategory.StateMachine)]
	public class SendEventRepeat : FsmStateAction
	{
		// Token: 0x060072AC RID: 29356 RVA: 0x00234A94 File Offset: 0x00232C94
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.repeatTime = null;
			this.sendEventOnEntry = false;
		}

		// Token: 0x060072AD RID: 29357 RVA: 0x00234AB2 File Offset: 0x00232CB2
		public override void OnEnter()
		{
			if (this.sendEventOnEntry)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
			}
			this.timer = 0f;
		}

		// Token: 0x060072AE RID: 29358 RVA: 0x00234AE4 File Offset: 0x00232CE4
		public override void OnUpdate()
		{
			if (this.timer < this.repeatTime.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
			this.timer -= this.repeatTime.Value;
		}

		// Token: 0x040072AF RID: 29359
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x040072B0 RID: 29360
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		// Token: 0x040072B1 RID: 29361
		public FsmFloat repeatTime;

		// Token: 0x040072B2 RID: 29362
		public bool sendEventOnEntry;

		// Token: 0x040072B3 RID: 29363
		private float timer;
	}
}
