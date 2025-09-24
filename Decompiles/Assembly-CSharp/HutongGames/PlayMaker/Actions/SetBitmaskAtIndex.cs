using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC8 RID: 3016
	public class SetBitmaskAtIndex : FsmStateAction
	{
		// Token: 0x06005CAA RID: 23722 RVA: 0x001D2A57 File Offset: 0x001D0C57
		public override void Reset()
		{
			this.ReadMask = null;
			this.Index = null;
			this.SetValue = null;
			this.WriteMask = null;
		}

		// Token: 0x06005CAB RID: 23723 RVA: 0x001D2A78 File Offset: 0x001D0C78
		public override void OnEnter()
		{
			if (this.SetValue.Value)
			{
				this.WriteMask.Value |= 1 << this.Index.Value;
			}
			else
			{
				this.WriteMask.Value &= ~(1 << this.Index.Value);
			}
			base.Finish();
		}

		// Token: 0x0400583F RID: 22591
		public FsmInt ReadMask;

		// Token: 0x04005840 RID: 22592
		public FsmInt Index;

		// Token: 0x04005841 RID: 22593
		public FsmBool SetValue;

		// Token: 0x04005842 RID: 22594
		[UIHint(UIHint.Variable)]
		public FsmInt WriteMask;
	}
}
