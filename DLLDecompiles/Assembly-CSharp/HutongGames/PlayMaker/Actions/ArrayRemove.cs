using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E2A RID: 3626
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Remove an item from an array.")]
	public class ArrayRemove : FsmStateAction
	{
		// Token: 0x0600681C RID: 26652 RVA: 0x0020BE91 File Offset: 0x0020A091
		public override void Reset()
		{
			this.array = null;
			this.value = null;
			this.allMatches = new FsmBool
			{
				Value = true
			};
		}

		// Token: 0x0600681D RID: 26653 RVA: 0x0020BEB3 File Offset: 0x0020A0B3
		public override void OnEnter()
		{
			this.DoRemoveValue();
			base.Finish();
		}

		// Token: 0x0600681E RID: 26654 RVA: 0x0020BEC4 File Offset: 0x0020A0C4
		private void DoRemoveValue()
		{
			if (this.array == null || this.value == null)
			{
				return;
			}
			this.value.UpdateValue();
			List<object> list = new List<object>(this.array.Values);
			if (this.allMatches.Value)
			{
				list.RemoveAll((object x) => (x == null && this.value.GetValue() == null) || (x != null && x.Equals(this.value.GetValue())));
			}
			else
			{
				list.Remove(this.value.GetValue());
			}
			this.array.Values = list.ToArray();
			this.array.SaveChanges();
		}

		// Token: 0x0400674C RID: 26444
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x0400674D RID: 26445
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("Item to remove.")]
		public FsmVar value;

		// Token: 0x0400674E RID: 26446
		[Tooltip("Remove all instances of the value. Otherwise removes only the first instance.")]
		public FsmBool allMatches;
	}
}
