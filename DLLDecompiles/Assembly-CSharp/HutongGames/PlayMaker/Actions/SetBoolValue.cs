using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8B RID: 3979
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Bool Variable.")]
	public class SetBoolValue : FsmStateAction
	{
		// Token: 0x06006E0C RID: 28172 RVA: 0x00222001 File Offset: 0x00220201
		public override void Reset()
		{
			this.boolVariable = null;
			this.boolValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E0D RID: 28173 RVA: 0x00222018 File Offset: 0x00220218
		public override void OnEnter()
		{
			this.boolVariable.Value = this.boolValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E0E RID: 28174 RVA: 0x0022203E File Offset: 0x0022023E
		public override void OnUpdate()
		{
			this.boolVariable.Value = this.boolValue.Value;
		}

		// Token: 0x04006DBC RID: 28092
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Bool Variable to set.")]
		public FsmBool boolVariable;

		// Token: 0x04006DBD RID: 28093
		[RequiredField]
		[Tooltip("Value to set it to: Check to set to True, Uncheck to set to False.")]
		public FsmBool boolValue;

		// Token: 0x04006DBE RID: 28094
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
