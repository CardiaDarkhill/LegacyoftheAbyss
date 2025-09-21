using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001217 RID: 4631
	public class CancelCameraShake : FsmStateAction
	{
		// Token: 0x06007AFE RID: 31486 RVA: 0x0024E831 File Offset: 0x0024CA31
		public override void Reset()
		{
			this.Camera = GlobalSettings.Camera.MainCameraShakeManager;
			this.Profile = null;
		}

		// Token: 0x06007AFF RID: 31487 RVA: 0x0024E84C File Offset: 0x0024CA4C
		public override void OnEnter()
		{
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile shake = this.Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null)
			{
				cameraManagerReference.CancelShake(shake);
			}
			base.Finish();
		}

		// Token: 0x04007B61 RID: 31585
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B62 RID: 31586
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;
	}
}
