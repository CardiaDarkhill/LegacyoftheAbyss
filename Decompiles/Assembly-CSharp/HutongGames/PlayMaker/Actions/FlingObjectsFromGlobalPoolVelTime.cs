using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C44 RID: 3140
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
	public class FlingObjectsFromGlobalPoolVelTime : RigidBody2dActionBase
	{
		// Token: 0x06005F4E RID: 24398 RVA: 0x001E45D4 File Offset: 0x001E27D4
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

		// Token: 0x06005F4F RID: 24399 RVA: 0x001E4639 File Offset: 0x001E2839
		public override void OnEnter()
		{
			this.delayTimer = this.startDelay.Value;
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x001E464C File Offset: 0x001E284C
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
							float x2 = Random.Range(this.speedMinX.Value, this.speedMaxX.Value);
							float y2 = Random.Range(this.speedMinY.Value, this.speedMaxY.Value);
							Vector2 linearVelocity = new Vector2(x2, y2);
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

		// Token: 0x04005C98 RID: 23704
		[RequiredField]
		[Tooltip("GameObject to spawn.")]
		public FsmGameObject gameObject;

		// Token: 0x04005C99 RID: 23705
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04005C9A RID: 23706
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04005C9B RID: 23707
		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		// Token: 0x04005C9C RID: 23708
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04005C9D RID: 23709
		[Tooltip("Maximum amount of objects to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x04005C9E RID: 23710
		public FsmFloat speedMinX;

		// Token: 0x04005C9F RID: 23711
		public FsmFloat speedMaxX;

		// Token: 0x04005CA0 RID: 23712
		public FsmFloat speedMinY;

		// Token: 0x04005CA1 RID: 23713
		public FsmFloat speedMaxY;

		// Token: 0x04005CA2 RID: 23714
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04005CA3 RID: 23715
		public FsmFloat originVariationY;

		// Token: 0x04005CA4 RID: 23716
		public FsmFloat startDelay;

		// Token: 0x04005CA5 RID: 23717
		private float vectorX;

		// Token: 0x04005CA6 RID: 23718
		private float vectorY;

		// Token: 0x04005CA7 RID: 23719
		private float timer;

		// Token: 0x04005CA8 RID: 23720
		private float delayTimer;

		// Token: 0x04005CA9 RID: 23721
		private bool originAdjusted;
	}
}
