using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001313 RID: 4883
	public class CreateNoiseContinuous : FsmStateAction
	{
		// Token: 0x06007EC5 RID: 32453 RVA: 0x00259BE1 File Offset: 0x00257DE1
		public override void Reset()
		{
			this.From = null;
			this.LocalOrigin = null;
			this.Radius = null;
			this.Delay = null;
		}

		// Token: 0x06007EC6 RID: 32454 RVA: 0x00259C00 File Offset: 0x00257E00
		public override void OnEnter()
		{
			this.obj = this.From.GetSafe(this);
			if (!this.obj)
			{
				base.Finish();
				return;
			}
			if (this.InitialDelay.Value <= 0f)
			{
				this.Noise();
				this.nextNoiseTime = Time.timeAsDouble + (double)this.Delay.Value;
				return;
			}
			this.nextNoiseTime = Time.timeAsDouble + (double)this.InitialDelay.Value;
		}

		// Token: 0x06007EC7 RID: 32455 RVA: 0x00259C7C File Offset: 0x00257E7C
		public override void OnUpdate()
		{
			if (Time.timeAsDouble < this.nextNoiseTime)
			{
				return;
			}
			this.nextNoiseTime = Time.timeAsDouble + (double)this.Delay.Value;
			this.Noise();
		}

		// Token: 0x06007EC8 RID: 32456 RVA: 0x00259CAA File Offset: 0x00257EAA
		private void Noise()
		{
			NoiseMaker.CreateNoise(this.obj.transform.TransformPoint(this.LocalOrigin.Value), this.Radius.Value, NoiseMaker.Intensities.Normal, false);
		}

		// Token: 0x04007E71 RID: 32369
		public FsmOwnerDefault From;

		// Token: 0x04007E72 RID: 32370
		public FsmVector2 LocalOrigin;

		// Token: 0x04007E73 RID: 32371
		public FsmFloat Radius;

		// Token: 0x04007E74 RID: 32372
		public FsmFloat InitialDelay;

		// Token: 0x04007E75 RID: 32373
		public FsmFloat Delay;

		// Token: 0x04007E76 RID: 32374
		private GameObject obj;

		// Token: 0x04007E77 RID: 32375
		private double nextNoiseTime;
	}
}
