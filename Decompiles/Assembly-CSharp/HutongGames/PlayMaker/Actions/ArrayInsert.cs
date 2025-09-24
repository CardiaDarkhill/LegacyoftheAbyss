using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E28 RID: 3624
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Insert an item into an Array at the specified index.")]
	public class ArrayInsert : FsmStateAction
	{
		// Token: 0x06006814 RID: 26644 RVA: 0x0020BDD9 File Offset: 0x00209FD9
		public override void Reset()
		{
			this.array = null;
			this.value = null;
			this.atIndex = null;
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x0020BDF0 File Offset: 0x00209FF0
		public override void OnEnter()
		{
			this.DoInsertValue();
			base.Finish();
		}

		// Token: 0x06006816 RID: 26646 RVA: 0x0020BDFE File Offset: 0x00209FFE
		private void DoInsertValue()
		{
			this.value.UpdateValue();
			this.array.InsertItem(this.value.GetValue(), this.atIndex.Value);
		}

		// Token: 0x04006746 RID: 26438
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006747 RID: 26439
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("Item to add.")]
		public FsmVar value;

		// Token: 0x04006748 RID: 26440
		[Tooltip("The index to insert at.\n0 = first, 1 = second...")]
		public FsmInt atIndex;
	}
}
