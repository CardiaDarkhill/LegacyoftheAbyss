using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFA RID: 2810
	[ActionCategory("Hollow Knight")]
	public class SetDamageEnemyDirectionV2 : FsmStateAction
	{
		// Token: 0x06005909 RID: 22793 RVA: 0x001C3BEB File Offset: 0x001C1DEB
		public override void Reset()
		{
			this.Target = new FsmOwnerDefault();
			this.Direction = null;
			this.UseChildren = null;
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x001C3C08 File Offset: 0x001C1E08
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				if (!this.UseChildren.Value)
				{
					DamageEnemies component = safe.GetComponent<DamageEnemies>();
					if (component != null)
					{
						this.SetDirection(component);
					}
				}
				else
				{
					foreach (DamageEnemies direction in safe.GetComponentsInChildren<DamageEnemies>(true))
					{
						this.SetDirection(direction);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x001C3C7A File Offset: 0x001C1E7A
		private void SetDirection(DamageEnemies damager)
		{
			if (!this.Direction.IsNone)
			{
				damager.direction = this.Direction.Value;
			}
		}

		// Token: 0x04005429 RID: 21545
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault Target;

		// Token: 0x0400542A RID: 21546
		public FsmFloat Direction;

		// Token: 0x0400542B RID: 21547
		public FsmBool UseChildren;
	}
}
