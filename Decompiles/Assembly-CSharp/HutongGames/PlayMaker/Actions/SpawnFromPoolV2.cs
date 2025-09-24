using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6C RID: 3436
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates a certain amount of objects held in designated Object Pool and fires them off")]
	public class SpawnFromPoolV2 : RigidBody2dActionBase
	{
		// Token: 0x06006460 RID: 25696 RVA: 0x001FA053 File Offset: 0x001F8253
		public override void Reset()
		{
			this.pool = null;
			this.setPosition = null;
			this.spawnMin = null;
			this.spawnMax = null;
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x001FA090 File Offset: 0x001F8290
		public override void OnEnter()
		{
			GameObject value = this.pool.Value;
			if (value != null)
			{
				int num = Random.Range(this.spawnMin.Value, this.spawnMax.Value + 1);
				for (int i = 1; i <= num; i++)
				{
					int childCount = value.transform.childCount;
					if (childCount <= 0)
					{
						base.Finish();
					}
					if (childCount == 0)
					{
						return;
					}
					GameObject gameObject = value.transform.GetChild(Random.Range(0, childCount)).gameObject;
					gameObject.SetActive(true);
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
					if (!this.setPosition.IsNone)
					{
						gameObject.transform.position = this.setPosition.Value;
					}
					gameObject.transform.parent = null;
				}
			}
			base.Finish();
		}

		// Token: 0x040062EA RID: 25322
		[RequiredField]
		[Tooltip("Pool object to draw from.")]
		public FsmGameObject pool;

		// Token: 0x040062EB RID: 25323
		public FsmVector3 setPosition;

		// Token: 0x040062EC RID: 25324
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x040062ED RID: 25325
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x040062EE RID: 25326
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x040062EF RID: 25327
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x040062F0 RID: 25328
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x040062F1 RID: 25329
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x040062F2 RID: 25330
		[Tooltip("Optional: Name of FSM on object you want to send an event to after spawn")]
		public FsmString FSM;

		// Token: 0x040062F3 RID: 25331
		[Tooltip("Optional: Event you want to send to object after spawn")]
		public FsmString FSMEvent;

		// Token: 0x040062F4 RID: 25332
		private float vectorX;

		// Token: 0x040062F5 RID: 25333
		private float vectorY;

		// Token: 0x040062F6 RID: 25334
		private bool originAdjusted;
	}
}
