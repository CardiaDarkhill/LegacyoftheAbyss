using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7A RID: 3450
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class SpawnRandomObjectsV2 : RigidBody2dActionBase
	{
		// Token: 0x06006498 RID: 25752 RVA: 0x001FBE54 File Offset: 0x001FA054
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
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
			this.originVariationX = null;
			this.originVariationY = null;
		}

		// Token: 0x06006499 RID: 25753 RVA: 0x001FBEBC File Offset: 0x001FA0BC
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
					if (this.rb2d)
					{
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
			base.Finish();
		}

		// Token: 0x04006377 RID: 25463
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x04006378 RID: 25464
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006379 RID: 25465
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x0400637A RID: 25466
		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x0400637B RID: 25467
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x0400637C RID: 25468
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x0400637D RID: 25469
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x0400637E RID: 25470
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x0400637F RID: 25471
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04006380 RID: 25472
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04006381 RID: 25473
		public FsmFloat originVariationY;

		// Token: 0x04006382 RID: 25474
		private float vectorX;

		// Token: 0x04006383 RID: 25475
		private float vectorY;

		// Token: 0x04006384 RID: 25476
		private bool originAdjusted;
	}
}
