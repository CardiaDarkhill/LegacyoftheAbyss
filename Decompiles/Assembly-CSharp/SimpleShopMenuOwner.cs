using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public abstract class SimpleShopMenuOwner : MonoBehaviour
{
	// Token: 0x140000DE RID: 222
	// (add) Token: 0x06004198 RID: 16792 RVA: 0x00120E0C File Offset: 0x0011F00C
	// (remove) Token: 0x06004199 RID: 16793 RVA: 0x00120E44 File Offset: 0x0011F044
	public event Action ClosedNoPurchase;

	// Token: 0x140000DF RID: 223
	// (add) Token: 0x0600419A RID: 16794 RVA: 0x00120E7C File Offset: 0x0011F07C
	// (remove) Token: 0x0600419B RID: 16795 RVA: 0x00120EB4 File Offset: 0x0011F0B4
	public event Action ClosedPurchase;

	// Token: 0x1700077A RID: 1914
	// (get) Token: 0x0600419C RID: 16796 RVA: 0x00120EE9 File Offset: 0x0011F0E9
	public string ShopTitle
	{
		get
		{
			return this.shopTitle;
		}
	}

	// Token: 0x1700077B RID: 1915
	// (get) Token: 0x0600419D RID: 16797 RVA: 0x00120EF6 File Offset: 0x0011F0F6
	public string PurchaseText
	{
		get
		{
			return this.purchaseText;
		}
	}

	// Token: 0x1700077C RID: 1916
	// (get) Token: 0x0600419E RID: 16798 RVA: 0x00120F03 File Offset: 0x0011F103
	public virtual bool ClosePaneOnPurchase
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600419F RID: 16799 RVA: 0x00120F06 File Offset: 0x0011F106
	protected virtual void Start()
	{
		if (!SimpleShopMenuOwner._spawnedMenu && this.shopMenuPrefab)
		{
			SimpleShopMenuOwner._spawnedMenu = Object.Instantiate<SimpleShopMenu>(this.shopMenuPrefab);
		}
	}

	// Token: 0x060041A0 RID: 16800 RVA: 0x00120F34 File Offset: 0x0011F134
	public bool OpenMenu()
	{
		List<ISimpleShopItem> items = this.GetItems();
		if (items == null || items.Count == 0)
		{
			return false;
		}
		if (SimpleShopMenuOwner._spawnedMenu)
		{
			SimpleShopMenuOwner._spawnedMenu.SetStock(this, items);
			SimpleShopMenuOwner._spawnedMenu.Activate();
			return true;
		}
		return false;
	}

	// Token: 0x060041A1 RID: 16801 RVA: 0x00120F7A File Offset: 0x0011F17A
	public void RefreshStock()
	{
		SimpleShopMenuOwner._spawnedMenu.SetStock(this, this.GetItems());
	}

	// Token: 0x060041A2 RID: 16802 RVA: 0x00120F90 File Offset: 0x0011F190
	public bool HasStockLeft()
	{
		List<ISimpleShopItem> items = this.GetItems();
		return items != null && items.Count > 0;
	}

	// Token: 0x060041A3 RID: 16803 RVA: 0x00120FB2 File Offset: 0x0011F1B2
	public void ClosedMenu(bool didPurchase, int purchaseIndex)
	{
		if (didPurchase)
		{
			if (purchaseIndex >= 0)
			{
				this.OnPurchasedItem(purchaseIndex);
			}
			Action closedPurchase = this.ClosedPurchase;
			if (closedPurchase == null)
			{
				return;
			}
			closedPurchase();
			return;
		}
		else
		{
			Action closedNoPurchase = this.ClosedNoPurchase;
			if (closedNoPurchase == null)
			{
				return;
			}
			closedNoPurchase();
			return;
		}
	}

	// Token: 0x060041A4 RID: 16804 RVA: 0x00120FE3 File Offset: 0x0011F1E3
	public void PurchaseNoClose(int purchaseIndex)
	{
		if (purchaseIndex >= 0)
		{
			this.OnPurchasedItem(purchaseIndex);
		}
	}

	// Token: 0x060041A5 RID: 16805
	protected abstract List<ISimpleShopItem> GetItems();

	// Token: 0x060041A6 RID: 16806
	protected abstract void OnPurchasedItem(int itemIndex);

	// Token: 0x0400432A RID: 17194
	[SerializeField]
	private SimpleShopMenu shopMenuPrefab;

	// Token: 0x0400432B RID: 17195
	[SerializeField]
	private LocalisedString shopTitle;

	// Token: 0x0400432C RID: 17196
	[SerializeField]
	private LocalisedString purchaseText = new LocalisedString("UI", "CTRL_BUY");

	// Token: 0x0400432D RID: 17197
	private static SimpleShopMenu _spawnedMenu;
}
