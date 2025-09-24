using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000681 RID: 1665
public class InventoryFloatingToolSlots : MonoBehaviour
{
	// Token: 0x06003B8F RID: 15247 RVA: 0x00105F60 File Offset: 0x00104160
	private void Awake()
	{
		this.initialPos = base.transform.localPosition;
		InventoryPane componentInParent = base.GetComponentInParent<InventoryPane>();
		if (componentInParent)
		{
			componentInParent.OnPrePaneStart += this.Evaluate;
		}
		this.Evaluate();
	}

	// Token: 0x06003B90 RID: 15248 RVA: 0x00105FAC File Offset: 0x001041AC
	private void Evaluate()
	{
		this.currentConfig = null;
		InventoryFloatingToolSlots.Slot[] slots;
		foreach (InventoryFloatingToolSlots.Config config in this.configs)
		{
			slots = config.Slots;
			for (int j = 0; j < slots.Length; j++)
			{
				slots[j].SlotObject.OnSetEquipSaved -= this.SaveEquips;
			}
			if (config.Condition.IsFulfilled)
			{
				this.currentConfig = config;
			}
		}
		bool isEquipped = Gameplay.CursedCrest.IsEquipped;
		if (this.currentConfig == null || isEquipped)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		base.transform.SetLocalPosition2D(this.initialPos + this.currentConfig.PositionOffset.MultiplyElements(base.transform.localScale));
		GameObject[] brackets;
		foreach (InventoryFloatingToolSlots.Config config2 in this.configs)
		{
			brackets = config2.Brackets;
			for (int j = 0; j < brackets.Length; j++)
			{
				brackets[j].SetActive(false);
			}
			foreach (InventoryFloatingToolSlots.Slot slot2 in config2.Slots)
			{
				slot2.SlotObject.SetIsVisible(false);
				slot2.SlotObject.gameObject.SetActive(false);
				slot2.CursedSlot.SetActive(false);
			}
		}
		brackets = this.currentConfig.Brackets;
		for (int i = 0; i < brackets.Length; i++)
		{
			brackets[i].SetActive(true);
		}
		slots = this.currentConfig.Slots;
		for (int i = 0; i < slots.Length; i++)
		{
			InventoryFloatingToolSlots.Slot slot = slots[i];
			slot.SlotObject.gameObject.SetActive(true);
			slot.SlotObject.SetIsVisible(true);
			slot.SlotObject.ItemFlashAmount = 0f;
			slot.SlotObject.SlotInfo = new ToolCrest.SlotInfo
			{
				Type = slot.Type
			};
			slot.SlotObject.SetCrestInfo(null, -1, () => PlayerData.instance.ExtraToolEquips.GetData(slot.Id), delegate(ToolCrestsData.SlotData data)
			{
				PlayerData.instance.ExtraToolEquips.SetData(slot.Id, data);
			});
			string equippedTool = PlayerData.instance.ExtraToolEquips.GetData(slot.Id).EquippedTool;
			ToolItem toolItem = string.IsNullOrEmpty(equippedTool) ? null : ToolItemManager.GetToolByName(equippedTool);
			slot.SlotObject.SetEquipped(toolItem, false, false);
			slot.SlotObject.OnSetEquipSaved += this.SaveEquips;
		}
	}

	// Token: 0x06003B91 RID: 15249 RVA: 0x00106274 File Offset: 0x00104474
	private void SaveEquips()
	{
		if (this.currentConfig == null)
		{
			return;
		}
		foreach (InventoryFloatingToolSlots.Slot slot in this.currentConfig.Slots)
		{
			ToolItemManager.SetExtraEquippedTool(slot.Id, slot.SlotObject.EquippedItem);
		}
	}

	// Token: 0x06003B92 RID: 15250 RVA: 0x001062C0 File Offset: 0x001044C0
	public InventoryToolCrestSlot GetEquippedToolSlot(ToolItem toolItem)
	{
		if (this.currentConfig == null || !toolItem)
		{
			return null;
		}
		return (from slot in this.currentConfig.Slots
		select slot.SlotObject).FirstOrDefault((InventoryToolCrestSlot slot) => slot.EquippedItem == toolItem);
	}

	// Token: 0x06003B93 RID: 15251 RVA: 0x00106331 File Offset: 0x00104531
	public IEnumerable<InventoryToolCrestSlot> GetSlots()
	{
		if (this.currentConfig == null)
		{
			return Enumerable.Empty<InventoryToolCrestSlot>();
		}
		return from slot in this.currentConfig.Slots
		select slot.SlotObject;
	}

	// Token: 0x06003B94 RID: 15252 RVA: 0x00106370 File Offset: 0x00104570
	public void SetInEquipMode(bool value)
	{
		this.bracketsFader.FadeTo(value ? InventoryToolCrestSlot.InvalidItemColor.r : InventoryToolCrest.DeselectedColor.r, 0.15f, null, true, null);
	}

	// Token: 0x04003DC6 RID: 15814
	[SerializeField]
	private InventoryFloatingToolSlots.Config[] configs;

	// Token: 0x04003DC7 RID: 15815
	[SerializeField]
	private NestedFadeGroupBase bracketsFader;

	// Token: 0x04003DC8 RID: 15816
	private InventoryFloatingToolSlots.Config currentConfig;

	// Token: 0x04003DC9 RID: 15817
	private Vector2 initialPos;

	// Token: 0x02001987 RID: 6535
	[Serializable]
	private class Slot
	{
		// Token: 0x04009631 RID: 38449
		public InventoryToolCrestSlot SlotObject;

		// Token: 0x04009632 RID: 38450
		public ToolItemType Type;

		// Token: 0x04009633 RID: 38451
		public string Id;

		// Token: 0x04009634 RID: 38452
		public GameObject CursedSlot;
	}

	// Token: 0x02001988 RID: 6536
	[Serializable]
	private class Config
	{
		// Token: 0x04009635 RID: 38453
		public InventoryFloatingToolSlots.Slot[] Slots;

		// Token: 0x04009636 RID: 38454
		public GameObject[] Brackets;

		// Token: 0x04009637 RID: 38455
		public Vector2 PositionOffset;

		// Token: 0x04009638 RID: 38456
		public PlayerDataTest Condition;
	}
}
