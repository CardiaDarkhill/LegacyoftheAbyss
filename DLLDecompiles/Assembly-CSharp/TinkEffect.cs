using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000C4 RID: 196
public class TinkEffect : HitResponseBase, IHitResponder
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000630 RID: 1584 RVA: 0x0001F715 File Offset: 0x0001D915
	public int HitPriority
	{
		get
		{
			return this.hitPriority;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000631 RID: 1585 RVA: 0x0001F71D File Offset: 0x0001D91D
	// (set) Token: 0x06000632 RID: 1586 RVA: 0x0001F725 File Offset: 0x0001D925
	public IHitResponderOverride OverrideResponder
	{
		get
		{
			return this.overrideResponder;
		}
		set
		{
			this.overrideResponder = value;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000633 RID: 1587 RVA: 0x0001F72E File Offset: 0x0001D92E
	public bool HitRecurseUpwards
	{
		get
		{
			return !this.IsActive;
		}
	}

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000634 RID: 1588 RVA: 0x0001F73C File Offset: 0x0001D93C
	// (remove) Token: 0x06000635 RID: 1589 RVA: 0x0001F774 File Offset: 0x0001D974
	public event TinkEffect.TinkEffectSpawnEvent OnSpawnedTink;

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000636 RID: 1590 RVA: 0x0001F7A9 File Offset: 0x0001D9A9
	// (set) Token: 0x06000637 RID: 1591 RVA: 0x0001F7B1 File Offset: 0x0001D9B1
	public override bool IsActive
	{
		get
		{
			return this.isActive;
		}
		set
		{
			this.isActive = value;
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x0001F7BC File Offset: 0x0001D9BC
	private bool? IsFsmEventValid(string eventName)
	{
		if (!this.fsm || string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		return new bool?(this.fsm.FsmEvents.Any((FsmEvent fsmEvent) => fsmEvent.Name.Equals(eventName)));
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x0001F81B File Offset: 0x0001DA1B
	private void OnValidate()
	{
		if (this.onlyReactToDown)
		{
			this.directionMask = 0.SetBitAtIndex(3);
			this.onlyReactToDown = false;
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0001F83C File Offset: 0x0001DA3C
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		this.collider = base.GetComponent<Collider2D>();
		this.heroDamager = base.GetComponent<DamageHero>();
		if (!base.transform.IsOnHeroPlane() && !this.activeOnAnyPlane)
		{
			base.enabled = false;
		}
		this.preventDamageHealthManagers.RemoveAll((HealthManager o) => o == null);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0001F8B4 File Offset: 0x0001DAB4
	private void OnEnable()
	{
		this.AddDebugDrawComponent();
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0001F8BC File Offset: 0x0001DABC
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.overrideResponder != null && this.overrideResponder.WillRespond(damageInstance))
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.Source)
		{
			return IHitResponder.Response.None;
		}
		DamageEnemies component = damageInstance.Source.GetComponent<DamageEnemies>();
		if (!component)
		{
			return IHitResponder.Response.None;
		}
		return this.TryDoTinkReaction(component, true, true) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0001F92C File Offset: 0x0001DB2C
	public bool TryDoTinkReaction(Collider2D collision, bool doCamShake, bool doSound)
	{
		if (!this.isActive)
		{
			return false;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 17)
		{
			return false;
		}
		DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
		return component && this.TryDoTinkReaction(component, doCamShake, doSound);
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0001F974 File Offset: 0x0001DB74
	private bool CanTink(DamageEnemies damager)
	{
		NailSlashTerrainThunk componentInChildren = damager.GetComponentInChildren<NailSlashTerrainThunk>();
		return (!componentInChildren || !componentInChildren.WillHandleTink(this.collider)) && (!damager.doesNotTink || (this.tinkProperties & damager.AllowedTinkFlags) > ITinkResponder.TinkFlags.None);
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x0001F9BC File Offset: 0x0001DBBC
	private bool TryDoTinkReaction(DamageEnemies damager, bool doCamShake, bool doSound)
	{
		if (damager.attackType == AttackTypes.Coal)
		{
			return false;
		}
		if (!this.CanTink(damager))
		{
			return false;
		}
		GameObject gameObject = damager.gameObject;
		bool flag = gameObject.CompareTag("Nail Attack");
		if (damager.doesNotTinkThroughWalls && this.collider)
		{
			HeroController instance = HeroController.instance;
			Vector3 position = gameObject.transform.position;
			Vector3 v = flag ? instance.transform.position : position;
			Vector2 to = this.collider.ClosestPoint(v);
			RaycastHit2D raycastHit2D;
			if (global::Helper.IsLineHittingNoTriggers(v, to, 256, null, out raycastHit2D))
			{
				return false;
			}
		}
		Vector2 vector;
		return this.TryDoTinkReactionNoDamager(gameObject, doCamShake, doSound, flag, out vector);
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0001FA68 File Offset: 0x0001DC68
	public bool TryDoTinkReactionNoDamager(GameObject obj, bool doCamShake, bool doSound, bool isNailAttack, out Vector2 tinkPosition)
	{
		bool flag = true;
		bool flag2 = Time.timeAsDouble >= TinkEffect._nextTinkTime;
		NailAttackBase component = obj.GetComponent<NailAttackBase>();
		bool flag3 = isNailAttack && component && !component.CanHitSpikes;
		HeroController instance = HeroController.instance;
		Vector3 position = obj.transform.position;
		tinkPosition = position;
		Vector3 vector = isNailAttack ? instance.transform.position : position;
		if (this.useOwnYPosition)
		{
			position.y = (vector.y = base.transform.position.y);
		}
		DamageEnemies component2 = obj.GetComponent<DamageEnemies>();
		bool flag4 = component2 != null;
		if (!flag4)
		{
			component2 = obj.transform.parent.gameObject.GetComponent<DamageEnemies>();
			flag4 = (component2 != null);
			if (!flag4)
			{
				return false;
			}
		}
		float actualHitDirection;
		if (isNailAttack)
		{
			if (this.heroDamager && this.heroDamager.hazardType == HazardType.SPIKES && flag3)
			{
				instance.TakeDamage(this.heroDamager.gameObject, CollisionSide.top, this.heroDamager.damageDealt, HazardType.SPIKES, this.heroDamager.damagePropertyFlags);
				return false;
			}
			actualHitDirection = this.GetActualHitDirection(component2);
		}
		else
		{
			actualHitDirection = this.GetActualHitDirection(component2);
			ITinkResponder component3 = obj.GetComponent<ITinkResponder>();
			bool flag5 = component3 != null;
			if (flag5)
			{
				if ((this.ignoreResponders & component3.ResponderType) != ITinkResponder.TinkFlags.None)
				{
					return false;
				}
				component3.Tinked();
			}
			if (this.onlyReactToNail)
			{
				if (!flag5)
				{
					return false;
				}
				flag = false;
			}
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(actualHitDirection);
		switch (cardinalDirection)
		{
		case 0:
			if (!this.directionMask.IsBitSet(1))
			{
				return false;
			}
			if (isNailAttack && this.checkSlashPosition && obj.transform.position.x > base.transform.position.x)
			{
				return false;
			}
			this.TryPreventDamage(component2, 1);
			break;
		case 1:
			if (!this.directionMask.IsBitSet(2))
			{
				return false;
			}
			this.TryPreventDamage(component2, 2);
			break;
		case 2:
			if (!this.directionMask.IsBitSet(0))
			{
				return false;
			}
			if (isNailAttack && this.checkSlashPosition && obj.transform.position.x < base.transform.position.x)
			{
				return false;
			}
			this.TryPreventDamage(component2, 0);
			break;
		case 3:
			if (!this.directionMask.IsBitSet(3))
			{
				return false;
			}
			this.TryPreventDamage(component2, 3);
			break;
		}
		if (flag2 && this.RecoilHero && flag4)
		{
			component2.OnTinkEffectTink();
		}
		bool flag6 = base.gameObject.GetComponent<NonBouncer>();
		if (flag4)
		{
			int layer = base.gameObject.layer;
			if ((layer == 11 || layer == 17 || layer == 19) && !flag6)
			{
				component2.OnBounceableTink();
				switch (cardinalDirection)
				{
				case 0:
					component2.OnBounceableTinkRight();
					break;
				case 1:
					component2.OnBounceableTinkUp();
					break;
				case 2:
					component2.OnBounceableTinkLeft();
					break;
				case 3:
					component2.OnBounceableTinkDown();
					break;
				}
			}
		}
		if (isNailAttack)
		{
			if (flag2)
			{
				TinkEffect._nextTinkTime = Time.timeAsDouble + 0.009999999776482582;
			}
			if (!this.gameCam)
			{
				this.gameCam = GameCameras.instance;
			}
			if (doCamShake && flag2 && this.gameCam)
			{
				if (this.overrideCamShake)
				{
					this.camShakeOverride.DoShake(this, true);
				}
				else
				{
					this.gameCam.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		Vector3 euler = new Vector3(0f, 0f, 0f);
		bool flag7 = this.collider != null;
		if (this.useNailPosition && (!flag4 || !component2.IgnoreNailPosition))
		{
			flag7 = false;
		}
		Vector2 vector2 = Vector2.zero;
		float num = 0f;
		float num2 = 0f;
		if (flag7)
		{
			Bounds bounds = this.collider.bounds;
			vector2 = base.transform.TransformPoint(this.collider.offset);
			num = bounds.size.x * 0.5f;
			num2 = bounds.size.y * 0.5f;
		}
		Vector3 vector3;
		switch (cardinalDirection)
		{
		case 0:
			if (isNailAttack && flag2 && this.RecoilHero)
			{
				instance.RecoilLeft();
			}
			if (flag)
			{
				if (this.sendDirectionalFSMEvents && this.fsm)
				{
					this.fsm.SendEvent("TINK RIGHT");
				}
				base.SendHitInDirection(obj, HitInstance.HitDirection.Right);
			}
			if (flag7)
			{
				float num3 = Mathf.Max(0f, num2 - 1.5f);
				position.y = Mathf.Clamp(position.y, vector2.y - num3, vector2.y + num3);
				vector3 = new Vector3(vector2.x - num, position.y, 0.002f);
			}
			else if (isNailAttack)
			{
				vector3 = new Vector3(vector.x + 2f, vector.y, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector.x, vector.y, 0.002f);
			}
			break;
		case 1:
			if (isNailAttack && flag2 && this.RecoilHero)
			{
				instance.RecoilDown();
			}
			if (flag)
			{
				if (this.sendDirectionalFSMEvents && this.fsm)
				{
					this.fsm.SendEvent("TINK UP");
				}
				base.SendHitInDirection(obj, HitInstance.HitDirection.Up);
			}
			if (flag7)
			{
				vector3 = new Vector3(position.x, Mathf.Max(vector2.y - num2, position.y), 0.002f);
			}
			else if (isNailAttack)
			{
				vector3 = new Vector3(vector.x, vector.y + 2f, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector.x, vector.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 90f);
			break;
		case 2:
			if (isNailAttack && flag2 && this.RecoilHero)
			{
				instance.RecoilRight();
			}
			if (flag)
			{
				if (this.sendDirectionalFSMEvents && this.fsm)
				{
					this.fsm.SendEvent("TINK LEFT");
				}
				base.SendHitInDirection(obj, HitInstance.HitDirection.Left);
			}
			if (flag7)
			{
				float num4 = Mathf.Max(0f, num2 - 1.5f);
				position.y = Mathf.Clamp(position.y, vector2.y - num4, vector2.y + num4);
				vector3 = new Vector3(vector2.x + num, position.y, 0.002f);
			}
			else if (isNailAttack)
			{
				vector3 = new Vector3(vector.x - 2f, vector.y, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector.x, vector.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 180f);
			break;
		default:
			if (flag)
			{
				if (this.sendDirectionalFSMEvents && this.fsm)
				{
					this.fsm.SendEvent("TINK DOWN");
					if (isNailAttack)
					{
						this.fsm.SendEvent(instance.cState.facingRight ? "TINK DOWN R" : "TINK DOWN L");
					}
				}
				base.SendHitInDirection(obj, HitInstance.HitDirection.Down);
			}
			if (flag7)
			{
				float num5 = position.x;
				if (num5 < vector2.x - num)
				{
					num5 = vector2.x - num;
				}
				if (num5 > vector2.x + num)
				{
					num5 = vector2.x + num;
				}
				vector3 = new Vector3(num5, Mathf.Min(vector2.y + num2, position.y), 0.002f);
			}
			else if (isNailAttack)
			{
				vector3 = new Vector3(vector.x, vector.y - 2f, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector.x, vector.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 270f);
			break;
		}
		GameObject gameObject = flag3 ? Effects.TinkEffectDullPrefab : this.blockEffect;
		if (flag2)
		{
			Quaternion rotation = Quaternion.Euler(euler);
			if (gameObject)
			{
				AudioSource component4 = gameObject.Spawn(vector3, rotation).GetComponent<AudioSource>();
				if (component4)
				{
					component4.pitch = Random.Range(0.85f, 1.15f);
					component4.volume = (doSound ? 1f : 0f);
				}
			}
			TinkEffect.TinkEffectSpawnEvent onSpawnedTink = this.OnSpawnedTink;
			if (onSpawnedTink != null)
			{
				onSpawnedTink(vector3, rotation);
			}
		}
		tinkPosition = vector3;
		if (!flag)
		{
			return true;
		}
		if (this.sendFSMEvent && this.fsm)
		{
			this.fsm.SendEvent(this.FSMEvent);
		}
		this.OnTinked.Invoke();
		if (flag4 && component2.attackType == AttackTypes.Heavy)
		{
			this.OnTinkedHeavy.Invoke();
		}
		switch (cardinalDirection)
		{
		case 0:
			this.OnTinkedRight.Invoke();
			break;
		case 1:
			this.OnTinkedUp.Invoke();
			break;
		case 2:
			this.OnTinkedLeft.Invoke();
			break;
		case 3:
			this.OnTinkedDown.Invoke();
			break;
		}
		return true;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x000203AC File Offset: 0x0001E5AC
	private void TryPreventDamage(DamageEnemies damager, int bit)
	{
		if (!damager)
		{
			return;
		}
		if (this.preventDamageDirection.IsBitSet(bit))
		{
			foreach (HealthManager hitResponder in this.preventDamageHealthManagers)
			{
				damager.PreventDamage(hitResponder);
			}
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00020418 File Offset: 0x0001E618
	private float GetActualHitDirection(DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = base.transform.position - damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0002047F File Offset: 0x0001E67F
	public void SetFsmEvent(string eventName)
	{
		this.sendFSMEvent = true;
		this.fsm = base.GetComponent<PlayMakerFSM>();
		this.FSMEvent = eventName;
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0002049B File Offset: 0x0001E69B
	public void SetSendDirectionalFSMEvents(bool set)
	{
		this.sendDirectionalFSMEvents = set;
	}

	// Token: 0x040005FA RID: 1530
	[SerializeField]
	private bool isActive = true;

	// Token: 0x040005FB RID: 1531
	[SerializeField]
	private int hitPriority;

	// Token: 0x040005FC RID: 1532
	[Space]
	public GameObject blockEffect;

	// Token: 0x040005FD RID: 1533
	[SerializeField]
	private bool overrideCamShake;

	// Token: 0x040005FE RID: 1534
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCamShake", true, false, false)]
	private CameraShakeTarget camShakeOverride;

	// Token: 0x040005FF RID: 1535
	public bool useNailPosition;

	// Token: 0x04000600 RID: 1536
	public bool useOwnYPosition;

	// Token: 0x04000601 RID: 1537
	public bool sendFSMEvent;

	// Token: 0x04000602 RID: 1538
	public PlayMakerFSM fsm;

	// Token: 0x04000603 RID: 1539
	[ModifiableProperty]
	[Conditional("sendFSMEvent", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	public string FSMEvent;

	// Token: 0x04000604 RID: 1540
	public bool sendDirectionalFSMEvents;

	// Token: 0x04000605 RID: 1541
	public bool RecoilHero = true;

	// Token: 0x04000606 RID: 1542
	public bool onlyReactToNail;

	// Token: 0x04000607 RID: 1543
	public bool noHarpoonHook;

	// Token: 0x04000608 RID: 1544
	[SerializeField]
	private ITinkResponder.TinkFlags tinkProperties;

	// Token: 0x04000609 RID: 1545
	[SerializeField]
	private ITinkResponder.TinkFlags ignoreResponders;

	// Token: 0x0400060A RID: 1546
	[Obsolete]
	[HideInInspector]
	public bool onlyReactToDown;

	// Token: 0x0400060B RID: 1547
	[SerializeField]
	[EnumPickerBitmask(typeof(HitInstance.HitDirection))]
	private int directionMask = -1;

	// Token: 0x0400060C RID: 1548
	[SerializeField]
	private bool checkSlashPosition;

	// Token: 0x0400060D RID: 1549
	[SerializeField]
	private bool activeOnAnyPlane;

	// Token: 0x0400060E RID: 1550
	[SerializeField]
	[EnumPickerBitmask(typeof(HitInstance.HitDirection))]
	private int preventDamageDirection;

	// Token: 0x0400060F RID: 1551
	[SerializeField]
	private List<HealthManager> preventDamageHealthManagers = new List<HealthManager>();

	// Token: 0x04000610 RID: 1552
	[Space]
	public UnityEvent OnTinked;

	// Token: 0x04000611 RID: 1553
	[Space]
	public UnityEvent OnTinkedHeavy;

	// Token: 0x04000612 RID: 1554
	[Space]
	public UnityEvent OnTinkedUp;

	// Token: 0x04000613 RID: 1555
	public UnityEvent OnTinkedDown;

	// Token: 0x04000614 RID: 1556
	public UnityEvent OnTinkedLeft;

	// Token: 0x04000615 RID: 1557
	public UnityEvent OnTinkedRight;

	// Token: 0x04000616 RID: 1558
	private Collider2D collider;

	// Token: 0x04000617 RID: 1559
	private bool hasCollider;

	// Token: 0x04000618 RID: 1560
	private DamageHero heroDamager;

	// Token: 0x04000619 RID: 1561
	private GameCameras gameCam;

	// Token: 0x0400061A RID: 1562
	private IHitResponderOverride overrideResponder;

	// Token: 0x0400061B RID: 1563
	private const float REPEAT_DELAY = 0.01f;

	// Token: 0x0400061C RID: 1564
	private static double _nextTinkTime;

	// Token: 0x02001433 RID: 5171
	// (Invoke) Token: 0x060082E5 RID: 33509
	public delegate void TinkEffectSpawnEvent(Vector3 position, Quaternion rotation);
}
