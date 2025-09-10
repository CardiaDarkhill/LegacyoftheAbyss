using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001220 RID: 4640
	[ActionCategory("Hollow Knight")]
	public class ScreenFlashLifeblood : FsmStateAction
	{
		// Token: 0x06007B1F RID: 31519 RVA: 0x0024ED50 File Offset: 0x0024CF50
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance && instance.cameraCtrl)
			{
				if (Gameplay.PoisonPouchTool.IsEquipped)
				{
					instance.cameraCtrl.ScreenFlashPoison();
				}
				else
				{
					instance.cameraCtrl.ScreenFlashLifeblood();
				}
			}
			base.Finish();
		}
	}
}
