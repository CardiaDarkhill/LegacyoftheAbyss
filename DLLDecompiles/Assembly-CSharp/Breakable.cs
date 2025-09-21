using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200049F RID: 1183
public class Breakable : DebugDrawColliderRuntimeAdder, IHitResponder, IBreakerBreakable
{
	// Token: 0x14000086 RID: 134
	// (add) Token: 0x06002AE1 RID: 10977 RVA: 0x000BA9F0 File Offset: 0x000B8BF0
	// (remove) Token: 0x06002AE2 RID: 10978 RVA: 0x000BAA28 File Offset: 0x000B8C28
	public event Action<HitInstance> BrokenHit;

	// Token: 0x14000087 RID: 135
	// (add) Token: 0x06002AE3 RID: 10979 RVA: 0x000BAA60 File Offset: 0x000B8C60
	// (remove) Token: 0x06002AE4 RID: 10980 RVA: 0x000BAA98 File Offset: 0x000B8C98
	public event Action AlreadyBroken;

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06002AE5 RID: 10981 RVA: 0x000BAACD File Offset: 0x000B8CCD
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Basic;
		}
	}

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x06002AE6 RID: 10982 RVA: 0x000BAAD0 File Offset: 0x000B8CD0
	public UnityEvent OnHit
	{
		get
		{
			return this.onHit;
		}
	}

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x06002AE7 RID: 10983 RVA: 0x000BAAD8 File Offset: 0x000B8CD8
	public UnityEvent OnBreak
	{
		get
		{
			return this.onBreak;
		}
	}

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x06002AE8 RID: 10984 RVA: 0x000BAAE0 File Offset: 0x000B8CE0
	public UnityEvent OnBroken
	{
		get
		{
			return this.onBroken;
		}
	}

	// Token: 0x17000506 RID: 1286
	// (get) Token: 0x06002AE9 RID: 10985 RVA: 0x000BAAE8 File Offset: 0x000B8CE8
	public bool IsBroken
	{
		get
		{
			return this.isBroken;
		}
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x000BAAF0 File Offset: 0x000B8CF0
	protected void Reset()
	{
		this.inertBackgroundThreshold = 1f;
		this.inertForegroundThreshold = -1f;
		this.effectOffset = new Vector3(0f, 0.5f, 0f);
		this.flingSpeedMin = 10f;
		this.flingSpeedMax = 17f;
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000BAB43 File Offset: 0x000B8D43
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.effectOffset), 0.15f);
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000BAB60 File Offset: 0x000B8D60
	private void OnValidate()
	{
		if (this.freezeMoment > 0)
		{
			this.breakFreezeMoment = (FreezeMomentTypes)this.freezeMoment;
			this.freezeMoment = 0;
		}
		if (this.emitNoise)
		{
			this.emitNoise = false;
			this.noiseRadius = 0f;
		}
		if (this.hitsToBreak <= 0)
		{
			this.hitsToBreak = 1;
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000BABB4 File Offset: 0x000B8DB4
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		Transform transform = base.transform;
		this.bodyCollider = base.GetComponent<Collider2D>();
		this.flingDrops.SetAllActive(false);
		this.remnantParts.SetAllActive(false);
		this.debrisParts.SetAllActive(false);
		this.debrisPartsAlt.SetAllActive(false);
		if (!this.persistent && !this.silkDropsPersistent)
		{
			this.persistent = base.GetComponent<PersistentBoolItem>();
		}
		if (transform.Find("rosary") != null && transform.Find("rosary").gameObject.activeSelf)
		{
			this.smallGeoDrops = new MinMaxInt(8, 10);
			if (!this.persistent)
			{
				this.persistent = base.gameObject.AddComponent<PersistentBoolItem>();
			}
		}
		if (this.persistent != null)
		{
			this.persistent.OnGetSaveState += delegate(out bool val)
			{
				val = this.isBroken;
			};
			this.persistent.OnSetSaveState += delegate(bool val)
			{
				this.isBroken = val;
				if (this.isBroken)
				{
					this.SetAlreadyBroken();
				}
			};
		}
		if (this.hitsToBreak > 1)
		{
			this.hitJitter = base.GetComponent<JitterSelfForTime>();
			if (!this.hitJitter && this.autoAddJitterComponent)
			{
				this.hitJitter = JitterSelfForTime.AddHandler(base.gameObject, new Vector3(0.1f, 0.1f, 0f), 0.2f, 30f);
			}
		}
		this.silkHits = 0;
		bool flag = this.IsPartVisible(this.silkDropsCondition);
		bool flag2 = this.IsPartVisible(this.threadThinObject);
		if (flag || flag2)
		{
			this.hitsToBreak++;
			this.silkHits = 1;
			if (flag)
			{
				this.hitsToBreak++;
				this.silkHits = 2;
			}
			if (this.silkDropsPersistent)
			{
				this.silkDropsPersistent.OnGetSaveState += delegate(out bool val)
				{
					val = (this.silkHits < this.startSilkHits || this.isBroken);
				};
				this.silkDropsPersistent.OnSetSaveState += delegate(bool val)
				{
					this.silkDropsCondition.SetActive(!val);
					if (this.threadThinObject)
					{
						this.threadThinObject.SetActive(!val);
					}
					if (!val)
					{
						return;
					}
					this.hitsLeft -= this.silkHits;
					this.hitsToBreak -= this.silkHits;
					this.silkHits = 0;
				};
				ResetDynamicHierarchy resetter = base.gameObject.AddComponent<ResetDynamicHierarchy>();
				this.silkDropsPersistent.SemiPersistentReset += delegate()
				{
					this.SetCooldown();
					this.isBroken = false;
					this.hitsLeft = this.hitsToBreak;
					this.silkHits = this.startSilkHits;
					resetter.DoReset(true);
				};
			}
		}
		this.startSilkHits = this.silkHits;
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000BADF3 File Offset: 0x000B8FF3
	private void OnEnable()
	{
		this.SetCooldown();
		if (this.resetOnEnable)
		{
			this.isBroken = false;
		}
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000BAE0A File Offset: 0x000B900A
	public void SetCooldown()
	{
		this.hitCooldownEndTime = Time.timeAsDouble + (double)(5f * Time.fixedDeltaTime);
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000BAE24 File Offset: 0x000B9024
	public void SetHitCoolDown()
	{
		this.hitCooldownEndTime = Time.timeAsDouble + (double)this.hitCoolDown;
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000BAE3C File Offset: 0x000B903C
	protected void Start()
	{
		this.CreateAdditionalDebrisParts(this.debrisParts);
		bool flag;
		if (this.ignoreDamagers)
		{
			flag = false;
			if (!base.gameObject.GetComponent<NonBouncer>())
			{
				base.gameObject.AddComponent<NonBouncer>();
			}
		}
		else if (this.useHeroPlane)
		{
			flag = !base.transform.IsOnHeroPlane();
		}
		else
		{
			float z = base.transform.position.z;
			flag = ((Math.Abs(this.inertBackgroundThreshold) > Mathf.Epsilon && z > this.inertBackgroundThreshold) || (Math.Abs(this.inertForegroundThreshold) > Mathf.Epsilon && z < this.inertForegroundThreshold));
		}
		if (flag)
		{
			BoxCollider2D component = base.GetComponent<BoxCollider2D>();
			if (component != null)
			{
				component.enabled = false;
			}
			Object.Destroy(this);
			return;
		}
		Transform transform = base.transform;
		this.angleOffset *= Mathf.Sign(transform.localScale.x);
		if (this.breakEffectPrefab != null)
		{
			this.breakEffects = Object.Instantiate<GameObject>(this.breakEffectPrefab, transform.position, transform.rotation);
			this.breakEffects.SetActive(false);
		}
		this.hitsLeft = this.hitsToBreak;
		tk2dSprite[] componentsInChildren = base.GetComponentsInChildren<tk2dSprite>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ForceBuild();
		}
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000BAF94 File Offset: 0x000B9194
	protected virtual void CreateAdditionalDebrisParts(List<GameObject> sourceDebrisParts)
	{
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000BAF96 File Offset: 0x000B9196
	[ContextMenu("Break Self")]
	public void BreakSelf()
	{
		if (this.isBroken)
		{
			return;
		}
		this.Break(70f, 110f, 1f);
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000BAFB6 File Offset: 0x000B91B6
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if (this.immuneToBreakableBreaker)
		{
			return;
		}
		this.BreakSelf();
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000BAFC8 File Offset: 0x000B91C8
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if (this.immuneToBreakableBreaker)
		{
			return;
		}
		float direction = (float)((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
		this.Hit(new HitInstance
		{
			Source = breaker.gameObject,
			AttackType = AttackTypes.Generic,
			DamageDealt = 1,
			Direction = direction,
			Multiplier = 1f
		});
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000BB050 File Offset: 0x000B9250
	public void SetAlreadyBroken()
	{
		this.silkHits = 0;
		this.wasAlreadyBroken = true;
		this.SetStaticPartsActivation(true);
		foreach (GameObject gameObject in this.flingDrops)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(true);
			}
		}
		if (this.AlreadyBroken != null)
		{
			this.AlreadyBroken();
		}
		if (this.onBroken != null)
		{
			this.onBroken.Invoke();
		}
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000BB0C4 File Offset: 0x000B92C4
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.firstHitOnly && !damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (this.ignoreDamagers)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage && damageInstance.IsManualTrigger)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble <= this.hitCooldownEndTime)
		{
			if (damageInstance.AttackType == AttackTypes.Spikes)
			{
				DamageEnemies component = damageInstance.Source.GetComponent<DamageEnemies>();
				if (component)
				{
					component.PreventDamage(this);
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		if (this.breakableRange && this.breakableRange.isActiveAndEnabled && !this.breakableRange.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (this.isBroken)
		{
			return IHitResponder.Response.None;
		}
		foreach (Breakable breakable in this.requireBroken)
		{
			if (breakable && !breakable.isBroken)
			{
				return IHitResponder.Response.None;
			}
		}
		float impactAngle = damageInstance.Direction;
		float num = damageInstance.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		bool flag = false;
		if (damageInstance.AttackType == AttackTypes.Spell && this.spellHitEffectPrefab != null)
		{
			this.spellHitEffectPrefab.Spawn(base.transform.position).SetPositionZ(0.0031f);
			flag = true;
		}
		if (!flag)
		{
			if (!damageInstance.IsNailDamage && damageInstance.AttackType != AttackTypes.Generic)
			{
				impactAngle = 90f;
				num = 1f;
			}
			Vector3 position = (damageInstance.Source.transform.position + base.transform.TransformPoint(this.effectOffset)) * 0.5f;
			Breakable.SpawnNailHitEffect(this.strikeEffectPrefab, position, impactAngle);
			Breakable.SpawnNailHitEffect(this.nailHitEffectPrefab, position, impactAngle);
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		Transform transform = this.dustHitRegularPrefab;
		float num2;
		float num3;
		Vector3 euler;
		if (cardinalDirection == 2)
		{
			this.angleOffset *= -1f;
			num2 = 120f;
			num3 = 160f;
			euler = new Vector3(180f, 90f, 270f);
		}
		else if (cardinalDirection == 0)
		{
			num2 = 30f;
			num3 = 70f;
			euler = new Vector3(0f, 90f, 270f);
		}
		else if (cardinalDirection == 1)
		{
			this.angleOffset = 0f;
			num2 = 70f;
			num3 = 110f;
			num *= 1.5f;
			euler = new Vector3(270f, 90f, 270f);
		}
		else
		{
			this.angleOffset = 0f;
			num2 = 160f;
			num3 = 380f;
			transform = this.dustHitDownPrefab;
			euler = new Vector3(-72.5f, -180f, -180f);
		}
		Vector3 position2 = base.transform.TransformPoint(this.effectOffset);
		if (transform != null)
		{
			transform.Spawn(position2, Quaternion.Euler(euler));
		}
		GameManager instance = GameManager.instance;
		this.SetHitCoolDown();
		if (this.onHit != null)
		{
			this.onHit.Invoke();
		}
		if (this.hitJitter)
		{
			this.hitJitter.StartTimedJitter();
		}
		if (damageInstance.AttackType == AttackTypes.Heavy || damageInstance.AttackType == AttackTypes.Explosion)
		{
			this.hitsLeft = 0;
		}
		else
		{
			this.hitsLeft--;
		}
		if (this.passHitToEffects)
		{
			this.passHitToEffects.ReceiveHitEffect(damageInstance, this.passHitToEffects.transform.InverseTransformPoint(position2));
		}
		if (this.hitsLeft > 0)
		{
			instance.FreezeMoment(this.hitFreezeMoment, null);
			this.hitShake.DoShake(this, true);
			this.hitAudioEvent.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
			if (this.hitAudioClipTable)
			{
				this.hitAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
			if (this.flingSelfOnHit)
			{
				Rigidbody2D component2 = base.GetComponent<Rigidbody2D>();
				if (component2 != null)
				{
					float num4 = Random.Range(num2, num3);
					Vector2 a = new Vector2(Mathf.Cos(num4 * 0.017453292f), Mathf.Sin(num4 * 0.017453292f));
					float d = Random.Range(this.flingSpeedMin, this.flingSpeedMax) * num;
					component2.linearVelocity = a * d;
					SpinSelfSimple component3 = base.GetComponent<SpinSelfSimple>();
					if (component3 != null)
					{
						component3.DoSpin();
					}
				}
			}
			if (this.silkHits > 0)
			{
				if (damageInstance.IsNailDamage)
				{
					if (this.silkGetEffect != null)
					{
						Transform transform2 = base.transform;
						this.silkGetEffect.Spawn(transform2.position, transform2.rotation);
					}
					HeroController.instance.AddSilk(1, false);
				}
				if (this.silkDropEffect != null)
				{
					Transform transform3 = base.transform;
					this.silkDropEffect.Spawn(transform3.position, transform3.rotation);
				}
				this.silkHits--;
				if (this.silkHits == 1)
				{
					this.silkDropsCondition.SetActive(false);
					if (this.threadThinObject)
					{
						this.threadThinObject.SetActive(true);
					}
				}
				else if (this.silkHits == 0)
				{
					this.silkDropsCondition.SetActive(false);
					if (this.threadThinObject)
					{
						this.threadThinObject.SetActive(false);
					}
				}
			}
			return IHitResponder.Response.GenericHit;
		}
		if (this.breakEffects)
		{
			this.breakEffects.SetActive(true);
		}
		this.brokenByNail = damageInstance.IsNailDamage;
		this.Break(num2, num3, num);
		if (this.threadThinObject)
		{
			this.threadThinObject.SetActive(false);
		}
		if (this.BrokenHit != null)
		{
			this.BrokenHit(damageInstance);
		}
		instance.FreezeMoment(this.breakFreezeMoment, null);
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000BB684 File Offset: 0x000B9884
	private static void SpawnNailHitEffect(Transform nailHitEffectPrefab, Vector3 position, float impactAngle)
	{
		if (nailHitEffectPrefab == null)
		{
			return;
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(impactAngle);
		float y = 1.5f;
		float minInclusive;
		float maxInclusive;
		if (cardinalDirection == 3)
		{
			minInclusive = 250f;
			maxInclusive = 290f;
		}
		else if (cardinalDirection == 1)
		{
			minInclusive = 70f;
			maxInclusive = 110f;
		}
		else
		{
			minInclusive = 340f;
			maxInclusive = 380f;
		}
		float x = (cardinalDirection == 2) ? -1.5f : 1.5f;
		Transform transform = nailHitEffectPrefab.Spawn(position);
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.z = Random.Range(minInclusive, maxInclusive);
		transform.eulerAngles = eulerAngles;
		Vector3 localScale = transform.localScale;
		localScale.x = x;
		localScale.y = y;
		transform.localScale = localScale;
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000BB730 File Offset: 0x000B9930
	private bool IsPartVisible(GameObject obj)
	{
		if (!obj)
		{
			return false;
		}
		if (!obj.activeInHierarchy)
		{
			return false;
		}
		SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
		return !component || component.enabled;
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x000BB76C File Offset: 0x000B996C
	private void SetStaticPartsActivation(bool broken)
	{
		if (!this.wasAlreadyBroken)
		{
			this.silkDrops.GetRandomValue(true);
			if (this.silkDropsCondition)
			{
				this.IsPartVisible(this.silkDropsCondition);
			}
		}
		if (this.wholeRenderer != null)
		{
			this.wholeRenderer.enabled = !broken;
		}
		foreach (GameObject gameObject in this.wholeParts)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(!broken);
			}
		}
		foreach (GameObject gameObject2 in this.remnantParts)
		{
			if (!(gameObject2 == null))
			{
				gameObject2.SetActive(broken);
			}
		}
		if (this.hitEventReciever != null)
		{
			FSMUtility.SendEventToGameObject(this.hitEventReciever, "HIT", false);
		}
		if (this.bodyCollider)
		{
			this.bodyCollider.enabled = !broken;
		}
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x000BB858 File Offset: 0x000B9A58
	public void Break(float flingAngleMin, float flingAngleMax, float impactMultiplier)
	{
		if (this.isBroken)
		{
			return;
		}
		Vector3 position = base.transform.position;
		this.SetStaticPartsActivation(true);
		List<GameObject> list = this.debrisParts;
		if (this.useAltDebris && (float)Random.Range(1, 100) <= this.altDebrisChance)
		{
			list = this.debrisPartsAlt;
		}
		foreach (GameObject gameObject in list)
		{
			if (!(gameObject == null))
			{
				this.FlingDebrisPart(gameObject, flingAngleMin, flingAngleMax, impactMultiplier);
			}
		}
		foreach (GameObject gameObject2 in this.flingDrops)
		{
			if (!(gameObject2 == null))
			{
				this.FlingDebrisPart(gameObject2, flingAngleMin, flingAngleMax, impactMultiplier);
			}
		}
		if (this.containingParticles.Length != 0)
		{
			GameObject gameObject3 = Probability.GetRandomGameObjectByProbability(this.containingParticles);
			if (gameObject3)
			{
				if (gameObject3.transform.parent != base.transform)
				{
					gameObject3 = gameObject3.Spawn(position);
				}
				gameObject3.SetActive(true);
			}
		}
		Breakable.FlingObject[] array2 = this.flingObjectRegister;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Fling(position);
		}
		this.breakAudioEvent.SpawnAndPlayOneShot(this.audioSourcePrefab, position, null);
		this.breakAudioClipTable.SpawnAndPlayOneShot(this.audioSourcePrefab, position, false, 1f, null);
		if (this.hitEventReciever != null)
		{
			FSMUtility.SendEventToGameObject(this.hitEventReciever, "HIT", false);
		}
		if (this.forwardBreakEvent)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "BREAK", false);
		}
		if (this.breakShake.Camera)
		{
			this.breakShake.DoShake(this, true);
		}
		else
		{
			GameObject gameObject4 = GameObject.FindGameObjectWithTag("CameraParent");
			if (gameObject4 != null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject4, "CameraShake");
				if (playMakerFSM != null)
				{
					playMakerFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		float angleMin = (float)(this.megaFlingGeo ? 65 : 80);
		float angleMax = (float)(this.megaFlingGeo ? 115 : 100);
		float speedMin = (float)(this.megaFlingGeo ? 30 : 15);
		float speedMax = (float)(this.megaFlingGeo ? 45 : 30);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = Gameplay.SmallGeoPrefab,
			AmountMin = this.smallGeoDrops.Start,
			AmountMax = this.smallGeoDrops.End,
			SpeedMin = speedMin,
			SpeedMax = speedMax,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOffset, null, -1f);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = Gameplay.MediumGeoPrefab,
			AmountMin = this.mediumGeoDrops.Start,
			AmountMax = this.mediumGeoDrops.End,
			SpeedMin = speedMin,
			SpeedMax = speedMax,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOffset, null, -1f);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = Gameplay.LargeGeoPrefab,
			AmountMin = this.largeGeoDrops.Start,
			AmountMax = this.largeGeoDrops.End,
			SpeedMin = speedMin,
			SpeedMax = speedMax,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOffset, null, -1f);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = Gameplay.LargeSmoothGeoPrefab,
			AmountMin = this.largeSmoothGeoDrops.Start,
			AmountMax = this.largeSmoothGeoDrops.End,
			SpeedMin = speedMin,
			SpeedMax = speedMax,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOffset, null, -1f);
		FlingUtils.SpawnAndFlingShellShards(new FlingUtils.Config
		{
			Prefab = Gameplay.ShellShardPrefab,
			AmountMin = this.shellShardDrops.Start,
			AmountMax = this.shellShardDrops.End,
			SpeedMin = speedMin,
			SpeedMax = speedMax,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOffset, null);
		foreach (Breakable.ItemDropGroup itemDropGroup in this.itemDropGroups)
		{
			if (itemDropGroup.Drops.Length != 0 && itemDropGroup.TotalProbability >= 1f)
			{
				int num;
				SavedItem randomItemByProbability = Probability.GetRandomItemByProbability<Breakable.ItemDropProbability, SavedItem>(itemDropGroup.Drops, out num, null, null);
				if (randomItemByProbability && randomItemByProbability.CanGetMore() && Gameplay.CollectableItemPickupPrefab)
				{
					CollectableItemPickup collectableItemPickup = itemDropGroup.Drops[num].CustomPickupPrefab;
					if (!collectableItemPickup)
					{
						collectableItemPickup = Gameplay.CollectableItemPickupInstantPrefab;
					}
					CollectableItemPickup collectableItemPickup2 = Object.Instantiate<CollectableItemPickup>(collectableItemPickup);
					collectableItemPickup2.transform.SetPosition2D(base.transform.TransformPoint(this.effectOffset));
					collectableItemPickup2.SetItem(randomItemByProbability, false);
					FlingUtils.FlingObject(new FlingUtils.SelfConfig
					{
						Object = collectableItemPickup2.gameObject,
						SpeedMin = speedMin,
						SpeedMax = speedMax,
						AngleMin = angleMin,
						AngleMax = angleMax
					}, base.transform, this.effectOffset);
				}
			}
		}
		if (this.bodyCollider)
		{
			this.bodyCollider.enabled = false;
		}
		this.isBroken = true;
		if (this.deparentOnBreak)
		{
			base.transform.parent = null;
		}
		if (this.onBreak != null)
		{
			this.onBreak.Invoke();
		}
		NoiseMaker.CreateNoise(base.transform.position, this.noiseRadius, NoiseMaker.Intensities.Normal, false);
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x000BBE64 File Offset: 0x000BA064
	private void FlingDebrisPart(GameObject debrisPart, float flingAngleMin, float flingAngleMax, float impactMultiplier)
	{
		Rigidbody2D component = debrisPart.GetComponent<Rigidbody2D>();
		SpinSelf component2 = debrisPart.GetComponent<SpinSelf>();
		SpinSelfSimple component3 = debrisPart.GetComponent<SpinSelfSimple>();
		debrisPart.SetActive(true);
		if ((!component || !component.freezeRotation) && !component2 && !component3)
		{
			debrisPart.transform.SetRotationZ(debrisPart.transform.localEulerAngles.z + this.angleOffset);
		}
		debrisPart.transform.SetParent(null, true);
		if (component != null)
		{
			float num = Random.Range(flingAngleMin, flingAngleMax);
			Vector2 a = new Vector2(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f));
			float d = Random.Range(this.flingSpeedMin, this.flingSpeedMax) * impactMultiplier;
			component.linearVelocity = a * d;
		}
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x000BBF31 File Offset: 0x000BA131
	private IEnumerator SilkAddRoutine(int silkToAdd)
	{
		yield return new WaitForSeconds(0.01f);
		HeroController.instance.AddSilk(silkToAdd, true);
		this.silkAddRoutine = null;
		yield break;
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x000BBF47 File Offset: 0x000BA147
	public void SetHitsToBreak(int hits)
	{
		this.hitsToBreak = hits;
		this.hitsLeft = hits;
	}

	// Token: 0x06002AFF RID: 11007 RVA: 0x000BBF57 File Offset: 0x000BA157
	public float GetHitCoolDown()
	{
		return this.hitCoolDown;
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x000BBF5F File Offset: 0x000BA15F
	public void SetHitCoolDownDuration(float value)
	{
		this.hitCoolDown = value;
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x000BBF68 File Offset: 0x000BA168
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.None, false);
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x000BBFD3 File Offset: 0x000BA1D3
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04002BD9 RID: 11225
	private Collider2D bodyCollider;

	// Token: 0x04002BDA RID: 11226
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002BDB RID: 11227
	[HideInInspector]
	[SerializeField]
	private bool ignorePersistence;

	// Token: 0x04002BDC RID: 11228
	[Space]
	[SerializeField]
	private bool resetOnEnable;

	// Token: 0x04002BDD RID: 11229
	[SerializeField]
	private bool useAltDebris;

	// Token: 0x04002BDE RID: 11230
	[SerializeField]
	private float altDebrisChance = 100f;

	// Token: 0x04002BDF RID: 11231
	[Tooltip("Renderer which presents the undestroyed object.")]
	[SerializeField]
	private Renderer wholeRenderer;

	// Token: 0x04002BE0 RID: 11232
	[Tooltip("List of child game objects which also represent the whole object.")]
	[SerializeField]
	private GameObject[] wholeParts;

	// Token: 0x04002BE1 RID: 11233
	[Tooltip("List of child game objects which represent remnants that remain static after destruction.")]
	[SerializeField]
	private GameObject[] remnantParts;

	// Token: 0x04002BE2 RID: 11234
	[SerializeField]
	private GameObject[] flingDrops;

	// Token: 0x04002BE3 RID: 11235
	[SerializeField]
	private List<GameObject> debrisParts;

	// Token: 0x04002BE4 RID: 11236
	[SerializeField]
	private List<GameObject> debrisPartsAlt;

	// Token: 0x04002BE5 RID: 11237
	[SerializeField]
	private float angleOffset = -60f;

	// Token: 0x04002BE6 RID: 11238
	[SerializeField]
	[Tooltip("Breakables behind this threshold are inert.")]
	[ModifiableProperty]
	[Conditional("useHeroPlane", false, false, false)]
	private float inertBackgroundThreshold;

	// Token: 0x04002BE7 RID: 11239
	[SerializeField]
	[Tooltip("Breakables in front of this threshold are inert.")]
	[ModifiableProperty]
	[Conditional("useHeroPlane", false, false, false)]
	private float inertForegroundThreshold;

	// Token: 0x04002BE8 RID: 11240
	[SerializeField]
	private bool useHeroPlane;

	// Token: 0x04002BE9 RID: 11241
	[Tooltip("Breakable effects are spawned at this offset.")]
	[SerializeField]
	private Vector3 effectOffset;

	// Token: 0x04002BEA RID: 11242
	[EnsurePrefab]
	[Tooltip("Prefab to spawn for audio.")]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04002BEB RID: 11243
	[SerializeField]
	private AudioEvent hitAudioEvent;

	// Token: 0x04002BEC RID: 11244
	[Tooltip("Table of audio clips to play upon hit.")]
	[SerializeField]
	private RandomAudioClipTable hitAudioClipTable;

	// Token: 0x04002BED RID: 11245
	[Tooltip("Table of audio clips to play upon break.")]
	[SerializeField]
	private AudioEvent breakAudioEvent;

	// Token: 0x04002BEE RID: 11246
	[Tooltip("Table of audio clips to play upon break.")]
	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	// Token: 0x04002BEF RID: 11247
	[Tooltip("Prefab to spawn when hit from a non-down angle.")]
	[SerializeField]
	private Transform dustHitRegularPrefab;

	// Token: 0x04002BF0 RID: 11248
	[Tooltip("Prefab to spawn when hit from a down angle.")]
	[SerializeField]
	private Transform dustHitDownPrefab;

	// Token: 0x04002BF1 RID: 11249
	[Tooltip("Optional prefab to spawn additional effects on break.")]
	[SerializeField]
	private GameObject breakEffectPrefab;

	// Token: 0x04002BF2 RID: 11250
	[SerializeField]
	private float flingSpeedMin;

	// Token: 0x04002BF3 RID: 11251
	[SerializeField]
	private float flingSpeedMax;

	// Token: 0x04002BF4 RID: 11252
	[SerializeField]
	private bool flingSelfOnHit;

	// Token: 0x04002BF5 RID: 11253
	[Tooltip("Strike effect prefab to spawn.")]
	[SerializeField]
	private Transform strikeEffectPrefab;

	// Token: 0x04002BF6 RID: 11254
	[Tooltip("Nail hit prefab to spawn.")]
	[SerializeField]
	private Transform nailHitEffectPrefab;

	// Token: 0x04002BF7 RID: 11255
	[Tooltip("Spell hit effect prefab to spawn.")]
	[SerializeField]
	private Transform spellHitEffectPrefab;

	// Token: 0x04002BF8 RID: 11256
	[Tooltip("Legacy flag that was set but has always been broken but is no longer used?")]
	[SerializeField]
	private bool preventParticleRotation;

	// Token: 0x04002BF9 RID: 11257
	[Tooltip("Object to send HIT event to.")]
	[SerializeField]
	private GameObject hitEventReciever;

	// Token: 0x04002BFA RID: 11258
	[Tooltip("Forward break effect to sibling FSMs.")]
	[SerializeField]
	private bool forwardBreakEvent;

	// Token: 0x04002BFB RID: 11259
	[SerializeField]
	[Tooltip("If enabled, this breakable can only be broken using e.g., a BreakableBreaker.")]
	private bool ignoreDamagers;

	// Token: 0x04002BFC RID: 11260
	[Space]
	[SerializeField]
	private EnemyHitEffectsRegular passHitToEffects;

	// Token: 0x04002BFD RID: 11261
	[Space]
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x04002BFE RID: 11262
	[SerializeField]
	private float hitCoolDown = 0.1f;

	// Token: 0x04002BFF RID: 11263
	[SerializeField]
	private bool firstHitOnly;

	// Token: 0x04002C00 RID: 11264
	[Obsolete("Just use noise radius now.")]
	[HideInInspector]
	[SerializeField]
	private bool emitNoise = true;

	// Token: 0x04002C01 RID: 11265
	[SerializeField]
	private float noiseRadius = 3f;

	// Token: 0x04002C02 RID: 11266
	[SerializeField]
	private bool autoAddJitterComponent = true;

	// Token: 0x04002C03 RID: 11267
	[SerializeField]
	private TrackTriggerObjects breakableRange;

	// Token: 0x04002C04 RID: 11268
	[SerializeField]
	private CameraShakeTarget hitShake;

	// Token: 0x04002C05 RID: 11269
	[SerializeField]
	private CameraShakeTarget breakShake;

	// Token: 0x04002C06 RID: 11270
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private int freezeMoment;

	// Token: 0x04002C07 RID: 11271
	[SerializeField]
	private FreezeMomentTypes hitFreezeMoment = FreezeMomentTypes.None;

	// Token: 0x04002C08 RID: 11272
	[SerializeField]
	private FreezeMomentTypes breakFreezeMoment = FreezeMomentTypes.None;

	// Token: 0x04002C09 RID: 11273
	[Space]
	public Probability.ProbabilityGameObject[] containingParticles;

	// Token: 0x04002C0A RID: 11274
	public Breakable.FlingObject[] flingObjectRegister;

	// Token: 0x04002C0B RID: 11275
	[Space]
	[SerializeField]
	private MinMaxInt smallGeoDrops;

	// Token: 0x04002C0C RID: 11276
	[SerializeField]
	private MinMaxInt mediumGeoDrops;

	// Token: 0x04002C0D RID: 11277
	[SerializeField]
	private MinMaxInt largeGeoDrops;

	// Token: 0x04002C0E RID: 11278
	[SerializeField]
	private MinMaxInt largeSmoothGeoDrops;

	// Token: 0x04002C0F RID: 11279
	[SerializeField]
	private bool megaFlingGeo;

	// Token: 0x04002C10 RID: 11280
	[SerializeField]
	private MinMaxInt shellShardDrops;

	// Token: 0x04002C11 RID: 11281
	[SerializeField]
	private int silkGain;

	// Token: 0x04002C12 RID: 11282
	[SerializeField]
	private Breakable.ItemDropGroup[] itemDropGroups;

	// Token: 0x04002C13 RID: 11283
	[SerializeField]
	private MinMaxInt silkDrops;

	// Token: 0x04002C14 RID: 11284
	[SerializeField]
	private GameObject silkDropEffect;

	// Token: 0x04002C15 RID: 11285
	[SerializeField]
	private GameObject silkGetEffect;

	// Token: 0x04002C16 RID: 11286
	[SerializeField]
	private GameObject silkDropsCondition;

	// Token: 0x04002C17 RID: 11287
	[SerializeField]
	private GameObject threadThinObject;

	// Token: 0x04002C18 RID: 11288
	[SerializeField]
	private PersistentBoolItem silkDropsPersistent;

	// Token: 0x04002C19 RID: 11289
	[SerializeField]
	private bool deparentOnBreak;

	// Token: 0x04002C1A RID: 11290
	[SerializeField]
	private bool immuneToBreakableBreaker;

	// Token: 0x04002C1B RID: 11291
	[Space]
	[SerializeField]
	private Breakable[] requireBroken;

	// Token: 0x04002C1C RID: 11292
	[Space]
	[SerializeField]
	private UnityEvent onHit;

	// Token: 0x04002C1D RID: 11293
	[SerializeField]
	private UnityEvent onBreak;

	// Token: 0x04002C1E RID: 11294
	[SerializeField]
	private UnityEvent onBroken;

	// Token: 0x04002C1F RID: 11295
	private GameObject breakEffects;

	// Token: 0x04002C20 RID: 11296
	private double hitCooldownEndTime;

	// Token: 0x04002C21 RID: 11297
	private bool isBroken;

	// Token: 0x04002C22 RID: 11298
	private bool wasAlreadyBroken;

	// Token: 0x04002C23 RID: 11299
	private int hitsLeft;

	// Token: 0x04002C24 RID: 11300
	private int silkHits;

	// Token: 0x04002C25 RID: 11301
	private int startSilkHits;

	// Token: 0x04002C26 RID: 11302
	private bool brokenByNail;

	// Token: 0x04002C27 RID: 11303
	private Coroutine silkAddRoutine;

	// Token: 0x04002C28 RID: 11304
	private JitterSelfForTime hitJitter;

	// Token: 0x020017B5 RID: 6069
	[Serializable]
	public class FlingObject
	{
		// Token: 0x06008E84 RID: 36484 RVA: 0x0028D9C0 File Offset: 0x0028BBC0
		public FlingObject()
		{
			this.spawnMin = 25;
			this.spawnMax = 30;
			this.speedMin = 9f;
			this.speedMax = 20f;
			this.angleMin = 20f;
			this.angleMax = 160f;
			this.originVariation = new Vector2(0.5f, 0.5f);
		}

		// Token: 0x06008E85 RID: 36485 RVA: 0x0028DA24 File Offset: 0x0028BC24
		public void Fling(Vector3 origin)
		{
			if (!this.referenceObject)
			{
				return;
			}
			int num = Random.Range(this.spawnMin, this.spawnMax + 1);
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = this.referenceObject.Spawn();
				if (gameObject)
				{
					gameObject.transform.position = origin + new Vector3(Random.Range(-this.originVariation.x, this.originVariation.x), Random.Range(-this.originVariation.y, this.originVariation.y), 0f);
					float num2 = Random.Range(this.speedMin, this.speedMax);
					float num3 = Random.Range(this.angleMin, this.angleMax);
					float x = num2 * Mathf.Cos(num3 * 0.017453292f);
					float y = num2 * Mathf.Sin(num3 * 0.017453292f);
					Vector2 force = new Vector2(x, y);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if (component)
					{
						component.AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
		}

		// Token: 0x04008F1B RID: 36635
		public GameObject referenceObject;

		// Token: 0x04008F1C RID: 36636
		[Space]
		public int spawnMin;

		// Token: 0x04008F1D RID: 36637
		public int spawnMax;

		// Token: 0x04008F1E RID: 36638
		public float speedMin;

		// Token: 0x04008F1F RID: 36639
		public float speedMax;

		// Token: 0x04008F20 RID: 36640
		public float angleMin;

		// Token: 0x04008F21 RID: 36641
		public float angleMax;

		// Token: 0x04008F22 RID: 36642
		public Vector2 originVariation;
	}

	// Token: 0x020017B6 RID: 6070
	[Serializable]
	private class ItemDropProbability : Probability.ProbabilityBase<SavedItem>
	{
		// Token: 0x17000F4D RID: 3917
		// (get) Token: 0x06008E86 RID: 36486 RVA: 0x0028DB37 File Offset: 0x0028BD37
		public override SavedItem Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x04008F23 RID: 36643
		[SerializeField]
		private SavedItem item;

		// Token: 0x04008F24 RID: 36644
		public CollectableItemPickup CustomPickupPrefab;
	}

	// Token: 0x020017B7 RID: 6071
	[Serializable]
	private class ItemDropGroup
	{
		// Token: 0x04008F25 RID: 36645
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		// Token: 0x04008F26 RID: 36646
		public Breakable.ItemDropProbability[] Drops;
	}
}
