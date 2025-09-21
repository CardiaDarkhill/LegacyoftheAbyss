using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C02 RID: 3074
	[ActionCategory(ActionCategory.Convert)]
	public class ConvertDoubleBoolToString : FsmStateAction
	{
		// Token: 0x06005DEE RID: 24046 RVA: 0x001D9C95 File Offset: 0x001D7E95
		public override void Reset()
		{
			this.boolVariable1 = null;
			this.boolVariable2 = null;
			this.stringVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DEF RID: 24047 RVA: 0x001D9CB3 File Offset: 0x001D7EB3
		public override void OnEnter()
		{
			this.DoConvertBoolToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DF0 RID: 24048 RVA: 0x001D9CC9 File Offset: 0x001D7EC9
		public override void OnUpdate()
		{
			this.DoConvertBoolToFloat();
		}

		// Token: 0x06005DF1 RID: 24049 RVA: 0x001D9CD4 File Offset: 0x001D7ED4
		private void DoConvertBoolToFloat()
		{
			if (!this.boolVariable1.Value && !this.boolVariable2.Value)
			{
				this.stringVariable.Value = this.bothFalseValue.Value;
				return;
			}
			if (this.boolVariable1.Value && this.boolVariable2.Value)
			{
				this.stringVariable.Value = this.bothTrueValue.Value;
				return;
			}
			this.stringVariable.Value = this.oneTrueValue.Value;
		}

		// Token: 0x04005A48 RID: 23112
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable1;

		// Token: 0x04005A49 RID: 23113
		[UIHint(UIHint.Variable)]
		public FsmBool boolVariable2;

		// Token: 0x04005A4A RID: 23114
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		// Token: 0x04005A4B RID: 23115
		public FsmString bothFalseValue;

		// Token: 0x04005A4C RID: 23116
		public FsmString oneTrueValue;

		// Token: 0x04005A4D RID: 23117
		public FsmString bothTrueValue;

		// Token: 0x04005A4E RID: 23118
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
