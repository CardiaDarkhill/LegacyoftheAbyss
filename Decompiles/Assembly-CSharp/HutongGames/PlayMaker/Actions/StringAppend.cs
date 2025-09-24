using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CA RID: 4298
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Adds a String to the end of a String.")]
	public class StringAppend : FsmStateAction
	{
		// Token: 0x06007478 RID: 29816 RVA: 0x0023A710 File Offset: 0x00238910
		public override void Reset()
		{
			this.stringVariable = null;
			this.appendString = null;
		}

		// Token: 0x06007479 RID: 29817 RVA: 0x0023A720 File Offset: 0x00238920
		public override void OnEnter()
		{
			FsmString fsmString = this.stringVariable;
			fsmString.Value += this.appendString.Value;
			base.Finish();
		}

		// Token: 0x040074B4 RID: 29876
		[RequiredField]
		[Tooltip("Strings to add to.")]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		// Token: 0x040074B5 RID: 29877
		[Tooltip("String to append")]
		public FsmString appendString;
	}
}
