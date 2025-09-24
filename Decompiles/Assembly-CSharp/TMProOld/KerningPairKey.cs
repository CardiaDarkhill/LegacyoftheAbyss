using System;

namespace TMProOld
{
	// Token: 0x020007F3 RID: 2035
	public struct KerningPairKey
	{
		// Token: 0x060047CD RID: 18381 RVA: 0x0014DC82 File Offset: 0x0014BE82
		public KerningPairKey(int ascii_left, int ascii_right)
		{
			this.ascii_Left = ascii_left;
			this.ascii_Right = ascii_right;
			this.key = (ascii_right << 16) + ascii_left;
		}

		// Token: 0x040047BB RID: 18363
		public int ascii_Left;

		// Token: 0x040047BC RID: 18364
		public int ascii_Right;

		// Token: 0x040047BD RID: 18365
		public int key;
	}
}
