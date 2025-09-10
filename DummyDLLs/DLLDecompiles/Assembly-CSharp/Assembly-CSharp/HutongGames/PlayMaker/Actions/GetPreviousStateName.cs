using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109C RID: 4252
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the name of the previously active state and stores it in a String Variable.")]
	public class GetPreviousStateName : FsmStateAction
	{
		// Token: 0x06007395 RID: 29589 RVA: 0x00237A2F File Offset: 0x00235C2F
		public override void Reset()
		{
			this.storeName = null;
		}

		// Token: 0x06007396 RID: 29590 RVA: 0x00237A38 File Offset: 0x00235C38
		public override void OnEnter()
		{
			this.storeName.Value = ((base.Fsm.PreviousActiveState == null) ? null : base.Fsm.PreviousActiveState.Name);
			base.Finish();
		}

		// Token: 0x040073C9 RID: 29641
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name in a String Variable.")]
		public FsmString storeName;
	}
}
