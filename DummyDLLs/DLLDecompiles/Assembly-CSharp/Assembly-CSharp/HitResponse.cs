using System;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000500 RID: 1280
public class HitResponse : HitResponseBase, IHitResponder, IBreakerBreakable
{
	// Token: 0x14000090 RID: 144
	// (add) Token: 0x06002DC2 RID: 11714 RVA: 0x000C82C4 File Offset: 0x000C64C4
	// (remove) Token: 0x06002DC3 RID: 11715 RVA: 0x000C82FC File Offset: 0x000C64FC
	public event Action<HitInstance.HitDirection> WasHitDirectional;

	// Token: 0x17000528 RID: 1320
	// (get) Token: 0x06002DC4 RID: 11716 RVA: 0x000C8331 File Offset: 0x000C6531
	public int HitPriority
	{
		get
		{
			return this.hitPriority;
		}
	}

	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x000C8339 File Offset: 0x000C6539
	public bool HitRecurseUpwards
	{
		get
		{
			return !this.blockHitRecurseUpwards;
		}
	}

	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x06002DC6 RID: 11718 RVA: 0x000C8344 File Offset: 0x000C6544
	// (set) Token: 0x06002DC7 RID: 11719 RVA: 0x000C834C File Offset: 0x000C654C
	public override bool IsActive
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x000C8358 File Offset: 0x000C6558
	private bool? IsFsmEventValid(string eventName)
	{
		if (!this.addFsmEventDir)
		{
			return this.fsmTarget.IsEventValid(eventName, false);
		}
		if (!this.fsmTarget)
		{
			return null;
		}
		if (string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		return new bool?(this.fsmTarget.IsEventValid(eventName + "LEFT") || this.fsmTarget.IsEventValid(eventName + "RIGHT") || this.fsmTarget.IsEventValid(eventName + "UP") || this.fsmTarget.IsEventValid(eventName + "DOWN"));
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x000C840D File Offset: 0x000C660D
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.hitEffectOffset, 0.2f);
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x000C8430 File Offset: 0x000C6630
	protected override void Awake()
	{
		base.Awake();
		if (!base.transform.IsOnHeroPlane())
		{
			base.enabled = false;
		}
		if (this.harpoonHook)
		{
			this.harpoonHook.OnHookStart.AddListener(delegate()
			{
				this.isHarpoonHooked = true;
			});
			this.harpoonHook.OnHookEnd.AddListener(delegate()
			{
				this.isHarpoonHooked = false;
				if (this.hookQueuedHit.Source == null)
				{
					return;
				}
				HitInstance hitInstance = this.hookQueuedHit;
				this.hookQueuedHit = default(HitInstance);
				if (hitInstance.MagnitudeMultiplier < 1f)
				{
					hitInstance.MagnitudeMultiplier = 1f;
				}
				this.Hit(hitInstance);
			});
			this.harpoonHook.OnHookCancel.AddListener(delegate()
			{
				this.isHarpoonHooked = false;
				this.hookQueuedHit = default(HitInstance);
			});
		}
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x000C84B8 File Offset: 0x000C66B8
	private void Start()
	{
		this.ResetCombo();
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x000C84C0 File Offset: 0x000C66C0
	private void Update()
	{
		if (this.currentHitCombo > 0)
		{
			this.hitComboTimer -= Time.deltaTime;
			if (this.hitComboTimer < 0f)
			{
				this.ResetCombo();
			}
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x000C84F0 File Offset: 0x000C66F0
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		if (this.onlyTakeHeroDamage && !damageInstance.IsHeroDamage)
		{
			return IHitResponder.Response.None;
		}
		if (this.firstHitOnly && !damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (!this.IsDamageInstanceAllowed(damageInstance))
		{
			return IHitResponder.Response.None;
		}
		if (this.ignoreMultiHit && damageInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.Full)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble - this.lastHitTime < 0.10000000149011612)
		{
			return IHitResponder.Response.None;
		}
		if (this.isHarpoonHooked)
		{
			this.hookQueuedHit = damageInstance;
			return IHitResponder.Response.None;
		}
		this.lastHitTime = Time.timeAsDouble;
		float num = damageInstance.MagnitudeMultiplier;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
		Vector3 lossyScale = base.transform.lossyScale;
		string text = this.fsmEvent;
		bool flag = false;
		float minInclusive;
		float maxInclusive;
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (this.ignoreLeft)
			{
				return IHitResponder.Response.None;
			}
			if (this.addFsmEventDir)
			{
				if (this.flipEventWithScale && lossyScale.x < 0f)
				{
					text += "RIGHT";
				}
				else
				{
					text += "LEFT";
				}
			}
			if (this.bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilRight();
			}
			minInclusive = 120f;
			maxInclusive = 160f;
			break;
		case HitInstance.HitDirection.Right:
			if (this.ignoreRight)
			{
				return IHitResponder.Response.None;
			}
			if (this.addFsmEventDir)
			{
				if (this.flipEventWithScale && lossyScale.x < 0f)
				{
					text += "LEFT";
				}
				else
				{
					text += "RIGHT";
				}
			}
			if (this.bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilLeft();
			}
			minInclusive = 30f;
			maxInclusive = 70f;
			break;
		case HitInstance.HitDirection.Up:
			if (this.ignoreUp)
			{
				return IHitResponder.Response.None;
			}
			if (this.addFsmEventDir)
			{
				if (this.flipEventWithScale && lossyScale.y < 0f)
				{
					text += "DOWN";
				}
				else
				{
					text += "UP";
				}
			}
			if (this.bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilDown();
			}
			minInclusive = 70f;
			maxInclusive = 110f;
			num *= 1.5f;
			break;
		case HitInstance.HitDirection.Down:
			if (this.ignoreDown)
			{
				return IHitResponder.Response.None;
			}
			flag = true;
			if (this.addFsmEventDir)
			{
				if (this.flipEventWithScale && lossyScale.y < 0f)
				{
					text += "UP";
				}
				else
				{
					text += "DOWN";
				}
			}
			minInclusive = 160f;
			maxInclusive = 380f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		Vector3 position = damageInstance.Source.transform.position;
		foreach (HitResponse.SimpleFling simpleFling in this.spawnFling)
		{
			if (simpleFling.Prefab)
			{
				Rigidbody2D component = simpleFling.Prefab.Spawn(base.transform.TransformPoint(this.hitEffectOffset)).GetComponent<Rigidbody2D>();
				if (!(component == null))
				{
					float num2 = Random.Range(minInclusive, maxInclusive);
					Vector2 a = new Vector2(Mathf.Cos(num2 * 0.017453292f), Mathf.Sin(num2 * 0.017453292f));
					MinMaxFloat flingSpeed = simpleFling.FlingSpeed;
					float d = flingSpeed.GetRandomValue() * num;
					component.linearVelocity = a * d;
				}
			}
		}
		if (this.positionSetFSM)
		{
			FsmVector2 fsmVector = this.positionSetFSM.FsmVariables.FindFsmVector2("Hit Source Position");
			if (fsmVector != null)
			{
				fsmVector.Value = position;
			}
		}
		this.hitShake.DoShake(this, true);
		if (this.hitEffect && (!this.hitEffectTimer || this.hitEffectTimer.HasEnded))
		{
			Vector3 position2 = base.transform.position;
			Vector3 vector = new Vector3(this.positionHitX ? position.x : position2.x, this.positionHitY ? position.y : position2.y, this.hitEffect.transform.position.z);
			if (this.hitEffectOffset != Vector3.zero)
			{
				vector += base.transform.TransformVector(this.hitEffectOffset);
			}
			GameObject gameObject = this.hitEffect.Spawn(vector);
			if (this.angleEffectForDownSpike && flag)
			{
				gameObject.transform.SetRotation2D(180f - damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
			}
			if (this.hitEffectTimer)
			{
				this.hitEffectTimer.ResetTimer();
			}
		}
		base.SendHitInDirection(damageInstance.Source, hitDirection);
		if (this.hitCombo.End > 0)
		{
			this.hitComboTimer = this.hitComboCoolown;
			this.currentHitCombo++;
			if (this.currentHitCombo >= this.targetHitCombo)
			{
				this.ResetCombo();
				this.ComboResponse();
				return IHitResponder.Response.GenericHit;
			}
		}
		if (this.fsmTarget && !string.IsNullOrEmpty(text))
		{
			if (this.enableFsmOnSend)
			{
				this.fsmTarget.enabled = true;
			}
			this.fsmTarget.SendEvent(text);
		}
		if (this.OnHit != null)
		{
			this.OnHit.Invoke();
		}
		if (this.WasHitDirectional != null)
		{
			this.WasHitDirectional(hitDirection);
		}
		if (this.passHit)
		{
			if (this.setOriginOnPassHit)
			{
				this.passHit.ReceiveHitEffect(damageInstance, this.passHit.transform.InverseTransformPoint(position));
			}
			else
			{
				this.passHit.ReceiveHitEffect(damageInstance);
			}
		}
		if (this.passHitToBreakable)
		{
			this.passHitToBreakable.Hit(damageInstance);
		}
		if (this.passRecoil)
		{
			this.passRecoil.RecoilByDamage(damageInstance);
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x000C8AEE File Offset: 0x000C6CEE
	private bool IsDamageInstanceAllowed(HitInstance damageInstance)
	{
		return (this.unconditionallyAllowedTypes & damageInstance.SpecialType) != SpecialTypes.None || this.allowedAttackTypes.IsBitSet((int)damageInstance.AttackType);
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x000C8B12 File Offset: 0x000C6D12
	public void ResetCombo()
	{
		this.targetHitCombo = this.hitCombo.GetRandomValue(true);
		this.currentHitCombo = 0;
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x000C8B30 File Offset: 0x000C6D30
	private void ComboResponse()
	{
		if (this.fsmTarget && !string.IsNullOrEmpty(this.comboFsmEvent))
		{
			if (this.enableFsmOnSend)
			{
				this.fsmTarget.enabled = true;
			}
			this.fsmTarget.SendEvent(this.comboFsmEvent);
		}
		if (this.OnCombo != null)
		{
			this.OnCombo.Invoke();
		}
	}

	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x06002DD1 RID: 11729 RVA: 0x000C8B8F File Offset: 0x000C6D8F
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			if (!this.passHitToBreakable)
			{
				return BreakableBreaker.BreakableTypes.Basic;
			}
			return this.passHitToBreakable.BreakableType;
		}
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x000C8BAB File Offset: 0x000C6DAB
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if (this.passHitToBreakable)
		{
			this.passHitToBreakable.HitFromBreaker(breaker);
		}
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x000C8BC6 File Offset: 0x000C6DC6
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if (this.passHitToBreakable)
		{
			this.passHitToBreakable.BreakFromBreaker(breaker);
		}
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x000C8BF0 File Offset: 0x000C6DF0
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04002FB7 RID: 12215
	[SerializeField]
	private bool ignoreLeft;

	// Token: 0x04002FB8 RID: 12216
	[SerializeField]
	private bool ignoreRight;

	// Token: 0x04002FB9 RID: 12217
	[SerializeField]
	private bool ignoreUp;

	// Token: 0x04002FBA RID: 12218
	[SerializeField]
	private bool ignoreDown;

	// Token: 0x04002FBB RID: 12219
	[SerializeField]
	private bool ignoreMultiHit;

	// Token: 0x04002FBC RID: 12220
	[SerializeField]
	private bool onlyTakeHeroDamage;

	// Token: 0x04002FBD RID: 12221
	[SerializeField]
	private bool firstHitOnly;

	// Token: 0x04002FBE RID: 12222
	[SerializeField]
	private bool blockHitRecurseUpwards;

	// Token: 0x04002FBF RID: 12223
	[SerializeField]
	private int hitPriority;

	// Token: 0x04002FC0 RID: 12224
	[Space]
	[SerializeField]
	private SpecialTypes unconditionallyAllowedTypes;

	// Token: 0x04002FC1 RID: 12225
	[SerializeField]
	[EnumPickerBitmask(typeof(AttackTypes))]
	private int allowedAttackTypes = -1;

	// Token: 0x04002FC2 RID: 12226
	[Space]
	[SerializeField]
	private CameraShakeTarget hitShake;

	// Token: 0x04002FC3 RID: 12227
	[SerializeField]
	private GameObject hitEffect;

	// Token: 0x04002FC4 RID: 12228
	[SerializeField]
	private Vector3 hitEffectOffset;

	// Token: 0x04002FC5 RID: 12229
	[SerializeField]
	private TimerGroup hitEffectTimer;

	// Token: 0x04002FC6 RID: 12230
	[SerializeField]
	private bool positionHitX;

	// Token: 0x04002FC7 RID: 12231
	[SerializeField]
	private bool positionHitY;

	// Token: 0x04002FC8 RID: 12232
	[SerializeField]
	private bool angleEffectForDownSpike;

	// Token: 0x04002FC9 RID: 12233
	[SerializeField]
	private PlayMakerFSM positionSetFSM;

	// Token: 0x04002FCA RID: 12234
	[SerializeField]
	private EnemyHitEffectsRegular passHit;

	// Token: 0x04002FCB RID: 12235
	[SerializeField]
	private bool setOriginOnPassHit;

	// Token: 0x04002FCC RID: 12236
	[SerializeField]
	private Breakable passHitToBreakable;

	// Token: 0x04002FCD RID: 12237
	[SerializeField]
	private Recoil passRecoil;

	// Token: 0x04002FCE RID: 12238
	[SerializeField]
	private bool bounceRecoil;

	// Token: 0x04002FCF RID: 12239
	[SerializeField]
	private HarpoonHook harpoonHook;

	// Token: 0x04002FD0 RID: 12240
	[Space]
	[SerializeField]
	private HitResponse.SimpleFling[] spawnFling;

	// Token: 0x04002FD1 RID: 12241
	[Space]
	[SerializeField]
	private MinMaxInt hitCombo;

	// Token: 0x04002FD2 RID: 12242
	[SerializeField]
	private float hitComboCoolown;

	// Token: 0x04002FD3 RID: 12243
	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	// Token: 0x04002FD4 RID: 12244
	[SerializeField]
	private bool enableFsmOnSend;

	// Token: 0x04002FD5 RID: 12245
	[SerializeField]
	private bool addFsmEventDir;

	// Token: 0x04002FD6 RID: 12246
	[SerializeField]
	private bool flipEventWithScale;

	// Token: 0x04002FD7 RID: 12247
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	private string fsmEvent;

	// Token: 0x04002FD8 RID: 12248
	public UnityEvent OnHit;

	// Token: 0x04002FD9 RID: 12249
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	private string comboFsmEvent;

	// Token: 0x04002FDA RID: 12250
	public UnityEvent OnCombo;

	// Token: 0x04002FDB RID: 12251
	private bool isHarpoonHooked;

	// Token: 0x04002FDC RID: 12252
	private HitInstance hookQueuedHit;

	// Token: 0x04002FDD RID: 12253
	private int targetHitCombo;

	// Token: 0x04002FDE RID: 12254
	private int currentHitCombo;

	// Token: 0x04002FDF RID: 12255
	private float hitComboTimer;

	// Token: 0x04002FE0 RID: 12256
	private double lastHitTime;

	// Token: 0x02001802 RID: 6146
	[Serializable]
	private struct SimpleFling
	{
		// Token: 0x04009059 RID: 36953
		public GameObject Prefab;

		// Token: 0x0400905A RID: 36954
		public MinMaxFloat FlingSpeed;
	}
}
