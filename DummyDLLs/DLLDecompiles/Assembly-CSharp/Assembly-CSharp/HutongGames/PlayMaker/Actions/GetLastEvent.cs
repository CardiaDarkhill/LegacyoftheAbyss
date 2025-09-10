using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109B RID: 4251
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the event that caused the transition to the current state, and stores it in a String Variable.")]
	public class GetLastEvent : FsmStateAction
	{
		// Token: 0x06007392 RID: 29586 RVA: 0x002379E7 File Offset: 0x00235BE7
		public override void Reset()
		{
			this.storeEvent = null;
		}

		// Token: 0x06007393 RID: 29587 RVA: 0x002379F0 File Offset: 0x00235BF0
		public override void OnEnter()
		{
			this.storeEvent.Value = ((base.Fsm.LastTransition == null) ? "START" : base.Fsm.LastTransition.EventName);
			base.Finish();
		}

		// Token: 0x040073C8 RID: 29640
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name of the last event in a String Variable.")]
		public FsmString storeEvent;
	}
}
