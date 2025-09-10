using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class tk2dAnimateUpDown : MonoBehaviour
{
	// Token: 0x0600064E RID: 1614 RVA: 0x000206B8 File Offset: 0x0001E8B8
	private void Start()
	{
		if (this.hasStarted)
		{
			return;
		}
		this.hasStarted = true;
		if (this.animator)
		{
			this.renderer = this.animator.GetComponent<MeshRenderer>();
		}
		if (this.startUp)
		{
			this.isAnimatingDown = false;
			this.renderer.enabled = true;
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.upAnim);
			this.animator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
			return;
		}
		this.isAnimatingDown = true;
		this.renderer.enabled = false;
		tk2dSpriteAnimationClip clipByName2 = this.animator.GetClipByName(this.downAnim);
		this.animator.PlayFromFrame(clipByName2, clipByName2.frames.Length - 1);
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00020772 File Offset: 0x0001E972
	[ContextMenu("Animate Up", true)]
	[ContextMenu("Animate Down", true)]
	private bool CanAnimate()
	{
		return Application.isPlaying;
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00020779 File Offset: 0x0001E979
	[ContextMenu("Animate Up")]
	public void AnimateUp()
	{
		if (!this.isAnimatingDown)
		{
			return;
		}
		this.isAnimatingDown = false;
		this.PlayAnimation(this.upAnim);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00020797 File Offset: 0x0001E997
	[ContextMenu("Animate Down")]
	public void AnimateDown()
	{
		if (this.isAnimatingDown)
		{
			return;
		}
		this.isAnimatingDown = true;
		this.PlayAnimation(this.downAnim);
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000207B8 File Offset: 0x0001E9B8
	private void PlayAnimation(string animName)
	{
		this.Start();
		if (!this.animator)
		{
			return;
		}
		this.renderer.enabled = true;
		this.animator.Play(animName);
		this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationComplete);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00020808 File Offset: 0x0001EA08
	private void OnAnimationComplete(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (this.isAnimatingDown)
		{
			this.renderer.enabled = false;
		}
	}

	// Token: 0x04000628 RID: 1576
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04000629 RID: 1577
	[Space]
	[SerializeField]
	private string upAnim;

	// Token: 0x0400062A RID: 1578
	[SerializeField]
	private string downAnim;

	// Token: 0x0400062B RID: 1579
	[SerializeField]
	private bool startUp;

	// Token: 0x0400062C RID: 1580
	private MeshRenderer renderer;

	// Token: 0x0400062D RID: 1581
	private bool hasStarted;

	// Token: 0x0400062E RID: 1582
	private bool isAnimatingDown;
}
