using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF8 RID: 2808
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyAmount : FsmStateAction
	{
		// Token: 0x06005903 RID: 22787 RVA: 0x001C3B01 File Offset: 0x001C1D01
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.damageDealt = null;
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x001C3B18 File Offset: 0x001C1D18
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null && !this.damageDealt.IsNone)
				{
					component.damageDealt = this.damageDealt.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04005425 RID: 21541
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04005426 RID: 21542
		public FsmInt damageDealt;
	}
}
