using System;
using UnityEngine;

// Token: 0x0200039F RID: 927
public class BossStatueSetUnlockIndicator : MonoBehaviour
{
	// Token: 0x06001F44 RID: 8004 RVA: 0x0008EF8C File Offset: 0x0008D18C
	private void Start()
	{
		foreach (BossStatue bossStatue in Object.FindObjectsOfType<BossStatue>())
		{
			if (this.CheckIfNewBossStatue(bossStatue.StatueState) || this.CheckIfNewBossStatue(bossStatue.DreamStatueState))
			{
				this.newStatueCount++;
				bossStatue.OnSeenNewStatue += delegate()
				{
					this.newStatueCount--;
					if (this.newStatueCount == 0)
					{
						GameManager.instance.playerData.unlockedNewBossStatue = false;
						return;
					}
					if (this.newStatueCount < 0)
					{
						Debug.LogError("New statue count fell below zero. This means something has gone wrong!");
					}
				};
			}
		}
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x0008EFED File Offset: 0x0008D1ED
	private bool CheckIfNewBossStatue(BossStatue.Completion completion)
	{
		return completion.isUnlocked && !completion.hasBeenSeen;
	}

	// Token: 0x04001E36 RID: 7734
	private int newStatueCount;
}
