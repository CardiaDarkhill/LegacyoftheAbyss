using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AE RID: 430
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item List")]
public class CollectableItemList : NamedScriptableObjectList<CollectableItem>, ICollectionViewerItemList
{
	// Token: 0x060010B8 RID: 4280 RVA: 0x0004F472 File Offset: 0x0004D672
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableItem collectableItem in base.List)
		{
			yield return collectableItem;
		}
		List<CollectableItem>.Enumerator enumerator = default(List<CollectableItem>.Enumerator);
		yield break;
		yield break;
	}
}
