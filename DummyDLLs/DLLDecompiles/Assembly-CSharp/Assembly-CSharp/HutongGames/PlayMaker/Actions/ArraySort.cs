using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E30 RID: 3632
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Sort items in an Array.")]
	public class ArraySort : FsmStateAction
	{
		// Token: 0x06006831 RID: 26673 RVA: 0x0020C22A File Offset: 0x0020A42A
		public override void Reset()
		{
			this.array = null;
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x0020C234 File Offset: 0x0020A434
		public override void OnEnter()
		{
			List<object> list = new List<object>(this.array.Values);
			list.Sort();
			this.array.Values = list.ToArray();
			base.Finish();
		}

		// Token: 0x0400675C RID: 26460
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array to sort.")]
		public FsmArray array;
	}
}
