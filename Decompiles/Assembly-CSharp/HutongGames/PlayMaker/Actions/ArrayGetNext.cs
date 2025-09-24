using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E26 RID: 3622
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Each time this action is called it gets the next item from a Array. This lets you quickly loop through all the items of an array to perform actions on them.")]
	public class ArrayGetNext : FsmStateAction
	{
		// Token: 0x0600680B RID: 26635 RVA: 0x0020BAE5 File Offset: 0x00209CE5
		public override void Reset()
		{
			this.array = null;
			this.startIndex = null;
			this.endIndex = null;
			this.currentIndex = null;
			this.loopEvent = null;
			this.finishedEvent = null;
			this.resetFlag = null;
			this.result = null;
		}

		// Token: 0x0600680C RID: 26636 RVA: 0x0020BB20 File Offset: 0x00209D20
		public override void OnEnter()
		{
			if (this.nextItemIndex == 0 && this.startIndex.Value > 0)
			{
				this.nextItemIndex = this.startIndex.Value;
			}
			if (this.resetFlag.Value)
			{
				this.nextItemIndex = this.startIndex.Value;
				this.resetFlag.Value = false;
			}
			this.DoGetNextItem();
			base.Finish();
		}

		// Token: 0x0600680D RID: 26637 RVA: 0x0020BB8C File Offset: 0x00209D8C
		private void DoGetNextItem()
		{
			if (this.nextItemIndex >= this.array.Length)
			{
				this.nextItemIndex = 0;
				this.currentIndex.Value = this.array.Length - 1;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.result.SetValue(this.array.Get(this.nextItemIndex));
			if (this.nextItemIndex >= this.array.Length)
			{
				this.nextItemIndex = 0;
				this.currentIndex.Value = this.array.Length - 1;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			if (this.endIndex.Value > 0 && this.nextItemIndex >= this.endIndex.Value)
			{
				this.nextItemIndex = 0;
				this.currentIndex.Value = this.endIndex.Value;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextItemIndex++;
			this.currentIndex.Value = this.nextItemIndex - 1;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x04006736 RID: 26422
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006737 RID: 26423
		[Tooltip("From where to start iteration, leave as 0 to start from the beginning")]
		public FsmInt startIndex;

		// Token: 0x04006738 RID: 26424
		[Tooltip("When to end iteration, leave as 0 to iterate until the end")]
		public FsmInt endIndex;

		// Token: 0x04006739 RID: 26425
		[Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

		// Token: 0x0400673A RID: 26426
		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		// Token: 0x0400673B RID: 26427
		[Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		// Token: 0x0400673C RID: 26428
		[ActionSection("Result")]
		[MatchElementType("array")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current array item in a variable of the same type.")]
		public FsmVar result;

		// Token: 0x0400673D RID: 26429
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current array index in an int variable.")]
		public FsmInt currentIndex;

		// Token: 0x0400673E RID: 26430
		private int nextItemIndex;
	}
}
