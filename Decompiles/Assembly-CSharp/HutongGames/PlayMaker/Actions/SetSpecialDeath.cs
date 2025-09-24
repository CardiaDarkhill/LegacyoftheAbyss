using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AD RID: 4781
	[ActionCategory("Hollow Knight")]
	public class SetSpecialDeath : FsmStateAction
	{
		// Token: 0x06007D50 RID: 32080 RVA: 0x00256099 File Offset: 0x00254299
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.hasSpecialDeath = new FsmBool();
		}

		// Token: 0x06007D51 RID: 32081 RVA: 0x002560B4 File Offset: 0x002542B4
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.hasSpecialDeath.IsNone)
				{
					component.hasSpecialDeath = this.hasSpecialDeath.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D50 RID: 32080
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D51 RID: 32081
		public FsmBool hasSpecialDeath;
	}
}
