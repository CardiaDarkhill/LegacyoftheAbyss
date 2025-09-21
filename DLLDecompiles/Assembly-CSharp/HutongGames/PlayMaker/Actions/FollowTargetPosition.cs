using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C51 RID: 3153
	public class FollowTargetPosition : FsmStateAction
	{
		// Token: 0x06005F8E RID: 24462 RVA: 0x001E5273 File Offset: 0x001E3473
		public override void Reset()
		{
			this.Target = null;
			this.FollowPos = null;
			this.LerpFactor = null;
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x001E528C File Offset: 0x001E348C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.self = safe.transform;
				if (this.self)
				{
					return;
				}
			}
			base.Finish();
		}

		// Token: 0x06005F90 RID: 24464 RVA: 0x001E52D0 File Offset: 0x001E34D0
		public override void OnUpdate()
		{
			float value = this.LerpFactor.Value;
			Vector2 a = this.self.position;
			Vector2 value2 = this.FollowPos.Value;
			this.self.SetPosition2D(Vector2.Lerp(a, value2, this.LerpFactor.Value));
		}

		// Token: 0x04005CE7 RID: 23783
		public FsmOwnerDefault Target;

		// Token: 0x04005CE8 RID: 23784
		public FsmVector2 FollowPos;

		// Token: 0x04005CE9 RID: 23785
		public FsmFloat LerpFactor;

		// Token: 0x04005CEA RID: 23786
		private Transform self;
	}
}
