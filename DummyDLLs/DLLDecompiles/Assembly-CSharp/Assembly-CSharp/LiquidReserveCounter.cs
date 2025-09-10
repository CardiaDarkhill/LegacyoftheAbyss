using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020006C9 RID: 1737
public class LiquidReserveCounter : CurrencyCounterTyped<ToolItemStatesLiquid>
{
	// Token: 0x1700073E RID: 1854
	// (get) Token: 0x06003EC5 RID: 16069 RVA: 0x001143BB File Offset: 0x001125BB
	protected override int Count
	{
		get
		{
			if (!this.item)
			{
				return 0;
			}
			return this.item.LiquidSavedData.RefillsLeft;
		}
	}

	// Token: 0x1700073F RID: 1855
	// (get) Token: 0x06003EC6 RID: 16070 RVA: 0x001143DC File Offset: 0x001125DC
	protected override ToolItemStatesLiquid CounterType
	{
		get
		{
			return this.item;
		}
	}

	// Token: 0x06003EC7 RID: 16071 RVA: 0x001143E4 File Offset: 0x001125E4
	protected override void Awake()
	{
		base.Awake();
		if (LiquidReserveCounter._templateItem == null)
		{
			LiquidReserveCounter._templateItem = this;
			return;
		}
		LiquidReserveCounter._itemCounters.Add(this);
		CurrencyCounterStack currencyCounterStack = base.transform.parent ? base.transform.parent.GetComponent<CurrencyCounterStack>() : null;
		if (currencyCounterStack)
		{
			currencyCounterStack.AddNewCounter(this);
		}
	}

	// Token: 0x06003EC8 RID: 16072 RVA: 0x0011444B File Offset: 0x0011264B
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (LiquidReserveCounter._templateItem == this)
		{
			LiquidReserveCounter._templateItem = null;
			return;
		}
		LiquidReserveCounter._itemCounters.Remove(this);
	}

	// Token: 0x06003EC9 RID: 16073 RVA: 0x00114473 File Offset: 0x00112673
	public void SetItem(ToolItemStatesLiquid newItem)
	{
		this.item = newItem;
		this.fill.Color = this.item.LiquidColor;
		base.UpdateCounterStart();
	}

	// Token: 0x06003ECA RID: 16074 RVA: 0x00114498 File Offset: 0x00112698
	protected override void RefreshText(bool isCountingUp)
	{
		if (!this.item)
		{
			return;
		}
		int refillsMax = this.item.RefillsMax;
		float t = (float)this.item.LiquidSavedData.RefillsLeft / (float)refillsMax;
		float lerpedValue = this.fillPosRange.GetLerpedValue(t);
		this.fill.transform.SetLocalPositionY(lerpedValue);
	}

	// Token: 0x06003ECB RID: 16075 RVA: 0x001144F4 File Offset: 0x001126F4
	private static LiquidReserveCounter GetCurrencyCounter(ToolItemStatesLiquid item, bool getNew)
	{
		LiquidReserveCounter liquidReserveCounter = LiquidReserveCounter._itemCounters.FirstOrDefault((LiquidReserveCounter c) => c.item == item && c.IsActiveOrQueued);
		if (liquidReserveCounter != null || !getNew)
		{
			return liquidReserveCounter;
		}
		liquidReserveCounter = LiquidReserveCounter._itemCounters.FirstOrDefault((LiquidReserveCounter c) => !c.IsActiveOrQueued);
		if (liquidReserveCounter != null)
		{
			liquidReserveCounter.SetItem(item);
			return liquidReserveCounter;
		}
		if (LiquidReserveCounter._templateItem == null)
		{
			return null;
		}
		liquidReserveCounter = Object.Instantiate<LiquidReserveCounter>(LiquidReserveCounter._templateItem, LiquidReserveCounter._templateItem.transform.parent);
		liquidReserveCounter.SetItem(item);
		return liquidReserveCounter;
	}

	// Token: 0x06003ECC RID: 16076 RVA: 0x001145AC File Offset: 0x001127AC
	public static void Take(ToolItemStatesLiquid item, int amount)
	{
		LiquidReserveCounter currencyCounter = LiquidReserveCounter.GetCurrencyCounter(item, true);
		if (currencyCounter == null)
		{
			return;
		}
		currencyCounter.IconOverride = null;
		if (currencyCounter.infiniteIcon)
		{
			currencyCounter.infiniteIcon.gameObject.SetActive(false);
		}
		currencyCounter.QueueTake(amount);
	}

	// Token: 0x06003ECD RID: 16077 RVA: 0x001145F8 File Offset: 0x001127F8
	public static void InfiniteRefillPopup(ToolItemStatesLiquid item)
	{
		LiquidReserveCounter currencyCounter = LiquidReserveCounter.GetCurrencyCounter(item, true);
		if (currencyCounter == null)
		{
			return;
		}
		if (currencyCounter.infiniteIcon)
		{
			currencyCounter.infiniteIcon.gameObject.SetActive(true);
			currencyCounter.IconOverride = currencyCounter.infiniteIcon;
		}
		currencyCounter.QueuePopup();
	}

	// Token: 0x04004064 RID: 16484
	[Space]
	[SerializeField]
	private NestedFadeGroupSpriteRenderer fill;

	// Token: 0x04004065 RID: 16485
	[SerializeField]
	private MinMaxFloat fillPosRange;

	// Token: 0x04004066 RID: 16486
	[Space]
	[SerializeField]
	private CurrencyCounterIcon infiniteIcon;

	// Token: 0x04004067 RID: 16487
	private ToolItemStatesLiquid item;

	// Token: 0x04004068 RID: 16488
	private static readonly List<LiquidReserveCounter> _itemCounters = new List<LiquidReserveCounter>();

	// Token: 0x04004069 RID: 16489
	private static LiquidReserveCounter _templateItem;
}
