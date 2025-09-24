using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD8 RID: 3288
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterInState : FsmStateAction
	{
		// Token: 0x060061EF RID: 25071 RVA: 0x001EF8DB File Offset: 0x001EDADB
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x001EF8E4 File Offset: 0x001EDAE4
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					ParticleSystem component = ownerDefaultTarget.GetComponent<ParticleSystem>();
					if (component)
					{
						component.Play();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x060061F1 RID: 25073 RVA: 0x001EF930 File Offset: 0x001EDB30
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
		}

		// Token: 0x0400600B RID: 24587
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;
	}
}
