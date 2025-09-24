using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2B RID: 3371
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable to the lowest of two values.")]
	public class SetFloatToLowest : FsmStateAction
	{
		// Token: 0x06006346 RID: 25414 RVA: 0x001F5CDE File Offset: 0x001F3EDE
		public override void Reset()
		{
			this.floatVariable = null;
			this.value1 = null;
			this.value2 = null;
			this.everyFrame = false;
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x001F5CFC File Offset: 0x001F3EFC
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

		// Token: 0x06006348 RID: 25416 RVA: 0x001F5D60 File Offset: 0x001F3F60
		public override void OnUpdate()
		{
			if (this.value1.Value < this.value2.Value)
			{
				this.floatVariable.Value = this.value1.Value;
				return;
			}
			this.floatVariable.Value = this.value2.Value;
		}

		// Token: 0x040061AD RID: 25005
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		// Token: 0x040061AE RID: 25006
		[RequiredField]
		public FsmFloat value1;

		// Token: 0x040061AF RID: 25007
		[RequiredField]
		public FsmFloat value2;

		// Token: 0x040061B0 RID: 25008
		public bool everyFrame;
	}
}
