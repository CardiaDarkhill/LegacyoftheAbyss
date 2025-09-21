using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C1 RID: 4801
	[Tooltip("Cancels all lag hits on target")]
	public class CancelAllLagHits : FsmStateAction
	{
		// Token: 0x06007D8D RID: 32141 RVA: 0x00256AE0 File Offset: 0x00254CE0
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007D8E RID: 32142 RVA: 0x00256AEC File Offset: 0x00254CEC
		public override void OnEnter()
		{
			HealthManager safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.CancelAllLagHits();
			}
			base.Finish();
		}

		// Token: 0x04007D7C RID: 32124
		[RequiredField]
		[CheckForComponent(typeof(HealthManager))]
		public FsmOwnerDefault Target;
	}
}
