using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137B RID: 4987
	[ActionCategory("Hollow Knight")]
	public class MenuStyleUnlockActionV2 : FsmStateAction
	{
		// Token: 0x0600805C RID: 32860 RVA: 0x0025E40C File Offset: 0x0025C60C
		public override void Reset()
		{
			this.UnlockKey = null;
			this.ForceChange = true;
		}

		// Token: 0x0600805D RID: 32861 RVA: 0x0025E421 File Offset: 0x0025C621
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.UnlockKey.Value))
			{
				MenuStyleUnlock.Unlock(this.UnlockKey.Value, this.ForceChange.Value);
			}
			base.Finish();
		}

		// Token: 0x04007FC5 RID: 32709
		public FsmString UnlockKey;

		// Token: 0x04007FC6 RID: 32710
		public FsmBool ForceChange;
	}
}
