using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001218 RID: 4632
	[Serializable]
	public class FsmCameraShakeTarget
	{
		// Token: 0x06007B01 RID: 31489 RVA: 0x0024E89C File Offset: 0x0024CA9C
		public void DoShake(Object source)
		{
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && cameraShakeProfile)
			{
				cameraManagerReference.DoShake(cameraShakeProfile, source, true, true, true);
			}
		}

		// Token: 0x06007B02 RID: 31490 RVA: 0x0024E8E8 File Offset: 0x0024CAE8
		public void CancelShake()
		{
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && cameraShakeProfile)
			{
				cameraManagerReference.CancelShake(cameraShakeProfile);
			}
		}

		// Token: 0x04007B63 RID: 31587
		[ObjectType(typeof(CameraManagerReference))]
		public FsmObject Camera;

		// Token: 0x04007B64 RID: 31588
		[ObjectType(typeof(CameraShakeProfile))]
		public FsmObject Profile;
	}
}
