using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007EC RID: 2028
	public class Compute_DT_EventArgs
	{
		// Token: 0x060047BB RID: 18363 RVA: 0x0014D79D File Offset: 0x0014B99D
		public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, float progress)
		{
			this.EventType = type;
			this.ProgressPercentage = progress;
		}

		// Token: 0x060047BC RID: 18364 RVA: 0x0014D7B3 File Offset: 0x0014B9B3
		public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, Color[] colors)
		{
			this.EventType = type;
			this.Colors = colors;
		}

		// Token: 0x0400478D RID: 18317
		public Compute_DistanceTransform_EventTypes EventType;

		// Token: 0x0400478E RID: 18318
		public float ProgressPercentage;

		// Token: 0x0400478F RID: 18319
		public Color[] Colors;
	}
}
