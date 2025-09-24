using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class CollectableItemManager : ManagerSingleton<CollectableItemManager>
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x060010BA RID: 4282 RVA: 0x0004F48A File Offset: 0x0004D68A
	// (set) Token: 0x060010BB RID: 4283 RVA: 0x0004F496 File Offset: 0x0004D696
	public static CollectableItem CollectedItem
	{
		get
		{
			return ManagerSingleton<CollectableItemManager>.Instance.collectedItem;
		}
		set
		{
			if (value != null)
			{
				InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.None;
			}
			ManagerSingleton<CollectableItemManager>.Instance.collectedItem = value;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060010BC RID: 4284 RVA: 0x0004F4B2 File Offset: 0x0004D6B2
	// (set) Token: 0x060010BD RID: 4285 RVA: 0x0004F4B9 File Offset: 0x0004D6B9
	public static int Version { get; private set; }

	// Token: 0x060010BE RID: 4286 RVA: 0x0004F4C1 File Offset: 0x0004D6C1
	public static void IncrementVersion()
	{
		CollectableItemManager.Version++;
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x0004F4CF File Offset: 0x0004D6CF
	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<CollectableItemManager>.UnsafeInstance == this)
		{
			CollectableItemManager.Version++;
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0004F4F0 File Offset: 0x0004D6F0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (ManagerSingleton<CollectableItemManager>.UnsafeInstance == this)
		{
			CollectableItemManager.Version++;
		}
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0004F511 File Offset: 0x0004D711
	public static void AddItem(CollectableItem item, int amount = 1)
	{
		if (!ManagerSingleton<CollectableItemManager>.Instance)
		{
			return;
		}
		ManagerSingleton<CollectableItemManager>.Instance.InternalAddItem(item, amount);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0004F52C File Offset: 0x0004D72C
	public static void RemoveItem(CollectableItem item, int amount = 1)
	{
		if (!ManagerSingleton<CollectableItemManager>.Instance)
		{
			return;
		}
		ManagerSingleton<CollectableItemManager>.Instance.InternalRemoveItem(item, amount);
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0004F547 File Offset: 0x0004D747
	public static void ClearStatic()
	{
		CollectableItemManager._collectedItemCache.UpdateCache(null, -1);
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x0004F558 File Offset: 0x0004D758
	public static List<CollectableItem> GetCollectedItems()
	{
		bool flag = CollectableItemManager.IsInHiddenMode();
		if (!CollectableItemManager._collectedItemCache.ShouldUpdate(CollectableItemManager.Version))
		{
			return CollectableItemManager._collectedItemCache.Value;
		}
		if (!ManagerSingleton<CollectableItemManager>.Instance)
		{
			CollectableItemManager._collectedItemCache.UpdateCache(new List<CollectableItem>(), CollectableItemManager.Version);
		}
		else if (flag)
		{
			CollectableItemManager._collectedItemCache.UpdateCache(ManagerSingleton<CollectableItemManager>.Instance.InternalGetCollectedItems((CollectableItem item) => item.IsVisibleWithBareInventory || item.SaveData.AmountWhileHidden > 0), CollectableItemManager.Version);
		}
		else
		{
			CollectableItemManager._collectedItemCache.UpdateCache(ManagerSingleton<CollectableItemManager>.Instance.InternalGetCollectedItems(null), CollectableItemManager.Version);
		}
		return CollectableItemManager._collectedItemCache.Value;
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x0004F60C File Offset: 0x0004D80C
	public static bool IsInHiddenMode()
	{
		HeroController instance = HeroController.instance;
		bool flag = instance && instance.Config && instance.Config.ForceBareInventory;
		if (CollectableItemManager.previousHiddenMode != flag)
		{
			CollectableItemManager.previousHiddenMode = flag;
			CollectableItemManager.Version++;
		}
		return flag;
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x0004F660 File Offset: 0x0004D860
	public static void ApplyHiddenItems()
	{
		CollectableItemsData collectables = GameManager.instance.playerData.Collectables;
		foreach (string itemName in collectables.GetValidNames(null))
		{
			CollectableItemsData.Data data = collectables.GetData(itemName);
			if (data.AmountWhileHidden > 0)
			{
				data.Amount += data.AmountWhileHidden;
				data.AmountWhileHidden = 0;
				collectables.SetData(itemName, data);
			}
		}
		CollectableItemManager.IncrementVersion();
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x0004F6F4 File Offset: 0x0004D8F4
	private void InternalAddItem(CollectableItem item, int amount)
	{
		if (!this.IsItemInMasterList(item) || amount <= 0)
		{
			return;
		}
		if (CollectableItemManager.IsInHiddenMode())
		{
			this.AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
			{
				data.AmountWhileHidden += amount;
			});
			return;
		}
		this.AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
		{
			data.Amount += amount;
		});
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0004F75C File Offset: 0x0004D95C
	private void InternalRemoveItem(CollectableItem item, int amount)
	{
		if (!this.IsItemInMasterList(item) || amount <= 0)
		{
			return;
		}
		if (CollectableItemManager.IsInHiddenMode())
		{
			this.AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
			{
				data.AmountWhileHidden -= amount;
				if (data.AmountWhileHidden < 0)
				{
					data.AmountWhileHidden = 0;
				}
			});
			return;
		}
		this.AffectItemData(item.name, delegate(ref CollectableItemsData.Data data)
		{
			data.Amount -= amount;
			if (data.Amount < 0)
			{
				data.Amount = 0;
			}
		});
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0004F7C4 File Offset: 0x0004D9C4
	private List<CollectableItem> InternalGetCollectedItems(Func<CollectableItem, bool> predicate)
	{
		if (!this.masterList)
		{
			return new List<CollectableItem>();
		}
		if (!Application.isPlaying)
		{
			return this.masterList.ToList<CollectableItem>();
		}
		List<CollectableItem> list = (from itemName in GameManager.instance.playerData.Collectables.GetValidNames(null)
		where !this.IsItemInMasterList(itemName)
		select itemName).Select(new Func<string, CollectableItem>(this.GetInvalidItem)).ToList<CollectableItem>();
		ICollection<CollectableItem> collection;
		if (predicate == null)
		{
			collection = this.GetAllCollectables();
		}
		else
		{
			ICollection<CollectableItem> collection2 = this.GetAllCollectables().Where(predicate).ToList<CollectableItem>();
			collection = collection2;
		}
		ICollection<CollectableItem> collection3 = collection;
		List<CollectableItem> list2 = new List<CollectableItem>(collection3.Count + list.Count);
		list2.AddRange(from item in collection3
		where item.IsVisible
		select item);
		list2.AddRange(list);
		foreach (CollectableItem collectableItem in list2)
		{
			collectableItem.ReportPreviouslyCollected();
		}
		return list2;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0004F8D8 File Offset: 0x0004DAD8
	public ICollection<CollectableItem> GetAllCollectables()
	{
		return this.masterList;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0004F8E0 File Offset: 0x0004DAE0
	public static CollectableItem GetItemByName(string itemName)
	{
		return ManagerSingleton<CollectableItemManager>.Instance.masterList.GetByName(itemName);
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0004F8F4 File Offset: 0x0004DAF4
	private void AffectItemData(string itemName, CollectableItemManager.ItemAffectingDelegate affector)
	{
		CollectableItemsData collectables = GameManager.instance.playerData.Collectables;
		CollectableItemsData.Data data = collectables.GetData(itemName);
		affector(ref data);
		collectables.SetData(itemName, data);
		CollectableItemManager.IncrementVersion();
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0004F92C File Offset: 0x0004DB2C
	private bool IsItemInMasterList(CollectableItem item)
	{
		return this.IsItemInMasterList(item.name);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0004F93C File Offset: 0x0004DB3C
	private bool IsItemInMasterList(string itemName)
	{
		if (!this.masterList)
		{
			Debug.LogError("Collectable Item masterList is not assigned!");
			return false;
		}
		using (IEnumerator<CollectableItem> enumerator = this.masterList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.name.Equals(itemName))
				{
					return true;
				}
			}
		}
		Debug.LogError(string.Format("Collectable Item: {0} not found in master list!", itemName));
		return false;
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x0004F9C0 File Offset: 0x0004DBC0
	private CollectableItem GetInvalidItem(string itemName)
	{
		CollectableItem collectableItem = this.invalidTemplate ? Object.Instantiate<CollectableItem>(this.invalidTemplate) : ScriptableObject.CreateInstance<CollectableItemBasic>();
		collectableItem.name = itemName;
		return collectableItem;
	}

	// Token: 0x04000FF3 RID: 4083
	[SerializeField]
	private CollectableItemList masterList;

	// Token: 0x04000FF4 RID: 4084
	[SerializeField]
	private CollectableItem invalidTemplate;

	// Token: 0x04000FF5 RID: 4085
	private CollectableItem collectedItem;

	// Token: 0x04000FF7 RID: 4087
	private static bool previousHiddenMode;

	// Token: 0x04000FF8 RID: 4088
	private static readonly ObjectCache<List<CollectableItem>> _collectedItemCache = new ObjectCache<List<CollectableItem>>();

	// Token: 0x020014EA RID: 5354
	// (Invoke) Token: 0x06008537 RID: 34103
	private delegate void ItemAffectingDelegate(ref CollectableItemsData.Data data);
}
