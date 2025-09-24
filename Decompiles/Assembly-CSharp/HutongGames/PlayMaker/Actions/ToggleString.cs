using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D97 RID: 3479
	[ActionCategory(ActionCategory.String)]
	public class ToggleString : FsmStateAction
	{
		// Token: 0x06006521 RID: 25889 RVA: 0x001FE525 File Offset: 0x001FC725
		public override void Reset()
		{
			this.stringVariable = null;
			this.stringValue1 = null;
			this.stringValue2 = null;
		}

		// Token: 0x06006522 RID: 25890 RVA: 0x001FE53C File Offset: 0x001FC73C
		public override void OnEnter()
		{
			if (this.stringVariable.Value == this.stringValue1.Value)
			{
				this.stringVariable.Value = this.stringValue2.Value;
			}
			else if (this.stringVariable.Value == this.stringValue2.Value)
			{
				this.stringVariable.Value = this.stringValue1.Value;
			}
			base.Finish();
		}

		// Token: 0x0400641E RID: 25630
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		// Token: 0x0400641F RID: 25631
		public FsmString stringValue1;

		// Token: 0x04006420 RID: 25632
		public FsmString stringValue2;
	}
}
