using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005DF RID: 1503
[CreateAssetMenu(fileName = "New Crest", menuName = "Hornet/Tool Crest")]
public class ToolCrest : ToolBase
{
	// Token: 0x170005DB RID: 1499
	// (get) Token: 0x06003547 RID: 13639 RVA: 0x000EC344 File Offset: 0x000EA544
	public LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x170005DC RID: 1500
	// (get) Token: 0x06003548 RID: 13640 RVA: 0x000EC34C File Offset: 0x000EA54C
	public LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x170005DD RID: 1501
	// (get) Token: 0x06003549 RID: 13641 RVA: 0x000EC354 File Offset: 0x000EA554
	public LocalisedString ItemNamePrefix
	{
		get
		{
			return this.itemNamePrefix;
		}
	}

	// Token: 0x170005DE RID: 1502
	// (get) Token: 0x0600354A RID: 13642 RVA: 0x000EC35C File Offset: 0x000EA55C
	public LocalisedString GetPromptDesc
	{
		get
		{
			return this.getPromptDesc;
		}
	}

	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x0600354B RID: 13643 RVA: 0x000EC364 File Offset: 0x000EA564
	public LocalisedString EquipText
	{
		get
		{
			return this.equipText;
		}
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x0600354C RID: 13644 RVA: 0x000EC36C File Offset: 0x000EA56C
	public Sprite CrestSprite
	{
		get
		{
			return this.crestSprite;
		}
	}

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x0600354D RID: 13645 RVA: 0x000EC374 File Offset: 0x000EA574
	public Sprite CrestSilhouette
	{
		get
		{
			return this.crestSilhouette;
		}
	}

	// Token: 0x170005E2 RID: 1506
	// (get) Token: 0x0600354E RID: 13646 RVA: 0x000EC37C File Offset: 0x000EA57C
	public Sprite CrestGlow
	{
		get
		{
			return this.crestGlow;
		}
	}

	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x0600354F RID: 13647 RVA: 0x000EC384 File Offset: 0x000EA584
	public bool IsHidden
	{
		get
		{
			return this.isHidden;
		}
	}

	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x06003550 RID: 13648 RVA: 0x000EC38C File Offset: 0x000EA58C
	public GameObject DisplayPrefab
	{
		get
		{
			return this.displayPrefab;
		}
	}

	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003551 RID: 13649 RVA: 0x000EC394 File Offset: 0x000EA594
	public ToolCrest.SlotInfo[] Slots
	{
		get
		{
			return this.slots;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003552 RID: 13650 RVA: 0x000EC39C File Offset: 0x000EA59C
	public bool HasCustomAction
	{
		get
		{
			return this.hasCustomAction;
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003553 RID: 13651 RVA: 0x000EC3A4 File Offset: 0x000EA5A4
	public InventoryItemComboButtonPromptDisplay.Display CustomButtonCombo
	{
		get
		{
			return this.customButtonCombo;
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003554 RID: 13652 RVA: 0x000EC3AC File Offset: 0x000EA5AC
	public HeroControllerConfig HeroConfig
	{
		get
		{
			return this.heroConfig;
		}
	}

	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003555 RID: 13653 RVA: 0x000EC3B4 File Offset: 0x000EA5B4
	// (set) Token: 0x06003556 RID: 13654 RVA: 0x000EC3CB File Offset: 0x000EA5CB
	public ToolCrestsData.Data SaveData
	{
		get
		{
			return PlayerData.instance.ToolEquips.GetData(this.name);
		}
		set
		{
			PlayerData.instance.ToolEquips.SetData(this.name, value);
		}
	}

	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003557 RID: 13655 RVA: 0x000EC3E3 File Offset: 0x000EA5E3
	public bool IsUnlocked
	{
		get
		{
			return this.SaveData.IsUnlocked;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x06003558 RID: 13656 RVA: 0x000EC3F0 File Offset: 0x000EA5F0
	public bool IsUpgradedVersionUnlocked
	{
		get
		{
			return this.upgradedVersion && this.upgradedVersion.IsUnlocked;
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x06003559 RID: 13657 RVA: 0x000EC40C File Offset: 0x000EA60C
	public bool IsBaseVersion
	{
		get
		{
			return !this.previousVersion;
		}
	}

	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x0600355A RID: 13658 RVA: 0x000EC41C File Offset: 0x000EA61C
	public bool IsVisible
	{
		get
		{
			return !this.IsUpgradedVersionUnlocked && this.IsUnlocked && (!this.IsHidden || this.IsEquipped);
		}
	}

	// Token: 0x170005EE RID: 1518
	// (get) Token: 0x0600355B RID: 13659 RVA: 0x000EC442 File Offset: 0x000EA642
	public override bool IsEquipped
	{
		get
		{
			return PlayerData.instance.CurrentCrestID == this.name;
		}
	}

	// Token: 0x170005EF RID: 1519
	// (get) Token: 0x0600355C RID: 13660 RVA: 0x000EC459 File Offset: 0x000EA659
	// (set) Token: 0x0600355D RID: 13661 RVA: 0x000EC47C File Offset: 0x000EA67C
	public new string name
	{
		get
		{
			if (!this.cachedName)
			{
				this.nameCache = base.name;
				this.cachedName = true;
			}
			return this.nameCache;
		}
		set
		{
			this.nameCache = value;
			base.name = value;
		}
	}

	// Token: 0x0600355E RID: 13662 RVA: 0x000EC48C File Offset: 0x000EA68C
	private void OnValidate()
	{
		if (this.oldPreviousVersion && this.oldPreviousVersion.upgradedVersion == this)
		{
			this.oldPreviousVersion.upgradedVersion = null;
		}
		if (this.previousVersion)
		{
			this.previousVersion.upgradedVersion = this;
		}
		this.oldPreviousVersion = this.previousVersion;
	}

	// Token: 0x0600355F RID: 13663 RVA: 0x000EC4EA File Offset: 0x000EA6EA
	private void OnEnable()
	{
		this.OnValidate();
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x000EC4F4 File Offset: 0x000EA6F4
	public void Unlock()
	{
		if (this.IsUnlocked)
		{
			return;
		}
		if (this.previousVersion)
		{
			if (!this.previousVersion.IsUnlocked)
			{
				this.previousVersion.Unlock();
			}
			ToolCrestsData.Data saveData = default(ToolCrestsData.Data);
			saveData.IsUnlocked = true;
			List<ToolCrestsData.SlotData> list = this.previousVersion.SaveData.Slots;
			saveData.Slots = ((list != null) ? list.ToList<ToolCrestsData.SlotData>() : null);
			saveData.DisplayNewIndicator = true;
			this.SaveData = saveData;
			if (PlayerData.instance.CurrentCrestID == this.previousVersion.name)
			{
				ToolItemManager.SetEquippedCrest(this.name);
			}
		}
		else
		{
			ToolCrestsData.Data saveData = default(ToolCrestsData.Data);
			saveData.IsUnlocked = true;
			saveData.Slots = this.slots.Select((ToolCrest.SlotInfo slotInfo, int _) => new ToolCrestsData.SlotData
			{
				IsUnlocked = !slotInfo.IsLocked
			}).ToList<ToolCrestsData.SlotData>();
			saveData.DisplayNewIndicator = true;
			this.SaveData = saveData;
		}
		ToolItemManager.ReportCrestUnlocked(this.IsBaseVersion);
		InventoryPaneList.SetNextOpen("Tools");
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x000EC607 File Offset: 0x000EA807
	public override void Get(bool showPopup = true)
	{
		this.Unlock();
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x000EC60F File Offset: 0x000EA80F
	public override bool CanGetMore()
	{
		return !this.IsUnlocked;
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x000EC61A File Offset: 0x000EA81A
	public override Sprite GetPopupIcon()
	{
		return this.CrestSprite;
	}

	// Token: 0x040038B5 RID: 14517
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x040038B6 RID: 14518
	[SerializeField]
	private LocalisedString description;

	// Token: 0x040038B7 RID: 14519
	[Space]
	[SerializeField]
	private LocalisedString itemNamePrefix;

	// Token: 0x040038B8 RID: 14520
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString getPromptDesc;

	// Token: 0x040038B9 RID: 14521
	[SerializeField]
	private LocalisedString equipText;

	// Token: 0x040038BA RID: 14522
	[Space]
	[SerializeField]
	private Sprite crestSprite;

	// Token: 0x040038BB RID: 14523
	[SerializeField]
	private Sprite crestSilhouette;

	// Token: 0x040038BC RID: 14524
	[SerializeField]
	private Sprite crestGlow;

	// Token: 0x040038BD RID: 14525
	[Space]
	[SerializeField]
	private bool isHidden;

	// Token: 0x040038BE RID: 14526
	[SerializeField]
	private GameObject displayPrefab;

	// Token: 0x040038BF RID: 14527
	[Space]
	[SerializeField]
	private ToolCrest.SlotInfo[] slots;

	// Token: 0x040038C0 RID: 14528
	[Space]
	[SerializeField]
	private bool hasCustomAction;

	// Token: 0x040038C1 RID: 14529
	[SerializeField]
	private InventoryItemComboButtonPromptDisplay.Display customButtonCombo;

	// Token: 0x040038C2 RID: 14530
	[Space]
	[SerializeField]
	private HeroControllerConfig heroConfig;

	// Token: 0x040038C3 RID: 14531
	[Space]
	[SerializeField]
	private ToolCrest previousVersion;

	// Token: 0x040038C4 RID: 14532
	[NonSerialized]
	private ToolCrest oldPreviousVersion;

	// Token: 0x040038C5 RID: 14533
	[NonSerialized]
	private ToolCrest upgradedVersion;

	// Token: 0x040038C6 RID: 14534
	private bool cachedName;

	// Token: 0x040038C7 RID: 14535
	private string nameCache;

	// Token: 0x020018E5 RID: 6373
	[Serializable]
	public struct SlotInfo
	{
		// Token: 0x060092A6 RID: 37542 RVA: 0x0029C61D File Offset: 0x0029A81D
		private bool IsAttackType()
		{
			return this.Type.IsAttackType();
		}

		// Token: 0x0400939F RID: 37791
		public Vector2 Position;

		// Token: 0x040093A0 RID: 37792
		public ToolItemType Type;

		// Token: 0x040093A1 RID: 37793
		[ModifiableProperty]
		[Conditional("IsAttackType", true, true, true)]
		public AttackToolBinding AttackBinding;

		// Token: 0x040093A2 RID: 37794
		[Space]
		public int NavUpIndex;

		// Token: 0x040093A3 RID: 37795
		public int NavDownIndex;

		// Token: 0x040093A4 RID: 37796
		public int NavLeftIndex;

		// Token: 0x040093A5 RID: 37797
		public int NavRightIndex;

		// Token: 0x040093A6 RID: 37798
		[Space]
		public int NavUpFallbackIndex;

		// Token: 0x040093A7 RID: 37799
		public int NavDownFallbackIndex;

		// Token: 0x040093A8 RID: 37800
		public int NavLeftFallbackIndex;

		// Token: 0x040093A9 RID: 37801
		public int NavRightFallbackIndex;

		// Token: 0x040093AA RID: 37802
		[Space]
		public bool IsLocked;
	}
}
