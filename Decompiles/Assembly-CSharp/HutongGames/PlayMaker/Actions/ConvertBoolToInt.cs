using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E69 RID: 3689
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to an Integer value.")]
	public class ConvertBoolToInt : FsmStateAction
	{
		// Token: 0x0600693F RID: 26943 RVA: 0x002103D5 File Offset: 0x0020E5D5
		public override void Reset()
		{
			this.boolVariable = null;
			this.intVariable = null;
			this.falseValue = 0;
			this.trueValue = 1;
			this.everyFrame = false;
		}

		// Token: 0x06006940 RID: 26944 RVA: 0x00210404 File Offset: 0x0020E604
		public override void OnEnter()
		{
			this.DoConvertBoolToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006941 RID: 26945 RVA: 0x0021041A File Offset: 0x0020E61A
		public override void OnUpdate()
		{
			this.DoConvertBoolToInt();
		}

		// Token: 0x06006942 RID: 26946 RVA: 0x00210422 File Offset: 0x0020E622
		private void DoConvertBoolToInt()
		{
			this.intVariable.Value = (this.boolVariable.Value ? this.trueValue.Value : this.falseValue.Value);
		}

		// Token: 0x04006887 RID: 26759
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to convert.")]
		public FsmBool boolVariable;

		// Token: 0x04006888 RID: 26760
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Integer variable to set based on the Bool variable value.")]
		public FsmInt intVariable;

		// Token: 0x04006889 RID: 26761
		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt falseValue;

		// Token: 0x0400688A RID: 26762
		[Tooltip("Integer value if Bool variable is false.")]
		public FsmInt trueValue;

		// Token: 0x0400688B RID: 26763
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
