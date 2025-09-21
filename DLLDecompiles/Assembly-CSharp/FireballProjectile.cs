using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

// Token: 0x020002E8 RID: 744
[RequireComponent(typeof(DamageHero))]
public class FireballProjectile : MonoBehaviour, IHitResponder
{
	// Token: 0x06001A48 RID: 6728 RVA: 0x00078DE5 File Offset: 0x00076FE5
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector2.down * this.GroundedDistance);
	}

	// Token: 0x06001A49 RID: 6729 RVA: 0x00078E24 File Offset: 0x00077024
	private void Awake()
	{
		this.heroDamager = base.GetComponent<DamageHero>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
		this.damageEnemies = base.GetComponent<DamageEnemies>();
		this.spinSelf = base.GetComponent<SpinSelf>();
		this.renderer = base.GetComponent<Renderer>();
		this.bouncer = base.GetComponent<ObjectBounce>();
		this.tinkEffect = base.GetComponent<TinkEffect>();
		this.childParticles = base.GetComponentsInChildren<ParticleSystem>(true);
		this.jitterSelf = base.GetComponent<JitterSelf>();
		if (!this.jitterSelf)
		{
			this.jitterSelf = JitterSelf.Add(base.gameObject, new JitterSelfConfig
			{
				AmountMin = this.ExplodeAnticJitter,
				AmountMax = this.ExplodeAnticJitter,
				UseCameraRenderHooks = true
			}, CameraRenderHooks.CameraSource.MainCamera);
		}
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x00078F00 File Offset: 0x00077100
	private void Start()
	{
		if (this.heroDamager)
		{
			this.heroDamager.HeroDamaged += this.OnDamagedPlayer;
		}
		if (this.damageEnemies)
		{
			this.damageEnemies.DamagedEnemy += this.OnDamagedEnemy;
		}
		if (this.bouncer)
		{
			this.bouncer.Bounced += this.OnBounce;
		}
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x00078F7C File Offset: 0x0007717C
	private void OnDestroy()
	{
		if (this.heroDamager)
		{
			this.heroDamager.HeroDamaged -= this.OnDamagedPlayer;
		}
		if (this.damageEnemies)
		{
			this.damageEnemies.DamagedEnemy -= this.OnDamagedEnemy;
		}
		if (this.bouncer)
		{
			this.bouncer.Bounced -= this.OnBounce;
		}
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x00078FF8 File Offset: 0x000771F8
	private void OnEnable()
	{
		this.SetActive(true);
		this.terrainBouncesLeft = this.TerrainBounces;
		this.idleTimeLeft = this.IdleLifeTime;
		this.wasHit = false;
		if (this.spinSelf)
		{
			this.spinSelf.enabled = true;
		}
		this.isInExplodeAntic = false;
		if (this.body)
		{
			this.body.isKinematic = false;
		}
		if (this.jitterSelf)
		{
			this.jitterSelf.StopJitter();
		}
		if (this.collider)
		{
			this.collider.enabled = true;
		}
		if (this.tinkEffect)
		{
			this.tinkEffect.enabled = true;
		}
		if (this.ActiveBeforeExplosion)
		{
			this.ActiveBeforeExplosion.SetActive(true);
		}
		if (this.damageEnemies && this.enemyDamageAmount == 0)
		{
			this.enemyDamageAmount = this.damageEnemies.damageDealt;
		}
		if (this.heroDamager)
		{
			if (this.heroDamageAmount == 0)
			{
				this.heroDamageAmount = this.heroDamager.damageDealt;
			}
			else
			{
				this.heroDamager.damageDealt = this.heroDamageAmount;
			}
		}
		this.ExplosionChild.SetActive(false);
		if (this.loopSource)
		{
			this.loopSource.clip = this.loopClip;
			this.loopSource.Play();
		}
		this.DisableEnemyDamage();
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x00079161 File Offset: 0x00077361
	public void DisableEnemyDamage()
	{
		this.damageEnemies.damageDealt = 0;
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x00079170 File Offset: 0x00077370
	private void Update()
	{
		if (!this.isActive)
		{
			return;
		}
		float num = this.idleTimeLeft + this.ExplodeAnticTime;
		if (num <= 0f)
		{
			this.Break();
		}
		else if (num <= this.ExplodeAnticTime && !this.isInExplodeAntic)
		{
			this.isInExplodeAntic = true;
			ParticleSystem[] anticParticles = this.AnticParticles;
			for (int i = 0; i < anticParticles.Length; i++)
			{
				anticParticles[i].Play(true);
			}
			this.stopMovementRoutine = base.StartCoroutine(this.StopMovementOnGround());
			if (this.tinkEffect)
			{
				this.tinkEffect.enabled = false;
			}
			if (this.loopSource)
			{
				this.loopSource.clip = this.anticLoopClip;
				this.loopSource.Play();
			}
			if (this.jitterSelf)
			{
				this.jitterSelf.StartJitter();
			}
		}
		this.idleTimeLeft -= Time.deltaTime;
		if (this.CanHitBackPause > 0f)
		{
			this.CanHitBackPause -= Time.deltaTime;
		}
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x00079280 File Offset: 0x00077480
	private IEnumerator StopMovementOnGround()
	{
		yield return new WaitUntil(() => Helper.Raycast2D(base.transform.position, Vector2.down, this.GroundedDistance, 256).collider != null && (!this.body || this.body.linearVelocity.y <= 0f));
		if (this.body)
		{
			this.body.linearVelocity = Vector2.zero;
			this.body.angularVelocity = 0f;
		}
		yield break;
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x00079290 File Offset: 0x00077490
	private void FixedUpdate()
	{
		if (!this.isActive)
		{
			return;
		}
		if (!this.isInExplodeAntic)
		{
			if ((base.transform.position - this.previousPosition).magnitude < 0.01f)
			{
				this.stationarySteps++;
			}
			else
			{
				this.stationarySteps = 0;
			}
			this.previousPosition = base.transform.position;
			if (this.stationarySteps >= 5)
			{
				this.idleTimeLeft = 0f;
			}
		}
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x0007931C File Offset: 0x0007751C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.isActive || this.isInExplodeAntic)
		{
			return;
		}
		if (collision.gameObject.layer == 8)
		{
			if (this.terrainBouncesLeft <= 0)
			{
				this.idleTimeLeft = 0f;
			}
			else
			{
				this.idleTimeLeft = this.IdleLifeTime;
			}
			this.terrainBouncesLeft--;
			this.bounceSound.SpawnAndPlayOneShot(base.transform.position, null);
		}
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x00079390 File Offset: 0x00077590
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Lava"))
		{
			this.Break();
		}
		if (this.ExplosionRock)
		{
			PhysLayers layer = (PhysLayers)collision.gameObject.layer;
			if (layer != PhysLayers.PLAYER)
			{
				if (layer != PhysLayers.ENEMIES)
				{
					if (layer != PhysLayers.HERO_BOX)
					{
						return;
					}
				}
				else
				{
					if (this.wasHit)
					{
						this.Break();
						return;
					}
					return;
				}
			}
			this.Break();
			return;
		}
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000793ED File Offset: 0x000775ED
	private IEnumerator MonitorExplosion()
	{
		if (this.childParticles.Length == 0)
		{
			if (this.ExplosionChild)
			{
				yield return new WaitUntil(() => !this.ExplosionChild.activeInHierarchy);
			}
		}
		else
		{
			yield return new WaitUntil(delegate()
			{
				bool flag = false;
				ParticleSystem[] array = this.childParticles;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].IsAlive(true))
					{
						flag = true;
					}
				}
				return !flag;
			});
		}
		if (this.DoRecycle)
		{
			base.gameObject.Recycle();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000793FC File Offset: 0x000775FC
	private void SetActive(bool isActive)
	{
		this.isActive = isActive;
		if (this.spinSelf)
		{
			this.spinSelf.enabled = isActive;
		}
		if (this.body)
		{
			this.body.isKinematic = !isActive;
			this.body.angularVelocity = 0f;
			this.body.linearVelocity = Vector2.zero;
		}
		if (this.renderer)
		{
			this.renderer.enabled = isActive;
		}
		ParticleSystem[] trailParticles;
		if (isActive)
		{
			trailParticles = this.TrailParticles;
			for (int i = 0; i < trailParticles.Length; i++)
			{
				trailParticles[i].Play(true);
			}
			return;
		}
		trailParticles = this.TrailParticles;
		for (int i = 0; i < trailParticles.Length; i++)
		{
			trailParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (this.stopMovementRoutine != null)
		{
			base.StopCoroutine(this.stopMovementRoutine);
		}
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000794D4 File Offset: 0x000776D4
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!this.isActive || !this.body || this.CanHitBackPause > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage && damageInstance.AttackType != AttackTypes.Generic)
		{
			return IHitResponder.Response.None;
		}
		if (this.isInExplodeAntic && !this.ExplosionRock)
		{
			this.Break();
			return IHitResponder.Response.GenericHit;
		}
		float num = 0f;
		if (damageInstance.IsNailDamage)
		{
			switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
			{
			case HitInstance.HitDirection.Left:
				num = 180f - this.HitBackAngle;
				break;
			case HitInstance.HitDirection.Right:
				num = this.HitBackAngle;
				break;
			case HitInstance.HitDirection.Up:
				num = 90f;
				break;
			case HitInstance.HitDirection.Down:
				num = 270f;
				break;
			}
		}
		else
		{
			num = damageInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular);
		}
		float value = this.HitBackSpeed * Mathf.Cos(num * 0.017453292f);
		float value2 = this.HitBackSpeed * Mathf.Sin(num * 0.017453292f);
		this.body.SetVelocity(new float?(value), new float?(value2));
		if (!this.ExplosionRock)
		{
			this.terrainBouncesLeft = 0;
			this.idleTimeLeft = this.IdleLifeTime;
		}
		this.wasHit = true;
		this.damageEnemies.damageDealt = this.enemyDamageAmount;
		this.damageEnemies.AwardJournalKill = true;
		this.knockbackSound.SpawnAndPlayOneShot(base.transform.position, null);
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0007964C File Offset: 0x0007784C
	public void Break()
	{
		if (!this.isActive)
		{
			return;
		}
		this.SetActive(false);
		base.transform.SetRotation2D(0f);
		if (this.ExplosionChild)
		{
			this.ExplosionChild.SetActive(true);
			if (this.damageEnemies)
			{
				DamageEnemies componentInChildren = this.ExplosionChild.GetComponentInChildren<DamageEnemies>();
				if (componentInChildren)
				{
					componentInChildren.AwardJournalKill = this.damageEnemies.AwardJournalKill;
				}
			}
		}
		if (this.ActiveBeforeExplosion)
		{
			this.ActiveBeforeExplosion.SetActive(false);
		}
		if (this.damageEnemies)
		{
			this.damageEnemies.damageDealt = 0;
		}
		if (this.heroDamager)
		{
			this.heroDamager.damageDealt = 0;
		}
		if (this.body)
		{
			this.body.isKinematic = true;
		}
		if (this.collider)
		{
			this.collider.enabled = false;
		}
		base.StartCoroutine(this.MonitorExplosion());
		this.deathSound.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.loopSource)
		{
			this.loopSource.Stop();
		}
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x0007977E File Offset: 0x0007797E
	public void OnDamagedPlayer()
	{
		this.Break();
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x00079786 File Offset: 0x00077986
	public void OnDamagedEnemy()
	{
		if (this.wasHit)
		{
			this.Break();
		}
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x00079796 File Offset: 0x00077996
	public void OnBounce()
	{
		if (this.spinSelf)
		{
			this.spinSelf.enabled = false;
		}
	}

	// Token: 0x04001930 RID: 6448
	private const int MAX_STATIONARY_STEPS = 5;

	// Token: 0x04001931 RID: 6449
	private const float STEP_MOVEMENT_THRESHOLD = 0.01f;

	// Token: 0x04001932 RID: 6450
	public int TerrainBounces = 3;

	// Token: 0x04001933 RID: 6451
	private int terrainBouncesLeft;

	// Token: 0x04001934 RID: 6452
	public float IdleLifeTime = 2f;

	// Token: 0x04001935 RID: 6453
	private float idleTimeLeft;

	// Token: 0x04001936 RID: 6454
	public ParticleSystem[] TrailParticles;

	// Token: 0x04001937 RID: 6455
	public ParticleSystem[] AnticParticles;

	// Token: 0x04001938 RID: 6456
	[Space]
	public GameObject ActiveBeforeExplosion;

	// Token: 0x04001939 RID: 6457
	[Space]
	public GameObject ExplosionChild;

	// Token: 0x0400193A RID: 6458
	public float ExplodeAnticTime = 0.5f;

	// Token: 0x0400193B RID: 6459
	public Vector2 ExplodeAnticJitter = new Vector2(0.1f, 0.1f);

	// Token: 0x0400193C RID: 6460
	public float GroundedDistance = 1f;

	// Token: 0x0400193D RID: 6461
	public bool DoRecycle = true;

	// Token: 0x0400193E RID: 6462
	public bool ExplosionRock;

	// Token: 0x0400193F RID: 6463
	[Space]
	public float CanHitBackPause;

	// Token: 0x04001940 RID: 6464
	public float HitBackSpeed;

	// Token: 0x04001941 RID: 6465
	public float HitBackAngle;

	// Token: 0x04001942 RID: 6466
	[Space]
	[SerializeField]
	private AudioSource loopSource;

	// Token: 0x04001943 RID: 6467
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04001944 RID: 6468
	[SerializeField]
	private AudioClip anticLoopClip;

	// Token: 0x04001945 RID: 6469
	[SerializeField]
	private AudioEvent knockbackSound;

	// Token: 0x04001946 RID: 6470
	[SerializeField]
	private AudioEvent bounceSound;

	// Token: 0x04001947 RID: 6471
	[SerializeField]
	private AudioEvent deathSound;

	// Token: 0x04001948 RID: 6472
	private int heroDamageAmount;

	// Token: 0x04001949 RID: 6473
	private int enemyDamageAmount;

	// Token: 0x0400194A RID: 6474
	private ParticleSystem[] childParticles;

	// Token: 0x0400194B RID: 6475
	private bool isActive;

	// Token: 0x0400194C RID: 6476
	private bool wasHit;

	// Token: 0x0400194D RID: 6477
	private Vector2 previousPosition;

	// Token: 0x0400194E RID: 6478
	private int stationarySteps;

	// Token: 0x0400194F RID: 6479
	private bool isInExplodeAntic;

	// Token: 0x04001950 RID: 6480
	private Coroutine stopMovementRoutine;

	// Token: 0x04001951 RID: 6481
	private JitterSelf jitterSelf;

	// Token: 0x04001952 RID: 6482
	private DamageHero heroDamager;

	// Token: 0x04001953 RID: 6483
	private DamageEnemies damageEnemies;

	// Token: 0x04001954 RID: 6484
	private Rigidbody2D body;

	// Token: 0x04001955 RID: 6485
	private Collider2D collider;

	// Token: 0x04001956 RID: 6486
	private SpinSelf spinSelf;

	// Token: 0x04001957 RID: 6487
	private ObjectBounce bouncer;

	// Token: 0x04001958 RID: 6488
	private TinkEffect tinkEffect;

	// Token: 0x04001959 RID: 6489
	private Renderer renderer;
}
