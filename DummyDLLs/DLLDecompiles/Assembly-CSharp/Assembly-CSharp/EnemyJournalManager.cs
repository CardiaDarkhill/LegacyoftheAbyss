using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class EnemyJournalManager : MonoBehaviour
{
	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001AF2 RID: 6898 RVA: 0x0007D9C8 File Offset: 0x0007BBC8
	private static EnemyJournalManager Instance
	{
		get
		{
			if (!EnemyJournalManager._instance)
			{
				EnemyJournalManager._instance = Object.FindObjectOfType<EnemyJournalManager>();
			}
			return EnemyJournalManager._instance;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001AF3 RID: 6899 RVA: 0x0007D9E5 File Offset: 0x0007BBE5
	// (set) Token: 0x06001AF4 RID: 6900 RVA: 0x0007D9FF File Offset: 0x0007BBFF
	public static EnemyJournalRecord UpdatedRecord
	{
		get
		{
			if (!EnemyJournalManager.Instance)
			{
				return null;
			}
			return EnemyJournalManager.Instance.updatedRecord;
		}
		set
		{
			if (EnemyJournalManager.Instance)
			{
				EnemyJournalManager.Instance.updatedRecord = value;
			}
		}
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x0007DA18 File Offset: 0x0007BC18
	private void Awake()
	{
		EnemyJournalManager._instance = this;
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x0007DA20 File Offset: 0x0007BC20
	private void Start()
	{
		if (this.journalUpdateMessagePrefab)
		{
			this.journalUpdateMessage = Object.Instantiate<GameObject>(this.journalUpdateMessagePrefab, base.transform, true);
			this.journalUpdateMessage.SetActive(false);
		}
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x0007DA53 File Offset: 0x0007BC53
	private void OnDestroy()
	{
		if (EnemyJournalManager._instance == this)
		{
			EnemyJournalManager._instance = null;
		}
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x0007DA68 File Offset: 0x0007BC68
	public static void RecordKill(EnemyJournalRecord journalRecord, bool showPopup, bool forcePopup)
	{
		GameManager instance = GameManager.instance;
		if (instance)
		{
			int num;
			int num2;
			EnemyJournalManager.RecordKillToJournalData(journalRecord.name, instance.playerData.EnemyJournalKillData, out num, out num2);
			bool flag = num2 == journalRecord.KillsRequired || (num == 0 && journalRecord.KillsRequired == 0);
			if (flag)
			{
				foreach (EnemyJournalRecord enemyJournalRecord in journalRecord.CompleteOthers)
				{
					if (!(enemyJournalRecord == null))
					{
						while (enemyJournalRecord.KillCount < enemyJournalRecord.KillsRequired)
						{
							EnemyJournalManager.RecordKill(enemyJournalRecord, false, false);
						}
					}
				}
			}
			if (showPopup && EnemyJournalManager.Instance && instance.playerData.hasJournal && ToolItemManager.ActiveState != ToolsActiveStates.Cutscene && !HeroController.instance.Config.ForceBareInventory)
			{
				EnemyJournalManager.Instance.ShowJournalUpdateMessage(num == 0, flag || forcePopup, journalRecord);
			}
		}
		journalRecord.Increment();
		if (showPopup)
		{
			QuestManager.MaybeShowQuestUpdated(null, journalRecord, null);
		}
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x0007DB7C File Offset: 0x0007BD7C
	public static void RecordKill(EnemyJournalRecord journalRecord, bool showPopup = true)
	{
		EnemyJournalManager.RecordKill(journalRecord, showPopup, false);
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x0007DB86 File Offset: 0x0007BD86
	[Obsolete]
	public static void CheckJournalAchievements()
	{
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x0007DB88 File Offset: 0x0007BD88
	public static int GetCompletedEnemiesCount()
	{
		if (!EnemyJournalManager.Instance)
		{
			return 0;
		}
		int num = 0;
		foreach (EnemyJournalRecord enemyJournalRecord in EnemyJournalManager.Instance.recordList)
		{
			if (enemyJournalRecord.IsRequiredForCompletion && enemyJournalRecord.KillCount >= enemyJournalRecord.KillsRequired)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x0007DC00 File Offset: 0x0007BE00
	public static void RecordKillToJournalData(string journalRecordName, EnemyJournalKillData journalData, out int previousKills, out int currentKills)
	{
		EnemyJournalKillData.KillData killData = journalData.GetKillData(journalRecordName);
		previousKills = killData.Kills;
		killData.Kills++;
		journalData.RecordKillData(journalRecordName, killData);
		currentKills = killData.Kills;
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x0007DC3C File Offset: 0x0007BE3C
	public static EnemyJournalKillData.KillData GetKillData(EnemyJournalRecord journalRecord)
	{
		GameManager instance = GameManager.instance;
		if (!journalRecord || !instance)
		{
			return default(EnemyJournalKillData.KillData);
		}
		return instance.playerData.EnemyJournalKillData.GetKillData(journalRecord.name);
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x0007DC80 File Offset: 0x0007BE80
	public static void SetJournalSeen(EnemyJournalRecord journalRecord)
	{
		GameManager instance = GameManager.instance;
		if (!journalRecord || !instance)
		{
			return;
		}
		EnemyJournalKillData.KillData killData = instance.playerData.EnemyJournalKillData.GetKillData(journalRecord.name);
		killData.HasBeenSeen = true;
		instance.playerData.EnemyJournalKillData.RecordKillData(journalRecord.name, killData);
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x0007DCDC File Offset: 0x0007BEDC
	private void ShowJournalUpdateMessage(bool isFirstKill, bool isFinalKill, EnemyJournalRecord record)
	{
		bool flag = false;
		if (isFinalKill)
		{
			flag = true;
		}
		else if (isFirstKill)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		this.updatedRecord = record;
		InventoryPaneList.SetNextOpen("Journal");
		if (!this.journalUpdateMessage)
		{
			return;
		}
		if (this.journalUpdateMessage.activeSelf)
		{
			this.journalUpdateMessage.SetActive(false);
		}
		this.journalUpdateMessage.SetActive(true);
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(this.journalUpdateMessage, "Journal Msg");
		if (playMakerFSM)
		{
			FSMUtility.SetBool(playMakerFSM, "Full", isFinalKill);
			FSMUtility.SetBool(playMakerFSM, "Should Recycle", true);
		}
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x0007DD70 File Offset: 0x0007BF70
	public static List<EnemyJournalRecord> GetKilledEnemies()
	{
		return EnemyJournalManager.GetEnemies(EnemyJournalManager.CheckTypes.Seen);
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x0007DD78 File Offset: 0x0007BF78
	public static List<EnemyJournalRecord> GetCompletedEnemies()
	{
		return EnemyJournalManager.GetEnemies(EnemyJournalManager.CheckTypes.Completed);
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x0007DD80 File Offset: 0x0007BF80
	public static List<EnemyJournalRecord> GetRequiredEnemies()
	{
		return EnemyJournalManager.GetEnemies(EnemyJournalManager.CheckTypes.Required);
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x0007DD88 File Offset: 0x0007BF88
	public static List<EnemyJournalRecord> GetAllEnemies()
	{
		return EnemyJournalManager.GetEnemies(EnemyJournalManager.CheckTypes.All);
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x0007DD90 File Offset: 0x0007BF90
	public static EnemyJournalRecord GetRecord(string name)
	{
		if (!EnemyJournalManager.Instance || !EnemyJournalManager.Instance.recordList)
		{
			return null;
		}
		return EnemyJournalManager.Instance.recordList.GetByName(name);
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x0007DDC4 File Offset: 0x0007BFC4
	private static List<EnemyJournalRecord> GetEnemies(EnemyJournalManager.CheckTypes checkType)
	{
		if (!EnemyJournalManager.Instance || !EnemyJournalManager.Instance.recordList)
		{
			return new List<EnemyJournalRecord>();
		}
		if (checkType == EnemyJournalManager.CheckTypes.All || !Application.isPlaying)
		{
			return EnemyJournalManager.Instance.recordList.ToList<EnemyJournalRecord>();
		}
		switch (checkType)
		{
		case EnemyJournalManager.CheckTypes.Seen:
			return (from record in EnemyJournalManager.Instance.recordList
			where record.IsVisible
			select record).ToList<EnemyJournalRecord>();
		case EnemyJournalManager.CheckTypes.Completed:
			return (from record in EnemyJournalManager.Instance.recordList
			where record.KillCount >= record.KillsRequired
			select record).ToList<EnemyJournalRecord>();
		case EnemyJournalManager.CheckTypes.Required:
			return (from record in EnemyJournalManager.Instance.recordList
			where record.IsRequiredForCompletion || record.IsVisible
			select record).ToList<EnemyJournalRecord>();
		default:
			throw new ArgumentOutOfRangeException("checkType", checkType, null);
		}
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x0007DED4 File Offset: 0x0007C0D4
	public static bool IsAllRequiredComplete()
	{
		EnemyJournalManager instance = EnemyJournalManager.Instance;
		if (!instance || !instance.recordList)
		{
			return false;
		}
		foreach (EnemyJournalRecord enemyJournalRecord in instance.recordList)
		{
			if (enemyJournalRecord.IsRequiredForCompletion && !enemyJournalRecord.IsAlwaysUnlocked && enemyJournalRecord.KillCount < enemyJournalRecord.KillsRequired)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04001A04 RID: 6660
	private static EnemyJournalManager _instance;

	// Token: 0x04001A05 RID: 6661
	[SerializeField]
	private EnemyJournalRecordList recordList;

	// Token: 0x04001A06 RID: 6662
	[SerializeField]
	private GameObject journalUpdateMessagePrefab;

	// Token: 0x04001A07 RID: 6663
	private GameObject journalUpdateMessage;

	// Token: 0x04001A08 RID: 6664
	private EnemyJournalRecord updatedRecord;

	// Token: 0x020015DF RID: 5599
	private enum CheckTypes
	{
		// Token: 0x040088EB RID: 35051
		Seen,
		// Token: 0x040088EC RID: 35052
		Completed,
		// Token: 0x040088ED RID: 35053
		Required,
		// Token: 0x040088EE RID: 35054
		All
	}
}
