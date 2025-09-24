using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class CollectableRelicManager : ManagerSingleton<CollectableRelicManager>
{
	// Token: 0x06001313 RID: 4883 RVA: 0x00057D7D File Offset: 0x00055F7D
	public static CollectableRelicsData.Data GetRelicData(CollectableRelic relic)
	{
		return PlayerData.instance.Relics.GetData(relic.name);
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x00057D94 File Offset: 0x00055F94
	public static void SetRelicData(CollectableRelic relic, CollectableRelicsData.Data data)
	{
		PlayerData.instance.Relics.SetData(relic.name, data);
		CollectableItemManager.IncrementVersion();
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00057DB1 File Offset: 0x00055FB1
	public static CollectableRelic GetRelic(string relicName)
	{
		return ManagerSingleton<CollectableRelicManager>.Instance.masterList.GetByName(relicName);
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x00057DC3 File Offset: 0x00055FC3
	public static IReadOnlyList<CollectableRelic> GetAllRelics()
	{
		return ManagerSingleton<CollectableRelicManager>.Instance.masterList;
	}

	// Token: 0x040011A8 RID: 4520
	[SerializeField]
	private CollectableRelicList masterList;
}
