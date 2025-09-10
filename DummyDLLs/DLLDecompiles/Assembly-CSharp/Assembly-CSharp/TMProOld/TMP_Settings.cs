using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000817 RID: 2071
	[ExecuteInEditMode]
	[Serializable]
	public class TMP_Settings : ScriptableObject
	{
		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06004912 RID: 18706 RVA: 0x00156171 File Offset: 0x00154371
		public static bool enableWordWrapping
		{
			get
			{
				return TMP_Settings.instance.m_enableWordWrapping;
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06004913 RID: 18707 RVA: 0x0015617D File Offset: 0x0015437D
		public static bool enableKerning
		{
			get
			{
				return TMP_Settings.instance.m_enableKerning;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06004914 RID: 18708 RVA: 0x00156189 File Offset: 0x00154389
		public static bool enableExtraPadding
		{
			get
			{
				return TMP_Settings.instance.m_enableExtraPadding;
			}
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06004915 RID: 18709 RVA: 0x00156195 File Offset: 0x00154395
		public static bool enableTintAllSprites
		{
			get
			{
				return TMP_Settings.instance.m_enableTintAllSprites;
			}
		}

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x06004916 RID: 18710 RVA: 0x001561A1 File Offset: 0x001543A1
		public static bool enableParseEscapeCharacters
		{
			get
			{
				return TMP_Settings.instance.m_enableParseEscapeCharacters;
			}
		}

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x06004917 RID: 18711 RVA: 0x001561AD File Offset: 0x001543AD
		public static int missingGlyphCharacter
		{
			get
			{
				return TMP_Settings.instance.m_missingGlyphCharacter;
			}
		}

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x06004918 RID: 18712 RVA: 0x001561B9 File Offset: 0x001543B9
		public static bool warningsDisabled
		{
			get
			{
				return TMP_Settings.instance.m_warningsDisabled;
			}
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x06004919 RID: 18713 RVA: 0x001561C5 File Offset: 0x001543C5
		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontAsset;
			}
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x0600491A RID: 18714 RVA: 0x001561D1 File Offset: 0x001543D1
		public static string defaultFontAssetPath
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontAssetPath;
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x0600491B RID: 18715 RVA: 0x001561DD File Offset: 0x001543DD
		public static float defaultFontSize
		{
			get
			{
				return TMP_Settings.instance.m_defaultFontSize;
			}
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x0600491C RID: 18716 RVA: 0x001561E9 File Offset: 0x001543E9
		public static float defaultTextContainerWidth
		{
			get
			{
				return TMP_Settings.instance.m_defaultTextContainerWidth;
			}
		}

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x0600491D RID: 18717 RVA: 0x001561F5 File Offset: 0x001543F5
		public static float defaultTextContainerHeight
		{
			get
			{
				return TMP_Settings.instance.m_defaultTextContainerHeight;
			}
		}

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x0600491E RID: 18718 RVA: 0x00156201 File Offset: 0x00154401
		public static List<TMP_FontAsset> fallbackFontAssets
		{
			get
			{
				return TMP_Settings.instance.m_fallbackFontAssets;
			}
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x0600491F RID: 18719 RVA: 0x0015620D File Offset: 0x0015440D
		public static bool matchMaterialPreset
		{
			get
			{
				return TMP_Settings.instance.m_matchMaterialPreset;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06004920 RID: 18720 RVA: 0x00156219 File Offset: 0x00154419
		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				return TMP_Settings.instance.m_defaultSpriteAsset;
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x06004921 RID: 18721 RVA: 0x00156225 File Offset: 0x00154425
		public static string defaultSpriteAssetPath
		{
			get
			{
				return TMP_Settings.instance.m_defaultSpriteAssetPath;
			}
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x06004922 RID: 18722 RVA: 0x00156231 File Offset: 0x00154431
		public static TMP_StyleSheet defaultStyleSheet
		{
			get
			{
				return TMP_Settings.instance.m_defaultStyleSheet;
			}
		}

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06004923 RID: 18723 RVA: 0x0015623D File Offset: 0x0015443D
		public static TextAsset leadingCharacters
		{
			get
			{
				return TMP_Settings.instance.m_leadingCharacters;
			}
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x06004924 RID: 18724 RVA: 0x00156249 File Offset: 0x00154449
		public static TextAsset followingCharacters
		{
			get
			{
				return TMP_Settings.instance.m_followingCharacters;
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x06004925 RID: 18725 RVA: 0x00156255 File Offset: 0x00154455
		public static TMP_Settings.LineBreakingTable linebreakingRules
		{
			get
			{
				if (TMP_Settings.instance.m_linebreakingRules == null)
				{
					TMP_Settings.LoadLinebreakingRules();
				}
				return TMP_Settings.instance.m_linebreakingRules;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x06004926 RID: 18726 RVA: 0x00156272 File Offset: 0x00154472
		public static TMP_Settings instance
		{
			get
			{
				if (TMP_Settings.s_Instance == null)
				{
					TMP_Settings.s_Instance = (Resources.Load("TMP Settings") as TMP_Settings);
				}
				return TMP_Settings.s_Instance;
			}
		}

		// Token: 0x06004927 RID: 18727 RVA: 0x0015629C File Offset: 0x0015449C
		public static TMP_Settings LoadDefaultSettings()
		{
			if (TMP_Settings.s_Instance == null)
			{
				TMP_Settings x = Resources.Load("TMP Settings") as TMP_Settings;
				if (x != null)
				{
					TMP_Settings.s_Instance = x;
				}
			}
			return TMP_Settings.s_Instance;
		}

		// Token: 0x06004928 RID: 18728 RVA: 0x001562DA File Offset: 0x001544DA
		public static TMP_Settings GetSettings()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance;
		}

		// Token: 0x06004929 RID: 18729 RVA: 0x001562F0 File Offset: 0x001544F0
		public static TMP_FontAsset GetFontAsset()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultFontAsset;
		}

		// Token: 0x0600492A RID: 18730 RVA: 0x0015630B File Offset: 0x0015450B
		public static TMP_SpriteAsset GetSpriteAsset()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultSpriteAsset;
		}

		// Token: 0x0600492B RID: 18731 RVA: 0x00156326 File Offset: 0x00154526
		public static TMP_StyleSheet GetStyleSheet()
		{
			if (TMP_Settings.instance == null)
			{
				return null;
			}
			return TMP_Settings.instance.m_defaultStyleSheet;
		}

		// Token: 0x0600492C RID: 18732 RVA: 0x00156344 File Offset: 0x00154544
		public static void LoadLinebreakingRules()
		{
			if (TMP_Settings.instance == null)
			{
				return;
			}
			if (TMP_Settings.s_Instance.m_linebreakingRules == null)
			{
				TMP_Settings.s_Instance.m_linebreakingRules = new TMP_Settings.LineBreakingTable();
			}
			TMP_Settings.s_Instance.m_linebreakingRules.leadingCharacters = TMP_Settings.GetCharacters(TMP_Settings.s_Instance.m_leadingCharacters);
			TMP_Settings.s_Instance.m_linebreakingRules.followingCharacters = TMP_Settings.GetCharacters(TMP_Settings.s_Instance.m_followingCharacters);
		}

		// Token: 0x0600492D RID: 18733 RVA: 0x001563B8 File Offset: 0x001545B8
		private static Dictionary<int, char> GetCharacters(TextAsset file)
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>();
			foreach (char c in file.text)
			{
				if (!dictionary.ContainsKey((int)c))
				{
					dictionary.Add((int)c, c);
				}
			}
			return dictionary;
		}

		// Token: 0x0400491A RID: 18714
		private static TMP_Settings s_Instance;

		// Token: 0x0400491B RID: 18715
		[SerializeField]
		private bool m_enableWordWrapping;

		// Token: 0x0400491C RID: 18716
		[SerializeField]
		private bool m_enableKerning;

		// Token: 0x0400491D RID: 18717
		[SerializeField]
		private bool m_enableExtraPadding;

		// Token: 0x0400491E RID: 18718
		[SerializeField]
		private bool m_enableTintAllSprites;

		// Token: 0x0400491F RID: 18719
		[SerializeField]
		private bool m_enableParseEscapeCharacters;

		// Token: 0x04004920 RID: 18720
		[SerializeField]
		private int m_missingGlyphCharacter;

		// Token: 0x04004921 RID: 18721
		[SerializeField]
		private bool m_warningsDisabled;

		// Token: 0x04004922 RID: 18722
		[SerializeField]
		private TMP_FontAsset m_defaultFontAsset;

		// Token: 0x04004923 RID: 18723
		[SerializeField]
		private string m_defaultFontAssetPath;

		// Token: 0x04004924 RID: 18724
		[SerializeField]
		private float m_defaultFontSize;

		// Token: 0x04004925 RID: 18725
		[SerializeField]
		private float m_defaultTextContainerWidth;

		// Token: 0x04004926 RID: 18726
		[SerializeField]
		private float m_defaultTextContainerHeight;

		// Token: 0x04004927 RID: 18727
		[SerializeField]
		private List<TMP_FontAsset> m_fallbackFontAssets;

		// Token: 0x04004928 RID: 18728
		[SerializeField]
		private bool m_matchMaterialPreset;

		// Token: 0x04004929 RID: 18729
		[SerializeField]
		private TMP_SpriteAsset m_defaultSpriteAsset;

		// Token: 0x0400492A RID: 18730
		[SerializeField]
		private string m_defaultSpriteAssetPath;

		// Token: 0x0400492B RID: 18731
		[SerializeField]
		private TMP_StyleSheet m_defaultStyleSheet;

		// Token: 0x0400492C RID: 18732
		[SerializeField]
		private TextAsset m_leadingCharacters;

		// Token: 0x0400492D RID: 18733
		[SerializeField]
		private TextAsset m_followingCharacters;

		// Token: 0x0400492E RID: 18734
		[SerializeField]
		private TMP_Settings.LineBreakingTable m_linebreakingRules;

		// Token: 0x02001AD1 RID: 6865
		public class LineBreakingTable
		{
			// Token: 0x04009A92 RID: 39570
			public Dictionary<int, char> leadingCharacters;

			// Token: 0x04009A93 RID: 39571
			public Dictionary<int, char> followingCharacters;
		}
	}
}
