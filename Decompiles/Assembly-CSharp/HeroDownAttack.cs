using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class HeroDownAttack : MonoBehaviour
{
	// Token: 0x06000F87 RID: 3975 RVA: 0x0004AD90 File Offset: 0x00048F90
	private void Awake()
	{
		this.hc = HeroController.instance;
		this.attack = base.GetComponent<NailAttackBase>();
		this.attack.AttackStarting += this.OnAttackStarting;
		this.attack.EndedDamage += this.OnEndedDamage;
		this.attack.EnemyDamager.HitResponded += this.OnHitResponded;
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0004AE00 File Offset: 0x00049000
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (!this.attack)
		{
			return;
		}
		if (!this.attack.IsDamagerActive)
		{
			return;
		}
		if (!otherCollider)
		{
			return;
		}
		GameObject gameObject = otherCollider.gameObject;
		PhysLayers layer = (PhysLayers)gameObject.layer;
		bool flag = this.attack.EnemyDamager.manualTrigger && layer == PhysLayers.ENEMIES;
		if (flag && !this.attack.CanDamageEnemies)
		{
			return;
		}
		if ((flag || layer == PhysLayers.INTERACTIVE_OBJECT || layer == PhysLayers.TERRAIN || gameObject.GetComponent<TinkEffect>()) && this.attack.EnemyDamager.DoDamage(otherCollider, true))
		{
			return;
		}
		if (!flag && layer != PhysLayers.INTERACTIVE_OBJECT && layer != PhysLayers.HERO_ATTACK)
		{
			return;
		}
		this.lastCollider = otherCollider;
		this.ContinueBounceTrigger(otherCollider.gameObject);
		this.lastCollider = null;
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0004AEC4 File Offset: 0x000490C4
	private static bool IsNonBounce(GameObject obj)
	{
		NonBouncer component = obj.GetComponent<NonBouncer>();
		return (component && component.active) || obj.GetComponent<BounceBalloon>();
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0004AEFC File Offset: 0x000490FC
	private void ContinueBounceTrigger(GameObject otherObj)
	{
		if (HeroDownAttack.IsNonBounce(otherObj))
		{
			return;
		}
		DamageHero component = otherObj.GetComponent<DamageHero>();
		if (component)
		{
			if (!this.attack.CanHitSpikes && component.hazardType == HazardType.SPIKES)
			{
				return;
			}
			if (component.hazardType <= HazardType.ENEMY && !component.noBounceCooldown && !component.GetComponent<HealthManager>())
			{
				component.SetCooldown(0.5f);
			}
			if (this.attack.EnemyDamager)
			{
				if (this.lastCollider != null)
				{
					this.attack.EnemyDamager.TryDoDamage(this.lastCollider, true);
				}
				else
				{
					this.attack.EnemyDamager.DoDamage(component.gameObject, true);
				}
			}
		}
		if (!this.hc.CanCustomRecoil())
		{
			return;
		}
		this.bounceQueued = true;
		this.attack.QueueBounce();
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0004AFD6 File Offset: 0x000491D6
	private void OnAttackStarting()
	{
		this.bounceQueued = false;
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x0004AFE0 File Offset: 0x000491E0
	private void OnEndedDamage(bool didHit)
	{
		if (!didHit)
		{
			return;
		}
		if (this.bounceQueued)
		{
			this.bounceQueued = false;
			NailSlash nailSlash = this.attack as NailSlash;
			if (nailSlash == null)
			{
				return;
			}
			this.hc.AffectedByGravity(true);
			nailSlash.DoDownspikeBounce();
		}
	}

	// Token: 0x06000F8D RID: 3981 RVA: 0x0004B028 File Offset: 0x00049228
	private void OnHitResponded(DamageEnemies.HitResponse hitResponse)
	{
		if (!this.attack.IsDamagerActive)
		{
			return;
		}
		PhysLayers layer = (PhysLayers)hitResponse.Target.layer;
		if (layer == PhysLayers.TERRAIN || layer == PhysLayers.SOFT_TERRAIN)
		{
			return;
		}
		if (HeroDownAttack.IsNonBounce(hitResponse.Target))
		{
			return;
		}
		this.hc.StartDownspikeInvulnerability();
		if (this.hc.CanCustomRecoil())
		{
			this.hc.AffectedByGravity(false);
			this.hc.ResetVelocity();
		}
		this.ContinueBounceTrigger(hitResponse.Target);
	}

	// Token: 0x04000F21 RID: 3873
	private HeroController hc;

	// Token: 0x04000F22 RID: 3874
	private NailAttackBase attack;

	// Token: 0x04000F23 RID: 3875
	private bool bounceQueued;

	// Token: 0x04000F24 RID: 3876
	private Collider2D lastCollider;
}
