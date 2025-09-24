using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109D RID: 4253
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Immediately return to the previously active state.")]
	public class GotoPreviousState : FsmStateAction
	{
		// Token: 0x06007398 RID: 29592 RVA: 0x00237A73 File Offset: 0x00235C73
		public override void Reset()
		{
		}

		// Token: 0x06007399 RID: 29593 RVA: 0x00237A75 File Offset: 0x00235C75
		public override void OnEnter()
		{
			if (base.Fsm.PreviousActiveState != null)
			{
				base.Log("Goto Previous State: " + base.Fsm.PreviousActiveState.Name);
				base.Fsm.GotoPreviousState();
			}
			base.Finish();
		}
	}
}
