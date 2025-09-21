using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001229 RID: 4649
	public class SetCamLock : FsmStateAction
	{
		// Token: 0x06007B35 RID: 31541 RVA: 0x0024EFE8 File Offset: 0x0024D1E8
		public override void Reset()
		{
			this.xLockMin = new FsmFloat
			{
				UseVariable = true
			};
			this.xLockMax = new FsmFloat
			{
				UseVariable = true
			};
			this.yLockMin = new FsmFloat
			{
				UseVariable = true
			};
			this.yLockMax = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06007B36 RID: 31542 RVA: 0x0024F044 File Offset: 0x0024D244
		public override void OnEnter()
		{
			this.camTarget = GameCameras.instance.cameraTarget.GetComponent<CameraTarget>();
			this.DoSetCamLock();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007B37 RID: 31543 RVA: 0x0024F06F File Offset: 0x0024D26F
		public override void OnUpdate()
		{
			this.DoSetCamLock();
		}

		// Token: 0x06007B38 RID: 31544 RVA: 0x0024F078 File Offset: 0x0024D278
		public void DoSetCamLock()
		{
			if (!this.xLockMin.IsNone)
			{
				this.camTarget.xLockMin = this.xLockMin.Value;
			}
			if (!this.xLockMax.IsNone)
			{
				this.camTarget.xLockMax = this.xLockMax.Value;
			}
			if (!this.yLockMin.IsNone)
			{
				this.camTarget.yLockMin = this.yLockMin.Value;
			}
			if (!this.yLockMax.IsNone)
			{
				this.camTarget.yLockMax = this.yLockMax.Value;
			}
		}

		// Token: 0x04007B7F RID: 31615
		public FsmFloat xLockMin;

		// Token: 0x04007B80 RID: 31616
		public FsmFloat xLockMax;

		// Token: 0x04007B81 RID: 31617
		public FsmFloat yLockMin;

		// Token: 0x04007B82 RID: 31618
		public FsmFloat yLockMax;

		// Token: 0x04007B83 RID: 31619
		public bool everyFrame;

		// Token: 0x04007B84 RID: 31620
		private CameraTarget camTarget;
	}
}
