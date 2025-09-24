using System;

// Token: 0x020001F2 RID: 498
[Serializable]
public class SaveGameData
{
	// Token: 0x06001337 RID: 4919 RVA: 0x0005824C File Offset: 0x0005644C
	public SaveGameData()
	{
		this.playerData = new PlayerData();
		this.sceneData = new SceneData();
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x0005826A File Offset: 0x0005646A
	public SaveGameData(PlayerData playerData, SceneData sceneData)
	{
		this.playerData = playerData;
		this.sceneData = sceneData;
	}

	// Token: 0x040011AE RID: 4526
	public PlayerData playerData;

	// Token: 0x040011AF RID: 4527
	public SceneData sceneData;
}
