using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001216 RID: 4630
	public class DoCameraShakeRepeatingV2 : FsmStateAction
	{
		// Token: 0x06007AF9 RID: 31481 RVA: 0x0024E644 File Offset: 0x0024C844
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
			this.RepeatDelay = null;
			this.vibrate = true;
		}

		// Token: 0x06007AFA RID: 31482 RVA: 0x0024E6AC File Offset: 0x0024C8AC
		public override void OnEnter()
		{
			if (this.Delay.Value <= 0f)
			{
				this.DoShake();
				return;
			}
			this.delayLeft = this.Delay.Value;
		}

		// Token: 0x06007AFB RID: 31483 RVA: 0x0024E6D8 File Offset: 0x0024C8D8
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
			if (this.repeatDelayLeft > 0f)
			{
				this.repeatDelayLeft -= Time.deltaTime;
				if (this.repeatDelayLeft <= 0f)
				{
					this.DoShake();
				}
			}
		}

		// Token: 0x06007AFC RID: 31484 RVA: 0x0024E74C File Offset: 0x0024C94C
		private void DoShake()
		{
			CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
			CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
			if (cameraManagerReference != null && cameraShakeProfile)
			{
				GameObject safe = this.Target.GetSafe(this);
				if (!this.MaxCameraDistance.IsNone && safe)
				{
					cameraManagerReference.DoShakeInRange(cameraShakeProfile, base.Owner, this.MaxCameraDistance.Value, safe.transform.position, this.DoFreeze.Value, this.vibrate.Value);
				}
				else
				{
					cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, this.DoFreeze.Value, this.vibrate.Value, true);
				}
			}
			this.repeatDelayLeft = this.RepeatDelay.Value;
		}

		// Token: 0x04007B57 RID: 31575
		public FsmOwnerDefault Target;

		// Token: 0x04007B58 RID: 31576
		public FsmVector2 MaxCameraDistance;

		// Token: 0x04007B59 RID: 31577
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B5A RID: 31578
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B5B RID: 31579
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B5C RID: 31580
		public FsmFloat Delay;

		// Token: 0x04007B5D RID: 31581
		public FsmFloat RepeatDelay;

		// Token: 0x04007B5E RID: 31582
		public FsmBool vibrate;

		// Token: 0x04007B5F RID: 31583
		private float delayLeft;

		// Token: 0x04007B60 RID: 31584
		private float repeatDelayLeft;
	}
}
