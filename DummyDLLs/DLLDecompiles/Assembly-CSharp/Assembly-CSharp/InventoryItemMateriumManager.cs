using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200069B RID: 1691
public class InventoryItemMateriumManager : InventoryItemListManager<InventoryItemMaterium, MateriumItem>
{
	// Token: 0x06003C5A RID: 15450 RVA: 0x001097BC File Offset: 0x001079BC
	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase component = base.GetComponent<InventoryPaneBase>();
		if (component)
		{
			component.OnPaneStart += MateriumItemManager.CheckAchievements;
		}
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x001097F0 File Offset: 0x001079F0
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if (this.displayIcon)
		{
			this.displayIcon.sprite = null;
		}
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x00109814 File Offset: 0x00107A14
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		InventoryItemMaterium inventoryItemMaterium = selectable as InventoryItemMaterium;
		if (inventoryItemMaterium == null)
		{
			base.SetDisplay(selectable);
			return;
		}
		if (inventoryItemMaterium.ItemData.IsCollected)
		{
			base.SetDisplay(selectable);
			if (this.displayIcon)
			{
				this.displayIcon.sprite = inventoryItemMaterium.Sprite;
				return;
			}
		}
		else
		{
			this.SetDisplay(selectable.gameObject);
		}
	}

	// Token: 0x06003C5D RID: 15453 RVA: 0x00109878 File Offset: 0x00107A78
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemMaterium> selectableItems, List<MateriumItem> items)
	{
		for (int i = 0; i < selectableItems.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(true);
			selectableItems[i].ItemData = items[i];
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			}
		};
	}

	// Token: 0x06003C5E RID: 15454 RVA: 0x001098DC File Offset: 0x00107ADC
	protected override List<MateriumItem> GetItems()
	{
		return (from record in ManagerSingleton<MateriumItemManager>.Instance.MasterList
		where record.IsRequiredForCompletion || record.IsCollected
		select record).ToList<MateriumItem>();
	}

	// Token: 0x04003E53 RID: 15955
	[Space]
	[SerializeField]
	private SpriteRenderer displayIcon;
}
