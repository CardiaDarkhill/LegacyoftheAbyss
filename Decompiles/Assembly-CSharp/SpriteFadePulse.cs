using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class SpriteFadePulse : MonoBehaviour
{
	// Token: 0x06000766 RID: 1894 RVA: 0x0002425D File Offset: 0x0002245D
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.lowAlphaOriginal = this.lowAlpha;
		this.paused = this.startPaused;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00024283 File Offset: 0x00022483
	private void Start()
	{
		this.ResetFade();
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0002428B File Offset: 0x0002248B
	private void OnEnable()
	{
		this.ResetFade();
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00024294 File Offset: 0x00022494
	private void ResetFade()
	{
		this.lowAlpha = this.lowAlphaOriginal;
		this.paused = this.startPaused;
		this.currentLerpTime = 0f;
		Color color = this.spriteRenderer.color;
		color.a = this.lowAlpha;
		this.spriteRenderer.color = color;
		this.FadeIn();
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x000242F0 File Offset: 0x000224F0
	private void Update()
	{
		if (!this.paused)
		{
			float t = this.currentLerpTime / this.fadeDuration;
			this.currentAlpha = Mathf.Lerp(this.lowAlpha, this.highAlpha, t);
			Color color = this.spriteRenderer.color;
			color.a = this.currentAlpha;
			this.spriteRenderer.color = color;
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

	// Token: 0x0600076B RID: 1899 RVA: 0x000243C3 File Offset: 0x000225C3
	public void FadeIn()
	{
		this.state = 0;
		this.currentLerpTime = 0f;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x000243D7 File Offset: 0x000225D7
	public void FadeOut()
	{
		this.state = 1;
		this.currentLerpTime = this.fadeDuration;
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x000243EC File Offset: 0x000225EC
	public void EndFade()
	{
		this.lowAlpha = 0f;
		this.state = 2;
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00024400 File Offset: 0x00022600
	public void PauseFade()
	{
		this.paused = true;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00024409 File Offset: 0x00022609
	public void UnpauseFade()
	{
		this.paused = false;
	}

	// Token: 0x04000731 RID: 1841
	public float lowAlpha;

	// Token: 0x04000732 RID: 1842
	public float highAlpha;

	// Token: 0x04000733 RID: 1843
	public float fadeDuration;

	// Token: 0x04000734 RID: 1844
	public bool startPaused;

	// Token: 0x04000735 RID: 1845
	private bool paused;

	// Token: 0x04000736 RID: 1846
	private SpriteRenderer spriteRenderer;

	// Token: 0x04000737 RID: 1847
	private int state;

	// Token: 0x04000738 RID: 1848
	private float currentLerpTime;

	// Token: 0x04000739 RID: 1849
	private float currentAlpha;

	// Token: 0x0400073A RID: 1850
	private float lowAlphaOriginal;
}
