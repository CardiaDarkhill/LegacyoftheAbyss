using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AF RID: 4271
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the String data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventStringData : FsmStateAction
	{
		// Token: 0x060073F4 RID: 29684 RVA: 0x00238B2E File Offset: 0x00236D2E
		public override void Reset()
		{
			this.stringData = null;
		}

		// Token: 0x060073F5 RID: 29685 RVA: 0x00238B37 File Offset: 0x00236D37
		public override void OnEnter()
		{
			Fsm.EventData.StringData = this.stringData.Value;
			base.Finish();
		}

		// Token: 0x0400740C RID: 29708
		[Tooltip("The string value to send.")]
		public FsmString stringData;
	}
}
