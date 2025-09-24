using System;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000721 RID: 1825
public class ShopMenuStock : MonoBehaviour
{
	// Token: 0x1700076E RID: 1902
	// (get) Token: 0x060040FC RID: 16636 RVA: 0x0011D57D File Offset: 0x0011B77D
	// (set) Token: 0x060040FD RID: 16637 RVA: 0x0011D59D File Offset: 0x0011B79D
	public string Title
	{
		get
		{
			if (!this.titleText)
			{
				return string.Empty;
			}
			return this.titleText.text;
		}
		set
		{
			if (this.titleText)
			{
				this.titleText.text = value;
			}
		}
	}

	// Token: 0x1700076F RID: 1903
	// (get) Token: 0x060040FE RID: 16638 RVA: 0x0011D5B8 File Offset: 0x0011B7B8
	// (set) Token: 0x060040FF RID: 16639 RVA: 0x0011D5C0 File Offset: 0x0011B7C0
	public bool WasItemPurchased { get; private set; }

	// Token: 0x06004100 RID: 16640 RVA: 0x0011D5C9 File Offset: 0x0011B7C9
	private void Start()
	{
		this.SpawnStock();
	}

	// Token: 0x06004101 RID: 16641 RVA: 0x0011D5D4 File Offset: 0x0011B7D4
	public void SpawnStock()
	{
		if (this.MasterList)
		{
			this.spawnedStock = this.MasterList.spawnedStock;
			this.spawnedSubItems = this.MasterList.spawnedSubItems;
			this.subItemsLayout = this.MasterList.subItemsLayout;
			return;
		}
		this.lastSpawnTime = (double)Time.time;
		int num = 0;
		int num2 = 0;
		foreach (ShopItem shopItem in this.stock)
		{
			if (shopItem.IsAvailable)
			{
				num++;
			}
			if (shopItem.HasSubItems)
			{
				int subItemsCount = shopItem.SubItemsCount;
				if (subItemsCount > num2)
				{
					num2 = subItemsCount;
				}
			}
		}
		this.templateItem.gameObject.SetActive(false);
		if (this.templateSubItem)
		{
			this.templateSubItem.gameObject.SetActive(false);
		}
		int j = num - this.spawnedStock.Count;
		if (j > 0)
		{
			Transform parent = this.templateItem.transform.parent;
			bool flag = false;
			if (parent != null)
			{
				flag = parent.gameObject.activeSelf;
				parent.gameObject.SetActive(true);
			}
			while (j > 0)
			{
				ShopItemStats shopItemStats = Object.Instantiate<ShopItemStats>(this.templateItem, parent);
				shopItemStats.gameObject.SetActive(true);
				shopItemStats.gameObject.SetActive(false);
				this.spawnedStock.Add(shopItemStats);
				j--;
			}
			if (!flag)
			{
				parent.gameObject.SetActive(false);
			}
		}
		int k = num2 - this.spawnedSubItems.Count;
		if (k > 0)
		{
			Transform parent2 = this.templateSubItem.transform.parent;
			bool flag2 = false;
			if (parent2 != null)
			{
				flag2 = parent2.gameObject.activeSelf;
				parent2.gameObject.SetActive(true);
			}
			while (k > 0)
			{
				ShopSubItemStats shopSubItemStats = Object.Instantiate<ShopSubItemStats>(this.templateSubItem, parent2);
				shopSubItemStats.gameObject.SetActive(true);
				shopSubItemStats.gameObject.SetActive(false);
				this.spawnedSubItems.Add(shopSubItemStats);
				k--;
			}
			if (!flag2)
			{
				parent2.gameObject.SetActive(false);
			}
		}
		int num3 = 0;
		bool flag3 = false;
		GameObject gameObject = base.gameObject;
		foreach (ShopItem shopItem2 in this.stock)
		{
			if (num3 >= this.spawnedStock.Count)
			{
				break;
			}
			if (shopItem2.IsAvailable)
			{
				this.spawnedStock[num3++].Item = shopItem2;
				if (shopItem2.EnsurePool(gameObject))
				{
					flag3 = true;
				}
			}
		}
		if (flag3)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(gameObject);
		}
		for (int l = num3; l < this.spawnedStock.Count; l++)
		{
			this.spawnedStock[l].Item = null;
		}
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x0011D894 File Offset: 0x0011BA94
	public void SetStock(ShopItem[] newStock)
	{
		if (this.stock == newStock && Time.timeAsDouble - this.lastSpawnTime < 0.5)
		{
			return;
		}
		this.stock = newStock;
		bool activeSelf = base.gameObject.activeSelf;
		if (!activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.SpawnStock();
		if (!activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x0011D8F7 File Offset: 0x0011BAF7
	public IEnumerable<ShopItem> EnumerateStock()
	{
		return from item in this.stock
		where item
		select item;
	}

	// Token: 0x06004104 RID: 16644 RVA: 0x0011D923 File Offset: 0x0011BB23
	public void UpdateStock()
	{
	}

	// Token: 0x06004105 RID: 16645 RVA: 0x0011D928 File Offset: 0x0011BB28
	public bool StockLeft()
	{
		foreach (ShopItem shopItem in this.stock)
		{
			if (shopItem && shopItem.IsAvailable)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x0011D964 File Offset: 0x0011BB64
	public bool StockLeftNotInfinite()
	{
		foreach (ShopItem shopItem in this.stock)
		{
			if (shopItem && shopItem.IsAvailableNotInfinite)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004107 RID: 16647 RVA: 0x0011D9A0 File Offset: 0x0011BBA0
	public void BuildItemList()
	{
		this.SpawnStock();
		this.availableStock.Clear();
		float num = 0f;
		foreach (ShopItemStats shopItemStats in this.spawnedStock)
		{
			if (shopItemStats.IsAvailable())
			{
				shopItemStats.transform.localPosition = new Vector3(0f, num, 0f);
				shopItemStats.ItemNumber = this.availableStock.Count;
				this.availableStock.Add(shopItemStats);
				num += this.yDistance;
				shopItemStats.gameObject.SetActive(true);
				shopItemStats.UpdateAppearance();
			}
		}
		foreach (ShopSubItemStats shopSubItemStats in this.spawnedSubItems)
		{
			shopSubItemStats.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004108 RID: 16648 RVA: 0x0011DAA4 File Offset: 0x0011BCA4
	public int GetItemCount()
	{
		return this.availableStock.Count;
	}

	// Token: 0x06004109 RID: 16649 RVA: 0x0011DAB1 File Offset: 0x0011BCB1
	public int GetCost(int itemNum)
	{
		return this.availableStock[itemNum].GetCost();
	}

	// Token: 0x0600410A RID: 16650 RVA: 0x0011DAC4 File Offset: 0x0011BCC4
	public string GetName(int itemNum)
	{
		return this.availableStock[itemNum].GetName();
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x0011DAD7 File Offset: 0x0011BCD7
	public string GetDesc(int itemNum)
	{
		return this.availableStock[itemNum].GetDesc();
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x0011DAEA File Offset: 0x0011BCEA
	public float GetYDistance()
	{
		return this.yDistance;
	}

	// Token: 0x0600410D RID: 16653 RVA: 0x0011DAF2 File Offset: 0x0011BCF2
	public Sprite GetItemSprite(int itemNum)
	{
		return this.availableStock[itemNum].Item.ItemSprite;
	}

	// Token: 0x0600410E RID: 16654 RVA: 0x0011DB0A File Offset: 0x0011BD0A
	public Sprite GetSubItemPurchaseIcon(int itemNum, int subItem)
	{
		return this.availableStock[itemNum].Item.GetSubItem(subItem).PurchaseIcon;
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x0011DB28 File Offset: 0x0011BD28
	public Vector3 GetItemSpriteScale(int itemNum)
	{
		return Vector3.one * this.availableStock[itemNum].Item.ItemSpriteScale;
	}

	// Token: 0x06004110 RID: 16656 RVA: 0x0011DB4A File Offset: 0x0011BD4A
	public Sprite GetItemCurrencySprite(int itemNum)
	{
		return this.availableStock[itemNum].GetCurrencySprite();
	}

	// Token: 0x06004111 RID: 16657 RVA: 0x0011DB5D File Offset: 0x0011BD5D
	public bool CanBuy(int itemNum)
	{
		return this.availableStock[itemNum].CanBuy();
	}

	// Token: 0x06004112 RID: 16658 RVA: 0x0011DB70 File Offset: 0x0011BD70
	public void BuyFail(int itemNum)
	{
		this.availableStock[itemNum].BuyFail();
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x0011DB83 File Offset: 0x0011BD83
	public bool IsAtMax(int itemNum)
	{
		return this.availableStock[itemNum].IsAtMax();
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x0011DB96 File Offset: 0x0011BD96
	public GameObject GetItemGameObject(int itemNum)
	{
		return this.availableStock[itemNum].gameObject;
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x0011DBA9 File Offset: 0x0011BDA9
	public bool IsToolItem(int itemNum)
	{
		return this.availableStock[itemNum].Item.IsToolItem();
	}

	// Token: 0x06004116 RID: 16662 RVA: 0x0011DBC1 File Offset: 0x0011BDC1
	public ToolItemType GetToolType(int itemNum)
	{
		return this.availableStock[itemNum].Item.GetToolType();
	}

	// Token: 0x06004117 RID: 16663 RVA: 0x0011DBD9 File Offset: 0x0011BDD9
	public ShopItem.PurchaseTypes GetPurchaseType(int itemNum)
	{
		return this.availableStock[itemNum].Item.GetPurchaseType();
	}

	// Token: 0x06004118 RID: 16664 RVA: 0x0011DBF1 File Offset: 0x0011BDF1
	public CollectableItem GetRequiredItem(int itemNum)
	{
		return this.availableStock[itemNum].Item.RequiredItem;
	}

	// Token: 0x06004119 RID: 16665 RVA: 0x0011DC09 File Offset: 0x0011BE09
	public int GetRequiredItemAmount(int itemNum)
	{
		return this.availableStock[itemNum].Item.RequiredItemAmount;
	}

	// Token: 0x0600411A RID: 16666 RVA: 0x0011DC21 File Offset: 0x0011BE21
	public void SetWasItemPurchased(bool value)
	{
		this.WasItemPurchased = value;
	}

	// Token: 0x0600411B RID: 16667 RVA: 0x0011DC2C File Offset: 0x0011BE2C
	public void DisplayCurrencyCounters()
	{
		bool flag = false;
		bool flag2 = false;
		foreach (ShopItem shopItem in this.stock)
		{
			if (shopItem && shopItem.IsAvailable)
			{
				CurrencyType currencyType = shopItem.CurrencyType;
				if (currencyType != CurrencyType.Money)
				{
					if (currencyType != CurrencyType.Shard)
					{
						throw new ArgumentOutOfRangeException();
					}
					flag2 = true;
				}
				else
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			CurrencyCounter.Show(CurrencyType.Money, true);
		}
		if (flag2)
		{
			CurrencyCounter.Show(CurrencyType.Shard, true);
		}
		foreach (ShopItem shopItem2 in this.stock)
		{
			if (shopItem2 && shopItem2.IsAvailable)
			{
				if (shopItem2.RequiredItem)
				{
					ItemCurrencyCounter.Show(shopItem2.RequiredItem);
				}
				if (shopItem2.UpgradeFromItem)
				{
					ItemCurrencyCounter.Show(shopItem2.UpgradeFromItem);
				}
			}
		}
	}

	// Token: 0x0600411C RID: 16668 RVA: 0x0011DD00 File Offset: 0x0011BF00
	public void HideCurrencyCounters()
	{
		HashSet<CurrencyType> hashSet = new HashSet<CurrencyType>(this.stock.Length);
		HashSet<CollectableItem> hashSet2 = new HashSet<CollectableItem>(this.stock.Length);
		foreach (ShopItem shopItem in this.stock)
		{
			if (shopItem)
			{
				hashSet.Add(shopItem.CurrencyType);
				if (shopItem.RequiredItem)
				{
					hashSet2.Add(shopItem.RequiredItem);
				}
				if (shopItem.UpgradeFromItem)
				{
					hashSet2.Add(shopItem.UpgradeFromItem);
				}
			}
		}
		foreach (CurrencyType type in hashSet)
		{
			CurrencyCounter.HideForced(type);
		}
		foreach (CollectableItem item in hashSet2)
		{
			ItemCurrencyCounter.HideForced(item);
		}
	}

	// Token: 0x0600411D RID: 16669 RVA: 0x0011DE0C File Offset: 0x0011C00C
	public string GetEventAfterPurchase(int itemNum)
	{
		return this.availableStock[itemNum].Item.EventAfterPurchase;
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x0011DE24 File Offset: 0x0011C024
	public bool GetHasSubItems(int itemNum)
	{
		return this.availableStock[itemNum].Item.HasSubItems;
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x0011DE3C File Offset: 0x0011C03C
	public void SetupSubItems(int itemNum)
	{
		ShopItem item = this.availableStock[itemNum].Item;
		int subItemsCount = item.SubItemsCount;
		for (int i = 0; i < subItemsCount; i++)
		{
			ShopItem.SubItem subItem = item.GetSubItem(i);
			ShopSubItemStats shopSubItemStats = this.spawnedSubItems[i];
			shopSubItemStats.Setup(subItem);
			shopSubItemStats.gameObject.SetActive(true);
		}
		if (this.subItemsLayout)
		{
			this.subItemsLayout.ForceUpdateLayoutNoCanvas();
		}
	}

	// Token: 0x06004120 RID: 16672 RVA: 0x0011DEAC File Offset: 0x0011C0AC
	public string GetSubItemSelectPrompt(int itemNum)
	{
		return this.availableStock[itemNum].Item.SubItemSelectPrompt;
	}

	// Token: 0x04004275 RID: 17013
	[SerializeField]
	private float yDistance = -1.5f;

	// Token: 0x04004276 RID: 17014
	[SerializeField]
	private TextMeshPro titleText;

	// Token: 0x04004277 RID: 17015
	public ShopMenuStock MasterList;

	// Token: 0x04004278 RID: 17016
	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopItem[] stock;

	// Token: 0x04004279 RID: 17017
	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopItemStats templateItem;

	// Token: 0x0400427A RID: 17018
	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private ShopSubItemStats templateSubItem;

	// Token: 0x0400427B RID: 17019
	[SerializeField]
	[Conditional("MasterList", false, false, false)]
	private LayoutGroup subItemsLayout;

	// Token: 0x0400427C RID: 17020
	private List<ShopItemStats> spawnedStock = new List<ShopItemStats>();

	// Token: 0x0400427D RID: 17021
	private List<ShopSubItemStats> spawnedSubItems = new List<ShopSubItemStats>();

	// Token: 0x0400427E RID: 17022
	private readonly List<ShopItemStats> availableStock = new List<ShopItemStats>();

	// Token: 0x0400427F RID: 17023
	private Vector3 selfPos;

	// Token: 0x04004281 RID: 17025
	private double lastSpawnTime;
}
