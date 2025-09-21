using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D75 RID: 3445
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject over time and fires them off in random directions.")]
	public class SpawnRandomObjectsOverTime : RigidBody2dActionBase
	{
		// Token: 0x06006481 RID: 25729 RVA: 0x001FB188 File Offset: 0x001F9388
		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.spawnMin = null;
			this.frequency = null;
			this.spawnMax = null;
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
			this.originVariation = null;
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x001FB1ED File Offset: 0x001F93ED
		public override void OnEnter()
		{
		}

		// Token: 0x06006483 RID: 25731 RVA: 0x001FB1EF File Offset: 0x001F93EF
		public override void OnUpdate()
		{
			this.DoSpawn();
		}

		// Token: 0x06006484 RID: 25732 RVA: 0x001FB1F8 File Offset: 0x001F93F8
		private void DoSpawn()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.frequency.Value)
			{
				this.timer = 0f;
				GameObject value = this.gameObject.Value;
				if (value != null)
				{
					Vector3 a = Vector3.zero;
					Vector3 zero = Vector3.zero;
					if (this.spawnPoint.Value != null)
					{
						a = this.spawnPoint.Value.transform.position;
						if (!this.position.IsNone)
						{
							a += this.position.Value;
						}
					}
					else if (!this.position.IsNone)
					{
						a = this.position.Value;
					}
					int num = Random.Range(this.spawnMin.Value, this.spawnMax.Value + 1);
					for (int i = 1; i <= num; i++)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(value, a, Quaternion.Euler(zero));
						if (this.originVariation != null)
						{
							float x = gameObject.transform.position.x + Random.Range(-this.originVariation.Value, this.originVariation.Value);
							float y = gameObject.transform.position.y + Random.Range(-this.originVariation.Value, this.originVariation.Value);
							float z = gameObject.transform.position.z;
							gameObject.transform.position = new Vector3(x, y, z);
						}
						base.CacheRigidBody2d(gameObject);
						float num2 = Random.Range(this.speedMin.Value, this.speedMax.Value);
						float num3 = Random.Range(this.angleMin.Value, this.angleMax.Value);
						this.vectorX = num2 * Mathf.Cos(num3 * 0.017453292f);
						this.vectorY = num2 * Mathf.Sin(num3 * 0.017453292f);
						Vector2 linearVelocity;
						linearVelocity.x = this.vectorX;
						linearVelocity.y = this.vectorY;
						this.rb2d.linearVelocity = linearVelocity;
					}
				}
			}
		}

		// Token: 0x04006338 RID: 25400
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x04006339 RID: 25401
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x0400633A RID: 25402
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x0400633B RID: 25403
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x0400633C RID: 25404
		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x0400633D RID: 25405
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x0400633E RID: 25406
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x0400633F RID: 25407
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04006340 RID: 25408
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04006341 RID: 25409
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04006342 RID: 25410
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariation;

		// Token: 0x04006343 RID: 25411
		private float vectorX;

		// Token: 0x04006344 RID: 25412
		private float vectorY;

		// Token: 0x04006345 RID: 25413
		private float timer;
	}
}
