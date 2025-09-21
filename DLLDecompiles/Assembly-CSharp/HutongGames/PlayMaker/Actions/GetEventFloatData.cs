using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001083 RID: 4227
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Float data from the last Event.")]
	public class GetEventFloatData : FsmStateAction
	{
		// Token: 0x06007326 RID: 29478 RVA: 0x00236246 File Offset: 0x00234446
		public override void Reset()
		{
			this.getFloatData = null;
		}

		// Token: 0x06007327 RID: 29479 RVA: 0x0023624F File Offset: 0x0023444F
		public override void OnEnter()
		{
			this.getFloatData.Value = Fsm.EventData.FloatData;
			base.Finish();
		}

		// Token: 0x04007329 RID: 29481
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the float data in a variable.")]
		public FsmFloat getFloatData;
	}
}
