using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121A RID: 4634
	public class SendCameraShakeWorldForce : FsmStateAction
	{
		// Token: 0x06007B09 RID: 31497 RVA: 0x0024EA26 File Offset: 0x0024CC26
		public override void Reset()
		{
			this.Camera = GlobalSettings.Camera.MainCameraShakeManager;
			this.WorldForce = null;
		}

		// Token: 0x06007B0A RID: 31498 RVA: 0x0024EA40 File Offset: 0x0024CC40
		public override void OnEnter()
		{
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			if (cameraManagerReference != null)
			{
				cameraManagerReference.SendWorldForce((CameraShakeWorldForceIntensities)this.WorldForce.Value);
			}
			base.Finish();
		}

		// Token: 0x04007B69 RID: 31593
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B6A RID: 31594
		[ObjectType(typeof(CameraShakeWorldForceIntensities))]
		public FsmEnum WorldForce;
	}
}
