using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E23 RID: 3619
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Delete the item at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
	public class ArrayDeleteAt : FsmStateAction
	{
		// Token: 0x060067F7 RID: 26615 RVA: 0x0020B79E File Offset: 0x0020999E
		public override void Reset()
		{
			this.array = null;
			this.index = null;
			this.indexOutOfRangeEvent = null;
		}

		// Token: 0x060067F8 RID: 26616 RVA: 0x0020B7B5 File Offset: 0x002099B5
		public override void OnEnter()
		{
			this.DoDeleteAt();
			base.Finish();
		}

		// Token: 0x060067F9 RID: 26617 RVA: 0x0020B7C4 File Offset: 0x002099C4
		private void DoDeleteAt()
		{
			if (this.index.Value >= 0 && this.index.Value < this.array.Length)
			{
				List<object> list = new List<object>(this.array.Values);
				list.RemoveAt(this.index.Value);
				this.array.Values = list.ToArray();
				return;
			}
			base.Fsm.Event(this.indexOutOfRangeEvent);
		}

		// Token: 0x04006729 RID: 26409
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x0400672A RID: 26410
		[Tooltip("The index into the array.")]
		public FsmInt index;

		// Token: 0x0400672B RID: 26411
		[ActionSection("Result")]
		[Tooltip("The event to trigger if the index is out of range.")]
		public FsmEvent indexOutOfRangeEvent;
	}
}
