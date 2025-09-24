using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D39 RID: 3385
	[ActionCategory("Particle System")]
	[Tooltip("Set particle emission on or off on an object with a particle emitter")]
	public class SetParticleEmissionSpeed : FsmStateAction
	{
		// Token: 0x0600637E RID: 25470 RVA: 0x001F65B1 File Offset: 0x001F47B1
		public override void Reset()
		{
			this.gameObject = null;
			this.emissionSpeed = null;
			this.everyFrame = false;
		}

		// Token: 0x0600637F RID: 25471 RVA: 0x001F65C8 File Offset: 0x001F47C8
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
				this.DoSetEmitSpeed();
				if (!this.everyFrame)
				{
					base.Finish();
				}
			}
		}

		// Token: 0x06006380 RID: 25472 RVA: 0x001F660F File Offset: 0x001F480F
		public override void OnUpdate()
		{
			this.DoSetEmitSpeed();
		}

		// Token: 0x06006381 RID: 25473 RVA: 0x001F6618 File Offset: 0x001F4818
		private void DoSetEmitSpeed()
		{
			this.emitter.main.startSpeedMultiplier = this.emissionSpeed.Value;
		}

		// Token: 0x040061D5 RID: 25045
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061D6 RID: 25046
		public FsmFloat emissionSpeed;

		// Token: 0x040061D7 RID: 25047
		public bool everyFrame;

		// Token: 0x040061D8 RID: 25048
		private ParticleSystem emitter;
	}
}
