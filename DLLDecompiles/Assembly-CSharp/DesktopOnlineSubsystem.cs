using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public abstract class DesktopOnlineSubsystem : IDisposable
{
	// Token: 0x06002703 RID: 9987 RVA: 0x000AFFA1 File Offset: 0x000AE1A1
	public virtual void Dispose()
	{
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000AFFA3 File Offset: 0x000AE1A3
	public virtual void Update()
	{
	}

	// Token: 0x06002705 RID: 9989
	public abstract bool? IsAchievementUnlocked(string achievementId);

	// Token: 0x06002706 RID: 9990
	public abstract void PushAchievementUnlock(string achievementId);

	// Token: 0x06002707 RID: 9991
	public abstract void UpdateAchievementProgress(string achievementId, int value, int max);

	// Token: 0x06002708 RID: 9992
	public abstract void ResetAchievements();

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x06002709 RID: 9993
	public abstract bool AreAchievementsFetched { get; }

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x0600270A RID: 9994 RVA: 0x000AFFA5 File Offset: 0x000AE1A5
	public virtual bool HasNativeAchievementsDialog
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x0600270B RID: 9995 RVA: 0x000AFFA8 File Offset: 0x000AE1A8
	public virtual bool LimitedGraphicsSettings
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000AFFAB File Offset: 0x000AE1AB
	public virtual GamepadType OverrideGamepadDisplay(GamepadType currentGamepadType)
	{
		return currentGamepadType;
	}

	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600270D RID: 9997 RVA: 0x000AFFAE File Offset: 0x000AE1AE
	public virtual int DefaultHudSetting
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x0600270E RID: 9998 RVA: 0x000AFFB1 File Offset: 0x000AE1B1
	public virtual string UserId
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x0600270F RID: 9999 RVA: 0x000AFFB4 File Offset: 0x000AE1B4
	public virtual bool HandlesGameSaves
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06002710 RID: 10000 RVA: 0x000AFFB7 File Offset: 0x000AE1B7
	public virtual bool HandlesRoamingSharedData
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06002711 RID: 10001 RVA: 0x000AFFBA File Offset: 0x000AE1BA
	public virtual Platform.ISharedData RoamingSharedData
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06002712 RID: 10002 RVA: 0x000AFFBD File Offset: 0x000AE1BD
	public virtual bool WillPreloadSaveFiles
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000AFFC0 File Offset: 0x000AE1C0
	public virtual void IsSaveSlotInUse(int slotIndex, Action<bool> callback)
	{
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000AFFC2 File Offset: 0x000AE1C2
	public virtual void ReadSaveSlot(int slotIndex, Action<byte[]> callback)
	{
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000AFFC4 File Offset: 0x000AE1C4
	public virtual void WriteSaveSlot(int slotIndex, byte[] bytes, Action<bool> callback)
	{
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000AFFC6 File Offset: 0x000AE1C6
	public virtual void ClearSaveSlot(int slotIndex, Action<bool> callback)
	{
	}

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x06002717 RID: 10007 RVA: 0x000AFFC8 File Offset: 0x000AE1C8
	public virtual SaveRestoreHandler SaveRestoreHandler { get; }

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x06002718 RID: 10008 RVA: 0x000AFFD0 File Offset: 0x000AE1D0
	public virtual Platform.EngagementRequirements EngagementRequirement
	{
		get
		{
			return Platform.EngagementRequirements.Invisible;
		}
	}

	// Token: 0x17000430 RID: 1072
	// (get) Token: 0x06002719 RID: 10009 RVA: 0x000AFFD3 File Offset: 0x000AE1D3
	public virtual Platform.EngagementStates EngagementState
	{
		get
		{
			return Platform.EngagementStates.Engaged;
		}
	}

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x0600271A RID: 10010 RVA: 0x000AFFD6 File Offset: 0x000AE1D6
	public virtual string EngagedDisplayName
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x0600271B RID: 10011 RVA: 0x000AFFD9 File Offset: 0x000AE1D9
	public virtual Texture2D EngagedDisplayImage
	{
		get
		{
			return null;
		}
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000AFFDC File Offset: 0x000AE1DC
	public virtual void BeginEngagement()
	{
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000AFFDE File Offset: 0x000AE1DE
	public virtual void ClearEngagment()
	{
	}

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x0600271E RID: 10014 RVA: 0x000AFFE0 File Offset: 0x000AE1E0
	public virtual bool HandleLoadSharedDataAndNotify
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000AFFE3 File Offset: 0x000AE1E3
	public virtual void LoadSharedDataAndNotify(bool mounted)
	{
	}
}
