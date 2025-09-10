using System;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public class ActiveCorpse : CorpseItems, AntRegion.ICheck
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x0600183B RID: 6203 RVA: 0x0006E951 File Offset: 0x0006CB51
	public bool CanEnterAntRegion
	{
		get
		{
			return this.rb.bodyType != RigidbodyType2D.Kinematic;
		}
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x0006E964 File Offset: 0x0006CB64
	public static bool TryGetCorpse(GameObject gameObject, out ActiveCorpse corpse)
	{
		return ActiveCorpse.activeCorpses.TryGetValue(gameObject, out corpse);
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x0006E972 File Offset: 0x0006CB72
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.effectPos, 0.2f);
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x0006E9A0 File Offset: 0x0006CBA0
	protected override void Awake()
	{
		base.Awake();
		this.sprite = base.GetComponent<tk2dSprite>();
		if (this.sprite == null)
		{
			this.destroyed = true;
			Object.Destroy(this);
			return;
		}
		this.renderer = this.sprite.GetComponent<MeshRenderer>();
		this.rb = base.GetComponent<Rigidbody2D>();
		this.polyCollider = base.GetComponent<PolygonCollider2D>();
		this.hasCollider = this.polyCollider;
		this.originalBodyType = this.rb.bodyType;
		if (this.useVisiblity)
		{
			this.visibility = base.gameObject.AddComponentIfNotPresent<VisibilityGroup>();
			this.hasVisibility = true;
		}
		this.audio = base.GetComponent<AudioSource>();
		ActiveCorpse.activeCorpses[base.gameObject] = this;
		this.landTint = base.gameObject.AddComponent<CorpseLandTint>();
		if (this.corpseType == ActiveCorpse.Types.Small)
		{
			this.minBounceSpeed = 25f;
			this.minWallSpeed = 14f;
			this.bounceFactor = 0.5f;
			this.maxBounceLaunchVelocity = -60f;
			return;
		}
		if (this.corpseType == ActiveCorpse.Types.Medium)
		{
			this.minBounceSpeed = 25f;
			this.minWallSpeed = 15f;
			this.bounceFactor = 0.45f;
			this.maxBounceLaunchVelocity = -50f;
			return;
		}
		if (this.corpseType == ActiveCorpse.Types.Large)
		{
			this.minBounceSpeed = 0f;
		}
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x0006EAF3 File Offset: 0x0006CCF3
	protected override void Start()
	{
		if (this.destroyed)
		{
			return;
		}
		base.Start();
		this.PlayStartAudio();
		this.hasStarted = true;
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x0006EB11 File Offset: 0x0006CD11
	private void OnEnable()
	{
		if (this.destroyed)
		{
			return;
		}
		if (this.hasStarted)
		{
			this.PlayStartAudio();
		}
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x0006EB2C File Offset: 0x0006CD2C
	private void OnDisable()
	{
		if (this.destroyed)
		{
			return;
		}
		this.blockAudio = false;
		this.spellBurnEffect = null;
		this.queuedBurn = false;
		this.hasEnteredWater = false;
		this.rb.bodyType = this.originalBodyType;
		if (this.colliderTriggerChanged)
		{
			if (this.hasCollider)
			{
				this.polyCollider.isTrigger = this.colliderWasTrigger;
			}
			this.colliderTriggerChanged = false;
		}
		if (this.isRecyclable)
		{
			this.state = ActiveCorpse.State.InAir;
			this.hasLandedEver = false;
			this.bouncingAway = false;
		}
	}

	// Token: 0x06001842 RID: 6210 RVA: 0x0006EBB4 File Offset: 0x0006CDB4
	private void OnDestroy()
	{
		ActiveCorpse.activeCorpses.Remove(base.gameObject);
	}

	// Token: 0x06001843 RID: 6211 RVA: 0x0006EBC8 File Offset: 0x0006CDC8
	private void Update()
	{
		if (this.queuedBurn)
		{
			this.DoQueuedBurnEffects();
		}
		if (!this.hasCollider)
		{
			return;
		}
		if (this.state == ActiveCorpse.State.InAir)
		{
			float num = this.velocityY;
			if (num >= 0.1f || num <= -0.1f)
			{
				this.prevVelocityY = this.velocityY;
			}
			Vector2 linearVelocity = this.rb.linearVelocity;
			this.velocityY = linearVelocity.y;
			this.prevVelocityX = this.velocityX;
			this.velocityX = linearVelocity.x;
			num = this.velocityY;
			if (num < 0.1f && num > -0.1f)
			{
				Vector3 position = base.transform.position;
				Bounds bounds = this.polyCollider.bounds;
				Vector3 v = new Vector3(position.x - 0.3f, bounds.min.y + 0.2f, position.z);
				Vector3 v2 = new Vector3(position.x + 0.3f, bounds.min.y + 0.2f, position.z);
				RaycastHit2D raycastHit2D = Helper.Raycast2D(v, -Vector2.up, 1f, 256);
				RaycastHit2D raycastHit2D2 = Helper.Raycast2D(v2, -Vector2.up, 1f, 256);
				if (raycastHit2D.collider != null || raycastHit2D2.collider != null)
				{
					if (Math.Abs(this.minBounceSpeed) > Mathf.Epsilon && this.prevVelocityY <= -this.minBounceSpeed && !this.noBounce)
					{
						this.Bounce();
					}
					else if (this.bounceAway)
					{
						this.Bounce();
					}
					else
					{
						this.Land();
					}
				}
				this.forceLandingTimer += Time.deltaTime;
				if (this.forceLandingTimer > 0.25f)
				{
					this.Land();
				}
			}
			else
			{
				this.forceLandingTimer = 0f;
			}
			num = this.velocityX;
			if (num < 0.1f && num > -0.1f && this.state == ActiveCorpse.State.InAir)
			{
				if (Mathf.Abs(this.prevVelocityX) > this.minWallSpeed && !this.noBounce)
				{
					this.WallHit();
				}
				else if (Mathf.Abs(this.prevVelocityX) > 0.2f && !this.didSoftWallHit)
				{
					this.WallHitSoft();
				}
			}
			if (this.bouncingAway && this.hasVisibility && !this.visibility.IsVisible)
			{
				this.Disable();
				return;
			}
			if (this.bounceAway && base.transform.position.y < this.bounceAwayTerminateY)
			{
				if (this.bounceAwayEffect)
				{
					this.bounceAwayEffect.SetActive(true);
					this.bounceAwayEffect.transform.parent = null;
				}
				this.Disable();
				return;
			}
		}
		else if (this.state == ActiveCorpse.State.Landed)
		{
			this.velocityY = this.rb.linearVelocity.y;
			if (this.velocityY < -0.1f || this.velocityY > 0.5f)
			{
				this.explodeTimer = 0f;
				if (!this.isInert)
				{
					this.Fall();
				}
			}
			if (this.explodes)
			{
				if (this.explodeTimer < this.explodePause)
				{
					this.explodeTimer += Time.deltaTime;
					return;
				}
				if (Math.Abs(this.anticTime) < Mathf.Epsilon)
				{
					this.Explode();
					return;
				}
				this.StartExplodeAntic();
				return;
			}
		}
		else if (this.state == ActiveCorpse.State.BounceFrame)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.BounceFling();
			return;
		}
		else if (this.state == ActiveCorpse.State.WallHitFrame)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.WallFling();
			return;
		}
		else if (this.state == ActiveCorpse.State.ExplodeAntic)
		{
			if (this.explodeTimer < this.anticTime)
			{
				this.explodeTimer += Time.deltaTime;
				return;
			}
			this.Explode();
		}
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x0006EFBC File Offset: 0x0006D1BC
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.destroyed)
		{
			return;
		}
		if (this.state == ActiveCorpse.State.InAir && collision.gameObject.layer == 17 && this.rb.linearVelocity.y <= -0.1f)
		{
			DamageHero component = collision.gameObject.GetComponent<DamageHero>();
			if (component != null && component.hazardType == HazardType.SPIKES && !component.noCorpseSpikeStick)
			{
				if (!this.explodeInstantlyOnSpikes)
				{
					this.SpikeLand();
					return;
				}
				this.Explode();
			}
		}
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x0006F03B File Offset: 0x0006D23B
	public void SetBlockAudio(bool blockAudio)
	{
		this.blockAudio = blockAudio;
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x0006F044 File Offset: 0x0006D244
	public void SetOnGround()
	{
		Vector3 position = base.transform.position;
		Bounds bounds = this.polyCollider.bounds;
		if (Physics2D.RaycastNonAlloc(new Vector3(position.x - 0.3f, bounds.min.y + 0.2f, position.z), Vector2.down, ActiveCorpse.buffer, 200f, 256) > 0)
		{
			RaycastHit2D raycastHit2D = ActiveCorpse.buffer[0];
			position.y -= raycastHit2D.distance - 1f;
			base.transform.position = position;
			return;
		}
		if (Physics2D.RaycastNonAlloc(new Vector3(position.x + 0.3f, bounds.min.y + 0.2f, position.z), Vector2.down, ActiveCorpse.buffer, 200f, 256) > 0)
		{
			RaycastHit2D raycastHit2D2 = ActiveCorpse.buffer[0];
			position.y -= raycastHit2D2.distance - 1f;
			base.transform.position = position;
		}
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x0006F160 File Offset: 0x0006D360
	private void PlayStartAudio()
	{
		if (this.blockAudio)
		{
			this.blockAudio = false;
			return;
		}
		if (!this.noVoice && this.startVoiceAudioTable)
		{
			if (this.audio)
			{
				this.audio.Stop();
				this.audio.clip = this.startVoiceAudioTable.SelectClip(false);
				this.audio.pitch = this.startVoiceAudioTable.SelectPitch();
				this.audio.volume = this.startVoiceAudioTable.SelectVolume();
				this.audio.loop = false;
				this.audio.Play();
			}
			else
			{
				this.startVoiceAudioTable.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, false, 1f, null);
			}
		}
		this.startAudioClips.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		if (this.HasAnimator)
		{
			this.Animator.Play("Death Air");
		}
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x0006F268 File Offset: 0x0006D468
	private void Land()
	{
		this.hasLandedEver = true;
		if (this.HasAnimator)
		{
			if (this.inAirZone)
			{
				this.Animator.Play((this.Animator.GetClipByName("Death Land Air") != null) ? "Death Land Air" : "Death Land");
			}
			else
			{
				this.Animator.Play((this.hasLandedEver && this.useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
			}
		}
		if (!this.hasEnteredWater)
		{
			this.landAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		if (!this.noVoice && this.landVoiceAudioTable && !this.hasPlayedLandVoice && !this.hasEnteredWater)
		{
			if (this.audio)
			{
				this.audio.Stop();
				this.audio.clip = this.landVoiceAudioTable.SelectClip(false);
				this.audio.pitch = this.landVoiceAudioTable.SelectPitch();
				this.audio.volume = this.landVoiceAudioTable.SelectVolume();
				this.audio.loop = false;
				this.audio.Play();
			}
			else
			{
				this.landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			this.hasPlayedLandVoice = true;
		}
		this.state = ActiveCorpse.State.Landed;
		if (this.activateOnLand && !this.didActivateOnLand)
		{
			this.activateOnLand.SetActive(true);
			this.didActivateOnLand = true;
		}
		if (!this.hasLanded)
		{
			this.hasLanded = true;
			base.ActivatePickup();
		}
		if (!this.noTint)
		{
			this.landTint.Landed(false);
		}
		Vector2 linearVelocity = this.rb.linearVelocity;
		linearVelocity.x = 0f;
		this.rb.linearVelocity = linearVelocity;
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x0006F440 File Offset: 0x0006D640
	private void Fall()
	{
		if (this.HasAnimator)
		{
			if (!this.bouncingAway)
			{
				this.Animator.PlayFromFrame("Death Air", 0);
				this.Animator.PlayFromFrame((this.hasLandedEver && this.useAfterBounceAnim) ? "Death Air AfterBounce" : "Death Air", 0);
			}
			else
			{
				this.Animator.PlayFromFrame((this.Animator.GetClipByName("Death BounceAway") != null) ? "Death BounceAway" : "Death Air", 0);
			}
		}
		this.state = ActiveCorpse.State.InAir;
		if (this.hasLanded)
		{
			this.hasLanded = false;
			base.DeactivatePickup();
		}
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x0006F4E0 File Offset: 0x0006D6E0
	private void Bounce()
	{
		this.hasLandedEver = true;
		if (this.explodes && (!this.HasAnimator || this.Animator.GetClipByName("Death Land") == null))
		{
			this.Explode();
			return;
		}
		this.rb.linearVelocity = new Vector2(0f, 0f);
		this.rb.bodyType = RigidbodyType2D.Kinematic;
		if (this.HasAnimator)
		{
			this.Animator.Play((this.hasLandedEver && this.useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
		}
		if (!this.hasEnteredWater)
		{
			this.landAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		if (!this.noVoice && !this.hasPlayedLandVoice && !this.hasEnteredWater && this.landVoiceAudioTable)
		{
			if (this.audio)
			{
				this.audio.Stop();
				this.audio.clip = this.landVoiceAudioTable.SelectClip(false);
				this.audio.pitch = this.landVoiceAudioTable.SelectPitch();
				this.audio.volume = this.landVoiceAudioTable.SelectVolume();
				this.audio.loop = false;
				this.audio.Play();
			}
			else
			{
				this.landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			this.hasPlayedLandVoice = true;
		}
		this.timer = 0.08f;
		if (this.activateOnLand && !this.didActivateOnLand)
		{
			this.activateOnLand.SetActive(true);
			this.activateOnLand.transform.parent = null;
			this.didActivateOnLand = true;
		}
		this.state = ActiveCorpse.State.BounceFrame;
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x0006F6A4 File Offset: 0x0006D8A4
	private void BounceFling()
	{
		base.transform.Translate(0f, 0.2f, 0f);
		if (this.bounceAway && this.prevVelocityY > -25f)
		{
			this.prevVelocityY = -25f;
		}
		if (this.prevVelocityY < this.maxBounceLaunchVelocity)
		{
			this.prevVelocityY = this.maxBounceLaunchVelocity;
		}
		this.rb.linearVelocity = new Vector2(this.prevVelocityX * this.bounceFactor, -(this.prevVelocityY * this.bounceFactor));
		this.rb.bodyType = RigidbodyType2D.Dynamic;
		this.prevVelocityY = 0f;
		if (this.bounceAway)
		{
			this.SetColliderTrigger();
			this.bouncingAway = true;
		}
		this.Fall();
	}

	// Token: 0x0600184C RID: 6220 RVA: 0x0006F764 File Offset: 0x0006D964
	private void WallHit()
	{
		this.rb.linearVelocity = new Vector2(0f, 0f);
		this.rb.bodyType = RigidbodyType2D.Kinematic;
		if (this.HasAnimator)
		{
			this.Animator.Play("Death Land");
			this.Animator.Stop();
		}
		if (!this.hasEnteredWater)
		{
			this.landAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		this.timer = 0.08f;
		this.state = ActiveCorpse.State.WallHitFrame;
	}

	// Token: 0x0600184D RID: 6221 RVA: 0x0006F7EC File Offset: 0x0006D9EC
	private void WallHitSoft()
	{
		this.rb.linearVelocity = new Vector2(this.prevVelocityX * -this.bounceFactor / 2f, 0f);
		this.prevVelocityX = 0f;
		this.didSoftWallHit = true;
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x0006F82C File Offset: 0x0006DA2C
	private void WallFling()
	{
		this.rb.linearVelocity = new Vector2(this.prevVelocityX * -this.bounceFactor / 1.75f, 0f);
		this.rb.bodyType = RigidbodyType2D.Dynamic;
		base.transform.localEulerAngles = Vector3.zero;
		this.prevVelocityY = 0f;
		this.prevVelocityX = 0f;
		this.Fall();
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x0006F89C File Offset: 0x0006DA9C
	private void SpikeLand()
	{
		base.transform.Translate(0f, -0.25f, 0f);
		if (this.HasAnimator)
		{
			this.Animator.Play((this.hasLandedEver && this.useAfterBounceAnim) ? "Death Land AfterBounce" : "Death Land");
		}
		if (this.corpseType == ActiveCorpse.Types.Object)
		{
			Audio.ObjectSpikeLandAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			Transform transform = base.transform;
			Vector3 position = transform.position;
			transform.position = new Vector3(position.x, position.y - 0.5f, position.z);
		}
		else
		{
			Audio.CorpseSpikeLandAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		this.rb.linearVelocity = new Vector2(0f, 0f);
		this.rb.angularVelocity = 0f;
		this.rb.bodyType = RigidbodyType2D.Kinematic;
		if (!this.noVoice && this.landVoiceAudioTable && !this.hasPlayedLandVoice)
		{
			if (this.audio)
			{
				this.audio.Stop();
				this.landVoiceAudioTable.PlayOneShot(this.audio, this.voicePitchOffset, false);
			}
			else
			{
				this.landVoiceAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			this.hasPlayedLandVoice = true;
		}
		JitterSelfForTime jitterSelfForTime = base.gameObject.GetComponent<JitterSelfForTime>();
		if (!jitterSelfForTime)
		{
			jitterSelfForTime = JitterSelfForTime.AddHandler(base.gameObject, new Vector3(0.075f, 0.075f), 0.25f, 30f);
		}
		jitterSelfForTime.StartTimedJitter();
		if (!this.noTint)
		{
			this.landTint.Landed(false);
		}
		this.state = ActiveCorpse.State.SpikeLand;
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x0006FA58 File Offset: 0x0006DC58
	private void StartExplodeAntic()
	{
		this.explodeTimer = 0f;
		if (this.anticParticle)
		{
			this.anticParticle.Play();
		}
		if (this.anticObject)
		{
			this.anticObject.SetActive(true);
		}
		this.state = ActiveCorpse.State.ExplodeAntic;
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x0006FAA8 File Offset: 0x0006DCA8
	private void Explode()
	{
		if (this.anticParticle)
		{
			this.anticParticle.Stop();
			this.anticParticle.gameObject.transform.parent = null;
		}
		if (this.anticObject)
		{
			this.anticObject.SetActive(false);
		}
		if (this.explosionObject)
		{
			this.explosionObject.SetActive(true);
			this.explosionObject.transform.parent = null;
		}
		if (!this.corpseRemains)
		{
			this.Disable();
		}
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x0006FB34 File Offset: 0x0006DD34
	public void SetInAirZone(bool isInAirZone)
	{
		this.inAirZone = isInAirZone;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x0006FB3D File Offset: 0x0006DD3D
	public void SetInert(bool setIsInert)
	{
		this.isInert = setIsInert;
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x0006FB46 File Offset: 0x0006DD46
	public void SetNoTint(bool setNoTint)
	{
		this.noTint = setNoTint;
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x0006FB4F File Offset: 0x0006DD4F
	public void SetBounceAway(bool setBounceAway)
	{
		this.bounceAway = setBounceAway;
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x0006FB58 File Offset: 0x0006DD58
	public void QueueBurnEffects(tk2dSprite enemySprite, AttackTypes attackType, NailElements nailElement, GameObject damageSource, float scale, TagDamageTaker enemyTagDamageTaker)
	{
		this.queuedBurn = true;
		this.queuedBurnAttackType = attackType;
		this.queuedBurnNailElement = nailElement;
		this.queuedBurnDamageSource = damageSource;
		this.queuedBurnScale = scale;
		this.queuedTagDamageTaker = enemyTagDamageTaker;
		this.hasQueuedTagDamageTaker = (this.queuedTagDamageTaker != null);
		if (this.hasQueuedTagDamageTaker)
		{
			this.queuedTagDamage.Clear();
			this.queuedTagDamage.AddRange(this.queuedTagDamageTaker.TaggedDamage.Keys);
		}
		if (!enemySprite || !enemySprite.HasKeywordsDefined || !this.sprite)
		{
			return;
		}
		Color color = enemySprite.color * this.sprite.color.grayscale;
		color.a = this.sprite.color.a;
		this.sprite.color = color;
		enemySprite.MoveKeywords(this.sprite);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		enemySprite.GetComponent<MeshRenderer>().GetPropertyBlock(materialPropertyBlock);
		float @float = materialPropertyBlock.GetFloat(ActiveCorpse._blackThreadAmountProp);
		materialPropertyBlock.Clear();
		this.renderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(ActiveCorpse._blackThreadAmountProp, @float);
		this.renderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x0006FC88 File Offset: 0x0006DE88
	private void DoQueuedBurnEffects()
	{
		this.queuedBurn = false;
		AttackTypes attackTypes = this.queuedBurnAttackType;
		this.queuedBurnAttackType = AttackTypes.Nail;
		NailElements nailElements = this.queuedBurnNailElement;
		this.queuedBurnNailElement = NailElements.None;
		GameObject gameObject = this.queuedBurnDamageSource;
		this.queuedBurnDamageSource = null;
		float scale = this.queuedBurnScale;
		this.queuedBurnScale = 0f;
		Transform transform = base.transform;
		bool flag = false;
		if (this.queuedTagDamageTaker)
		{
			foreach (KeyValuePair<DamageTag, DamageTagInfo> keyValuePair in this.queuedTagDamageTaker.TaggedDamage)
			{
				ParticleEffectsLerpEmission corpseBurnEffect = keyValuePair.Key.CorpseBurnEffect;
				if (corpseBurnEffect)
				{
					keyValuePair.Key.SpawnDeathEffects(transform.transform.position);
					this.DoBurnEffect(transform, this.effectPos, corpseBurnEffect, scale, false);
					if (keyValuePair.Key.NailElement == NailElements.Fire || keyValuePair.Key.SpecialDamageType == DamageTag.SpecialDamageTypes.Lightning)
					{
						flag = true;
					}
				}
			}
			this.queuedTagDamageTaker = null;
		}
		else if (this.hasQueuedTagDamageTaker)
		{
			foreach (DamageTag damageTag in this.queuedTagDamage)
			{
				ParticleEffectsLerpEmission corpseBurnEffect2 = damageTag.CorpseBurnEffect;
				if (corpseBurnEffect2)
				{
					if (damageTag.NailElement == NailElements.Fire || damageTag.SpecialDamageType == DamageTag.SpecialDamageTypes.Lightning)
					{
						flag = true;
					}
					damageTag.SpawnDeathEffects(transform.transform.position);
					this.DoBurnEffect(transform, this.effectPos, corpseBurnEffect2, scale, false);
					break;
				}
			}
			this.queuedTagDamage.Clear();
		}
		this.hasQueuedTagDamageTaker = false;
		if (!flag)
		{
			if (attackTypes == AttackTypes.Spell)
			{
				flag = true;
				this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.SpellBurnEffect, scale, false);
			}
			else if (attackTypes == AttackTypes.Fire || attackTypes == AttackTypes.Explosion || nailElements == NailElements.Fire)
			{
				flag = true;
				this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.FireBurnEffect, scale, true);
			}
			else if (attackTypes == AttackTypes.Lightning)
			{
				flag = true;
				this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.ZapBurnEffect, scale, false);
			}
			else if (gameObject)
			{
				if (gameObject.CompareTag("Explosion"))
				{
					flag = true;
					this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.FireBurnEffect, scale, false);
				}
				else
				{
					DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
					if (component)
					{
						ToolItem representingTool = component.RepresentingTool;
						if (representingTool && representingTool.Type == ToolItemType.Skill && Gameplay.SpellCrest.IsEquipped)
						{
							flag = true;
							this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.SoulBurnEffect, scale, false);
						}
						else if (component.ZapDamageTicks > 0)
						{
							flag = true;
							this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.ZapBurnEffect, scale, false);
						}
						else if (component.PoisonDamageTicks > 0)
						{
							this.DoBurnEffect(transform, this.effectPos, GlobalSettings.Corpse.PoisonBurnEffect, scale, false);
						}
					}
				}
			}
		}
		if (flag && this.sprite != null)
		{
			if (base.IsBlackThreaded)
			{
				this.sprite.color *= GlobalSettings.Corpse.SpellBurnColorBlackThread;
				return;
			}
			this.sprite.color *= GlobalSettings.Corpse.SpellBurnColor;
		}
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x0006FFE4 File Offset: 0x0006E1E4
	private void DoBurnEffect(Transform corpseTrans, Vector2 localPos, ParticleEffectsLerpEmission spellBurnEffect, float scale, bool isFire = false)
	{
		float spellBurnDuration = GlobalSettings.Corpse.SpellBurnDuration;
		if (!spellBurnEffect || spellBurnDuration < 0f)
		{
			return;
		}
		ParticleEffectsLerpEmission particleEffectsLerpEmission = spellBurnEffect.Spawn(corpseTrans);
		Transform transform = particleEffectsLerpEmission.transform;
		transform.SetLocalPosition2D(localPos);
		if (isFire)
		{
			this.RecordFireBurn(particleEffectsLerpEmission, spellBurnDuration);
		}
		FollowTransform component = particleEffectsLerpEmission.GetComponent<FollowTransform>();
		if (component)
		{
			transform.SetParent(null, true);
			transform.SetRotation2D(spellBurnEffect.transform.localEulerAngles.z);
			component.Target = corpseTrans;
			component.ClearTargetOnDisable();
		}
		ParticleEffectsScaleToCollider component2 = particleEffectsLerpEmission.GetComponent<ParticleEffectsScaleToCollider>();
		Collider2D component3 = corpseTrans.GetComponent<Collider2D>();
		if (component2 && component3)
		{
			component2.SetScaleToCollider(component3);
		}
		else
		{
			particleEffectsLerpEmission.TotalMultiplier = scale;
			Vector3 localScale = spellBurnEffect.transform.localScale;
			localScale.x *= scale;
			localScale.y *= scale;
			transform.localScale = localScale;
		}
		particleEffectsLerpEmission.Play(spellBurnDuration);
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x000700CF File Offset: 0x0006E2CF
	private void RecordFireBurn(ParticleEffectsLerpEmission spellBurnEffect, float duration)
	{
		this.spellBurnEffect = spellBurnEffect;
		this.burnEndTime = Time.timeAsDouble + (double)duration;
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000700E8 File Offset: 0x0006E2E8
	public void Acid()
	{
		this.hasEnteredWater = true;
		this.SetColliderTrigger();
		if (this.state == ActiveCorpse.State.Landed)
		{
			this.state = ActiveCorpse.State.InAir;
		}
		if (Time.timeAsDouble < this.burnEndTime && this.spellBurnEffect != null)
		{
			this.burnEndTime = 0.0;
			this.spellBurnEffect.Stop();
			ExtinguishEffectSpawner component = this.spellBurnEffect.GetComponent<ExtinguishEffectSpawner>();
			if (component != null)
			{
				component.PlayEffect(base.transform.position);
			}
			this.spellBurnEffect = null;
		}
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x00070174 File Offset: 0x0006E374
	private void SetColliderTrigger()
	{
		if (this.hasCollider)
		{
			if (this.polyCollider.isTrigger)
			{
				return;
			}
			if (!this.colliderTriggerChanged)
			{
				this.colliderWasTrigger = this.polyCollider.isTrigger;
			}
			this.polyCollider.isTrigger = true;
			this.colliderTriggerChanged = true;
		}
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x000701C3 File Offset: 0x0006E3C3
	private void Disable()
	{
		if (this.isRecyclable)
		{
			base.gameObject.Recycle();
			base.SetIsBlackThreaded(false);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001715 RID: 5909
	public ActiveCorpse.Types corpseType;

	// Token: 0x04001716 RID: 5910
	public bool noTint;

	// Token: 0x04001717 RID: 5911
	public bool noBounce;

	// Token: 0x04001718 RID: 5912
	[Space]
	[SerializeField]
	private bool isRecyclable;

	// Token: 0x04001719 RID: 5913
	[SerializeField]
	private bool useVisiblity;

	// Token: 0x0400171A RID: 5914
	public bool bounceAway;

	// Token: 0x0400171B RID: 5915
	public bool useAfterBounceAnim;

	// Token: 0x0400171C RID: 5916
	[ModifiableProperty]
	[Conditional("bounceAway", true, false, true)]
	public float bounceAwayTerminateY;

	// Token: 0x0400171D RID: 5917
	[ModifiableProperty]
	[Conditional("bounceAway", true, false, true)]
	public GameObject bounceAwayEffect;

	// Token: 0x0400171E RID: 5918
	private bool bouncingAway;

	// Token: 0x0400171F RID: 5919
	private bool inAirZone;

	// Token: 0x04001720 RID: 5920
	private float minBounceSpeed;

	// Token: 0x04001721 RID: 5921
	private float minWallSpeed;

	// Token: 0x04001722 RID: 5922
	private float maxBounceLaunchVelocity;

	// Token: 0x04001723 RID: 5923
	private float bounceFactor;

	// Token: 0x04001724 RID: 5924
	[SerializeField]
	protected AudioSource audioPlayerPrefab;

	// Token: 0x04001725 RID: 5925
	[SerializeField]
	private bool noVoice;

	// Token: 0x04001726 RID: 5926
	[SerializeField]
	private AudioEventRandom startAudioClips;

	// Token: 0x04001727 RID: 5927
	[SerializeField]
	private RandomAudioClipTable startVoiceAudioTable;

	// Token: 0x04001728 RID: 5928
	[SerializeField]
	private RandomAudioClipTable landAudioTable;

	// Token: 0x04001729 RID: 5929
	[SerializeField]
	private RandomAudioClipTable landVoiceAudioTable;

	// Token: 0x0400172A RID: 5930
	[SerializeField]
	private float voicePitchOffset;

	// Token: 0x0400172B RID: 5931
	[SerializeField]
	private GameObject activateOnLand;

	// Token: 0x0400172C RID: 5932
	[Space]
	public bool explodes;

	// Token: 0x0400172D RID: 5933
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public float explodePause;

	// Token: 0x0400172E RID: 5934
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public float anticTime;

	// Token: 0x0400172F RID: 5935
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public ParticleSystem anticParticle;

	// Token: 0x04001730 RID: 5936
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public GameObject anticObject;

	// Token: 0x04001731 RID: 5937
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public GameObject explosionObject;

	// Token: 0x04001732 RID: 5938
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public bool corpseRemains;

	// Token: 0x04001733 RID: 5939
	[ModifiableProperty]
	[Conditional("explodes", true, false, true)]
	public bool explodeInstantlyOnSpikes;

	// Token: 0x04001734 RID: 5940
	[Space]
	[SerializeField]
	private Vector2 effectPos;

	// Token: 0x04001735 RID: 5941
	private const string DEATH_AIR_CLIP = "Death Air";

	// Token: 0x04001736 RID: 5942
	private const string DEATH_LAND_CLIP = "Death Land";

	// Token: 0x04001737 RID: 5943
	private const string DEATH_LAND_AIR_CLIP = "Death Land Air";

	// Token: 0x04001738 RID: 5944
	private const string DEATH_BOUNCE_AWAY_CLIP = "Death BounceAway";

	// Token: 0x04001739 RID: 5945
	private const string DEATH_AIR_AFTER_BOUNCE_CLIP = "Death Air AfterBounce";

	// Token: 0x0400173A RID: 5946
	private const string DEATH_LAND_AFTER_BOUNCE_CLIP = "Death Land AfterBounce";

	// Token: 0x0400173B RID: 5947
	private ActiveCorpse.State state;

	// Token: 0x0400173C RID: 5948
	private float timer;

	// Token: 0x0400173D RID: 5949
	private float explodeTimer;

	// Token: 0x0400173E RID: 5950
	private float forceLandingTimer;

	// Token: 0x0400173F RID: 5951
	private float velocityX;

	// Token: 0x04001740 RID: 5952
	private float prevVelocityX;

	// Token: 0x04001741 RID: 5953
	private float velocityY;

	// Token: 0x04001742 RID: 5954
	private float prevVelocityY;

	// Token: 0x04001743 RID: 5955
	private bool hasLanded;

	// Token: 0x04001744 RID: 5956
	private bool hasPlayedLandVoice;

	// Token: 0x04001745 RID: 5957
	private bool didSoftWallHit;

	// Token: 0x04001746 RID: 5958
	private bool didActivateOnLand;

	// Token: 0x04001747 RID: 5959
	private bool isInert;

	// Token: 0x04001748 RID: 5960
	private bool hasStarted;

	// Token: 0x04001749 RID: 5961
	private bool hasEnteredWater;

	// Token: 0x0400174A RID: 5962
	private bool hasLandedEver;

	// Token: 0x0400174B RID: 5963
	private PolygonCollider2D polyCollider;

	// Token: 0x0400174C RID: 5964
	private bool hasCollider;

	// Token: 0x0400174D RID: 5965
	private AudioSource audio;

	// Token: 0x0400174E RID: 5966
	private Rigidbody2D rb;

	// Token: 0x0400174F RID: 5967
	private tk2dSprite sprite;

	// Token: 0x04001750 RID: 5968
	private MeshRenderer renderer;

	// Token: 0x04001751 RID: 5969
	private CorpseLandTint landTint;

	// Token: 0x04001752 RID: 5970
	private bool blockAudio;

	// Token: 0x04001753 RID: 5971
	private bool queuedBurn;

	// Token: 0x04001754 RID: 5972
	private AttackTypes queuedBurnAttackType;

	// Token: 0x04001755 RID: 5973
	private NailElements queuedBurnNailElement;

	// Token: 0x04001756 RID: 5974
	private GameObject queuedBurnDamageSource;

	// Token: 0x04001757 RID: 5975
	private float queuedBurnScale;

	// Token: 0x04001758 RID: 5976
	private bool hasQueuedTagDamageTaker;

	// Token: 0x04001759 RID: 5977
	private TagDamageTaker queuedTagDamageTaker;

	// Token: 0x0400175A RID: 5978
	private List<DamageTag> queuedTagDamage = new List<DamageTag>();

	// Token: 0x0400175B RID: 5979
	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	// Token: 0x0400175C RID: 5980
	private static Dictionary<GameObject, ActiveCorpse> activeCorpses = new Dictionary<GameObject, ActiveCorpse>();

	// Token: 0x0400175D RID: 5981
	private bool colliderTriggerChanged;

	// Token: 0x0400175E RID: 5982
	private bool colliderWasTrigger;

	// Token: 0x0400175F RID: 5983
	private bool hasVisibility;

	// Token: 0x04001760 RID: 5984
	private VisibilityGroup visibility;

	// Token: 0x04001761 RID: 5985
	private RigidbodyType2D originalBodyType;

	// Token: 0x04001762 RID: 5986
	private bool destroyed;

	// Token: 0x04001763 RID: 5987
	private static RaycastHit2D[] buffer = new RaycastHit2D[1];

	// Token: 0x04001764 RID: 5988
	private ParticleEffectsLerpEmission spellBurnEffect;

	// Token: 0x04001765 RID: 5989
	private double burnEndTime;

	// Token: 0x02001590 RID: 5520
	public enum Types
	{
		// Token: 0x040087C4 RID: 34756
		Small,
		// Token: 0x040087C5 RID: 34757
		Medium,
		// Token: 0x040087C6 RID: 34758
		Large,
		// Token: 0x040087C7 RID: 34759
		Object
	}

	// Token: 0x02001591 RID: 5521
	private enum State
	{
		// Token: 0x040087C9 RID: 34761
		InAir,
		// Token: 0x040087CA RID: 34762
		Landed,
		// Token: 0x040087CB RID: 34763
		BounceFrame,
		// Token: 0x040087CC RID: 34764
		WallHitFrame,
		// Token: 0x040087CD RID: 34765
		ExplodeAntic,
		// Token: 0x040087CE RID: 34766
		SpikeLand
	}
}
