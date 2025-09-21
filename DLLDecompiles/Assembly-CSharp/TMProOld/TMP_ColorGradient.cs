using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000805 RID: 2053
	[Serializable]
	public class TMP_ColorGradient : ScriptableObject
	{
		// Token: 0x060047E7 RID: 18407 RVA: 0x0014EFA0 File Offset: 0x0014D1A0
		public TMP_ColorGradient()
		{
			Color white = Color.white;
			this.topLeft = white;
			this.topRight = white;
			this.bottomLeft = white;
			this.bottomRight = white;
		}

		// Token: 0x060047E8 RID: 18408 RVA: 0x0014EFD5 File Offset: 0x0014D1D5
		public TMP_ColorGradient(Color color)
		{
			this.topLeft = color;
			this.topRight = color;
			this.bottomLeft = color;
			this.bottomRight = color;
		}

		// Token: 0x060047E9 RID: 18409 RVA: 0x0014EFF9 File Offset: 0x0014D1F9
		public TMP_ColorGradient(Color color0, Color color1, Color color2, Color color3)
		{
			this.topLeft = color0;
			this.topRight = color1;
			this.bottomLeft = color2;
			this.bottomRight = color3;
		}

		// Token: 0x0400487B RID: 18555
		public Color topLeft;

		// Token: 0x0400487C RID: 18556
		public Color topRight;

		// Token: 0x0400487D RID: 18557
		public Color bottomLeft;

		// Token: 0x0400487E RID: 18558
		public Color bottomRight;
	}
}
