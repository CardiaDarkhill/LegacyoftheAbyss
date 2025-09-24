using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD7 RID: 3287
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class PlayParticleEmitterConditional : FsmStateAction
	{
		// Token: 0x060061EA RID: 25066 RVA: 0x001EF804 File Offset: 0x001EDA04
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x001EF810 File Offset: 0x001EDA10
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					if (!this.emitter)
					{
						base.Finish();
					}
				}
			}
		}

		// Token: 0x060061EC RID: 25068 RVA: 0x001EF860 File Offset: 0x001EDA60
		public override void OnUpdate()
		{
			this.playing = this.emitter.isPlaying;
			if (!this.playing && this.conditionalBool.Value)
			{
				this.emitter.Play();
			}
			if (this.playing && !this.conditionalBool.Value)
			{
				this.emitter.Stop();
			}
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x001EF8BE File Offset: 0x001EDABE
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.emitter.Stop();
			}
		}

		// Token: 0x04006006 RID: 24582
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006007 RID: 24583
		public FsmBool conditionalBool;

		// Token: 0x04006008 RID: 24584
		public bool stopOnExit;

		// Token: 0x04006009 RID: 24585
		private ParticleSystem emitter;

		// Token: 0x0400600A RID: 24586
		private bool playing;
	}
}
