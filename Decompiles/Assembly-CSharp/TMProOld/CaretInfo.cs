using System;

namespace TMProOld
{
	// Token: 0x0200082E RID: 2094
	public struct CaretInfo
	{
		// Token: 0x06004A7E RID: 19070 RVA: 0x0016090F File Offset: 0x0015EB0F
		public CaretInfo(int index, CaretPosition position)
		{
			this.index = index;
			this.position = position;
		}

		// Token: 0x04004A7D RID: 19069
		public int index;

		// Token: 0x04004A7E RID: 19070
		public CaretPosition position;
	}
}
