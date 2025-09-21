using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001295 RID: 4757
	public class PlayParticleSystemPool : FsmStateAction
	{
		// Token: 0x06007CF5 RID: 31989 RVA: 0x0025518A File Offset: 0x0025338A
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007CF6 RID: 31990 RVA: 0x00255194 File Offset: 0x00253394
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				ParticleSystemPool component = safe.GetComponent<ParticleSystemPool>();
				if (component)
				{
					component.PlayParticles();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D0C RID: 32012
		public FsmOwnerDefault Target;
	}
}
