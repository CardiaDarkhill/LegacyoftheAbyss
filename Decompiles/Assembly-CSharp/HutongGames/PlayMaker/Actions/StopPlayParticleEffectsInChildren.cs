using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137E RID: 4990
	public class StopPlayParticleEffectsInChildren : FsmStateAction
	{
		// Token: 0x06008067 RID: 32871 RVA: 0x0025E590 File Offset: 0x0025C790
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06008068 RID: 32872 RVA: 0x0025E59C File Offset: 0x0025C79C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				PlayParticleEffects[] componentsInChildren = safe.GetComponentsInChildren<PlayParticleEffects>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].StopParticleSystems();
				}
			}
			base.Finish();
		}

		// Token: 0x04007FCC RID: 32716
		public FsmOwnerDefault Target;
	}
}
