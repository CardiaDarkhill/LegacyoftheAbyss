using System;
using System.Collections;
using TeamCherry.GameCore;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public sealed class XBoxConsolePlatform : Platform, VibrationManager.IVibrationMixerProvider
{
	// Token: 0x060028C7 RID: 10439 RVA: 0x000B32D8 File Offset: 0x000B14D8
	protected override void Awake()
	{
		base.Awake();
		this.achievementIDMap = Resources.Load<AchievementIDMap>("XB1AchievementMap");
		this.localSharedData = new GameCoreSharedData("localSharedData", "localSharedData.dat");
		this.sharedData = new GameCoreSharedData("sharedData", "sharedData.dat");
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000B3325 File Offset: 0x000B1525
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.platformVibrationHelper.Destroy();
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000B3338 File Offset: 0x000B1538
	protected override void Update()
	{
		base.Update();
		this.platformVibrationHelper.UpdateVibration();
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000B334B File Offset: 0x000B154B
	private void OnPlayerPrefsInitialised()
	{
		this.isPlayerPrefsLoaded = true;
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000B3354 File Offset: 0x000B1554
	public override void LoadSharedDataAndNotify(bool mounted)
	{
		CoreLoop.InvokeSafe(delegate
		{
			if (this.loadingSharedDataRoutine != null)
			{
				this.StopCoroutine(this.loadingSharedDataRoutine);
			}
			if (mounted)
			{
				this.loadingSharedDataRoutine = this.StartCoroutine(this.LoadSharedAndNotifyRoutine(true));
				return;
			}
			this.hasMountedSharedData = false;
			this.NotifySaveMountStateChanged(false);
		});
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000B3379 File Offset: 0x000B1579
	private IEnumerator LoadSharedAndNotifyRoutine(bool mounted)
	{
		try
		{
			this.localSharedData.LoadData(null);
			this.sharedData.LoadData(null);
			while (!this.localSharedData.HasLoaded || !this.sharedData.HasLoaded)
			{
				yield return null;
			}
		}
		finally
		{
			this.hasMountedSharedData = true;
			this.NotifySaveMountStateChanged(mounted);
			this.loadingSharedDataRoutine = null;
		}
		yield break;
		yield break;
	}

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x060028CD RID: 10445 RVA: 0x000B3390 File Offset: 0x000B1590
	public override string DisplayName
	{
		get
		{
			return Application.platform.ToString();
		}
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000B33B0 File Offset: 0x000B15B0
	public override SystemLanguage GetSystemLanguage()
	{
		return base.GetSystemLanguage();
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x060028CF RID: 10447 RVA: 0x000B33B8 File Offset: 0x000B15B8
	public override bool ShowLanguageSelect
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x060028D0 RID: 10448 RVA: 0x000B33BB File Offset: 0x000B15BB
	public override bool IsFileSystemProtected
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x060028D1 RID: 10449 RVA: 0x000B33BE File Offset: 0x000B15BE
	public override bool WillPreloadSaveFiles
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x060028D2 RID: 10450 RVA: 0x000B33C1 File Offset: 0x000B15C1
	public override bool ShowSaveFileWriteIcon
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x060028D3 RID: 10451 RVA: 0x000B33C4 File Offset: 0x000B15C4
	public override bool IsFileWriteLimited
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x060028D4 RID: 10452 RVA: 0x000B33C7 File Offset: 0x000B15C7
	public override bool IsSaveStoreMounted
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x060028D5 RID: 10453 RVA: 0x000B33CA File Offset: 0x000B15CA
	public override bool IsSharedDataMounted
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000B33CD File Offset: 0x000B15CD
	public override void MountSaveStore()
	{
		base.MountSaveStore();
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000B33D5 File Offset: 0x000B15D5
	public override void PrepareForNewGame(int slotIndex)
	{
		base.PrepareForNewGame(slotIndex);
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000B33DE File Offset: 0x000B15DE
	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000B33EA File Offset: 0x000B15EA
	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		if (callback != null)
		{
			callback(null);
		}
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000B33F6 File Offset: 0x000B15F6
	public override void EnsureSaveSlotSpace(int slotIndex, Action<bool> callback)
	{
		if (callback != null)
		{
			callback(true);
		}
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000B3402 File Offset: 0x000B1602
	public override void WriteSaveSlot(int slotIndex, byte[] binary, Action<bool> callback)
	{
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000B340E File Offset: 0x000B160E
	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000B341A File Offset: 0x000B161A
	public override void OnSetGameData(int slotIndex)
	{
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x060028DE RID: 10462 RVA: 0x000B341C File Offset: 0x000B161C
	public override Platform.ISharedData LocalSharedData
	{
		get
		{
			return this.localSharedData;
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x060028DF RID: 10463 RVA: 0x000B3424 File Offset: 0x000B1624
	public override Platform.ISharedData RoamingSharedData
	{
		get
		{
			return this.sharedData;
		}
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000B342C File Offset: 0x000B162C
	public override void SaveScreenCapture(Texture2D texture2D, Action<bool> callback)
	{
		base.SaveScreenCapture(texture2D, callback);
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000B3436 File Offset: 0x000B1636
	public override void LoadScreenCaptures(Action<Texture2D[]> captures)
	{
		base.LoadScreenCaptures(captures);
	}

	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x060028E2 RID: 10466 RVA: 0x000B343F File Offset: 0x000B163F
	public override SaveRestoreHandler SaveRestoreHandler
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000B3442 File Offset: 0x000B1642
	public override void AdjustGameSettings(GameSettings gameSettings)
	{
		base.AdjustGameSettings(gameSettings);
	}

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x060028E4 RID: 10468 RVA: 0x000B344B File Offset: 0x000B164B
	public override bool IsFiringAchievementsFromSavesAllowed
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000B3450 File Offset: 0x000B1650
	public override bool TryGetAchievementState(string achievementId, out AchievementState state)
	{
		state = default(AchievementState);
		AchievementIDMap.AchievementIDPair achievementIDPair;
		this.achievementIDMap.TryGetAchievementInformation(achievementId, out achievementIDPair);
		state.isValid = false;
		return false;
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000B347C File Offset: 0x000B167C
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		AchievementState achievementState;
		if (this.TryGetAchievementState(achievementId, out achievementState) && achievementState.isValid)
		{
			return new bool?(achievementState.isUnlocked);
		}
		return null;
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000B34B4 File Offset: 0x000B16B4
	public override void PushAchievementUnlock(string achievementId)
	{
		AchievementIDMap.AchievementIDPair achievementIDPair;
		this.achievementIDMap.TryGetAchievementInformation(achievementId, out achievementIDPair);
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000B34D0 File Offset: 0x000B16D0
	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		AchievementIDMap.AchievementIDPair achievementIDPair;
		if (this.achievementIDMap.TryGetAchievementInformation(achievementId, out achievementIDPair))
		{
			Mathf.Min(Mathf.RoundToInt((float)value / (float)max * 100f), 100);
		}
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000B3505 File Offset: 0x000B1705
	public override void ResetAchievements()
	{
	}

	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x060028EA RID: 10474 RVA: 0x000B3507 File Offset: 0x000B1707
	public override bool AreAchievementsFetched
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x060028EB RID: 10475 RVA: 0x000B350A File Offset: 0x000B170A
	public override bool HasNativeAchievementsDialog
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x000B350D File Offset: 0x000B170D
	public override void ShowNativeAchievementsDialog()
	{
		base.ShowNativeAchievementsDialog();
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x000B3515 File Offset: 0x000B1715
	public override void SetSocialPresence(string socialStatusKey, bool isActive)
	{
		base.SetSocialPresence(socialStatusKey, isActive);
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000B351F File Offset: 0x000B171F
	public override void AddSocialStat(string name, int amount)
	{
		base.AddSocialStat(name, amount);
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x000B3529 File Offset: 0x000B1729
	public override void FlushSocialEvents()
	{
		base.FlushSocialEvents();
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000B3531 File Offset: 0x000B1731
	public override void UpdateLocation(string location)
	{
		base.UpdateLocation(location);
	}

	// Token: 0x060028F1 RID: 10481 RVA: 0x000B353A File Offset: 0x000B173A
	public override void UpdatePlayTime(float playTime)
	{
		base.UpdatePlayTime(playTime);
	}

	// Token: 0x170004AB RID: 1195
	// (get) Token: 0x060028F2 RID: 10482 RVA: 0x000B3543 File Offset: 0x000B1743
	public override bool WillManageResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004AC RID: 1196
	// (get) Token: 0x060028F3 RID: 10483 RVA: 0x000B3546 File Offset: 0x000B1746
	public override bool WillDisplayGraphicsSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x060028F4 RID: 10484 RVA: 0x000B3549 File Offset: 0x000B1749
	public override bool LimitedGraphicsSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x060028F5 RID: 10485 RVA: 0x000B354C File Offset: 0x000B174C
	public override bool IsSpriteScalingApplied
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x060028F6 RID: 10486 RVA: 0x000B354F File Offset: 0x000B174F
	public override int DefaultHudSetting
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x060028F7 RID: 10487 RVA: 0x000B3552 File Offset: 0x000B1752
	public override int ExtendedVideoBlankFrames
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne || Application.platform == RuntimePlatform.GameCoreXboxOne)
			{
				return 1;
			}
			return base.ExtendedVideoBlankFrames;
		}
	}

	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x060028F8 RID: 10488 RVA: 0x000B356E File Offset: 0x000B176E
	public override int MaxVideoFrameRate
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne || Application.platform == RuntimePlatform.GameCoreXboxOne)
			{
				return 30;
			}
			return base.MaxVideoFrameRate;
		}
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x060028F9 RID: 10489 RVA: 0x000B358B File Offset: 0x000B178B
	public override float EnterSceneWait
	{
		get
		{
			if (Application.platform == RuntimePlatform.XboxOne)
			{
				return 0.2f;
			}
			return base.EnterSceneWait;
		}
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000B35A4 File Offset: 0x000B17A4
	public override void SetTargetFrameRate(int frameRate)
	{
		if (Application.platform == RuntimePlatform.GameCoreXboxOne || Application.platform == RuntimePlatform.GameCoreXboxSeries)
		{
			if (!this.hasEverCalledResolution)
			{
				this.hasEverCalledResolution = true;
				Resolution[] resolutions = Screen.resolutions;
			}
			Resolution currentResolution = Screen.currentResolution;
			if (!this.canRestore)
			{
				this.originalResolution = currentResolution;
				this.vSyncCount = QualitySettings.vSyncCount;
				this.originalFrameRate = Application.targetFrameRate;
				this.originalFullScreenMode = Screen.fullScreenMode;
				this.canRestore = true;
			}
			FullScreenMode fullScreenMode = Screen.fullScreenMode;
			RefreshRate preferredRefreshRate = new RefreshRate
			{
				denominator = 1U,
				numerator = (uint)frameRate
			};
			Screen.SetResolution(currentResolution.width, currentResolution.height, fullScreenMode, preferredRefreshRate);
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = frameRate;
			return;
		}
		base.SetTargetFrameRate(frameRate);
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000B3664 File Offset: 0x000B1864
	public override bool RestoreFrameRate()
	{
		if (!this.canRestore)
		{
			return false;
		}
		Screen.SetResolution(this.originalResolution.width, this.originalResolution.height, this.originalFullScreenMode, this.originalResolution.refreshRateRatio);
		base.RestoreFrameRate();
		this.canRestore = false;
		return true;
	}

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x060028FC RID: 10492 RVA: 0x000B36B6 File Offset: 0x000B18B6
	public override bool WillDisplayControllerSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060028FD RID: 10493 RVA: 0x000B36B9 File Offset: 0x000B18B9
	public override bool WillDisplayKeyboardSettings
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x060028FE RID: 10494 RVA: 0x000B36BC File Offset: 0x000B18BC
	public override bool WillDisplayQuitButton
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x060028FF RID: 10495 RVA: 0x000B36BF File Offset: 0x000B18BF
	public override bool IsControllerImplicit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x06002900 RID: 10496 RVA: 0x000B36C2 File Offset: 0x000B18C2
	public override bool IsMouseSupported
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x06002901 RID: 10497 RVA: 0x000B36C5 File Offset: 0x000B18C5
	public override Platform.AcceptRejectInputStyles AcceptRejectInputStyle
	{
		get
		{
			return Platform.AcceptRejectInputStyles.NonJapaneseStyle;
		}
	}

	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x06002902 RID: 10498 RVA: 0x000B36C8 File Offset: 0x000B18C8
	public override Platform.EngagementRequirements EngagementRequirement
	{
		get
		{
			return Platform.EngagementRequirements.MustDisplay;
		}
	}

	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06002903 RID: 10499 RVA: 0x000B36CB File Offset: 0x000B18CB
	public override Platform.EngagementStates EngagementState
	{
		get
		{
			return Platform.EngagementStates.Engaged;
		}
	}

	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x06002904 RID: 10500 RVA: 0x000B36CE File Offset: 0x000B18CE
	public override bool IsSavingAllowedByEngagement
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000B36D1 File Offset: 0x000B18D1
	public override void ClearEngagement()
	{
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000B36D3 File Offset: 0x000B18D3
	public override void BeginEngagement()
	{
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x000B36D5 File Offset: 0x000B18D5
	public override void UpdateWaitingForEngagement()
	{
	}

	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x06002908 RID: 10504 RVA: 0x000B36D7 File Offset: 0x000B18D7
	public override bool CanReEngage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x06002909 RID: 10505 RVA: 0x000B36DA File Offset: 0x000B18DA
	public override string EngagedDisplayName
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x0600290A RID: 10506 RVA: 0x000B36E1 File Offset: 0x000B18E1
	public override Texture2D EngagedDisplayImage
	{
		get
		{
			return null;
		}
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000B36E4 File Offset: 0x000B18E4
	public override void SetDisengageHandler(Platform.IDisengageHandler disengageHandler)
	{
		base.SetDisengageHandler(disengageHandler);
	}

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x0600290C RID: 10508 RVA: 0x000B36ED File Offset: 0x000B18ED
	public override bool IsPlayerPrefsLoaded
	{
		get
		{
			return this.isPlayerPrefsLoaded;
		}
	}

	// Token: 0x170004C0 RID: 1216
	// (get) Token: 0x0600290D RID: 10509 RVA: 0x000B36F5 File Offset: 0x000B18F5
	public override bool RequiresPreferencesSyncOnEngage
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x000B36F8 File Offset: 0x000B18F8
	protected override void BecomeCurrent()
	{
		base.BecomeCurrent();
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x000B3700 File Offset: 0x000B1900
	public VibrationMixer GetVibrationMixer()
	{
		return this.platformVibrationHelper.GetMixer();
	}

	// Token: 0x040024C4 RID: 9412
	private GameCoreSharedData localSharedData;

	// Token: 0x040024C5 RID: 9413
	private GameCoreSharedData sharedData;

	// Token: 0x040024C6 RID: 9414
	private AchievementIDMap achievementIDMap;

	// Token: 0x040024C7 RID: 9415
	private bool isPlayerPrefsLoaded;

	// Token: 0x040024C8 RID: 9416
	private PlatformVibrationHelper platformVibrationHelper = new PlatformVibrationHelper();

	// Token: 0x040024C9 RID: 9417
	private Coroutine loadingSharedDataRoutine;

	// Token: 0x040024CA RID: 9418
	private volatile bool hasMountedSharedData;

	// Token: 0x040024CB RID: 9419
	private bool hasEverCalledResolution;
}
