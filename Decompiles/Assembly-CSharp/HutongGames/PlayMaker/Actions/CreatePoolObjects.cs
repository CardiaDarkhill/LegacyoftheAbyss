using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C08 RID: 3080
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class CreatePoolObjects : RigidBody2dActionBase
	{
		// Token: 0x06005E05 RID: 24069 RVA: 0x001DA0FE File Offset: 0x001D82FE
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.amount = null;
			this.originVariationX = null;
			this.originVariationY = null;
			this.deactivate = false;
		}

		// Token: 0x06005E06 RID: 24070 RVA: 0x001DA138 File Offset: 0x001D8338
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 b = this.pool.Value.transform.position;
				Vector3 zero = Vector3.zero;
				if (!this.position.IsNone)
				{
					b = this.position.Value + b;
				}
				int value2 = this.amount.Value;
				for (int i = 1; i <= value2; i++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(value, b, Quaternion.Euler(zero));
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
					gameObject.transform.parent = this.pool.Value.transform;
					if (this.deactivate)
					{
						gameObject.SetActive(false);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04005A61 RID: 23137
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		// Token: 0x04005A62 RID: 23138
		[RequiredField]
		[Tooltip("GameObject which will be used as pool (spawned objects will be parented to this).")]
		public FsmGameObject pool;

		// Token: 0x04005A63 RID: 23139
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04005A64 RID: 23140
		[Tooltip("Amount of clones to be spawned.")]
		public FsmInt amount;

		// Token: 0x04005A65 RID: 23141
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04005A66 RID: 23142
		public FsmFloat originVariationY;

		// Token: 0x04005A67 RID: 23143
		[Tooltip("Deactivate the pool objects after creating. Use if the objects don't deactivate themselves.")]
		public bool deactivate;

		// Token: 0x04005A68 RID: 23144
		private float vectorX;

		// Token: 0x04005A69 RID: 23145
		private float vectorY;

		// Token: 0x04005A6A RID: 23146
		private bool originAdjusted;
	}
}
