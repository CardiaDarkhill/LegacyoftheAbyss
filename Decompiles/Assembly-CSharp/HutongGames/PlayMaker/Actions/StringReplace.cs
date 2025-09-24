using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CC RID: 4300
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Replace a substring with a new String.")]
	public class StringReplace : FsmStateAction
	{
		// Token: 0x0600747D RID: 29821 RVA: 0x0023A7AF File Offset: 0x002389AF
		public override void Reset()
		{
			this.stringVariable = null;
			this.replace = "";
			this.with = "";
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600747E RID: 29822 RVA: 0x0023A7E6 File Offset: 0x002389E6
		public override void OnEnter()
		{
			this.DoReplace();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600747F RID: 29823 RVA: 0x0023A7FC File Offset: 0x002389FC
		public override void OnUpdate()
		{
			this.DoReplace();
		}

		// Token: 0x06007480 RID: 29824 RVA: 0x0023A804 File Offset: 0x00238A04
		private void DoReplace()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Replace(this.replace.Value, this.with.Value);
		}

		// Token: 0x040074B9 RID: 29881
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String Variable to examine.")]
		public FsmString stringVariable;

		// Token: 0x040074BA RID: 29882
		[Tooltip("Replace this string...")]
		public FsmString replace;

		// Token: 0x040074BB RID: 29883
		[Tooltip("... with this string.")]
		public FsmString with;

		// Token: 0x040074BC RID: 29884
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x040074BD RID: 29885
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
