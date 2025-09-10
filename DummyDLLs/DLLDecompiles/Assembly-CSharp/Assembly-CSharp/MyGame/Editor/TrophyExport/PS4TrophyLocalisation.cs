using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

namespace MyGame.Editor.TrophyExport
{
	// Token: 0x02000899 RID: 2201
	[CreateAssetMenu(menuName = "Localization/PS4 Trophy Localisation Data")]
	public sealed class PS4TrophyLocalisation : ScriptableObject
	{
		// Token: 0x06004C44 RID: 19524 RVA: 0x001676A4 File Offset: 0x001658A4
		[ContextMenu("Link Internal ID")]
		private void LinkInternalID()
		{
		}

		// Token: 0x04004D94 RID: 19860
		public string sheet = "Achievements";

		// Token: 0x04004D95 RID: 19861
		public LocalisedString title;

		// Token: 0x04004D96 RID: 19862
		public LocalisedString description;

		// Token: 0x04004D97 RID: 19863
		public List<PS4TrophyLocalisation.TrophyEntrySource> trophyEntries = new List<PS4TrophyLocalisation.TrophyEntrySource>();

		// Token: 0x04004D98 RID: 19864
		public AssetLinker<AchievementIDMap> achievementIdMap;

		// Token: 0x02001B02 RID: 6914
		[Serializable]
		public class TrophyEntrySource
		{
			// Token: 0x04009B51 RID: 39761
			public int id;

			// Token: 0x04009B52 RID: 39762
			public LocalisedString name;

			// Token: 0x04009B53 RID: 39763
			public LocalisedString description;
		}
	}
}
