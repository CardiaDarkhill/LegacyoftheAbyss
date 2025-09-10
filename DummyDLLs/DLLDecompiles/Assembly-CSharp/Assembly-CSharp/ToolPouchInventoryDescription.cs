using System;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public class ToolPouchInventoryDescription : MonoBehaviour
{
	// Token: 0x06003E90 RID: 16016 RVA: 0x00113AC8 File Offset: 0x00111CC8
	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		this.UpdateDisplay(instance.ToolPouchUpgrades, this.pouchUpgradesGroup, this.pouchUpgradesCounter);
		this.UpdateDisplay(instance.ToolKitUpgrades, this.kitUpgradesGroup, this.kitUpgradesCounter);
	}

	// Token: 0x06003E91 RID: 16017 RVA: 0x00113B0B File Offset: 0x00111D0B
	private void UpdateDisplay(int upgrades, GameObject group, IconCounter counter)
	{
		if (upgrades > 0)
		{
			if (group)
			{
				group.SetActive(true);
			}
			if (counter)
			{
				counter.CurrentValue = upgrades;
				return;
			}
		}
		else if (group)
		{
			group.SetActive(false);
		}
	}

	// Token: 0x04004037 RID: 16439
	[SerializeField]
	private GameObject pouchUpgradesGroup;

	// Token: 0x04004038 RID: 16440
	[SerializeField]
	private IconCounter pouchUpgradesCounter;

	// Token: 0x04004039 RID: 16441
	[SerializeField]
	private GameObject kitUpgradesGroup;

	// Token: 0x0400403A RID: 16442
	[SerializeField]
	private IconCounter kitUpgradesCounter;
}
