using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F71 RID: 3953
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Float variable to its absolute value.")]
	public class FloatAbs : FsmStateAction
	{
		// Token: 0x06006D97 RID: 28055 RVA: 0x00220F00 File Offset: 0x0021F100
		public override void Reset()
		{
			this.floatVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D98 RID: 28056 RVA: 0x00220F10 File Offset: 0x0021F110
		public override void OnEnter()
		{
			this.DoFloatAbs();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D99 RID: 28057 RVA: 0x00220F26 File Offset: 0x0021F126
		public override void OnUpdate()
		{
			this.DoFloatAbs();
		}

		// Token: 0x06006D9A RID: 28058 RVA: 0x00220F2E File Offset: 0x0021F12E
		private void DoFloatAbs()
		{
			this.floatVariable.Value = Mathf.Abs(this.floatVariable.Value);
		}

		// Token: 0x04006D58 RID: 27992
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D59 RID: 27993
		[Tooltip("Repeat every frame. Useful if the Float variable is changing.")]
		public bool everyFrame;
	}
}
