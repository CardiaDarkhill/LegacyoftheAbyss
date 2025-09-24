using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E29 RID: 3625
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Gets the number of items in an Array.")]
	public class ArrayLength : FsmStateAction
	{
		// Token: 0x06006818 RID: 26648 RVA: 0x0020BE34 File Offset: 0x0020A034
		public override void Reset()
		{
			this.array = null;
			this.length = null;
			this.everyFrame = false;
		}

		// Token: 0x06006819 RID: 26649 RVA: 0x0020BE4B File Offset: 0x0020A04B
		public override void OnEnter()
		{
			this.length.Value = this.array.Length;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600681A RID: 26650 RVA: 0x0020BE71 File Offset: 0x0020A071
		public override void OnUpdate()
		{
			this.length.Value = this.array.Length;
		}

		// Token: 0x04006749 RID: 26441
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable.")]
		public FsmArray array;

		// Token: 0x0400674A RID: 26442
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the length in an {{Int Variable}}.")]
		public FsmInt length;

		// Token: 0x0400674B RID: 26443
		[Tooltip("Repeat every frame. Useful if the array is changing and you're waiting for a particular length.")]
		public bool everyFrame;
	}
}
