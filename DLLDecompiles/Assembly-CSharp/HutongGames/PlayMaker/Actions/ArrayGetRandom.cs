using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E27 RID: 3623
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Get a Random item from an Array.")]
	public class ArrayGetRandom : FsmStateAction
	{
		// Token: 0x0600680F RID: 26639 RVA: 0x0020BCCF File Offset: 0x00209ECF
		public override void Reset()
		{
			this.array = null;
			this.storeValue = null;
			this.index = null;
			this.everyFrame = false;
			this.noRepeat = false;
		}

		// Token: 0x06006810 RID: 26640 RVA: 0x0020BCF9 File Offset: 0x00209EF9
		public override void OnEnter()
		{
			this.DoGetRandomValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006811 RID: 26641 RVA: 0x0020BD0F File Offset: 0x00209F0F
		public override void OnUpdate()
		{
			this.DoGetRandomValue();
		}

		// Token: 0x06006812 RID: 26642 RVA: 0x0020BD18 File Offset: 0x00209F18
		private void DoGetRandomValue()
		{
			if (this.storeValue.IsNone)
			{
				return;
			}
			if (!this.noRepeat.Value || this.array.Length == 1)
			{
				this.randomIndex = Random.Range(0, this.array.Length);
			}
			else
			{
				do
				{
					this.randomIndex = Random.Range(0, this.array.Length);
				}
				while (this.randomIndex == this.lastIndex);
				this.lastIndex = this.randomIndex;
			}
			this.index.Value = this.randomIndex;
			this.storeValue.SetValue(this.array.Get(this.index.Value));
		}

		// Token: 0x0400673F RID: 26431
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array to use.")]
		public FsmArray array;

		// Token: 0x04006740 RID: 26432
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a variable.")]
		[MatchElementType("array")]
		public FsmVar storeValue;

		// Token: 0x04006741 RID: 26433
		[Tooltip("The index of the value in the array.")]
		[UIHint(UIHint.Variable)]
		public FsmInt index;

		// Token: 0x04006742 RID: 26434
		[Tooltip("Don't get the same item twice in a row.")]
		public FsmBool noRepeat;

		// Token: 0x04006743 RID: 26435
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006744 RID: 26436
		private int randomIndex;

		// Token: 0x04006745 RID: 26437
		private int lastIndex = -1;
	}
}
