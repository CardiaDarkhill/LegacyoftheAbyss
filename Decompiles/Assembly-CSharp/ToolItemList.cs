using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020005EB RID: 1515
[CreateAssetMenu(fileName = "New Tool List", menuName = "Hornet/Tool Item List")]
public class ToolItemList : NamedScriptableObjectList<ToolItem>
{
	// Token: 0x060035F6 RID: 13814 RVA: 0x000EDC44 File Offset: 0x000EBE44
	[ContextMenu("Sort By Type")]
	public void SortByType()
	{
		IEnumerable<ToolItemType> enumerable = typeof(ToolItemType).GetValuesWithOrder().Cast<ToolItemType>();
		Dictionary<ToolItemType, List<ToolItem>> dictionary = new Dictionary<ToolItemType, List<ToolItem>>(enumerable.Count<ToolItemType>());
		foreach (ToolItemType key in enumerable)
		{
			dictionary[key] = new List<ToolItem>();
		}
		foreach (ToolItem toolItem in base.List)
		{
			if (!(toolItem == null))
			{
				dictionary[toolItem.Type].Add(toolItem);
			}
		}
		base.List.Clear();
		foreach (ToolItemType key2 in enumerable)
		{
			base.List.AddRange(dictionary[key2]);
		}
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x000EDD5C File Offset: 0x000EBF5C
	[ContextMenu("Unlock All", true)]
	public bool CanUnlockAll()
	{
		return Application.isPlaying;
	}

	// Token: 0x060035F8 RID: 13816 RVA: 0x000EDD64 File Offset: 0x000EBF64
	[ContextMenu("Unlock All")]
	public void UnlockAll()
	{
		PlayerData instance = PlayerData.instance;
		if (instance != null)
		{
			instance.SeenToolGetPrompt = true;
			instance.SeenToolWeaponGetPrompt = true;
		}
		foreach (ToolItem toolItem in this)
		{
			if (toolItem)
			{
				toolItem.SetUnlockedTestsComplete();
				toolItem.Unlock(null, ToolItem.PopupFlags.Default);
			}
		}
	}
}
