using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2C RID: 3372
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable to the smallest of two values.")]
	public class SetFloatToSmallest : FsmStateAction
	{
		// Token: 0x0600634A RID: 25418 RVA: 0x001F5DBA File Offset: 0x001F3FBA
		public override void Reset()
		{
			this.floatVariable = null;
			this.value1 = null;
			this.value2 = null;
			this.everyFrame = false;
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x001F5DD8 File Offset: 0x001F3FD8
		public override void OnEnter()
		{
			if (this.value1.Value < this.value2.Value)
			{
				this.floatVariable.Value = this.value1.Value;
			}
			else
			{
				this.floatVariable.Value = this.value2.Value;
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600634C RID: 25420 RVA: 0x001F5E3C File Offset: 0x001F403C
		public override void OnUpdate()
		{
			if (this.value1.Value < this.value2.Value)
			{
				this.floatVariable.Value = this.value1.Value;
				return;
			}
			this.floatVariable.Value = this.value2.Value;
		}

		// Token: 0x040061B1 RID: 25009
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		// Token: 0x040061B2 RID: 25010
		[RequiredField]
		public FsmFloat value1;

		// Token: 0x040061B3 RID: 25011
		[RequiredField]
		public FsmFloat value2;

		// Token: 0x040061B4 RID: 25012
		public bool everyFrame;
	}
}
