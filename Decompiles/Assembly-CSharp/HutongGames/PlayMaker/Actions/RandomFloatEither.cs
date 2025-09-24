using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200100D RID: 4109
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float Variable to a random choice of two floats.")]
	public class RandomFloatEither : FsmStateAction
	{
		// Token: 0x06007104 RID: 28932 RVA: 0x0022CCC9 File Offset: 0x0022AEC9
		public override void Reset()
		{
			this.value1 = 0f;
			this.value2 = 1f;
			this.storeResult = null;
		}

		// Token: 0x06007105 RID: 28933 RVA: 0x0022CCF4 File Offset: 0x0022AEF4
		public override void OnEnter()
		{
			if ((float)Random.Range(0, 100) < 50f)
			{
				this.storeResult.Value = this.value1.Value;
			}
			else
			{
				this.storeResult.Value = this.value2.Value;
			}
			base.Finish();
		}

		// Token: 0x0400709B RID: 28827
		[RequiredField]
		public FsmFloat value1;

		// Token: 0x0400709C RID: 28828
		[RequiredField]
		public FsmFloat value2;

		// Token: 0x0400709D RID: 28829
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;
	}
}
