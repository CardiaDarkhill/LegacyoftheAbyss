using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3C RID: 3388
	[ActionCategory("Particle System")]
	public class SetParticleForceOverTime : FsmStateAction
	{
		// Token: 0x0600638B RID: 25483 RVA: 0x001F6854 File Offset: 0x001F4A54
		public override void Reset()
		{
			this.gameObject = null;
			this.enabled = false;
			this.resetOnExit = false;
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x001F6870 File Offset: 0x001F4A70
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					ownerDefaultTarget.GetComponent<ParticleSystem>().forceOverLifetime.enabled = this.enabled.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x001F68C4 File Offset: 0x001F4AC4
		public override void OnExit()
		{
			if (this.resetOnExit && this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					ownerDefaultTarget.GetComponent<ParticleSystem>().forceOverLifetime.enabled = !this.enabled.Value;
				}
			}
		}

		// Token: 0x040061E3 RID: 25059
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061E4 RID: 25060
		public FsmBool enabled;

		// Token: 0x040061E5 RID: 25061
		public bool resetOnExit;
	}
}
