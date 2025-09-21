using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001312 RID: 4882
	public class CreateNoiseV2 : FsmStateAction
	{
		// Token: 0x06007EC2 RID: 32450 RVA: 0x00259B52 File Offset: 0x00257D52
		public override void Reset()
		{
			this.From = null;
			this.LocalOrigin = null;
			this.Radius = null;
			this.Intensity = null;
		}

		// Token: 0x06007EC3 RID: 32451 RVA: 0x00259B70 File Offset: 0x00257D70
		public override void OnEnter()
		{
			GameObject safe = this.From.GetSafe(this);
			if (safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(this.LocalOrigin.Value), this.Radius.Value, (NoiseMaker.Intensities)this.Intensity.Value, false);
			}
			base.Finish();
		}

		// Token: 0x04007E6D RID: 32365
		public FsmOwnerDefault From;

		// Token: 0x04007E6E RID: 32366
		public FsmVector2 LocalOrigin;

		// Token: 0x04007E6F RID: 32367
		public FsmFloat Radius;

		// Token: 0x04007E70 RID: 32368
		[ObjectType(typeof(NoiseMaker.Intensities))]
		public FsmEnum Intensity;
	}
}
