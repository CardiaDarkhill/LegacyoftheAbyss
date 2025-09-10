using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7B RID: 3963
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Subtracts a value from a Float Variable.")]
	public class FloatSubtract : FsmStateAction
	{
		// Token: 0x06006DC6 RID: 28102 RVA: 0x0022153F File Offset: 0x0021F73F
		public override void Reset()
		{
			this.floatVariable = null;
			this.subtract = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x06006DC7 RID: 28103 RVA: 0x0022155D File Offset: 0x0021F75D
		public override void OnEnter()
		{
			this.DoFloatSubtract();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DC8 RID: 28104 RVA: 0x00221573 File Offset: 0x0021F773
		public override void OnUpdate()
		{
			this.DoFloatSubtract();
		}

		// Token: 0x06006DC9 RID: 28105 RVA: 0x0022157C File Offset: 0x0021F77C
		private void DoFloatSubtract()
		{
			if (!this.perSecond)
			{
				this.floatVariable.Value -= this.subtract.Value;
				return;
			}
			this.floatVariable.Value -= this.subtract.Value * Time.deltaTime;
		}

		// Token: 0x04006D81 RID: 28033
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to subtract from.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D82 RID: 28034
		[RequiredField]
		[Tooltip("Value to subtract from the float variable.")]
		public FsmFloat subtract;

		// Token: 0x04006D83 RID: 28035
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006D84 RID: 28036
		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;
	}
}
