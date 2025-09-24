using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200126C RID: 4716
	public class HeroPlayLookUpAnim : FsmStateAction
	{
		// Token: 0x06007C5F RID: 31839 RVA: 0x00252F7B File Offset: 0x0025117B
		public override void Reset()
		{
		}

		// Token: 0x06007C60 RID: 31840 RVA: 0x00252F80 File Offset: 0x00251180
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				HeroAnimationController animCtrl = instance.AnimCtrl;
				if (animCtrl)
				{
					animCtrl.PlayLookUp();
				}
			}
			base.Finish();
		}
	}
}
