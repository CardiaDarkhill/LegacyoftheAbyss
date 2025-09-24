using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7E RID: 3454
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class StopParticleEmitter : FsmStateAction
	{
		// Token: 0x060064AA RID: 25770 RVA: 0x001FC9A9 File Offset: 0x001FABA9
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060064AB RID: 25771 RVA: 0x001FC9B4 File Offset: 0x001FABB4
		public override void OnEnter()
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

		// Token: 0x040063AC RID: 25516
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;
	}
}
