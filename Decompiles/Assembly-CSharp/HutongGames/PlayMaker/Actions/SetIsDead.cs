using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B6 RID: 4790
	[ActionCategory("Hollow Knight")]
	public class SetIsDead : FsmStateAction
	{
		// Token: 0x06007D6C RID: 32108 RVA: 0x0025656B File Offset: 0x0025476B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.setValue = new FsmBool();
		}

		// Token: 0x06007D6D RID: 32109 RVA: 0x00256584 File Offset: 0x00254784
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetIsDead(this.setValue.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D64 RID: 32100
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D65 RID: 32101
		public FsmBool setValue;
	}
}
