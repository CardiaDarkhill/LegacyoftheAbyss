using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002CD RID: 717
[ActionCategory("Hollow Knight")]
public class SetCorpsePrefab : FsmStateAction
{
	// Token: 0x060019B0 RID: 6576 RVA: 0x0007617A File Offset: 0x0007437A
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.corpsePrefab = new FsmGameObject();
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x00076194 File Offset: 0x00074394
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.CorpsePrefab = this.corpsePrefab.Value;
			}
		}
		base.Finish();
	}

	// Token: 0x040018AE RID: 6318
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040018AF RID: 6319
	public FsmGameObject corpsePrefab;
}
