using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C01 RID: 3073
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Float value.")]
	public class ConvertDoubleBoolToFloat : FsmStateAction
	{
		// Token: 0x06005DE9 RID: 24041 RVA: 0x001D9B90 File Offset: 0x001D7D90
		public override void Reset()
		{
			this.boolVariable1 = null;
			this.boolVariable2 = null;
			this.floatVariable = null;
			this.bothFalseValue = 0f;
			this.oneTrueValue = 0.5f;
			this.bothTrueValue = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x001D9BE9 File Offset: 0x001D7DE9
		public override void OnEnter()
		{
			this.DoConvertBoolToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x001D9BFF File Offset: 0x001D7DFF
		public override void OnUpdate()
		{
			this.DoConvertBoolToFloat();
		}

		// Token: 0x06005DEC RID: 24044 RVA: 0x001D9C08 File Offset: 0x001D7E08
		private void DoConvertBoolToFloat()
		{
			if (!this.boolVariable1.Value && !this.boolVariable2.Value)
			{
				this.floatVariable.Value = this.bothFalseValue.Value;
				return;
			}
			if (this.boolVariable1.Value && this.boolVariable2.Value)
			{
				this.floatVariable.Value = this.bothTrueValue.Value;
				return;
			}
			this.floatVariable.Value = this.oneTrueValue.Value;
		}

		// Token: 0x04005A41 RID: 23105
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable1;

		// Token: 0x04005A42 RID: 23106
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable2;

		// Token: 0x04005A43 RID: 23107
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to set based on the Bool variable value.")]
		public FsmFloat floatVariable;

		// Token: 0x04005A44 RID: 23108
		public FsmFloat bothFalseValue;

		// Token: 0x04005A45 RID: 23109
		public FsmFloat oneTrueValue;

		// Token: 0x04005A46 RID: 23110
		public FsmFloat bothTrueValue;

		// Token: 0x04005A47 RID: 23111
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
