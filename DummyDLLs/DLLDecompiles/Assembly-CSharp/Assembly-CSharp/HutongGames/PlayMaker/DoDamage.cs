using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF7 RID: 2807
	[ActionCategory("Hollow Knight")]
	public class DoDamage : FsmStateAction
	{
		// Token: 0x06005900 RID: 22784 RVA: 0x001C3A5B File Offset: 0x001C1C5B
		public override void Reset()
		{
			this.damager = new FsmOwnerDefault();
			this.target = new FsmGameObject();
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x001C3A74 File Offset: 0x001C1C74
		public override void OnEnter()
		{
			GameObject gameObject = (this.damager.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.damager.GameObject.Value;
			if (this.target.Value == null)
			{
				base.Finish();
				return;
			}
			if (gameObject != null)
			{
				DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
				if (component != null)
				{
					component.DoDamage(this.target.Value, true);
					component.ForceUpdate();
				}
			}
			base.Finish();
		}

		// Token: 0x04005423 RID: 21539
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault damager;

		// Token: 0x04005424 RID: 21540
		public FsmGameObject target;
	}
}
