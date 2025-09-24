using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005EF RID: 1519
public class ToolItemManager : ManagerSingleton<ToolItemManager>
{
	// Token: 0x140000A7 RID: 167
	// (add) Token: 0x060035FA RID: 13818 RVA: 0x000EDDDC File Offset: 0x000EBFDC
	// (remove) Token: 0x060035FB RID: 13819 RVA: 0x000EDE10 File Offset: 0x000EC010
	public static event Action<AttackToolBinding> BoundAttackToolUpdated;

	// Token: 0x140000A8 RID: 168
	// (add) Token: 0x060035FC RID: 13820 RVA: 0x000EDE44 File Offset: 0x000EC044
	// (remove) Token: 0x060035FD RID: 13821 RVA: 0x000EDE78 File Offset: 0x000EC078
	public static event Action<AttackToolBinding> BoundAttackToolUsed;

	// Token: 0x140000A9 RID: 169
	// (add) Token: 0x060035FE RID: 13822 RVA: 0x000EDEAC File Offset: 0x000EC0AC
	// (remove) Token: 0x060035FF RID: 13823 RVA: 0x000EDEE0 File Offset: 0x000EC0E0
	public static event Action<AttackToolBinding> BoundAttackToolFailed;

	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x06003600 RID: 13824 RVA: 0x000EDF13 File Offset: 0x000EC113
	public static bool IsCustomToolOverride
	{
		get
		{
			return ManagerSingleton<ToolItemManager>.Instance && ManagerSingleton<ToolItemManager>.Instance.customToolOverride;
		}
	}

	// Token: 0x17000627 RID: 1575
	// (get) Token: 0x06003601 RID: 13825 RVA: 0x000EDF32 File Offset: 0x000EC132
	public static ToolsActiveStates ActiveState
	{
		get
		{
			return ToolItemManager.activeState;
		}
	}

	// Token: 0x17000628 RID: 1576
	// (get) Token: 0x06003602 RID: 13826 RVA: 0x000EDF39 File Offset: 0x000EC139
	public static bool IsInCutscene
	{
		get
		{
			return ToolItemManager.ActiveState == ToolsActiveStates.Cutscene;
		}
	}

	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x06003603 RID: 13827 RVA: 0x000EDF43 File Offset: 0x000EC143
	// (set) Token: 0x06003604 RID: 13828 RVA: 0x000EDF5D File Offset: 0x000EC15D
	public static ToolItem UnlockedTool
	{
		get
		{
			if (!ManagerSingleton<ToolItemManager>.Instance)
			{
				return null;
			}
			return ManagerSingleton<ToolItemManager>.Instance.unlockedTool;
		}
		set
		{
			if (ManagerSingleton<ToolItemManager>.Instance)
			{
				ManagerSingleton<ToolItemManager>.Instance.unlockedTool = value;
			}
		}
	}

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x06003605 RID: 13829 RVA: 0x000EDF76 File Offset: 0x000EC176
	// (set) Token: 0x06003606 RID: 13830 RVA: 0x000EDF7D File Offset: 0x000EC17D
	public static bool IsCursed { get; private set; }

	// Token: 0x1700062B RID: 1579
	// (get) Token: 0x06003607 RID: 13831 RVA: 0x000EDF85 File Offset: 0x000EC185
	// (set) Token: 0x06003608 RID: 13832 RVA: 0x000EDF8C File Offset: 0x000EC18C
	public static bool IsInfiniteToolUseEnabled
	{
		get
		{
			return ToolItemManager._sIsInfiniteToolUseEnabled;
		}
		set
		{
			ToolItemManager._sIsInfiniteToolUseEnabled = value;
			if (!ToolItemManager._sIsInfiniteToolUseEnabled)
			{
				return;
			}
			if (!PlayerData.HasInstance)
			{
				return;
			}
			ToolItemsData tools = PlayerData.instance.Tools;
			foreach (ToolItem toolItem in ToolItemManager.GetUnlockedTools())
			{
				ToolItemsData.Data data = tools.GetData(toolItem.name);
				data.AmountLeft = ToolItemManager.GetToolStorageAmount(toolItem);
				tools.SetData(toolItem.name, data);
				AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(toolItem);
				if (attackToolBinding != null)
				{
					ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
				}
			}
		}
	}

	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x06003609 RID: 13833 RVA: 0x000EE038 File Offset: 0x000EC238
	// (set) Token: 0x0600360A RID: 13834 RVA: 0x000EE03F File Offset: 0x000EC23F
	public static int Version { get; private set; }

	// Token: 0x140000AA RID: 170
	// (add) Token: 0x0600360B RID: 13835 RVA: 0x000EE048 File Offset: 0x000EC248
	// (remove) Token: 0x0600360C RID: 13836 RVA: 0x000EE07C File Offset: 0x000EC27C
	public static event Action OnEquippedStateChanged;

	// Token: 0x0600360D RID: 13837 RVA: 0x000EE0AF File Offset: 0x000EC2AF
	public static void IncrementVersion()
	{
		ToolItemManager.Version++;
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x000EE0C0 File Offset: 0x000EC2C0
	protected override void Awake()
	{
		base.Awake();
		GameManager.instance.NextSceneWillActivate += this.OnDestroyPersonalPools;
		GameManager.instance.SceneInit += this.SceneInit;
		if (ManagerSingleton<ToolItemManager>.Instance == this)
		{
			ToolItemManager.activeState = ToolsActiveStates.Active;
		}
		ToolItemManager.IncrementVersion();
		if (this.cursedCrest == null && this.crestList != null)
		{
			foreach (ToolCrest toolCrest in this.crestList)
			{
				if (toolCrest.name == "Cursed")
				{
					this.cursedCrest = toolCrest;
					break;
				}
			}
		}
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x000EE188 File Offset: 0x000EC388
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.NextSceneWillActivate -= this.OnDestroyPersonalPools;
			GameManager.UnsafeInstance.SceneInit -= this.SceneInit;
		}
		ToolItemManager.ClearToolCache();
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x000EE1D8 File Offset: 0x000EC3D8
	private void SceneInit()
	{
		if (this.toolOverrideReminder == null)
		{
			this.toolOverrideReminder = new ControlReminder.SingleConfig
			{
				AppearEvent = "SHOW CUSTOM TOOL REMINDER",
				DisappearEvent = "HIDE CUSTOM TOOL REMINDER",
				FadeInDelay = 8f,
				FadeInTime = 1f,
				FadeOutTime = 0.5f,
				DisappearOnButtonPress = true,
				Button = HeroActionButton.QUICK_CAST
			};
		}
		ControlReminder.AddReminder(this.toolOverrideReminder, false);
		LocalisedString text = new LocalisedString("Prompts", "THROW_TOOL_GENERIC");
		if (this.equipChangedToolSingleReminder == null)
		{
			this.equipChangedToolSingleReminder = new ControlReminder.SingleConfig
			{
				Text = text,
				FadeInDelay = 0.5f,
				FadeInTime = 1f,
				FadeOutTime = 0.5f,
				DisappearOnButtonPress = true,
				Button = HeroActionButton.QUICK_CAST
			};
		}
		ControlReminder.AddReminder(this.equipChangedToolSingleReminder, false);
		if (this.equipChangedToolModifierReminder == null)
		{
			this.equipChangedToolModifierReminder = new ControlReminder.DoubleConfig
			{
				Text = text,
				FadeInDelay = 0.5f,
				FadeInTime = 1f,
				FadeOutTime = 0.5f,
				Button1 = HeroActionButton.QUICK_CAST
			};
		}
		ControlReminder.AddReminder(this.equipChangedToolModifierReminder, false);
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x000EE300 File Offset: 0x000EC500
	private void OnDestroyPersonalPools()
	{
		ToolItemManager.RefreshEquippedState();
		if (!GameManager.instance.IsInSceneTransition)
		{
			return;
		}
		ToolItemManager.ClearCustomToolOverride();
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x000EE31C File Offset: 0x000EC51C
	public static IEnumerable<ToolItem> GetUnlockedTools()
	{
		if (!ManagerSingleton<ToolItemManager>.Instance)
		{
			return Enumerable.Empty<ToolItem>();
		}
		return from tool in ManagerSingleton<ToolItemManager>.Instance.toolItems
		where tool && tool.IsUnlockedNotHidden
		select tool;
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x000EE36C File Offset: 0x000EC56C
	public static List<ToolCrest> GetAllCrests()
	{
		if (ManagerSingleton<ToolItemManager>.Instance)
		{
			return (from crest in ManagerSingleton<ToolItemManager>.Instance.crestList
			where crest != null
			select crest).ToList<ToolCrest>();
		}
		return new List<ToolCrest>();
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x000EE3BE File Offset: 0x000EC5BE
	public static ToolCrest GetCrestByName(string name)
	{
		if (!ManagerSingleton<ToolItemManager>.Instance)
		{
			return null;
		}
		return ManagerSingleton<ToolItemManager>.Instance.crestList.GetByName(name);
	}

	// Token: 0x06003615 RID: 13845 RVA: 0x000EE3DE File Offset: 0x000EC5DE
	public static ToolItem GetToolByName(string name)
	{
		if (!ManagerSingleton<ToolItemManager>.Instance)
		{
			return null;
		}
		return ManagerSingleton<ToolItemManager>.Instance.toolItems.GetByName(name);
	}

	// Token: 0x06003616 RID: 13846 RVA: 0x000EE400 File Offset: 0x000EC600
	public static List<ToolItem> GetEquippedToolsForCrest(string crestId)
	{
		if (string.IsNullOrEmpty(crestId))
		{
			return null;
		}
		List<ToolCrestsData.SlotData> slots = PlayerData.instance.ToolEquips.GetData(crestId).Slots;
		if (slots == null)
		{
			return null;
		}
		return (from slotInfo in slots
		select ToolItemManager.GetToolByName(slotInfo.EquippedTool)).ToList<ToolItem>();
	}

	// Token: 0x06003617 RID: 13847 RVA: 0x000EE45C File Offset: 0x000EC65C
	private static List<ToolItem> GetCurrentEquippedTools()
	{
		object obj = ToolItemManager.GetEquippedToolsForCrest(PlayerData.instance.CurrentCrestID) ?? new List<ToolItem>();
		IEnumerable<ToolItem> collection = from data in PlayerData.instance.ExtraToolEquips.GetValidDatas((ToolCrestsData.SlotData data) => !string.IsNullOrEmpty(data.EquippedTool))
		select ToolItemManager.GetToolByName(data.EquippedTool);
		object obj2 = obj;
		obj2.AddRange(collection);
		return obj2;
	}

	// Token: 0x06003618 RID: 13848 RVA: 0x000EE4DC File Offset: 0x000EC6DC
	public static void SetEquippedTools(string crestId, List<string> equippedTools)
	{
		if (string.IsNullOrEmpty(crestId))
		{
			return;
		}
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		PlayerData instance2 = PlayerData.instance;
		ToolCrestsData.Data data = instance2.ToolEquips.GetData(crestId);
		if (data.Slots == null)
		{
			data.Slots = new List<ToolCrestsData.SlotData>(equippedTools.Count);
		}
		ToolCrest crestByName = ToolItemManager.GetCrestByName(crestId);
		ToolItem toolItem = null;
		bool flag = false;
		for (int i = 0; i < equippedTools.Count; i++)
		{
			string text = equippedTools[i];
			if (i >= data.Slots.Count || !(data.Slots[i].EquippedTool == text))
			{
				instance.queueEquipsChanged = true;
				ToolItem toolByName = ToolItemManager.GetToolByName(text);
				if (toolByName && toolByName.Type.IsAttackType())
				{
					toolItem = toolByName;
				}
				if (i < crestByName.Slots.Length && crestByName.Slots[i].Type.IsAttackType())
				{
					flag = true;
				}
			}
		}
		if (flag || toolItem)
		{
			instance.queueAttackToolsChanged = true;
			if (instance.lastEquippedAttackTool)
			{
				instance.equipChangedToolSingleReminder.Disappear(true, true);
				instance.equipChangedToolModifierReminder.Disappear(true, true);
				instance.lastEquippedAttackTool = null;
			}
			instance.lastEquippedAttackTool = ((toolItem && toolItem.Type == ToolItemType.Red) ? toolItem : null);
		}
		for (int j = 0; j < equippedTools.Count; j++)
		{
			if (j < data.Slots.Count)
			{
				ToolCrestsData.SlotData value = data.Slots[j];
				value.EquippedTool = equippedTools[j];
				data.Slots[j] = value;
			}
			else
			{
				data.Slots.Add(new ToolCrestsData.SlotData
				{
					EquippedTool = equippedTools[j]
				});
			}
		}
		instance2.ToolEquips.SetData(crestId, data);
		ToolItemManager.ClearToolCache();
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x000EE6BC File Offset: 0x000EC8BC
	public static void SetExtraEquippedTool(string slotId, ToolItem tool)
	{
		ToolItemManager.SetExtraEquippedTool(slotId, tool ? tool.name : string.Empty);
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x000EE6DC File Offset: 0x000EC8DC
	public static void SetExtraEquippedTool(string slotId, string toolName)
	{
		PlayerData instance = PlayerData.instance;
		ToolCrestsData.SlotData data = instance.ExtraToolEquips.GetData(slotId);
		if (data.EquippedTool == toolName)
		{
			return;
		}
		data.EquippedTool = toolName;
		instance.ExtraToolEquips.SetData(slotId, data);
		ToolItemManager.ClearToolCache();
		ManagerSingleton<ToolItemManager>.Instance.queueEquipsChanged = true;
	}

	// Token: 0x0600361B RID: 13851 RVA: 0x000EE730 File Offset: 0x000EC930
	public static void SetEquippedCrest(string crestId)
	{
		if (string.IsNullOrEmpty(crestId))
		{
			ToolCrest toolCrest = ManagerSingleton<ToolItemManager>.Instance.crestList.FirstOrDefault((ToolCrest crest) => crest != null);
			if (!(toolCrest != null))
			{
				return;
			}
			crestId = toolCrest.name;
		}
		PlayerData instance = PlayerData.instance;
		List<ToolCrestsData.SlotData> slots = instance.ToolEquips.GetData(crestId).Slots;
		if (slots != null)
		{
			for (int i = 0; i < slots.Count; i++)
			{
				ToolCrestsData.SlotData slotData = slots[i];
				bool flag = false;
				foreach (ToolCrestsData.SlotData slotData2 in instance.ExtraToolEquips.GetValidDatas(null))
				{
					if (!string.IsNullOrEmpty(slotData.EquippedTool) && !string.IsNullOrEmpty(slotData2.EquippedTool) && !(slotData.EquippedTool != slotData2.EquippedTool))
					{
						slotData.EquippedTool = string.Empty;
						flag = true;
					}
				}
				if (flag)
				{
					slots[i] = slotData;
				}
			}
		}
		if (instance.CurrentCrestID == crestId)
		{
			return;
		}
		instance.PreviousCrestID = instance.CurrentCrestID;
		instance.CurrentCrestID = crestId;
		instance.IsCurrentCrestTemp = false;
		ToolItemManager.ClearToolCache();
		ToolItemManager.UpdateToolCap();
		if (ManagerSingleton<ToolItemManager>.Instance.cursedCrest)
		{
			ToolItemManager.IsCursed = ManagerSingleton<ToolItemManager>.Instance.cursedCrest.IsEquipped;
		}
		ToolItemManager instance2 = ManagerSingleton<ToolItemManager>.Instance;
		if (instance2)
		{
			instance2.queueEquipsChanged = true;
		}
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x000EE8D0 File Offset: 0x000ECAD0
	public static void SendEquippedChangedEvent(bool force = false)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance)
		{
			return;
		}
		if (!instance.queueEquipsChanged && !force)
		{
			return;
		}
		ToolItemManager.RefreshEquippedState();
		HeroController instance2 = HeroController.instance;
		if (instance2)
		{
			instance2.RefreshSilk();
		}
		EventRegister.SendEvent(EventRegisterEvents.EquipsChangedEvent, null);
		Action onEquippedStateChanged = ToolItemManager.OnEquippedStateChanged;
		if (onEquippedStateChanged != null)
		{
			onEquippedStateChanged();
		}
		EventRegister.SendEvent(EventRegisterEvents.EquipsChangedPostEvent, null);
		instance.queueEquipsChanged = false;
		instance.queuedReminder = null;
		if (PlayerData.instance.SeenToolUsePrompt)
		{
			return;
		}
		ToolItem toolItem = instance.lastEquippedAttackTool;
		if (!toolItem)
		{
			return;
		}
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(toolItem);
		if (attackToolBinding == null)
		{
			return;
		}
		switch (attackToolBinding.GetValueOrDefault())
		{
		case AttackToolBinding.Neutral:
			instance.queuedReminder = instance.equipChangedToolSingleReminder;
			return;
		case AttackToolBinding.Up:
			instance.equipChangedToolModifierReminder.Button2 = HeroActionButton.UP;
			instance.queuedReminder = instance.equipChangedToolModifierReminder;
			return;
		case AttackToolBinding.Down:
			instance.equipChangedToolModifierReminder.Button2 = HeroActionButton.DOWN;
			instance.queuedReminder = instance.equipChangedToolModifierReminder;
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x000EE9D8 File Offset: 0x000ECBD8
	public static void RefreshEquippedState()
	{
		ToolItemManager.ClearToolCache();
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (instance != null && instance.cursedCrest)
		{
			ToolItemManager.IsCursed = instance.cursedCrest.IsEquipped;
		}
	}

	// Token: 0x0600361E RID: 13854 RVA: 0x000EEA16 File Offset: 0x000ECC16
	private static void ClearToolCache()
	{
		ToolItemManager.toolCache.Clear();
		ToolItemManager.IncrementVersion();
	}

	// Token: 0x0600361F RID: 13855 RVA: 0x000EEA28 File Offset: 0x000ECC28
	public static void ShowQueuedReminder()
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance || instance.queuedReminder == null)
		{
			return;
		}
		instance.queuedReminder.DoAppear();
		instance.queuedReminder = null;
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x000EEA60 File Offset: 0x000ECC60
	public static bool IsToolEquipped(ToolItem tool, ToolEquippedReadSource readSource)
	{
		if (ToolItemManager.IsCursed)
		{
			return false;
		}
		if (!tool)
		{
			return false;
		}
		ToolsActiveStates toolsActiveStates = ToolItemManager.ActiveState;
		if (toolsActiveStates != ToolsActiveStates.Cutscene)
		{
			if (toolsActiveStates == ToolsActiveStates.Disabled)
			{
				return false;
			}
		}
		else if (readSource == ToolEquippedReadSource.Active)
		{
			return false;
		}
		if (ManagerSingleton<ToolItemManager>.Instance.customToolOverride == tool)
		{
			return true;
		}
		bool result;
		if (ToolItemManager.toolCache.TryGetValue(tool, out result))
		{
			return result;
		}
		bool flag = ToolItemManager.IsToolEquipped(tool.name);
		ToolItemManager.toolCache.Add(tool, flag);
		return flag;
	}

	// Token: 0x06003621 RID: 13857 RVA: 0x000EEAD8 File Offset: 0x000ECCD8
	public static bool IsToolEquipped(string name)
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		PlayerData instance = PlayerData.instance;
		foreach (string itemName in instance.ExtraToolEquips.GetValidNames(null))
		{
			if (instance.ExtraToolEquips.GetData(itemName).EquippedTool == name)
			{
				return true;
			}
		}
		string currentCrestID = instance.CurrentCrestID;
		List<ToolCrestsData.SlotData> slots = instance.ToolEquips.GetData(currentCrestID).Slots;
		if (slots == null)
		{
			return false;
		}
		using (List<ToolCrestsData.SlotData>.Enumerator enumerator2 = slots.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.EquippedTool == name)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003622 RID: 13858 RVA: 0x000EEBD0 File Offset: 0x000ECDD0
	public static void UnequipTool(ToolItem toolItem)
	{
		if (!toolItem)
		{
			return;
		}
		ToolItemManager.toolCache.Remove(toolItem);
		ToolItemManager.ReplaceToolEquips(toolItem.name, string.Empty);
	}

	// Token: 0x06003623 RID: 13859 RVA: 0x000EEBF7 File Offset: 0x000ECDF7
	public static void ReplaceToolEquips(ToolItem oldTool, ToolItem newTool)
	{
		if (!oldTool)
		{
			return;
		}
		ToolItemManager.toolCache.Remove(oldTool);
		ToolItemManager.ReplaceToolEquips(oldTool.name, newTool ? newTool.name : string.Empty);
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x000EEC30 File Offset: 0x000ECE30
	private static void ReplaceToolEquips(string oldToolName, string newToolName)
	{
		if (string.IsNullOrEmpty(oldToolName))
		{
			return;
		}
		ToolItemManager.IncrementVersion();
		PlayerData instance = PlayerData.instance;
		foreach (string itemName in instance.ExtraToolEquips.GetValidNames(null))
		{
			ToolCrestsData.SlotData data = instance.ExtraToolEquips.GetData(itemName);
			string equippedTool = data.EquippedTool;
			if (!string.IsNullOrEmpty(equippedTool) && !(equippedTool != oldToolName))
			{
				data.EquippedTool = newToolName;
				instance.ExtraToolEquips.SetData(itemName, data);
				ManagerSingleton<ToolItemManager>.Instance.queueEquipsChanged = true;
			}
		}
		string currentCrestID = instance.CurrentCrestID;
		if (string.IsNullOrEmpty(currentCrestID))
		{
			return;
		}
		List<ToolCrestsData.SlotData> slots = instance.ToolEquips.GetData(currentCrestID).Slots;
		if (slots == null)
		{
			return;
		}
		for (int i = 0; i < slots.Count; i++)
		{
			ToolCrestsData.SlotData slotData = slots[i];
			string equippedTool2 = slotData.EquippedTool;
			if (!string.IsNullOrEmpty(equippedTool2) && !(equippedTool2 != oldToolName))
			{
				slotData.EquippedTool = newToolName;
				slots[i] = slotData;
				ManagerSingleton<ToolItemManager>.Instance.queueEquipsChanged = true;
			}
		}
		ToolItemManager.UpdateToolCap();
	}

	// Token: 0x06003625 RID: 13861 RVA: 0x000EED68 File Offset: 0x000ECF68
	private static void UpdateToolCap()
	{
		foreach (ToolItem toolItem in ToolItemManager.GetAllTools())
		{
			ToolItemsData.Data savedData = toolItem.SavedData;
			int toolStorageAmount = ToolItemManager.GetToolStorageAmount(toolItem);
			if (savedData.AmountLeft > toolStorageAmount)
			{
				savedData.AmountLeft = toolStorageAmount;
				toolItem.SavedData = savedData;
			}
		}
	}

	// Token: 0x06003626 RID: 13862 RVA: 0x000EEDD4 File Offset: 0x000ECFD4
	public static IEnumerable<ToolItem> GetAllTools()
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance || !instance.toolItems)
		{
			return Enumerable.Empty<ToolItem>();
		}
		return instance.toolItems;
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x000EEE08 File Offset: 0x000ED008
	public static void UnlockAllTools()
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance || !instance.toolItems)
		{
			return;
		}
		instance.toolItems.UnlockAll();
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x000EEE3C File Offset: 0x000ED03C
	public static void UnlockAllCrests()
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance || !instance.crestList)
		{
			return;
		}
		instance.crestList.UnlockAll();
		PlayerData instance2 = PlayerData.instance;
		instance2.UnlockedExtraBlueSlot = true;
		instance2.UnlockedExtraYellowSlot = true;
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x000EEE84 File Offset: 0x000ED084
	public static ToolItem GetBoundAttackTool(AttackToolBinding binding, ToolEquippedReadSource readSource)
	{
		AttackToolBinding attackToolBinding;
		return ToolItemManager.GetBoundAttackTool(binding, readSource, out attackToolBinding);
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x000EEE9C File Offset: 0x000ED09C
	public static ToolItem GetBoundAttackTool(AttackToolBinding binding, ToolEquippedReadSource readSource, out AttackToolBinding usedBinding)
	{
		usedBinding = binding;
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance || !instance.crestList)
		{
			return null;
		}
		ToolsActiveStates toolsActiveStates = ToolItemManager.activeState;
		if (toolsActiveStates != ToolsActiveStates.Cutscene)
		{
			if (toolsActiveStates != ToolsActiveStates.Disabled)
			{
				goto IL_35;
			}
		}
		else if (readSource != ToolEquippedReadSource.Active)
		{
			goto IL_35;
		}
		return null;
		IL_35:
		if (CollectableItemManager.IsInHiddenMode())
		{
			return null;
		}
		string currentCrestID = PlayerData.instance.CurrentCrestID;
		if (string.IsNullOrEmpty(currentCrestID))
		{
			return null;
		}
		ToolCrestsData.Data data = PlayerData.instance.ToolEquips.GetData(currentCrestID);
		if (instance.queueAttackToolsChanged || currentCrestID != instance.previousEquippedCrest)
		{
			ToolCrest byName = instance.crestList.GetByName(currentCrestID);
			if (!byName)
			{
				Debug.LogErrorFormat("Could not load crest {0}", new object[]
				{
					currentCrestID
				});
				return null;
			}
			List<ToolCrest.SlotInfo> list = new List<ToolCrest.SlotInfo>(3);
			List<ToolItem> list2 = new List<ToolItem>(3);
			if (data.Slots != null && byName.Slots != null)
			{
				for (int i = 0; i < Mathf.Min(byName.Slots.Length, data.Slots.Count); i++)
				{
					string equippedTool = data.Slots[i].EquippedTool;
					if (!string.IsNullOrEmpty(equippedTool))
					{
						ToolItem toolByName = ToolItemManager.GetToolByName(equippedTool);
						if (!toolByName)
						{
							Debug.LogErrorFormat("Equipped tool {0} could not be found", new object[]
							{
								equippedTool
							});
						}
						else if (toolByName.Type.IsAttackType())
						{
							list2.Add(toolByName);
							list.Add(byName.Slots[i]);
						}
					}
				}
			}
			ToolItemManager toolItemManager = instance;
			if (toolItemManager.boundAttackTools == null)
			{
				toolItemManager.boundAttackTools = new ToolItem[3];
			}
			if (instance.customToolOverride)
			{
				instance.boundAttackTools[0] = instance.customToolOverride;
				instance.boundAttackTools[1] = null;
				instance.boundAttackTools[2] = null;
			}
			else
			{
				instance.boundAttackTools[0] = ToolItemManager.GetToolForBinding(list2, list, AttackToolBinding.Neutral);
				instance.boundAttackTools[1] = ToolItemManager.GetToolForBinding(list2, list, AttackToolBinding.Up);
				instance.boundAttackTools[2] = ToolItemManager.GetToolForBinding(list2, list, AttackToolBinding.Down);
			}
			instance.queueAttackToolsChanged = false;
			instance.previousEquippedCrest = currentCrestID;
		}
		if (readSource != ToolEquippedReadSource.Active)
		{
			if (readSource != ToolEquippedReadSource.Hud)
			{
				throw new ArgumentOutOfRangeException("readSource", readSource, null);
			}
			return instance.boundAttackTools[(int)binding];
		}
		else
		{
			if (instance.customToolOverride)
			{
				return instance.customToolOverride;
			}
			ToolItem toolItem = instance.boundAttackTools[(int)binding];
			if (!toolItem && binding != AttackToolBinding.Neutral)
			{
				usedBinding = AttackToolBinding.Neutral;
				return instance.boundAttackTools[0];
			}
			return toolItem;
		}
	}

	// Token: 0x0600362B RID: 13867 RVA: 0x000EF100 File Offset: 0x000ED300
	private static ToolItem GetToolForBinding(List<ToolItem> equippedAttackTools, List<ToolCrest.SlotInfo> crestSlots, AttackToolBinding binding)
	{
		for (int i = 0; i < equippedAttackTools.Count; i++)
		{
			if (crestSlots[i].AttackBinding == binding)
			{
				return equippedAttackTools[i];
			}
		}
		return null;
	}

	// Token: 0x0600362C RID: 13868 RVA: 0x000EF138 File Offset: 0x000ED338
	public static AttackToolBinding? GetAttackToolBinding(ToolItem tool)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (instance.customToolOverride)
		{
			if (tool == instance.customToolOverride)
			{
				return new AttackToolBinding?(AttackToolBinding.Neutral);
			}
			return null;
		}
		else
		{
			if (!tool || !tool.Type.IsAttackType())
			{
				return null;
			}
			string currentCrestID = PlayerData.instance.CurrentCrestID;
			if (string.IsNullOrEmpty(currentCrestID))
			{
				return null;
			}
			ToolCrest byName = instance.crestList.GetByName(currentCrestID);
			if (!byName)
			{
				Debug.LogErrorFormat("Could not load crest {0}", new object[]
				{
					currentCrestID
				});
				return null;
			}
			List<ToolCrestsData.SlotData> slots = PlayerData.instance.ToolEquips.GetData(currentCrestID).Slots;
			if (slots == null)
			{
				return null;
			}
			for (int i = 0; i < Mathf.Min(slots.Count, byName.Slots.Length); i++)
			{
				string equippedTool = slots[i].EquippedTool;
				if (!string.IsNullOrEmpty(equippedTool) && equippedTool.Equals(tool.name))
				{
					return new AttackToolBinding?(byName.Slots[i].AttackBinding);
				}
			}
			return null;
		}
	}

	// Token: 0x0600362D RID: 13869 RVA: 0x000EF27C File Offset: 0x000ED47C
	public static int GetToolStorageAmount(ToolItem tool)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (tool == instance.customToolOverride)
		{
			return instance.customToolAmount;
		}
		if (tool.PreventStorageIncrease)
		{
			return tool.BaseStorageAmount;
		}
		float num = (float)tool.BaseStorageAmount * Gameplay.ToolPouchUpgradeIncrease * (float)PlayerData.instance.ToolPouchUpgrades;
		if (Gameplay.ShellSatchelTool.IsEquipped)
		{
			num += (float)tool.BaseStorageAmount * Gameplay.ShellSatchelToolIncrease;
		}
		return tool.BaseStorageAmount + Mathf.FloorToInt(num);
	}

	// Token: 0x0600362E RID: 13870 RVA: 0x000EF2F8 File Offset: 0x000ED4F8
	public static bool TryReplenishTools(bool doReplenish, ToolItemManager.ReplenishMethod method)
	{
		if (string.IsNullOrEmpty(PlayerData.instance.CurrentCrestID))
		{
			return false;
		}
		bool flag = false;
		HeroController instance = HeroController.instance;
		List<ToolItem> currentEquippedTools = ToolItemManager.GetCurrentEquippedTools();
		if (currentEquippedTools == null)
		{
			return false;
		}
		ArrayForEnumAttribute.EnsureArraySize<float>(ref ToolItemManager._startingCurrencyAmounts, typeof(CurrencyType));
		ArrayForEnumAttribute.EnsureArraySize<float>(ref ToolItemManager._endingCurrencyAmounts, typeof(CurrencyType));
		Array values = Enum.GetValues(typeof(CurrencyType));
		for (int i = 0; i < values.Length; i++)
		{
			CurrencyType type = (CurrencyType)values.GetValue(i);
			ToolItemManager._endingCurrencyAmounts[i] = (ToolItemManager._startingCurrencyAmounts[i] = (float)CurrencyManager.GetCurrencyAmount(type));
		}
		Dictionary<ToolItemStatesLiquid, int> liquidCostsTemp = ToolItemManager._liquidCostsTemp;
		if (liquidCostsTemp != null)
		{
			liquidCostsTemp.Clear();
		}
		bool flag2 = false;
		bool flag3 = true;
		currentEquippedTools.RemoveAll((ToolItem tool) => tool == null || !tool.IsAutoReplenished());
		while (flag3)
		{
			flag3 = false;
			foreach (ToolItem toolItem in currentEquippedTools)
			{
				if ((method != ToolItemManager.ReplenishMethod.QuickCraft || toolItem.ReplenishResource != ToolItem.ReplenishResources.None) && toolItem.ReplenishUsage != ToolItem.ReplenishUsages.OneForOne)
				{
					ToolItemsData.Data toolData = PlayerData.instance.GetToolData(toolItem.name);
					int toolStorageAmount = ToolItemManager.GetToolStorageAmount(toolItem);
					if (toolData.AmountLeft < toolStorageAmount)
					{
						flag2 = true;
						float num;
						switch (toolItem.ReplenishUsage)
						{
						case ToolItem.ReplenishUsages.Percentage:
							num = 1f / (float)toolItem.BaseStorageAmount * (float)Gameplay.ToolReplenishCost;
							break;
						case ToolItem.ReplenishUsages.OneForOne:
							num = 1f;
							break;
						case ToolItem.ReplenishUsages.Custom:
							num = 0f;
							break;
						default:
							throw new ArgumentOutOfRangeException();
						}
						num *= toolItem.ReplenishUsageMultiplier;
						float inCost = num;
						int num2;
						bool flag4 = toolItem.TryReplenishSingle(false, num, out num, out num2);
						if (flag4 && doReplenish)
						{
							if (toolItem.ReplenishResource != ToolItem.ReplenishResources.None && ToolItemManager._endingCurrencyAmounts[(int)toolItem.ReplenishResource] - num <= -0.5f)
							{
								continue;
							}
							num2 = 0;
							flag4 = toolItem.TryReplenishSingle(true, inCost, out num, out num2);
						}
						bool flag5 = true;
						ToolItemStatesLiquid toolItemStatesLiquid = toolItem as ToolItemStatesLiquid;
						if (toolItemStatesLiquid != null)
						{
							if (ToolItemManager._liquidCostsTemp == null)
							{
								ToolItemManager._liquidCostsTemp = new Dictionary<ToolItemStatesLiquid, int>();
							}
							int num3 = ToolItemManager._liquidCostsTemp.GetValueOrDefault(toolItemStatesLiquid, 0);
							if (toolItemStatesLiquid.LiquidSavedData.RefillsLeft > num3 && !toolItemStatesLiquid.LiquidSavedData.UsedExtra)
							{
								num3 += num2;
								ToolItemManager._liquidCostsTemp[toolItemStatesLiquid] = num3;
							}
							else
							{
								flag5 = false;
							}
						}
						if (flag4)
						{
							if (num > 0f && toolItem.ReplenishResource != ToolItem.ReplenishResources.None)
							{
								float num4 = ToolItemManager._endingCurrencyAmounts[(int)toolItem.ReplenishResource];
								if (num4 <= 0f || num4 - num <= -0.5f)
								{
									continue;
								}
								num4 -= num;
								ToolItemManager._endingCurrencyAmounts[(int)toolItem.ReplenishResource] = Mathf.Max(num4, 0f);
							}
							if (doReplenish && flag5)
							{
								toolData.AmountLeft++;
								if (toolData.AmountLeft < toolStorageAmount)
								{
									flag3 = true;
								}
								PlayerData.instance.Tools.SetData(toolItem.name, toolData);
							}
							flag = true;
						}
					}
				}
			}
		}
		if (!flag && method == ToolItemManager.ReplenishMethod.QuickCraft && Mathf.CeilToInt(ToolItemManager._endingCurrencyAmounts[1]) > 0)
		{
			flag = true;
			if (flag2)
			{
				ToolItemManager._endingCurrencyAmounts[1] = 0f;
			}
			else
			{
				float num5 = ToolItemManager._endingCurrencyAmounts[1];
				num5 -= (float)Gameplay.ToolmasterQuickCraftNoneUsage;
				if (num5 < 0f)
				{
					num5 = 0f;
				}
				ToolItemManager._endingCurrencyAmounts[1] = num5;
			}
		}
		if (!doReplenish)
		{
			return flag;
		}
		bool flag6 = method != ToolItemManager.ReplenishMethod.BenchSilent;
		for (int j = 0; j < values.Length; j++)
		{
			int num6 = Mathf.RoundToInt(ToolItemManager._startingCurrencyAmounts[j] - ToolItemManager._endingCurrencyAmounts[j]);
			if (num6 > 0)
			{
				CurrencyManager.TakeCurrency(num6, (CurrencyType)values.GetValue(j), flag6);
			}
		}
		if (ToolItemManager._liquidCostsTemp != null)
		{
			foreach (KeyValuePair<ToolItemStatesLiquid, int> keyValuePair in ToolItemManager._liquidCostsTemp)
			{
				ToolItemStatesLiquid toolItemStatesLiquid2;
				int num7;
				keyValuePair.Deconstruct(out toolItemStatesLiquid2, out num7);
				ToolItemStatesLiquid toolItemStatesLiquid3 = toolItemStatesLiquid2;
				int num8 = num7;
				if (num8 <= 0)
				{
					if (flag6)
					{
						toolItemStatesLiquid3.ShowLiquidInfiniteRefills();
					}
				}
				else
				{
					toolItemStatesLiquid3.TakeLiquid(num8, flag6);
				}
			}
			ToolItemManager._liquidCostsTemp.Clear();
		}
		ToolItemManager.ReportAllBoundAttackToolsUpdated();
		ToolItemManager.SendEquippedChangedEvent(true);
		return flag;
	}

	// Token: 0x0600362F RID: 13871 RVA: 0x000EF774 File Offset: 0x000ED974
	public static void ReportAllBoundAttackToolsUpdated()
	{
		foreach (AttackToolBinding binding in Enum.GetValues(typeof(AttackToolBinding)).Cast<AttackToolBinding>())
		{
			ToolItemManager.ReportBoundAttackToolUpdated(binding);
		}
		ToolItemManager.IncrementVersion();
	}

	// Token: 0x06003630 RID: 13872 RVA: 0x000EF7D4 File Offset: 0x000ED9D4
	public static void ReportBoundAttackToolUpdated(AttackToolBinding binding)
	{
		Action<AttackToolBinding> boundAttackToolUpdated = ToolItemManager.BoundAttackToolUpdated;
		if (boundAttackToolUpdated == null)
		{
			return;
		}
		boundAttackToolUpdated(binding);
	}

	// Token: 0x06003631 RID: 13873 RVA: 0x000EF7E8 File Offset: 0x000ED9E8
	public static void ReportBoundAttackToolUsed(AttackToolBinding binding)
	{
		Action<AttackToolBinding> boundAttackToolUsed = ToolItemManager.BoundAttackToolUsed;
		if (boundAttackToolUsed != null)
		{
			boundAttackToolUsed(binding);
		}
		ToolItem boundAttackTool = ToolItemManager.GetBoundAttackTool(binding, ToolEquippedReadSource.Active);
		if (!boundAttackTool)
		{
			return;
		}
		if (ToolItemManager.IsInfiniteToolUseEnabled)
		{
			ToolItemsData.Data savedData = boundAttackTool.SavedData;
			savedData.AmountLeft = ToolItemManager.GetToolStorageAmount(boundAttackTool);
			boundAttackTool.SavedData = savedData;
		}
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance)
		{
			return;
		}
		if (boundAttackTool == instance.lastEquippedAttackTool)
		{
			instance.equipChangedToolSingleReminder.Disappear(false, true);
			instance.equipChangedToolModifierReminder.Disappear(false, true);
			instance.lastEquippedAttackTool = null;
			PlayerData.instance.SeenToolUsePrompt = true;
			instance.queuedReminder = null;
		}
	}

	// Token: 0x06003632 RID: 13874 RVA: 0x000EF889 File Offset: 0x000EDA89
	public static void ReportBoundAttackToolFailed(AttackToolBinding binding)
	{
		Action<AttackToolBinding> boundAttackToolFailed = ToolItemManager.BoundAttackToolFailed;
		if (boundAttackToolFailed == null)
		{
			return;
		}
		boundAttackToolFailed(binding);
	}

	// Token: 0x06003633 RID: 13875 RVA: 0x000EF89C File Offset: 0x000EDA9C
	public static void SetCustomToolOverride(ToolItem tool, int? amount)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		ToolItem toolItem = instance.customToolOverride;
		if (toolItem)
		{
			ToolItemsData.Data savedData = toolItem.SavedData;
			savedData.AmountLeft = 0;
			toolItem.SavedData = savedData;
		}
		instance.customToolOverride = null;
		instance.customToolAmount = (amount ?? ToolItemManager.GetToolStorageAmount(tool));
		if (tool)
		{
			ToolItemsData.Data savedData2 = tool.SavedData;
			savedData2.AmountLeft = instance.customToolAmount;
			tool.SavedData = savedData2;
			EventRegister.SendEvent(EventRegisterEvents.ShowCustomToolReminder, null);
		}
		else
		{
			EventRegister.SendEvent(EventRegisterEvents.HideCustomToolReminder, null);
		}
		instance.customToolOverride = tool;
		instance.queueAttackToolsChanged = true;
		ToolItemManager.ReportAllBoundAttackToolsUpdated();
	}

	// Token: 0x06003634 RID: 13876 RVA: 0x000EF94B File Offset: 0x000EDB4B
	public static void SetCustomToolOverride(ToolItem tool, int? amount, LocalisedString promptText, float promptAppearDelay)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		instance.toolOverrideReminder.Text = promptText;
		instance.toolOverrideReminder.FadeInDelay = promptAppearDelay;
		ToolItemManager.SetCustomToolOverride(tool, amount);
	}

	// Token: 0x06003635 RID: 13877 RVA: 0x000EF970 File Offset: 0x000EDB70
	public static void ClearCustomToolOverride()
	{
		ToolItemManager.SetCustomToolOverride(null, null);
	}

	// Token: 0x06003636 RID: 13878 RVA: 0x000EF98C File Offset: 0x000EDB8C
	public static void AutoEquip(ToolItem tool)
	{
		if (tool == null)
		{
			return;
		}
		string currentCrestID = PlayerData.instance.CurrentCrestID;
		ToolCrest crestByName = ToolItemManager.GetCrestByName(currentCrestID);
		List<ToolItem> equippedToolsForCrest = ToolItemManager.GetEquippedToolsForCrest(currentCrestID);
		List<string> list = new List<string>(crestByName.Slots.Length);
		for (int i = 0; i < crestByName.Slots.Length; i++)
		{
			ToolItem toolItem = (equippedToolsForCrest != null && i < equippedToolsForCrest.Count) ? equippedToolsForCrest[i] : null;
			list.Add((toolItem != null) ? toolItem.name : string.Empty);
		}
		int num;
		if (tool.Type == ToolItemType.Skill)
		{
			num = -1;
			for (int j = 0; j < crestByName.Slots.Length; j++)
			{
				ToolCrest.SlotInfo slotInfo = crestByName.Slots[j];
				if (slotInfo.Type == ToolItemType.Skill && slotInfo.AttackBinding == AttackToolBinding.Neutral)
				{
					num = j;
					break;
				}
			}
		}
		else
		{
			int num2 = -1;
			int num3 = -1;
			for (int k = 0; k < crestByName.Slots.Length; k++)
			{
				if (crestByName.Slots[k].Type == tool.Type)
				{
					num2 = k;
					if (string.IsNullOrEmpty(list[k]))
					{
						num3 = k;
					}
				}
			}
			num = ((num3 >= 0) ? num3 : num2);
		}
		if (num >= 0)
		{
			list[num] = tool.name;
		}
		ToolItemManager.UnlockedTool = tool;
		InventoryPaneList.SetNextOpen("Tools");
		ToolItemManager.SetEquippedTools(currentCrestID, list);
		ToolItemManager.SendEquippedChangedEvent(false);
	}

	// Token: 0x06003637 RID: 13879 RVA: 0x000EFAF4 File Offset: 0x000EDCF4
	public static void AutoEquip(ToolCrest crest, bool markTemp)
	{
		PlayerData instance = PlayerData.instance;
		if (crest == null)
		{
			crest = ToolItemManager.GetCrestByName(instance.PreviousCrestID);
			if (crest == null)
			{
				crest = ToolItemManager.GetAllCrests().FirstOrDefault((ToolCrest c) => c.IsVisible);
				if (crest == null)
				{
					return;
				}
			}
		}
		string currentCrestID = instance.CurrentCrestID;
		string name = crest.name;
		if (name != currentCrestID)
		{
			instance.PreviousCrestID = currentCrestID;
		}
		else
		{
			markTemp = false;
		}
		bool isCurrentCrestTemp = instance.IsCurrentCrestTemp;
		ToolItemManager.SetEquippedCrest(name);
		instance.IsCurrentCrestTemp = markTemp;
		List<string> list = new List<string>(crest.Slots.Length);
		if (!markTemp && !isCurrentCrestTemp)
		{
			ToolItemManager.<>c__DisplayClass100_0 CS$<>8__locals1;
			CS$<>8__locals1.equipPool = (ToolItemManager.GetEquippedToolsForCrest(currentCrestID) ?? new List<ToolItem>());
			ToolItemManager.<AutoEquip>g__RemoveNonSkills|100_1(ref CS$<>8__locals1);
			if (CS$<>8__locals1.equipPool.Count == 0)
			{
				CS$<>8__locals1.equipPool = (ToolItemManager.GetEquippedToolsForCrest(name) ?? new List<ToolItem>());
				ToolItemManager.<AutoEquip>g__RemoveNonSkills|100_1(ref CS$<>8__locals1);
			}
			for (int i = 0; i < crest.Slots.Length; i++)
			{
				ToolCrest.SlotInfo slotInfo = crest.Slots[i];
				if (slotInfo.Type != ToolItemType.Skill)
				{
					list.Add(string.Empty);
				}
				else if (CS$<>8__locals1.equipPool.Count > 0)
				{
					ToolItem toolItem = CS$<>8__locals1.equipPool[0];
					list.Add(toolItem.name);
					CS$<>8__locals1.equipPool.RemoveAt(0);
				}
				else if (slotInfo.AttackBinding == AttackToolBinding.Neutral)
				{
					if (Gameplay.DefaultSkillTool.IsUnlocked)
					{
						list.Add(Gameplay.DefaultSkillTool.name);
					}
					else
					{
						ToolItem toolItem2 = ToolItemManager.GetUnlockedTools().FirstOrDefault((ToolItem t) => t.Type == ToolItemType.Skill);
						if (toolItem2)
						{
							list.Add(toolItem2.name);
						}
					}
				}
			}
		}
		ToolItemManager.SetEquippedTools(crest.name, list);
		ToolItemManager.SendEquippedChangedEvent(false);
		InventoryPaneList.SetNextOpen("Tools");
	}

	// Token: 0x06003638 RID: 13880 RVA: 0x000EFD00 File Offset: 0x000EDF00
	public static void RemoveToolFromAllCrests(ToolItem tool)
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!tool || !instance)
		{
			return;
		}
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(tool);
		ToolCrestsData toolEquips = PlayerData.instance.ToolEquips;
		foreach (ToolCrest toolCrest in instance.crestList)
		{
			ToolCrestsData.Data data = toolEquips.GetData(toolCrest.name);
			if (data.Slots != null)
			{
				bool flag = false;
				for (int i = 0; i < data.Slots.Count; i++)
				{
					ToolCrestsData.SlotData slotData = data.Slots[i];
					if (!(slotData.EquippedTool != tool.name))
					{
						slotData.EquippedTool = string.Empty;
						data.Slots[i] = slotData;
						flag = true;
					}
				}
				if (flag)
				{
					instance.queueEquipsChanged = true;
					toolEquips.SetData(toolCrest.name, data);
				}
			}
		}
		if (attackToolBinding != null)
		{
			instance.queueAttackToolsChanged = true;
			ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
		}
		ToolItemManager.SendEquippedChangedEvent(false);
	}

	// Token: 0x06003639 RID: 13881 RVA: 0x000EFE2C File Offset: 0x000EE02C
	[UsedImplicitly]
	public static void ResetPreviousCrest()
	{
		ToolItemManager instance = ManagerSingleton<ToolItemManager>.Instance;
		if (!instance)
		{
			return;
		}
		PlayerData instance2 = PlayerData.instance;
		ToolCrestsData toolEquips = instance2.ToolEquips;
		ToolCrestsData.Data data = toolEquips.GetData(instance2.PreviousCrestID);
		if (data.Slots == null)
		{
			return;
		}
		for (int i = 0; i < data.Slots.Count; i++)
		{
			ToolCrestsData.SlotData slotData = data.Slots[i];
			if (!string.IsNullOrEmpty(slotData.EquippedTool))
			{
				ToolItem toolByName = ToolItemManager.GetToolByName(slotData.EquippedTool);
				if (toolByName && toolByName.Type != ToolItemType.Skill)
				{
					slotData.EquippedTool = string.Empty;
					data.Slots[i] = slotData;
				}
			}
		}
		instance.queueEquipsChanged = true;
		toolEquips.SetData(instance2.PreviousCrestID, data);
		ToolItemManager.ReportAllBoundAttackToolsUpdated();
		ToolItemManager.SendEquippedChangedEvent(false);
	}

	// Token: 0x0600363A RID: 13882 RVA: 0x000EFF00 File Offset: 0x000EE100
	public static int GetOwnedToolsCount(ToolItemManager.OwnToolsCheckFlags checkFlags)
	{
		return ToolItemManager.GetUnlockedTools().Count(delegate(ToolItem tool)
		{
			switch (tool.Type)
			{
			case ToolItemType.Red:
				return (checkFlags & ToolItemManager.OwnToolsCheckFlags.Red) == ToolItemManager.OwnToolsCheckFlags.Red;
			case ToolItemType.Blue:
				return (checkFlags & ToolItemManager.OwnToolsCheckFlags.Blue) == ToolItemManager.OwnToolsCheckFlags.Blue;
			case ToolItemType.Yellow:
				return (checkFlags & ToolItemManager.OwnToolsCheckFlags.Yellow) == ToolItemManager.OwnToolsCheckFlags.Yellow;
			case ToolItemType.Skill:
				return (checkFlags & ToolItemManager.OwnToolsCheckFlags.Skill) == ToolItemManager.OwnToolsCheckFlags.Skill;
			default:
				return false;
			}
		});
	}

	// Token: 0x0600363B RID: 13883 RVA: 0x000EFF30 File Offset: 0x000EE130
	public static void SetIsInCutscene(bool value)
	{
		ToolItemManager.SetActiveState(value ? ToolsActiveStates.Cutscene : ToolsActiveStates.Active);
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x000EFF3E File Offset: 0x000EE13E
	public static void SetActiveState(ToolsActiveStates value)
	{
		ToolItemManager.SetActiveState(value, true);
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x000EFF47 File Offset: 0x000EE147
	public static void SetActiveState(ToolsActiveStates value, bool skipAnims)
	{
		if (value == ToolItemManager.activeState)
		{
			return;
		}
		ToolItemManager.activeState = value;
		ToolItemManager.ReportAllBoundAttackToolsUpdated();
		ToolItemManager.SendEquippedChangedEvent(true);
		EventRegister.SendEvent("DELIVERY HUD REFRESH", null);
		if (!skipAnims)
		{
			EventRegister.SendEvent(EventRegisterEvents.InventoryOpenComplete, null);
		}
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x000EFF7C File Offset: 0x000EE17C
	public static void ReportToolUnlocked(ToolItemType type)
	{
		ToolItemManager.ReportToolUnlocked(type, false);
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x000EFF88 File Offset: 0x000EE188
	public static void ReportToolUnlocked(ToolItemType type, bool queueAchievement)
	{
		GameManager instance = GameManager.instance;
		string key;
		string key2;
		Func<ToolItem, bool> condition;
		if (type == ToolItemType.Skill)
		{
			key = "FIRST_SILK_SKILL";
			key2 = "ALL_SILK_SKILLS";
			condition = ((ToolItem tool) => tool.Type == ToolItemType.Skill);
		}
		else
		{
			key = "FIRST_TOOL";
			key2 = "ALL_TOOLS";
			condition = ((ToolItem tool) => tool.Type != ToolItemType.Skill);
		}
		int count = ToolItemManager.GetCount(ToolItemManager.GetUnlockedTools(), condition);
		int count2 = ToolItemManager.GetCount(ToolItemManager.GetAllTools(), condition);
		if (queueAchievement)
		{
			instance.QueueAchievement(key);
			instance.QueueAchievementProgress(key2, count, count2);
			return;
		}
		instance.AwardAchievement(key);
		instance.UpdateAchievementProgress(key2, count, count2);
	}

	// Token: 0x06003640 RID: 13888 RVA: 0x000F003C File Offset: 0x000EE23C
	public static int GetCount(IEnumerable<ToolItem> collection, Func<ToolItem, bool> condition)
	{
		return (from tool in collection
		where (condition == null || condition(tool)) && tool.IsCounted
		group tool by tool.CountKey).Count<IGrouping<SavedItem, ToolItem>>();
	}

	// Token: 0x06003641 RID: 13889 RVA: 0x000F0094 File Offset: 0x000EE294
	public static void ReportCrestUnlocked(bool reportAchievement)
	{
		if (reportAchievement)
		{
			GameManager instance = GameManager.instance;
			instance.AwardAchievement("FIRST_CREST");
			int unlockedCrestsCount = ToolItemManager.GetUnlockedCrestsCount();
			int max = ToolItemManager.GetAllCrests().Count((ToolCrest crest) => !crest.IsHidden && crest.IsBaseVersion);
			instance.UpdateAchievementProgress("ALL_CRESTS", unlockedCrestsCount, max);
		}
	}

	// Token: 0x06003642 RID: 13890 RVA: 0x000F00F0 File Offset: 0x000EE2F0
	public static int GetUnlockedCrestsCount()
	{
		return ToolItemManager.GetAllCrests().Count((ToolCrest crest) => !crest.IsHidden && crest.IsBaseVersion && crest.IsUnlocked);
	}

	// Token: 0x06003645 RID: 13893 RVA: 0x000F0130 File Offset: 0x000EE330
	[CompilerGenerated]
	internal static void <AutoEquip>g__RemoveNonSkills|100_1(ref ToolItemManager.<>c__DisplayClass100_0 A_0)
	{
		for (int i = A_0.equipPool.Count - 1; i >= 0; i--)
		{
			ToolItem toolItem = A_0.equipPool[i];
			if (!toolItem || toolItem.Type != ToolItemType.Skill)
			{
				A_0.equipPool.RemoveAt(i);
			}
		}
	}

	// Token: 0x04003928 RID: 14632
	public const string EQUIPS_CHANGED_EVENT = "TOOL EQUIPS CHANGED";

	// Token: 0x04003929 RID: 14633
	public const string EQUIPS_CHANGED_EVENT_POST = "POST TOOL EQUIPS CHANGED";

	// Token: 0x0400392A RID: 14634
	private const float COST_ROUND_UP = 0.1f;

	// Token: 0x0400392E RID: 14638
	[SerializeField]
	private ToolItemList toolItems;

	// Token: 0x0400392F RID: 14639
	[SerializeField]
	private ToolCrestList crestList;

	// Token: 0x04003930 RID: 14640
	[Space]
	[SerializeField]
	private ToolCrest cursedCrest;

	// Token: 0x04003931 RID: 14641
	private ToolItem[] boundAttackTools;

	// Token: 0x04003932 RID: 14642
	private bool queueAttackToolsChanged;

	// Token: 0x04003933 RID: 14643
	private bool queueEquipsChanged;

	// Token: 0x04003934 RID: 14644
	private string previousEquippedCrest;

	// Token: 0x04003935 RID: 14645
	private ToolItem customToolOverride;

	// Token: 0x04003936 RID: 14646
	private int customToolAmount;

	// Token: 0x04003937 RID: 14647
	private ControlReminder.SingleConfig toolOverrideReminder;

	// Token: 0x04003938 RID: 14648
	private ToolItem lastEquippedAttackTool;

	// Token: 0x04003939 RID: 14649
	private ControlReminder.SingleConfig equipChangedToolSingleReminder;

	// Token: 0x0400393A RID: 14650
	private ControlReminder.DoubleConfig equipChangedToolModifierReminder;

	// Token: 0x0400393B RID: 14651
	private ControlReminder.ConfigBase queuedReminder;

	// Token: 0x0400393C RID: 14652
	private ToolItem unlockedTool;

	// Token: 0x0400393D RID: 14653
	private static ToolsActiveStates activeState;

	// Token: 0x0400393E RID: 14654
	private static float[] _startingCurrencyAmounts;

	// Token: 0x0400393F RID: 14655
	private static float[] _endingCurrencyAmounts;

	// Token: 0x04003940 RID: 14656
	private static Dictionary<ToolItemStatesLiquid, int> _liquidCostsTemp;

	// Token: 0x04003942 RID: 14658
	private static bool _sIsInfiniteToolUseEnabled;

	// Token: 0x04003943 RID: 14659
	private static readonly Dictionary<ToolItem, bool> toolCache = new Dictionary<ToolItem, bool>();

	// Token: 0x020018F4 RID: 6388
	[Flags]
	public enum OwnToolsCheckFlags
	{
		// Token: 0x040093E3 RID: 37859
		None = 0,
		// Token: 0x040093E4 RID: 37860
		Red = 1,
		// Token: 0x040093E5 RID: 37861
		Blue = 2,
		// Token: 0x040093E6 RID: 37862
		Yellow = 4,
		// Token: 0x040093E7 RID: 37863
		Skill = 8,
		// Token: 0x040093E8 RID: 37864
		AllTools = 7,
		// Token: 0x040093E9 RID: 37865
		All = -1
	}

	// Token: 0x020018F5 RID: 6389
	public enum ReplenishMethod
	{
		// Token: 0x040093EB RID: 37867
		Bench,
		// Token: 0x040093EC RID: 37868
		QuickCraft,
		// Token: 0x040093ED RID: 37869
		BenchSilent
	}

	// Token: 0x020018F6 RID: 6390
	public sealed class ToolStatus
	{
		// Token: 0x17001051 RID: 4177
		// (get) Token: 0x060092B8 RID: 37560 RVA: 0x0029C76C File Offset: 0x0029A96C
		public bool IsEquipped
		{
			get
			{
				if (this.version != ToolItemManager.Version)
				{
					this.version = ToolItemManager.Version;
					if (this.tool)
					{
						this.isEquipped = ToolItemManager.IsToolEquipped(this.tool, ToolEquippedReadSource.Active);
					}
					else
					{
						this.isEquipped = false;
					}
				}
				return this.isEquipped;
			}
		}

		// Token: 0x17001052 RID: 4178
		// (get) Token: 0x060092B9 RID: 37561 RVA: 0x0029C7C0 File Offset: 0x0029A9C0
		public bool IsEquippedCutscene
		{
			get
			{
				if (this.cutsceneVersion != ToolItemManager.Version)
				{
					this.cutsceneVersion = ToolItemManager.Version;
					if (this.tool)
					{
						this.cutsceneEquipped = ToolItemManager.IsToolEquipped(this.tool, ToolEquippedReadSource.Hud);
					}
					else
					{
						this.cutsceneEquipped = false;
					}
				}
				return this.cutsceneEquipped;
			}
		}

		// Token: 0x17001053 RID: 4179
		// (get) Token: 0x060092BA RID: 37562 RVA: 0x0029C813 File Offset: 0x0029AA13
		public bool IsEquippedAny
		{
			get
			{
				return this.IsEquipped || this.IsEquippedCutscene;
			}
		}

		// Token: 0x060092BB RID: 37563 RVA: 0x0029C825 File Offset: 0x0029AA25
		public ToolStatus(ToolItem tool)
		{
			this.tool = tool;
		}

		// Token: 0x040093EE RID: 37870
		private readonly ToolItem tool;

		// Token: 0x040093EF RID: 37871
		private int version;

		// Token: 0x040093F0 RID: 37872
		private bool isEquipped;

		// Token: 0x040093F1 RID: 37873
		private int cutsceneVersion;

		// Token: 0x040093F2 RID: 37874
		private bool cutsceneEquipped;
	}
}
