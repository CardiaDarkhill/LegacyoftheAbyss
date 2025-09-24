using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7B RID: 3451
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class SpawnRandomObjectsVelocity : RigidBody2dActionBase
	{
		// Token: 0x0600649B RID: 25755 RVA: 0x001FC11C File Offset: 0x001FA31C
		public override void Reset()
		{
			this.gameObject = null;
			this.spawnPoint = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.spawnMin = null;
			this.spawnMax = null;
			this.speedMinX = null;
			this.speedMaxX = null;
			this.speedMinY = null;
			this.speedMaxY = null;
			this.originVariationX = null;
			this.originVariationY = null;
		}

		// Token: 0x0600649C RID: 25756 RVA: 0x001FC184 File Offset: 0x001FA384
		public override void OnEnter()
		{
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
					float x = gameObject.transform.position.x;
					float y = gameObject.transform.position.y;
					float z = gameObject.transform.position.z;
					if (this.originVariationX != null)
					{
						x = gameObject.transform.position.x + Random.Range(-this.originVariationX.Value, this.originVariationX.Value);
						this.originAdjusted = true;
					}
					if (this.originVariationY != null)
					{
						y = gameObject.transform.position.y + Random.Range(-this.originVariationY.Value, this.originVariationY.Value);
						this.originAdjusted = true;
					}
					if (this.originAdjusted)
					{
						gameObject.transform.position = new Vector3(x, y, z);
					}
					base.CacheRigidBody2d(gameObject);
					float x2 = Random.Range(this.speedMinX.Value, this.speedMaxX.Value);
					float y2 = Random.Range(this.speedMinY.Value, this.speedMaxY.Value);
					Vector2 linearVelocity = new Vector2(x2, y2);
					this.rb2d.linearVelocity = linearVelocity;
				}
			}
			base.Finish();
		}

		// Token: 0x04006385 RID: 25477
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x04006386 RID: 25478
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006387 RID: 25479
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006388 RID: 25480
		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04006389 RID: 25481
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x0400638A RID: 25482
		public FsmFloat speedMinX;

		// Token: 0x0400638B RID: 25483
		public FsmFloat speedMaxX;

		// Token: 0x0400638C RID: 25484
		public FsmFloat speedMinY;

		// Token: 0x0400638D RID: 25485
		public FsmFloat speedMaxY;

		// Token: 0x0400638E RID: 25486
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x0400638F RID: 25487
		public FsmFloat originVariationY;

		// Token: 0x04006390 RID: 25488
		private float vectorX;

		// Token: 0x04006391 RID: 25489
		private float vectorY;

		// Token: 0x04006392 RID: 25490
		private bool originAdjusted;
	}
}
