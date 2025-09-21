using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AF RID: 4783
	[ActionCategory("Hollow Knight")]
	public class InstaDeath : FsmStateAction
	{
		// Token: 0x06007D56 RID: 32086 RVA: 0x0025618B File Offset: 0x0025438B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06007D57 RID: 32087 RVA: 0x00256198 File Offset: 0x00254398
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					if (!component.isDead)
					{
						float value = this.direction.IsNone ? DirectionUtils.GetAngle(component.GetAttackDirection()) : this.direction.Value;
						component.Die(new float?(value), AttackTypes.Generic, NailElements.None, null, false, 1f, true, false);
					}
				}
				else
				{
					safe.GetComponent<EnemyDeathEffects>().ReceiveDeathEvent(new float?(DirectionUtils.GetAngle(1)), AttackTypes.Generic, 0f, false);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D54 RID: 32084
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D55 RID: 32085
		public FsmFloat direction;
	}
}
