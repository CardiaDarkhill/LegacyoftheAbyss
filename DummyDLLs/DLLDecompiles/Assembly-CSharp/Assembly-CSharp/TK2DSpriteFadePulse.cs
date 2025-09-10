using System;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class TK2DSpriteFadePulse : MonoBehaviour
{
	// Token: 0x06000780 RID: 1920 RVA: 0x0002491F File Offset: 0x00022B1F
	private void Awake()
	{
		this.tk2d_sprite = base.GetComponent<tk2dSprite>();
		this.lowAlphaOriginal = this.lowAlpha;
		this.paused = this.startPaused;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00024945 File Offset: 0x00022B45
	private void OnEnable()
	{
		this.lowAlpha = this.lowAlphaOriginal;
		this.FadeIn();
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0002495C File Offset: 0x00022B5C
	private void Update()
	{
		if (!this.paused)
		{
			float t = this.currentLerpTime / this.fadeDuration;
			this.currentAlpha = Mathf.Lerp(this.lowAlpha, this.highAlpha, t);
			Color color = this.tk2d_sprite.color;
			color.a = this.currentAlpha;
			this.tk2d_sprite.color = color;
			if (this.state == 0)
			{
				this.currentLerpTime += Time.deltaTime;
				if (this.currentLerpTime > this.fadeDuration)
				{
					this.FadeOut();
					return;
				}
			}
			else if (this.state == 1)
			{
				this.currentLerpTime -= Time.deltaTime;
				if (this.currentLerpTime < 0f)
				{
					this.FadeIn();
					return;
				}
			}
			else
			{
				this.currentLerpTime -= Time.deltaTime;
			}
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00024A2F File Offset: 0x00022C2F
	public void FadeIn()
	{
		this.state = 0;
		this.currentLerpTime = 0f;
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00024A43 File Offset: 0x00022C43
	public void FadeOut()
	{
		this.state = 1;
		this.currentLerpTime = this.fadeDuration;
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x00024A58 File Offset: 0x00022C58
	public void EndFade()
	{
		this.lowAlpha = 0f;
		this.state = 2;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00024A6C File Offset: 0x00022C6C
	public void PauseFade()
	{
		this.paused = true;
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00024A75 File Offset: 0x00022C75
	public void UnpauseFade()
	{
		this.paused = false;
	}

	// Token: 0x04000751 RID: 1873
	public float lowAlpha;

	// Token: 0x04000752 RID: 1874
	public float highAlpha;

	// Token: 0x04000753 RID: 1875
	public float fadeDuration;

	// Token: 0x04000754 RID: 1876
	public bool startPaused;

	// Token: 0x04000755 RID: 1877
	private bool paused;

	// Token: 0x04000756 RID: 1878
	private tk2dSprite tk2d_sprite;

	// Token: 0x04000757 RID: 1879
	private int state;

	// Token: 0x04000758 RID: 1880
	private float currentLerpTime;

	// Token: 0x04000759 RID: 1881
	private float currentAlpha;

	// Token: 0x0400075A RID: 1882
	private float lowAlphaOriginal;
}
