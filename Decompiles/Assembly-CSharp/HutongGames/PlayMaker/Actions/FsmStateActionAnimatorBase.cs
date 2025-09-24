using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE1 RID: 3553
	public abstract class FsmStateActionAnimatorBase : ComponentAction<Animator>
	{
		// Token: 0x060066B8 RID: 26296
		public abstract void OnActionUpdate();

		// Token: 0x060066B9 RID: 26297 RVA: 0x00208423 File Offset: 0x00206623
		public override void Reset()
		{
			this.everyFrame = false;
			this.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnUpdate;
		}

		// Token: 0x060066BA RID: 26298 RVA: 0x00208433 File Offset: 0x00206633
		public override void OnPreprocess()
		{
			if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorMove)
			{
				base.Fsm.HandleAnimatorMove = true;
			}
			if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				base.Fsm.HandleAnimatorIK = true;
			}
		}

		// Token: 0x060066BB RID: 26299 RVA: 0x0020845F File Offset: 0x0020665F
		public override void OnUpdate()
		{
			if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066BC RID: 26300 RVA: 0x0020847D File Offset: 0x0020667D
		public override void DoAnimatorMove()
		{
			if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorMove)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066BD RID: 26301 RVA: 0x0020849C File Offset: 0x0020669C
		public override void DoAnimatorIK(int layerIndex)
		{
			this.IklayerIndex = layerIndex;
			if (this.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0400660F RID: 26127
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006610 RID: 26128
		[Tooltip("Select when to perform the action, during OnUpdate, OnAnimatorMove, OnAnimatorIK")]
		public FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector everyFrameOption;

		// Token: 0x04006611 RID: 26129
		protected int IklayerIndex;

		// Token: 0x02001B9A RID: 7066
		public enum AnimatorFrameUpdateSelector
		{
			// Token: 0x04009DDC RID: 40412
			OnUpdate,
			// Token: 0x04009DDD RID: 40413
			OnAnimatorMove,
			// Token: 0x04009DDE RID: 40414
			OnAnimatorIK
		}
	}
}
