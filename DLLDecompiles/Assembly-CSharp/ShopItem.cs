using System;
using GlobalEnums;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200071E RID: 1822
[CreateAssetMenu(menuName = "Hornet/Shop Item")]
public class ShopItem : ScriptableObject
{
	// Token: 0x17000757 RID: 1879
	// (get) Token: 0x060040C1 RID: 16577 RVA: 0x0011C86B File Offset: 0x0011AA6B
	// (set) Token: 0x060040C2 RID: 16578 RVA: 0x0011C873 File Offset: 0x0011AA73
	public Func<int> OverrideCostDelegate { get; set; }

	// Token: 0x17000758 RID: 1880
	// (get) Token: 0x060040C3 RID: 16579 RVA: 0x0011C87C File Offset: 0x0011AA7C
	public string DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x17000759 RID: 1881
	// (get) Token: 0x060040C4 RID: 16580 RVA: 0x0011C88C File Offset: 0x0011AA8C
	public string Description
	{
		get
		{
			LocalisedString s;
			if (!this.descriptionMultiple.IsEmpty && this.savedItem)
			{
				try
				{
					s = ((this.savedItem.GetSavedAmount() > 0) ? this.descriptionMultiple : this.description);
					goto IL_4A;
				}
				catch (NotImplementedException)
				{
					s = this.description;
					goto IL_4A;
				}
			}
			s = this.description;
			IL_4A:
			if (this.requiredTools == ToolItemManager.OwnToolsCheckFlags.None)
			{
				return s;
			}
			int num = this.requiredToolsAmount - ToolItemManager.GetOwnedToolsCount(this.requiredTools);
			if (num <= 0)
			{
				return s;
			}
			string str;
			if (num == 1 && !this.requiredToolsDescription.Single.IsEmpty)
			{
				str = this.requiredToolsDescription.Single;
			}
			else
			{
				try
				{
					str = string.Format(this.requiredToolsDescription.Plural, num);
				}
				catch (FormatException exception)
				{
					str = this.requiredToolsDescription.Plural;
					Debug.LogException(exception, this);
				}
			}
			return s + "\n\n" + str;
		}
	}

	// Token: 0x1700075A RID: 1882
	// (get) Token: 0x060040C5 RID: 16581 RVA: 0x0011C9A0 File Offset: 0x0011ABA0
	public Sprite ItemSprite
	{
		get
		{
			if (this.itemSprite)
			{
				return this.itemSprite;
			}
			if (this.upgradeFromItem)
			{
				return this.upgradeFromItem.GetIcon(CollectableItem.ReadSource.Shop);
			}
			if (!this.savedItem)
			{
				return null;
			}
			return this.savedItem.GetPopupIcon();
		}
	}

	// Token: 0x1700075B RID: 1883
	// (get) Token: 0x060040C6 RID: 16582 RVA: 0x0011C9F5 File Offset: 0x0011ABF5
	public float ItemSpriteScale
	{
		get
		{
			return this.itemSpriteScale;
		}
	}

	// Token: 0x1700075C RID: 1884
	// (get) Token: 0x060040C7 RID: 16583 RVA: 0x0011C9FD File Offset: 0x0011ABFD
	public CurrencyType CurrencyType
	{
		get
		{
			return this.currencyType;
		}
	}

	// Token: 0x1700075D RID: 1885
	// (get) Token: 0x060040C8 RID: 16584 RVA: 0x0011CA05 File Offset: 0x0011AC05
	public int Cost
	{
		get
		{
			if (this.OverrideCostDelegate != null)
			{
				return this.OverrideCostDelegate();
			}
			if (this.costReference)
			{
				return this.costReference.Value;
			}
			return this.cost;
		}
	}

	// Token: 0x1700075E RID: 1886
	// (get) Token: 0x060040C9 RID: 16585 RVA: 0x0011CA3A File Offset: 0x0011AC3A
	public CollectableItem RequiredItem
	{
		get
		{
			return this.requiredItem;
		}
	}

	// Token: 0x1700075F RID: 1887
	// (get) Token: 0x060040CA RID: 16586 RVA: 0x0011CA42 File Offset: 0x0011AC42
	public int RequiredItemAmount
	{
		get
		{
			return this.requiredItemAmount;
		}
	}

	// Token: 0x17000760 RID: 1888
	// (get) Token: 0x060040CB RID: 16587 RVA: 0x0011CA4A File Offset: 0x0011AC4A
	public ToolItemManager.OwnToolsCheckFlags RequiredTools
	{
		get
		{
			return this.requiredTools;
		}
	}

	// Token: 0x17000761 RID: 1889
	// (get) Token: 0x060040CC RID: 16588 RVA: 0x0011CA52 File Offset: 0x0011AC52
	public int RequiredToolsAmount
	{
		get
		{
			return this.requiredToolsAmount;
		}
	}

	// Token: 0x17000762 RID: 1890
	// (get) Token: 0x060040CD RID: 16589 RVA: 0x0011CA5A File Offset: 0x0011AC5A
	public CollectableItem UpgradeFromItem
	{
		get
		{
			return this.upgradeFromItem;
		}
	}

	// Token: 0x17000763 RID: 1891
	// (get) Token: 0x060040CE RID: 16590 RVA: 0x0011CA64 File Offset: 0x0011AC64
	public bool IsAvailable
	{
		get
		{
			if (this.IsPurchased)
			{
				return false;
			}
			if (this.UpgradeFromItem && this.UpgradeFromItem.CollectedAmount <= 0)
			{
				return false;
			}
			foreach (QuestTest questTest in this.questsAppearConditions)
			{
				if (!questTest.IsFulfilled)
				{
					return false;
				}
			}
			return this.extraAppearConditions.IsFulfilled;
		}
	}

	// Token: 0x17000764 RID: 1892
	// (get) Token: 0x060040CF RID: 16591 RVA: 0x0011CACC File Offset: 0x0011ACCC
	public bool IsPurchased
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return (!string.IsNullOrEmpty(this.playerDataBoolName) && instance.GetVariable(this.playerDataBoolName)) || (!string.IsNullOrEmpty(this.playerDataIntName) && instance.GetVariable(this.playerDataIntName) != BellhomePaintColours.None) || (this.savedItem && !this.savedItem.CanGetMore());
		}
	}

	// Token: 0x17000765 RID: 1893
	// (get) Token: 0x060040D0 RID: 16592 RVA: 0x0011CB36 File Offset: 0x0011AD36
	public bool IsAvailableNotInfinite
	{
		get
		{
			return this.IsAvailable && (!string.IsNullOrEmpty(this.playerDataBoolName) || (this.savedItem && this.savedItem.IsUnique));
		}
	}

	// Token: 0x17000766 RID: 1894
	// (get) Token: 0x060040D1 RID: 16593 RVA: 0x0011CB6E File Offset: 0x0011AD6E
	public SavedItem Item
	{
		get
		{
			return this.savedItem;
		}
	}

	// Token: 0x17000767 RID: 1895
	// (get) Token: 0x060040D2 RID: 16594 RVA: 0x0011CB76 File Offset: 0x0011AD76
	public string EventAfterPurchase
	{
		get
		{
			return this.eventAfterPurchased;
		}
	}

	// Token: 0x17000768 RID: 1896
	// (get) Token: 0x060040D3 RID: 16595 RVA: 0x0011CB80 File Offset: 0x0011AD80
	public int SubItemsCount
	{
		get
		{
			int num = 0;
			if (this.subItems != null)
			{
				ShopItem.SubItem[] array = this.subItems;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Condition.IsFulfilled)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x17000769 RID: 1897
	// (get) Token: 0x060040D4 RID: 16596 RVA: 0x0011CBC4 File Offset: 0x0011ADC4
	public bool HasSubItems
	{
		get
		{
			return this.SubItemsCount > 0;
		}
	}

	// Token: 0x1700076A RID: 1898
	// (get) Token: 0x060040D5 RID: 16597 RVA: 0x0011CBCF File Offset: 0x0011ADCF
	public LocalisedString SubItemSelectPrompt
	{
		get
		{
			return this.subItemSelectPrompt;
		}
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x0011CBD7 File Offset: 0x0011ADD7
	public bool IsUsingCostReference()
	{
		return this.costReference;
	}

	// Token: 0x060040D7 RID: 16599 RVA: 0x0011CBE4 File Offset: 0x0011ADE4
	public bool IsUsingRequiredTools()
	{
		return this.requiredTools > ToolItemManager.OwnToolsCheckFlags.None;
	}

	// Token: 0x060040D8 RID: 16600 RVA: 0x0011CBF0 File Offset: 0x0011ADF0
	private bool? ItemSpriteValidation()
	{
		if (this.savedItem && this.ItemSprite && this.itemSprite == null)
		{
			return null;
		}
		return new bool?(this.itemSprite);
	}

	// Token: 0x060040D9 RID: 16601 RVA: 0x0011CC3F File Offset: 0x0011AE3F
	private void OnValidate()
	{
		if (this.requiredItemAmount < 1 && this.requiredItem)
		{
			this.requiredItemAmount = 1;
		}
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x0011CC5E File Offset: 0x0011AE5E
	private void Awake()
	{
		this.OnValidate();
		if (this.questsAppearConditions == null)
		{
			this.questsAppearConditions = Array.Empty<QuestTest>();
		}
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x0011CC7C File Offset: 0x0011AE7C
	public void SetPurchased(Action onComplete, int subItemIndex)
	{
		CurrencyType currencyType = this.CurrencyType;
		if (currencyType != CurrencyType.Money)
		{
			if (currencyType != CurrencyType.Shard)
			{
				throw new ArgumentOutOfRangeException();
			}
			CurrencyManager.TakeShards(this.Cost);
		}
		else
		{
			CurrencyManager.TakeGeo(this.Cost);
		}
		ShopItem.ConditionalSpawn[] array = this.spawnOnPurchaseConditionals;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].TryInstantiate();
		}
		if (this.requiredItem)
		{
			this.requiredItem.Consume(this.requiredItemAmount, true);
		}
		if (!string.IsNullOrEmpty(this.playerDataBoolName))
		{
			PlayerData.instance.SetVariable(this.playerDataBoolName, true);
		}
		if (!string.IsNullOrEmpty(this.playerDataIntName) && subItemIndex >= 0 && subItemIndex < this.SubItemsCount)
		{
			PlayerData.instance.SetVariable(this.playerDataIntName, this.GetSubItem(subItemIndex).Value);
		}
		if (this.upgradeFromItem)
		{
			this.upgradeFromItem.Consume(this.upgradeFromItem.CollectedAmount, true);
		}
		if (this.onPurchase != null)
		{
			this.onPurchase.Invoke();
		}
		foreach (string text in this.setExtraPlayerDataBools)
		{
			if (!string.IsNullOrEmpty(text))
			{
				PlayerData.instance.SetVariable(text, true);
			}
		}
		foreach (PlayerDataIntOperation playerDataIntOperation in this.setExtraPlayerDataInts)
		{
			playerDataIntOperation.Execute();
		}
		if ((this.typeFlags & ShopItem.TypeFlags.Map) != ShopItem.TypeFlags.None)
		{
			GameManager instance = GameManager.instance;
			instance.UpdateGameMap();
			instance.DidPurchaseMap = true;
			instance.CheckMapAchievements();
		}
		else if (this.typeFlags.HasFlag(ShopItem.TypeFlags.MapUpdateItem))
		{
			GameManager.instance.UpdateGameMapPins();
		}
		CollectableItemManager.IncrementVersion();
		ToolItem toolItem = this.savedItem as ToolItem;
		if (toolItem != null)
		{
			toolItem.Unlock(onComplete, ToolItem.PopupFlags.Tutorial);
			return;
		}
		if (this.savedItem)
		{
			this.savedItem.Get(false);
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x0011CE67 File Offset: 0x0011B067
	public bool IsToolItem()
	{
		return this.savedItem && this.savedItem is ToolItem;
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x0011CE88 File Offset: 0x0011B088
	public bool IsAtMax()
	{
		CollectableItem collectableItem = this.savedItem as CollectableItem;
		return !(collectableItem == null) && collectableItem.IsAtMax();
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x0011CEB4 File Offset: 0x0011B0B4
	public ToolItemType GetToolType()
	{
		ToolItem toolItem = this.savedItem as ToolItem;
		if (toolItem == null)
		{
			return ToolItemType.Red;
		}
		return toolItem.Type;
	}

	// Token: 0x060040DF RID: 16607 RVA: 0x0011CEDE File Offset: 0x0011B0DE
	public ShopItem.PurchaseTypes GetPurchaseType()
	{
		return this.purchaseType;
	}

	// Token: 0x060040E0 RID: 16608 RVA: 0x0011CEE6 File Offset: 0x0011B0E6
	public ShopItem.TypeFlags GetTypeFlags()
	{
		return this.typeFlags;
	}

	// Token: 0x060040E1 RID: 16609 RVA: 0x0011CEF0 File Offset: 0x0011B0F0
	public bool EnsurePool(GameObject gameObject)
	{
		bool result = false;
		ShopItem.ConditionalSpawn[] array = this.spawnOnPurchaseConditionals;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].EnsurePool(gameObject))
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060040E2 RID: 16610 RVA: 0x0011CF24 File Offset: 0x0011B124
	public ShopItem.SubItem GetSubItem(int index)
	{
		int num = 0;
		foreach (ShopItem.SubItem subItem in this.subItems)
		{
			if (subItem.Condition.IsFulfilled)
			{
				if (num == index)
				{
					return subItem;
				}
				num++;
			}
		}
		throw new IndexOutOfRangeException();
	}

	// Token: 0x060040E3 RID: 16611 RVA: 0x0011CF6C File Offset: 0x0011B16C
	public static ShopItem CreateTemp(string name)
	{
		ShopItem shopItem = ScriptableObject.CreateInstance<ShopItem>();
		shopItem.displayName.Sheet = "TEMP";
		shopItem.displayName.Key = name;
		shopItem.extraAppearConditions = new PlayerDataTest();
		shopItem.questsAppearConditions = new QuestTest[0];
		shopItem.spawnOnPurchaseConditionals = new ShopItem.ConditionalSpawn[0];
		shopItem.setExtraPlayerDataBools = new string[0];
		shopItem.setExtraPlayerDataInts = new PlayerDataIntOperation[0];
		return shopItem;
	}

	// Token: 0x04004244 RID: 16964
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04004245 RID: 16965
	[SerializeField]
	private LocalisedString description;

	// Token: 0x04004246 RID: 16966
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString descriptionMultiple;

	// Token: 0x04004247 RID: 16967
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ItemSpriteValidation")]
	private Sprite itemSprite;

	// Token: 0x04004248 RID: 16968
	[SerializeField]
	private float itemSpriteScale = 1f;

	// Token: 0x04004249 RID: 16969
	[SerializeField]
	private ShopItem.PurchaseTypes purchaseType;

	// Token: 0x0400424A RID: 16970
	[Space]
	[SerializeField]
	[EnumPickerBitmask]
	private ShopItem.TypeFlags typeFlags;

	// Token: 0x0400424B RID: 16971
	[Space]
	[SerializeField]
	private CurrencyType currencyType;

	// Token: 0x0400424C RID: 16972
	[SerializeField]
	private CostReference costReference;

	// Token: 0x0400424D RID: 16973
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingCostReference", false, true, false)]
	private int cost;

	// Token: 0x0400424E RID: 16974
	[Space]
	[SerializeField]
	private CollectableItem requiredItem;

	// Token: 0x0400424F RID: 16975
	[SerializeField]
	[ModifiableProperty]
	[Conditional("requiredItem", true, false, false)]
	private int requiredItemAmount = 1;

	// Token: 0x04004250 RID: 16976
	[SerializeField]
	[EnumPickerBitmask]
	private ToolItemManager.OwnToolsCheckFlags requiredTools;

	// Token: 0x04004251 RID: 16977
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRequiredTools", true, true, false)]
	private int requiredToolsAmount;

	// Token: 0x04004252 RID: 16978
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRequiredTools", true, true, false)]
	private ShopItem.LocalisedStringPlural requiredToolsDescription;

	// Token: 0x04004253 RID: 16979
	[SerializeField]
	private CollectableItem upgradeFromItem;

	// Token: 0x04004254 RID: 16980
	[Space]
	[SerializeField]
	private PlayerDataTest extraAppearConditions;

	// Token: 0x04004255 RID: 16981
	[SerializeField]
	private QuestTest[] questsAppearConditions;

	// Token: 0x04004256 RID: 16982
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBoolName;

	// Token: 0x04004257 RID: 16983
	[SerializeField]
	private SavedItem savedItem;

	// Token: 0x04004258 RID: 16984
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(BellhomePaintColours), false)]
	private string playerDataIntName;

	// Token: 0x04004259 RID: 16985
	[SerializeField]
	[Conditional("playerDataIntName", true, false, false)]
	private ShopItem.SubItem[] subItems;

	// Token: 0x0400425A RID: 16986
	[SerializeField]
	[Conditional("playerDataIntName", true, false, false)]
	private LocalisedString subItemSelectPrompt;

	// Token: 0x0400425B RID: 16987
	[Space]
	[SerializeField]
	private UnityEvent onPurchase;

	// Token: 0x0400425C RID: 16988
	[SerializeField]
	private ShopItem.ConditionalSpawn[] spawnOnPurchaseConditionals;

	// Token: 0x0400425D RID: 16989
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string[] setExtraPlayerDataBools;

	// Token: 0x0400425E RID: 16990
	[SerializeField]
	private PlayerDataIntOperation[] setExtraPlayerDataInts;

	// Token: 0x0400425F RID: 16991
	[Space]
	[SerializeField]
	private string eventAfterPurchased;

	// Token: 0x02001A06 RID: 6662
	[Serializable]
	private class ConditionalSpawn
	{
		// Token: 0x060095C4 RID: 38340 RVA: 0x002A62B0 File Offset: 0x002A44B0
		public bool EnsurePool(GameObject gameObject)
		{
			if (!this.Condition.IsFulfilled)
			{
				return false;
			}
			bool result = false;
			foreach (GameObject gameObject2 in this.GameObjectsToSpawn)
			{
				if (gameObject2)
				{
					PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, 1, false, false, false);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060095C5 RID: 38341 RVA: 0x002A6300 File Offset: 0x002A4500
		public void TryInstantiate()
		{
			if (!this.Condition.IsFulfilled)
			{
				return;
			}
			foreach (GameObject gameObject in this.GameObjectsToSpawn)
			{
				if (gameObject)
				{
					gameObject.Spawn();
				}
			}
		}

		// Token: 0x04009823 RID: 38947
		public PlayerDataTest Condition;

		// Token: 0x04009824 RID: 38948
		public GameObject[] GameObjectsToSpawn;
	}

	// Token: 0x02001A07 RID: 6663
	[Serializable]
	private struct LocalisedStringPlural
	{
		// Token: 0x04009825 RID: 38949
		public LocalisedString Plural;

		// Token: 0x04009826 RID: 38950
		[SerializeField]
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Single;
	}

	// Token: 0x02001A08 RID: 6664
	[Flags]
	public enum TypeFlags
	{
		// Token: 0x04009828 RID: 38952
		None = 0,
		// Token: 0x04009829 RID: 38953
		Item = 1,
		// Token: 0x0400982A RID: 38954
		Map = 2,
		// Token: 0x0400982B RID: 38955
		MapUpdateItem = 4,
		// Token: 0x0400982C RID: 38956
		BellhomeFurnishing = 8
	}

	// Token: 0x02001A09 RID: 6665
	public enum PurchaseTypes
	{
		// Token: 0x0400982E RID: 38958
		Purchase,
		// Token: 0x0400982F RID: 38959
		Craft,
		// Token: 0x04009830 RID: 38960
		Repair
	}

	// Token: 0x02001A0A RID: 6666
	[Serializable]
	public struct SubItem
	{
		// Token: 0x04009831 RID: 38961
		public BellhomePaintColours Value;

		// Token: 0x04009832 RID: 38962
		public Sprite Icon;

		// Token: 0x04009833 RID: 38963
		public Sprite PurchaseIcon;

		// Token: 0x04009834 RID: 38964
		public PlayerDataTest Condition;
	}
}
