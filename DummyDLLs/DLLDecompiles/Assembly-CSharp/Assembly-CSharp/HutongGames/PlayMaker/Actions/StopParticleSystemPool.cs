using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001296 RID: 4758
	public class StopParticleSystemPool : FsmStateAction
	{
		// Token: 0x06007CF8 RID: 31992 RVA: 0x002551D9 File Offset: 0x002533D9
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007CF9 RID: 31993 RVA: 0x002551E4 File Offset: 0x002533E4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				ParticleSystemPool component = safe.GetComponent<ParticleSystemPool>();
				if (component)
				{
					component.StopParticles();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D0D RID: 32013
		public FsmOwnerDefault Target;
	}
}
