using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001245 RID: 4677
	public class DamageHeroDirectly : FsmStateAction
	{
		// Token: 0x06007BAF RID: 31663 RVA: 0x00250368 File Offset: 0x0024E568
		public override void Reset()
		{
			this.damager = null;
			this.damageAmount = null;
			this.spikeHazard = false;
		}

		// Token: 0x06007BB0 RID: 31664 RVA: 0x00250380 File Offset: 0x0024E580
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.damager);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			HeroController.instance.CancelDownspikeInvulnerability();
			PlayerData.instance.isInvincible = false;
			HeroController.instance.cState.parrying = false;
			HazardType hazardType = HazardType.ENEMY;
			if (this.spikeHazard)
			{
				hazardType = HazardType.SPIKES;
			}
			else if (this.sinkHazard)
			{
				hazardType = HazardType.SINK;
			}
			if (ownerDefaultTarget.transform.position.x > HeroController.instance.gameObject.transform.position.x)
			{
				HeroController.instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
			}
			else
			{
				HeroController.instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
			}
			base.Finish();
		}

		// Token: 0x04007BEB RID: 31723
		public FsmOwnerDefault damager;

		// Token: 0x04007BEC RID: 31724
		public FsmInt damageAmount;

		// Token: 0x04007BED RID: 31725
		public bool spikeHazard;

		// Token: 0x04007BEE RID: 31726
		public bool sinkHazard;
	}
}
