using System;

namespace TMProOld
{
	// Token: 0x020007F0 RID: 2032
	[Serializable]
	public class TMP_Glyph : TMP_TextElement
	{
		// Token: 0x060047CA RID: 18378 RVA: 0x0014DBF4 File Offset: 0x0014BDF4
		public static TMP_Glyph Clone(TMP_Glyph source)
		{
			return new TMP_Glyph
			{
				id = source.id,
				x = source.x,
				y = source.y,
				width = source.width,
				height = source.height,
				xOffset = source.xOffset,
				yOffset = source.yOffset,
				xAdvance = source.xAdvance,
				scale = source.scale
			};
		}
	}
}
