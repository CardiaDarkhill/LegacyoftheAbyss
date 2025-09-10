using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000339 RID: 825
public class FlingGameObjectsAtIntervals : MonoBehaviour
{
	// Token: 0x06001CD6 RID: 7382 RVA: 0x000863A8 File Offset: 0x000845A8
	private void OnEnable()
	{
		this.spawnRoutine = base.StartCoroutine(this.SpawnRepeating());
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x000863BC File Offset: 0x000845BC
	private void OnDisable()
	{
		if (this.spawnRoutine != null)
		{
			base.StopCoroutine(this.spawnRoutine);
		}
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x000863D2 File Offset: 0x000845D2
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.spawnOffset, 0.2f);
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x00086403 File Offset: 0x00084603
	private IEnumerator SpawnRepeating()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(this.minSpawnDelay, this.maxSpawnDelay));
			if (this.spawnObjects.Length != 0)
			{
				this.spawnObjects[Random.Range(0, this.spawnObjects.Length)].Fling(base.transform.TransformPoint(this.spawnOffset), 1f);
			}
		}
		yield break;
	}

	// Token: 0x04001C37 RID: 7223
	[SerializeField]
	private Vector2 spawnOffset;

	// Token: 0x04001C38 RID: 7224
	[SerializeField]
	private float minSpawnDelay = 1f;

	// Token: 0x04001C39 RID: 7225
	[SerializeField]
	private float maxSpawnDelay = 3f;

	// Token: 0x04001C3A RID: 7226
	[SerializeField]
	private FlingGameObject[] spawnObjects;

	// Token: 0x04001C3B RID: 7227
	private Coroutine spawnRoutine;
}
