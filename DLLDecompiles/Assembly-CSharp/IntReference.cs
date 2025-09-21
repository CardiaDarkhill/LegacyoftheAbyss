using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000674 RID: 1652
public class IntReference : ScriptableObject
{
	// Token: 0x170006B2 RID: 1714
	// (get) Token: 0x06003B3A RID: 15162 RVA: 0x00104D1A File Offset: 0x00102F1A
	public int Value
	{
		get
		{
			return this.value;
		}
	}

	// Token: 0x04003D7E RID: 15742
	[SerializeField]
	[FormerlySerializedAs("cost")]
	private int value;
}
