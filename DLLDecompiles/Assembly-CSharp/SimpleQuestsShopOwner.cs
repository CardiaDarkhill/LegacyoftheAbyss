using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000728 RID: 1832
public class SimpleQuestsShopOwner : SimpleShopMenuOwner
{
	// Token: 0x17000777 RID: 1911
	// (get) Token: 0x06004171 RID: 16753 RVA: 0x0011FD4A File Offset: 0x0011DF4A
	// (set) Token: 0x06004172 RID: 16754 RVA: 0x0011FD52 File Offset: 0x0011DF52
	[UsedImplicitly]
	public Quest QueuedQuest { get; private set; }

	// Token: 0x17000778 RID: 1912
	// (get) Token: 0x06004173 RID: 16755 RVA: 0x0011FD5B File Offset: 0x0011DF5B
	// (set) Token: 0x06004174 RID: 16756 RVA: 0x0011FD63 File Offset: 0x0011DF63
	[UsedImplicitly]
	public CollectableItem QueuedCustomDelivery { get; private set; }

	// Token: 0x17000779 RID: 1913
	// (get) Token: 0x06004175 RID: 16757 RVA: 0x0011FD6C File Offset: 0x0011DF6C
	// (set) Token: 0x06004176 RID: 16758 RVA: 0x0011FD74 File Offset: 0x0011DF74
	[UsedImplicitly]
	public string QueuedPurchaseDlg { get; private set; }

	// Token: 0x06004177 RID: 16759 RVA: 0x0011FD80 File Offset: 0x0011DF80
	protected override List<ISimpleShopItem> GetItems()
	{
		PlayerData instance = PlayerData.instance;
		List<string> list;
		bool flag;
		if (!string.IsNullOrEmpty(this.genericQuestsList))
		{
			list = (instance.GetVariable(this.genericQuestsList) ?? new List<string>());
			if (list.Count < 2)
			{
				list.Clear();
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			list = null;
			flag = false;
		}
		if (this.runningList == null)
		{
			this.runningList = new List<SimpleQuestsShopOwner.ShopItemInfo>();
		}
		else
		{
			this.runningList.Clear();
		}
		if (this.runningGenericList == null)
		{
			this.runningGenericList = new List<SimpleQuestsShopOwner.ShopItemInfo>();
		}
		else
		{
			this.runningGenericList.Clear();
		}
		int num = 0;
		foreach (SimpleQuestsShopOwner.ShopItemInfo shopItemInfo in this.quests)
		{
			if (shopItemInfo.Quest && shopItemInfo.Quest.IsCompleted)
			{
				num++;
			}
		}
		foreach (SimpleQuestsShopOwner.ShopItemInfo shopItemInfo2 in this.quests)
		{
			if (shopItemInfo2.AppearCondition.IsFulfilled && (shopItemInfo2.AppearAfterCountCompleted <= 0 || num >= shopItemInfo2.AppearAfterCountCompleted))
			{
				bool flag2 = false;
				foreach (FullQuestBase fullQuestBase in shopItemInfo2.AppearAfterCompleted)
				{
					if (fullQuestBase && !fullQuestBase.IsCompleted)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					Quest quest = shopItemInfo2.Quest;
					if (shopItemInfo2.CustomDelivery)
					{
						if (quest && (!quest.IsAccepted || quest.IsCompleted))
						{
							continue;
						}
						if (shopItemInfo2.CustomDelivery.CollectedAmount > 0)
						{
							continue;
						}
					}
					else
					{
						if (!quest || !quest.IsAvailable)
						{
							continue;
						}
						if (shopItemInfo2.IsRepeatable)
						{
							if (quest.IsAccepted && !quest.IsCompleted)
							{
								continue;
							}
							if (flag)
							{
								this.runningGenericList.Add(shopItemInfo2);
								continue;
							}
							if (list != null && !list.Contains(quest.name))
							{
								continue;
							}
						}
						else if (quest.IsAccepted || quest.IsCompleted)
						{
							continue;
						}
					}
					this.runningList.Add(shopItemInfo2);
				}
			}
		}
		if (flag)
		{
			this.runningGenericList.Shuffle<SimpleQuestsShopOwner.ShopItemInfo>();
			while (this.runningGenericList.Count > this.genericQuestCap)
			{
				this.runningGenericList.RemoveAt(this.runningGenericList.Count - 1);
			}
			foreach (SimpleQuestsShopOwner.ShopItemInfo shopItemInfo3 in this.quests)
			{
				if (this.runningGenericList.Contains(shopItemInfo3))
				{
					list.Add(shopItemInfo3.Quest.name);
					this.runningList.Add(shopItemInfo3);
				}
			}
			instance.SetVariable(this.genericQuestsList, list);
		}
		if (this.currentList == null)
		{
			this.currentList = new List<ISimpleShopItem>();
		}
		else
		{
			this.currentList.Clear();
		}
		this.currentList.AddRange(this.runningList);
		this.runningList.Clear();
		this.runningGenericList.Clear();
		if (flag)
		{
			return this.GetItems();
		}
		return this.currentList;
	}

	// Token: 0x06004178 RID: 16760 RVA: 0x00120110 File Offset: 0x0011E310
	protected override void OnPurchasedItem(int itemIndex)
	{
		if (itemIndex < 0 || itemIndex >= this.currentList.Count)
		{
			Debug.LogError("Can't handle purchase for itemIndex out of range: " + itemIndex.ToString(), this);
			return;
		}
		SimpleQuestsShopOwner.ShopItemInfo shopItemInfo = (SimpleQuestsShopOwner.ShopItemInfo)this.currentList[itemIndex];
		this.QueuedCustomDelivery = shopItemInfo.CustomDelivery;
		this.QueuedQuest = (this.QueuedCustomDelivery ? null : shopItemInfo.Quest);
		if (string.IsNullOrEmpty(this.purchasedDlgBitmask))
		{
			this.QueuedPurchaseDlg = shopItemInfo.PurchasedDlg;
			return;
		}
		int num = this.quests.IndexOf(shopItemInfo);
		bool flag = num < 0;
		PlayerData instance = PlayerData.instance;
		int num2 = instance.GetVariable(this.purchasedDlgBitmask);
		if (!flag && num2.IsBitSet(num))
		{
			this.QueuedPurchaseDlg = (shopItemInfo.RePurchasedDlg.IsEmpty ? shopItemInfo.PurchasedDlg : shopItemInfo.RePurchasedDlg);
			return;
		}
		this.QueuedPurchaseDlg = shopItemInfo.PurchasedDlg;
		if (!flag)
		{
			num2 = num2.SetBitAtIndex(num);
			instance.SetVariable(this.purchasedDlgBitmask, num2);
		}
	}

	// Token: 0x06004179 RID: 16761 RVA: 0x00120228 File Offset: 0x0011E428
	[UsedImplicitly]
	public bool IsAnyQuestInProgress()
	{
		foreach (SimpleQuestsShopOwner.ShopItemInfo shopItemInfo in this.quests)
		{
			if (shopItemInfo.CustomDelivery)
			{
				if (shopItemInfo.CustomDelivery.CollectedAmount > 0)
				{
					return true;
				}
			}
			else
			{
				Quest quest = shopItemInfo.Quest;
				if (quest && quest.IsAccepted && !quest.IsCompleted)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x040042EF RID: 17135
	[SerializeField]
	private List<SimpleQuestsShopOwner.ShopItemInfo> quests;

	// Token: 0x040042F0 RID: 17136
	[SerializeField]
	[PlayerDataField(typeof(int), false)]
	private string purchasedDlgBitmask;

	// Token: 0x040042F1 RID: 17137
	[SerializeField]
	[PlayerDataField(typeof(List<string>), false)]
	private string genericQuestsList;

	// Token: 0x040042F2 RID: 17138
	[SerializeField]
	[ModifiableProperty]
	[Conditional("genericQuestsList", true, false, false)]
	private int genericQuestCap;

	// Token: 0x040042F3 RID: 17139
	private List<SimpleQuestsShopOwner.ShopItemInfo> runningList;

	// Token: 0x040042F4 RID: 17140
	private List<SimpleQuestsShopOwner.ShopItemInfo> runningGenericList;

	// Token: 0x040042F5 RID: 17141
	private List<ISimpleShopItem> currentList;

	// Token: 0x02001A10 RID: 6672
	[Serializable]
	public class ShopItemInfo : ISimpleShopItem
	{
		// Token: 0x060095D0 RID: 38352 RVA: 0x002A645A File Offset: 0x002A465A
		public string GetDisplayName()
		{
			if (this.CustomDelivery)
			{
				return this.CustomDelivery.GetDisplayName(CollectableItem.ReadSource.GetPopup);
			}
			if (!this.Quest)
			{
				return string.Empty;
			}
			return this.Quest.DisplayName;
		}

		// Token: 0x060095D1 RID: 38353 RVA: 0x002A649C File Offset: 0x002A469C
		public Sprite GetIcon()
		{
			if (this.CustomDelivery)
			{
				return this.CustomDelivery.GetPopupIcon();
			}
			if (!this.Quest)
			{
				return null;
			}
			DeliveryQuestItem deliveryQuestItem = (from target in this.Quest.Targets
			select target.Counter).OfType<DeliveryQuestItem>().FirstOrDefault<DeliveryQuestItem>();
			if (!deliveryQuestItem)
			{
				return null;
			}
			return deliveryQuestItem.GetPopupIcon();
		}

		// Token: 0x060095D2 RID: 38354 RVA: 0x002A651B File Offset: 0x002A471B
		public int GetCost()
		{
			return 0;
		}

		// Token: 0x060095D3 RID: 38355 RVA: 0x002A651E File Offset: 0x002A471E
		public bool DelayPurchase()
		{
			return true;
		}

		// Token: 0x0400984D RID: 38989
		public Quest Quest;

		// Token: 0x0400984E RID: 38990
		public PlayerDataTest AppearCondition;

		// Token: 0x0400984F RID: 38991
		public FullQuestBase[] AppearAfterCompleted;

		// Token: 0x04009850 RID: 38992
		public int AppearAfterCountCompleted;

		// Token: 0x04009851 RID: 38993
		public CollectableItem CustomDelivery;

		// Token: 0x04009852 RID: 38994
		[ModifiableProperty]
		[Conditional("CustomDelivery", false, false, false)]
		public bool IsRepeatable = true;

		// Token: 0x04009853 RID: 38995
		[Space]
		public LocalisedString PurchasedDlg;

		// Token: 0x04009854 RID: 38996
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString RePurchasedDlg;
	}
}
