using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130F RID: 4879
	public class SetBlockNeedolinText : FsmStateAction
	{
		// Token: 0x06007EB9 RID: 32441 RVA: 0x00259A34 File Offset: 0x00257C34
		public override void Reset()
		{
			this.IsBlocked = null;
		}

		// Token: 0x06007EBA RID: 32442 RVA: 0x00259A3D File Offset: 0x00257C3D
		public override void OnEnter()
		{
			if (this.IsBlocked.Value)
			{
				NeedolinMsgBox.AddBlocker(base.Owner);
			}
			else
			{
				NeedolinMsgBox.RemoveBlocker(base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04007E67 RID: 32359
		[RequiredField]
		public FsmBool IsBlocked;
	}
}
