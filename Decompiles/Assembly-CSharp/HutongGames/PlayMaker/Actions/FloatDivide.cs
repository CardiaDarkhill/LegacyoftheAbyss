using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F76 RID: 3958
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Divides one Float by another.")]
	public class FloatDivide : FsmStateAction
	{
		// Token: 0x06006DB0 RID: 28080 RVA: 0x0022114E File Offset: 0x0021F34E
		public override void Reset()
		{
			this.floatVariable = null;
			this.divideBy = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DB1 RID: 28081 RVA: 0x00221165 File Offset: 0x0021F365
		public override void OnEnter()
		{
			this.floatVariable.Value /= this.divideBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DB2 RID: 28082 RVA: 0x00221192 File Offset: 0x0021F392
		public override void OnUpdate()
		{
			this.floatVariable.Value /= this.divideBy.Value;
		}

		// Token: 0x04006D69 RID: 28009
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to divide.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D6A RID: 28010
		[RequiredField]
		[Tooltip("Divide the float variable by this value.")]
		public FsmFloat divideBy;

		// Token: 0x04006D6B RID: 28011
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;
	}
}
