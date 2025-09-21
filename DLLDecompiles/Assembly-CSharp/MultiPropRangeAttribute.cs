using System;
using UnityEngine;

// Token: 0x02000775 RID: 1909
[AttributeUsage(AttributeTargets.Field)]
public class MultiPropRangeAttribute : PropertyAttribute
{
	// Token: 0x06004400 RID: 17408 RVA: 0x0012A736 File Offset: 0x00128936
	public MultiPropRangeAttribute(float min, float max)
	{
		this.Min = min;
		this.Max = max;
	}

	// Token: 0x04004547 RID: 17735
	public readonly float Min;

	// Token: 0x04004548 RID: 17736
	public readonly float Max;
}
