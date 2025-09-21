using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100F RID: 4111
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either leave a int as is or multiply it by -1")]
	public class RandomlyFlipInt : FsmStateAction
	{
		// Token: 0x0600710A RID: 28938 RVA: 0x0022CD93 File Offset: 0x0022AF93
		public override void Reset()
		{
			this.storeResult = null;
		}

		// Token: 0x0600710B RID: 28939 RVA: 0x0022CD9C File Offset: 0x0022AF9C
		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				this.storeResult.Value = this.storeResult.Value * -1;
			}
			base.Finish();
		}

		// Token: 0x0400709F RID: 28831
		[UIHint(UIHint.Variable)]
		public FsmInt storeResult;
	}
}
