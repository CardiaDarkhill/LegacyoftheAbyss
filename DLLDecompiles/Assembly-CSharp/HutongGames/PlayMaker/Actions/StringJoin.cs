using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CB RID: 4299
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Join an array of strings into a single string.")]
	public class StringJoin : FsmStateAction
	{
		// Token: 0x0600747B RID: 29819 RVA: 0x0023A754 File Offset: 0x00238954
		public override void OnEnter()
		{
			if (!this.stringArray.IsNone && !this.storeResult.IsNone)
			{
				this.storeResult.Value = string.Join(this.separator.Value, this.stringArray.stringValues);
			}
			base.Finish();
		}

		// Token: 0x040074B6 RID: 29878
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		[Tooltip("Array of string to join into a single string.")]
		public FsmArray stringArray;

		// Token: 0x040074B7 RID: 29879
		[Tooltip("Separator to add between each string.")]
		public FsmString separator;

		// Token: 0x040074B8 RID: 29880
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the joined string in string variable.")]
		public FsmString storeResult;
	}
}
