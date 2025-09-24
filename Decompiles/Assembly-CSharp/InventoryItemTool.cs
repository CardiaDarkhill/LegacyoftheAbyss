using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class InventoryItemTool : InventoryItemToolBase
{
	// Token: 0x170006EC RID: 1772
	// (get) Token: 0x06003CB8 RID: 15544 RVA: 0x0010A728 File Offset: 0x00108928
	public ToolItemType ToolType
	{
		get
		{
			return this.itemData.Type;
		}
	}

	// Token: 0x170006ED RID: 1773
	// (get) Token: 0x06003CB9 RID: 15545 RVA: 0x0010A735 File Offset: 0x00108935
	public override ToolItem ItemData
	{
		get
		{
			return this.itemData;
		}
	}

	// Token: 0x170006EE RID: 1774
	// (get) Token: 0x06003CBA RID: 15546 RVA: 0x0010A73D File Offset: 0x0010893D
	public override string DisplayName
	{
		get
		{
			if (!this.itemData.IsUnlockedNotHidden)
			{
				return string.Empty;
			}
			return this.itemData.DisplayName;
		}
	}

	// Token: 0x170006EF RID: 1775
	// (get) Token: 0x06003CBB RID: 15547 RVA: 0x0010A762 File Offset: 0x00108962
	public override string Description
	{
		get
		{
			if (!this.itemData.IsUnlockedNotHidden)
			{
				return string.Empty;
			}
			return this.itemData.Description;
		}
	}

	// Token: 0x170006F0 RID: 1776
	// (get) Token: 0x06003CBC RID: 15548 RVA: 0x0010A787 File Offset: 0x00108987
	public override Sprite Sprite
	{
		get
		{
			if (!this.itemData.IsUnlockedNotHidden)
			{
				return null;
			}
			return this.itemData.InventorySpriteBase;
		}
	}

	// Token: 0x170006F1 RID: 1777
	// (get) Token: 0x06003CBD RID: 15549 RVA: 0x0010A7A4 File Offset: 0x001089A4
	public override Color? CursorColor
	{
		get
		{
			if (this.itemData && this.manager)
			{
				return new Color?(this.manager.GetToolTypeColor(this.itemData.Type));
			}
			return null;
		}
	}

	// Token: 0x170006F2 RID: 1778
	// (get) Token: 0x06003CBE RID: 15550 RVA: 0x0010A7F0 File Offset: 0x001089F0
	// (set) Token: 0x06003CBF RID: 15551 RVA: 0x0010A840 File Offset: 0x00108A40
	protected override bool IsSeen
	{
		get
		{
			if (!this.itemData || !this.itemData.IsUnlockedNotHidden)
			{
				return true;
			}
			IToolExtraNew toolExtraNew = this.itemData as IToolExtraNew;
			return (toolExtraNew == null || !toolExtraNew.HasExtraNew) && this.itemData.HasBeenSeen;
		}
		set
		{
			if (!this.itemData || !this.itemData.IsUnlockedNotHidden)
			{
				return;
			}
			this.itemData.HasBeenSeen = value;
			if (value)
			{
				IToolExtraNew toolExtraNew = this.itemData as IToolExtraNew;
				if (toolExtraNew != null && toolExtraNew.HasExtraNew)
				{
					toolExtraNew.SetExtraSeen();
				}
			}
		}
	}

	// Token: 0x06003CC0 RID: 15552 RVA: 0x0010A894 File Offset: 0x00108A94
	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize<RuntimeAnimatorController>(ref this.slotAnimatorControllers, typeof(ToolItemType));
		ArrayForEnumAttribute.EnsureArraySize<RuntimeAnimatorController>(ref this.attackAnimatorControllers, typeof(AttackToolBinding));
		ArrayForEnumAttribute.EnsureArraySize<RuntimeAnimatorController>(ref this.skillAnimatorControllers, typeof(AttackToolBinding));
	}

	// Token: 0x06003CC1 RID: 15553 RVA: 0x0010A8E6 File Offset: 0x00108AE6
	protected override void Awake()
	{
		base.Awake();
		this.GetComponentsIfNeeded();
	}

	// Token: 0x06003CC2 RID: 15554 RVA: 0x0010A8F4 File Offset: 0x00108AF4
	private void GetComponentsIfNeeded()
	{
		if (!this.manager)
		{
			this.manager = base.GetComponentInParent<InventoryItemToolManager>();
		}
	}

	// Token: 0x06003CC3 RID: 15555 RVA: 0x0010A910 File Offset: 0x00108B10
	public void SetData(ToolItem newItemData)
	{
		this.GetComponentsIfNeeded();
		if (this.manager)
		{
			InventoryItemToolManager inventoryItemToolManager = this.manager;
			inventoryItemToolManager.OnToolRefresh = (Action<bool>)Delegate.Remove(inventoryItemToolManager.OnToolRefresh, new Action<bool>(this.UpdateEquippedDisplay));
			if (newItemData)
			{
				InventoryItemToolManager inventoryItemToolManager2 = this.manager;
				inventoryItemToolManager2.OnToolRefresh = (Action<bool>)Delegate.Combine(inventoryItemToolManager2.OnToolRefresh, new Action<bool>(this.UpdateEquippedDisplay));
			}
		}
		bool flag = this.itemData != newItemData;
		this.itemData = newItemData;
		if (flag)
		{
			base.ItemDataUpdated();
		}
		base.gameObject.name = (newItemData ? newItemData.name : "null");
		this.RefreshIcon();
		if (newItemData && this.slotAnimator)
		{
			RuntimeAnimatorController runtimeAnimatorController = this.slotAnimatorControllers[(int)newItemData.Type];
			this.slotAnimator.runtimeAnimatorController = runtimeAnimatorController;
			this.UpdateAttackSlotAnimator();
			this.slotAnimator.Play(this.isAnimatorEquipped ? InventoryItemTool._fullAnim : InventoryItemTool._emptyAnim);
		}
		this.UpdateDisplay();
	}

	// Token: 0x06003CC4 RID: 15556 RVA: 0x0010AA21 File Offset: 0x00108C21
	protected override bool IsToolEquipped(ToolItem toolItem)
	{
		return toolItem.IsEquippedHud;
	}

	// Token: 0x06003CC5 RID: 15557 RVA: 0x0010AA2C File Offset: 0x00108C2C
	private void RefreshIcon()
	{
		if (this.itemData && this.itemData.IsUnlockedNotHidden)
		{
			if (this.itemIcon)
			{
				this.itemIcon.sprite = this.itemData.InventorySpriteBase;
			}
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(false);
				return;
			}
		}
		else
		{
			if (this.itemIcon)
			{
				this.itemIcon.sprite = null;
			}
			if (this.emptyNotch)
			{
				this.emptyNotch.SetActive(true);
			}
		}
	}

	// Token: 0x06003CC6 RID: 15558 RVA: 0x0010AAC4 File Offset: 0x00108CC4
	public override bool Submit()
	{
		if (!this.manager || !this.manager.CanChangeEquips(this.itemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Regular))
		{
			return false;
		}
		if (this.manager.EquipState != InventoryItemToolManager.EquipStates.SelectTool)
		{
			return base.Submit();
		}
		if (!this.itemData.IsUnlockedNotHidden)
		{
			return false;
		}
		this.DoPick();
		return true;
	}

	// Token: 0x06003CC7 RID: 15559 RVA: 0x0010AB24 File Offset: 0x00108D24
	protected override bool DoPress()
	{
		InventoryItemToolManager.EquipStates equipState = this.manager.EquipState;
		if (equipState == InventoryItemToolManager.EquipStates.None)
		{
			if (InventoryItemToolManager.IsToolEquipped(this.itemData))
			{
				this.manager.UnequipTool(this.itemData, null);
			}
			else
			{
				if (!this.itemData.IsUnlockedNotHidden)
				{
					return false;
				}
				if (!this.manager.TryPickupOrPlaceTool(this.itemData))
				{
					base.ReportFailure();
				}
			}
			return true;
		}
		if (equipState != InventoryItemToolManager.EquipStates.SelectTool)
		{
			return false;
		}
		if (!this.itemData.IsUnlockedNotHidden)
		{
			return false;
		}
		this.DoPick();
		return true;
	}

	// Token: 0x06003CC8 RID: 15560 RVA: 0x0010ABA9 File Offset: 0x00108DA9
	private void DoPick()
	{
		if (!InventoryItemToolManager.IsToolEquipped(this.itemData))
		{
			this.manager.EndSelection(this);
		}
	}

	// Token: 0x06003CC9 RID: 15561 RVA: 0x0010ABC4 File Offset: 0x00108DC4
	public override bool Cancel()
	{
		if (this.manager.ShowingToolMsg)
		{
			this.manager.HideToolEquipMsg(false);
		}
		else if (this.manager.ShowingCursedMsg)
		{
			this.manager.HideCursedMsg(false);
		}
		else if (this.manager && this.manager.SelectedSlot)
		{
			this.manager.EndSelection(null);
		}
		return base.Cancel();
	}

	// Token: 0x06003CCA RID: 15562 RVA: 0x0010AC38 File Offset: 0x00108E38
	private void UpdateEquippedDisplay(bool isInstant)
	{
		this.GetComponentsIfNeeded();
		Color color;
		if (this.manager.PickedUpTool == this.itemData || (this.manager.SelectedSlot && this.manager.SelectedSlot.Type != this.itemData.Type))
		{
			color = InventoryToolCrestSlot.InvalidItemColor;
		}
		else
		{
			color = Color.white;
		}
		if (this.itemIcon)
		{
			this.itemIcon.color = color;
		}
		if (InventoryItemToolManager.IsToolEquipped(this.itemData))
		{
			if (this.selectedIndicator)
			{
				this.selectedIndicator.Color = this.manager.GetToolTypeColor(this.itemData.Type).MultiplyElements(color);
			}
			if (this.slotAnimator && (!this.isAnimatorEquipped || isInstant))
			{
				this.UpdateAttackSlotAnimator();
				if (isInstant)
				{
					this.slotAnimator.Play(InventoryItemTool._fullAnim, 0, 1f);
				}
				else
				{
					this.slotAnimator.Play(InventoryItemTool._equipAnim);
				}
				this.isAnimatorEquipped = true;
			}
		}
		else if (this.slotAnimator && (this.isAnimatorEquipped || isInstant))
		{
			if (isInstant)
			{
				this.slotAnimator.Play(InventoryItemTool._emptyAnim, 0, 1f);
			}
			else
			{
				this.slotAnimator.Play(InventoryItemTool._unequipAnim);
			}
			this.isAnimatorEquipped = false;
		}
		this.RefreshIcon();
		this.UpdateDisplay();
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x0010ADAC File Offset: 0x00108FAC
	private void UpdateAttackSlotAnimator()
	{
		if (!this.itemData.Type.IsAttackType())
		{
			return;
		}
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this.itemData);
		if (attackToolBinding == null)
		{
			return;
		}
		this.slotAnimator.runtimeAnimatorController = ((this.itemData.Type == ToolItemType.Skill) ? this.skillAnimatorControllers[(int)attackToolBinding.Value] : this.attackAnimatorControllers[(int)attackToolBinding.Value]);
	}

	// Token: 0x06003CCC RID: 15564 RVA: 0x0010AE1C File Offset: 0x0010901C
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectable nextSelectable = base.GetNextSelectable(direction);
		InventoryItemTool inventoryItemTool = nextSelectable as InventoryItemTool;
		if (this.manager && this.manager.EquipState == InventoryItemToolManager.EquipStates.SelectTool && (!nextSelectable || inventoryItemTool == null || inventoryItemTool.ToolType != this.manager.SelectedSlot.Type))
		{
			return this;
		}
		return nextSelectable;
	}

	// Token: 0x06003CCD RID: 15565 RVA: 0x0010AE7F File Offset: 0x0010907F
	public override InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		if (this.manager.EquipState != InventoryItemToolManager.EquipStates.SelectTool)
		{
			return base.GetNextSelectablePage(currentSelected, direction);
		}
		return null;
	}

	// Token: 0x06003CCE RID: 15566 RVA: 0x0010AE99 File Offset: 0x00109099
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		this.manager.SetHoveringTool(this, true);
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x0010AEB0 File Offset: 0x001090B0
	public override void Deselect()
	{
		base.Deselect();
		if (this.manager.HoveringTool == this)
		{
			bool flag = this.manager.NextSelected && this.manager.NextSelected is InventoryItemTool;
			this.manager.SetHoveringTool(null, !flag);
		}
	}

	// Token: 0x04003E74 RID: 15988
	[Header("Tool")]
	[SerializeField]
	private ToolItem itemData;

	// Token: 0x04003E75 RID: 15989
	[SerializeField]
	private SpriteRenderer itemIcon;

	// Token: 0x04003E76 RID: 15990
	[SerializeField]
	private GameObject emptyNotch;

	// Token: 0x04003E77 RID: 15991
	[SerializeField]
	private NestedFadeGroupSpriteRenderer selectedIndicator;

	// Token: 0x04003E78 RID: 15992
	[Space]
	[SerializeField]
	private Animator slotAnimator;

	// Token: 0x04003E79 RID: 15993
	[SerializeField]
	[ArrayForEnum(typeof(ToolItemType))]
	private RuntimeAnimatorController[] slotAnimatorControllers;

	// Token: 0x04003E7A RID: 15994
	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] attackAnimatorControllers;

	// Token: 0x04003E7B RID: 15995
	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] skillAnimatorControllers;

	// Token: 0x04003E7C RID: 15996
	private bool isAnimatorEquipped;

	// Token: 0x04003E7D RID: 15997
	[NonSerialized]
	private InventoryItemToolManager manager;

	// Token: 0x04003E7E RID: 15998
	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	// Token: 0x04003E7F RID: 15999
	private static readonly int _fullAnim = Animator.StringToHash("Full");

	// Token: 0x04003E80 RID: 16000
	private static readonly int _equipAnim = Animator.StringToHash("Equip");

	// Token: 0x04003E81 RID: 16001
	private static readonly int _unequipAnim = Animator.StringToHash("Unequip");
}
