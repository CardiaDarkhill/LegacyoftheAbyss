using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000722 RID: 1826
public abstract class ShopOwnerBase : MonoBehaviour
{
	// Token: 0x17000770 RID: 1904
	// (get) Token: 0x06004122 RID: 16674
	public abstract ShopItem[] Stock { get; }

	// Token: 0x17000771 RID: 1905
	// (get) Token: 0x06004123 RID: 16675 RVA: 0x0011DEFD File Offset: 0x0011C0FD
	public GameObject ShopObject
	{
		get
		{
			this.SpawnUpdateShop();
			return ShopOwnerBase._spawnedShop.gameObject;
		}
	}

	// Token: 0x06004124 RID: 16676 RVA: 0x0011DF0F File Offset: 0x0011C10F
	private void OnEnable()
	{
		this.SpawnUpdateShop();
	}

	// Token: 0x06004125 RID: 16677 RVA: 0x0011DF17 File Offset: 0x0011C117
	private void OnDestroy()
	{
		if (ShopOwnerBase._spawnedShop != null)
		{
			Object.Destroy(ShopOwnerBase._spawnedShop.gameObject);
		}
		ShopOwnerBase._spawnedShop = null;
	}

	// Token: 0x06004126 RID: 16678 RVA: 0x0011DF3C File Offset: 0x0011C13C
	private void SpawnUpdateShop()
	{
		if (!ShopOwnerBase._spawnedShop && this.shopPrefab)
		{
			ShopOwnerBase._spawnedShop = Object.Instantiate<ShopMenuStock>(this.shopPrefab);
			this.ListenToShopDestroyed(ShopOwnerBase._spawnedShop);
		}
		if (ShopOwnerBase._spawnedShop)
		{
			ShopOwnerBase._spawnedShop.SetStock(this.Stock);
			ShopOwnerBase._spawnedShop.Title = this.shopTitle;
		}
	}

	// Token: 0x06004127 RID: 16679 RVA: 0x0011DFAE File Offset: 0x0011C1AE
	private void ListenToShopDestroyed(ShopMenuStock shop)
	{
		shop.gameObject.AddComponent<OnDestroyEventAnnouncer>().OnDestroyEvent += this.OnSpawnedShopDestroyed;
	}

	// Token: 0x06004128 RID: 16680 RVA: 0x0011DFCC File Offset: 0x0011C1CC
	private void OnSpawnedShopDestroyed(OnDestroyEventAnnouncer announcer)
	{
		announcer.OnDestroyEvent -= this.OnSpawnedShopDestroyed;
		ShopOwnerBase._spawnedShop = null;
	}

	// Token: 0x04004282 RID: 17026
	[SerializeField]
	private ShopMenuStock shopPrefab;

	// Token: 0x04004283 RID: 17027
	[SerializeField]
	private LocalisedString shopTitle;

	// Token: 0x04004284 RID: 17028
	private static ShopMenuStock _spawnedShop;
}
