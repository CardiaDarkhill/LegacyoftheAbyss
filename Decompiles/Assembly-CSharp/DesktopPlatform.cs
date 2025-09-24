using System;
using System.Collections.Generic;
using System.IO;
using GlobalEnums;
using InControl;
using UnityEngine;

// Token: 0x02000452 RID: 1106
public class DesktopPlatform : Platform, VibrationManager.IVibrationMixerProvider
{
	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x060026D1 RID: 9937 RVA: 0x000AF5FF File Offset: 0x000AD7FF
	public Platform.ISharedData LocalRoamingSharedData
	{
		get
		{
			return this.roamingSharedData;
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x060026D2 RID: 9938 RVA: 0x000AF608 File Offset: 0x000AD808
	public override bool IsRunningOnHandHeld
	{
		get
		{
			SteamOnlineSubsystem steamOnlineSubsystem = this.onlineSubsystem as SteamOnlineSubsystem;
			if (steamOnlineSubsystem != null)
			{
				return steamOnlineSubsystem.IsRunningOnSteamDeck;
			}
			return base.IsRunningOnHandHeld;
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x060026D3 RID: 9939 RVA: 0x000AF634 File Offset: 0x000AD834
	// (set) Token: 0x060026D4 RID: 9940 RVA: 0x000AF66C File Offset: 0x000AD86C
	public override Platform.ScreenModeState ScreenMode
	{
		get
		{
			SteamOnlineSubsystem steamOnlineSubsystem = this.onlineSubsystem as SteamOnlineSubsystem;
			if (steamOnlineSubsystem != null)
			{
				if (steamOnlineSubsystem.IsRunningOnSteamDeck)
				{
					base.SetScreenMode(Platform.ScreenModeState.HandHeld);
				}
				return base.ScreenMode;
			}
			return base.ScreenMode;
		}
		set
		{
			if (this.onlineSubsystem is SteamOnlineSubsystem)
			{
				return;
			}
			base.SetScreenMode(value);
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x060026D5 RID: 9941 RVA: 0x000AF684 File Offset: 0x000AD884
	public override Platform.HandHeldTypes HandHeldType
	{
		get
		{
			SteamOnlineSubsystem steamOnlineSubsystem = this.onlineSubsystem as SteamOnlineSubsystem;
			if (steamOnlineSubsystem != null && steamOnlineSubsystem.IsRunningOnSteamDeck)
			{
				return Platform.HandHeldTypes.SteamDeck;
			}
			return base.HandHeldType;
		}
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000AF6B0 File Offset: 0x000AD8B0
	protected override void Awake()
	{
		base.Awake();
		this.saveDirPath = Application.persistentDataPath;
		this.CreateOnlineSubsystem();
		string path = "default";
		if (this.onlineSubsystem != null)
		{
			string userId = this.onlineSubsystem.UserId;
			if (!string.IsNullOrEmpty(userId))
			{
				path = userId;
			}
		}
		this.saveDirPath = Path.Combine(this.saveDirPath, path);
		this.localSharedData = new PlayerPrefsSharedData(false);
		this.roamingSharedData = new JsonSharedData(this.saveDirPath, "shared.dat", true);
		this.desktopSaveRestoreHandler = new DesktopSaveRestoreHandler(this.saveDirPath);
		Platform.UseFieldInfoCache = false;
		if (this.onlineSubsystem == null)
		{
			this.OnOnlineSubsystemAchievementsFetched();
		}
		this.vibrationHelper = new PlatformVibrationHelper();
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x000AF760 File Offset: 0x000AD960
	private void CreateOnlineSubsystem()
	{
		List<DesktopPlatform.CreateOnlineSubsystemDelegate> list = new List<DesktopPlatform.CreateOnlineSubsystemDelegate>();
		if (SteamOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new SteamOnlineSubsystem(this));
		}
		if (GOGGalaxyOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new GOGGalaxyOnlineSubsystem(this));
		}
		if (GameCoreOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new GameCoreOnlineSubsystem(this));
		}
		if (StreamingOnlineSubsystem.IsPackaged(this))
		{
			list.Add(() => new StreamingOnlineSubsystem());
		}
		if (list.Count == 0)
		{
			Debug.LogFormat(this, "No online subsystems packaged.", Array.Empty<object>());
			return;
		}
		if (list.Count > 1)
		{
			Debug.LogErrorFormat(this, "Multiple online subsystems packaged.", Array.Empty<object>());
			Application.Quit();
			return;
		}
		this.onlineSubsystem = list[0]();
		Debug.LogFormat(this, "Selected online subsystem " + this.onlineSubsystem.GetType().Name, Array.Empty<object>());
		GOGGalaxyOnlineSubsystem goggalaxyOnlineSubsystem = this.onlineSubsystem as GOGGalaxyOnlineSubsystem;
		if (goggalaxyOnlineSubsystem != null && !goggalaxyOnlineSubsystem.DidInitialize)
		{
			this.onlineSubsystem = null;
			Debug.LogError("GOG was not initialised, will not be used as online subsystem.");
		}
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x000AF885 File Offset: 0x000ADA85
	protected override void OnDestroy()
	{
		if (this.onlineSubsystem != null)
		{
			this.onlineSubsystem.Dispose();
			this.onlineSubsystem = null;
		}
		this.vibrationHelper.Destroy();
		base.OnDestroy();
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000AF8B2 File Offset: 0x000ADAB2
	protected override void Update()
	{
		base.Update();
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null)
		{
			desktopOnlineSubsystem.Update();
		}
		this.vibrationHelper.UpdateVibration();
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000AF8D6 File Offset: 0x000ADAD6
	public override void BeginEngagement()
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem == null)
		{
			return;
		}
		desktopOnlineSubsystem.BeginEngagement();
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000AF8E8 File Offset: 0x000ADAE8
	public override void ClearEngagement()
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem == null)
		{
			return;
		}
		desktopOnlineSubsystem.ClearEngagment();
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000AF8FC File Offset: 0x000ADAFC
	public override void LoadSharedDataAndNotify(bool mounted)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandleLoadSharedDataAndNotify)
		{
			this.onlineSubsystem.LoadSharedDataAndNotify(mounted);
			return;
		}
		base.LoadSharedDataAndNotify(mounted);
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x060026DD RID: 9949 RVA: 0x000AF92F File Offset: 0x000ADB2F
	public override string DisplayName
	{
		get
		{
			return "Desktop";
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x060026DE RID: 9950 RVA: 0x000AF936 File Offset: 0x000ADB36
	public override SaveRestoreHandler SaveRestoreHandler
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			return ((desktopOnlineSubsystem != null) ? desktopOnlineSubsystem.SaveRestoreHandler : null) ?? this.desktopSaveRestoreHandler;
		}
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000AF954 File Offset: 0x000ADB54
	private string GetSaveSlotPath(int slotIndex, Platform.SaveSlotFileNameUsage usage)
	{
		return Path.Combine(this.saveDirPath, base.GetSaveSlotFileName(slotIndex, usage));
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000AF969 File Offset: 0x000ADB69
	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			this.onlineSubsystem.IsSaveSlotInUse(slotIndex, callback);
			return;
		}
		this.LocalIsSaveSlotInUse(slotIndex, callback);
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000AF998 File Offset: 0x000ADB98
	public void LocalIsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		string saveSlotPath = this.GetSaveSlotPath(slotIndex, Platform.SaveSlotFileNameUsage.Primary);
		bool inUse;
		try
		{
			inUse = File.Exists(saveSlotPath);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			inUse = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			Action<bool> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(inUse);
		});
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000AF9F8 File Offset: 0x000ADBF8
	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			this.onlineSubsystem.ReadSaveSlot(slotIndex, callback);
			return;
		}
		this.LocalReadSaveSlot(slotIndex, callback);
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x000AFA24 File Offset: 0x000ADC24
	public void LocalReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		string saveSlotPath = this.GetSaveSlotPath(slotIndex, Platform.SaveSlotFileNameUsage.Primary);
		byte[] bytes;
		try
		{
			if (File.Exists(saveSlotPath))
			{
				bytes = File.ReadAllBytes(saveSlotPath);
			}
			else
			{
				bytes = null;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			bytes = null;
		}
		CoreLoop.InvokeSafe(delegate
		{
			Action<byte[]> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(bytes);
		});
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000AFA98 File Offset: 0x000ADC98
	public override void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback)
	{
		CoreLoop.InvokeNext(delegate
		{
			Action<bool> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(true);
		});
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000AFAB8 File Offset: 0x000ADCB8
	public override void WriteSaveSlot(int slotIndex, byte[] bytes, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			this.onlineSubsystem.WriteSaveSlot(slotIndex, bytes, callback);
			return;
		}
		string saveSlotPath = this.GetSaveSlotPath(slotIndex, Platform.SaveSlotFileNameUsage.Primary);
		string saveSlotPath2 = this.GetSaveSlotPath(slotIndex, Platform.SaveSlotFileNameUsage.Backup);
		string text = saveSlotPath + ".new";
		if (File.Exists(text))
		{
			Debug.LogWarning(string.Format("Temp file <b>{0}</b> was found and is likely corrupted. The file has been deleted.", text));
		}
		try
		{
			File.WriteAllBytes(text, bytes);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		bool successful;
		try
		{
			if (File.Exists(saveSlotPath))
			{
				File.Replace(text, saveSlotPath, saveSlotPath2 + this.GetBackupNumber(saveSlotPath2).ToString());
			}
			else
			{
				File.Move(text, saveSlotPath);
			}
			successful = true;
		}
		catch (Exception exception2)
		{
			Debug.LogException(exception2);
			successful = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			Action<bool> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(successful);
		});
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000AFBB4 File Offset: 0x000ADDB4
	private int GetBackupNumber(string backupPath)
	{
		int num = 0;
		int num2 = 3;
		string[] files = Directory.GetFiles(Path.GetDirectoryName(backupPath));
		List<string> list = new List<string>();
		foreach (string text in files)
		{
			if (text.StartsWith(backupPath))
			{
				list.Add(text);
			}
		}
		if (list.Count > 0)
		{
			int index = 0;
			int num3 = int.MaxValue;
			int num4 = 0;
			for (int j = list.Count - 1; j >= 0; j--)
			{
				string text2 = list[j].Replace(backupPath, "");
				if (text2 != "")
				{
					try
					{
						num = int.Parse(text2);
						if (num < num3)
						{
							num3 = num;
							index = j;
						}
						if (num > num4)
						{
							num4 = num;
						}
					}
					catch
					{
						Debug.LogWarning(string.Format("Backup file: {0} does not have a numerical extension, and will be ignored.", list[j]));
					}
				}
			}
			num = num4;
			if (list.Count >= num2)
			{
				File.Delete(list[index]);
			}
		}
		return num + 1;
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000AFCB8 File Offset: 0x000ADEB8
	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null && desktopOnlineSubsystem.HandlesGameSaves)
		{
			this.onlineSubsystem.ClearSaveSlot(slotIndex, callback);
			return;
		}
		string saveSlotPath = this.GetSaveSlotPath(slotIndex, Platform.SaveSlotFileNameUsage.Primary);
		bool successful;
		try
		{
			File.Delete(saveSlotPath);
			successful = true;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			successful = false;
		}
		CoreLoop.InvokeNext(delegate
		{
			Action<bool> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(successful);
		});
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x060026E8 RID: 9960 RVA: 0x000AFD40 File Offset: 0x000ADF40
	public override Platform.ISharedData LocalSharedData
	{
		get
		{
			return this.localSharedData;
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x060026E9 RID: 9961 RVA: 0x000AFD48 File Offset: 0x000ADF48
	public override Platform.ISharedData RoamingSharedData
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null || !desktopOnlineSubsystem.HandlesRoamingSharedData)
			{
				return this.roamingSharedData;
			}
			return this.onlineSubsystem.RoamingSharedData;
		}
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000AFD7C File Offset: 0x000ADF7C
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		return new bool?(((desktopOnlineSubsystem != null) ? desktopOnlineSubsystem.IsAchievementUnlocked(achievementId) : null) ?? this.RoamingSharedData.GetBool(achievementId, false));
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000AFDC9 File Offset: 0x000ADFC9
	public override void PushAchievementUnlock(string achievementId)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem != null)
		{
			desktopOnlineSubsystem.PushAchievementUnlock(achievementId);
		}
		this.RoamingSharedData.SetBool(achievementId, true);
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000AFDEA File Offset: 0x000ADFEA
	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem == null)
		{
			return;
		}
		desktopOnlineSubsystem.UpdateAchievementProgress(achievementId, value, max);
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000AFDFF File Offset: 0x000ADFFF
	public override void ResetAchievements()
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem == null)
		{
			return;
		}
		desktopOnlineSubsystem.ResetAchievements();
	}

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x060026EE RID: 9966 RVA: 0x000AFE11 File Offset: 0x000AE011
	public override bool AreAchievementsFetched
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			return desktopOnlineSubsystem == null || desktopOnlineSubsystem.AreAchievementsFetched;
		}
	}

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x060026EF RID: 9967 RVA: 0x000AFE24 File Offset: 0x000AE024
	public override bool HasNativeAchievementsDialog
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return base.HasNativeAchievementsDialog;
			}
			return desktopOnlineSubsystem.HasNativeAchievementsDialog;
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x060026F0 RID: 9968 RVA: 0x000AFE3C File Offset: 0x000AE03C
	public override Platform.AcceptRejectInputStyles AcceptRejectInputStyle
	{
		get
		{
			return Platform.AcceptRejectInputStyles.NonJapaneseStyle;
		}
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000AFE40 File Offset: 0x000AE040
	public bool IncludesPlugin(string pluginName)
	{
		string path = "Plugins";
		string path2 = Path.Combine(Path.Combine(Application.dataPath, path), pluginName);
		return File.Exists(path2) || Directory.Exists(path2);
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000AFE75 File Offset: 0x000AE075
	public void OnOnlineSubsystemAchievementsFetched()
	{
		base.OnAchievementsFetched();
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000AFE7D File Offset: 0x000AE07D
	public void OnOnlineSubsystemAchievementsFailed()
	{
		this.onlineSubsystem = null;
		base.OnAchievementsFetched();
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x060026F4 RID: 9972 RVA: 0x000AFE8C File Offset: 0x000AE08C
	public override bool ShowLanguageSelect
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x060026F5 RID: 9973 RVA: 0x000AFE8F File Offset: 0x000AE08F
	public override bool IsControllerImplicit
	{
		get
		{
			return ManagerSingleton<InputHandler>.Instance && ManagerSingleton<InputHandler>.Instance.lastActiveController == BindingSourceType.DeviceBindingSource;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x060026F6 RID: 9974 RVA: 0x000AFEAD File Offset: 0x000AE0AD
	public override bool WillPreloadSaveFiles
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return base.WillPreloadSaveFiles;
			}
			return desktopOnlineSubsystem.WillPreloadSaveFiles;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x060026F7 RID: 9975 RVA: 0x000AFEC5 File Offset: 0x000AE0C5
	public override Platform.EngagementRequirements EngagementRequirement
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return base.EngagementRequirement;
			}
			return desktopOnlineSubsystem.EngagementRequirement;
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x060026F8 RID: 9976 RVA: 0x000AFEDD File Offset: 0x000AE0DD
	public override Platform.EngagementStates EngagementState
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return base.EngagementState;
			}
			return desktopOnlineSubsystem.EngagementState;
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x060026F9 RID: 9977 RVA: 0x000AFEF5 File Offset: 0x000AE0F5
	public override string EngagedDisplayName
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			return ((desktopOnlineSubsystem != null) ? desktopOnlineSubsystem.EngagedDisplayName : null) ?? base.EngagedDisplayName;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x060026FA RID: 9978 RVA: 0x000AFF13 File Offset: 0x000AE113
	public override Texture2D EngagedDisplayImage
	{
		get
		{
			if (this.onlineSubsystem == null)
			{
				return base.EngagedDisplayImage;
			}
			return this.onlineSubsystem.EngagedDisplayImage;
		}
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000AFF2F File Offset: 0x000AE12F
	public VibrationMixer GetVibrationMixer()
	{
		return this.vibrationHelper.GetMixer();
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x060026FC RID: 9980 RVA: 0x000AFF3C File Offset: 0x000AE13C
	public override int DefaultHudSetting
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return 1;
			}
			return desktopOnlineSubsystem.DefaultHudSetting;
		}
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000AFF4F File Offset: 0x000AE14F
	public override GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
		if (desktopOnlineSubsystem == null)
		{
			return base.OverrideGamepadDisplay(currentGamepadType);
		}
		return desktopOnlineSubsystem.OverrideGamepadDisplay(currentGamepadType);
	}

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x060026FE RID: 9982 RVA: 0x000AFF69 File Offset: 0x000AE169
	public override bool LimitedGraphicsSettings
	{
		get
		{
			DesktopOnlineSubsystem desktopOnlineSubsystem = this.onlineSubsystem;
			if (desktopOnlineSubsystem == null)
			{
				return base.LimitedGraphicsSettings;
			}
			return desktopOnlineSubsystem.LimitedGraphicsSettings;
		}
	}

	// Token: 0x04002415 RID: 9237
	private string saveDirPath;

	// Token: 0x04002416 RID: 9238
	private Platform.ISharedData localSharedData;

	// Token: 0x04002417 RID: 9239
	private Platform.ISharedData roamingSharedData;

	// Token: 0x04002418 RID: 9240
	private DesktopOnlineSubsystem onlineSubsystem;

	// Token: 0x04002419 RID: 9241
	private PlatformVibrationHelper vibrationHelper;

	// Token: 0x0400241A RID: 9242
	private DesktopSaveRestoreHandler desktopSaveRestoreHandler;

	// Token: 0x0200173A RID: 5946
	// (Invoke) Token: 0x06008D1F RID: 36127
	private delegate DesktopOnlineSubsystem CreateOnlineSubsystemDelegate();
}
