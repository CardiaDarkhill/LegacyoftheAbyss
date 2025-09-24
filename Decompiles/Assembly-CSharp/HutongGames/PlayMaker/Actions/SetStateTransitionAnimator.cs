using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FE RID: 4606
	public class SetStateTransitionAnimator : FSMUtility.GetComponentFsmStateAction<StateTransitionAnimator>
	{
		// Token: 0x06007AA0 RID: 31392 RVA: 0x0024CF52 File Offset: 0x0024B152
		public override void Reset()
		{
			base.Reset();
			this.SetState = null;
			this.IsInstant = null;
		}

		// Token: 0x06007AA1 RID: 31393 RVA: 0x0024CF68 File Offset: 0x0024B168
		protected override void DoAction(StateTransitionAnimator component)
		{
			component.SetState(this.SetState.Value, this.IsInstant.Value);
		}

		// Token: 0x04007AE7 RID: 31463
		public FsmBool SetState;

		// Token: 0x04007AE8 RID: 31464
		public FsmBool IsInstant;
	}
}
