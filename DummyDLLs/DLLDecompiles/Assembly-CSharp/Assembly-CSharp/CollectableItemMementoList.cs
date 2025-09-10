using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B1 RID: 433
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item Memento List")]
public class CollectableItemMementoList : NamedScriptableObjectList<CollectableItemMemento>, ICollectionViewerItemList
{
	// Token: 0x060010DD RID: 4317 RVA: 0x0004FB36 File Offset: 0x0004DD36
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableItemMemento collectableItemMemento in base.List)
		{
			yield return collectableItemMemento;
		}
		List<CollectableItemMemento>.Enumerator enumerator = default(List<CollectableItemMemento>.Enumerator);
		yield break;
		yield break;
	}
}
