using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AD RID: 4269
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the Float data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventFloatData : FsmStateAction
	{
		// Token: 0x060073EE RID: 29678 RVA: 0x00238AD2 File Offset: 0x00236CD2
		public override void Reset()
		{
			this.floatData = null;
		}

		// Token: 0x060073EF RID: 29679 RVA: 0x00238ADB File Offset: 0x00236CDB
		public override void OnEnter()
		{
			Fsm.EventData.FloatData = this.floatData.Value;
			base.Finish();
		}

		// Token: 0x0400740A RID: 29706
		[Tooltip("The float value to send.")]
		public FsmFloat floatData;
	}
}
