using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001214 RID: 4628
	public class DoCameraShakeV4 : FsmStateAction
	{
		// Token: 0x06007AEE RID: 31470 RVA: 0x0024E274 File Offset: 0x0024C474
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
			this.vibrate = new FsmBool(true);
		}

		// Token: 0x06007AEF RID: 31471 RVA: 0x0024E2E1 File Offset: 0x0024C4E1
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

		// Token: 0x06007AF0 RID: 31472 RVA: 0x0024E314 File Offset: 0x0024C514
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

		// Token: 0x06007AF1 RID: 31473 RVA: 0x0024E348 File Offset: 0x0024C548
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

		// Token: 0x06007AF2 RID: 31474 RVA: 0x0024E3A0 File Offset: 0x0024C5A0
		private void DoShake()
		{
			if (this.didShake)
			{
				return;
			}
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
			bool value = this.vibrate.Value;
			if (cameraManagerReference != null && cameraShakeProfile)
			{
				GameObject safe = this.Target.GetSafe(this);
				if (!this.MaxCameraDistance.IsNone && safe)
				{
					cameraManagerReference.DoShakeInRange(cameraShakeProfile, base.Owner, this.MaxCameraDistance.Value, safe.transform.position, this.DoFreeze.Value, value);
				}
				else
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, this.DoFreeze.Value, value, true);
				}
			}
			this.didShake = true;
			base.Finish();
		}

		// Token: 0x04007B44 RID: 31556
		public FsmOwnerDefault Target;

		// Token: 0x04007B45 RID: 31557
		public FsmVector2 MaxCameraDistance;

		// Token: 0x04007B46 RID: 31558
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B47 RID: 31559
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B48 RID: 31560
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B49 RID: 31561
		public FsmFloat Delay;

		// Token: 0x04007B4A RID: 31562
		public bool CancelOnExit;

		// Token: 0x04007B4B RID: 31563
		public FsmBool vibrate;

		// Token: 0x04007B4C RID: 31564
		private float delayLeft;

		// Token: 0x04007B4D RID: 31565
		private bool didShake;
	}
}
