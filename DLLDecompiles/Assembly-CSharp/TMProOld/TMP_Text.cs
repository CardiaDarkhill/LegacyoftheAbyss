using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x0200082A RID: 2090
	public class TMP_Text : MaskableGraphic
	{
		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x0600499F RID: 18847 RVA: 0x0015742D File Offset: 0x0015562D
		// (set) Token: 0x060049A0 RID: 18848 RVA: 0x00157435 File Offset: 0x00155635
		public string text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (this.m_text == value)
				{
					return;
				}
				this.m_text = value;
				this.m_inputSource = TMP_Text.TextInputSources.String;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060049A1 RID: 18849 RVA: 0x00157475 File Offset: 0x00155675
		// (set) Token: 0x060049A2 RID: 18850 RVA: 0x0015747D File Offset: 0x0015567D
		public bool isRightToLeftText
		{
			get
			{
				return this.m_isRightToLeft;
			}
			set
			{
				if (this.m_isRightToLeft == value)
				{
					return;
				}
				this.m_isRightToLeft = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060049A3 RID: 18851 RVA: 0x001574B1 File Offset: 0x001556B1
		// (set) Token: 0x060049A4 RID: 18852 RVA: 0x001574B9 File Offset: 0x001556B9
		public TMP_FontAsset font
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				if (this.m_fontAsset == value)
				{
					return;
				}
				this.m_fontAsset = value;
				this.LoadFontAsset();
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060049A5 RID: 18853 RVA: 0x001574F8 File Offset: 0x001556F8
		// (set) Token: 0x060049A6 RID: 18854 RVA: 0x00157500 File Offset: 0x00155700
		public virtual Material fontSharedMaterial
		{
			get
			{
				return this.m_sharedMaterial;
			}
			set
			{
				if (this.m_sharedMaterial == value)
				{
					return;
				}
				this.SetSharedMaterial(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x060049A7 RID: 18855 RVA: 0x00157532 File Offset: 0x00155732
		// (set) Token: 0x060049A8 RID: 18856 RVA: 0x0015753A File Offset: 0x0015573A
		public virtual Material[] fontSharedMaterials
		{
			get
			{
				return this.GetSharedMaterials();
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x060049A9 RID: 18857 RVA: 0x0015755D File Offset: 0x0015575D
		// (set) Token: 0x060049AA RID: 18858 RVA: 0x0015756C File Offset: 0x0015576C
		public Material fontMaterial
		{
			get
			{
				return this.GetMaterial(this.m_sharedMaterial);
			}
			set
			{
				if (this.m_sharedMaterial != null && this.m_sharedMaterial.GetInstanceID() == value.GetInstanceID())
				{
					return;
				}
				this.m_sharedMaterial = value;
				this.m_padding = this.GetPaddingForMaterial();
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x060049AB RID: 18859 RVA: 0x001575C8 File Offset: 0x001557C8
		// (set) Token: 0x060049AC RID: 18860 RVA: 0x001575D6 File Offset: 0x001557D6
		public virtual Material[] fontMaterials
		{
			get
			{
				return this.GetMaterials(this.m_fontSharedMaterials);
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x060049AD RID: 18861 RVA: 0x001575F9 File Offset: 0x001557F9
		// (set) Token: 0x060049AE RID: 18862 RVA: 0x00157601 File Offset: 0x00155801
		public override Color color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				if (this.m_fontColor == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_fontColor = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x060049AF RID: 18863 RVA: 0x00157626 File Offset: 0x00155826
		// (set) Token: 0x060049B0 RID: 18864 RVA: 0x00157633 File Offset: 0x00155833
		public float alpha
		{
			get
			{
				return this.m_fontColor.a;
			}
			set
			{
				if (this.m_fontColor.a == value)
				{
					return;
				}
				this.m_fontColor.a = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x060049B1 RID: 18865 RVA: 0x0015765D File Offset: 0x0015585D
		// (set) Token: 0x060049B2 RID: 18866 RVA: 0x00157665 File Offset: 0x00155865
		public bool enableVertexGradient
		{
			get
			{
				return this.m_enableVertexGradient;
			}
			set
			{
				if (this.m_enableVertexGradient == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableVertexGradient = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x060049B3 RID: 18867 RVA: 0x00157685 File Offset: 0x00155885
		// (set) Token: 0x060049B4 RID: 18868 RVA: 0x0015768D File Offset: 0x0015588D
		public VertexGradient colorGradient
		{
			get
			{
				return this.m_fontColorGradient;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradient = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x060049B5 RID: 18869 RVA: 0x001576A3 File Offset: 0x001558A3
		// (set) Token: 0x060049B6 RID: 18870 RVA: 0x001576AB File Offset: 0x001558AB
		public TMP_ColorGradient colorGradientPreset
		{
			get
			{
				return this.m_fontColorGradientPreset;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradientPreset = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x060049B7 RID: 18871 RVA: 0x001576C1 File Offset: 0x001558C1
		// (set) Token: 0x060049B8 RID: 18872 RVA: 0x001576C9 File Offset: 0x001558C9
		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.m_spriteAsset = value;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x060049B9 RID: 18873 RVA: 0x001576D2 File Offset: 0x001558D2
		// (set) Token: 0x060049BA RID: 18874 RVA: 0x001576DA File Offset: 0x001558DA
		public bool tintAllSprites
		{
			get
			{
				return this.m_tintAllSprites;
			}
			set
			{
				if (this.m_tintAllSprites == value)
				{
					return;
				}
				this.m_tintAllSprites = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x060049BB RID: 18875 RVA: 0x001576FA File Offset: 0x001558FA
		// (set) Token: 0x060049BC RID: 18876 RVA: 0x00157702 File Offset: 0x00155902
		public bool overrideColorTags
		{
			get
			{
				return this.m_overrideHtmlColors;
			}
			set
			{
				if (this.m_overrideHtmlColors == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_overrideHtmlColors = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x060049BD RID: 18877 RVA: 0x00157722 File Offset: 0x00155922
		// (set) Token: 0x060049BE RID: 18878 RVA: 0x0015775A File Offset: 0x0015595A
		public Color32 faceColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_faceColor;
				}
				this.m_faceColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
				return this.m_faceColor;
			}
			set
			{
				if (this.m_faceColor.Compare(value))
				{
					return;
				}
				this.SetFaceColor(value);
				this.m_havePropertiesChanged = true;
				this.m_faceColor = value;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x060049BF RID: 18879 RVA: 0x0015778C File Offset: 0x0015598C
		// (set) Token: 0x060049C0 RID: 18880 RVA: 0x001577C4 File Offset: 0x001559C4
		public Color32 outlineColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineColor;
				}
				this.m_outlineColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
				return this.m_outlineColor;
			}
			set
			{
				if (this.m_outlineColor.Compare(value))
				{
					return;
				}
				this.SetOutlineColor(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineColor = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x060049C1 RID: 18881 RVA: 0x001577F0 File Offset: 0x001559F0
		// (set) Token: 0x060049C2 RID: 18882 RVA: 0x00157823 File Offset: 0x00155A23
		public float outlineWidth
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineWidth;
				}
				this.m_outlineWidth = this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
				return this.m_outlineWidth;
			}
			set
			{
				if (this.m_outlineWidth == value)
				{
					return;
				}
				this.SetOutlineThickness(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineWidth = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x060049C3 RID: 18883 RVA: 0x0015784A File Offset: 0x00155A4A
		// (set) Token: 0x060049C4 RID: 18884 RVA: 0x00157854 File Offset: 0x00155A54
		public float fontSize
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				if (this.m_fontSize == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_fontSize = value;
				if (!this.m_enableAutoSizing)
				{
					this.m_fontSizeBase = this.m_fontSize;
				}
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x060049C5 RID: 18885 RVA: 0x001578A0 File Offset: 0x00155AA0
		public float fontScale
		{
			get
			{
				return this.m_fontScale;
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x060049C6 RID: 18886 RVA: 0x001578A8 File Offset: 0x00155AA8
		// (set) Token: 0x060049C7 RID: 18887 RVA: 0x001578B0 File Offset: 0x00155AB0
		public int fontWeight
		{
			get
			{
				return this.m_fontWeight;
			}
			set
			{
				if (this.m_fontWeight == value)
				{
					return;
				}
				this.m_fontWeight = value;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x060049C8 RID: 18888 RVA: 0x001578D8 File Offset: 0x00155AD8
		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				if (!canvas)
				{
					return 1f;
				}
				if (!this.font)
				{
					return canvas.scaleFactor;
				}
				if (this.m_currentFontAsset == null || this.m_currentFontAsset.fontInfo.PointSize <= 0f || this.m_fontSize <= 0f)
				{
					return 1f;
				}
				return this.m_fontSize / this.m_currentFontAsset.fontInfo.PointSize;
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x060049C9 RID: 18889 RVA: 0x0015795D File Offset: 0x00155B5D
		// (set) Token: 0x060049CA RID: 18890 RVA: 0x00157965 File Offset: 0x00155B65
		public bool enableAutoSizing
		{
			get
			{
				return this.m_enableAutoSizing;
			}
			set
			{
				if (this.m_enableAutoSizing == value)
				{
					return;
				}
				this.m_enableAutoSizing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x060049CB RID: 18891 RVA: 0x00157984 File Offset: 0x00155B84
		// (set) Token: 0x060049CC RID: 18892 RVA: 0x0015798C File Offset: 0x00155B8C
		public float fontSizeMin
		{
			get
			{
				return this.m_fontSizeMin;
			}
			set
			{
				if (this.m_fontSizeMin == value)
				{
					return;
				}
				this.m_fontSizeMin = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060049CD RID: 18893 RVA: 0x001579AB File Offset: 0x00155BAB
		// (set) Token: 0x060049CE RID: 18894 RVA: 0x001579B3 File Offset: 0x00155BB3
		public float fontSizeMax
		{
			get
			{
				return this.m_fontSizeMax;
			}
			set
			{
				if (this.m_fontSizeMax == value)
				{
					return;
				}
				this.m_fontSizeMax = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060049CF RID: 18895 RVA: 0x001579D2 File Offset: 0x00155BD2
		// (set) Token: 0x060049D0 RID: 18896 RVA: 0x001579DA File Offset: 0x00155BDA
		public FontStyles fontStyle
		{
			get
			{
				return this.m_fontStyle;
			}
			set
			{
				if (this.m_fontStyle == value)
				{
					return;
				}
				this.m_fontStyle = value;
				this.m_havePropertiesChanged = true;
				this.checkPaddingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060049D1 RID: 18897 RVA: 0x00157A07 File Offset: 0x00155C07
		public bool isUsingBold
		{
			get
			{
				return this.m_isUsingBold;
			}
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060049D2 RID: 18898 RVA: 0x00157A0F File Offset: 0x00155C0F
		// (set) Token: 0x060049D3 RID: 18899 RVA: 0x00157A17 File Offset: 0x00155C17
		public TextAlignmentOptions alignment
		{
			get
			{
				return this.m_textAlignment;
			}
			set
			{
				if (this.m_textAlignment == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_textAlignment = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060049D4 RID: 18900 RVA: 0x00157A37 File Offset: 0x00155C37
		// (set) Token: 0x060049D5 RID: 18901 RVA: 0x00157A3F File Offset: 0x00155C3F
		public float characterSpacing
		{
			get
			{
				return this.m_characterSpacing;
			}
			set
			{
				if (this.m_characterSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_characterSpacing = value;
			}
		}

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060049D6 RID: 18902 RVA: 0x00157A6C File Offset: 0x00155C6C
		// (set) Token: 0x060049D7 RID: 18903 RVA: 0x00157A74 File Offset: 0x00155C74
		public float lineSpacing
		{
			get
			{
				return this.m_lineSpacing;
			}
			set
			{
				if (this.m_lineSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_lineSpacing = value;
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060049D8 RID: 18904 RVA: 0x00157AA1 File Offset: 0x00155CA1
		// (set) Token: 0x060049D9 RID: 18905 RVA: 0x00157AA9 File Offset: 0x00155CA9
		public float paragraphSpacing
		{
			get
			{
				return this.m_paragraphSpacing;
			}
			set
			{
				if (this.m_paragraphSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_paragraphSpacing = value;
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060049DA RID: 18906 RVA: 0x00157AD6 File Offset: 0x00155CD6
		// (set) Token: 0x060049DB RID: 18907 RVA: 0x00157ADE File Offset: 0x00155CDE
		public float characterWidthAdjustment
		{
			get
			{
				return this.m_charWidthMaxAdj;
			}
			set
			{
				if (this.m_charWidthMaxAdj == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_charWidthMaxAdj = value;
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060049DC RID: 18908 RVA: 0x00157B0B File Offset: 0x00155D0B
		// (set) Token: 0x060049DD RID: 18909 RVA: 0x00157B13 File Offset: 0x00155D13
		public bool enableWordWrapping
		{
			get
			{
				return this.m_enableWordWrapping;
			}
			set
			{
				if (this.m_enableWordWrapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.m_isCalculateSizeRequired = true;
				this.m_enableWordWrapping = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060049DE RID: 18910 RVA: 0x00157B47 File Offset: 0x00155D47
		// (set) Token: 0x060049DF RID: 18911 RVA: 0x00157B4F File Offset: 0x00155D4F
		public float wordWrappingRatios
		{
			get
			{
				return this.m_wordWrappingRatios;
			}
			set
			{
				if (this.m_wordWrappingRatios == value)
				{
					return;
				}
				this.m_wordWrappingRatios = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060049E0 RID: 18912 RVA: 0x00157B7C File Offset: 0x00155D7C
		// (set) Token: 0x060049E1 RID: 18913 RVA: 0x00157B84 File Offset: 0x00155D84
		public bool enableAdaptiveJustification
		{
			get
			{
				return this.m_enableAdaptiveJustification;
			}
			set
			{
				if (this.m_enableAdaptiveJustification == value)
				{
					return;
				}
				this.m_enableAdaptiveJustification = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060049E2 RID: 18914 RVA: 0x00157BB1 File Offset: 0x00155DB1
		// (set) Token: 0x060049E3 RID: 18915 RVA: 0x00157BB9 File Offset: 0x00155DB9
		public TextOverflowModes OverflowMode
		{
			get
			{
				return this.m_overflowMode;
			}
			set
			{
				if (this.m_overflowMode == value)
				{
					return;
				}
				this.m_overflowMode = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060049E4 RID: 18916 RVA: 0x00157BE6 File Offset: 0x00155DE6
		// (set) Token: 0x060049E5 RID: 18917 RVA: 0x00157BEE File Offset: 0x00155DEE
		public bool enableKerning
		{
			get
			{
				return this.m_enableKerning;
			}
			set
			{
				if (this.m_enableKerning == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_enableKerning = value;
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060049E6 RID: 18918 RVA: 0x00157C1B File Offset: 0x00155E1B
		// (set) Token: 0x060049E7 RID: 18919 RVA: 0x00157C23 File Offset: 0x00155E23
		public bool extraPadding
		{
			get
			{
				return this.m_enableExtraPadding;
			}
			set
			{
				if (this.m_enableExtraPadding == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableExtraPadding = value;
				this.UpdateMeshPadding();
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060049E8 RID: 18920 RVA: 0x00157C49 File Offset: 0x00155E49
		// (set) Token: 0x060049E9 RID: 18921 RVA: 0x00157C51 File Offset: 0x00155E51
		public bool richText
		{
			get
			{
				return this.m_isRichText;
			}
			set
			{
				if (this.m_isRichText == value)
				{
					return;
				}
				this.m_isRichText = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_isInputParsingRequired = true;
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060049EA RID: 18922 RVA: 0x00157C85 File Offset: 0x00155E85
		// (set) Token: 0x060049EB RID: 18923 RVA: 0x00157C8D File Offset: 0x00155E8D
		public bool parseCtrlCharacters
		{
			get
			{
				return this.m_parseCtrlCharacters;
			}
			set
			{
				if (this.m_parseCtrlCharacters == value)
				{
					return;
				}
				this.m_parseCtrlCharacters = value;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
				this.m_isInputParsingRequired = true;
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060049EC RID: 18924 RVA: 0x00157CC1 File Offset: 0x00155EC1
		// (set) Token: 0x060049ED RID: 18925 RVA: 0x00157CC9 File Offset: 0x00155EC9
		public bool isOverlay
		{
			get
			{
				return this.m_isOverlay;
			}
			set
			{
				if (this.m_isOverlay == value)
				{
					return;
				}
				this.m_isOverlay = value;
				this.SetShaderDepth();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x060049EE RID: 18926 RVA: 0x00157CEF File Offset: 0x00155EEF
		// (set) Token: 0x060049EF RID: 18927 RVA: 0x00157CF7 File Offset: 0x00155EF7
		public bool isOrthographic
		{
			get
			{
				return this.m_isOrthographic;
			}
			set
			{
				if (this.m_isOrthographic == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isOrthographic = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x060049F0 RID: 18928 RVA: 0x00157D17 File Offset: 0x00155F17
		// (set) Token: 0x060049F1 RID: 18929 RVA: 0x00157D1F File Offset: 0x00155F1F
		public bool enableCulling
		{
			get
			{
				return this.m_isCullingEnabled;
			}
			set
			{
				if (this.m_isCullingEnabled == value)
				{
					return;
				}
				this.m_isCullingEnabled = value;
				this.SetCulling();
				this.m_havePropertiesChanged = true;
			}
		}

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x060049F2 RID: 18930 RVA: 0x00157D3F File Offset: 0x00155F3F
		// (set) Token: 0x060049F3 RID: 18931 RVA: 0x00157D47 File Offset: 0x00155F47
		public bool ignoreVisibility
		{
			get
			{
				return this.m_ignoreCulling;
			}
			set
			{
				if (this.m_ignoreCulling == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_ignoreCulling = value;
			}
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x060049F4 RID: 18932 RVA: 0x00157D61 File Offset: 0x00155F61
		// (set) Token: 0x060049F5 RID: 18933 RVA: 0x00157D69 File Offset: 0x00155F69
		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return this.m_horizontalMapping;
			}
			set
			{
				if (this.m_horizontalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_horizontalMapping = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x060049F6 RID: 18934 RVA: 0x00157D89 File Offset: 0x00155F89
		// (set) Token: 0x060049F7 RID: 18935 RVA: 0x00157D91 File Offset: 0x00155F91
		public TextureMappingOptions verticalMapping
		{
			get
			{
				return this.m_verticalMapping;
			}
			set
			{
				if (this.m_verticalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_verticalMapping = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x060049F8 RID: 18936 RVA: 0x00157DB1 File Offset: 0x00155FB1
		// (set) Token: 0x060049F9 RID: 18937 RVA: 0x00157DB9 File Offset: 0x00155FB9
		public TextRenderFlags renderMode
		{
			get
			{
				return this.m_renderMode;
			}
			set
			{
				if (this.m_renderMode == value)
				{
					return;
				}
				this.m_renderMode = value;
				this.m_havePropertiesChanged = true;
			}
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060049FA RID: 18938 RVA: 0x00157DD3 File Offset: 0x00155FD3
		// (set) Token: 0x060049FB RID: 18939 RVA: 0x00157DDB File Offset: 0x00155FDB
		public int maxVisibleCharacters
		{
			get
			{
				return this.m_maxVisibleCharacters;
			}
			set
			{
				if (this.m_maxVisibleCharacters == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleCharacters = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x060049FC RID: 18940 RVA: 0x00157DFB File Offset: 0x00155FFB
		// (set) Token: 0x060049FD RID: 18941 RVA: 0x00157E03 File Offset: 0x00156003
		public int maxVisibleWords
		{
			get
			{
				return this.m_maxVisibleWords;
			}
			set
			{
				if (this.m_maxVisibleWords == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleWords = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x060049FE RID: 18942 RVA: 0x00157E23 File Offset: 0x00156023
		// (set) Token: 0x060049FF RID: 18943 RVA: 0x00157E2B File Offset: 0x0015602B
		public int maxVisibleLines
		{
			get
			{
				return this.m_maxVisibleLines;
			}
			set
			{
				if (this.m_maxVisibleLines == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.m_maxVisibleLines = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06004A00 RID: 18944 RVA: 0x00157E52 File Offset: 0x00156052
		// (set) Token: 0x06004A01 RID: 18945 RVA: 0x00157E5A File Offset: 0x0015605A
		public bool useMaxVisibleDescender
		{
			get
			{
				return this.m_useMaxVisibleDescender;
			}
			set
			{
				if (this.m_useMaxVisibleDescender == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x06004A02 RID: 18946 RVA: 0x00157E7A File Offset: 0x0015607A
		// (set) Token: 0x06004A03 RID: 18947 RVA: 0x00157E82 File Offset: 0x00156082
		public int pageToDisplay
		{
			get
			{
				return this.m_pageToDisplay;
			}
			set
			{
				if (this.m_pageToDisplay == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_pageToDisplay = value;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06004A04 RID: 18948 RVA: 0x00157EA2 File Offset: 0x001560A2
		// (set) Token: 0x06004A05 RID: 18949 RVA: 0x00157EAA File Offset: 0x001560AA
		public virtual Vector4 margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				if (this.m_margin == value)
				{
					return;
				}
				this.m_margin = value;
				this.ComputeMarginSize();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x06004A06 RID: 18950 RVA: 0x00157ED5 File Offset: 0x001560D5
		public TMP_TextInfo textInfo
		{
			get
			{
				return this.m_textInfo;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06004A07 RID: 18951 RVA: 0x00157EDD File Offset: 0x001560DD
		// (set) Token: 0x06004A08 RID: 18952 RVA: 0x00157EE5 File Offset: 0x001560E5
		public bool havePropertiesChanged
		{
			get
			{
				return this.m_havePropertiesChanged;
			}
			set
			{
				if (this.m_havePropertiesChanged == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.m_isInputParsingRequired = true;
				this.SetAllDirty();
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06004A09 RID: 18953 RVA: 0x00157F05 File Offset: 0x00156105
		// (set) Token: 0x06004A0A RID: 18954 RVA: 0x00157F0D File Offset: 0x0015610D
		public bool isUsingLegacyAnimationComponent
		{
			get
			{
				return this.m_isUsingLegacyAnimationComponent;
			}
			set
			{
				this.m_isUsingLegacyAnimationComponent = value;
			}
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06004A0B RID: 18955 RVA: 0x00157F16 File Offset: 0x00156116
		public new Transform transform
		{
			get
			{
				if (this.m_transform == null)
				{
					this.m_transform = base.GetComponent<Transform>();
				}
				return this.m_transform;
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06004A0C RID: 18956 RVA: 0x00157F38 File Offset: 0x00156138
		public new RectTransform rectTransform
		{
			get
			{
				if (this.m_rectTransform == null)
				{
					this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06004A0D RID: 18957 RVA: 0x00157F5A File Offset: 0x0015615A
		// (set) Token: 0x06004A0E RID: 18958 RVA: 0x00157F62 File Offset: 0x00156162
		public virtual bool autoSizeTextContainer { get; set; }

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06004A0F RID: 18959 RVA: 0x00157F6B File Offset: 0x0015616B
		public virtual Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06004A10 RID: 18960 RVA: 0x00157F73 File Offset: 0x00156173
		// (set) Token: 0x06004A11 RID: 18961 RVA: 0x00157F7B File Offset: 0x0015617B
		public bool isVolumetricText
		{
			get
			{
				return this.m_isVolumetricText;
			}
			set
			{
				if (this.m_isVolumetricText == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.m_textInfo.ResetVertexLayout(value);
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06004A12 RID: 18962 RVA: 0x00157FB0 File Offset: 0x001561B0
		public Bounds bounds
		{
			get
			{
				if (this.m_mesh == null)
				{
					return default(Bounds);
				}
				return this.GetCompoundBounds();
			}
		}

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06004A13 RID: 18963 RVA: 0x00157FDC File Offset: 0x001561DC
		public Bounds textBounds
		{
			get
			{
				if (this.m_textInfo == null)
				{
					return default(Bounds);
				}
				return this.GetTextBounds();
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06004A14 RID: 18964 RVA: 0x00158001 File Offset: 0x00156201
		public float flexibleHeight
		{
			get
			{
				return this.m_flexibleHeight;
			}
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06004A15 RID: 18965 RVA: 0x00158009 File Offset: 0x00156209
		public float flexibleWidth
		{
			get
			{
				return this.m_flexibleWidth;
			}
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06004A16 RID: 18966 RVA: 0x00158011 File Offset: 0x00156211
		public float minHeight
		{
			get
			{
				return this.m_minHeight;
			}
		}

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06004A17 RID: 18967 RVA: 0x00158019 File Offset: 0x00156219
		public float minWidth
		{
			get
			{
				return this.m_minWidth;
			}
		}

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06004A18 RID: 18968 RVA: 0x00158021 File Offset: 0x00156221
		public virtual float preferredWidth
		{
			get
			{
				if (!this.m_isPreferredWidthDirty)
				{
					return this.m_preferredWidth;
				}
				this.m_preferredWidth = this.GetPreferredWidth();
				return this.m_preferredWidth;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06004A19 RID: 18969 RVA: 0x00158044 File Offset: 0x00156244
		public virtual float preferredHeight
		{
			get
			{
				if (!this.m_isPreferredHeightDirty)
				{
					return this.m_preferredHeight;
				}
				this.m_preferredHeight = this.GetPreferredHeight();
				return this.m_preferredHeight;
			}
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06004A1A RID: 18970 RVA: 0x00158067 File Offset: 0x00156267
		public virtual float renderedWidth
		{
			get
			{
				return this.GetRenderedWidth();
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06004A1B RID: 18971 RVA: 0x0015806F File Offset: 0x0015626F
		public virtual float renderedHeight
		{
			get
			{
				return this.GetRenderedHeight();
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06004A1C RID: 18972 RVA: 0x00158077 File Offset: 0x00156277
		public int layoutPriority
		{
			get
			{
				return this.m_layoutPriority;
			}
		}

		// Token: 0x06004A1D RID: 18973 RVA: 0x0015807F File Offset: 0x0015627F
		protected virtual void LoadFontAsset()
		{
		}

		// Token: 0x06004A1E RID: 18974 RVA: 0x00158081 File Offset: 0x00156281
		protected virtual void SetSharedMaterial(Material mat)
		{
		}

		// Token: 0x06004A1F RID: 18975 RVA: 0x00158083 File Offset: 0x00156283
		protected virtual Material GetMaterial(Material mat)
		{
			return null;
		}

		// Token: 0x06004A20 RID: 18976 RVA: 0x00158086 File Offset: 0x00156286
		protected virtual void SetFontBaseMaterial(Material mat)
		{
		}

		// Token: 0x06004A21 RID: 18977 RVA: 0x00158088 File Offset: 0x00156288
		protected virtual Material[] GetSharedMaterials()
		{
			return null;
		}

		// Token: 0x06004A22 RID: 18978 RVA: 0x0015808B File Offset: 0x0015628B
		protected virtual void SetSharedMaterials(Material[] materials)
		{
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x0015808D File Offset: 0x0015628D
		protected virtual Material[] GetMaterials(Material[] mats)
		{
			return null;
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x00158090 File Offset: 0x00156290
		protected virtual Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x001580BC File Offset: 0x001562BC
		protected void SetVertexColorGradient(TMP_ColorGradient gradient)
		{
			if (gradient == null)
			{
				return;
			}
			this.m_fontColorGradient.bottomLeft = gradient.bottomLeft;
			this.m_fontColorGradient.bottomRight = gradient.bottomRight;
			this.m_fontColorGradient.topLeft = gradient.topLeft;
			this.m_fontColorGradient.topRight = gradient.topRight;
			this.SetVerticesDirty();
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x0015811D File Offset: 0x0015631D
		protected virtual void SetFaceColor(Color32 color)
		{
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x0015811F File Offset: 0x0015631F
		protected virtual void SetOutlineColor(Color32 color)
		{
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x00158121 File Offset: 0x00156321
		protected virtual void SetOutlineThickness(float thickness)
		{
		}

		// Token: 0x06004A29 RID: 18985 RVA: 0x00158123 File Offset: 0x00156323
		protected virtual void SetShaderDepth()
		{
		}

		// Token: 0x06004A2A RID: 18986 RVA: 0x00158125 File Offset: 0x00156325
		protected virtual void SetCulling()
		{
		}

		// Token: 0x06004A2B RID: 18987 RVA: 0x00158127 File Offset: 0x00156327
		protected virtual float GetPaddingForMaterial()
		{
			return 0f;
		}

		// Token: 0x06004A2C RID: 18988 RVA: 0x0015812E File Offset: 0x0015632E
		protected virtual float GetPaddingForMaterial(Material mat)
		{
			return 0f;
		}

		// Token: 0x06004A2D RID: 18989 RVA: 0x00158135 File Offset: 0x00156335
		protected virtual Vector3[] GetTextContainerLocalCorners()
		{
			return null;
		}

		// Token: 0x06004A2E RID: 18990 RVA: 0x00158138 File Offset: 0x00156338
		public virtual void ForceMeshUpdate()
		{
		}

		// Token: 0x06004A2F RID: 18991 RVA: 0x0015813A File Offset: 0x0015633A
		public virtual void ForceMeshUpdate(bool ignoreActiveState)
		{
		}

		// Token: 0x06004A30 RID: 18992 RVA: 0x0015813C File Offset: 0x0015633C
		internal void SetTextInternal(string text)
		{
			this.m_text = text;
			this.m_renderMode = TextRenderFlags.DontRender;
			this.m_isInputParsingRequired = true;
			this.ForceMeshUpdate();
			this.m_renderMode = TextRenderFlags.Render;
		}

		// Token: 0x06004A31 RID: 18993 RVA: 0x00158164 File Offset: 0x00156364
		public virtual void UpdateGeometry(Mesh mesh, int index)
		{
		}

		// Token: 0x06004A32 RID: 18994 RVA: 0x00158166 File Offset: 0x00156366
		public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
		}

		// Token: 0x06004A33 RID: 18995 RVA: 0x00158168 File Offset: 0x00156368
		public virtual void UpdateVertexData()
		{
		}

		// Token: 0x06004A34 RID: 18996 RVA: 0x0015816A File Offset: 0x0015636A
		public virtual void SetVertices(Vector3[] vertices)
		{
		}

		// Token: 0x06004A35 RID: 18997 RVA: 0x0015816C File Offset: 0x0015636C
		public virtual void UpdateMeshPadding()
		{
		}

		// Token: 0x06004A36 RID: 18998 RVA: 0x0015816E File Offset: 0x0015636E
		public new void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			this.InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}

		// Token: 0x06004A37 RID: 18999 RVA: 0x00158186 File Offset: 0x00156386
		public new void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			base.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			this.InternalCrossFadeAlpha(alpha, duration, ignoreTimeScale);
		}

		// Token: 0x06004A38 RID: 19000 RVA: 0x0015819A File Offset: 0x0015639A
		protected virtual void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
		}

		// Token: 0x06004A39 RID: 19001 RVA: 0x0015819C File Offset: 0x0015639C
		protected virtual void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
		}

		// Token: 0x06004A3A RID: 19002 RVA: 0x001581A0 File Offset: 0x001563A0
		protected void ParseInputText()
		{
			this.m_isInputParsingRequired = false;
			switch (this.m_inputSource)
			{
			case TMP_Text.TextInputSources.Text:
			case TMP_Text.TextInputSources.String:
				this.StringToCharArray(this.m_text, ref this.m_char_buffer);
				break;
			case TMP_Text.TextInputSources.SetText:
				this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
				break;
			}
			this.SetArraySizes(this.m_char_buffer);
		}

		// Token: 0x06004A3B RID: 19003 RVA: 0x00158206 File Offset: 0x00156406
		public void SetText(string text)
		{
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x06004A3C RID: 19004 RVA: 0x0015823D File Offset: 0x0015643D
		public void SetText(string text, float arg0)
		{
			this.SetText(text, arg0, 255f, 255f);
		}

		// Token: 0x06004A3D RID: 19005 RVA: 0x00158251 File Offset: 0x00156451
		public void SetText(string text, float arg0, float arg1)
		{
			this.SetText(text, arg0, arg1, 255f);
		}

		// Token: 0x06004A3E RID: 19006 RVA: 0x00158264 File Offset: 0x00156464
		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (text == this.old_text && arg0 == this.old_arg0 && arg1 == this.old_arg1 && arg2 == this.old_arg2)
			{
				return;
			}
			this.old_text = text;
			this.old_arg1 = 255f;
			this.old_arg2 = 255f;
			int precision = 0;
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '{')
				{
					if (text[i + 2] == ':')
					{
						precision = (int)(text[i + 3] - '0');
					}
					switch (text[i + 1])
					{
					case '0':
						this.old_arg0 = arg0;
						this.AddFloatToCharArray(arg0, ref num, precision);
						break;
					case '1':
						this.old_arg1 = arg1;
						this.AddFloatToCharArray(arg1, ref num, precision);
						break;
					case '2':
						this.old_arg2 = arg2;
						this.AddFloatToCharArray(arg2, ref num, precision);
						break;
					}
					if (text[i + 2] == ':')
					{
						i += 4;
					}
					else
					{
						i += 2;
					}
				}
				else
				{
					this.m_input_CharArray[num] = c;
					num++;
				}
			}
			this.m_input_CharArray[num] = '\0';
			this.m_charArray_Length = num;
			this.m_inputSource = TMP_Text.TextInputSources.SetText;
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x06004A3F RID: 19007 RVA: 0x001583B8 File Offset: 0x001565B8
		public void SetText(StringBuilder text)
		{
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.StringBuilderToIntArray(text, ref this.m_char_buffer);
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x06004A40 RID: 19008 RVA: 0x001583F0 File Offset: 0x001565F0
		public void SetCharArray(char[] charArray)
		{
			if (charArray == null || charArray.Length == 0)
			{
				return;
			}
			if (this.m_char_buffer.Length <= charArray.Length)
			{
				int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
				this.m_char_buffer = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < charArray.Length)
			{
				if (charArray[i] != '\\' || i >= charArray.Length - 1)
				{
					goto IL_94;
				}
				int num3 = (int)charArray[i + 1];
				if (num3 != 110)
				{
					if (num3 != 114)
					{
						if (num3 != 116)
						{
							goto IL_94;
						}
						this.m_char_buffer[num2] = 9;
						i++;
						num2++;
					}
					else
					{
						this.m_char_buffer[num2] = 13;
						i++;
						num2++;
					}
				}
				else
				{
					this.m_char_buffer[num2] = 10;
					i++;
					num2++;
				}
				IL_A3:
				i++;
				continue;
				IL_94:
				this.m_char_buffer[num2] = (int)charArray[i];
				num2++;
				goto IL_A3;
			}
			this.m_char_buffer[num2] = 0;
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.m_havePropertiesChanged = true;
			this.m_isInputParsingRequired = true;
		}

		// Token: 0x06004A41 RID: 19009 RVA: 0x001584C8 File Offset: 0x001566C8
		protected void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray == null || this.m_charArray_Length == 0)
			{
				return;
			}
			if (charBuffer.Length <= this.m_charArray_Length)
			{
				int num = (this.m_charArray_Length > 1024) ? (this.m_charArray_Length + 256) : Mathf.NextPowerOfTwo(this.m_charArray_Length + 1);
				charBuffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < this.m_charArray_Length; i++)
			{
				if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
				{
					charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
					i++;
					num2++;
				}
				else
				{
					charBuffer[num2] = (int)charArray[i];
					num2++;
				}
			}
			charBuffer[num2] = 0;
		}

		// Token: 0x06004A42 RID: 19010 RVA: 0x00158574 File Offset: 0x00156774
		protected void StringToCharArray(string text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = (text.Length > 1024) ? (text.Length + 256) : Mathf.NextPowerOfTwo(text.Length + 1);
				chars = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < text.Length)
			{
				if (this.m_inputSource != TMP_Text.TextInputSources.Text || text[i] != '\\' || text.Length <= i + 1)
				{
					goto IL_19B;
				}
				int num3 = (int)text[i + 1];
				if (num3 <= 92)
				{
					if (num3 != 85)
					{
						if (num3 != 92)
						{
							goto IL_19B;
						}
						if (!this.m_parseCtrlCharacters || text.Length <= i + 2)
						{
							goto IL_19B;
						}
						chars[num2] = (int)text[i + 1];
						chars[num2 + 1] = (int)text[i + 2];
						i += 2;
						num2 += 2;
					}
					else
					{
						if (text.Length <= i + 9)
						{
							goto IL_19B;
						}
						chars[num2] = this.GetUTF32(i + 2);
						i += 9;
						num2++;
					}
				}
				else if (num3 != 110)
				{
					switch (num3)
					{
					case 114:
						if (!this.m_parseCtrlCharacters)
						{
							goto IL_19B;
						}
						chars[num2] = 13;
						i++;
						num2++;
						break;
					case 115:
						goto IL_19B;
					case 116:
						if (!this.m_parseCtrlCharacters)
						{
							goto IL_19B;
						}
						chars[num2] = 9;
						i++;
						num2++;
						break;
					case 117:
						if (text.Length <= i + 5)
						{
							goto IL_19B;
						}
						chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
						i += 5;
						num2++;
						break;
					default:
						goto IL_19B;
					}
				}
				else
				{
					if (!this.m_parseCtrlCharacters)
					{
						goto IL_19B;
					}
					chars[num2] = 10;
					i++;
					num2++;
				}
				IL_1EB:
				i++;
				continue;
				IL_19B:
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
					goto IL_1EB;
				}
				chars[num2] = (int)text[i];
				num2++;
				goto IL_1EB;
			}
			chars[num2] = 0;
		}

		// Token: 0x06004A43 RID: 19011 RVA: 0x00158784 File Offset: 0x00156984
		protected void StringBuilderToIntArray(StringBuilder text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars == null || chars.Length <= text.Length)
			{
				int num = (text.Length > 1024) ? (text.Length + 256) : Mathf.NextPowerOfTwo(text.Length + 1);
				chars = new int[num];
			}
			int num2 = 0;
			int i = 0;
			while (i < text.Length)
			{
				if (!this.m_parseCtrlCharacters || text[i] != '\\' || text.Length <= i + 1)
				{
					goto IL_175;
				}
				int num3 = (int)text[i + 1];
				if (num3 <= 92)
				{
					if (num3 != 85)
					{
						if (num3 != 92)
						{
							goto IL_175;
						}
						if (text.Length <= i + 2)
						{
							goto IL_175;
						}
						chars[num2] = (int)text[i + 1];
						chars[num2 + 1] = (int)text[i + 2];
						i += 2;
						num2 += 2;
					}
					else
					{
						if (text.Length <= i + 9)
						{
							goto IL_175;
						}
						chars[num2] = this.GetUTF32(i + 2);
						i += 9;
						num2++;
					}
				}
				else if (num3 != 110)
				{
					switch (num3)
					{
					case 114:
						chars[num2] = 13;
						i++;
						num2++;
						break;
					case 115:
						goto IL_175;
					case 116:
						chars[num2] = 9;
						i++;
						num2++;
						break;
					case 117:
						if (text.Length <= i + 5)
						{
							goto IL_175;
						}
						chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
						i += 5;
						num2++;
						break;
					default:
						goto IL_175;
					}
				}
				else
				{
					chars[num2] = 10;
					i++;
					num2++;
				}
				IL_1C5:
				i++;
				continue;
				IL_175:
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
					goto IL_1C5;
				}
				chars[num2] = (int)text[i];
				num2++;
				goto IL_1C5;
			}
			chars[num2] = 0;
		}

		// Token: 0x06004A44 RID: 19012 RVA: 0x0015896C File Offset: 0x00156B6C
		protected void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				char[] input_CharArray = this.m_input_CharArray;
				int num = index;
				index = num + 1;
				input_CharArray[num] = 45;
				number = -number;
			}
			number += this.k_Power[Mathf.Min(9, precision)];
			int num2 = (int)number;
			this.AddIntToCharArray(num2, ref index, precision);
			if (precision > 0)
			{
				char[] input_CharArray2 = this.m_input_CharArray;
				int num = index;
				index = num + 1;
				input_CharArray2[num] = 46;
				number -= (float)num2;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num3 = (int)number;
					char[] input_CharArray3 = this.m_input_CharArray;
					num = index;
					index = num + 1;
					input_CharArray3[num] = (ushort)(num3 + 48);
					number -= (float)num3;
				}
			}
		}

		// Token: 0x06004A45 RID: 19013 RVA: 0x00158A08 File Offset: 0x00156C08
		protected void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				char[] input_CharArray = this.m_input_CharArray;
				int num = index;
				index = num + 1;
				input_CharArray[num] = 45;
				number = -number;
			}
			int num2 = index;
			do
			{
				this.m_input_CharArray[num2++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num3 = num2;
			while (index + 1 < num2)
			{
				num2--;
				char c = this.m_input_CharArray[index];
				this.m_input_CharArray[index] = this.m_input_CharArray[num2];
				this.m_input_CharArray[num2] = c;
				index++;
			}
			index = num3;
		}

		// Token: 0x06004A46 RID: 19014 RVA: 0x00158A8C File Offset: 0x00156C8C
		protected virtual int SetArraySizes(int[] chars)
		{
			return 0;
		}

		// Token: 0x06004A47 RID: 19015 RVA: 0x00158A8F File Offset: 0x00156C8F
		protected virtual void GenerateTextMesh()
		{
		}

		// Token: 0x06004A48 RID: 19016 RVA: 0x00158A94 File Offset: 0x00156C94
		public Vector2 GetPreferredValues()
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float preferredWidth = this.GetPreferredWidth();
			float preferredHeight = this.GetPreferredHeight();
			return new Vector2(preferredWidth, preferredHeight);
		}

		// Token: 0x06004A49 RID: 19017 RVA: 0x00158AD4 File Offset: 0x00156CD4
		public Vector2 GetPreferredValues(float width, float height)
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			Vector2 margin = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		// Token: 0x06004A4A RID: 19018 RVA: 0x00158B1C File Offset: 0x00156D1C
		public Vector2 GetPreferredValues(string text)
		{
			this.m_isCalculatingPreferredValues = true;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 margin = TMP_Text.k_LargePositiveVector2;
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		// Token: 0x06004A4B RID: 19019 RVA: 0x00158B68 File Offset: 0x00156D68
		public Vector2 GetPreferredValues(string text, float width, float height)
		{
			this.m_isCalculatingPreferredValues = true;
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 margin = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(margin);
			float preferredHeight = this.GetPreferredHeight(margin);
			return new Vector2(preferredWidth, preferredHeight);
		}

		// Token: 0x06004A4C RID: 19020 RVA: 0x00158BB4 File Offset: 0x00156DB4
		protected float GetPreferredWidth()
		{
			float defaultFontSize = this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize;
			Vector2 marginSize = TMP_Text.k_LargePositiveVector2;
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float x = this.CalculatePreferredValues(defaultFontSize, marginSize).x;
			this.m_isPreferredWidthDirty = false;
			return x;
		}

		// Token: 0x06004A4D RID: 19021 RVA: 0x00158C10 File Offset: 0x00156E10
		protected float GetPreferredWidth(Vector2 margin)
		{
			float defaultFontSize = this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize;
			return this.CalculatePreferredValues(defaultFontSize, margin).x;
		}

		// Token: 0x06004A4E RID: 19022 RVA: 0x00158C44 File Offset: 0x00156E44
		protected float GetPreferredHeight()
		{
			float defaultFontSize = this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize;
			Vector2 marginSize = new Vector2((this.m_marginWidth != 0f) ? this.m_marginWidth : TMP_Text.k_LargePositiveFloat, TMP_Text.k_LargePositiveFloat);
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.m_isCalculatingPreferredValues = true;
				this.ParseInputText();
			}
			float y = this.CalculatePreferredValues(defaultFontSize, marginSize).y;
			this.m_isPreferredHeightDirty = false;
			return y;
		}

		// Token: 0x06004A4F RID: 19023 RVA: 0x00158CC0 File Offset: 0x00156EC0
		protected float GetPreferredHeight(Vector2 margin)
		{
			float defaultFontSize = this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize;
			return this.CalculatePreferredValues(defaultFontSize, margin).y;
		}

		// Token: 0x06004A50 RID: 19024 RVA: 0x00158CF4 File Offset: 0x00156EF4
		public Vector2 GetRenderedValues()
		{
			return this.GetTextBounds().size;
		}

		// Token: 0x06004A51 RID: 19025 RVA: 0x00158D14 File Offset: 0x00156F14
		protected float GetRenderedWidth()
		{
			return this.GetRenderedValues().x;
		}

		// Token: 0x06004A52 RID: 19026 RVA: 0x00158D21 File Offset: 0x00156F21
		protected float GetRenderedHeight()
		{
			return this.GetRenderedValues().y;
		}

		// Token: 0x06004A53 RID: 19027 RVA: 0x00158D30 File Offset: 0x00156F30
		protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize)
		{
			if (this.m_fontAsset == null || this.m_fontAsset.characterDictionary == null)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID().ToString());
				return Vector2.zero;
			}
			if (this.m_char_buffer == null || this.m_char_buffer.Length == 0 || this.m_char_buffer[0] == 0)
			{
				return Vector2.zero;
			}
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			this.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			int totalCharacterCount = this.m_totalCharacterCount;
			if (this.m_internalCharacterInfo == null || totalCharacterCount > this.m_internalCharacterInfo.Length)
			{
				this.m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount > 1024) ? (totalCharacterCount + 256) : Mathf.NextPowerOfTwo(totalCharacterCount)];
			}
			this.m_fontScale = defaultFontSize / this.m_currentFontAsset.fontInfo.PointSize * (this.m_isOrthographic ? 1f : 0.1f);
			this.m_fontScaleMultiplier = 1f;
			float num = defaultFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
			float num2 = this.m_fontScale;
			this.m_currentFontSize = defaultFontSize;
			this.m_sizeStack.SetDefault(this.m_currentFontSize);
			this.m_style = this.m_fontStyle;
			this.m_baselineOffset = 0f;
			this.m_styleStack.Clear();
			this.m_lineOffset = 0f;
			this.m_lineHeight = 0f;
			float num3 = this.m_currentFontAsset.fontInfo.LineHeight - (this.m_currentFontAsset.fontInfo.Ascender - this.m_currentFontAsset.fontInfo.Descender);
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			this.m_xAdvance = 0f;
			float a = 0f;
			this.tag_LineIndent = 0f;
			this.tag_Indent = 0f;
			this.m_indentStack.SetDefault(0f);
			this.tag_NoParsing = false;
			this.m_characterCount = 0;
			this.m_firstCharacterOfLine = 0;
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_lineNumber = 0;
			float x = marginSize.x;
			this.m_marginLeft = 0f;
			this.m_marginRight = 0f;
			this.m_width = -1f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			this.m_maxAscender = 0f;
			this.m_maxDescender = 0f;
			bool flag = true;
			bool flag2 = false;
			WordWrapState wordWrapState = default(WordWrapState);
			this.SaveWordWrappingState(ref wordWrapState, 0, 0);
			WordWrapState wordWrapState2 = default(WordWrapState);
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			while (this.m_char_buffer[num9] != 0)
			{
				int num10 = this.m_char_buffer[num9];
				this.m_textElementType = TMP_TextElementType.Character;
				this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
				this.m_currentFontAsset = this.m_materialReferences[this.m_currentMaterialIndex].fontAsset;
				int currentMaterialIndex = this.m_currentMaterialIndex;
				if (!this.m_isRichText || num10 != 60)
				{
					goto IL_37A;
				}
				this.m_isParsingText = true;
				if (!this.ValidateHtmlTag(this.m_char_buffer, num9 + 1, out num8))
				{
					goto IL_37A;
				}
				num9 = num8;
				if (this.m_textElementType != TMP_TextElementType.Character)
				{
					goto IL_37A;
				}
				IL_1039:
				num9++;
				continue;
				IL_37A:
				this.m_isParsingText = false;
				bool isUsingAlternateTypeface = this.m_textInfo.characterInfo[this.m_characterCount].isUsingAlternateTypeface;
				float num11 = 1f;
				if (this.m_textElementType == TMP_TextElementType.Character)
				{
					if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num10))
						{
							num10 = (int)char.ToUpper((char)num10);
						}
					}
					else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num10))
						{
							num10 = (int)char.ToLower((char)num10);
						}
					}
					else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps) && char.IsLower((char)num10))
					{
						num11 = 0.8f;
						num10 = (int)char.ToUpper((char)num10);
					}
				}
				if (this.m_textElementType == TMP_TextElementType.Sprite)
				{
					TMP_Sprite tmp_Sprite = this.m_currentSpriteAsset.spriteInfoList[this.m_spriteIndex];
					if (tmp_Sprite == null)
					{
						goto IL_1039;
					}
					num10 = 57344 + this.m_spriteIndex;
					this.m_currentFontAsset = this.m_fontAsset;
					float num12 = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
					num2 = this.m_fontAsset.fontInfo.Ascender / tmp_Sprite.height * tmp_Sprite.scale * num12;
					this.m_cached_TextElement = tmp_Sprite;
					this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
					this.m_currentMaterialIndex = currentMaterialIndex;
				}
				else if (this.m_textElementType == TMP_TextElementType.Character)
				{
					this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
					if (this.m_cached_TextElement == null)
					{
						goto IL_1039;
					}
					this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
					this.m_fontScale = this.m_currentFontSize * num11 / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
					num2 = this.m_fontScale * this.m_fontScaleMultiplier * this.m_cached_TextElement.scale;
					this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
				}
				float num13 = num2;
				if (num10 == 173)
				{
					num2 = 0f;
				}
				this.m_internalCharacterInfo[this.m_characterCount].character = (char)num10;
				if (this.m_enableKerning && this.m_characterCount >= 1)
				{
					int character = (int)this.m_internalCharacterInfo[this.m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num10);
					KerningPair kerningPair;
					this.m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out kerningPair);
					if (kerningPair != null)
					{
						this.m_xAdvance += kerningPair.XadvanceOffset * num2;
					}
				}
				float num14 = 0f;
				if (this.m_monoSpacing != 0f)
				{
					num14 = this.m_monoSpacing / 2f - (this.m_cached_TextElement.width / 2f + this.m_cached_TextElement.xOffset) * num2;
					this.m_xAdvance += num14;
				}
				float num15;
				if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((this.m_style & FontStyles.Bold) == FontStyles.Bold || (this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
				{
					num15 = 1f + this.m_currentFontAsset.boldSpacing * 0.01f;
				}
				else
				{
					num15 = 1f;
				}
				this.m_internalCharacterInfo[this.m_characterCount].baseLine = 0f - this.m_lineOffset + this.m_baselineOffset;
				float num16 = this.m_currentFontAsset.fontInfo.Ascender * ((this.m_textElementType == TMP_TextElementType.Character) ? num2 : this.m_internalCharacterInfo[this.m_characterCount].scale) + this.m_baselineOffset;
				this.m_internalCharacterInfo[this.m_characterCount].ascender = num16 - this.m_lineOffset;
				this.m_maxLineAscender = ((num16 > this.m_maxLineAscender) ? num16 : this.m_maxLineAscender);
				float num17 = this.m_currentFontAsset.fontInfo.Descender * ((this.m_textElementType == TMP_TextElementType.Character) ? num2 : this.m_internalCharacterInfo[this.m_characterCount].scale) + this.m_baselineOffset;
				float num18 = this.m_internalCharacterInfo[this.m_characterCount].descender = num17 - this.m_lineOffset;
				this.m_maxLineDescender = ((num17 < this.m_maxLineDescender) ? num17 : this.m_maxLineDescender);
				if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript || (this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					float num19 = (num16 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num16 = this.m_maxLineAscender;
					this.m_maxLineAscender = ((num19 > this.m_maxLineAscender) ? num19 : this.m_maxLineAscender);
					float num20 = (num17 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
					num17 = this.m_maxLineDescender;
					this.m_maxLineDescender = ((num20 < this.m_maxLineDescender) ? num20 : this.m_maxLineDescender);
				}
				if (this.m_lineNumber == 0)
				{
					this.m_maxAscender = ((this.m_maxAscender > num16) ? this.m_maxAscender : num16);
				}
				if (num10 == 9 || !char.IsWhiteSpace((char)num10) || this.m_textElementType == TMP_TextElementType.Sprite)
				{
					float num21 = (this.m_width != -1f) ? Mathf.Min(x + 0.0001f - this.m_marginLeft - this.m_marginRight, this.m_width) : (x + 0.0001f - this.m_marginLeft - this.m_marginRight);
					num6 = this.m_xAdvance + this.m_cached_TextElement.xAdvance * ((num10 != 173) ? num2 : num13);
					if (num6 > num21 && this.enableWordWrapping && this.m_characterCount != this.m_firstCharacterOfLine)
					{
						if (num7 == wordWrapState2.previous_WordBreak || flag)
						{
							if (!this.m_isCharacterWrappingEnabled)
							{
								this.m_isCharacterWrappingEnabled = true;
							}
							else
							{
								flag2 = true;
							}
						}
						num9 = this.RestoreWordWrappingState(ref wordWrapState2);
						num7 = num9;
						if (this.m_char_buffer[num9] == 173)
						{
							this.m_isTextTruncated = true;
							this.m_char_buffer[num9] = 45;
							this.CalculatePreferredValues(defaultFontSize, marginSize);
							return Vector2.zero;
						}
						if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
						{
							float num22 = this.m_maxLineAscender - this.m_startOfLineAscender;
							this.m_lineOffset += num22;
							wordWrapState2.lineOffset = this.m_lineOffset;
							wordWrapState2.previousLineAscender = this.m_maxLineAscender;
						}
						float num23 = this.m_maxLineAscender - this.m_lineOffset;
						float num24 = this.m_maxLineDescender - this.m_lineOffset;
						this.m_maxDescender = ((this.m_maxDescender < num24) ? this.m_maxDescender : num24);
						this.m_firstCharacterOfLine = this.m_characterCount;
						num4 += this.m_xAdvance;
						if (this.m_enableWordWrapping)
						{
							num5 = this.m_maxAscender - this.m_maxDescender;
						}
						else
						{
							num5 = Mathf.Max(num5, num23 - num24);
						}
						this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount - 1);
						this.m_lineNumber++;
						if (this.m_lineHeight == 0f)
						{
							float num25 = this.m_internalCharacterInfo[this.m_characterCount].ascender - this.m_internalCharacterInfo[this.m_characterCount].baseLine;
							float num26 = 0f - this.m_maxLineDescender + num25 + (num3 + this.m_lineSpacing + this.m_lineSpacingDelta) * num;
							this.m_lineOffset += num26;
							this.m_startOfLineAscender = num25;
						}
						else
						{
							this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * num;
						}
						this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						this.m_xAdvance = 0f + this.tag_Indent;
						goto IL_1039;
					}
				}
				if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
				{
					float num27 = this.m_maxLineAscender - this.m_startOfLineAscender;
					num18 -= num27;
					this.m_lineOffset += num27;
					this.m_startOfLineAscender += num27;
					wordWrapState2.lineOffset = this.m_lineOffset;
					wordWrapState2.previousLineAscender = this.m_startOfLineAscender;
				}
				if (num10 == 9)
				{
					float num28 = this.m_currentFontAsset.fontInfo.TabWidth * num2;
					float num29 = Mathf.Ceil(this.m_xAdvance / num28) * num28;
					this.m_xAdvance = ((num29 > this.m_xAdvance) ? num29 : (this.m_xAdvance + num28));
				}
				else if (this.m_monoSpacing != 0f)
				{
					this.m_xAdvance += this.m_monoSpacing - num14 + (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
				}
				else
				{
					this.m_xAdvance += (this.m_cached_TextElement.xAdvance * num15 + this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
				}
				if (num10 == 13)
				{
					a = Mathf.Max(a, num4 + this.m_xAdvance);
					num4 = 0f;
					this.m_xAdvance = 0f + this.tag_Indent;
				}
				if (num10 == 10 || this.m_characterCount == totalCharacterCount - 1)
				{
					if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
					{
						float num30 = this.m_maxLineAscender - this.m_startOfLineAscender;
						num18 -= num30;
						this.m_lineOffset += num30;
					}
					float num31 = this.m_maxLineDescender - this.m_lineOffset;
					this.m_maxDescender = ((this.m_maxDescender < num31) ? this.m_maxDescender : num31);
					this.m_firstCharacterOfLine = this.m_characterCount + 1;
					if (num10 == 10 && this.m_characterCount != totalCharacterCount - 1)
					{
						a = Mathf.Max(a, num4 + num6);
						num4 = 0f;
					}
					else
					{
						num4 = Mathf.Max(a, num4 + num6);
					}
					num5 = this.m_maxAscender - this.m_maxDescender;
					if (num10 == 10)
					{
						this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount);
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
						this.m_lineNumber++;
						if (this.m_lineHeight == 0f)
						{
							float num26 = 0f - this.m_maxLineDescender + num16 + (num3 + this.m_lineSpacing + this.m_paragraphSpacing + this.m_lineSpacingDelta) * num;
							this.m_lineOffset += num26;
						}
						else
						{
							this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + this.m_paragraphSpacing) * num;
						}
						this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
						this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
						this.m_startOfLineAscender = num16;
						this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
					}
				}
				if (this.m_enableWordWrapping || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((char.IsWhiteSpace((char)num10) || num10 == 45 || num10 == 173) && !this.m_isNonBreakingSpace && num10 != 160 && num10 != 8209 && num10 != 8239 && num10 != 8288)
					{
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
						this.m_isCharacterWrappingEnabled = false;
						flag = false;
					}
					else if (((num10 > 4352 && num10 < 4607) || (num10 > 11904 && num10 < 40959) || (num10 > 43360 && num10 < 43391) || (num10 > 44032 && num10 < 55295) || (num10 > 63744 && num10 < 64255) || (num10 > 65072 && num10 < 65103) || (num10 > 65280 && num10 < 65519)) && !this.m_isNonBreakingSpace)
					{
						if (flag || flag2 || (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num10) && this.m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey((int)this.m_internalCharacterInfo[this.m_characterCount + 1].character)))
						{
							this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
							this.m_isCharacterWrappingEnabled = false;
							flag = false;
						}
					}
					else if (flag || this.m_isCharacterWrappingEnabled || flag2)
					{
						this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
					}
				}
				this.m_characterCount++;
				goto IL_1039;
			}
			this.m_isCharacterWrappingEnabled = false;
			num4 += ((this.m_margin.x > 0f) ? this.m_margin.x : 0f);
			num4 += ((this.m_margin.z > 0f) ? this.m_margin.z : 0f);
			num5 += ((this.m_margin.y > 0f) ? this.m_margin.y : 0f);
			num5 += ((this.m_margin.w > 0f) ? this.m_margin.w : 0f);
			num4 = (float)((int)(num4 * 100f + 1f)) / 100f;
			num5 = (float)((int)(num5 * 100f + 1f)) / 100f;
			return new Vector2(num4, num5);
		}

		// Token: 0x06004A54 RID: 19028 RVA: 0x00159E70 File Offset: 0x00158070
		protected virtual Bounds GetCompoundBounds()
		{
			return default(Bounds);
		}

		// Token: 0x06004A55 RID: 19029 RVA: 0x00159E88 File Offset: 0x00158088
		protected Bounds GetTextBounds()
		{
			if (this.m_textInfo == null)
			{
				return default(Bounds);
			}
			Extents extents = new Extents(TMP_Text.k_LargePositiveVector2, TMP_Text.k_LargeNegativeVector2);
			for (int i = 0; i < this.m_textInfo.characterCount; i++)
			{
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					extents.min.x = Mathf.Min(extents.min.x, this.m_textInfo.characterInfo[i].bottomLeft.x);
					extents.min.y = Mathf.Min(extents.min.y, this.m_textInfo.characterInfo[i].descender);
					extents.max.x = Mathf.Max(extents.max.x, this.m_textInfo.characterInfo[i].xAdvance);
					extents.max.y = Mathf.Max(extents.max.y, this.m_textInfo.characterInfo[i].ascender);
				}
			}
			Vector2 v;
			v.x = extents.max.x - extents.min.x;
			v.y = extents.max.y - extents.min.y;
			return new Bounds((extents.min + extents.max) / 2f, v);
		}

		// Token: 0x06004A56 RID: 19030 RVA: 0x0015A023 File Offset: 0x00158223
		protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
		}

		// Token: 0x06004A57 RID: 19031 RVA: 0x0015A028 File Offset: 0x00158228
		protected void ResizeLineExtents(int size)
		{
			size = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size + 1));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < this.m_textInfo.lineInfo.Length)
				{
					array[i] = this.m_textInfo.lineInfo[i];
				}
				else
				{
					array[i].lineExtents.min = TMP_Text.k_LargePositiveVector2;
					array[i].lineExtents.max = TMP_Text.k_LargeNegativeVector2;
					array[i].ascender = TMP_Text.k_LargeNegativeFloat;
					array[i].descender = TMP_Text.k_LargePositiveFloat;
				}
			}
			this.m_textInfo.lineInfo = array;
		}

		// Token: 0x06004A58 RID: 19032 RVA: 0x0015A0E7 File Offset: 0x001582E7
		public virtual TMP_TextInfo GetTextInfo(string text)
		{
			return null;
		}

		// Token: 0x06004A59 RID: 19033 RVA: 0x0015A0EA File Offset: 0x001582EA
		protected virtual void ComputeMarginSize()
		{
		}

		// Token: 0x06004A5A RID: 19034 RVA: 0x0015A0EC File Offset: 0x001582EC
		protected int GetArraySizes(int[] chars)
		{
			int num = 0;
			this.m_totalCharacterCount = 0;
			this.m_isUsingBold = false;
			this.m_isParsingText = false;
			int num2 = 0;
			while (chars[num2] != 0)
			{
				int num3 = chars[num2];
				if (this.m_isRichText && num3 == 60 && this.ValidateHtmlTag(chars, num2 + 1, out num))
				{
					num2 = num;
					if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						this.m_isUsingBold = true;
					}
				}
				else
				{
					char.IsWhiteSpace((char)num3);
					this.m_totalCharacterCount++;
				}
				num2++;
			}
			return this.m_totalCharacterCount;
		}

		// Token: 0x06004A5B RID: 19035 RVA: 0x0015A170 File Offset: 0x00158370
		protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.currentFontAsset = this.m_currentFontAsset;
			state.currentSpriteAsset = this.m_currentSpriteAsset;
			state.currentMaterial = this.m_currentMaterial;
			state.currentMaterialIndex = this.m_currentMaterialIndex;
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = this.m_lineVisibleCharacterCount;
			state.visible_LinkCount = this.m_textInfo.linkCount;
			state.firstCharacterIndex = this.m_firstCharacterOfLine;
			state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
			state.fontStyle = this.m_style;
			state.fontScale = this.m_fontScale;
			state.fontScaleMultiplier = this.m_fontScaleMultiplier;
			state.currentFontSize = this.m_currentFontSize;
			state.xAdvance = this.m_xAdvance;
			state.maxCapHeight = this.m_maxCapHeight;
			state.maxAscender = this.m_maxAscender;
			state.maxDescender = this.m_maxDescender;
			state.maxLineAscender = this.m_maxLineAscender;
			state.maxLineDescender = this.m_maxLineDescender;
			state.previousLineAscender = this.m_startOfLineAscender;
			state.preferredWidth = this.m_preferredWidth;
			state.preferredHeight = this.m_preferredHeight;
			state.meshExtents = this.m_meshExtents;
			state.lineNumber = this.m_lineNumber;
			state.lineOffset = this.m_lineOffset;
			state.baselineOffset = this.m_baselineOffset;
			state.vertexColor = this.m_htmlColor;
			state.tagNoParsing = this.tag_NoParsing;
			state.colorStack = this.m_colorStack;
			state.sizeStack = this.m_sizeStack;
			state.fontWeightStack = this.m_fontWeightStack;
			state.styleStack = this.m_styleStack;
			state.actionStack = this.m_actionStack;
			state.materialReferenceStack = this.m_materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
			}
		}

		// Token: 0x06004A5C RID: 19036 RVA: 0x0015A35C File Offset: 0x0015855C
		protected int RestoreWordWrappingState(ref WordWrapState state)
		{
			int previous_WordBreak = state.previous_WordBreak;
			this.m_currentFontAsset = state.currentFontAsset;
			this.m_currentSpriteAsset = state.currentSpriteAsset;
			this.m_currentMaterial = state.currentMaterial;
			this.m_currentMaterialIndex = state.currentMaterialIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_lineVisibleCharacterCount = state.visible_CharacterCount;
			this.m_textInfo.linkCount = state.visible_LinkCount;
			this.m_firstCharacterOfLine = state.firstCharacterIndex;
			this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_style = state.fontStyle;
			this.m_fontScale = state.fontScale;
			this.m_fontScaleMultiplier = state.fontScaleMultiplier;
			this.m_currentFontSize = state.currentFontSize;
			this.m_xAdvance = state.xAdvance;
			this.m_maxCapHeight = state.maxCapHeight;
			this.m_maxAscender = state.maxAscender;
			this.m_maxDescender = state.maxDescender;
			this.m_maxLineAscender = state.maxLineAscender;
			this.m_maxLineDescender = state.maxLineDescender;
			this.m_startOfLineAscender = state.previousLineAscender;
			this.m_preferredWidth = state.preferredWidth;
			this.m_preferredHeight = state.preferredHeight;
			this.m_meshExtents = state.meshExtents;
			this.m_lineNumber = state.lineNumber;
			this.m_lineOffset = state.lineOffset;
			this.m_baselineOffset = state.baselineOffset;
			this.m_htmlColor = state.vertexColor;
			this.tag_NoParsing = state.tagNoParsing;
			this.m_colorStack = state.colorStack;
			this.m_sizeStack = state.sizeStack;
			this.m_fontWeightStack = state.fontWeightStack;
			this.m_styleStack = state.styleStack;
			this.m_actionStack = state.actionStack;
			this.m_materialReferenceStack = state.materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				this.m_textInfo.lineInfo[this.m_lineNumber] = state.lineInfo;
			}
			return previous_WordBreak;
		}

		// Token: 0x06004A5D RID: 19037 RVA: 0x0015A54C File Offset: 0x0015874C
		protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			vertexColor.a = ((this.m_fontColor32.a < vertexColor.a) ? this.m_fontColor32.a : vertexColor.a);
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (this.m_fontColorGradientPreset != null)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradientPreset.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradientPreset.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradientPreset.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradientPreset.bottomRight * vertexColor;
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight * vertexColor;
			}
			if (!this.m_isSDFShader)
			{
				style_padding = 0f;
			}
			FaceInfo fontInfo = this.m_currentFontAsset.fontInfo;
			Vector2 vector;
			vector.x = (this.m_cached_TextElement.x - padding - style_padding) / fontInfo.AtlasWidth;
			vector.y = 1f - (this.m_cached_TextElement.y + padding + style_padding + this.m_cached_TextElement.height) / fontInfo.AtlasHeight;
			Vector2 vector2;
			vector2.x = vector.x;
			vector2.y = 1f - (this.m_cached_TextElement.y - padding - style_padding) / fontInfo.AtlasHeight;
			Vector2 vector3;
			vector3.x = (this.m_cached_TextElement.x + padding + style_padding + this.m_cached_TextElement.width) / fontInfo.AtlasWidth;
			vector3.y = vector2.y;
			Vector2 uv;
			uv.x = vector3.x;
			uv.y = vector.y;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = uv;
		}

		// Token: 0x06004A5E RID: 19038 RVA: 0x0015AB0C File Offset: 0x00158D0C
		protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			if (this.m_tintAllSprites)
			{
				this.m_tintSprite = true;
			}
			Color32 color = this.m_tintSprite ? this.m_spriteColor.Multiply(vertexColor) : this.m_spriteColor;
			color.a = ((color.a < this.m_fontColor32.a) ? (color.a = ((color.a < vertexColor.a) ? color.a : vertexColor.a)) : this.m_fontColor32.a);
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else if (this.m_fontColorGradientPreset != null)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradientPreset.bottomLeft) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradientPreset.topLeft) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradientPreset.topRight) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradientPreset.bottomRight) : color);
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradient.bottomLeft) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradient.topLeft) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradient.topRight) : color);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = (this.m_tintSprite ? color.Multiply(this.m_fontColorGradient.bottomRight) : color);
			}
			Vector2 vector = new Vector2(this.m_cached_TextElement.x / (float)this.m_currentSpriteAsset.spriteSheet.width, this.m_cached_TextElement.y / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector2 = new Vector2(vector.x, (this.m_cached_TextElement.y + this.m_cached_TextElement.height) / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector3 = new Vector2((this.m_cached_TextElement.x + this.m_cached_TextElement.width) / (float)this.m_currentSpriteAsset.spriteSheet.width, vector2.y);
			Vector2 uv = new Vector2(vector3.x, vector.y);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = uv;
		}

		// Token: 0x06004A5F RID: 19039 RVA: 0x0015B124 File Offset: 0x00159324
		protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		// Token: 0x06004A60 RID: 19040 RVA: 0x0015B484 File Offset: 0x00159684
		protected virtual void FillCharacterVertexBuffers(int i, int index_X4, bool isVolumetric)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			if (isVolumetric)
			{
				Vector3 b = new Vector3(0f, 0f, this.m_fontSize * this.m_fontScale);
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[4 + index_X4] = characterInfo[i].vertex_BL.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[5 + index_X4] = characterInfo[i].vertex_TL.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[6 + index_X4] = characterInfo[i].vertex_TR.position + b;
				this.m_textInfo.meshInfo[materialReferenceIndex].vertices[7 + index_X4] = characterInfo[i].vertex_BR.position + b;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[4 + index_X4] = characterInfo[i].vertex_BL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[5 + index_X4] = characterInfo[i].vertex_TL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[6 + index_X4] = characterInfo[i].vertex_TR.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[7 + index_X4] = characterInfo[i].vertex_BR.uv;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[4 + index_X4] = characterInfo[i].vertex_BL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[5 + index_X4] = characterInfo[i].vertex_TL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[6 + index_X4] = characterInfo[i].vertex_TR.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[7 + index_X4] = characterInfo[i].vertex_BR.uv2;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			if (isVolumetric)
			{
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue);
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[4 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[5 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[6 + index_X4] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[7 + index_X4] = color;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + ((!isVolumetric) ? 4 : 8);
		}

		// Token: 0x06004A61 RID: 19041 RVA: 0x0015BB04 File Offset: 0x00159D04
		protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		// Token: 0x06004A62 RID: 19042 RVA: 0x0015BE64 File Offset: 0x0015A064
		protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor)
		{
			if (this.m_cached_Underline_GlyphInfo == null)
			{
				if (!TMP_Settings.warningsDisabled)
				{
					Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
				}
				return;
			}
			int num = index + 12;
			if (num > this.m_textInfo.meshInfo[0].vertices.Length)
			{
				this.m_textInfo.meshInfo[0].ResizeMeshInfo(num / 4);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			float num2 = this.m_cached_Underline_GlyphInfo.width / 2f * maxScale;
			if (end.x - start.x < this.m_cached_Underline_GlyphInfo.width * maxScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float num3 = this.m_padding * startScale / maxScale;
			float num4 = this.m_padding * endScale / maxScale;
			float height = this.m_cached_Underline_GlyphInfo.height;
			Vector3[] vertices = this.m_textInfo.meshInfo[0].vertices;
			vertices[index] = start + new Vector3(0f, 0f - (height + this.m_padding) * maxScale, 0f);
			vertices[index + 1] = start + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
			vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
			vertices[index + 4] = vertices[index + 3];
			vertices[index + 5] = vertices[index + 2];
			vertices[index + 6] = end + new Vector3(-num2, this.m_padding * maxScale, 0f);
			vertices[index + 7] = end + new Vector3(-num2, -(height + this.m_padding) * maxScale, 0f);
			vertices[index + 8] = vertices[index + 7];
			vertices[index + 9] = vertices[index + 6];
			vertices[index + 10] = end + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 11] = end + new Vector3(0f, -(height + this.m_padding) * maxScale, 0f);
			Vector2[] uvs = this.m_textInfo.meshInfo[0].uvs0;
			Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (this.m_cached_Underline_GlyphInfo.y + this.m_padding + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector2 = new Vector2(vector.x, 1f - (this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector3 = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			Vector2 vector5 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector6 = new Vector2(vector5.x, vector.y);
			Vector2 vector7 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector8 = new Vector2(vector7.x, vector.y);
			uvs[index] = vector;
			uvs[1 + index] = vector2;
			uvs[2 + index] = vector3;
			uvs[3 + index] = vector4;
			uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
			uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
			uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
			uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
			uvs[8 + index] = vector6;
			uvs[9 + index] = vector5;
			uvs[10 + index] = vector7;
			uvs[11 + index] = vector8;
			float x = (vertices[index + 2].x - start.x) / (end.x - start.x);
			float scale = Mathf.Abs(sdfScale);
			Vector2[] uvs2 = this.m_textInfo.meshInfo[0].uvs2;
			uvs2[index] = this.PackUV(0f, 0f, scale);
			uvs2[1 + index] = this.PackUV(0f, 1f, scale);
			uvs2[2 + index] = this.PackUV(x, 1f, scale);
			uvs2[3 + index] = this.PackUV(x, 0f, scale);
			float x2 = (vertices[index + 4].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[4 + index] = this.PackUV(x2, 0f, scale);
			uvs2[5 + index] = this.PackUV(x2, 1f, scale);
			uvs2[6 + index] = this.PackUV(x, 1f, scale);
			uvs2[7 + index] = this.PackUV(x, 0f, scale);
			x2 = (vertices[index + 8].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[8 + index] = this.PackUV(x2, 0f, scale);
			uvs2[9 + index] = this.PackUV(x2, 1f, scale);
			uvs2[10 + index] = this.PackUV(1f, 1f, scale);
			uvs2[11 + index] = this.PackUV(1f, 0f, scale);
			Color32[] colors = this.m_textInfo.meshInfo[0].colors32;
			colors[index] = underlineColor;
			colors[1 + index] = underlineColor;
			colors[2 + index] = underlineColor;
			colors[3 + index] = underlineColor;
			colors[4 + index] = underlineColor;
			colors[5 + index] = underlineColor;
			colors[6 + index] = underlineColor;
			colors[7 + index] = underlineColor;
			colors[8 + index] = underlineColor;
			colors[9 + index] = underlineColor;
			colors[10 + index] = underlineColor;
			colors[11 + index] = underlineColor;
			index += 12;
		}

		// Token: 0x06004A63 RID: 19043 RVA: 0x0015C69D File Offset: 0x0015A89D
		protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
		{
			fontAsset.characterDictionary.TryGetValue(95, out this.m_cached_Underline_GlyphInfo);
			fontAsset.characterDictionary.TryGetValue(8230, out this.m_cached_Ellipsis_GlyphInfo);
		}

		// Token: 0x06004A64 RID: 19044 RVA: 0x0015C6CC File Offset: 0x0015A8CC
		protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
		{
			bool flag = (this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
			int num = fontWeight / 100;
			TMP_FontAsset result;
			if (flag)
			{
				result = this.m_currentFontAsset.fontWeights[num].italicTypeface;
			}
			else
			{
				result = this.m_currentFontAsset.fontWeights[num].regularTypeface;
			}
			return result;
		}

		// Token: 0x06004A65 RID: 19045 RVA: 0x0015C72C File Offset: 0x0015A92C
		protected virtual void SetActiveSubMeshes(bool state)
		{
		}

		// Token: 0x06004A66 RID: 19046 RVA: 0x0015C730 File Offset: 0x0015A930
		protected Vector2 PackUV(float x, float y, float scale)
		{
			Vector2 vector;
			vector.x = Mathf.Floor(x * 511f);
			vector.y = Mathf.Floor(y * 511f);
			vector.x = vector.x * 4096f + vector.y;
			vector.y = scale;
			return vector;
		}

		// Token: 0x06004A67 RID: 19047 RVA: 0x0015C788 File Offset: 0x0015A988
		protected float PackUV(float x, float y)
		{
			float num = (float)Math.Floor((double)(x * 511f));
			double num2 = Math.Floor((double)(y * 511f));
			return (float)((double)num * 4096.0 + num2);
		}

		// Token: 0x06004A68 RID: 19048 RVA: 0x0015C7C0 File Offset: 0x0015A9C0
		protected int HexToInt(char hex)
		{
			switch (hex)
			{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
				break;
			case 'A':
				return 10;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			default:
				switch (hex)
				{
				case 'a':
					return 10;
				case 'b':
					return 11;
				case 'c':
					return 12;
				case 'd':
					return 13;
				case 'e':
					return 14;
				case 'f':
					return 15;
				}
				break;
			}
			return 15;
		}

		// Token: 0x06004A69 RID: 19049 RVA: 0x0015C890 File Offset: 0x0015AA90
		protected int GetUTF16(int i)
		{
			return this.HexToInt(this.m_text[i]) * 4096 + this.HexToInt(this.m_text[i + 1]) * 256 + this.HexToInt(this.m_text[i + 2]) * 16 + this.HexToInt(this.m_text[i + 3]);
		}

		// Token: 0x06004A6A RID: 19050 RVA: 0x0015C900 File Offset: 0x0015AB00
		protected int GetUTF32(int i)
		{
			return 0 + this.HexToInt(this.m_text[i]) * 268435456 + this.HexToInt(this.m_text[i + 1]) * 16777216 + this.HexToInt(this.m_text[i + 2]) * 1048576 + this.HexToInt(this.m_text[i + 3]) * 65536 + this.HexToInt(this.m_text[i + 4]) * 4096 + this.HexToInt(this.m_text[i + 5]) * 256 + this.HexToInt(this.m_text[i + 6]) * 16 + this.HexToInt(this.m_text[i + 7]);
		}

		// Token: 0x06004A6B RID: 19051 RVA: 0x0015C9DC File Offset: 0x0015ABDC
		protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			if (tagCount == 7)
			{
				byte r = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				return new Color32(r, g, b, byte.MaxValue);
			}
			if (tagCount == 9)
			{
				byte r2 = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte g2 = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b2 = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				byte a = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				return new Color32(r2, g2, b2, a);
			}
			if (tagCount == 13)
			{
				byte r3 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g3 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b3 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				return new Color32(r3, g3, b3, byte.MaxValue);
			}
			if (tagCount == 15)
			{
				byte r4 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte g4 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b4 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				byte a2 = (byte)(this.HexToInt(hexChars[13]) * 16 + this.HexToInt(hexChars[14]));
				return new Color32(r4, g4, b4, a2);
			}
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		// Token: 0x06004A6C RID: 19052 RVA: 0x0015CBA4 File Offset: 0x0015ADA4
		protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
		{
			if (length == 7)
			{
				byte r = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte g = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				return new Color32(r, g, b, byte.MaxValue);
			}
			if (length == 9)
			{
				byte r2 = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte g2 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b2 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				byte a = (byte)(this.HexToInt(hexChars[startIndex + 7]) * 16 + this.HexToInt(hexChars[startIndex + 8]));
				return new Color32(r2, g2, b2, a);
			}
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x0015CCB0 File Offset: 0x0015AEB0
		protected float ConvertToFloat(char[] chars, int startIndex, int length, int decimalPointIndex)
		{
			if (startIndex == 0)
			{
				return -9999f;
			}
			int num = startIndex + length - 1;
			float num2 = 0f;
			float num3 = 1f;
			decimalPointIndex = ((decimalPointIndex > 0) ? decimalPointIndex : (num + 1));
			if (chars[startIndex] == '-')
			{
				startIndex++;
				num3 = -1f;
			}
			if (chars[startIndex] == '+' || chars[startIndex] == '%')
			{
				startIndex++;
			}
			for (int i = startIndex; i < num + 1; i++)
			{
				if (!char.IsDigit(chars[i]) && chars[i] != '.')
				{
					return -9999f;
				}
				switch (decimalPointIndex - i)
				{
				case -3:
					num2 += (float)(chars[i] - '0') * 0.001f;
					break;
				case -2:
					num2 += (float)(chars[i] - '0') * 0.01f;
					break;
				case -1:
					num2 += (float)(chars[i] - '0') * 0.1f;
					break;
				case 1:
					num2 += (float)(chars[i] - '0');
					break;
				case 2:
					num2 += (float)((chars[i] - '0') * '\n');
					break;
				case 3:
					num2 += (float)((chars[i] - '0') * 'd');
					break;
				case 4:
					num2 += (float)((chars[i] - '0') * 'Ϩ');
					break;
				}
			}
			return num2 * num3;
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x0015CDDC File Offset: 0x0015AFDC
		protected bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
		{
			int num = 0;
			byte b = 0;
			TagUnits tagUnits = TagUnits.Pixels;
			TagType tagType = TagType.None;
			int num2 = 0;
			this.m_xmlAttribute[num2].nameHashCode = 0;
			this.m_xmlAttribute[num2].valueType = TagType.None;
			this.m_xmlAttribute[num2].valueHashCode = 0;
			this.m_xmlAttribute[num2].valueStartIndex = 0;
			this.m_xmlAttribute[num2].valueLength = 0;
			this.m_xmlAttribute[num2].valueDecimalIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			int num3 = startIndex;
			while (num3 < chars.Length && chars[num3] != 0 && num < this.m_htmlTag.Length && chars[num3] != 60)
			{
				if (chars[num3] == 62)
				{
					flag2 = true;
					endIndex = num3;
					this.m_htmlTag[num] = '\0';
					break;
				}
				this.m_htmlTag[num] = (char)chars[num3];
				num++;
				if (b == 1)
				{
					if (tagType == TagType.None)
					{
						if (chars[num3] == 43 || chars[num3] == 45 || char.IsDigit((char)chars[num3]))
						{
							tagType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute = this.m_xmlAttribute;
							int num4 = num2;
							xmlAttribute[num4].valueLength = xmlAttribute[num4].valueLength + 1;
						}
						else if (chars[num3] == 35)
						{
							tagType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute2 = this.m_xmlAttribute;
							int num5 = num2;
							xmlAttribute2[num5].valueLength = xmlAttribute2[num5].valueLength + 1;
						}
						else if (chars[num3] == 34)
						{
							tagType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueStartIndex = num;
						}
						else
						{
							tagType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode ^ chars[num3]);
							XML_TagAttribute[] xmlAttribute3 = this.m_xmlAttribute;
							int num6 = num2;
							xmlAttribute3[num6].valueLength = xmlAttribute3[num6].valueLength + 1;
						}
					}
					else if (tagType == TagType.NumericalValue)
					{
						if (chars[num3] == 46)
						{
							this.m_xmlAttribute[num2].valueDecimalIndex = num - 1;
						}
						if (chars[num3] == 112 || chars[num3] == 101 || chars[num3] == 37 || chars[num3] == 32)
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
							if (chars[num3] == 101)
							{
								tagUnits = TagUnits.FontUnits;
							}
							else if (chars[num3] == 37)
							{
								tagUnits = TagUnits.Percentage;
							}
						}
						else if (b != 2)
						{
							XML_TagAttribute[] xmlAttribute4 = this.m_xmlAttribute;
							int num7 = num2;
							xmlAttribute4[num7].valueLength = xmlAttribute4[num7].valueLength + 1;
						}
					}
					else if (tagType == TagType.ColorValue)
					{
						if (chars[num3] != 32)
						{
							XML_TagAttribute[] xmlAttribute5 = this.m_xmlAttribute;
							int num8 = num2;
							xmlAttribute5[num8].valueLength = xmlAttribute5[num8].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
					else if (tagType == TagType.StringValue)
					{
						if (chars[num3] != 34)
						{
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode ^ chars[num3]);
							XML_TagAttribute[] xmlAttribute6 = this.m_xmlAttribute;
							int num9 = num2;
							xmlAttribute6[num9].valueLength = xmlAttribute6[num9].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
				}
				if (chars[num3] == 61)
				{
					b = 1;
				}
				if (b == 0 && chars[num3] == 32)
				{
					if (flag)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagType = TagType.None;
					num2++;
					this.m_xmlAttribute[num2].nameHashCode = 0;
					this.m_xmlAttribute[num2].valueType = TagType.None;
					this.m_xmlAttribute[num2].valueHashCode = 0;
					this.m_xmlAttribute[num2].valueStartIndex = 0;
					this.m_xmlAttribute[num2].valueLength = 0;
					this.m_xmlAttribute[num2].valueDecimalIndex = 0;
				}
				if (b == 0)
				{
					this.m_xmlAttribute[num2].nameHashCode = (this.m_xmlAttribute[num2].nameHashCode << 3) - this.m_xmlAttribute[num2].nameHashCode + chars[num3];
				}
				if (b == 2 && chars[num3] == 32)
				{
					b = 0;
				}
				num3++;
			}
			if (!flag2)
			{
				return false;
			}
			if (this.tag_NoParsing && this.m_xmlAttribute[0].nameHashCode != 53822163 && this.m_xmlAttribute[0].nameHashCode != 49429939)
			{
				return false;
			}
			if (this.m_xmlAttribute[0].nameHashCode == 53822163 || this.m_xmlAttribute[0].nameHashCode == 49429939)
			{
				this.tag_NoParsing = false;
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 7)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 9)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			int nameHashCode = this.m_xmlAttribute[0].nameHashCode;
			float num10;
			int valueHashCode3;
			if (nameHashCode > 230446)
			{
				if (nameHashCode <= 6971027)
				{
					if (nameHashCode > 1112618)
					{
						if (nameHashCode <= 1750458)
						{
							if (nameHashCode <= 1441524)
							{
								if (nameHashCode <= 1286342)
								{
									if (nameHashCode == 1117479)
									{
										goto IL_22E7;
									}
									if (nameHashCode != 1286342)
									{
										return false;
									}
									goto IL_3296;
								}
								else if (nameHashCode != 1356515)
								{
									if (nameHashCode != 1441524)
									{
										return false;
									}
									goto IL_27AA;
								}
							}
							else if (nameHashCode <= 1524585)
							{
								if (nameHashCode == 1482398)
								{
									goto IL_2EAB;
								}
								if (nameHashCode != 1524585)
								{
									return false;
								}
								goto IL_26D6;
							}
							else
							{
								if (nameHashCode == 1619421)
								{
									goto IL_2984;
								}
								if (nameHashCode != 1750458)
								{
									return false;
								}
								return false;
							}
						}
						else if (nameHashCode <= 2109854)
						{
							if (nameHashCode <= 1983971)
							{
								if (nameHashCode == 1913798)
								{
									goto IL_3296;
								}
								if (nameHashCode != 1983971)
								{
									return false;
								}
							}
							else
							{
								if (nameHashCode == 2068980)
								{
									goto IL_27AA;
								}
								if (nameHashCode != 2109854)
								{
									return false;
								}
								goto IL_2EAB;
							}
						}
						else if (nameHashCode <= 2246877)
						{
							if (nameHashCode == 2152041)
							{
								goto IL_26D6;
							}
							if (nameHashCode != 2246877)
							{
								return false;
							}
							goto IL_2984;
						}
						else
						{
							if (nameHashCode == 6815845)
							{
								goto IL_32C0;
							}
							if (nameHashCode == 6886018)
							{
								goto IL_26C9;
							}
							if (nameHashCode != 6971027)
							{
								return false;
							}
							goto IL_288C;
						}
						num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
						if (num10 == -9999f || num10 == 0f)
						{
							return false;
						}
						switch (tagUnits)
						{
						case TagUnits.Pixels:
							this.m_cSpacing = num10;
							break;
						case TagUnits.FontUnits:
							this.m_cSpacing = num10;
							this.m_cSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
							break;
						case TagUnits.Percentage:
							return false;
						}
						return true;
						IL_26D6:
						num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
						if (num10 == -9999f || num10 == 0f)
						{
							return false;
						}
						switch (tagUnits)
						{
						case TagUnits.Pixels:
							this.m_monoSpacing = num10;
							break;
						case TagUnits.FontUnits:
							this.m_monoSpacing = num10;
							this.m_monoSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
							break;
						case TagUnits.Percentage:
							return false;
						}
						return true;
						IL_27AA:
						num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
						if (num10 == -9999f || num10 == 0f)
						{
							return false;
						}
						switch (tagUnits)
						{
						case TagUnits.Pixels:
							this.tag_Indent = num10;
							break;
						case TagUnits.FontUnits:
							this.tag_Indent = num10;
							this.tag_Indent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
							break;
						case TagUnits.Percentage:
							this.tag_Indent = this.m_marginWidth * num10 / 100f;
							break;
						}
						this.m_indentStack.Add(this.tag_Indent);
						this.m_xAdvance = this.tag_Indent;
						return true;
						IL_2984:
						int valueHashCode = this.m_xmlAttribute[0].valueHashCode;
						TMP_SpriteAsset tmp_SpriteAsset;
						if (this.m_xmlAttribute[0].valueType == TagType.None || this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
						{
							if (this.m_defaultSpriteAsset == null)
							{
								if (TMP_Settings.defaultSpriteAsset != null)
								{
									this.m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
								}
								else
								{
									this.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
								}
							}
							this.m_currentSpriteAsset = this.m_defaultSpriteAsset;
							if (this.m_currentSpriteAsset == null)
							{
								return false;
							}
						}
						else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode, out tmp_SpriteAsset))
						{
							this.m_currentSpriteAsset = tmp_SpriteAsset;
						}
						else
						{
							if (tmp_SpriteAsset == null)
							{
								tmp_SpriteAsset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
							}
							if (tmp_SpriteAsset == null)
							{
								return false;
							}
							MaterialReferenceManager.AddSpriteAsset(valueHashCode, tmp_SpriteAsset);
							this.m_currentSpriteAsset = tmp_SpriteAsset;
						}
						if (this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
						{
							int num11 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
							if (num11 == -9999)
							{
								return false;
							}
							if (num11 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
							{
								return false;
							}
							this.m_spriteIndex = num11;
						}
						else if (this.m_xmlAttribute[1].nameHashCode == 43347 || this.m_xmlAttribute[1].nameHashCode == 30547)
						{
							int spriteIndex = this.m_currentSpriteAsset.GetSpriteIndex(this.m_xmlAttribute[1].valueHashCode);
							if (spriteIndex == -1)
							{
								return false;
							}
							this.m_spriteIndex = spriteIndex;
						}
						else
						{
							if (this.m_xmlAttribute[1].nameHashCode != 295562 && this.m_xmlAttribute[1].nameHashCode != 205930)
							{
								return false;
							}
							int num12 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex);
							if (num12 == -9999)
							{
								return false;
							}
							if (num12 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
							{
								return false;
							}
							this.m_spriteIndex = num12;
						}
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentSpriteAsset.material, this.m_currentSpriteAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
						this.m_spriteColor = TMP_Text.s_colorWhite;
						this.m_tintSprite = false;
						if (this.m_xmlAttribute[1].nameHashCode == 45819 || this.m_xmlAttribute[1].nameHashCode == 33019)
						{
							this.m_tintSprite = (this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex) != 0f);
						}
						else if (this.m_xmlAttribute[2].nameHashCode == 45819 || this.m_xmlAttribute[2].nameHashCode == 33019)
						{
							this.m_tintSprite = (this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength, this.m_xmlAttribute[2].valueDecimalIndex) != 0f);
						}
						if (this.m_xmlAttribute[1].nameHashCode == 281955 || this.m_xmlAttribute[1].nameHashCode == 192323)
						{
							this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength);
						}
						else if (this.m_xmlAttribute[2].nameHashCode == 281955 || this.m_xmlAttribute[2].nameHashCode == 192323)
						{
							this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength);
						}
						this.m_xmlAttribute[1].nameHashCode = 0;
						this.m_xmlAttribute[2].nameHashCode = 0;
						this.m_textElementType = TMP_TextElementType.Sprite;
						return true;
						IL_2EAB:
						num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
						if (num10 == -9999f || num10 == 0f)
						{
							return false;
						}
						this.m_marginLeft = num10;
						switch (tagUnits)
						{
						case TagUnits.FontUnits:
							this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
							break;
						case TagUnits.Percentage:
							this.m_marginLeft = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * this.m_marginLeft / 100f;
							break;
						}
						this.m_marginLeft = ((this.m_marginLeft >= 0f) ? this.m_marginLeft : 0f);
						this.m_marginRight = this.m_marginLeft;
						return true;
						IL_3296:
						int valueHashCode2 = this.m_xmlAttribute[0].valueHashCode;
						if (this.m_isParsingText)
						{
							this.m_actionStack.Add(valueHashCode2);
						}
						return true;
					}
					TMP_Style style;
					if (nameHashCode > 322689)
					{
						if (nameHashCode <= 1022986)
						{
							if (nameHashCode <= 976214)
							{
								if (nameHashCode == 327550)
								{
									goto IL_225B;
								}
								if (nameHashCode != 976214)
								{
									return false;
								}
							}
							else
							{
								if (nameHashCode == 982252)
								{
									goto IL_2797;
								}
								if (nameHashCode != 1022986)
								{
									return false;
								}
								goto IL_2363;
							}
						}
						else if (nameHashCode <= 1065846)
						{
							if (nameHashCode == 1027847)
							{
								goto IL_22E7;
							}
							if (nameHashCode != 1065846)
							{
								return false;
							}
						}
						else
						{
							if (nameHashCode == 1071884)
							{
								goto IL_2797;
							}
							if (nameHashCode != 1112618)
							{
								return false;
							}
							goto IL_2363;
						}
						this.m_lineJustification = this.m_textAlignment;
						return true;
						IL_2363:
						style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
						if (style == null)
						{
							style = TMP_StyleSheet.GetStyle(this.m_styleStack.CurrentItem());
							this.m_styleStack.Remove();
						}
						if (style == null)
						{
							return false;
						}
						for (int i = 0; i < style.styleClosingTagArray.Length; i++)
						{
							if (style.styleClosingTagArray[i] == 60)
							{
								this.ValidateHtmlTag(style.styleClosingTagArray, i + 1, out i);
							}
						}
						return true;
						IL_2797:
						this.m_htmlColor = this.m_colorStack.Remove();
						return true;
					}
					if (nameHashCode <= 276254)
					{
						if (nameHashCode <= 237918)
						{
							if (nameHashCode != 233057)
							{
								if (nameHashCode != 237918)
								{
									return false;
								}
								goto IL_225B;
							}
						}
						else
						{
							if (nameHashCode == 275917)
							{
								goto IL_21E3;
							}
							if (nameHashCode != 276254)
							{
								return false;
							}
							goto IL_2008;
						}
					}
					else if (nameHashCode <= 281955)
					{
						if (nameHashCode == 280416)
						{
							return false;
						}
						if (nameHashCode != 281955)
						{
							return false;
						}
						goto IL_23DF;
					}
					else
					{
						if (nameHashCode == 320078)
						{
							goto IL_1F54;
						}
						if (nameHashCode != 322689)
						{
							return false;
						}
					}
					style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
					if (style == null)
					{
						return false;
					}
					this.m_styleStack.Add(style.hashCode);
					for (int j = 0; j < style.styleOpeningTagArray.Length; j++)
					{
						if (style.styleOpeningTagArray[j] == 60 && !this.ValidateHtmlTag(style.styleOpeningTagArray, j + 1, out j))
						{
							return false;
						}
					}
					return true;
					IL_225B:
					num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
					if (num10 == -9999f || num10 == 0f)
					{
						return false;
					}
					switch (tagUnits)
					{
					case TagUnits.Pixels:
						this.m_width = num10;
						break;
					case TagUnits.FontUnits:
						return false;
					case TagUnits.Percentage:
						this.m_width = this.m_marginWidth * num10 / 100f;
						break;
					}
					return true;
					IL_22E7:
					this.m_width = -1f;
					return true;
				}
				if (nameHashCode > 54741026)
				{
					if (nameHashCode <= 566686826)
					{
						if (nameHashCode <= 374360934)
						{
							if (nameHashCode <= 103415287)
							{
								if (nameHashCode != 72669687 && nameHashCode != 103415287)
								{
									return false;
								}
								valueHashCode3 = this.m_xmlAttribute[0].valueHashCode;
								if (valueHashCode3 != 764638571 && valueHashCode3 != 523367755)
								{
									Material material;
									if (MaterialReferenceManager.TryGetMaterial(valueHashCode3, out material))
									{
										if (this.m_currentFontAsset.atlas.GetInstanceID() != material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
										{
											return false;
										}
										this.m_currentMaterial = material;
										this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
										this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
									}
									else
									{
										material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
										if (material == null)
										{
											return false;
										}
										if (this.m_currentFontAsset.atlas.GetInstanceID() != material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
										{
											return false;
										}
										MaterialReferenceManager.AddFontMaterial(valueHashCode3, material);
										this.m_currentMaterial = material;
										this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
										this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
									}
									return true;
								}
								if (this.m_currentFontAsset.atlas.GetInstanceID() != this.m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
								{
									return false;
								}
								this.m_currentMaterial = this.m_materialReferences[0].material;
								this.m_currentMaterialIndex = 0;
								this.m_materialReferenceStack.Add(this.m_materialReferences[0]);
								return true;
							}
							else
							{
								if (nameHashCode != 343615334 && nameHashCode != 374360934)
								{
									return false;
								}
								if (this.m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != this.m_materialReferenceStack.PreviousItem().material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
								{
									return false;
								}
								MaterialReference materialReference = this.m_materialReferenceStack.Remove();
								this.m_currentMaterial = materialReference.material;
								this.m_currentMaterialIndex = materialReference.index;
								return true;
							}
						}
						else if (nameHashCode <= 514803617)
						{
							if (nameHashCode == 457225591)
							{
								goto IL_151F;
							}
							if (nameHashCode != 514803617)
							{
								return false;
							}
						}
						else
						{
							if (nameHashCode == 551025096)
							{
								goto IL_2E89;
							}
							if (nameHashCode != 566686826)
							{
								return false;
							}
							goto IL_2E67;
						}
					}
					else if (nameHashCode <= 1100728678)
					{
						if (nameHashCode <= 766244328)
						{
							if (nameHashCode != 730022849)
							{
								if (nameHashCode != 766244328)
								{
									return false;
								}
								goto IL_2E89;
							}
						}
						else
						{
							if (nameHashCode == 781906058)
							{
								goto IL_2E67;
							}
							if (nameHashCode != 1100728678)
							{
								return false;
							}
							goto IL_2FC9;
						}
					}
					else if (nameHashCode <= 1109386397)
					{
						if (nameHashCode == 1109349752)
						{
							goto IL_31BD;
						}
						if (nameHashCode != 1109386397)
						{
							return false;
						}
						goto IL_289F;
					}
					else
					{
						if (nameHashCode == 1897350193)
						{
							goto IL_3280;
						}
						if (nameHashCode == 1897386838)
						{
							goto IL_2977;
						}
						if (nameHashCode != 2012149182)
						{
							return false;
						}
						goto IL_139E;
					}
					this.m_style |= FontStyles.LowerCase;
					return true;
					IL_2E89:
					this.m_style |= FontStyles.SmallCaps;
					return true;
				}
				if (nameHashCode <= 9133802)
				{
					if (nameHashCode <= 7513474)
					{
						if (nameHashCode <= 7054088)
						{
							if (nameHashCode == 7011901)
							{
								goto IL_2FB1;
							}
							if (nameHashCode != 7054088)
							{
								return false;
							}
						}
						else
						{
							if (nameHashCode == 7443301)
							{
								goto IL_32C0;
							}
							if (nameHashCode != 7513474)
							{
								return false;
							}
							goto IL_26C9;
						}
					}
					else if (nameHashCode <= 7639357)
					{
						if (nameHashCode == 7598483)
						{
							goto IL_288C;
						}
						if (nameHashCode != 7639357)
						{
							return false;
						}
						goto IL_2FB1;
					}
					else if (nameHashCode != 7681544)
					{
						if (nameHashCode != 9133802)
						{
							return false;
						}
						goto IL_2E67;
					}
					this.m_monoSpacing = 0f;
					return true;
					IL_2FB1:
					this.m_marginLeft = 0f;
					this.m_marginRight = 0f;
					return true;
				}
				if (nameHashCode <= 15115642)
				{
					if (nameHashCode <= 11642281)
					{
						if (nameHashCode != 10723418)
						{
							if (nameHashCode != 11642281)
							{
								return false;
							}
							goto IL_1609;
						}
					}
					else
					{
						if (nameHashCode == 13526026)
						{
							goto IL_2E67;
						}
						if (nameHashCode != 15115642)
						{
							return false;
						}
					}
					this.tag_NoParsing = true;
					return true;
				}
				if (nameHashCode > 47840323)
				{
					if (nameHashCode != 50348802)
					{
						if (nameHashCode == 52232547)
						{
							goto IL_2E78;
						}
						if (nameHashCode != 54741026)
						{
							return false;
						}
					}
					this.m_baselineOffset = 0f;
					return true;
				}
				if (nameHashCode != 16034505)
				{
					if (nameHashCode != 47840323)
					{
						return false;
					}
					goto IL_2E78;
				}
				IL_1609:
				num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
				if (num10 == -9999f || num10 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					this.m_baselineOffset = num10;
					return true;
				case TagUnits.FontUnits:
					this.m_baselineOffset = num10 * this.m_fontScale * this.m_fontAsset.fontInfo.Ascender;
					return true;
				case TagUnits.Percentage:
					return false;
				default:
					return false;
				}
				IL_2E67:
				this.m_style |= FontStyles.UpperCase;
				return true;
				IL_26C9:
				this.m_cSpacing = 0f;
				return true;
				IL_288C:
				this.tag_Indent = this.m_indentStack.Remove();
				return true;
				IL_32C0:
				bool isParsingText = this.m_isParsingText;
				this.m_actionStack.Remove();
				return true;
			}
			if (nameHashCode <= 4556)
			{
				if (nameHashCode > 66)
				{
					if (nameHashCode <= 395)
					{
						if (nameHashCode <= 98)
						{
							if (nameHashCode <= 83)
							{
								if (nameHashCode != 73)
								{
									if (nameHashCode != 83)
									{
										return false;
									}
									goto IL_1111;
								}
							}
							else
							{
								if (nameHashCode == 85)
								{
									goto IL_1140;
								}
								if (nameHashCode != 98)
								{
									return false;
								}
								goto IL_1098;
							}
						}
						else if (nameHashCode <= 115)
						{
							if (nameHashCode != 105)
							{
								if (nameHashCode != 115)
								{
									return false;
								}
								goto IL_1111;
							}
						}
						else
						{
							if (nameHashCode == 117)
							{
								goto IL_1140;
							}
							if (nameHashCode != 395)
							{
								return false;
							}
							goto IL_10C3;
						}
						this.m_style |= FontStyles.Italic;
						return true;
						IL_1111:
						this.m_style |= FontStyles.Strikethrough;
						return true;
						IL_1140:
						this.m_style |= FontStyles.Underline;
						return true;
					}
					if (nameHashCode <= 426)
					{
						if (nameHashCode <= 412)
						{
							if (nameHashCode != 402)
							{
								if (nameHashCode != 412)
								{
									return false;
								}
								goto IL_1122;
							}
						}
						else
						{
							if (nameHashCode == 414)
							{
								goto IL_1150;
							}
							if (nameHashCode != 426)
							{
								return false;
							}
							return true;
						}
					}
					else if (nameHashCode <= 434)
					{
						if (nameHashCode == 427)
						{
							goto IL_10C3;
						}
						if (nameHashCode != 434)
						{
							return false;
						}
					}
					else
					{
						if (nameHashCode == 444)
						{
							goto IL_1122;
						}
						if (nameHashCode == 446)
						{
							goto IL_1150;
						}
						if (nameHashCode != 4556)
						{
							return false;
						}
						goto IL_154E;
					}
					this.m_style &= (FontStyles)(-3);
					return true;
					IL_1122:
					if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
					{
						this.m_style &= (FontStyles)(-65);
					}
					return true;
					IL_1150:
					if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
					{
						this.m_style &= (FontStyles)(-5);
					}
					return true;
					IL_10C3:
					if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
					{
						this.m_style &= (FontStyles)(-2);
						this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
					}
					return true;
				}
				if (nameHashCode <= -1616441709)
				{
					if (nameHashCode <= -1831660941)
					{
						if (nameHashCode <= -1883544150)
						{
							if (nameHashCode == -1885698441)
							{
								goto IL_151F;
							}
							if (nameHashCode != -1883544150)
							{
								return false;
							}
						}
						else
						{
							if (nameHashCode == -1847322671)
							{
								goto IL_2E9A;
							}
							if (nameHashCode != -1831660941)
							{
								return false;
							}
							goto IL_2E78;
						}
					}
					else if (nameHashCode <= -1668324918)
					{
						if (nameHashCode == -1690034531)
						{
							goto IL_30C3;
						}
						if (nameHashCode != -1668324918)
						{
							return false;
						}
					}
					else
					{
						if (nameHashCode == -1632103439)
						{
							goto IL_2E9A;
						}
						if (nameHashCode != -1616441709)
						{
							return false;
						}
						goto IL_2E78;
					}
					this.m_style &= (FontStyles)(-9);
					return true;
					IL_2E9A:
					this.m_style &= (FontStyles)(-33);
					return true;
				}
				if (nameHashCode <= -842656867)
				{
					if (nameHashCode <= -855002522)
					{
						if (nameHashCode != -884817987)
						{
							if (nameHashCode != -855002522)
							{
								return false;
							}
							goto IL_2FC9;
						}
					}
					else
					{
						if (nameHashCode == -842693512)
						{
							goto IL_31BD;
						}
						if (nameHashCode != -842656867)
						{
							return false;
						}
						goto IL_289F;
					}
				}
				else if (nameHashCode <= -445537194)
				{
					if (nameHashCode == -445573839)
					{
						goto IL_3280;
					}
					if (nameHashCode != -445537194)
					{
						return false;
					}
					goto IL_2977;
				}
				else
				{
					if (nameHashCode == -330774850)
					{
						goto IL_139E;
					}
					if (nameHashCode != 66)
					{
						return false;
					}
					goto IL_1098;
				}
				IL_30C3:
				num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
				if (num10 == -9999f || num10 == 0f)
				{
					return false;
				}
				this.m_marginRight = num10;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					this.m_marginRight *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
					break;
				case TagUnits.Percentage:
					this.m_marginRight = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * this.m_marginRight / 100f;
					break;
				}
				this.m_marginRight = ((this.m_marginRight >= 0f) ? this.m_marginRight : 0f);
				return true;
				IL_1098:
				this.m_style |= FontStyles.Bold;
				this.m_fontWeightInternal = 700;
				this.m_fontWeightStack.Add(700);
				return true;
			}
			if (nameHashCode <= 32745)
			{
				if (nameHashCode <= 20863)
				{
					if (nameHashCode <= 6552)
					{
						if (nameHashCode <= 4742)
						{
							if (nameHashCode != 4728)
							{
								if (nameHashCode != 4742)
								{
									return false;
								}
								goto IL_1285;
							}
						}
						else
						{
							if (nameHashCode == 6380)
							{
								goto IL_154E;
							}
							if (nameHashCode != 6552)
							{
								return false;
							}
						}
						this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize > 0f) ? this.m_currentFontAsset.fontInfo.SubSize : 1f);
						this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
						this.m_style |= FontStyles.Subscript;
						return true;
					}
					if (nameHashCode <= 20677)
					{
						if (nameHashCode != 6566)
						{
							if (nameHashCode != 20677)
							{
								return false;
							}
							goto IL_1600;
						}
					}
					else
					{
						if (nameHashCode == 20849)
						{
							goto IL_11D8;
						}
						if (nameHashCode != 20863)
						{
							return false;
						}
						goto IL_12F1;
					}
					IL_1285:
					this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize > 0f) ? this.m_currentFontAsset.fontInfo.SubSize : 1f);
					this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
					this.m_style |= FontStyles.Superscript;
					return true;
				}
				if (nameHashCode <= 28511)
				{
					if (nameHashCode <= 22673)
					{
						if (nameHashCode == 22501)
						{
							goto IL_1600;
						}
						if (nameHashCode != 22673)
						{
							return false;
						}
					}
					else
					{
						if (nameHashCode == 22687)
						{
							goto IL_12F1;
						}
						if (nameHashCode != 28511)
						{
							return false;
						}
						goto IL_19D9;
					}
				}
				else if (nameHashCode <= 31169)
				{
					if (nameHashCode == 30266)
					{
						goto IL_2050;
					}
					if (nameHashCode != 31169)
					{
						return false;
					}
					goto IL_16F3;
				}
				else
				{
					if (nameHashCode == 31191)
					{
						goto IL_16AF;
					}
					if (nameHashCode != 32745)
					{
						return false;
					}
					goto IL_1705;
				}
				IL_11D8:
				if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
				{
					if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
					{
						this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize > 0f) ? this.m_currentFontAsset.fontInfo.SubSize : 1f);
						this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
					}
					else
					{
						this.m_baselineOffset = 0f;
						this.m_fontScaleMultiplier = 1f;
					}
					this.m_style &= (FontStyles)(-257);
				}
				return true;
				IL_12F1:
				if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
				{
					if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
					{
						this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize > 0f) ? this.m_currentFontAsset.fontInfo.SubSize : 1f);
						this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
					}
					else
					{
						this.m_baselineOffset = 0f;
						this.m_fontScaleMultiplier = 1f;
					}
					this.m_style &= (FontStyles)(-129);
				}
				return true;
				IL_1600:
				this.m_isIgnoringAlignment = false;
				return true;
			}
			if (nameHashCode > 144016)
			{
				if (nameHashCode <= 156816)
				{
					if (nameHashCode <= 154158)
					{
						if (nameHashCode != 145592)
						{
							if (nameHashCode != 154158)
							{
								return false;
							}
							goto IL_1CB8;
						}
					}
					else
					{
						if (nameHashCode == 155913)
						{
							goto IL_217F;
						}
						if (nameHashCode != 156816)
						{
							return false;
						}
						goto IL_16FC;
					}
				}
				else if (nameHashCode <= 186285)
				{
					if (nameHashCode != 158392)
					{
						if (nameHashCode != 186285)
						{
							return false;
						}
						goto IL_21E3;
					}
				}
				else
				{
					if (nameHashCode == 186622)
					{
						goto IL_2008;
					}
					if (nameHashCode == 192323)
					{
						goto IL_23DF;
					}
					if (nameHashCode != 230446)
					{
						return false;
					}
					goto IL_1F54;
				}
				this.m_currentFontSize = this.m_sizeStack.Remove();
				this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
				return true;
			}
			if (nameHashCode <= 43991)
			{
				if (nameHashCode <= 43066)
				{
					if (nameHashCode == 41311)
					{
						goto IL_19D9;
					}
					if (nameHashCode != 43066)
					{
						return false;
					}
					goto IL_2050;
				}
				else
				{
					if (nameHashCode == 43969)
					{
						goto IL_16F3;
					}
					if (nameHashCode != 43991)
					{
						return false;
					}
					goto IL_16AF;
				}
			}
			else if (nameHashCode <= 141358)
			{
				if (nameHashCode == 45545)
				{
					goto IL_1705;
				}
				if (nameHashCode != 141358)
				{
					return false;
				}
				goto IL_1CB8;
			}
			else
			{
				if (nameHashCode == 143113)
				{
					goto IL_217F;
				}
				if (nameHashCode != 144016)
				{
					return false;
				}
			}
			IL_16FC:
			this.m_isNonBreakingSpace = false;
			return true;
			IL_1CB8:
			MaterialReference materialReference2 = this.m_materialReferenceStack.Remove();
			this.m_currentFontAsset = materialReference2.fontAsset;
			this.m_currentMaterial = materialReference2.material;
			this.m_currentMaterialIndex = materialReference2.index;
			this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
			return true;
			IL_217F:
			if (this.m_isParsingText)
			{
				this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextLength = this.m_characterCount - this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextfirstCharacterIndex;
				this.m_textInfo.linkCount++;
			}
			return true;
			IL_16AF:
			if (this.m_overflowMode == TextOverflowModes.Page)
			{
				this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
				this.m_lineOffset = 0f;
				this.m_pageNumber++;
				this.m_isNewPage = true;
			}
			return true;
			IL_16F3:
			this.m_isNonBreakingSpace = true;
			return true;
			IL_1705:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				if (this.m_htmlTag[5] == '+')
				{
					this.m_currentFontSize = this.m_fontSize + num10;
					this.m_sizeStack.Add(this.m_currentFontSize);
					this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
					return true;
				}
				if (this.m_htmlTag[5] == '-')
				{
					this.m_currentFontSize = this.m_fontSize + num10;
					this.m_sizeStack.Add(this.m_currentFontSize);
					this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
					return true;
				}
				this.m_currentFontSize = num10;
				this.m_sizeStack.Add(this.m_currentFontSize);
				this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
				return true;
			case TagUnits.FontUnits:
				this.m_currentFontSize = this.m_fontSize * num10;
				this.m_sizeStack.Add(this.m_currentFontSize);
				this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
				return true;
			case TagUnits.Percentage:
				this.m_currentFontSize = this.m_fontSize * num10 / 100f;
				this.m_sizeStack.Add(this.m_currentFontSize);
				this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
				return true;
			default:
				return false;
			}
			IL_19D9:
			int valueHashCode4 = this.m_xmlAttribute[0].valueHashCode;
			int nameHashCode2 = this.m_xmlAttribute[1].nameHashCode;
			valueHashCode3 = this.m_xmlAttribute[1].valueHashCode;
			if (valueHashCode4 == 764638571 || valueHashCode4 == 523367755)
			{
				this.m_currentFontAsset = this.m_materialReferences[0].fontAsset;
				this.m_currentMaterial = this.m_materialReferences[0].material;
				this.m_currentMaterialIndex = 0;
				this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
				this.m_materialReferenceStack.Add(this.m_materialReferences[0]);
				return true;
			}
			TMP_FontAsset tmp_FontAsset;
			if (!MaterialReferenceManager.TryGetFontAsset(valueHashCode4, out tmp_FontAsset))
			{
				tmp_FontAsset = Resources.Load<TMP_FontAsset>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
				if (tmp_FontAsset == null)
				{
					return false;
				}
				MaterialReferenceManager.AddFontAsset(tmp_FontAsset);
			}
			if (nameHashCode2 == 0 && valueHashCode3 == 0)
			{
				this.m_currentMaterial = tmp_FontAsset.material;
				this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
				this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
			}
			else
			{
				if (nameHashCode2 != 103415287 && nameHashCode2 != 72669687)
				{
					return false;
				}
				Material material;
				if (MaterialReferenceManager.TryGetMaterial(valueHashCode3, out material))
				{
					this.m_currentMaterial = material;
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
					this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
				}
				else
				{
					material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength));
					if (material == null)
					{
						return false;
					}
					MaterialReferenceManager.AddFontMaterial(valueHashCode3, material);
					this.m_currentMaterial = material;
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
					this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
				}
			}
			this.m_currentFontAsset = tmp_FontAsset;
			this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * (this.m_isOrthographic ? 1f : 0.1f);
			return true;
			IL_2050:
			if (this.m_isParsingText)
			{
				int linkCount = this.m_textInfo.linkCount;
				if (linkCount + 1 > this.m_textInfo.linkInfo.Length)
				{
					TMP_TextInfo.Resize<TMP_LinkInfo>(ref this.m_textInfo.linkInfo, linkCount + 1);
				}
				this.m_textInfo.linkInfo[linkCount].textComponent = this;
				this.m_textInfo.linkInfo[linkCount].hashCode = this.m_xmlAttribute[0].valueHashCode;
				this.m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_characterCount;
				this.m_textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + this.m_xmlAttribute[0].valueStartIndex;
				this.m_textInfo.linkInfo[linkCount].linkIdLength = this.m_xmlAttribute[0].valueLength;
				this.m_textInfo.linkInfo[linkCount].SetLinkID(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength);
			}
			return true;
			IL_154E:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				this.m_xAdvance = num10;
				return true;
			case TagUnits.FontUnits:
				this.m_xAdvance = num10 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
				return true;
			case TagUnits.Percentage:
				this.m_xAdvance = this.m_marginWidth * num10 / 100f;
				return true;
			default:
				return false;
			}
			IL_139E:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			if ((this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
			{
				return true;
			}
			this.m_style &= (FontStyles)(-2);
			int num13 = (int)num10;
			if (num13 <= 400)
			{
				if (num13 <= 200)
				{
					if (num13 != 100)
					{
						if (num13 == 200)
						{
							this.m_fontWeightInternal = 200;
						}
					}
					else
					{
						this.m_fontWeightInternal = 100;
					}
				}
				else if (num13 != 300)
				{
					if (num13 == 400)
					{
						this.m_fontWeightInternal = 400;
					}
				}
				else
				{
					this.m_fontWeightInternal = 300;
				}
			}
			else if (num13 <= 600)
			{
				if (num13 != 500)
				{
					if (num13 == 600)
					{
						this.m_fontWeightInternal = 600;
					}
				}
				else
				{
					this.m_fontWeightInternal = 500;
				}
			}
			else if (num13 != 700)
			{
				if (num13 != 800)
				{
					if (num13 == 900)
					{
						this.m_fontWeightInternal = 900;
					}
				}
				else
				{
					this.m_fontWeightInternal = 800;
				}
			}
			else
			{
				this.m_fontWeightInternal = 700;
				this.m_style |= FontStyles.Bold;
			}
			this.m_fontWeightStack.Add(this.m_fontWeightInternal);
			return true;
			IL_151F:
			this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
			if (this.m_fontWeightInternal == 400)
			{
				this.m_style &= (FontStyles)(-2);
			}
			return true;
			IL_1F54:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				this.m_xAdvance += num10;
				return true;
			case TagUnits.FontUnits:
				this.m_xAdvance += num10 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
				return true;
			case TagUnits.Percentage:
				return false;
			default:
				return false;
			}
			IL_2008:
			if (this.m_xmlAttribute[0].valueLength != 3)
			{
				return false;
			}
			this.m_htmlColor.a = (byte)(this.HexToInt(this.m_htmlTag[7]) * 16 + this.HexToInt(this.m_htmlTag[8]));
			return true;
			IL_21E3:
			num13 = this.m_xmlAttribute[0].valueHashCode;
			if (num13 <= -458210101)
			{
				if (num13 == -523808257)
				{
					this.m_lineJustification = TextAlignmentOptions.Justified;
					return true;
				}
				if (num13 == -458210101)
				{
					this.m_lineJustification = TextAlignmentOptions.Center;
					return true;
				}
			}
			else
			{
				if (num13 == 3774683)
				{
					this.m_lineJustification = TextAlignmentOptions.Left;
					return true;
				}
				if (num13 == 136703040)
				{
					this.m_lineJustification = TextAlignmentOptions.Right;
					return true;
				}
			}
			return false;
			IL_23DF:
			if (this.m_htmlTag[6] == '#' && num == 13)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (this.m_htmlTag[6] == '#' && num == 15)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			num13 = this.m_xmlAttribute[0].valueHashCode;
			if (num13 <= 26556144)
			{
				if (num13 <= 125395)
				{
					if (num13 == -36881330)
					{
						this.m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
						this.m_colorStack.Add(this.m_htmlColor);
						return true;
					}
					if (num13 == 125395)
					{
						this.m_htmlColor = Color.red;
						this.m_colorStack.Add(this.m_htmlColor);
						return true;
					}
				}
				else
				{
					if (num13 == 3573310)
					{
						this.m_htmlColor = Color.blue;
						this.m_colorStack.Add(this.m_htmlColor);
						return true;
					}
					if (num13 == 26556144)
					{
						this.m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
						this.m_colorStack.Add(this.m_htmlColor);
						return true;
					}
				}
			}
			else if (num13 <= 121463835)
			{
				if (num13 == 117905991)
				{
					this.m_htmlColor = Color.black;
					this.m_colorStack.Add(this.m_htmlColor);
					return true;
				}
				if (num13 == 121463835)
				{
					this.m_htmlColor = Color.green;
					this.m_colorStack.Add(this.m_htmlColor);
					return true;
				}
			}
			else
			{
				if (num13 == 140357351)
				{
					this.m_htmlColor = Color.white;
					this.m_colorStack.Add(this.m_htmlColor);
					return true;
				}
				if (num13 == 554054276)
				{
					this.m_htmlColor = Color.yellow;
					this.m_colorStack.Add(this.m_htmlColor);
					return true;
				}
			}
			return false;
			IL_289F:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			switch (tagUnits)
			{
			case TagUnits.Pixels:
				this.tag_LineIndent = num10;
				break;
			case TagUnits.FontUnits:
				this.tag_LineIndent = num10;
				this.tag_LineIndent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				this.tag_LineIndent = this.m_marginWidth * num10 / 100f;
				break;
			}
			this.m_xAdvance += this.tag_LineIndent;
			return true;
			IL_2977:
			this.tag_LineIndent = 0f;
			return true;
			IL_2E78:
			this.m_style &= (FontStyles)(-17);
			return true;
			IL_2FC9:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			this.m_marginLeft = num10;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
				break;
			case TagUnits.Percentage:
				this.m_marginLeft = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * this.m_marginLeft / 100f;
				break;
			}
			this.m_marginLeft = ((this.m_marginLeft >= 0f) ? this.m_marginLeft : 0f);
			return true;
			IL_31BD:
			num10 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
			if (num10 == -9999f || num10 == 0f)
			{
				return false;
			}
			this.m_lineHeight = num10;
			switch (tagUnits)
			{
			case TagUnits.FontUnits:
				this.m_lineHeight *= this.m_fontAsset.fontInfo.LineHeight * this.m_fontScale;
				break;
			case TagUnits.Percentage:
				this.m_lineHeight = this.m_fontAsset.fontInfo.LineHeight * this.m_lineHeight / 100f * this.m_fontScale;
				break;
			}
			return true;
			IL_3280:
			this.m_lineHeight = 0f;
			return true;
		}

		// Token: 0x040049AD RID: 18861
		[SerializeField]
		protected string m_text;

		// Token: 0x040049AE RID: 18862
		[SerializeField]
		protected bool m_isRightToLeft;

		// Token: 0x040049AF RID: 18863
		[SerializeField]
		protected TMP_FontAsset m_fontAsset;

		// Token: 0x040049B0 RID: 18864
		protected TMP_FontAsset m_currentFontAsset;

		// Token: 0x040049B1 RID: 18865
		protected bool m_isSDFShader;

		// Token: 0x040049B2 RID: 18866
		[SerializeField]
		protected Material m_sharedMaterial;

		// Token: 0x040049B3 RID: 18867
		protected Material m_currentMaterial;

		// Token: 0x040049B4 RID: 18868
		protected MaterialReference[] m_materialReferences = new MaterialReference[32];

		// Token: 0x040049B5 RID: 18869
		protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

		// Token: 0x040049B6 RID: 18870
		protected TMP_XmlTagStack<MaterialReference> m_materialReferenceStack = new TMP_XmlTagStack<MaterialReference>(new MaterialReference[16]);

		// Token: 0x040049B7 RID: 18871
		protected int m_currentMaterialIndex;

		// Token: 0x040049B8 RID: 18872
		[SerializeField]
		protected Material[] m_fontSharedMaterials;

		// Token: 0x040049B9 RID: 18873
		[SerializeField]
		protected Material m_fontMaterial;

		// Token: 0x040049BA RID: 18874
		[SerializeField]
		protected Material[] m_fontMaterials;

		// Token: 0x040049BB RID: 18875
		protected bool m_isMaterialDirty;

		// Token: 0x040049BC RID: 18876
		[FormerlySerializedAs("m_fontColor")]
		[SerializeField]
		protected Color32 m_fontColor32 = Color.white;

		// Token: 0x040049BD RID: 18877
		[SerializeField]
		protected Color m_fontColor = Color.white;

		// Token: 0x040049BE RID: 18878
		protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		// Token: 0x040049BF RID: 18879
		[SerializeField]
		protected bool m_enableVertexGradient;

		// Token: 0x040049C0 RID: 18880
		[SerializeField]
		protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		// Token: 0x040049C1 RID: 18881
		[SerializeField]
		protected TMP_ColorGradient m_fontColorGradientPreset;

		// Token: 0x040049C2 RID: 18882
		protected TMP_SpriteAsset m_spriteAsset;

		// Token: 0x040049C3 RID: 18883
		[SerializeField]
		protected bool m_tintAllSprites;

		// Token: 0x040049C4 RID: 18884
		protected bool m_tintSprite;

		// Token: 0x040049C5 RID: 18885
		protected Color32 m_spriteColor;

		// Token: 0x040049C6 RID: 18886
		[SerializeField]
		protected bool m_overrideHtmlColors;

		// Token: 0x040049C7 RID: 18887
		[SerializeField]
		protected Color32 m_faceColor = Color.white;

		// Token: 0x040049C8 RID: 18888
		[SerializeField]
		protected Color32 m_outlineColor = Color.black;

		// Token: 0x040049C9 RID: 18889
		protected float m_outlineWidth;

		// Token: 0x040049CA RID: 18890
		[SerializeField]
		protected float m_fontSize = 36f;

		// Token: 0x040049CB RID: 18891
		protected float m_currentFontSize;

		// Token: 0x040049CC RID: 18892
		[SerializeField]
		protected float m_fontSizeBase = 36f;

		// Token: 0x040049CD RID: 18893
		protected TMP_XmlTagStack<float> m_sizeStack = new TMP_XmlTagStack<float>(new float[16]);

		// Token: 0x040049CE RID: 18894
		[SerializeField]
		protected int m_fontWeight = 400;

		// Token: 0x040049CF RID: 18895
		protected int m_fontWeightInternal;

		// Token: 0x040049D0 RID: 18896
		protected TMP_XmlTagStack<int> m_fontWeightStack = new TMP_XmlTagStack<int>(new int[16]);

		// Token: 0x040049D1 RID: 18897
		[SerializeField]
		protected bool m_enableAutoSizing;

		// Token: 0x040049D2 RID: 18898
		protected float m_maxFontSize;

		// Token: 0x040049D3 RID: 18899
		protected float m_minFontSize;

		// Token: 0x040049D4 RID: 18900
		[SerializeField]
		protected float m_fontSizeMin;

		// Token: 0x040049D5 RID: 18901
		[SerializeField]
		protected float m_fontSizeMax;

		// Token: 0x040049D6 RID: 18902
		[SerializeField]
		protected FontStyles m_fontStyle;

		// Token: 0x040049D7 RID: 18903
		protected FontStyles m_style;

		// Token: 0x040049D8 RID: 18904
		protected bool m_isUsingBold;

		// Token: 0x040049D9 RID: 18905
		[SerializeField]
		[FormerlySerializedAs("m_lineJustification")]
		protected TextAlignmentOptions m_textAlignment;

		// Token: 0x040049DA RID: 18906
		protected TextAlignmentOptions m_lineJustification;

		// Token: 0x040049DB RID: 18907
		protected Vector3[] m_textContainerLocalCorners = new Vector3[4];

		// Token: 0x040049DC RID: 18908
		[SerializeField]
		protected float m_characterSpacing;

		// Token: 0x040049DD RID: 18909
		protected float m_cSpacing;

		// Token: 0x040049DE RID: 18910
		protected float m_monoSpacing;

		// Token: 0x040049DF RID: 18911
		[SerializeField]
		protected float m_lineSpacing;

		// Token: 0x040049E0 RID: 18912
		protected float m_lineSpacingDelta;

		// Token: 0x040049E1 RID: 18913
		protected float m_lineHeight;

		// Token: 0x040049E2 RID: 18914
		[SerializeField]
		protected float m_lineSpacingMax;

		// Token: 0x040049E3 RID: 18915
		[SerializeField]
		protected float m_paragraphSpacing;

		// Token: 0x040049E4 RID: 18916
		[SerializeField]
		protected float m_charWidthMaxAdj;

		// Token: 0x040049E5 RID: 18917
		protected float m_charWidthAdjDelta;

		// Token: 0x040049E6 RID: 18918
		[SerializeField]
		protected bool m_enableWordWrapping;

		// Token: 0x040049E7 RID: 18919
		protected bool m_isCharacterWrappingEnabled;

		// Token: 0x040049E8 RID: 18920
		protected bool m_isNonBreakingSpace;

		// Token: 0x040049E9 RID: 18921
		protected bool m_isIgnoringAlignment;

		// Token: 0x040049EA RID: 18922
		[SerializeField]
		protected float m_wordWrappingRatios = 0.4f;

		// Token: 0x040049EB RID: 18923
		[SerializeField]
		protected bool m_enableAdaptiveJustification;

		// Token: 0x040049EC RID: 18924
		protected float m_adaptiveJustificationThreshold = 10f;

		// Token: 0x040049ED RID: 18925
		[SerializeField]
		protected TextOverflowModes m_overflowMode;

		// Token: 0x040049EE RID: 18926
		protected bool m_isTextTruncated;

		// Token: 0x040049EF RID: 18927
		[SerializeField]
		protected bool m_enableKerning;

		// Token: 0x040049F0 RID: 18928
		[SerializeField]
		protected bool m_enableExtraPadding;

		// Token: 0x040049F1 RID: 18929
		[SerializeField]
		protected bool checkPaddingRequired;

		// Token: 0x040049F2 RID: 18930
		[SerializeField]
		protected bool m_isRichText = true;

		// Token: 0x040049F3 RID: 18931
		[SerializeField]
		protected bool m_parseCtrlCharacters = true;

		// Token: 0x040049F4 RID: 18932
		protected bool m_isOverlay;

		// Token: 0x040049F5 RID: 18933
		[SerializeField]
		protected bool m_isOrthographic;

		// Token: 0x040049F6 RID: 18934
		[SerializeField]
		protected bool m_isCullingEnabled;

		// Token: 0x040049F7 RID: 18935
		[SerializeField]
		protected bool m_ignoreCulling = true;

		// Token: 0x040049F8 RID: 18936
		[SerializeField]
		protected TextureMappingOptions m_horizontalMapping;

		// Token: 0x040049F9 RID: 18937
		[SerializeField]
		protected TextureMappingOptions m_verticalMapping;

		// Token: 0x040049FA RID: 18938
		protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

		// Token: 0x040049FB RID: 18939
		protected int m_maxVisibleCharacters = 99999;

		// Token: 0x040049FC RID: 18940
		protected int m_maxVisibleWords = 99999;

		// Token: 0x040049FD RID: 18941
		protected int m_maxVisibleLines = 99999;

		// Token: 0x040049FE RID: 18942
		[SerializeField]
		protected bool m_useMaxVisibleDescender = true;

		// Token: 0x040049FF RID: 18943
		[SerializeField]
		protected int m_pageToDisplay = 1;

		// Token: 0x04004A00 RID: 18944
		protected bool m_isNewPage;

		// Token: 0x04004A01 RID: 18945
		[SerializeField]
		protected Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		// Token: 0x04004A02 RID: 18946
		protected float m_marginLeft;

		// Token: 0x04004A03 RID: 18947
		protected float m_marginRight;

		// Token: 0x04004A04 RID: 18948
		protected float m_marginWidth;

		// Token: 0x04004A05 RID: 18949
		protected float m_marginHeight;

		// Token: 0x04004A06 RID: 18950
		protected float m_width = -1f;

		// Token: 0x04004A07 RID: 18951
		[SerializeField]
		protected TMP_TextInfo m_textInfo;

		// Token: 0x04004A08 RID: 18952
		[SerializeField]
		protected bool m_havePropertiesChanged;

		// Token: 0x04004A09 RID: 18953
		[SerializeField]
		protected bool m_isUsingLegacyAnimationComponent;

		// Token: 0x04004A0A RID: 18954
		protected Transform m_transform;

		// Token: 0x04004A0B RID: 18955
		protected RectTransform m_rectTransform;

		// Token: 0x04004A0D RID: 18957
		protected Mesh m_mesh;

		// Token: 0x04004A0E RID: 18958
		[SerializeField]
		protected bool m_isVolumetricText;

		// Token: 0x04004A0F RID: 18959
		protected float m_flexibleHeight = -1f;

		// Token: 0x04004A10 RID: 18960
		protected float m_flexibleWidth = -1f;

		// Token: 0x04004A11 RID: 18961
		protected float m_minHeight;

		// Token: 0x04004A12 RID: 18962
		protected float m_minWidth;

		// Token: 0x04004A13 RID: 18963
		protected float m_preferredWidth;

		// Token: 0x04004A14 RID: 18964
		protected float m_renderedWidth;

		// Token: 0x04004A15 RID: 18965
		protected bool m_isPreferredWidthDirty;

		// Token: 0x04004A16 RID: 18966
		protected float m_preferredHeight;

		// Token: 0x04004A17 RID: 18967
		protected float m_renderedHeight;

		// Token: 0x04004A18 RID: 18968
		protected bool m_isPreferredHeightDirty;

		// Token: 0x04004A19 RID: 18969
		protected bool m_isCalculatingPreferredValues;

		// Token: 0x04004A1A RID: 18970
		protected int m_layoutPriority;

		// Token: 0x04004A1B RID: 18971
		protected bool m_isCalculateSizeRequired;

		// Token: 0x04004A1C RID: 18972
		protected bool m_isLayoutDirty;

		// Token: 0x04004A1D RID: 18973
		protected bool m_verticesAlreadyDirty;

		// Token: 0x04004A1E RID: 18974
		protected bool m_layoutAlreadyDirty;

		// Token: 0x04004A1F RID: 18975
		protected bool m_isAwake;

		// Token: 0x04004A20 RID: 18976
		[SerializeField]
		protected bool m_isInputParsingRequired;

		// Token: 0x04004A21 RID: 18977
		[SerializeField]
		protected TMP_Text.TextInputSources m_inputSource;

		// Token: 0x04004A22 RID: 18978
		protected string old_text;

		// Token: 0x04004A23 RID: 18979
		protected float old_arg0;

		// Token: 0x04004A24 RID: 18980
		protected float old_arg1;

		// Token: 0x04004A25 RID: 18981
		protected float old_arg2;

		// Token: 0x04004A26 RID: 18982
		protected float m_fontScale;

		// Token: 0x04004A27 RID: 18983
		protected float m_fontScaleMultiplier;

		// Token: 0x04004A28 RID: 18984
		protected char[] m_htmlTag = new char[128];

		// Token: 0x04004A29 RID: 18985
		protected XML_TagAttribute[] m_xmlAttribute = new XML_TagAttribute[8];

		// Token: 0x04004A2A RID: 18986
		protected float tag_LineIndent;

		// Token: 0x04004A2B RID: 18987
		protected float tag_Indent;

		// Token: 0x04004A2C RID: 18988
		protected TMP_XmlTagStack<float> m_indentStack = new TMP_XmlTagStack<float>(new float[16]);

		// Token: 0x04004A2D RID: 18989
		protected bool tag_NoParsing;

		// Token: 0x04004A2E RID: 18990
		protected bool m_isParsingText;

		// Token: 0x04004A2F RID: 18991
		protected int[] m_char_buffer;

		// Token: 0x04004A30 RID: 18992
		private TMP_CharacterInfo[] m_internalCharacterInfo;

		// Token: 0x04004A31 RID: 18993
		protected char[] m_input_CharArray = new char[256];

		// Token: 0x04004A32 RID: 18994
		private int m_charArray_Length;

		// Token: 0x04004A33 RID: 18995
		protected int m_totalCharacterCount;

		// Token: 0x04004A34 RID: 18996
		protected int m_characterCount;

		// Token: 0x04004A35 RID: 18997
		protected int m_firstCharacterOfLine;

		// Token: 0x04004A36 RID: 18998
		protected int m_firstVisibleCharacterOfLine;

		// Token: 0x04004A37 RID: 18999
		protected int m_lastCharacterOfLine;

		// Token: 0x04004A38 RID: 19000
		protected int m_lastVisibleCharacterOfLine;

		// Token: 0x04004A39 RID: 19001
		protected int m_lineNumber;

		// Token: 0x04004A3A RID: 19002
		protected int m_lineVisibleCharacterCount;

		// Token: 0x04004A3B RID: 19003
		protected int m_pageNumber;

		// Token: 0x04004A3C RID: 19004
		protected float m_maxAscender;

		// Token: 0x04004A3D RID: 19005
		protected float m_maxCapHeight;

		// Token: 0x04004A3E RID: 19006
		protected float m_maxDescender;

		// Token: 0x04004A3F RID: 19007
		protected float m_maxLineAscender;

		// Token: 0x04004A40 RID: 19008
		protected float m_maxLineDescender;

		// Token: 0x04004A41 RID: 19009
		protected float m_startOfLineAscender;

		// Token: 0x04004A42 RID: 19010
		protected float m_lineOffset;

		// Token: 0x04004A43 RID: 19011
		protected Extents m_meshExtents;

		// Token: 0x04004A44 RID: 19012
		protected Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		// Token: 0x04004A45 RID: 19013
		protected TMP_XmlTagStack<Color32> m_colorStack = new TMP_XmlTagStack<Color32>(new Color32[16]);

		// Token: 0x04004A46 RID: 19014
		protected float m_tabSpacing;

		// Token: 0x04004A47 RID: 19015
		protected float m_spacing;

		// Token: 0x04004A48 RID: 19016
		protected TMP_XmlTagStack<int> m_styleStack = new TMP_XmlTagStack<int>(new int[16]);

		// Token: 0x04004A49 RID: 19017
		protected TMP_XmlTagStack<int> m_actionStack = new TMP_XmlTagStack<int>(new int[16]);

		// Token: 0x04004A4A RID: 19018
		protected float m_padding;

		// Token: 0x04004A4B RID: 19019
		protected float m_baselineOffset;

		// Token: 0x04004A4C RID: 19020
		protected float m_xAdvance;

		// Token: 0x04004A4D RID: 19021
		protected TMP_TextElementType m_textElementType;

		// Token: 0x04004A4E RID: 19022
		protected TMP_TextElement m_cached_TextElement;

		// Token: 0x04004A4F RID: 19023
		protected TMP_Glyph m_cached_Underline_GlyphInfo;

		// Token: 0x04004A50 RID: 19024
		protected TMP_Glyph m_cached_Ellipsis_GlyphInfo;

		// Token: 0x04004A51 RID: 19025
		protected TMP_SpriteAsset m_defaultSpriteAsset;

		// Token: 0x04004A52 RID: 19026
		protected TMP_SpriteAsset m_currentSpriteAsset;

		// Token: 0x04004A53 RID: 19027
		protected int m_spriteCount;

		// Token: 0x04004A54 RID: 19028
		protected int m_spriteIndex;

		// Token: 0x04004A55 RID: 19029
		protected InlineGraphicManager m_inlineGraphics;

		// Token: 0x04004A56 RID: 19030
		protected bool m_ignoreActiveState;

		// Token: 0x04004A57 RID: 19031
		private readonly float[] k_Power = new float[]
		{
			0.5f,
			0.05f,
			0.005f,
			0.0005f,
			5E-05f,
			5E-06f,
			5E-07f,
			5E-08f,
			5E-09f,
			5E-10f
		};

		// Token: 0x04004A58 RID: 19032
		protected static Vector2 k_LargePositiveVector2 = new Vector2(2.1474836E+09f, 2.1474836E+09f);

		// Token: 0x04004A59 RID: 19033
		protected static Vector2 k_LargeNegativeVector2 = new Vector2(-2.1474836E+09f, -2.1474836E+09f);

		// Token: 0x04004A5A RID: 19034
		protected static float k_LargePositiveFloat = 32768f;

		// Token: 0x04004A5B RID: 19035
		protected static float k_LargeNegativeFloat = -32768f;

		// Token: 0x04004A5C RID: 19036
		protected static int k_LargePositiveInt = int.MaxValue;

		// Token: 0x04004A5D RID: 19037
		protected static int k_LargeNegativeInt = -2147483647;

		// Token: 0x02001AD2 RID: 6866
		protected enum TextInputSources
		{
			// Token: 0x04009A95 RID: 39573
			Text,
			// Token: 0x04009A96 RID: 39574
			SetText,
			// Token: 0x04009A97 RID: 39575
			SetCharArray,
			// Token: 0x04009A98 RID: 39576
			String
		}
	}
}
