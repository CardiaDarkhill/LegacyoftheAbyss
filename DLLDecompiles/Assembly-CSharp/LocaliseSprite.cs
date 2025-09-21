using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200040E RID: 1038
[RequireComponent(typeof(SpriteRenderer))]
public class LocaliseSprite : MonoBehaviour
{
	// Token: 0x06002332 RID: 9010 RVA: 0x000A0F95 File Offset: 0x0009F195
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x000A0FA4 File Offset: 0x0009F1A4
	private void Start()
	{
		LanguageCode languageCode = Language.CurrentLanguage();
		foreach (LocaliseSprite.LangSpritePair langSpritePair in this.sprites)
		{
			if (langSpritePair.language == languageCode)
			{
				this.spriteRenderer.sprite = langSpritePair.sprite;
				return;
			}
		}
	}

	// Token: 0x040021DB RID: 8667
	public LocaliseSprite.LangSpritePair[] sprites;

	// Token: 0x040021DC RID: 8668
	private SpriteRenderer spriteRenderer;

	// Token: 0x020016A3 RID: 5795
	[Serializable]
	public struct LangSpritePair
	{
		// Token: 0x04008B96 RID: 35734
		public LanguageCode language;

		// Token: 0x04008B97 RID: 35735
		public Sprite sprite;
	}
}
