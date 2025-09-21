using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122B RID: 4651
	public class CameraFollowYInState : FsmStateAction
	{
		// Token: 0x06007B40 RID: 31552 RVA: 0x0024F237 File Offset: 0x0024D437
		public override void Reset()
		{
			this.Target = null;
			this.KeepOffset = true;
		}

		// Token: 0x06007B41 RID: 31553 RVA: 0x0024F24C File Offset: 0x0024D44C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.followTarget = safe.transform;
			GameCameras instance = GameCameras.instance;
			this.camCtrl = instance.cameraController;
			this.camTarget = instance.cameraTarget;
			float y = this.followTarget.position.y;
			float y2 = this.camTarget.transform.position.y;
			this.initialOffset = y - y2;
			this.camCtrl.SetMode(CameraController.CameraMode.PANNING);
			this.AdjustCameraPosition();
		}

		// Token: 0x06007B42 RID: 31554 RVA: 0x0024F2E1 File Offset: 0x0024D4E1
		public override void OnUpdate()
		{
			this.AdjustCameraPosition();
		}

		// Token: 0x06007B43 RID: 31555 RVA: 0x0024F2E9 File Offset: 0x0024D4E9
		public override void OnExit()
		{
			this.camCtrl.SetMode(CameraController.CameraMode.PREVIOUS);
		}

		// Token: 0x06007B44 RID: 31556 RVA: 0x0024F2F8 File Offset: 0x0024D4F8
		private void AdjustCameraPosition()
		{
			float num = this.followTarget.position.y;
			if (this.KeepOffset.Value)
			{
				num += this.initialOffset;
			}
			this.camCtrl.SnapTargetToY(num);
		}

		// Token: 0x04007B8B RID: 31627
		public FsmOwnerDefault Target;

		// Token: 0x04007B8C RID: 31628
		public FsmBool KeepOffset;

		// Token: 0x04007B8D RID: 31629
		private float initialOffset;

		// Token: 0x04007B8E RID: 31630
		private Transform followTarget;

		// Token: 0x04007B8F RID: 31631
		private CameraController camCtrl;

		// Token: 0x04007B90 RID: 31632
		private CameraTarget camTarget;
	}
}
