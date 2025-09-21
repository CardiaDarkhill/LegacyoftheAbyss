using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6B RID: 3691
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts an Enum value to a String value.")]
	public class ConvertEnumToString : FsmStateAction
	{
		// Token: 0x06006949 RID: 26953 RVA: 0x002104EB File Offset: 0x0020E6EB
		public override void Reset()
		{
			this.enumVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x0600694A RID: 26954 RVA: 0x00210502 File Offset: 0x0020E702
		public override void OnEnter()
		{
			this.DoConvertEnumToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600694B RID: 26955 RVA: 0x00210518 File Offset: 0x0020E718
		public override void OnUpdate()
		{
			this.DoConvertEnumToString();
		}

		// Token: 0x0600694C RID: 26956 RVA: 0x00210520 File Offset: 0x0020E720
		private void DoConvertEnumToString()
		{
			this.stringVariable.Value = ((this.enumVariable.Value != null) ? this.enumVariable.Value.ToString() : "");
		}

		// Token: 0x04006891 RID: 26769
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum variable to convert.")]
		public FsmEnum enumVariable;

		// Token: 0x04006892 RID: 26770
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String variable to store the converted value.")]
		public FsmString stringVariable;

		// Token: 0x04006893 RID: 26771
		[Tooltip("Repeat every frame. Useful if the Enum variable is changing.")]
		public bool everyFrame;
	}
}
