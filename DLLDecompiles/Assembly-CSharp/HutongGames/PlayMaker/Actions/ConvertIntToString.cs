using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6F RID: 3695
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts an Integer value to a String value with an optional format.")]
	public class ConvertIntToString : FsmStateAction
	{
		// Token: 0x0600695D RID: 26973 RVA: 0x0021072A File Offset: 0x0020E92A
		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		// Token: 0x0600695E RID: 26974 RVA: 0x00210748 File Offset: 0x0020E948
		public override void OnEnter()
		{
			this.DoConvertIntToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x0021075E File Offset: 0x0020E95E
		public override void OnUpdate()
		{
			this.DoConvertIntToString();
		}

		// Token: 0x06006960 RID: 26976 RVA: 0x00210768 File Offset: 0x0020E968
		private void DoConvertIntToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.intVariable.Value.ToString();
				return;
			}
			this.stringVariable.Value = this.intVariable.Value.ToString(this.format.Value);
		}

		// Token: 0x0400689F RID: 26783
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Int variable to convert.")]
		public FsmInt intVariable;

		// Token: 0x040068A0 RID: 26784
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("A String variable to store the converted value.")]
		public FsmString stringVariable;

		// Token: 0x040068A1 RID: 26785
		[Tooltip("Optional Format, allows for leading zeros. E.g., 0000")]
		public FsmString format;

		// Token: 0x040068A2 RID: 26786
		[Tooltip("Repeat every frame. Useful if the Int variable is changing.")]
		public bool everyFrame;
	}
}
