using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D79 RID: 3449
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadialRandom : FsmStateAction
	{
		// Token: 0x06006493 RID: 25747 RVA: 0x001FBB04 File Offset: 0x001F9D04
		public override void Reset()
		{
			this.Prefab = null;
			this.SpawnPoint = null;
			this.SetRotation = null;
			this.SpawnAsChildren = null;
			this.MinCount = null;
			this.MaxCount = null;
			this.MinAngle = null;
			this.MaxAngle = null;
			this.MinSpawnRadius = null;
			this.MaxSpawnRadius = null;
			this.MinSpacing = null;
			this.NeedsLineOfSight = null;
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x001FBB65 File Offset: 0x001F9D65
		public override void Awake()
		{
			this.recycleResetDelegate = new Action<GameObject>(this.OnRecycleReset);
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x001FBB7C File Offset: 0x001F9D7C
		public override void OnEnter()
		{
			GameObject safe = this.SpawnPoint.GetSafe(this);
			GameObject value = this.Prefab.Value;
			if (safe && value)
			{
				this.tempPosStore.Clear();
				foreach (GameObject gameObject in this.spawnedObjs)
				{
					this.tempPosStore.Add(gameObject.transform.position);
				}
				Vector3 position = safe.transform.position;
				bool flag = false;
				int num = Random.Range(this.MinCount.Value, this.MaxCount.Value + 1);
				for (int i = 0; i < num; i++)
				{
					int num2 = 100;
					float num3;
					Vector3 vector;
					do
					{
						IL_C0:
						num3 = Random.Range(this.MinAngle.Value, this.MaxAngle.Value);
						float x = Mathf.Cos(num3 * 0.017453292f);
						float y = Mathf.Sin(num3 * 0.017453292f);
						float d = Random.Range(this.MinSpawnRadius.Value, this.MaxSpawnRadius.Value);
						vector = position + new Vector3(x, y, 0f) * d;
						foreach (Vector2 b in this.tempPosStore)
						{
							if (Vector2.Distance(vector, b) <= this.MinSpacing.Value)
							{
								num2--;
								if (num2 <= 0)
								{
									break;
								}
								goto IL_C0;
							}
						}
						Vector3 vector2 = vector - position;
						if (!this.NeedsLineOfSight.Value || num2 <= 0 || !Helper.IsRayHittingNoTriggers(position, vector2.normalized, vector2.magnitude, 256))
						{
							break;
						}
						num2--;
					}
					while (num2 > 0);
					if (num2 <= 0)
					{
						if (flag)
						{
							break;
						}
						vector = position;
					}
					flag = true;
					GameObject gameObject2 = value.Spawn(this.SpawnAsChildren.Value ? safe.transform : null);
					gameObject2.transform.position = vector;
					if (this.SetRotation.Value)
					{
						gameObject2.transform.SetRotation2D(num3);
					}
					this.tempPosStore.Add(vector);
					if (this.spawnedObjs.AddIfNotPresent(gameObject2))
					{
						RecycleResetHandler.Add(gameObject2, this.recycleResetDelegate);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006496 RID: 25750 RVA: 0x001FBE24 File Offset: 0x001FA024
		private void OnRecycleReset(GameObject obj)
		{
			this.spawnedObjs.Remove(obj);
		}

		// Token: 0x04006368 RID: 25448
		[RequiredField]
		public FsmGameObject Prefab;

		// Token: 0x04006369 RID: 25449
		[RequiredField]
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x0400636A RID: 25450
		public FsmBool SetRotation;

		// Token: 0x0400636B RID: 25451
		public FsmBool SpawnAsChildren;

		// Token: 0x0400636C RID: 25452
		public FsmInt MinCount;

		// Token: 0x0400636D RID: 25453
		public FsmInt MaxCount;

		// Token: 0x0400636E RID: 25454
		public FsmFloat MinAngle;

		// Token: 0x0400636F RID: 25455
		public FsmFloat MaxAngle;

		// Token: 0x04006370 RID: 25456
		public FsmFloat MinSpawnRadius;

		// Token: 0x04006371 RID: 25457
		public FsmFloat MaxSpawnRadius;

		// Token: 0x04006372 RID: 25458
		public FsmFloat MinSpacing;

		// Token: 0x04006373 RID: 25459
		public FsmBool NeedsLineOfSight;

		// Token: 0x04006374 RID: 25460
		private Action<GameObject> recycleResetDelegate;

		// Token: 0x04006375 RID: 25461
		private readonly List<GameObject> spawnedObjs = new List<GameObject>();

		// Token: 0x04006376 RID: 25462
		private readonly List<Vector2> tempPosStore = new List<Vector2>();
	}
}
