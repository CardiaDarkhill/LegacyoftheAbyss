using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD6 RID: 3286
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterChildren : FsmStateAction
	{
		// Token: 0x060061E6 RID: 25062 RVA: 0x001EF6BE File Offset: 0x001ED8BE
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060061E7 RID: 25063 RVA: 0x001EF6C8 File Offset: 0x001ED8C8
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (object obj in ownerDefaultTarget.transform)
					{
						ParticleSystem component = ((Transform)obj).GetComponent<ParticleSystem>();
						if (component)
						{
							component.Play();
							if (this.resetTimeIfPlaying)
							{
								component.time = 0f;
							}
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x060061E8 RID: 25064 RVA: 0x001EF76C File Offset: 0x001ED96C
		public override void OnExit()
		{
			if (this.gameObject != null && this.stopOnStateExit)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (object obj in ownerDefaultTarget.transform)
					{
						ParticleSystem component = ((Transform)obj).GetComponent<ParticleSystem>();
						if (component)
						{
							component.Stop();
						}
					}
				}
			}
		}

		// Token: 0x04006003 RID: 24579
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006004 RID: 24580
		public bool resetTimeIfPlaying;

		// Token: 0x04006005 RID: 24581
		public bool stopOnStateExit;
	}
}
