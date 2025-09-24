using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006A8 RID: 1704
public class InventoryItemToolManager : InventoryItemListManager<InventoryItemTool, ToolItem>, IInventoryPaneAvailabilityProvider
{
	// Token: 0x170006F6 RID: 1782
	// (get) Token: 0x06003CE6 RID: 15590 RVA: 0x0010B62B File Offset: 0x0010982B
	// (set) Token: 0x06003CE7 RID: 15591 RVA: 0x0010B633 File Offset: 0x00109833
	public InventoryItemTool HoveringTool { get; private set; }

	// Token: 0x170006F7 RID: 1783
	// (get) Token: 0x06003CE8 RID: 15592 RVA: 0x0010B63C File Offset: 0x0010983C
	// (set) Token: 0x06003CE9 RID: 15593 RVA: 0x0010B644 File Offset: 0x00109844
	public InventoryItemToolManager.EquipStates EquipState
	{
		get
		{
			return this.equipState;
		}
		private set
		{
			this.equipState = value;
			this.UpdateButtonPrompts();
			this.paneList.CanSwitchPanes = (value != InventoryItemToolManager.EquipStates.SwitchCrest);
			this.paneList.InSubMenu = (value > InventoryItemToolManager.EquipStates.None);
		}
	}

	// Token: 0x170006F8 RID: 1784
	// (get) Token: 0x06003CEA RID: 15594 RVA: 0x0010B674 File Offset: 0x00109874
	// (set) Token: 0x06003CEB RID: 15595 RVA: 0x0010B67C File Offset: 0x0010987C
	public bool ShowingToolMsg { get; private set; }

	// Token: 0x170006F9 RID: 1785
	// (get) Token: 0x06003CEC RID: 15596 RVA: 0x0010B685 File Offset: 0x00109885
	// (set) Token: 0x06003CED RID: 15597 RVA: 0x0010B68D File Offset: 0x0010988D
	public bool ShowingCrestMsg { get; private set; }

	// Token: 0x170006FA RID: 1786
	// (get) Token: 0x06003CEE RID: 15598 RVA: 0x0010B696 File Offset: 0x00109896
	// (set) Token: 0x06003CEF RID: 15599 RVA: 0x0010B69E File Offset: 0x0010989E
	public bool ShowingCursedMsg { get; private set; }

	// Token: 0x170006FB RID: 1787
	// (get) Token: 0x06003CF0 RID: 15600 RVA: 0x0010B6A7 File Offset: 0x001098A7
	public bool IsHoldingTool
	{
		get
		{
			return this.PickedUpTool != null;
		}
	}

	// Token: 0x170006FC RID: 1788
	// (get) Token: 0x06003CF1 RID: 15601 RVA: 0x0010B6B5 File Offset: 0x001098B5
	// (set) Token: 0x06003CF2 RID: 15602 RVA: 0x0010B6BD File Offset: 0x001098BD
	public ToolItem PickedUpTool { get; private set; }

	// Token: 0x170006FD RID: 1789
	// (get) Token: 0x06003CF3 RID: 15603 RVA: 0x0010B6C6 File Offset: 0x001098C6
	// (set) Token: 0x06003CF4 RID: 15604 RVA: 0x0010B6CE File Offset: 0x001098CE
	public InventoryToolCrestSlot SelectedSlot { get; private set; }

	// Token: 0x170006FE RID: 1790
	// (get) Token: 0x06003CF5 RID: 15605 RVA: 0x0010B6D7 File Offset: 0x001098D7
	public CollectableItem SlotUnlockItem
	{
		get
		{
			return this.slotUnlockItem;
		}
	}

	// Token: 0x170006FF RID: 1791
	// (get) Token: 0x06003CF6 RID: 15606 RVA: 0x0010B6DF File Offset: 0x001098DF
	public bool CanUnlockSlot
	{
		get
		{
			return this.slotUnlockItem.CollectedAmount > 0;
		}
	}

	// Token: 0x17000700 RID: 1792
	// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x0010B6EF File Offset: 0x001098EF
	public InventoryItemCollectable SlotUnlockItemDisplay
	{
		get
		{
			return this.slotUnlockItemDisplay;
		}
	}

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x06003CF8 RID: 15608 RVA: 0x0010B6F7 File Offset: 0x001098F7
	public CrestSocketUnlockInventoryDescription SocketUnlockInventoryDescription
	{
		get
		{
			return this.slotUnlockDescExtra;
		}
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x0010B6FF File Offset: 0x001098FF
	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize<NestedFadeGroupSpriteRenderer>(ref this.listSectionHeaders, typeof(ToolItemType));
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.reloadCurrencyCounters, typeof(CurrencyType));
	}

	// Token: 0x06003CFA RID: 15610 RVA: 0x0010B734 File Offset: 0x00109934
	protected override void Awake()
	{
		this.pane = base.GetComponent<InventoryPane>();
		base.Awake();
		this.OnValidate();
		if (this.toolAmountText)
		{
			this.initialToolAmountText = this.toolAmountText.text;
		}
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		if (this.pane)
		{
			this.pane.OnPaneEnd += delegate()
			{
				if (this.tweenTool)
				{
					this.tweenTool.Cancel();
				}
				this.HideEquipMsgsInstant();
				this.EndSwitchingCrest();
				this.EquipState = InventoryItemToolManager.EquipStates.None;
				if (this.crestGroup)
				{
					this.crestGroup.AlphaSelf = 0f;
				}
				if (this.toolGroup)
				{
					this.toolGroup.AlphaSelf = 1f;
				}
				this.PickedUpTool = null;
				this.selectedBeforePickup = null;
				this.SelectedSlot = null;
				base.GetSelectables(null).ForEach(delegate(InventoryItemTool tool)
				{
					tool.ItemData.HasBeenSeen = true;
				});
			};
			this.pane.OnPaneStart += this.UpdateTextDisplays;
		}
		this.UpdateTextDisplays();
		this.SetToolUsePrompt(null, false, 0f);
	}

	// Token: 0x06003CFB RID: 15611 RVA: 0x0010B7DC File Offset: 0x001099DC
	public override void InstantScroll()
	{
		if (!base.CurrentSelected)
		{
			ToolItem unlockedTool = ToolItemManager.UnlockedTool;
			InventoryItemSelectable startSelectable = this.GetStartSelectable();
			ToolItemManager.UnlockedTool = unlockedTool;
			if (startSelectable != null)
			{
				if (!startSelectable.transform.IsChildOf(base.ItemList.transform))
				{
					return;
				}
				base.ItemList.ScrollTo(startSelectable, true);
			}
			return;
		}
		if (!base.CurrentSelected.transform.IsChildOf(base.ItemList.transform))
		{
			return;
		}
		base.ItemList.ScrollTo(base.CurrentSelected, true);
	}

	// Token: 0x06003CFC RID: 15612 RVA: 0x0010B868 File Offset: 0x00109A68
	private void Start()
	{
		this.EquipState = InventoryItemToolManager.EquipStates.None;
		if (this.toolEquipMsg)
		{
			this.toolEquipMsg.gameObject.SetActive(true);
			this.toolEquipMsg.AlphaSelf = 0f;
		}
		if (this.crestEquipMsg)
		{
			this.crestEquipMsg.gameObject.SetActive(true);
			this.crestEquipMsg.AlphaSelf = 0f;
		}
		if (this.cursedEquipMsg)
		{
			this.cursedEquipMsg.gameObject.SetActive(true);
			this.cursedEquipMsg.AlphaSelf = 0f;
		}
	}

	// Token: 0x06003CFD RID: 15613 RVA: 0x0010B908 File Offset: 0x00109B08
	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if (this.displayIcon)
		{
			this.displayIcon.gameObject.SetActive(false);
		}
		this.HideEquipMsgs(true);
		this.SetToolUsePrompt(null, false, 0f);
		if (this.toolAmountText)
		{
			this.toolAmountText.gameObject.SetActive(false);
		}
		this.descriptionIconGroup.SetActive(true);
		this.slotUnlockDescExtra.gameObject.SetActive(false);
		this.showEquipPrompt = false;
		this.showReloadPrompt = false;
		this.showCustomTogglePrompt = false;
		this.UpdateButtonPrompts();
		this.reloadCurrencyCounters.SetAllActive(false);
		this.currencyParent.gameObject.SetActive(false);
	}

	// Token: 0x06003CFE RID: 15614 RVA: 0x0010B9CC File Offset: 0x00109BCC
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		ToolItem toolItem = null;
		bool flag = true;
		InventoryItemTool inventoryItemTool = selectable as InventoryItemTool;
		if (inventoryItemTool != null)
		{
			toolItem = inventoryItemTool.ItemData;
			flag = this.CrestHasSlot(toolItem.Type);
		}
		InventoryItemToolBase inventoryItemToolBase = selectable as InventoryItemToolBase;
		InventoryToolCrestSlot inventoryToolCrestSlot = selectable as InventoryToolCrestSlot;
		Sprite sprite;
		Color color;
		if (inventoryItemToolBase != null)
		{
			sprite = inventoryItemToolBase.Sprite;
			color = inventoryItemToolBase.SpriteTint;
			if (inventoryToolCrestSlot == null || !inventoryToolCrestSlot.IsLocked)
			{
				if (this.displayIcon)
				{
					this.displayIcon.gameObject.SetActive(true);
					this.displayIcon.sprite = sprite;
					this.displayIcon.color = color;
				}
				this.showEquipPrompt = true;
			}
		}
		else
		{
			sprite = null;
			color = Color.white;
		}
		if (inventoryToolCrestSlot != null)
		{
			if (inventoryToolCrestSlot.IsLocked)
			{
				if (this.CanUnlockSlot)
				{
					this.slotUnlockDescExtra.SetSlotSprite(sprite, color);
					this.slotUnlockDescExtra.gameObject.SetActive(true);
					this.slotUnlockItemDisplay.Item = this.slotUnlockItem;
				}
			}
			else
			{
				if (inventoryToolCrestSlot.EquippedItem)
				{
					toolItem = inventoryToolCrestSlot.EquippedItem;
				}
				flag = this.ToolListHasType(inventoryToolCrestSlot.Type);
			}
		}
		bool flag2 = this.CanChangeEquips();
		bool isHeroCursed = this.IsHeroCursed;
		if (toolItem)
		{
			if (toolItem.IsUnlockedNotHidden)
			{
				if (this.toolAmountText && toolItem.DisplayAmountText)
				{
					ToolItemsData.Data toolData = PlayerData.instance.GetToolData(toolItem.name);
					int toolStorageAmount = ToolItemManager.GetToolStorageAmount(toolItem);
					this.toolAmountText.text = string.Format(this.initialToolAmountText, toolData.AmountLeft, toolStorageAmount);
					this.toolAmountText.gameObject.SetActive(true);
				}
				if (toolItem.DisplayTogglePrompt)
				{
					this.showCustomTogglePrompt = true;
					if (this.customTogglePromptText)
					{
						this.customTogglePromptText.text = toolItem.CustomToggleText;
					}
				}
				if (toolItem.ReplenishUsage == ToolItem.ReplenishUsages.OneForOne)
				{
					this.showReloadPrompt = true;
					if (this.reloadPrompt)
					{
						this.reloadPrompt.AlphaSelf = ((toolItem.CanReload() && flag2) ? 1f : this.disabledListSectionOpacity);
					}
					if (toolItem.ReplenishResource != ToolItem.ReplenishResources.None)
					{
						this.currencyParent.gameObject.SetActive(true);
						GameObject gameObject = this.reloadCurrencyCounters[(int)toolItem.ReplenishResource];
						if (gameObject)
						{
							gameObject.SetActive(true);
						}
					}
				}
				if ((!this.showReloadPrompt || !this.showCustomTogglePrompt) && !toolItem.HideUsePrompt)
				{
					this.SetToolUsePrompt(ToolItemManager.GetAttackToolBinding(toolItem), toolItem.ShowPromptHold, toolItem.ExtraDescriptionSection ? this.buttonPromptExtraDescOffset : 0f);
				}
				if (this.customTogglePrompt)
				{
					if (this.showCustomTogglePrompt)
					{
						this.customTogglePrompt.AlphaSelf = ((flag2 && !isHeroCursed) ? 1f : this.disabledListSectionOpacity);
					}
					else
					{
						this.customTogglePrompt.AlphaSelf = 1f;
					}
				}
				if (toolItem.HasCustomAction)
				{
					this.comboButtonPromptDisplay.Show(toolItem.CustomButtonCombo);
				}
			}
			else
			{
				this.showEquipPrompt = false;
			}
		}
		if (inventoryItemTool != null || inventoryToolCrestSlot != null)
		{
			if (this.equipPrompt)
			{
				this.equipPrompt.AlphaSelf = ((flag && flag2 && !isHeroCursed) ? 1f : this.disabledListSectionOpacity);
			}
			if (this.equipPromptText)
			{
				ToolItemType toolItemType = (inventoryItemTool != null) ? inventoryItemTool.ToolType : inventoryToolCrestSlot.Type;
				if (toolItem != null && toolItem.IsEquipped)
				{
					this.equipPromptText.text = ((toolItemType == ToolItemType.Skill) ? this.unequipSkillText : this.unequipText);
				}
				else
				{
					this.equipPromptText.text = ((toolItemType == ToolItemType.Skill) ? this.equipSkillText : this.equipText);
				}
			}
		}
		else if (selectable is InventoryItemSelectableButtonEvent)
		{
			if (this.equipPrompt)
			{
				this.equipPrompt.AlphaSelf = 1f;
			}
			if (this.equipPromptText)
			{
				this.equipPromptText.text = (flag2 ? this.changeCrestText : this.viewCrestsText);
			}
		}
		this.UpdateButtonPrompts();
	}

	// Token: 0x17000702 RID: 1794
	// (get) Token: 0x06003CFF RID: 15615 RVA: 0x0010BE07 File Offset: 0x0010A007
	public bool IsHeroCursed
	{
		get
		{
			return Gameplay.CursedCrest.IsEquipped;
		}
	}

	// Token: 0x06003D00 RID: 15616 RVA: 0x0010BE14 File Offset: 0x0010A014
	public bool TryPickupOrPlaceTool(ToolItem tool)
	{
		this.PickedUpTool = tool;
		if (!tool)
		{
			return false;
		}
		IEnumerable<InventoryToolCrestSlot> enumerable = null;
		IEnumerable<InventoryToolCrestSlot> enumerable2 = null;
		IEnumerable<InventoryToolCrestSlot> enumerable3 = null;
		if (this.crestList)
		{
			enumerable2 = this.crestList.GetSlots();
			if (InventoryItemToolManager.GetAvailableSlotCount(enumerable2, new ToolItemType?(tool.Type), true) > 0)
			{
				enumerable = enumerable2;
			}
		}
		if (enumerable == null && this.extraSlots)
		{
			enumerable3 = this.extraSlots.GetSlots();
			if (InventoryItemToolManager.GetAvailableSlotCount(enumerable3, new ToolItemType?(tool.Type), true) > 0)
			{
				enumerable = enumerable3;
			}
		}
		if (enumerable == null)
		{
			if (InventoryItemToolManager.GetAvailableSlotCount(enumerable2, new ToolItemType?(tool.Type), false) > 0)
			{
				enumerable = enumerable2;
			}
			else if (InventoryItemToolManager.GetAvailableSlotCount(enumerable3, new ToolItemType?(tool.Type), false) > 0)
			{
				enumerable = enumerable3;
			}
		}
		if (enumerable != null)
		{
			InventoryToolCrestSlot availableSlot = this.GetAvailableSlot(enumerable, tool.Type);
			if (availableSlot)
			{
				this.EquipState = InventoryItemToolManager.EquipStates.PlaceTool;
				this.selectedBeforePickup = base.CurrentSelected;
				if (availableSlot.Type.IsAttackType())
				{
					if (InventoryItemToolManager.GetAvailableSlotCount(enumerable, new ToolItemType?(availableSlot.Type), false) == 1)
					{
						this.PlaceTool(availableSlot, true);
					}
					else
					{
						base.PlayMoveSound();
						base.SetSelected(availableSlot, null, false);
					}
				}
				else if (InventoryItemToolManager.GetAvailableSlotCount(enumerable, new ToolItemType?(availableSlot.Type), true) > 0)
				{
					this.PlaceTool(availableSlot, true);
				}
				else
				{
					int availableSlotCount = InventoryItemToolManager.GetAvailableSlotCount(enumerable2, new ToolItemType?(availableSlot.Type), false);
					int availableSlotCount2 = InventoryItemToolManager.GetAvailableSlotCount(enumerable3, new ToolItemType?(availableSlot.Type), false);
					if (availableSlotCount + availableSlotCount2 == 1)
					{
						this.PlaceTool(availableSlot, true);
					}
					else
					{
						base.PlayMoveSound();
						base.SetSelected(availableSlot, null, false);
					}
				}
				this.RefreshTools();
				return true;
			}
		}
		this.PickedUpTool = null;
		return false;
	}

	// Token: 0x06003D01 RID: 15617 RVA: 0x0010BFC8 File Offset: 0x0010A1C8
	public void PlaceTool(InventoryToolCrestSlot slot, bool isManual)
	{
		if (slot && this.PickedUpTool.Type != slot.Type)
		{
			return;
		}
		ToolItem pickedUpTool = this.PickedUpTool;
		this.PickedUpTool = null;
		this.EquipState = InventoryItemToolManager.EquipStates.None;
		if (isManual)
		{
			slot.SetEquipped(pickedUpTool, true, true);
		}
		if (!this.selectedBeforePickup)
		{
			return;
		}
		if (isManual)
		{
			slot.PreOpenSlot();
		}
		if (this.tweenTool && slot)
		{
			this.tweenTool.DoPlace(this.selectedBeforePickup.transform.position, slot.transform.position, pickedUpTool, new Action(this.<PlaceTool>g__Selected|121_0));
			return;
		}
		this.<PlaceTool>g__Selected|121_0();
	}

	// Token: 0x06003D02 RID: 15618 RVA: 0x0010C084 File Offset: 0x0010A284
	public InventoryToolCrestSlot GetAvailableSlot(IEnumerable<InventoryToolCrestSlot> slots, ToolItemType toolType)
	{
		InventoryToolCrestSlot inventoryToolCrestSlot = null;
		foreach (InventoryToolCrestSlot inventoryToolCrestSlot2 in slots)
		{
			if (!inventoryToolCrestSlot2.IsLocked && inventoryToolCrestSlot2.Type == toolType)
			{
				if (!inventoryToolCrestSlot)
				{
					inventoryToolCrestSlot = inventoryToolCrestSlot2;
				}
				if (!inventoryToolCrestSlot2.EquippedItem)
				{
					return inventoryToolCrestSlot2;
				}
			}
		}
		return inventoryToolCrestSlot;
	}

	// Token: 0x06003D03 RID: 15619 RVA: 0x0010C0F8 File Offset: 0x0010A2F8
	private static int GetAvailableSlotCount(IEnumerable<InventoryToolCrestSlot> slots, ToolItemType? toolType, bool checkEmpty)
	{
		return slots.Count(delegate(InventoryToolCrestSlot slot)
		{
			if (!slot.IsLocked)
			{
				if (toolType != null)
				{
					ToolItemType type = slot.Type;
					ToolItemType? toolType2 = toolType;
					if (!(type == toolType2.GetValueOrDefault() & toolType2 != null))
					{
						return false;
					}
				}
				return !checkEmpty || slot.EquippedItem == null;
			}
			return false;
		});
	}

	// Token: 0x06003D04 RID: 15620 RVA: 0x0010C12B File Offset: 0x0010A32B
	public static bool IsToolEquipped(ToolItem toolItem)
	{
		return ToolItemManager.IsToolEquipped(toolItem, ToolEquippedReadSource.Hud);
	}

	// Token: 0x06003D05 RID: 15621 RVA: 0x0010C134 File Offset: 0x0010A334
	public bool CrestHasSlot(ToolItemType type)
	{
		return (this.crestList && this.crestList.CrestHasSlot(type)) || (this.extraSlots && InventoryItemToolManager.GetAvailableSlotCount(this.extraSlots.GetSlots(), new ToolItemType?(type), false) > 0);
	}

	// Token: 0x06003D06 RID: 15622 RVA: 0x0010C188 File Offset: 0x0010A388
	public bool CrestHasAnySlots()
	{
		return (this.crestList && this.crestList.CrestHasAnySlots()) || (!this.IsHeroCursed && (this.extraSlots && InventoryItemToolManager.GetAvailableSlotCount(this.extraSlots.GetSlots(), null, false) > 0));
	}

	// Token: 0x06003D07 RID: 15623 RVA: 0x0010C1E8 File Offset: 0x0010A3E8
	public bool ToolListHasType(ToolItemType type)
	{
		if (this.toolList)
		{
			using (List<InventoryItemTool>.Enumerator enumerator = this.toolList.GetListItems<InventoryItemTool>(null).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ToolType == type)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06003D08 RID: 15624 RVA: 0x0010C258 File Offset: 0x0010A458
	public void UnequipTool(ToolItem toolItem, InventoryToolCrestSlot slot)
	{
		if (!toolItem)
		{
			return;
		}
		ToolItemManager.UnequipTool(toolItem);
		if (!slot && this.crestList)
		{
			slot = this.crestList.GetEquippedToolSlot(toolItem);
		}
		if (!slot && this.extraSlots)
		{
			slot = this.extraSlots.GetEquippedToolSlot(toolItem);
		}
		Vector3? vector = null;
		Vector3? vector2 = null;
		if (slot)
		{
			vector = new Vector3?(slot.transform.position);
			slot.SetEquipped(null, true, false);
		}
		if (this.toolList)
		{
			InventoryItemTool inventoryItemTool = this.toolList.GetListItems<InventoryItemTool>((InventoryItemTool t) => t.ItemData == toolItem).FirstOrDefault<InventoryItemTool>();
			if (inventoryItemTool != null)
			{
				this.toolList.ScrollTo(inventoryItemTool, true);
				vector2 = new Vector3?(inventoryItemTool.transform.position);
			}
		}
		if (vector != null && vector2 != null && this.tweenTool)
		{
			this.tweenTool.DoReturn(vector.Value, vector2.Value, toolItem, new Action(this.RefreshTools));
			return;
		}
		this.RefreshTools();
	}

	// Token: 0x06003D09 RID: 15625 RVA: 0x0010C3BB File Offset: 0x0010A5BB
	public void RefreshTools()
	{
		this.RefreshTools(false, true);
	}

	// Token: 0x06003D0A RID: 15626 RVA: 0x0010C3C8 File Offset: 0x0010A5C8
	public void RefreshTools(bool isInstant, bool updateCrest)
	{
		for (int i = 0; i < this.listSectionHeaders.Length; i++)
		{
			Color color = this.listSectionHeaders[i].Color;
			if (this.SelectedSlot && i != (int)this.SelectedSlot.Type)
			{
				color.a = this.disabledListSectionOpacity;
			}
			else
			{
				color.a = 1f;
			}
			this.listSectionHeaders[i].Color = color;
		}
		bool isHidden = this.crestList.CurrentCrest.IsHidden;
		if (this.crestButtonLockedDisplay)
		{
			this.crestButtonLockedDisplay.SetActive(isHidden);
		}
		if (this.crestButtonNormalDisplay)
		{
			this.crestButtonNormalDisplay.SetActive(!isHidden);
		}
		if (updateCrest)
		{
			InventoryToolCrest currentCrest = this.crestList.CurrentCrest;
			if (currentCrest)
			{
				currentCrest.UpdateListDisplay(isInstant);
			}
			InventoryFloatingToolSlots inventoryFloatingToolSlots = this.extraSlots;
			InventoryItemToolManager.EquipStates equipStates = this.EquipState;
			inventoryFloatingToolSlots.SetInEquipMode(equipStates == InventoryItemToolManager.EquipStates.PlaceTool || equipStates == InventoryItemToolManager.EquipStates.SelectTool);
		}
		Action<bool> onToolRefresh = this.OnToolRefresh;
		if (onToolRefresh != null)
		{
			onToolRefresh(isInstant);
		}
		if (this.refreshCurrentSelected)
		{
			if (base.CurrentSelected == null || !base.CurrentSelected.gameObject.activeInHierarchy)
			{
				base.SetSelected(InventoryItemManager.SelectedActionType.LeftMost, true);
			}
			this.refreshCurrentSelected = false;
		}
	}

	// Token: 0x06003D0B RID: 15627 RVA: 0x0010C50E File Offset: 0x0010A70E
	public void OnAppliedCrest()
	{
		this.refreshCurrentSelected = true;
	}

	// Token: 0x06003D0C RID: 15628 RVA: 0x0010C518 File Offset: 0x0010A718
	public void StartSelection(InventoryToolCrestSlot slot)
	{
		if (this.toolList == null)
		{
			return;
		}
		List<InventoryItemTool> listItems = this.toolList.GetListItems<InventoryItemTool>((InventoryItemTool toolItem) => toolItem.ToolType == slot.Type);
		List<InventoryItemTool> list = (from toolItem in listItems
		where !InventoryItemToolManager.IsToolEquipped(toolItem.ItemData)
		select toolItem).ToList<InventoryItemTool>();
		InventoryItemTool inventoryItemTool = null;
		if (list.Count > 0)
		{
			inventoryItemTool = list[0];
		}
		else if (listItems.Count > 0)
		{
			inventoryItemTool = listItems[0];
		}
		if (inventoryItemTool == null)
		{
			return;
		}
		this.SelectedSlot = slot;
		this.EquipState = InventoryItemToolManager.EquipStates.SelectTool;
		base.PlayMoveSound();
		base.SetSelected(inventoryItemTool, null, false);
		this.RefreshTools();
	}

	// Token: 0x06003D0D RID: 15629 RVA: 0x0010C5E8 File Offset: 0x0010A7E8
	public void EndSelection(InventoryItemTool tool)
	{
		if (!this.SelectedSlot)
		{
			return;
		}
		if (tool && tool.ItemData && this.SelectedSlot.Type == tool.ToolType)
		{
			if (this.tweenTool)
			{
				this.SelectedSlot.SetEquipped(tool.ItemData, true, true);
				this.tweenTool.DoPlace(tool.transform.position, this.SelectedSlot.transform.position, tool.ItemData, new Action(this.<EndSelection>g__SelectionEnd|134_0));
				return;
			}
			this.SelectedSlot.SetEquipped(tool.ItemData, true, true);
		}
		this.<EndSelection>g__SelectionEnd|134_0();
	}

	// Token: 0x06003D0E RID: 15630 RVA: 0x0010C6AD File Offset: 0x0010A8AD
	public bool BeginSwitchingCrest()
	{
		if (this.EquipState != InventoryItemToolManager.EquipStates.None)
		{
			return false;
		}
		this.EquipState = InventoryItemToolManager.EquipStates.SwitchCrest;
		this.HideEquipMsgs(true);
		this.RefreshTools();
		return true;
	}

	// Token: 0x06003D0F RID: 15631 RVA: 0x0010C6CE File Offset: 0x0010A8CE
	public void PaneMovePrevented()
	{
		if (this.equipState != InventoryItemToolManager.EquipStates.SwitchCrest)
		{
			return;
		}
		this.crestList.PaneMovePrevented();
	}

	// Token: 0x06003D10 RID: 15632 RVA: 0x0010C6E5 File Offset: 0x0010A8E5
	public bool EndSwitchingCrest()
	{
		if (this.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest)
		{
			return false;
		}
		this.EquipState = InventoryItemToolManager.EquipStates.None;
		this.HideCrestEquipMsg(true);
		return true;
	}

	// Token: 0x06003D11 RID: 15633 RVA: 0x0010C704 File Offset: 0x0010A904
	public float FadeToolGroup(bool fadeIn)
	{
		if (!this.toolGroup)
		{
			return 0f;
		}
		float num = this.toolGroup.FadeTo((float)(fadeIn ? 1 : 0), this.groupFadeTime, null, true, null);
		if (this.groupFadeRoutine != null)
		{
			base.StopCoroutine(this.groupFadeRoutine);
		}
		if (fadeIn)
		{
			this.groupFadeRoutine = this.StartTimerRoutine(0f, num, null, null, delegate
			{
				if (this.cursor)
				{
					this.cursor.Activate();
				}
				if (!this.pane.IsPaneActive)
				{
					return;
				}
				InventoryItemManager.SelectedActionType select = InventoryItemManager.SelectedActionType.Previous;
				InventoryToolCrestSlot inventoryToolCrestSlot = base.CurrentSelected as InventoryToolCrestSlot;
				if (inventoryToolCrestSlot != null && !this.crestList.CurrentCrest.HasSlot(inventoryToolCrestSlot))
				{
					select = InventoryItemManager.SelectedActionType.LeftMost;
				}
				base.SetProxyActive(true, select);
			}, true);
		}
		else
		{
			if (this.cursor)
			{
				this.cursor.Deactivate();
			}
			if (this.pane.IsPaneActive)
			{
				base.SetProxyActive(false, InventoryItemManager.SelectedActionType.Default);
			}
		}
		return num;
	}

	// Token: 0x06003D12 RID: 15634 RVA: 0x0010C7A9 File Offset: 0x0010A9A9
	public float FadeCrestGroup(bool fadeIn)
	{
		if (this.crestGroup)
		{
			return this.crestGroup.FadeTo((float)(fadeIn ? 1 : 0), this.groupFadeTime, null, true, null);
		}
		return 0f;
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x0010C7DA File Offset: 0x0010A9DA
	public Color GetToolTypeColor(ToolItemType type)
	{
		return UI.GetToolTypeColor(type);
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x0010C7E2 File Offset: 0x0010A9E2
	public bool CanChangeEquips()
	{
		return GameManager.instance.playerData.atBench || CheatManager.CanChangeEquipsAnywhere;
	}

	// Token: 0x06003D15 RID: 15637 RVA: 0x0010C7FC File Offset: 0x0010A9FC
	public bool CanChangeEquips(ToolItemType promptToolType, InventoryItemToolManager.CanChangeEquipsTypes changeType)
	{
		if (changeType == InventoryItemToolManager.CanChangeEquipsTypes.Regular && this.IsHeroCursed)
		{
			if (this.ShowingCursedMsg)
			{
				this.HideCursedMsg(false);
			}
			else
			{
				this.ShowCursedMsg(false, promptToolType);
			}
			return false;
		}
		if (this.CanChangeEquips())
		{
			return true;
		}
		if (this.ShowingToolMsg)
		{
			this.HideToolEquipMsg(false);
		}
		else
		{
			this.ShowToolEquipMsg(promptToolType, changeType);
		}
		return false;
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x0010C854 File Offset: 0x0010AA54
	public void ShowToolEquipMsg(ToolItemType type, InventoryItemToolManager.CanChangeEquipsTypes changeType)
	{
		if (!this.toolEquipMsg || this.ShowingToolMsg)
		{
			return;
		}
		if (this.toolEquipMsgText)
		{
			TMP_Text tmp_Text = this.toolEquipMsgText;
			LocalisedString s;
			if (changeType != InventoryItemToolManager.CanChangeEquipsTypes.Reload)
			{
				if (changeType != InventoryItemToolManager.CanChangeEquipsTypes.Transform)
				{
					s = ((type == ToolItemType.Skill) ? this.toolEquipMsgSkill : this.toolEquipMsgTool);
				}
				else
				{
					s = this.transformMsg;
				}
			}
			else
			{
				s = this.reloadMsg;
			}
			tmp_Text.text = s;
		}
		this.toolEquipMsg.FadeTo(1f, this.toolMsgFadeInTime, null, true, null);
		this.ShowingToolMsg = true;
		this.paneList.InSubMenu = true;
		this.hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)this.toolMsgFadeInTime;
		this.failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
	}

	// Token: 0x06003D17 RID: 15639 RVA: 0x0010C92C File Offset: 0x0010AB2C
	public void HideToolEquipMsg(bool force = false)
	{
		if (!this.toolEquipMsg || !this.ShowingToolMsg)
		{
			return;
		}
		if (!force && Time.unscaledTimeAsDouble < this.hideEquipMessageAllowedTime)
		{
			return;
		}
		this.toolEquipMsg.FadeTo(0f, this.toolMsgFadeOutTime, null, true, null);
		this.ShowingToolMsg = false;
		this.paneList.InSubMenu = false;
	}

	// Token: 0x06003D18 RID: 15640 RVA: 0x0010C990 File Offset: 0x0010AB90
	public void HideToolEquipMsgInstant()
	{
		if (!this.toolEquipMsg || !this.ShowingToolMsg)
		{
			return;
		}
		this.toolEquipMsg.FadeTo(0f, 0f, null, true, null);
		this.ShowingToolMsg = false;
		this.paneList.InSubMenu = false;
	}

	// Token: 0x06003D19 RID: 15641 RVA: 0x0010C9DF File Offset: 0x0010ABDF
	public void HideEquipMsgs(bool force = false)
	{
		this.HideToolEquipMsg(force);
		this.HideCrestEquipMsg(force);
		this.HideCursedMsg(force);
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x0010C9F6 File Offset: 0x0010ABF6
	public void HideEquipMsgsInstant()
	{
		this.HideToolEquipMsgInstant();
		this.HideCrestEquipMsgInstant();
		this.HideCursedMsgInstant();
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x0010CA0A File Offset: 0x0010AC0A
	public void ShowCrestEquipMsg()
	{
		this.ShowingCrestMsg = this.ShowBasicEquipMsg(this.crestEquipMsg, this.ShowingCrestMsg);
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x0010CA24 File Offset: 0x0010AC24
	public void HideCrestEquipMsg(bool force = false)
	{
		this.ShowingCrestMsg = this.HideBasicEquipMsg(this.crestEquipMsg, this.toolMsgFadeOutTime, this.ShowingCrestMsg, force);
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x0010CA45 File Offset: 0x0010AC45
	public void HideCrestEquipMsgInstant()
	{
		this.ShowingCrestMsg = this.HideBasicEquipMsg(this.crestEquipMsg, 0f, this.ShowingCrestMsg, true);
	}

	// Token: 0x06003D1E RID: 15646 RVA: 0x0010CA68 File Offset: 0x0010AC68
	public void ShowCursedMsg(bool isCrestEquip, ToolItemType toolType)
	{
		if (this.cursedEquipMsgText)
		{
			if (isCrestEquip)
			{
				this.cursedEquipMsgText.text = this.cursedEquipMsgCrest;
			}
			else if (toolType == ToolItemType.Skill)
			{
				this.cursedEquipMsgText.text = this.cursedEquipMsgSkill;
			}
			else
			{
				this.cursedEquipMsgText.text = this.cursedEquipMsgTool;
			}
		}
		this.ShowingCursedMsg = this.ShowBasicEquipMsg(this.cursedEquipMsg, this.ShowingCursedMsg);
	}

	// Token: 0x06003D1F RID: 15647 RVA: 0x0010CAE7 File Offset: 0x0010ACE7
	public void HideCursedMsg(bool force = false)
	{
		this.ShowingCursedMsg = this.HideBasicEquipMsg(this.cursedEquipMsg, this.toolMsgFadeOutTime, this.ShowingCursedMsg, force);
	}

	// Token: 0x06003D20 RID: 15648 RVA: 0x0010CB08 File Offset: 0x0010AD08
	public void HideCursedMsgInstant()
	{
		this.ShowingCursedMsg = this.HideBasicEquipMsg(this.cursedEquipMsg, 0f, this.ShowingCursedMsg, true);
	}

	// Token: 0x06003D21 RID: 15649 RVA: 0x0010CB28 File Offset: 0x0010AD28
	private bool ShowBasicEquipMsg(NestedFadeGroupBase msgGroup, bool showingBool)
	{
		if (!msgGroup || showingBool)
		{
			return showingBool;
		}
		msgGroup.FadeTo(1f, this.toolMsgFadeInTime, null, true, null);
		this.paneList.InSubMenu = true;
		this.hideEquipMessageAllowedTime = Time.unscaledTimeAsDouble + (double)this.toolMsgFadeInTime;
		this.failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
		return true;
	}

	// Token: 0x06003D22 RID: 15650 RVA: 0x0010CB9C File Offset: 0x0010AD9C
	private bool HideBasicEquipMsg(NestedFadeGroupBase msgGroup, float fadeTime, bool showingBool, bool force)
	{
		if (!msgGroup || !showingBool)
		{
			return showingBool;
		}
		if (!force && Time.unscaledTimeAsDouble < this.hideEquipMessageAllowedTime)
		{
			return true;
		}
		msgGroup.FadeTo(0f, fadeTime, null, true, null);
		this.paneList.InSubMenu = false;
		return false;
	}

	// Token: 0x06003D23 RID: 15651 RVA: 0x0010CBDC File Offset: 0x0010ADDC
	protected override List<ToolItem> GetItems()
	{
		if (!PlayerData.instance.ConstructedFarsight)
		{
			List<ToolItem> list = ToolItemManager.GetUnlockedTools().ToList<ToolItem>();
			this.currentToolCount = list.Count;
			return list;
		}
		this.currentToolCount = 0;
		List<ToolItem> list2 = ToolItemManager.GetAllTools().ToList<ToolItem>();
		for (int i = list2.Count - 1; i >= 0; i--)
		{
			ToolItem toolItem = list2[i];
			if (toolItem.IsUnlockedNotHidden)
			{
				this.currentToolCount++;
			}
			else if (!toolItem.IsCounted)
			{
				list2.RemoveAt(i);
			}
			else
			{
				SavedItem countKey = toolItem.CountKey;
				foreach (ToolItem toolItem2 in list2)
				{
					if (!(toolItem2 == toolItem) && toolItem2.CountKey == countKey)
					{
						list2.RemoveAt(i);
						break;
					}
				}
			}
		}
		return list2;
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x0010CCD4 File Offset: 0x0010AED4
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemTool> selectableItems, List<ToolItem> items)
	{
		for (int k = 0; k < selectableItems.Count; k++)
		{
			selectableItems[k].gameObject.SetActive(true);
			selectableItems[k].SetData(items[k]);
		}
		int[] array = typeof(ToolItemType).GetValuesWithOrder().ToArray<int>();
		List<InventoryItemGrid.GridSection> list = new List<InventoryItemGrid.GridSection>(array.Length);
		int[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			int i = array2[j];
			list.Add(new InventoryItemGrid.GridSection
			{
				Header = this.listSectionHeaders[i].transform,
				Items = (from item in selectableItems
				where item.ToolType == (ToolItemType)i
				select item).Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			});
		}
		return list;
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x0010CD9C File Offset: 0x0010AF9C
	protected override void OnItemListSetup()
	{
		if (!this.completionText)
		{
			return;
		}
		if (PlayerData.instance.ConstructedFarsight)
		{
			this.completionText.gameObject.SetActive(true);
			int count = ToolItemManager.GetCount(ToolItemManager.GetAllTools(), null);
			int num = Mathf.Min(this.currentToolCount, count);
			this.completionText.text = string.Format("{0} / {1}", num, count);
			return;
		}
		this.completionText.gameObject.SetActive(false);
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x0010CE20 File Offset: 0x0010B020
	public bool IsAvailable()
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		if (ToolItemManager.GetAllCrests().Count((ToolCrest crest) => crest.IsVisible) > 1)
		{
			return true;
		}
		if (this.GetItems().Count <= 0)
		{
			return false;
		}
		using (List<ToolCrest>.Enumerator enumerator = ToolItemManager.GetAllCrests().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsVisible)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x0010CEC0 File Offset: 0x0010B0C0
	public void SetToolUsePrompt(AttackToolBinding? binding, bool showHold, float offsetY)
	{
		if (this.comboButtonPromptDisplay == null)
		{
			return;
		}
		Vector2 value = this.currencyParentInitialPos.GetValueOrDefault();
		if (this.currencyParentInitialPos == null)
		{
			value = this.currencyParent.transform.localPosition;
			this.currencyParentInitialPos = new Vector2?(value);
		}
		if (binding == null)
		{
			this.comboButtonPromptDisplay.Hide();
			this.currencyParent.transform.SetLocalPosition2D(this.currencyParentInitialPos.Value);
			return;
		}
		this.currencyParent.transform.SetLocalPosition2D(this.currencyPromptAltPos);
		Vector3 vector = this.comboButtonPromptDisplay.transform.localPosition;
		value = this.buttonPromptInitialPos.GetValueOrDefault();
		if (this.buttonPromptInitialPos == null)
		{
			value = vector;
			this.buttonPromptInitialPos = new Vector2?(value);
		}
		if (this.currencyParent.gameObject.activeSelf)
		{
			vector = this.buttonPromptCurrencyAltPos;
		}
		else
		{
			vector = this.buttonPromptInitialPos.Value;
			vector.y += offsetY;
		}
		this.comboButtonPromptDisplay.transform.localPosition = vector;
		this.comboButtonPromptDisplay.Show(new InventoryItemComboButtonPromptDisplay.Display
		{
			ActionButton = HeroActionButton.QUICK_CAST,
			DirectionModifier = binding.Value,
			PromptText = this.toolUsePromptText,
			ShowHold = showHold
		});
	}

	// Token: 0x06003D28 RID: 15656 RVA: 0x0010D028 File Offset: 0x0010B228
	private void UpdateTextDisplays()
	{
		LocalisedString localisedString = this.CanChangeEquips() ? this.changeCrestText : this.viewCrestsText;
		if (this.changeCrestButton)
		{
			this.changeCrestButton.InteractionText = localisedString;
		}
		if (this.changeCrestButtonText)
		{
			this.changeCrestButtonText.Text = localisedString;
		}
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x0010D080 File Offset: 0x0010B280
	protected override InventoryItemSelectable GetStartSelectable()
	{
		InventoryItemTool inventoryItemTool = base.GetSelectables(null).FirstOrDefault((InventoryItemTool tool) => ToolItemManager.UnlockedTool == tool.ItemData);
		ToolItemManager.UnlockedTool = null;
		if (inventoryItemTool)
		{
			return inventoryItemTool;
		}
		return base.GetStartSelectable();
	}

	// Token: 0x06003D2A RID: 15658 RVA: 0x0010D0D0 File Offset: 0x0010B2D0
	private void UpdateButtonPrompts()
	{
		bool active = this.showEquipPrompt && this.equipState != InventoryItemToolManager.EquipStates.SwitchCrest;
		bool active2 = this.equipState == InventoryItemToolManager.EquipStates.None;
		bool active3 = this.equipState == InventoryItemToolManager.EquipStates.SwitchCrest;
		bool active4 = this.equipState > InventoryItemToolManager.EquipStates.None;
		if (this.equipPrompt)
		{
			this.equipPrompt.gameObject.SetActive(active);
		}
		if (this.changeCrestPrompt)
		{
			this.changeCrestPrompt.AlphaSelf = (this.IsHeroCursed ? this.disabledListSectionOpacity : 1f);
			this.changeCrestPrompt.gameObject.SetActive(active2);
		}
		if (this.selectCrestPrompt)
		{
			this.selectCrestPrompt.gameObject.SetActive(active3);
			this.selectCrestPrompt.AlphaSelf = (this.CanChangeEquips() ? 1f : this.disabledListSectionOpacity);
		}
		if (this.cancelPrompt)
		{
			this.cancelPrompt.SetActive(active4);
		}
		if (this.reloadPrompt)
		{
			this.reloadPrompt.gameObject.SetActive(this.showReloadPrompt && this.equipState != InventoryItemToolManager.EquipStates.SwitchCrest);
		}
		if (this.customTogglePrompt)
		{
			this.customTogglePrompt.gameObject.SetActive(this.showCustomTogglePrompt && this.equipState != InventoryItemToolManager.EquipStates.SwitchCrest);
		}
		if (this.boolToggleParent)
		{
			this.boolToggleParent.SetActive(this.showReloadPrompt && this.equipState != InventoryItemToolManager.EquipStates.SwitchCrest);
		}
		if (this.boolToggleFill)
		{
			this.boolToggleFill.SetActive(false);
		}
		if (this.promptLayout)
		{
			this.promptLayout.ForceUpdateLayoutNoCanvas();
		}
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x0010D28E File Offset: 0x0010B48E
	public void SetHoveringTool(InventoryItemTool tool, bool refreshTools)
	{
		this.HoveringTool = tool;
		if (refreshTools)
		{
			this.RefreshTools();
		}
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x0010D2A0 File Offset: 0x0010B4A0
	public override bool MoveSelection(InventoryItemManager.SelectionDirection direction)
	{
		bool flag = base.MoveSelection(direction);
		if (!flag)
		{
			InventoryItemToolManager.EquipStates equipStates = this.equipState;
			if (equipStates == InventoryItemToolManager.EquipStates.PlaceTool || equipStates == InventoryItemToolManager.EquipStates.SelectTool)
			{
				return true;
			}
		}
		return flag;
	}

	// Token: 0x06003D2F RID: 15663 RVA: 0x0010D39C File Offset: 0x0010B59C
	[CompilerGenerated]
	private void <PlaceTool>g__Selected|121_0()
	{
		base.SetSelected(this.selectedBeforePickup, null, false);
		this.selectedBeforePickup = null;
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x0010D3C8 File Offset: 0x0010B5C8
	[CompilerGenerated]
	private void <EndSelection>g__SelectionEnd|134_0()
	{
		base.PlayMoveSound();
		base.SetSelected(this.SelectedSlot, null, false);
		this.SelectedSlot = null;
		this.EquipState = InventoryItemToolManager.EquipStates.None;
		this.RefreshTools();
	}

	// Token: 0x04003E97 RID: 16023
	public Action<bool> OnToolRefresh;

	// Token: 0x04003E98 RID: 16024
	[SerializeField]
	private SpriteRenderer displayIcon;

	// Token: 0x04003E99 RID: 16025
	[SerializeField]
	private InventoryItemGrid toolList;

	// Token: 0x04003E9A RID: 16026
	[SerializeField]
	private InventoryToolCrestList crestList;

	// Token: 0x04003E9B RID: 16027
	[SerializeField]
	private InventoryFloatingToolSlots extraSlots;

	// Token: 0x04003E9C RID: 16028
	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private NestedFadeGroupSpriteRenderer[] listSectionHeaders;

	// Token: 0x04003E9D RID: 16029
	[SerializeField]
	private float disabledListSectionOpacity = 0.5f;

	// Token: 0x04003E9E RID: 16030
	[Space]
	[SerializeField]
	private LayoutGroup promptLayout;

	// Token: 0x04003E9F RID: 16031
	[SerializeField]
	private NestedFadeGroupBase equipPrompt;

	// Token: 0x04003EA0 RID: 16032
	[SerializeField]
	private TMP_Text equipPromptText;

	// Token: 0x04003EA1 RID: 16033
	[SerializeField]
	private LocalisedString equipText;

	// Token: 0x04003EA2 RID: 16034
	[SerializeField]
	private LocalisedString unequipText;

	// Token: 0x04003EA3 RID: 16035
	[SerializeField]
	private LocalisedString equipSkillText;

	// Token: 0x04003EA4 RID: 16036
	[SerializeField]
	private LocalisedString unequipSkillText;

	// Token: 0x04003EA5 RID: 16037
	[SerializeField]
	private NestedFadeGroupBase changeCrestPrompt;

	// Token: 0x04003EA6 RID: 16038
	[SerializeField]
	private NestedFadeGroupBase selectCrestPrompt;

	// Token: 0x04003EA7 RID: 16039
	[SerializeField]
	private GameObject cancelPrompt;

	// Token: 0x04003EA8 RID: 16040
	[SerializeField]
	private NestedFadeGroupBase reloadPrompt;

	// Token: 0x04003EA9 RID: 16041
	[SerializeField]
	private NestedFadeGroupBase customTogglePrompt;

	// Token: 0x04003EAA RID: 16042
	[SerializeField]
	private TMP_Text customTogglePromptText;

	// Token: 0x04003EAB RID: 16043
	[SerializeField]
	[ArrayForEnum(typeof(CurrencyType))]
	private GameObject[] reloadCurrencyCounters;

	// Token: 0x04003EAC RID: 16044
	[SerializeField]
	private GameObject boolToggleParent;

	// Token: 0x04003EAD RID: 16045
	[SerializeField]
	private GameObject boolToggleFill;

	// Token: 0x04003EAE RID: 16046
	[Space]
	[SerializeField]
	private NestedFadeGroupBase toolGroup;

	// Token: 0x04003EAF RID: 16047
	[SerializeField]
	private NestedFadeGroupBase crestGroup;

	// Token: 0x04003EB0 RID: 16048
	[SerializeField]
	private float groupFadeTime = 0.1f;

	// Token: 0x04003EB1 RID: 16049
	private Coroutine groupFadeRoutine;

	// Token: 0x04003EB2 RID: 16050
	[SerializeField]
	private InventoryItemToolTween tweenTool;

	// Token: 0x04003EB3 RID: 16051
	[SerializeField]
	private NestedFadeGroupBase toolEquipMsg;

	// Token: 0x04003EB4 RID: 16052
	[SerializeField]
	private TMP_Text toolEquipMsgText;

	// Token: 0x04003EB5 RID: 16053
	[SerializeField]
	private LocalisedString toolEquipMsgTool;

	// Token: 0x04003EB6 RID: 16054
	[SerializeField]
	private LocalisedString toolEquipMsgSkill;

	// Token: 0x04003EB7 RID: 16055
	[SerializeField]
	private LocalisedString reloadMsg;

	// Token: 0x04003EB8 RID: 16056
	[SerializeField]
	private LocalisedString transformMsg;

	// Token: 0x04003EB9 RID: 16057
	[SerializeField]
	private NestedFadeGroupBase crestEquipMsg;

	// Token: 0x04003EBA RID: 16058
	[SerializeField]
	private float toolMsgFadeInTime;

	// Token: 0x04003EBB RID: 16059
	[SerializeField]
	private float toolMsgFadeOutTime;

	// Token: 0x04003EBC RID: 16060
	[SerializeField]
	private NestedFadeGroupBase cursedEquipMsg;

	// Token: 0x04003EBD RID: 16061
	[SerializeField]
	private TMP_Text cursedEquipMsgText;

	// Token: 0x04003EBE RID: 16062
	[SerializeField]
	private LocalisedString cursedEquipMsgTool;

	// Token: 0x04003EBF RID: 16063
	[SerializeField]
	private LocalisedString cursedEquipMsgSkill;

	// Token: 0x04003EC0 RID: 16064
	[SerializeField]
	private LocalisedString cursedEquipMsgCrest;

	// Token: 0x04003EC1 RID: 16065
	[Space]
	[SerializeField]
	private TMP_Text toolAmountText;

	// Token: 0x04003EC2 RID: 16066
	[SerializeField]
	private LocalisedString toolUsePromptText;

	// Token: 0x04003EC3 RID: 16067
	[SerializeField]
	private InventoryItemComboButtonPromptDisplay comboButtonPromptDisplay;

	// Token: 0x04003EC4 RID: 16068
	[SerializeField]
	private float buttonPromptExtraDescOffset;

	// Token: 0x04003EC5 RID: 16069
	[SerializeField]
	private Vector2 buttonPromptCurrencyAltPos;

	// Token: 0x04003EC6 RID: 16070
	[SerializeField]
	private Transform currencyParent;

	// Token: 0x04003EC7 RID: 16071
	[SerializeField]
	private Vector2 currencyPromptAltPos;

	// Token: 0x04003EC8 RID: 16072
	[Space]
	[SerializeField]
	private InventoryItemSelectableButtonEvent changeCrestButton;

	// Token: 0x04003EC9 RID: 16073
	[SerializeField]
	private GameObject crestButtonNormalDisplay;

	// Token: 0x04003ECA RID: 16074
	[SerializeField]
	private GameObject crestButtonLockedDisplay;

	// Token: 0x04003ECB RID: 16075
	[SerializeField]
	private SetTextMeshProGameText changeCrestButtonText;

	// Token: 0x04003ECC RID: 16076
	[SerializeField]
	private LocalisedString changeCrestText;

	// Token: 0x04003ECD RID: 16077
	[SerializeField]
	private LocalisedString viewCrestsText;

	// Token: 0x04003ECE RID: 16078
	[SerializeField]
	private GameObject descriptionIconGroup;

	// Token: 0x04003ECF RID: 16079
	[Space]
	[SerializeField]
	private CollectableItem slotUnlockItem;

	// Token: 0x04003ED0 RID: 16080
	[SerializeField]
	private CrestSocketUnlockInventoryDescription slotUnlockDescExtra;

	// Token: 0x04003ED1 RID: 16081
	[SerializeField]
	private InventoryItemCollectable slotUnlockItemDisplay;

	// Token: 0x04003ED2 RID: 16082
	[Space]
	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	// Token: 0x04003ED3 RID: 16083
	[Space]
	[SerializeField]
	private TMP_Text completionText;

	// Token: 0x04003ED4 RID: 16084
	private string initialToolAmountText;

	// Token: 0x04003ED5 RID: 16085
	private double hideEquipMessageAllowedTime;

	// Token: 0x04003ED6 RID: 16086
	private bool showReloadPrompt;

	// Token: 0x04003ED7 RID: 16087
	private bool showCustomTogglePrompt;

	// Token: 0x04003ED8 RID: 16088
	private bool showEquipPrompt;

	// Token: 0x04003ED9 RID: 16089
	private int currentToolCount;

	// Token: 0x04003EDA RID: 16090
	private Vector2? buttonPromptInitialPos;

	// Token: 0x04003EDB RID: 16091
	private Vector2? currencyParentInitialPos;

	// Token: 0x04003EDC RID: 16092
	private InventoryPane pane;

	// Token: 0x04003EDD RID: 16093
	private InventoryPaneList paneList;

	// Token: 0x04003EDF RID: 16095
	private InventoryItemToolManager.EquipStates equipState;

	// Token: 0x04003EE4 RID: 16100
	private InventoryItemSelectable selectedBeforePickup;

	// Token: 0x04003EE6 RID: 16102
	private bool refreshCurrentSelected;

	// Token: 0x020019A2 RID: 6562
	public enum EquipStates
	{
		// Token: 0x04009679 RID: 38521
		None,
		// Token: 0x0400967A RID: 38522
		PlaceTool,
		// Token: 0x0400967B RID: 38523
		SelectTool,
		// Token: 0x0400967C RID: 38524
		SwitchCrest
	}

	// Token: 0x020019A3 RID: 6563
	public enum CanChangeEquipsTypes
	{
		// Token: 0x0400967E RID: 38526
		Regular,
		// Token: 0x0400967F RID: 38527
		Reload,
		// Token: 0x04009680 RID: 38528
		Transform
	}
}
