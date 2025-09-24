using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020002F3 RID: 755
[CreateAssetMenu(menuName = "Hornet/Enemy Journal Record")]
public class EnemyJournalRecord : QuestTargetCounter
{
	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001B08 RID: 6920 RVA: 0x0007DF64 File Offset: 0x0007C164
	public Sprite IconSprite
	{
		get
		{
			return this.iconSprite;
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001B09 RID: 6921 RVA: 0x0007DF6C File Offset: 0x0007C16C
	public Sprite EnemySprite
	{
		get
		{
			return this.enemySprite;
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001B0A RID: 6922 RVA: 0x0007DF74 File Offset: 0x0007C174
	public LocalisedString DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06001B0B RID: 6923 RVA: 0x0007DF7C File Offset: 0x0007C17C
	public LocalisedString Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001B0C RID: 6924 RVA: 0x0007DF84 File Offset: 0x0007C184
	public LocalisedString Notes
	{
		get
		{
			return this.notes;
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001B0D RID: 6925 RVA: 0x0007DF8C File Offset: 0x0007C18C
	public int KillsRequired
	{
		get
		{
			return this.killsRequired;
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001B0E RID: 6926 RVA: 0x0007DF94 File Offset: 0x0007C194
	public bool IsAlwaysUnlocked
	{
		get
		{
			return this.isAlwaysUnlocked;
		}
	}

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001B0F RID: 6927 RVA: 0x0007DF9C File Offset: 0x0007C19C
	public EnemyJournalRecord.RecordTypes RecordType
	{
		get
		{
			return this.recordType;
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001B10 RID: 6928 RVA: 0x0007DFA4 File Offset: 0x0007C1A4
	public bool IsRequiredForCompletion
	{
		get
		{
			bool result;
			switch (this.requiredType)
			{
			case EnemyJournalRecord.RequiredTypes.NotRequired:
				result = false;
				break;
			case EnemyJournalRecord.RequiredTypes.Required:
				result = true;
				break;
			case EnemyJournalRecord.RequiredTypes.RequiredSteelSoul:
				result = (PlayerData.instance.permadeathMode == PermadeathModes.On);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001B11 RID: 6929 RVA: 0x0007DFEB File Offset: 0x0007C1EB
	public bool SeenInJournal
	{
		get
		{
			return EnemyJournalManager.GetKillData(this).HasBeenSeen;
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001B12 RID: 6930 RVA: 0x0007DFF8 File Offset: 0x0007C1F8
	public int KillCount
	{
		get
		{
			return EnemyJournalManager.GetKillData(this).Kills;
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001B13 RID: 6931 RVA: 0x0007E005 File Offset: 0x0007C205
	public bool IsVisible
	{
		get
		{
			return this.IsAlwaysUnlocked || this.KillCount > 0;
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001B14 RID: 6932 RVA: 0x0007E01A File Offset: 0x0007C21A
	public IEnumerable<EnemyJournalRecord> CompleteOthers
	{
		get
		{
			return this.completeOthers;
		}
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x0007E022 File Offset: 0x0007C222
	private void OnValidate()
	{
		if (this.isRequiredForCompletion)
		{
			this.requiredType = EnemyJournalRecord.RequiredTypes.Required;
			this.isRequiredForCompletion = false;
		}
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x0007E03A File Offset: 0x0007C23A
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x0007E042 File Offset: 0x0007C242
	public override Sprite GetQuestCounterSprite(int index)
	{
		return this.EnemySprite;
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x0007E04C File Offset: 0x0007C24C
	public void PopulateLocalisations(string sheet, string subKey)
	{
		this.displayName = new LocalisedString
		{
			Sheet = sheet,
			Key = "NAME_" + subKey
		};
		this.description = new LocalisedString
		{
			Sheet = sheet,
			Key = "DESC_" + subKey
		};
		this.notes = new LocalisedString
		{
			Sheet = sheet,
			Key = "NOTE_" + subKey
		};
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x0007E0D4 File Offset: 0x0007C2D4
	public override void Get(bool showPopup = true)
	{
		EnemyJournalManager.RecordKill(this, showPopup);
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x0007E0DD File Offset: 0x0007C2DD
	public override bool CanGetMore()
	{
		return true;
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x0007E0E0 File Offset: 0x0007C2E0
	public override Sprite GetPopupIcon()
	{
		return this.IconSprite;
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x0007E0E8 File Offset: 0x0007C2E8
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return this.KillCount;
	}

	// Token: 0x04001A09 RID: 6665
	[SerializeField]
	private Sprite iconSprite;

	// Token: 0x04001A0A RID: 6666
	[SerializeField]
	private Sprite enemySprite;

	// Token: 0x04001A0B RID: 6667
	[SerializeField]
	private LocalisedString displayName = new LocalisedString
	{
		Sheet = "Journal"
	};

	// Token: 0x04001A0C RID: 6668
	[SerializeField]
	private LocalisedString description = new LocalisedString
	{
		Sheet = "Journal"
	};

	// Token: 0x04001A0D RID: 6669
	[SerializeField]
	private LocalisedString notes = new LocalisedString
	{
		Sheet = "Journal"
	};

	// Token: 0x04001A0E RID: 6670
	[SerializeField]
	private int killsRequired = 10;

	// Token: 0x04001A0F RID: 6671
	[SerializeField]
	private bool isAlwaysUnlocked;

	// Token: 0x04001A10 RID: 6672
	[SerializeField]
	private EnemyJournalRecord.RecordTypes recordType;

	// Token: 0x04001A11 RID: 6673
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool isRequiredForCompletion = true;

	// Token: 0x04001A12 RID: 6674
	[SerializeField]
	private EnemyJournalRecord.RequiredTypes requiredType = EnemyJournalRecord.RequiredTypes.Required;

	// Token: 0x04001A13 RID: 6675
	[Space]
	[SerializeField]
	private EnemyJournalRecord[] completeOthers;

	// Token: 0x020015E1 RID: 5601
	public enum RecordTypes
	{
		// Token: 0x040088F4 RID: 35060
		Enemy,
		// Token: 0x040088F5 RID: 35061
		Other
	}

	// Token: 0x020015E2 RID: 5602
	private enum RequiredTypes
	{
		// Token: 0x040088F7 RID: 35063
		NotRequired,
		// Token: 0x040088F8 RID: 35064
		Required,
		// Token: 0x040088F9 RID: 35065
		RequiredSteelSoul
	}
}
