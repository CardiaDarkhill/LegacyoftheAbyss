using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CD RID: 4301
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Splits a string into substrings using separator characters.")]
	public class StringSplit : FsmStateAction
	{
		// Token: 0x06007482 RID: 29826 RVA: 0x0023A85C File Offset: 0x00238A5C
		public override void Reset()
		{
			this.stringToSplit = null;
			this.separators = null;
			this.trimStrings = false;
			this.trimChars = null;
			this.stringArray = null;
		}

		// Token: 0x06007483 RID: 29827 RVA: 0x0023A888 File Offset: 0x00238A88
		public override void OnEnter()
		{
			char[] array = this.trimChars.Value.ToCharArray();
			if (!this.stringToSplit.IsNone && !this.stringArray.IsNone)
			{
				FsmArray fsmArray = this.stringArray;
				object[] values = this.stringToSplit.Value.Split(this.separators.Value.ToCharArray());
				fsmArray.Values = values;
				if (this.trimStrings.Value)
				{
					for (int i = 0; i < this.stringArray.Values.Length; i++)
					{
						string text = this.stringArray.Values[i] as string;
						if (text != null)
						{
							if (!this.trimChars.IsNone && array.Length != 0)
							{
								this.stringArray.Set(i, text.Trim(array));
							}
							else
							{
								this.stringArray.Set(i, text.Trim());
							}
						}
					}
				}
				this.stringArray.SaveChanges();
			}
			base.Finish();
		}

		// Token: 0x040074BE RID: 29886
		[UIHint(UIHint.Variable)]
		[Tooltip("String to split.")]
		public FsmString stringToSplit;

		// Token: 0x040074BF RID: 29887
		[Tooltip("Characters used to split the string.\nUse '\\n' for newline\nUse '\\t' for tab")]
		public FsmString separators;

		// Token: 0x040074C0 RID: 29888
		[Tooltip("Remove all leading and trailing white-space characters from each separated string.")]
		public FsmBool trimStrings;

		// Token: 0x040074C1 RID: 29889
		[Tooltip("Optional characters used to trim each separated string.")]
		public FsmString trimChars;

		// Token: 0x040074C2 RID: 29890
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		[Tooltip("Store the split strings in a String Array.")]
		public FsmArray stringArray;
	}
}
