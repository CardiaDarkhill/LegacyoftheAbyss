using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2A RID: 3370
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable to the highest of two values.")]
	public class SetFloatToHighest : FsmStateAction
	{
		// Token: 0x06006342 RID: 25410 RVA: 0x001F5C01 File Offset: 0x001F3E01
		public override void Reset()
		{
			this.floatVariable = null;
			this.value1 = null;
			this.value2 = null;
			this.everyFrame = false;
		}

		// Token: 0x06006343 RID: 25411 RVA: 0x001F5C20 File Offset: 0x001F3E20
		public override void OnEnter()
		{
			if (this.value1.Value > this.value2.Value)
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

		// Token: 0x06006344 RID: 25412 RVA: 0x001F5C84 File Offset: 0x001F3E84
		public override void OnUpdate()
		{
			if (this.value1.Value > this.value2.Value)
			{
				this.floatVariable.Value = this.value1.Value;
				return;
			}
			this.floatVariable.Value = this.value2.Value;
		}

		// Token: 0x040061A9 RID: 25001
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		// Token: 0x040061AA RID: 25002
		[RequiredField]
		public FsmFloat value1;

		// Token: 0x040061AB RID: 25003
		[RequiredField]
		public FsmFloat value2;

		// Token: 0x040061AC RID: 25004
		public bool everyFrame;
	}
}
