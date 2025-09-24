using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D80 RID: 3456
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmittersInChildren : FsmStateAction
	{
		// Token: 0x060064B0 RID: 25776 RVA: 0x001FCA73 File Offset: 0x001FAC73
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x001FCA7C File Offset: 0x001FAC7C
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (ParticleSystem particleSystem in ownerDefaultTarget.GetComponentsInChildren<ParticleSystem>())
					{
						if (particleSystem.isPlaying)
						{
							particleSystem.Stop();
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040063AE RID: 25518
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;
	}
}
