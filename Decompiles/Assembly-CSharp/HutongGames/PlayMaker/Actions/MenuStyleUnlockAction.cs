using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137A RID: 4986
	[ActionCategory("Hollow Knight")]
	public class MenuStyleUnlockAction : FsmStateAction
	{
		// Token: 0x06008059 RID: 32857 RVA: 0x0025E3D0 File Offset: 0x0025C5D0
		public override void Reset()
		{
			this.unlockKey = null;
		}

		// Token: 0x0600805A RID: 32858 RVA: 0x0025E3D9 File Offset: 0x0025C5D9
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.unlockKey.Value))
			{
				MenuStyleUnlock.Unlock(this.unlockKey.Value, true);
			}
			base.Finish();
		}

		// Token: 0x04007FC4 RID: 32708
		public FsmString unlockKey;
	}
}
