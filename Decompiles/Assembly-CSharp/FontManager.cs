using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200033B RID: 827
public sealed class FontManager : MonoBehaviour
{
	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06001CDD RID: 7389 RVA: 0x00086438 File Offset: 0x00084638
	// (remove) Token: 0x06001CDE RID: 7390 RVA: 0x0008646C File Offset: 0x0008466C
	public static event FontManager.LanguageChangedHandler OnLanguageChanged;

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x06001CDF RID: 7391 RVA: 0x0008649F File Offset: 0x0008469F
	// (set) Token: 0x06001CE0 RID: 7392 RVA: 0x000864A6 File Offset: 0x000846A6
	public static LanguageCode CurrentLanguage
	{
		get
		{
			return FontManager._currentLanguage;
		}
		set
		{
			if (FontManager._currentLanguage != value)
			{
				FontManager._currentLanguage = value;
				FontManager.LanguageChangedHandler onLanguageChanged = FontManager.OnLanguageChanged;
				if (onLanguageChanged == null)
				{
					return;
				}
				onLanguageChanged(value);
			}
		}
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x000864C6 File Offset: 0x000846C6
	private void Awake()
	{
		FontManager.instance = this;
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x000864CE File Offset: 0x000846CE
	private void Start()
	{
		this.started = true;
		FontManager.CurrentLanguage = Language.CurrentLanguage();
		this.ChangedLanguage(FontManager.CurrentLanguage);
		this.hasSetLanguage = true;
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x000864F3 File Offset: 0x000846F3
	private void OnEnable()
	{
		if (this.started && !this.hasSetLanguage)
		{
			FontManager.CurrentLanguage = Language.CurrentLanguage();
			this.ChangedLanguage(FontManager.CurrentLanguage);
			this.hasSetLanguage = true;
		}
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x00086521 File Offset: 0x00084721
	private void OnDestroy()
	{
		if (FontManager.instance == this)
		{
			FontManager.instance = null;
		}
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x00086538 File Offset: 0x00084738
	public void ChangedLanguage(LanguageCode newLanguage)
	{
		FontManager.CurrentLanguage = newLanguage;
		foreach (FontFallbackBase fontFallbackBase in this.fallbacks)
		{
			fontFallbackBase.OnChangedLanguage(newLanguage);
		}
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x00086590 File Offset: 0x00084790
	private void ForceUpdate()
	{
		this.ChangedLanguage(FontManager.CurrentLanguage);
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x0008659D File Offset: 0x0008479D
	public static void ForceUpdateLanguage()
	{
		if (FontManager.instance != null)
		{
			FontManager.instance.ForceUpdate();
		}
	}

	// Token: 0x04001C3C RID: 7228
	private static FontManager instance;

	// Token: 0x04001C3E RID: 7230
	private static LanguageCode _currentLanguage;

	// Token: 0x04001C3F RID: 7231
	[SerializeField]
	private List<FontFallbackBase> fallbacks = new List<FontFallbackBase>();

	// Token: 0x04001C40 RID: 7232
	private bool started;

	// Token: 0x04001C41 RID: 7233
	private bool hasSetLanguage;

	// Token: 0x02001604 RID: 5636
	// (Invoke) Token: 0x060088AD RID: 34989
	public delegate void LanguageChangedHandler(LanguageCode lang);
}
