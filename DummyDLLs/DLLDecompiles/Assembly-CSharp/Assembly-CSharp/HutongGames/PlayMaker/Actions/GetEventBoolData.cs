using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001082 RID: 4226
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Bool data from the last Event.")]
	public class GetEventBoolData : FsmStateAction
	{
		// Token: 0x06007323 RID: 29475 RVA: 0x00236218 File Offset: 0x00234418
		public override void Reset()
		{
			this.getBoolData = null;
		}

		// Token: 0x06007324 RID: 29476 RVA: 0x00236221 File Offset: 0x00234421
		public override void OnEnter()
		{
			this.getBoolData.Value = Fsm.EventData.BoolData;
			base.Finish();
		}

		// Token: 0x04007328 RID: 29480
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the bool data in a variable.")]
		public FsmBool getBoolData;
	}
}
