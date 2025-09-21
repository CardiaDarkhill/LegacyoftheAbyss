using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C40 RID: 3136
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
	public class FlingObjectsFromGlobalPoolTime : RigidBody2dActionBase
	{
		// Token: 0x06005F3C RID: 24380 RVA: 0x001E380C File Offset: 0x001E1A0C
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

		// Token: 0x06005F3D RID: 24381 RVA: 0x001E3871 File Offset: 0x001E1A71
		public override void OnEnter()
		{
			this.delayTimer = this.startDelay.Value;
		}

		// Token: 0x06005F3E RID: 24382 RVA: 0x001E3884 File Offset: 0x001E1A84
		public override void OnUpdate()
		{
			if (this.delayTimer <= 0f)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= this.frequency.Value)
				{
					this.timer = 0f;
					if (this.gameObject.Value != null)
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
							GameObject gameObject = this.gameObject.Value.Spawn(a, Quaternion.Euler(zero));
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
							BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
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
						return;
					}
				}
			}
			else
			{
				this.delayTimer -= Time.deltaTime;
			}
		}

		// Token: 0x04005C54 RID: 23636
		[RequiredField]
		[Tooltip("GameObject to spawn.")]
		public FsmGameObject gameObject;

		// Token: 0x04005C55 RID: 23637
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04005C56 RID: 23638
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04005C57 RID: 23639
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x04005C58 RID: 23640
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04005C59 RID: 23641
		[Tooltip("Maximum amount of objects to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x04005C5A RID: 23642
		[Tooltip("Minimum speed objects are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x04005C5B RID: 23643
		[Tooltip("Maximum speed objects are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04005C5C RID: 23644
		[Tooltip("Minimum angle objects are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04005C5D RID: 23645
		[Tooltip("Maximum angle objects are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04005C5E RID: 23646
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04005C5F RID: 23647
		public FsmFloat originVariationY;

		// Token: 0x04005C60 RID: 23648
		public FsmFloat startDelay;

		// Token: 0x04005C61 RID: 23649
		private float vectorX;

		// Token: 0x04005C62 RID: 23650
		private float vectorY;

		// Token: 0x04005C63 RID: 23651
		private float timer;

		// Token: 0x04005C64 RID: 23652
		private float delayTimer;

		// Token: 0x04005C65 RID: 23653
		private bool originAdjusted;

		// Token: 0x04005C66 RID: 23654
		private ObjectPool objectPool;
	}
}
