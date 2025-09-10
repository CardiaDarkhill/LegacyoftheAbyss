using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001AA RID: 426
public abstract class CollectableItem : QuestTargetCounter, ICollectionViewerItem
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06001059 RID: 4185 RVA: 0x0004E990 File Offset: 0x0004CB90
	public LocalisedString UseResponseTextOverride
	{
		get
		{
			return this.useResponseTextOverride;
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x0600105A RID: 4186 RVA: 0x0004E998 File Offset: 0x0004CB98
	public bool PreventUseChaining
	{
		get
		{
			return this.preventUseChaining;
		}
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x0600105B RID: 4187 RVA: 0x0004E9A0 File Offset: 0x0004CBA0
	public AudioEventRandom UseSounds
	{
		get
		{
			return this.useSounds;
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x0600105C RID: 4188 RVA: 0x0004E9A8 File Offset: 0x0004CBA8
	public AudioEventRandom InstantUseSounds
	{
		get
		{
			return this.instantUseSounds;
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x0600105D RID: 4189 RVA: 0x0004E9B0 File Offset: 0x0004CBB0
	public bool AlwaysPlayInstantUse
	{
		get
		{
			return this.alwaysPlayInstantUse;
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x0600105E RID: 4190 RVA: 0x0004E9B8 File Offset: 0x0004CBB8
	public bool SkipBenchUseEffect
	{
		get
		{
			return this.skipBenchUseEffect;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x0600105F RID: 4191 RVA: 0x0004E9C0 File Offset: 0x0004CBC0
	public GameObject ExtraUseEffect
	{
		get
		{
			return this.extraUseEffect;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06001060 RID: 4192 RVA: 0x0004E9C8 File Offset: 0x0004CBC8
	public CustomInventoryItemCollectableDisplay CustomInventoryDisplay
	{
		get
		{
			return this.customInventoryDisplay;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06001061 RID: 4193 RVA: 0x0004E9D0 File Offset: 0x0004CBD0
	public GameObject ExtraDescriptionSection
	{
		get
		{
			return this.extraDescriptionSection;
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06001062 RID: 4194 RVA: 0x0004E9D8 File Offset: 0x0004CBD8
	public bool HideInShopCounters
	{
		get
		{
			return this.hideInShopCounters;
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06001063 RID: 4195
	public abstract bool DisplayAmount { get; }

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06001064 RID: 4196 RVA: 0x0004E9E0 File Offset: 0x0004CBE0
	// (set) Token: 0x06001065 RID: 4197 RVA: 0x0004E9F7 File Offset: 0x0004CBF7
	public CollectableItemsData.Data SaveData
	{
		get
		{
			return PlayerData.instance.Collectables.GetData(base.name);
		}
		set
		{
			PlayerData.instance.Collectables.SetData(base.name, value);
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06001066 RID: 4198 RVA: 0x0004EA0F File Offset: 0x0004CC0F
	public virtual int CollectedAmount
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0;
			}
			if (CollectableItemManager.IsInHiddenMode())
			{
				return this.SaveData.AmountWhileHidden;
			}
			return this.SaveData.Amount;
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06001067 RID: 4199 RVA: 0x0004EA38 File Offset: 0x0004CC38
	public bool IsVisible
	{
		get
		{
			return !this.isHidden && this.CollectedAmount > 0;
		}
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06001068 RID: 4200 RVA: 0x0004EA50 File Offset: 0x0004CC50
	// (set) Token: 0x06001069 RID: 4201 RVA: 0x0004EA84 File Offset: 0x0004CC84
	public bool IsSeen
	{
		get
		{
			int isSeenIndex = this.IsSeenIndex;
			if (isSeenIndex < 0)
			{
				return true;
			}
			int isSeenMask = this.SaveData.IsSeenMask;
			int num = 1 << isSeenIndex;
			return (isSeenMask & num) == num;
		}
		set
		{
			int isSeenIndex = this.IsSeenIndex;
			if (isSeenIndex < 0)
			{
				return;
			}
			CollectableItemsData.Data saveData = this.SaveData;
			int num = saveData.IsSeenMask;
			int num2 = 1 << isSeenIndex;
			if (value)
			{
				num |= num2;
			}
			else
			{
				num &= ~num2;
			}
			saveData.IsSeenMask = num;
			this.SaveData = saveData;
		}
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x0600106A RID: 4202 RVA: 0x0004EACF File Offset: 0x0004CCCF
	protected virtual int IsSeenIndex
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x0600106B RID: 4203 RVA: 0x0004EAD2 File Offset: 0x0004CCD2
	public bool IsVisibleWithBareInventory
	{
		get
		{
			return this.isVisibleWithBareInventory;
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x0600106C RID: 4204 RVA: 0x0004EADA File Offset: 0x0004CCDA
	public Quest UseQuestForCap
	{
		get
		{
			return this.useQuestForCap;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x0600106D RID: 4205 RVA: 0x0004EAE2 File Offset: 0x0004CCE2
	protected virtual bool CanShowQuestUpdatedForItem
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600106E RID: 4206
	public abstract string GetDisplayName(CollectableItem.ReadSource readSource);

	// Token: 0x0600106F RID: 4207
	public abstract string GetDescription(CollectableItem.ReadSource readSource);

	// Token: 0x06001070 RID: 4208
	public abstract Sprite GetIcon(CollectableItem.ReadSource readSource);

	// Token: 0x06001071 RID: 4209 RVA: 0x0004EAE5 File Offset: 0x0004CCE5
	public virtual InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		return null;
	}

	// Token: 0x06001072 RID: 4210 RVA: 0x0004EAE8 File Offset: 0x0004CCE8
	protected virtual IEnumerable<CollectableItem.UseResponse> GetUseResponses()
	{
		return this.useResponses;
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001073 RID: 4211 RVA: 0x0004EAF0 File Offset: 0x0004CCF0
	public virtual bool TakeItemOnConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x0004EAF4 File Offset: 0x0004CCF4
	public string[] GetUseResponseDescriptions()
	{
		return (from response in this.GetUseResponses()
		where !response.Description.IsEmpty
		select string.Format(response.Description, response.GetAmountText())).ToArray<string>();
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x0004EB54 File Offset: 0x0004CD54
	public virtual bool ShouldStopCollectNoMsg()
	{
		return false;
	}

	// Token: 0x06001076 RID: 4214 RVA: 0x0004EB58 File Offset: 0x0004CD58
	public void Collect(int amount = 1, bool showPopup = true)
	{
		if (this.ShouldStopCollectNoMsg())
		{
			return;
		}
		if (!this.IsAtMax())
		{
			if (this.resetIsSeen)
			{
				this.IsSeen = false;
			}
			this.AddAmount(amount);
			if (showPopup)
			{
				CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this, this.IsAtMax() ? UI.MaxItemsTextColor : Color.white, null, false);
				if (this.CanShowQuestUpdatedForItem && QuestManager.MaybeShowQuestUpdated(this, itemUiMsg))
				{
					showPopup = false;
				}
				CollectableItemHeroReaction.DoReaction();
			}
			this.SetHasNew(showPopup);
			EventRegister.SendEvent(EventRegisterEvents.ItemCollected, null);
			PlayerStory.RecordEvent(this.storyEvent);
			this.OnCollected();
			ToolItemManager.ReportAllBoundAttackToolsUpdated();
			ItemCurrencyCounter.UpdateValue(this);
			return;
		}
		if (!showPopup)
		{
			return;
		}
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = UI.MaxItemsPopup,
			Icon = this.GetIcon(CollectableItem.ReadSource.GetPopup),
			IconScale = this.GetUIMsgIconScale(),
			RepresentingObject = this
		}, UI.MaxItemsTextColor, null, false);
		CollectableItemHeroReaction.DoReaction();
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x0004EC4C File Offset: 0x0004CE4C
	public override void SetHasNew(bool hasPopup)
	{
		if (hasPopup)
		{
			CollectableItemManager.CollectedItem = this;
			InventoryPaneList.SetNextOpen("Inv");
		}
		if (this.resetIsSeen)
		{
			this.IsSeen = false;
		}
		PlayerData.instance.InvPaneHasNew = true;
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0004EC7B File Offset: 0x0004CE7B
	protected virtual void AddAmount(int amount)
	{
		CollectableItemManager.AddItem(this, amount);
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x0004EC84 File Offset: 0x0004CE84
	protected virtual void OnCollected()
	{
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x0004EC86 File Offset: 0x0004CE86
	public void Take(int amount = 1, bool showCounter = true)
	{
		CollectableItemManager.RemoveItem(this, amount);
		if (showCounter)
		{
			ItemCurrencyCounter.Take(this, amount);
		}
		else
		{
			ItemCurrencyCounter.UpdateValue(this);
		}
		this.OnTaken();
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x0004ECA7 File Offset: 0x0004CEA7
	protected virtual void OnTaken()
	{
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x0004ECA9 File Offset: 0x0004CEA9
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return this.SaveData.Amount + this.SaveData.AmountWhileHidden;
		}
		return this.SaveData.Amount;
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x0600107D RID: 4221 RVA: 0x0004ECD5 File Offset: 0x0004CED5
	public override bool CanConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x0004ECD8 File Offset: 0x0004CED8
	public override void Consume(int amount, bool showCounter)
	{
		this.Take(amount, showCounter && this.ShowCounterOnConsume);
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x0004ECF0 File Offset: 0x0004CEF0
	public virtual bool IsAtMax()
	{
		if (this.customMaxAmount > 0)
		{
			return this.CollectedAmount >= this.customMaxAmount;
		}
		if (this.useQuestForCap)
		{
			foreach (FullQuestBase.QuestTarget questTarget in this.useQuestForCap.Targets)
			{
				if (!(questTarget.Counter != this) && this.CollectedAmount >= questTarget.Count)
				{
					return true;
				}
			}
			return false;
		}
		int consumableItemCap = Gameplay.ConsumableItemCap;
		return consumableItemCap > 0 && this.CollectedAmount >= consumableItemCap;
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x0004EDA0 File Offset: 0x0004CFA0
	public virtual void ConsumeItemResponse()
	{
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			return;
		}
		PlayerData instance2 = PlayerData.instance;
		foreach (CollectableItem.UseResponse useResponse in this.GetUseResponses())
		{
			switch (useResponse.UseType)
			{
			case CollectableItem.UseTypes.Rosaries:
				CurrencyManager.AddCurrency(useResponse.GetAmount(), CurrencyType.Money, true);
				break;
			case CollectableItem.UseTypes.Shards:
				if (CurrencyManager.GetCurrencyAmount(CurrencyType.Shard) < Gameplay.GetCurrencyCap(CurrencyType.Shard))
				{
					CurrencyManager.AddCurrency(useResponse.GetAmount(), CurrencyType.Shard, true);
				}
				else
				{
					CurrencyCounter.ReportFail(CurrencyType.Shard);
				}
				break;
			case CollectableItem.UseTypes.ReturnCocoon:
				if (!string.IsNullOrEmpty(instance2.HeroCorpseScene))
				{
					instance.CocoonBroken(true, true);
					EventRegister.SendEvent(EventRegisterEvents.BreakHeroCorpse, null);
				}
				break;
			case CollectableItem.UseTypes.GetSilk:
				instance.AddSilk(useResponse.GetAmount(), false);
				break;
			}
		}
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x0004EE8C File Offset: 0x0004D08C
	public bool IsConsumeAtMax()
	{
		HeroController instance = HeroController.instance;
		if (!instance)
		{
			return false;
		}
		PlayerData playerData = instance.playerData;
		bool flag = false;
		foreach (CollectableItem.UseResponse useResponse in this.GetUseResponses())
		{
			CollectableItem.UseTypes useType = useResponse.UseType;
			if (useType - CollectableItem.UseTypes.Rosaries > 2)
			{
				if (useType == CollectableItem.UseTypes.GetSilk)
				{
					if (playerData.silk < playerData.CurrentSilkMax)
					{
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		return !flag;
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x0004EF18 File Offset: 0x0004D118
	public override void Get(bool showPopup = true)
	{
		this.Collect(1, showPopup);
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x0004EF22 File Offset: 0x0004D122
	public override bool CanGetMore()
	{
		return this.IsConsumable() || !this.IsAtMax();
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x0004EF37 File Offset: 0x0004D137
	public override Sprite GetQuestCounterSprite(int index)
	{
		return this.GetIcon(CollectableItem.ReadSource.Inventory);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x0004EF40 File Offset: 0x0004D140
	public override float GetUIMsgIconScale()
	{
		return this.popupIconScale;
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x0004EF48 File Offset: 0x0004D148
	public virtual bool IsConsumable()
	{
		return this.HasUseResponse();
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x0004EF50 File Offset: 0x0004D150
	private bool HasUseResponse()
	{
		return this.GetUseResponses().Any((CollectableItem.UseResponse use) => use.UseType > CollectableItem.UseTypes.None);
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x0004EF7C File Offset: 0x0004D17C
	public bool CanConsumeRightNow()
	{
		return this.HasUseResponse() && !this.IsConsumeAtMax() && !GameManager.instance.IsMemoryScene();
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0004EFA0 File Offset: 0x0004D1A0
	public bool ConsumeClosesInventory(bool extraCondition)
	{
		foreach (CollectableItem.UseResponse useResponse in this.GetUseResponses())
		{
			CollectableItem.UseTypes useType = useResponse.UseType;
			if (useType != CollectableItem.UseTypes.ReturnCocoon)
			{
				if (useType == CollectableItem.UseTypes.GetSilk)
				{
					return true;
				}
			}
			else
			{
				PlayerData instance = PlayerData.instance;
				if (!extraCondition || !string.IsNullOrEmpty(instance.HeroCorpseScene))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0004F018 File Offset: 0x0004D218
	public override Sprite GetPopupIcon()
	{
		return this.GetIcon(CollectableItem.ReadSource.GetPopup);
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0004F021 File Offset: 0x0004D221
	public override string GetPopupName()
	{
		return this.GetDisplayName(CollectableItem.ReadSource.GetPopup);
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0004F02A File Offset: 0x0004D22A
	public override int GetSavedAmount()
	{
		return this.CollectedAmount;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x0004F032 File Offset: 0x0004D232
	public string GetCollectionName()
	{
		return this.GetDisplayName(CollectableItem.ReadSource.Shop);
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x0004F03B File Offset: 0x0004D23B
	public string GetCollectionDesc()
	{
		return this.GetDescription(CollectableItem.ReadSource.Shop);
	}

	// Token: 0x0600108F RID: 4239 RVA: 0x0004F044 File Offset: 0x0004D244
	public Sprite GetCollectionIcon()
	{
		return this.GetIcon(CollectableItem.ReadSource.Shop);
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x0004F04D File Offset: 0x0004D24D
	public virtual bool IsVisibleInCollection()
	{
		return this.CollectedAmount > 0;
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x0004F058 File Offset: 0x0004D258
	public bool IsRequiredInCollection()
	{
		return true;
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x0004F05B File Offset: 0x0004D25B
	public virtual void ReportPreviouslyCollected()
	{
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x0004F077 File Offset: 0x0004D277
	string ICollectionViewerItem.get_name()
	{
		return base.name;
	}

	// Token: 0x04000FCE RID: 4046
	[SerializeField]
	private float popupIconScale = 1f;

	// Token: 0x04000FCF RID: 4047
	[Space]
	[SerializeField]
	private CollectableItem.UseResponse[] useResponses;

	// Token: 0x04000FD0 RID: 4048
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString useResponseTextOverride;

	// Token: 0x04000FD1 RID: 4049
	[SerializeField]
	private bool preventUseChaining;

	// Token: 0x04000FD2 RID: 4050
	[SerializeField]
	private AudioEventRandom useSounds;

	// Token: 0x04000FD3 RID: 4051
	[SerializeField]
	private AudioEventRandom instantUseSounds;

	// Token: 0x04000FD4 RID: 4052
	[SerializeField]
	private bool alwaysPlayInstantUse;

	// Token: 0x04000FD5 RID: 4053
	[SerializeField]
	private bool skipBenchUseEffect;

	// Token: 0x04000FD6 RID: 4054
	[SerializeField]
	private GameObject extraUseEffect;

	// Token: 0x04000FD7 RID: 4055
	[Space]
	[SerializeField]
	private CustomInventoryItemCollectableDisplay customInventoryDisplay;

	// Token: 0x04000FD8 RID: 4056
	[SerializeField]
	private GameObject extraDescriptionSection;

	// Token: 0x04000FD9 RID: 4057
	[SerializeField]
	private bool resetIsSeen;

	// Token: 0x04000FDA RID: 4058
	[SerializeField]
	private bool isVisibleWithBareInventory;

	// Token: 0x04000FDB RID: 4059
	[SerializeField]
	private bool isHidden;

	// Token: 0x04000FDC RID: 4060
	[SerializeField]
	private bool hideInShopCounters;

	// Token: 0x04000FDD RID: 4061
	[Space]
	[SerializeField]
	private Quest useQuestForCap;

	// Token: 0x04000FDE RID: 4062
	[SerializeField]
	private int customMaxAmount;

	// Token: 0x04000FDF RID: 4063
	[Space]
	[SerializeField]
	private PlayerStory.EventTypes storyEvent = PlayerStory.EventTypes.None;

	// Token: 0x020014E4 RID: 5348
	public enum ReadSource
	{
		// Token: 0x0400851B RID: 34075
		Inventory,
		// Token: 0x0400851C RID: 34076
		GetPopup,
		// Token: 0x0400851D RID: 34077
		Tiny,
		// Token: 0x0400851E RID: 34078
		Shop,
		// Token: 0x0400851F RID: 34079
		TakePopup
	}

	// Token: 0x020014E5 RID: 5349
	public enum UseTypes
	{
		// Token: 0x04008521 RID: 34081
		None,
		// Token: 0x04008522 RID: 34082
		Rosaries,
		// Token: 0x04008523 RID: 34083
		Shards,
		// Token: 0x04008524 RID: 34084
		ReturnCocoon,
		// Token: 0x04008525 RID: 34085
		GetSilk
	}

	// Token: 0x020014E6 RID: 5350
	[Serializable]
	public struct UseResponse
	{
		// Token: 0x06008524 RID: 34084 RVA: 0x0026EEDD File Offset: 0x0026D0DD
		public int GetAmount()
		{
			if (this.Amount != 0)
			{
				return this.Amount;
			}
			return this.AmountRange.GetRandomValue(true);
		}

		// Token: 0x06008525 RID: 34085 RVA: 0x0026EEFA File Offset: 0x0026D0FA
		public string GetAmountText()
		{
			if (this.Amount != 0)
			{
				return this.Amount.ToString();
			}
			return string.Format("{0}-{1}", this.AmountRange.Start, this.AmountRange.End);
		}

		// Token: 0x06008526 RID: 34086 RVA: 0x0026EF3A File Offset: 0x0026D13A
		private bool UsesAmount()
		{
			return this.UseType != CollectableItem.UseTypes.ReturnCocoon;
		}

		// Token: 0x06008527 RID: 34087 RVA: 0x0026EF48 File Offset: 0x0026D148
		private bool UsesAmountRange()
		{
			return this.UsesAmount() && this.Amount == 0;
		}

		// Token: 0x04008526 RID: 34086
		public CollectableItem.UseTypes UseType;

		// Token: 0x04008527 RID: 34087
		[ModifiableProperty]
		[Conditional("UsesAmount", true, true, false)]
		public int Amount;

		// Token: 0x04008528 RID: 34088
		[ModifiableProperty]
		[Conditional("UsesAmountRange", true, true, false)]
		public MinMaxInt AmountRange;

		// Token: 0x04008529 RID: 34089
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Description;
	}
}
