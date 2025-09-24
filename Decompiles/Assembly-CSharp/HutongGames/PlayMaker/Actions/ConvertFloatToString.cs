using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6D RID: 3693
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Float value to a String value with optional format.")]
	public class ConvertFloatToString : FsmStateAction
	{
		// Token: 0x06006953 RID: 26963 RVA: 0x0021061A File Offset: 0x0020E81A
		public override void Reset()
		{
			this.floatVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
			this.format = null;
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x00210638 File Offset: 0x0020E838
		public override void OnEnter()
		{
			this.DoConvertFloatToString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x0021064E File Offset: 0x0020E84E
		public override void OnUpdate()
		{
			this.DoConvertFloatToString();
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x00210658 File Offset: 0x0020E858
		private void DoConvertFloatToString()
		{
			if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
			{
				this.stringVariable.Value = this.floatVariable.Value.ToString();
				return;
			}
			this.stringVariable.Value = this.floatVariable.Value.ToString(this.format.Value);
		}

		// Token: 0x04006898 RID: 26776
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to convert.")]
		public FsmFloat floatVariable;

		// Token: 0x04006899 RID: 26777
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("A string variable to store the converted value.")]
		public FsmString stringVariable;

		// Token: 0x0400689A RID: 26778
		[Tooltip("Optional Format, allows for leading zeros. E.g., 0000")]
		public FsmString format;

		// Token: 0x0400689B RID: 26779
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
