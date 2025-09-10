using System;
using UnityEngine;

// Token: 0x0200077D RID: 1917
public class WeightedItem
{
	// Token: 0x170007A1 RID: 1953
	// (get) Token: 0x0600441A RID: 17434 RVA: 0x0012AF0A File Offset: 0x0012910A
	public float Weight
	{
		get
		{
			return this.weight;
		}
	}

	// Token: 0x0400455B RID: 17755
	[SerializeField]
	[Range(0.001f, 10f)]
	private float weight;
}
