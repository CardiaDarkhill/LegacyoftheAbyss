using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4C RID: 3148
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Multiplies one Float by another.")]
	public class FloatMultiplyV2 : FsmStateAction
	{
		// Token: 0x06005F75 RID: 24437 RVA: 0x001E4FA6 File Offset: 0x001E31A6
		public override void Reset()
		{
			this.floatVariable = null;
			this.multiplyBy = null;
			this.everyFrame = false;
			this.fixedUpdate = false;
		}

		// Token: 0x06005F76 RID: 24438 RVA: 0x001E4FC4 File Offset: 0x001E31C4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005F77 RID: 24439 RVA: 0x001E4FD2 File Offset: 0x001E31D2
		public override void OnEnter()
		{
			this.floatVariable.Value *= this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F78 RID: 24440 RVA: 0x001E4FFF File Offset: 0x001E31FF
		public override void OnUpdate()
		{
			if (!this.fixedUpdate)
			{
				this.floatVariable.Value *= this.multiplyBy.Value;
			}
		}

		// Token: 0x06005F79 RID: 24441 RVA: 0x001E5026 File Offset: 0x001E3226
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.floatVariable.Value *= this.multiplyBy.Value;
			}
		}

		// Token: 0x04005CD3 RID: 23763
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to multiply.")]
		public FsmFloat floatVariable;

		// Token: 0x04005CD4 RID: 23764
		[RequiredField]
		[Tooltip("Multiply the float variable by this value.")]
		public FsmFloat multiplyBy;

		// Token: 0x04005CD5 RID: 23765
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x04005CD6 RID: 23766
		public bool fixedUpdate;
	}
}
