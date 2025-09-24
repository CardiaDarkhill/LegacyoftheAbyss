using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122C RID: 4652
	public class SetCameraIgnoringXOffset : FsmStateAction
	{
		// Token: 0x06007B46 RID: 31558 RVA: 0x0024F340 File Offset: 0x0024D540
		public override void Reset()
		{
			this.IgnoreXOffset = null;
		}

		// Token: 0x06007B47 RID: 31559 RVA: 0x0024F349 File Offset: 0x0024D549
		public override void OnEnter()
		{
			GameCameras.instance.cameraTarget.SetIgnoringXOffset(this.IgnoreXOffset.Value);
			base.Finish();
		}

		// Token: 0x04007B91 RID: 31633
		public FsmBool IgnoreXOffset;
	}
}
