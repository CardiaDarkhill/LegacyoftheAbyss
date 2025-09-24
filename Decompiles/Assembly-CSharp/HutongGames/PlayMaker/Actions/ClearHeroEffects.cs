using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125E RID: 4702
	public class ClearHeroEffects : FsmStateAction
	{
		// Token: 0x06007C1C RID: 31772 RVA: 0x00251B28 File Offset: 0x0024FD28
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			this.hc.ClearEffects();
			base.Finish();
		}

		// Token: 0x04007C37 RID: 31799
		private HeroController hc;
	}
}
