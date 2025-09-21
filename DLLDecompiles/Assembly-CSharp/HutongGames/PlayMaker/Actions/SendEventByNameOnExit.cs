using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0E RID: 3342
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByNameOnExit : FsmStateAction
	{
		// Token: 0x060062CA RID: 25290 RVA: 0x001F399F File Offset: 0x001F1B9F
		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x001F39AF File Offset: 0x001F1BAF
		public override void OnExit()
		{
			base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
		}

		// Token: 0x04006133 RID: 24883
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04006134 RID: 24884
		[RequiredField]
		public FsmString sendEvent;
	}
}
