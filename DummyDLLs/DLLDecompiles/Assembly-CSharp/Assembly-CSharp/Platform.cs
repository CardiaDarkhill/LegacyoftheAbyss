using System;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x0200045A RID: 1114
public abstract class Platform : MonoBehaviour
{
	// Token: 0x1400007C RID: 124
	// (add) Token: 0x06002771 RID: 10097 RVA: 0x000B0F3C File Offset: 0x000AF13C
	// (remove) Token: 0x06002772 RID: 10098 RVA: 0x000B0F70 File Offset: 0x000AF170
	public static event Action PlatformBecameCurrent;

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x06002773 RID: 10099
	public abstract string DisplayName { get; }

	// Token: 0x06002774 RID: 10100 RVA: 0x000B0FA3 File Offset: 0x000AF1A3
	public virtual SystemLanguage GetSystemLanguage()
	{
		return Application.systemLanguage;
	}

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002775 RID: 10101
	public abstract bool ShowLanguageSelect { get; }

	// Token: 0x06002776 RID: 10102 RVA: 0x000B0FAC File Offset: 0x000AF1AC
	public virtual int GetEstimateAllocatableMemoryMB()
	{
		long totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
		long totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
		int num = (int)((totalReservedMemoryLong - totalUnusedReservedMemoryLong) / 1024L / 1024L);
		int b = SystemInfo.systemMemorySize / 2;
		return Mathf.Max((int)(totalReservedMemoryLong / 1024L / 1024L), b) - num;
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002777 RID: 10103 RVA: 0x000B0FF6 File Offset: 0x000AF1F6
	public virtual bool IsRunningOnHandHeld
	{
		get
		{
			return this.ScreenMode >= Platform.ScreenModeState.HandHeld;
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002778 RID: 10104 RVA: 0x000B1004 File Offset: 0x000AF204
	// (set) Token: 0x06002779 RID: 10105 RVA: 0x000B100C File Offset: 0x000AF20C
	public virtual Platform.ScreenModeState ScreenMode
	{
		get
		{
			return this.screenMode;
		}
		set
		{
			this.SetScreenMode(value);
		}
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000B1015 File Offset: 0x000AF215
	protected void SetScreenMode(Platform.ScreenModeState newState)
	{
		if (this.screenMode != newState)
		{
			this.screenMode = newState;
			Platform.ScreenModeChanged onScreenModeChanged = this.OnScreenModeChanged;
			if (onScreenModeChanged == null)
			{
				return;
			}
			onScreenModeChanged(newState);
		}
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x0600277B RID: 10107 RVA: 0x000B1038 File Offset: 0x000AF238
	public virtual Platform.HandHeldTypes HandHeldType
	{
		get
		{
			return Platform.HandHeldTypes.None;
		}
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000B103B File Offset: 0x000AF23B
	public bool IsTargetHandHeld(Platform.HandHeldTypes targetType)
	{
		return targetType == Platform.HandHeldTypes.None || (this.HandHeldType & targetType) > Platform.HandHeldTypes.None;
	}

	// Token: 0x1400007D RID: 125
	// (add) Token: 0x0600277D RID: 10109 RVA: 0x000B1050 File Offset: 0x000AF250
	// (remove) Token: 0x0600277E RID: 10110 RVA: 0x000B1088 File Offset: 0x000AF288
	public event Platform.ScreenModeChanged OnScreenModeChanged;

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x0600277F RID: 10111 RVA: 0x000B10BD File Offset: 0x000AF2BD
	public virtual float EnterSceneWait
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002780 RID: 10112 RVA: 0x000B10C4 File Offset: 0x000AF2C4
	public virtual bool IsFileSystemProtected
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002781 RID: 10113 RVA: 0x000B10C7 File Offset: 0x000AF2C7
	public virtual bool WillPreloadSaveFiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002782 RID: 10114 RVA: 0x000B10CA File Offset: 0x000AF2CA
	public virtual bool ShowSaveFileWriteIcon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002783 RID: 10115 RVA: 0x000B10CD File Offset: 0x000AF2CD
	public virtual bool IsFileWriteLimited
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002784 RID: 10116 RVA: 0x000B10D0 File Offset: 0x000AF2D0
	public virtual bool IsSaveStoreMounted
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06002785 RID: 10117 RVA: 0x000B10D3 File Offset: 0x000AF2D3
	public virtual bool IsSharedDataMounted
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1400007E RID: 126
	// (add) Token: 0x06002786 RID: 10118 RVA: 0x000B10D8 File Offset: 0x000AF2D8
	// (remove) Token: 0x06002787 RID: 10119 RVA: 0x000B110C File Offset: 0x000AF30C
	public static event Platform.SaveStoreMountEvent OnSaveStoreStateChanged;

	// Token: 0x06002788 RID: 10120 RVA: 0x000B1140 File Offset: 0x000AF340
	public void NotifySaveMountStateChanged(bool mounted)
	{
		if (Platform.OnSaveStoreStateChanged != null)
		{
			CoreLoop.InvokeSafe(delegate
			{
				Platform.OnSaveStoreStateChanged(mounted);
			});
		}
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000B1172 File Offset: 0x000AF372
	public virtual void LoadSharedDataAndNotify(bool mounted)
	{
		this.NotifySaveMountStateChanged(mounted);
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000B117B File Offset: 0x000AF37B
	public virtual void MountSaveStore()
	{
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000B117D File Offset: 0x000AF37D
	public static bool IsSaveSlotIndexValid(int slotIndex)
	{
		return slotIndex >= 0 && slotIndex < 5;
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000B118C File Offset: 0x000AF38C
	protected string GetSaveSlotFileName(int slotIndex, Platform.SaveSlotFileNameUsage usage)
	{
		string text;
		if (slotIndex == 0)
		{
			text = "user.dat";
		}
		else
		{
			text = string.Format("user{0}.dat", slotIndex);
		}
		if (usage != Platform.SaveSlotFileNameUsage.Backup)
		{
			if (usage == Platform.SaveSlotFileNameUsage.BackupMarkedForDeletion)
			{
				text += ".del";
			}
		}
		else
		{
			text += ".bak";
		}
		return text;
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000B11DA File Offset: 0x000AF3DA
	public virtual void PrepareForNewGame(int slotIndex)
	{
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000B11DC File Offset: 0x000AF3DC
	public virtual void OnSetGameData(int slotIndex)
	{
	}

	// Token: 0x0600278F RID: 10127
	public abstract void IsSaveSlotInUse(int slotIndex, Action<bool> callback);

	// Token: 0x06002790 RID: 10128
	public abstract void ReadSaveSlot(int slotIndex, Action<byte[]> callback);

	// Token: 0x06002791 RID: 10129
	public abstract void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback);

	// Token: 0x06002792 RID: 10130
	public abstract void WriteSaveSlot(int slotIndex, byte[] binary, Action<bool> callback);

	// Token: 0x06002793 RID: 10131
	public abstract void ClearSaveSlot(int slotIndex, Action<bool> callback);

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06002794 RID: 10132
	public abstract Platform.ISharedData LocalSharedData { get; }

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06002795 RID: 10133
	public abstract Platform.ISharedData RoamingSharedData { get; }

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06002796 RID: 10134 RVA: 0x000B11DE File Offset: 0x000AF3DE
	public virtual string UserDataDirectory
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000B11E5 File Offset: 0x000AF3E5
	public virtual void SaveScreenCapture(Texture2D texture2D, Action<bool> callback)
	{
		Debug.LogError("SaveScreenCapture not implemented");
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000B11FB File Offset: 0x000AF3FB
	public virtual void LoadScreenCaptures(Action<Texture2D[]> captures)
	{
		Debug.LogError("LoadScreenCaptures not implemented");
		captures(null);
	}

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x06002799 RID: 10137 RVA: 0x000B120E File Offset: 0x000AF40E
	public virtual SaveRestoreHandler SaveRestoreHandler { get; }

	// Token: 0x0600279A RID: 10138 RVA: 0x000B1218 File Offset: 0x000AF418
	public void CreateSaveRestorePoint(int slot, string identifier, bool noTrim, byte[] bytes, Action<bool> callback = null)
	{
		if (this.SaveRestoreHandler != null)
		{
			Action<bool> callback2 = (callback == null) ? null : new Action<bool>(delegate(bool success)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			});
			this.SaveRestoreHandler.WriteSaveRestorePoint(slot, identifier, noTrim, bytes, callback2);
			return;
		}
		Debug.LogError("Unable to create save restore point. Missing Save Restore Handler.", this);
		Action<bool> callback3 = callback;
		if (callback3 == null)
		{
			return;
		}
		callback3(false);
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000B1284 File Offset: 0x000AF484
	public void WriteSaveBackup(int slot, byte[] bytes, Action<bool> callback = null)
	{
		if (this.SaveRestoreHandler != null)
		{
			Action<bool> callback2 = (callback == null) ? null : new Action<bool>(delegate(bool success)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			});
			this.SaveRestoreHandler.WriteVersionBackup(slot, bytes, callback2);
			return;
		}
		Debug.LogError("Unable to write save backup point. Missing Save Restore Handler.", this);
		Action<bool> callback3 = callback;
		if (callback3 == null)
		{
			return;
		}
		callback3(false);
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000B12E9 File Offset: 0x000AF4E9
	public FetchDataRequest FetchRestorePoints(int slot)
	{
		if (this.SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to fetch save restore point. Missing Save Restore Handler.", this);
			return FetchDataRequest.Error;
		}
		return this.SaveRestoreHandler.FetchRestorePoints(slot);
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000B1310 File Offset: 0x000AF510
	public FetchDataRequest FetchVersionRestorePoints(int slot)
	{
		if (this.SaveRestoreHandler == null)
		{
			Debug.LogError("Unable to fetch version restore point. Missing Save Restore Handler.", this);
			return FetchDataRequest.Error;
		}
		return this.SaveRestoreHandler.FetchVersionBackupPoints(slot);
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000B1338 File Offset: 0x000AF538
	public void DeleteRestorePointsForSlot(int slot, Action<bool> callback = null)
	{
		if (this.SaveRestoreHandler != null)
		{
			Action<bool> callback2 = (callback == null) ? null : new Action<bool>(delegate(bool success)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			});
			this.SaveRestoreHandler.DeleteRestorePoints(slot, callback2);
			return;
		}
		Debug.LogError("Unable to delete save restore point. Missing Save Restore Handler.", this);
		Action<bool> callback3 = callback;
		if (callback3 == null)
		{
			return;
		}
		callback3(false);
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000B139C File Offset: 0x000AF59C
	public void DeleteVersionBackupsForSlot(int slot, Action<bool> callback = null)
	{
		if (this.SaveRestoreHandler != null)
		{
			Action<bool> callback2 = (callback == null) ? null : new Action<bool>(delegate(bool success)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			});
			this.SaveRestoreHandler.DeleteVersionBackups(slot, callback2);
			return;
		}
		Debug.LogError("Unable to delete backup files from previous versions. Missing Save Restore Handler.", this);
		Action<bool> callback3 = callback;
		if (callback3 == null)
		{
			return;
		}
		callback3(false);
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x060027A0 RID: 10144 RVA: 0x000B1400 File Offset: 0x000AF600
	public virtual bool ShowSaveDataImport
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x060027A1 RID: 10145 RVA: 0x000B1403 File Offset: 0x000AF603
	public virtual string SaveImportLabel
	{
		get
		{
			return "Save Import";
		}
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000B140A File Offset: 0x000AF60A
	public virtual void FetchImportData(Action<List<Platform.ImportDataInfo>> callback)
	{
		if (callback != null)
		{
			callback(null);
		}
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000B1416 File Offset: 0x000AF616
	public virtual void DisplayImportDataResultMessage(Platform.ImportDataResult importDataResult, Action callback = null)
	{
		if (callback != null)
		{
			callback();
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000B1421 File Offset: 0x000AF621
	public virtual void CloseSystemDialogs(Action callback = null)
	{
		if (callback != null)
		{
			callback();
		}
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000B142C File Offset: 0x000AF62C
	public virtual void AdjustGameSettings(GameSettings gameSettings)
	{
	}

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x060027A6 RID: 10150 RVA: 0x000B142E File Offset: 0x000AF62E
	public virtual bool IsFiringAchievementsFromSavesAllowed
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000B1434 File Offset: 0x000AF634
	public virtual bool TryGetAchievementState(string achievementId, out AchievementState state)
	{
		bool? flag = this.IsAchievementUnlocked(achievementId);
		if (flag == null)
		{
			state = default(AchievementState);
			return false;
		}
		state = new AchievementState
		{
			isValid = true,
			isUnlocked = flag.Value
		};
		return true;
	}

	// Token: 0x060027A8 RID: 10152
	public abstract bool? IsAchievementUnlocked(string achievementId);

	// Token: 0x060027A9 RID: 10153
	public abstract void PushAchievementUnlock(string achievementId);

	// Token: 0x060027AA RID: 10154 RVA: 0x000B1481 File Offset: 0x000AF681
	public virtual void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	// Token: 0x060027AB RID: 10155
	public abstract void ResetAchievements();

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x060027AC RID: 10156
	public abstract bool AreAchievementsFetched { get; }

	// Token: 0x1400007F RID: 127
	// (add) Token: 0x060027AD RID: 10157 RVA: 0x000B1484 File Offset: 0x000AF684
	// (remove) Token: 0x060027AE RID: 10158 RVA: 0x000B14B8 File Offset: 0x000AF6B8
	public static event Platform.AchievementsFetchedDelegate AchievementsFetched;

	// Token: 0x060027AF RID: 10159 RVA: 0x000B14EB File Offset: 0x000AF6EB
	protected void OnAchievementsFetched()
	{
		if (Platform.AchievementsFetched != null)
		{
			Platform.AchievementsFetched();
		}
	}

	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x060027B0 RID: 10160 RVA: 0x000B14FE File Offset: 0x000AF6FE
	public virtual bool HasNativeAchievementsDialog
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000B1501 File Offset: 0x000AF701
	public virtual void ShowNativeAchievementsDialog()
	{
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000B1503 File Offset: 0x000AF703
	public virtual void SetSocialPresence(string socialStatusKey, bool isActive)
	{
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000B1505 File Offset: 0x000AF705
	public virtual void AddSocialStat(string name, int amount)
	{
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x000B1507 File Offset: 0x000AF707
	public virtual void FlushSocialEvents()
	{
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000B1509 File Offset: 0x000AF709
	public virtual void UpdateLocation(string location)
	{
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000B150B File Offset: 0x000AF70B
	public virtual void UpdatePlayTime(float playTime)
	{
	}

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x060027B7 RID: 10167 RVA: 0x000B150D File Offset: 0x000AF70D
	public virtual bool WillManageResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x060027B8 RID: 10168 RVA: 0x000B1510 File Offset: 0x000AF710
	public virtual bool WillDisplayGraphicsSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x060027B9 RID: 10169 RVA: 0x000B1513 File Offset: 0x000AF713
	public virtual bool LimitedGraphicsSettings
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x060027BA RID: 10170 RVA: 0x000B1518 File Offset: 0x000AF718
	// (set) Token: 0x060027BB RID: 10171 RVA: 0x000B1543 File Offset: 0x000AF743
	public Platform.ResolutionModes ResolutionMode
	{
		get
		{
			Platform.ResolutionModes? resolutionModes = this.resolutionModeOverride;
			if (resolutionModes == null)
			{
				return this.resolutionMode;
			}
			return resolutionModes.GetValueOrDefault();
		}
		set
		{
			this.resolutionMode = value;
			this.ChangeGraphicsTier(this.GraphicsTier, true);
		}
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x000B155C File Offset: 0x000AF75C
	public void DropResolutionModeInScene(Platform.ResolutionModes newMode)
	{
		if (newMode != Platform.ResolutionModes.Scale)
		{
			if (newMode == Platform.ResolutionModes.NativeHUDScaledMain)
			{
				if (this.ResolutionMode == Platform.ResolutionModes.Native)
				{
					this.resolutionModeOverride = new Platform.ResolutionModes?(newMode);
				}
			}
		}
		else
		{
			Platform.ResolutionModes resolutionModes = this.ResolutionMode;
			if (resolutionModes == Platform.ResolutionModes.Native || resolutionModes == Platform.ResolutionModes.NativeHUDScaledMain)
			{
				this.resolutionModeOverride = new Platform.ResolutionModes?(newMode);
			}
		}
		this.ChangeGraphicsTier(this.GraphicsTier, true);
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000B15B0 File Offset: 0x000AF7B0
	public virtual void AdjustGraphicsSettings(GameSettings gameSettings)
	{
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x060027BE RID: 10174 RVA: 0x000B15B2 File Offset: 0x000AF7B2
	protected virtual Platform.GraphicsTiers InitialGraphicsTier
	{
		get
		{
			return Platform.GraphicsTiers.High;
		}
	}

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x060027BF RID: 10175 RVA: 0x000B15B5 File Offset: 0x000AF7B5
	public Platform.GraphicsTiers GraphicsTier
	{
		get
		{
			return this.graphicsTier;
		}
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000B15BD File Offset: 0x000AF7BD
	protected void ChangeGraphicsTier(Platform.GraphicsTiers graphicsTier, bool isForced)
	{
		if (this.graphicsTier != graphicsTier || isForced)
		{
			this.graphicsTier = graphicsTier;
			this.RefreshGraphicsTier();
		}
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000B15DC File Offset: 0x000AF7DC
	public void RefreshGraphicsTier()
	{
		this.OnGraphicsTierChanged(this.graphicsTier);
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000B15EA File Offset: 0x000AF7EA
	protected virtual void OnGraphicsTierChanged(Platform.GraphicsTiers graphicsTier)
	{
		Shader.globalMaximumLOD = Platform.GetMaximumShaderLOD(graphicsTier);
		if (Platform.GraphicsTierChanged != null)
		{
			Platform.GraphicsTierChanged(graphicsTier);
		}
	}

	// Token: 0x14000080 RID: 128
	// (add) Token: 0x060027C3 RID: 10179 RVA: 0x000B160C File Offset: 0x000AF80C
	// (remove) Token: 0x060027C4 RID: 10180 RVA: 0x000B1640 File Offset: 0x000AF840
	public static event Platform.GraphicsTierChangedDelegate GraphicsTierChanged;

	// Token: 0x060027C5 RID: 10181 RVA: 0x000B1673 File Offset: 0x000AF873
	public static int GetMaximumShaderLOD(Platform.GraphicsTiers graphicsTier)
	{
		switch (graphicsTier)
		{
		case Platform.GraphicsTiers.VeryLow:
			return 700;
		case Platform.GraphicsTiers.Low:
			return 800;
		case Platform.GraphicsTiers.Medium:
			return 900;
		case Platform.GraphicsTiers.High:
			return 1000;
		default:
			return int.MaxValue;
		}
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x060027C6 RID: 10182 RVA: 0x000B16AA File Offset: 0x000AF8AA
	public virtual bool IsSpriteScalingApplied
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x060027C7 RID: 10183 RVA: 0x000B16AD File Offset: 0x000AF8AD
	public virtual float SpriteScalingFactor
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x060027C8 RID: 10184 RVA: 0x000B16B4 File Offset: 0x000AF8B4
	public virtual int DefaultHudSetting
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000B16B7 File Offset: 0x000AF8B7
	public virtual GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		return currentGamepadType;
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x060027CA RID: 10186 RVA: 0x000B16BA File Offset: 0x000AF8BA
	public virtual int ExtendedVideoBlankFrames
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x060027CB RID: 10187 RVA: 0x000B16BD File Offset: 0x000AF8BD
	public virtual int MaxVideoFrameRate
	{
		get
		{
			return 60;
		}
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000B16C4 File Offset: 0x000AF8C4
	public virtual void SetTargetFrameRate(int frameRate)
	{
		if (frameRate == 0)
		{
			Debug.LogError("Cannot set target frame rate to 0.");
			return;
		}
		Resolution currentResolution = Screen.currentResolution;
		this.RecordFrameRate(currentResolution);
		int num = Mathf.RoundToInt((float)currentResolution.refreshRateRatio.value);
		if (num % frameRate == 0)
		{
			QualitySettings.vSyncCount = num / frameRate;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
		Application.targetFrameRate = frameRate;
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000B171E File Offset: 0x000AF91E
	protected virtual bool RecordFrameRate(Resolution currentResolution)
	{
		if (this.canRestore)
		{
			return false;
		}
		this.originalResolution = currentResolution;
		this.vSyncCount = QualitySettings.vSyncCount;
		this.originalFrameRate = Application.targetFrameRate;
		this.originalFullScreenMode = Screen.fullScreenMode;
		this.canRestore = true;
		return true;
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000B175A File Offset: 0x000AF95A
	public virtual bool RestoreFrameRate()
	{
		if (!this.canRestore)
		{
			return false;
		}
		QualitySettings.vSyncCount = this.vSyncCount;
		Application.targetFrameRate = this.originalFrameRate;
		this.canRestore = false;
		return true;
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x060027CF RID: 10191 RVA: 0x000B1784 File Offset: 0x000AF984
	public virtual bool WillDisplayControllerSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x060027D0 RID: 10192 RVA: 0x000B1787 File Offset: 0x000AF987
	public virtual bool WillDisplayKeyboardSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x060027D1 RID: 10193 RVA: 0x000B178A File Offset: 0x000AF98A
	public virtual bool WillDisplayQuitButton
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x060027D2 RID: 10194 RVA: 0x000B178D File Offset: 0x000AF98D
	public virtual bool IsControllerImplicit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x060027D3 RID: 10195 RVA: 0x000B1790 File Offset: 0x000AF990
	public virtual bool IsMouseSupported
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x060027D4 RID: 10196 RVA: 0x000B1793 File Offset: 0x000AF993
	public virtual bool WillEverPauseOnControllerDisconnected
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x060027D5 RID: 10197 RVA: 0x000B1796 File Offset: 0x000AF996
	public virtual bool IsPausingOnControllerDisconnected
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x060027D6 RID: 10198 RVA: 0x000B1799 File Offset: 0x000AF999
	public virtual bool SupportsVibrationFromAudio
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x060027D7 RID: 10199
	public abstract Platform.AcceptRejectInputStyles AcceptRejectInputStyle { get; }

	// Token: 0x14000081 RID: 129
	// (add) Token: 0x060027D8 RID: 10200 RVA: 0x000B179C File Offset: 0x000AF99C
	// (remove) Token: 0x060027D9 RID: 10201 RVA: 0x000B17D0 File Offset: 0x000AF9D0
	public static event Platform.AcceptRejectInputStyleChangedDelegate AcceptRejectInputStyleChanged;

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x060027DA RID: 10202 RVA: 0x000B1804 File Offset: 0x000AFA04
	public bool WasLastInputKeyboard
	{
		get
		{
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			return instance.lastActiveController == BindingSourceType.KeyBindingSource || instance.lastActiveController == BindingSourceType.MouseBindingSource;
		}
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000B182C File Offset: 0x000AFA2C
	public Platform.MenuActions GetMenuAction(HeroActions ia, bool ignoreAttack = false, bool isContinuous = false)
	{
		return this.GetMenuAction(this.GetPressedState(ia.MenuSubmit, isContinuous), this.GetPressedState(ia.MenuCancel, isContinuous), this.GetPressedState(ia.Jump, isContinuous), !ignoreAttack && this.GetPressedState(ia.Attack, isContinuous), this.GetPressedState(ia.Cast, isContinuous), this.GetPressedState(ia.MenuExtra, isContinuous), ia.MenuSuper.WasPressed, ia.Dash.WasPressed, ia.DreamNail.WasPressed);
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000B18B4 File Offset: 0x000AFAB4
	private bool GetPressedState(PlayerAction action, bool isContinuous)
	{
		if (!isContinuous)
		{
			return action.WasPressed;
		}
		return action.IsPressed;
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000B18C8 File Offset: 0x000AFAC8
	public Platform.MenuActions GetMenuAction(bool menuSubmitInput, bool menuCancelInput, bool jumpInput, bool attackInput, bool castInput, bool menuExtraInput = false, bool menuSuperInput = false, bool dashInput = false, bool dreamNailInput = false)
	{
		if (this.WasLastInputKeyboard)
		{
			if (menuSubmitInput || jumpInput)
			{
				return Platform.MenuActions.Submit;
			}
			if (menuCancelInput || attackInput || castInput)
			{
				return Platform.MenuActions.Cancel;
			}
			if (menuExtraInput || dashInput)
			{
				return Platform.MenuActions.Extra;
			}
			if (menuSuperInput || dreamNailInput)
			{
				return Platform.MenuActions.Super;
			}
		}
		else
		{
			if (menuSubmitInput)
			{
				return Platform.MenuActions.Submit;
			}
			if (menuCancelInput)
			{
				return Platform.MenuActions.Cancel;
			}
			if (menuExtraInput)
			{
				return Platform.MenuActions.Extra;
			}
			if (menuSuperInput)
			{
				return Platform.MenuActions.Super;
			}
		}
		return Platform.MenuActions.None;
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x060027DE RID: 10206 RVA: 0x000B1918 File Offset: 0x000AFB18
	public virtual bool FetchScenesBeforeFade
	{
		get
		{
			return !CheatManager.DisableAsyncSceneLoad;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x060027DF RID: 10207 RVA: 0x000B1922 File Offset: 0x000AFB22
	public virtual float MaximumLoadDurationForNonCriticalGarbageCollection
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x060027E0 RID: 10208 RVA: 0x000B1929 File Offset: 0x000AFB29
	public virtual int MaximumSceneTransitionsWithoutNonCriticalGarbageCollection
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x060027E1 RID: 10209 RVA: 0x000B192C File Offset: 0x000AFB2C
	protected virtual bool ChangesBackgroundLoadingPriority
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000B1930 File Offset: 0x000AFB30
	public virtual void SetSceneLoadState(bool isInProgress, bool isHighPriority = false)
	{
		if (isInProgress && isHighPriority)
		{
			CheatManager.BoostModeActive = true;
			if (this.ChangesBackgroundLoadingPriority)
			{
				this.SetBackgroundLoadingPriority(ThreadPriority.High);
			}
			GameCameras instance = GameCameras.instance;
			if (instance && instance.mainCamera)
			{
				instance.SetMainCameraActive(false);
				return;
			}
		}
		else
		{
			CheatManager.BoostModeActive = false;
			if (this.ChangesBackgroundLoadingPriority)
			{
				this.RestoreBackgroundLoadingPriority();
			}
			GameCameras instance2 = GameCameras.instance;
			if (instance2 && instance2.mainCamera)
			{
				instance2.SetMainCameraActive(true);
			}
		}
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000B19B0 File Offset: 0x000AFBB0
	public void SetBackgroundLoadingPriority(ThreadPriority threadPriority)
	{
		if (this.originalLoadingPriority == null)
		{
			this.originalLoadingPriority = new ThreadPriority?(Application.backgroundLoadingPriority);
		}
		Application.backgroundLoadingPriority = threadPriority;
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000B19D5 File Offset: 0x000AFBD5
	public void RestoreBackgroundLoadingPriority()
	{
		if (this.originalLoadingPriority != null)
		{
			Application.backgroundLoadingPriority = this.originalLoadingPriority.Value;
		}
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000B19F4 File Offset: 0x000AFBF4
	public virtual void SetGameManagerReady()
	{
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x000B19F6 File Offset: 0x000AFBF6
	public virtual void OnScreenFaded()
	{
		if (this.resolutionModeOverride == null)
		{
			return;
		}
		this.resolutionModeOverride = null;
		this.ChangeGraphicsTier(this.GraphicsTier, true);
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x060027E7 RID: 10215 RVA: 0x000B1A1F File Offset: 0x000AFC1F
	// (set) Token: 0x060027E8 RID: 10216 RVA: 0x000B1A26 File Offset: 0x000AFC26
	public static bool UseFieldInfoCache { get; set; } = true;

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x060027E9 RID: 10217 RVA: 0x000B1A2E File Offset: 0x000AFC2E
	public virtual Platform.EngagementRequirements EngagementRequirement
	{
		get
		{
			return Platform.EngagementRequirements.Invisible;
		}
	}

	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x060027EA RID: 10218 RVA: 0x000B1A31 File Offset: 0x000AFC31
	public virtual Platform.EngagementStates EngagementState
	{
		get
		{
			return Platform.EngagementStates.Engaged;
		}
	}

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x060027EB RID: 10219 RVA: 0x000B1A34 File Offset: 0x000AFC34
	public virtual bool IsSavingAllowedByEngagement
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000B1A37 File Offset: 0x000AFC37
	public virtual void ClearEngagement()
	{
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000B1A39 File Offset: 0x000AFC39
	public virtual void BeginEngagement()
	{
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000B1A3B File Offset: 0x000AFC3B
	public virtual void UpdateWaitingForEngagement()
	{
	}

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x060027EF RID: 10223 RVA: 0x000B1A3D File Offset: 0x000AFC3D
	public virtual bool CanReEngage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x060027F0 RID: 10224 RVA: 0x000B1A40 File Offset: 0x000AFC40
	public virtual string EngagedDisplayName
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x060027F1 RID: 10225 RVA: 0x000B1A43 File Offset: 0x000AFC43
	public virtual Texture2D EngagedDisplayImage
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x060027F2 RID: 10226 RVA: 0x000B1A46 File Offset: 0x000AFC46
	public Platform.IDisengageHandler DisengageHandler
	{
		get
		{
			return this.disengageHandler;
		}
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000B1A4E File Offset: 0x000AFC4E
	public virtual void SetDisengageHandler(Platform.IDisengageHandler disengageHandler)
	{
		this.disengageHandler = disengageHandler;
	}

	// Token: 0x14000082 RID: 130
	// (add) Token: 0x060027F4 RID: 10228 RVA: 0x000B1A58 File Offset: 0x000AFC58
	// (remove) Token: 0x060027F5 RID: 10229 RVA: 0x000B1A90 File Offset: 0x000AFC90
	public event Platform.OnEngagedDisplayInfoChanged EngagedDisplayInfoChanged;

	// Token: 0x060027F6 RID: 10230 RVA: 0x000B1AC5 File Offset: 0x000AFCC5
	public void NotifyEngagedDisplayInfoChanged()
	{
		if (this.EngagedDisplayInfoChanged != null)
		{
			this.EngagedDisplayInfoChanged();
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x060027F7 RID: 10231 RVA: 0x000B1ADA File Offset: 0x000AFCDA
	public virtual bool IsPlayerPrefsLoaded
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x060027F8 RID: 10232 RVA: 0x000B1ADD File Offset: 0x000AFCDD
	public virtual bool RequiresPreferencesSyncOnEngage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x060027F9 RID: 10233 RVA: 0x000B1AE0 File Offset: 0x000AFCE0
	public static Platform Current
	{
		get
		{
			return Platform.current;
		}
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000B1AE7 File Offset: 0x000AFCE7
	protected virtual void Awake()
	{
		Platform.current = this;
		this.ChangeGraphicsTier(this.InitialGraphicsTier, true);
		if (!this.IsMouseSupported)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000B1B10 File Offset: 0x000AFD10
	protected virtual void OnDestroy()
	{
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000B1B12 File Offset: 0x000AFD12
	protected virtual void Update()
	{
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000B1B14 File Offset: 0x000AFD14
	protected virtual void BecomeCurrent()
	{
		Platform.current = this;
		if (Platform.PlatformBecameCurrent != null)
		{
			Platform.PlatformBecameCurrent();
		}
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000B1B2D File Offset: 0x000AFD2D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		Platform.CreatePlatform().BecomeCurrent();
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000B1B39 File Offset: 0x000AFD39
	private static Platform CreatePlatform()
	{
		return Platform.CreatePlatform<DesktopPlatform>();
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000B1B40 File Offset: 0x000AFD40
	private static Platform CreatePlatform<PlatformTy>() where PlatformTy : Platform
	{
		GameObject gameObject = new GameObject(typeof(PlatformTy).Name);
		PlatformTy platformTy = gameObject.AddComponent<PlatformTy>();
		Object.DontDestroyOnLoad(gameObject);
		return platformTy;
	}

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002801 RID: 10241 RVA: 0x000B1B73 File Offset: 0x000AFD73
	protected static bool IsPlatformSimulationEnabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04002436 RID: 9270
	protected Platform.ScreenModeState screenMode;

	// Token: 0x04002439 RID: 9273
	public const int SaveSlotCount = 5;

	// Token: 0x0400243A RID: 9274
	protected const string FirstSaveFileName = "user.dat";

	// Token: 0x0400243B RID: 9275
	protected const string NonFirstSaveFileNameFormat = "user{0}.dat";

	// Token: 0x0400243C RID: 9276
	protected const string BackupSuffix = ".bak";

	// Token: 0x0400243D RID: 9277
	protected const string BackupMarkedForDeletionSuffix = ".del";

	// Token: 0x04002440 RID: 9280
	private Platform.ResolutionModes resolutionMode;

	// Token: 0x04002441 RID: 9281
	private Platform.ResolutionModes? resolutionModeOverride;

	// Token: 0x04002442 RID: 9282
	private Platform.GraphicsTiers graphicsTier;

	// Token: 0x04002444 RID: 9284
	protected bool canRestore;

	// Token: 0x04002445 RID: 9285
	protected int vSyncCount;

	// Token: 0x04002446 RID: 9286
	protected int originalFrameRate;

	// Token: 0x04002447 RID: 9287
	protected Resolution originalResolution;

	// Token: 0x04002448 RID: 9288
	protected FullScreenMode originalFullScreenMode;

	// Token: 0x0400244A RID: 9290
	private ThreadPriority? originalLoadingPriority;

	// Token: 0x0400244C RID: 9292
	private Platform.IDisengageHandler disengageHandler;

	// Token: 0x0400244E RID: 9294
	private static Platform current;

	// Token: 0x0200175B RID: 5979
	public enum ResolutionModes
	{
		// Token: 0x04008DFA RID: 36346
		Native,
		// Token: 0x04008DFB RID: 36347
		Scale,
		// Token: 0x04008DFC RID: 36348
		NativeHUDScaledMain
	}

	// Token: 0x0200175C RID: 5980
	[Flags]
	public enum ScreenModeState
	{
		// Token: 0x04008DFE RID: 36350
		Standard = 0,
		// Token: 0x04008DFF RID: 36351
		HandHeld = 2,
		// Token: 0x04008E00 RID: 36352
		HandHeldSmall = 4,
		// Token: 0x04008E01 RID: 36353
		IncludeFutureFlags = -2147483648
	}

	// Token: 0x0200175D RID: 5981
	[Flags]
	public enum HandHeldTypes
	{
		// Token: 0x04008E03 RID: 36355
		None = 0,
		// Token: 0x04008E04 RID: 36356
		Switch = 1,
		// Token: 0x04008E05 RID: 36357
		SteamDeck = 4,
		// Token: 0x04008E06 RID: 36358
		IncludeFutureFlags = -2147483648
	}

	// Token: 0x0200175E RID: 5982
	// (Invoke) Token: 0x06008D7A RID: 36218
	public delegate void ScreenModeChanged(Platform.ScreenModeState screenMode);

	// Token: 0x0200175F RID: 5983
	// (Invoke) Token: 0x06008D7E RID: 36222
	public delegate void SaveStoreMountEvent(bool mounted);

	// Token: 0x02001760 RID: 5984
	protected enum SaveSlotFileNameUsage
	{
		// Token: 0x04008E08 RID: 36360
		Primary,
		// Token: 0x04008E09 RID: 36361
		Backup,
		// Token: 0x04008E0A RID: 36362
		BackupMarkedForDeletion
	}

	// Token: 0x02001761 RID: 5985
	public interface ISharedData
	{
		// Token: 0x06008D81 RID: 36225
		bool HasKey(string key);

		// Token: 0x06008D82 RID: 36226
		void DeleteKey(string key);

		// Token: 0x06008D83 RID: 36227
		void DeleteAll();

		// Token: 0x06008D84 RID: 36228
		void ImportData(Platform.ISharedData otherData);

		// Token: 0x06008D85 RID: 36229
		bool GetBool(string key, bool def);

		// Token: 0x06008D86 RID: 36230
		void SetBool(string key, bool val);

		// Token: 0x06008D87 RID: 36231
		int GetInt(string key, int def);

		// Token: 0x06008D88 RID: 36232
		void SetInt(string key, int val);

		// Token: 0x06008D89 RID: 36233
		float GetFloat(string key, float def);

		// Token: 0x06008D8A RID: 36234
		void SetFloat(string key, float val);

		// Token: 0x06008D8B RID: 36235
		string GetString(string key, string def);

		// Token: 0x06008D8C RID: 36236
		void SetString(string key, string val);

		// Token: 0x06008D8D RID: 36237
		void Save();
	}

	// Token: 0x02001762 RID: 5986
	public abstract class ImportDataInfo
	{
		// Token: 0x06008D8E RID: 36238
		public abstract void Save(int slot, Action<bool> callback = null);

		// Token: 0x04008E0B RID: 36363
		public int slot;

		// Token: 0x04008E0C RID: 36364
		public bool isSaveSlot;
	}

	// Token: 0x02001763 RID: 5987
	public struct ImportDataResult
	{
		// Token: 0x04008E0D RID: 36365
		public int importedCount;

		// Token: 0x04008E0E RID: 36366
		public int importTotal;

		// Token: 0x04008E0F RID: 36367
		public bool success;
	}

	// Token: 0x02001764 RID: 5988
	// (Invoke) Token: 0x06008D91 RID: 36241
	public delegate void AchievementsFetchedDelegate();

	// Token: 0x02001765 RID: 5989
	public enum GraphicsTiers
	{
		// Token: 0x04008E11 RID: 36369
		VeryLow,
		// Token: 0x04008E12 RID: 36370
		Low,
		// Token: 0x04008E13 RID: 36371
		Medium,
		// Token: 0x04008E14 RID: 36372
		High
	}

	// Token: 0x02001766 RID: 5990
	// (Invoke) Token: 0x06008D95 RID: 36245
	public delegate void GraphicsTierChangedDelegate(Platform.GraphicsTiers graphicsTier);

	// Token: 0x02001767 RID: 5991
	public enum AcceptRejectInputStyles
	{
		// Token: 0x04008E16 RID: 36374
		NonJapaneseStyle,
		// Token: 0x04008E17 RID: 36375
		JapaneseStyle
	}

	// Token: 0x02001768 RID: 5992
	// (Invoke) Token: 0x06008D99 RID: 36249
	public delegate void AcceptRejectInputStyleChangedDelegate();

	// Token: 0x02001769 RID: 5993
	public enum MenuActions
	{
		// Token: 0x04008E19 RID: 36377
		None,
		// Token: 0x04008E1A RID: 36378
		Submit,
		// Token: 0x04008E1B RID: 36379
		Cancel,
		// Token: 0x04008E1C RID: 36380
		Extra,
		// Token: 0x04008E1D RID: 36381
		Super
	}

	// Token: 0x0200176A RID: 5994
	public enum EngagementRequirements
	{
		// Token: 0x04008E1F RID: 36383
		Invisible,
		// Token: 0x04008E20 RID: 36384
		MustDisplay
	}

	// Token: 0x0200176B RID: 5995
	public enum EngagementStates
	{
		// Token: 0x04008E22 RID: 36386
		NotEngaged,
		// Token: 0x04008E23 RID: 36387
		EngagePending,
		// Token: 0x04008E24 RID: 36388
		Engaged
	}

	// Token: 0x0200176C RID: 5996
	public interface IDisengageHandler
	{
		// Token: 0x06008D9C RID: 36252
		void OnDisengage(Action next);
	}

	// Token: 0x0200176D RID: 5997
	// (Invoke) Token: 0x06008D9E RID: 36254
	public delegate void OnEngagedDisplayInfoChanged();
}
