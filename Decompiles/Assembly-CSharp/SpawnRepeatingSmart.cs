using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000281 RID: 641
public class SpawnRepeatingSmart : MonoBehaviour
{
	// Token: 0x060016AA RID: 5802 RVA: 0x0006612D File Offset: 0x0006432D
	private void OnEnable()
	{
		this.timer = 0f;
		this.hc = base.GetComponentInParent<HeroController>();
		this.gm = GameManager.instance;
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x00066151 File Offset: 0x00064351
	private void OnDisable()
	{
		this.hc = null;
		this.gm = null;
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x00066164 File Offset: 0x00064364
	private void Update()
	{
		if (this.gm && this.gm.GameState == GameState.LOADING)
		{
			for (int i = this.spawnedObjects.Count - 1; i >= 0; i--)
			{
				this.spawnedObjects[i].Recycle();
			}
			return;
		}
		if (this.hc && !this.hc.isHeroInPosition)
		{
			return;
		}
		if (this.spawnDelay > 0f)
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.spawnDelay)
			{
				this.timer = 0f;
				this.TrySpawn();
			}
		}
		if (this.spawnDistance > 0f && Vector2.Distance(this.lastSpawnPosition, base.transform.position) >= this.spawnDistance)
		{
			this.TrySpawn();
		}
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x00066248 File Offset: 0x00064448
	private void TrySpawn()
	{
		if (this.maxInDistance <= 0 || this.minDistance <= 0f)
		{
			this.DoSpawn();
			return;
		}
		int num = 0;
		Vector3 position = base.transform.position;
		foreach (GameObject gameObject in this.spawnedObjects)
		{
			if (!(gameObject == null) && Vector2.Distance(position, gameObject.transform.position) <= this.minDistance)
			{
				num++;
			}
		}
		if (num < this.maxInDistance)
		{
			this.DoSpawn();
		}
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x00066300 File Offset: 0x00064500
	private void DoSpawn()
	{
		Vector3 position = base.transform.position;
		this.lastSpawnPosition = position;
		GameObject gameObject = this.prefab.Spawn(position);
		this.spawnedObjects.Add(gameObject);
		RecycleResetHandler.Add(gameObject, delegate(GameObject o)
		{
			this.spawnedObjects.Remove(o);
		});
	}

	// Token: 0x04001527 RID: 5415
	[SerializeField]
	private GameObject prefab;

	// Token: 0x04001528 RID: 5416
	[SerializeField]
	private float minDistance;

	// Token: 0x04001529 RID: 5417
	[SerializeField]
	private int maxInDistance;

	// Token: 0x0400152A RID: 5418
	[SerializeField]
	private float spawnDelay;

	// Token: 0x0400152B RID: 5419
	[SerializeField]
	private float spawnDistance;

	// Token: 0x0400152C RID: 5420
	private readonly List<GameObject> spawnedObjects = new List<GameObject>();

	// Token: 0x0400152D RID: 5421
	private float timer;

	// Token: 0x0400152E RID: 5422
	private Vector2 lastSpawnPosition;

	// Token: 0x0400152F RID: 5423
	private HeroController hc;

	// Token: 0x04001530 RID: 5424
	private GameManager gm;
}
