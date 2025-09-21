using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E22 RID: 3618
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Check if an Array contains a value. Optionally get its index.")]
	public class ArrayContains : FsmStateAction
	{
		// Token: 0x060067F3 RID: 26611 RVA: 0x0020B68D File Offset: 0x0020988D
		public override void Reset()
		{
			this.array = null;
			this.value = null;
			this.index = null;
			this.isContained = null;
			this.isContainedEvent = null;
			this.isNotContainedEvent = null;
		}

		// Token: 0x060067F4 RID: 26612 RVA: 0x0020B6B9 File Offset: 0x002098B9
		public override void OnEnter()
		{
			this.DoCheckContainsValue();
			base.Finish();
		}

		// Token: 0x060067F5 RID: 26613 RVA: 0x0020B6C8 File Offset: 0x002098C8
		private void DoCheckContainsValue()
		{
			this.value.UpdateValue();
			int num;
			if (this.value.GetValue() == null || this.value.GetValue().Equals(null))
			{
				num = Array.FindIndex<object>(this.array.Values, (object x) => x == null || x.Equals(null));
			}
			else
			{
				num = Array.IndexOf<object>(this.array.Values, this.value.GetValue());
			}
			bool flag = num != -1;
			this.isContained.Value = flag;
			this.index.Value = num;
			if (flag)
			{
				base.Fsm.Event(this.isContainedEvent);
				return;
			}
			base.Fsm.Event(this.isNotContainedEvent);
		}

		// Token: 0x04006723 RID: 26403
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006724 RID: 26404
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("The value to check against in the array.")]
		public FsmVar value;

		// Token: 0x04006725 RID: 26405
		[ActionSection("Result")]
		[Tooltip("The index of the value in the array.")]
		[UIHint(UIHint.Variable)]
		public FsmInt index;

		// Token: 0x04006726 RID: 26406
		[Tooltip("Store in a bool whether it contains that element or not.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		// Token: 0x04006727 RID: 26407
		[Tooltip("Event sent if the array contains that element.")]
		public FsmEvent isContainedEvent;

		// Token: 0x04006728 RID: 26408
		[Tooltip("Event sent if the array does not contain that element.")]
		public FsmEvent isNotContainedEvent;
	}
}
