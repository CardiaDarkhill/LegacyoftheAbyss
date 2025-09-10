using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001314 RID: 4884
	public class CreateNoiseRect : FsmStateAction
	{
		// Token: 0x06007ECA RID: 32458 RVA: 0x00259CEB File Offset: 0x00257EEB
		public override void Reset()
		{
			this.From = null;
			this.LocalOrigin = null;
			this.Size = null;
			this.Intensity = null;
		}

		// Token: 0x06007ECB RID: 32459 RVA: 0x00259D0C File Offset: 0x00257F0C
		public override void OnEnter()
		{
			GameObject safe = this.From.GetSafe(this);
			if (safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(this.LocalOrigin.Value), this.Size.Value, (NoiseMaker.Intensities)this.Intensity.Value, false);
			}
			base.Finish();
		}

		// Token: 0x04007E78 RID: 32376
		public FsmOwnerDefault From;

		// Token: 0x04007E79 RID: 32377
		public FsmVector2 LocalOrigin;

		// Token: 0x04007E7A RID: 32378
		public FsmVector2 Size;

		// Token: 0x04007E7B RID: 32379
		[ObjectType(typeof(NoiseMaker.Intensities))]
		public FsmEnum Intensity;
	}
}
