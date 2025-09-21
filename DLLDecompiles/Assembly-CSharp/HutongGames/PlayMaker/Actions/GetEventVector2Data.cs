using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001088 RID: 4232
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the Vector2 data from the last Event.")]
	public class GetEventVector2Data : FsmStateAction
	{
		// Token: 0x06007335 RID: 29493 RVA: 0x002365E2 File Offset: 0x002347E2
		public override void Reset()
		{
			this.getVector2Data = null;
		}

		// Token: 0x06007336 RID: 29494 RVA: 0x002365EB File Offset: 0x002347EB
		public override void OnEnter()
		{
			this.getVector2Data.Value = Fsm.EventData.Vector2Data;
			base.Finish();
		}

		// Token: 0x0400733E RID: 29502
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vector2 data in a variable.")]
		public FsmVector2 getVector2Data;
	}
}
