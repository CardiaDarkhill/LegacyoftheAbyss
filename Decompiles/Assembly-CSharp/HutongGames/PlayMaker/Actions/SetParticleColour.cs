using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D35 RID: 3381
	[ActionCategory("Particle System")]
	public class SetParticleColour : FsmStateAction
	{
		// Token: 0x0600636C RID: 25452 RVA: 0x001F62AA File Offset: 0x001F44AA
		public override void Reset()
		{
			this.gameObject = null;
			this.colour = null;
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x001F62BC File Offset: 0x001F44BC
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					this.emitter.main.startColor = this.colour.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x040061C6 RID: 25030
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061C7 RID: 25031
		public FsmColor colour;

		// Token: 0x040061C8 RID: 25032
		private ParticleSystem emitter;
	}
}
