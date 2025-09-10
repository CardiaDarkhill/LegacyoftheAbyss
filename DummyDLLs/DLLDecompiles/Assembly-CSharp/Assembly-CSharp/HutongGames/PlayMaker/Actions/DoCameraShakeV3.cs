using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001213 RID: 4627
	public class DoCameraShakeV3 : FsmStateAction
	{
		// Token: 0x06007AE8 RID: 31464 RVA: 0x0024E088 File Offset: 0x0024C288
		public override void Reset()
		{
			this.VisibleRenderer = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.SpecifyGameObject,
				GameObject = null
			};
			this.Camera = GlobalSettings.Camera.MainCameraShakeManager;
			this.Profile = null;
			this.cancelOnExit = false;
			this.DoFreeze = new FsmBool(true);
			this.Delay = null;
			this.Vibrate = true;
		}

		// Token: 0x06007AE9 RID: 31465 RVA: 0x0024E0F0 File Offset: 0x0024C2F0
		public override void OnEnter()
		{
			this.didShake = false;
			GameObject safe = this.VisibleRenderer.GetSafe(this);
			if (safe)
			{
				Renderer component = safe.GetComponent<Renderer>();
				if (component && !component.isVisible)
				{
					base.Finish();
					return;
				}
			}
			if (this.Delay.Value <= 0f)
			{
				this.DoShake();
				return;
			}
			this.delayLeft = this.Delay.Value;
		}

		// Token: 0x06007AEA RID: 31466 RVA: 0x0024E161 File Offset: 0x0024C361
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

		// Token: 0x06007AEB RID: 31467 RVA: 0x0024E198 File Offset: 0x0024C398
		public override void OnExit()
		{
			if (this.cancelOnExit && this.didShake)
			{
				CameraManagerReference cameraManagerReference = this.Camera.Value as CameraManagerReference;
				CameraShakeProfile cameraShakeProfile = this.Profile.Value as CameraShakeProfile;
				if (cameraManagerReference != null && cameraShakeProfile)
				{
					cameraManagerReference.CancelShake(cameraShakeProfile);
				}
			}
		}

		// Token: 0x06007AEC RID: 31468 RVA: 0x0024E1F0 File Offset: 0x0024C3F0
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
				cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, this.DoFreeze.Value, this.Vibrate.Value, true);
			}
			this.didShake = true;
			base.Finish();
		}

		// Token: 0x04007B3B RID: 31547
		public FsmOwnerDefault VisibleRenderer;

		// Token: 0x04007B3C RID: 31548
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B3D RID: 31549
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B3E RID: 31550
		public bool cancelOnExit;

		// Token: 0x04007B3F RID: 31551
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B40 RID: 31552
		public FsmFloat Delay;

		// Token: 0x04007B41 RID: 31553
		public FsmBool Vibrate;

		// Token: 0x04007B42 RID: 31554
		private float delayLeft;

		// Token: 0x04007B43 RID: 31555
		private bool didShake;
	}
}
