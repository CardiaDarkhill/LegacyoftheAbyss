using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001221 RID: 4641
	[ActionCategory("Hollow Knight")]
	public class ScreenFlashBomb : FsmStateAction
	{
		// Token: 0x06007B21 RID: 31521 RVA: 0x0024EDAA File Offset: 0x0024CFAA
		public override void OnEnter()
		{
			if (GameManager.instance && GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.ScreenFlashBomb();
			}
			base.Finish();
		}
	}
}
