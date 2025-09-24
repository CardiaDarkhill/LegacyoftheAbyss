using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012B0 RID: 4784
	[ActionCategory("Hollow Knight")]
	public class SetHP : FsmStateAction
	{
		// Token: 0x06007D59 RID: 32089 RVA: 0x0025623D File Offset: 0x0025443D
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.hp = new FsmInt();
		}

		// Token: 0x06007D5A RID: 32090 RVA: 0x00256258 File Offset: 0x00254458
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !this.hp.IsNone)
				{
					component.hp = this.hp.Value;
					if (this.hp.Value > 0)
					{
						component.isDead = false;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D56 RID: 32086
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D57 RID: 32087
		public FsmInt hp;
	}
}
