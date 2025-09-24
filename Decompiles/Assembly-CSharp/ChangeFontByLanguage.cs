using System;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class ChangeFontByLanguage : MonoBehaviour
{
	// Token: 0x06000051 RID: 81 RVA: 0x00003714 File Offset: 0x00001914
	private void Awake()
	{
		if (this.didAwake)
		{
			return;
		}
		this.didAwake = true;
		this.tmpro = base.GetComponent<TextMeshPro>();
		if (this.tmpro)
		{
			if (this.defaultFont == null)
			{
				this.defaultFont = this.tmpro.font;
			}
			this.defaultMaterial = this.tmpro.fontSharedMaterial;
			this.startFontSize = this.tmpro.fontSize;
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x0000378B File Offset: 0x0000198B
	private void Start()
	{
		this.SetFont();
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003793 File Offset: 0x00001993
	private void OnEnable()
	{
		if (!this.onlyOnStart || CheatManager.ForceLanguageComponentUpdates)
		{
			this.SetFont();
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000037AA File Offset: 0x000019AA
	private void OnDestroy()
	{
		this.FallbackMaterialReference = null;
	}

	// Token: 0x06000055 RID: 85 RVA: 0x000037B4 File Offset: 0x000019B4
	public void SetFont()
	{
		if (!this.didAwake)
		{
			this.Awake();
		}
		if (this.tmpro == null)
		{
			return;
		}
		LanguageCode languageCode = Language.CurrentLanguage();
		if (languageCode == LanguageCode.JA)
		{
			this.tmpro.fontSize = this.GetFontScale(languageCode);
			this.SetFont(this.fontJA, this.copyMaterialSettingsJA);
		}
		else if (languageCode == LanguageCode.RU)
		{
			this.tmpro.fontSize = this.GetFontScale(languageCode);
			this.SetFont(this.fontRU, this.copyMaterialSettingsRU);
		}
		else if (languageCode == LanguageCode.ZH)
		{
			this.tmpro.fontSize = this.GetFontScale(languageCode);
			this.SetFont(this.fontZH, this.copyMaterialSettingsZH);
		}
		else if (languageCode == LanguageCode.KO)
		{
			this.tmpro.fontSize = this.GetFontScale(languageCode);
			this.SetFont(this.fontKO, this.copyMaterialSettingsKO);
		}
		else
		{
			this.tmpro.fontSize = this.startFontSize;
			if (this.defaultFont != null)
			{
				this.tmpro.font = this.defaultFont;
			}
		}
		if (this.changeTextFontScaleOnHandHeld)
		{
			this.changeTextFontScaleOnHandHeld.DoUpdate();
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000056 RID: 86 RVA: 0x000038E4 File Offset: 0x00001AE4
	// (set) Token: 0x06000057 RID: 87 RVA: 0x000038EC File Offset: 0x00001AEC
	public Material FallbackMaterialReference
	{
		get
		{
			return this.fallbackMaterialReference;
		}
		set
		{
			if (this.fallbackMaterialReference == value)
			{
				return;
			}
			if (this.fallbackMaterialReference != null && this.fallbackMaterialReference != value)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.fallbackMaterialReference);
			}
			this.fallbackMaterialReference = value;
			TMP_MaterialManager.AddFallbackMaterialReference(this.fallbackMaterialReference);
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003944 File Offset: 0x00001B44
	private void SetFont(TMP_FontAsset fontAsset, bool copyMaterial)
	{
		if (fontAsset != null)
		{
			if (fontAsset != this.tmpro.font)
			{
				this.tmpro.font = fontAsset;
				if (copyMaterial)
				{
					Material fallbackMaterial = TMP_MaterialManager.GetFallbackMaterial(this.defaultMaterial, this.tmpro.fontSharedMaterial);
					this.FallbackMaterialReference = fallbackMaterial;
					this.tmpro.fontSharedMaterial = fallbackMaterial;
					return;
				}
			}
		}
		else if (this.tmpro.font != this.defaultFont)
		{
			this.FallbackMaterialReference = null;
			this.tmpro.font = this.defaultFont;
			this.tmpro.fontSharedMaterial = this.defaultMaterial;
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000039E8 File Offset: 0x00001BE8
	[Obsolete("Use GetFontScale(LanguageCode)")]
	private float GetFontScale(string lang)
	{
		switch (this.fontScaleLangType)
		{
		case ChangeFontByLanguage.FontScaleLangTypes.AreaName:
			return this.fontScaleAreaName.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.SubAreaName:
			return this.fontScaleSubAreaName.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.WideMap:
			return this.fontScaleWideMap.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.CreditsTitle:
			return this.fontScaleCreditsTitle.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.ExcerptAuthor:
			return this.fontScaleExcerptAuthor.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.QuestType:
			return this.fontScaleQuestType.GetFontScale(lang, this.startFontSize);
		default:
			return this.startFontSize;
		}
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003A98 File Offset: 0x00001C98
	private float GetFontScale(LanguageCode lang)
	{
		switch (this.fontScaleLangType)
		{
		case ChangeFontByLanguage.FontScaleLangTypes.AreaName:
			return this.fontScaleAreaName.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.SubAreaName:
			return this.fontScaleSubAreaName.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.WideMap:
			return this.fontScaleWideMap.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.CreditsTitle:
			return this.fontScaleCreditsTitle.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.ExcerptAuthor:
			return this.fontScaleExcerptAuthor.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.QuestType:
			return this.fontScaleQuestType.GetFontScale(lang, this.startFontSize);
		case ChangeFontByLanguage.FontScaleLangTypes.QuestName:
			return this.fontScaleQuestName.GetFontScale(lang, this.startFontSize);
		default:
			return this.startFontSize;
		}
	}

	// Token: 0x04000031 RID: 49
	public TMP_FontAsset defaultFont;

	// Token: 0x04000032 RID: 50
	public TMP_FontAsset fontJA;

	// Token: 0x04000033 RID: 51
	public bool copyMaterialSettingsJA = true;

	// Token: 0x04000034 RID: 52
	public TMP_FontAsset fontRU;

	// Token: 0x04000035 RID: 53
	public bool copyMaterialSettingsRU = true;

	// Token: 0x04000036 RID: 54
	public TMP_FontAsset fontZH;

	// Token: 0x04000037 RID: 55
	public bool copyMaterialSettingsZH = true;

	// Token: 0x04000038 RID: 56
	public TMP_FontAsset fontKO;

	// Token: 0x04000039 RID: 57
	public bool copyMaterialSettingsKO = true;

	// Token: 0x0400003A RID: 58
	public bool onlyOnStart;

	// Token: 0x0400003B RID: 59
	[SerializeField]
	private ChangeTextFontScaleOnHandHeld changeTextFontScaleOnHandHeld;

	// Token: 0x0400003C RID: 60
	private new bool didAwake;

	// Token: 0x0400003D RID: 61
	private TextMeshPro tmpro;

	// Token: 0x0400003E RID: 62
	private float startFontSize;

	// Token: 0x0400003F RID: 63
	public ChangeFontByLanguage.FontScaleLangTypes fontScaleLangType;

	// Token: 0x04000040 RID: 64
	private ChangeFontByLanguage.FontScaleLang fontScaleAreaName = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = new float?(3.3f),
		fontSizeRU = new float?(2.2f),
		fontSizeZH = new float?(4.2f),
		fontSizeKO = new float?(3.4f)
	};

	// Token: 0x04000041 RID: 65
	private ChangeFontByLanguage.FontScaleLang fontScaleSubAreaName = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = null,
		fontSizeRU = new float?(2.8f),
		fontSizeZH = new float?(4.1f),
		fontSizeKO = new float?(3.6f)
	};

	// Token: 0x04000042 RID: 66
	private ChangeFontByLanguage.FontScaleLang fontScaleWideMap = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = new float?(4.7f),
		fontSizeRU = new float?(3.25f),
		fontSizeZH = new float?(6.3f),
		fontSizeKO = new float?(5.4f)
	};

	// Token: 0x04000043 RID: 67
	private ChangeFontByLanguage.FontScaleLang fontScaleCreditsTitle = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = null,
		fontSizeRU = new float?(5.5f),
		fontSizeZH = null,
		fontSizeKO = null
	};

	// Token: 0x04000044 RID: 68
	private ChangeFontByLanguage.FontScaleLang fontScaleExcerptAuthor = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = new float?(4.5f),
		fontSizeRU = new float?(4.5f),
		fontSizeZH = new float?(4.5f),
		fontSizeKO = new float?(4.5f)
	};

	// Token: 0x04000045 RID: 69
	private ChangeFontByLanguage.FontScaleLang fontScaleQuestType = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = new float?(4f),
		fontSizeRU = new float?(4f),
		fontSizeZH = new float?(5f),
		fontSizeKO = new float?(4f)
	};

	// Token: 0x04000046 RID: 70
	private ChangeFontByLanguage.FontScaleLang fontScaleQuestName = new ChangeFontByLanguage.FontScaleLang
	{
		fontSizeJA = new float?(6f),
		fontSizeRU = new float?(6f),
		fontSizeZH = new float?(5.5f),
		fontSizeKO = new float?(6f)
	};

	// Token: 0x04000047 RID: 71
	private Material defaultMaterial;

	// Token: 0x04000048 RID: 72
	private Material fallbackMaterialReference;

	// Token: 0x020013B7 RID: 5047
	public enum FontScaleLangTypes
	{
		// Token: 0x0400807C RID: 32892
		None,
		// Token: 0x0400807D RID: 32893
		AreaName,
		// Token: 0x0400807E RID: 32894
		SubAreaName,
		// Token: 0x0400807F RID: 32895
		WideMap,
		// Token: 0x04008080 RID: 32896
		CreditsTitle,
		// Token: 0x04008081 RID: 32897
		ExcerptAuthor,
		// Token: 0x04008082 RID: 32898
		QuestType,
		// Token: 0x04008083 RID: 32899
		QuestName
	}

	// Token: 0x020013B8 RID: 5048
	private class FontScaleLang
	{
		// Token: 0x06008147 RID: 33095 RVA: 0x00261200 File Offset: 0x0025F400
		public float GetFontScale(string lang, float defaultScale)
		{
			if (lang == "JA")
			{
				if (this.fontSizeJA == null)
				{
					return defaultScale;
				}
				return this.fontSizeJA.Value;
			}
			else if (lang == "RU")
			{
				if (this.fontSizeRU == null)
				{
					return defaultScale;
				}
				return this.fontSizeRU.Value;
			}
			else if (lang == "ZH")
			{
				if (this.fontSizeZH == null)
				{
					return defaultScale;
				}
				return this.fontSizeZH.Value;
			}
			else
			{
				if (!(lang == "KO"))
				{
					return defaultScale;
				}
				if (this.fontSizeKO == null)
				{
					return defaultScale;
				}
				return this.fontSizeKO.Value;
			}
		}

		// Token: 0x06008148 RID: 33096 RVA: 0x002612B0 File Offset: 0x0025F4B0
		public float GetFontScale(LanguageCode lang, float defaultScale)
		{
			if (lang == LanguageCode.JA)
			{
				if (this.fontSizeJA == null)
				{
					return defaultScale;
				}
				return this.fontSizeJA.Value;
			}
			else if (lang == LanguageCode.RU)
			{
				if (this.fontSizeRU == null)
				{
					return defaultScale;
				}
				return this.fontSizeRU.Value;
			}
			else if (lang == LanguageCode.ZH)
			{
				if (this.fontSizeZH == null)
				{
					return defaultScale;
				}
				return this.fontSizeZH.Value;
			}
			else
			{
				if (lang != LanguageCode.KO)
				{
					return defaultScale;
				}
				if (this.fontSizeKO == null)
				{
					return defaultScale;
				}
				return this.fontSizeKO.Value;
			}
		}

		// Token: 0x04008084 RID: 32900
		public float? fontSizeJA;

		// Token: 0x04008085 RID: 32901
		public float? fontSizeRU;

		// Token: 0x04008086 RID: 32902
		public float? fontSizeZH;

		// Token: 0x04008087 RID: 32903
		public float? fontSizeKO;
	}
}
