using System;
using UnityEngine;

// Token: 0x0200079C RID: 1948
public class WaitForTk2dAnimatorClipFinish : CustomYieldInstruction
{
	// Token: 0x170007B3 RID: 1971
	// (get) Token: 0x060044BE RID: 17598 RVA: 0x0012CDB1 File Offset: 0x0012AFB1
	public override bool keepWaiting
	{
		get
		{
			return !this.hasEnded;
		}
	}

	// Token: 0x060044BF RID: 17599 RVA: 0x0012CDBC File Offset: 0x0012AFBC
	public WaitForTk2dAnimatorClipFinish(tk2dSpriteAnimator animator, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> onCompletion = null)
	{
		this.hasEnded = false;
		this.animator = animator;
		this.doOnCompletion = onCompletion;
		this.currentClip = animator.CurrentClip;
		if (animator.CurrentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once)
		{
			this.onCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
			animator.AnimationCompleted = this.onCompleted;
		}
		else
		{
			this.onFrameChanged = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>(this.OnFrameChanged);
			animator.FrameChanged = this.onFrameChanged;
		}
		animator.AnimationChanged += this.AnimatorOnAnimationChanged;
	}

	// Token: 0x060044C0 RID: 17600 RVA: 0x0012CE4E File Offset: 0x0012B04E
	private void AnimatorOnAnimationChanged(tk2dSpriteAnimator tk2dSpriteAnimator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
	{
		this.Cancel();
	}

	// Token: 0x060044C1 RID: 17601 RVA: 0x0012CE56 File Offset: 0x0012B056
	private void OnFrameChanged(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip, int previousFrame, int currentFrame)
	{
		if (currentFrame != clip.frames.Length - 1 && currentFrame > previousFrame)
		{
			return;
		}
		this.Cancel();
	}

	// Token: 0x060044C2 RID: 17602 RVA: 0x0012CE72 File Offset: 0x0012B072
	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		this.Cancel();
		Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> action = this.doOnCompletion;
		if (action == null)
		{
			return;
		}
		action(anim, clip);
	}

	// Token: 0x060044C3 RID: 17603 RVA: 0x0012CE8C File Offset: 0x0012B08C
	public void Cancel()
	{
		if (this.hasEnded)
		{
			return;
		}
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.FrameChanged = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int>)Delegate.Remove(tk2dSpriteAnimator.FrameChanged, this.onFrameChanged);
		this.onFrameChanged = null;
		tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
		tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator2.AnimationCompleted, this.onCompleted);
		this.onCompleted = null;
		this.animator.AnimationChanged -= this.AnimatorOnAnimationChanged;
		this.hasEnded = true;
	}

	// Token: 0x040045AE RID: 17838
	private bool hasEnded;

	// Token: 0x040045AF RID: 17839
	private tk2dSpriteAnimator animator;

	// Token: 0x040045B0 RID: 17840
	private Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int, int> onFrameChanged;

	// Token: 0x040045B1 RID: 17841
	private Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> onCompleted;

	// Token: 0x040045B2 RID: 17842
	private readonly Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> doOnCompletion;

	// Token: 0x040045B3 RID: 17843
	private tk2dSpriteAnimationClip currentClip;
}
