using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public class ItemCurrencyCounter : CurrencyCounterTyped<CollectableItem>
{
	// Token: 0x1700073C RID: 1852
	// (get) Token: 0x06003EB1 RID: 16049 RVA: 0x00113FD2 File Offset: 0x001121D2
	protected override int Count
	{
		get
		{
			if (!this.item)
			{
				return 0;
			}
			return this.item.CollectedAmount;
		}
	}

	// Token: 0x1700073D RID: 1853
	// (get) Token: 0x06003EB2 RID: 16050 RVA: 0x00113FEE File Offset: 0x001121EE
	protected override CollectableItem CounterType
	{
		get
		{
			return this.item;
		}
	}

	// Token: 0x06003EB3 RID: 16051 RVA: 0x00113FF8 File Offset: 0x001121F8
	protected override void Awake()
	{
		base.Awake();
		if (ItemCurrencyCounter._templateItem == null)
		{
			ItemCurrencyCounter._templateItem = this;
			return;
		}
		ItemCurrencyCounter.ItemCounters.Add(this);
		CurrencyCounterStack currencyCounterStack = base.transform.parent ? base.transform.parent.GetComponent<CurrencyCounterStack>() : null;
		if (currencyCounterStack)
		{
			currencyCounterStack.AddNewCounter(this);
		}
	}

	// Token: 0x06003EB4 RID: 16052 RVA: 0x0011405F File Offset: 0x0011225F
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (ItemCurrencyCounter._templateItem == this)
		{
			ItemCurrencyCounter._templateItem = null;
			return;
		}
		ItemCurrencyCounter.ItemCounters.Remove(this);
	}

	// Token: 0x06003EB5 RID: 16053 RVA: 0x00114087 File Offset: 0x00112287
	public void SetItem(CollectableItem newItem)
	{
		this.item = newItem;
		if (this.iconSprite)
		{
			this.iconSprite.sprite = newItem.GetIcon(CollectableItem.ReadSource.Tiny);
		}
		base.UpdateCounterStart();
	}

	// Token: 0x06003EB6 RID: 16054 RVA: 0x001140B8 File Offset: 0x001122B8
	private static ItemCurrencyCounter GetCurrencyCounter(CollectableItem item, bool getNew)
	{
		if (item.HideInShopCounters)
		{
			return null;
		}
		ItemCurrencyCounter itemCurrencyCounter = ItemCurrencyCounter.ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => c.item == item && c.IsActive);
		if (itemCurrencyCounter != null || !getNew)
		{
			return itemCurrencyCounter;
		}
		itemCurrencyCounter = ItemCurrencyCounter.ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => !c.IsActive);
		if (itemCurrencyCounter != null)
		{
			itemCurrencyCounter.SetItem(item);
			return itemCurrencyCounter;
		}
		if (ItemCurrencyCounter._templateItem == null)
		{
			return null;
		}
		itemCurrencyCounter = Object.Instantiate<ItemCurrencyCounter>(ItemCurrencyCounter._templateItem, ItemCurrencyCounter._templateItem.transform.parent);
		itemCurrencyCounter.SetItem(item);
		return itemCurrencyCounter;
	}

	// Token: 0x06003EB7 RID: 16055 RVA: 0x00114180 File Offset: 0x00112380
	public static void Show(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = ItemCurrencyCounter.GetCurrencyCounter(item, true);
		if (currencyCounter == null)
		{
			return;
		}
		currencyCounter.InternalShow();
	}

	// Token: 0x06003EB8 RID: 16056 RVA: 0x001141A8 File Offset: 0x001123A8
	public static void Hide(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = ItemCurrencyCounter.GetCurrencyCounter(item, false);
		if (currencyCounter == null)
		{
			return;
		}
		currencyCounter.InternalHide(false);
	}

	// Token: 0x06003EB9 RID: 16057 RVA: 0x001141D0 File Offset: 0x001123D0
	public static void HideForced(CollectableItem item)
	{
		ItemCurrencyCounter currencyCounter = ItemCurrencyCounter.GetCurrencyCounter(item, false);
		if (currencyCounter == null)
		{
			return;
		}
		currencyCounter.InternalHide(true);
	}

	// Token: 0x06003EBA RID: 16058 RVA: 0x001141F8 File Offset: 0x001123F8
	public static void Take(CollectableItem item, int amount)
	{
		ItemCurrencyCounter currencyCounter = ItemCurrencyCounter.GetCurrencyCounter(item, true);
		if (currencyCounter == null)
		{
			return;
		}
		currencyCounter.InternalTake(amount);
	}

	// Token: 0x06003EBB RID: 16059 RVA: 0x00114220 File Offset: 0x00112420
	public static void UpdateValue(CollectableItem item)
	{
		ItemCurrencyCounter itemCurrencyCounter = ItemCurrencyCounter.ItemCounters.FirstOrDefault((ItemCurrencyCounter c) => c.item == item && c.IsActive);
		if (itemCurrencyCounter == null)
		{
			return;
		}
		itemCurrencyCounter.UpdateValue();
	}

	// Token: 0x04004057 RID: 16471
	[SerializeField]
	private SpriteRenderer iconSprite;

	// Token: 0x04004058 RID: 16472
	private CollectableItem item;

	// Token: 0x04004059 RID: 16473
	private static readonly List<ItemCurrencyCounter> ItemCounters = new List<ItemCurrencyCounter>();

	// Token: 0x0400405A RID: 16474
	private static ItemCurrencyCounter _templateItem;
}
