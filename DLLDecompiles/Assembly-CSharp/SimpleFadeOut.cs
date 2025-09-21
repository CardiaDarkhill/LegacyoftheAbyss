using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class SimpleFadeOut : MonoBehaviour
{
	// Token: 0x06000758 RID: 1880 RVA: 0x00023F2C File Offset: 0x0002212C
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.originalColour = this.spriteRenderer.color;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00023F4B File Offset: 0x0002214B
	private void OnEnable()
	{
		this.ResetFade();
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00023F54 File Offset: 0x00022154
	private void ResetFade()
	{
		this.currentLerpTime = 0f;
		if (this.resetOnEnable)
		{
			this.spriteRenderer.color = this.originalColour;
		}
		this.startColor = this.spriteRenderer.color;
		Color original = this.startColor;
		float? a = new float?(0f);
		this.fadeColor = original.Where(null, null, null, a);
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00023FD0 File Offset: 0x000221D0
	private void Update()
	{
		if (this.waitForCall)
		{
			return;
		}
		this.currentLerpTime += (this.isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
		if (this.currentLerpTime > this.fadeDuration)
		{
			this.currentLerpTime = this.fadeDuration;
			base.gameObject.SetActive(false);
		}
		float t = this.currentLerpTime / this.fadeDuration;
		this.spriteRenderer.color = Color.Lerp(this.startColor, this.fadeColor, t);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00024058 File Offset: 0x00022258
	public void FadeOut()
	{
		this.waitForCall = false;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00024061 File Offset: 0x00022261
	public void SetColor(Color color)
	{
		this.spriteRenderer.color = color;
		this.ResetFade();
	}

	// Token: 0x0400071C RID: 1820
	[SerializeField]
	[Tooltip("The time to complete one half cycle of a pulse.")]
	private float fadeDuration = 1f;

	// Token: 0x0400071D RID: 1821
	[SerializeField]
	private bool waitForCall;

	// Token: 0x0400071E RID: 1822
	[SerializeField]
	private bool resetOnEnable;

	// Token: 0x0400071F RID: 1823
	[SerializeField]
	private bool isRealtime;

	// Token: 0x04000720 RID: 1824
	private Color startColor;

	// Token: 0x04000721 RID: 1825
	private Color fadeColor;

	// Token: 0x04000722 RID: 1826
	private float currentLerpTime;

	// Token: 0x04000723 RID: 1827
	private Color originalColour;

	// Token: 0x04000724 RID: 1828
	private SpriteRenderer spriteRenderer;
}
