using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB6 RID: 2998
	[ActionCategory(ActionCategory.Array)]
	public class ArrayAddRangeArray : FsmStateAction
	{
		// Token: 0x06005C5F RID: 23647 RVA: 0x001D16EC File Offset: 0x001CF8EC
		public override string ErrorCheck()
		{
			if (this.arrayTo.TypeConstraint == this.arrayFrom.TypeConstraint)
			{
				return base.ErrorCheck();
			}
			return "Array types do not match";
		}

		// Token: 0x06005C60 RID: 23648 RVA: 0x001D1712 File Offset: 0x001CF912
		public override void Reset()
		{
			this.arrayTo = null;
			this.arrayFrom = null;
		}

		// Token: 0x06005C61 RID: 23649 RVA: 0x001D1722 File Offset: 0x001CF922
		public override void OnEnter()
		{
			this.DoAddRange();
			base.Finish();
		}

		// Token: 0x06005C62 RID: 23650 RVA: 0x001D1730 File Offset: 0x001CF930
		private void DoAddRange()
		{
			int num = this.arrayFrom.Length;
			if (num <= 0)
			{
				return;
			}
			foreach (object value in this.arrayFrom.Values)
			{
				this.arrayTo.Set(this.arrayTo.Length - num, value);
				num--;
			}
		}

		// Token: 0x040057D4 RID: 22484
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray arrayTo;

		// Token: 0x040057D5 RID: 22485
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray arrayFrom;
	}
}
