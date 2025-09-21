using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000720 RID: 1824
public class ShopItemStats : MonoBehaviour
{
	// Token: 0x1700076C RID: 1900
	// (get) Token: 0x060040E7 RID: 16615 RVA: 0x0011CFFF File Offset: 0x0011B1FF
	private bool DisplayMoneyCost
	{
		get
		{
			return this.shopItem.Cost > 0 || !this.shopItem.RequiredItem;
		}
	}

	// Token: 0x1700076D RID: 1901
	// (get) Token: 0x060040E8 RID: 16616 RVA: 0x0011D024 File Offset: 0x0011B224
	// (set) Token: 0x060040E9 RID: 16617 RVA: 0x0011D02C File Offset: 0x0011B22C
	public ShopItem Item
	{
		get
		{
			return this.shopItem;
		}
		set
		{
			this.SetItem(value);
		}
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x0011D035 File Offset: 0x0011B235
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x0011D03D File Offset: 0x0011B23D
	private void OnEnable()
	{
		this.fadeGroup.AlphaSelf = 0f;
		this.hidden = true;
	}

	// Token: 0x060040EC RID: 16620 RVA: 0x0011D058 File Offset: 0x0011B258
	private void Update()
	{
		float y = base.transform.position.y;
		if (y > this.topY || y < this.botY)
		{
			if (!this.hidden)
			{
				this.fadeGroup.AlphaSelf = 0f;
				this.hidden = true;
				return;
			}
		}
		else if (this.hidden)
		{
			this.fadeGroup.AlphaSelf = 1f;
			this.hidden = false;
		}
	}

	// Token: 0x060040ED RID: 16621 RVA: 0x0011D0C7 File Offset: 0x0011B2C7
	private void Init()
	{
		if (this.isInitialised)
		{
			return;
		}
		this.initialCostSpriteScale = this.costSprite.transform.localScale;
		this.isInitialised = true;
	}

	// Token: 0x060040EE RID: 16622 RVA: 0x0011D0EF File Offset: 0x0011B2EF
	public int GetCost()
	{
		return this.shopItem.Cost;
	}

	// Token: 0x060040EF RID: 16623 RVA: 0x0011D0FC File Offset: 0x0011B2FC
	public string GetName()
	{
		return this.shopItem.DisplayName;
	}

	// Token: 0x060040F0 RID: 16624 RVA: 0x0011D109 File Offset: 0x0011B309
	public string GetDesc()
	{
		return this.shopItem.Description;
	}

	// Token: 0x060040F1 RID: 16625 RVA: 0x0011D116 File Offset: 0x0011B316
	public int GetItemNumber()
	{
		return this.ItemNumber;
	}

	// Token: 0x060040F2 RID: 16626 RVA: 0x0011D120 File Offset: 0x0011B320
	public Sprite GetCurrencySprite()
	{
		CurrencyType currencyType = this.Item.CurrencyType;
		Sprite result;
		if (currencyType != CurrencyType.Money)
		{
			if (currencyType != CurrencyType.Shard)
			{
				throw new ArgumentOutOfRangeException();
			}
			result = this.shardSprite;
		}
		else
		{
			result = this.rosarySprite;
		}
		return result;
	}

	// Token: 0x060040F3 RID: 16627 RVA: 0x0011D15C File Offset: 0x0011B35C
	private bool HasRequiredItems()
	{
		return (this.Item.RequiredTools == ToolItemManager.OwnToolsCheckFlags.None || ToolItemManager.GetOwnedToolsCount(this.Item.RequiredTools) >= this.Item.RequiredToolsAmount) && (!this.Item.UpgradeFromItem || this.Item.UpgradeFromItem.CollectedAmount > 0) && (!this.Item.RequiredItem || this.Item.RequiredItem.CollectedAmount >= this.Item.RequiredItemAmount);
	}

	// Token: 0x060040F4 RID: 16628 RVA: 0x0011D1F0 File Offset: 0x0011B3F0
	public bool CanBuy()
	{
		if (!this.HasRequiredItems())
		{
			return false;
		}
		CurrencyType currencyType = this.Item.CurrencyType;
		bool result;
		if (currencyType != CurrencyType.Money)
		{
			if (currencyType != CurrencyType.Shard)
			{
				throw new ArgumentOutOfRangeException();
			}
			result = (PlayerData.instance.ShellShards >= this.shopItem.Cost);
		}
		else
		{
			result = (PlayerData.instance.geo >= this.shopItem.Cost);
		}
		return result;
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x0011D260 File Offset: 0x0011B460
	public void BuyFail()
	{
		PlayerData instance = PlayerData.instance;
		CurrencyType currencyType = this.Item.CurrencyType;
		if (currencyType != CurrencyType.Money)
		{
			if (currencyType != CurrencyType.Shard)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (instance.ShellShards < this.shopItem.Cost)
			{
				CurrencyCounter.ReportFail(CurrencyType.Shard);
				return;
			}
		}
		else if (instance.geo < this.shopItem.Cost)
		{
			CurrencyCounter.ReportFail(CurrencyType.Money);
			return;
		}
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x0011D2C2 File Offset: 0x0011B4C2
	public bool IsAtMax()
	{
		return this.Item.IsAtMax();
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x0011D2CF File Offset: 0x0011B4CF
	public bool IsAvailable()
	{
		return this.shopItem && this.shopItem.IsAvailable;
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x0011D2EC File Offset: 0x0011B4EC
	public void SetItem(ShopItem item)
	{
		this.Init();
		this.shopItem = item;
		if (!this.shopItem)
		{
			return;
		}
		if (this.itemSprite)
		{
			this.itemSprite.sprite = item.ItemSprite;
			this.itemSprite.transform.localScale = Vector3.one * item.ItemSpriteScale * this.itemSpriteSize;
		}
		if (this.itemCostText)
		{
			this.itemCostText.text = (this.DisplayMoneyCost ? item.Cost.ToString() : item.RequiredItemAmount.ToString());
		}
		bool flag = item.UpgradeFromItem || (item.Item && item.Item.HasUpgradeIcon());
		if (this.materialCostIcon)
		{
			this.materialCostIcon.gameObject.SetActive(item.RequiredItem && !flag);
		}
		if (this.upgradeIcon)
		{
			this.upgradeIcon.gameObject.SetActive(flag);
		}
		if (this.costSprite)
		{
			if (this.DisplayMoneyCost)
			{
				this.costSprite.sprite = this.GetCurrencySprite();
				this.costSprite.transform.localScale = this.initialCostSpriteScale;
				return;
			}
			this.costSprite.sprite = item.RequiredItem.GetIcon(CollectableItem.ReadSource.Tiny);
			this.costSprite.transform.localScale = Vector3.one * this.requiredItemCostScale;
		}
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x0011D48C File Offset: 0x0011B68C
	public void UpdateAppearance()
	{
		if (this.CanBuy())
		{
			if (this.costSprite)
			{
				this.costSprite.color = this.activeColour;
			}
			if (this.itemCostText)
			{
				this.itemCostText.color = this.activeColour;
			}
		}
		else
		{
			if (this.costSprite)
			{
				this.costSprite.color = this.inactiveColour;
			}
			if (this.itemCostText)
			{
				this.itemCostText.color = this.inactiveColour;
			}
		}
		if (this.materialCostIcon)
		{
			this.materialCostIcon.color = (this.HasRequiredItems() ? this.activeColour : this.inactiveColour);
		}
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x0011D549 File Offset: 0x0011B749
	public void SetPurchased(Action onComplete, int subItemIndex)
	{
		this.shopItem.SetPurchased(onComplete, subItemIndex);
	}

	// Token: 0x04004262 RID: 16994
	[SerializeField]
	private ShopItem shopItem;

	// Token: 0x04004263 RID: 16995
	[Space]
	[SerializeField]
	private Color activeColour;

	// Token: 0x04004264 RID: 16996
	[SerializeField]
	private Color inactiveColour;

	// Token: 0x04004265 RID: 16997
	[SerializeField]
	private SpriteRenderer costSprite;

	// Token: 0x04004266 RID: 16998
	[SerializeField]
	private Sprite rosarySprite;

	// Token: 0x04004267 RID: 16999
	[SerializeField]
	private Sprite shardSprite;

	// Token: 0x04004268 RID: 17000
	[SerializeField]
	private float requiredItemCostScale = 0.5f;

	// Token: 0x04004269 RID: 17001
	[SerializeField]
	private TextMeshPro itemCostText;

	// Token: 0x0400426A RID: 17002
	[SerializeField]
	private SpriteRenderer itemSprite;

	// Token: 0x0400426B RID: 17003
	[SerializeField]
	private float itemSpriteSize = 1f;

	// Token: 0x0400426C RID: 17004
	[SerializeField]
	private SpriteRenderer materialCostIcon;

	// Token: 0x0400426D RID: 17005
	[SerializeField]
	private SpriteRenderer upgradeIcon;

	// Token: 0x0400426E RID: 17006
	[Space]
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x0400426F RID: 17007
	[SerializeField]
	private float topY;

	// Token: 0x04004270 RID: 17008
	[SerializeField]
	private float botY;

	// Token: 0x04004271 RID: 17009
	[NonSerialized]
	public int ItemNumber;

	// Token: 0x04004272 RID: 17010
	private bool isInitialised;

	// Token: 0x04004273 RID: 17011
	private Vector3 initialCostSpriteScale;

	// Token: 0x04004274 RID: 17012
	private bool hidden = true;
}
