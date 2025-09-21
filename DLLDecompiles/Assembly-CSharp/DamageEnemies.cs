using System;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002C5 RID: 709
public class DamageEnemies : DebugDrawColliderRuntimeAdder, CustomPlayerLoop.ILateFixedUpdate
{
	// Token: 0x1400003D RID: 61
	// (add) Token: 0x060018F9 RID: 6393 RVA: 0x000724D8 File Offset: 0x000706D8
	// (remove) Token: 0x060018FA RID: 6394 RVA: 0x00072510 File Offset: 0x00070710
	public event Action WillDamageEnemy;

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x060018FB RID: 6395 RVA: 0x00072548 File Offset: 0x00070748
	// (remove) Token: 0x060018FC RID: 6396 RVA: 0x00072580 File Offset: 0x00070780
	public event Action<Collider2D> WillDamageEnemyCollider;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x060018FD RID: 6397 RVA: 0x000725B8 File Offset: 0x000707B8
	// (remove) Token: 0x060018FE RID: 6398 RVA: 0x000725F0 File Offset: 0x000707F0
	public event Action<HealthManager, HitInstance> WillDamageEnemyOptions;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x060018FF RID: 6399 RVA: 0x00072628 File Offset: 0x00070828
	// (remove) Token: 0x06001900 RID: 6400 RVA: 0x00072660 File Offset: 0x00070860
	public event Action ParriedEnemy;

	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06001901 RID: 6401 RVA: 0x00072698 File Offset: 0x00070898
	// (remove) Token: 0x06001902 RID: 6402 RVA: 0x000726D0 File Offset: 0x000708D0
	public event DamageEnemies.EndedDamageDelegate EndedDamage;

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06001903 RID: 6403 RVA: 0x00072708 File Offset: 0x00070908
	// (remove) Token: 0x06001904 RID: 6404 RVA: 0x00072740 File Offset: 0x00070940
	public event Action DamagedEnemy;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x06001905 RID: 6405 RVA: 0x00072778 File Offset: 0x00070978
	// (remove) Token: 0x06001906 RID: 6406 RVA: 0x000727B0 File Offset: 0x000709B0
	public event Action<GameObject> DamagedEnemyGameObject;

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x06001907 RID: 6407 RVA: 0x000727E8 File Offset: 0x000709E8
	// (remove) Token: 0x06001908 RID: 6408 RVA: 0x00072820 File Offset: 0x00070A20
	public event Action<HealthManager> DamagedEnemyHealthManager;

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x06001909 RID: 6409 RVA: 0x00072858 File Offset: 0x00070A58
	// (remove) Token: 0x0600190A RID: 6410 RVA: 0x00072890 File Offset: 0x00070A90
	public event Action MultiHitEvaluated;

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600190B RID: 6411 RVA: 0x000728C8 File Offset: 0x00070AC8
	// (remove) Token: 0x0600190C RID: 6412 RVA: 0x00072900 File Offset: 0x00070B00
	public event Action<DamageEnemies.HitResponse> HitResponded;

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x0600190D RID: 6413 RVA: 0x00072935 File Offset: 0x00070B35
	public bool IgnoreNailPosition
	{
		get
		{
			return this.ignoreNailPosition;
		}
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x0600190E RID: 6414 RVA: 0x0007293D File Offset: 0x00070B3D
	public bool OnlyDamageEnemies
	{
		get
		{
			return this.onlyDamageEnemies;
		}
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x0600190F RID: 6415 RVA: 0x00072945 File Offset: 0x00070B45
	public ToolItem RepresentingTool
	{
		get
		{
			return this.representingTool;
		}
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001910 RID: 6416 RVA: 0x00072950 File Offset: 0x00070B50
	public int PoisonDamageTicks
	{
		get
		{
			if (this.isPoisonDamageOverridden)
			{
				return this.poisonDamageTicks;
			}
			if (this.doesNotFlags.HasFlag(DamageEnemies.DoesNotFlags.PoisonTicks))
			{
				return 0;
			}
			if (!Gameplay.PoisonPouchTool.Status.IsEquipped)
			{
				return 0;
			}
			if (!this.representingTool)
			{
				return this.poisonDamageTicks;
			}
			return this.representingTool.PoisonDamageTicks;
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06001911 RID: 6417 RVA: 0x000729B8 File Offset: 0x00070BB8
	public int ZapDamageTicks
	{
		get
		{
			if (this.doesNotFlags.HasFlag(DamageEnemies.DoesNotFlags.LightningTicks))
			{
				return 0;
			}
			if (!Gameplay.ZapImbuementTool.Status.IsEquipped)
			{
				return 0;
			}
			if (!this.representingTool)
			{
				return this.zapDamageTicks;
			}
			return this.representingTool.ZapDamageTicks;
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x06001912 RID: 6418 RVA: 0x00072A11 File Offset: 0x00070C11
	public ITinkResponder.TinkFlags AllowedTinkFlags
	{
		get
		{
			return this.allowedTinkFlags;
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x06001913 RID: 6419 RVA: 0x00072A19 File Offset: 0x00070C19
	// (set) Token: 0x06001914 RID: 6420 RVA: 0x00072A21 File Offset: 0x00070C21
	public float? ExtraUpDirection { get; set; }

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x06001915 RID: 6421 RVA: 0x00072A2A File Offset: 0x00070C2A
	// (set) Token: 0x06001916 RID: 6422 RVA: 0x00072A32 File Offset: 0x00070C32
	public NailElements NailElement { get; set; }

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x06001917 RID: 6423 RVA: 0x00072A3B File Offset: 0x00070C3B
	// (set) Token: 0x06001918 RID: 6424 RVA: 0x00072A43 File Offset: 0x00070C43
	public NailImbuementConfig NailImbuement { get; set; }

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06001919 RID: 6425 RVA: 0x00072A4C File Offset: 0x00070C4C
	// (set) Token: 0x0600191A RID: 6426 RVA: 0x00072A54 File Offset: 0x00070C54
	public bool AwardJournalKill
	{
		get
		{
			return this.awardJournalKill;
		}
		set
		{
			this.awardJournalKill = value;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x0600191B RID: 6427 RVA: 0x00072A5D File Offset: 0x00070C5D
	// (set) Token: 0x0600191C RID: 6428 RVA: 0x00072A65 File Offset: 0x00070C65
	public bool DidHit { get; private set; }

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x0600191D RID: 6429 RVA: 0x00072A6E File Offset: 0x00070C6E
	// (set) Token: 0x0600191E RID: 6430 RVA: 0x00072A76 File Offset: 0x00070C76
	public bool DidHitEnemy { get; private set; }

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x0600191F RID: 6431 RVA: 0x00072A7F File Offset: 0x00070C7F
	public bool CircleDirection
	{
		get
		{
			return this.directionSourceOverride == DamageEnemies.DirectionSourceOverrides.CircleDirection;
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001920 RID: 6432 RVA: 0x00072A8A File Offset: 0x00070C8A
	// (set) Token: 0x06001921 RID: 6433 RVA: 0x00072A92 File Offset: 0x00070C92
	public float DamageMultiplier
	{
		get
		{
			return this.damageMultiplier;
		}
		set
		{
			this.damageMultiplier = value;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001922 RID: 6434 RVA: 0x00072A9B File Offset: 0x00070C9B
	public LagHitOptions LagHits
	{
		get
		{
			if (!this.lagHitOptionsProfile)
			{
				return this.lagHitOptions;
			}
			return this.lagHitOptionsProfile.Options;
		}
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x00072ABC File Offset: 0x00070CBC
	private bool? IsDeathEventValid()
	{
		return this.deathEventTarget.IsEventValid(this.deathEvent, false);
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x00072AD0 File Offset: 0x00070CD0
	private bool ShowDeathEventField()
	{
		return this.HandlesMultiHitsDeath() && this.deathEventTarget;
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x00072AE7 File Offset: 0x00070CE7
	private bool HandlesMultiHitsDeath()
	{
		return this.multiHitter && this.hitsUntilDeath > 0;
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x00072AFC File Offset: 0x00070CFC
	[UsedImplicitly]
	private bool ShowDamageDealt()
	{
		return !this.useNailDamage && !this.damageAsset;
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x00072B18 File Offset: 0x00070D18
	private void OnValidate()
	{
		if (this.doesNotGenerateSilk)
		{
			this.silkGeneration = HitSilkGeneration.None;
			this.doesNotGenerateSilk = false;
		}
		if (this.circleDirection)
		{
			this.directionSourceOverride = DamageEnemies.DirectionSourceOverrides.CircleDirection;
			this.circleDirection = false;
		}
		if (this.attackType == AttackTypes.Piercer_OBSOLETE)
		{
			this.specialType |= SpecialTypes.Piercer;
			this.attackType = AttackTypes.Generic;
		}
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x00072B70 File Offset: 0x00070D70
	protected override void Awake()
	{
		CustomPlayerLoop.RegisterLateFixedUpdate(this);
		base.Awake();
		this.OnValidate();
		if (this.multiHitter)
		{
			SpriteFlash spriteFlash = base.GetComponent<SpriteFlash>();
			if (spriteFlash)
			{
				this.WillDamageEnemy += delegate()
				{
					spriteFlash.flashWhiteQuick();
				};
			}
		}
		if (this.damageAsset)
		{
			this.damageDealt = this.damageAsset.Value;
		}
		if (this.onlyDamageEnemiesChance < 100)
		{
			this.CheckOnlyDamageEnemies();
		}
		this.hasSharedDamageGroup = this.sharedDamagedGroup;
	}

	// Token: 0x06001929 RID: 6441 RVA: 0x00072C08 File Offset: 0x00070E08
	private void Start()
	{
		this.isNailAttack = base.gameObject.CompareTag("Nail Attack");
		this.sourceIsHero = (this.isNailAttack || base.GetComponentInParent<HeroController>() != null || this.representingTool != null);
		this.started = true;
		ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
	}

	// Token: 0x0600192A RID: 6442 RVA: 0x00072C74 File Offset: 0x00070E74
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
		}
		this.hitsResponded.Clear();
		this.StartDamage();
		if (this.onlyDamageEnemiesChance < 100)
		{
			this.CheckOnlyDamageEnemies();
		}
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x00072CC0 File Offset: 0x00070EC0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.DoDamage(collision.gameObject, true);
	}

	// Token: 0x0600192C RID: 6444 RVA: 0x00072CD0 File Offset: 0x00070ED0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.HandlesMultiHitsDeath() && this.hitsLeftUntilDeath <= 0)
		{
			return;
		}
		PhysLayers layer = (PhysLayers)collision.gameObject.layer;
		if (layer == PhysLayers.HERO_BOX || layer == PhysLayers.PLAYER || layer == PhysLayers.ENEMY_ATTACK || layer == PhysLayers.CORPSE || layer == PhysLayers.ATTACK_DETECTOR)
		{
			return;
		}
		SpikeSlashReaction spikeSlashReaction = null;
		bool? flag = null;
		if (layer == PhysLayers.HERO_ATTACK && !collision.gameObject.CompareTag("Enemy Projectile") && !collision.gameObject.CompareTag("Hero Damage Target") && !collision.GetComponent<TinkEffect>())
		{
			spikeSlashReaction = collision.GetComponent<SpikeSlashReaction>();
			flag = new bool?(spikeSlashReaction);
			if (flag != null && !flag.Value)
			{
				return;
			}
		}
		if (this.forceSpikeUpdate)
		{
			if (flag == null)
			{
				spikeSlashReaction = collision.GetComponent<SpikeSlashReaction>();
				flag = new bool?(spikeSlashReaction);
			}
			if (flag != null && flag.Value)
			{
				this.spikeSlashReactions.Add(new DamageEnemies.SpikeSlashData(spikeSlashReaction, collision));
			}
		}
		this.enteredColliders.Add(collision);
		this.frameQueue.Add(collision);
		if (this.contactFSMEvent != "")
		{
			FSMUtility.SendEventToGameObject(collision.gameObject, this.contactFSMEvent, false);
		}
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x00072E0C File Offset: 0x0007100C
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.enteredColliders.Remove(collision);
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x00072E1B File Offset: 0x0007101B
	private void OnDisable()
	{
		ComponentSingleton<DamageEnemiesCallbackHooks>.Instance.OnFixedUpdate -= this.OnFixedUpdate;
		this.EndDamage();
		this.ClearLists();
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x00072E40 File Offset: 0x00071040
	public void ClearLists()
	{
		this.ClearDamageBuffer();
		if (!this.isProcessingBuffer)
		{
			this.processingDamageBuffer.Clear();
		}
		this.hitsResponded.Clear();
		if (!this.isDoingDamage)
		{
			this.tempHitsResponded.Clear();
		}
		this.ClearPreventDamage();
		this.damagedColliders.Clear();
		this.spikeSlashReactions.Clear();
		this.frameQueue.Clear();
		this.enteredColliders.Clear();
		this.evaluatingColliders.Clear();
	}

	// Token: 0x06001930 RID: 6448 RVA: 0x00072EC1 File Offset: 0x000710C1
	private void OnDestroy()
	{
		CustomPlayerLoop.UnregisterLateFixedUpdate(this);
	}

	// Token: 0x06001931 RID: 6449 RVA: 0x00072EC9 File Offset: 0x000710C9
	private void OnFixedUpdate()
	{
		if (this.multiHitter && this.stepsToNextHit > 0)
		{
			this.stepsToNextHit--;
		}
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x00072EEA File Offset: 0x000710EA
	public void ForceUpdate()
	{
		this.LateFixedUpdate();
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x00072EF4 File Offset: 0x000710F4
	public void LateFixedUpdate()
	{
		this.hitsResponded.Clear();
		this.evaluatingColliders.Clear();
		if (this.frameQueue.Count > 0)
		{
			foreach (Collider2D item in this.enteredColliders)
			{
				this.frameQueue.Add(item);
			}
			this.evaluatingColliders.AddRange(this.frameQueue);
			this.frameQueue.Clear();
		}
		else
		{
			this.evaluatingColliders.AddRange(this.enteredColliders);
		}
		bool flag = true;
		if (!this.multiHitter)
		{
			if (!this.manualTrigger)
			{
				this.EvaluateDamage();
			}
			else
			{
				this.EvaluateSpikeColliders();
			}
		}
		else if (this.stepsToNextHit <= 0 && this.evaluatingColliders.Count > 0)
		{
			this.ClearPreventDamage();
			if (this.EvaluateDamage())
			{
				this.stepsToNextHit = this.stepsPerHit;
			}
			Action multiHitEvaluated = this.MultiHitEvaluated;
			if (multiHitEvaluated != null)
			{
				multiHitEvaluated();
			}
		}
		else
		{
			if (this.stepsToNextHit > 0 && this.evaluatingColliders.Count > 0)
			{
				this.EvaluateDamage();
			}
			flag = false;
		}
		this.evaluatingColliders.Clear();
		this.spikeSlashReactions.Clear();
		if (flag && this.DidHit && this.endDamageEnemyExit && this.currentDamageBuffer.Count == 0)
		{
			this.EndDamage();
			return;
		}
		this.ProcessDamageBuffer();
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x0007306C File Offset: 0x0007126C
	private bool EvaluateDamage()
	{
		bool result = false;
		for (int i = 0; i < this.evaluatingColliders.Count; i++)
		{
			Collider2D collider2D = this.evaluatingColliders[i];
			if (collider2D == null)
			{
				this.enteredColliders.Remove(collider2D);
			}
			else if (!collider2D.isActiveAndEnabled)
			{
				this.enteredColliders.Remove(collider2D);
			}
			else if (this.attackType == AttackTypes.Spikes || this.damagedColliders.Add(collider2D) || this.PreventDamage(collider2D))
			{
				if (this.DoDamage(collider2D, true))
				{
					result = true;
				}
			}
			else if (this.multiHitter && this.stepsToNextHit <= 0 && this.DoDamage(collider2D, false))
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x00073120 File Offset: 0x00071320
	private void EvaluateSpikeColliders()
	{
		for (int i = this.spikeSlashReactions.Count - 1; i >= 0; i--)
		{
			DamageEnemies.SpikeSlashData spikeSlashData = this.spikeSlashReactions[i];
			if (!(spikeSlashData.spikeSlashReaction == null) && spikeSlashData.spikeSlashReaction.isActiveAndEnabled)
			{
				if (this.attackType == AttackTypes.Spikes || this.damagedColliders.Add(spikeSlashData.collider2D))
				{
					if (this.DoDamage(spikeSlashData.collider2D, true))
					{
					}
				}
				else if (this.multiHitter && this.stepsToNextHit <= 0)
				{
					this.DoDamage(spikeSlashData.collider2D, false);
				}
			}
		}
		this.spikeSlashReactions.Clear();
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000731C8 File Offset: 0x000713C8
	private void ProcessDamageBuffer()
	{
		List<DamageEnemies.HitResponse> list = this.currentDamageBuffer;
		List<DamageEnemies.HitResponse> list2 = this.processingDamageBuffer;
		this.processingDamageBuffer = list;
		this.currentDamageBuffer = list2;
		if (this.processingDamageBuffer.Count == 0)
		{
			return;
		}
		bool flag = false;
		try
		{
			this.isProcessingBuffer = true;
			foreach (DamageEnemies.HitResponse hitResponse in this.processingDamageBuffer)
			{
				if (!this.HasBeenDamaged(hitResponse.Responder))
				{
					HitInstance hit = hitResponse.Hit;
					if (this.multiHitter && this.hitsLeftUntilDeath == 1 && this.finalHitMagMultOverride > 0f)
					{
						hit.MagnitudeMultiplier = this.finalHitMagMultOverride;
					}
					IHitResponder.HitResponse hitResponse2 = hitResponse.Responder.Hit(hit);
					if (hitResponse2 != IHitResponder.Response.None)
					{
						if (hitResponse2.consumeCharges)
						{
							flag = true;
						}
						this.hitsResponded.Add(hitResponse.Responder);
						this.lastRespondedCycle = CustomPlayerLoop.FixedUpdateCycle;
						if (this.hitOnceUntilEnd)
						{
							this.PreventDamage(hitResponse.Responder);
						}
						if (!this.DidHit && this.firstHitFreeze > FreezeMomentTypes.None)
						{
							GameManager.instance.FreezeMoment(this.firstHitFreeze, null);
						}
						Action<DamageEnemies.HitResponse> hitResponded = this.HitResponded;
						if (hitResponded != null)
						{
							hitResponded(hitResponse);
						}
						this.DidHit = true;
						if (hitResponse.HealthManager)
						{
							bool flag2 = hitResponse2 == IHitResponder.Response.DamageEnemy;
							if (flag2)
							{
								hitResponse.HealthManager.DoLagHits(this.LagHits, hit);
							}
							this.DidHitEnemy = true;
							if (this.attackType != AttackTypes.Spikes && !this.manualTrigger)
							{
								this.PreventDamage(hitResponse.HealthManager);
							}
							if (flag2)
							{
								Action<GameObject> damagedEnemyGameObject = this.DamagedEnemyGameObject;
								if (damagedEnemyGameObject != null)
								{
									damagedEnemyGameObject(hitResponse.Target);
								}
								Action<HealthManager> damagedEnemyHealthManager = this.DamagedEnemyHealthManager;
								if (damagedEnemyHealthManager != null)
								{
									damagedEnemyHealthManager(hitResponse.HealthManager);
								}
								Action damagedEnemy = this.DamagedEnemy;
								if (damagedEnemy != null)
								{
									damagedEnemy();
								}
								this.DoEnemyDamageNailImbuement(hitResponse.HealthManager, hitResponse.Hit);
							}
						}
					}
				}
			}
		}
		finally
		{
			this.isProcessingBuffer = false;
			this.processingDamageBuffer.Clear();
			if (this.wasCriticalHitForced)
			{
				HeroController instance = HeroController.instance;
				HeroController.WandererCrestStateInfo wandererState = instance.WandererState;
				wandererState.QueuedNextHitCritical = false;
				instance.WandererState = wandererState;
				this.wasCriticalHitForced = false;
			}
		}
		if (flag && this.hitsLeftUntilDeath > 0)
		{
			this.hitsLeftUntilDeath--;
			if (this.hitsLeftUntilDeath <= 0)
			{
				if (!string.IsNullOrEmpty(this.deathEvent))
				{
					this.deathEventTarget.SendEvent(this.deathEvent);
				}
				if (this.deathEndDamage)
				{
					this.EndDamage();
				}
			}
		}
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0007348C File Offset: 0x0007168C
	private void CheckOnlyDamageEnemies()
	{
		if (Random.Range(1, 101) <= this.onlyDamageEnemiesChance)
		{
			this.onlyDamageEnemies = true;
			return;
		}
		this.onlyDamageEnemies = false;
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000734AD File Offset: 0x000716AD
	public void setOnlyDamageEnemies(bool onlyDamage)
	{
		this.onlyDamageEnemies = onlyDamage;
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000734B6 File Offset: 0x000716B6
	public void SetNailDamageMultiplier(float multiplier)
	{
		this.nailDamageMultiplier = multiplier;
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000734BF File Offset: 0x000716BF
	public static bool IsNailAttackObject(GameObject gameObject)
	{
		return gameObject.CompareTag("Nail Attack") || gameObject.GetComponentInParent<HeroExtraNailSlash>();
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000734DB File Offset: 0x000716DB
	public bool DoDamage(Collider2D col, bool isFirstHit = true)
	{
		Action<Collider2D> willDamageEnemyCollider = this.WillDamageEnemyCollider;
		if (willDamageEnemyCollider != null)
		{
			willDamageEnemyCollider(col);
		}
		return this.DoDamage(col.gameObject, isFirstHit);
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000734FC File Offset: 0x000716FC
	public bool TryDoDamage(Collider2D target, bool isFirstHit = true)
	{
		return !(target == null) && target.isActiveAndEnabled && (this.attackType == AttackTypes.Spikes || this.damagedColliders.Add(target) || this.PreventDamage(target)) && this.DoDamage(target, true);
	}

	// Token: 0x0600193D RID: 6461 RVA: 0x0007353C File Offset: 0x0007173C
	public bool DoDamage(GameObject target, bool isFirstHit = true)
	{
		NailElements nailElement = this.NailElement;
		NailImbuementConfig nailImbuement = this.NailImbuement;
		bool result;
		try
		{
			if (this.attackType == AttackTypes.Fire)
			{
				this.NailElement = NailElements.Fire;
				this.NailImbuement = Effects.FireNail;
			}
			if (target == null)
			{
				result = false;
			}
			else
			{
				this.isDoingDamage = true;
				HeroController instance = HeroController.instance;
				this.tempDamageStack.SetupNew(this.damageDealt);
				float num = (float)this.damageDealtOffset;
				float num2 = this.magnitudeMult;
				bool rageHit = false;
				bool hunterCombo = false;
				if (this.useNailDamage || this.useHeroDamageAffectors)
				{
					if (this.useNailDamage)
					{
						this.tempDamageStack.SetupNew(PlayerData.instance.nailDamage);
						this.tempDamageStack.AddMultiplier(this.nailDamageMultiplier);
					}
					if (this.NailImbuement)
					{
						this.tempDamageStack.AddMultiplier(this.NailImbuement.NailDamageMultiplier);
						num2 *= this.NailImbuement.NailDamageMultiplier;
					}
					if (instance.WarriorState.IsInRageMode && DamageEnemies.IsNailAttackObject(base.gameObject))
					{
						float warriorDamageMultiplier = Gameplay.WarriorDamageMultiplier;
						this.tempDamageStack.AddMultiplier(warriorDamageMultiplier);
						num2 *= warriorDamageMultiplier;
						rageHit = true;
					}
					AttackTypes attackTypes = this.attackType;
					if (attackTypes == AttackTypes.Nail || attackTypes == AttackTypes.NailBeam || this.isNailAttack)
					{
						HeroController.HunterUpgCrestStateInfo hunterUpgState = instance.HunterUpgState;
						if (hunterUpgState.CurrentMeterHits > 0)
						{
							if (Gameplay.HunterCrest2.IsEquipped)
							{
								if (hunterUpgState.CurrentMeterHits >= Gameplay.HunterComboHits)
								{
									this.tempDamageStack.AddMultiplier(Gameplay.HunterComboDamageMult);
									hunterCombo = true;
								}
							}
							else if (Gameplay.HunterCrest3.IsEquipped)
							{
								if (hunterUpgState.IsComboMeterAboveExtra)
								{
									this.tempDamageStack.AddMultiplier(Gameplay.HunterCombo2ExtraDamageMult);
									hunterCombo = true;
								}
								else if (hunterUpgState.CurrentMeterHits >= Gameplay.HunterCombo2Hits)
								{
									this.tempDamageStack.AddMultiplier(Gameplay.HunterCombo2DamageMult);
									hunterCombo = true;
								}
							}
						}
						if (instance.SilkTauntEffectConsume())
						{
							this.tempDamageStack.AddMultiplier(1.5f);
						}
						ToolItem barbedWireTool = Gameplay.BarbedWireTool;
						if (barbedWireTool && barbedWireTool.Status.IsEquipped)
						{
							this.tempDamageStack.AddMultiplier(Gameplay.BarbedWireDamageDealtMultiplier);
						}
					}
					this.tempDamageStack.AddOffset(-num);
				}
				else if (this.representingTool && this.representingTool.Type.IsAttackType())
				{
					float multiplier = 1f + (float)PlayerData.instance.ToolKitUpgrades * Gameplay.ToolKitDamageIncrease;
					this.tempDamageStack.AddMultiplier(multiplier);
					if (this.representingTool.Type == ToolItemType.Skill && Gameplay.ZapImbuementTool.Status.IsEquipped)
					{
						this.tempDamageStack.AddMultiplier(Gameplay.ZapDamageMult);
					}
				}
				bool criticalHit = false;
				if ((!this.doesNotCriticalHit && this.attackType == AttackTypes.Nail && isFirstHit) || this.isNailAttack)
				{
					if (instance.WandererState.QueuedNextHitCritical)
					{
						criticalHit = true;
						this.wasCriticalHitForced = true;
					}
					else if (!instance.WandererState.CriticalHitsLocked && instance.IsWandererLucky && Random.Range(0f, 1f) <= Gameplay.WandererCritChance * instance.GetLuckModifier())
					{
						criticalHit = true;
					}
				}
				this.tempHitsResponded.Clear();
				HitTaker.GetHitResponders(this.tempHitsResponded, target, this.hitsResponded);
				HealthManager healthManager = null;
				bool flag = false;
				int i = 0;
				while (i < this.tempHitsResponded.Count)
				{
					IHitResponder hitResponder = this.tempHitsResponded[i];
					healthManager = (hitResponder as HealthManager);
					if (healthManager)
					{
						if (healthManager.IsPartOfSendToTarget && healthManager.SendDamageTo)
						{
							this.tempHitsResponded[i] = healthManager.SendDamageTo;
							healthManager = healthManager.SendDamageTo;
							break;
						}
						break;
					}
					else
					{
						if ((this.specialType & SpecialTypes.Taunt) == SpecialTypes.None && !flag && (hitResponder is ReceivedDamageProxy || hitResponder is global::HitResponse || hitResponder is CurrencyObjectBase))
						{
							flag = true;
						}
						i++;
					}
				}
				if (this.onlyDamageEnemies && !healthManager && !flag)
				{
					result = false;
				}
				else
				{
					this.tempDamageStack.AddMultiplier(this.DamageMultiplier);
					float num3 = (float)this.tempDamageStack.PopDamage();
					float[] array = this.damageMultPerHit;
					if (array != null && array.Length > 0)
					{
						if (this.hitCounts == null)
						{
							this.hitCounts = new Dictionary<GameObject, int>();
						}
						int num4;
						if (isFirstHit || !this.hitCounts.TryGetValue(target, out num4))
						{
							num4 = 0;
						}
						if (num4 >= this.damageMultPerHit.Length)
						{
							num4 = this.damageMultPerHit.Length - 1;
						}
						float num5 = this.damageMultPerHit[num4];
						num3 *= num5;
						if (num5 > Mathf.Epsilon && num3 < 0.51f)
						{
							num3 = 0.51f;
						}
						this.hitCounts[target] = num4 + 1;
					}
					int num6 = Mathf.RoundToInt(num3);
					bool flag2 = num6 > 0 || this.canWeakHit;
					EnemyHitEffectsProfile.EffectsTypes hitEffectsType;
					if (this.multiHitter)
					{
						hitEffectsType = (isFirstHit ? this.multiHitFirstEffects : this.multiHitEffects);
					}
					else
					{
						hitEffectsType = (this.minimalHitEffects ? EnemyHitEffectsProfile.EffectsTypes.Minimal : EnemyHitEffectsProfile.EffectsTypes.Full);
					}
					float num7;
					if (this.directionSourceOverride == DamageEnemies.DirectionSourceOverrides.AwayFromHero)
					{
						Transform transform = HeroController.instance.transform;
						Vector2 normalized = (base.transform.position - transform.position).normalized;
						num7 = Vector2.Angle(Vector2.right, normalized);
					}
					else
					{
						num7 = this.direction;
						if (this.flipDirectionIfXScaleNegative && base.transform.lossyScale.x < 0f)
						{
							num7 += 180f;
						}
						if (this.flipDirectionIfBehind && this.forwardVector != Vector2.zero)
						{
							Vector2 b = base.transform.position;
							Vector2 vector = target.transform.position - b;
							Vector2 vector2 = base.transform.TransformDirection(this.forwardVector);
							if (Vector2.Dot(vector.normalized, vector2.normalized) < 0f)
							{
								num7 += 180f;
								num2 *= this.flippedDirMagnitude;
							}
						}
					}
					while (num7 < 0f)
					{
						num7 += 360f;
					}
					while (num7 >= 360f)
					{
						num7 -= 360f;
					}
					ToolItem toolItem;
					if (this.representingTool)
					{
						toolItem = this.representingTool;
					}
					else if (this.NailImbuement)
					{
						toolItem = this.NailImbuement.ToolSource;
					}
					else
					{
						toolItem = null;
					}
					HitInstance hitInstance = new HitInstance
					{
						Source = base.gameObject,
						IsFirstHit = isFirstHit,
						AttackType = this.attackType,
						NailElement = (this.NailImbuement ? this.NailElement : NailElements.None),
						NailImbuement = this.NailImbuement,
						IsUsingNeedleDamageMult = (this.useNailDamage || this.useHeroDamageAffectors),
						RepresentingTool = this.representingTool,
						PoisonDamageTicks = this.PoisonDamageTicks,
						ZapDamageTicks = this.ZapDamageTicks,
						DamageScalingLevel = this.damageScalingLevel,
						ToolDamageFlags = (toolItem ? toolItem.DamageFlags : this.damageFlags),
						CircleDirection = (this.directionSourceOverride == DamageEnemies.DirectionSourceOverrides.CircleDirection),
						DamageDealt = num6,
						StunDamage = (this.doesNotStun ? 0f : this.stunDamage),
						CanWeakHit = this.canWeakHit,
						Direction = num7,
						UseCorpseDirection = this.corpseDirection.IsEnabled,
						CorpseDirection = this.corpseDirection.Value,
						CanTriggerBouncePod = this.canTriggerBouncePod,
						ExtraUpDirection = this.ExtraUpDirection,
						IgnoreInvulnerable = this.ignoreInvuln,
						MagnitudeMultiplier = num2,
						UseCorpseMagnitudeMult = this.corpseMagnitudeMult.IsEnabled,
						CorpseMagnitudeMultiplier = this.corpseMagnitudeMult.Value,
						UseCurrencyMagnitudeMult = this.currencyMagnitudeMult.IsEnabled,
						CurrencyMagnitudeMult = this.currencyMagnitudeMult.Value,
						MoveAngle = 0f,
						MoveDirection = this.moveDirection,
						Multiplier = 1f,
						SpecialType = this.specialType,
						SlashEffectOverrides = ((this.slashEffectOverrides.Length != 0) ? this.slashEffectOverrides : null),
						HitEffectsType = hitEffectsType,
						SilkGeneration = this.silkGeneration,
						NonLethal = this.nonLethal,
						RageHit = rageHit,
						CriticalHit = criticalHit,
						HunterCombo = hunterCombo,
						UseBouncePodDirection = this.useBouncePodDirection,
						BouncePodDirection = this.bouncePodDirection,
						IsManualTrigger = this.manualTrigger,
						IsHeroDamage = (this.isHeroDamage || this.sourceIsHero),
						IsNailTag = this.isNailAttack,
						IgnoreNailPosition = this.IgnoreNailPosition
					};
					bool flag3 = false;
					PhysLayers layer;
					if (healthManager != null)
					{
						layer = (PhysLayers)healthManager.gameObject.layer;
						flag3 = true;
						this.lastTargetDamaged = target;
						if (flag2)
						{
							FSMUtility.SendEventToGameObject(target, "TAKE DAMAGE", false);
							if (this.damageFSMEvent != "")
							{
								FSMUtility.SendEventToGameObject(target, this.damageFSMEvent, false);
							}
							if (this.dealtDamageFSM)
							{
								if (this.dealtDamageFSMEvent == "DASH HIT")
								{
									NonBouncer component = healthManager.GetComponent<NonBouncer>();
									bool flag4 = false;
									if (component)
									{
										flag4 = component.active;
									}
									if (!flag4)
									{
										this.dealtDamageFSM.SendEventSafe(this.dealtDamageFSMEvent);
									}
								}
								else
								{
									this.dealtDamageFSM.SendEventSafe(this.dealtDamageFSMEvent);
								}
							}
							if (this.dealtDamageFSM && !healthManager.GetComponent<NonBouncer>())
							{
								this.dealtDamageFSM.SendEventSafe(this.dealtDamageFSMEvent);
							}
							this.DealtDamage.Invoke();
							Action willDamageEnemy = this.WillDamageEnemy;
							if (willDamageEnemy != null)
							{
								willDamageEnemy();
							}
							Action<HealthManager, HitInstance> willDamageEnemyOptions = this.WillDamageEnemyOptions;
							if (willDamageEnemyOptions != null)
							{
								willDamageEnemyOptions(healthManager, hitInstance);
							}
						}
					}
					else
					{
						layer = (PhysLayers)target.layer;
						this.lastTargetDamaged = target;
						if (this.dealtDamageFSM && target.CompareTag("Recoiler") && target.GetComponent<IsCoralCrustWall>())
						{
							this.dealtDamageFSM.SendEventSafe(this.dealtDamageFSMEvent);
						}
					}
					if (!flag2)
					{
						this.isDoingDamage = false;
						if (!base.enabled || !base.gameObject.activeInHierarchy)
						{
							this.tempHitsResponded.Clear();
						}
						result = flag3;
					}
					else
					{
						if (flag3 && this.dealtDamageFSM)
						{
							this.lastRecordedTarget = target;
							this.dealtDamageFSM.SendEventSafe(this.targetRecordedFSMEvent);
						}
						foreach (IHitResponder responder in this.tempHitsResponded)
						{
							this.AddAndOrder(new DamageEnemies.HitResponse
							{
								Target = target,
								Responder = responder,
								Hit = hitInstance,
								HealthManager = healthManager,
								LayerOnHit = layer
							});
							flag3 = true;
						}
						this.isDoingDamage = false;
						this.tempHitsResponded.Clear();
						result = flag3;
					}
				}
			}
		}
		finally
		{
			this.NailElement = nailElement;
			this.NailImbuement = nailImbuement;
		}
		return result;
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x000740B8 File Offset: 0x000722B8
	private void AddAndOrder(DamageEnemies.HitResponse response)
	{
		int i = this.currentDamageBuffer.BinarySearch(response, DamageEnemies.orderComparer);
		if (i >= 0)
		{
			while (i < this.currentDamageBuffer.Count)
			{
				if (this.currentDamageBuffer[i].Responder.HitPriority != response.Responder.HitPriority)
				{
					break;
				}
				i++;
			}
		}
		else
		{
			i = ~i;
		}
		this.currentDamageBuffer.Insert(i, response);
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x00074124 File Offset: 0x00072324
	private void DoEnemyDamageNailImbuement(HealthManager healthManager, HitInstance hitInstance)
	{
		NailImbuementConfig nailImbuement = hitInstance.NailImbuement;
		if (!nailImbuement || !healthManager || !hitInstance.IsFirstHit)
		{
			return;
		}
		EnemyHitEffectsProfile inertHitEffect = nailImbuement.InertHitEffect;
		if (inertHitEffect)
		{
			inertHitEffect.SpawnEffects(healthManager.transform, healthManager.EffectOrigin, hitInstance, null, -1f);
		}
		HeroController instance = HeroController.instance;
		int randomValue = ((Random.Range(0f, 1f) <= nailImbuement.LuckyHitChance * instance.GetLuckModifier()) ? nailImbuement.LuckyHitsToTag : nailImbuement.HitsToTag).GetRandomValue(true);
		if (randomValue > 1 && !healthManager.CheckNailImbuementHit(nailImbuement, randomValue - 1))
		{
			return;
		}
		EnemyHitEffectsProfile startHitEffect = nailImbuement.StartHitEffect;
		if (startHitEffect)
		{
			startHitEffect.SpawnEffects(healthManager.transform, healthManager.EffectOrigin, hitInstance, null, -1f);
		}
		DamageTag damageTag = nailImbuement.DamageTag;
		if (damageTag)
		{
			healthManager.AddDamageTagToStack(damageTag, nailImbuement.DamageTagTicksOverride);
			return;
		}
		NailImbuementConfig.ImbuedLagHitOptions lagHits = nailImbuement.LagHits;
		if (lagHits == null)
		{
			return;
		}
		healthManager.DoLagHits(lagHits, hitInstance);
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x0007423C File Offset: 0x0007243C
	public void DoDamageFSM(GameObject target)
	{
		this.DoDamage(target, true);
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x00074247 File Offset: 0x00072447
	public int GetDamageDealt()
	{
		return this.damageDealt;
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x00074250 File Offset: 0x00072450
	public float GetDirection()
	{
		float num = this.direction;
		if (this.flipDirectionIfXScaleNegative && base.transform.lossyScale.x < 0f)
		{
			num += 180f;
		}
		return num;
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x0007428C File Offset: 0x0007248C
	public GameObject GetLastTargetDamaged()
	{
		return this.lastTargetDamaged;
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x00074294 File Offset: 0x00072494
	public GameObject GetLastRecordedTarget()
	{
		if (!this.lastRecordedTarget)
		{
			return this.lastTargetDamaged;
		}
		return this.lastRecordedTarget;
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000742B0 File Offset: 0x000724B0
	public bool GetDoesNotTink()
	{
		return this.doesNotTink;
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000742B8 File Offset: 0x000724B8
	public void SetDirection(float newDirection)
	{
		this.direction = newDirection;
	}

	// Token: 0x06001947 RID: 6471 RVA: 0x000742C4 File Offset: 0x000724C4
	public void SetDirectionByHeroFacing()
	{
		HeroController instance = HeroController.instance;
		this.direction = (float)(instance.cState.facingRight ? 0 : 180);
	}

	// Token: 0x06001948 RID: 6472 RVA: 0x000742F3 File Offset: 0x000724F3
	public void FlipDirection()
	{
		this.direction += 180f;
		if (this.direction > 360f)
		{
			this.direction -= 360f;
		}
	}

	// Token: 0x06001949 RID: 6473 RVA: 0x00074328 File Offset: 0x00072528
	public void StartDamage()
	{
		if (this.hasSharedDamageGroup)
		{
			this.sharedDamagedGroup.DamageStart(this);
		}
		this.stepsToNextHit = 0;
		this.hitsLeftUntilDeath = (this.HandlesMultiHitsDeath() ? this.hitsUntilDeath : 0);
		this.DidHit = false;
		this.DidHitEnemy = false;
		this.endedDamage = false;
		this.damagePrevented.Clear();
		this.frameQueue.Clear();
	}

	// Token: 0x0600194A RID: 6474 RVA: 0x00074392 File Offset: 0x00072592
	private void ClearDamageBuffer()
	{
		this.currentDamageBuffer.Clear();
	}

	// Token: 0x0600194B RID: 6475 RVA: 0x000743A0 File Offset: 0x000725A0
	public void EndDamage()
	{
		if (this.hasSharedDamageGroup)
		{
			this.sharedDamagedGroup.DamageEnd(this);
		}
		this.enteredColliders.Clear();
		this.damagedColliders.Clear();
		Dictionary<GameObject, int> dictionary = this.hitCounts;
		if (dictionary != null)
		{
			dictionary.Clear();
		}
		this.stepsToNextHit = 0;
		this.hitsLeftUntilDeath = 0;
		if (!this.endedDamage)
		{
			this.endedDamage = true;
			DamageEnemies.EndedDamageDelegate endedDamageDelegate = this.EndedDamage;
			if (endedDamageDelegate != null)
			{
				endedDamageDelegate(this.DidHit);
			}
		}
		this.DidHit = false;
		this.DidHitEnemy = false;
	}

	// Token: 0x0600194C RID: 6476 RVA: 0x0007442A File Offset: 0x0007262A
	public bool PreventDamage(Collider2D col)
	{
		if (this.hasSharedDamageGroup)
		{
			return this.sharedDamagedGroup.PreventDamage(col);
		}
		return this.damagedColliders.Add(col);
	}

	// Token: 0x0600194D RID: 6477 RVA: 0x0007444D File Offset: 0x0007264D
	public void PreventDamage(IHitResponder hitResponder)
	{
		if (this.hasSharedDamageGroup)
		{
			this.sharedDamagedGroup.PreventDamage(hitResponder);
		}
		this.damagePrevented.Add(hitResponder);
	}

	// Token: 0x0600194E RID: 6478 RVA: 0x00074470 File Offset: 0x00072670
	public void ClearPreventDamage()
	{
		if (this.hasSharedDamageGroup)
		{
			this.sharedDamagedGroup.ClearDamagePrevented();
		}
		this.damagePrevented.Clear();
	}

	// Token: 0x0600194F RID: 6479 RVA: 0x00074490 File Offset: 0x00072690
	public bool HasBeenDamaged(IHitResponder hitResponder)
	{
		if (this.hasSharedDamageGroup)
		{
			return this.sharedDamagedGroup.HasDamaged(hitResponder);
		}
		return this.hitsResponded.Contains(hitResponder) || this.damagePrevented.Contains(hitResponder);
	}

	// Token: 0x06001950 RID: 6480 RVA: 0x000744C3 File Offset: 0x000726C3
	public bool HasResponded(IHitResponder hitResponder)
	{
		return this.hitsResponded.Contains(hitResponder);
	}

	// Token: 0x06001951 RID: 6481 RVA: 0x000744D1 File Offset: 0x000726D1
	public void TryClearRespondedList()
	{
		if (this.lastRespondedCycle != CustomPlayerLoop.FixedUpdateCycle)
		{
			this.hitsResponded.Clear();
		}
	}

	// Token: 0x06001952 RID: 6482 RVA: 0x000744EB File Offset: 0x000726EB
	public void SendParried()
	{
		this.SendParried(true);
	}

	// Token: 0x06001953 RID: 6483 RVA: 0x000744F4 File Offset: 0x000726F4
	public void SendParried(bool bouncable)
	{
		Action parriedEnemy = this.ParriedEnemy;
		if (parriedEnemy != null)
		{
			parriedEnemy();
		}
		if (bouncable)
		{
			this.OnBounceableTink();
		}
	}

	// Token: 0x06001954 RID: 6484 RVA: 0x00074510 File Offset: 0x00072710
	public void OnTinkEffectTink()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "DAMAGER TINKED");
		this.Tinked.Invoke();
	}

	// Token: 0x06001955 RID: 6485 RVA: 0x0007452D File Offset: 0x0007272D
	public void OnBounceableTink()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED");
	}

	// Token: 0x06001956 RID: 6486 RVA: 0x0007453F File Offset: 0x0007273F
	public void OnBounceableTinkDown()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED DOWN");
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x00074551 File Offset: 0x00072751
	public void OnBounceableTinkUp()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED UP");
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x00074563 File Offset: 0x00072763
	public void OnBounceableTinkRight()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED RIGHT");
	}

	// Token: 0x06001959 RID: 6489 RVA: 0x00074575 File Offset: 0x00072775
	public void OnBounceableTinkLeft()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "BOUNCE TINKED LEFT");
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x00074587 File Offset: 0x00072787
	public void OnHitSpikes()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "DAMAGER HIT SPIKES");
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x00074599 File Offset: 0x00072799
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.None, false);
	}

	// Token: 0x0600195C RID: 6492 RVA: 0x000745A8 File Offset: 0x000727A8
	public void CopyDamagePropsFrom(DamageEnemies otherDamager)
	{
		this.attackType = otherDamager.attackType;
		this.useNailDamage = otherDamager.useNailDamage;
		this.nailDamageMultiplier = otherDamager.nailDamageMultiplier;
		this.damageDealt = otherDamager.damageDealt;
		this.damageAsset = otherDamager.damageAsset;
		this.useHeroDamageAffectors = otherDamager.useHeroDamageAffectors;
		this.canWeakHit = otherDamager.canWeakHit;
	}

	// Token: 0x0600195D RID: 6493 RVA: 0x00074609 File Offset: 0x00072809
	public void OverridePoisonDamage(int value)
	{
		this.isPoisonDamageOverridden = true;
		this.poisonDamageTicks = value;
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x0007470B File Offset: 0x0007290B
	bool CustomPlayerLoop.ILateFixedUpdate.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x040017EA RID: 6122
	private static DamageEnemies.HitOrderComparer orderComparer = new DamageEnemies.HitOrderComparer();

	// Token: 0x040017F5 RID: 6133
	public AttackTypes attackType = AttackTypes.Generic;

	// Token: 0x040017F6 RID: 6134
	[EnumPickerBitmask]
	public SpecialTypes specialType;

	// Token: 0x040017F7 RID: 6135
	[Space]
	public bool useNailDamage;

	// Token: 0x040017F8 RID: 6136
	[ModifiableProperty]
	[Conditional("useNailDamage", true, false, false)]
	public float nailDamageMultiplier = 1f;

	// Token: 0x040017F9 RID: 6137
	[ModifiableProperty]
	[Conditional("ShowDamageDealt", true, true, false)]
	public int damageDealt;

	// Token: 0x040017FA RID: 6138
	[SerializeField]
	[ModifiableProperty]
	[Conditional("useNailDamage", false, false, false)]
	[QuickCreateAsset("Data Assets/Damages", "damageDealt", "value")]
	private DamageReference damageAsset;

	// Token: 0x040017FB RID: 6139
	[ModifiableProperty]
	[Conditional("useNailDamage", false, false, false)]
	public bool useHeroDamageAffectors;

	// Token: 0x040017FC RID: 6140
	public int damageDealtOffset;

	// Token: 0x040017FD RID: 6141
	[SerializeField]
	private bool ignoreNailPosition;

	// Token: 0x040017FE RID: 6142
	[SerializeField]
	private float[] damageMultPerHit;

	// Token: 0x040017FF RID: 6143
	[ModifiableProperty]
	[Conditional("doesNotStun", false, false, false)]
	public float stunDamage = 1f;

	// Token: 0x04001800 RID: 6144
	public bool canWeakHit;

	// Token: 0x04001801 RID: 6145
	[SerializeField]
	private bool onlyDamageEnemies;

	// Token: 0x04001802 RID: 6146
	[SerializeField]
	private int onlyDamageEnemiesChance = 100;

	// Token: 0x04001803 RID: 6147
	[SerializeField]
	private bool isHeroDamage;

	// Token: 0x04001804 RID: 6148
	[Space]
	[SerializeField]
	private SharedDamagedGroup sharedDamagedGroup;

	// Token: 0x04001805 RID: 6149
	private bool hasSharedDamageGroup;

	// Token: 0x04001806 RID: 6150
	[Space]
	[SerializeField]
	private DamageEnemies.DirectionSourceOverrides directionSourceOverride;

	// Token: 0x04001807 RID: 6151
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool circleDirection;

	// Token: 0x04001808 RID: 6152
	public float direction;

	// Token: 0x04001809 RID: 6153
	public OverrideFloat corpseDirection;

	// Token: 0x0400180A RID: 6154
	[SerializeField]
	private bool canTriggerBouncePod;

	// Token: 0x0400180B RID: 6155
	[SerializeField]
	private bool useBouncePodDirection;

	// Token: 0x0400180C RID: 6156
	[SerializeField]
	private float bouncePodDirection;

	// Token: 0x0400180D RID: 6157
	[SerializeField]
	private bool flipDirectionIfXScaleNegative;

	// Token: 0x0400180E RID: 6158
	[Space]
	[SerializeField]
	private bool flipDirectionIfBehind;

	// Token: 0x0400180F RID: 6159
	[SerializeField]
	[ModifiableProperty]
	[Conditional("flipDirectionIfBehind", true, false, false)]
	private Vector2 forwardVector;

	// Token: 0x04001810 RID: 6160
	[SerializeField]
	[ModifiableProperty]
	[Conditional("flipDirectionIfBehind", true, false, false)]
	private float flippedDirMagnitude = 1f;

	// Token: 0x04001811 RID: 6161
	[Space]
	public bool ignoreInvuln = true;

	// Token: 0x04001812 RID: 6162
	public float magnitudeMult;

	// Token: 0x04001813 RID: 6163
	public OverrideFloat corpseMagnitudeMult;

	// Token: 0x04001814 RID: 6164
	public OverrideFloat currencyMagnitudeMult;

	// Token: 0x04001815 RID: 6165
	public bool moveDirection;

	// Token: 0x04001816 RID: 6166
	[SerializeField]
	private ToolItem representingTool;

	// Token: 0x04001817 RID: 6167
	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private ToolDamageFlags damageFlags;

	// Token: 0x04001818 RID: 6168
	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private int poisonDamageTicks;

	// Token: 0x04001819 RID: 6169
	[SerializeField]
	[ModifiableProperty]
	[Conditional("representingTool", false, false, false)]
	private int zapDamageTicks;

	// Token: 0x0400181A RID: 6170
	public bool manualTrigger;

	// Token: 0x0400181B RID: 6171
	public bool hitOnceUntilEnd;

	// Token: 0x0400181C RID: 6172
	public bool endDamageEnemyExit;

	// Token: 0x0400181D RID: 6173
	[Space]
	[SerializeField]
	private int damageScalingLevel;

	// Token: 0x0400181E RID: 6174
	[Space]
	public bool multiHitter;

	// Token: 0x0400181F RID: 6175
	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public int stepsPerHit;

	// Token: 0x04001820 RID: 6176
	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public int hitsUntilDeath;

	// Token: 0x04001821 RID: 6177
	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public float finalHitMagMultOverride;

	// Token: 0x04001822 RID: 6178
	[ModifiableProperty]
	[Conditional("HandlesMultiHitsDeath", true, true, false)]
	public PlayMakerFSM deathEventTarget;

	// Token: 0x04001823 RID: 6179
	[ModifiableProperty]
	[InspectorValidation("IsDeathEventValid")]
	[Conditional("ShowDeathEventField", true, true, false)]
	public string deathEvent;

	// Token: 0x04001824 RID: 6180
	[ModifiableProperty]
	[Conditional("HandlesMultiHitsDeath", true, true, false)]
	public bool deathEndDamage;

	// Token: 0x04001825 RID: 6181
	public string contactFSMEvent;

	// Token: 0x04001826 RID: 6182
	public string damageFSMEvent;

	// Token: 0x04001827 RID: 6183
	public PlayMakerFSM dealtDamageFSM;

	// Token: 0x04001828 RID: 6184
	public string dealtDamageFSMEvent;

	// Token: 0x04001829 RID: 6185
	public string targetRecordedFSMEvent;

	// Token: 0x0400182A RID: 6186
	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public EnemyHitEffectsProfile.EffectsTypes multiHitFirstEffects;

	// Token: 0x0400182B RID: 6187
	[ModifiableProperty]
	[Conditional("multiHitter", true, false, false)]
	public EnemyHitEffectsProfile.EffectsTypes multiHitEffects;

	// Token: 0x0400182C RID: 6188
	[Space]
	public GameObject[] slashEffectOverrides;

	// Token: 0x0400182D RID: 6189
	[ModifiableProperty]
	[Conditional("multiHitter", false, false, false)]
	public bool minimalHitEffects;

	// Token: 0x0400182E RID: 6190
	public bool doesNotStun;

	// Token: 0x0400182F RID: 6191
	public bool doesNotTink;

	// Token: 0x04001830 RID: 6192
	public bool doesNotTinkThroughWalls;

	// Token: 0x04001831 RID: 6193
	public bool doesNotParry;

	// Token: 0x04001832 RID: 6194
	[SerializeField]
	private DamageEnemies.DoesNotFlags doesNotFlags;

	// Token: 0x04001833 RID: 6195
	[SerializeField]
	private ITinkResponder.TinkFlags allowedTinkFlags;

	// Token: 0x04001834 RID: 6196
	public bool nonLethal;

	// Token: 0x04001835 RID: 6197
	[SerializeField]
	private bool doesNotCriticalHit;

	// Token: 0x04001836 RID: 6198
	[SerializeField]
	private bool forceSpikeUpdate;

	// Token: 0x04001837 RID: 6199
	[SerializeField]
	private HitSilkGeneration silkGeneration;

	// Token: 0x04001838 RID: 6200
	[SerializeField]
	private bool awardJournalKill = true;

	// Token: 0x04001839 RID: 6201
	[SerializeField]
	private FreezeMomentTypes firstHitFreeze = FreezeMomentTypes.None;

	// Token: 0x0400183A RID: 6202
	[Space]
	[SerializeField]
	private LagHitOptionsProfile lagHitOptionsProfile;

	// Token: 0x0400183B RID: 6203
	[SerializeField]
	[ModifiableProperty]
	[Conditional("lagHitOptionsProfile", false, false, false)]
	private LagHitOptions lagHitOptions;

	// Token: 0x0400183C RID: 6204
	[Space]
	[SerializeField]
	public UnityEvent DealtDamage;

	// Token: 0x0400183D RID: 6205
	[SerializeField]
	public UnityEvent Tinked;

	// Token: 0x0400183E RID: 6206
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool doesNotGenerateSilk;

	// Token: 0x0400183F RID: 6207
	private int stepsToNextHit;

	// Token: 0x04001840 RID: 6208
	private int hitsLeftUntilDeath;

	// Token: 0x04001841 RID: 6209
	private bool endedDamage;

	// Token: 0x04001842 RID: 6210
	private GameObject lastTargetDamaged;

	// Token: 0x04001843 RID: 6211
	private GameObject lastRecordedTarget;

	// Token: 0x04001844 RID: 6212
	private bool isPoisonDamageOverridden;

	// Token: 0x04001845 RID: 6213
	private bool wasCriticalHitForced;

	// Token: 0x04001846 RID: 6214
	private readonly HashSet<Collider2D> frameQueue = new HashSet<Collider2D>();

	// Token: 0x04001847 RID: 6215
	private readonly HashSet<Collider2D> enteredColliders = new HashSet<Collider2D>();

	// Token: 0x04001848 RID: 6216
	private readonly List<Collider2D> evaluatingColliders = new List<Collider2D>();

	// Token: 0x04001849 RID: 6217
	private readonly HashSet<Collider2D> damagedColliders = new HashSet<Collider2D>();

	// Token: 0x0400184A RID: 6218
	private readonly HashSet<IHitResponder> hitsResponded = new HashSet<IHitResponder>();

	// Token: 0x0400184B RID: 6219
	private readonly List<IHitResponder> tempHitsResponded = new List<IHitResponder>();

	// Token: 0x0400184C RID: 6220
	private readonly HashSet<IHitResponder> damagePrevented = new HashSet<IHitResponder>();

	// Token: 0x0400184D RID: 6221
	private List<DamageEnemies.HitResponse> currentDamageBuffer = new List<DamageEnemies.HitResponse>();

	// Token: 0x0400184E RID: 6222
	private List<DamageEnemies.HitResponse> processingDamageBuffer = new List<DamageEnemies.HitResponse>();

	// Token: 0x0400184F RID: 6223
	private List<DamageEnemies.SpikeSlashData> spikeSlashReactions = new List<DamageEnemies.SpikeSlashData>();

	// Token: 0x04001850 RID: 6224
	private int lastRespondedCycle = -1;

	// Token: 0x04001851 RID: 6225
	private Dictionary<GameObject, int> hitCounts;

	// Token: 0x04001857 RID: 6231
	private float damageMultiplier = 1f;

	// Token: 0x04001858 RID: 6232
	private bool isProcessingBuffer;

	// Token: 0x04001859 RID: 6233
	private bool isDoingDamage;

	// Token: 0x0400185A RID: 6234
	private bool sourceIsHero;

	// Token: 0x0400185B RID: 6235
	private bool isNailAttack;

	// Token: 0x0400185C RID: 6236
	private bool started;

	// Token: 0x0400185D RID: 6237
	private readonly DamageStack tempDamageStack = new DamageStack();

	// Token: 0x020015A8 RID: 5544
	public struct HitResponse
	{
		// Token: 0x0400882A RID: 34858
		public GameObject Target;

		// Token: 0x0400882B RID: 34859
		public IHitResponder Responder;

		// Token: 0x0400882C RID: 34860
		public HitInstance Hit;

		// Token: 0x0400882D RID: 34861
		public HealthManager HealthManager;

		// Token: 0x0400882E RID: 34862
		public PhysLayers LayerOnHit;
	}

	// Token: 0x020015A9 RID: 5545
	private enum DirectionSourceOverrides
	{
		// Token: 0x04008830 RID: 34864
		None,
		// Token: 0x04008831 RID: 34865
		CircleDirection,
		// Token: 0x04008832 RID: 34866
		AwayFromHero
	}

	// Token: 0x020015AA RID: 5546
	private sealed class HitOrderComparer : IComparer<DamageEnemies.HitResponse>
	{
		// Token: 0x060087BB RID: 34747 RVA: 0x00277B84 File Offset: 0x00275D84
		public int Compare(DamageEnemies.HitResponse x, DamageEnemies.HitResponse y)
		{
			return y.Responder.HitPriority.CompareTo(x.Responder.HitPriority);
		}
	}

	// Token: 0x020015AB RID: 5547
	// (Invoke) Token: 0x060087BE RID: 34750
	public delegate void EndedDamageDelegate(bool didHit);

	// Token: 0x020015AC RID: 5548
	private struct SpikeSlashData
	{
		// Token: 0x060087C1 RID: 34753 RVA: 0x00277BB7 File Offset: 0x00275DB7
		public SpikeSlashData(SpikeSlashReaction spikeSlashReaction, Collider2D collider2D)
		{
			this.spikeSlashReaction = spikeSlashReaction;
			this.collider2D = collider2D;
		}

		// Token: 0x04008833 RID: 34867
		public SpikeSlashReaction spikeSlashReaction;

		// Token: 0x04008834 RID: 34868
		public Collider2D collider2D;
	}

	// Token: 0x020015AD RID: 5549
	[Flags]
	[Serializable]
	private enum DoesNotFlags
	{
		// Token: 0x04008836 RID: 34870
		None = 0,
		// Token: 0x04008837 RID: 34871
		PoisonTicks = 1,
		// Token: 0x04008838 RID: 34872
		LightningTicks = 2
	}
}
