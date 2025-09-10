using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C0 RID: 4288
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Builds a String from other Strings.")]
	public class BuildString : FsmStateAction
	{
		// Token: 0x06007447 RID: 29767 RVA: 0x00239F4D File Offset: 0x0023814D
		public override void Reset()
		{
			this.stringParts = new FsmString[3];
			this.separator = null;
			this.addToEnd = true;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007448 RID: 29768 RVA: 0x00239F7C File Offset: 0x0023817C
		public override void OnEnter()
		{
			this.DoBuildString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007449 RID: 29769 RVA: 0x00239F92 File Offset: 0x00238192
		public override void OnUpdate()
		{
			this.DoBuildString();
		}

		// Token: 0x0600744A RID: 29770 RVA: 0x00239F9C File Offset: 0x0023819C
		private void DoBuildString()
		{
			if (this.storeResult == null)
			{
				return;
			}
			string value = this.separator.Value;
			bool flag = !string.IsNullOrEmpty(value);
			int num = 0;
			num += this.stringParts.Length;
			if (flag)
			{
				if (num >= 2)
				{
					num++;
				}
				if (this.addToEnd.Value)
				{
					num++;
				}
			}
			if (num >= 3)
			{
				BuildString.stringBuilder.Clear();
				for (int i = 0; i < this.stringParts.Length - 1; i++)
				{
					BuildString.stringBuilder.Append(this.stringParts[i]);
					if (flag)
					{
						BuildString.stringBuilder.Append(value);
					}
				}
				StringBuilder stringBuilder = BuildString.stringBuilder;
				FsmString[] array = this.stringParts;
				stringBuilder.Append(array[array.Length - 1]);
				if (flag && this.addToEnd.Value)
				{
					BuildString.stringBuilder.Append(value);
				}
				this.storeResult.Value = BuildString.stringBuilder.ToString();
				BuildString.stringBuilder.Clear();
				return;
			}
			this.result = string.Empty;
			for (int j = 0; j < this.stringParts.Length - 1; j++)
			{
				string str = this.result;
				FsmString fsmString = this.stringParts[j];
				this.result = str + ((fsmString != null) ? fsmString.ToString() : null);
				if (flag)
				{
					this.result += value;
				}
			}
			string str2 = this.result;
			FsmString[] array2 = this.stringParts;
			FsmString fsmString2 = array2[array2.Length - 1];
			this.result = str2 + ((fsmString2 != null) ? fsmString2.ToString() : null);
			if (flag && this.addToEnd.Value)
			{
				this.result += value;
			}
			this.storeResult.Value = this.result;
		}

		// Token: 0x04007489 RID: 29833
		[RequiredField]
		[Tooltip("Array of Strings to combine.")]
		public FsmString[] stringParts;

		// Token: 0x0400748A RID: 29834
		[Tooltip("Separator to insert between each String. E.g. space character.")]
		public FsmString separator;

		// Token: 0x0400748B RID: 29835
		[Tooltip("Add Separator to end of built string.")]
		public FsmBool addToEnd;

		// Token: 0x0400748C RID: 29836
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the final String in a variable.")]
		public FsmString storeResult;

		// Token: 0x0400748D RID: 29837
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x0400748E RID: 29838
		private string result;

		// Token: 0x0400748F RID: 29839
		private static StringBuilder stringBuilder = new StringBuilder();
	}
}
