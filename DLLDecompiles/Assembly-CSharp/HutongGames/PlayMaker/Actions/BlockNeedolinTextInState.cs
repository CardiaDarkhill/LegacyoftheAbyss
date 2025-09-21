using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130E RID: 4878
	public class BlockNeedolinTextInState : FsmStateAction
	{
		// Token: 0x06007EB4 RID: 32436 RVA: 0x00259993 File Offset: 0x00257B93
		public override void Reset()
		{
			this.IsBlocked = null;
		}

		// Token: 0x06007EB5 RID: 32437 RVA: 0x0025999C File Offset: 0x00257B9C
		public override void OnEnter()
		{
			if (this.IsBlocked.Value)
			{
				NeedolinMsgBox.AddBlocker(base.Owner);
			}
			this.wasBlocked = this.IsBlocked.Value;
		}

		// Token: 0x06007EB6 RID: 32438 RVA: 0x002599C8 File Offset: 0x00257BC8
		public override void OnUpdate()
		{
			if (this.IsBlocked.Value == this.wasBlocked)
			{
				return;
			}
			if (this.IsBlocked.Value)
			{
				NeedolinMsgBox.AddBlocker(base.Owner);
			}
			else
			{
				NeedolinMsgBox.RemoveBlocker(base.Owner);
			}
			this.wasBlocked = this.IsBlocked.Value;
		}

		// Token: 0x06007EB7 RID: 32439 RVA: 0x00259A1F File Offset: 0x00257C1F
		public override void OnExit()
		{
			NeedolinMsgBox.RemoveBlocker(base.Owner);
		}

		// Token: 0x04007E65 RID: 32357
		[RequiredField]
		public FsmBool IsBlocked;

		// Token: 0x04007E66 RID: 32358
		private bool wasBlocked;
	}
}
