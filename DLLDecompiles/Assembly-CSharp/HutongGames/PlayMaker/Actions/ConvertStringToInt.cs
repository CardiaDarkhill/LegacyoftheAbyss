using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E72 RID: 3698
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts an String value to an Int value.")]
	public class ConvertStringToInt : FsmStateAction
	{
		// Token: 0x0600696C RID: 26988 RVA: 0x0021098B File Offset: 0x0020EB8B
		public override void Reset()
		{
			this.intVariable = null;
			this.stringVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x0600696D RID: 26989 RVA: 0x002109A2 File Offset: 0x0020EBA2
		public override void OnEnter()
		{
			this.DoConvertStringToInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600696E RID: 26990 RVA: 0x002109B8 File Offset: 0x0020EBB8
		public override void OnUpdate()
		{
			this.DoConvertStringToInt();
		}

		// Token: 0x0600696F RID: 26991 RVA: 0x002109C0 File Offset: 0x0020EBC0
		private void DoConvertStringToInt()
		{
			this.intVariable.Value = int.Parse(this.stringVariable.Value);
		}

		// Token: 0x040068AA RID: 26794
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String variable to convert to an integer.")]
		public FsmString stringVariable;

		// Token: 0x040068AB RID: 26795
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Int variable.")]
		public FsmInt intVariable;

		// Token: 0x040068AC RID: 26796
		[Tooltip("Repeat every frame. Useful if the String variable is changing.")]
		public bool everyFrame;
	}
}
