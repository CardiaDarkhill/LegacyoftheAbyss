using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001254 RID: 4692
	public class HeroClampFallVelocityV2 : FsmStateAction
	{
		// Token: 0x06007BF6 RID: 31734 RVA: 0x002511F6 File Offset: 0x0024F3F6
		public override void Reset()
		{
			this.everyFrame = null;
		}

		// Token: 0x06007BF7 RID: 31735 RVA: 0x00251200 File Offset: 0x0024F400
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			if (!this.hc)
			{
				base.Finish();
				return;
			}
			this.hc = this.hc.GetComponent<HeroController>();
			this.body = this.hc.Body;
			this.OnUpdate();
			if (!this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007BF8 RID: 31736 RVA: 0x00251267 File Offset: 0x0024F467
		public override void OnExit()
		{
			this.hc = null;
			this.body = null;
		}

		// Token: 0x06007BF9 RID: 31737 RVA: 0x00251278 File Offset: 0x0024F478
		public override void OnUpdate()
		{
			Vector2 linearVelocity = this.body.linearVelocity;
			float num = -this.hc.GetMaxFallVelocity();
			if (linearVelocity.y < num)
			{
				this.body.linearVelocity = new Vector2(linearVelocity.x, num);
			}
		}

		// Token: 0x04007C1F RID: 31775
		public FsmBool everyFrame;

		// Token: 0x04007C20 RID: 31776
		private HeroController hc;

		// Token: 0x04007C21 RID: 31777
		private Rigidbody2D body;
	}
}
