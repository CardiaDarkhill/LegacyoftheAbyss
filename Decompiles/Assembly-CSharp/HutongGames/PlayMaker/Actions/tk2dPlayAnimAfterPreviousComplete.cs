using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8E RID: 3470
	public class tk2dPlayAnimAfterPreviousComplete : FsmStateAction
	{
		// Token: 0x060064F5 RID: 25845 RVA: 0x001FDC81 File Offset: 0x001FBE81
		public override void Reset()
		{
			this.Target = null;
			this.AnimName = null;
			this.StoreDidPlay = null;
		}

		// Token: 0x060064F6 RID: 25846 RVA: 0x001FDC98 File Offset: 0x001FBE98
		public override void OnEnter()
		{
			this.StoreDidPlay.Value = false;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.animator = safe.GetComponent<tk2dSpriteAnimator>();
				if (this.animator)
				{
					tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
					tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x060064F7 RID: 25847 RVA: 0x001FDD0D File Offset: 0x001FBF0D
		public override void OnExit()
		{
			this.ClearEvent(ref this.animator);
		}

		// Token: 0x060064F8 RID: 25848 RVA: 0x001FDD1B File Offset: 0x001FBF1B
		private void ClearEvent(ref tk2dSpriteAnimator animator)
		{
			if (animator)
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = animator;
				tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
				animator = null;
			}
		}

		// Token: 0x060064F9 RID: 25849 RVA: 0x001FDD4C File Offset: 0x001FBF4C
		private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
		{
			if (string.IsNullOrEmpty(this.AnimName.Value))
			{
				return;
			}
			animator.Play(this.AnimName.Value);
			this.StoreDidPlay.Value = true;
			this.ClearEvent(ref animator);
			base.Finish();
		}

		// Token: 0x040063F7 RID: 25591
		public FsmOwnerDefault Target;

		// Token: 0x040063F8 RID: 25592
		public FsmString AnimName;

		// Token: 0x040063F9 RID: 25593
		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidPlay;

		// Token: 0x040063FA RID: 25594
		private tk2dSpriteAnimator animator;
	}
}
