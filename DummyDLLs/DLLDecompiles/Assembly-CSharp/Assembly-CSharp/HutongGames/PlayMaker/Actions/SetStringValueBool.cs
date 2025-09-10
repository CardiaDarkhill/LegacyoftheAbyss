using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C9 RID: 4297
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValueBool : FsmStateAction
	{
		// Token: 0x06007473 RID: 29811 RVA: 0x0023A671 File Offset: 0x00238871
		public override void Reset()
		{
			this.stringVariable = null;
			this.trueValue = null;
			this.falseValue = null;
		}

		// Token: 0x06007474 RID: 29812 RVA: 0x0023A688 File Offset: 0x00238888
		public override void OnEnter()
		{
			this.DoSetStringValue();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06007475 RID: 29813 RVA: 0x0023A69E File Offset: 0x0023889E
		public override void OnUpdate()
		{
			this.DoSetStringValue();
		}

		// Token: 0x06007476 RID: 29814 RVA: 0x0023A6A8 File Offset: 0x002388A8
		private void DoSetStringValue()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.testBool.Value)
			{
				if (this.trueValue != null)
				{
					this.stringVariable.Value = this.trueValue.Value;
					return;
				}
			}
			else if (this.falseValue != null)
			{
				this.stringVariable.Value = this.falseValue.Value;
			}
		}

		// Token: 0x040074AF RID: 29871
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		// Token: 0x040074B0 RID: 29872
		[UIHint(UIHint.Variable)]
		public FsmBool testBool;

		// Token: 0x040074B1 RID: 29873
		public FsmString trueValue;

		// Token: 0x040074B2 RID: 29874
		public FsmString falseValue;

		// Token: 0x040074B3 RID: 29875
		public bool everyframe;
	}
}
