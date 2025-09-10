using System;
using UnityEngine;

// Token: 0x02000675 RID: 1653
public class InvAnimateUpAndDown : MonoBehaviour
{
	// Token: 0x170006B3 RID: 1715
	// (get) Token: 0x06003B3C RID: 15164 RVA: 0x00104D2A File Offset: 0x00102F2A
	// (set) Token: 0x06003B3D RID: 15165 RVA: 0x00104D32 File Offset: 0x00102F32
	public bool IsLastAnimatedDown { get; private set; }

	// Token: 0x06003B3E RID: 15166 RVA: 0x00104D3B File Offset: 0x00102F3B
	private void Awake()
	{
		this.spriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
	}

	// Token: 0x06003B3F RID: 15167 RVA: 0x00104D58 File Offset: 0x00102F58
	private void Update()
	{
		if (this.animatingDown && !this.spriteAnimator.Playing)
		{
			this.meshRenderer.enabled = false;
			this.animatingDown = false;
		}
		if (this.timer > 0f)
		{
			this.timer -= Time.unscaledDeltaTime;
		}
		if (this.readyingAnimUp && this.timer <= 0f)
		{
			this.animatingDown = false;
			this.meshRenderer.enabled = true;
			if (this.randomStartFrameSpriteMax > 0)
			{
				int frame = Random.Range(0, this.randomStartFrameSpriteMax);
				this.spriteAnimator.PlayFromFrame(this.upAnimation, frame);
			}
			else
			{
				this.spriteAnimator.Play(this.upAnimation);
			}
			this.readyingAnimUp = false;
		}
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x00104E18 File Offset: 0x00103018
	public void Show()
	{
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = true;
		}
		this.IsLastAnimatedDown = false;
		if (this.spriteAnimator == null)
		{
			return;
		}
		tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.upAnimation);
		if (clipByName != null)
		{
			this.spriteAnimator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
		}
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x00104E7F File Offset: 0x0010307F
	public void AnimateUp()
	{
		this.readyingAnimUp = true;
		this.timer = this.upDelay;
		this.IsLastAnimatedDown = false;
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x00104E9B File Offset: 0x0010309B
	public void Hide()
	{
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = false;
		}
		this.IsLastAnimatedDown = true;
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x00104EBD File Offset: 0x001030BD
	public void AnimateDown()
	{
		this.spriteAnimator.Play(this.downAnimation);
		this.animatingDown = true;
		this.IsLastAnimatedDown = true;
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x00104EDE File Offset: 0x001030DE
	public void ReplayUpAnim()
	{
		this.meshRenderer.enabled = true;
		this.spriteAnimator.PlayFromFrame(0);
	}

	// Token: 0x04003D7F RID: 15743
	public string upAnimation;

	// Token: 0x04003D80 RID: 15744
	public string downAnimation;

	// Token: 0x04003D81 RID: 15745
	public float upDelay;

	// Token: 0x04003D82 RID: 15746
	public int randomStartFrameSpriteMax;

	// Token: 0x04003D83 RID: 15747
	private tk2dSpriteAnimator spriteAnimator;

	// Token: 0x04003D84 RID: 15748
	private MeshRenderer meshRenderer;

	// Token: 0x04003D85 RID: 15749
	private float timer;

	// Token: 0x04003D86 RID: 15750
	private bool animatingDown;

	// Token: 0x04003D87 RID: 15751
	private bool readyingAnimUp;
}
