using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D78 RID: 3448
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadialV2 : FsmStateAction
	{
		// Token: 0x0600648F RID: 25743 RVA: 0x001FB92C File Offset: 0x001F9B2C
		public override void Awake()
		{
			GameObject value = this.Prefab.Value;
			if (value != null)
			{
				PersonalObjectPool.EnsurePooledInScene(base.Fsm.Owner.gameObject, value, this.MaxCount.Value, true, false, false);
			}
		}

		// Token: 0x06006490 RID: 25744 RVA: 0x001FB974 File Offset: 0x001F9B74
		public override void Reset()
		{
			this.Prefab = null;
			this.SpawnPoint = null;
			this.SpawnAsChildren = null;
			this.MinCount = null;
			this.MaxCount = null;
			this.MinAngle = null;
			this.MaxAngle = null;
			this.AngleErrorMax = null;
			this.SpawnRadius = null;
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x001FB9C0 File Offset: 0x001F9BC0
		public override void OnEnter()
		{
			GameObject safe = this.SpawnPoint.GetSafe(this);
			GameObject value = this.Prefab.Value;
			if (safe && value)
			{
				Vector3 position = safe.transform.position;
				int num = Random.Range(this.MinCount.Value, this.MaxCount.Value + 1);
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = value.Spawn(this.SpawnAsChildren.Value ? safe.transform : null);
					float num2 = Mathf.Lerp(this.MinAngle.Value, this.MaxAngle.Value, (float)i / (float)num);
					num2 += Random.Range(-this.AngleErrorMax.Value, this.AngleErrorMax.Value);
					gameObject.transform.SetRotation2D(num2);
					float x = Mathf.Cos(num2 * 0.017453292f);
					float y = Mathf.Sin(num2 * 0.017453292f);
					gameObject.transform.position = position + new Vector3(x, y, 0f) * this.SpawnRadius.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x0400635F RID: 25439
		public FsmGameObject Prefab;

		// Token: 0x04006360 RID: 25440
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04006361 RID: 25441
		public FsmBool SpawnAsChildren;

		// Token: 0x04006362 RID: 25442
		public FsmInt MinCount;

		// Token: 0x04006363 RID: 25443
		public FsmInt MaxCount;

		// Token: 0x04006364 RID: 25444
		public FsmFloat MinAngle;

		// Token: 0x04006365 RID: 25445
		public FsmFloat MaxAngle;

		// Token: 0x04006366 RID: 25446
		public FsmFloat AngleErrorMax;

		// Token: 0x04006367 RID: 25447
		public FsmFloat SpawnRadius;
	}
}
