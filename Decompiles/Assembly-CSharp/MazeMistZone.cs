using System;

// Token: 0x020007C2 RID: 1986
public class MazeMistZone : SceneTransitionZoneBase
{
	// Token: 0x170007E2 RID: 2018
	// (get) Token: 0x060045FA RID: 17914 RVA: 0x001306A1 File Offset: 0x0012E8A1
	protected override string TargetScene
	{
		get
		{
			return "Dust_Maze_09_entrance";
		}
	}

	// Token: 0x170007E3 RID: 2019
	// (get) Token: 0x060045FB RID: 17915 RVA: 0x001306A8 File Offset: 0x0012E8A8
	protected override string TargetGate
	{
		get
		{
			return "Death Respawn Marker";
		}
	}

	// Token: 0x060045FC RID: 17916 RVA: 0x001306AF File Offset: 0x0012E8AF
	protected override void OnPreTransition()
	{
		base.OnPreTransition();
		MazeController.ResetSaveData();
		PlayerData instance = PlayerData.instance;
		instance.tempRespawnType = 0;
		instance.tempRespawnMarker = "Death Respawn Marker";
		GameManager instance2 = GameManager.instance;
		instance2.RespawningHero = true;
		instance2.TimePasses();
	}
}
