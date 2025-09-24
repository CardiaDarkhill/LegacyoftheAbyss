using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001253 RID: 4691
	public class HeroClampFallVelocity : FsmStateAction
	{
		// Token: 0x06007BF1 RID: 31729 RVA: 0x00251145 File Offset: 0x0024F345
		public override void Reset()
		{
			this.Hero = null;
		}

		// Token: 0x06007BF2 RID: 31730 RVA: 0x00251150 File Offset: 0x0024F350
		public override void OnEnter()
		{
			GameObject safe = this.Hero.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.hc = safe.GetComponent<HeroController>();
			this.body = this.hc.Body;
		}

		// Token: 0x06007BF3 RID: 31731 RVA: 0x00251196 File Offset: 0x0024F396
		public override void OnExit()
		{
			this.hc = null;
			this.body = null;
		}

		// Token: 0x06007BF4 RID: 31732 RVA: 0x002511A8 File Offset: 0x0024F3A8
		public override void OnUpdate()
		{
			Vector2 linearVelocity = this.body.linearVelocity;
			float num = -this.hc.GetMaxFallVelocity();
			if (linearVelocity.y < num)
			{
				this.body.linearVelocity = new Vector2(linearVelocity.x, num);
			}
		}

		// Token: 0x04007C1C RID: 31772
		public FsmOwnerDefault Hero;

		// Token: 0x04007C1D RID: 31773
		private HeroController hc;

		// Token: 0x04007C1E RID: 31774
		private Rigidbody2D body;
	}
}
