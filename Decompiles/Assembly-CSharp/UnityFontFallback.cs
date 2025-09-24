using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200033D RID: 829
[CreateAssetMenu(fileName = "Font Fallback Helper", menuName = "Font/Font Fallback Helper (Unity)")]
[Serializable]
public class UnityFontFallback : FontFallbackBase
{
	// Token: 0x06001CED RID: 7405 RVA: 0x000866BC File Offset: 0x000848BC
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.fallbacks.Clear();
		for (int i = 0; i < this.languageFallbacks.Count; i++)
		{
			UnityFontFallback.Fallback fallback = this.languageFallbacks[i];
			if (fallback != null)
			{
				LanguageCode language = (LanguageCode)fallback.language;
				this.fallbacks[language] = fallback;
			}
		}
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x00086717 File Offset: 0x00084917
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x00086720 File Offset: 0x00084920
	public override void OnChangedLanguage(LanguageCode newLanguage)
	{
		this.Init();
		UnityFontFallback.Fallback fallback;
		if (this.fallbacks.TryGetValue(newLanguage, out fallback))
		{
			this.font.fontNames = fallback.fontNames;
		}
	}

	// Token: 0x04001C46 RID: 7238
	public Font font;

	// Token: 0x04001C47 RID: 7239
	[SerializeField]
	private List<UnityFontFallback.Fallback> languageFallbacks = new List<UnityFontFallback.Fallback>();

	// Token: 0x04001C48 RID: 7240
	[NonSerialized]
	private bool init;

	// Token: 0x04001C49 RID: 7241
	private Dictionary<LanguageCode, UnityFontFallback.Fallback> fallbacks = new Dictionary<LanguageCode, UnityFontFallback.Fallback>();

	// Token: 0x02001606 RID: 5638
	[Serializable]
	private sealed class Fallback
	{
		// Token: 0x04008978 RID: 35192
		public SupportedLanguages language = SupportedLanguages.EN;

		// Token: 0x04008979 RID: 35193
		public string[] fontNames;
	}
}
