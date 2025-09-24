using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8D RID: 3981
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an integer variable using a float value.")]
	public class SetIntFromFloat : FsmStateAction
	{
		// Token: 0x06006E14 RID: 28180 RVA: 0x002220BB File Offset: 0x002202BB
		public override void Reset()
		{
			this.intVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E15 RID: 28181 RVA: 0x002220D2 File Offset: 0x002202D2
		public override void OnEnter()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E16 RID: 28182 RVA: 0x002220F9 File Offset: 0x002202F9
		public override void OnUpdate()
		{
			this.intVariable.Value = (int)this.floatValue.Value;
		}

		// Token: 0x04006DC2 RID: 28098
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int variable to set.")]
		public FsmInt intVariable;

		// Token: 0x04006DC3 RID: 28099
		[Tooltip("The float value.")]
		public FsmFloat floatValue;

		// Token: 0x04006DC4 RID: 28100
		[Tooltip("Do it every frame.")]
		public bool everyFrame;
	}
}
