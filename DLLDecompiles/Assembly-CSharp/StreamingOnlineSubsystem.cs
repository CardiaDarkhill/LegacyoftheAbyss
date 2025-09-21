using System;
using System.IO;

// Token: 0x0200046C RID: 1132
public class StreamingOnlineSubsystem : DesktopOnlineSubsystem
{
	// Token: 0x0600288C RID: 10380 RVA: 0x000B2CDC File Offset: 0x000B0EDC
	public static bool IsPackaged(DesktopPlatform desktopPlatform)
	{
		return desktopPlatform.IncludesPlugin(Path.Combine("x86_64", "IsStreamingBuild"));
	}

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x0600288D RID: 10381 RVA: 0x000B2CF3 File Offset: 0x000B0EF3
	public override bool AreAchievementsFetched
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x0600288E RID: 10382 RVA: 0x000B2CF6 File Offset: 0x000B0EF6
	public override bool LimitedGraphicsSettings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000B2CFC File Offset: 0x000B0EFC
	public override bool? IsAchievementUnlocked(string achievementId)
	{
		return null;
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000B2D12 File Offset: 0x000B0F12
	public override void PushAchievementUnlock(string achievementId)
	{
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000B2D14 File Offset: 0x000B0F14
	public override void UpdateAchievementProgress(string achievementId, int value, int max)
	{
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000B2D16 File Offset: 0x000B0F16
	public override void ResetAchievements()
	{
	}
}
