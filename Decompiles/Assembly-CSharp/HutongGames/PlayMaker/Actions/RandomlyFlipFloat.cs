using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100E RID: 4110
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either leave a flaot as is or multiply it by -1")]
	public class RandomlyFlipFloat : FsmStateAction
	{
		// Token: 0x06007107 RID: 28935 RVA: 0x0022CD4D File Offset: 0x0022AF4D
		public override void Reset()
		{
			this.storeResult = null;
		}

		// Token: 0x06007108 RID: 28936 RVA: 0x0022CD56 File Offset: 0x0022AF56
		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				this.storeResult.Value = this.storeResult.Value * -1f;
			}
			base.Finish();
		}

		// Token: 0x0400709E RID: 28830
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;
	}
}
