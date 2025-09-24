using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001211 RID: 4625
	public class DoCameraShake : FsmStateAction
	{
		// Token: 0x06007ADC RID: 31452 RVA: 0x0024DCC8 File Offset: 0x0024BEC8
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
		}

		// Token: 0x06007ADD RID: 31453 RVA: 0x0024DD24 File Offset: 0x0024BF24
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

		// Token: 0x06007ADE RID: 31454 RVA: 0x0024DD95 File Offset: 0x0024BF95
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

		// Token: 0x06007ADF RID: 31455 RVA: 0x0024DDCC File Offset: 0x0024BFCC
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

		// Token: 0x06007AE0 RID: 31456 RVA: 0x0024DE24 File Offset: 0x0024C024
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
				cameraManagerReference.DoShake(cameraShakeProfile, base.Owner, this.DoFreeze.Value, true, true);
			}
			this.didShake = true;
			base.Finish();
		}

		// Token: 0x04007B2A RID: 31530
		public FsmOwnerDefault VisibleRenderer;

		// Token: 0x04007B2B RID: 31531
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B2C RID: 31532
		[ObjectType(typeof(CameraShakeProfile))]
		[RequiredField]
		public FsmObject Profile;

		// Token: 0x04007B2D RID: 31533
		public bool cancelOnExit;

		// Token: 0x04007B2E RID: 31534
		[RequiredField]
		public FsmBool DoFreeze;

		// Token: 0x04007B2F RID: 31535
		public FsmFloat Delay;

		// Token: 0x04007B30 RID: 31536
		private float delayLeft;

		// Token: 0x04007B31 RID: 31537
		private bool didShake;
	}
}
