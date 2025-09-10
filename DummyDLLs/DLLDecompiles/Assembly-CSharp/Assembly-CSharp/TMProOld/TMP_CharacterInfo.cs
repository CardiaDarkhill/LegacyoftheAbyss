using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007F7 RID: 2039
	public struct TMP_CharacterInfo
	{
		// Token: 0x040047CA RID: 18378
		public char character;

		// Token: 0x040047CB RID: 18379
		public short index;

		// Token: 0x040047CC RID: 18380
		public TMP_TextElementType elementType;

		// Token: 0x040047CD RID: 18381
		public TMP_TextElement textElement;

		// Token: 0x040047CE RID: 18382
		public TMP_FontAsset fontAsset;

		// Token: 0x040047CF RID: 18383
		public TMP_SpriteAsset spriteAsset;

		// Token: 0x040047D0 RID: 18384
		public int spriteIndex;

		// Token: 0x040047D1 RID: 18385
		public Material material;

		// Token: 0x040047D2 RID: 18386
		public int materialReferenceIndex;

		// Token: 0x040047D3 RID: 18387
		public bool isUsingAlternateTypeface;

		// Token: 0x040047D4 RID: 18388
		public float pointSize;

		// Token: 0x040047D5 RID: 18389
		public short lineNumber;

		// Token: 0x040047D6 RID: 18390
		public short pageNumber;

		// Token: 0x040047D7 RID: 18391
		public int vertexIndex;

		// Token: 0x040047D8 RID: 18392
		public TMP_Vertex vertex_TL;

		// Token: 0x040047D9 RID: 18393
		public TMP_Vertex vertex_BL;

		// Token: 0x040047DA RID: 18394
		public TMP_Vertex vertex_TR;

		// Token: 0x040047DB RID: 18395
		public TMP_Vertex vertex_BR;

		// Token: 0x040047DC RID: 18396
		public Vector3 topLeft;

		// Token: 0x040047DD RID: 18397
		public Vector3 bottomLeft;

		// Token: 0x040047DE RID: 18398
		public Vector3 topRight;

		// Token: 0x040047DF RID: 18399
		public Vector3 bottomRight;

		// Token: 0x040047E0 RID: 18400
		public float origin;

		// Token: 0x040047E1 RID: 18401
		public float ascender;

		// Token: 0x040047E2 RID: 18402
		public float baseLine;

		// Token: 0x040047E3 RID: 18403
		public float descender;

		// Token: 0x040047E4 RID: 18404
		public float xAdvance;

		// Token: 0x040047E5 RID: 18405
		public float aspectRatio;

		// Token: 0x040047E6 RID: 18406
		public float scale;

		// Token: 0x040047E7 RID: 18407
		public Color32 color;

		// Token: 0x040047E8 RID: 18408
		public FontStyles style;

		// Token: 0x040047E9 RID: 18409
		public bool isVisible;
	}
}
