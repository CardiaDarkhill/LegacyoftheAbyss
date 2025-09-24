using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002D0 RID: 720
[ActionCategory("Hollow Knight")]
public class PreSpawnCorpse : FsmStateAction
{
	// Token: 0x060019B9 RID: 6585 RVA: 0x00076307 File Offset: 0x00074507
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x00076314 File Offset: 0x00074514
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.PreInstantiate();
			}
		}
		base.Finish();
	}

	// Token: 0x040018B4 RID: 6324
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;
}
