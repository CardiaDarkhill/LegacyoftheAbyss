using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E70 RID: 3696
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Material variable to an Object variable. Useful if you want to use Set Property (which only works on Object variables).")]
	public class ConvertMaterialToObject : FsmStateAction
	{
		// Token: 0x06006962 RID: 26978 RVA: 0x002107E4 File Offset: 0x0020E9E4
		public override void Reset()
		{
			this.materialVariable = null;
			this.objectVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006963 RID: 26979 RVA: 0x002107FB File Offset: 0x0020E9FB
		public override void OnEnter()
		{
			this.DoConvertMaterialToObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006964 RID: 26980 RVA: 0x00210811 File Offset: 0x0020EA11
		public override void OnUpdate()
		{
			this.DoConvertMaterialToObject();
		}

		// Token: 0x06006965 RID: 26981 RVA: 0x00210819 File Offset: 0x0020EA19
		private void DoConvertMaterialToObject()
		{
			this.objectVariable.Value = this.materialVariable.Value;
		}

		// Token: 0x040068A3 RID: 26787
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Material variable to convert to an Object.")]
		public FsmMaterial materialVariable;

		// Token: 0x040068A4 RID: 26788
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Object variable.")]
		public FsmObject objectVariable;

		// Token: 0x040068A5 RID: 26789
		[Tooltip("Repeat every frame. Useful if the Material variable is changing.")]
		public bool everyFrame;
	}
}
