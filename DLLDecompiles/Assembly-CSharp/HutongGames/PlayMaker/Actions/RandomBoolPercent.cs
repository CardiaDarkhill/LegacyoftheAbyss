using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE6 RID: 3302
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Bool Variable to True or False randomly.")]
	public class RandomBoolPercent : FsmStateAction
	{
		// Token: 0x0600622B RID: 25131 RVA: 0x001F08CC File Offset: 0x001EEACC
		public override void Reset()
		{
			this.trueChance = 50f;
			this.storeResult = null;
		}

		// Token: 0x0600622C RID: 25132 RVA: 0x001F08E8 File Offset: 0x001EEAE8
		public override void OnEnter()
		{
			float num = (float)Random.Range(1, 100);
			this.storeResult.Value = (num < this.trueChance.Value);
			base.Finish();
		}

		// Token: 0x04006046 RID: 24646
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		// Token: 0x04006047 RID: 24647
		public FsmFloat trueChance;
	}
}
