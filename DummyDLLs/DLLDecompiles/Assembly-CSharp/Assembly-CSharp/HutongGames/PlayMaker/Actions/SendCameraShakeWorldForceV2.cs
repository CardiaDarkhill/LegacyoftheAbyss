using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121B RID: 4635
	public class SendCameraShakeWorldForceV2 : FsmStateAction
	{
		// Token: 0x06007B0C RID: 31500 RVA: 0x0024EA8B File Offset: 0x0024CC8B
		public override void Reset()
		{
			this.Camera = GlobalSettings.Camera.MainCameraShakeManager;
			this.WorldForce = null;
			this.EveryFrame = null;
			this.Rate = 0.1f;
		}

		// Token: 0x06007B0D RID: 31501 RVA: 0x0024EABC File Offset: 0x0024CCBC
		public override void OnEnter()
		{
			this.camera = (this.Camera.Value as CameraManagerReference);
			this.hasReference = (this.camera != null);
			if (this.hasReference)
			{
				this.camera.SendWorldForce((CameraShakeWorldForceIntensities)this.WorldForce.Value);
			}
			if (!this.EveryFrame.Value || !this.hasReference)
			{
				base.Finish();
			}
		}

		// Token: 0x06007B0E RID: 31502 RVA: 0x0024EB2F File Offset: 0x0024CD2F
		public override void OnUpdate()
		{
			if (!this.hasReference)
			{
				base.Finish();
				return;
			}
			this.camera.SendWorldShaking((CameraShakeWorldForceIntensities)this.WorldForce.Value);
		}

		// Token: 0x04007B6B RID: 31595
		[ObjectType(typeof(CameraManagerReference))]
		[RequiredField]
		public FsmObject Camera;

		// Token: 0x04007B6C RID: 31596
		[ObjectType(typeof(CameraShakeWorldForceIntensities))]
		public FsmEnum WorldForce;

		// Token: 0x04007B6D RID: 31597
		public FsmBool EveryFrame;

		// Token: 0x04007B6E RID: 31598
		public FsmFloat Rate;

		// Token: 0x04007B6F RID: 31599
		private CameraManagerReference camera;

		// Token: 0x04007B70 RID: 31600
		private bool hasReference;
	}
}
