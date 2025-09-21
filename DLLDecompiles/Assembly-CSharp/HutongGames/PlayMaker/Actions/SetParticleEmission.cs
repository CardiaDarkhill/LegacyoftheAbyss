using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D36 RID: 3382
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmission : FsmStateAction
	{
		// Token: 0x0600636F RID: 25455 RVA: 0x001F6329 File Offset: 0x001F4529
		public override void Reset()
		{
			this.gameObject = null;
			this.emission = false;
			this.resetOnExit = false;
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x001F6348 File Offset: 0x001F4548
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
						component.emission.enabled = this.emission.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x001F63A8 File Offset: 0x001F45A8
		public override void OnExit()
		{
			if (this.resetOnExit && this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					ownerDefaultTarget.GetComponent<ParticleSystem>().emission.enabled = !this.emission.Value;
				}
			}
		}

		// Token: 0x040061C9 RID: 25033
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061CA RID: 25034
		public FsmBool emission;

		// Token: 0x040061CB RID: 25035
		public bool resetOnExit;
	}
}
