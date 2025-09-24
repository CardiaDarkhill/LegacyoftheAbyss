using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;

// Token: 0x020006BA RID: 1722
[DefaultExecutionOrder(2)]
public class InventoryToolCrestSlot : InventoryItemToolBase
{
	// Token: 0x140000DB RID: 219
	// (add) Token: 0x06003E44 RID: 15940 RVA: 0x00112274 File Offset: 0x00110474
	// (remove) Token: 0x06003E45 RID: 15941 RVA: 0x001122AC File Offset: 0x001104AC
	public event Action OnSetEquipSaved;

	// Token: 0x17000725 RID: 1829
	// (get) Token: 0x06003E46 RID: 15942 RVA: 0x001122E4 File Offset: 0x001104E4
	private MaterialPropertyBlock Block
	{
		get
		{
			MaterialPropertyBlock result;
			if ((result = this.block) == null)
			{
				result = (this.block = new MaterialPropertyBlock());
			}
			return result;
		}
	}

	// Token: 0x17000726 RID: 1830
	// (get) Token: 0x06003E47 RID: 15943 RVA: 0x00112309 File Offset: 0x00110509
	// (set) Token: 0x06003E48 RID: 15944 RVA: 0x00112311 File Offset: 0x00110511
	public InventoryToolCrest Crest { get; private set; }

	// Token: 0x17000727 RID: 1831
	// (get) Token: 0x06003E49 RID: 15945 RVA: 0x0011231A File Offset: 0x0011051A
	// (set) Token: 0x06003E4A RID: 15946 RVA: 0x00112322 File Offset: 0x00110522
	public int SlotIndex { get; private set; }

	// Token: 0x17000728 RID: 1832
	// (get) Token: 0x06003E4B RID: 15947 RVA: 0x0011232B File Offset: 0x0011052B
	public override string DisplayName
	{
		get
		{
			if (!this.EquippedItem)
			{
				return string.Empty;
			}
			return this.EquippedItem.DisplayName;
		}
	}

	// Token: 0x17000729 RID: 1833
	// (get) Token: 0x06003E4C RID: 15948 RVA: 0x00112350 File Offset: 0x00110550
	public override string Description
	{
		get
		{
			if (!this.EquippedItem)
			{
				return string.Empty;
			}
			return this.EquippedItem.Description;
		}
	}

	// Token: 0x1700072A RID: 1834
	// (get) Token: 0x06003E4D RID: 15949 RVA: 0x00112375 File Offset: 0x00110575
	public override Sprite Sprite
	{
		get
		{
			if (!this.EquippedItem)
			{
				return this.slotTypeSprite;
			}
			return this.EquippedItem.InventorySpriteBase;
		}
	}

	// Token: 0x1700072B RID: 1835
	// (get) Token: 0x06003E4E RID: 15950 RVA: 0x00112396 File Offset: 0x00110596
	private Sprite ItemSprite
	{
		get
		{
			if (!this.EquippedItem)
			{
				return null;
			}
			return this.EquippedItem.GetInventorySprite((this.EquippedItem.PoisonDamageTicks > 0 && this.IsToolEquipped(Gameplay.PoisonPouchTool)) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default);
		}
	}

	// Token: 0x1700072C RID: 1836
	// (get) Token: 0x06003E4F RID: 15951 RVA: 0x001123D4 File Offset: 0x001105D4
	public override Color SpriteTint
	{
		get
		{
			if (this.EquippedItem && this.itemIcon)
			{
				return Color.white;
			}
			if (this.IsLocked && (!this.manager || !this.manager.CanUnlockSlot))
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			if (this.slotTypeIcon)
			{
				return this.slotTypeIcon.Color;
			}
			return Color.white;
		}
	}

	// Token: 0x1700072D RID: 1837
	// (get) Token: 0x06003E50 RID: 15952 RVA: 0x0011245C File Offset: 0x0011065C
	public override Color? CursorColor
	{
		get
		{
			if (!this.manager)
			{
				return base.CursorColor;
			}
			if (this.isPulsingColour)
			{
				return new Color?(this.pulseColourA);
			}
			if (this.IsLocked)
			{
				return base.CursorColor;
			}
			return new Color?(this.manager.GetToolTypeColor(this.SlotInfo.Type));
		}
	}

	// Token: 0x1700072E RID: 1838
	// (get) Token: 0x06003E51 RID: 15953 RVA: 0x001124BB File Offset: 0x001106BB
	// (set) Token: 0x06003E52 RID: 15954 RVA: 0x001124E8 File Offset: 0x001106E8
	public float ItemFlashAmount
	{
		get
		{
			if (this.itemIcon)
			{
				return this.itemIcon.sharedMaterial.GetFloat(InventoryToolCrestSlot._flashAmountProp);
			}
			return 0f;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (this.itemIcon)
			{
				MaterialPropertyBlock materialPropertyBlock = this.Block;
				materialPropertyBlock.Clear();
				this.itemIcon.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(InventoryToolCrestSlot._flashAmountProp, value);
				this.itemIcon.SetPropertyBlock(materialPropertyBlock);
			}
			if (this.slotTypeIconGroup)
			{
				this.slotTypeIconGroup.AlphaSelf = 1f - value;
			}
			if (this.slotTypeIconFilled)
			{
				this.slotTypeIconFilled.AlphaSelf = (this.EquippedItem ? 0f : value);
			}
		}
	}

	// Token: 0x1700072F RID: 1839
	// (get) Token: 0x06003E53 RID: 15955 RVA: 0x00112587 File Offset: 0x00110787
	// (set) Token: 0x06003E54 RID: 15956 RVA: 0x0011258F File Offset: 0x0011078F
	public ToolItem EquippedItem { get; private set; }

	// Token: 0x17000730 RID: 1840
	// (get) Token: 0x06003E55 RID: 15957 RVA: 0x00112598 File Offset: 0x00110798
	public override ToolItem ItemData
	{
		get
		{
			return this.EquippedItem;
		}
	}

	// Token: 0x17000731 RID: 1841
	// (get) Token: 0x06003E56 RID: 15958 RVA: 0x001125A0 File Offset: 0x001107A0
	public ToolItemType Type
	{
		get
		{
			return this.SlotInfo.Type;
		}
	}

	// Token: 0x17000732 RID: 1842
	// (get) Token: 0x06003E57 RID: 15959 RVA: 0x001125B0 File Offset: 0x001107B0
	public bool IsLocked
	{
		get
		{
			return !(this.Crest == null) && !(this.Crest.CrestData == null) && this.slotInfo.IsLocked && !this.SaveData.IsUnlocked;
		}
	}

	// Token: 0x17000733 RID: 1843
	// (get) Token: 0x06003E58 RID: 15960 RVA: 0x00112600 File Offset: 0x00110800
	// (set) Token: 0x06003E59 RID: 15961 RVA: 0x00112668 File Offset: 0x00110868
	public ToolCrestsData.SlotData SaveData
	{
		get
		{
			if (this.getSavedDataOverride != null)
			{
				return this.getSavedDataOverride();
			}
			List<ToolCrestsData.SlotData> slots = PlayerData.instance.ToolEquips.GetData(this.Crest.name).Slots;
			if (slots == null || this.SlotIndex >= slots.Count)
			{
				return default(ToolCrestsData.SlotData);
			}
			return slots[this.SlotIndex];
		}
		private set
		{
			if (this.setSavedDataOverride != null)
			{
				this.setSavedDataOverride(value);
				return;
			}
			PlayerData instance = PlayerData.instance;
			ToolCrestsData.Data data = instance.ToolEquips.GetData(this.Crest.name);
			List<ToolCrestsData.SlotData> list = data.Slots;
			if (list == null)
			{
				list = (data.Slots = new List<ToolCrestsData.SlotData>());
				instance.ToolEquips.SetData(this.Crest.name, data);
			}
			while (list.Count < this.SlotIndex + 1)
			{
				list.Add(default(ToolCrestsData.SlotData));
			}
			list[this.SlotIndex] = value;
		}
	}

	// Token: 0x17000734 RID: 1844
	// (get) Token: 0x06003E5A RID: 15962 RVA: 0x00112704 File Offset: 0x00110904
	// (set) Token: 0x06003E5B RID: 15963 RVA: 0x0011270C File Offset: 0x0011090C
	public ToolCrest.SlotInfo SlotInfo
	{
		get
		{
			return this.slotInfo;
		}
		set
		{
			this.slotInfo = value;
			this.GetComponentsIfNeeded();
			if (this.itemIcon)
			{
				MaterialPropertyBlock materialPropertyBlock = this.Block;
				materialPropertyBlock.Clear();
				this.itemIcon.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetColor(InventoryToolCrestSlot._flashColorProp, this.manager.GetToolTypeColor(this.slotInfo.Type));
				this.itemIcon.SetPropertyBlock(materialPropertyBlock);
			}
			if (this.IsLocked && !this.spawnedUnlockBurstEffect && this.unlockBurstEffectPrefab)
			{
				PassColour passColour = Object.Instantiate<PassColour>(this.unlockBurstEffectPrefab, base.transform);
				passColour.gameObject.SetActive(false);
				passColour.transform.localPosition = Vector3.zero;
				this.spawnedUnlockBurstEffect = passColour;
			}
			if (this.spawnedUnlockBurstEffect)
			{
				this.spawnedUnlockBurstEffect.SetColour(this.manager.GetToolTypeColor(this.Type));
			}
			if (this.slotAnimator && this.slotInfo.Type.IsAttackType())
			{
				this.slotAnimator.runtimeAnimatorController = this.attackAnimatorControllers[(int)this.slotInfo.AttackBinding];
			}
		}
	}

	// Token: 0x17000735 RID: 1845
	// (get) Token: 0x06003E5C RID: 15964 RVA: 0x00112835 File Offset: 0x00110A35
	// (set) Token: 0x06003E5D RID: 15965 RVA: 0x0011283C File Offset: 0x00110A3C
	protected override bool IsSeen
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	// Token: 0x17000736 RID: 1846
	// (get) Token: 0x06003E5E RID: 15966 RVA: 0x00112843 File Offset: 0x00110A43
	protected override bool IsAutoNavSelectable
	{
		get
		{
			return this.wasVisible;
		}
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x0011284C File Offset: 0x00110A4C
	protected override void Awake()
	{
		base.Awake();
		if (this.itemIcon)
		{
			this.itemIcon.sprite = null;
		}
		if (this.itemIconMask)
		{
			this.itemIconMask.sprite = null;
		}
		this.GetComponentsIfNeeded();
		InventoryPaneBase componentInParent = base.GetComponentInParent<InventoryPaneBase>();
		if (componentInParent)
		{
			componentInParent.OnPaneEnd += delegate()
			{
				if (this.spawnedUnlockBurstEffect)
				{
					this.spawnedUnlockBurstEffect.gameObject.SetActive(false);
				}
			};
		}
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x001128B8 File Offset: 0x00110AB8
	protected override void OnValidate()
	{
		base.OnValidate();
		ArrayForEnumAttribute.EnsureArraySize<RuntimeAnimatorController>(ref this.attackAnimatorControllers, typeof(AttackToolBinding));
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x001128D5 File Offset: 0x00110AD5
	protected override void OnEnable()
	{
		base.OnEnable();
		this.previousAnimId = -1;
		if (this.queuedAnimId != 0)
		{
			this.PlayAnim(this.queuedAnimId);
		}
		if (this.queuedSmallAnimId != 0)
		{
			this.PlayAnimSmall(this.queuedSmallAnimId);
		}
	}

	// Token: 0x06003E62 RID: 15970 RVA: 0x0011290C File Offset: 0x00110B0C
	protected override void OnDisable()
	{
		base.OnDisable();
		this.queuedAnimId = 0;
		this.queuedSmallAnimId = 0;
	}

	// Token: 0x06003E63 RID: 15971 RVA: 0x00112922 File Offset: 0x00110B22
	public void SetIsVisible(bool isVisible)
	{
		this.wasVisible = isVisible;
		base.EvaluateAutoNav();
	}

	// Token: 0x06003E64 RID: 15972 RVA: 0x00112934 File Offset: 0x00110B34
	protected override void Update()
	{
		base.Update();
		if (this.isPulsingColour)
		{
			this.pulseColourTimeElapsed += Time.unscaledDeltaTime;
			if (this.pulseColourTimeElapsed > this.unlockReadyColorPulseDuration)
			{
				this.pulseColourTimeElapsed %= this.unlockReadyColorPulseDuration;
			}
			float t = this.unlockReadyColourPulseCurve.Evaluate(this.pulseColourTimeElapsed / this.unlockReadyColorPulseDuration);
			Color color = Color.Lerp(this.pulseColourA, this.pulseColourB, t);
			float groupAlpha = Mathf.Lerp(0.5f, 1f, t);
			this.SetSlotColour(color, groupAlpha, false);
			if (this.isSelected)
			{
				this.UpdateDisplay();
			}
		}
	}

	// Token: 0x06003E65 RID: 15973 RVA: 0x001129DC File Offset: 0x00110BDC
	private void GetComponentsIfNeeded()
	{
		if (!this.manager)
		{
			this.manager = base.GetComponentInParent<InventoryItemToolManager>();
			if (this.manager)
			{
				InventoryItemToolManager inventoryItemToolManager = this.manager;
				inventoryItemToolManager.OnToolRefresh = (Action<bool>)Delegate.Combine(inventoryItemToolManager.OnToolRefresh, new Action<bool>(this.UpdateSlotDisplay));
			}
			else
			{
				Debug.LogWarningFormat(this, "Tool Slot \"{0}\" couldn't find parent manager!", new object[]
				{
					base.gameObject.name
				});
			}
		}
		if (!this.Crest)
		{
			this.Crest = base.GetComponentInParent<InventoryToolCrest>();
		}
		if (!this.crestList)
		{
			this.crestList = base.GetComponentInParent<InventoryToolCrestList>();
		}
	}

	// Token: 0x06003E66 RID: 15974 RVA: 0x00112A89 File Offset: 0x00110C89
	public void SetCrestInfo(InventoryToolCrest crest, int slotIndex, Func<ToolCrestsData.SlotData> getSavedDataOverrideFunc = null, Action<ToolCrestsData.SlotData> setSavedDataOverrideAction = null)
	{
		this.Crest = crest;
		this.SlotIndex = slotIndex;
		this.getSavedDataOverride = getSavedDataOverrideFunc;
		this.setSavedDataOverride = setSavedDataOverrideAction;
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x00112AA8 File Offset: 0x00110CA8
	public void PreOpenSlot()
	{
		if (!this.EquippedItem)
		{
			this.PlayAnim(InventoryToolCrestSlot._equipAnim);
		}
		this.isPreOpened = true;
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x00112ACC File Offset: 0x00110CCC
	public void SetEquipped(ToolItem toolItem, bool isManual, bool refreshTools)
	{
		this.GetComponentsIfNeeded();
		bool flag = this.EquippedItem != toolItem;
		this.EquippedItem = toolItem;
		if (flag)
		{
			base.ItemDataUpdated();
		}
		if (isManual)
		{
			if (this.OnSetEquipSaved != null)
			{
				this.OnSetEquipSaved();
			}
			if (this.slotAnimator)
			{
				if (toolItem)
				{
					if (!this.isPreOpened)
					{
						this.PlayAnim(InventoryToolCrestSlot._equipAnim);
					}
				}
				else
				{
					this.PlayAnim(InventoryToolCrestSlot._unequipAnim);
				}
				this.isPreOpened = false;
			}
			if (refreshTools)
			{
				this.manager.RefreshTools();
			}
		}
		else
		{
			this.PlaySlotStateAnims(this.IsLocked, this.manager.CanUnlockSlot, true);
		}
		this.RefreshIcon();
		this.UpdateDisplay();
	}

	// Token: 0x06003E69 RID: 15977 RVA: 0x00112B80 File Offset: 0x00110D80
	public override bool Submit()
	{
		this.GetComponentsIfNeeded();
		if (!this.manager)
		{
			return false;
		}
		if (this.IsLocked)
		{
			if (this.EquippedItem)
			{
				this.manager.UnequipTool(this.EquippedItem, this);
				return true;
			}
			if (this.manager.CanUnlockSlot)
			{
				this.unlockHoldRoutine = base.StartCoroutine(this.UnlockHoldRoutine());
				this.UpdateSlotDisplay(false);
				return true;
			}
			return false;
		}
		else
		{
			if (!this.manager.CanChangeEquips(this.Type, InventoryItemToolManager.CanChangeEquipsTypes.Regular))
			{
				return false;
			}
			if (this.manager.EquipState == InventoryItemToolManager.EquipStates.PlaceTool)
			{
				this.DoPlace();
				return true;
			}
			return base.Submit();
		}
	}

	// Token: 0x06003E6A RID: 15978 RVA: 0x00112C27 File Offset: 0x00110E27
	public override bool SubmitReleased()
	{
		return this.TryCancelUnlockHold() || base.SubmitReleased();
	}

	// Token: 0x06003E6B RID: 15979 RVA: 0x00112C3C File Offset: 0x00110E3C
	protected override bool DoPress()
	{
		InventoryItemToolManager.EquipStates equipState = this.manager.EquipState;
		if (equipState == InventoryItemToolManager.EquipStates.None)
		{
			if (this.EquippedItem)
			{
				this.manager.UnequipTool(this.EquippedItem, this);
			}
			else
			{
				this.manager.StartSelection(this);
			}
			return true;
		}
		if (equipState != InventoryItemToolManager.EquipStates.PlaceTool)
		{
			return false;
		}
		this.DoPlace();
		return true;
	}

	// Token: 0x06003E6C RID: 15980 RVA: 0x00112C96 File Offset: 0x00110E96
	private void DoPlace()
	{
		if (this.manager.IsHoldingTool)
		{
			this.manager.PlaceTool(this, true);
		}
	}

	// Token: 0x06003E6D RID: 15981 RVA: 0x00112CB4 File Offset: 0x00110EB4
	public override bool Cancel()
	{
		this.GetComponentsIfNeeded();
		if (!this.manager)
		{
			return base.Cancel();
		}
		if (this.manager.ShowingToolMsg)
		{
			this.manager.HideToolEquipMsg(false);
			return false;
		}
		if (this.manager.IsHoldingTool)
		{
			this.manager.PlayMoveSound();
			this.manager.PlaceTool(null, false);
			return true;
		}
		return base.Cancel();
	}

	// Token: 0x06003E6E RID: 15982 RVA: 0x00112D24 File Offset: 0x00110F24
	public override InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction)
	{
		InventoryItemSelectable nextFallbackSelectable;
		InventoryToolCrestSlot inventoryToolCrestSlot;
		this.GetNextSelectableAndSlot(direction, out nextFallbackSelectable, out inventoryToolCrestSlot);
		if (!this.manager || this.manager.EquipState != InventoryItemToolManager.EquipStates.PlaceTool)
		{
			return nextFallbackSelectable;
		}
		if (!inventoryToolCrestSlot)
		{
			return this.GetSlotFromAutoNavGroup(direction, this.Type);
		}
		if (this.IsSlotInvalid(this.Type, inventoryToolCrestSlot))
		{
			inventoryToolCrestSlot = inventoryToolCrestSlot.GetNextSlotOfType(direction, this.Type);
		}
		if (inventoryToolCrestSlot == null)
		{
			nextFallbackSelectable = base.GetNextFallbackSelectable(direction);
			inventoryToolCrestSlot = (nextFallbackSelectable as InventoryToolCrestSlot);
			if (inventoryToolCrestSlot == null)
			{
				return this.GetSlotFromAutoNavGroup(direction, this.Type);
			}
			if (inventoryToolCrestSlot.Type != this.Type)
			{
				inventoryToolCrestSlot = inventoryToolCrestSlot.GetNextSlotOfType(direction, this.Type);
			}
		}
		if (!inventoryToolCrestSlot)
		{
			return this.GetSlotFromAutoNavGroup(direction, this.Type);
		}
		return inventoryToolCrestSlot;
	}

	// Token: 0x06003E6F RID: 15983 RVA: 0x00112DF4 File Offset: 0x00110FF4
	private InventoryItemSelectable GetSlotFromAutoNavGroup(InventoryItemManager.SelectionDirection direction, ToolItemType type)
	{
		return base.GetSelectableFromAutoNavGroup<InventoryToolCrestSlot>(direction, (InventoryToolCrestSlot slot) => !this.IsSlotInvalid(type, slot));
	}

	// Token: 0x06003E70 RID: 15984 RVA: 0x00112E28 File Offset: 0x00111028
	private InventoryToolCrestSlot GetNextSlotOfType(InventoryItemManager.SelectionDirection direction, ToolItemType type)
	{
		InventoryItemSelectable inventoryItemSelectable;
		InventoryToolCrestSlot nextSlotOfType;
		this.GetNextSelectableAndSlot(direction, out inventoryItemSelectable, out nextSlotOfType);
		if (nextSlotOfType && this.IsSlotInvalid(type, nextSlotOfType))
		{
			nextSlotOfType = nextSlotOfType.GetNextSlotOfType(direction, type);
		}
		return nextSlotOfType;
	}

	// Token: 0x06003E71 RID: 15985 RVA: 0x00112E5C File Offset: 0x0011105C
	private void GetNextSelectableAndSlot(InventoryItemManager.SelectionDirection direction, out InventoryItemSelectable nextSelectable, out InventoryToolCrestSlot nextSlot)
	{
		nextSelectable = base.GetNextSelectable(direction);
		nextSlot = (nextSelectable ? (nextSelectable.Get(new InventoryItemManager.SelectionDirection?(direction)) as InventoryToolCrestSlot) : null);
	}

	// Token: 0x06003E72 RID: 15986 RVA: 0x00112E87 File Offset: 0x00111087
	private bool IsSlotInvalid(ToolItemType type, InventoryToolCrestSlot nextSlot)
	{
		return nextSlot.Type != type || (nextSlot.IsLocked && !this.manager.CanUnlockSlot);
	}

	// Token: 0x06003E73 RID: 15987 RVA: 0x00112EAC File Offset: 0x001110AC
	private void UpdateSlotDisplay(bool isInstant)
	{
		int frameCount = Time.frameCount;
		if (this.lastUpdate == frameCount && this.lastEquipState == this.manager.EquipState && this.isSelected == this.lastSelectState)
		{
			return;
		}
		this.lastEquipState = this.manager.EquipState;
		this.lastSelectState = this.isSelected;
		this.lastUpdate = frameCount;
		Color toolTypeColor = this.manager.GetToolTypeColor(this.Type);
		float h;
		float num;
		float v;
		Color.RGBToHSV(toolTypeColor, out h, out num, out v);
		Color color = Color.HSVToRGB(h, num * 0.4f, v);
		Color color2 = Color.HSVToRGB(h, 0f, v);
		bool flag;
		bool flag2;
		bool flag3;
		bool fadeAlpha;
		if (this.Crest)
		{
			flag = (this.crestList.CurrentCrest == this.Crest);
			flag2 = this.IsLocked;
			flag3 = (flag2 && this.manager.CanUnlockSlot);
			fadeAlpha = this.crestList.IsSetupComplete;
		}
		else
		{
			flag = true;
			flag2 = false;
			flag3 = false;
			fadeAlpha = true;
		}
		this.PlaySlotStateAnims(flag2, flag3, false);
		if (this.wasSelected)
		{
			bool flag4 = this.manager.EquipState == InventoryItemToolManager.EquipStates.SwitchCrest;
		}
		bool flag5 = this.isSelected && (this.wasSelected || this.manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest);
		this.wasSelected = flag5;
		float groupAlpha;
		Color color3;
		if (flag2)
		{
			if (flag3 && flag5)
			{
				groupAlpha = 1f;
			}
			else
			{
				InventoryItemToolManager.EquipStates equipState = this.manager.EquipState;
				groupAlpha = ((equipState == InventoryItemToolManager.EquipStates.PlaceTool || equipState == InventoryItemToolManager.EquipStates.SelectTool) ? 0.3f : 0.5f);
			}
			color3 = Color.white;
		}
		else if (this.manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest && flag)
		{
			groupAlpha = 1f;
			color3 = Color.white;
			switch (this.manager.EquipState)
			{
			case InventoryItemToolManager.EquipStates.None:
				if (this.manager.HoveringTool && this.manager.HoveringTool.ToolType != this.Type)
				{
					groupAlpha = 0.3f;
				}
				break;
			case InventoryItemToolManager.EquipStates.PlaceTool:
				if (this.manager.PickedUpTool && this.manager.PickedUpTool.Type != this.Type)
				{
					groupAlpha = 0.3f;
					color3 = InventoryToolCrestSlot.InvalidItemColor;
				}
				break;
			case InventoryItemToolManager.EquipStates.SelectTool:
				if (this.manager.SelectedSlot && this.manager.SelectedSlot != this)
				{
					groupAlpha = 0.3f;
					color3 = InventoryToolCrestSlot.InvalidItemColor;
				}
				break;
			}
		}
		else
		{
			groupAlpha = 1f;
			color3 = Color.white;
		}
		if (this.unlockHoldRoutine != null)
		{
			this.isPulsingColour = false;
			this.SetSlotColour(toolTypeColor, 1f, fadeAlpha);
		}
		else if (flag3 && flag5)
		{
			if (!this.isPulsingColour)
			{
				this.isPulsingColour = true;
				this.pulseColourA = color;
				this.pulseColourB = toolTypeColor;
				this.pulseColourTimeElapsed = 0f;
				this.SetSlotColour(this.pulseColourA, 0.5f, fadeAlpha);
			}
		}
		else
		{
			this.isPulsingColour = false;
			this.SetSlotColour(flag2 ? color2 : toolTypeColor, groupAlpha, fadeAlpha);
		}
		if (this.amountText)
		{
			if (flag && this.EquippedItem && this.EquippedItem.DisplayAmountText)
			{
				ToolItemsData.Data toolData = PlayerData.instance.GetToolData(this.EquippedItem.name);
				this.amountText.text = toolData.AmountLeft.ToString();
				this.amountText.color = color3;
				this.amountText.gameObject.SetActive(true);
			}
			else
			{
				this.amountText.gameObject.SetActive(false);
			}
		}
		(this.slotTypeIconGroup ? this.slotTypeIconGroup.transform : base.transform).localScale = (flag2 ? new Vector3(0.8f, 0.8f, 1f) : Vector3.one);
		if (this.itemIcon)
		{
			this.itemIcon.color = base.UpdateGetIconColour(this.itemIcon, color3, !isInstant);
		}
		this.RefreshIcon();
		this.UpdateDisplay();
	}

	// Token: 0x06003E74 RID: 15988 RVA: 0x001132C8 File Offset: 0x001114C8
	protected override bool IsToolEquipped(ToolItem toolItem)
	{
		InventoryToolCrest crest = this.Crest;
		if (!crest)
		{
			return false;
		}
		if (!this.crestList.IsSwitchingCrests && crest == this.crestList.CurrentCrest)
		{
			return toolItem.IsEquippedHud;
		}
		return crest.GetEquippedToolSlot(toolItem);
	}

	// Token: 0x06003E75 RID: 15989 RVA: 0x0011331C File Offset: 0x0011151C
	private void RefreshIcon()
	{
		Sprite itemSprite = this.ItemSprite;
		if (this.itemIcon)
		{
			this.itemIcon.sprite = itemSprite;
		}
		if (this.itemIconMask)
		{
			this.itemIconMask.sprite = itemSprite;
		}
	}

	// Token: 0x06003E76 RID: 15990 RVA: 0x00113364 File Offset: 0x00111564
	private void PlaySlotStateAnims(bool isLocked, bool isUnlockReady, bool force)
	{
		if (!isLocked)
		{
			if (this.EquippedItem)
			{
				if (force || this.previousAnimId != InventoryToolCrestSlot._equipAnim)
				{
					this.PlayAnim(InventoryToolCrestSlot._fullAnim);
				}
			}
			else if (force || this.previousAnimId != InventoryToolCrestSlot._unequipAnim)
			{
				this.PlayAnim(InventoryToolCrestSlot._emptyAnim);
			}
			this.PlayAnimSmall(InventoryToolCrestSlot._filledAnim);
			return;
		}
		if (isUnlockReady)
		{
			this.PlayAnim((this.isSelected && this.manager.EquipState != InventoryItemToolManager.EquipStates.SwitchCrest) ? InventoryToolCrestSlot._unlockReadySelectedAnim : InventoryToolCrestSlot._unlockReadyIdleAnim);
			this.PlayAnimSmall(InventoryToolCrestSlot._lockedAnim);
			return;
		}
		this.PlayAnim(InventoryToolCrestSlot._lockedAnim);
		this.PlayAnimSmall(InventoryToolCrestSlot._lockedAnim);
	}

	// Token: 0x06003E77 RID: 15991 RVA: 0x00113412 File Offset: 0x00111612
	private void PlayAnim(int animId)
	{
		if (this.slotAnimator && this.slotAnimator.isActiveAndEnabled)
		{
			this.slotAnimator.Play(animId);
		}
		else
		{
			this.queuedAnimId = animId;
		}
		this.previousAnimId = animId;
	}

	// Token: 0x06003E78 RID: 15992 RVA: 0x0011344A File Offset: 0x0011164A
	private void PlayAnimSmall(int animId)
	{
		if (this.slotFilledAnimator && this.slotFilledAnimator.isActiveAndEnabled)
		{
			this.slotFilledAnimator.Play(animId);
			return;
		}
		this.queuedSmallAnimId = animId;
	}

	// Token: 0x06003E79 RID: 15993 RVA: 0x0011347C File Offset: 0x0011167C
	private void SetSlotColour(Color color, float groupAlpha, bool fadeAlpha)
	{
		if (this.slotTypeIcon)
		{
			this.slotTypeIcon.Color = color;
		}
		if (this.slotTypeIconFilled)
		{
			this.slotTypeIconFilled.BaseColor = color;
		}
		if (this.slotTypeGroup)
		{
			if (fadeAlpha)
			{
				this.slotTypeGroup.FadeTo(groupAlpha, 0.1f, null, true, null);
				return;
			}
			this.slotTypeGroup.AlphaSelf = groupAlpha;
		}
	}

	// Token: 0x06003E7A RID: 15994 RVA: 0x001134ED File Offset: 0x001116ED
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		if (!this.isSelected)
		{
			this.isSelected = true;
			this.UpdateSlotDisplay(false);
		}
		base.Select(direction);
	}

	// Token: 0x06003E7B RID: 15995 RVA: 0x0011350C File Offset: 0x0011170C
	public override void Deselect()
	{
		if (this.isSelected)
		{
			this.isSelected = false;
			this.UpdateSlotDisplay(false);
		}
		base.Deselect();
	}

	// Token: 0x06003E7C RID: 15996 RVA: 0x0011352A File Offset: 0x0011172A
	private IEnumerator UnlockHoldRoutine()
	{
		this.unlockHoldShakeTransform = (this.slotTypeGroup ? this.slotTypeGroup.transform : base.transform);
		this.unlockHoldInitialPosition = this.unlockHoldShakeTransform.localPosition;
		this.onUnlockHoldEnd = delegate()
		{
			this.crestList.IsBlocked = false;
			this.unlockHoldShakeTransform.localPosition = this.unlockHoldInitialPosition;
			if (this.unlockHoldParticles)
			{
				this.unlockHoldParticles.StopParticleSystems();
			}
		};
		this.crestList.IsBlocked = true;
		if (this.unlockHoldParticles)
		{
			this.unlockHoldParticles.PlayParticleSystems();
		}
		InventoryItemCollectable unlockItem = this.manager.SlotUnlockItemDisplay;
		CrestSocketUnlockInventoryDescription unlockDesc = this.manager.SocketUnlockInventoryDescription;
		unlockDesc.StartConsume();
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.016666668f);
		double beforeWaitTime;
		for (float elapsed = 0f; elapsed < this.unlockHoldDuration; elapsed += (float)(Time.unscaledTimeAsDouble - beforeWaitTime))
		{
			float num = elapsed / this.unlockHoldDuration;
			this.unlockHoldShakeTransform.localPosition = this.unlockHoldInitialPosition + Random.insideUnitCircle * this.unlockHoldShakeMagnitude.GetLerpedValue(num);
			unlockItem.SetConsumeShakeAmount(num, 1f);
			unlockDesc.SetConsumeShakeAmount(num);
			this.UpdateUnlockRumble(num);
			beforeWaitTime = Time.unscaledTimeAsDouble;
			yield return wait;
		}
		unlockItem.SetConsumeShakeAmount(0f, 1f);
		unlockDesc.SetConsumeShakeAmount(0f);
		unlockDesc.ConsumeCompleted();
		this.onUnlockHoldEnd();
		this.onUnlockHoldEnd = null;
		this.unlockHoldRoutine = null;
		ToolCrestsData.SlotData saveData = this.SaveData;
		saveData.IsUnlocked = true;
		this.SaveData = saveData;
		this.manager.SlotUnlockItem.Take(1, false);
		if (this.spawnedUnlockBurstEffect)
		{
			this.spawnedUnlockBurstEffect.gameObject.SetActive(false);
			this.spawnedUnlockBurstEffect.gameObject.SetActive(true);
		}
		unlockItem.PlayConsumeEffect();
		this.StopUnlockRumble();
		this.PlayFinalShake();
		this.UpdateSlotDisplay(false);
		this.manager.SetDisplay(this);
		this.manager.RefreshTools();
		yield break;
	}

	// Token: 0x06003E7D RID: 15997 RVA: 0x0011353C File Offset: 0x0011173C
	private bool TryCancelUnlockHold()
	{
		if (this.unlockHoldRoutine == null)
		{
			return false;
		}
		base.StopCoroutine(this.unlockHoldRoutine);
		this.unlockHoldRoutine = null;
		InventoryItemCollectable slotUnlockItemDisplay = this.manager.SlotUnlockItemDisplay;
		slotUnlockItemDisplay.SetConsumeShakeAmount(0f, 1f);
		slotUnlockItemDisplay.StopConsumeRumble();
		this.UpdateSlotDisplay(false);
		this.onUnlockHoldEnd();
		this.onUnlockHoldEnd = null;
		this.manager.SocketUnlockInventoryDescription.CancelConsume();
		this.StopUnlockRumble();
		return true;
	}

	// Token: 0x06003E7E RID: 15998 RVA: 0x001135B8 File Offset: 0x001117B8
	private void UpdateUnlockRumble(float strength)
	{
		if (this.consumeRumbleEmission == null)
		{
			this.consumeRumbleEmission = VibrationManager.PlayVibrationClipOneShot(this.unlockRumble, null, true, "", true);
		}
		VibrationEmission vibrationEmission = this.consumeRumbleEmission;
		if (vibrationEmission == null)
		{
			return;
		}
		vibrationEmission.SetStrength(strength);
	}

	// Token: 0x06003E7F RID: 15999 RVA: 0x00113604 File Offset: 0x00111804
	public void StopUnlockRumble()
	{
		VibrationEmission vibrationEmission = this.consumeRumbleEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.consumeRumbleEmission = null;
	}

	// Token: 0x06003E80 RID: 16000 RVA: 0x00113620 File Offset: 0x00111820
	private void PlayFinalShake()
	{
		VibrationManager.PlayVibrationClipOneShot(this.unlockShake, null, false, "", true);
	}

	// Token: 0x04003FEC RID: 16364
	private const float DISABLED_SLOT_OPACITY = 0.5f;

	// Token: 0x04003FED RID: 16365
	private const float WRONG_SLOT_OPACITY = 0.3f;

	// Token: 0x04003FEE RID: 16366
	private const float SLOT_FADE_DURATION = 0.1f;

	// Token: 0x04003FEF RID: 16367
	private const float LOCKED_SLOT_SCALE = 0.8f;

	// Token: 0x04003FF0 RID: 16368
	public static readonly Color InvalidItemColor = new Color(0.3f, 0.3f, 0.3f, 1f);

	// Token: 0x04003FF2 RID: 16370
	[Header("Tool Crest Slot")]
	[SerializeField]
	private Sprite slotTypeSprite;

	// Token: 0x04003FF3 RID: 16371
	[SerializeField]
	private NestedFadeGroupBase slotTypeGroup;

	// Token: 0x04003FF4 RID: 16372
	[SerializeField]
	private NestedFadeGroupBase slotTypeIconGroup;

	// Token: 0x04003FF5 RID: 16373
	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotTypeIcon;

	// Token: 0x04003FF6 RID: 16374
	[SerializeField]
	private NestedFadeGroupSpriteRenderer slotTypeIconFilled;

	// Token: 0x04003FF7 RID: 16375
	[SerializeField]
	private Animator slotFilledAnimator;

	// Token: 0x04003FF8 RID: 16376
	[SerializeField]
	private SpriteRenderer itemIcon;

	// Token: 0x04003FF9 RID: 16377
	[SerializeField]
	private SpriteMask itemIconMask;

	// Token: 0x04003FFA RID: 16378
	[SerializeField]
	private Animator slotAnimator;

	// Token: 0x04003FFB RID: 16379
	[SerializeField]
	[ArrayForEnum(typeof(AttackToolBinding))]
	private RuntimeAnimatorController[] attackAnimatorControllers;

	// Token: 0x04003FFC RID: 16380
	[SerializeField]
	private TextMeshPro amountText;

	// Token: 0x04003FFD RID: 16381
	[Space]
	[SerializeField]
	private AnimationCurve unlockReadyColourPulseCurve;

	// Token: 0x04003FFE RID: 16382
	[SerializeField]
	private float unlockReadyColorPulseDuration;

	// Token: 0x04003FFF RID: 16383
	[SerializeField]
	private float unlockHoldDuration;

	// Token: 0x04004000 RID: 16384
	[SerializeField]
	private MinMaxFloat unlockHoldShakeMagnitude;

	// Token: 0x04004001 RID: 16385
	[SerializeField]
	private PlayParticleEffects unlockHoldParticles;

	// Token: 0x04004002 RID: 16386
	[SerializeField]
	private PassColour unlockBurstEffectPrefab;

	// Token: 0x04004003 RID: 16387
	[Header("Vibrations")]
	[SerializeField]
	private VibrationDataAsset unlockRumble;

	// Token: 0x04004004 RID: 16388
	[SerializeField]
	private VibrationDataAsset unlockShake;

	// Token: 0x04004005 RID: 16389
	private bool isPreOpened;

	// Token: 0x04004006 RID: 16390
	private bool wasVisible;

	// Token: 0x04004007 RID: 16391
	[NonSerialized]
	private InventoryItemToolManager manager;

	// Token: 0x04004008 RID: 16392
	private InventoryToolCrestList crestList;

	// Token: 0x04004009 RID: 16393
	private int previousAnimId;

	// Token: 0x0400400A RID: 16394
	private bool isPulsingColour;

	// Token: 0x0400400B RID: 16395
	private Color pulseColourA;

	// Token: 0x0400400C RID: 16396
	private Color pulseColourB;

	// Token: 0x0400400D RID: 16397
	private float pulseColourTimeElapsed;

	// Token: 0x0400400E RID: 16398
	private Transform unlockHoldShakeTransform;

	// Token: 0x0400400F RID: 16399
	private Vector3 unlockHoldInitialPosition;

	// Token: 0x04004010 RID: 16400
	private Coroutine unlockHoldRoutine;

	// Token: 0x04004011 RID: 16401
	private Action onUnlockHoldEnd;

	// Token: 0x04004012 RID: 16402
	private Func<ToolCrestsData.SlotData> getSavedDataOverride;

	// Token: 0x04004013 RID: 16403
	private Action<ToolCrestsData.SlotData> setSavedDataOverride;

	// Token: 0x04004014 RID: 16404
	private bool isSelected;

	// Token: 0x04004015 RID: 16405
	private bool wasSelected;

	// Token: 0x04004016 RID: 16406
	private PassColour spawnedUnlockBurstEffect;

	// Token: 0x04004017 RID: 16407
	private VibrationEmission consumeRumbleEmission;

	// Token: 0x04004018 RID: 16408
	private int queuedAnimId;

	// Token: 0x04004019 RID: 16409
	private int queuedSmallAnimId;

	// Token: 0x0400401A RID: 16410
	private static readonly int _equipAnim = Animator.StringToHash("Equip");

	// Token: 0x0400401B RID: 16411
	private static readonly int _unequipAnim = Animator.StringToHash("Unequip");

	// Token: 0x0400401C RID: 16412
	private static readonly int _fullAnim = Animator.StringToHash("Full");

	// Token: 0x0400401D RID: 16413
	private static readonly int _emptyAnim = Animator.StringToHash("Empty");

	// Token: 0x0400401E RID: 16414
	private static readonly int _lockedAnim = Animator.StringToHash("Locked");

	// Token: 0x0400401F RID: 16415
	private static readonly int _unlockReadyIdleAnim = Animator.StringToHash("Unlock Ready Idle");

	// Token: 0x04004020 RID: 16416
	private static readonly int _unlockReadySelectedAnim = Animator.StringToHash("Unlock Ready Selected");

	// Token: 0x04004021 RID: 16417
	private static readonly int _filledAnim = Animator.StringToHash("Filled");

	// Token: 0x04004022 RID: 16418
	private static readonly int _flashAmountProp = Shader.PropertyToID("_FlashAmount");

	// Token: 0x04004023 RID: 16419
	private static readonly int _flashColorProp = Shader.PropertyToID("_FlashColor");

	// Token: 0x04004024 RID: 16420
	private MaterialPropertyBlock block;

	// Token: 0x04004028 RID: 16424
	private ToolCrest.SlotInfo slotInfo;

	// Token: 0x04004029 RID: 16425
	private ToolItem itemData;

	// Token: 0x0400402A RID: 16426
	private int lastUpdate;

	// Token: 0x0400402B RID: 16427
	private InventoryItemToolManager.EquipStates lastEquipState;

	// Token: 0x0400402C RID: 16428
	private bool lastSelectState;
}
