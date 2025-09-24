using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012BE RID: 4798
	[ActionCategory("Hollow Knight")]
	public class HasTakenDamage : FsmStateAction
	{
		// Token: 0x06007D84 RID: 32132 RVA: 0x00256994 File Offset: 0x00254B94
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.takenDamageBool = new FsmBool();
			this.takenDamageEvent = null;
		}

		// Token: 0x06007D85 RID: 32133 RVA: 0x002569B4 File Offset: 0x00254BB4
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					bool flag = component.HasTakenDamage();
					if (!this.takenDamageBool.IsNone)
					{
						this.takenDamageBool.Value = flag;
					}
					if (flag)
					{
						base.Fsm.Event(this.eventTarget, this.takenDamageEvent);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D75 RID: 32117
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D76 RID: 32118
		public FsmBool takenDamageBool;

		// Token: 0x04007D77 RID: 32119
		public FsmEventTarget eventTarget;

		// Token: 0x04007D78 RID: 32120
		public FsmEvent takenDamageEvent;
	}
}
