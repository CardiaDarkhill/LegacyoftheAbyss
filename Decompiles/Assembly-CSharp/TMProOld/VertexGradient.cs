using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007F9 RID: 2041
	[Serializable]
	public struct VertexGradient
	{
		// Token: 0x060047D5 RID: 18389 RVA: 0x0014DE70 File Offset: 0x0014C070
		public VertexGradient(Color color)
		{
			this.topLeft = color;
			this.topRight = color;
			this.bottomLeft = color;
			this.bottomRight = color;
		}

		// Token: 0x060047D6 RID: 18390 RVA: 0x0014DE8E File Offset: 0x0014C08E
		public VertexGradient(Color color0, Color color1, Color color2, Color color3)
		{
			this.topLeft = color0;
			this.topRight = color1;
			this.bottomLeft = color2;
			this.bottomRight = color3;
		}

		// Token: 0x040047EF RID: 18415
		public Color topLeft;

		// Token: 0x040047F0 RID: 18416
		public Color topRight;

		// Token: 0x040047F1 RID: 18417
		public Color bottomLeft;

		// Token: 0x040047F2 RID: 18418
		public Color bottomRight;
	}
}
