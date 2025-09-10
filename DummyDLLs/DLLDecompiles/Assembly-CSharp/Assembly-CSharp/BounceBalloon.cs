using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000498 RID: 1176
public class BounceBalloon : UnlockablePropBase, IHitResponder, ICancellable
{
	// Token: 0x06002A7B RID: 10875 RVA: 0x000B8110 File Offset: 0x000B6310
	private void Awake()
	{
		this.ambientFloat = base.GetComponentInChildren<AmbientFloat>();
		this.collider = base.GetComponent<Collider2D>();
		this.harpoonHook = base.GetComponent<HarpoonHook>();
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
				HitInstance damageInstance = this.hookQueuedHit;
				this.hookQueuedHit = default(HitInstance);
				damageInstance.Direction = 270f;
				damageInstance.IsHarpoon = true;
				this.Hit(damageInstance);
			});
			this.harpoonHook.OnHookCancel.AddListener(delegate()
			{
				this.isHarpoonHooked = false;
				this.hookQueuedHit = default(HitInstance);
			});
		}
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000B81A4 File Offset: 0x000B63A4
	private void Start()
	{
		GameManager instance = GameManager.instance;
		this.inputHandler = instance.GetComponent<InputHandler>();
		if (this.isInflated)
		{
			this.Opened();
			return;
		}
		this.SetDeflated();
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000B81D8 File Offset: 0x000B63D8
	private void OnDestroy()
	{
		if (BounceBalloon.activeBalloon == this)
		{
			BounceBalloon.activeBalloon = null;
		}
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000B81F0 File Offset: 0x000B63F0
	private void Update()
	{
		if (this.cullFramesLeft > 0)
		{
			this.cullFramesLeft--;
			if (this.cullFramesLeft == 0 && this.animator)
			{
				this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
			}
		}
		if (this.inputHandler.inputActions.Attack.WasPressed)
		{
			this.attackCancelled = true;
		}
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x000B8254 File Offset: 0x000B6454
	private void SpawnImpactEffect(HitInstance damageInstance)
	{
		if (!this.impactEffectPrefab)
		{
			return;
		}
		float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
		this.impactEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, true, false, false) + 180f);
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x000B82AD File Offset: 0x000B64AD
	private void SpawnSlashEffect()
	{
		if (!this.slashEffectPrefab)
		{
			return;
		}
		this.slashEffectPrefab.Spawn(base.transform.position);
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000B82D4 File Offset: 0x000B64D4
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!this.isInflated)
		{
			return IHitResponder.Response.None;
		}
		if (this.animator)
		{
			this.animator.Play(BounceBalloon._bounceAnim, 0, 0f);
		}
		this.bounceSound.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.hitParticles)
		{
			this.hitParticles.PlayParticles();
		}
		if (damageInstance.AttackType != AttackTypes.Nail)
		{
			this.SpawnImpactEffect(damageInstance);
			return IHitResponder.Response.GenericHit;
		}
		if (this.isHarpoonHooked)
		{
			this.hookQueuedHit = damageInstance;
			return IHitResponder.Response.None;
		}
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = source ? source.GetComponent<DamageEnemies>() : null;
		bool flag = damageEnemies == null || !damageEnemies.DidHitEnemy;
		HeroController instance = HeroController.instance;
		switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			if (flag)
			{
				instance.RecoilRightLong();
			}
			this.sideHitShake.DoShake(this, true);
			this.SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Right:
			if (flag)
			{
				instance.RecoilLeftLong();
			}
			this.sideHitShake.DoShake(this, true);
			this.SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				instance.RecoilDown();
			}
			this.sideHitShake.DoShake(this, true);
			this.SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		default:
			this.impactShake.DoShake(this, true);
			if (flag)
			{
				this.SpawnImpactEffect(damageInstance);
				VibrationManager.PlayVibrationClipOneShot(this.downBounceVibration, null, false, "", false);
				this.CancelBounce();
				if (BounceBalloon.activeBalloon != null)
				{
					BounceBalloon.activeBalloon.CancelBounce();
				}
				HeroUtility.AddCancellable(this);
				this.bounceRoutines.PushIfNotNull(base.StartCoroutine(this.Bounce(damageInstance)));
				return IHitResponder.Response.GenericHit;
			}
			this.SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		}
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000B84B8 File Offset: 0x000B66B8
	public void CancelBounce()
	{
		if (BounceBalloon.activeBalloon == this)
		{
			BounceBalloon.activeBalloon = null;
		}
		HeroUtility.RemoveCancellable(this);
		while (this.bounceRoutines.Count > 0)
		{
			base.StopCoroutine(this.bounceRoutines.Pop());
		}
		if (this.bouncePullStarted)
		{
			BounceShared.OnBouncePullInterrupted();
			this.bouncePullStarted = false;
		}
		if (this.onBounceEnd != null)
		{
			this.onBounceEnd();
			this.onBounceEnd = null;
		}
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000B852D File Offset: 0x000B672D
	private IEnumerator Bounce(HitInstance hit)
	{
		Transform transform = base.transform;
		HeroController hc = HeroController.instance;
		hc.crestAttacksFSM.SendEvent("BOUNCE CANCEL");
		hc.sprintFSM.SendEvent("BOUNCE CANCEL");
		hc.FinishDownspike();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		hc.RelinquishControl();
		this.bouncePullStarted = true;
		yield return this.bounceRoutines.PushIfNotNullReturn(base.StartCoroutine(BounceShared.BouncePull(transform, transform.TransformPoint(this.heroBouncePos), hc, hit)));
		this.bouncePullStarted = false;
		hc.RelinquishControl();
		int animationVersion = hc.StopAnimationControlVersioned();
		hc.AffectedByGravity(false);
		int controlVersion = HeroController.ControlVersion;
		this.bounceShake.DoShake(this, true);
		this.OnBounce.Invoke();
		Transform transform2 = this.bounceEffects.transform;
		Vector3 position = transform2.position;
		transform2.position = new Vector3(hc.transform.position.x, position.y, position.z);
		tk2dSpriteAnimationClip clip = hc.AnimCtrl.GetClip("Updraft Rise");
		hc.AnimCtrl.animator.PlayFromFrame(clip, 0);
		CameraTarget camTarget = GameCameras.instance.cameraTarget;
		camTarget.SetUpdraft(true);
		Rigidbody2D body = hc.GetComponent<Rigidbody2D>();
		body.linearVelocity = Vector2.zero;
		this.onBounceEnd = delegate()
		{
			hc.OnTakenDamage -= this.onBounceEnd;
			this.onBounceEnd = null;
			this.CancelBounce();
			camTarget.SetUpdraft(false);
			hc.ResetHardLandingTimer();
			hc.StartAnimationControl(animationVersion);
			hc.ResetAirMoves();
		};
		hc.OnTakenDamage += this.onBounceEnd;
		this.attackCancelled = false;
		float elapsed = 0f;
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		while (elapsed <= this.raiseDuration && !this.attackCancelled)
		{
			this.RaiseMovement(body);
			yield return wait;
			elapsed += Time.fixedDeltaTime;
		}
		if (this.attackCancelled)
		{
			hc.cState.downSpikeBouncing = false;
			hc.cState.downSpikeRecovery = false;
			hc.SetStartWithAttack();
		}
		else
		{
			hc.SetStartWithBalloonBounce();
		}
		this.onBounceEnd();
		if (controlVersion == HeroController.ControlVersion)
		{
			hc.RegainControl();
		}
		if (BounceBalloon.activeBalloon == this)
		{
			BounceBalloon.activeBalloon = null;
		}
		HeroUtility.RemoveCancellable(this);
		yield break;
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x000B8544 File Offset: 0x000B6744
	private void RaiseMovement(Rigidbody2D body)
	{
		Vector2 linearVelocity = body.linearVelocity;
		bool flag = false;
		if (this.inputHandler.inputActions.Right.IsPressed)
		{
			linearVelocity.x += this.xAccelerationFactor * Time.deltaTime;
			flag = true;
		}
		if (this.inputHandler.inputActions.Left.IsPressed)
		{
			linearVelocity.x -= this.xAccelerationFactor * Time.deltaTime;
			flag = true;
		}
		if (!flag)
		{
			if (linearVelocity.x > 0f)
			{
				linearVelocity.x -= this.xDecelerationFactor * Time.deltaTime;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			else if (linearVelocity.x < 0f)
			{
				linearVelocity.x += this.xDecelerationFactor * Time.deltaTime;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
		}
		linearVelocity.x = Mathf.Clamp(linearVelocity.x, -this.xMaxSpeed, this.xMaxSpeed);
		linearVelocity.y = this.raiseVelocity;
		body.linearVelocity = linearVelocity;
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x000B866C File Offset: 0x000B686C
	private IEnumerator Inflate()
	{
		this.inflateSound.SpawnAndPlayOneShot(base.transform.position, null);
		float duration;
		if (this.animator)
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.Play(BounceBalloon._inflateAnim);
			yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash == BounceBalloon._inflateAnim);
			duration = this.animator.GetCurrentAnimatorStateInfo(0).length;
		}
		else
		{
			duration = 0f;
		}
		if (this.ambientFloat)
		{
			for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
			{
				if (this.ambientFloat)
				{
					this.ambientFloat.SpeedMultiplier = elapsed / duration;
				}
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(duration);
		}
		this.isInflated = true;
		this.SetActive(true);
		if (this.animator)
		{
			this.animator.Play(BounceBalloon._inflatedAnim);
			this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		yield break;
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000B867B File Offset: 0x000B687B
	public override void Open()
	{
		if (this.inflateRoutine != null || this.isInflated)
		{
			return;
		}
		this.inflateRoutine = base.StartCoroutine(this.Inflate());
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x000B86A0 File Offset: 0x000B68A0
	public override void Opened()
	{
		this.isInflated = true;
		if (this.animator)
		{
			this.animator.Play(BounceBalloon._inflatedAnim);
		}
		this.SetActive(true);
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x000B86D0 File Offset: 0x000B68D0
	private void SetActive(bool value)
	{
		if (this.ambientFloat)
		{
			this.ambientFloat.SpeedMultiplier = (value ? 1f : 0f);
		}
		if (this.collider)
		{
			this.collider.enabled = value;
		}
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000B871D File Offset: 0x000B691D
	public void SetDeflated()
	{
		this.isInflated = false;
		this.inflateRoutine = null;
		if (this.animator)
		{
			this.animator.Play(BounceBalloon._deflatedAnim);
		}
		this.SetActive(false);
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000B8751 File Offset: 0x000B6951
	public void DoCancellation()
	{
		this.CancelBounce();
	}

	// Token: 0x04002B19 RID: 11033
	private static readonly int _bounceAnim = Animator.StringToHash("Bounce");

	// Token: 0x04002B1A RID: 11034
	private static readonly int _deflatedAnim = Animator.StringToHash("Deflated");

	// Token: 0x04002B1B RID: 11035
	private static readonly int _inflateAnim = Animator.StringToHash("Inflate");

	// Token: 0x04002B1C RID: 11036
	private static readonly int _inflatedAnim = Animator.StringToHash("Inflated");

	// Token: 0x04002B1D RID: 11037
	[SerializeField]
	private Animator animator;

	// Token: 0x04002B1E RID: 11038
	[SerializeField]
	private CameraShakeTarget sideHitShake;

	// Token: 0x04002B1F RID: 11039
	[SerializeField]
	private CameraShakeTarget impactShake;

	// Token: 0x04002B20 RID: 11040
	[SerializeField]
	private GameObject impactEffectPrefab;

	// Token: 0x04002B21 RID: 11041
	[SerializeField]
	private GameObject slashEffectPrefab;

	// Token: 0x04002B22 RID: 11042
	[SerializeField]
	private ParticleSystemPool hitParticles;

	// Token: 0x04002B23 RID: 11043
	[SerializeField]
	private Vector2 heroBouncePos;

	// Token: 0x04002B24 RID: 11044
	[SerializeField]
	private CameraShakeTarget bounceShake;

	// Token: 0x04002B25 RID: 11045
	[SerializeField]
	private AudioEventRandom inflateSound;

	// Token: 0x04002B26 RID: 11046
	[SerializeField]
	private AudioEventRandom bounceSound;

	// Token: 0x04002B27 RID: 11047
	[Space]
	[SerializeField]
	private GameObject bounceEffects;

	// Token: 0x04002B28 RID: 11048
	public UnityEvent OnBounce;

	// Token: 0x04002B29 RID: 11049
	[Space]
	[SerializeField]
	private float raiseVelocity = 18f;

	// Token: 0x04002B2A RID: 11050
	[SerializeField]
	private float raiseDuration = 0.5f;

	// Token: 0x04002B2B RID: 11051
	[SerializeField]
	private float xAccelerationFactor = 1f;

	// Token: 0x04002B2C RID: 11052
	[SerializeField]
	private float xDecelerationFactor = 2f;

	// Token: 0x04002B2D RID: 11053
	[SerializeField]
	private float xMaxSpeed = 4f;

	// Token: 0x04002B2E RID: 11054
	[Space]
	[SerializeField]
	private bool isInflated;

	// Token: 0x04002B2F RID: 11055
	[Space]
	[SerializeField]
	private VibrationDataAsset downBounceVibration;

	// Token: 0x04002B30 RID: 11056
	private bool attackCancelled;

	// Token: 0x04002B31 RID: 11057
	private int cullFramesLeft = 2;

	// Token: 0x04002B32 RID: 11058
	private Coroutine inflateRoutine;

	// Token: 0x04002B33 RID: 11059
	private InputHandler inputHandler;

	// Token: 0x04002B34 RID: 11060
	private AmbientFloat ambientFloat;

	// Token: 0x04002B35 RID: 11061
	private Collider2D collider;

	// Token: 0x04002B36 RID: 11062
	private HarpoonHook harpoonHook;

	// Token: 0x04002B37 RID: 11063
	private bool isHarpoonHooked;

	// Token: 0x04002B38 RID: 11064
	private HitInstance hookQueuedHit;

	// Token: 0x04002B39 RID: 11065
	private readonly Stack<Coroutine> bounceRoutines = new Stack<Coroutine>();

	// Token: 0x04002B3A RID: 11066
	private Action onBounceEnd;

	// Token: 0x04002B3B RID: 11067
	private static BounceBalloon activeBalloon;

	// Token: 0x04002B3C RID: 11068
	private bool bouncePullStarted;
}
