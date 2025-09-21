using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D77 RID: 3447
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadial : FsmStateAction
	{
		// Token: 0x0600648B RID: 25739 RVA: 0x001FB7C8 File Offset: 0x001F99C8
		public override void Awake()
		{
			GameObject value = this.Prefab.Value;
			if (value != null)
			{
				PersonalObjectPool.EnsurePooledInScene(base.Fsm.Owner.gameObject, value, this.MaxCount.Value, true, false, false);
			}
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x001FB80E File Offset: 0x001F9A0E
		public override void Reset()
		{
			this.Prefab = null;
			this.SpawnPoint = null;
			this.MinCount = null;
			this.MaxCount = null;
			this.MinAngle = null;
			this.MaxAngle = null;
			this.AngleErrorMax = null;
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x001FB844 File Offset: 0x001F9A44
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
					GameObject gameObject = value.Spawn();
					gameObject.transform.position = position;
					float num2 = Mathf.Lerp(this.MinAngle.Value, this.MaxAngle.Value, (float)i / (float)num);
					num2 += Random.Range(-this.AngleErrorMax.Value, this.AngleErrorMax.Value);
					gameObject.transform.SetRotation2D(num2);
				}
			}
			base.Finish();
		}

		// Token: 0x04006358 RID: 25432
		public FsmGameObject Prefab;

		// Token: 0x04006359 RID: 25433
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x0400635A RID: 25434
		public FsmInt MinCount;

		// Token: 0x0400635B RID: 25435
		public FsmInt MaxCount;

		// Token: 0x0400635C RID: 25436
		public FsmFloat MinAngle;

		// Token: 0x0400635D RID: 25437
		public FsmFloat MaxAngle;

		// Token: 0x0400635E RID: 25438
		public FsmFloat AngleErrorMax;
	}
}
