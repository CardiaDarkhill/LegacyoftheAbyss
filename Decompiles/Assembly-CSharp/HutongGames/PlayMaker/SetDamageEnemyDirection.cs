using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF9 RID: 2809
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyDirection : FsmStateAction
	{
		// Token: 0x06005906 RID: 22790 RVA: 0x001C3B77 File Offset: 0x001C1D77
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.damageDirection = null;
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x001C3B8C File Offset: 0x001C1D8C
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null && !this.damageDirection.IsNone)
				{
					component.direction = this.damageDirection.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04005427 RID: 21543
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04005428 RID: 21544
		public FsmFloat damageDirection;
	}
}
