using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E2E RID: 3630
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Set the value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
	public class ArraySet : FsmStateAction
	{
		// Token: 0x06006829 RID: 26665 RVA: 0x0020C067 File Offset: 0x0020A267
		public override void Reset()
		{
			this.array = null;
			this.index = null;
			this.value = null;
			this.everyFrame = false;
			this.indexOutOfRange = null;
		}

		// Token: 0x0600682A RID: 26666 RVA: 0x0020C08C File Offset: 0x0020A28C
		public override void OnEnter()
		{
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600682B RID: 26667 RVA: 0x0020C0A2 File Offset: 0x0020A2A2
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x0600682C RID: 26668 RVA: 0x0020C0AC File Offset: 0x0020A2AC
		private void DoSetValue()
		{
			if (this.array.IsNone)
			{
				return;
			}
			if (this.index.Value >= 0 && this.index.Value < this.array.Length)
			{
				this.value.UpdateValue();
				this.array.Set(this.index.Value, this.value.GetValue());
				return;
			}
			base.Fsm.Event(this.indexOutOfRange);
		}

		// Token: 0x04006754 RID: 26452
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x04006755 RID: 26453
		[Tooltip("The index into the array.")]
		public FsmInt index;

		// Token: 0x04006756 RID: 26454
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("Set the value of the array at the specified index.")]
		public FsmVar value;

		// Token: 0x04006757 RID: 26455
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04006758 RID: 26456
		[ActionSection("Events")]
		[Tooltip("The event to trigger if the index is out of range")]
		public FsmEvent indexOutOfRange;
	}
}
