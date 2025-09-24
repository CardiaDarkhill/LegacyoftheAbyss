using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001227 RID: 4647
	public class StartFreeCameraModeV2 : FsmStateAction
	{
		// Token: 0x06007B30 RID: 31536 RVA: 0x0024EF3E File Offset: 0x0024D13E
		public override void Reset()
		{
			this.StoreObject = null;
			this.UseXOffset = null;
		}

		// Token: 0x06007B31 RID: 31537 RVA: 0x0024EF50 File Offset: 0x0024D150
		public override void OnEnter()
		{
			CameraTarget cameraTarget = GameCameras.instance.cameraTarget;
			cameraTarget.StartFreeMode(this.UseXOffset.Value);
			this.StoreObject.Value = cameraTarget.gameObject;
			base.Finish();
		}

		// Token: 0x04007B7D RID: 31613
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		// Token: 0x04007B7E RID: 31614
		public FsmBool UseXOffset;
	}
}
