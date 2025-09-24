using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E21 RID: 3617
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if 2 Array Variables have the same values.")]
	public class ArrayCompare : FsmStateAction
	{
		// Token: 0x060067EE RID: 26606 RVA: 0x0020B59B File Offset: 0x0020979B
		public override void Reset()
		{
			this.array1 = null;
			this.array2 = null;
			this.SequenceEqual = null;
			this.SequenceNotEqual = null;
		}

		// Token: 0x060067EF RID: 26607 RVA: 0x0020B5B9 File Offset: 0x002097B9
		public override void OnEnter()
		{
			this.DoSequenceEqual();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067F0 RID: 26608 RVA: 0x0020B5D0 File Offset: 0x002097D0
		private void DoSequenceEqual()
		{
			if (this.array1.Values == null || this.array2.Values == null)
			{
				return;
			}
			this.storeResult.Value = this.TestSequenceEqual(this.array1.Values, this.array2.Values);
			base.Fsm.Event(this.storeResult.Value ? this.SequenceEqual : this.SequenceNotEqual);
		}

		// Token: 0x060067F1 RID: 26609 RVA: 0x0020B648 File Offset: 0x00209848
		private bool TestSequenceEqual(object[] _array1, object[] _array2)
		{
			if (_array1.Length != _array2.Length)
			{
				return false;
			}
			for (int i = 0; i < this.array1.Length; i++)
			{
				if (!_array1[i].Equals(_array2[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400671D RID: 26397
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The first Array Variable to test.")]
		public FsmArray array1;

		// Token: 0x0400671E RID: 26398
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The second Array Variable to test.")]
		public FsmArray array2;

		// Token: 0x0400671F RID: 26399
		[Tooltip("Event to send if the 2 arrays have the same values.")]
		public FsmEvent SequenceEqual;

		// Token: 0x04006720 RID: 26400
		[Tooltip("Event to send if the 2 arrays have different values.")]
		public FsmEvent SequenceNotEqual;

		// Token: 0x04006721 RID: 26401
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006722 RID: 26402
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
