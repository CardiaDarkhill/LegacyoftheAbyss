using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200058C RID: 1420
public abstract class BasicQuestBase : QuestGroupBase
{
	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x060032CE RID: 13006
	public abstract QuestType QuestType { get; }

	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x060032CF RID: 13007 RVA: 0x000E219A File Offset: 0x000E039A
	public LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x060032D0 RID: 13008 RVA: 0x000E21A2 File Offset: 0x000E03A2
	public string Location
	{
		get
		{
			if (!this.location.IsEmpty)
			{
				return this.location;
			}
			return string.Empty;
		}
	}

	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x060032D1 RID: 13009
	public abstract bool IsAvailable { get; }

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x060032D2 RID: 13010
	public abstract bool IsAccepted { get; }

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x060032D3 RID: 13011
	public abstract bool IsHidden { get; }

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x060032D4 RID: 13012
	// (set) Token: 0x060032D5 RID: 13013
	public abstract bool HasBeenSeen { get; set; }

	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x060032D6 RID: 13014
	public abstract bool IsMapMarkerVisible { get; }

	// Token: 0x060032D7 RID: 13015
	public abstract string GetDescription(BasicQuestBase.ReadSource readSource);

	// Token: 0x060032D8 RID: 13016 RVA: 0x000E21C2 File Offset: 0x000E03C2
	public void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		this.DoInit();
	}

	// Token: 0x060032D9 RID: 13017 RVA: 0x000E21DA File Offset: 0x000E03DA
	protected virtual void DoInit()
	{
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x000E21DC File Offset: 0x000E03DC
	public void OnSelected()
	{
		this.HasBeenSeen = true;
	}

	// Token: 0x060032DB RID: 13019 RVA: 0x000E21E5 File Offset: 0x000E03E5
	public override string GetPopupName()
	{
		return this.DisplayName;
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x000E21F2 File Offset: 0x000E03F2
	public override void Get(bool showPopup = true)
	{
		QuestManager.IncrementVersion();
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x000E21FC File Offset: 0x000E03FC
	public override Sprite GetPopupIcon()
	{
		QuestType questType = this.QuestType;
		if (!questType)
		{
			return null;
		}
		return questType.Icon;
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x000E2220 File Offset: 0x000E0420
	public static void SetInventoryNewItem(BasicQuestBase quest)
	{
		InventoryPaneList.SetNextOpen("Quests");
		QuestManager.UpdatedQuest = quest;
		PlayerData.instance.QuestPaneHasNew = true;
	}

	// Token: 0x040036BD RID: 14013
	[Header("- Basic Quest Base")]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x040036BE RID: 14014
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString location;

	// Token: 0x040036BF RID: 14015
	private bool init;

	// Token: 0x020018A3 RID: 6307
	public enum ReadSource
	{
		// Token: 0x040092D8 RID: 37592
		Inventory,
		// Token: 0x040092D9 RID: 37593
		QuestBoard
	}
}
