using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008AD RID: 2221
	[CreateAssetMenu(menuName = "Localization/Xbox Achievement Database")]
	public sealed class XboxAchievementDatabase : ScriptableObject
	{
		// Token: 0x06004CD3 RID: 19667 RVA: 0x00168C07 File Offset: 0x00166E07
		private void OnEnable()
		{
			this.isValid = false;
		}

		// Token: 0x06004CD4 RID: 19668 RVA: 0x00168C10 File Offset: 0x00166E10
		private void OnValidate()
		{
			this.isValid = false;
		}

		// Token: 0x06004CD5 RID: 19669 RVA: 0x00168C1C File Offset: 0x00166E1C
		private void UpdateLookup()
		{
			if (this.isValid)
			{
				return;
			}
			this.achievementLookup.Clear();
			for (int i = 0; i < this.achievements.Count; i++)
			{
				AchievementEntry achievementEntry = this.achievements[i];
				this.achievementLookup[achievementEntry.stableGuid] = achievementEntry;
			}
			this.isValid = true;
		}

		// Token: 0x06004CD6 RID: 19670 RVA: 0x00168C7C File Offset: 0x00166E7C
		public AchievementEntry GetOrCreateEntryByGuid(string guid)
		{
			this.UpdateLookup();
			AchievementEntry result;
			if (this.achievementLookup.TryGetValue(guid, out result))
			{
				return result;
			}
			AchievementEntry achievementEntry = new AchievementEntry
			{
				stableGuid = guid
			};
			this.achievementLookup[guid] = achievementEntry;
			this.achievements.Add(achievementEntry);
			return achievementEntry;
		}

		// Token: 0x06004CD7 RID: 19671 RVA: 0x00168CC8 File Offset: 0x00166EC8
		private string GetElementName(int index)
		{
			try
			{
				AchievementEntry achievementEntry = this.achievements[index];
				return string.Format("{0}: {1} : {2}", index + 1, achievementEntry.internalAchievementID, achievementEntry.stableGuid);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			return string.Format("Element {0}", index);
		}

		// Token: 0x06004CD8 RID: 19672 RVA: 0x00168D2C File Offset: 0x00166F2C
		private static string PrintLocalisedString(LocalisedString localisedString)
		{
			return string.Concat(new string[]
			{
				"!!",
				localisedString.Sheet,
				"/",
				localisedString.Key,
				"!!"
			});
		}

		// Token: 0x06004CD9 RID: 19673 RVA: 0x00168D64 File Offset: 0x00166F64
		[ContextMenu("Export Internal ID to Localisation Asset")]
		private void ExportInternalIDToLocalisation()
		{
			XboxAchievementDatabase.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.localisation = this.xboxLocalizationData.Asset;
			if (CS$<>8__locals1.localisation)
			{
				foreach (AchievementEntry achievementEntry in this.achievements)
				{
					XboxAchievementDatabase.<>c__DisplayClass12_1 CS$<>8__locals2;
					CS$<>8__locals2.achievementEntry = achievementEntry;
					if (!string.IsNullOrEmpty(CS$<>8__locals2.achievementEntry.internalAchievementID))
					{
						XboxAchievementDatabase.<ExportInternalIDToLocalisation>g__AddLocalisation|12_0(CS$<>8__locals2.achievementEntry.achievementNameId, "_NAME", ref CS$<>8__locals1, ref CS$<>8__locals2);
						XboxAchievementDatabase.<ExportInternalIDToLocalisation>g__AddLocalisation|12_0(CS$<>8__locals2.achievementEntry.unlockedDescriptionId, "_DESC", ref CS$<>8__locals1, ref CS$<>8__locals2);
						XboxAchievementDatabase.<ExportInternalIDToLocalisation>g__AddLocalisation|12_0(CS$<>8__locals2.achievementEntry.lockedDescriptionId, "_DESC", ref CS$<>8__locals1, ref CS$<>8__locals2);
					}
				}
			}
		}

		// Token: 0x06004CDA RID: 19674 RVA: 0x00168E38 File Offset: 0x00167038
		[ContextMenu("Import Internal ID")]
		private void ImportInternalID()
		{
			if (!(this.xboxAchievementIDMap.Asset != null))
			{
				Debug.Log("Xbox id map not assigned");
			}
		}

		// Token: 0x06004CDC RID: 19676 RVA: 0x00168E78 File Offset: 0x00167078
		[CompilerGenerated]
		internal static void <ExportInternalIDToLocalisation>g__AddLocalisation|12_0(string xboxLocalisationID, string suffix, ref XboxAchievementDatabase.<>c__DisplayClass12_0 A_2, ref XboxAchievementDatabase.<>c__DisplayClass12_1 A_3)
		{
			string text = A_3.achievementEntry.internalAchievementID + suffix;
			LocalizedStringEntry localizedStringEntry;
			if (A_2.localisation.TryGetLocalizedStringEntry(xboxLocalisationID, out localizedStringEntry))
			{
				LocalisedString localisedString = new LocalisedString("Achievements", text);
				if (localizedStringEntry.localisedString != localisedString)
				{
					Debug.Log(string.Concat(new string[]
					{
						xboxLocalisationID,
						" localised string changed from ",
						XboxAchievementDatabase.PrintLocalisedString(localizedStringEntry.localisedString),
						" to ",
						XboxAchievementDatabase.PrintLocalisedString(localisedString)
					}), A_2.localisation);
					localizedStringEntry.localisedString = localisedString;
					return;
				}
			}
			else
			{
				Debug.LogError(string.Format("#{0} : {1} : Did not find {2} for {3} in {4}", new object[]
				{
					A_3.achievementEntry.achievementId,
					A_3.achievementEntry.internalAchievementID,
					xboxLocalisationID,
					text,
					A_2.localisation
				}), A_2.localisation);
			}
		}

		// Token: 0x04004DDD RID: 19933
		private const string SHEET = "Achievements";

		// Token: 0x04004DDE RID: 19934
		[NamedArray("GetElementName")]
		public List<AchievementEntry> achievements = new List<AchievementEntry>();

		// Token: 0x04004DDF RID: 19935
		public AssetLinker<XboxLocalizationData> xboxLocalizationData;

		// Token: 0x04004DE0 RID: 19936
		public AssetLinker<AchievementIDMap> xboxAchievementIDMap;

		// Token: 0x04004DE1 RID: 19937
		[NonSerialized]
		private Dictionary<string, AchievementEntry> achievementLookup = new Dictionary<string, AchievementEntry>();

		// Token: 0x04004DE2 RID: 19938
		[NonSerialized]
		private bool isValid;
	}
}
