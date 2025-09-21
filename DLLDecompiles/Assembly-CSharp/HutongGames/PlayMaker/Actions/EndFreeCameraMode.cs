using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001228 RID: 4648
	public class EndFreeCameraMode : FsmStateAction
	{
		// Token: 0x06007B33 RID: 31539 RVA: 0x0024EF98 File Offset: 0x0024D198
		public override void OnEnter()
		{
			GameCameras instance = GameCameras.instance;
			instance.cameraTarget.EndFreeMode();
			CameraController cameraController = instance.cameraController;
			cameraController.SetMode(CameraController.CameraMode.FOLLOWING);
			CameraLockArea currentLockArea = cameraController.CurrentLockArea;
			if (currentLockArea)
			{
				cameraController.LockToArea(currentLockArea);
			}
			base.Finish();
		}
	}
}
