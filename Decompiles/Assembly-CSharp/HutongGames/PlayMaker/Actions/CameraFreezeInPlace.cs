using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121C RID: 4636
	[ActionCategory("Hollow Knight")]
	public class CameraFreezeInPlace : FsmStateAction
	{
		// Token: 0x06007B10 RID: 31504 RVA: 0x0024EB63 File Offset: 0x0024CD63
		public override void Reset()
		{
			this.freezeTargetAlso = true;
		}

		// Token: 0x06007B11 RID: 31505 RVA: 0x0024EB71 File Offset: 0x0024CD71
		public override void OnEnter()
		{
			if (GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.FreezeInPlace(this.freezeTargetAlso.Value);
			}
			base.Finish();
		}

		// Token: 0x04007B71 RID: 31601
		public FsmBool freezeTargetAlso;
	}
}
