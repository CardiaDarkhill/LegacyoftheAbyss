using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137D RID: 4989
	public class PlayParticleEffectsInState : FsmStateAction
	{
		// Token: 0x06008062 RID: 32866 RVA: 0x0025E4B4 File Offset: 0x0025C6B4
		public override void Reset()
		{
			this.Target = null;
			this.StartDelay = null;
		}

		// Token: 0x06008063 RID: 32867 RVA: 0x0025E4C4 File Offset: 0x0025C6C4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.playEffects = safe.GetComponent<PlayParticleEffects>();
			if (!this.playEffects)
			{
				base.Finish();
				return;
			}
			this.delayTimeLeft = this.StartDelay.Value;
			if (this.delayTimeLeft <= 0f)
			{
				this.playEffects.PlayParticleSystems();
				base.Finish();
			}
		}

		// Token: 0x06008064 RID: 32868 RVA: 0x0025E53C File Offset: 0x0025C73C
		public override void OnUpdate()
		{
			if (this.delayTimeLeft > 0f)
			{
				this.delayTimeLeft -= Time.deltaTime;
				if (this.delayTimeLeft <= 0f)
				{
					this.playEffects.PlayParticleSystems();
					base.Finish();
				}
			}
		}

		// Token: 0x06008065 RID: 32869 RVA: 0x0025E57B File Offset: 0x0025C77B
		public override void OnExit()
		{
			this.playEffects.StopParticleSystems();
		}

		// Token: 0x04007FC8 RID: 32712
		[CheckForComponent(typeof(PlayParticleEffects))]
		public FsmOwnerDefault Target;

		// Token: 0x04007FC9 RID: 32713
		public FsmFloat StartDelay;

		// Token: 0x04007FCA RID: 32714
		private PlayParticleEffects playEffects;

		// Token: 0x04007FCB RID: 32715
		private float delayTimeLeft;
	}
}
