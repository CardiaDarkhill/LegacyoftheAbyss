using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200031D RID: 797
[ActionCategory("Hollow Knight")]
public class TrackSpawnedEnemiesAdd : FsmStateAction
{
	// Token: 0x06001C10 RID: 7184 RVA: 0x00082BF0 File Offset: 0x00080DF0
	public override void Reset()
	{
		this.Target = null;
		this.SpawnedEnemy = null;
		this.UsesEnemySpawner = null;
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x00082C08 File Offset: 0x00080E08
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (safe && this.SpawnedEnemy.Value)
		{
			TrackSpawnedEnemies track = safe.GetComponent<TrackSpawnedEnemies>() ?? safe.AddComponent<TrackSpawnedEnemies>();
			if (this.UsesEnemySpawner.Value)
			{
				EnemySpawner component = this.SpawnedEnemy.Value.GetComponent<EnemySpawner>();
				if (component)
				{
					component.OnEnemySpawned += delegate(GameObject enemy)
					{
						this.AddTracked(track, enemy);
					};
				}
			}
			else
			{
				this.AddTracked(track, this.SpawnedEnemy.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x00082CB8 File Offset: 0x00080EB8
	private void AddTracked(TrackSpawnedEnemies tracker, GameObject obj)
	{
		HealthManager component = obj.GetComponent<HealthManager>();
		if (component)
		{
			tracker.Add(component);
		}
	}

	// Token: 0x04001B0E RID: 6926
	public FsmOwnerDefault Target;

	// Token: 0x04001B0F RID: 6927
	public FsmGameObject SpawnedEnemy;

	// Token: 0x04001B10 RID: 6928
	public FsmBool UsesEnemySpawner;
}
