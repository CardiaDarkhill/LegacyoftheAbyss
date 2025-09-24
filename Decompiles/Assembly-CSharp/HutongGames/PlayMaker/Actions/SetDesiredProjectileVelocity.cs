using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001302 RID: 4866
	public class SetDesiredProjectileVelocity : FsmStateAction
	{
		// Token: 0x06007E83 RID: 32387 RVA: 0x00259378 File Offset: 0x00257578
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007E84 RID: 32388 RVA: 0x00259384 File Offset: 0x00257584
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				ProjectileVelocityManager component = safe.GetComponent<ProjectileVelocityManager>();
				if (component)
				{
					component.DesiredVelocity = this.DesiredVelocity.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007E42 RID: 32322
		public FsmOwnerDefault Target;

		// Token: 0x04007E43 RID: 32323
		public FsmVector2 DesiredVelocity;

		// Token: 0x04007E44 RID: 32324
		private ProjectileVelocityManager manager;
	}
}
