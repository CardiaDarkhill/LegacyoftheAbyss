using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AB RID: 4779
	[ActionCategory("Hollow Knight")]
	public class SetInvincibleDelay : FsmStateAction
	{
		// Token: 0x06007D45 RID: 32069 RVA: 0x00255DC9 File Offset: 0x00253FC9
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.Invincible = null;
			this.Delay = null;
			this.InvincibleFromDirection = null;
			this.resetOnStateExit = false;
		}

		// Token: 0x06007D46 RID: 32070 RVA: 0x00255DF2 File Offset: 0x00253FF2
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06007D47 RID: 32071 RVA: 0x00255DFF File Offset: 0x00253FFF
		public override void OnUpdate()
		{
			if (this.timer < this.Delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoSetInvincible();
			base.Finish();
		}

		// Token: 0x06007D48 RID: 32072 RVA: 0x00255E34 File Offset: 0x00254034
		private void DoSetInvincible()
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

		// Token: 0x06007D49 RID: 32073 RVA: 0x00255EAC File Offset: 0x002540AC
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

		// Token: 0x04007D43 RID: 32067
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D44 RID: 32068
		public FsmBool Invincible;

		// Token: 0x04007D45 RID: 32069
		public FsmInt InvincibleFromDirection;

		// Token: 0x04007D46 RID: 32070
		public FsmFloat Delay;

		// Token: 0x04007D47 RID: 32071
		public bool resetOnStateExit;

		// Token: 0x04007D48 RID: 32072
		private float timer;
	}
}
