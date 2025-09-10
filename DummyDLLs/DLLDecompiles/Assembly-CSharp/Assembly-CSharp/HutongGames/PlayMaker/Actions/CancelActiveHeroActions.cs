using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001271 RID: 4721
	public sealed class CancelActiveHeroActions : FsmStateAction
	{
		// Token: 0x06007C70 RID: 31856 RVA: 0x002531DE File Offset: 0x002513DE
		public override void OnEnter()
		{
			HeroUtility.CancelCancellables();
			base.Finish();
		}
	}
}
