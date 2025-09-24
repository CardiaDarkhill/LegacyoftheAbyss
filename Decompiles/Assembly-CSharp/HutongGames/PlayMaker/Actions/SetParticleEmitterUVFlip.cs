using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3B RID: 3387
	[ActionCategory("Particle System")]
	public class SetParticleEmitterUVFlip : FsmStateAction
	{
		// Token: 0x06006388 RID: 25480 RVA: 0x001F67A9 File Offset: 0x001F49A9
		public override void Reset()
		{
			this.gameObject = null;
			this.flipU = null;
			this.flipV = null;
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x001F67C0 File Offset: 0x001F49C0
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.emitter = ownerDefaultTarget.GetComponent<ParticleSystem>();
					ParticleSystem.TextureSheetAnimationModule textureSheetAnimation = this.emitter.textureSheetAnimation;
					if (!this.flipU.IsNone)
					{
						textureSheetAnimation.flipU = this.flipU.Value;
					}
					if (!this.flipV.IsNone)
					{
						textureSheetAnimation.flipV = this.flipV.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040061DF RID: 25055
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040061E0 RID: 25056
		public FsmFloat flipU;

		// Token: 0x040061E1 RID: 25057
		public FsmFloat flipV;

		// Token: 0x040061E2 RID: 25058
		private ParticleSystem emitter;
	}
}
