using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121D RID: 4637
	[ActionCategory("Hollow Knight")]
	public class CameraStopFreeze : FsmStateAction
	{
		// Token: 0x06007B13 RID: 31507 RVA: 0x0024EBAC File Offset: 0x0024CDAC
		public override void Reset()
		{
			this.freezeTargetAlso = true;
		}

		// Token: 0x06007B14 RID: 31508 RVA: 0x0024EBBA File Offset: 0x0024CDBA
		public override void OnEnter()
		{
			if (GameManager.instance.cameraCtrl)
			{
				GameManager.instance.cameraCtrl.StopFreeze(this.freezeTargetAlso.Value);
			}
			base.Finish();
		}

		// Token: 0x04007B72 RID: 31602
		public FsmBool freezeTargetAlso;
	}
}
