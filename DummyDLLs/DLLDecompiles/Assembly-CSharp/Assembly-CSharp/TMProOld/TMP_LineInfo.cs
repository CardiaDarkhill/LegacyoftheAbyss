using System;

namespace TMProOld
{
	// Token: 0x02000811 RID: 2065
	public struct TMP_LineInfo
	{
		// Token: 0x040048EE RID: 18670
		public int characterCount;

		// Token: 0x040048EF RID: 18671
		public int visibleCharacterCount;

		// Token: 0x040048F0 RID: 18672
		public int spaceCount;

		// Token: 0x040048F1 RID: 18673
		public int wordCount;

		// Token: 0x040048F2 RID: 18674
		public int firstCharacterIndex;

		// Token: 0x040048F3 RID: 18675
		public int firstVisibleCharacterIndex;

		// Token: 0x040048F4 RID: 18676
		public int lastCharacterIndex;

		// Token: 0x040048F5 RID: 18677
		public int lastVisibleCharacterIndex;

		// Token: 0x040048F6 RID: 18678
		public float length;

		// Token: 0x040048F7 RID: 18679
		public float lineHeight;

		// Token: 0x040048F8 RID: 18680
		public float ascender;

		// Token: 0x040048F9 RID: 18681
		public float baseline;

		// Token: 0x040048FA RID: 18682
		public float descender;

		// Token: 0x040048FB RID: 18683
		public float maxAdvance;

		// Token: 0x040048FC RID: 18684
		public float width;

		// Token: 0x040048FD RID: 18685
		public float marginLeft;

		// Token: 0x040048FE RID: 18686
		public float marginRight;

		// Token: 0x040048FF RID: 18687
		public TextAlignmentOptions alignment;

		// Token: 0x04004900 RID: 18688
		public Extents lineExtents;
	}
}
