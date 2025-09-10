using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6A RID: 3690
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a String value.")]
	public class ConvertBoolToString : FsmStateAction
	{
		// Token: 0x06006944 RID: 26948 RVA: 0x0021045C File Offset: 0x0020E65C
		public override void Reset()
		{
			this.boolVariable = null;
			this.stringVariable = null;
			this.falseString = "False";
			this.trueString = "True";
			this.everyFrame = false;
		}

		// Token: 0x06006945 RID: 26949 RVA: 0x00210493 File Offset: 0x0020E693
		public override void OnEnter()
		{
			this.DoConvertBoolToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006946 RID: 26950 RVA: 0x002104A9 File Offset: 0x0020E6A9
		public override void OnUpdate()
		{
			this.DoConvertBoolToString();
		}

		// Token: 0x06006947 RID: 26951 RVA: 0x002104B1 File Offset: 0x0020E6B1
		private void DoConvertBoolToString()
		{
			this.stringVariable.Value = (this.boolVariable.Value ? this.trueString.Value : this.falseString.Value);
		}

		// Token: 0x0400688C RID: 26764
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to convert.")]
		public FsmBool boolVariable;

		// Token: 0x0400688D RID: 26765
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String variable to set based on the Bool variable value.")]
		public FsmString stringVariable;

		// Token: 0x0400688E RID: 26766
		[Tooltip("String value if Bool variable is false.")]
		public FsmString falseString;

		// Token: 0x0400688F RID: 26767
		[Tooltip("String value if Bool variable is true.")]
		public FsmString trueString;

		// Token: 0x04006890 RID: 26768
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
