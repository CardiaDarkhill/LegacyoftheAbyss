using System;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005E5 RID: 1509
public abstract class ToolItem : ToolBase
{
	// Token: 0x170005F4 RID: 1524
	// (get) Token: 0x06003589 RID: 13705 RVA: 0x000ECE4B File Offset: 0x000EB04B
	public ToolItemType Type
	{
		get
		{
			return this.type;
		}
	}

	// Token: 0x170005F5 RID: 1525
	// (get) Token: 0x0600358A RID: 13706 RVA: 0x000ECE53 File Offset: 0x000EB053
	public int BaseStorageAmount
	{
		get
		{
			if (!this.HasLimitedUses())
			{
				return 0;
			}
			return this.baseStorageAmount;
		}
	}

	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x0600358B RID: 13707 RVA: 0x000ECE65 File Offset: 0x000EB065
	public bool PreventStorageIncrease
	{
		get
		{
			return this.preventStorageIncrease;
		}
	}

	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x0600358C RID: 13708 RVA: 0x000ECE6D File Offset: 0x000EB06D
	public ToolItem.ReplenishResources ReplenishResource
	{
		get
		{
			return this.replenishResource;
		}
	}

	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x0600358D RID: 13709 RVA: 0x000ECE75 File Offset: 0x000EB075
	public ToolItem.ReplenishUsages ReplenishUsage
	{
		get
		{
			return this.replenishUsage;
		}
	}

	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x0600358E RID: 13710 RVA: 0x000ECE7D File Offset: 0x000EB07D
	public float ReplenishUsageMultiplier
	{
		get
		{
			return this.replenishUsageMultiplier;
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x0600358F RID: 13711 RVA: 0x000ECE85 File Offset: 0x000EB085
	public bool IsCustomUsage
	{
		get
		{
			return this.isCustomUsage;
		}
	}

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x06003590 RID: 13712 RVA: 0x000ECE8D File Offset: 0x000EB08D
	public ToolDamageFlags DamageFlags
	{
		get
		{
			return this.damageFlags;
		}
	}

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x06003591 RID: 13713 RVA: 0x000ECE95 File Offset: 0x000EB095
	public int PoisonDamageTicks
	{
		get
		{
			return this.poisonDamageTicks;
		}
	}

	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x06003592 RID: 13714 RVA: 0x000ECE9D File Offset: 0x000EB09D
	public bool UsePoisonTintRecolour
	{
		get
		{
			return this.usePoisonTintRecolour;
		}
	}

	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x06003593 RID: 13715 RVA: 0x000ECEA5 File Offset: 0x000EB0A5
	public float PoisonHueShift
	{
		get
		{
			return this.poisonHueShift;
		}
	}

	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x06003594 RID: 13716 RVA: 0x000ECEAD File Offset: 0x000EB0AD
	public int ZapDamageTicks
	{
		get
		{
			return this.zapDamageTicks;
		}
	}

	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x06003595 RID: 13717 RVA: 0x000ECEB5 File Offset: 0x000EB0B5
	public bool ShowPromptHold
	{
		get
		{
			return this.showPromptHold;
		}
	}

	// Token: 0x17000601 RID: 1537
	// (get) Token: 0x06003596 RID: 13718 RVA: 0x000ECEBD File Offset: 0x000EB0BD
	public GameObject ExtraDescriptionSection
	{
		get
		{
			return this.extraDescriptionSection;
		}
	}

	// Token: 0x17000602 RID: 1538
	// (get) Token: 0x06003597 RID: 13719
	public abstract ToolItem.UsageOptions Usage { get; }

	// Token: 0x17000603 RID: 1539
	// (get) Token: 0x06003598 RID: 13720 RVA: 0x000ECEC5 File Offset: 0x000EB0C5
	public virtual bool UsableWhenEmpty
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000604 RID: 1540
	// (get) Token: 0x06003599 RID: 13721 RVA: 0x000ECEC8 File Offset: 0x000EB0C8
	public virtual bool UsableWhenEmptyPrevented
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000605 RID: 1541
	// (get) Token: 0x0600359A RID: 13722 RVA: 0x000ECECB File Offset: 0x000EB0CB
	public virtual bool HideUsePrompt
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000606 RID: 1542
	// (get) Token: 0x0600359B RID: 13723
	public abstract LocalisedString DisplayName { get; }

	// Token: 0x17000607 RID: 1543
	// (get) Token: 0x0600359C RID: 13724
	public abstract LocalisedString Description { get; }

	// Token: 0x17000608 RID: 1544
	// (get) Token: 0x0600359D RID: 13725 RVA: 0x000ECECE File Offset: 0x000EB0CE
	private bool IsUnlockedTest
	{
		get
		{
			return this.alternateUnlockedTest.IsDefined && this.alternateUnlockedTest.IsFulfilled;
		}
	}

	// Token: 0x17000609 RID: 1545
	// (get) Token: 0x0600359E RID: 13726 RVA: 0x000ECEEA File Offset: 0x000EB0EA
	public bool IsUnlocked
	{
		get
		{
			return this.SavedData.IsUnlocked || this.IsUnlockedTest;
		}
	}

	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x0600359F RID: 13727 RVA: 0x000ECF01 File Offset: 0x000EB101
	public bool IsUnlockedNotHidden
	{
		get
		{
			return !this.SavedData.IsHidden && this.IsUnlocked;
		}
	}

	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x060035A0 RID: 13728 RVA: 0x000ECF18 File Offset: 0x000EB118
	// (set) Token: 0x060035A1 RID: 13729 RVA: 0x000ECF28 File Offset: 0x000EB128
	public bool HasBeenSeen
	{
		get
		{
			return this.SavedData.HasBeenSeen;
		}
		set
		{
			ToolItemsData.Data savedData = this.SavedData;
			savedData.HasBeenSeen = value;
			this.SavedData = savedData;
		}
	}

	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x060035A2 RID: 13730 RVA: 0x000ECF4B File Offset: 0x000EB14B
	// (set) Token: 0x060035A3 RID: 13731 RVA: 0x000ECF58 File Offset: 0x000EB158
	public bool HasBeenSelected
	{
		get
		{
			return this.SavedData.HasBeenSelected;
		}
		set
		{
			ToolItemsData.Data savedData = this.SavedData;
			savedData.HasBeenSelected = value;
			this.SavedData = savedData;
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x060035A4 RID: 13732 RVA: 0x000ECF7B File Offset: 0x000EB17B
	public override bool IsEquipped
	{
		get
		{
			return ToolItemManager.IsToolEquipped(this, ToolEquippedReadSource.Active);
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x060035A5 RID: 13733 RVA: 0x000ECF84 File Offset: 0x000EB184
	public bool IsEquippedHud
	{
		get
		{
			return ToolItemManager.IsToolEquipped(this, ToolEquippedReadSource.Hud);
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x060035A6 RID: 13734 RVA: 0x000ECF8D File Offset: 0x000EB18D
	public ToolItemManager.ToolStatus Status
	{
		get
		{
			if (this.status == null)
			{
				this.status = new ToolItemManager.ToolStatus(this);
			}
			return this.status;
		}
	}

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x060035A7 RID: 13735 RVA: 0x000ECFA9 File Offset: 0x000EB1A9
	// (set) Token: 0x060035A8 RID: 13736 RVA: 0x000ECFCC File Offset: 0x000EB1CC
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

	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x060035A9 RID: 13737 RVA: 0x000ECFDC File Offset: 0x000EB1DC
	public bool IsEmpty
	{
		get
		{
			return this.HasLimitedUses() && this.SavedData.AmountLeft <= 0 && this.baseStorageAmount > 0;
		}
	}

	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x060035AA RID: 13738 RVA: 0x000ECFFF File Offset: 0x000EB1FF
	// (set) Token: 0x060035AB RID: 13739 RVA: 0x000ED014 File Offset: 0x000EB214
	public ToolItemsData.Data SavedData
	{
		get
		{
			return PlayerData.instance.GetToolData(this.name);
		}
		set
		{
			ToolItemsData.Data data = value;
			if (!data.IsUnlocked && this.IsUnlockedTest)
			{
				data.IsUnlocked = true;
			}
			PlayerData.instance.SetToolData(this.name, data);
		}
	}

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x060035AC RID: 13740 RVA: 0x000ED04C File Offset: 0x000EB24C
	public bool DisplayAmountText
	{
		get
		{
			int num;
			if (this.type == ToolItemType.Red)
			{
				num = 0;
			}
			else
			{
				num = 1;
			}
			return this.BaseStorageAmount > num;
		}
	}

	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x060035AD RID: 13741 RVA: 0x000ED070 File Offset: 0x000EB270
	public virtual bool DisplayTogglePrompt
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x060035AE RID: 13742 RVA: 0x000ED073 File Offset: 0x000EB273
	public string CustomToggleText
	{
		get
		{
			return this.togglePromptText;
		}
	}

	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x060035AF RID: 13743 RVA: 0x000ED080 File Offset: 0x000EB280
	public bool HasCustomAction
	{
		get
		{
			return this.hasCustomAction && !this.IsAttackType();
		}
	}

	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x060035B0 RID: 13744 RVA: 0x000ED095 File Offset: 0x000EB295
	public InventoryItemComboButtonPromptDisplay.Display CustomButtonCombo
	{
		get
		{
			return this.customButtonCombo;
		}
	}

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x060035B1 RID: 13745 RVA: 0x000ED09D File Offset: 0x000EB29D
	public bool IsCounted
	{
		get
		{
			return this.isCounted;
		}
	}

	// Token: 0x17000619 RID: 1561
	// (get) Token: 0x060035B2 RID: 13746 RVA: 0x000ED0A5 File Offset: 0x000EB2A5
	public SavedItem CountKey
	{
		get
		{
			if (!this.countKey)
			{
				return this;
			}
			return this.countKey;
		}
	}

	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x060035B3 RID: 13747 RVA: 0x000ED0BC File Offset: 0x000EB2BC
	public Sprite InventorySpriteBase
	{
		get
		{
			return this.GetInventorySprite(ToolItem.IconVariants.Default);
		}
	}

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x060035B4 RID: 13748 RVA: 0x000ED0C5 File Offset: 0x000EB2C5
	public Sprite InventorySpriteModified
	{
		get
		{
			return this.GetInventorySprite((this.PoisonDamageTicks > 0 && Gameplay.PoisonPouchTool.IsEquippedHud) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default);
		}
	}

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x060035B5 RID: 13749 RVA: 0x000ED0E6 File Offset: 0x000EB2E6
	public Sprite HudSpriteBase
	{
		get
		{
			return this.GetHudSprite(ToolItem.IconVariants.Default);
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x060035B6 RID: 13750 RVA: 0x000ED0EF File Offset: 0x000EB2EF
	public Sprite HudSpriteModified
	{
		get
		{
			return this.GetHudSprite((this.PoisonDamageTicks > 0 && Gameplay.PoisonPouchTool.IsEquippedHud) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default);
		}
	}

	// Token: 0x060035B7 RID: 13751
	public abstract Sprite GetInventorySprite(ToolItem.IconVariants iconVariant);

	// Token: 0x060035B8 RID: 13752
	public abstract Sprite GetHudSprite(ToolItem.IconVariants iconVariant);

	// Token: 0x060035B9 RID: 13753 RVA: 0x000ED110 File Offset: 0x000EB310
	private bool IsAttackType()
	{
		return this.type.IsAttackType();
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x000ED11D File Offset: 0x000EB31D
	private bool HasLimitedUses()
	{
		return this.type != ToolItemType.Skill;
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x000ED12B File Offset: 0x000EB32B
	public bool IsAutoReplenished()
	{
		return this.HasLimitedUses() && this.BaseStorageAmount > 0;
	}

	// Token: 0x060035BC RID: 13756 RVA: 0x000ED140 File Offset: 0x000EB340
	public bool IsCustomReplenish()
	{
		return this.replenishUsage == ToolItem.ReplenishUsages.Custom;
	}

	// Token: 0x060035BD RID: 13757 RVA: 0x000ED14B File Offset: 0x000EB34B
	public virtual bool TryReplenishSingle(bool doReplenish, float inCost, out float outCost, out int reserveCost)
	{
		outCost = inCost;
		reserveCost = 0;
		return true;
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x000ED155 File Offset: 0x000EB355
	public virtual void OnWasUsed(bool wasEmpty)
	{
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x000ED158 File Offset: 0x000EB358
	public void SetUnlockedTestsComplete()
	{
		if (this.alternateUnlockedTest == null)
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		PlayerDataTest.TestGroup[] testGroups = this.alternateUnlockedTest.TestGroups;
		for (int i = 0; i < testGroups.Length; i++)
		{
			foreach (PlayerDataTest.Test test in testGroups[i].Tests)
			{
				if (test.Type == PlayerDataTest.TestType.Bool)
				{
					instance.SetVariable(test.FieldName, test.BoolValue);
				}
			}
		}
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x000ED1D4 File Offset: 0x000EB3D4
	public void Unlock(Action afterTutorialMsg = null, ToolItem.PopupFlags popupFlags = ToolItem.PopupFlags.Default)
	{
		this.OnUnlocked();
		bool flag = (popupFlags & ToolItem.PopupFlags.ItemGet) > ToolItem.PopupFlags.None;
		bool flag2 = (popupFlags & ToolItem.PopupFlags.Tutorial) > ToolItem.PopupFlags.None;
		PlayerData instance = PlayerData.instance;
		bool flag3;
		if (this.IsUnlocked)
		{
			ToolItemsData.Data savedData = this.SavedData;
			savedData.AmountLeft = ToolItemManager.GetToolStorageAmount(this);
			flag3 = savedData.IsHidden;
			savedData.IsHidden = false;
			this.SavedData = savedData;
			AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
			if (attackToolBinding != null)
			{
				ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
			}
			if (flag)
			{
				this.ShowRefillMsg();
			}
		}
		else
		{
			this.SavedData = new ToolItemsData.Data
			{
				IsUnlocked = true,
				AmountLeft = ((this.unlockStartAmount >= 0) ? this.unlockStartAmount : ToolItemManager.GetToolStorageAmount(this))
			};
			bool flag4 = afterTutorialMsg == null;
			afterTutorialMsg = (Action)Delegate.Combine(afterTutorialMsg, new Action(delegate()
			{
				ToolItemManager.ReportToolUnlocked(this.type);
			}));
			if (flag2 && !this.preventTutorialMsg && (!instance.SeenToolGetPrompt || (this.type == ToolItemType.Red && !instance.SeenToolWeaponGetPrompt)))
			{
				if (flag4)
				{
					afterTutorialMsg = (Action)Delegate.Combine(afterTutorialMsg, new Action(delegate()
					{
						GameCameras.instance.HUDIn();
					}));
				}
				ToolTutorialMsg.Spawn(this, afterTutorialMsg);
				instance.SeenToolGetPrompt = true;
				if (this.type == ToolItemType.Red)
				{
					instance.SeenToolWeaponGetPrompt = true;
				}
				afterTutorialMsg = null;
			}
			flag3 = true;
			if (this.getReplaces)
			{
				ToolItemManager.ReplaceToolEquips(this.getReplaces, this);
				this.getReplaces.Lock();
			}
		}
		this.SetHasNew(flag3);
		if (flag3 && flag)
		{
			CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this, null, false);
			CollectableItemHeroReaction.DoReaction();
			QuestManager.MaybeShowQuestUpdated(this, itemUiMsg);
		}
		if (afterTutorialMsg != null)
		{
			afterTutorialMsg();
		}
	}

	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x060035C1 RID: 13761 RVA: 0x000ED379 File Offset: 0x000EB579
	public override bool CanConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060035C2 RID: 13762 RVA: 0x000ED37C File Offset: 0x000EB57C
	public override void Consume(int amount, bool showCounter)
	{
		if (amount > 0)
		{
			this.Lock();
		}
	}

	// Token: 0x060035C3 RID: 13763 RVA: 0x000ED388 File Offset: 0x000EB588
	public void Lock()
	{
		ToolItemsData.Data savedData = this.SavedData;
		savedData.IsHidden = true;
		this.SavedData = savedData;
		ToolItemManager.RemoveToolFromAllCrests(this);
	}

	// Token: 0x060035C4 RID: 13764 RVA: 0x000ED3B4 File Offset: 0x000EB5B4
	protected void ShowRefillMsg()
	{
		if (this.refillMsg.IsEmpty)
		{
			return;
		}
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = this.refillMsg,
			Icon = base.GetUIMsgSprite(),
			IconScale = this.GetUIMsgIconScale(),
			RepresentingObject = this
		}, null, false);
	}

	// Token: 0x060035C5 RID: 13765 RVA: 0x000ED419 File Offset: 0x000EB619
	protected virtual Sprite GetFullIcon()
	{
		return base.GetUIMsgSprite();
	}

	// Token: 0x060035C6 RID: 13766 RVA: 0x000ED424 File Offset: 0x000EB624
	public void ShowRefillMsgFull()
	{
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = this.refillMsg,
			Icon = this.GetFullIcon(),
			IconScale = this.GetUIMsgIconScale(),
			RepresentingObject = this
		}, null, false);
	}

	// Token: 0x060035C7 RID: 13767 RVA: 0x000ED47B File Offset: 0x000EB67B
	public override void SetHasNew(bool hasPopup)
	{
		if (hasPopup)
		{
			ToolItemManager.UnlockedTool = this;
			InventoryPaneList.SetNextOpen("Tools");
		}
		PlayerData.instance.ToolPaneHasNew = true;
	}

	// Token: 0x060035C8 RID: 13768 RVA: 0x000ED49B File Offset: 0x000EB69B
	protected virtual void OnUnlocked()
	{
	}

	// Token: 0x060035C9 RID: 13769 RVA: 0x000ED49D File Offset: 0x000EB69D
	public override void Get(bool showPopup = true)
	{
		this.Unlock(null, showPopup ? ToolItem.PopupFlags.Default : ToolItem.PopupFlags.None);
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x000ED4AD File Offset: 0x000EB6AD
	public override bool CanGetMore()
	{
		return !this.IsUnlocked;
	}

	// Token: 0x060035CB RID: 13771 RVA: 0x000ED4B8 File Offset: 0x000EB6B8
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		if (!this.IsUnlocked)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060035CC RID: 13772 RVA: 0x000ED4C8 File Offset: 0x000EB6C8
	public void CollectFree(int amount)
	{
		if (!this.IsUnlocked)
		{
			Debug.LogError("Trying to replenish a tool that is not unlocked!", this);
			return;
		}
		ToolItemsData.Data savedData = this.SavedData;
		int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
		int num = savedData.AmountLeft + amount;
		if (num > toolStorageAmount)
		{
			num = toolStorageAmount;
		}
		if (num != savedData.AmountLeft)
		{
			savedData.AmountLeft = num;
			this.SavedData = savedData;
			AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
			if (attackToolBinding != null)
			{
				ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
			}
		}
	}

	// Token: 0x060035CD RID: 13773 RVA: 0x000ED53C File Offset: 0x000EB73C
	public void CustomUsage(int amount)
	{
		ToolItemsData.Data savedData = this.SavedData;
		if (savedData.AmountLeft <= 0)
		{
			return;
		}
		savedData.AmountLeft -= amount;
		if (savedData.AmountLeft < 0)
		{
			savedData.AmountLeft = 0;
		}
		this.SavedData = savedData;
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
		if (attackToolBinding == null)
		{
			return;
		}
		ToolItemManager.ReportBoundAttackToolUsed(attackToolBinding.Value);
		ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
		ToolItemLimiter.ReportToolUsed(this);
	}

	// Token: 0x060035CE RID: 13774 RVA: 0x000ED5AD File Offset: 0x000EB7AD
	public bool CanReload()
	{
		return this.SavedData.AmountLeft < ToolItemManager.GetToolStorageAmount(this) && (this.ReplenishResource == ToolItem.ReplenishResources.None || CurrencyManager.GetCurrencyAmount((CurrencyType)this.ReplenishResource) >= 1);
	}

	// Token: 0x060035CF RID: 13775 RVA: 0x000ED5E0 File Offset: 0x000EB7E0
	public void ReloadSingle()
	{
		ToolItemsData.Data savedData = this.SavedData;
		savedData.AmountLeft++;
		int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
		if (savedData.AmountLeft > toolStorageAmount)
		{
			savedData.AmountLeft = toolStorageAmount;
		}
		this.SavedData = savedData;
		if (this.replenishResource == ToolItem.ReplenishResources.None)
		{
			return;
		}
		CurrencyManager.TakeCurrency(1, (CurrencyType)this.ReplenishResource, true);
	}

	// Token: 0x060035D0 RID: 13776 RVA: 0x000ED636 File Offset: 0x000EB836
	public void StartedReloading(AudioSource audioSource)
	{
		audioSource.clip = this.reloadAudioLoop;
		audioSource.loop = true;
		audioSource.Play();
	}

	// Token: 0x060035D1 RID: 13777 RVA: 0x000ED651 File Offset: 0x000EB851
	public void StoppedReloading(AudioSource audioSource, bool didFinish)
	{
		audioSource.Stop();
		audioSource.loop = false;
		if (didFinish)
		{
			this.reloadEndAudio.PlayOnSource(audioSource);
		}
	}

	// Token: 0x060035D2 RID: 13778 RVA: 0x000ED66F File Offset: 0x000EB86F
	public override Sprite GetPopupIcon()
	{
		return this.GetInventorySprite(ToolItem.IconVariants.Default);
	}

	// Token: 0x060035D3 RID: 13779 RVA: 0x000ED678 File Offset: 0x000EB878
	public override string GetPopupName()
	{
		return this.DisplayName;
	}

	// Token: 0x060035D4 RID: 13780 RVA: 0x000ED685 File Offset: 0x000EB885
	public override int GetSavedAmount()
	{
		if (!this.IsUnlocked)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x060035D5 RID: 13781 RVA: 0x000ED692 File Offset: 0x000EB892
	public virtual bool CanToggle
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060035D6 RID: 13782 RVA: 0x000ED695 File Offset: 0x000EB895
	public virtual bool DoToggle(out bool didChangeVisually)
	{
		didChangeVisually = false;
		return false;
	}

	// Token: 0x060035D7 RID: 13783 RVA: 0x000ED69B File Offset: 0x000EB89B
	public virtual void PlayToggleAudio(AudioSource audioSource)
	{
	}

	// Token: 0x040038E1 RID: 14561
	[SerializeField]
	[Tooltip("Is this counted in the total tools achievement count?")]
	private bool isCounted = true;

	// Token: 0x040038E2 RID: 14562
	[SerializeField]
	[Tooltip("Tools sharing a key will be counted as 1 in the achievement count.")]
	private SavedItem countKey;

	// Token: 0x040038E3 RID: 14563
	[SerializeField]
	private ToolItem getReplaces;

	// Token: 0x040038E4 RID: 14564
	[Space]
	[SerializeField]
	private ToolItemType type;

	// Token: 0x040038E5 RID: 14565
	[SerializeField]
	private PlayerDataTest alternateUnlockedTest;

	// Token: 0x040038E6 RID: 14566
	[SerializeField]
	private bool preventTutorialMsg;

	// Token: 0x040038E7 RID: 14567
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasLimitedUses", true, true, false)]
	private int baseStorageAmount;

	// Token: 0x040038E8 RID: 14568
	[SerializeField]
	private int unlockStartAmount = -1;

	// Token: 0x040038E9 RID: 14569
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasLimitedUses", true, true, false)]
	private bool preventStorageIncrease;

	// Token: 0x040038EA RID: 14570
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsAutoReplenished", true, true, false)]
	private ToolItem.ReplenishResources replenishResource = ToolItem.ReplenishResources.Shard;

	// Token: 0x040038EB RID: 14571
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsAutoReplenished", true, true, false)]
	private ToolItem.ReplenishUsages replenishUsage;

	// Token: 0x040038EC RID: 14572
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsCustomReplenish", false, true, false)]
	private float replenishUsageMultiplier = 1f;

	// Token: 0x040038ED RID: 14573
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasLimitedUses", true, true, false)]
	private bool isCustomUsage;

	// Token: 0x040038EE RID: 14574
	[Space]
	[SerializeField]
	private AudioClip reloadAudioLoop;

	// Token: 0x040038EF RID: 14575
	[SerializeField]
	private AudioEvent reloadEndAudio;

	// Token: 0x040038F0 RID: 14576
	[Space]
	[SerializeField]
	private LocalisedString togglePromptText;

	// Token: 0x040038F1 RID: 14577
	[Space]
	[SerializeField]
	[EnumPickerBitmask]
	private ToolDamageFlags damageFlags;

	// Token: 0x040038F2 RID: 14578
	[SerializeField]
	private int poisonDamageTicks;

	// Token: 0x040038F3 RID: 14579
	[SerializeField]
	private bool usePoisonTintRecolour;

	// Token: 0x040038F4 RID: 14580
	[SerializeField]
	[Range(-1f, 1f)]
	private float poisonHueShift;

	// Token: 0x040038F5 RID: 14581
	[SerializeField]
	private int zapDamageTicks;

	// Token: 0x040038F6 RID: 14582
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsAttackType", false, true, false)]
	private bool hasCustomAction;

	// Token: 0x040038F7 RID: 14583
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsAttackType", false, true, false)]
	private InventoryItemComboButtonPromptDisplay.Display customButtonCombo;

	// Token: 0x040038F8 RID: 14584
	[SerializeField]
	private bool showPromptHold;

	// Token: 0x040038F9 RID: 14585
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString refillMsg;

	// Token: 0x040038FA RID: 14586
	[Space]
	[SerializeField]
	private GameObject extraDescriptionSection;

	// Token: 0x040038FB RID: 14587
	private ToolItemManager.ToolStatus status;

	// Token: 0x040038FC RID: 14588
	private bool cachedName;

	// Token: 0x040038FD RID: 14589
	private string nameCache;

	// Token: 0x020018EB RID: 6379
	public enum ThrowAnimType
	{
		// Token: 0x040093B8 RID: 37816
		Default,
		// Token: 0x040093B9 RID: 37817
		Up,
		// Token: 0x040093BA RID: 37818
		Down
	}

	// Token: 0x020018EC RID: 6380
	public enum ReplenishResources
	{
		// Token: 0x040093BC RID: 37820
		None = -1,
		// Token: 0x040093BD RID: 37821
		Money,
		// Token: 0x040093BE RID: 37822
		Shard
	}

	// Token: 0x020018ED RID: 6381
	public enum ReplenishUsages
	{
		// Token: 0x040093C0 RID: 37824
		Percentage,
		// Token: 0x040093C1 RID: 37825
		OneForOne,
		// Token: 0x040093C2 RID: 37826
		Custom
	}

	// Token: 0x020018EE RID: 6382
	[Flags]
	public enum PopupFlags
	{
		// Token: 0x040093C4 RID: 37828
		None = 0,
		// Token: 0x040093C5 RID: 37829
		ItemGet = 1,
		// Token: 0x040093C6 RID: 37830
		Tutorial = 2,
		// Token: 0x040093C7 RID: 37831
		Default = 3
	}

	// Token: 0x020018EF RID: 6383
	public enum IconVariants
	{
		// Token: 0x040093C9 RID: 37833
		Default,
		// Token: 0x040093CA RID: 37834
		Poison
	}

	// Token: 0x020018F0 RID: 6384
	[Serializable]
	public struct UsageOptions
	{
		// Token: 0x17001050 RID: 4176
		// (get) Token: 0x060092B1 RID: 37553 RVA: 0x0029C6F0 File Offset: 0x0029A8F0
		public int ThrowAnimVerticalDirection
		{
			get
			{
				if (this.ThrowAnim != ToolItem.ThrowAnimType.Up)
				{
					return 0;
				}
				return 1;
			}
		}

		// Token: 0x040093CB RID: 37835
		public GameObject ThrowPrefab;

		// Token: 0x040093CC RID: 37836
		public bool UseAltForQuickSling;

		// Token: 0x040093CD RID: 37837
		public float ThrowCooldown;

		// Token: 0x040093CE RID: 37838
		public ToolItem.ThrowAnimType ThrowAnim;

		// Token: 0x040093CF RID: 37839
		public Vector2 ThrowVelocity;

		// Token: 0x040093D0 RID: 37840
		public Vector2 ThrowVelocityAlt;

		// Token: 0x040093D1 RID: 37841
		public Vector2 ThrowOffset;

		// Token: 0x040093D2 RID: 37842
		public Vector2 ThrowOffsetAlt;

		// Token: 0x040093D3 RID: 37843
		public bool ScaleToHero;

		// Token: 0x040093D4 RID: 37844
		public bool FlipScale;

		// Token: 0x040093D5 RID: 37845
		public bool SetDamageDirection;

		// Token: 0x040093D6 RID: 37846
		public string FsmEventName;

		// Token: 0x040093D7 RID: 37847
		public bool IsNonBlockingEvent;

		// Token: 0x040093D8 RID: 37848
		public int SilkRequired;

		// Token: 0x040093D9 RID: 37849
		public int MaxActive;

		// Token: 0x040093DA RID: 37850
		public int MaxActiveAlt;
	}
}
