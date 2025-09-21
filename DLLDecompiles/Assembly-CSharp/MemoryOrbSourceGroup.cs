using System;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class MemoryOrbSourceGroup : MemoryOrbSource
{
	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06002238 RID: 8760 RVA: 0x0009DE54 File Offset: 0x0009C054
	protected override bool IsActive
	{
		get
		{
			MemoryOrbGroup[] array = this.groups;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsAllCollected)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x04002108 RID: 8456
	[Space]
	[SerializeField]
	private MemoryOrbGroup[] groups;
}
