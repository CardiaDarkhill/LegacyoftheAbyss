using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3F RID: 3135
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
	public class FlingObjectsFromGlobalPool : RigidBody2dActionBase
	{
		// Token: 0x06005F38 RID: 24376 RVA: 0x001E348C File Offset: 0x001E168C
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
			this.FSM = new FsmString
			{
				UseVariable = true
			};
			this.FSMEvent = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x001E3518 File Offset: 0x001E1718
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
					float num2 = Random.Range(this.speedMin.Value, this.speedMax.Value);
					float num3 = Random.Range(this.angleMin.Value, this.angleMax.Value);
					this.vectorX = num2 * Mathf.Cos(num3 * 0.017453292f);
					this.vectorY = num2 * Mathf.Sin(num3 * 0.017453292f);
					Vector2 linearVelocity;
					linearVelocity.x = this.vectorX;
					linearVelocity.y = this.vectorY;
					this.rb2d.linearVelocity = linearVelocity;
					if (!this.FSM.IsNone)
					{
						FSMUtility.LocateFSM(gameObject, this.FSM.Value).SendEvent(this.FSMEvent.Value);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x001E37A0 File Offset: 0x001E19A0
		private string GetFriendlyNameForAngle(float angle)
		{
			angle = Mathf.Repeat(angle, 360f);
			if (angle >= 45f && angle < 135f)
			{
				return "Up";
			}
			if (angle >= 135f && angle < 225f)
			{
				return "Left";
			}
			if (angle >= 225f && angle < 315f)
			{
				return "Down";
			}
			return "Right";
		}

		// Token: 0x04005C44 RID: 23620
		[RequiredField]
		[Tooltip("GameObject to spawn.")]
		public FsmGameObject gameObject;

		// Token: 0x04005C45 RID: 23621
		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		// Token: 0x04005C46 RID: 23622
		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		// Token: 0x04005C47 RID: 23623
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x04005C48 RID: 23624
		[Tooltip("Maximum amount of objects to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x04005C49 RID: 23625
		[Tooltip("Minimum speed objects are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x04005C4A RID: 23626
		[Tooltip("Maximum speed objects are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x04005C4B RID: 23627
		[Tooltip("Minimum angle objects are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x04005C4C RID: 23628
		[Tooltip("Maximum angle objects are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x04005C4D RID: 23629
		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		// Token: 0x04005C4E RID: 23630
		public FsmFloat originVariationY;

		// Token: 0x04005C4F RID: 23631
		[Tooltip("Optional: Name of FSM on object you want to send an event to after spawn")]
		public FsmString FSM;

		// Token: 0x04005C50 RID: 23632
		[Tooltip("Optional: Event you want to send to object after spawn")]
		public FsmString FSMEvent;

		// Token: 0x04005C51 RID: 23633
		private float vectorX;

		// Token: 0x04005C52 RID: 23634
		private float vectorY;

		// Token: 0x04005C53 RID: 23635
		private bool originAdjusted;
	}
}
