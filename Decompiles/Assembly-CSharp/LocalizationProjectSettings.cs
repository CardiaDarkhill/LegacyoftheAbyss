using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000014 RID: 20
public class LocalizationProjectSettings : LocalizationProjectSettingsBase
{
	// Token: 0x06000090 RID: 144 RVA: 0x000047D4 File Offset: 0x000029D4
	public override bool TryGetSavedLanguageCode(out string languageCode)
	{
		if (Platform.Current && Platform.Current.LocalSharedData.HasKey("M2H_lastLanguage"))
		{
			languageCode = Platform.Current.LocalSharedData.GetString("M2H_lastLanguage", "");
			return true;
		}
		languageCode = LanguageCode.EN.ToString();
		return false;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00004833 File Offset: 0x00002A33
	public override SystemLanguage GetSystemLanguage()
	{
		return Platform.Current.GetSystemLanguage();
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00004840 File Offset: 0x00002A40
	public override void OnSwitchedLanguage(LanguageCode newLang)
	{
		if (!Platform.Current)
		{
			return;
		}
		Platform.Current.LocalSharedData.SetString("M2H_lastLanguage", newLang.ToString() ?? "");
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004894 File Offset: 0x00002A94
	public override bool ShouldCheckText(string sheetTitle, string key)
	{
		string[] array = this.excludeSheets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == sheetTitle)
			{
				return false;
			}
		}
		foreach (string value in this.excludeKeys)
		{
			if (key.Contains(value))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x000048E8 File Offset: 0x00002AE8
	public override bool IsTextOverflowing(string sheetTitle, string text)
	{
		return false;
	}

	// Token: 0x0400006B RID: 107
	[SerializeField]
	private string[] excludeSheets;

	// Token: 0x0400006C RID: 108
	[SerializeField]
	private string[] excludeKeys;

	// Token: 0x0400006D RID: 109
	[SerializeField]
	private global::LocalizationProjectSettings.BoxTest[] boxTests;

	// Token: 0x0400006E RID: 110
	private const string LAST_LANGUAGE_KEY = "M2H_lastLanguage";

	// Token: 0x020013BC RID: 5052
	[Serializable]
	public struct BoxTest
	{
		// Token: 0x0400808C RID: 32908
		public string[] IncludeTitles;

		// Token: 0x0400808D RID: 32909
		public AssetReferenceGameObject TextBoxPrefab;
	}
}
