using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFC RID: 2812
	public class GetDamageEnemyAttackType : FsmStateAction
	{
		// Token: 0x06005913 RID: 22803 RVA: 0x001C3D92 File Offset: 0x001C1F92
		public override void Reset()
		{
			this.Target = null;
			this.StoreAttackType = null;
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x001C3DA4 File Offset: 0x001C1FA4
		public override void OnEnter()
		{
			this.StoreAttackType.Value = AttackTypes.Generic;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component)
				{
					this.StoreAttackType.Value = component.attackType;
				}
			}
			base.Finish();
		}

		// Token: 0x04005430 RID: 21552
		public FsmOwnerDefault Target;

		// Token: 0x04005431 RID: 21553
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(AttackTypes))]
		public FsmEnum StoreAttackType;
	}
}
