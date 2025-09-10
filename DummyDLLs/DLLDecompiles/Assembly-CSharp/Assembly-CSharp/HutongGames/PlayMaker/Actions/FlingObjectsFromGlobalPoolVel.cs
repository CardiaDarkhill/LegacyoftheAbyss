using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C43 RID: 3139
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
	public class FlingObjectsFromGlobalPoolVel : RigidBody2dActionBase
	{
		// Token: 0x06005F4B RID: 24395 RVA: 0x001E4344 File Offset: 0x001E2544
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

		// Token: 0x06005F4C RID: 24396 RVA: 0x001E43AC File Offset: 0x001E25AC
		public override void OnEnter()
		{
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
			}
			base.Finish();
		}

		// Token: 0x04005C8A RID: 23690
		[RequiredField]
		[Tooltip("GameObject to spawn.")]
		public FsmGameObject gameObject;

		// Token: 0x04005C8B RID: 23691
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04005C8C RID: 23692
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04005C8D RID: 23693
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04005C8E RID: 23694
		[Tooltip("Maximum amount of objects to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x04005C8F RID: 23695
		public FsmFloat speedMinX;

		// Token: 0x04005C90 RID: 23696
		public FsmFloat speedMaxX;

		// Token: 0x04005C91 RID: 23697
		public FsmFloat speedMinY;

		// Token: 0x04005C92 RID: 23698
		public FsmFloat speedMaxY;

		// Token: 0x04005C93 RID: 23699
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04005C94 RID: 23700
		public FsmFloat originVariationY;

		// Token: 0x04005C95 RID: 23701
		private float vectorX;

		// Token: 0x04005C96 RID: 23702
		private float vectorY;

		// Token: 0x04005C97 RID: 23703
		private bool originAdjusted;
	}
}
