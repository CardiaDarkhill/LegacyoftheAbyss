using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class CollectableItemRelicType : CollectableItem, ICollectionViewerItemList
{
	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06001113 RID: 4371 RVA: 0x000508B2 File Offset: 0x0004EAB2
	public override bool DisplayAmount
	{
		get
		{
			return this.CollectedAmount > 1;
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001114 RID: 4372 RVA: 0x000508BD File Offset: 0x0004EABD
	public override int CollectedAmount
	{
		get
		{
			return this.relics.Count((CollectableRelic relic) => relic.IsInInventory);
		}
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001115 RID: 4373 RVA: 0x000508E9 File Offset: 0x0004EAE9
	public CustomInventoryItemCollectableDisplay IconOverridePrefab
	{
		get
		{
			return this.iconOverridePrefab;
		}
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06001116 RID: 4374 RVA: 0x000508F1 File Offset: 0x0004EAF1
	public CollectableItemRelicType.RelicPlayTypes RelicPlayType
	{
		get
		{
			return this.relicPlayType;
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001117 RID: 4375 RVA: 0x000508F9 File Offset: 0x0004EAF9
	public int RewardAmount
	{
		get
		{
			return this.rewardAmount;
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001118 RID: 4376 RVA: 0x00050901 File Offset: 0x0004EB01
	public IEnumerable<CollectableRelic> Relics
	{
		get
		{
			return this.relics;
		}
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0005090C File Offset: 0x0004EB0C
	private void OnEnable()
	{
		for (int i = this.relics.Count - 1; i >= 0; i--)
		{
			CollectableRelic collectableRelic = this.relics[i];
			if (collectableRelic == null)
			{
				this.relics.RemoveAt(i);
			}
			else
			{
				collectableRelic.RelicType = this;
			}
		}
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x0005095C File Offset: 0x0004EB5C
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.typeName;
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00050969 File Offset: 0x0004EB69
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		if (!this.appendDescription.IsEmpty)
		{
			return string.Format("{0}\n\n{1}", this.typeDescription, this.appendDescription);
		}
		return this.typeDescription;
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x000509A4 File Offset: 0x0004EBA4
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		return this.typeIcon;
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x000509AC File Offset: 0x0004EBAC
	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableRelic collectableRelic in this.relics)
		{
			yield return collectableRelic;
		}
		List<CollectableRelic>.Enumerator enumerator = default(List<CollectableRelic>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04001025 RID: 4133
	[Space]
	[SerializeField]
	private LocalisedString typeName;

	// Token: 0x04001026 RID: 4134
	[SerializeField]
	private LocalisedString typeDescription;

	// Token: 0x04001027 RID: 4135
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString appendDescription;

	// Token: 0x04001028 RID: 4136
	[SerializeField]
	private Sprite typeIcon;

	// Token: 0x04001029 RID: 4137
	[SerializeField]
	private CustomInventoryItemCollectableDisplay iconOverridePrefab;

	// Token: 0x0400102A RID: 4138
	[Space]
	[SerializeField]
	private CollectableItemRelicType.RelicPlayTypes relicPlayType;

	// Token: 0x0400102B RID: 4139
	[SerializeField]
	private int rewardAmount;

	// Token: 0x0400102C RID: 4140
	[Space]
	[SerializeField]
	private List<CollectableRelic> relics = new List<CollectableRelic>();

	// Token: 0x020014F6 RID: 5366
	public enum RelicPlayTypes
	{
		// Token: 0x0400855B RID: 34139
		None,
		// Token: 0x0400855C RID: 34140
		Gramaphone
	}
}
