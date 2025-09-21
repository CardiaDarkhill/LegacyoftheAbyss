using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C96 RID: 3222
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class KeepFloatPositive : FsmStateAction
	{
		// Token: 0x060060C4 RID: 24772 RVA: 0x001EAB25 File Offset: 0x001E8D25
		public override void Reset()
		{
			this.floatVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x060060C5 RID: 24773 RVA: 0x001EAB35 File Offset: 0x001E8D35
		public override void OnEnter()
		{
			this.KeepPositive();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060C6 RID: 24774 RVA: 0x001EAB4B File Offset: 0x001E8D4B
		public override void OnUpdate()
		{
			this.KeepPositive();
		}

		// Token: 0x060060C7 RID: 24775 RVA: 0x001EAB53 File Offset: 0x001E8D53
		private void KeepPositive()
		{
			if (this.floatVariable.Value < 0f)
			{
				this.floatVariable.Value *= -1f;
			}
		}

		// Token: 0x04005E52 RID: 24146
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		// Token: 0x04005E53 RID: 24147
		public bool everyFrame;
	}
}
