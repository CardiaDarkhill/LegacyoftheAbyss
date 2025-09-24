using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD9 RID: 3289
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterInStateV2 : FsmStateAction
	{
		// Token: 0x060061F3 RID: 25075 RVA: 0x001EF985 File Offset: 0x001EDB85
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060061F4 RID: 25076 RVA: 0x001EF990 File Offset: 0x001EDB90
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

		// Token: 0x060061F5 RID: 25077 RVA: 0x001EF9DC File Offset: 0x001EDBDC
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
						component.Stop(this.withChildren.Value, (ParticleSystemStopBehavior)this.stopAction.Value);
					}
				}
			}
		}

		// Token: 0x0400600C RID: 24588
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400600D RID: 24589
		public FsmBool withChildren;

		// Token: 0x0400600E RID: 24590
		[ObjectType(typeof(ParticleSystemStopBehavior))]
		public FsmEnum stopAction;
	}
}
