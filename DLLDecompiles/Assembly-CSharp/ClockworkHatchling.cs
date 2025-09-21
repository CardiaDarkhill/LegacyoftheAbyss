using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class ClockworkHatchling : MonoBehaviour
{
	// Token: 0x17000274 RID: 628
	// (get) Token: 0x060017CB RID: 6091 RVA: 0x0006BDCA File Offset: 0x00069FCA
	// (set) Token: 0x060017CC RID: 6092 RVA: 0x0006BDD2 File Offset: 0x00069FD2
	public ClockworkHatchling.State LastFrameState { get; private set; }

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x060017CD RID: 6093 RVA: 0x0006BDDB File Offset: 0x00069FDB
	// (set) Token: 0x060017CE RID: 6094 RVA: 0x0006BDE3 File Offset: 0x00069FE3
	public ClockworkHatchling.State PreviousState { get; private set; }

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x060017CF RID: 6095 RVA: 0x0006BDEC File Offset: 0x00069FEC
	// (set) Token: 0x060017D0 RID: 6096 RVA: 0x0006BDF4 File Offset: 0x00069FF4
	public ClockworkHatchling.State CurrentState
	{
		get
		{
			return this.currentState;
		}
		private set
		{
			if (this.currentState != value)
			{
				this.PreviousState = this.currentState;
				if (this.PreviousState == ClockworkHatchling.State.Sing)
				{
					this.ToggleSingLoop(false);
				}
			}
			this.currentState = value;
		}
	}

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060017D1 RID: 6097 RVA: 0x0006BE23 File Offset: 0x0006A023
	public bool IsGrounded
	{
		get
		{
			return this.groundColliders.Count > 0;
		}
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x0006BE34 File Offset: 0x0006A034
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.sprite = base.GetComponent<tk2dSprite>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.col = base.GetComponent<Collider2D>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		this.hasVoiceSource = (this.voiceAudioSource != null);
		this.hasToolSource = this.toolSource;
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x0006BEB8 File Offset: 0x0006A0B8
	private void Start()
	{
		if (this.enemyRange)
		{
			this.enemyRange.OnTriggerStayed += delegate(Collider2D collision, GameObject _)
			{
				if (!this.canDamage)
				{
					return;
				}
				if (!this.target)
				{
					if (collision.CompareTag("Hatchling Magnet"))
					{
						this.target = collision.gameObject;
					}
					else if (!collision.CompareTag("Ignore Hatchling") && !collision.CompareTag("FloorDisturber") && !collision.CompareTag("Enemy Range Exclude") && Physics2D.Linecast(base.transform.position, collision.transform.position, LayerMask.GetMask(new string[]
					{
						"Terrain",
						"Soft Terrain"
					})).collider == null)
					{
						this.target = collision.gameObject;
					}
				}
				if (this.CurrentState == ClockworkHatchling.State.Follow && this.target && this.cooldownTimer <= 0f)
				{
					this.CurrentState = ClockworkHatchling.State.Attack;
				}
			};
			this.enemyRange.OnTriggerExited += delegate(Collider2D collision, GameObject _)
			{
				if (this.CurrentState != ClockworkHatchling.State.Attack && this.target && this.target == collision.gameObject)
				{
					this.target = null;
				}
			};
		}
		if (this.groundRange)
		{
			this.groundRange.OnTriggerEntered += delegate(Collider2D collision, GameObject _)
			{
				if (!this.groundColliders.Contains(collision))
				{
					this.groundColliders.Add(collision);
				}
			};
			this.groundRange.OnTriggerExited += delegate(Collider2D collision, GameObject _)
			{
				if (this.groundColliders.Contains(collision))
				{
					this.groundColliders.Remove(collision);
				}
			};
		}
		this.startZ = Random.Range(0.0041f, 0.0049f);
		this.sleepZ = Random.Range(0.003f, 0.0035f);
		base.transform.SetPositionZ(this.startZ);
		this.CheckPoison();
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x0006BF7C File Offset: 0x0006A17C
	private void OnEnable()
	{
		this.DoEnableReset();
		HeroPerformanceRegion.StartedPerforming += this.OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming += this.OnStoppedNeedolin;
		ToolItemManager.OnEquippedStateChanged += this.OnEquippedStateChanged;
		HeroController instance = HeroController.instance;
		instance.preHeroInPosition += this.OnPreHeroInPosition;
		instance.heroInPosition += this.OnHeroInPosition;
		instance.OnDeath += this.Break;
		instance.HeroLeavingScene += this.OnHeroLeavingScene;
		this.canDamage = instance.isHeroInPosition;
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x0006C01C File Offset: 0x0006A21C
	private void OnDisable()
	{
		this.DoDisableReset();
		this.isOnBench = false;
		this.isClearingEffects = false;
		HeroPerformanceRegion.StartedPerforming -= this.OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming -= this.OnStoppedNeedolin;
		ToolItemManager.OnEquippedStateChanged -= this.OnEquippedStateChanged;
		HeroController unsafeInstance = HeroController.UnsafeInstance;
		if (unsafeInstance)
		{
			unsafeInstance.preHeroInPosition -= this.OnPreHeroInPosition;
			unsafeInstance.heroInPosition -= this.OnHeroInPosition;
			unsafeInstance.OnDeath -= this.Break;
			unsafeInstance.HeroLeavingScene -= this.OnHeroLeavingScene;
		}
		this.lastTargetDamaged = null;
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x0006C0D0 File Offset: 0x0006A2D0
	private void DoEnableReset()
	{
		this.hpCurrent = this.hp;
		this.details = this.normalDetails;
		if (this.audioSource)
		{
			this.audioSource.pitch = Random.Range(0.85f, 1.15f);
			if (this.loopClips.Length != 0)
			{
				this.audioSource.clip = this.loopClips[Random.Range(0, this.loopClips.Length)];
			}
		}
		if (this.enemyRange)
		{
			this.enemyRange.gameObject.SetActive(false);
		}
		if (this.groundRange)
		{
			this.groundRange.gameObject.SetActive(true);
		}
		if (this.col)
		{
			this.col.enabled = false;
		}
		this.groundColliders.Clear();
		this.hitParticles.Stop();
		this.deathParticles.Stop();
		this.damagedParticles.Stop();
		this.damagedAudioLoop.Stop();
		this.target = null;
		this.LastFrameState = ClockworkHatchling.State.None;
		this.CurrentState = ClockworkHatchling.State.None;
		if (this.terrainCollider)
		{
			this.terrainCollider.enabled = true;
		}
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = false;
		}
		this.cooldownTimer = this.attackCooldown * Random.Range(0.8f, 1.2f);
		this.ResetNeedolinTimer();
		if (this.spawnRoutine != null)
		{
			base.StopCoroutine(this.spawnRoutine);
		}
		this.spawnRoutine = base.StartCoroutine(this.Spawn());
		this.CheckPoison();
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x0006C266 File Offset: 0x0006A466
	private void DoDisableReset()
	{
		this.hitLandedRoutine = null;
		this.hitBlockedRoutine = null;
		this.quickSpawn = false;
		this.dreamSpawn = false;
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x0006C284 File Offset: 0x0006A484
	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			Color poisonPouchTintColour = Gameplay.PoisonPouchTintColour;
			if (this.representingTool)
			{
				this.sprite.EnableKeyword("CAN_HUESHIFT");
				this.sprite.SetFloat(PoisonTintBase.HueShiftPropId, this.representingTool.PoisonHueShift);
				this.damager.OverridePoisonDamage(this.representingTool.PoisonDamageTicks);
			}
			else
			{
				this.sprite.EnableKeyword("RECOLOUR");
				this.sprite.color = poisonPouchTintColour;
			}
			this.hitParticles.main.startColor = poisonPouchTintColour;
			this.damagedParticles.main.startColor = poisonPouchTintColour;
			this.ptPoisonTrail.Play();
			return;
		}
		this.sprite.DisableKeyword("CAN_HUESHIFT");
		this.sprite.DisableKeyword("RECOLOUR");
		this.sprite.color = Color.white;
		this.hitParticles.main.startColor = this.particleColourDefault;
		this.damagedParticles.main.startColor = this.particleColourDefault;
		this.damager.OverridePoisonDamage(0);
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x0006C3D4 File Offset: 0x0006A5D4
	private void Update()
	{
		if (this.cooldownTimer > 0f)
		{
			this.cooldownTimer -= Time.deltaTime;
		}
		if (this.needolinPlaying && this.IsInNeedolinRange() && this.CurrentState != ClockworkHatchling.State.Sing)
		{
			this.needolinReactTimer += Time.deltaTime;
			if (this.needolinReactTimer >= this.needolinReactTime)
			{
				this.CurrentState = ClockworkHatchling.State.Sing;
				this.animator.Play(this.details.singAnim);
				this.body.linearVelocity = new Vector2(this.body.linearVelocity.x, 4f);
				this.ResetNeedolinTimer();
				this.ToggleSingLoop(true);
				return;
			}
		}
		else if ((!this.needolinPlaying || !this.IsInNeedolinRange()) && this.CurrentState == ClockworkHatchling.State.Sing)
		{
			this.needolinReactTimer += Time.deltaTime;
			if (this.needolinReactTimer >= this.needolinReactTime)
			{
				if (this.isOnBench)
				{
					this.CurrentState = ClockworkHatchling.State.BenchRestStart;
				}
				else
				{
					base.StartCoroutine(this.WakeUp());
				}
				this.ResetNeedolinTimer();
				return;
			}
		}
		else if (this.needolinReactTimer > 0f)
		{
			this.needolinReactTimer = 0f;
		}
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x0006C508 File Offset: 0x0006A708
	private void FixedUpdate()
	{
		ClockworkHatchling.State state = this.CurrentState;
		switch (this.CurrentState)
		{
		case ClockworkHatchling.State.Follow:
		{
			if (this.LastFrameState != ClockworkHatchling.State.Follow)
			{
				if (this.col)
				{
					this.col.enabled = true;
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(true);
				}
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				if (this.audioSource)
				{
					this.audioSource.Play();
				}
				this.body.isKinematic = false;
				this.targetRadius = Random.Range(0.1f, 0.75f);
				this.offset = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(1.25f, 3f));
				this.awayTimer = 0f;
				base.transform.SetPositionZ(this.startZ);
			}
			float heroDistance = this.GetHeroDistance();
			float speedMax = Mathf.Clamp(heroDistance + 4f, 4f, 18f);
			this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			this.DoChase(HeroController.instance.transform, 2f, speedMax, 40f, this.targetRadius, 0.9f, this.offset);
			this.DoBuzz(0.75f, 1f, 18f, 80f, 110f, new Vector2(50f, 50f));
			if (heroDistance * 1.15f > 10f)
			{
				this.awayTimer += Time.fixedDeltaTime;
				if (this.awayTimer >= 1f)
				{
					state = ClockworkHatchling.State.Tele;
				}
			}
			else
			{
				this.awayTimer = 0f;
			}
			break;
		}
		case ClockworkHatchling.State.Aggro:
		{
			if (this.lastTargetDamaged == null)
			{
				state = ClockworkHatchling.State.Follow;
			}
			if (this.LastFrameState != ClockworkHatchling.State.Aggro)
			{
				if (this.col)
				{
					this.col.enabled = true;
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(true);
				}
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				if (this.audioSource)
				{
					this.audioSource.Play();
				}
				this.body.isKinematic = false;
				this.targetRadius = Random.Range(0.1f, 0.75f);
				this.offset = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(1.25f, 3f));
				this.awayTimer = 0f;
				base.transform.SetPositionZ(this.startZ);
			}
			float heroDistance2 = this.GetHeroDistance();
			float speedMax2 = Mathf.Clamp(heroDistance2 + 4f, 4f, 18f);
			this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			this.DoChase(this.lastTargetDamaged.transform, 2f, speedMax2, 40f, this.targetRadius, 0.9f, this.offset);
			this.DoBuzz(0.75f, 1f, 18f, 80f, 110f, new Vector2(50f, 50f));
			if (heroDistance2 * 1.15f > 10f)
			{
				this.awayTimer += Time.fixedDeltaTime;
				if (this.awayTimer >= 1f)
				{
					state = ClockworkHatchling.State.Tele;
				}
			}
			else
			{
				this.awayTimer = 0f;
			}
			break;
		}
		case ClockworkHatchling.State.Tele:
			if (this.LastFrameState != ClockworkHatchling.State.Tele)
			{
				if (this.audioSource)
				{
					this.audioSource.Stop();
				}
				if (this.animator)
				{
					this.animator.Play(this.details.teleStartAnim);
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(false);
				}
				if (this.groundRange)
				{
					this.groundRange.gameObject.SetActive(false);
				}
				if (this.terrainCollider)
				{
					this.terrainCollider.enabled = false;
				}
				if (this.attackChargeAudioTable)
				{
					this.attackChargeAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
				}
			}
			this.DoChase(HeroController.instance.transform, 2f, 35f, 400f, 0f, 0f, new Vector2(0f, 0.5f));
			this.DoFace(false, false, this.details.turnFlyAnim, true, 0.5f);
			if (this.GetHeroDistance() < 2f)
			{
				state = ClockworkHatchling.State.None;
				base.StartCoroutine(this.TeleEnd());
			}
			break;
		case ClockworkHatchling.State.Attack:
			this.damager.gameObject.SetActive(true);
			if (this.LastFrameState != ClockworkHatchling.State.Attack)
			{
				if (this.audioSource)
				{
					this.audioSource.Stop();
					if (this.attackChargeAudioTable)
					{
						this.attackChargeAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
					}
				}
				this.PlayAttackVoice();
				if (this.animator)
				{
					this.animator.Play(this.details.attackAnim);
				}
				if (this.enemyRange)
				{
					this.enemyRange.gameObject.SetActive(false);
				}
				this.attackFinishTime = Time.timeAsDouble + 2.0;
			}
			if (Time.timeAsDouble > this.attackFinishTime || this.target == null)
			{
				this.target = null;
				state = ClockworkHatchling.State.Follow;
			}
			else
			{
				this.DoFace(false, true, this.details.turnAttackAnim, true, 0.1f);
				this.DoChaseSimple(this.target.transform, 25f, 100f, 0f, 0f);
			}
			break;
		case ClockworkHatchling.State.BenchRestStart:
			if (this.LastFrameState != ClockworkHatchling.State.BenchRestStart)
			{
				this.body.linearVelocity = Vector2.zero;
				this.ToggleSingLoop(false);
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				this.benchRestWaitTime = Time.timeAsDouble + (double)Random.Range(1f, 2.5f);
			}
			if (Time.timeAsDouble < this.benchRestWaitTime)
			{
				this.DoBuzz(0.75f, 1f, 2f, 30f, 50f, new Vector2(1f, 1f));
				this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			}
			else
			{
				state = ClockworkHatchling.State.BenchRestLower;
			}
			break;
		case ClockworkHatchling.State.BenchRestLower:
		{
			if (this.LastFrameState != ClockworkHatchling.State.BenchRestLower)
			{
				if (this.animator)
				{
					this.animator.Play(this.details.flyAnim);
				}
				base.transform.SetPositionZ(this.sleepZ);
				this.body.isKinematic = false;
			}
			this.body.AddForce(new Vector2(0f, -5f));
			Vector2 linearVelocity = this.body.linearVelocity;
			linearVelocity.x *= 0.85f;
			this.body.linearVelocity = linearVelocity;
			this.DoFace(false, true, this.details.turnFlyAnim, true, 0.5f);
			if (this.IsGrounded)
			{
				state = ClockworkHatchling.State.BenchResting;
				if (this.audioSource)
				{
					this.audioSource.Stop();
				}
				this.body.isKinematic = true;
				this.body.linearVelocity = Vector2.zero;
				if (this.animator)
				{
					this.animator.Play(this.details.restStartAnim);
				}
				Vector3 position = base.transform.position;
				if (this.attackChargeAudioTable)
				{
					this.attackChargeAudioTable.SpawnAndPlayOneShot(position, false);
				}
				if (this.details.groundPoint)
				{
					RaycastHit2D raycastHit2D = Helper.Raycast2D(position, Vector2.down, 2f, 256);
					if (raycastHit2D.collider != null)
					{
						Vector2 point = raycastHit2D.point;
						position.y = point.y + (position.y - this.details.groundPoint.position.y);
						base.transform.position = position;
					}
				}
			}
			break;
		}
		case ClockworkHatchling.State.Recoil:
		{
			Vector2 linearVelocity2 = this.body.linearVelocity;
			this.body.linearVelocity = new Vector2(linearVelocity2.x * 0.9f, linearVelocity2.y * 0.975f);
			break;
		}
		case ClockworkHatchling.State.Sing:
		{
			Vector2 linearVelocity3 = this.body.linearVelocity;
			this.body.linearVelocity = new Vector2(linearVelocity3.x * 0.9f, linearVelocity3.y * 0.9f);
			break;
		}
		}
		this.LastFrameState = this.CurrentState;
		this.CurrentState = state;
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x0006CE38 File Offset: 0x0006B038
	private IEnumerator Spawn()
	{
		yield return null;
		if (this.audioSource)
		{
			this.audioSource.Play();
		}
		if (this.meshRenderer && !this.dreamSpawn)
		{
			this.meshRenderer.enabled = true;
		}
		this.damager.gameObject.SetActive(false);
		if (!this.quickSpawn)
		{
			float finishDelay = 0f;
			if (this.animator)
			{
				tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.details.hatchAnim);
				if (clipByName != null)
				{
					this.animator.Play(clipByName);
					finishDelay = clipByName.Duration;
				}
			}
			if (this.body)
			{
				float num = Random.Range(65f, 75f);
				float num2 = Random.Range(85f, 95f);
				Vector2 linearVelocity = new Vector2
				{
					x = num * Mathf.Cos(num2 * 0.017453292f),
					y = num * Mathf.Sin(num2 * 0.017453292f)
				};
				this.body.linearVelocity = linearVelocity;
			}
			if (this.throwAudioTable)
			{
				this.throwAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			for (float elapsed = 0f; elapsed < finishDelay; elapsed += Time.fixedDeltaTime)
			{
				if (this.body)
				{
					this.body.linearVelocity *= 0.8f;
				}
				yield return new WaitForFixedUpdate();
			}
			this.openEffect.SetActive(true);
			Vector3 position = base.transform.position;
			this.pinPrefab.Spawn().transform.position = new Vector3(position.x, position.y, position.z + 0.01f);
			if (this.activateAudioTable)
			{
				this.activateAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			this.PlaySpawnVoice();
		}
		this.CurrentState = (GameManager.instance.playerData.atBench ? ClockworkHatchling.State.BenchRestStart : ClockworkHatchling.State.Follow);
		this.spawnRoutine = null;
		yield break;
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x0006CE47 File Offset: 0x0006B047
	private float GetHeroDistance()
	{
		return Vector2.Distance(base.transform.position, HeroController.instance.transform.position);
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x0006CE72 File Offset: 0x0006B072
	private IEnumerator TeleEnd()
	{
		if (this.groundRange)
		{
			this.groundRange.gameObject.SetActive(true);
		}
		if (this.terrainCollider)
		{
			this.terrainCollider.enabled = true;
		}
		if (this.audioSource)
		{
			this.audioSource.Play();
		}
		if (this.animator)
		{
			tk2dSpriteAnimationClip clip = this.animator.GetClipByName(this.details.teleEndAnim);
			this.animator.Play(clip);
			for (float elapsed = 0f; elapsed < clip.Duration; elapsed += Time.fixedDeltaTime)
			{
				if (this.body)
				{
					this.body.linearVelocity *= 0.7f;
				}
				yield return new WaitForFixedUpdate();
			}
			clip = null;
		}
		this.CurrentState = ClockworkHatchling.State.Follow;
		yield break;
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x0006CE81 File Offset: 0x0006B081
	public void Break()
	{
		this.damager.gameObject.SetActive(false);
		this.hpCurrent = 0;
		this.StartHitLanded();
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x0006CEA1 File Offset: 0x0006B0A1
	public void FsmHitLanded()
	{
		this.StartHitLanded();
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x0006CEAC File Offset: 0x0006B0AC
	private void StartHitLanded()
	{
		this.hitCount++;
		int num = this.hitState + 1;
		this.hitState = num;
		this.blockHitState = num;
		if (this.hitLandedRoutine == null)
		{
			this.hitLandedRoutine = base.StartCoroutine(this.HitLanded());
		}
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x0006CEF8 File Offset: 0x0006B0F8
	public void FsmHitBlocked()
	{
		int num = this.hitState + 1;
		this.hitState = num;
		this.blockHitState = num;
		if (this.hitBlockedRoutine == null)
		{
			this.hitBlockedRoutine = base.StartCoroutine(this.HitBlocked());
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x0006CF36 File Offset: 0x0006B136
	private IEnumerator HitLanded()
	{
		yield return new WaitForFixedUpdate();
		this.explodeSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		this.body.linearVelocity = Vector2.zero;
		if (this.spriteFlash)
		{
			this.spriteFlash.CancelFlash();
		}
		float seconds = 2f;
		this.hpCurrent -= this.hitCount;
		this.hitCount = 0;
		this.hitParticles.Play();
		this.damager.gameObject.SetActive(false);
		if (this.hpCurrent > 0)
		{
			float num = (float)Random.Range(20, 30);
			num *= base.transform.localScale.x;
			this.body.linearVelocity = new Vector2(num, this.body.linearVelocity.y);
			this.CurrentState = ClockworkHatchling.State.Recoil;
			if (this.hpCurrent * 3 < this.hp)
			{
				this.damagedParticles.Play();
				this.damagedAudioLoop.Play();
			}
			yield return new WaitForSeconds(0.35f);
			float heroDistance = this.GetHeroDistance();
			this.CurrentState = ((heroDistance * 1.15f > 12f) ? ClockworkHatchling.State.Tele : ClockworkHatchling.State.Follow);
			this.cooldownTimer = this.attackCooldown * Random.Range(0.8f, 1.2f);
		}
		else
		{
			this.CurrentState = ClockworkHatchling.State.None;
			if (this.animator)
			{
				tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName("Burst");
				if (clipByName != null)
				{
					this.animator.Play(clipByName);
					seconds = clipByName.Duration;
				}
			}
			if (this.breakAudioTable)
			{
				this.breakAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			this.hitParticles.Stop();
			this.damagedParticles.Stop();
			this.damagedAudioLoop.Stop();
			this.deathParticles.Play();
			yield return new WaitForSeconds(seconds);
			this.meshRenderer.enabled = false;
			yield return new WaitForSeconds(1f);
			base.gameObject.Recycle();
		}
		this.hitLandedRoutine = null;
		yield break;
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x0006CF45 File Offset: 0x0006B145
	private IEnumerator HitBlocked()
	{
		yield return new WaitForFixedUpdate();
		if (this.blockHitState == this.hitState)
		{
			yield return new WaitForSeconds(this.blockedResetDelay);
		}
		if (this.blockHitState == this.hitState)
		{
			this.damager.gameObject.SetActive(false);
		}
		this.hitBlockedRoutine = null;
		yield break;
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x0006CF54 File Offset: 0x0006B154
	public void FsmHazardReload()
	{
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x0006CF56 File Offset: 0x0006B156
	[ContextMenu("Clear Effects")]
	public void ClearEffects()
	{
		this.ClearEffects(false);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x0006CF5F File Offset: 0x0006B15F
	public void ClearEffects(bool unEquipped)
	{
		if (this.isClearingEffects || !base.isActiveAndEnabled)
		{
			return;
		}
		this.isClearingEffects = true;
		base.StopAllCoroutines();
		if (unEquipped)
		{
			base.StartCoroutine(this.UnEquippedRoutine());
			return;
		}
		base.StartCoroutine(this.ClearEffectsRoutine());
	}

	// Token: 0x060017E7 RID: 6119 RVA: 0x0006CF9D File Offset: 0x0006B19D
	private IEnumerator UnEquippedRoutine()
	{
		this.explodeSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		this.body.linearVelocity = Vector2.zero;
		if (this.spriteFlash)
		{
			this.spriteFlash.CancelFlash();
		}
		float seconds = 2f;
		this.hpCurrent--;
		this.hitParticles.Play();
		this.damager.gameObject.SetActive(false);
		this.CurrentState = ClockworkHatchling.State.None;
		if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName("Burst");
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		if (this.breakAudioTable)
		{
			this.breakAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		this.hitParticles.Stop();
		this.damagedParticles.Stop();
		this.damagedAudioLoop.Stop();
		this.deathParticles.Play();
		yield return new WaitForSeconds(seconds);
		this.meshRenderer.enabled = false;
		yield return new WaitForSeconds(1f);
		this.isClearingEffects = false;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x060017E8 RID: 6120 RVA: 0x0006CFAC File Offset: 0x0006B1AC
	private IEnumerator ClearEffectsRoutine()
	{
		yield return new WaitForSeconds(0.01f);
		Object.Instantiate<GameObject>(this.clearEffectsDummyPrefab, base.transform.position, Quaternion.identity);
		this.meshRenderer.enabled = false;
		this.isClearingEffects = false;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x0006CFBB File Offset: 0x0006B1BB
	public void FsmBenchRestStart()
	{
		this.CurrentState = ClockworkHatchling.State.BenchRestStart;
		this.isOnBench = true;
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x0006CFCB File Offset: 0x0006B1CB
	public void FsmBenchRestEnd()
	{
		this.isOnBench = false;
		if (this.CurrentState == ClockworkHatchling.State.BenchResting)
		{
			base.StartCoroutine(this.WakeUp());
			return;
		}
		this.CurrentState = ClockworkHatchling.State.Follow;
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x0006CFF2 File Offset: 0x0006B1F2
	private IEnumerator WakeUp()
	{
		float seconds = 0f;
		if (this.animator)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.details.restEndAnim);
			if (clipByName != null)
			{
				this.animator.Play(clipByName);
				seconds = clipByName.Duration;
			}
		}
		if (this.activateAudioTable)
		{
			this.activateAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		this.PlaySpawnVoice();
		this.ToggleSingLoop(false);
		yield return new WaitForSeconds(seconds);
		if (this.isOnBench)
		{
			this.currentState = ClockworkHatchling.State.BenchRestStart;
		}
		else
		{
			this.CurrentState = ClockworkHatchling.State.Follow;
		}
		yield break;
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x0006D001 File Offset: 0x0006B201
	private void OnHeroLeavingScene()
	{
		this.canDamage = false;
		this.CurrentState = ClockworkHatchling.State.Follow;
		this.damager.gameObject.SetActive(false);
	}

	// Token: 0x060017ED RID: 6125 RVA: 0x0006D024 File Offset: 0x0006B224
	public void OnPreHeroInPosition(bool forceDirect)
	{
		HeroController instance = HeroController.instance;
		base.transform.SetPosition2D(instance.transform.position);
	}

	// Token: 0x060017EE RID: 6126 RVA: 0x0006D054 File Offset: 0x0006B254
	public void OnHeroInPosition(bool forceDirect)
	{
		this.DoDisableReset();
		HeroController instance = HeroController.instance;
		this.body.MovePosition(instance.transform.position + new Vector2(2f * (float)(instance.cState.facingRight ? -1 : 1), 1.5f));
		this.quickSpawn = true;
		this.canDamage = true;
		this.DoEnableReset();
	}

	// Token: 0x060017EF RID: 6127 RVA: 0x0006D0C3 File Offset: 0x0006B2C3
	public void SetLastTargetDamaged(GameObject target)
	{
		this.lastTargetDamaged = target;
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x0006D0CC File Offset: 0x0006B2CC
	private void OnEquippedStateChanged()
	{
		if (this.hasToolSource && !this.toolSource.Status.IsEquippedAny)
		{
			this.ClearEffects(true);
		}
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x0006D0EF File Offset: 0x0006B2EF
	private void OnStartedNeedolin()
	{
		this.needolinPlaying = true;
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x0006D0F8 File Offset: 0x0006B2F8
	private void OnStoppedNeedolin()
	{
		this.needolinPlaying = false;
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x0006D101 File Offset: 0x0006B301
	private bool IsInNeedolinRange()
	{
		return HeroPerformanceRegion.GetAffectedState(base.transform, false) == HeroPerformanceRegion.AffectedState.ActiveInner;
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x0006D112 File Offset: 0x0006B312
	private void ResetNeedolinTimer()
	{
		this.needolinReactTime = Random.Range(0.75f, 1f);
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x0006D12C File Offset: 0x0006B32C
	private void PlayVoiceTable(RandomAudioClipTable table)
	{
		if (!this.hasVoiceSource)
		{
			return;
		}
		if (table == null)
		{
			return;
		}
		AudioClip audioClip = table.SelectClip(false);
		if (audioClip == null)
		{
			return;
		}
		float pitch = table.SelectPitch();
		float volume = table.SelectVolume();
		this.voiceAudioSource.pitch = pitch;
		this.voiceAudioSource.volume = volume;
		this.voiceAudioSource.PlayOneShot(audioClip);
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x0006D190 File Offset: 0x0006B390
	private void PlayAttackVoice()
	{
		this.PlayVoiceTable(this.attackVoiceTable);
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x0006D19E File Offset: 0x0006B39E
	public void PlaySpawnVoice()
	{
		this.PlayVoiceTable(this.spawnVoiceTable);
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x0006D1AC File Offset: 0x0006B3AC
	public void PlayTurnVoice()
	{
		this.PlayVoiceTable(this.turnVoiceTable);
	}

	// Token: 0x060017F9 RID: 6137 RVA: 0x0006D1BC File Offset: 0x0006B3BC
	public void ToggleSingLoop(bool active)
	{
		if (!this.hasVoiceSource)
		{
			return;
		}
		if (!active)
		{
			this.voiceAudioSource.Stop();
			this.voiceAudioSource.volume = 1f;
			return;
		}
		if (this.singVoiceTable == null)
		{
			return;
		}
		AudioClip audioClip = this.singVoiceTable.SelectClip(false);
		if (audioClip == null)
		{
			return;
		}
		this.voiceAudioSource.loop = true;
		this.voiceAudioSource.pitch = this.singVoiceTable.SelectPitch();
		this.voiceAudioSource.clip = audioClip;
		this.voiceAudioSource.time = Random.Range(0f, audioClip.length);
		this.voiceAudioSource.volume = this.singVoiceTable.SelectVolume();
		this.voiceAudioSource.Play();
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x0006D284 File Offset: 0x0006B484
	private void DoFace(bool spriteFacesRight, bool playNewAnimation, string newAnimationClip, bool pauseBetweenTurns, float pauseTime)
	{
		if (this.body == null)
		{
			return;
		}
		ref Vector2 linearVelocity = this.body.linearVelocity;
		Vector3 localScale = base.transform.localScale;
		float x = linearVelocity.x;
		if (this.CurrentState != this.LastFrameState)
		{
			this.xScale = base.transform.localScale.x;
			this.pauseTimer = 0f;
		}
		if (this.xScale < 0f)
		{
			this.xScale *= -1f;
		}
		if (this.pauseTimer <= 0f || !pauseBetweenTurns)
		{
			if (x > 0f)
			{
				if (spriteFacesRight)
				{
					if (Math.Abs(localScale.x - this.xScale) > Mathf.Epsilon)
					{
						this.PlayTurnVoice();
						this.pauseTimer = pauseTime;
						localScale.x = this.xScale;
						if (playNewAnimation)
						{
							this.animator.Play(newAnimationClip);
							this.animator.PlayFromFrame(0);
						}
					}
				}
				else if (Math.Abs(localScale.x - -this.xScale) > Mathf.Epsilon)
				{
					this.PlayTurnVoice();
					this.pauseTimer = pauseTime;
					localScale.x = -this.xScale;
					if (playNewAnimation)
					{
						this.animator.Play(newAnimationClip);
						this.animator.PlayFromFrame(0);
					}
				}
			}
			else if (x <= 0f)
			{
				if (spriteFacesRight)
				{
					if (Math.Abs(localScale.x - -this.xScale) > Mathf.Epsilon)
					{
						this.PlayTurnVoice();
						this.pauseTimer = pauseTime;
						localScale.x = -this.xScale;
						if (playNewAnimation)
						{
							this.animator.Play(newAnimationClip);
							this.animator.PlayFromFrame(0);
						}
					}
				}
				else if (Math.Abs(localScale.x - this.xScale) > Mathf.Epsilon)
				{
					this.PlayTurnVoice();
					this.pauseTimer = pauseTime;
					localScale.x = this.xScale;
					if (playNewAnimation)
					{
						this.animator.Play(newAnimationClip);
						this.animator.PlayFromFrame(0);
					}
				}
			}
		}
		else
		{
			this.pauseTimer -= Time.deltaTime;
		}
		base.transform.SetScaleX(localScale.x);
	}

	// Token: 0x060017FB RID: 6139 RVA: 0x0006D4BC File Offset: 0x0006B6BC
	private void DoChase(Transform chaseTarget, float distance, float speedMax, float accelerationForce, float chaseTargetRadius, float deceleration, Vector2 chaseOffset)
	{
		if (this.body == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = chaseTarget.position;
		float num = Mathf.Sqrt(Mathf.Pow(position.x - (position2.x + chaseOffset.x), 2f) + Mathf.Pow(position.y - (position2.y + chaseOffset.y), 2f));
		Vector2 vector = this.body.linearVelocity;
		if (num <= distance - chaseTargetRadius || num >= distance + chaseTargetRadius)
		{
			Vector2 vector2 = new Vector2(position2.x + chaseOffset.x - position.x, position2.y + chaseOffset.y - position.y);
			vector2 = Vector2.ClampMagnitude(vector2, 1f);
			vector2 = new Vector2(vector2.x * accelerationForce, vector2.y * accelerationForce);
			if (num < distance)
			{
				vector2 = new Vector2(-vector2.x, -vector2.y);
			}
			this.body.AddForce(vector2);
			vector = Vector2.ClampMagnitude(vector, speedMax);
			this.body.linearVelocity = vector;
			return;
		}
		vector = this.body.linearVelocity;
		if (vector.x < 0f)
		{
			vector.x *= deceleration;
			if (vector.x > 0f)
			{
				vector.x = 0f;
			}
		}
		else if (vector.x > 0f)
		{
			vector.x *= deceleration;
			if (vector.x < 0f)
			{
				vector.x = 0f;
			}
		}
		if (vector.y < 0f)
		{
			vector.y *= deceleration;
			if (vector.y > 0f)
			{
				vector.y = 0f;
			}
		}
		else if (vector.y > 0f)
		{
			vector.y *= deceleration;
			if (vector.y < 0f)
			{
				vector.y = 0f;
			}
		}
		this.body.linearVelocity = vector;
	}

	// Token: 0x060017FC RID: 6140 RVA: 0x0006D6CC File Offset: 0x0006B8CC
	private void DoBuzz(float waitMin, float waitMax, float speedMax, float accelerationMin, float accelerationMax, Vector2 roamingRange)
	{
		if (this.body == null)
		{
			return;
		}
		float num = 1.125f;
		Vector3 position = base.transform.position;
		if (this.CurrentState != this.LastFrameState)
		{
			this.startX = position.x;
			this.startY = position.y;
		}
		Vector2 linearVelocity = this.body.linearVelocity;
		if (position.y < this.startY - roamingRange.y)
		{
			if (linearVelocity.y < 0f)
			{
				this.accelY = accelerationMax;
				this.accelY /= 2000f;
				linearVelocity.y /= num;
				this.waitTime = Random.Range(waitMin, waitMax);
			}
		}
		else if (position.y > this.startY + roamingRange.y && linearVelocity.y > 0f)
		{
			this.accelY = -accelerationMax;
			this.accelY /= 2000f;
			linearVelocity.y /= num;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (position.x < this.startX - roamingRange.x)
		{
			if (linearVelocity.x < 0f)
			{
				this.accelX = accelerationMax;
				this.accelX /= 2000f;
				linearVelocity.x /= num;
				this.waitTime = Random.Range(waitMin, waitMax);
			}
		}
		else if (position.x > this.startX + roamingRange.x && linearVelocity.x > 0f)
		{
			this.accelX = -accelerationMax;
			this.accelX /= 2000f;
			linearVelocity.x /= num;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (this.waitTime <= Mathf.Epsilon)
		{
			if (base.transform.position.y < this.startY - roamingRange.y)
			{
				this.accelY = Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.y > this.startY + roamingRange.y)
			{
				this.accelY = Random.Range(-accelerationMax, accelerationMin);
			}
			else
			{
				this.accelY = Random.Range(-accelerationMax, accelerationMax);
			}
			if (base.transform.position.x < this.startX - roamingRange.x)
			{
				this.accelX = Random.Range(accelerationMin, accelerationMax);
			}
			else if (base.transform.position.x > this.startX + roamingRange.x)
			{
				this.accelX = Random.Range(-accelerationMax, accelerationMin);
			}
			else
			{
				this.accelX = Random.Range(-accelerationMax, accelerationMax);
			}
			this.accelY /= 2000f;
			this.accelX /= 2000f;
			this.waitTime = Random.Range(waitMin, waitMax);
		}
		if (this.waitTime > Mathf.Epsilon)
		{
			this.waitTime -= Time.deltaTime;
		}
		linearVelocity.x += this.accelX;
		linearVelocity.y += this.accelY;
		if (linearVelocity.x > speedMax)
		{
			linearVelocity.x = speedMax;
		}
		if (linearVelocity.x < -speedMax)
		{
			linearVelocity.x = -speedMax;
		}
		if (linearVelocity.y > speedMax)
		{
			linearVelocity.y = speedMax;
		}
		if (linearVelocity.y < -speedMax)
		{
			linearVelocity.y = -speedMax;
		}
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x060017FD RID: 6141 RVA: 0x0006DA4C File Offset: 0x0006BC4C
	private void DoChaseSimple(Transform chaseTarget, float speedMax, float accelerationForce, float offsetX, float offsetY)
	{
		if (this.body == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = chaseTarget.position;
		Vector2 vector = new Vector2(position2.x + offsetX - position.x, position2.y + offsetY - position.y);
		vector = Vector2.ClampMagnitude(vector, 1f);
		vector = new Vector2(vector.x * accelerationForce, vector.y * accelerationForce);
		this.body.AddForce(vector);
		Vector2 vector2 = this.body.linearVelocity;
		vector2 = Vector2.ClampMagnitude(vector2, speedMax);
		this.body.linearVelocity = vector2;
	}

	// Token: 0x0400168A RID: 5770
	private const float NEEDOLIN_REACT_MIN = 0.75f;

	// Token: 0x0400168B RID: 5771
	private const float NEEDOLIN_REACT_MAX = 1f;

	// Token: 0x0400168C RID: 5772
	[SerializeField]
	private ToolItem toolSource;

	// Token: 0x0400168D RID: 5773
	public ClockworkHatchling.TypeDetails normalDetails = new ClockworkHatchling.TypeDetails
	{
		attackAnim = "Attack",
		flyAnim = "Fly",
		hatchAnim = "Hatch",
		teleEndAnim = "Tele End",
		teleStartAnim = "Tele Start",
		turnAttackAnim = "TurnToAttack",
		turnFlyAnim = "TurnToFly",
		restStartAnim = "Rest Start",
		restEndAnim = "Rest End",
		singAnim = "Sing",
		spatterColor = Color.black
	};

	// Token: 0x0400168E RID: 5774
	[SerializeField]
	private TriggerEnterEvent enemyRange;

	// Token: 0x0400168F RID: 5775
	[SerializeField]
	private TriggerEnterEvent groundRange;

	// Token: 0x04001690 RID: 5776
	[SerializeField]
	private Collider2D terrainCollider;

	// Token: 0x04001691 RID: 5777
	[SerializeField]
	private ParticleSystem hitParticles;

	// Token: 0x04001692 RID: 5778
	[SerializeField]
	private ParticleSystem deathParticles;

	// Token: 0x04001693 RID: 5779
	[SerializeField]
	private ParticleSystem damagedParticles;

	// Token: 0x04001694 RID: 5780
	[SerializeField]
	private AudioSource damagedAudioLoop;

	// Token: 0x04001695 RID: 5781
	[SerializeField]
	private AudioClip[] loopClips;

	// Token: 0x04001696 RID: 5782
	[SerializeField]
	private int hp;

	// Token: 0x04001697 RID: 5783
	[SerializeField]
	private RandomAudioClipTable attackChargeAudioTable;

	// Token: 0x04001698 RID: 5784
	[SerializeField]
	private RandomAudioClipTable activateAudioTable;

	// Token: 0x04001699 RID: 5785
	[SerializeField]
	private RandomAudioClipTable throwAudioTable;

	// Token: 0x0400169A RID: 5786
	[SerializeField]
	private RandomAudioClipTable breakAudioTable;

	// Token: 0x0400169B RID: 5787
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x0400169C RID: 5788
	[SerializeField]
	private AudioEvent explodeSound = new AudioEvent
	{
		PitchMin = 0.85f,
		PitchMax = 1.15f,
		Volume = 1f
	};

	// Token: 0x0400169D RID: 5789
	[SerializeField]
	private GameObject openEffect;

	// Token: 0x0400169E RID: 5790
	[SerializeField]
	private float attackCooldown;

	// Token: 0x0400169F RID: 5791
	[SerializeField]
	private DamageEnemies damager;

	// Token: 0x040016A0 RID: 5792
	[SerializeField]
	private GameObject pinPrefab;

	// Token: 0x040016A1 RID: 5793
	[SerializeField]
	private GameObject clearEffectsDummyPrefab;

	// Token: 0x040016A2 RID: 5794
	[SerializeField]
	private ParticleSystem ptPoisonTrail;

	// Token: 0x040016A3 RID: 5795
	[SerializeField]
	private Color particleColourDefault;

	// Token: 0x040016A4 RID: 5796
	[SerializeField]
	private ToolItem representingTool;

	// Token: 0x040016A5 RID: 5797
	[SerializeField]
	private float blockedResetDelay = 0.1f;

	// Token: 0x040016A6 RID: 5798
	[Header("Audio")]
	[SerializeField]
	private AudioSource voiceAudioSource;

	// Token: 0x040016A7 RID: 5799
	[SerializeField]
	private RandomAudioClipTable spawnVoiceTable;

	// Token: 0x040016A8 RID: 5800
	[SerializeField]
	private RandomAudioClipTable attackVoiceTable;

	// Token: 0x040016A9 RID: 5801
	[SerializeField]
	private RandomAudioClipTable turnVoiceTable;

	// Token: 0x040016AA RID: 5802
	[SerializeField]
	private RandomAudioClipTable singVoiceTable;

	// Token: 0x040016AB RID: 5803
	private readonly List<Collider2D> groundColliders = new List<Collider2D>();

	// Token: 0x040016AC RID: 5804
	private GameObject target;

	// Token: 0x040016AD RID: 5805
	private ClockworkHatchling.TypeDetails details;

	// Token: 0x040016AE RID: 5806
	private ClockworkHatchling.State currentState;

	// Token: 0x040016AF RID: 5807
	private int hpCurrent;

	// Token: 0x040016B0 RID: 5808
	private float targetRadius;

	// Token: 0x040016B1 RID: 5809
	private Vector3 offset;

	// Token: 0x040016B2 RID: 5810
	private float awayTimer;

	// Token: 0x040016B3 RID: 5811
	private double attackFinishTime;

	// Token: 0x040016B4 RID: 5812
	private double benchRestWaitTime;

	// Token: 0x040016B5 RID: 5813
	private bool quickSpawn;

	// Token: 0x040016B6 RID: 5814
	private bool dreamSpawn;

	// Token: 0x040016B7 RID: 5815
	private float startZ;

	// Token: 0x040016B8 RID: 5816
	private float sleepZ;

	// Token: 0x040016B9 RID: 5817
	private AudioSource audioSource;

	// Token: 0x040016BA RID: 5818
	private MeshRenderer meshRenderer;

	// Token: 0x040016BB RID: 5819
	private tk2dSprite sprite;

	// Token: 0x040016BC RID: 5820
	private tk2dSpriteAnimator animator;

	// Token: 0x040016BD RID: 5821
	private Rigidbody2D body;

	// Token: 0x040016BE RID: 5822
	private Collider2D col;

	// Token: 0x040016BF RID: 5823
	private SpriteFlash spriteFlash;

	// Token: 0x040016C0 RID: 5824
	private float cooldownTimer;

	// Token: 0x040016C1 RID: 5825
	[Space]
	public GameObject lastTargetDamaged;

	// Token: 0x040016C2 RID: 5826
	private bool needolinPlaying;

	// Token: 0x040016C3 RID: 5827
	private float needolinReactTime;

	// Token: 0x040016C4 RID: 5828
	private float needolinReactTimer;

	// Token: 0x040016C7 RID: 5831
	private bool isOnBench;

	// Token: 0x040016C8 RID: 5832
	private Coroutine hitBlockedRoutine;

	// Token: 0x040016C9 RID: 5833
	private int hitState;

	// Token: 0x040016CA RID: 5834
	private int blockHitState;

	// Token: 0x040016CB RID: 5835
	private int hitCount;

	// Token: 0x040016CC RID: 5836
	private Coroutine hitLandedRoutine;

	// Token: 0x040016CD RID: 5837
	private Coroutine spawnRoutine;

	// Token: 0x040016CE RID: 5838
	private bool hasVoiceSource;

	// Token: 0x040016CF RID: 5839
	private bool hasSingSource;

	// Token: 0x040016D0 RID: 5840
	private bool canDamage;

	// Token: 0x040016D1 RID: 5841
	private bool hasToolSource;

	// Token: 0x040016D2 RID: 5842
	private bool isClearingEffects;

	// Token: 0x040016D3 RID: 5843
	private float pauseTimer;

	// Token: 0x040016D4 RID: 5844
	private float xScale;

	// Token: 0x040016D5 RID: 5845
	private float startX;

	// Token: 0x040016D6 RID: 5846
	private float startY;

	// Token: 0x040016D7 RID: 5847
	private float accelY;

	// Token: 0x040016D8 RID: 5848
	private float accelX;

	// Token: 0x040016D9 RID: 5849
	private float waitTime;

	// Token: 0x02001584 RID: 5508
	[Serializable]
	public struct TypeDetails
	{
		// Token: 0x04008785 RID: 34693
		public int damage;

		// Token: 0x04008786 RID: 34694
		public AudioEvent birthSound;

		// Token: 0x04008787 RID: 34695
		public Color spatterColor;

		// Token: 0x04008788 RID: 34696
		public Transform groundPoint;

		// Token: 0x04008789 RID: 34697
		public string attackAnim;

		// Token: 0x0400878A RID: 34698
		public string flyAnim;

		// Token: 0x0400878B RID: 34699
		public string hatchAnim;

		// Token: 0x0400878C RID: 34700
		public string teleEndAnim;

		// Token: 0x0400878D RID: 34701
		public string teleStartAnim;

		// Token: 0x0400878E RID: 34702
		public string turnAttackAnim;

		// Token: 0x0400878F RID: 34703
		public string turnFlyAnim;

		// Token: 0x04008790 RID: 34704
		public string restStartAnim;

		// Token: 0x04008791 RID: 34705
		public string restEndAnim;

		// Token: 0x04008792 RID: 34706
		public string singAnim;
	}

	// Token: 0x02001585 RID: 5509
	public enum State
	{
		// Token: 0x04008794 RID: 34708
		None,
		// Token: 0x04008795 RID: 34709
		Follow,
		// Token: 0x04008796 RID: 34710
		Aggro,
		// Token: 0x04008797 RID: 34711
		Tele,
		// Token: 0x04008798 RID: 34712
		Attack,
		// Token: 0x04008799 RID: 34713
		BenchRestStart,
		// Token: 0x0400879A RID: 34714
		BenchRestLower,
		// Token: 0x0400879B RID: 34715
		BenchResting,
		// Token: 0x0400879C RID: 34716
		Recoil,
		// Token: 0x0400879D RID: 34717
		Sing
	}
}
