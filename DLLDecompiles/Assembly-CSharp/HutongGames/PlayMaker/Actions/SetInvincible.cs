using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AA RID: 4778
	[ActionCategory("Hollow Knight")]
	public class SetInvincible : FsmStateAction
	{
		// Token: 0x06007D41 RID: 32065 RVA: 0x00255CBF File Offset: 0x00253EBF
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.Invincible = null;
			this.InvincibleFromDirection = null;
			this.resetOnStateExit = false;
		}

		// Token: 0x06007D42 RID: 32066 RVA: 0x00255CE4 File Offset: 0x00253EE4
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!this.Invincible.IsNone)
					{
						component.IsInvincible = this.Invincible.Value;
					}
					if (!this.InvincibleFromDirection.IsNone)
					{
						component.InvincibleFromDirection = this.InvincibleFromDirection.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06007D43 RID: 32067 RVA: 0x00255D5C File Offset: 0x00253F5C
		public override void OnExit()
		{
			if (this.resetOnStateExit)
			{
				HealthManager component = this.target.GetSafe(this).GetComponent<HealthManager>();
				if (component != null)
				{
					if (!this.Invincible.IsNone)
					{
						component.IsInvincible = !this.Invincible.Value;
					}
					if (!this.InvincibleFromDirection.IsNone)
					{
						component.InvincibleFromDirection = 0;
					}
				}
			}
		}

		// Token: 0x04007D3F RID: 32063
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D40 RID: 32064
		public FsmBool Invincible;

		// Token: 0x04007D41 RID: 32065
		public FsmInt InvincibleFromDirection;

		// Token: 0x04007D42 RID: 32066
		public bool resetOnStateExit;
	}
}
