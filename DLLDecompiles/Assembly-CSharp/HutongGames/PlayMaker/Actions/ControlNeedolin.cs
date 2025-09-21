using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E2 RID: 4834
	public class ControlNeedolin : FsmStateAction
	{
		// Token: 0x06007E19 RID: 32281 RVA: 0x00258354 File Offset: 0x00256554
		public override void Reset()
		{
			this.isPlaying = null;
		}

		// Token: 0x06007E1A RID: 32282 RVA: 0x0025835D File Offset: 0x0025655D
		public override void OnEnter()
		{
			HeroPerformanceRegion.IsPerforming = this.isPlaying.Value;
			base.Finish();
		}

		// Token: 0x04007DF1 RID: 32241
		public FsmOwnerDefault target;

		// Token: 0x04007DF2 RID: 32242
		public FsmBool isPlaying;
	}
}
