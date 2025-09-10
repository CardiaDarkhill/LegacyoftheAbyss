using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200058E RID: 1422
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Tool Damage)")]
public class CollectableItemToolDamage : CollectableItemBasic
{
	// Token: 0x060032E0 RID: 13024 RVA: 0x000E2245 File Offset: 0x000E0445
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<int>(ref this.dropAmounts, typeof(HealthManager.EnemySize));
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x000E225C File Offset: 0x000E045C
	public static CollectableItemToolDamage GetItem(ToolDamageFlags damageFlags, HealthManager.EnemySize enemySize, out int amount)
	{
		amount = 0;
		if (damageFlags == ToolDamageFlags.None)
		{
			return null;
		}
		foreach (CollectableItemToolDamage collectableItemToolDamage in QuestManager.GetActiveQuests().SelectMany((FullQuestBase quest) => from target in quest.Targets
		select target.Counter).OfType<CollectableItemToolDamage>())
		{
			if ((damageFlags & collectableItemToolDamage.fromDamageType) != ToolDamageFlags.None)
			{
				amount = collectableItemToolDamage.dropAmounts[(int)enemySize];
				return collectableItemToolDamage;
			}
		}
		return null;
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x000E22F0 File Offset: 0x000E04F0
	public override Sprite GetQuestCounterSprite(int index)
	{
		return this.GetIcon(CollectableItem.ReadSource.Inventory);
	}

	// Token: 0x040036C5 RID: 14021
	[Space]
	[SerializeField]
	private ToolDamageFlags fromDamageType;

	// Token: 0x040036C6 RID: 14022
	[SerializeField]
	[ArrayForEnum(typeof(HealthManager.EnemySize))]
	private int[] dropAmounts;
}
