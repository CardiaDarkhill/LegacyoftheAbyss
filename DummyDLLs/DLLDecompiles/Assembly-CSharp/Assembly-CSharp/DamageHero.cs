using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002C7 RID: 711
public class DamageHero : MonoBehaviour, IInitialisable
{
	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06001962 RID: 6498 RVA: 0x0007471C File Offset: 0x0007291C
	// (remove) Token: 0x06001963 RID: 6499 RVA: 0x00074754 File Offset: 0x00072954
	public event Action HeroDamaged;

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06001964 RID: 6500 RVA: 0x00074789 File Offset: 0x00072989
	public bool OverrideCollisionSide
	{
		get
		{
			return this.overrideCollisionSide;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06001965 RID: 6501 RVA: 0x00074791 File Offset: 0x00072991
	public CollisionSide CollisionSide
	{
		get
		{
			return this.collisionSide;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001966 RID: 6502 RVA: 0x00074799 File Offset: 0x00072999
	public bool InvertCollisionSide
	{
		get
		{
			return this.invertCollisionSide;
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06001967 RID: 6503 RVA: 0x000747A1 File Offset: 0x000729A1
	public bool CanCauseDamage
	{
		get
		{
			return Time.timeAsDouble >= this.damageAllowedTime;
		}
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000747B3 File Offset: 0x000729B3
	private bool? IsFsmEventValid(string eventName)
	{
		return this.HeroDamagedFSM.IsEventValid(eventName, false);
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000747C4 File Offset: 0x000729C4
	private bool? IsFsmBoolValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName) || !this.HeroDamagedFSM)
		{
			return null;
		}
		return new bool?(this.HeroDamagedFSM.FsmVariables.BoolVariables.Any((FsmBool fsmBool) => fsmBool.Name == eventName));
	}

	// Token: 0x0600196A RID: 6506 RVA: 0x00074828 File Offset: 0x00072A28
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		if (this.damageAsset)
		{
			this.damageDealt = this.damageAsset.Value;
		}
		this.healthManager = base.GetComponentInParent<HealthManager>();
		if (this.healthManager)
		{
			this.healthManagerColliders = this.healthManager.GetComponents<Collider2D>();
			this.healthManager.TookDamage += this.OnDamaged;
		}
		this.recoil = base.GetComponentInParent<Recoil>();
		this.nonBouncer = base.GetComponentInParent<NonBouncer>();
		this.hasNonBouncer = (this.nonBouncer != null);
		Rigidbody2D rigidbody2D = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
		if (this.canClashTink && !this.noTerrainThunk && !rigidbody2D && this.collider)
		{
			this.collider.isTrigger = false;
			rigidbody2D = base.gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
			rigidbody2D.simulated = true;
			rigidbody2D.useFullKinematicContacts = true;
		}
		if (base.transform.parent)
		{
			this.parentCollider = base.transform.parent.GetComponent<Collider2D>();
		}
		if (this.hazardType == HazardType.STEAM)
		{
			FsmTemplate hornetMultiWounderFsmTemplate = Gameplay.HornetMultiWounderFsmTemplate;
			PlayMakerFSM playMakerFSM = base.gameObject.AddComponent<PlayMakerFSM>();
			playMakerFSM.Reset();
			playMakerFSM.SetFsmTemplate(hornetMultiWounderFsmTemplate);
			playMakerFSM.FsmVariables.FindFsmBool("z2 Steam Hazard").Value = true;
			this.damageDealt = 0;
		}
		return true;
	}

	// Token: 0x0600196B RID: 6507 RVA: 0x000749A1 File Offset: 0x00072BA1
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x0600196C RID: 6508 RVA: 0x000749BC File Offset: 0x00072BBC
	private void Awake()
	{
		DamageHero._damageHeroes[base.gameObject] = this;
		this.OnAwake();
	}

	// Token: 0x0600196D RID: 6509 RVA: 0x000749D6 File Offset: 0x00072BD6
	private void OnDestroy()
	{
		if (this.healthManager)
		{
			this.healthManager.TookDamage -= this.OnDamaged;
		}
		DamageHero._damageHeroes.Remove(base.gameObject);
	}

	// Token: 0x0600196E RID: 6510 RVA: 0x00074A10 File Offset: 0x00072C10
	private void OnEnable()
	{
		if (this.resetOnEnable)
		{
			if (this.initialValue == null)
			{
				this.initialValue = new int?(this.damageDealt);
			}
			else
			{
				this.damageDealt = this.initialValue.Value;
			}
		}
		this.nailClashRoutine = null;
		this.preventClashTink = false;
		if (base.transform.parent && this.collider)
		{
			Rigidbody2D componentInParent = base.transform.parent.GetComponentInParent<Rigidbody2D>();
			if (componentInParent)
			{
				int attachedColliders = componentInParent.GetAttachedColliders(this.parentAttachedColliders);
				for (int i = 0; i < attachedColliders; i++)
				{
					Physics2D.IgnoreCollision(this.parentAttachedColliders[i], this.collider);
				}
			}
		}
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Danger, false);
	}

	// Token: 0x0600196F RID: 6511 RVA: 0x00074AD8 File Offset: 0x00072CD8
	private void OnDisable()
	{
		if (this.cancelAttack)
		{
			HeroController instance = HeroController.instance;
			if (instance)
			{
				instance.NailParryRecover();
			}
			this.cancelAttack = false;
		}
	}

	// Token: 0x06001970 RID: 6512 RVA: 0x00074B08 File Offset: 0x00072D08
	public static bool TryGet(GameObject gameObject, out DamageHero damageHero)
	{
		return DamageHero._damageHeroes.TryGetValue(gameObject, out damageHero);
	}

	// Token: 0x06001971 RID: 6513 RVA: 0x00074B16 File Offset: 0x00072D16
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.enabled)
		{
			return;
		}
		this.TryClashTinkCollider(collision);
	}

	// Token: 0x06001972 RID: 6514 RVA: 0x00074B28 File Offset: 0x00072D28
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.canClashTink && collision.gameObject.layer == 8 && !this.noTerrainThunk)
		{
			if (!this.noTerrainRecoil && this.recoil)
			{
				int attackDirection;
				int num;
				TerrainThunkUtils.GenerateTerrainThunk(collision, this.contactsTempStore, TerrainThunkUtils.SlashDirection.None, this.recoil.transform.position, out attackDirection, out num, new TerrainThunkUtils.TerrainThunkConditionDelegate(this.TerrainThunkCondition));
				if (num != 3 && num != 1)
				{
					this.recoil.RecoilByDirection(attackDirection, 0.5f);
				}
			}
			else
			{
				int attackDirection;
				int num;
				TerrainThunkUtils.GenerateTerrainThunk(collision, this.contactsTempStore, TerrainThunkUtils.SlashDirection.None, Vector2.zero, out attackDirection, out num, new TerrainThunkUtils.TerrainThunkConditionDelegate(this.TerrainThunkCondition));
			}
		}
		this.TryClashTinkCollider(collision.collider);
	}

	// Token: 0x06001973 RID: 6515 RVA: 0x00074BF0 File Offset: 0x00072DF0
	private bool TerrainThunkCondition(TerrainThunkUtils.TerrainThunkConditionArgs args)
	{
		if (args.RecoilDirection == 1)
		{
			return false;
		}
		if (!this.parentCollider)
		{
			return true;
		}
		Vector3 min = this.parentCollider.bounds.min;
		return args.ThunkPos.y >= min.y + 0.05f;
	}

	// Token: 0x06001974 RID: 6516 RVA: 0x00074C48 File Offset: 0x00072E48
	private void TryClashTinkCollider(Collider2D collision)
	{
		if (this.hazardType == HazardType.SPIKES)
		{
			DamageEnemies component = collision.GetComponent<DamageEnemies>();
			if (component)
			{
				component.OnHitSpikes();
			}
		}
		if (!this.canClashTink)
		{
			return;
		}
		if (this.nailClashRoutine != null)
		{
			return;
		}
		if (this.preventClashTink)
		{
			return;
		}
		if (collision.gameObject.layer != 16)
		{
			return;
		}
		string tag = collision.gameObject.tag;
		Transform transform = collision.transform;
		Vector3 position = transform.position;
		Transform parent = transform.parent;
		if (!parent)
		{
			return;
		}
		DamageEnemies componentInChildren = parent.GetComponentInChildren<DamageEnemies>();
		if (!componentInChildren)
		{
			return;
		}
		if (this.healthManager != null)
		{
			if (componentInChildren.HasBeenDamaged(this.healthManager))
			{
				return;
			}
			componentInChildren.PreventDamage(this.healthManager);
			this.healthManager.CancelLagHitsForSource(componentInChildren.gameObject);
		}
		HeroController instance = HeroController.instance;
		if (componentInChildren.doesNotParry)
		{
			return;
		}
		if (tag == "Nail Attack" && !this.noClashFreeze && instance.parryInvulnTimer < Mathf.Epsilon)
		{
			GameManager.instance.FreezeMoment(FreezeMomentTypes.NailClashEffect, null);
		}
		componentInChildren.SendParried(!this.hasNonBouncer || !this.nonBouncer.active);
		if (this.healthManagerColliders != null)
		{
			foreach (Collider2D col in this.healthManagerColliders)
			{
				componentInChildren.PreventDamage(col);
			}
		}
		NailAttackBase component2 = componentInChildren.GetComponent<NailAttackBase>();
		if (component2 && !component2.CanHitSpikes)
		{
			CollisionSide damageSide;
			if (this.overrideCollisionSide)
			{
				damageSide = this.collisionSide;
			}
			else
			{
				CollisionSide collisionSide;
				switch (DirectionUtils.GetCardinalDirection(componentInChildren.direction))
				{
				case 0:
					collisionSide = CollisionSide.right;
					break;
				case 1:
					collisionSide = CollisionSide.top;
					break;
				case 2:
					collisionSide = CollisionSide.left;
					break;
				case 3:
					collisionSide = CollisionSide.bottom;
					break;
				default:
					collisionSide = CollisionSide.other;
					break;
				}
				damageSide = collisionSide;
			}
			instance.TakeDamage(base.gameObject, damageSide, 1, HazardType.ENEMY, this.damagePropertyFlags);
			return;
		}
		this.nailClashRoutine = base.StartCoroutine(this.NailClash(componentInChildren.direction, tag, position));
	}

	// Token: 0x06001975 RID: 6517 RVA: 0x00074E49 File Offset: 0x00073049
	private IEnumerator NailClash(float direction, string colliderTag, Vector3 clasherPos)
	{
		HeroController hc = HeroController.instance;
		Effects.NailClashTinkShake.DoShake(this, false);
		Effects.NailClashParrySound.SpawnAndPlayOneShot(base.transform.position, null);
		if (colliderTag == "Nail Attack")
		{
			hc.NailParry();
			this.cancelAttack = true;
			if (direction < 45f)
			{
				if (this.noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(1.5f, 0f, 0f));
				}
				else
				{
					hc.RecoilLeft();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(1.5f, 0f, 0f));
				}
				this.ClashEvents.OnClashRight.Invoke();
			}
			else if (direction < 135f)
			{
				if (this.noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(0f, 1.5f, 0f));
				}
				else
				{
					hc.RecoilDown();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(0f, 1.5f, 0f));
				}
				this.ClashEvents.OnClashUp.Invoke();
			}
			else if (direction < 225f)
			{
				if (this.noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(-1.5f, 0f, 0f));
				}
				else
				{
					hc.RecoilRight();
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(-1.5f, 0f, 0f));
				}
				this.ClashEvents.OnClashLeft.Invoke();
			}
			else if (direction < 360f)
			{
				if (this.noClashFreeze)
				{
					Effects.NailClashParryEffectSmall.Spawn(clasherPos + new Vector3(-1.5f * hc.gameObject.transform.localScale.x, -1f, 0f));
				}
				else
				{
					hc.DownspikeBounce(false, null);
					Effects.NailClashParryEffect.Spawn(clasherPos + new Vector3(-1.5f * hc.gameObject.transform.localScale.x, -1f, 0f));
				}
				this.ClashEvents.OnClashDown.Invoke();
			}
		}
		else
		{
			this.cancelAttack = false;
			Effects.NailClashParryEffect.Spawn(clasherPos);
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "PARRIED", false);
		if (base.transform.parent)
		{
			FSMUtility.SendEventToGameObject(base.transform.parent.gameObject, "PARRIED", false);
		}
		yield return new WaitForSeconds(0.1f);
		if (this.cancelAttack)
		{
			hc.NailParryRecover();
			this.cancelAttack = false;
		}
		yield return null;
		this.nailClashRoutine = null;
		yield break;
	}

	// Token: 0x06001976 RID: 6518 RVA: 0x00074E6D File Offset: 0x0007306D
	private void OnDamaged()
	{
		this.preventClashTink = true;
	}

	// Token: 0x06001977 RID: 6519 RVA: 0x00074E78 File Offset: 0x00073078
	public void SendHeroDamagedEvent()
	{
		if (this.HeroDamaged != null)
		{
			this.HeroDamaged();
		}
		if (this.HeroDamagedFSM != null)
		{
			if (!string.IsNullOrEmpty(this.HeroDamagedFSMEvent))
			{
				this.HeroDamagedFSM.SendEvent(this.HeroDamagedFSMEvent);
			}
			if (!string.IsNullOrEmpty(this.HeroDamagedFSMBool))
			{
				FsmBool fsmBool = this.HeroDamagedFSM.FsmVariables.BoolVariables.FirstOrDefault((FsmBool b) => b.Name == this.HeroDamagedFSMBool);
				if (fsmBool != null)
				{
					fsmBool.Value = true;
				}
			}
			if (!string.IsNullOrEmpty(this.HeroDamagedFSMGameObject))
			{
				FsmGameObject fsmGameObject = this.HeroDamagedFSM.FsmVariables.GameObjectVariables.FirstOrDefault((FsmGameObject b) => b.Name == this.HeroDamagedFSMGameObject);
				if (fsmGameObject != null)
				{
					fsmGameObject.Value = base.transform.gameObject;
				}
			}
		}
		this.OnDamagedHero.Invoke();
	}

	// Token: 0x06001978 RID: 6520 RVA: 0x00074F4E File Offset: 0x0007314E
	public void SetDamageAmount(int amount)
	{
		this.damageDealt = amount;
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x00074F58 File Offset: 0x00073158
	public void SetCooldown(float cooldown)
	{
		if (cooldown <= 0f)
		{
			return;
		}
		double num = Time.timeAsDouble + (double)cooldown;
		if (num > this.damageAllowedTime)
		{
			this.damageAllowedTime = num;
		}
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x00074F87 File Offset: 0x00073187
	public bool IsDamagerSpikes()
	{
		return this.hazardType == HazardType.SPIKES;
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x00074F95 File Offset: 0x00073195
	[ContextMenu("Test", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x00074F9C File Offset: 0x0007319C
	[ContextMenu("Test")]
	private void Test()
	{
		HeroController.instance.GetComponentInChildren<HeroBox>().TakeDamageFromDamager(this, base.gameObject);
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x00074FF0 File Offset: 0x000731F0
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400185F RID: 6239
	[ModifiableProperty]
	[Conditional("damageAsset", false, false, false)]
	public int damageDealt = 1;

	// Token: 0x04001860 RID: 6240
	public HazardType hazardType = HazardType.ENEMY;

	// Token: 0x04001861 RID: 6241
	[SerializeField]
	[QuickCreateAsset("Data Assets/Damages", "damageDealt", "value")]
	private DamageReference damageAsset;

	// Token: 0x04001862 RID: 6242
	[Space]
	[EnumPickerBitmask]
	public DamagePropertyFlags damagePropertyFlags;

	// Token: 0x04001863 RID: 6243
	[Space]
	public bool resetOnEnable;

	// Token: 0x04001864 RID: 6244
	private int? initialValue;

	// Token: 0x04001865 RID: 6245
	public bool canClashTink;

	// Token: 0x04001866 RID: 6246
	public bool forceParry;

	// Token: 0x04001867 RID: 6247
	[ModifiableProperty]
	[Conditional("canClashTink", true, false, false)]
	public bool noClashFreeze;

	// Token: 0x04001868 RID: 6248
	[ModifiableProperty]
	[Conditional("canClashTink", true, false, false)]
	public bool noTerrainThunk;

	// Token: 0x04001869 RID: 6249
	public bool noTerrainRecoil;

	// Token: 0x0400186A RID: 6250
	public bool noCorpseSpikeStick;

	// Token: 0x0400186B RID: 6251
	public bool noBounceCooldown;

	// Token: 0x0400186C RID: 6252
	[SerializeField]
	[Space]
	private bool overrideCollisionSide;

	// Token: 0x0400186D RID: 6253
	[ModifiableProperty]
	[Conditional("overrideCollisionSide", true, false, true)]
	[SerializeField]
	private CollisionSide collisionSide;

	// Token: 0x0400186E RID: 6254
	[SerializeField]
	private bool invertCollisionSide;

	// Token: 0x0400186F RID: 6255
	[Space]
	public PlayMakerFSM HeroDamagedFSM;

	// Token: 0x04001870 RID: 6256
	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	public bool AlwaysSendDamaged;

	// Token: 0x04001871 RID: 6257
	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	[InspectorValidation("IsFsmEventValid")]
	public string HeroDamagedFSMEvent;

	// Token: 0x04001872 RID: 6258
	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	[InspectorValidation("IsFsmBoolValid")]
	public string HeroDamagedFSMBool;

	// Token: 0x04001873 RID: 6259
	[ModifiableProperty]
	[Conditional("HeroDamagedFSM", true, false, true)]
	public string HeroDamagedFSMGameObject;

	// Token: 0x04001874 RID: 6260
	[Space]
	public DamageHero.ClashEventsWrapper ClashEvents;

	// Token: 0x04001875 RID: 6261
	public UnityEvent OnDamagedHero;

	// Token: 0x04001876 RID: 6262
	private bool preventClashTink;

	// Token: 0x04001877 RID: 6263
	private double damageAllowedTime;

	// Token: 0x04001878 RID: 6264
	private Coroutine nailClashRoutine;

	// Token: 0x04001879 RID: 6265
	private Collider2D collider;

	// Token: 0x0400187A RID: 6266
	private readonly ContactPoint2D[] contactsTempStore = new ContactPoint2D[10];

	// Token: 0x0400187B RID: 6267
	private readonly Collider2D[] parentAttachedColliders = new Collider2D[10];

	// Token: 0x0400187C RID: 6268
	private Collider2D parentCollider;

	// Token: 0x0400187D RID: 6269
	private HealthManager healthManager;

	// Token: 0x0400187E RID: 6270
	private Collider2D[] healthManagerColliders;

	// Token: 0x0400187F RID: 6271
	private Recoil recoil;

	// Token: 0x04001880 RID: 6272
	private static readonly Dictionary<GameObject, DamageHero> _damageHeroes = new Dictionary<GameObject, DamageHero>();

	// Token: 0x04001881 RID: 6273
	private bool hasAwaken;

	// Token: 0x04001882 RID: 6274
	private bool hasStarted;

	// Token: 0x04001883 RID: 6275
	private bool hasNonBouncer;

	// Token: 0x04001884 RID: 6276
	private NonBouncer nonBouncer;

	// Token: 0x04001885 RID: 6277
	private bool cancelAttack;

	// Token: 0x020015AF RID: 5551
	[Serializable]
	public class ClashEventsWrapper
	{
		// Token: 0x0400883A RID: 34874
		public UnityEvent OnClashUp;

		// Token: 0x0400883B RID: 34875
		public UnityEvent OnClashDown;

		// Token: 0x0400883C RID: 34876
		public UnityEvent OnClashLeft;

		// Token: 0x0400883D RID: 34877
		public UnityEvent OnClashRight;
	}
}
