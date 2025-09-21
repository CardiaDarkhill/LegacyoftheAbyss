using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E68 RID: 3688
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Float value.")]
	public class ConvertBoolToFloat : FsmStateAction
	{
		// Token: 0x0600693A RID: 26938 RVA: 0x00210308 File Offset: 0x0020E508
		public override void Reset()
		{
			this.boolVariable = null;
			this.floatVariable = null;
			this.falseValue = 0f;
			this.trueValue = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600693B RID: 26939 RVA: 0x0021033F File Offset: 0x0020E53F
		public override void OnEnter()
		{
			this.DoConvertBoolToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600693C RID: 26940 RVA: 0x00210355 File Offset: 0x0020E555
		public override void OnUpdate()
		{
			this.DoConvertBoolToFloat();
		}

		// Token: 0x0600693D RID: 26941 RVA: 0x00210360 File Offset: 0x0020E560
		private void DoConvertBoolToFloat()
		{
			if (this.boolVariable.Value && !this.trueValue.IsNone)
			{
				this.floatVariable.Value = this.trueValue.Value;
			}
			if (!this.boolVariable.Value && !this.falseValue.IsNone)
			{
				this.floatVariable.Value = this.falseValue.Value;
			}
		}

		// Token: 0x04006882 RID: 26754
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to convert.")]
		public FsmBool boolVariable;

		// Token: 0x04006883 RID: 26755
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to set based on the Bool variable value.")]
		public FsmFloat floatVariable;

		// Token: 0x04006884 RID: 26756
		[Tooltip("Float value if Bool variable is false.")]
		public FsmFloat falseValue;

		// Token: 0x04006885 RID: 26757
		[Tooltip("Float value if Bool variable is true.")]
		public FsmFloat trueValue;

		// Token: 0x04006886 RID: 26758
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
