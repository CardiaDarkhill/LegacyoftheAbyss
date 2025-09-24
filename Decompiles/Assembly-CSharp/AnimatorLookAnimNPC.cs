using System;
using UnityEngine;

// Token: 0x0200048C RID: 1164
public class AnimatorLookAnimNPC : LookAnimNPC
{
	// Token: 0x06002A05 RID: 10757 RVA: 0x000B6616 File Offset: 0x000B4816
	protected override void FlipSprite()
	{
		if (!this.sprite)
		{
			return;
		}
		this.sprite.flipX = !this.sprite.flipX;
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x000B663F File Offset: 0x000B483F
	protected override bool GetIsSpriteFlipped()
	{
		return this.sprite && this.sprite.flipX;
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x000B665B File Offset: 0x000B485B
	protected override void PlayAnim(string animName)
	{
		if (!this.animator)
		{
			return;
		}
		this.animator.Play(animName);
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000B6678 File Offset: 0x000B4878
	protected override bool IsAnimPlaying(string animName)
	{
		if (!this.animator)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return currentAnimatorStateInfo.shortNameHash == Animator.StringToHash(animName) && currentAnimatorStateInfo.normalizedTime < 1f;
	}

	// Token: 0x04002A82 RID: 10882
	[Header("Animator")]
	[SerializeField]
	private Animator animator;

	// Token: 0x04002A83 RID: 10883
	[SerializeField]
	private SpriteRenderer sprite;
}
