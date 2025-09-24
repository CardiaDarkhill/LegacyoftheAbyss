using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class Downspike : NailAttackBase
{
	// Token: 0x06000C91 RID: 3217 RVA: 0x00037ECC File Offset: 0x000360CC
	protected override void Awake()
	{
		base.Awake();
		Transform root = base.transform.root;
		try
		{
			this.heroCtrl = root.GetComponent<HeroController>();
		}
		catch (NullReferenceException ex)
		{
			string str = "NailSlash: could not find HeroController on parent: ";
			string name = root.name;
			string str2 = " ";
			NullReferenceException ex2 = ex;
			Debug.LogError(str + name + str2 + ((ex2 != null) ? ex2.ToString() : null));
		}
		this.audio = base.GetComponent<AudioSource>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.poly = base.GetComponent<PolygonCollider2D>();
		this.mesh = base.GetComponent<MeshRenderer>();
		this.poly.enabled = false;
		this.mesh.enabled = false;
		this.horizontalKnockbackDamager.gameObject.SetActive(false);
		this.verticalKnockbackDamager.gameObject.SetActive(false);
		if (this.enemyDamager)
		{
			this.enemyDamager.WillDamageEnemyCollider += this.OnDamagingEnemy;
			this.enemyDamager.WillDamageEnemy += this.OnEnemyDamaged;
			this.enemyDamager.ParriedEnemy += this.OnEnemyParried;
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00037FF0 File Offset: 0x000361F0
	public void StartSlash()
	{
		this.enemyDamager.ExtraUpDirection = new float?((this.heroCtrl.transform.localScale.x < 0f) ? this.rightExtraDirection : this.leftExtraDirection);
		this.OnSlashStarting();
		this.audio.Play();
		base.PlayVibration();
		this.anim.PlayFromFrame(this.animName, 0);
		this.anim.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered);
		this.mesh.enabled = true;
		if (this.clashTinkPoly)
		{
			this.clashTinkPoly.enabled = true;
		}
		base.CanDamageEnemies = true;
		base.IsDamagerActive = true;
		this.queuedBounce = false;
		this.bounceTriggerHit = false;
		this.heroBox.HeroBoxDownspike();
		base.OnPlaySlash();
		this.poly.enabled = true;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x000380D8 File Offset: 0x000362D8
	public void CancelAttack()
	{
		this.poly.enabled = false;
		this.mesh.enabled = false;
		if (this.clashTinkPoly)
		{
			this.clashTinkPoly.enabled = false;
		}
		this.heroBox.HeroBoxNormal();
		base.OnCancelAttack();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00038127 File Offset: 0x00036327
	private void OnDamagingEnemy(Collider2D col)
	{
		if (this.enemyDamager.damageDealt <= 0 && !this.enemyDamager.useNailDamage)
		{
			return;
		}
		this.horizontalKnockbackDamager.PreventDamage(col);
		this.verticalKnockbackDamager.PreventDamage(col);
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x00038160 File Offset: 0x00036360
	private void OnEnemyDamaged()
	{
		if (this.enemyDamager.damageDealt <= 0 && !this.enemyDamager.useNailDamage)
		{
			return;
		}
		if (this.useKnockbackDamagers)
		{
			this.currentKnockbackDamagerSteps = this.knockbackDamagerActiveSteps;
			this.horizontalKnockbackDamager.direction = (float)((this.heroCtrl.transform.localScale.x > 0f) ? 180 : 0);
			this.horizontalKnockbackDamager.gameObject.SetActive(true);
			this.verticalKnockbackDamager.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000381EF File Offset: 0x000363EF
	private void OnEnemyParried()
	{
		base.CanDamageEnemies = false;
		this.horizontalKnockbackDamager.gameObject.SetActive(false);
		this.verticalKnockbackDamager.gameObject.SetActive(false);
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0003821A File Offset: 0x0003641A
	protected override void OnAttackCancelled()
	{
		this.horizontalKnockbackDamager.gameObject.SetActive(false);
		this.verticalKnockbackDamager.gameObject.SetActive(false);
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00038240 File Offset: 0x00036440
	private void FixedUpdate()
	{
		if (this.currentKnockbackDamagerSteps > 0)
		{
			this.currentKnockbackDamagerSteps--;
			if (this.currentKnockbackDamagerSteps <= 0)
			{
				this.horizontalKnockbackDamager.gameObject.SetActive(false);
				this.verticalKnockbackDamager.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0003828F File Offset: 0x0003648F
	private void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
	{
		base.IsDamagerActive = false;
		this.bounceTriggerHit = true;
		this.TryDownBounce();
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x000382A5 File Offset: 0x000364A5
	private void TryDownBounce()
	{
		if (!this.queuedBounce)
		{
			return;
		}
		if (this.waitForBounceTrigger && !this.bounceTriggerHit)
		{
			return;
		}
		if (!this.heroCtrl.CanCustomRecoil())
		{
			return;
		}
		this.heroCtrl.DownspikeBounce(false, null);
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x000382DC File Offset: 0x000364DC
	public override void QueueBounce()
	{
		base.QueueBounce();
		this.queuedBounce = true;
		this.TryDownBounce();
	}

	// Token: 0x04000C13 RID: 3091
	[SerializeField]
	private string animName;

	// Token: 0x04000C14 RID: 3092
	[SerializeField]
	private float leftExtraDirection;

	// Token: 0x04000C15 RID: 3093
	[SerializeField]
	private float rightExtraDirection;

	// Token: 0x04000C16 RID: 3094
	[SerializeField]
	private bool waitForBounceTrigger;

	// Token: 0x04000C17 RID: 3095
	[Space]
	[SerializeField]
	private bool useKnockbackDamagers;

	// Token: 0x04000C18 RID: 3096
	[SerializeField]
	private DamageEnemies horizontalKnockbackDamager;

	// Token: 0x04000C19 RID: 3097
	[SerializeField]
	private DamageEnemies verticalKnockbackDamager;

	// Token: 0x04000C1A RID: 3098
	[SerializeField]
	private int knockbackDamagerActiveSteps = 3;

	// Token: 0x04000C1B RID: 3099
	private int currentKnockbackDamagerSteps;

	// Token: 0x04000C1C RID: 3100
	[SerializeField]
	private HeroBox heroBox;

	// Token: 0x04000C1D RID: 3101
	private HeroController heroCtrl;

	// Token: 0x04000C1E RID: 3102
	private tk2dSpriteAnimator anim;

	// Token: 0x04000C1F RID: 3103
	private MeshRenderer mesh;

	// Token: 0x04000C20 RID: 3104
	private float slashAngle;

	// Token: 0x04000C21 RID: 3105
	private bool queuedBounce;

	// Token: 0x04000C22 RID: 3106
	private bool bounceTriggerHit;

	// Token: 0x04000C23 RID: 3107
	private PolygonCollider2D poly;

	// Token: 0x04000C24 RID: 3108
	private int polyCounter;

	// Token: 0x04000C25 RID: 3109
	private AudioSource audio;
}
