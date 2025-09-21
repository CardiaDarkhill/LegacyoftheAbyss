using System;
using GlobalSettings;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001219 RID: 4633
	public class CameraRumbleSequence : FsmStateAction
	{
		// Token: 0x06007B04 RID: 31492 RVA: 0x0024E937 File Offset: 0x0024CB37
		public override void Reset()
		{
			this.StartDelay = null;
			this.Rumble = new FsmCameraShakeTarget
			{
				Camera = GlobalSettings.Camera.MainCameraShakeManager
			};
			this.EndShake = new FsmCameraShakeTarget
			{
				Camera = GlobalSettings.Camera.MainCameraShakeManager
			};
		}

		// Token: 0x06007B05 RID: 31493 RVA: 0x0024E976 File Offset: 0x0024CB76
		public override void OnEnter()
		{
			this.delayTimeLeft = this.StartDelay.Value;
			if (this.delayTimeLeft <= 0f)
			{
				this.Rumble.DoShake(base.Owner);
				base.Finish();
			}
		}

		// Token: 0x06007B06 RID: 31494 RVA: 0x0024E9B0 File Offset: 0x0024CBB0
		public override void OnUpdate()
		{
			if (this.delayTimeLeft > 0f)
			{
				this.delayTimeLeft -= Time.deltaTime;
				if (this.delayTimeLeft <= 0f)
				{
					this.Rumble.DoShake(base.Owner);
					base.Finish();
				}
			}
		}

		// Token: 0x06007B07 RID: 31495 RVA: 0x0024EA00 File Offset: 0x0024CC00
		public override void OnExit()
		{
			this.Rumble.CancelShake();
			this.EndShake.DoShake(base.Owner);
		}

		// Token: 0x04007B65 RID: 31589
		public FsmFloat StartDelay;

		// Token: 0x04007B66 RID: 31590
		public FsmCameraShakeTarget Rumble;

		// Token: 0x04007B67 RID: 31591
		public FsmCameraShakeTarget EndShake;

		// Token: 0x04007B68 RID: 31592
		private float delayTimeLeft;
	}
}
