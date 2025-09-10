using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AE RID: 4270
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the Int data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventIntData : FsmStateAction
	{
		// Token: 0x060073F1 RID: 29681 RVA: 0x00238B00 File Offset: 0x00236D00
		public override void Reset()
		{
			this.intData = null;
		}

		// Token: 0x060073F2 RID: 29682 RVA: 0x00238B09 File Offset: 0x00236D09
		public override void OnEnter()
		{
			Fsm.EventData.IntData = this.intData.Value;
			base.Finish();
		}

		// Token: 0x0400740B RID: 29707
		[Tooltip("The int value to send.")]
		public FsmInt intData;
	}
}
