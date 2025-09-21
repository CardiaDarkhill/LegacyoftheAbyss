using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001223 RID: 4643
	[ActionCategory("Hollow Knight")]
	public class ScreenFlash : FsmStateAction
	{
		// Token: 0x06007B25 RID: 31525 RVA: 0x0024EE22 File Offset: 0x0024D022
		public override void OnEnter()
		{
			if (GameManager.instance && GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.ScreenFlash(this.flashColour.Value);
			}
			base.Finish();
		}

		// Token: 0x04007B78 RID: 31608
		public FsmColor flashColour;
	}
}
