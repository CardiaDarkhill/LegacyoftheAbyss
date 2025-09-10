using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F83 RID: 3971
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Subtracts a value to an Integer Variable.")]
	public class IntSubtract : FsmStateAction
	{
		// Token: 0x06006DEB RID: 28139 RVA: 0x00221A68 File Offset: 0x0021FC68
		public override void Reset()
		{
			this.intVariable = null;
			this.subtract = null;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x06006DEC RID: 28140 RVA: 0x00221A86 File Offset: 0x0021FC86
		public override void OnEnter()
		{
			this.doSubtract();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DED RID: 28141 RVA: 0x00221A9C File Offset: 0x0021FC9C
		public override void OnUpdate()
		{
			this.doSubtract();
		}

		// Token: 0x06006DEE RID: 28142 RVA: 0x00221AA4 File Offset: 0x0021FCA4
		private void doSubtract()
		{
			if (this.perSecond)
			{
				int num = Mathf.Abs(this.subtract.Value);
				this._acc += (float)num * Time.deltaTime;
				if (this._acc >= (float)num)
				{
					this._acc = 0f;
					this.intVariable.Value -= this.subtract.Value;
					return;
				}
			}
			else
			{
				this.intVariable.Value -= this.subtract.Value;
			}
		}

		// Token: 0x04006D9E RID: 28062
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int variable to subtract from.")]
		public FsmInt intVariable;

		// Token: 0x04006D9F RID: 28063
		[RequiredField]
		[Tooltip("Value to subtract from the int variable.")]
		public FsmInt subtract;

		// Token: 0x04006DA0 RID: 28064
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006DA1 RID: 28065
		[Tooltip("Used with Every Frame. Subtracts the value over one second to make the operation frame rate independent.")]
		public bool perSecond;

		// Token: 0x04006DA2 RID: 28066
		private float _acc;
	}
}
