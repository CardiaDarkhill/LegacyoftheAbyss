using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F74 RID: 3956
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Clamps the value of Float Variable to a Min/Max range.")]
	public class FloatClamp : FsmStateAction
	{
		// Token: 0x06006DA6 RID: 28070 RVA: 0x0022106B File Offset: 0x0021F26B
		public override void Reset()
		{
			this.floatVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DA7 RID: 28071 RVA: 0x00221089 File Offset: 0x0021F289
		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DA8 RID: 28072 RVA: 0x0022109F File Offset: 0x0021F29F
		public override void OnUpdate()
		{
			this.DoClamp();
		}

		// Token: 0x06006DA9 RID: 28073 RVA: 0x002210A7 File Offset: 0x0021F2A7
		private void DoClamp()
		{
			this.floatVariable.Value = Mathf.Clamp(this.floatVariable.Value, this.minValue.Value, this.maxValue.Value);
		}

		// Token: 0x04006D61 RID: 28001
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D62 RID: 28002
		[RequiredField]
		[Tooltip("The minimum value allowed.")]
		public FsmFloat minValue;

		// Token: 0x04006D63 RID: 28003
		[RequiredField]
		[Tooltip("The maximum value allowed.")]
		public FsmFloat maxValue;

		// Token: 0x04006D64 RID: 28004
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
