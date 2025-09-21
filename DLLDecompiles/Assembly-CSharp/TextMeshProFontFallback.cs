using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x0200033C RID: 828
[CreateAssetMenu(fileName = "TMP Font Fallback Helper", menuName = "Font/Font Fallback Helper (TMP)")]
[Serializable]
public class TextMeshProFontFallback : FontFallbackBase
{
	// Token: 0x06001CE9 RID: 7401 RVA: 0x000865CC File Offset: 0x000847CC
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.fallbacks.Clear();
		for (int i = 0; i < this.languageFallbacks.Count; i++)
		{
			TextMeshProFontFallback.Fallback fallback = this.languageFallbacks[i];
			if (fallback != null)
			{
				LanguageCode language = (LanguageCode)fallback.language;
				this.fallbacks[language] = fallback;
			}
		}
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x00086627 File Offset: 0x00084827
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06001CEB RID: 7403 RVA: 0x00086630 File Offset: 0x00084830
	public override void OnChangedLanguage(LanguageCode newLanguage)
	{
		this.Init();
		TextMeshProFontFallback.Fallback fallback;
		if (this.fallbacks.TryGetValue(newLanguage, out fallback))
		{
			if (this.font.fallbackFontAssets == null)
			{
				this.font.fallbackFontAssets = new List<TMP_FontAsset>(fallback.fallbackFonts);
				return;
			}
			this.font.fallbackFontAssets.Clear();
			this.font.fallbackFontAssets.AddRange(fallback.fallbackFonts);
		}
	}

	// Token: 0x04001C42 RID: 7234
	public TMP_FontAsset font;

	// Token: 0x04001C43 RID: 7235
	[SerializeField]
	private List<TextMeshProFontFallback.Fallback> languageFallbacks = new List<TextMeshProFontFallback.Fallback>();

	// Token: 0x04001C44 RID: 7236
	[NonSerialized]
	private bool init;

	// Token: 0x04001C45 RID: 7237
	private Dictionary<LanguageCode, TextMeshProFontFallback.Fallback> fallbacks = new Dictionary<LanguageCode, TextMeshProFontFallback.Fallback>();

	// Token: 0x02001605 RID: 5637
	[Serializable]
	private sealed class Fallback
	{
		// Token: 0x04008976 RID: 35190
		public SupportedLanguages language = SupportedLanguages.EN;

		// Token: 0x04008977 RID: 35191
		public List<TMP_FontAsset> fallbackFonts = new List<TMP_FontAsset>();
	}
}
