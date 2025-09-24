using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BF RID: 4799
	public class HeroStealEnemyCurrency : FsmStateAction
	{
		// Token: 0x06007D87 RID: 32135 RVA: 0x00256A2F File Offset: 0x00254C2F
		public override void Reset()
		{
			this.Enemy = null;
		}

		// Token: 0x06007D88 RID: 32136 RVA: 0x00256A38 File Offset: 0x00254C38
		public override void OnEnter()
		{
			GameObject safe = this.Enemy.GetSafe(this);
			if (safe)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component)
				{
					component.DoStealHit();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D79 RID: 32121
		public FsmOwnerDefault Enemy;
	}
}
