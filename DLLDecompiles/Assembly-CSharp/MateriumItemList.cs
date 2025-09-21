using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C9 RID: 457
[CreateAssetMenu(menuName = "Hornet/Materium/Materium Item List")]
public class MateriumItemList : NamedScriptableObjectList<MateriumItem>, ICollectionViewerItemList
{
	// Token: 0x060011EF RID: 4591 RVA: 0x00053B34 File Offset: 0x00051D34
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (MateriumItem materiumItem in base.List)
		{
			yield return materiumItem;
		}
		List<MateriumItem>.Enumerator enumerator = default(List<MateriumItem>.Enumerator);
		yield break;
		yield break;
	}
}
