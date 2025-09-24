using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E67 RID: 3687
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Color.")]
	public class ConvertBoolToColor : FsmStateAction
	{
		// Token: 0x06006935 RID: 26933 RVA: 0x00210279 File Offset: 0x0020E479
		public override void Reset()
		{
			this.boolVariable = null;
			this.colorVariable = null;
			this.falseColor = Color.black;
			this.trueColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x002102B0 File Offset: 0x0020E4B0
		public override void OnEnter()
		{
			this.DoConvertBoolToColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006937 RID: 26935 RVA: 0x002102C6 File Offset: 0x0020E4C6
		public override void OnUpdate()
		{
			this.DoConvertBoolToColor();
		}

		// Token: 0x06006938 RID: 26936 RVA: 0x002102CE File Offset: 0x0020E4CE
		private void DoConvertBoolToColor()
		{
			this.colorVariable.Value = (this.boolVariable.Value ? this.trueColor.Value : this.falseColor.Value);
		}

		// Token: 0x0400687D RID: 26749
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to convert.")]
		public FsmBool boolVariable;

		// Token: 0x0400687E RID: 26750
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color variable to set based on the bool variable value.")]
		public FsmColor colorVariable;

		// Token: 0x0400687F RID: 26751
		[Tooltip("Color if Bool variable is false.")]
		public FsmColor falseColor;

		// Token: 0x04006880 RID: 26752
		[Tooltip("Color if Bool variable is true.")]
		public FsmColor trueColor;

		// Token: 0x04006881 RID: 26753
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool everyFrame;
	}
}
