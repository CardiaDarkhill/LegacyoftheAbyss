using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001226 RID: 4646
	public class StartFreeCameraMode : FsmStateAction
	{
		// Token: 0x06007B2D RID: 31533 RVA: 0x0024EEF7 File Offset: 0x0024D0F7
		public override void Reset()
		{
			this.StoreObject = null;
		}

		// Token: 0x06007B2E RID: 31534 RVA: 0x0024EF00 File Offset: 0x0024D100
		public override void OnEnter()
		{
			CameraTarget cameraTarget = GameCameras.instance.cameraTarget;
			cameraTarget.StartFreeMode(true);
			this.StoreObject.Value = cameraTarget.gameObject;
			base.Finish();
		}

		// Token: 0x04007B7C RID: 31612
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;
	}
}
