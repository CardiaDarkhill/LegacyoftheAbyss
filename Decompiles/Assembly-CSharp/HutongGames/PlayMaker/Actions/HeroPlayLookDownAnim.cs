using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126D RID: 4717
	public class HeroPlayLookDownAnim : FsmStateAction
	{
		// Token: 0x06007C62 RID: 31842 RVA: 0x00252FBF File Offset: 0x002511BF
		public override void Reset()
		{
		}

		// Token: 0x06007C63 RID: 31843 RVA: 0x00252FC4 File Offset: 0x002511C4
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroAnimationController animCtrl = instance.AnimCtrl;
				if (animCtrl)
				{
					animCtrl.PlayLookDown();
				}
			}
			base.Finish();
		}
	}
}
