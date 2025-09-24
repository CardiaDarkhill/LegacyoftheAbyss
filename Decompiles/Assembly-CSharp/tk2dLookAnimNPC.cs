using System;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public class tk2dLookAnimNPC : LookAnimNPC
{
	// Token: 0x0600321C RID: 12828 RVA: 0x000DF4A8 File Offset: 0x000DD6A8
	protected override void Awake()
	{
		base.Awake();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.anim;
		this.sprite = (this.anim ? (this.anim.Sprite as tk2dSprite) : base.GetComponent<tk2dSprite>());
		this.hasSprite = this.sprite;
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x000DF50F File Offset: 0x000DD70F
	protected override float GetXScale()
	{
		if (this.hasSprite)
		{
			return this.sprite.scale.x * base.transform.lossyScale.x;
		}
		return base.GetXScale();
	}

	// Token: 0x0600321E RID: 12830 RVA: 0x000DF544 File Offset: 0x000DD744
	protected override void FlipSprite()
	{
		if (!this.hasSprite)
		{
			return;
		}
		Vector3 scale = this.sprite.scale;
		scale.x *= -1f;
		this.sprite.scale = scale;
	}

	// Token: 0x0600321F RID: 12831 RVA: 0x000DF582 File Offset: 0x000DD782
	protected override bool GetIsSpriteFlipped()
	{
		return this.hasSprite && this.sprite.scale.x * base.transform.lossyScale.x < 0f;
	}

	// Token: 0x06003220 RID: 12832 RVA: 0x000DF5B8 File Offset: 0x000DD7B8
	protected override bool GetWasFacingLeft()
	{
		if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.NoFlip)
		{
			return base.GetWasFacingLeft();
		}
		float num = this.hasSprite ? (this.sprite.scale.x * base.transform.lossyScale.x) : base.transform.lossyScale.x;
		if (!base.DefaultLeft)
		{
			return num < 0f;
		}
		return num > 0f;
	}

	// Token: 0x06003221 RID: 12833 RVA: 0x000DF629 File Offset: 0x000DD829
	protected override void EnsureCorrectFacing()
	{
		if (this.turnFlipType != LookAnimNPC.TurnFlipTypes.NoFlip)
		{
			if (this.GetWasFacingLeft())
			{
				if (base.State == LookAnimNPC.AnimState.Right)
				{
					base.State = LookAnimNPC.AnimState.Left;
					return;
				}
			}
			else if (base.State == LookAnimNPC.AnimState.Left)
			{
				base.State = LookAnimNPC.AnimState.Right;
			}
		}
	}

	// Token: 0x06003222 RID: 12834 RVA: 0x000DF65B File Offset: 0x000DD85B
	protected override void PlayAnim(string animName)
	{
		if (!this.anim || string.IsNullOrEmpty(animName))
		{
			return;
		}
		this.anim.Play(animName);
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x000DF680 File Offset: 0x000DD880
	protected override bool IsAnimPlaying(string animName)
	{
		return this.anim && !string.IsNullOrEmpty(animName) && this.anim.IsPlaying(animName) && this.anim.ClipTimeSeconds <= this.anim.CurrentClip.Duration;
	}

	// Token: 0x040035BD RID: 13757
	private tk2dSpriteAnimator anim;

	// Token: 0x040035BE RID: 13758
	private tk2dSprite sprite;

	// Token: 0x040035BF RID: 13759
	private bool hasSprite;
}
