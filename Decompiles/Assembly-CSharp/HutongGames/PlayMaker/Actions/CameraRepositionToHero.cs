using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121E RID: 4638
	[ActionCategory("Hollow Knight")]
	public class CameraRepositionToHero : FsmStateAction
	{
		// Token: 0x06007B16 RID: 31510 RVA: 0x0024EBF5 File Offset: 0x0024CDF5
		public override void OnEnter()
		{
			if (GameManager.instance && GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.PositionToHero(false);
			}
			base.Finish();
		}
	}
}
