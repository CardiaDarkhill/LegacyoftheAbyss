using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002CF RID: 719
[ActionCategory("Hollow Knight")]
public class SetCorpseSpawnPoint : FsmStateAction
{
	// Token: 0x060019B6 RID: 6582 RVA: 0x00076283 File Offset: 0x00074483
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
		this.spawnPoint = new FsmVector3();
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x0007629C File Offset: 0x0007449C
	public override void OnEnter()
	{
		GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
		if (gameObject != null)
		{
			EnemyDeathEffects component = gameObject.GetComponent<EnemyDeathEffects>();
			if (component != null)
			{
				component.corpseSpawnPoint = this.spawnPoint.Value;
			}
		}
		base.Finish();
	}

	// Token: 0x040018B2 RID: 6322
	[UIHint(UIHint.Variable)]
	public FsmOwnerDefault target;

	// Token: 0x040018B3 RID: 6323
	public FsmVector3 spawnPoint;
}
