using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D76 RID: 3446
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject over time and fires them off in random directions.")]
	public class SpawnRandomObjectsOverTimeV2 : RigidBody2dActionBase
	{
		// Token: 0x06006486 RID: 25734 RVA: 0x001FB434 File Offset: 0x001F9634
		public override void OnEnter()
		{
		}

		// Token: 0x06006487 RID: 25735 RVA: 0x001FB438 File Offset: 0x001F9638
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
			this.originVariationX = null;
			this.originVariationY = null;
			this.scaleMin = 1f;
			this.scaleMax = 1f;
		}

		// Token: 0x06006488 RID: 25736 RVA: 0x001FB4C4 File Offset: 0x001F96C4
		public override void OnUpdate()
		{
			this.DoSpawn();
		}

		// Token: 0x06006489 RID: 25737 RVA: 0x001FB4CC File Offset: 0x001F96CC
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
						if (gameObject.GetComponent<Rigidbody2D>() != null)
						{
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
						if (this.scaleMin != null && this.scaleMax != null)
						{
							float num4 = Random.Range(this.scaleMin.Value, this.scaleMax.Value);
							if (num4 != 1f)
							{
								gameObject.transform.localScale = new Vector3(num4, num4, num4);
							}
						}
					}
				}
			}
		}

		// Token: 0x04006346 RID: 25414
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x04006347 RID: 25415
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04006348 RID: 25416
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04006349 RID: 25417
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x0400634A RID: 25418
		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x0400634B RID: 25419
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x0400634C RID: 25420
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x0400634D RID: 25421
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x0400634E RID: 25422
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x0400634F RID: 25423
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04006350 RID: 25424
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04006351 RID: 25425
		public FsmFloat originVariationY;

		// Token: 0x04006352 RID: 25426
		[Tooltip("Minimum scale of clone.")]
		public FsmFloat scaleMin = 1f;

		// Token: 0x04006353 RID: 25427
		[Tooltip("Maximum scale of clone.")]
		public FsmFloat scaleMax = 1f;

		// Token: 0x04006354 RID: 25428
		private float vectorX;

		// Token: 0x04006355 RID: 25429
		private float vectorY;

		// Token: 0x04006356 RID: 25430
		private float timer;

		// Token: 0x04006357 RID: 25431
		private bool originAdjusted;
	}
}
