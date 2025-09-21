using System;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class MirrorTk2dAnimDelayed : MonoBehaviour
{
	// Token: 0x060004B8 RID: 1208 RVA: 0x00019455 File Offset: 0x00017655
	private void OnDisable()
	{
		this.delayLeft = 0f;
		this.previouslyPlaying = null;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001946C File Offset: 0x0001766C
	private void LateUpdate()
	{
		if (this.delayLeft > 0f)
		{
			this.delayLeft -= Time.deltaTime;
			if (this.delayLeft <= 0f)
			{
				this.animator.Play(this.previouslyPlaying);
			}
		}
		if (this.previouslyPlaying != null && this.mirrorAnimator.IsPlaying(this.previouslyPlaying))
		{
			return;
		}
		if (!this.mirrorAnimator.Playing)
		{
			return;
		}
		this.delayLeft = this.delay;
		this.previouslyPlaying = this.mirrorAnimator.CurrentClip;
		if (this.delayLeft <= 0f)
		{
			this.animator.Play(this.previouslyPlaying);
		}
	}

	// Token: 0x04000485 RID: 1157
	[SerializeField]
	private tk2dSpriteAnimator mirrorAnimator;

	// Token: 0x04000486 RID: 1158
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04000487 RID: 1159
	[SerializeField]
	private float delay;

	// Token: 0x04000488 RID: 1160
	private tk2dSpriteAnimationClip previouslyPlaying;

	// Token: 0x04000489 RID: 1161
	private float delayLeft;
}
