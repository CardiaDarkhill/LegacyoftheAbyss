using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1E RID: 3614
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Add an item to the end of an Array.")]
	public class ArrayAdd : FsmStateAction
	{
		// Token: 0x060067E3 RID: 26595 RVA: 0x0020B3EA File Offset: 0x002095EA
		public override void Reset()
		{
			this.array = null;
			this.value = null;
		}

		// Token: 0x060067E4 RID: 26596 RVA: 0x0020B3FA File Offset: 0x002095FA
		public override void OnEnter()
		{
			this.DoAddValue();
			base.Finish();
		}

		// Token: 0x060067E5 RID: 26597 RVA: 0x0020B408 File Offset: 0x00209608
		private void DoAddValue()
		{
			this.array.Resize(this.array.Length + 1);
			this.value.UpdateValue();
			this.array.Set(this.array.Length - 1, this.value.GetValue());
		}

		// Token: 0x04006717 RID: 26391
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006718 RID: 26392
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("Item to add.")]
		public FsmVar value;
	}
}
