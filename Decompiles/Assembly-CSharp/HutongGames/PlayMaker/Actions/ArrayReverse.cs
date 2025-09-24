using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E2D RID: 3629
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Reverse the order of items in an Array.")]
	public class ArrayReverse : FsmStateAction
	{
		// Token: 0x06006826 RID: 26662 RVA: 0x0020C019 File Offset: 0x0020A219
		public override void Reset()
		{
			this.array = null;
		}

		// Token: 0x06006827 RID: 26663 RVA: 0x0020C024 File Offset: 0x0020A224
		public override void OnEnter()
		{
			List<object> list = new List<object>(this.array.Values);
			list.Reverse();
			this.array.Values = list.ToArray();
			base.Finish();
		}

		// Token: 0x04006753 RID: 26451
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array to reverse.")]
		public FsmArray array;
	}
}
