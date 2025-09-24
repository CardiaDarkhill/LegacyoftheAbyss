using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Token: 0x020002EA RID: 746
public class HealthManager : MonoBehaviour, IHitResponder, ITagDamageTakerOwner, IInitialisable
{
	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06001A6B RID: 6763 RVA: 0x00079D4F File Offset: 0x00077F4F
	public HealthManager SendDamageTo
	{
		get
		{
			return this.sendDamageTo;
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001A6C RID: 6764 RVA: 0x00079D57 File Offset: 0x00077F57
	public bool IsPartOfSendToTarget
	{
		get
		{
			return this.isPartOfSendToTarget;
		}
	}

	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06001A6D RID: 6765 RVA: 0x00079D5F File Offset: 0x00077F5F
	public Vector3 EffectOrigin
	{
		get
		{
			return this.effectOrigin;
		}
	}

	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06001A6E RID: 6766 RVA: 0x00079D67 File Offset: 0x00077F67
	// (set) Token: 0x06001A6F RID: 6767 RVA: 0x00079D6F File Offset: 0x00077F6F
	public HealthManager.EnemyTypes EnemyType
	{
		get
		{
			return this.enemyType;
		}
		set
		{
			this.enemyType = value;
		}
	}

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x00079D78 File Offset: 0x00077F78
	public bool DoNotGiveSilk
	{
		get
		{
			return this.doNotGiveSilk;
		}
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001A71 RID: 6769 RVA: 0x00079D80 File Offset: 0x00077F80
	public Vector3 TargetPoint
	{
		get
		{
			if (this.hasTargetPointOverride)
			{
				return this.targetPointOverride.position;
			}
			return base.transform.position;
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001A72 RID: 6770 RVA: 0x00079DA1 File Offset: 0x00077FA1
	public bool MegaFlingGeo
	{
		get
		{
			return this.megaFlingGeo;
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x06001A73 RID: 6771 RVA: 0x00079DA9 File Offset: 0x00077FA9
	public bool PreventInvincibleEffect
	{
		get
		{
			return this.preventInvincibleEffect;
		}
	}

	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06001A74 RID: 6772 RVA: 0x00079DB4 File Offset: 0x00077FB4
	// (remove) Token: 0x06001A75 RID: 6773 RVA: 0x00079DEC File Offset: 0x00077FEC
	public event HealthManager.DeathEvent OnDeath;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x06001A76 RID: 6774 RVA: 0x00079E24 File Offset: 0x00078024
	// (remove) Token: 0x06001A77 RID: 6775 RVA: 0x00079E5C File Offset: 0x0007805C
	public event Action StartedDead;

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x06001A78 RID: 6776 RVA: 0x00079E91 File Offset: 0x00078091
	// (set) Token: 0x06001A79 RID: 6777 RVA: 0x00079E99 File Offset: 0x00078099
	public bool IsInvincible
	{
		get
		{
			return this.invincible;
		}
		set
		{
			this.invincible = value;
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001A7A RID: 6778 RVA: 0x00079EA2 File Offset: 0x000780A2
	// (set) Token: 0x06001A7B RID: 6779 RVA: 0x00079EAA File Offset: 0x000780AA
	public int InvincibleFromDirection
	{
		get
		{
			return this.invincibleFromDirection;
		}
		set
		{
			this.invincibleFromDirection = value;
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x06001A7C RID: 6780 RVA: 0x00079EB3 File Offset: 0x000780B3
	public bool ImmuneToCoal
	{
		get
		{
			return this.immuneToCoal;
		}
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x06001A7D RID: 6781 RVA: 0x00079EBB File Offset: 0x000780BB
	// (set) Token: 0x06001A7E RID: 6782 RVA: 0x00079EC3 File Offset: 0x000780C3
	public bool HasClearedItemDrops { get; private set; }

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06001A7F RID: 6783 RVA: 0x00079ECC File Offset: 0x000780CC
	// (set) Token: 0x06001A80 RID: 6784 RVA: 0x00079ED4 File Offset: 0x000780D4
	public bool WillAwardJournalKill { get; private set; }

	// Token: 0x1400004E RID: 78
	// (add) Token: 0x06001A81 RID: 6785 RVA: 0x00079EE0 File Offset: 0x000780E0
	// (remove) Token: 0x06001A82 RID: 6786 RVA: 0x00079F18 File Offset: 0x00078118
	public event Action TookDamage;

	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06001A83 RID: 6787 RVA: 0x00079F4D File Offset: 0x0007814D
	public SpriteFlash SpriteFlash
	{
		get
		{
			return base.GetComponent<SpriteFlash>();
		}
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06001A84 RID: 6788 RVA: 0x00079F55 File Offset: 0x00078155
	public Vector2 TagDamageEffectPos
	{
		get
		{
			return this.EffectOrigin;
		}
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00079F64 File Offset: 0x00078164
	private void OnValidate()
	{
		if ((this.itemDropGroups == null || this.itemDropGroups.Count == 0) && (this._itemDropProbability > 0f || (this._itemDrops != null && this._itemDrops.Length != 0)))
		{
			this.itemDropGroups = new List<HealthManager.ItemDropGroup>
			{
				new HealthManager.ItemDropGroup
				{
					TotalProbability = this._itemDropProbability,
					Drops = new List<HealthManager.ItemDropProbability>(this._itemDrops)
				}
			};
			this._itemDropProbability = 0f;
			this._itemDrops = null;
		}
		this.hasTargetPointOverride = (this.targetPointOverride != null);
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x00079FFB File Offset: 0x000781FB
	protected void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0007A004 File Offset: 0x00078204
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.OnValidate();
		this.corpseEventResponder = base.gameObject.GetComponent<EventRelayResponder>();
		this.boxCollider = base.GetComponent<BoxCollider2D>();
		this.recoil = base.GetComponent<Recoil>();
		this.hitEffectReceiver = base.GetComponent<IHitEffectReciever>();
		this.enemyDeathEffects = base.GetComponent<EnemyDeathEffects>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.damageHero = base.GetComponent<DamageHero>();
		this.tagDamageTaker = TagDamageTaker.Add(base.gameObject, this);
		this.tagDamageTaker.SetIgnoreColliderState(this.tagDamageTakerIgnoreColliderState);
		this.blackThreadState = base.GetComponent<BlackThreadState>();
		this.hasBlackThreadState = (this.blackThreadState != null);
		this.initHp = this.hp;
		this.HealToMax();
		foreach (PlayMakerFSM playMakerFSM in base.gameObject.GetComponents<PlayMakerFSM>())
		{
			if (playMakerFSM.FsmName == "Stun Control" || playMakerFSM.FsmName == "Stun")
			{
				this.stunControlFsm = playMakerFSM;
				break;
			}
		}
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component != null)
		{
			component.OnGetSaveState += delegate(out bool val)
			{
				if (this.ignorePersistence)
				{
					val = false;
					return;
				}
				val = this.isDead;
			};
			if (component.LoadedValue && component.ItemData.Value)
			{
				this.<OnAwake>g__OnSaveState|171_1(true);
			}
			component.OnSetSaveState += this.<OnAwake>g__OnSaveState|171_1;
		}
		return true;
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x0007A170 File Offset: 0x00078370
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		this.evasionByHitRemaining = -1f;
		if (!string.IsNullOrEmpty(this.sendKilledToName))
		{
			this.sendKilledTo = GameObject.Find(this.sendKilledToName);
		}
		else if (this.sendKilledToObject != null)
		{
			this.sendKilledTo = this.sendKilledToObject;
		}
		this.AddPhysicalPusher();
		foreach (HealthManager.ItemDropGroup itemDropGroup in this.itemDropGroups)
		{
			if (itemDropGroup.Drops.Count > 0 && itemDropGroup.TotalProbability >= 1f)
			{
				foreach (HealthManager.ItemDropProbability itemDropProbability in itemDropGroup.Drops)
				{
					if (itemDropProbability.CustomPickupPrefab && itemDropProbability.LimitActiveInScene > 0)
					{
						PersonalObjectPool.EnsurePooledInScene(base.gameObject, itemDropProbability.CustomPickupPrefab.gameObject, itemDropProbability.LimitActiveInScene, false, false, false);
					}
				}
			}
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		return true;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x0007A2BC File Offset: 0x000784BC
	protected void OnEnable()
	{
		HealthManager._activeHealthManagers.AddIfNotPresent(this);
		this.hasTargetPointOverride = (this.targetPointOverride != null);
		base.StartCoroutine(this.CheckPersistence());
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x0007A2E9 File Offset: 0x000784E9
	private void OnDisable()
	{
		HealthManager._activeHealthManagers.Remove(this);
		this.queuedDropItem.Reset();
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x0007A302 File Offset: 0x00078502
	public static IEnumerable<HealthManager> EnumerateActiveEnemies()
	{
		return HealthManager._activeHealthManagers;
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x0007A309 File Offset: 0x00078509
	protected void Start()
	{
		this.OnStart();
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x0007A312 File Offset: 0x00078512
	protected IEnumerator CheckPersistence()
	{
		yield return null;
		if (this.isDead)
		{
			base.gameObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x0007A324 File Offset: 0x00078524
	protected void Update()
	{
		if (!this.isDead && base.transform.position.y < -400f)
		{
			this.Die(null, AttackTypes.Generic, NailElements.None, null, false, 1f, false, false);
		}
		this.evasionByHitRemaining -= Time.deltaTime;
		if (this.rapidBulletTimer > 0f)
		{
			this.rapidBulletTimer -= Time.deltaTime;
			if (this.rapidBulletTimer <= 0f)
			{
				this.rapidBulletCount = 0;
			}
		}
		if (this.rapidBombTimer > 0f)
		{
			this.rapidBombTimer -= Time.deltaTime;
			if (this.rapidBombTimer <= 0f)
			{
				this.rapidBombCount = 0;
			}
		}
		if (this.tinkTimer > 0f)
		{
			this.tinkTimer -= Time.deltaTime;
		}
		this.tagDamageTaker.Tick(this.takeTagDamageWhileInvincible || !this.IsInvincible || this.piercable || this.invincibleFromDirection != 0);
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x0007A430 File Offset: 0x00078630
	public IHitResponder.HitResponse Hit(HitInstance hitInstance)
	{
		if (this.isDead)
		{
			return IHitResponder.Response.None;
		}
		if (this.evasionByHitRemaining > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit && hitInstance.DamageDealt <= 0 && !hitInstance.CanWeakHit)
		{
			return IHitResponder.Response.None;
		}
		FSMUtility.SendEventToGameObject(hitInstance.Source, "DEALT DAMAGE", false);
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		if (this.IsBlockingByDirection(cardinalDirection, hitInstance.AttackType, hitInstance.SpecialType))
		{
			this.Invincible(hitInstance);
			return IHitResponder.Response.Invincible;
		}
		this.TakeDamage(hitInstance);
		return IHitResponder.Response.DamageEnemy;
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x0007A4D8 File Offset: 0x000786D8
	private void Invincible(HitInstance hitInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		this.lastHitInstance = hitInstance;
		this.directionOfLastAttack = cardinalDirection;
		this.lastAttackType = hitInstance.AttackType;
		DamageEnemies component = hitInstance.Source.GetComponent<DamageEnemies>();
		if (component && !this.dontSendTinkToDamager)
		{
			component.OnTinkEffectTink();
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT", false);
		NonBouncer component2 = base.GetComponent<NonBouncer>();
		bool flag = false;
		if (component2)
		{
			flag = component2.active;
		}
		if (!flag)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
		}
		if (!this.preventInvincibleAttackBlock)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "ATTACK BLOCKED", false);
		}
		if (this.invincibleRecoil && this.recoil != null)
		{
			this.recoil.RecoilByDirection(cardinalDirection, hitInstance.MagnitudeMultiplier);
		}
		if (!(base.GetComponent<DontClinkGates>() != null) && this.tinkTimer <= 0f)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "HIT", false);
			if (!this.PreventInvincibleEffect)
			{
				if (!this.preventInvincibleShake && hitInstance.AttackType == AttackTypes.Nail)
				{
					if (cardinalDirection == 0)
					{
						HeroController.instance.RecoilLeft();
					}
					else if (cardinalDirection == 2)
					{
						HeroController.instance.RecoilRight();
					}
				}
				if (!this.preventInvincibleShake)
				{
					if (hitInstance.IsNailTag)
					{
						Effects.BlockedHitShake.DoShake(this, true);
					}
					else
					{
						Effects.BlockedHitShakeNoFreeze.DoShake(this, true);
					}
				}
				BoxCollider2D component3 = this.boxCollider;
				if (component3 == null)
				{
					component3 = base.GetComponent<BoxCollider2D>();
				}
				Vector2 v;
				Vector3 eulerAngles;
				if (component3 != null)
				{
					switch (cardinalDirection)
					{
					case 0:
						v = new Vector2(base.transform.GetPositionX() + component3.offset.x - component3.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
						eulerAngles = new Vector3(0f, 0f, 0f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT R", false);
						break;
					case 1:
						v = new Vector2(hitInstance.Source.transform.GetPositionX(), Mathf.Max(hitInstance.Source.transform.GetPositionY(), base.transform.GetPositionY() + component3.offset.y - component3.size.y * 0.5f));
						eulerAngles = new Vector3(0f, 0f, 90f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT U", false);
						break;
					case 2:
						v = new Vector2(base.transform.GetPositionX() + component3.offset.x + component3.size.x * 0.5f, hitInstance.Source.transform.GetPositionY());
						eulerAngles = new Vector3(0f, 0f, 180f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED HIT L", false);
						break;
					case 3:
					{
						float positionX = hitInstance.Source.transform.GetPositionX();
						float positionY = hitInstance.Source.transform.GetPositionY();
						float num = base.transform.GetPositionX() + component3.offset.x - component3.size.x * 0.5f;
						float num2 = base.transform.GetPositionX() + component3.offset.x + component3.size.x * 0.5f;
						float num3 = base.transform.GetPositionY() + component3.offset.y + component3.size.y * 0.5f;
						float x;
						if (positionX < num)
						{
							x = num;
						}
						else if (positionX > num2)
						{
							x = num2;
						}
						else
						{
							x = positionX;
						}
						float y;
						if (positionY > num3)
						{
							y = num3;
						}
						else
						{
							y = positionY;
						}
						v = new Vector2(x, y);
						eulerAngles = new Vector3(0f, 0f, 270f);
						FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED DOWN", false);
						break;
					}
					default:
						v = base.transform.position;
						eulerAngles = new Vector3(0f, 0f, 0f);
						break;
					}
				}
				else
				{
					v = base.transform.position;
					eulerAngles = new Vector3(0f, 0f, 0f);
				}
				GameObject gameObject = this.blockHitPrefab.Spawn();
				gameObject.transform.position = v;
				gameObject.transform.eulerAngles = eulerAngles;
				if (this.hasAlternateInvincibleSound)
				{
					AudioSource component4 = base.GetComponent<AudioSource>();
					if (this.alternateInvincibleSound != null && component4 != null)
					{
						component4.PlayOneShot(this.alternateInvincibleSound);
					}
				}
				else
				{
					this.regularInvincibleAudio.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
				}
			}
			this.tinkTimer = 0.1f;
		}
		this.evasionByHitRemaining = 0f;
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x0007A9DC File Offset: 0x00078BDC
	private HitInstance ApplyDamageScaling(HitInstance hitInstance)
	{
		int level;
		if (hitInstance.IsUsingNeedleDamageMult)
		{
			level = PlayerData.instance.nailUpgrades;
		}
		else if (hitInstance.RepresentingTool && hitInstance.RepresentingTool.Type != ToolItemType.Skill)
		{
			level = PlayerData.instance.ToolKitUpgrades;
		}
		else
		{
			level = hitInstance.DamageScalingLevel - 1;
		}
		float multFromLevel = this.damageScaling.GetMultFromLevel(level);
		hitInstance.DamageDealt = Mathf.RoundToInt((float)hitInstance.DamageDealt * multFromLevel);
		return hitInstance;
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x0007AA54 File Offset: 0x00078C54
	private void TakeDamage(HitInstance hitInstance)
	{
		if (hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full && this.hasBlackThreadState && this.blackThreadState.IsVisiblyThreaded)
		{
			hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.Minimal;
		}
		if (this.IsImmuneTo(hitInstance, true))
		{
			return;
		}
		DamageEnemies component = hitInstance.Source.GetComponent<DamageEnemies>();
		if (component && component.AwardJournalKill)
		{
			this.WillAwardJournalKill = true;
		}
		hitInstance = this.ApplyDamageScaling(hitInstance);
		if ((hitInstance.SpecialType & SpecialTypes.RapidBullet) != SpecialTypes.None)
		{
			this.rapidBulletTimer = 0.15f;
			this.rapidBulletCount++;
			if (this.rapidBulletCount > 1)
			{
				hitInstance.DamageDealt /= this.rapidBulletCount;
				if (hitInstance.DamageDealt < 1)
				{
					hitInstance.DamageDealt = 1;
				}
			}
		}
		if ((hitInstance.SpecialType & SpecialTypes.RapidBomb) != SpecialTypes.None)
		{
			this.rapidBombTimer = 0.75f;
			this.rapidBombCount++;
			if (this.rapidBombCount > 1)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Rapid Bomb trying to deal ",
					hitInstance.DamageDealt.ToString(),
					" damage. Getting divided by ",
					this.rapidBombCount.ToString(),
					"..."
				}));
				hitInstance.DamageDealt /= this.rapidBombCount;
				if (hitInstance.DamageDealt < 1)
				{
					hitInstance.DamageDealt = 1;
				}
				Debug.Log("New rapid bomb damage: " + hitInstance.DamageDealt.ToString());
			}
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && this.isMossExtractable && QuestManager.GetQuest("Extractor Green").IsAccepted && !QuestManager.GetQuest("Extractor Green").IsCompleted)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT", false);
			EventRegister.SendEvent(EventRegisterEvents.StartExtractM, null);
			return;
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && this.isSwampExtractable && QuestManager.GetQuest("Extractor Swamp").IsAccepted && !QuestManager.GetQuest("Extractor Swamp").IsCompleted)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT", false);
			EventRegister.SendEvent(EventRegisterEvents.StartExtractSwamp, null);
			return;
		}
		if (hitInstance.AttackType == AttackTypes.ExtractMoss && this.isBluebloodExtractable)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "EXTRACT", false);
			EventRegister.SendEvent(EventRegisterEvents.StartExtractBlueblood, null);
			return;
		}
		if (hitInstance.CriticalHit)
		{
			hitInstance.DamageDealt = Mathf.RoundToInt((float)hitInstance.DamageDealt * Gameplay.WandererCritMultiplier);
			hitInstance.MagnitudeMultiplier *= Gameplay.WandererCritMagnitudeMult;
			GameObject wandererCritEffect = Gameplay.WandererCritEffect;
			if (wandererCritEffect)
			{
				wandererCritEffect.Spawn(base.transform.TransformPoint(this.effectOrigin)).transform.SetRotation2D(hitInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
			}
			GameManager.instance.FreezeMoment(FreezeMomentTypes.HeroDamageShort, null);
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
		if (hitInstance.HunterCombo)
		{
			GameObject hunterComboDamageEffect = Gameplay.HunterComboDamageEffect;
			if (hunterComboDamageEffect)
			{
				GameObject gameObject = hunterComboDamageEffect.Spawn(base.transform.TransformPoint(this.effectOrigin));
				switch (cardinalDirection)
				{
				case 0:
					gameObject.transform.SetRotation2D(0f);
					break;
				case 1:
				case 3:
					gameObject.transform.SetRotation2D(90f);
					break;
				case 2:
					gameObject.transform.SetRotation2D(180f);
					break;
				}
				Vector3 localScale = hunterComboDamageEffect.transform.localScale;
				if (Gameplay.HunterCrest3.IsEquipped && HeroController.instance.HunterUpgState.IsComboMeterAboveExtra)
				{
					Vector2 hunterCombo2DamageExtraScale = Gameplay.HunterCombo2DamageExtraScale;
					localScale.x *= hunterCombo2DamageExtraScale.x;
					localScale.y *= hunterCombo2DamageExtraScale.y;
				}
				gameObject.transform.localScale = localScale;
			}
		}
		this.lastHitInstance = hitInstance;
		this.directionOfLastAttack = cardinalDirection;
		this.lastAttackType = hitInstance.AttackType;
		bool flag = hitInstance.DamageDealt <= 0 && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit;
		if (hitInstance.AttackType == AttackTypes.Heavy)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK HEAVY DAMAGE", false);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "HIT", false);
		NonBouncer component2 = base.GetComponent<NonBouncer>();
		bool flag2 = false;
		if (component2)
		{
			flag2 = component2.active;
		}
		if (!flag2)
		{
			FSMUtility.SendEventToGameObject(hitInstance.Source, "HIT LANDED", false);
			FSMUtility.SendEventToGameObject(hitInstance.Source, "DEALT ACTUAL DAMAGE", false);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK DAMAGE", false);
		if (!this.hasBlackThreadState || !this.blackThreadState.IsInForcedSing)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "SING DURATION END", false);
		}
		if (this.sendHitTo != null)
		{
			FSMUtility.SendEventToGameObject(this.sendHitTo, "HIT", false);
		}
		if (flag)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "WEAK HIT", false);
		}
		if (hitInstance.AttackType == AttackTypes.Spell)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK SPELL DAMAGE", false);
		}
		if (hitInstance.AttackType == AttackTypes.Explosion)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "TOOK EXPLOSION DAMAGE", false);
		}
		if (this.recoil != null)
		{
			this.recoil.RecoilByDirection(cardinalDirection, hitInstance.MagnitudeMultiplier);
		}
		bool flag3 = false;
		bool flag4 = false;
		AttackTypes attackType = hitInstance.AttackType;
		switch (attackType)
		{
		case AttackTypes.Nail:
			break;
		case AttackTypes.Generic:
			flag4 = true;
			goto IL_8A0;
		case AttackTypes.Spell:
			goto IL_565;
		default:
			if (attackType != AttackTypes.NailBeam)
			{
				if (attackType != AttackTypes.Heavy)
				{
					goto IL_8A0;
				}
				if (!hitInstance.Source || !DamageEnemies.IsNailAttackObject(hitInstance.Source))
				{
					goto IL_565;
				}
			}
			break;
		}
		HeroController.instance.NailHitEnemy(this, hitInstance);
		IL_565:
		if (hitInstance.AttackType == AttackTypes.Nail && this.enemyType != HealthManager.EnemyTypes.Shade && this.enemyType != HealthManager.EnemyTypes.Armoured && !this.DoNotGiveSilk)
		{
			if (!flag)
			{
				HeroController.instance.SilkGain(hitInstance);
			}
			if (hitInstance.SilkGeneration != HitSilkGeneration.None && PlayerData.instance.CurrentCrestID == "Reaper" && HeroController.instance.ReaperState.IsInReaperMode)
			{
				float angleMin = 0f;
				float angleMax = 360f;
				int i;
				switch (this.reaperBundles)
				{
				case HealthManager.ReaperBundleTiers.Normal:
					i = HeroController.instance.GetReaperPayout();
					break;
				case HealthManager.ReaperBundleTiers.Reduced:
					i = 1;
					break;
				case HealthManager.ReaperBundleTiers.None:
					i = 0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				int num = i;
				if (num > 0)
				{
					float degrees;
					if (this.flingSilkOrbsAimObject != null)
					{
						Vector3 position = this.flingSilkOrbsAimObject.transform.position;
						Vector3 position2 = base.transform.position;
						float y = position.y - position2.y;
						float x = position.x - position2.x;
						float num2 = Mathf.Atan2(y, x) * 57.295776f;
						angleMin = num2 - 45f;
						angleMax = num2 + 45f;
						degrees = num2;
					}
					else if (this.flingSilkOrbsDown)
					{
						angleMin = 225f;
						angleMax = 315f;
						degrees = 270f;
					}
					else
					{
						switch (cardinalDirection)
						{
						case 0:
							angleMin = 315f;
							angleMax = 415f;
							degrees = 0f;
							break;
						case 1:
							angleMin = 45f;
							angleMax = 135f;
							degrees = 90f;
							break;
						case 2:
							angleMin = 125f;
							angleMax = 225f;
							degrees = 180f;
							break;
						case 3:
							angleMin = 225f;
							angleMax = 315f;
							degrees = 270f;
							break;
						default:
							degrees = 0f;
							break;
						}
					}
					FlingUtils.SpawnAndFling(new FlingUtils.Config
					{
						Prefab = Gameplay.ReaperBundlePrefab,
						AmountMin = num,
						AmountMax = num,
						SpeedMin = 25f,
						SpeedMax = 50f,
						AngleMin = angleMin,
						AngleMax = angleMax
					}, base.transform, this.effectOrigin, null, -1f);
					GameObject reapHitEffectPrefab = Effects.ReapHitEffectPrefab;
					if (reapHitEffectPrefab)
					{
						Vector2 original;
						float rotation;
						switch (DirectionUtils.GetCardinalDirection(degrees))
						{
						case 1:
							original = new Vector2(1f, 1f);
							rotation = 90f;
							break;
						case 2:
							original = new Vector2(-1f, 1f);
							rotation = 0f;
							break;
						case 3:
							original = new Vector2(1f, -1f);
							rotation = 90f;
							break;
						default:
							original = new Vector2(1f, 1f);
							rotation = 0f;
							break;
						}
						GameObject gameObject2 = reapHitEffectPrefab.Spawn(base.transform.TransformPoint(this.effectOrigin));
						gameObject2.transform.SetRotation2D(rotation);
						gameObject2.transform.localScale = original.ToVector3(gameObject2.transform.localScale.z);
					}
				}
			}
		}
		if (!flag && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit)
		{
			flag3 = true;
		}
		flag4 = true;
		IL_8A0:
		Vector3 position3 = (hitInstance.Source.transform.position + base.transform.position) * 0.5f + this.effectOrigin;
		if (flag4 && hitInstance.SlashEffectOverrides != null && hitInstance.SlashEffectOverrides.Length != 0)
		{
			GameObject[] array = hitInstance.SlashEffectOverrides.SpawnAll(base.transform.position);
			float num3 = Mathf.Sign(HeroController.instance.transform.localScale.x);
			GameObject[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				GameObject gameObject3 = array2[i];
				if (gameObject3)
				{
					Vector3 initialScale = gameObject3.transform.localScale;
					Vector3 initialScale2 = initialScale;
					initialScale2.x *= num3;
					gameObject3.transform.localScale = initialScale2;
					if (!gameObject3.GetComponent<ActiveRecycler>())
					{
						GameObject closureEffect = gameObject3;
						RecycleResetHandler.Add(gameObject3, delegate()
						{
							closureEffect.transform.localScale = initialScale;
						});
					}
				}
			}
			flag3 = false;
		}
		if (flag3 && this.slashImpactPrefab)
		{
			GameObject gameObject4 = this.slashImpactPrefab.Spawn(position3, Quaternion.identity);
			float num4 = 1.7f;
			switch (cardinalDirection)
			{
			case 0:
				gameObject4.transform.SetRotation2D((float)Random.Range(340, 380));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			case 1:
				gameObject4.transform.SetRotation2D((float)Random.Range(70, 110));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			case 2:
				gameObject4.transform.SetRotation2D((float)Random.Range(340, 380));
				gameObject4.transform.localScale = new Vector3(-num4, num4, num4);
				break;
			case 3:
				gameObject4.transform.SetRotation2D(180f - hitInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
				gameObject4.transform.localScale = new Vector3(num4, num4, num4);
				break;
			}
		}
		if (this.hitEffectReceiver != null && hitInstance.AttackType != AttackTypes.RuinsWater)
		{
			this.hitEffectReceiver.ReceiveHitEffect(hitInstance);
		}
		int num5 = Mathf.RoundToInt((float)hitInstance.DamageDealt * hitInstance.Multiplier);
		if (this.damageOverride)
		{
			num5 = 1;
		}
		CheatManager.NailDamageStates nailDamage = CheatManager.NailDamage;
		if (nailDamage != CheatManager.NailDamageStates.InstaKill)
		{
			if (nailDamage == CheatManager.NailDamageStates.NoDamage)
			{
				num5 = 0;
			}
		}
		else
		{
			num5 = int.MaxValue;
		}
		this.hasTakenDamage = true;
		if (this.sendDamageTo == null)
		{
			this.hp = Mathf.Max(this.hp - num5, -1000);
		}
		else
		{
			this.sendDamageTo.hp = Mathf.Max(this.sendDamageTo.hp - num5, -1000);
		}
		if (hitInstance.NonLethal && this.hp <= 0)
		{
			this.hp = 1;
		}
		if (hitInstance.PoisonDamageTicks > 0)
		{
			DamageTag poisonPouchDamageTag = Gameplay.PoisonPouchDamageTag;
			this.tagDamageTaker.AddDamageTagToStack(poisonPouchDamageTag, hitInstance.PoisonDamageTicks);
		}
		if (hitInstance.ZapDamageTicks > 0)
		{
			DamageTag zapDamageTag = Gameplay.ZapDamageTag;
			this.tagDamageTaker.AddDamageTagToStack(zapDamageTag, hitInstance.ZapDamageTicks);
		}
		if (this.hp > 0)
		{
			if (this.TookDamage != null)
			{
				this.TookDamage();
			}
			this.NonFatalHit(hitInstance.IgnoreInvulnerable);
			this.ApplyStunDamage(hitInstance.StunDamage);
			return;
		}
		float num6 = hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Corpse);
		float magnitudeMultForType = hitInstance.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		bool disallowDropFling;
		if (hitInstance.Source != null)
		{
			Transform root = hitInstance.Source.transform.root;
			if (root.CompareTag("Player") && !hitInstance.UseCorpseDirection && num6.IsWithinTolerance(Mathf.Epsilon, 270f))
			{
				num6 = (float)((root.lossyScale.x < 0f) ? 0 : 180);
			}
			disallowDropFling = hitInstance.Source.GetComponent<BreakItemsOnContact>();
		}
		else
		{
			disallowDropFling = false;
		}
		this.DieDropFling(hitInstance.ToolDamageFlags, disallowDropFling);
		this.Die(new float?(num6), hitInstance.AttackType, hitInstance.NailElement, hitInstance.Source, hitInstance.IgnoreInvulnerable, magnitudeMultForType, false, disallowDropFling);
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x0007B750 File Offset: 0x00079950
	private bool IsImmuneTo(HitInstance hitInstance, bool wasFullHit)
	{
		AttackTypes attackType = hitInstance.AttackType;
		switch (attackType)
		{
		case AttackTypes.Nail:
			if (!this.immuneToNailAttacks)
			{
				return false;
			}
			break;
		case AttackTypes.Generic:
		case AttackTypes.Spell:
		case AttackTypes.Splatter:
		case AttackTypes.SharpShadow:
		case AttackTypes.NailBeam:
		case AttackTypes.Lightning:
			return false;
		case AttackTypes.Acid:
			if (!this.ignoreAcid)
			{
				return false;
			}
			break;
		case AttackTypes.RuinsWater:
			if (!this.ignoreWater)
			{
				if (!this.immuneToWater)
				{
					return false;
				}
				return true;
			}
			break;
		case AttackTypes.Lava:
			if (this.immuneToLava)
			{
				return true;
			}
			return false;
		case AttackTypes.Hunter:
			if (!this.immuneToHunterWeapon)
			{
				return false;
			}
			break;
		case AttackTypes.Explosion:
			if (this.immuneToExplosions)
			{
				if (wasFullHit)
				{
					FSMUtility.SendEventToGameObject(base.gameObject, "BLOCKED EXPLOSION", false);
				}
				return true;
			}
			return false;
		case AttackTypes.Coal:
			if (!this.immuneToCoal)
			{
				return false;
			}
			return true;
		case AttackTypes.Trap:
			if (!this.immuneToTraps)
			{
				return false;
			}
			return true;
		default:
			if (attackType != AttackTypes.Spikes)
			{
				return false;
			}
			if (!this.immuneToSpikes)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x0007B822 File Offset: 0x00079A22
	private void NonFatalHit(bool ignoreEvasion)
	{
		if (!ignoreEvasion)
		{
			if (this.hasAlternateHitAnimation)
			{
				if (this.animator != null)
				{
					this.animator.Play(this.alternateHitAnimation);
					return;
				}
			}
			else
			{
				this.evasionByHitRemaining = 0f;
			}
		}
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x0007B85C File Offset: 0x00079A5C
	public void ApplyStunDamage(float stunDamage)
	{
		if (!this.stunControlFsm)
		{
			return;
		}
		if (stunDamage <= Mathf.Epsilon)
		{
			return;
		}
		if (CheatManager.ForceNextHitStun || CheatManager.ForceStun)
		{
			stunDamage = float.MaxValue;
			CheatManager.ForceNextHitStun = false;
		}
		this.stunControlFsm.FsmVariables.FindFsmFloat("Stun Damage").Value = stunDamage;
		this.stunControlFsm.SendEvent("STUN DAMAGE");
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x0007B8C8 File Offset: 0x00079AC8
	public void ApplyExtraDamage(int damageAmount)
	{
		this.hp = Mathf.Max(this.hp - damageAmount, 0);
		if (this.hp <= 0)
		{
			this.Die(null, AttackTypes.Generic, true);
		}
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x0007B904 File Offset: 0x00079B04
	public void ApplyExtraDamage(HitInstance hitInstance)
	{
		hitInstance = this.ApplyDamageScaling(hitInstance);
		this.hp = Mathf.Max(this.hp - hitInstance.DamageDealt, 0);
		if (this.hp > 0)
		{
			return;
		}
		this.DieDropFling(hitInstance.ToolDamageFlags, false);
		this.Die(null, hitInstance.AttackType, hitInstance.NailElement, hitInstance.Source, hitInstance.IgnoreInvulnerable, 0f, false, false);
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0007B978 File Offset: 0x00079B78
	public bool CheckNailImbuementHit(NailImbuementConfig nailImbuement, int setHitCount)
	{
		if (this.damageTagHitsLeftTracker == null)
		{
			this.damageTagHitsLeftTracker = new Dictionary<NailImbuementConfig, int>(1);
		}
		int num;
		if (!this.damageTagHitsLeftTracker.TryGetValue(nailImbuement, out num))
		{
			this.damageTagHitsLeftTracker[nailImbuement] = setHitCount;
			return false;
		}
		num--;
		if (num <= 0)
		{
			this.damageTagHitsLeftTracker.Remove(nailImbuement);
			return true;
		}
		this.damageTagHitsLeftTracker[nailImbuement] = num;
		return false;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x0007B9DD File Offset: 0x00079BDD
	public void AddDamageTagToStack(DamageTag damageTag, int hitAmountOverride = 0)
	{
		this.tagDamageTaker.AddDamageTagToStack(damageTag, hitAmountOverride);
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0007B9EC File Offset: 0x00079BEC
	public bool ApplyTagDamage(DamageTag.DamageTagInstance damageTagInstance)
	{
		if (this.isDead)
		{
			return false;
		}
		if (damageTagInstance.isHeroDamage)
		{
			this.WillAwardJournalKill = true;
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK TAG DAMAGE", false);
		FSMUtility.SendEventToGameObject(base.gameObject, "TOOK DAMAGE", false);
		if (!this.hasBlackThreadState || !this.blackThreadState.IsInForcedSing)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "SING DURATION END", false);
		}
		this.hasTakenDamage = true;
		this.ApplyExtraDamage(new HitInstance
		{
			AttackType = AttackTypes.Generic,
			NailElement = damageTagInstance.nailElements,
			IsHeroDamage = damageTagInstance.isHeroDamage,
			DamageDealt = damageTagInstance.amount
		});
		return true;
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x0007BAA0 File Offset: 0x00079CA0
	private void DieDropFling(ToolDamageFlags toolDamageFlags, bool disallowDropFling)
	{
		if (!disallowDropFling && GameManager.instance.GetCurrentMapZoneEnum() == MapZone.MEMORY)
		{
			disallowDropFling = true;
		}
		if (this.dontDropMeat || disallowDropFling)
		{
			return;
		}
		int amount;
		CollectableItemToolDamage item = CollectableItemToolDamage.GetItem(toolDamageFlags, this.enemySize, out amount);
		if (item && item.CanGetMore())
		{
			this.queuedDropItem = new HealthManager.QueuedDropItem(item, amount);
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x0007BAF8 File Offset: 0x00079CF8
	public void Die(float? attackDirection, AttackTypes attackType, bool ignoreEvasion)
	{
		this.Die(attackDirection, attackType, ignoreEvasion, 1f);
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x0007BB08 File Offset: 0x00079D08
	public void Die(float? attackDirection, AttackTypes attackType, bool ignoreEvasion, float corpseFlingMultiplier)
	{
		this.Die(attackDirection, attackType, NailElements.None, null, ignoreEvasion, corpseFlingMultiplier, false, false);
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0007BB24 File Offset: 0x00079D24
	public void Die(float? attackDirection, AttackTypes attackType, NailElements nailElement, GameObject damageSource, bool ignoreEvasion = false, float corpseFlingMultiplier = 1f, bool overrideSpecialDeath = false, bool disallowDropFling = false)
	{
		HealthManager.<>c__DisplayClass194_0 CS$<>8__locals1 = new HealthManager.<>c__DisplayClass194_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.isDead)
		{
			return;
		}
		if (this.preventDeathAfterHero && HeroController.instance.cState.dead)
		{
			return;
		}
		this.CancelAllLagHits();
		float minCorpseFlingMagnitudeMult = GlobalSettings.Corpse.MinCorpseFlingMagnitudeMult;
		if (corpseFlingMultiplier < minCorpseFlingMagnitudeMult && Math.Abs(corpseFlingMultiplier) > Mathf.Epsilon)
		{
			corpseFlingMultiplier = minCorpseFlingMagnitudeMult;
		}
		Action<Transform> action = null;
		if (!disallowDropFling && attackType != AttackTypes.RuinsWater && GameManager.instance.GetCurrentMapZoneEnum() != MapZone.MEMORY)
		{
			CS$<>8__locals1.smallGeoExtra = 0;
			CS$<>8__locals1.mediumGeoExtra = 0;
			CS$<>8__locals1.largeGeoExtra = 0;
			CS$<>8__locals1.largeSmoothGeoExtra = 0;
			CS$<>8__locals1.shellShardExtra = 0;
			CS$<>8__locals1.shellShardBase = this.shellShardDrops;
			CS$<>8__locals1.thiefCharmEquipped = Gameplay.ThiefCharmTool.IsEquipped;
			if (CS$<>8__locals1.thiefCharmEquipped)
			{
				CS$<>8__locals1.smallGeoExtra = Mathf.CeilToInt((float)this.smallGeoDrops * Gameplay.ThiefCharmGeoSmallIncrease);
				CS$<>8__locals1.mediumGeoExtra = Mathf.CeilToInt((float)this.mediumGeoDrops * Gameplay.ThiefCharmGeoMedIncrease);
				CS$<>8__locals1.largeGeoExtra = Mathf.CeilToInt((float)this.largeGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
				CS$<>8__locals1.largeSmoothGeoExtra = Mathf.CeilToInt((float)this.largeSmoothGeoDrops * Gameplay.ThiefCharmGeoLargeIncrease);
			}
			float num = HealthManager._shellShardMultiplierArray[Random.Range(0, HealthManager._shellShardMultiplierArray.Length)];
			int num2 = Mathf.CeilToInt((float)CS$<>8__locals1.shellShardBase * num - (float)CS$<>8__locals1.shellShardBase);
			if (num2 > 8)
			{
				num2 = 8;
			}
			CS$<>8__locals1.shellShardBase += num2;
			ToolItem boneNecklaceTool = Gameplay.BoneNecklaceTool;
			if (boneNecklaceTool && boneNecklaceTool.IsEquipped)
			{
				CS$<>8__locals1.shellShardExtra = Mathf.CeilToInt((float)CS$<>8__locals1.shellShardBase * Gameplay.BoneNecklaceShellshardIncrease);
			}
			CS$<>8__locals1.dropFlingAngleMin = (this.megaFlingGeo ? 65 : 80);
			CS$<>8__locals1.dropFlingAngleMax = (this.megaFlingGeo ? 115 : 100);
			CS$<>8__locals1.dropFlingSpeedMin = (this.megaFlingGeo ? 30 : 15);
			CS$<>8__locals1.dropFlingSpeedMax = (this.megaFlingGeo ? 45 : 30);
			CS$<>8__locals1.sourceTransform = base.transform;
			CS$<>8__locals1.queuedItem = this.queuedDropItem;
			this.queuedDropItem.Reset();
			action = delegate(Transform spawnPoint)
			{
				HealthManager.<>c__DisplayClass194_1 CS$<>8__locals2 = new HealthManager.<>c__DisplayClass194_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.spawnPoint = spawnPoint;
				CS$<>8__locals1.<>4__this.SpawnCurrency(CS$<>8__locals2.spawnPoint, (float)CS$<>8__locals1.dropFlingSpeedMin, (float)CS$<>8__locals1.dropFlingSpeedMax, (float)CS$<>8__locals1.dropFlingAngleMin, (float)CS$<>8__locals1.dropFlingAngleMax, CS$<>8__locals1.<>4__this.smallGeoDrops, CS$<>8__locals1.<>4__this.mediumGeoDrops, CS$<>8__locals1.<>4__this.largeGeoDrops, CS$<>8__locals1.<>4__this.largeSmoothGeoDrops, false, CS$<>8__locals1.shellShardBase, false);
				CS$<>8__locals1.<>4__this.SpawnCurrency(CS$<>8__locals2.spawnPoint, (float)CS$<>8__locals1.dropFlingSpeedMin, (float)CS$<>8__locals1.dropFlingSpeedMax, (float)CS$<>8__locals1.dropFlingAngleMin, (float)CS$<>8__locals1.dropFlingAngleMax, 0, 0, 0, 0, false, CS$<>8__locals1.shellShardExtra, true);
				if (CS$<>8__locals1.smallGeoExtra > 0 || CS$<>8__locals1.mediumGeoExtra > 0 || CS$<>8__locals1.largeGeoExtra > 0 || CS$<>8__locals1.largeSmoothGeoExtra > 0)
				{
					if (CS$<>8__locals2.spawnPoint == CS$<>8__locals1.sourceTransform)
					{
						CS$<>8__locals2.<Die>g__SpawnExtraGeo|1();
					}
					else
					{
						CS$<>8__locals2.spawnPoint.GetComponent<MonoBehaviour>().ExecuteDelayed(0.2f, new Action(CS$<>8__locals2.<Die>g__SpawnExtraGeo|1));
					}
				}
				CS$<>8__locals1.<>4__this.SpawnQueuedItemDrop(CS$<>8__locals1.queuedItem, CS$<>8__locals2.spawnPoint);
				foreach (HealthManager.ItemDropGroup itemDropGroup in CS$<>8__locals1.<>4__this.itemDropGroups)
				{
					if (itemDropGroup.Drops.Count > 0 && itemDropGroup.TotalProbability >= 1f)
					{
						HealthManager.ItemDropProbability itemDropProbability = (HealthManager.ItemDropProbability)Probability.GetRandomItemRootByProbability<HealthManager.ItemDropProbability, SavedItem>(itemDropGroup.Drops.ToArray(), null);
						if (itemDropProbability != null)
						{
							CS$<>8__locals1.<>4__this.SpawnItemDrop(itemDropProbability.Item, itemDropProbability.Amount.GetRandomValue(true), itemDropProbability.CustomPickupPrefab, CS$<>8__locals2.spawnPoint, itemDropProbability.LimitActiveInScene);
						}
					}
				}
			};
		}
		if (!overrideSpecialDeath)
		{
			GameObject go = this.zeroHPEventOverride ? this.zeroHPEventOverride : base.gameObject;
			FSMUtility.SendEventToGameObject(go, "ZERO HP", false);
			if (this.blackThreadState)
			{
				this.blackThreadState.CancelAttack();
			}
			if (attackType == AttackTypes.Lava)
			{
				FSMUtility.SendEventToGameObject(go, "LAVA DEATH", false);
			}
			if (this.hasSpecialDeath)
			{
				this.NonFatalHit(ignoreEvasion);
				if (action != null)
				{
					action(base.transform);
				}
				return;
			}
		}
		this.isDead = true;
		if (this.damageHero != null)
		{
			this.damageHero.damageDealt = 0;
		}
		if (this.battleScene != null && !this.notifiedBattleScene)
		{
			BattleScene component = this.battleScene.GetComponent<BattleScene>();
			if (component != null)
			{
				if (!this.bigEnemyDeath)
				{
					component.DecrementEnemy();
				}
				else
				{
					component.DecrementBigEnemy();
				}
			}
		}
		if (this.deathAudioSnapshot != null)
		{
			this.deathAudioSnapshot.TransitionTo(6f);
		}
		if (this.sendKilledTo != null)
		{
			FSMUtility.SendEventToGameObject(this.sendKilledTo, "KILLED", false);
		}
		this.SendDeathEvent();
		if (attackType == AttackTypes.Splatter)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("AverageShake");
			this.corpseSplatPrefab.Spawn(base.transform.position + this.effectOrigin, Quaternion.identity);
			if (this.enemyDeathEffects)
			{
				this.enemyDeathEffects.EmitSound();
			}
			base.gameObject.SetActive(false);
			return;
		}
		bool flag;
		GameObject gameObject;
		if (this.enemyDeathEffects != null)
		{
			if (attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid || attackType == AttackTypes.Generic)
			{
				this.enemyDeathEffects.SkipKillFreeze = true;
			}
			this.enemyDeathEffects.ReceiveDeathEvent(attackDirection, attackType, nailElement, damageSource, corpseFlingMultiplier, this.deathReset, action, out flag, out gameObject);
		}
		else
		{
			flag = false;
			gameObject = null;
		}
		if (!flag && action != null)
		{
			action(gameObject ? gameObject.transform : base.transform);
		}
		if (gameObject != null && this.corpseEventResponder != null)
		{
			gameObject.GetComponent<EventRelay>().TemporaryEvent += this.corpseEventResponder.ReceiveEvent;
		}
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x0007BF68 File Offset: 0x0007A168
	private void SpawnCurrency(Transform spawnPoint, float dropFlingSpeedMin, float dropFlingSpeedMax, float dropFlingAngleMin, float dropFlingAngleMax, int smallGeoCount, int mediumGeoCount, int largeGeoCount, int largeSmoothGeoCount, bool shouldGeoFlash, int shellShardCount, bool shouldShardsFlash)
	{
		if (smallGeoCount > 0)
		{
			this.spawnedFlingTracker.Clear();
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = Gameplay.SmallGeoPrefab,
				AmountMin = smallGeoCount,
				AmountMax = smallGeoCount,
				SpeedMin = dropFlingSpeedMin,
				SpeedMax = dropFlingSpeedMax,
				AngleMin = dropFlingAngleMin,
				AngleMax = dropFlingAngleMax
			}, spawnPoint, this.effectOrigin, this.spawnedFlingTracker, -1f);
			if (shouldGeoFlash)
			{
				this.SetCurrencyFlashing(this.spawnedFlingTracker, false);
			}
		}
		if (mediumGeoCount > 0)
		{
			this.spawnedFlingTracker.Clear();
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = Gameplay.MediumGeoPrefab,
				AmountMin = mediumGeoCount,
				AmountMax = mediumGeoCount,
				SpeedMin = dropFlingSpeedMin,
				SpeedMax = dropFlingSpeedMax,
				AngleMin = dropFlingAngleMin,
				AngleMax = dropFlingAngleMax
			}, spawnPoint, this.effectOrigin, this.spawnedFlingTracker, -1f);
			if (shouldGeoFlash)
			{
				this.SetCurrencyFlashing(this.spawnedFlingTracker, false);
			}
		}
		if (largeGeoCount > 0)
		{
			this.spawnedFlingTracker.Clear();
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = Gameplay.LargeGeoPrefab,
				AmountMin = largeGeoCount,
				AmountMax = largeGeoCount,
				SpeedMin = dropFlingSpeedMin,
				SpeedMax = dropFlingSpeedMax,
				AngleMin = dropFlingAngleMin,
				AngleMax = dropFlingAngleMax
			}, spawnPoint, this.effectOrigin, this.spawnedFlingTracker, -1f);
			if (shouldGeoFlash)
			{
				this.SetCurrencyFlashing(this.spawnedFlingTracker, false);
			}
		}
		if (largeSmoothGeoCount > 0)
		{
			this.spawnedFlingTracker.Clear();
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = Gameplay.LargeSmoothGeoPrefab,
				AmountMin = largeSmoothGeoCount,
				AmountMax = largeSmoothGeoCount,
				SpeedMin = dropFlingSpeedMin,
				SpeedMax = dropFlingSpeedMax,
				AngleMin = dropFlingAngleMin,
				AngleMax = dropFlingAngleMax
			}, spawnPoint, this.effectOrigin, this.spawnedFlingTracker, -1f);
			if (shouldGeoFlash)
			{
				this.SetCurrencyFlashing(this.spawnedFlingTracker, false);
			}
		}
		if (shellShardCount > 0)
		{
			this.spawnedFlingTracker.Clear();
			FlingUtils.SpawnAndFlingShellShards(new FlingUtils.Config
			{
				Prefab = Gameplay.ShellShardPrefab,
				AmountMin = shellShardCount,
				AmountMax = shellShardCount,
				SpeedMin = dropFlingSpeedMin,
				SpeedMax = dropFlingSpeedMax,
				AngleMin = dropFlingAngleMin,
				AngleMax = dropFlingAngleMax
			}, spawnPoint, this.effectOrigin, this.spawnedFlingTracker);
			if (shouldShardsFlash)
			{
				this.SetCurrencyFlashing(this.spawnedFlingTracker, true);
			}
		}
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x0007C1F8 File Offset: 0x0007A3F8
	private void SpawnQueuedItemDrop(HealthManager.QueuedDropItem queuedDropItem, Transform spawnPoint)
	{
		if (!queuedDropItem.isQueued)
		{
			return;
		}
		CollectableItemToolDamage dropItem = queuedDropItem.dropItem;
		if (dropItem == null)
		{
			return;
		}
		this.SpawnItemDrop(dropItem, queuedDropItem.amount, Gameplay.CollectableItemPickupMeatPrefab, spawnPoint, 0);
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x0007C234 File Offset: 0x0007A434
	private void SpawnItemDrop(SavedItem dropItem, int count, CollectableItemPickup prefab, Transform spawnPoint, int limit)
	{
		if (!dropItem || !dropItem.CanGetMore())
		{
			return;
		}
		if (!prefab)
		{
			prefab = Gameplay.CollectableItemPickupInstantPrefab;
		}
		if (!prefab)
		{
			return;
		}
		Vector3 vector = spawnPoint.TransformPoint(this.effectOrigin);
		for (int i = 0; i < count; i++)
		{
			Vector3 position = vector;
			PrefabCollectable prefabCollectable = dropItem as PrefabCollectable;
			GameObject gameObject;
			bool flag;
			if (prefabCollectable != null)
			{
				gameObject = prefabCollectable.Spawn();
				flag = false;
			}
			else
			{
				CollectableItemPickup collectableItemPickup;
				if (limit > 0)
				{
					collectableItemPickup = ObjectPool.Spawn(prefab.gameObject, null, position, Quaternion.identity, true).GetComponent<CollectableItemPickup>();
				}
				else
				{
					collectableItemPickup = Object.Instantiate<CollectableItemPickup>(prefab);
					collectableItemPickup.transform.position = position;
				}
				flag = true;
				collectableItemPickup.SetItem(dropItem, false);
				gameObject = collectableItemPickup.gameObject;
			}
			FlingUtils.FlingObject(new FlingUtils.SelfConfig
			{
				Object = gameObject,
				SpeedMin = 15f,
				SpeedMax = 30f,
				AngleMin = 80f,
				AngleMax = 100f
			}, spawnPoint, this.effectOrigin);
			if (flag)
			{
				gameObject.transform.SetPositionZ(Random.Range(0.003f, 0.0039f));
			}
		}
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x0007C363 File Offset: 0x0007A563
	public void SendDeathEvent()
	{
		if (this.OnDeath != null)
		{
			this.OnDeath();
		}
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x0007C378 File Offset: 0x0007A578
	public void SetDead()
	{
		this.isDead = true;
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x0007C384 File Offset: 0x0007A584
	private void SetCurrencyFlashing(IReadOnlyList<GameObject> gameObjects, bool isShellShard)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			SpriteFlash component = gameObject.GetComponent<SpriteFlash>();
			if (component)
			{
				if (isShellShard)
				{
					component.GeoFlash();
				}
				else
				{
					component.FlashExtraRosary();
					GeoControl component2 = gameObject.GetComponent<GeoControl>();
					if (component2)
					{
						component2.SpawnThiefCharmEffect();
					}
				}
			}
		}
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0007C3FC File Offset: 0x0007A5FC
	public bool IsBlockingByDirection(int cardinalDirection, AttackTypes attackType, SpecialTypes specialType = SpecialTypes.None)
	{
		if (!this.invincible)
		{
			return false;
		}
		if (attackType == AttackTypes.Lava || attackType == AttackTypes.Coal)
		{
			return false;
		}
		if ((attackType == AttackTypes.Spell || attackType == AttackTypes.SharpShadow || attackType == AttackTypes.Explosion) && base.gameObject.CompareTag("Spell Vulnerable"))
		{
			return false;
		}
		if ((this.piercable || this.invincibleFromDirection != 0) && (attackType == AttackTypes.Explosion || attackType == AttackTypes.Lightning || (specialType & SpecialTypes.Piercer) != SpecialTypes.None))
		{
			return false;
		}
		if (this.invincibleFromDirection == 0)
		{
			return true;
		}
		switch (cardinalDirection)
		{
		case 0:
		{
			int num = this.invincibleFromDirection;
			if (num != 1)
			{
				switch (num)
				{
				case 5:
				case 8:
				case 10:
				case 12:
				case 13:
					return true;
				}
				return false;
			}
			return true;
		}
		case 1:
			switch (this.invincibleFromDirection)
			{
			case 2:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 13:
				return true;
			}
			return false;
		case 2:
			switch (this.invincibleFromDirection)
			{
			case 3:
			case 6:
			case 9:
			case 11:
			case 12:
			case 13:
				return true;
			}
			return false;
		case 3:
		{
			int num = this.invincibleFromDirection;
			return num == 4 || num - 7 <= 5;
		}
		default:
			return false;
		}
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x0007C55E File Offset: 0x0007A75E
	public void SetBattleScene(GameObject newBattleScene)
	{
		this.battleScene = newBattleScene;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x0007C567 File Offset: 0x0007A767
	public int GetAttackDirection()
	{
		return this.directionOfLastAttack;
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0007C56F File Offset: 0x0007A76F
	public AttackTypes GetLastAttackType()
	{
		return this.lastAttackType;
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x0007C577 File Offset: 0x0007A777
	public void SetPreventInvincibleEffect(bool set)
	{
		this.preventInvincibleEffect = set;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0007C580 File Offset: 0x0007A780
	public void SetGeoSmall(int amount)
	{
		this.smallGeoDrops = amount;
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x0007C589 File Offset: 0x0007A789
	public void SetGeoMedium(int amount)
	{
		this.mediumGeoDrops = amount;
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x0007C592 File Offset: 0x0007A792
	public void SetGeoLarge(int amount)
	{
		this.largeGeoDrops = amount;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x0007C59B File Offset: 0x0007A79B
	public void SetShellShards(int amount)
	{
		this.shellShardDrops = amount;
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x0007C5A4 File Offset: 0x0007A7A4
	public void ClearItemDropsBattleScene()
	{
		if (this.HasClearedItemDrops)
		{
			return;
		}
		for (int i = this.itemDropGroups.Count - 1; i >= 0; i--)
		{
			HealthManager.ItemDropGroup itemDropGroup = this.itemDropGroups[i];
			if (itemDropGroup.Drops.Count != 0)
			{
				for (int j = itemDropGroup.Drops.Count - 1; j >= 0; j--)
				{
					HealthManager.ItemDropProbability itemDropProbability = itemDropGroup.Drops[j];
					if (itemDropProbability.Item && !(itemDropProbability.Item is FullQuestBase))
					{
						itemDropGroup.Drops.RemoveAt(j);
					}
				}
				if (itemDropGroup.Drops.Count == 0)
				{
					this.itemDropGroups.RemoveAt(i);
				}
			}
		}
		this.HasClearedItemDrops = true;
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x0007C65D File Offset: 0x0007A85D
	public IEnumerable<SavedItem> EnumerateItemDrops()
	{
		foreach (HealthManager.ItemDropGroup itemDropGroup in this.itemDropGroups)
		{
			foreach (HealthManager.ItemDropProbability itemDropProbability in itemDropGroup.Drops)
			{
				if (itemDropProbability.Item)
				{
					yield return itemDropProbability.Item;
				}
			}
			List<HealthManager.ItemDropProbability>.Enumerator enumerator2 = default(List<HealthManager.ItemDropProbability>.Enumerator);
		}
		List<HealthManager.ItemDropGroup>.Enumerator enumerator = default(List<HealthManager.ItemDropGroup>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x0007C66D File Offset: 0x0007A86D
	public bool GetIsDead()
	{
		return this.isDead;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x0007C675 File Offset: 0x0007A875
	public void SetIsDead(bool set)
	{
		this.isDead = set;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x0007C67E File Offset: 0x0007A87E
	public void SetDamageOverride(bool set)
	{
		this.damageOverride = set;
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0007C687 File Offset: 0x0007A887
	public void SetSendKilledToObject(GameObject killedObject)
	{
		if (killedObject != null)
		{
			this.sendKilledToObject = killedObject;
		}
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x0007C699 File Offset: 0x0007A899
	public bool CheckInvincible()
	{
		return this.invincible;
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x0007C6A1 File Offset: 0x0007A8A1
	public void HealToMax()
	{
		this.hp = this.initHp;
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x0007C6AF File Offset: 0x0007A8AF
	public bool HasTakenDamage()
	{
		return this.hasTakenDamage;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x0007C6B7 File Offset: 0x0007A8B7
	public void AddHP(int hpAdd, int hpMax)
	{
		this.hp += hpAdd;
		this.isDead = false;
		if (this.hp > hpMax)
		{
			this.hp = hpMax;
		}
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x0007C6DE File Offset: 0x0007A8DE
	public void RefillHP()
	{
		this.isDead = false;
		this.hp = this.initHp;
		this.hasTakenDamage = false;
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x0007C6FA File Offset: 0x0007A8FA
	public void SetImmuneToNailAttacks(bool immune)
	{
		this.immuneToNailAttacks = immune;
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x0007C703 File Offset: 0x0007A903
	public void SetImmuneToTraps(bool immune)
	{
		this.immuneToTraps = immune;
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x0007C70C File Offset: 0x0007A90C
	public void SetImmuneToSpikes(bool immune)
	{
		this.immuneToSpikes = immune;
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x0007C715 File Offset: 0x0007A915
	public bool ShouldIgnore(HealthManager.IgnoreFlags ignoreFlags)
	{
		return ignoreFlags != HealthManager.IgnoreFlags.None && (this.ignoreFlags & ignoreFlags) == ignoreFlags;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x0007C728 File Offset: 0x0007A928
	public void DoLagHits(LagHitOptions options, HitInstance hitInstance)
	{
		if (this.hp <= 0)
		{
			return;
		}
		if (this.IsImmuneTo(hitInstance, false))
		{
			return;
		}
		if (!options.ShouldDoLagHits())
		{
			return;
		}
		bool doStartDelay = true;
		HealthManager.LagHitsTracker lagHitsTracker;
		if (this.runningSingleLagHits.TryGetValue(options.DamageType, out lagHitsTracker))
		{
			base.StopCoroutine(lagHitsTracker.LagHitsRoutine);
			lagHitsTracker.OnLagHitsEnd();
			this.runningSingleLagHits.Remove(options.DamageType);
			doStartDelay = false;
		}
		int damageDealt = options.UseNailDamage ? Mathf.RoundToInt((float)PlayerData.instance.nailDamage * options.NailDamageMultiplier) : options.HitDamage;
		if (hitInstance.AttackType == AttackTypes.Nail)
		{
			hitInstance.AttackType = AttackTypes.Generic;
		}
		hitInstance.DamageDealt = damageDealt;
		GameObject[] slashEffectOverrides = options.SlashEffectOverrides;
		hitInstance.SlashEffectOverrides = ((slashEffectOverrides != null && slashEffectOverrides.Length != 0) ? options.SlashEffectOverrides : null);
		hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.LagHit;
		hitInstance.MagnitudeMultiplier = options.MagnitudeMult;
		hitInstance.CriticalHit = false;
		hitInstance.StunDamage = 0f;
		ParticleEffectsLerpEmission spawnedHitMarkedEffect;
		options.OnStart(base.transform, this.effectOrigin, hitInstance, out spawnedHitMarkedEffect);
		HealthManager.LagHitsTracker tracker = new HealthManager.LagHitsTracker
		{
			Source = hitInstance.Source
		};
		tracker.OnLagHitsEnd = delegate()
		{
			if (spawnedHitMarkedEffect)
			{
				spawnedHitMarkedEffect.Stop();
				spawnedHitMarkedEffect = null;
			}
			this.runningLagHits.Remove(tracker);
		};
		LagHitDamageType damageType = options.DamageType;
		if (damageType == LagHitDamageType.WitchPoison || damageType == LagHitDamageType.Flintstone || damageType == LagHitDamageType.Dazzle)
		{
			this.runningSingleLagHits[options.DamageType] = tracker;
			HealthManager.LagHitsTracker tracker2 = tracker;
			tracker2.OnLagHitsEnd = (Action)Delegate.Combine(tracker2.OnLagHitsEnd, new Action(delegate()
			{
				this.runningSingleLagHits.Remove(options.DamageType);
			}));
		}
		this.runningLagHits.Add(tracker);
		tracker.LagHitsRoutine = base.StartCoroutine(this.LagHits(options, hitInstance, tracker.OnLagHitsEnd, doStartDelay));
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x0007C948 File Offset: 0x0007AB48
	public static void CancelAllLagHitsForSource(GameObject source)
	{
		foreach (HealthManager healthManager in HealthManager._activeHealthManagers)
		{
			healthManager.CancelLagHitsForSource(source);
		}
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x0007C998 File Offset: 0x0007AB98
	public void CancelLagHitsForSource(GameObject source)
	{
		for (int i = this.runningLagHits.Count - 1; i >= 0; i--)
		{
			HealthManager.LagHitsTracker lagHitsTracker = this.runningLagHits[i];
			if (!(lagHitsTracker.Source != source))
			{
				base.StopCoroutine(lagHitsTracker.LagHitsRoutine);
				lagHitsTracker.OnLagHitsEnd();
			}
		}
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x0007C9F0 File Offset: 0x0007ABF0
	public void CancelAllLagHits()
	{
		for (int i = this.runningLagHits.Count - 1; i >= 0; i--)
		{
			HealthManager.LagHitsTracker lagHitsTracker = this.runningLagHits[i];
			base.StopCoroutine(lagHitsTracker.LagHitsRoutine);
			lagHitsTracker.OnLagHitsEnd();
		}
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x0007CA39 File Offset: 0x0007AC39
	private IEnumerator LagHits(LagHitOptions options, HitInstance hitInstance, Action onLagHitsEnd, bool doStartDelay)
	{
		if (doStartDelay)
		{
			yield return new WaitForSeconds(options.StartDelay);
		}
		int hitsDone = 0;
		float elapsed = 0f;
		while (hitsDone < options.HitCount)
		{
			if (!options.IgnoreBlock)
			{
				int cardinalDirection = DirectionUtils.GetCardinalDirection(hitInstance.GetActualDirection(base.transform, HitInstance.TargetType.Regular));
				if (this.IsBlockingByDirection(cardinalDirection, hitInstance.AttackType, hitInstance.SpecialType))
				{
					break;
				}
			}
			if (elapsed >= options.HitDelay)
			{
				elapsed %= options.HitDelay;
				bool flag;
				if (options.IsExtraDamage)
				{
					this.ApplyExtraDamage(hitInstance);
					flag = true;
				}
				else
				{
					bool flag2 = this.DoNotGiveSilk;
					if (!options.HitsGiveSilk)
					{
						this.doNotGiveSilk = true;
					}
					flag = (this.Hit(hitInstance) == IHitResponder.Response.DamageEnemy);
					this.doNotGiveSilk = flag2;
				}
				if (flag)
				{
					options.OnHit(base.transform, this.effectOrigin, hitInstance);
				}
				int num = hitsDone;
				hitsDone = num + 1;
			}
			yield return null;
			elapsed += Time.deltaTime;
		}
		if (onLagHitsEnd != null)
		{
			onLagHitsEnd();
		}
		else
		{
			options.OnEnd(base.transform, this.effectOrigin, hitInstance);
		}
		yield break;
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x0007CA68 File Offset: 0x0007AC68
	private void AddPhysicalPusher()
	{
		if (!this.boxCollider)
		{
			return;
		}
		if (this.addedPhysicalPusher)
		{
			return;
		}
		this.addedPhysicalPusher = true;
		GameObject enemyPhysicalPusher = Effects.EnemyPhysicalPusher;
		this.physicalPusher = Object.Instantiate<GameObject>(enemyPhysicalPusher);
		this.physicalPusher.name = string.Format("{0} ({1})", enemyPhysicalPusher.gameObject.name, base.gameObject.name);
		this.physicalPusher.layer = 27;
		this.physicalPusher.transform.SetParentReset(base.transform);
		CapsuleCollider2D component = this.physicalPusher.GetComponent<CapsuleCollider2D>();
		component.offset = this.boxCollider.offset;
		Vector2 size = this.boxCollider.size;
		size.x = Mathf.Max(size.x - 0.5f, 0.5f);
		component.size = size;
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x0007CB3E File Offset: 0x0007AD3E
	public GameObject GetPhysicalPusher()
	{
		if (!this.addedPhysicalPusher)
		{
			this.AddPhysicalPusher();
		}
		return this.physicalPusher;
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x0007CB54 File Offset: 0x0007AD54
	public void DoStealHit()
	{
		int stolenGeo;
		int flingGeo;
		int stolenShards;
		int flingShards;
		if (!this.TrySteal(out stolenGeo, out flingGeo, out stolenShards, out flingShards))
		{
			return;
		}
		this.DoLagHits(new HealthManager.StealLagHit(this, stolenGeo, flingGeo, stolenShards, flingShards), this.lastHitInstance);
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x0007CB88 File Offset: 0x0007AD88
	private bool TrySteal(out int stolenGeo, out int flingGeo, out int stolenShards, out int flingShards)
	{
		stolenGeo = 0;
		flingGeo = 0;
		stolenShards = 0;
		flingShards = 0;
		if (!Gameplay.ThiefPickTool.IsEquipped)
		{
			return false;
		}
		int value = Gameplay.SmallGeoValue.Value;
		int value2 = Gameplay.MediumGeoValue.Value;
		int value3 = Gameplay.LargeGeoValue.Value;
		int num = this.smallGeoDrops * value + this.mediumGeoDrops * value2 + (this.largeGeoDrops + this.largeSmoothGeoDrops) * value3;
		int randomValue = Gameplay.ThiefPickGeoStealMin.GetRandomValue(true);
		float randomValue2 = Gameplay.ThiefPickGeoSteal.GetRandomValue();
		stolenGeo = Mathf.CeilToInt(randomValue2 * (float)num);
		if (stolenGeo < randomValue)
		{
			stolenGeo = randomValue;
		}
		bool flag = Random.Range(0f, 1f) < Gameplay.ThiefPickLooseChance;
		flingGeo = (flag ? Gameplay.ThiefPickGeoLoose.GetRandomValue(true) : 0);
		int i = stolenGeo + flingGeo;
		while (i > 0)
		{
			if (this.smallGeoDrops > 0)
			{
				this.smallGeoDrops--;
				i--;
			}
			else if (this.mediumGeoDrops > 0)
			{
				this.mediumGeoDrops--;
				this.smallGeoDrops += value2;
			}
			else if (this.largeGeoDrops > 0 || this.largeSmoothGeoDrops > 0)
			{
				if (this.largeSmoothGeoDrops > 0)
				{
					this.largeSmoothGeoDrops--;
				}
				else
				{
					this.largeGeoDrops--;
				}
				this.mediumGeoDrops += value3 / value2;
				this.smallGeoDrops += value3 % value2 / value;
			}
			else
			{
				if (flingGeo > 0)
				{
					flingGeo--;
				}
				else
				{
					if (stolenGeo <= 0)
					{
						break;
					}
					stolenGeo--;
				}
				i--;
			}
		}
		if (this.shellShardDrops > 0)
		{
			stolenShards = Gameplay.ThiefPickShardSteal.GetRandomValue(true);
			if (flag)
			{
				flingShards = Gameplay.ThiefPickShardLoose.GetRandomValue(true);
			}
		}
		return true;
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x0007CD64 File Offset: 0x0007AF64
	private void FlingStealCurrency(int amount, GameObject currencyPrefab)
	{
		ref Vector3 position = HeroController.instance.transform.position;
		Vector3 position2 = base.transform.position;
		float angleMin;
		float angleMax;
		if (position.x > position2.x)
		{
			angleMin = 30f;
			angleMax = 65f;
		}
		else
		{
			angleMin = 150f;
			angleMax = 115f;
		}
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = currencyPrefab,
			AmountMin = amount,
			AmountMax = amount,
			SpeedMin = 5f,
			SpeedMax = 20f,
			AngleMin = angleMin,
			AngleMax = angleMax
		}, base.transform, this.effectOrigin, null, -1f);
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x0007CE13 File Offset: 0x0007B013
	public void SetFlingSilkOrbsDown(bool set)
	{
		this.flingSilkOrbsDown = set;
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x0007CE1C File Offset: 0x0007B01C
	public void SetEffectOrigin(Vector3 set)
	{
		this.effectOrigin = set;
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0007CE77 File Offset: 0x0007B077
	Transform ITagDamageTakerOwner.get_transform()
	{
		return base.transform;
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x0007CE7F File Offset: 0x0007B07F
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x0007CE9D File Offset: 0x0007B09D
	[CompilerGenerated]
	private void <OnAwake>g__OnSaveState|171_1(bool val)
	{
		if (this.ignorePersistence)
		{
			return;
		}
		if (!val)
		{
			return;
		}
		this.isDead = true;
		Action startedDead = this.StartedDead;
		if (startedDead != null)
		{
			startedDead();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001970 RID: 6512
	private BoxCollider2D boxCollider;

	// Token: 0x04001971 RID: 6513
	private Recoil recoil;

	// Token: 0x04001972 RID: 6514
	private IHitEffectReciever hitEffectReceiver;

	// Token: 0x04001973 RID: 6515
	private EnemyDeathEffects enemyDeathEffects;

	// Token: 0x04001974 RID: 6516
	private tk2dSpriteAnimator animator;

	// Token: 0x04001975 RID: 6517
	private DamageHero damageHero;

	// Token: 0x04001976 RID: 6518
	private TagDamageTaker tagDamageTaker;

	// Token: 0x04001977 RID: 6519
	private Dictionary<NailImbuementConfig, int> damageTagHitsLeftTracker;

	// Token: 0x04001978 RID: 6520
	[Header("Assets")]
	[SerializeField]
	private AudioSource audioPlayerPrefab;

	// Token: 0x04001979 RID: 6521
	[SerializeField]
	private AudioEvent regularInvincibleAudio;

	// Token: 0x0400197A RID: 6522
	[SerializeField]
	private GameObject blockHitPrefab;

	// Token: 0x0400197B RID: 6523
	[SerializeField]
	private GameObject strikeNailPrefab;

	// Token: 0x0400197C RID: 6524
	[SerializeField]
	private GameObject slashImpactPrefab;

	// Token: 0x0400197D RID: 6525
	[SerializeField]
	private GameObject corpseSplatPrefab;

	// Token: 0x0400197E RID: 6526
	[Header("Body")]
	[SerializeField]
	public int hp;

	// Token: 0x0400197F RID: 6527
	[SerializeField]
	private HealthManager.DamageScalingConfig damageScaling;

	// Token: 0x04001980 RID: 6528
	[SerializeField]
	private HealthManager.EnemyTypes enemyType;

	// Token: 0x04001981 RID: 6529
	[SerializeField]
	private bool doNotGiveSilk;

	// Token: 0x04001982 RID: 6530
	[SerializeField]
	private HealthManager.IgnoreFlags ignoreFlags;

	// Token: 0x04001983 RID: 6531
	[SerializeField]
	private HealthManager.ReaperBundleTiers reaperBundles;

	// Token: 0x04001984 RID: 6532
	[SerializeField]
	private Vector3 effectOrigin;

	// Token: 0x04001985 RID: 6533
	[SerializeField]
	private bool ignoreKillAll;

	// Token: 0x04001986 RID: 6534
	[SerializeField]
	private HealthManager sendDamageTo;

	// Token: 0x04001987 RID: 6535
	[SerializeField]
	private bool isPartOfSendToTarget;

	// Token: 0x04001988 RID: 6536
	[SerializeField]
	private bool tagDamageTakerIgnoreColliderState;

	// Token: 0x04001989 RID: 6537
	[SerializeField]
	private bool takeTagDamageWhileInvincible;

	// Token: 0x0400198A RID: 6538
	[SerializeField]
	private Transform targetPointOverride;

	// Token: 0x0400198B RID: 6539
	private bool hasTargetPointOverride;

	// Token: 0x0400198C RID: 6540
	[Header("Scene")]
	[SerializeField]
	private GameObject battleScene;

	// Token: 0x0400198D RID: 6541
	[SerializeField]
	private GameObject sendHitTo;

	// Token: 0x0400198E RID: 6542
	[SerializeField]
	private GameObject sendKilledToObject;

	// Token: 0x0400198F RID: 6543
	[SerializeField]
	private string sendKilledToName;

	// Token: 0x04001990 RID: 6544
	[Header("Drops")]
	[SerializeField]
	private int smallGeoDrops;

	// Token: 0x04001991 RID: 6545
	[SerializeField]
	private int mediumGeoDrops;

	// Token: 0x04001992 RID: 6546
	[SerializeField]
	private int largeGeoDrops;

	// Token: 0x04001993 RID: 6547
	[SerializeField]
	private int largeSmoothGeoDrops;

	// Token: 0x04001994 RID: 6548
	[SerializeField]
	private bool megaFlingGeo;

	// Token: 0x04001995 RID: 6549
	[Space]
	[SerializeField]
	private int shellShardDrops;

	// Token: 0x04001996 RID: 6550
	[Space]
	[SerializeField]
	private bool flingSilkOrbsDown;

	// Token: 0x04001997 RID: 6551
	[SerializeField]
	private GameObject flingSilkOrbsAimObject;

	// Token: 0x04001998 RID: 6552
	[Space]
	[SerializeField]
	private List<HealthManager.ItemDropGroup> itemDropGroups;

	// Token: 0x04001999 RID: 6553
	[SerializeField]
	[Range(0f, 1f)]
	[FormerlySerializedAs("itemDropProbability")]
	[HideInInspector]
	private float _itemDropProbability;

	// Token: 0x0400199A RID: 6554
	[SerializeField]
	[FormerlySerializedAs("itemDrops")]
	[HideInInspector]
	private HealthManager.ItemDropProbability[] _itemDrops;

	// Token: 0x0400199B RID: 6555
	[Header("Hit")]
	[SerializeField]
	private bool hasAlternateHitAnimation;

	// Token: 0x0400199C RID: 6556
	[SerializeField]
	private string alternateHitAnimation;

	// Token: 0x0400199D RID: 6557
	[Header("Invincible")]
	[SerializeField]
	private bool invincible;

	// Token: 0x0400199E RID: 6558
	[SerializeField]
	private bool piercable;

	// Token: 0x0400199F RID: 6559
	[SerializeField]
	private int invincibleFromDirection;

	// Token: 0x040019A0 RID: 6560
	[SerializeField]
	private bool preventInvincibleEffect;

	// Token: 0x040019A1 RID: 6561
	[SerializeField]
	private bool preventInvincibleShake;

	// Token: 0x040019A2 RID: 6562
	[SerializeField]
	private bool preventInvincibleAttackBlock;

	// Token: 0x040019A3 RID: 6563
	[SerializeField]
	private bool invincibleRecoil;

	// Token: 0x040019A4 RID: 6564
	[SerializeField]
	private bool dontSendTinkToDamager;

	// Token: 0x040019A5 RID: 6565
	[SerializeField]
	private bool hasAlternateInvincibleSound;

	// Token: 0x040019A6 RID: 6566
	[SerializeField]
	private AudioClip alternateInvincibleSound;

	// Token: 0x040019A7 RID: 6567
	[SerializeField]
	private bool immuneToNailAttacks;

	// Token: 0x040019A8 RID: 6568
	[SerializeField]
	private bool immuneToExplosions;

	// Token: 0x040019A9 RID: 6569
	[SerializeField]
	private bool immuneToBeams;

	// Token: 0x040019AA RID: 6570
	[SerializeField]
	private bool immuneToHunterWeapon;

	// Token: 0x040019AB RID: 6571
	[SerializeField]
	private bool immuneToCoal;

	// Token: 0x040019AC RID: 6572
	[SerializeField]
	private bool immuneToTraps;

	// Token: 0x040019AD RID: 6573
	[SerializeField]
	private bool immuneToWater;

	// Token: 0x040019AE RID: 6574
	[SerializeField]
	private bool immuneToSpikes;

	// Token: 0x040019AF RID: 6575
	[SerializeField]
	private bool immuneToLava;

	// Token: 0x040019B0 RID: 6576
	[SerializeField]
	private bool isMossExtractable;

	// Token: 0x040019B1 RID: 6577
	[SerializeField]
	private bool isSwampExtractable;

	// Token: 0x040019B2 RID: 6578
	[SerializeField]
	private bool isBluebloodExtractable;

	// Token: 0x040019B3 RID: 6579
	[Header("Death")]
	[SerializeField]
	private AudioMixerSnapshot deathAudioSnapshot;

	// Token: 0x040019B4 RID: 6580
	[SerializeField]
	public bool hasSpecialDeath;

	// Token: 0x040019B5 RID: 6581
	[SerializeField]
	public bool deathReset;

	// Token: 0x040019B6 RID: 6582
	[SerializeField]
	public bool damageOverride;

	// Token: 0x040019B7 RID: 6583
	[SerializeField]
	private bool ignoreAcid;

	// Token: 0x040019B8 RID: 6584
	[SerializeField]
	private bool ignoreWater;

	// Token: 0x040019B9 RID: 6585
	[SerializeField]
	private GameObject zeroHPEventOverride;

	// Token: 0x040019BA RID: 6586
	[SerializeField]
	private bool dontDropMeat;

	// Token: 0x040019BB RID: 6587
	[SerializeField]
	private HealthManager.EnemySize enemySize = HealthManager.EnemySize.Regular;

	// Token: 0x040019BC RID: 6588
	[SerializeField]
	private bool bigEnemyDeath;

	// Token: 0x040019BD RID: 6589
	[SerializeField]
	private bool preventDeathAfterHero;

	// Token: 0x040019BE RID: 6590
	private EventRelayResponder corpseEventResponder;

	// Token: 0x040019C1 RID: 6593
	[Header("Deprecated/Unusued Variables")]
	[SerializeField]
	private bool ignoreHazards;

	// Token: 0x040019C2 RID: 6594
	[SerializeField]
	private float invulnerableTime;

	// Token: 0x040019C3 RID: 6595
	[SerializeField]
	private bool semiPersistent;

	// Token: 0x040019C4 RID: 6596
	public bool isDead;

	// Token: 0x040019C5 RID: 6597
	public bool ignorePersistence;

	// Token: 0x040019C6 RID: 6598
	private GameObject sendKilledTo;

	// Token: 0x040019C7 RID: 6599
	private float evasionByHitRemaining;

	// Token: 0x040019C8 RID: 6600
	private HitInstance lastHitInstance;

	// Token: 0x040019C9 RID: 6601
	private int directionOfLastAttack;

	// Token: 0x040019CA RID: 6602
	private int initHp;

	// Token: 0x040019CB RID: 6603
	private bool hasTakenDamage;

	// Token: 0x040019CC RID: 6604
	private AttackTypes lastAttackType;

	// Token: 0x040019CD RID: 6605
	private const float rapidBulletTime = 0.15f;

	// Token: 0x040019CE RID: 6606
	private float rapidBulletTimer;

	// Token: 0x040019CF RID: 6607
	private int rapidBulletCount;

	// Token: 0x040019D0 RID: 6608
	private const float rapidBombTime = 0.75f;

	// Token: 0x040019D1 RID: 6609
	private float rapidBombTimer;

	// Token: 0x040019D2 RID: 6610
	private int rapidBombCount;

	// Token: 0x040019D3 RID: 6611
	public float tinkTimer;

	// Token: 0x040019D4 RID: 6612
	public const float timeBetweenTinks = 0.1f;

	// Token: 0x040019D5 RID: 6613
	private static readonly float[] _shellShardMultiplierArray = new float[]
	{
		1f,
		1f,
		1f,
		1.25f,
		1.25f,
		1.25f,
		1.5f,
		1.5f
	};

	// Token: 0x040019D8 RID: 6616
	private PlayMakerFSM stunControlFsm;

	// Token: 0x040019DA RID: 6618
	private bool notifiedBattleScene;

	// Token: 0x040019DB RID: 6619
	private readonly List<HealthManager.LagHitsTracker> runningLagHits = new List<HealthManager.LagHitsTracker>();

	// Token: 0x040019DC RID: 6620
	private readonly Dictionary<LagHitDamageType, HealthManager.LagHitsTracker> runningSingleLagHits = new Dictionary<LagHitDamageType, HealthManager.LagHitsTracker>();

	// Token: 0x040019DD RID: 6621
	private readonly List<GameObject> spawnedFlingTracker = new List<GameObject>();

	// Token: 0x040019DE RID: 6622
	private static readonly List<HealthManager> _activeHealthManagers = new List<HealthManager>();

	// Token: 0x040019DF RID: 6623
	private bool addedPhysicalPusher;

	// Token: 0x040019E0 RID: 6624
	private GameObject physicalPusher;

	// Token: 0x040019E1 RID: 6625
	private bool hasBlackThreadState;

	// Token: 0x040019E2 RID: 6626
	private BlackThreadState blackThreadState;

	// Token: 0x040019E3 RID: 6627
	private HealthManager.QueuedDropItem queuedDropItem;

	// Token: 0x040019E4 RID: 6628
	private bool hasAwaken;

	// Token: 0x040019E5 RID: 6629
	private bool hasStarted;

	// Token: 0x020015C9 RID: 5577
	[Serializable]
	private class ItemDropProbability : Probability.ProbabilityBase<SavedItem>
	{
		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x0600880E RID: 34830 RVA: 0x00279143 File Offset: 0x00277343
		public override SavedItem Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x0400888D RID: 34957
		[SerializeField]
		private SavedItem item;

		// Token: 0x0400888E RID: 34958
		public MinMaxInt Amount = new MinMaxInt(1, 1);

		// Token: 0x0400888F RID: 34959
		public CollectableItemPickup CustomPickupPrefab;

		// Token: 0x04008890 RID: 34960
		public int LimitActiveInScene;
	}

	// Token: 0x020015CA RID: 5578
	[Serializable]
	private class ItemDropGroup
	{
		// Token: 0x04008891 RID: 34961
		[Range(0f, 1f)]
		public float TotalProbability = 1f;

		// Token: 0x04008892 RID: 34962
		public List<HealthManager.ItemDropProbability> Drops;
	}

	// Token: 0x020015CB RID: 5579
	[Serializable]
	private class DamageScalingConfig
	{
		// Token: 0x06008811 RID: 34833 RVA: 0x00279174 File Offset: 0x00277374
		public float GetMultFromLevel(int level)
		{
			float result;
			if (level >= 0)
			{
				switch (level)
				{
				case 0:
					result = this.Level1Mult;
					break;
				case 1:
					result = this.Level2Mult;
					break;
				case 2:
					result = this.Level3Mult;
					break;
				case 3:
					result = this.Level4Mult;
					break;
				default:
					result = this.Level5Mult;
					break;
				}
			}
			else
			{
				result = 1f;
			}
			return result;
		}

		// Token: 0x04008893 RID: 34963
		[FormerlySerializedAs("Upg1Mult")]
		public float Level1Mult = 1f;

		// Token: 0x04008894 RID: 34964
		[FormerlySerializedAs("Upg2Mult")]
		public float Level2Mult = 1f;

		// Token: 0x04008895 RID: 34965
		[FormerlySerializedAs("Upg3Mult")]
		public float Level3Mult = 1f;

		// Token: 0x04008896 RID: 34966
		[FormerlySerializedAs("Upg4Mult")]
		public float Level4Mult = 1f;

		// Token: 0x04008897 RID: 34967
		[FormerlySerializedAs("Upg5Mult")]
		public float Level5Mult = 1f;
	}

	// Token: 0x020015CC RID: 5580
	[Serializable]
	public enum EnemyTypes
	{
		// Token: 0x04008899 RID: 34969
		Regular,
		// Token: 0x0400889A RID: 34970
		Shade = 3,
		// Token: 0x0400889B RID: 34971
		Armoured = 6
	}

	// Token: 0x020015CD RID: 5581
	public enum ReaperBundleTiers
	{
		// Token: 0x0400889D RID: 34973
		Normal,
		// Token: 0x0400889E RID: 34974
		Reduced,
		// Token: 0x0400889F RID: 34975
		None
	}

	// Token: 0x020015CE RID: 5582
	public enum EnemySize
	{
		// Token: 0x040088A1 RID: 34977
		Small,
		// Token: 0x040088A2 RID: 34978
		Regular,
		// Token: 0x040088A3 RID: 34979
		Large
	}

	// Token: 0x020015CF RID: 5583
	[Flags]
	public enum IgnoreFlags
	{
		// Token: 0x040088A5 RID: 34981
		None = 0,
		// Token: 0x040088A6 RID: 34982
		RageHeal = 1,
		// Token: 0x040088A7 RID: 34983
		WitchHeal = 2
	}

	// Token: 0x020015D0 RID: 5584
	private class LagHitsTracker
	{
		// Token: 0x040088A8 RID: 34984
		public GameObject Source;

		// Token: 0x040088A9 RID: 34985
		public Coroutine LagHitsRoutine;

		// Token: 0x040088AA RID: 34986
		public Action OnLagHitsEnd;
	}

	// Token: 0x020015D1 RID: 5585
	// (Invoke) Token: 0x06008815 RID: 34837
	public delegate void DeathEvent();

	// Token: 0x020015D2 RID: 5586
	public struct QueuedDropItem
	{
		// Token: 0x06008818 RID: 34840 RVA: 0x00279218 File Offset: 0x00277418
		public QueuedDropItem(CollectableItemToolDamage dropItem, int amount)
		{
			this = default(HealthManager.QueuedDropItem);
			this.isQueued = true;
			this.dropItem = dropItem;
			this.amount = amount;
		}

		// Token: 0x06008819 RID: 34841 RVA: 0x00279236 File Offset: 0x00277436
		public void Reset()
		{
			this.isQueued = false;
			this.dropItem = null;
		}

		// Token: 0x040088AB RID: 34987
		public bool isQueued;

		// Token: 0x040088AC RID: 34988
		public CollectableItemToolDamage dropItem;

		// Token: 0x040088AD RID: 34989
		public int amount;
	}

	// Token: 0x020015D3 RID: 5587
	private class StealLagHit : LagHitOptions
	{
		// Token: 0x0600881A RID: 34842 RVA: 0x00279248 File Offset: 0x00277448
		public StealLagHit(HealthManager healthManager, int stolenGeo, int flingGeo, int stolenShards, int flingShards)
		{
			this.healthManager = healthManager;
			this.stolenGeo = stolenGeo;
			this.flingGeo = flingGeo;
			this.stolenShards = stolenShards;
			this.flingShards = flingShards;
			this.UseNailDamage = true;
			this.NailDamageMultiplier = 0.25f;
			this.DamageType = LagHitDamageType.Slash;
			this.MagnitudeMult = 0.3f;
			this.StartDelay = 0.2f;
			this.HitCount = 1;
		}

		// Token: 0x0600881B RID: 34843 RVA: 0x002792B8 File Offset: 0x002774B8
		public override void OnEnd(Transform effectsPoint, Vector3 effectOrigin, HitInstance hitInstance)
		{
			Transform transform = this.healthManager.transform;
			Gameplay.ThiefSnatchEffectPrefab.Spawn(transform.position).Setup(transform, this.stolenGeo > 0, this.stolenShards > 0);
			if (this.flingGeo > 0)
			{
				this.healthManager.FlingStealCurrency(this.flingGeo, Gameplay.SmallGeoPrefab);
			}
			if (this.stolenGeo > 0)
			{
				CurrencyManager.AddGeo(this.stolenGeo);
			}
			if (this.stolenShards > 0)
			{
				CurrencyManager.AddShards(this.stolenShards);
			}
			if (this.flingShards > 0)
			{
				this.healthManager.FlingStealCurrency(this.flingShards, Gameplay.ShellShardPrefab);
			}
		}

		// Token: 0x040088AE RID: 34990
		private readonly HealthManager healthManager;

		// Token: 0x040088AF RID: 34991
		private readonly int stolenGeo;

		// Token: 0x040088B0 RID: 34992
		private readonly int flingGeo;

		// Token: 0x040088B1 RID: 34993
		private readonly int stolenShards;

		// Token: 0x040088B2 RID: 34994
		private readonly int flingShards;
	}
}
