using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7C RID: 3196
	public class GetPositionAtDistance : FsmStateAction
	{
		// Token: 0x06006045 RID: 24645 RVA: 0x001E78CE File Offset: 0x001E5ACE
		public override void Reset()
		{
			this.Target = null;
			this.PositionFrom = null;
			this.Distance = null;
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001E78E8 File Offset: 0x001E5AE8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			Vector3 position = safe.transform.position;
			Vector2 vector = this.PositionFrom.Value;
			Vector2 vector2 = position - vector;
			if (vector2.magnitude <= this.Distance.Value)
			{
				base.Finish();
				return;
			}
			Vector2 b = vector2.normalized * this.Distance.Value;
			safe.transform.SetPosition2D(vector + b);
		}

		// Token: 0x04005D9A RID: 23962
		public FsmOwnerDefault Target;

		// Token: 0x04005D9B RID: 23963
		public FsmVector3 PositionFrom;

		// Token: 0x04005D9C RID: 23964
		public FsmFloat Distance;
	}
}
