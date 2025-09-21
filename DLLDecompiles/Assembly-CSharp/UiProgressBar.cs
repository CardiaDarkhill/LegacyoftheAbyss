using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000746 RID: 1862
public class UiProgressBar : MonoBehaviour
{
	// Token: 0x17000784 RID: 1924
	// (get) Token: 0x06004256 RID: 16982 RVA: 0x00124F8D File Offset: 0x0012318D
	// (set) Token: 0x06004257 RID: 16983 RVA: 0x00124F95 File Offset: 0x00123195
	public float Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
			this.UpdateImage();
		}
	}

	// Token: 0x06004258 RID: 16984 RVA: 0x00124FA4 File Offset: 0x001231A4
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this.UpdateImage();
	}

	// Token: 0x06004259 RID: 16985 RVA: 0x00124FB4 File Offset: 0x001231B4
	private void Update()
	{
		this.UpdateBar(Time.deltaTime);
	}

	// Token: 0x0600425A RID: 16986 RVA: 0x00124FC4 File Offset: 0x001231C4
	public void UpdateBar(float deltaTime)
	{
		if (Math.Abs(this.lastBarFill - this.targetImageFill) < 0.001f)
		{
			return;
		}
		float num = Mathf.Lerp(this.lastBarFill, this.targetImageFill, deltaTime * this.barLerpSpeed);
		if (Math.Abs(num - this.targetImageFill) < 0.001f)
		{
			num = this.targetImageFill;
		}
		this.SetBarFill(num);
	}

	// Token: 0x0600425B RID: 16987 RVA: 0x00125028 File Offset: 0x00123228
	private void UpdateImage()
	{
		float lerpedValue = this.valueRange.GetLerpedValue(this.value);
		if (Math.Abs(lerpedValue - this.lastBarFill) > Mathf.Epsilon)
		{
			this.targetImageFill = lerpedValue;
			if (this.barLerpSpeed <= Mathf.Epsilon || !base.gameObject.activeInHierarchy)
			{
				this.SetBarFill(this.targetImageFill);
			}
			return;
		}
	}

	// Token: 0x0600425C RID: 16988 RVA: 0x0012508C File Offset: 0x0012328C
	private void SetBarFill(float fillAmount)
	{
		this.lastBarFill = fillAmount;
		if (this.barImage)
		{
			this.barImage.fillAmount = fillAmount;
		}
		if (this.barImageOther)
		{
			this.barImageOther.fillAmount = fillAmount;
		}
		if (this.barSprite)
		{
			if (this.propertyBlock == null)
			{
				this.propertyBlock = new MaterialPropertyBlock();
			}
			this.barSprite.GetPropertyBlock(this.propertyBlock);
			this.propertyBlock.SetFloat(UiProgressBar._arc1PropId, fillAmount);
			this.propertyBlock.SetFloat(UiProgressBar._arc2PropId, fillAmount);
			this.barSprite.SetPropertyBlock(this.propertyBlock);
		}
	}

	// Token: 0x0600425D RID: 16989 RVA: 0x00125138 File Offset: 0x00123338
	public void SetAngle(float angle)
	{
		if (this.barSprite)
		{
			if (this.propertyBlock == null)
			{
				this.propertyBlock = new MaterialPropertyBlock();
			}
			this.barSprite.GetPropertyBlock(this.propertyBlock);
			this.propertyBlock.SetFloat(UiProgressBar.ANGLE, angle);
			this.barSprite.SetPropertyBlock(this.propertyBlock);
		}
	}

	// Token: 0x0600425E RID: 16990 RVA: 0x00125198 File Offset: 0x00123398
	public void SetEdgeFade(float edgeFade)
	{
		if (this.barSprite)
		{
			if (this.propertyBlock == null)
			{
				this.propertyBlock = new MaterialPropertyBlock();
			}
			this.barSprite.GetPropertyBlock(this.propertyBlock);
			this.propertyBlock.SetFloat(UiProgressBar.EDGE_FADE, edgeFade);
			this.barSprite.SetPropertyBlock(this.propertyBlock);
		}
	}

	// Token: 0x0600425F RID: 16991 RVA: 0x001251F8 File Offset: 0x001233F8
	public void SetValueInstant(float val)
	{
		this.value = val;
		this.targetImageFill = this.valueRange.GetLerpedValue(val);
		this.SetBarFill(this.targetImageFill);
	}

	// Token: 0x040043EE RID: 17390
	[SerializeField]
	private Image barImage;

	// Token: 0x040043EF RID: 17391
	[SerializeField]
	private Image barImageOther;

	// Token: 0x040043F0 RID: 17392
	[SerializeField]
	private SpriteRenderer barSprite;

	// Token: 0x040043F1 RID: 17393
	[SerializeField]
	private MinMaxFloat valueRange;

	// Token: 0x040043F2 RID: 17394
	[SerializeField]
	private float barLerpSpeed;

	// Token: 0x040043F3 RID: 17395
	[Space]
	[SerializeField]
	private float value;

	// Token: 0x040043F4 RID: 17396
	private float lastBarFill;

	// Token: 0x040043F5 RID: 17397
	private float targetImageFill;

	// Token: 0x040043F6 RID: 17398
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x040043F7 RID: 17399
	private static readonly int _arc1PropId = Shader.PropertyToID("_Arc1");

	// Token: 0x040043F8 RID: 17400
	private static readonly int _arc2PropId = Shader.PropertyToID("_Arc2");

	// Token: 0x040043F9 RID: 17401
	private static readonly int ANGLE = Shader.PropertyToID("_Angle");

	// Token: 0x040043FA RID: 17402
	private static readonly int EDGE_FADE = Shader.PropertyToID("_EdgeFade");
}
