using System;
using System.IO;
using System.Text;
using GlobalEnums;
using Steamworks;
using UnityEngine;

// Token: 0x0200046B RID: 1131
public class SteamOnlineSubsystem : DesktopOnlineSubsystem
{
	// Token: 0x06002879 RID: 10361 RVA: 0x000B27DC File Offset: 0x000B09DC
	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "steam_api64.dll"));
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000B27F4 File Offset: 0x000B09F4
	public SteamOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		if (!Packsize.Test())
		{
			Debug.LogErrorFormat("Steamworks packsize incorrect.", Array.Empty<object>());
		}
		if (!DllCheck.Test())
		{
			Debug.LogErrorFormat("Steamworks binaries out of date or missing.", Array.Empty<object>());
		}
		if (SteamAPI.RestartAppIfNecessary(new AppId_t(1030300U)))
		{
			Debug.LogError("Application was not launched through Steam! Shutting down...");
			Application.Quit();
		}
		Debug.LogFormat("Steam initializing", Array.Empty<object>());
		this.didInitialize = SteamAPI.Init();
		if (this.didInitialize)
		{
			this.warningCallback = new SteamAPIWarningMessageHook_t(this.OnSteamLogMessage);
			SteamClient.SetWarningMessageHook(this.warningCallback);
			this.gameOverlayCallback = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
			this.statsReceivedCallback = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnStatsReceived));
			this.steamShutdownCallback = Callback<SteamShutdown_t>.Create(new Callback<SteamShutdown_t>.DispatchDelegate(this.OnSteamShutdown));
			this.achievementStoredCallback = Callback<UserAchievementStored_t>.Create(new Callback<UserAchievementStored_t>.DispatchDelegate(this.OnAchievementStored));
			string personaName = SteamFriends.GetPersonaName();
			Debug.LogFormat("Steam logged in as {0}", new object[]
			{
				personaName
			});
			if (!SteamUserStats.RequestCurrentStats())
			{
				Debug.LogErrorFormat("Steam unable to request current stats.", Array.Empty<object>());
				return;
			}
		}
		else
		{
			Debug.LogErrorFormat("Steam failed to initialize", Array.Empty<object>());
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x0600287B RID: 10363 RVA: 0x000B2938 File Offset: 0x000B0B38
	public bool DidInitialize
	{
		get
		{
			return this.didInitialize;
		}
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000B2940 File Offset: 0x000B0B40
	public override void Dispose()
	{
		if (this.didInitialize)
		{
			Debug.LogFormat("Shutting down Steam API.", Array.Empty<object>());
			SteamAPI.Shutdown();
		}
		base.Dispose();
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000B2964 File Offset: 0x000B0B64
	public override void Update()
	{
		base.Update();
		if (this.didInitialize)
		{
			SteamAPI.RunCallbacks();
		}
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000B297C File Offset: 0x000B0B7C
	private void OnSteamLogMessage(int severity, StringBuilder content)
	{
		string format = "Steam: " + ((content != null) ? content.ToString() : null);
		if (severity == 1)
		{
			Debug.LogWarningFormat(format, Array.Empty<object>());
			return;
		}
		Debug.LogFormat(format, Array.Empty<object>());
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000B29BC File Offset: 0x000B0BBC
	private void OnGameOverlayActivated(GameOverlayActivated_t ev)
	{
		Debug.LogFormat("Steam overlay became {0}.", new object[]
		{
			(ev.m_bActive == 0) ? "closed" : "opened"
		});
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000B29E8 File Offset: 0x000B0BE8
	private void OnStatsReceived(UserStatsReceived_t ev)
	{
		if (ev.m_eResult == EResult.k_EResultOK)
		{
			this.statsReceived = true;
			Debug.LogFormat("Steam stats received.", Array.Empty<object>());
			this.platform.OnOnlineSubsystemAchievementsFetched();
			return;
		}
		Debug.LogErrorFormat("Steam failed to receive stats: {0}", new object[]
		{
			ev.m_eResult
		});
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06002881 RID: 10369 RVA: 0x000B2A3E File Offset: 0x000B0C3E
	public override int DefaultHudSetting
	{
		get
		{
			if (!this.didInitialize || !SteamUtils.IsSteamRunningOnSteamDeck())
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x06002882 RID: 10370 RVA: 0x000B2A52 File Offset: 0x000B0C52
	public bool IsRunningOnSteamDeck
	{
		get
		{
			return this.didInitialize && SteamUtils.IsSteamRunningOnSteamDeck();
		}
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000B2A63 File Offset: 0x000B0C63
	public override GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		if (this.didInitialize && SteamUtils.IsSteamRunningOnSteamDeck())
		{
			return GamepadType.STEAM_DECK;
		}
		return base.OverrideGamepadDisplay(currentGamepadType);
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x06002884 RID: 10372 RVA: 0x000B2A80 File Offset: 0x000B0C80
	public override string UserId
	{
		get
		{
			if (!this.didInitialize)
			{
				return null;
			}
			return SteamUser.GetSteamID().GetAccountID().ToString();
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06002885 RID: 10373 RVA: 0x000B2AB2 File Offset: 0x000B0CB2
	public override bool AreAchievementsFetched
	{
		get
		{
			return this.statsReceived;
		}
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000B2ABA File Offset: 0x000B0CBA
	private void OnSteamShutdown(SteamShutdown_t ev)
	{
		Debug.LogFormat("Steam shut down.", Array.Empty<object>());
		this.didInitialize = false;
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000B2AD2 File Offset: 0x000B0CD2
	private void OnAchievementStored(UserAchievementStored_t ev)
	{
		Debug.LogFormat("Steam achievement {0} ({1}/{2}) upload complete", new object[]
		{
			ev.m_rgchAchievementName,
			ev.m_nCurProgress,
			ev.m_nMaxProgress
		});
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000B2B0C File Offset: 0x000B0D0C
	public override void PushAchievementUnlock(string achievementId)
	{
		if (!this.didInitialize)
		{
			Debug.LogErrorFormat("Unable to unlock achievement {0}, because Steam is not initialized", new object[]
			{
				achievementId
			});
			return;
		}
		try
		{
			SteamUserStats.SetAchievement(achievementId);
			SteamUserStats.StoreStats();
			Debug.LogFormat("Pushing achievement {0}", new object[]
			{
				achievementId
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000B2B74 File Offset: 0x000B0D74
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		bool? result;
		if (!this.didInitialize)
		{
			Debug.LogErrorFormat("Unable to retrieve achievement state for {0}, because Steam is not initialized", new object[]
			{
				achievementId
			});
			result = null;
			return result;
		}
		try
		{
			bool value;
			if (SteamUserStats.GetAchievement(achievementId, out value))
			{
				result = new bool?(value);
			}
			else
			{
				Debug.LogErrorFormat("Failed to retrieve achievement state for {0}", new object[]
				{
					achievementId
				});
				result = null;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = null;
		}
		return result;
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000B2C00 File Offset: 0x000B0E00
	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
		if (!this.didInitialize)
		{
			Debug.LogErrorFormat("Unable to update achievement progress for {0}, because Steam is not initialized", new object[]
			{
				achievementId
			});
			return;
		}
		try
		{
			SteamUserStats.SetStat(achievementId + "_STAT", value);
			SteamUserStats.StoreStats();
			Debug.LogFormat("Pushing stat {0}_STAT value: {1}/{2}", new object[]
			{
				achievementId,
				value,
				max
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000B2C84 File Offset: 0x000B0E84
	public override void ResetAchievements()
	{
		if (!this.didInitialize)
		{
			Debug.LogErrorFormat("Unable to reset all stats, because Steam is not initialized", Array.Empty<object>());
			return;
		}
		try
		{
			SteamUserStats.ResetAllStats(true);
			Debug.LogFormat("Reset all stats", Array.Empty<object>());
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x040024A4 RID: 9380
	private readonly DesktopPlatform platform;

	// Token: 0x040024A5 RID: 9381
	private bool didInitialize;

	// Token: 0x040024A6 RID: 9382
	private bool statsReceived;

	// Token: 0x040024A7 RID: 9383
	private readonly SteamAPIWarningMessageHook_t warningCallback;

	// Token: 0x040024A8 RID: 9384
	private Callback<GameOverlayActivated_t> gameOverlayCallback;

	// Token: 0x040024A9 RID: 9385
	private Callback<UserStatsReceived_t> statsReceivedCallback;

	// Token: 0x040024AA RID: 9386
	private Callback<SteamShutdown_t> steamShutdownCallback;

	// Token: 0x040024AB RID: 9387
	private Callback<UserAchievementStored_t> achievementStoredCallback;
}
