using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000350 RID: 848
public class GameCompletionScreen : MonoBehaviour
{
	// Token: 0x06001D59 RID: 7513 RVA: 0x00087800 File Offset: 0x00085A00
	private void Start()
	{
		this.gm = GameManager.instance;
		PlayerData playerData = this.gm.playerData;
		playerData.CountGameCompletion();
		SaveStats saveStats = new SaveStats(playerData, null);
		this.percentageNumber.text = saveStats.GetCompletionPercentage();
		this.playTimeNumber.text = saveStats.GetPlaytimeHHMMSS();
	}

	// Token: 0x04001C8E RID: 7310
	public TextMeshPro percentageNumber;

	// Token: 0x04001C8F RID: 7311
	public TextMeshPro playTimeNumber;

	// Token: 0x04001C90 RID: 7312
	private GameManager gm;
}
