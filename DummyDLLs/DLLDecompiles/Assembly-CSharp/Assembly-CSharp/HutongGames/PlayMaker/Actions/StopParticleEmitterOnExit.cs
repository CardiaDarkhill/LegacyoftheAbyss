using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7F RID: 3455
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmitterOnExit : FsmStateAction
	{
		// Token: 0x060064AD RID: 25773 RVA: 0x001FCA0F File Offset: 0x001FAC0F
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x001FCA18 File Offset: 0x001FAC18
		public override void OnExit()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
					if (component && component.isPlaying)
					{
						component.Stop();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040063AD RID: 25517
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;
	}
}
