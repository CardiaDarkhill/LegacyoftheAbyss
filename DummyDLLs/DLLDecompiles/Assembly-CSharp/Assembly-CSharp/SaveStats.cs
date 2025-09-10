using System;
using GlobalEnums;

// Token: 0x0200043D RID: 1085
[Serializable]
public class SaveStats
{
	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x0600255E RID: 9566 RVA: 0x000AB301 File Offset: 0x000A9501
	// (set) Token: 0x0600255F RID: 9567 RVA: 0x000AB309 File Offset: 0x000A9509
	public string Version { get; private set; }

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002560 RID: 9568 RVA: 0x000AB312 File Offset: 0x000A9512
	// (set) Token: 0x06002561 RID: 9569 RVA: 0x000AB31A File Offset: 0x000A951A
	public int RevisionBreak { get; private set; }

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002562 RID: 9570 RVA: 0x000AB323 File Offset: 0x000A9523
	// (set) Token: 0x06002563 RID: 9571 RVA: 0x000AB32B File Offset: 0x000A952B
	public int MaxHealth { get; private set; }

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x06002564 RID: 9572 RVA: 0x000AB334 File Offset: 0x000A9534
	// (set) Token: 0x06002565 RID: 9573 RVA: 0x000AB33C File Offset: 0x000A953C
	public int MaxSilk { get; private set; }

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002566 RID: 9574 RVA: 0x000AB345 File Offset: 0x000A9545
	// (set) Token: 0x06002567 RID: 9575 RVA: 0x000AB34D File Offset: 0x000A954D
	public bool IsSpoolBroken { get; private set; }

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x06002568 RID: 9576 RVA: 0x000AB356 File Offset: 0x000A9556
	// (set) Token: 0x06002569 RID: 9577 RVA: 0x000AB35E File Offset: 0x000A955E
	public int Geo { get; private set; }

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x0600256A RID: 9578 RVA: 0x000AB367 File Offset: 0x000A9567
	// (set) Token: 0x0600256B RID: 9579 RVA: 0x000AB36F File Offset: 0x000A956F
	public int Shards { get; private set; }

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x0600256C RID: 9580 RVA: 0x000AB378 File Offset: 0x000A9578
	// (set) Token: 0x0600256D RID: 9581 RVA: 0x000AB380 File Offset: 0x000A9580
	public MapZone MapZone { get; private set; }

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x0600256E RID: 9582 RVA: 0x000AB389 File Offset: 0x000A9589
	// (set) Token: 0x0600256F RID: 9583 RVA: 0x000AB391 File Offset: 0x000A9591
	public ExtraRestZones ExtraRestZone { get; private set; }

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x06002570 RID: 9584 RVA: 0x000AB39A File Offset: 0x000A959A
	// (set) Token: 0x06002571 RID: 9585 RVA: 0x000AB3A2 File Offset: 0x000A95A2
	public BellhomePaintColours BellhomePaintColour { get; private set; }

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x06002572 RID: 9586 RVA: 0x000AB3AB File Offset: 0x000A95AB
	// (set) Token: 0x06002573 RID: 9587 RVA: 0x000AB3B3 File Offset: 0x000A95B3
	public float PlayTime { get; private set; }

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06002574 RID: 9588 RVA: 0x000AB3BC File Offset: 0x000A95BC
	// (set) Token: 0x06002575 RID: 9589 RVA: 0x000AB3C4 File Offset: 0x000A95C4
	public PermadeathModes PermadeathMode { get; private set; }

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06002576 RID: 9590 RVA: 0x000AB3CD File Offset: 0x000A95CD
	// (set) Token: 0x06002577 RID: 9591 RVA: 0x000AB3D5 File Offset: 0x000A95D5
	public bool BossRushMode { get; private set; }

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x06002578 RID: 9592 RVA: 0x000AB3DE File Offset: 0x000A95DE
	// (set) Token: 0x06002579 RID: 9593 RVA: 0x000AB3E6 File Offset: 0x000A95E6
	public float CompletionPercentage { get; private set; }

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x0600257A RID: 9594 RVA: 0x000AB3EF File Offset: 0x000A95EF
	// (set) Token: 0x0600257B RID: 9595 RVA: 0x000AB3F7 File Offset: 0x000A95F7
	public bool UnlockedCompletionRate { get; private set; }

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x0600257C RID: 9596 RVA: 0x000AB400 File Offset: 0x000A9600
	// (set) Token: 0x0600257D RID: 9597 RVA: 0x000AB408 File Offset: 0x000A9608
	public bool IsBlackThreadInfected { get; private set; }

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x0600257E RID: 9598 RVA: 0x000AB411 File Offset: 0x000A9611
	// (set) Token: 0x0600257F RID: 9599 RVA: 0x000AB419 File Offset: 0x000A9619
	public bool HasClearedBlackThreads { get; set; }

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x06002580 RID: 9600 RVA: 0x000AB422 File Offset: 0x000A9622
	// (set) Token: 0x06002581 RID: 9601 RVA: 0x000AB42A File Offset: 0x000A962A
	public bool IsAct3 { get; private set; }

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x06002582 RID: 9602 RVA: 0x000AB433 File Offset: 0x000A9633
	// (set) Token: 0x06002583 RID: 9603 RVA: 0x000AB43B File Offset: 0x000A963B
	public bool IsAct3IntroCompleted { get; private set; }

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x06002584 RID: 9604 RVA: 0x000AB444 File Offset: 0x000A9644
	// (set) Token: 0x06002585 RID: 9605 RVA: 0x000AB44C File Offset: 0x000A964C
	public string CrestId { get; private set; }

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x06002586 RID: 9606 RVA: 0x000AB455 File Offset: 0x000A9655
	// (set) Token: 0x06002587 RID: 9607 RVA: 0x000AB45D File Offset: 0x000A965D
	public SaveSlotCompletionIcons.CompletionState CompletedEndings { get; private set; }

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x06002588 RID: 9608 RVA: 0x000AB466 File Offset: 0x000A9666
	// (set) Token: 0x06002589 RID: 9609 RVA: 0x000AB46E File Offset: 0x000A966E
	public SaveSlotCompletionIcons.CompletionState LastCompletedEnding { get; private set; }

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x0600258A RID: 9610 RVA: 0x000AB477 File Offset: 0x000A9677
	// (set) Token: 0x0600258B RID: 9611 RVA: 0x000AB47F File Offset: 0x000A967F
	public bool IsBlank { get; private set; }

	// Token: 0x0600258C RID: 9612 RVA: 0x000AB488 File Offset: 0x000A9688
	public SaveStats(PlayerData playerData, SaveGameData saveGameData)
	{
		this.Version = playerData.version;
		this.RevisionBreak = playerData.RevisionBreak;
		this.MaxHealth = playerData.maxHealthBase;
		this.IsSpoolBroken = playerData.IsSilkSpoolBroken;
		this.MaxSilk = playerData.CurrentSilkMaxBasic;
		this.Geo = playerData.geo;
		this.Shards = playerData.ShellShards;
		this.MapZone = playerData.mapZone;
		this.ExtraRestZone = playerData.extraRestZone;
		this.BellhomePaintColour = playerData.BelltownHouseColour;
		this.PlayTime = playerData.playTime;
		this.playTimeStruct.RawTime = playerData.playTime;
		this.PermadeathMode = playerData.permadeathMode;
		this.BossRushMode = playerData.bossRushMode;
		this.CompletionPercentage = playerData.completionPercentage;
		this.UnlockedCompletionRate = playerData.ConstructedFarsight;
		this.IsBlackThreadInfected = playerData.IsAct3IntroQueued;
		this.IsAct3 = playerData.blackThreadWorld;
		this.IsAct3IntroCompleted = (playerData.blackThreadWorld && playerData.act3_enclaveWakeSceneCompleted);
		this.CrestId = playerData.CurrentCrestID;
		this.saveGameData = saveGameData;
		this.CompletedEndings = playerData.CompletedEndings;
		this.LastCompletedEnding = playerData.LastCompletedEnding;
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000AB5BA File Offset: 0x000A97BA
	private SaveStats()
	{
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x0600258E RID: 9614 RVA: 0x000AB5C2 File Offset: 0x000A97C2
	public static SaveStats Blank
	{
		get
		{
			return new SaveStats
			{
				IsBlank = true
			};
		}
	}

	// Token: 0x0600258F RID: 9615 RVA: 0x000AB5D0 File Offset: 0x000A97D0
	public string GetPlaytimeHHMM()
	{
		if (this.playTimeStruct.HasHours)
		{
			return string.Format("{0:0}h {1:00}m", (int)this.playTimeStruct.Hours, (int)this.playTimeStruct.Minutes);
		}
		return string.Format("{0:0}m", (int)this.playTimeStruct.Minutes);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000AB634 File Offset: 0x000A9834
	public string GetPlaytimeHHMMSS()
	{
		if (!this.playTimeStruct.HasHours)
		{
			return string.Format("{0:0}m {1:00}s", (int)this.playTimeStruct.Minutes, (int)this.playTimeStruct.Seconds);
		}
		if (!this.playTimeStruct.HasMinutes)
		{
			return string.Format("{0:0}s", (int)this.playTimeStruct.Seconds);
		}
		return string.Format("{0:0}h {1:00}m {2:00}s", (int)this.playTimeStruct.Hours, (int)this.playTimeStruct.Minutes, (int)this.playTimeStruct.Seconds);
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000AB6E4 File Offset: 0x000A98E4
	public string GetCompletionPercentage()
	{
		return this.CompletionPercentage.ToString() + "%";
	}

	// Token: 0x04002326 RID: 8998
	private PlayTime playTimeStruct;

	// Token: 0x04002327 RID: 8999
	public readonly SaveGameData saveGameData;
}
