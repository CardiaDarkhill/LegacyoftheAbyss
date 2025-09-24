using System;
using TeamCherry.Cinematics;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class StagTravel : FastTravelCutscene
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060005FD RID: 1533 RVA: 0x0001EE42 File Offset: 0x0001D042
	protected override bool IsReadyToActivate
	{
		get
		{
			return this.isReadyToActivate;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060005FE RID: 1534 RVA: 0x0001EE4C File Offset: 0x0001D04C
	protected override bool ShouldFlipX
	{
		get
		{
			GameManager instance = GameManager.instance;
			Vector2 directionBetweenScenes = instance.gameMap.GetDirectionBetweenScenes(instance.lastSceneName, instance.playerData.nextScene);
			return directionBetweenScenes.x != 0f && directionBetweenScenes.x < 0f;
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x0001EE98 File Offset: 0x0001D098
	private bool UseChildrenVersion()
	{
		return PlayerData.instance.UnlockedFastTravelTeleport;
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x0001EEA4 File Offset: 0x0001D0A4
	protected override CinematicVideoReference GetVideoReference()
	{
		if (!this.UseChildrenVersion())
		{
			return this.GetNormalVideo();
		}
		return this.GetChildrenVideo();
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0001EEBB File Offset: 0x0001D0BB
	protected override void OnSkipped()
	{
		this.isReadyToActivate = true;
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x0001EEC4 File Offset: 0x0001D0C4
	protected override void OnFadedOut()
	{
		this.isReadyToActivate = true;
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0001EECD File Offset: 0x0001D0CD
	protected override VibrationDataAsset GetVibrationData()
	{
		if (!this.UseChildrenVersion())
		{
			return this.normalVideoVibration;
		}
		return this.childVideoVibration;
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0001EEE4 File Offset: 0x0001D0E4
	private CinematicVideoReference GetChildrenVideo()
	{
		if (Platform.Current.MaxVideoFrameRate <= 30 && this.childrenVideo30FPS)
		{
			return this.childrenVideo30FPS;
		}
		return this.childrenVideo;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x0001EF0E File Offset: 0x0001D10E
	private CinematicVideoReference GetNormalVideo()
	{
		if (Platform.Current.MaxVideoFrameRate <= 30 && this.normalVideo30FPS)
		{
			return this.normalVideo30FPS;
		}
		return this.normalVideo;
	}

	// Token: 0x040005CB RID: 1483
	[Space]
	[SerializeField]
	private CinematicVideoReference normalVideo;

	// Token: 0x040005CC RID: 1484
	[SerializeField]
	private CinematicVideoReference childrenVideo;

	// Token: 0x040005CD RID: 1485
	[Space]
	[SerializeField]
	private CinematicVideoReference normalVideo30FPS;

	// Token: 0x040005CE RID: 1486
	[SerializeField]
	private CinematicVideoReference childrenVideo30FPS;

	// Token: 0x040005CF RID: 1487
	[Space]
	[SerializeField]
	private VibrationDataAsset normalVideoVibration;

	// Token: 0x040005D0 RID: 1488
	[SerializeField]
	private VibrationDataAsset childVideoVibration;

	// Token: 0x040005D1 RID: 1489
	private bool isReadyToActivate;
}
