using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E25 RID: 3621
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Get a value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
	public class ArrayGet : FsmStateAction
	{
		// Token: 0x06006806 RID: 26630 RVA: 0x0020BA17 File Offset: 0x00209C17
		public override void Reset()
		{
			this.array = null;
			this.index = null;
			this.everyFrame = false;
			this.storeValue = null;
			this.indexOutOfRange = null;
		}

		// Token: 0x06006807 RID: 26631 RVA: 0x0020BA3C File Offset: 0x00209C3C
		public override void OnEnter()
		{
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006808 RID: 26632 RVA: 0x0020BA52 File Offset: 0x00209C52
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x06006809 RID: 26633 RVA: 0x0020BA5C File Offset: 0x00209C5C
		private void DoGetValue()
		{
			if (this.array.IsNone || this.storeValue.IsNone)
			{
				return;
			}
			if (this.index.Value >= 0 && this.index.Value < this.array.Length)
			{
				this.storeValue.SetValue(this.array.Get(this.index.Value));
				return;
			}
			base.Fsm.Event(this.indexOutOfRange);
		}

		// Token: 0x04006731 RID: 26417
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006732 RID: 26418
		[Tooltip("The index into the array.")]
		public FsmInt index;

		// Token: 0x04006733 RID: 26419
		[RequiredField]
		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a variable. NOTE: must be of the same type as the array.")]
		public FsmVar storeValue;

		// Token: 0x04006734 RID: 26420
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006735 RID: 26421
		[ActionSection("Events")]
		[Tooltip("The event to trigger if the index is out of range.")]
		public FsmEvent indexOutOfRange;
	}
}
