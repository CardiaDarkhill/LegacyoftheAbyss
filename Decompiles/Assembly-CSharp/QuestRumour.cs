using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Rumour")]
public class QuestRumour : BasicQuestBase
{
	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x060033E8 RID: 13288 RVA: 0x000E7B31 File Offset: 0x000E5D31
	public override QuestType QuestType
	{
		get
		{
			return this.questType;
		}
	}

	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x060033E9 RID: 13289 RVA: 0x000E7B3C File Offset: 0x000E5D3C
	public override bool IsAvailable
	{
		get
		{
			if (this.linkedQuest && this.linkedQuest.IsAccepted)
			{
				return false;
			}
			Quest[] array = this.requiredCompleteQuests;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsCompleted)
				{
					return false;
				}
			}
			return this.playerDataTest.IsFulfilled;
		}
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x060033EA RID: 13290 RVA: 0x000E7B91 File Offset: 0x000E5D91
	public override bool IsAccepted
	{
		get
		{
			return this.SavedData.IsAccepted;
		}
	}

	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x060033EB RID: 13291 RVA: 0x000E7B9E File Offset: 0x000E5D9E
	public override bool IsHidden
	{
		get
		{
			return !this.IsAvailable;
		}
	}

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x060033EC RID: 13292 RVA: 0x000E7BA9 File Offset: 0x000E5DA9
	// (set) Token: 0x060033ED RID: 13293 RVA: 0x000E7BB8 File Offset: 0x000E5DB8
	public override bool HasBeenSeen
	{
		get
		{
			return this.SavedData.HasBeenSeen;
		}
		set
		{
			QuestRumourData.Data savedData = this.SavedData;
			savedData.HasBeenSeen = value;
			this.SavedData = savedData;
		}
	}

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x060033EE RID: 13294 RVA: 0x000E7BDB File Offset: 0x000E5DDB
	public override bool IsMapMarkerVisible
	{
		get
		{
			return (!this.linkedQuest || !this.linkedQuest.IsAccepted) && this.IsAccepted;
		}
	}

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x060033EF RID: 13295 RVA: 0x000E7BFF File Offset: 0x000E5DFF
	// (set) Token: 0x060033F0 RID: 13296 RVA: 0x000E7C1B File Offset: 0x000E5E1B
	public QuestRumourData.Data SavedData
	{
		get
		{
			return GameManager.instance.playerData.QuestRumourData.GetData(base.name);
		}
		set
		{
			GameManager.instance.playerData.QuestRumourData.SetData(base.name, value);
			QuestManager.IncrementVersion();
		}
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x000E7C3D File Offset: 0x000E5E3D
	private void OnEnable()
	{
		this.questType = QuestType.Create(this.typeDisplayName, this.typeIcon, Color.white, null, null, null);
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x000E7C5E File Offset: 0x000E5E5E
	private void OnDisable()
	{
		Object.DestroyImmediate(this.questType);
		this.questType = null;
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x000E7C74 File Offset: 0x000E5E74
	public override void Get(bool showPopup = true)
	{
		QuestRumourData.Data savedData = this.SavedData;
		if (savedData.IsAccepted)
		{
			return;
		}
		savedData.IsAccepted = true;
		this.SavedData = savedData;
		if (!showPopup)
		{
			return;
		}
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = "TODO: Can still take rumours?",
			Icon = this.typeIcon,
			IconScale = 1f,
			RepresentingObject = this
		}, null, true);
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x000E7CE7 File Offset: 0x000E5EE7
	public override bool CanGetMore()
	{
		return !this.SavedData.IsAccepted;
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x000E7CF7 File Offset: 0x000E5EF7
	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield return this;
		yield break;
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x000E7D07 File Offset: 0x000E5F07
	public override string GetDescription(BasicQuestBase.ReadSource readSource)
	{
		return this.description;
	}

	// Token: 0x0400379C RID: 14236
	[Header("- Quest Rumour")]
	[SerializeField]
	private LocalisedString description;

	// Token: 0x0400379D RID: 14237
	[Space]
	[SerializeField]
	private LocalisedString typeDisplayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	// Token: 0x0400379E RID: 14238
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIcon;

	// Token: 0x0400379F RID: 14239
	[Space]
	[SerializeField]
	private FullQuestBase linkedQuest;

	// Token: 0x040037A0 RID: 14240
	[Header("Appear Conditions")]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	// Token: 0x040037A1 RID: 14241
	[SerializeField]
	private Quest[] requiredCompleteQuests;

	// Token: 0x040037A2 RID: 14242
	private QuestType questType;
}
