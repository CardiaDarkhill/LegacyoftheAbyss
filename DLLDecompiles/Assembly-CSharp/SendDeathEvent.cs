using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002CC RID: 716
[ActionCategory("Hollow Knight")]
public class SendDeathEvent : FsmStateAction
{
	// Token: 0x060019AD RID: 6573 RVA: 0x000760E8 File Offset: 0x000742E8
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.attackDirection = new FsmFloat
		{
			UseVariable = true
		};
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x00076108 File Offset: 0x00074308
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.ReceiveDeathEvent(new float?(this.attackDirection.Value), AttackTypes.Generic, false);
			}
		}
		base.Finish();
	}

	// Token: 0x040018AC RID: 6316
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040018AD RID: 6317
	public FsmFloat attackDirection;
}
