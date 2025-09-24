using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AE RID: 4782
	[ActionCategory("Hollow Knight")]
	public class SetSendKilledToObject : FsmStateAction
	{
		// Token: 0x06007D53 RID: 32083 RVA: 0x00256113 File Offset: 0x00254313
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.killedObject = new FsmGameObject();
		}

		// Token: 0x06007D54 RID: 32084 RVA: 0x0025612C File Offset: 0x0025432C
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.killedObject.IsNone)
				{
					component.SetSendKilledToObject(this.killedObject.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D52 RID: 32082
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D53 RID: 32083
		public FsmGameObject killedObject;
	}
}
