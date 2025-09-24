using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000015 RID: 21
public class LogoLanguage : MonoBehaviour
{
	// Token: 0x06000096 RID: 150 RVA: 0x000048F3 File Offset: 0x00002AF3
	private void OnEnable()
	{
		this.gm = GameManager.SilentInstance;
		if (this.gm)
		{
			this.gm.RefreshLanguageText += this.SetSprite;
		}
		this.SetSprite();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000492A File Offset: 0x00002B2A
	private void OnDisable()
	{
		if (!this.gm)
		{
			return;
		}
		this.gm.RefreshLanguageText -= this.SetSprite;
		this.gm = null;
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00004958 File Offset: 0x00002B58
	public void SetSprite()
	{
		Sprite sprite;
		if (Language.CurrentLanguage().ToString() == "ZH")
		{
			sprite = this.chineseSprite;
		}
		else
		{
			sprite = this.englishSprite;
		}
		if (this.spriteRenderer)
		{
			this.spriteRenderer.sprite = sprite;
		}
		if (this.uiImage)
		{
			this.uiImage.sprite = sprite;
			if (this.setNativeSize)
			{
				this.uiImage.SetNativeSize();
			}
		}
	}

	// Token: 0x0400006F RID: 111
	public SpriteRenderer spriteRenderer;

	// Token: 0x04000070 RID: 112
	[Space]
	public Image uiImage;

	// Token: 0x04000071 RID: 113
	public bool setNativeSize = true;

	// Token: 0x04000072 RID: 114
	[Space]
	public Sprite englishSprite;

	// Token: 0x04000073 RID: 115
	public Sprite chineseSprite;

	// Token: 0x04000074 RID: 116
	private GameManager gm;
}
