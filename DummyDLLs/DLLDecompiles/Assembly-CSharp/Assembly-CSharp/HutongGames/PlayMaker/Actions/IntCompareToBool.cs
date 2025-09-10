using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8F RID: 3215
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares two ints and sets a bool value depending on result")]
	public class IntCompareToBool : FsmStateAction
	{
		// Token: 0x060060A1 RID: 24737 RVA: 0x001EA2CA File Offset: 0x001E84CA
		public override void Reset()
		{
			this.integer1 = 0;
			this.integer2 = 0;
			this.equalBool = null;
			this.lessThanBool = null;
			this.greaterThanBool = null;
			this.everyFrame = false;
		}

		// Token: 0x060060A2 RID: 24738 RVA: 0x001EA300 File Offset: 0x001E8500
		public override void OnEnter()
		{
			this.DoIntCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x001EA316 File Offset: 0x001E8516
		public override void OnUpdate()
		{
			this.DoIntCompare();
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x001EA320 File Offset: 0x001E8520
		private void DoIntCompare()
		{
			if (!this.equalBool.IsNone)
			{
				if (this.integer1.Value == this.integer2.Value)
				{
					this.equalBool.Value = true;
				}
				else
				{
					this.equalBool.Value = false;
				}
			}
			if (!this.lessThanBool.IsNone)
			{
				if (this.integer1.Value < this.integer2.Value)
				{
					this.lessThanBool.Value = true;
				}
				else
				{
					this.lessThanBool.Value = false;
				}
			}
			if (!this.greaterThanBool.IsNone)
			{
				if (this.integer1.Value > this.integer2.Value)
				{
					this.greaterThanBool.Value = true;
					return;
				}
				this.greaterThanBool.Value = false;
			}
		}

		// Token: 0x04005E23 RID: 24099
		[RequiredField]
		public FsmInt integer1;

		// Token: 0x04005E24 RID: 24100
		[RequiredField]
		public FsmInt integer2;

		// Token: 0x04005E25 RID: 24101
		[Tooltip("Bool set if Int 1 equals Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		// Token: 0x04005E26 RID: 24102
		[Tooltip("Bool set if Int 1 is less than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		// Token: 0x04005E27 RID: 24103
		[Tooltip("Bool set if Int 1 is greater than Int 2")]
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		// Token: 0x04005E28 RID: 24104
		public bool everyFrame;
	}
}
