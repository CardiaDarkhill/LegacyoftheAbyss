using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B0 RID: 4272
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
	public class SetEventTarget : FsmStateAction
	{
		// Token: 0x060073F7 RID: 29687 RVA: 0x00238B5C File Offset: 0x00236D5C
		public override void Reset()
		{
			this.eventTarget = null;
			this.everyFrame = true;
		}

		// Token: 0x060073F8 RID: 29688 RVA: 0x00238B6C File Offset: 0x00236D6C
		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		// Token: 0x060073F9 RID: 29689 RVA: 0x00238B75 File Offset: 0x00236D75
		public override void OnEnter()
		{
			base.Fsm.EventTarget = this.eventTarget;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060073FA RID: 29690 RVA: 0x00238B96 File Offset: 0x00236D96
		public override void OnUpdate()
		{
			base.Fsm.EventTarget = this.eventTarget;
		}

		// Token: 0x0400740D RID: 29709
		[Tooltip("Set the target.")]
		public FsmEventTarget eventTarget;

		// Token: 0x0400740E RID: 29710
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
