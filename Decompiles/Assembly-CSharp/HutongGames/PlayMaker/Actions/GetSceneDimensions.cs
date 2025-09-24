using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001224 RID: 4644
	public class GetSceneDimensions : FsmStateAction
	{
		// Token: 0x06007B27 RID: 31527 RVA: 0x0024EE69 File Offset: 0x0024D069
		public override void Reset()
		{
			this.StoreWidth = null;
			this.StoreHeight = null;
		}

		// Token: 0x06007B28 RID: 31528 RVA: 0x0024EE7C File Offset: 0x0024D07C
		public override void OnEnter()
		{
			CameraController cameraController = GameCameras.instance.cameraController;
			this.StoreWidth.Value = cameraController.sceneWidth;
			this.StoreHeight.Value = cameraController.sceneHeight;
			base.Finish();
		}

		// Token: 0x04007B79 RID: 31609
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreWidth;

		// Token: 0x04007B7A RID: 31610
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHeight;
	}
}
