using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001222 RID: 4642
	[ActionCategory("Hollow Knight")]
	public class ScreenFlashTrobbio : FsmStateAction
	{
		// Token: 0x06007B23 RID: 31523 RVA: 0x0024EDE6 File Offset: 0x0024CFE6
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
