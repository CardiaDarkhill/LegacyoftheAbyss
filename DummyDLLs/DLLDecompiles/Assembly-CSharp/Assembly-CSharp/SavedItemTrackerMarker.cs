using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007CB RID: 1995
public class SavedItemTrackerMarker : MonoBehaviour
{
	// Token: 0x170007EE RID: 2030
	// (get) Token: 0x0600464A RID: 17994 RVA: 0x001310D6 File Offset: 0x0012F2D6
	public IReadOnlyCollection<SavedItem> Items
	{
		get
		{
			return this.items;
		}
	}

	// Token: 0x040046BD RID: 18109
	[SerializeField]
	private SavedItem[] items;
}
