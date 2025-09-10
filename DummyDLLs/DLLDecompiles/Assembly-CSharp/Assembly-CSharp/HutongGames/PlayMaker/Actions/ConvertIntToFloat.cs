using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E6E RID: 3694
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts an Integer value to a Float value.")]
	public class ConvertIntToFloat : FsmStateAction
	{
		// Token: 0x06006958 RID: 26968 RVA: 0x002106D4 File Offset: 0x0020E8D4
		public override void Reset()
		{
			this.intVariable = null;
			this.floatVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006959 RID: 26969 RVA: 0x002106EB File Offset: 0x0020E8EB
		public override void OnEnter()
		{
			this.DoConvertIntToFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600695A RID: 26970 RVA: 0x00210701 File Offset: 0x0020E901
		public override void OnUpdate()
		{
			this.DoConvertIntToFloat();
		}

		// Token: 0x0600695B RID: 26971 RVA: 0x00210709 File Offset: 0x0020E909
		private void DoConvertIntToFloat()
		{
			this.floatVariable.Value = (float)this.intVariable.Value;
		}

		// Token: 0x0400689C RID: 26780
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Integer variable to convert to a float.")]
		public FsmInt intVariable;

		// Token: 0x0400689D RID: 26781
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Float variable.")]
		public FsmFloat floatVariable;

		// Token: 0x0400689E RID: 26782
		[Tooltip("Repeat every frame. Useful if the Integer variable is changing.")]
		public bool everyFrame;
	}
}
