using System;
using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001264 RID: 4708
	public sealed class DamageHeroDirectlyV2 : FsmStateAction
	{
		// Token: 0x06007C40 RID: 31808 RVA: 0x0025262E File Offset: 0x0025082E
		public override void Reset()
		{
			this.damager = null;
			this.damageAmount = null;
			this.hazardType = null;
			this.damageDirection = null;
			this.invertDirection = null;
			this.overrideDirection = new FsmEnum
			{
				UseVariable = true
			};
		}

		// Token: 0x06007C41 RID: 31809 RVA: 0x00252668 File Offset: 0x00250868
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.damager);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			HeroController instance = HeroController.instance;
			instance.CancelDownspikeInvulnerability();
			PlayerData.instance.isInvincible = false;
			instance.cState.parrying = false;
			HazardType hazardType = (HazardType)this.hazardType.Value;
			if (this.overrideDirection.IsNone)
			{
				DamageHeroDirectlyV2.DamageDirection damageDirection = (DamageHeroDirectlyV2.DamageDirection)this.damageDirection.Value;
				if (damageDirection != DamageHeroDirectlyV2.DamageDirection.FromDamageSource)
				{
					if (damageDirection == DamageHeroDirectlyV2.DamageDirection.FromHeroFacingDirection)
					{
						if (this.invertDirection.Value)
						{
							if (instance.transform.localScale.x > 0f)
							{
								instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
							}
							else
							{
								instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
							}
						}
						else if (instance.transform.localScale.x < 0f)
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
						}
						else
						{
							instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
						}
					}
				}
				else if (this.invertDirection.Value)
				{
					if (ownerDefaultTarget.transform.position.x < instance.gameObject.transform.position.x)
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
					}
					else
					{
						instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
					}
				}
				else if (ownerDefaultTarget.transform.position.x > instance.gameObject.transform.position.x)
				{
					instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.right, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
				}
				else
				{
					instance.TakeDamage(ownerDefaultTarget.gameObject, CollisionSide.left, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
				}
			}
			else
			{
				instance.TakeDamage(ownerDefaultTarget.gameObject, (CollisionSide)this.overrideDirection.Value, this.damageAmount.Value, hazardType, DamagePropertyFlags.None);
			}
			base.Finish();
		}

		// Token: 0x04007C4F RID: 31823
		public FsmOwnerDefault damager;

		// Token: 0x04007C50 RID: 31824
		public FsmInt damageAmount;

		// Token: 0x04007C51 RID: 31825
		[ObjectType(typeof(HazardType))]
		public FsmEnum hazardType;

		// Token: 0x04007C52 RID: 31826
		[ObjectType(typeof(DamageHeroDirectlyV2.DamageDirection))]
		public FsmEnum damageDirection;

		// Token: 0x04007C53 RID: 31827
		public FsmBool invertDirection;

		// Token: 0x04007C54 RID: 31828
		[ObjectType(typeof(CollisionSide))]
		public FsmEnum overrideDirection;

		// Token: 0x02001BE3 RID: 7139
		public enum DamageDirection
		{
			// Token: 0x04009F77 RID: 40823
			FromDamageSource,
			// Token: 0x04009F78 RID: 40824
			FromHeroFacingDirection
		}
	}
}
