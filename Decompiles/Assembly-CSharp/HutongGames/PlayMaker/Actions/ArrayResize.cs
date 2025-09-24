using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E2C RID: 3628
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Resize an array.")]
	public class ArrayResize : FsmStateAction
	{
		// Token: 0x06006824 RID: 26660 RVA: 0x0020BFA4 File Offset: 0x0020A1A4
		public override void OnEnter()
		{
			if (this.newSize.Value >= 0)
			{
				this.array.Resize(this.newSize.Value);
			}
			else
			{
				base.LogError("Size out of range: " + this.newSize.Value.ToString());
				base.Fsm.Event(this.sizeOutOfRangeEvent);
			}
			base.Finish();
		}

		// Token: 0x04006750 RID: 26448
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to resize")]
		public FsmArray array;

		// Token: 0x04006751 RID: 26449
		[Tooltip("The new size of the array.")]
		public FsmInt newSize;

		// Token: 0x04006752 RID: 26450
		[Tooltip("The event to trigger if the new size is out of range")]
		public FsmEvent sizeOutOfRangeEvent;
	}
}
