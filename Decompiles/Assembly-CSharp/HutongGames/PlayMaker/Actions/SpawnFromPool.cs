using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6B RID: 3435
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates a certain amount of objects held in designated Object Pool and fires them off")]
	public class SpawnFromPool : RigidBody2dActionBase
	{
		// Token: 0x0600645D RID: 25693 RVA: 0x001F9EAA File Offset: 0x001F80AA
		public override void Reset()
		{
			this.pool = null;
			this.adjustPosition = null;
			this.spawnMin = null;
			this.spawnMax = null;
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
		}

		// Token: 0x0600645E RID: 25694 RVA: 0x001F9EE4 File Offset: 0x001F80E4
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
					if (!this.adjustPosition.IsNone)
					{
						gameObject.transform.position += this.adjustPosition.Value;
					}
					gameObject.transform.parent = null;
				}
			}
			base.Finish();
		}

		// Token: 0x040062DF RID: 25311
		[RequiredField]
		[Tooltip("Pool object to draw from.")]
		public FsmGameObject pool;

		// Token: 0x040062E0 RID: 25312
		public FsmVector3 adjustPosition;

		// Token: 0x040062E1 RID: 25313
		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		// Token: 0x040062E2 RID: 25314
		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		// Token: 0x040062E3 RID: 25315
		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		// Token: 0x040062E4 RID: 25316
		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		// Token: 0x040062E5 RID: 25317
		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		// Token: 0x040062E6 RID: 25318
		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		// Token: 0x040062E7 RID: 25319
		private float vectorX;

		// Token: 0x040062E8 RID: 25320
		private float vectorY;

		// Token: 0x040062E9 RID: 25321
		private bool originAdjusted;
	}
}
