using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8E RID: 3982
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an Integer Variable.")]
	public class SetIntValue : FsmStateAction
	{
		// Token: 0x06006E18 RID: 28184 RVA: 0x0022211A File Offset: 0x0022031A
		public override void Reset()
		{
			this.intVariable = null;
			this.intValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E19 RID: 28185 RVA: 0x00222131 File Offset: 0x00220331
		public override void OnEnter()
		{
			this.intVariable.Value = this.intValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E1A RID: 28186 RVA: 0x00222157 File Offset: 0x00220357
		public override void OnUpdate()
		{
			this.intVariable.Value = this.intValue.Value;
		}

		// Token: 0x04006DC5 RID: 28101
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Int Variable to Set")]
		public FsmInt intVariable;

		// Token: 0x04006DC6 RID: 28102
		[RequiredField]
		[Tooltip("Int Value")]
		public FsmInt intValue;

		// Token: 0x04006DC7 RID: 28103
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
