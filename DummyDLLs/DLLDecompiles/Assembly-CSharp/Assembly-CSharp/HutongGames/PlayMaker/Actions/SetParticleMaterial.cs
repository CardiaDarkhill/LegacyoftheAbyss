using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3D RID: 3389
	[ActionCategory("Particle System")]
	public class SetParticleMaterial : FsmStateAction
	{
		// Token: 0x0600638F RID: 25487 RVA: 0x001F6925 File Offset: 0x001F4B25
		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x001F6938 File Offset: 0x001F4B38
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					ownerDefaultTarget.GetComponent<ParticleSystemRenderer>().material = this.material.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x040061E6 RID: 25062
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061E7 RID: 25063
		public FsmMaterial material;

		// Token: 0x040061E8 RID: 25064
		private ParticleSystem emitter;
	}
}
