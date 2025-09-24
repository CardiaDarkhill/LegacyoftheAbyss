using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F79 RID: 3961
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Multiplies one Float by another.")]
	public class FloatMultiply : FsmStateAction
	{
		// Token: 0x06006DBD RID: 28093 RVA: 0x002213CA File Offset: 0x0021F5CA
		public override void Reset()
		{
			this.floatVariable = null;
			this.multiplyBy = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DBE RID: 28094 RVA: 0x002213E1 File Offset: 0x0021F5E1
		public override void OnEnter()
		{
			this.floatVariable.Value *= this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DBF RID: 28095 RVA: 0x0022140E File Offset: 0x0021F60E
		public override void OnUpdate()
		{
			this.floatVariable.Value *= this.multiplyBy.Value;
		}

		// Token: 0x04006D79 RID: 28025
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to multiply.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D7A RID: 28026
		[RequiredField]
		[Tooltip("Multiply the float variable by this value.")]
		public FsmFloat multiplyBy;

		// Token: 0x04006D7B RID: 28027
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;
	}
}
