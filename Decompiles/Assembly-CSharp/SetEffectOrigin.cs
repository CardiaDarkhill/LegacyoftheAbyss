using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002CE RID: 718
[ActionCategory("Hollow Knight")]
public class SetEffectOrigin : FsmStateAction
{
	// Token: 0x060019B3 RID: 6579 RVA: 0x000761FF File Offset: 0x000743FF
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.effectOrigin = new FsmVector3();
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x00076218 File Offset: 0x00074418
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.effectOrigin = this.effectOrigin.Value;
			}
		}
		base.Finish();
	}

	// Token: 0x040018B0 RID: 6320
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040018B1 RID: 6321
	public FsmVector3 effectOrigin;
}
