using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C1 RID: 4289
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Replaces each format item in a specified string with the text equivalent of variable's value. Stores the result in a string variable.\nSee C# <a href=\"http://msdn.microsoft.com/en-us/library/system.string.format(v=vs.90).aspx\" rel=\"nofollow\">string.Format documentation</a> for usage.")]
	public class FormatString : FsmStateAction
	{
		// Token: 0x0600744D RID: 29773 RVA: 0x0023A15D File Offset: 0x0023835D
		public override void Reset()
		{
			this.format = null;
			this.variables = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600744E RID: 29774 RVA: 0x0023A17B File Offset: 0x0023837B
		public override void OnEnter()
		{
			this.objectArray = new object[this.variables.Length];
			this.DoFormatString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600744F RID: 29775 RVA: 0x0023A1A4 File Offset: 0x002383A4
		public override void OnUpdate()
		{
			this.DoFormatString();
		}

		// Token: 0x06007450 RID: 29776 RVA: 0x0023A1AC File Offset: 0x002383AC
		private void DoFormatString()
		{
			for (int i = 0; i < this.variables.Length; i++)
			{
				this.variables[i].UpdateValue();
				this.objectArray[i] = this.variables[i].GetValue();
			}
			try
			{
				this.storeResult.Value = string.Format(this.format.Value, this.objectArray);
			}
			catch (FormatException ex)
			{
				base.LogError(ex.Message);
				base.Finish();
			}
		}

		// Token: 0x04007490 RID: 29840
		[RequiredField]
		[Tooltip("E.g. Hello {0} and {1}\nWith 2 variables that replace {0} and {1}\nSee C# string.Format docs.")]
		public FsmString format;

		// Token: 0x04007491 RID: 29841
		[Tooltip("Variables to use for each formatting item.")]
		public FsmVar[] variables;

		// Token: 0x04007492 RID: 29842
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the formatted result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x04007493 RID: 29843
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x04007494 RID: 29844
		private object[] objectArray;
	}
}
