using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007FE RID: 2046
	public struct Extents
	{
		// Token: 0x060047DB RID: 18395 RVA: 0x0014DFD1 File Offset: 0x0014C1D1
		public Extents(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x060047DC RID: 18396 RVA: 0x0014DFE4 File Offset: 0x0014C1E4
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Min (",
				this.min.x.ToString("f2"),
				", ",
				this.min.y.ToString("f2"),
				")   Max (",
				this.max.x.ToString("f2"),
				", ",
				this.max.y.ToString("f2"),
				")"
			});
		}

		// Token: 0x04004806 RID: 18438
		public Vector2 min;

		// Token: 0x04004807 RID: 18439
		public Vector2 max;
	}
}
