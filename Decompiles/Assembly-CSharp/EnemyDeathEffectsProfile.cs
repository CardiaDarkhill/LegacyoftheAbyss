using System;
using UnityEngine;

// Token: 0x020002D6 RID: 726
[CreateAssetMenu(menuName = "Profiles/Enemy Death Effects Profile")]
public class EnemyDeathEffectsProfile : ScriptableObject
{
	// Token: 0x060019C7 RID: 6599 RVA: 0x0007654C File Offset: 0x0007474C
	public void SpawnEffects(Transform spawnPoint, Vector3 offset, Transform corpse, Color? bloodColorOverride = null, float blackThreadAmount = -1f)
	{
		foreach (BloodSpawner.Config config in this.blood)
		{
			config.Position += offset;
			GameObject gameObject = BloodSpawner.SpawnBlood(config, spawnPoint, bloodColorOverride);
			if (gameObject && corpse)
			{
				FollowTransform follow = gameObject.GetComponent<FollowTransform>() ?? gameObject.AddComponent<FollowTransform>();
				follow.Target = corpse;
				RecycleResetHandler.Add(gameObject, delegate()
				{
					follow.Target = null;
				});
			}
		}
		Vector3 position = spawnPoint.TransformPoint(offset);
		GameObject[] array2 = this.spawnEffectPrefabs;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject2;
			BlackThreadDeathAltProxy component = (gameObject2 = array2[i]).GetComponent<BlackThreadDeathAltProxy>();
			if (component)
			{
				BlackThreadState component2 = spawnPoint.GetComponent<BlackThreadState>();
				if (component2 && component2.IsVisiblyThreaded)
				{
					gameObject2 = component.AltPrefab;
				}
			}
			if (gameObject2)
			{
				GameObject gameObject3 = gameObject2.Spawn(position);
				if (blackThreadAmount > 0f)
				{
					BlackThreadEffectRendererGroup component3 = gameObject3.GetComponent<BlackThreadEffectRendererGroup>();
					if (component3 != null)
					{
						component3.SetBlackThreadAmount(blackThreadAmount);
					}
				}
				if (corpse)
				{
					FollowTransform component4 = gameObject3.GetComponent<FollowTransform>();
					if (component4)
					{
						component4.Target = corpse;
						component4.Offset = Vector3.zero;
					}
				}
			}
		}
		FlingUtils.Config[] array3 = this.spawnFlings;
		for (int i = 0; i < array3.Length; i++)
		{
			FlingUtils.SpawnAndFling(array3[i], spawnPoint, offset, null, blackThreadAmount);
		}
		foreach (AudioEvent audioEvent in this.deathSounds)
		{
			audioEvent.SpawnAndPlayOneShot(position, null);
		}
	}

	// Token: 0x060019C8 RID: 6600 RVA: 0x0007670C File Offset: 0x0007490C
	public void EnsurePersonalPool(GameObject gameObject)
	{
		for (int i = 0; i < this.spawnFlings.Length; i++)
		{
			FlingUtils.Config config = this.spawnFlings[i];
			if (!(config.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, config.Prefab, 3, false, false, false);
			}
		}
		for (int j = 0; j < this.spawnEffectPrefabs.Length; j++)
		{
			GameObject prefab = this.spawnEffectPrefabs[j];
			PersonalObjectPool.EnsurePooledInScene(gameObject, prefab, 3, false, false, false);
		}
		PersonalObjectPool.CreateIfRequired(gameObject, false);
	}

	// Token: 0x040018B8 RID: 6328
	[SerializeField]
	private BloodSpawner.Config[] blood;

	// Token: 0x040018B9 RID: 6329
	[SerializeField]
	private GameObject[] spawnEffectPrefabs;

	// Token: 0x040018BA RID: 6330
	[SerializeField]
	private FlingUtils.Config[] spawnFlings;

	// Token: 0x040018BB RID: 6331
	[SerializeField]
	private AudioEvent[] deathSounds;
}
