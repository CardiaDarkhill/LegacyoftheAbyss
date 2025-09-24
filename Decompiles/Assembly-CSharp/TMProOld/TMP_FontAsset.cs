using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x0200080E RID: 2062
	[Serializable]
	public class TMP_FontAsset : TMP_Asset
	{
		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06004844 RID: 18500 RVA: 0x00150ABC File Offset: 0x0014ECBC
		public static TMP_FontAsset defaultFontAsset
		{
			get
			{
				if (TMP_FontAsset.s_defaultFontAsset == null)
				{
					TMP_FontAsset.s_defaultFontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/ARIAL SDF");
				}
				return TMP_FontAsset.s_defaultFontAsset;
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06004845 RID: 18501 RVA: 0x00150ADF File Offset: 0x0014ECDF
		public FaceInfo fontInfo
		{
			get
			{
				return this.m_fontInfo;
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x06004846 RID: 18502 RVA: 0x00150AE7 File Offset: 0x0014ECE7
		public Dictionary<int, TMP_Glyph> characterDictionary
		{
			get
			{
				if (this.m_characterDictionary == null)
				{
					this.ReadFontDefinition();
				}
				return this.m_characterDictionary;
			}
		}

		// Token: 0x1700083C RID: 2108
		// (get) Token: 0x06004847 RID: 18503 RVA: 0x00150AFD File Offset: 0x0014ECFD
		public Dictionary<int, KerningPair> kerningDictionary
		{
			get
			{
				return this.m_kerningDictionary;
			}
		}

		// Token: 0x1700083D RID: 2109
		// (get) Token: 0x06004848 RID: 18504 RVA: 0x00150B05 File Offset: 0x0014ED05
		public KerningTable kerningInfo
		{
			get
			{
				return this.m_kerningInfo;
			}
		}

		// Token: 0x06004849 RID: 18505 RVA: 0x00150B0D File Offset: 0x0014ED0D
		private void OnEnable()
		{
		}

		// Token: 0x0600484A RID: 18506 RVA: 0x00150B0F File Offset: 0x0014ED0F
		private void OnDisable()
		{
		}

		// Token: 0x0600484B RID: 18507 RVA: 0x00150B11 File Offset: 0x0014ED11
		public void AddFaceInfo(FaceInfo faceInfo)
		{
			this.m_fontInfo = faceInfo;
		}

		// Token: 0x0600484C RID: 18508 RVA: 0x00150B1C File Offset: 0x0014ED1C
		public void AddGlyphInfo(TMP_Glyph[] glyphInfo)
		{
			this.m_glyphInfoList = new List<TMP_Glyph>();
			int num = glyphInfo.Length;
			this.m_fontInfo.CharacterCount = num;
			this.m_characterSet = new int[num];
			for (int i = 0; i < num; i++)
			{
				TMP_Glyph tmp_Glyph = new TMP_Glyph();
				tmp_Glyph.id = glyphInfo[i].id;
				tmp_Glyph.x = glyphInfo[i].x;
				tmp_Glyph.y = glyphInfo[i].y;
				tmp_Glyph.width = glyphInfo[i].width;
				tmp_Glyph.height = glyphInfo[i].height;
				tmp_Glyph.xOffset = glyphInfo[i].xOffset;
				tmp_Glyph.yOffset = glyphInfo[i].yOffset;
				tmp_Glyph.xAdvance = glyphInfo[i].xAdvance;
				tmp_Glyph.scale = 1f;
				this.m_glyphInfoList.Add(tmp_Glyph);
				this.m_characterSet[i] = tmp_Glyph.id;
			}
			this.m_glyphInfoList = (from s in this.m_glyphInfoList
			orderby s.id
			select s).ToList<TMP_Glyph>();
		}

		// Token: 0x0600484D RID: 18509 RVA: 0x00150C32 File Offset: 0x0014EE32
		public void AddKerningInfo(KerningTable kerningTable)
		{
			this.m_kerningInfo = kerningTable;
		}

		// Token: 0x0600484E RID: 18510 RVA: 0x00150C3C File Offset: 0x0014EE3C
		public void ReadFontDefinition()
		{
			if (this.m_fontInfo == null)
			{
				return;
			}
			this.m_characterDictionary = new Dictionary<int, TMP_Glyph>();
			for (int i = 0; i < this.m_glyphInfoList.Count; i++)
			{
				TMP_Glyph tmp_Glyph = this.m_glyphInfoList[i];
				if (!this.m_characterDictionary.ContainsKey(tmp_Glyph.id))
				{
					this.m_characterDictionary.Add(tmp_Glyph.id, tmp_Glyph);
				}
				if (tmp_Glyph.scale == 0f)
				{
					tmp_Glyph.scale = 1f;
				}
			}
			TMP_Glyph tmp_Glyph2 = new TMP_Glyph();
			if (this.m_characterDictionary.ContainsKey(32))
			{
				this.m_characterDictionary[32].width = this.m_characterDictionary[32].xAdvance;
				this.m_characterDictionary[32].height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				this.m_characterDictionary[32].yOffset = this.m_fontInfo.Ascender;
				this.m_characterDictionary[32].scale = 1f;
			}
			else
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 32;
				tmp_Glyph2.x = 0f;
				tmp_Glyph2.y = 0f;
				tmp_Glyph2.width = this.m_fontInfo.Ascender / 5f;
				tmp_Glyph2.height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				tmp_Glyph2.xOffset = 0f;
				tmp_Glyph2.yOffset = this.m_fontInfo.Ascender;
				tmp_Glyph2.xAdvance = this.m_fontInfo.PointSize / 4f;
				tmp_Glyph2.scale = 1f;
				this.m_characterDictionary.Add(32, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(160))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				this.m_characterDictionary.Add(160, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(8203))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				tmp_Glyph2.width = 0f;
				tmp_Glyph2.xAdvance = 0f;
				this.m_characterDictionary.Add(8203, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(8288))
			{
				tmp_Glyph2 = TMP_Glyph.Clone(this.m_characterDictionary[32]);
				tmp_Glyph2.width = 0f;
				tmp_Glyph2.xAdvance = 0f;
				this.m_characterDictionary.Add(8288, tmp_Glyph2);
			}
			if (!this.m_characterDictionary.ContainsKey(10))
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 10;
				tmp_Glyph2.x = 0f;
				tmp_Glyph2.y = 0f;
				tmp_Glyph2.width = 10f;
				tmp_Glyph2.height = this.m_characterDictionary[32].height;
				tmp_Glyph2.xOffset = 0f;
				tmp_Glyph2.yOffset = this.m_characterDictionary[32].yOffset;
				tmp_Glyph2.xAdvance = 0f;
				tmp_Glyph2.scale = 1f;
				this.m_characterDictionary.Add(10, tmp_Glyph2);
				if (!this.m_characterDictionary.ContainsKey(13))
				{
					this.m_characterDictionary.Add(13, tmp_Glyph2);
				}
			}
			if (!this.m_characterDictionary.ContainsKey(9))
			{
				tmp_Glyph2 = new TMP_Glyph();
				tmp_Glyph2.id = 9;
				tmp_Glyph2.x = this.m_characterDictionary[32].x;
				tmp_Glyph2.y = this.m_characterDictionary[32].y;
				tmp_Glyph2.width = this.m_characterDictionary[32].width * (float)this.tabSize + (this.m_characterDictionary[32].xAdvance - this.m_characterDictionary[32].width) * (float)(this.tabSize - 1);
				tmp_Glyph2.height = this.m_characterDictionary[32].height;
				tmp_Glyph2.xOffset = this.m_characterDictionary[32].xOffset;
				tmp_Glyph2.yOffset = this.m_characterDictionary[32].yOffset;
				tmp_Glyph2.xAdvance = this.m_characterDictionary[32].xAdvance * (float)this.tabSize;
				tmp_Glyph2.scale = 1f;
				this.m_characterDictionary.Add(9, tmp_Glyph2);
			}
			this.m_fontInfo.TabWidth = this.m_characterDictionary[9].xAdvance;
			if (this.m_fontInfo.CapHeight == 0f && this.m_characterDictionary.ContainsKey(65))
			{
				this.m_fontInfo.CapHeight = this.m_characterDictionary[65].yOffset;
			}
			if (this.m_fontInfo.Scale == 0f)
			{
				this.m_fontInfo.Scale = 1f;
			}
			this.m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = this.m_kerningInfo.kerningPairs;
			for (int j = 0; j < kerningPairs.Count; j++)
			{
				KerningPair kerningPair = kerningPairs[j];
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!this.m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					this.m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
				else if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Kerning Key for [",
						kerningPairKey.ascii_Left.ToString(),
						"] and [",
						kerningPairKey.ascii_Right.ToString(),
						"] already exists."
					}));
				}
			}
			this.hashCode = TMP_TextUtilities.GetSimpleHashCode(base.name);
			this.materialHashCode = TMP_TextUtilities.GetSimpleHashCode(this.material.name);
		}

		// Token: 0x0600484F RID: 18511 RVA: 0x00151217 File Offset: 0x0014F417
		public bool HasCharacter(int character)
		{
			return this.m_characterDictionary != null && this.m_characterDictionary.ContainsKey(character);
		}

		// Token: 0x06004850 RID: 18512 RVA: 0x00151234 File Offset: 0x0014F434
		public bool HasCharacter(char character)
		{
			return this.m_characterDictionary != null && this.m_characterDictionary.ContainsKey((int)character);
		}

		// Token: 0x06004851 RID: 18513 RVA: 0x00151254 File Offset: 0x0014F454
		public bool HasCharacter(char character, bool searchFallbacks)
		{
			if (this.m_characterDictionary == null)
			{
				return false;
			}
			if (this.m_characterDictionary.ContainsKey((int)character))
			{
				return true;
			}
			if (searchFallbacks)
			{
				if (this.fallbackFontAssets != null && this.fallbackFontAssets.Count > 0)
				{
					for (int i = 0; i < this.fallbackFontAssets.Count; i++)
					{
						if (this.fallbackFontAssets[i].characterDictionary != null && this.fallbackFontAssets[i].characterDictionary.ContainsKey((int)character))
						{
							return true;
						}
					}
				}
				if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
				{
					for (int j = 0; j < TMP_Settings.fallbackFontAssets.Count; j++)
					{
						if (TMP_Settings.fallbackFontAssets[j].characterDictionary != null && TMP_Settings.fallbackFontAssets[j].characterDictionary.ContainsKey((int)character))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06004852 RID: 18514 RVA: 0x00151334 File Offset: 0x0014F534
		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			if (this.m_characterDictionary == null)
			{
				missingCharacters = null;
				return false;
			}
			missingCharacters = new List<char>();
			for (int i = 0; i < text.Length; i++)
			{
				if (!this.m_characterDictionary.ContainsKey((int)text[i]))
				{
					missingCharacters.Add(text[i]);
				}
			}
			return missingCharacters.Count == 0;
		}

		// Token: 0x06004853 RID: 18515 RVA: 0x00151394 File Offset: 0x0014F594
		public static string GetCharacters(TMP_FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				text += ((char)fontAsset.m_glyphInfoList[i].id).ToString();
			}
			return text;
		}

		// Token: 0x06004854 RID: 18516 RVA: 0x001513E0 File Offset: 0x0014F5E0
		public static int[] GetCharactersArray(TMP_FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.m_glyphInfoList.Count];
			for (int i = 0; i < fontAsset.m_glyphInfoList.Count; i++)
			{
				array[i] = fontAsset.m_glyphInfoList[i].id;
			}
			return array;
		}

		// Token: 0x040048A3 RID: 18595
		private static TMP_FontAsset s_defaultFontAsset;

		// Token: 0x040048A4 RID: 18596
		public TMP_FontAsset.FontAssetTypes fontAssetType;

		// Token: 0x040048A5 RID: 18597
		[SerializeField]
		private FaceInfo m_fontInfo;

		// Token: 0x040048A6 RID: 18598
		[SerializeField]
		public Texture2D atlas;

		// Token: 0x040048A7 RID: 18599
		[SerializeField]
		private List<TMP_Glyph> m_glyphInfoList;

		// Token: 0x040048A8 RID: 18600
		private Dictionary<int, TMP_Glyph> m_characterDictionary;

		// Token: 0x040048A9 RID: 18601
		private Dictionary<int, KerningPair> m_kerningDictionary;

		// Token: 0x040048AA RID: 18602
		[SerializeField]
		private KerningTable m_kerningInfo;

		// Token: 0x040048AB RID: 18603
		[SerializeField]
		private KerningPair m_kerningPair;

		// Token: 0x040048AC RID: 18604
		[SerializeField]
		public List<TMP_FontAsset> fallbackFontAssets;

		// Token: 0x040048AD RID: 18605
		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		// Token: 0x040048AE RID: 18606
		[SerializeField]
		public TMP_FontWeights[] fontWeights = new TMP_FontWeights[10];

		// Token: 0x040048AF RID: 18607
		private int[] m_characterSet;

		// Token: 0x040048B0 RID: 18608
		public float normalStyle;

		// Token: 0x040048B1 RID: 18609
		public float normalSpacingOffset;

		// Token: 0x040048B2 RID: 18610
		public float boldStyle = 0.75f;

		// Token: 0x040048B3 RID: 18611
		public float boldSpacing = 7f;

		// Token: 0x040048B4 RID: 18612
		public byte italicStyle = 35;

		// Token: 0x040048B5 RID: 18613
		public byte tabSize = 10;

		// Token: 0x040048B6 RID: 18614
		private byte m_oldTabSize;

		// Token: 0x02001ABE RID: 6846
		public enum FontAssetTypes
		{
			// Token: 0x04009A5C RID: 39516
			None,
			// Token: 0x04009A5D RID: 39517
			SDF,
			// Token: 0x04009A5E RID: 39518
			Bitmap
		}
	}
}
