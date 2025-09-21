using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000692 RID: 1682
public abstract class InventoryItemListManager<TSelectable, TItem> : InventoryItemManager where TSelectable : InventoryItemSelectable
{
	// Token: 0x170006CC RID: 1740
	// (get) Token: 0x06003C09 RID: 15369 RVA: 0x00108533 File Offset: 0x00106733
	protected InventoryItemGrid ItemList
	{
		get
		{
			return this.itemList;
		}
	}

	// Token: 0x06003C0A RID: 15370 RVA: 0x0010853C File Offset: 0x0010673C
	protected override void Awake()
	{
		base.Awake();
		InventoryPaneBase component = base.GetComponent<InventoryPaneBase>();
		if (component)
		{
			component.OnPaneStart += this.UpdateList;
		}
		this.UpdateList();
	}

	// Token: 0x06003C0B RID: 15371 RVA: 0x00108578 File Offset: 0x00106778
	[ContextMenu("Preview List")]
	public void UpdateList()
	{
		if (!this.itemList)
		{
			return;
		}
		List<TItem> items = this.GetItems();
		this.SetupGrid(this.itemList, this.templateItem, items, new InventoryItemListManager<TSelectable, TItem>.GetGridSectionsDelegate(this.GetGridSections));
		this.OnItemListSetup();
	}

	// Token: 0x06003C0C RID: 15372 RVA: 0x001085C0 File Offset: 0x001067C0
	public override void InstantScroll()
	{
		if (base.CurrentSelected)
		{
			this.itemList.ScrollTo(base.CurrentSelected, true);
		}
	}

	// Token: 0x06003C0D RID: 15373 RVA: 0x001085E4 File Offset: 0x001067E4
	protected void SetupGrid(InventoryItemGrid grid, TSelectable currentTemplate, List<TItem> items, InventoryItemListManager<TSelectable, TItem>.GetGridSectionsDelegate getGridSections)
	{
		List<TSelectable> list = new List<TSelectable>(grid.transform.childCount);
		foreach (object obj in grid.transform)
		{
			TSelectable component = ((Transform)obj).GetComponent<TSelectable>();
			if (component)
			{
				list.Add(component);
			}
		}
		list.Remove(currentTemplate);
		currentTemplate.gameObject.SetActive(false);
		if (!Application.isPlaying || !this.isSetup)
		{
			this.isSetup = true;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				Object.DestroyImmediate(list[i].gameObject);
			}
			list.Clear();
		}
		int j;
		for (j = items.Count - list.Count; j > 0; j--)
		{
			TSelectable item = Object.Instantiate<TSelectable>(currentTemplate, grid.transform);
			list.Add(item);
			this.OnItemInstantiated(item);
		}
		while (j < 0)
		{
			TSelectable tselectable = list.Last<TSelectable>();
			tselectable.gameObject.SetActive(false);
			list.Remove(tselectable);
			j++;
		}
		grid.Setup(getGridSections(list, items));
	}

	// Token: 0x06003C0E RID: 15374 RVA: 0x00108738 File Offset: 0x00106938
	protected List<TSelectable> GetSelectables(Func<TSelectable, bool> predicate)
	{
		if (!this.itemList)
		{
			return null;
		}
		return this.itemList.GetListItems<TSelectable>(predicate);
	}

	// Token: 0x06003C0F RID: 15375
	protected abstract List<TItem> GetItems();

	// Token: 0x06003C10 RID: 15376
	protected abstract List<InventoryItemGrid.GridSection> GetGridSections(List<TSelectable> selectableItems, List<TItem> items);

	// Token: 0x06003C11 RID: 15377 RVA: 0x00108755 File Offset: 0x00106955
	protected virtual void OnItemInstantiated(TSelectable item)
	{
	}

	// Token: 0x06003C12 RID: 15378 RVA: 0x00108757 File Offset: 0x00106957
	protected virtual void OnItemListSetup()
	{
	}

	// Token: 0x06003C13 RID: 15379 RVA: 0x0010875C File Offset: 0x0010695C
	public override bool SubmitButtonSelected()
	{
		if (base.IsActionsBlocked)
		{
			return false;
		}
		InventoryItemSelectable currentSelected = base.CurrentSelected;
		InventoryItemListManager<TSelectable, TItem>.IMoveNextButton moveNextButton = currentSelected as InventoryItemListManager<TSelectable, TItem>.IMoveNextButton;
		if (moveNextButton == null || !moveNextButton.WillSubmitMoveNext)
		{
			return base.SubmitButtonSelected();
		}
		InventoryItemSelectable nextSelectable = currentSelected.GetNextSelectable(InventoryItemManager.SelectionDirection.Right);
		if (nextSelectable is InventoryItemListManager<TSelectable, TItem>.IMoveNextButton && nextSelectable.GetNextSelectable(InventoryItemManager.SelectionDirection.Down) != currentSelected)
		{
			base.SetSelected(nextSelectable, null, false);
			base.PlayMoveSound();
			return true;
		}
		InventoryItemSelectable inventoryItemSelectable = currentSelected;
		for (int i = 0; i < 10; i++)
		{
			moveNextButton = (inventoryItemSelectable.GetNextSelectable(InventoryItemManager.SelectionDirection.Left) as InventoryItemListManager<TSelectable, TItem>.IMoveNextButton);
			if (moveNextButton == null)
			{
				break;
			}
			InventoryItemSelectable inventoryItemSelectable2 = moveNextButton as InventoryItemSelectable;
			if (inventoryItemSelectable2 == null)
			{
				break;
			}
			inventoryItemSelectable = inventoryItemSelectable2;
		}
		moveNextButton = (inventoryItemSelectable.GetNextSelectable(InventoryItemManager.SelectionDirection.Down) as InventoryItemListManager<TSelectable, TItem>.IMoveNextButton);
		if (moveNextButton != null)
		{
			InventoryItemSelectable inventoryItemSelectable3 = moveNextButton as InventoryItemSelectable;
			if (inventoryItemSelectable3 != null)
			{
				base.SetSelected(inventoryItemSelectable3, null, false);
				base.PlayMoveSound();
				return true;
			}
		}
		TSelectable tselectable = this.GetSelectables(null).FirstOrDefault<TSelectable>();
		if (tselectable && tselectable != currentSelected)
		{
			base.SetSelected(tselectable, null, false);
			base.PlayMoveSound();
			return true;
		}
		return base.SubmitButtonSelected();
	}

	// Token: 0x04003E29 RID: 15913
	[SerializeField]
	[FormerlySerializedAs("questList")]
	private InventoryItemGrid itemList;

	// Token: 0x04003E2A RID: 15914
	[SerializeField]
	private TSelectable templateItem;

	// Token: 0x04003E2B RID: 15915
	private bool isSetup;

	// Token: 0x02001997 RID: 6551
	public interface IMoveNextButton
	{
		// Token: 0x170010AA RID: 4266
		// (get) Token: 0x06009496 RID: 38038
		bool WillSubmitMoveNext { get; }
	}

	// Token: 0x02001998 RID: 6552
	// (Invoke) Token: 0x06009498 RID: 38040
	protected delegate List<InventoryItemGrid.GridSection> GetGridSectionsDelegate(List<TSelectable> selectableItems, List<TItem> items);
}
