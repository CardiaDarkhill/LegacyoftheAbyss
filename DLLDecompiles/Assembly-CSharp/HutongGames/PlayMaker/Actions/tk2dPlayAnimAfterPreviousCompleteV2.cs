using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8F RID: 3471
	public class tk2dPlayAnimAfterPreviousCompleteV2 : FsmStateAction
	{
		// Token: 0x060064FB RID: 25851 RVA: 0x001FDD94 File Offset: 0x001FBF94
		public override void Reset()
		{
			this.Target = null;
			this.AnimName = null;
			this.RandomFrame = null;
			this.StoreDidPlay = null;
		}

		// Token: 0x060064FC RID: 25852 RVA: 0x001FDDB4 File Offset: 0x001FBFB4
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

		// Token: 0x060064FD RID: 25853 RVA: 0x001FDE29 File Offset: 0x001FC029
		public override void OnExit()
		{
			this.ClearEvent(ref this.animator);
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x001FDE37 File Offset: 0x001FC037
		private void ClearEvent(ref tk2dSpriteAnimator animator)
		{
			if (animator)
			{
				tk2dSpriteAnimator tk2dSpriteAnimator = animator;
				tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
				animator = null;
			}
		}

		// Token: 0x060064FF RID: 25855 RVA: 0x001FDE68 File Offset: 0x001FC068
		private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
		{
			if (string.IsNullOrEmpty(this.AnimName.Value))
			{
				return;
			}
			tk2dSpriteAnimationClip clipByName = animator.GetClipByName(this.AnimName.Value);
			if (this.RandomFrame.Value)
			{
				animator.PlayFromFrame(clipByName, Random.Range(0, clip.frames.Length));
			}
			else
			{
				animator.Play(clipByName);
			}
			this.StoreDidPlay.Value = true;
			this.ClearEvent(ref animator);
			base.Finish();
		}

		// Token: 0x040063FB RID: 25595
		public FsmOwnerDefault Target;

		// Token: 0x040063FC RID: 25596
		public FsmString AnimName;

		// Token: 0x040063FD RID: 25597
		public FsmBool RandomFrame;

		// Token: 0x040063FE RID: 25598
		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidPlay;

		// Token: 0x040063FF RID: 25599
		private tk2dSpriteAnimator animator;
	}
}
