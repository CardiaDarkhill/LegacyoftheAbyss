using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002D1 RID: 721
[ActionCategory("Hollow Knight")]
public class DoNotSpawnCorpse : FsmStateAction
{
	// Token: 0x060019BC RID: 6588 RVA: 0x00076374 File Offset: 0x00074574
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x00076384 File Offset: 0x00074584
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.doNotSpawnCorpse = true;
			}
		}
		base.Finish();
	}

	// Token: 0x040018B5 RID: 6325
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;
}
