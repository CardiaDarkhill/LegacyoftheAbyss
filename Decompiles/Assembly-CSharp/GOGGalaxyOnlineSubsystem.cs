using System;
using System.IO;
using Galaxy.Api;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class GOGGalaxyOnlineSubsystem : DesktopOnlineSubsystem
{
	// Token: 0x0600275B RID: 10075 RVA: 0x000B0AA8 File Offset: 0x000AECA8
	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "GalaxyCSharpGlue.dll"));
	}

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x0600275C RID: 10076 RVA: 0x000B0ABF File Offset: 0x000AECBF
	public bool DidInitialize
	{
		get
		{
			return this.didInitialize;
		}
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000B0AC8 File Offset: 0x000AECC8
	public GOGGalaxyOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		try
		{
			GalaxyInstance.Init(new InitParams("58684736355579041", "6df4eee80bbe552d39094e3ee31e457ff109405df303f1cebf263f964c1a8a1e"));
			this.didInitialize = true;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		if (this.didInitialize)
		{
			IListenerRegistrar listenerRegistrar = GalaxyInstance.ListenerRegistrar();
			this.authorization = new GOGGalaxyOnlineSubsystem.Authorization(this);
			listenerRegistrar.Register(GalaxyTypeAwareListenerAuth.GetListenerType(), this.authorization);
			this.statistics = new GOGGalaxyOnlineSubsystem.Statistics(this);
			listenerRegistrar.Register(GalaxyTypeAwareListenerUserStatsAndAchievementsRetrieve.GetListenerType(), this.statistics);
			this.authorization.SignIn();
			return;
		}
		Debug.LogErrorFormat("GOG failed to initialize", Array.Empty<object>());
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000B0B78 File Offset: 0x000AED78
	public override void Dispose()
	{
		if (this.statistics != null)
		{
			this.statistics.Dispose();
			this.statistics = null;
		}
		if (this.authorization != null)
		{
			this.authorization.Dispose();
			this.authorization = null;
		}
		if (this.didInitialize)
		{
			GalaxyInstance.Shutdown(true);
			this.didInitialize = false;
		}
		base.Dispose();
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000B0BD4 File Offset: 0x000AEDD4
	public override void Update()
	{
		base.Update();
		if (this.didInitialize)
		{
			GalaxyInstance.ProcessData();
		}
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000B0BE9 File Offset: 0x000AEDE9
	private void OnAuthorized()
	{
		this.statistics.Request();
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000B0BF6 File Offset: 0x000AEDF6
	private void OnAuthFailed()
	{
		this.platform.OnOnlineSubsystemAchievementsFailed();
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x06002762 RID: 10082 RVA: 0x000B0C03 File Offset: 0x000AEE03
	public override bool AreAchievementsFetched
	{
		get
		{
			return this.statistics != null && this.statistics.DidReceive;
		}
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000B0C1A File Offset: 0x000AEE1A
	private void OnStatisticsReceived()
	{
		this.platform.OnOnlineSubsystemAchievementsFetched();
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000B0C28 File Offset: 0x000AEE28
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		bool? result;
		if (!this.authorization.IsAuthorized || !this.statistics.DidReceive)
		{
			Debug.LogErrorFormat("Unable to get achievement {0}, because GOG is not authenticated.", new object[]
			{
				achievementId
			});
			result = null;
			return result;
		}
		bool value = false;
		uint num = 0U;
		try
		{
			GalaxyInstance.Stats().GetAchievement(achievementId, ref value, ref num);
			result = new bool?(value);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = null;
		}
		return result;
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000B0CAC File Offset: 0x000AEEAC
	public override void PushAchievementUnlock(string achievementId)
	{
		if (this.authorization.IsAuthorized)
		{
			try
			{
				GalaxyInstance.Stats().SetAchievement(achievementId);
				GalaxyInstance.Stats().StoreStatsAndAchievements();
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
		}
		Debug.LogErrorFormat("Unable to push achievement {0}, because GOG is not authenticated.", new object[]
		{
			achievementId
		});
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000B0D08 File Offset: 0x000AEF08
	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000B0D0C File Offset: 0x000AEF0C
	public override void ResetAchievements()
	{
		if (this.authorization.IsAuthorized)
		{
			try
			{
				GalaxyInstance.Stats().ResetStatsAndAchievements();
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
		}
		Debug.LogErrorFormat("Unable to reset achievements, because GOG is not authenticated.", Array.Empty<object>());
	}

	// Token: 0x04002429 RID: 9257
	private const string ClientId = "58684736355579041";

	// Token: 0x0400242A RID: 9258
	private const string ClientSecret = "6df4eee80bbe552d39094e3ee31e457ff109405df303f1cebf263f964c1a8a1e";

	// Token: 0x0400242B RID: 9259
	private DesktopPlatform platform;

	// Token: 0x0400242C RID: 9260
	private bool didInitialize;

	// Token: 0x0400242D RID: 9261
	private GOGGalaxyOnlineSubsystem.Authorization authorization;

	// Token: 0x0400242E RID: 9262
	private GOGGalaxyOnlineSubsystem.Statistics statistics;

	// Token: 0x02001759 RID: 5977
	private class Authorization : GlobalAuthListener
	{
		// Token: 0x17000F15 RID: 3861
		// (get) Token: 0x06008D6E RID: 36206 RVA: 0x00289B6F File Offset: 0x00287D6F
		public bool IsAuthorized
		{
			get
			{
				return this.isAuthorized;
			}
		}

		// Token: 0x06008D6F RID: 36207 RVA: 0x00289B77 File Offset: 0x00287D77
		public Authorization(GOGGalaxyOnlineSubsystem subsystem)
		{
			this.subsystem = subsystem;
			this.isAuthorized = false;
		}

		// Token: 0x06008D70 RID: 36208 RVA: 0x00289B8D File Offset: 0x00287D8D
		public override void OnAuthSuccess()
		{
			this.isAuthorized = true;
			Debug.LogFormat("GOG authorized", Array.Empty<object>());
			this.subsystem.OnAuthorized();
		}

		// Token: 0x06008D71 RID: 36209 RVA: 0x00289BB0 File Offset: 0x00287DB0
		public override void OnAuthFailure(IAuthListener.FailureReason failureReason)
		{
			this.isAuthorized = false;
			Debug.LogErrorFormat("GOG authorization failed: {0}", new object[]
			{
				failureReason
			});
			this.subsystem.OnAuthFailed();
		}

		// Token: 0x06008D72 RID: 36210 RVA: 0x00289BDD File Offset: 0x00287DDD
		public override void OnAuthLost()
		{
			this.isAuthorized = false;
			Debug.LogErrorFormat("GOG authorization lost", Array.Empty<object>());
			this.subsystem.OnAuthFailed();
		}

		// Token: 0x06008D73 RID: 36211 RVA: 0x00289C00 File Offset: 0x00287E00
		public void SignIn()
		{
			try
			{
				Debug.LogFormat("GOG signing in...", Array.Empty<object>());
				this.user = GalaxyInstance.User();
				this.user.SignInGalaxy();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		// Token: 0x04008DF3 RID: 36339
		private readonly GOGGalaxyOnlineSubsystem subsystem;

		// Token: 0x04008DF4 RID: 36340
		private IUser user;

		// Token: 0x04008DF5 RID: 36341
		private bool isAuthorized;
	}

	// Token: 0x0200175A RID: 5978
	private class Statistics : GlobalUserStatsAndAchievementsRetrieveListener
	{
		// Token: 0x17000F16 RID: 3862
		// (get) Token: 0x06008D74 RID: 36212 RVA: 0x00289C4C File Offset: 0x00287E4C
		public bool DidReceive
		{
			get
			{
				return this.didReceive;
			}
		}

		// Token: 0x06008D75 RID: 36213 RVA: 0x00289C54 File Offset: 0x00287E54
		public Statistics(GOGGalaxyOnlineSubsystem subsystem)
		{
			this.subsystem = subsystem;
			this.didReceive = false;
		}

		// Token: 0x06008D76 RID: 36214 RVA: 0x00289C6A File Offset: 0x00287E6A
		public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
		{
			Debug.LogFormat("Retrieved stats", Array.Empty<object>());
			this.didReceive = true;
			this.subsystem.OnStatisticsReceived();
		}

		// Token: 0x06008D77 RID: 36215 RVA: 0x00289C8D File Offset: 0x00287E8D
		public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, IUserStatsAndAchievementsRetrieveListener.FailureReason failureReason)
		{
			Debug.LogErrorFormat("Failed to retrieve stats: {0}", new object[]
			{
				failureReason
			});
		}

		// Token: 0x06008D78 RID: 36216 RVA: 0x00289CA8 File Offset: 0x00287EA8
		public void Request()
		{
			Debug.LogFormat("GOG requesting user stats and achievements...", Array.Empty<object>());
			this.stats = GalaxyInstance.Stats();
			this.stats.RequestUserStatsAndAchievements();
		}

		// Token: 0x04008DF6 RID: 36342
		private readonly GOGGalaxyOnlineSubsystem subsystem;

		// Token: 0x04008DF7 RID: 36343
		private bool didReceive;

		// Token: 0x04008DF8 RID: 36344
		private IStats stats;
	}
}
