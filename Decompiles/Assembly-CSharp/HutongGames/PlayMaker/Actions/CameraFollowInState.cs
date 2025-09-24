using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122A RID: 4650
	public class CameraFollowInState : FsmStateAction
	{
		// Token: 0x06007B3A RID: 31546 RVA: 0x0024F119 File Offset: 0x0024D319
		public override void Reset()
		{
			this.Target = null;
			this.KeepOffset = true;
		}

		// Token: 0x06007B3B RID: 31547 RVA: 0x0024F130 File Offset: 0x0024D330
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
			Vector2 a = this.followTarget.position;
			Vector2 b = this.camTarget.transform.position;
			this.initialOffset = a - b;
			this.camCtrl.SetMode(CameraController.CameraMode.PANNING);
			this.AdjustCameraPosition();
		}

		// Token: 0x06007B3C RID: 31548 RVA: 0x0024F1C9 File Offset: 0x0024D3C9
		public override void OnUpdate()
		{
			this.AdjustCameraPosition();
		}

		// Token: 0x06007B3D RID: 31549 RVA: 0x0024F1D1 File Offset: 0x0024D3D1
		public override void OnExit()
		{
			this.camCtrl.SetMode(CameraController.CameraMode.PREVIOUS);
		}

		// Token: 0x06007B3E RID: 31550 RVA: 0x0024F1E0 File Offset: 0x0024D3E0
		private void AdjustCameraPosition()
		{
			Vector2 vector = this.followTarget.position;
			if (this.KeepOffset.Value)
			{
				vector += this.initialOffset;
			}
			this.camCtrl.SnapTo(vector.x, vector.y);
		}

		// Token: 0x04007B85 RID: 31621
		public FsmOwnerDefault Target;

		// Token: 0x04007B86 RID: 31622
		public FsmBool KeepOffset;

		// Token: 0x04007B87 RID: 31623
		private Vector2 initialOffset;

		// Token: 0x04007B88 RID: 31624
		private Transform followTarget;

		// Token: 0x04007B89 RID: 31625
		private CameraController camCtrl;

		// Token: 0x04007B8A RID: 31626
		private CameraTarget camTarget;
	}
}
