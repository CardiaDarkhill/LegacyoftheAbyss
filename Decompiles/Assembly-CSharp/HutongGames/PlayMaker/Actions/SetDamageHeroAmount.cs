using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A2 RID: 4770
	[ActionCategory("Hollow Knight")]
	public class SetDamageHeroAmount : FsmStateAction
	{
		// Token: 0x06007D25 RID: 32037 RVA: 0x002558A4 File Offset: 0x00253AA4
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.damageDealt = null;
		}

		// Token: 0x06007D26 RID: 32038 RVA: 0x002558B8 File Offset: 0x00253AB8
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				DamageHero component = safe.GetComponent<DamageHero>();
				if (component != null && !this.damageDealt.IsNone)
				{
					component.damageDealt = this.damageDealt.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D2B RID: 32043
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D2C RID: 32044
		public FsmInt damageDealt;
	}
}
