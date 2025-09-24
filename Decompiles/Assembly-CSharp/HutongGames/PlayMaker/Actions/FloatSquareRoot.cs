using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4F RID: 3151
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Get a Float Variable square root value")]
	public class FloatSquareRoot : FsmStateAction
	{
		// Token: 0x06005F84 RID: 24452 RVA: 0x001E50FA File Offset: 0x001E32FA
		public override void Reset()
		{
			this.floatVariable = null;
			this.result = null;
			this.everyFrame = false;
		}

		// Token: 0x06005F85 RID: 24453 RVA: 0x001E5111 File Offset: 0x001E3311
		public override void OnEnter()
		{
			this.DoFloatSquareRoot();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x001E5127 File Offset: 0x001E3327
		public override void OnUpdate()
		{
			this.DoFloatSquareRoot();
		}

		// Token: 0x06005F87 RID: 24455 RVA: 0x001E512F File Offset: 0x001E332F
		private void DoFloatSquareRoot()
		{
			if (!this.result.IsNone)
			{
				this.result.Value = Mathf.Sqrt(this.floatVariable.Value);
			}
		}

		// Token: 0x04005CDD RID: 23773
		public FsmFloat floatVariable;

		// Token: 0x04005CDE RID: 23774
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat result;

		// Token: 0x04005CDF RID: 23775
		public bool everyFrame;
	}
}
