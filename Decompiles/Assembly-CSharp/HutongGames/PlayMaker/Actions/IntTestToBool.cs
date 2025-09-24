using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C93 RID: 3219
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Set bools based on the comparison of 2 ints.")]
	public class IntTestToBool : FsmStateAction
	{
		// Token: 0x060060B5 RID: 24757 RVA: 0x001EA5FC File Offset: 0x001E87FC
		public override void Reset()
		{
			this.int1 = 0;
			this.int2 = 0;
			this.everyFrame = false;
		}

		// Token: 0x060060B6 RID: 24758 RVA: 0x001EA61D File Offset: 0x001E881D
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060B7 RID: 24759 RVA: 0x001EA633 File Offset: 0x001E8833
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x060060B8 RID: 24760 RVA: 0x001EA63C File Offset: 0x001E883C
		private void DoCompare()
		{
			if (this.int1.Value == this.int2.Value)
			{
				this.equalBool.Value = true;
			}
			else
			{
				this.equalBool.Value = false;
			}
			if (this.int1.Value < this.int2.Value)
			{
				this.lessThanBool.Value = true;
			}
			else
			{
				this.lessThanBool.Value = false;
			}
			if (this.int1.Value > this.int2.Value)
			{
				this.greaterThanBool.Value = true;
				return;
			}
			this.greaterThanBool.Value = false;
		}

		// Token: 0x04005E39 RID: 24121
		[RequiredField]
		[Tooltip("The first int variable.")]
		public FsmInt int1;

		// Token: 0x04005E3A RID: 24122
		[RequiredField]
		[Tooltip("The second int variable.")]
		public FsmInt int2;

		// Token: 0x04005E3B RID: 24123
		[Tooltip("Bool set if Int 1 equals Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		// Token: 0x04005E3C RID: 24124
		[Tooltip("Bool set if Int 1 is less than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		// Token: 0x04005E3D RID: 24125
		[Tooltip("Bool set if Int 1 is greater than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		// Token: 0x04005E3E RID: 24126
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
