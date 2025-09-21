using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class tk2dAnimationTriggerFlipScale : MonoBehaviour
{
	// Token: 0x06000655 RID: 1621 RVA: 0x00020826 File Offset: 0x0001EA26
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00020834 File Offset: 0x0001EA34
	private void OnEnable()
	{
		this.initialScale = base.transform.localScale;
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
		tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x000208A0 File Offset: 0x0001EAA0
	private void OnDisable()
	{
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
		tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x000208FC File Offset: 0x0001EAFC
	private void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIndex)
	{
		string eventInfo = clip.GetFrame(frameIndex).eventInfo;
		if (!string.IsNullOrEmpty(eventInfo) && !string.IsNullOrEmpty(this.triggerInfoContains) && !eventInfo.Contains(this.triggerInfoContains))
		{
			return;
		}
		Vector3 localScale = base.transform.localScale;
		if (this.flipX)
		{
			localScale.x *= -1f;
		}
		if (this.flipY)
		{
			localScale.y *= -1f;
		}
		base.transform.localScale = localScale;
		this.hasTriggered = true;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00020989 File Offset: 0x0001EB89
	private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (!this.hasTriggered)
		{
			return;
		}
		this.hasTriggered = false;
		if (this.resetOnComplete)
		{
			base.transform.localScale = this.initialScale;
		}
	}

	// Token: 0x0400062F RID: 1583
	[SerializeField]
	private string triggerInfoContains;

	// Token: 0x04000630 RID: 1584
	[SerializeField]
	private bool flipX;

	// Token: 0x04000631 RID: 1585
	[SerializeField]
	private bool flipY;

	// Token: 0x04000632 RID: 1586
	[SerializeField]
	private bool resetOnComplete;

	// Token: 0x04000633 RID: 1587
	private bool hasTriggered;

	// Token: 0x04000634 RID: 1588
	private Vector3 initialScale;

	// Token: 0x04000635 RID: 1589
	private tk2dSpriteAnimator animator;
}
