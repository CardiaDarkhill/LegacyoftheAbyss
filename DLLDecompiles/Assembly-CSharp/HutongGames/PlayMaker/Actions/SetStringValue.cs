using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C8 RID: 4296
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Sets the value of a String Variable.")]
	public class SetStringValue : FsmStateAction
	{
		// Token: 0x0600746E RID: 29806 RVA: 0x0023A60A File Offset: 0x0023880A
		public override void Reset()
		{
			this.stringVariable = null;
			this.stringValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600746F RID: 29807 RVA: 0x0023A621 File Offset: 0x00238821
		public override void OnEnter()
		{
			this.DoSetStringValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007470 RID: 29808 RVA: 0x0023A637 File Offset: 0x00238837
		public override void OnUpdate()
		{
			this.DoSetStringValue();
		}

		// Token: 0x06007471 RID: 29809 RVA: 0x0023A63F File Offset: 0x0023883F
		private void DoSetStringValue()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.stringValue == null)
			{
				return;
			}
			this.stringVariable.Value = this.stringValue.Value;
		}

		// Token: 0x040074AC RID: 29868
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to set.")]
		public FsmString stringVariable;

		// Token: 0x040074AD RID: 29869
		[UIHint(UIHint.TextArea)]
		[Tooltip("The value to set the variable to.")]
		public FsmString stringValue;

		// Token: 0x040074AE RID: 29870
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
