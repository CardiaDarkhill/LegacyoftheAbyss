using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000800 RID: 2048
	public struct WordWrapState
	{
		// Token: 0x0400480A RID: 18442
		public int previous_WordBreak;

		// Token: 0x0400480B RID: 18443
		public int total_CharacterCount;

		// Token: 0x0400480C RID: 18444
		public int visible_CharacterCount;

		// Token: 0x0400480D RID: 18445
		public int visible_SpriteCount;

		// Token: 0x0400480E RID: 18446
		public int visible_LinkCount;

		// Token: 0x0400480F RID: 18447
		public int firstCharacterIndex;

		// Token: 0x04004810 RID: 18448
		public int firstVisibleCharacterIndex;

		// Token: 0x04004811 RID: 18449
		public int lastCharacterIndex;

		// Token: 0x04004812 RID: 18450
		public int lastVisibleCharIndex;

		// Token: 0x04004813 RID: 18451
		public int lineNumber;

		// Token: 0x04004814 RID: 18452
		public float maxCapHeight;

		// Token: 0x04004815 RID: 18453
		public float maxAscender;

		// Token: 0x04004816 RID: 18454
		public float maxDescender;

		// Token: 0x04004817 RID: 18455
		public float maxLineAscender;

		// Token: 0x04004818 RID: 18456
		public float maxLineDescender;

		// Token: 0x04004819 RID: 18457
		public float previousLineAscender;

		// Token: 0x0400481A RID: 18458
		public float xAdvance;

		// Token: 0x0400481B RID: 18459
		public float preferredWidth;

		// Token: 0x0400481C RID: 18460
		public float preferredHeight;

		// Token: 0x0400481D RID: 18461
		public float previousLineScale;

		// Token: 0x0400481E RID: 18462
		public int wordCount;

		// Token: 0x0400481F RID: 18463
		public FontStyles fontStyle;

		// Token: 0x04004820 RID: 18464
		public float fontScale;

		// Token: 0x04004821 RID: 18465
		public float fontScaleMultiplier;

		// Token: 0x04004822 RID: 18466
		public float currentFontSize;

		// Token: 0x04004823 RID: 18467
		public float baselineOffset;

		// Token: 0x04004824 RID: 18468
		public float lineOffset;

		// Token: 0x04004825 RID: 18469
		public TMP_TextInfo textInfo;

		// Token: 0x04004826 RID: 18470
		public TMP_LineInfo lineInfo;

		// Token: 0x04004827 RID: 18471
		public Color32 vertexColor;

		// Token: 0x04004828 RID: 18472
		public TMP_XmlTagStack<Color32> colorStack;

		// Token: 0x04004829 RID: 18473
		public TMP_XmlTagStack<float> sizeStack;

		// Token: 0x0400482A RID: 18474
		public TMP_XmlTagStack<int> fontWeightStack;

		// Token: 0x0400482B RID: 18475
		public TMP_XmlTagStack<int> styleStack;

		// Token: 0x0400482C RID: 18476
		public TMP_XmlTagStack<int> actionStack;

		// Token: 0x0400482D RID: 18477
		public TMP_XmlTagStack<MaterialReference> materialReferenceStack;

		// Token: 0x0400482E RID: 18478
		public TMP_FontAsset currentFontAsset;

		// Token: 0x0400482F RID: 18479
		public TMP_SpriteAsset currentSpriteAsset;

		// Token: 0x04004830 RID: 18480
		public Material currentMaterial;

		// Token: 0x04004831 RID: 18481
		public int currentMaterialIndex;

		// Token: 0x04004832 RID: 18482
		public Extents meshExtents;

		// Token: 0x04004833 RID: 18483
		public bool tagNoParsing;
	}
}
