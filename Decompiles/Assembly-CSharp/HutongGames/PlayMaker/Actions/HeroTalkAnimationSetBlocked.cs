using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F7 RID: 4599
	public class HeroTalkAnimationSetBlocked : FsmStateAction
	{
		// Token: 0x06007A84 RID: 31364 RVA: 0x0024CA0D File Offset: 0x0024AC0D
		public override void Reset()
		{
			this.Value = null;
		}

		// Token: 0x06007A85 RID: 31365 RVA: 0x0024CA16 File Offset: 0x0024AC16
		public override void OnEnter()
		{
			HeroTalkAnimation.SetBlocked(this.Value.Value);
			base.Finish();
		}

		// Token: 0x04007AC5 RID: 31429
		public FsmBool Value;
	}
}
