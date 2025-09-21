using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D38 RID: 3384
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmissionRate : FsmStateAction
	{
		// Token: 0x06006379 RID: 25465 RVA: 0x001F64FD File Offset: 0x001F46FD
		public override void Reset()
		{
			this.gameObject = null;
			this.emissionRate = null;
			this.everyFrame = false;
		}

		// Token: 0x0600637A RID: 25466 RVA: 0x001F6514 File Offset: 0x001F4714
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget)
				{
					this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
				}
				this.DoSetEmitRate();
				if (!this.everyFrame)
				{
					base.Finish();
				}
			}
		}

		// Token: 0x0600637B RID: 25467 RVA: 0x001F6563 File Offset: 0x001F4763
		public override void OnUpdate()
		{
			this.DoSetEmitRate();
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x001F656C File Offset: 0x001F476C
		private void DoSetEmitRate()
		{
			if (this.emitter)
			{
				this.emitter.emission.rateOverTime = this.emissionRate.Value;
			}
		}

		// Token: 0x040061D1 RID: 25041
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061D2 RID: 25042
		public FsmFloat emissionRate;

		// Token: 0x040061D3 RID: 25043
		public bool everyFrame;

		// Token: 0x040061D4 RID: 25044
		private ParticleSystem emitter;
	}
}
