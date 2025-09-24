using System;
using TeamCherry.Localization;

// Token: 0x02000467 RID: 1127
[Serializable]
public sealed class RestorePointData
{
	// Token: 0x06002869 RID: 10345 RVA: 0x000B25AB File Offset: 0x000B07AB
	public RestorePointData(SaveGameData saveGameData, AutoSaveName autoSaveName)
	{
		this.autoSaveName = autoSaveName;
		this.saveGameData = saveGameData;
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000B25C1 File Offset: 0x000B07C1
	private RestorePointData(SaveGameData saveGameData)
	{
		this.saveGameData = saveGameData;
		this.version = saveGameData.playerData.version;
		this.date = saveGameData.playerData.date;
		this.isVersionBackup = true;
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000B25F9 File Offset: 0x000B07F9
	public static RestorePointData CreateVersionBackup(SaveGameData saveGameData)
	{
		if (saveGameData == null || saveGameData.playerData == null)
		{
			return null;
		}
		return new RestorePointData(saveGameData)
		{
			date = saveGameData.playerData.date,
			version = saveGameData.playerData.version
		};
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000B2630 File Offset: 0x000B0830
	public RestorePointData()
	{
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000B2638 File Offset: 0x000B0838
	public void SetDateString()
	{
		this.date = RestorePointData.GetDateString();
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000B2645 File Offset: 0x000B0845
	public void SetVersion()
	{
		this.version = "1.0.28324";
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000B2654 File Offset: 0x000B0854
	private static string GetDateString()
	{
		return DateTime.Now.ToString("yyyy/MM/dd");
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000B2673 File Offset: 0x000B0873
	public bool IsValid()
	{
		return this.saveGameData != null && this.saveGameData.playerData != null;
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000B2690 File Offset: 0x000B0890
	public string GetName()
	{
		if (!this.IsValid())
		{
			return RestorePointData.INCOMPATIBLE;
		}
		if (this.isVersionBackup)
		{
			return this.version;
		}
		if (this.autoSaveName != AutoSaveName.NONE)
		{
			return GameManager.GetFormattedAutoSaveNameString(this.autoSaveName);
		}
		return GameManager.GetFormattedMapZoneStringV2(this.saveGameData.playerData.mapZone);
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000B26E8 File Offset: 0x000B08E8
	public string GetDateTime()
	{
		if (!this.IsValid())
		{
			return this.date;
		}
		string str = this.date;
		string str2 = " - ";
		PlayTime playTime = default(PlayTime);
		playTime.RawTime = this.saveGameData.playerData.playTime;
		return str + str2 + playTime.ToString();
	}

	// Token: 0x0400246F RID: 9327
	public SaveGameData saveGameData;

	// Token: 0x04002470 RID: 9328
	public string date;

	// Token: 0x04002471 RID: 9329
	public string version;

	// Token: 0x04002472 RID: 9330
	public AutoSaveName autoSaveName;

	// Token: 0x04002473 RID: 9331
	private static readonly LocalisedString INCOMPATIBLE = new LocalisedString("MainMenu", "PROFILE_INCOMPATIBLE");

	// Token: 0x04002474 RID: 9332
	private bool isVersionBackup;
}
