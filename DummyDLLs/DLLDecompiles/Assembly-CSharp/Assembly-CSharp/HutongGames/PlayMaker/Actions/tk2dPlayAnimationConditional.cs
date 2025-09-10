using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D92 RID: 3474
	public class tk2dPlayAnimationConditional : FsmStateAction
	{
		// Token: 0x06006507 RID: 25863 RVA: 0x001FE05C File Offset: 0x001FC25C
		public override void Reset()
		{
			this.Target = null;
			this.AnimName = null;
			this.Condition = null;
			this.EveryFrame = true;
		}

		// Token: 0x06006508 RID: 25864 RVA: 0x001FE07C File Offset: 0x001FC27C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.animator = safe.GetComponent<tk2dSpriteAnimator>();
				this.heroAnim = safe.GetComponent<IHeroAnimationController>();
			}
			if (!this.animator)
			{
				base.Finish();
				return;
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006509 RID: 25865 RVA: 0x001FE0DE File Offset: 0x001FC2DE
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600650A RID: 25866 RVA: 0x001FE0E8 File Offset: 0x001FC2E8
		private void DoAction()
		{
			if (!this.Condition.Value)
			{
				return;
			}
			if (this.heroAnim != null)
			{
				this.animator.Play(this.heroAnim.GetClip(this.AnimName.Value));
				return;
			}
			this.animator.Play(this.AnimName.Value);
		}

		// Token: 0x04006404 RID: 25604
		public FsmOwnerDefault Target;

		// Token: 0x04006405 RID: 25605
		public FsmString AnimName;

		// Token: 0x04006406 RID: 25606
		public FsmBool Condition;

		// Token: 0x04006407 RID: 25607
		public bool EveryFrame;

		// Token: 0x04006408 RID: 25608
		private tk2dSpriteAnimator animator;

		// Token: 0x04006409 RID: 25609
		private IHeroAnimationController heroAnim;
	}
}
