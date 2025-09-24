using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C7 RID: 4295
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an Int Variable.")]
	public class SetIntValueBool : FsmStateAction
	{
		// Token: 0x0600746A RID: 29802 RVA: 0x0023A571 File Offset: 0x00238771
		public override void Reset()
		{
			this.intVariable = null;
			this.trueValue = null;
			this.falseValue = null;
		}

		// Token: 0x0600746B RID: 29803 RVA: 0x0023A588 File Offset: 0x00238788
		public override void OnEnter()
		{
			this.DoSetStringValue();
			base.Finish();
		}

		// Token: 0x0600746C RID: 29804 RVA: 0x0023A598 File Offset: 0x00238798
		private void DoSetStringValue()
		{
			if (this.intVariable == null)
			{
				return;
			}
			if (this.testBool.Value)
			{
				if (!this.trueValue.IsNone)
				{
					this.intVariable.Value = this.trueValue.Value;
					return;
				}
			}
			else if (!this.falseValue.IsNone)
			{
				this.intVariable.Value = this.falseValue.Value;
			}
		}

		// Token: 0x040074A8 RID: 29864
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		// Token: 0x040074A9 RID: 29865
		[UIHint(UIHint.Variable)]
		public FsmBool testBool;

		// Token: 0x040074AA RID: 29866
		public FsmInt trueValue;

		// Token: 0x040074AB RID: 29867
		public FsmInt falseValue;
	}
}
