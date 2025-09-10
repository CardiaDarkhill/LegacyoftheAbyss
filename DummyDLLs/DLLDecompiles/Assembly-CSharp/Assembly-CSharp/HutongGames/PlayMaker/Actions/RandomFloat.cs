using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F86 RID: 3974
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float Variable to a random value between Min/Max.")]
	public class RandomFloat : FsmStateAction
	{
		// Token: 0x06006DF8 RID: 28152 RVA: 0x00221C0A File Offset: 0x0021FE0A
		public override void Reset()
		{
			this.min = 0f;
			this.max = 1f;
			this.storeResult = null;
		}

		// Token: 0x06006DF9 RID: 28153 RVA: 0x00221C33 File Offset: 0x0021FE33
		public override void OnEnter()
		{
			this.storeResult.Value = Random.Range(this.min.Value, this.max.Value);
			base.Finish();
		}

		// Token: 0x04006DA8 RID: 28072
		[RequiredField]
		[Tooltip("Minimum value for the random number.")]
		public FsmFloat min;

		// Token: 0x04006DA9 RID: 28073
		[RequiredField]
		[Tooltip("Maximum value for the random number.")]
		public FsmFloat max;

		// Token: 0x04006DAA RID: 28074
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Float variable.")]
		public FsmFloat storeResult;
	}
}
