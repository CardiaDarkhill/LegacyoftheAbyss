using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B1 RID: 4785
	[ActionCategory("Hollow Knight")]
	public class AddHP : FsmStateAction
	{
		// Token: 0x06007D5C RID: 32092 RVA: 0x002562CC File Offset: 0x002544CC
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.amount = new FsmInt();
		}

		// Token: 0x06007D5D RID: 32093 RVA: 0x002562E4 File Offset: 0x002544E4
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (this.healToMax.Value)
					{
						component.HealToMax();
					}
					else if (!this.amount.IsNone)
					{
						component.hp += this.amount.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D58 RID: 32088
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D59 RID: 32089
		public FsmInt amount;

		// Token: 0x04007D5A RID: 32090
		public FsmBool healToMax;
	}
}
