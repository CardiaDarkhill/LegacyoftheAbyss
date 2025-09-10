using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F72 RID: 3954
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAdd : FsmStateAction
	{
		// Token: 0x06006D9C RID: 28060 RVA: 0x00220F53 File Offset: 0x0021F153
		public override void Reset()
		{
			this.floatVariable = null;
			this.add = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x06006D9D RID: 28061 RVA: 0x00220F71 File Offset: 0x0021F171
		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D9E RID: 28062 RVA: 0x00220F87 File Offset: 0x0021F187
		public override void OnUpdate()
		{
			this.DoFloatAdd();
		}

		// Token: 0x06006D9F RID: 28063 RVA: 0x00220F90 File Offset: 0x0021F190
		private void DoFloatAdd()
		{
			if (!this.perSecond)
			{
				this.floatVariable.Value += this.add.Value;
				return;
			}
			this.floatVariable.Value += this.add.Value * Time.deltaTime;
		}

		// Token: 0x04006D5A RID: 27994
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to add to.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D5B RID: 27995
		[RequiredField]
		[Tooltip("Amount to add.")]
		public FsmFloat add;

		// Token: 0x04006D5C RID: 27996
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006D5D RID: 27997
		[Tooltip("Used with Every Frame. Adds the value over one second to make the operation frame rate independent.")]
		public bool perSecond;
	}
}
