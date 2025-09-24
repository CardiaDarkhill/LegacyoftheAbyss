using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A0 RID: 4256
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in LateUpdate, after the Update loop.")]
	public class LateUpdateEvent : FsmStateAction
	{
		// Token: 0x060073A2 RID: 29602 RVA: 0x00237C35 File Offset: 0x00235E35
		public override void Reset()
		{
			this.sendEvent = null;
		}

		// Token: 0x060073A3 RID: 29603 RVA: 0x00237C3E File Offset: 0x00235E3E
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x060073A4 RID: 29604 RVA: 0x00237C4C File Offset: 0x00235E4C
		public override void OnEnter()
		{
		}

		// Token: 0x060073A5 RID: 29605 RVA: 0x00237C4E File Offset: 0x00235E4E
		public override void OnLateUpdate()
		{
			base.Finish();
			base.Fsm.Event(this.sendEvent);
		}

		// Token: 0x040073CD RID: 29645
		[RequiredField]
		[Tooltip("Event to send in LateUpdate.")]
		public FsmEvent sendEvent;
	}
}
