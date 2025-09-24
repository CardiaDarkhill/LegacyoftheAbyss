using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D81 RID: 3457
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmittersInChildrenV2 : FsmStateAction
	{
		// Token: 0x060064B3 RID: 25779 RVA: 0x001FCAE1 File Offset: 0x001FACE1
		public override void Reset()
		{
			this.Target = null;
			this.StopBehaviour = ParticleSystemStopBehavior.StopEmitting;
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x001FCAF4 File Offset: 0x001FACF4
		public override void OnEnter()
		{
			if (this.Target != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
				if (ownerDefaultTarget != null)
				{
					foreach (ParticleSystem particleSystem in ownerDefaultTarget.GetComponentsInChildren<ParticleSystem>())
					{
						if (particleSystem.isPlaying)
						{
							particleSystem.Stop(true, this.StopBehaviour);
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040063AF RID: 25519
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault Target;

		// Token: 0x040063B0 RID: 25520
		public ParticleSystemStopBehavior StopBehaviour;
	}
}
