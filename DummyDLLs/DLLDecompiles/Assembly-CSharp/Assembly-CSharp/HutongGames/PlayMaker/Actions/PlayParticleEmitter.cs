using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD5 RID: 3285
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitter : FsmStateAction
	{
		// Token: 0x060061E3 RID: 25059 RVA: 0x001EF613 File Offset: 0x001ED813
		public override void Reset()
		{
			this.gameObject = null;
			this.emit = new FsmInt(0);
			this.resetIfPlaying = false;
		}

		// Token: 0x060061E4 RID: 25060 RVA: 0x001EF634 File Offset: 0x001ED834
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
						if (this.emit.Value <= 0)
						{
							if (this.resetIfPlaying && component.isPlaying)
							{
								component.Stop();
							}
							component.Play();
						}
						else
						{
							component.Emit(this.emit.Value);
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006000 RID: 24576
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006001 RID: 24577
		public FsmInt emit;

		// Token: 0x04006002 RID: 24578
		public bool resetIfPlaying;
	}
}
