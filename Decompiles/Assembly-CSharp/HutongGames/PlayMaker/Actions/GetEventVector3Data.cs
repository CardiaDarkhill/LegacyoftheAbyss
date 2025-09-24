using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001089 RID: 4233
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Vector3 data from the last Event.")]
	public class GetEventVector3Data : FsmStateAction
	{
		// Token: 0x06007338 RID: 29496 RVA: 0x00236610 File Offset: 0x00234810
		public override void Reset()
		{
			this.getVector3Data = null;
		}

		// Token: 0x06007339 RID: 29497 RVA: 0x00236619 File Offset: 0x00234819
		public override void OnEnter()
		{
			this.getVector3Data.Value = Fsm.EventData.Vector3Data;
			base.Finish();
		}

		// Token: 0x0400733F RID: 29503
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vector3 data in a variable.")]
		public FsmVector3 getVector3Data;
	}
}
