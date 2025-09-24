using System;

namespace TMProOld
{
	// Token: 0x020007F4 RID: 2036
	[Serializable]
	public class KerningPair
	{
		// Token: 0x060047CE RID: 18382 RVA: 0x0014DC9E File Offset: 0x0014BE9E
		public KerningPair(int left, int right, float offset)
		{
			this.AscII_Left = left;
			this.AscII_Right = right;
			this.XadvanceOffset = offset;
		}

		// Token: 0x040047BE RID: 18366
		public int AscII_Left;

		// Token: 0x040047BF RID: 18367
		public int AscII_Right;

		// Token: 0x040047C0 RID: 18368
		public float XadvanceOffset;
	}
}
