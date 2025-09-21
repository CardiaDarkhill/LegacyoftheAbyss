using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFB RID: 3323
	public class RendererIsVisible : ComponentAction<Renderer>
	{
		// Token: 0x0600627F RID: 25215 RVA: 0x001F2900 File Offset: 0x001F0B00
		public bool IsNotEveryFrame()
		{
			return !this.EveryFrame;
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x001F290B File Offset: 0x001F0B0B
		public override void Reset()
		{
			this.Target = null;
			this.LostVisibilityTime = null;
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x001F291C File Offset: 0x001F0B1C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe == null)
			{
				base.Finish();
				return;
			}
			if (base.UpdateCache(safe))
			{
				this.renderer = this.cachedComponent;
				this.lostVisibilityTimeLeft = (this.EveryFrame ? this.LostVisibilityTime.Value : 0f);
				this.Evaluate();
			}
			else
			{
				base.Finish();
			}
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006282 RID: 25218 RVA: 0x001F2997 File Offset: 0x001F0B97
		public override void OnUpdate()
		{
			this.Evaluate();
		}

		// Token: 0x06006283 RID: 25219 RVA: 0x001F29A0 File Offset: 0x001F0BA0
		private void Evaluate()
		{
			if (this.renderer.isVisible)
			{
				base.Fsm.Event(this.VisibleEvent);
				this.lostVisibilityTimeLeft = this.LostVisibilityTime.Value;
				return;
			}
			this.lostVisibilityTimeLeft -= Time.deltaTime;
			if (this.lostVisibilityTimeLeft <= 0f)
			{
				base.Fsm.Event(this.NotVisibleEvent);
			}
		}

		// Token: 0x040060EA RID: 24810
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault Target;

		// Token: 0x040060EB RID: 24811
		[HideIf("IsNotEveryFrame")]
		public FsmFloat LostVisibilityTime;

		// Token: 0x040060EC RID: 24812
		public FsmEvent VisibleEvent;

		// Token: 0x040060ED RID: 24813
		public FsmEvent NotVisibleEvent;

		// Token: 0x040060EE RID: 24814
		public bool EveryFrame;

		// Token: 0x040060EF RID: 24815
		private float lostVisibilityTimeLeft;

		// Token: 0x040060F0 RID: 24816
		private new Renderer renderer;
	}
}
