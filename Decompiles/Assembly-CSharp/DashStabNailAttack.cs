using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class DashStabNailAttack : NailAttackBase
{
	// Token: 0x06000C88 RID: 3208 RVA: 0x00037D30 File Offset: 0x00035F30
	protected override void Awake()
	{
		base.Awake();
		this.heroController = base.GetComponentInParent<HeroController>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		if (this.waitForBounceTrigger)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		}
		this.damager = base.GetComponent<DamageEnemies>();
		if (this.waitForEndDamage)
		{
			this.damager.EndedDamage += this.OnHitTrigger;
		}
		this.damager.WillDamageEnemyOptions += this.OnWillDamageEnemy;
		this.damager.WillDamageEnemyCollider += this.OnWillDamageEnemyCollider;
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00037DE3 File Offset: 0x00035FE3
	private void OnWillDamageEnemy(HealthManager healthManager, HitInstance hitInstance)
	{
		if (healthManager.GetComponent<NonBouncer>() && healthManager.GetComponent<NonBouncer>().active)
		{
			return;
		}
		this.DoRecoilHit();
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x00037E06 File Offset: 0x00036006
	private void OnWillDamageEnemyCollider(Collider2D otherCollider)
	{
		if (otherCollider.CompareTag("Recoiler") && otherCollider.gameObject.GetComponent<IsCoralCrustWall>())
		{
			this.DoRecoilHit();
		}
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00037E2D File Offset: 0x0003602D
	private void DoRecoilHit()
	{
		if (this.doEnemyHitRecoil)
		{
			this.heroController.sprintFSM.SendEventSafe("DASH RECOIL");
		}
		this.DoHitResponse();
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00037E52 File Offset: 0x00036052
	public override void OnSlashStarting()
	{
		base.OnSlashStarting();
		if (this.waitForBounceTrigger || this.waitForEndDamage)
		{
			this.isWaitingForTrigger = true;
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00037E71 File Offset: 0x00036071
	private void OnAnimationEventTriggered(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		this.OnHitTrigger(true);
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00037E7A File Offset: 0x0003607A
	private void DoHitResponse()
	{
		if (this.isWaitingForTrigger)
		{
			this.isHitQueued = true;
			return;
		}
		this.isHitQueued = false;
		this.heroController.sprintFSM.SendEventSafe("DASH HIT");
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00037EA8 File Offset: 0x000360A8
	private void OnHitTrigger(bool didHit)
	{
		if (!didHit)
		{
			return;
		}
		this.isWaitingForTrigger = false;
		if (this.isHitQueued)
		{
			this.DoHitResponse();
		}
	}

	// Token: 0x04000C0B RID: 3083
	[Space]
	[SerializeField]
	private bool waitForBounceTrigger;

	// Token: 0x04000C0C RID: 3084
	[SerializeField]
	private bool waitForEndDamage;

	// Token: 0x04000C0D RID: 3085
	[SerializeField]
	private bool doEnemyHitRecoil;

	// Token: 0x04000C0E RID: 3086
	private bool isWaitingForTrigger;

	// Token: 0x04000C0F RID: 3087
	private bool isHitQueued;

	// Token: 0x04000C10 RID: 3088
	private tk2dSpriteAnimator animator;

	// Token: 0x04000C11 RID: 3089
	private HeroController heroController;

	// Token: 0x04000C12 RID: 3090
	private DamageEnemies damager;
}
