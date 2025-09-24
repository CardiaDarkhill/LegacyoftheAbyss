using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B2 RID: 4786
	[ActionCategory("Hollow Knight")]
	public class SubtractHP : FsmStateAction
	{
		// Token: 0x06007D5F RID: 32095 RVA: 0x0025635F File Offset: 0x0025455F
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.amount = new FsmInt();
		}

		// Token: 0x06007D60 RID: 32096 RVA: 0x00256378 File Offset: 0x00254578
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.amount.IsNone)
				{
					component.hp -= this.amount.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D5B RID: 32091
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D5C RID: 32092
		public FsmInt amount;
	}
}
