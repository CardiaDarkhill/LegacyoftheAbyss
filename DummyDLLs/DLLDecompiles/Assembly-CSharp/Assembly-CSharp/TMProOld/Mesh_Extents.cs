using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007FF RID: 2047
	[Serializable]
	public struct Mesh_Extents
	{
		// Token: 0x060047DD RID: 18397 RVA: 0x0014E085 File Offset: 0x0014C285
		public Mesh_Extents(Vector2 min, Vector2 max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x060047DE RID: 18398 RVA: 0x0014E098 File Offset: 0x0014C298
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

		// Token: 0x04004808 RID: 18440
		public Vector2 min;

		// Token: 0x04004809 RID: 18441
		public Vector2 max;
	}
}
