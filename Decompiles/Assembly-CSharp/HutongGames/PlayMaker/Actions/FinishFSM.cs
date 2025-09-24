using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200107E RID: 4222
	[ActionCategory(ActionCategory.StateMachine)]
	[Note("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
	[Tooltip("Stop this FSM. If this FSM was launched by a {{Run FSM}} action, it will trigger a Finish event in that state.")]
	public class FinishFSM : FsmStateAction
	{
		// Token: 0x06007314 RID: 29460 RVA: 0x00235FAE File Offset: 0x002341AE
		public override void OnEnter()
		{
			base.Fsm.Stop();
		}
	}
}
