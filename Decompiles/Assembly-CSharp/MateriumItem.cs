using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001C8 RID: 456
[CreateAssetMenu(menuName = "Hornet/Materium/Materium Item")]
public class MateriumItem : SavedItem, ICollectionViewerItem
{
	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060011DA RID: 4570 RVA: 0x000538D4 File Offset: 0x00051AD4
	public bool IsRequiredForCompletion
	{
		get
		{
			bool result;
			switch (this.requiredType)
			{
			case MateriumItem.RequiredTypes.NotRequired:
				result = false;
				break;
			case MateriumItem.RequiredTypes.Required:
				result = true;
				break;
			case MateriumItem.RequiredTypes.RequiredSteelSoul:
				result = (PlayerData.instance.permadeathMode == PermadeathModes.On);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060011DB RID: 4571 RVA: 0x0005391B File Offset: 0x00051B1B
	public LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x060011DC RID: 4572 RVA: 0x00053923 File Offset: 0x00051B23
	public LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x060011DD RID: 4573 RVA: 0x0005392B File Offset: 0x00051B2B
	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x060011DE RID: 4574 RVA: 0x00053933 File Offset: 0x00051B33
	// (set) Token: 0x060011DF RID: 4575 RVA: 0x0005394A File Offset: 0x00051B4A
	private MateriumItemsData.Data SavedData
	{
		get
		{
			return PlayerData.instance.MateriumCollected.GetData(base.name);
		}
		set
		{
			PlayerData.instance.MateriumCollected.SetData(base.name, value);
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x060011E0 RID: 4576 RVA: 0x00053964 File Offset: 0x00051B64
	public bool IsCollected
	{
		get
		{
			if (this.SavedData.IsCollected)
			{
				return true;
			}
			if (!this.playerDataCondition.IsFulfilled)
			{
				return false;
			}
			this.OnValidate();
			if (this.itemQuests.Count > 0)
			{
				using (List<Quest>.Enumerator enumerator = this.itemQuests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsCompleted)
						{
							return true;
						}
					}
				}
				return false;
			}
			return this.collectedByDefault || this.playerDataCondition.IsDefined;
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060011E1 RID: 4577 RVA: 0x00053A08 File Offset: 0x00051C08
	// (set) Token: 0x060011E2 RID: 4578 RVA: 0x00053A18 File Offset: 0x00051C18
	public bool IsSeen
	{
		get
		{
			return this.SavedData.HasSeenInRelicBoard;
		}
		set
		{
			MateriumItemsData.Data savedData = this.SavedData;
			savedData.HasSeenInRelicBoard = value;
			this.SavedData = savedData;
		}
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x00053A3B File Offset: 0x00051C3B
	private void OnValidate()
	{
		if (this.itemQuest != null)
		{
			this.itemQuests.Add(this.itemQuest);
			this.itemQuest = null;
		}
		this.itemQuests.RemoveNulls<Quest>();
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x00053A6E File Offset: 0x00051C6E
	public string GetCollectionName()
	{
		return this.displayName;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x00053A7B File Offset: 0x00051C7B
	public string GetCollectionDesc()
	{
		return this.description;
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x00053A88 File Offset: 0x00051C88
	public Sprite GetCollectionIcon()
	{
		return this.icon;
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x00053A90 File Offset: 0x00051C90
	public bool IsVisibleInCollection()
	{
		return this.IsCollected;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x00053A98 File Offset: 0x00051C98
	public bool IsRequiredInCollection()
	{
		return this.IsRequiredForCompletion;
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x00053AA0 File Offset: 0x00051CA0
	public override void Get(bool showPopup = true)
	{
		PlayerData instance = PlayerData.instance;
		MateriumItemsData.Data data = instance.MateriumCollected.GetData(base.name);
		if (data.IsCollected)
		{
			return;
		}
		data.IsCollected = true;
		instance.MateriumCollected.SetData(base.name, data);
		if (!MateriumItemManager.ConstructedMaterium)
		{
			return;
		}
		MateriumItemManager.CheckAchievements();
		if (showPopup)
		{
			MateriumItemManager.ShowUpdateMessage();
		}
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x00053AFD File Offset: 0x00051CFD
	public override bool CanGetMore()
	{
		return !this.IsCollected;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x00053B08 File Offset: 0x00051D08
	public override Sprite GetPopupIcon()
	{
		return this.icon;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x00053B10 File Offset: 0x00051D10
	public override string GetPopupName()
	{
		return this.displayName;
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x00053B2C File Offset: 0x00051D2C
	string ICollectionViewerItem.get_name()
	{
		return base.name;
	}

	// Token: 0x040010C2 RID: 4290
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x040010C3 RID: 4291
	[SerializeField]
	private LocalisedString description;

	// Token: 0x040010C4 RID: 4292
	[SerializeField]
	private Sprite icon;

	// Token: 0x040010C5 RID: 4293
	[Space]
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private Quest itemQuest;

	// Token: 0x040010C6 RID: 4294
	[SerializeField]
	private List<Quest> itemQuests;

	// Token: 0x040010C7 RID: 4295
	[SerializeField]
	private PlayerDataTest playerDataCondition;

	// Token: 0x040010C8 RID: 4296
	[SerializeField]
	private bool collectedByDefault;

	// Token: 0x040010C9 RID: 4297
	[SerializeField]
	private MateriumItem.RequiredTypes requiredType = MateriumItem.RequiredTypes.Required;

	// Token: 0x0200150B RID: 5387
	private enum RequiredTypes
	{
		// Token: 0x040085B2 RID: 34226
		NotRequired,
		// Token: 0x040085B3 RID: 34227
		Required,
		// Token: 0x040085B4 RID: 34228
		RequiredSteelSoul
	}
}
