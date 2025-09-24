using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF6 RID: 2806
	[ActionCategory("Hollow Knight")]
	public class GetDamagerDirection : FsmStateAction
	{
		// Token: 0x060058FD RID: 22781 RVA: 0x001C39D8 File Offset: 0x001C1BD8
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeDirection = new FsmFloat();
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x001C39F0 File Offset: 0x001C1BF0
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
				if (component != null)
				{
					this.storeDirection.Value = component.GetDirection();
				}
			}
			base.Finish();
		}

		// Token: 0x04005421 RID: 21537
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04005422 RID: 21538
		public FsmFloat storeDirection;
	}
}
