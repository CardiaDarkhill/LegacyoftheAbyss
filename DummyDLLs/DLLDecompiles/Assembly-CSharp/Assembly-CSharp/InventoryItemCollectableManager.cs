using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class InventoryItemCollectableManager : InventoryItemListManager<InventoryItemCollectable, CollectableItem>
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060011A7 RID: 4519 RVA: 0x0005284E File Offset: 0x00050A4E
	// (set) Token: 0x060011A8 RID: 4520 RVA: 0x00052856 File Offset: 0x00050A56
	public bool ShowingMemoryUseMsg { get; private set; }

	// Token: 0x060011A9 RID: 4521 RVA: 0x00052860 File Offset: 0x00050A60
	protected override void Awake()
	{
		base.Awake();
		this.pane = base.GetComponent<InventoryPane>();
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		if (this.pane)
		{
			this.pane.OnPaneEnd += this.HideMemoryUseMsgInstant;
		}
		this.HideMemoryUseMsgInstant();
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x000528B8 File Offset: 0x00050AB8
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemCollectable> selectableItems, List<CollectableItem> items)
	{
		for (int i = 0; i < selectableItems.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(true);
			selectableItems[i].Item = items[i];
		}
		if (this.relicHeader)
		{
			this.relicHeader.gameObject.SetActive(false);
		}
		if (this.consumableHeader)
		{
			this.consumableHeader.gameObject.SetActive(false);
		}
		return (from item in selectableItems
		group item by new
		{
			IsConsumable = item.Item.IsConsumable()
		} into @group
		orderby @group.Key.IsConsumable
		select new InventoryItemGrid.GridSection
		{
			Items = @group.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>(),
			Header = (@group.Key.IsConsumable ? this.consumableHeader : null),
			HideHeaderIfNoneBefore = true
		}).ToList<InventoryItemGrid.GridSection>();
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x00052996 File Offset: 0x00050B96
	protected override List<CollectableItem> GetItems()
	{
		return CollectableItemManager.GetCollectedItems();
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x000529A0 File Offset: 0x00050BA0
	protected override InventoryItemSelectable GetStartSelectable()
	{
		InventoryItemSelectable result;
		if (this.selectableHelper != null && this.selectableHelper.TryGetSelectable(out result))
		{
			CollectableItemManager.CollectedItem = null;
			return result;
		}
		InventoryItemCollectable inventoryItemCollectable = base.GetSelectables(null).FirstOrDefault((InventoryItemCollectable item) => CollectableItemManager.CollectedItem == item.Item);
		CollectableItemManager.CollectedItem = null;
		if (inventoryItemCollectable)
		{
			return inventoryItemCollectable;
		}
		return base.GetStartSelectable();
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x00052A14 File Offset: 0x00050C14
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		this.HideMemoryUseMsg(true);
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x00052A24 File Offset: 0x00050C24
	public void ShowMemoryUseMsg()
	{
		if (!this.memoryUseMsg || this.ShowingMemoryUseMsg)
		{
			return;
		}
		this.memoryUseMsg.AlphaSelf = 0f;
		this.memoryUseMsg.gameObject.SetActive(true);
		this.memoryUseMsg.FadeTo(1f, this.msgFadeInTime, null, true, null);
		this.ShowingMemoryUseMsg = true;
		this.paneList.InSubMenu = true;
		this.hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)this.msgFadeInTime;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x00052AA8 File Offset: 0x00050CA8
	public void HideMemoryUseMsg(bool force = false)
	{
		if (!this.memoryUseMsg || !this.ShowingMemoryUseMsg)
		{
			return;
		}
		if (!force && Time.unscaledTimeAsDouble < this.hideEquipMessageAllowedTime)
		{
			return;
		}
		this.memoryUseMsg.FadeTo(0f, this.msgFadeOutTime, null, true, null);
		this.ShowingMemoryUseMsg = false;
		this.paneList.InSubMenu = false;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x00052B0C File Offset: 0x00050D0C
	public void HideMemoryUseMsgInstant()
	{
		if (!this.memoryUseMsg || !this.ShowingMemoryUseMsg)
		{
			return;
		}
		this.memoryUseMsg.AlphaSelf = 0f;
		this.memoryUseMsg.gameObject.SetActive(false);
		this.ShowingMemoryUseMsg = false;
		this.paneList.InSubMenu = false;
	}

	// Token: 0x04001096 RID: 4246
	[SerializeField]
	private Transform consumableHeader;

	// Token: 0x04001097 RID: 4247
	[SerializeField]
	private Transform relicHeader;

	// Token: 0x04001098 RID: 4248
	[SerializeField]
	private NestedFadeGroupBase memoryUseMsg;

	// Token: 0x04001099 RID: 4249
	[SerializeField]
	private float msgFadeInTime;

	// Token: 0x0400109A RID: 4250
	[SerializeField]
	private float msgFadeOutTime;

	// Token: 0x0400109B RID: 4251
	[SerializeField]
	private InventoryCollectableItemSelectionHelper selectableHelper;

	// Token: 0x0400109C RID: 4252
	private double hideEquipMessageAllowedTime;

	// Token: 0x0400109D RID: 4253
	private InventoryPane pane;

	// Token: 0x0400109E RID: 4254
	private InventoryPaneList paneList;
}
