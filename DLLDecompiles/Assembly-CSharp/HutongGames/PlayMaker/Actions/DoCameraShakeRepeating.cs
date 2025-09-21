using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001215 RID: 4629
	public class DoCameraShakeRepeating : FsmStateAction
	{
		// Token: 0x06007AF4 RID: 31476 RVA: 0x0024E47C File Offset: 0x0024C67C
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
		}

		// Token: 0x06007AF5 RID: 31477 RVA: 0x0024E4D8 File Offset: 0x0024C6D8
		public override void OnEnter()
		{
			if (this.Delay.Value <= 0f)
			{
				this.DoShake();
				return;
			}
			this.delayLeft = this.Delay.Value;
		}

		// Token: 0x06007AF6 RID: 31478 RVA: 0x0024E504 File Offset: 0x0024C704
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

		// Token: 0x06007AF7 RID: 31479 RVA: 0x0024E578 File Offset: 0x0024C778
		private void DoShake()
		{
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
			this.repeatDelayLeft = this.RepeatDelay.Value;
		}

		// Token: 0x04007B4E RID: 31566
		public FsmOwnerDefault Target;

		// Token: 0x04007B4F RID: 31567
		public FsmVector2 MaxCameraDistance;

		// Token: 0x04007B50 RID: 31568
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B51 RID: 31569
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B52 RID: 31570
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B53 RID: 31571
		public FsmFloat Delay;

		// Token: 0x04007B54 RID: 31572
		public FsmFloat RepeatDelay;

		// Token: 0x04007B55 RID: 31573
		private float delayLeft;

		// Token: 0x04007B56 RID: 31574
		private float repeatDelayLeft;
	}
}
