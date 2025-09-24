using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001085 RID: 4229
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Int data from the last Event.")]
	public class GetEventIntData : FsmStateAction
	{
		// Token: 0x0600732C RID: 29484 RVA: 0x0023649F File Offset: 0x0023469F
		public override void Reset()
		{
			this.getIntData = null;
		}

		// Token: 0x0600732D RID: 29485 RVA: 0x002364A8 File Offset: 0x002346A8
		public override void OnEnter()
		{
			this.getIntData.Value = Fsm.EventData.IntData;
			base.Finish();
		}

		// Token: 0x04007339 RID: 29497
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the int data in a variable.")]
		public FsmInt getIntData;
	}
}
