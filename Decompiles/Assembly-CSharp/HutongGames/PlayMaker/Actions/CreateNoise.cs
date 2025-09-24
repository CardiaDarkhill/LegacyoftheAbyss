using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001311 RID: 4881
	public class CreateNoise : FsmStateAction
	{
		// Token: 0x06007EBF RID: 32447 RVA: 0x00259AD9 File Offset: 0x00257CD9
		public override void Reset()
		{
			this.From = null;
			this.LocalOrigin = null;
			this.Radius = null;
		}

		// Token: 0x06007EC0 RID: 32448 RVA: 0x00259AF0 File Offset: 0x00257CF0
		public override void OnEnter()
		{
			GameObject safe = this.From.GetSafe(this);
			if (safe)
			{
				NoiseMaker.CreateNoise(safe.transform.TransformPoint(this.LocalOrigin.Value), this.Radius.Value, NoiseMaker.Intensities.Normal, false);
			}
			base.Finish();
		}

		// Token: 0x04007E6A RID: 32362
		public FsmOwnerDefault From;

		// Token: 0x04007E6B RID: 32363
		public FsmVector2 LocalOrigin;

		// Token: 0x04007E6C RID: 32364
		public FsmFloat Radius;
	}
}
