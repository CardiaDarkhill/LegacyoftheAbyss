using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D74 RID: 3444
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class SpawnRandomObjects : RigidBody2dActionBase
	{
		// Token: 0x0600647E RID: 25726 RVA: 0x001FAF18 File Offset: 0x001F9118
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
			this.originVariation = null;
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x001FAF78 File Offset: 0x001F9178
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
			base.Finish();
		}

		// Token: 0x0400632C RID: 25388
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x0400632D RID: 25389
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x0400632E RID: 25390
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x0400632F RID: 25391
		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04006330 RID: 25392
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x04006331 RID: 25393
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x04006332 RID: 25394
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04006333 RID: 25395
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04006334 RID: 25396
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04006335 RID: 25397
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariation;

		// Token: 0x04006336 RID: 25398
		private float vectorX;

		// Token: 0x04006337 RID: 25399
		private float vectorY;
	}
}
