using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001212 RID: 4626
	public class DoCameraShakeV2 : FsmStateAction
	{
		// Token: 0x06007AE2 RID: 31458 RVA: 0x0024DE9C File Offset: 0x0024C09C
		public override void Reset()
		{
			this.Target = null;
			this.Camera = GlobalSettings.Camera.MainCameraShakeManager;
			this.MaxCameraDistance = new FsmVector2
			{
				UseVariable = true
			};
			this.Profile = null;
			this.DoFreeze = new FsmBool(true);
			this.Delay = null;
			this.CancelOnExit = false;
		}

		// Token: 0x06007AE3 RID: 31459 RVA: 0x0024DEF8 File Offset: 0x0024C0F8
		public override void OnEnter()
		{
			this.didShake = false;
			if (this.Delay.Value <= 0f)
			{
				this.DoShake();
				return;
			}
			this.delayLeft = this.Delay.Value;
		}

		// Token: 0x06007AE4 RID: 31460 RVA: 0x0024DF2B File Offset: 0x0024C12B
		public override void OnUpdate()
		{
			if (this.delayLeft > 0f)
			{
				this.delayLeft -= Time.deltaTime;
				if (this.delayLeft <= 0f)
				{
					this.DoShake();
				}
			}
		}

		// Token: 0x06007AE5 RID: 31461 RVA: 0x0024DF60 File Offset: 0x0024C160
		public override void OnExit()
		{
			if (this.CancelOnExit && this.didShake)
			{
				CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
				CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
				if (cameraManagerReference != null && cameraShakeProfile)
				{
					cameraManagerReference.CancelShake(cameraShakeProfile);
				}
			}
		}

		// Token: 0x06007AE6 RID: 31462 RVA: 0x0024DFB8 File Offset: 0x0024C1B8
		private void DoShake()
		{
			if (this.didShake)
			{
				return;
			}
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && cameraShakeProfile)
			{
				GameObject safe = this.Target.GetSafe(this);
				if (!this.MaxCameraDistance.IsNone && safe)
				{
					cameraManagerReference.DoShakeInRange(cameraShakeProfile, base.Owner, this.MaxCameraDistance.Value, safe.transform.position, this.DoFreeze.Value, true);
				}
				else
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, this.DoFreeze.Value, true, true);
				}
			}
			this.didShake = true;
			base.Finish();
		}

		// Token: 0x04007B32 RID: 31538
		public FsmOwnerDefault Target;

		// Token: 0x04007B33 RID: 31539
		public FsmVector2 MaxCameraDistance;

		// Token: 0x04007B34 RID: 31540
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B35 RID: 31541
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B36 RID: 31542
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B37 RID: 31543
		public FsmFloat Delay;

		// Token: 0x04007B38 RID: 31544
		public bool CancelOnExit;

		// Token: 0x04007B39 RID: 31545
		private float delayLeft;

		// Token: 0x04007B3A RID: 31546
		private bool didShake;
	}
}
