using System;
using UnityEngine;

// Token: 0x020004AE RID: 1198
public class ChainAttackForce : MonoBehaviour, IHitResponder
{
	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x06002B54 RID: 11092 RVA: 0x000BE0D4 File Offset: 0x000BC2D4
	public bool HitRecurseUpwards
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000BE0D8 File Offset: 0x000BC2D8
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble < this.nextAttackTime)
		{
			return IHitResponder.Response.None;
		}
		this.nextAttackTime = Time.timeAsDouble + (double)this.delay;
		if (!this.interaction)
		{
			this.interaction = base.GetComponentInParent<ChainInteraction>();
		}
		if (this.interaction)
		{
			this.interaction.ApplyCollisionForce(this.interaction.gameObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), this.force, damageInstance.Source.transform.position.y);
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x04002CA2 RID: 11426
	public float force = 5f;

	// Token: 0x04002CA3 RID: 11427
	public float delay = 0.25f;

	// Token: 0x04002CA4 RID: 11428
	private double nextAttackTime;

	// Token: 0x04002CA5 RID: 11429
	private ChainInteraction interaction;
}
