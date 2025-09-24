using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200031C RID: 796
public class TrackSpawnedEnemies : MonoBehaviour
{
	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001C0C RID: 7180 RVA: 0x00082B8C File Offset: 0x00080D8C
	public int TotalTracked
	{
		get
		{
			return this.trackedEnemies.Count;
		}
	}

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x00082B99 File Offset: 0x00080D99
	public int TotalAlive
	{
		get
		{
			return (from enemy in this.trackedEnemies
			where enemy && enemy.hp > 0
			select enemy).ToList<HealthManager>().Count;
		}
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x00082BCF File Offset: 0x00080DCF
	public void Add(HealthManager enemyHealthManager)
	{
		this.trackedEnemies.Add(enemyHealthManager);
	}

	// Token: 0x04001B0D RID: 6925
	private List<HealthManager> trackedEnemies = new List<HealthManager>();
}
