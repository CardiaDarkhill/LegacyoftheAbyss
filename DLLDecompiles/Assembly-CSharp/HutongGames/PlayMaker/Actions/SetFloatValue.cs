using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8C RID: 3980
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class SetFloatValue : FsmStateAction
	{
		// Token: 0x06006E10 RID: 28176 RVA: 0x0022205E File Offset: 0x0022025E
		public override void Reset()
		{
			this.floatVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E11 RID: 28177 RVA: 0x00222075 File Offset: 0x00220275
		public override void OnEnter()
		{
			this.floatVariable.Value = this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E12 RID: 28178 RVA: 0x0022209B File Offset: 0x0022029B
		public override void OnUpdate()
		{
			this.floatVariable.Value = this.floatValue.Value;
		}

		// Token: 0x04006DBF RID: 28095
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Variable to set.")]
		public FsmFloat floatVariable;

		// Token: 0x04006DC0 RID: 28096
		[RequiredField]
		[Tooltip("Value to set it to.")]
		public FsmFloat floatValue;

		// Token: 0x04006DC1 RID: 28097
		[Tooltip("Perform this action every frame. Useful if the Value is changing.")]
		public bool everyFrame;
	}
}
