using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BCA RID: 3018
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Flips the value of a Bool Variable.")]
	public class BoolFlipEveryFrame : FsmStateAction
	{
		// Token: 0x06005CB0 RID: 23728 RVA: 0x001D2B71 File Offset: 0x001D0D71
		public override void Reset()
		{
			this.boolVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x06005CB1 RID: 23729 RVA: 0x001D2B81 File Offset: 0x001D0D81
		public override void OnEnter()
		{
			this.DoFlip();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CB2 RID: 23730 RVA: 0x001D2B97 File Offset: 0x001D0D97
		public override void OnUpdate()
		{
			this.DoFlip();
		}

		// Token: 0x06005CB3 RID: 23731 RVA: 0x001D2B9F File Offset: 0x001D0D9F
		private void DoFlip()
		{
			this.boolVariable.Value = !this.boolVariable.Value;
		}

		// Token: 0x04005848 RID: 22600
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Bool variable to flip.")]
		public FsmBool boolVariable;

		// Token: 0x04005849 RID: 22601
		public bool everyFrame;
	}
}
