using System;
using System.Collections.Generic;
using System.IO;
using TeamCherry.GameCore;
using UnityEngine;

// Token: 0x02000455 RID: 1109
public class GameCoreOnlineSubsystem : DesktopOnlineSubsystem
{
	// Token: 0x0600272E RID: 10030 RVA: 0x000B0494 File Offset: 0x000AE694
	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "XGamingRuntimeThunks.dll"));
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000B04AC File Offset: 0x000AE6AC
	public GameCoreOnlineSubsystem(DesktopPlatform platform)
	{
		this.platform = platform;
		this.achievementIdMap = Resources.Load<AchievementIDMap>("XB1AchievementMap");
		this.awardedAchievements = new HashSet<int>();
		GameCoreRuntimeManager.InitializeRuntime(false);
		this.gamecoreSaveRestoreHandler = new GameCoreSaveRestoreHandler();
		this.sharedData = new GameCoreSharedData("sharedData", "sharedData.dat");
		if (GameCoreRuntimeManager.SaveSystemInitialised)
		{
			this.MigrateLocalSaves();
		}
		GameCoreRuntimeManager.OnSaveSystemInitialised += this.MigrateLocalSaves;
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000B0528 File Offset: 0x000AE728
	~GameCoreOnlineSubsystem()
	{
		GameCoreRuntimeManager.OnSaveSystemInitialised -= this.MigrateLocalSaves;
	}

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06002731 RID: 10033 RVA: 0x000B0560 File Offset: 0x000AE760
	public override Platform.EngagementRequirements EngagementRequirement
	{
		get
		{
			return Platform.EngagementRequirements.MustDisplay;
		}
	}

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06002732 RID: 10034 RVA: 0x000B0563 File Offset: 0x000AE763
	public override Platform.EngagementStates EngagementState
	{
		get
		{
			if (GameCoreRuntimeManager.UserSignedIn)
			{
				return Platform.EngagementStates.Engaged;
			}
			if (GameCoreRuntimeManager.UserSignInPending)
			{
				return Platform.EngagementStates.EngagePending;
			}
			return Platform.EngagementStates.NotEngaged;
		}
	}

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x06002733 RID: 10035 RVA: 0x000B0578 File Offset: 0x000AE778
	public override string EngagedDisplayName
	{
		get
		{
			return GameCoreRuntimeManager.GamerTag;
		}
	}

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x06002734 RID: 10036 RVA: 0x000B057F File Offset: 0x000AE77F
	public override Texture2D EngagedDisplayImage
	{
		get
		{
			return GameCoreRuntimeManager.UserDisplayImage;
		}
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000B0586 File Offset: 0x000AE786
	public override void BeginEngagement()
	{
		GameCoreRuntimeManager.RequestUserSignIn(null);
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000B058E File Offset: 0x000AE78E
	public override void ClearEngagment()
	{
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x06002737 RID: 10039 RVA: 0x000B0590 File Offset: 0x000AE790
	public override bool AreAchievementsFetched
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x06002738 RID: 10040 RVA: 0x000B0593 File Offset: 0x000AE793
	public override bool HasNativeAchievementsDialog
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000B0598 File Offset: 0x000AE798
	public override void PushAchievementUnlock(string achievementId)
	{
		AchievementIDMap.AchievementIDPair achievementIDPair;
		if (this.achievementIdMap.TryGetAchievementInformation(achievementId, out achievementIDPair))
		{
			try
			{
				GameCoreRuntimeManager.UnlockAchievement(achievementIDPair.ServiceId);
				GameCoreRuntimeManager.FetchAchievements();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000B05E0 File Offset: 0x000AE7E0
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		return null;
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000B05F8 File Offset: 0x000AE7F8
	public override void UpdateAchievementProgress(string achievementId, int progressValue, int maxValue)
	{
		AchievementIDMap.AchievementIDPair achievementIDPair;
		if (this.achievementIdMap.TryGetAchievementInformation(achievementId, out achievementIDPair))
		{
			uint num = (uint)Mathf.Min(Mathf.RoundToInt((float)progressValue / (float)maxValue * 100f), 100);
			GameCoreRuntimeManager.UnlockAchievement(achievementIDPair.ServiceId, num);
			if ((ulong)num > (ulong)((long)maxValue))
			{
				GameCoreRuntimeManager.FetchAchievements();
			}
		}
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000B0644 File Offset: 0x000AE844
	public override void ResetAchievements()
	{
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000B0646 File Offset: 0x000AE846
	private string GetSaveContainerName(int slotIndex)
	{
		return GameCoreRuntimeManager.GetSaveSlotContainerName(slotIndex);
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000B064E File Offset: 0x000AE84E
	private string GetSaveFileName(int slotIndex)
	{
		return GameCoreRuntimeManager.GetMainSaveName(slotIndex);
	}

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x0600273F RID: 10047 RVA: 0x000B0656 File Offset: 0x000AE856
	public override bool HandlesGameSaves
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06002740 RID: 10048 RVA: 0x000B0659 File Offset: 0x000AE859
	public override bool HandlesRoamingSharedData
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x06002741 RID: 10049 RVA: 0x000B065C File Offset: 0x000AE85C
	public override Platform.ISharedData RoamingSharedData
	{
		get
		{
			return this.sharedData;
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06002742 RID: 10050 RVA: 0x000B0664 File Offset: 0x000AE864
	public override bool WillPreloadSaveFiles
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000B0668 File Offset: 0x000AE868
	public override void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
		if (callback == null)
		{
			return;
		}
		GameCoreRuntimeManager.FileExists(this.GetSaveContainerName(slotIndex), this.GetSaveFileName(slotIndex), delegate(bool exists)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(exists);
			});
		});
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000B06AC File Offset: 0x000AE8AC
	public override void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
		if (callback == null)
		{
			return;
		}
		GameCoreRuntimeManager.LoadSaveData(this.GetSaveContainerName(slotIndex), this.GetSaveFileName(slotIndex), delegate(byte[] success)
		{
			CoreLoop.InvokeSafe(delegate
			{
				callback(success);
			});
		});
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000B06F0 File Offset: 0x000AE8F0
	public override void WriteSaveSlot(int slotIndex, byte[] bytes, Action<bool> callback)
	{
		GameCoreRuntimeManager.Save(this.GetSaveContainerName(slotIndex), this.GetSaveFileName(slotIndex), bytes, delegate(bool success)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			}
		});
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000B072C File Offset: 0x000AE92C
	public override void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
		GameCoreRuntimeManager.DeleteContainer(this.GetSaveContainerName(slotIndex), delegate(bool success)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(success);
				});
			}
		});
	}

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06002747 RID: 10055 RVA: 0x000B075E File Offset: 0x000AE95E
	public override SaveRestoreHandler SaveRestoreHandler
	{
		get
		{
			return this.gamecoreSaveRestoreHandler;
		}
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000B0768 File Offset: 0x000AE968
	private void MigrateLocalSaves()
	{
		for (int i = 1; i <= 4; i++)
		{
			int slotIndex = i;
			Action<bool> <>9__3;
			this.platform.LocalReadSaveSlot(slotIndex, delegate(byte[] localBytes)
			{
				if (localBytes == null)
				{
					return;
				}
				this.platform.ReadSaveSlot(slotIndex, delegate(byte[] bytes)
				{
					if (bytes != null)
					{
						return;
					}
					Platform platform = this.platform;
					int slotIndex = slotIndex;
					byte[] localBytes = localBytes;
					Action<bool> callback;
					if ((callback = <>9__3) == null)
					{
						callback = (<>9__3 = delegate(bool success)
						{
							Debug.Log(string.Format("Migrated local slot {0} to cloud.", slotIndex));
						});
					}
					platform.WriteSaveSlot(slotIndex, localBytes, callback);
				});
			});
		}
		this.sharedData.LoadData(delegate
		{
			this.sharedData.ImportData(this.platform.LocalRoamingSharedData);
		});
	}

	// Token: 0x0400241D RID: 9245
	private const string SCID = "00000000-0000-0000-0000-0000636f5860";

	// Token: 0x0400241E RID: 9246
	private AchievementIDMap achievementIdMap;

	// Token: 0x0400241F RID: 9247
	private HashSet<int> awardedAchievements;

	// Token: 0x04002420 RID: 9248
	private DesktopPlatform platform;

	// Token: 0x04002421 RID: 9249
	private GameCoreSaveRestoreHandler gamecoreSaveRestoreHandler;

	// Token: 0x04002422 RID: 9250
	private GameCoreSharedData sharedData;
}
