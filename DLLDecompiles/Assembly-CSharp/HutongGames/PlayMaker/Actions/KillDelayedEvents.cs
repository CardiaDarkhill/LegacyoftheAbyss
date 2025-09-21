using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109F RID: 4255
	[ActionCategory(ActionCategory.StateMachine)]
	[Note("Kill all queued delayed events.")]
	[Tooltip("Kill all queued delayed events. Normally delayed events are automatically killed when the active state is exited, but you can override this behaviour in FSM settings. If you choose to keep delayed events you can use this action to kill them when needed.")]
	public class KillDelayedEvents : FsmStateAction
	{
		// Token: 0x060073A0 RID: 29600 RVA: 0x00237C1A File Offset: 0x00235E1A
		public override void OnEnter()
		{
			base.Fsm.KillDelayedEvents();
			base.Finish();
		}
	}
}
