using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E2B RID: 3627
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Remove all items from an Array.")]
	public class ArrayRemoveAll : FsmStateAction
	{
		// Token: 0x06006821 RID: 26657 RVA: 0x0020BF80 File Offset: 0x0020A180
		public override void Reset()
		{
			this.array = null;
		}

		// Token: 0x06006822 RID: 26658 RVA: 0x0020BF89 File Offset: 0x0020A189
		public override void OnEnter()
		{
			this.array.Reset();
			base.Finish();
		}

		// Token: 0x0400674F RID: 26447
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to remove all items from.")]
		public FsmArray array;
	}
}
