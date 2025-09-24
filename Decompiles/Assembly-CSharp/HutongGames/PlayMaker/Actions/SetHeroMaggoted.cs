using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011FF RID: 4607
	public class SetHeroMaggoted : FsmStateAction
	{
		// Token: 0x06007AA3 RID: 31395 RVA: 0x0024CF8E File Offset: 0x0024B18E
		public override void Reset()
		{
			this.Value = true;
		}

		// Token: 0x06007AA4 RID: 31396 RVA: 0x0024CF9C File Offset: 0x0024B19C
		public override void OnEnter()
		{
			if (this.Value.Value)
			{
				HeroController instance = HeroController.instance;
				if (Gameplay.MaggotCharm.IsEquipped && instance.playerData.MaggotCharmHits < 3)
				{
					instance.DidMaggotCharmHit();
				}
				else
				{
					MaggotRegion.SetIsMaggoted(true);
				}
			}
			else
			{
				MaggotRegion.SetIsMaggoted(false);
			}
			base.Finish();
		}

		// Token: 0x04007AE9 RID: 31465
		public FsmBool Value;
	}
}
