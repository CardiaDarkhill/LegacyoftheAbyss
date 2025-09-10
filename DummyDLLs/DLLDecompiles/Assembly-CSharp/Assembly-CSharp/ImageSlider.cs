using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000673 RID: 1651
public class ImageSlider : MonoBehaviour
{
	// Token: 0x170006B0 RID: 1712
	// (get) Token: 0x06003B33 RID: 15155 RVA: 0x00104C37 File Offset: 0x00102E37
	// (set) Token: 0x06003B34 RID: 15156 RVA: 0x00104C3F File Offset: 0x00102E3F
	public float Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.DisplayValue(value);
		}
	}

	// Token: 0x170006B1 RID: 1713
	// (get) Token: 0x06003B35 RID: 15157 RVA: 0x00104C48 File Offset: 0x00102E48
	// (set) Token: 0x06003B36 RID: 15158 RVA: 0x00104C55 File Offset: 0x00102E55
	public Color Color
	{
		get
		{
			return this.image.color;
		}
		set
		{
			this.image.color = value;
			if (this.glower)
			{
				this.glower.Color = value;
			}
		}
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x00104C7C File Offset: 0x00102E7C
	private void OnEnable()
	{
		this.DisplayValue(this.value);
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x00104C8C File Offset: 0x00102E8C
	private void DisplayValue(float newValue)
	{
		this.value = newValue;
		if (this.value > 0f && this.value < this.minDisplayValue)
		{
			this.value = this.minDisplayValue;
		}
		if (this.image)
		{
			this.image.fillAmount = this.value;
		}
		if (this.glower)
		{
			this.glower.gameObject.SetActive(this.value >= 1f);
		}
	}

	// Token: 0x04003D7A RID: 15738
	[SerializeField]
	private Image image;

	// Token: 0x04003D7B RID: 15739
	[SerializeField]
	private float minDisplayValue;

	// Token: 0x04003D7C RID: 15740
	[SerializeField]
	private NestedFadeGroupSpriteRenderer glower;

	// Token: 0x04003D7D RID: 15741
	private float value;
}
