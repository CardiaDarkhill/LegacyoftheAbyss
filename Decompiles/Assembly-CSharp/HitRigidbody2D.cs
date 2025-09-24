using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000502 RID: 1282
public class HitRigidbody2D : MonoBehaviour, IHitResponder
{
	// Token: 0x14000091 RID: 145
	// (add) Token: 0x06002DDD RID: 11741 RVA: 0x000C8D2C File Offset: 0x000C6F2C
	// (remove) Token: 0x06002DDE RID: 11742 RVA: 0x000C8D64 File Offset: 0x000C6F64
	public event Action<HitInstance> WasHitBy;

	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x06002DDF RID: 11743 RVA: 0x000C8D99 File Offset: 0x000C6F99
	// (set) Token: 0x06002DE0 RID: 11744 RVA: 0x000C8DA1 File Offset: 0x000C6FA1
	public bool WasHit { get; private set; }

	// Token: 0x06002DE1 RID: 11745 RVA: 0x000C8DAA File Offset: 0x000C6FAA
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.origin, 0.2f);
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x000C8DCC File Offset: 0x000C6FCC
	private void Awake()
	{
		if (!(this.groupRoot ? this.groupRoot : base.transform).IsOnHeroPlane())
		{
			Collider2D component = base.GetComponent<Collider2D>();
			if (component)
			{
				component.enabled = false;
			}
			return;
		}
		if (this.groupRoot)
		{
			this.groupBodies = (from b in this.groupRoot.GetComponentsInChildren<HitRigidbody2D>(true)
			where b != this
			select b).ToArray<HitRigidbody2D>();
		}
		this.hitEffects = base.GetComponents<IHitEffectReciever>();
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x000C8E53 File Offset: 0x000C7053
	private void OnEnable()
	{
		this.enabledTime = Time.timeAsDouble;
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x000C8E60 File Offset: 0x000C7060
	private void OnDisable()
	{
		this.WasHit = false;
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x000C8E69 File Offset: 0x000C7069
	private void FixedUpdate()
	{
		this.WasHit = false;
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x000C8E74 File Offset: 0x000C7074
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.AttackType == AttackTypes.Spikes)
		{
			return IHitResponder.Response.None;
		}
		if ((float)(Time.timeAsDouble - this.enabledTime) < 0.3f)
		{
			return IHitResponder.Response.GenericHit;
		}
		if (!this.body)
		{
			return IHitResponder.Response.None;
		}
		if (!this.alwaysRespondToHit && this.groupBodies != null)
		{
			HitRigidbody2D[] array = this.groupBodies;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].WasHit)
				{
					return IHitResponder.Response.None;
				}
			}
		}
		this.WasHit = true;
		Vector2 vector = base.transform.TransformPoint(this.origin);
		Vector2 zero = Vector2.zero;
		Rigidbody2D rigidbody2D;
		Vector2 a;
		if (this.body.gameObject.activeInHierarchy)
		{
			rigidbody2D = this.body;
			a = this.force;
		}
		else
		{
			rigidbody2D = this.breakReactBody;
			a = this.breakReactForce;
		}
		switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			zero.x = -1f;
			if (this.recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilRight();
				goto IL_1A0;
			}
			goto IL_1A0;
		case HitInstance.HitDirection.Right:
			zero.x = 1f;
			if (this.recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilLeft();
				goto IL_1A0;
			}
			goto IL_1A0;
		case HitInstance.HitDirection.Up:
			if (this.recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilDown();
			}
			break;
		case HitInstance.HitDirection.Down:
			break;
		default:
			goto IL_1A0;
		}
		zero.x = (float)(HeroController.instance.cState.facingRight ? 1 : -1);
		zero.y = 1f;
		IL_1A0:
		if (this.useHitDirectionX)
		{
			a.x *= zero.x;
		}
		if (this.useHitDirectionY)
		{
			a.y *= zero.y;
		}
		this.cameraShake.DoShake(this, true);
		if (this.hitEffect)
		{
			GameObject gameObject = this.hitEffect.Spawn();
			gameObject.transform.SetPosition2D(vector);
			gameObject.transform.localScale *= this.hitEffectScale;
		}
		IHitEffectReciever[] array2 = this.hitEffects;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].ReceiveHitEffect(damageInstance);
		}
		if (this.passHitToBreakable)
		{
			this.passHitToBreakable.Hit(damageInstance);
		}
		if (!this.dontPlayHitOnBreak || !this.passHitToBreakable || !this.passHitToBreakable.IsBroken)
		{
			this.hitAudio.SpawnAndPlayOneShot(vector, null);
		}
		if (!string.IsNullOrEmpty(this.hitEventRegister))
		{
			EventRegister.SendEvent(this.hitEventRegister, null);
		}
		this.OnHit.Invoke();
		if (this.WasHitBy != null)
		{
			this.WasHitBy(damageInstance);
		}
		a *= this.magnitudeMultiplierCurve.Evaluate(damageInstance.MagnitudeMultiplier);
		if (rigidbody2D)
		{
			if (this.resetVelocity)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
			}
			rigidbody2D.AddForceAtPosition(a, vector, ForceMode2D.Impulse);
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x04002FE6 RID: 12262
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04002FE7 RID: 12263
	[SerializeField]
	private Transform groupRoot;

	// Token: 0x04002FE8 RID: 12264
	[SerializeField]
	private bool alwaysRespondToHit;

	// Token: 0x04002FE9 RID: 12265
	[SerializeField]
	private Vector3 origin;

	// Token: 0x04002FEA RID: 12266
	[SerializeField]
	private Vector2 force;

	// Token: 0x04002FEB RID: 12267
	[SerializeField]
	private bool recoilHero;

	// Token: 0x04002FEC RID: 12268
	[SerializeField]
	private bool resetVelocity;

	// Token: 0x04002FED RID: 12269
	[SerializeField]
	private bool useHitDirectionX;

	// Token: 0x04002FEE RID: 12270
	[SerializeField]
	private bool useHitDirectionY;

	// Token: 0x04002FEF RID: 12271
	[SerializeField]
	private AnimationCurve magnitudeMultiplierCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04002FF0 RID: 12272
	[SerializeField]
	private CameraShakeTarget cameraShake;

	// Token: 0x04002FF1 RID: 12273
	[SerializeField]
	private GameObject hitEffect;

	// Token: 0x04002FF2 RID: 12274
	[SerializeField]
	private float hitEffectScale = 1f;

	// Token: 0x04002FF3 RID: 12275
	[SerializeField]
	private AudioEventRandom hitAudio;

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private bool dontPlayHitOnBreak;

	// Token: 0x04002FF5 RID: 12277
	[Space]
	[SerializeField]
	private Breakable passHitToBreakable;

	// Token: 0x04002FF6 RID: 12278
	[SerializeField]
	private Rigidbody2D breakReactBody;

	// Token: 0x04002FF7 RID: 12279
	[SerializeField]
	private Vector2 breakReactForce;

	// Token: 0x04002FF8 RID: 12280
	[Space]
	[SerializeField]
	private string hitEventRegister;

	// Token: 0x04002FF9 RID: 12281
	private double enabledTime;

	// Token: 0x04002FFA RID: 12282
	public UnityEvent OnHit;

	// Token: 0x04002FFC RID: 12284
	private HitRigidbody2D[] groupBodies;

	// Token: 0x04002FFD RID: 12285
	private IHitEffectReciever[] hitEffects;
}
