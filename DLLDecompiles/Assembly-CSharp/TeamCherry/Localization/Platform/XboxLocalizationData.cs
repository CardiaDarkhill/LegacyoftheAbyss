using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008B0 RID: 2224
	[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Xbox Localization Data")]
	public sealed class XboxLocalizationData : ScriptableObject
	{
		// Token: 0x06004CE0 RID: 19680 RVA: 0x001690A8 File Offset: 0x001672A8
		private void OnEnable()
		{
			this.isValid = false;
		}

		// Token: 0x06004CE1 RID: 19681 RVA: 0x001690B1 File Offset: 0x001672B1
		private void OnValidate()
		{
			this.isValid = false;
		}

		// Token: 0x06004CE2 RID: 19682 RVA: 0x001690BC File Offset: 0x001672BC
		private void UpdateLookup()
		{
			if (this.isValid)
			{
				return;
			}
			bool flag = this.localizedStringsLookup.Count == 0;
			for (int i = 0; i < this.localizedStrings.Count; i++)
			{
				LocalizedStringEntry localizedStringEntry = this.localizedStrings[i];
				this.localizedStringsLookup[localizedStringEntry.id] = localizedStringEntry;
			}
			this.isValid = true;
		}

		// Token: 0x06004CE3 RID: 19683 RVA: 0x00169120 File Offset: 0x00167320
		public void MergeEntry(LocalizedStringEntry incoming)
		{
			this.UpdateLookup();
			LocalizedStringEntry localizedStringEntry;
			if (!this.localizedStringsLookup.TryGetValue(incoming.id, out localizedStringEntry))
			{
				this.localizedStringsLookup[incoming.id] = incoming;
				this.localizedStrings.Add(incoming);
				return;
			}
			localizedStringEntry.MergeValues(incoming);
		}

		// Token: 0x06004CE4 RID: 19684 RVA: 0x00169170 File Offset: 0x00167370
		public LocalizedStringEntry GetById(string id)
		{
			this.UpdateLookup();
			LocalizedStringEntry localizedStringEntry;
			if (!this.localizedStringsLookup.TryGetValue(id, out localizedStringEntry))
			{
				Dictionary<string, LocalizedStringEntry> dictionary = this.localizedStringsLookup;
				LocalizedStringEntry localizedStringEntry2 = new LocalizedStringEntry();
				localizedStringEntry2.id = id;
				localizedStringEntry = localizedStringEntry2;
				dictionary[id] = localizedStringEntry2;
				this.localizedStrings.Add(localizedStringEntry);
			}
			return localizedStringEntry;
		}

		// Token: 0x06004CE5 RID: 19685 RVA: 0x001691BA File Offset: 0x001673BA
		public bool TryGetLocalizedStringEntry(string id, out LocalizedStringEntry value)
		{
			this.UpdateLookup();
			return this.localizedStringsLookup.TryGetValue(id, out value);
		}

		// Token: 0x06004CE6 RID: 19686 RVA: 0x001691D0 File Offset: 0x001673D0
		private string GetElementName(int index)
		{
			try
			{
				LocalizedStringEntry localizedStringEntry = this.localizedStrings[index];
				return string.Format("{0}: !!{1}/{2}!! : {3}", new object[]
				{
					index,
					localizedStringEntry.localisedString.Sheet,
					localizedStringEntry.localisedString.Key,
					localizedStringEntry.id
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return string.Format("Element {0}", index);
		}

		// Token: 0x06004CE7 RID: 19687 RVA: 0x00169254 File Offset: 0x00167454
		private bool AuditLanguages()
		{
			HashSet<SupportedLanguages> hashSet = new HashSet<SupportedLanguages>();
			HashSet<string> hashSet2 = new HashSet<string>();
			bool result = true;
			for (int i = 0; i < this.languages.Count; i++)
			{
				XboxLocalizationData.LocaleLink localeLink = this.languages[i];
				if (!hashSet.Add(localeLink.language))
				{
					Debug.LogError(string.Format("#{0} contains duplicated language code {1}", i, localeLink.language));
					result = false;
				}
				if (!hashSet2.Add(localeLink.locale))
				{
					Debug.LogError(string.Format("#{0} contains duplicated locale {1}", i, localeLink.locale));
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06004CE8 RID: 19688 RVA: 0x001692F4 File Offset: 0x001674F4
		[ContextMenu("Update Localised Strings")]
		public void UpdateLocalisedStrings()
		{
			if (!this.AuditLanguages())
			{
				Debug.LogError("Languages list contains errors, Please fix before trying again.", this);
				return;
			}
			LanguageCode code = Language.CurrentLanguage();
			try
			{
				foreach (XboxLocalizationData.LocaleLink localeLink in this.languages)
				{
					try
					{
						if (Language.SwitchLanguage((LanguageCode)localeLink.language))
						{
							string locale = localeLink.locale;
							for (int i = 0; i < this.localizedStrings.Count; i++)
							{
								LocalizedStringEntry localizedStringEntry = this.localizedStrings[i];
								if (localizedStringEntry.localisedString.IsEmpty)
								{
									Debug.LogError(string.Format("#{0} Unable to update {1} missing localised string", i, localizedStringEntry.id));
								}
								else
								{
									localizedStringEntry.AddOrUpdate(new LocalizedValue
									{
										locale = locale,
										text = localizedStringEntry.localisedString.ToString()
									});
								}
							}
						}
						else
						{
							Debug.LogError(string.Format("Failed to switch language to {0}. Skipping.", localeLink.language), this);
						}
					}
					catch (Exception arg)
					{
						Debug.LogError(string.Format("Encountered error while updating localisation for {0} - {1} : {2}", localeLink.language, localeLink.locale, arg), this);
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				Language.SwitchLanguage(code);
			}
		}

		// Token: 0x04004DF4 RID: 19956
		public string devDisplayLocale;

		// Token: 0x04004DF5 RID: 19957
		[NamedArray("GetElementName")]
		public List<LocalizedStringEntry> localizedStrings = new List<LocalizedStringEntry>();

		// Token: 0x04004DF6 RID: 19958
		public List<XboxLocalizationData.LocaleLink> languages = new List<XboxLocalizationData.LocaleLink>();

		// Token: 0x04004DF7 RID: 19959
		[NonSerialized]
		private Dictionary<string, LocalizedStringEntry> localizedStringsLookup = new Dictionary<string, LocalizedStringEntry>();

		// Token: 0x04004DF8 RID: 19960
		[NonSerialized]
		private bool isValid;

		// Token: 0x02001B09 RID: 6921
		[Serializable]
		public class LocaleLink
		{
			// Token: 0x04009B79 RID: 39801
			public SupportedLanguages language;

			// Token: 0x04009B7A RID: 39802
			public string locale;
		}
	}
}
