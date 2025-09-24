using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class SimpleSpriteFade : MonoBehaviour
{
	// Token: 0x0600075F RID: 1887 RVA: 0x00024088 File Offset: 0x00022288
	private void Awake()
	{
		if (!this.spriteRenderer)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		this.fadeGroup = base.GetComponent<NestedFadeGroupBase>();
		this.normalColor = this.spriteRenderer.color;
		if (this.fadeGroup)
		{
			this.normalColor.a = this.fadeGroup.AlphaSelf;
		}
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x000240EE File Offset: 0x000222EE
	private void OnEnable()
	{
		this.spriteRenderer.color = this.normalColor;
		if (this.fadeInOnStart)
		{
			this.FadeIn();
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00024110 File Offset: 0x00022310
	private void Update()
	{
		float num = this.isRealtime ? Time.unscaledDeltaTime : Time.deltaTime;
		if (!this.fadingIn && !this.fadingOut)
		{
			return;
		}
		if (this.fadingIn)
		{
			this.currentLerpTime += num;
			if (this.currentLerpTime > this.fadeDuration)
			{
				this.currentLerpTime = this.fadeDuration;
				this.fadingIn = false;
				if (this.recycleOnFadeIn)
				{
					base.gameObject.Recycle();
				}
				if (this.deactivateOnFadeIn)
				{
					base.gameObject.SetActive(false);
				}
			}
		}
		else if (this.fadingOut)
		{
			this.currentLerpTime -= num;
			if (this.currentLerpTime < 0f)
			{
				this.currentLerpTime = 0f;
				this.fadingOut = false;
			}
		}
		float t = this.currentLerpTime / this.fadeDuration;
		Color color = Color.Lerp(this.normalColor, this.fadeInColor, t);
		this.spriteRenderer.color = color;
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = color.a;
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x00024223 File Offset: 0x00022423
	public void FadeIn()
	{
		this.fadingIn = true;
		this.currentLerpTime = 0f;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00024237 File Offset: 0x00022437
	public void FadeOut()
	{
		this.fadingOut = true;
		this.currentLerpTime = this.fadeDuration;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0002424C File Offset: 0x0002244C
	public void SetDuration(float newDuration)
	{
		this.fadeDuration = newDuration;
	}

	// Token: 0x04000725 RID: 1829
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04000726 RID: 1830
	[Space]
	public Color fadeInColor;

	// Token: 0x04000727 RID: 1831
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04000728 RID: 1832
	private Color normalColor;

	// Token: 0x04000729 RID: 1833
	public float fadeDuration;

	// Token: 0x0400072A RID: 1834
	private bool fadingIn;

	// Token: 0x0400072B RID: 1835
	private bool fadingOut;

	// Token: 0x0400072C RID: 1836
	private float currentLerpTime;

	// Token: 0x0400072D RID: 1837
	public bool fadeInOnStart;

	// Token: 0x0400072E RID: 1838
	public bool deactivateOnFadeIn;

	// Token: 0x0400072F RID: 1839
	public bool recycleOnFadeIn;

	// Token: 0x04000730 RID: 1840
	public bool isRealtime;
}
