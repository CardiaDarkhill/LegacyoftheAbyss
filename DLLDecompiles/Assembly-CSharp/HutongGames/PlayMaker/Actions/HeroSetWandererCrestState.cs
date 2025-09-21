using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001255 RID: 4693
	public class HeroSetWandererCrestState : FsmStateAction
	{
		// Token: 0x06007BFB RID: 31739 RVA: 0x002512C6 File Offset: 0x0024F4C6
		public override void Reset()
		{
			this.QueuedNextHitCritical = new FsmBool
			{
				UseVariable = true
			};
			this.CriticalHitsLocked = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06007BFC RID: 31740 RVA: 0x002512EC File Offset: 0x0024F4EC
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			HeroController.WandererCrestStateInfo wandererState = instance.WandererState;
			if (!this.QueuedNextHitCritical.IsNone)
			{
				wandererState.QueuedNextHitCritical = this.QueuedNextHitCritical.Value;
			}
			if (!this.CriticalHitsLocked.IsNone)
			{
				wandererState.CriticalHitsLocked = this.CriticalHitsLocked.Value;
			}
			instance.WandererState = wandererState;
			base.Finish();
		}

		// Token: 0x04007C22 RID: 31778
		public FsmBool QueuedNextHitCritical;

		// Token: 0x04007C23 RID: 31779
		public FsmBool CriticalHitsLocked;
	}
}
