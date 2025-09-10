using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class SlideSurface : MonoBehaviour
{
	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x060030BF RID: 12479 RVA: 0x000D7604 File Offset: 0x000D5804
	public static bool IsHeroInside
	{
		get
		{
			return SlideSurface._heroInsideCount > 0;
		}
	}

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x060030C0 RID: 12480 RVA: 0x000D760E File Offset: 0x000D580E
	// (set) Token: 0x060030C1 RID: 12481 RVA: 0x000D7615 File Offset: 0x000D5815
	public static bool IsHeroSliding { get; private set; }

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x060030C2 RID: 12482 RVA: 0x000D7620 File Offset: 0x000D5820
	public static bool IsInJumpGracePeriod
	{
		get
		{
			using (List<SlideSurface>.Enumerator enumerator = SlideSurface._slideSurfaces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.jumpGraceTimeLeft > 0f)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x000D7680 File Offset: 0x000D5880
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<ParticleSystem>(ref this.slideParticles, typeof(SlideSurface.SlideSpeeds));
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x000D7698 File Offset: 0x000D5898
	private void Awake()
	{
		this.OnValidate();
		ParticleSystem[] array = this.slideParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		this.selfCollider = base.GetComponent<Collider2D>();
	}

	// Token: 0x060030C5 RID: 12485 RVA: 0x000D76D6 File Offset: 0x000D58D6
	private void OnEnable()
	{
		SlideSurface._slideSurfaces.AddIfNotPresent(this);
	}

	// Token: 0x060030C6 RID: 12486 RVA: 0x000D76E4 File Offset: 0x000D58E4
	private void Start()
	{
		this.ih = ManagerSingleton<InputHandler>.Instance;
		this.cameraTarget = GameCameras.instance.cameraTarget;
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x000D7701 File Offset: 0x000D5901
	private void OnDisable()
	{
		SlideSurface._slideSurfaces.Remove(this);
		if (this.didSetEnviroOverride)
		{
			if (this.enviroListener != null)
			{
				this.enviroListener.ClearOverride();
			}
			this.didSetEnviroOverride = false;
		}
	}

	// Token: 0x060030C8 RID: 12488 RVA: 0x000D7737 File Offset: 0x000D5937
	private void OnDestroy()
	{
		if (this.isHeroInside)
		{
			this.isHeroInside = false;
			SlideSurface._heroInsideCount--;
		}
	}

	// Token: 0x060030C9 RID: 12489 RVA: 0x000D7754 File Offset: 0x000D5954
	private void Update()
	{
		bool flag = false;
		if (!this.isHeroAttached)
		{
			if (this.enviroListener)
			{
				this.didSetEnviroOverride = false;
				this.enviroListener.ClearOverride();
				this.enviroListener = null;
			}
			if (this.jumpGraceTimeLeft <= 0f)
			{
				return;
			}
			flag = true;
			this.jumpGraceTimeLeft -= Time.deltaTime;
		}
		if (!flag && this.forcedSlideTimeLeft > 0f)
		{
			this.forcedSlideTimeLeft -= Time.deltaTime;
		}
		else if (this.ih.GetWasButtonPressedQueued(HeroActionButton.JUMP, true))
		{
			this.JumpOff();
			return;
		}
		if (flag)
		{
			return;
		}
		if (this.forcedRegularSlideTimeLeft > 0f)
		{
			this.forcedRegularSlideTimeLeft -= Time.deltaTime;
			this.inputDirection = 0f;
		}
		else
		{
			this.inputDirection = this.ih.inputActions.MoveVector.X;
		}
		this.UpdateFollowPositions();
		if (Math.Abs(this.inputDirection) <= Mathf.Epsilon)
		{
			this.slopeSpeedMultiplier = 1f;
			if (Math.Abs(this.inputAccelerationDirection) > Mathf.Epsilon)
			{
				this.animator.Play((this.inputAccelerationDirection > 0f) ? "Slide Fast End" : "Slide Brake End");
				this.UpdateSlideEffects(SlideSurface.SlideSpeeds.Regular);
			}
			this.inputAccelerationDirection = 0f;
			return;
		}
		this.inputAccelerationDirection = this.inputDirection * this.slopeNormal.x;
		if (this.inputAccelerationDirection > 0f)
		{
			this.slopeSpeedMultiplier = 1.5f;
			this.animator.Play("Slide Fast");
			this.UpdateSlideEffects(SlideSurface.SlideSpeeds.Fast);
			return;
		}
		this.slopeSpeedMultiplier = 0.7f;
		this.animator.Play("Slide Brake");
		this.UpdateSlideEffects(SlideSurface.SlideSpeeds.Brake);
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x000D7910 File Offset: 0x000D5B10
	private void FixedUpdate()
	{
		if (!this.isHeroAttached)
		{
			return;
		}
		float b = Mathf.Lerp(this.steepSlideSpeed, this.shallowSlideSpeed, this.slopeDot) * this.slopeSpeedMultiplier;
		float num = Mathf.Lerp(this.steepAcceleration, this.shallowAcceleration, this.slopeDot);
		this.currentSlideSpeed = Mathf.Lerp(this.currentSlideSpeed, b, num * Time.deltaTime);
		Vector2 vector = new Vector2(Mathf.Sign(this.slopeNormal.x) * this.currentSlideSpeed, 0f) * this.slopeDot;
		this.body.linearVelocity = vector;
		Vector2 vector2 = this.body.position + vector * Time.deltaTime;
		Vector2 position;
		if (!this.TryPositionOnSlope(vector2, out position))
		{
			this.Detach(true);
			this.hc.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
			{
				Velocity = new Vector2(15f * Mathf.Sign(this.slopeNormal.x), 0f),
				Decay = 2.5f
			});
			return;
		}
		this.body.MovePosition(position);
		this.UpdateFacing();
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x000D7A3C File Offset: 0x000D5C3C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.isHeroInside)
		{
			this.isHeroInside = true;
			SlideSurface._heroInsideCount++;
		}
		if (this.isHeroAttached)
		{
			return;
		}
		this.hc = collision.GetComponent<HeroController>();
		if (!this.hc)
		{
			return;
		}
		this.body = this.hc.GetComponent<Rigidbody2D>();
		Vector2 position = this.body.position;
		Vector2 vector = this.selfCollider.ClosestPoint(position);
		this.wasAbove = (position.y >= vector.y);
		this.OnTriggerStay2D(collision);
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x000D7AD0 File Offset: 0x000D5CD0
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (this.isHeroAttached)
		{
			return;
		}
		if (this.body.linearVelocity.y > 0f && this.body.gravityScale > 0f)
		{
			Vector2 position = this.body.position;
			Vector2 vector = this.selfCollider.ClosestPoint(position);
			if (position.y - 0.25f >= vector.y)
			{
				this.wasAbove = true;
				return;
			}
			if (!this.wasAbove)
			{
				return;
			}
		}
		if (this.hc.cState.mantling)
		{
			return;
		}
		this.heroCollider = this.hc.GetComponent<BoxCollider2D>();
		this.heroFeetDistance = this.heroCollider.size.y * 0.5f - this.heroCollider.offset.y;
		Vector2 position2 = this.body.position;
		Vector2 position3;
		if (!this.TryPositionOnSlope(position2, out position3))
		{
			return;
		}
		this.body.position = position3;
		this.isHeroAttached = true;
		this.endedOnGround = false;
		this.enviroListener = this.hc.GetComponent<EnviroRegionListener>();
		this.enviroListener.SetOverride(EnvironmentTypes.PeakPuff);
		this.didSetEnviroOverride = true;
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		this.hc.ResetAirMoves();
		this.hc.RelinquishControl();
		this.hc.StopAnimationControl();
		this.hc.AffectedByGravity(false);
		this.hc.SetAllowNailChargingWhileRelinquished(false);
		this.heroCollider.enabled = false;
		if (this.cameraLeading)
		{
			this.cameraTarget.SetSliding(true);
		}
		SlideSurface.IsHeroSliding = true;
		this.body.transform.SetPosition2D(this.body.position);
		this.UpdateFacing();
		this.animator = this.hc.GetComponent<tk2dSpriteAnimator>();
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		this.animator.Play("Slide Start");
		this.forcedSlideTimeLeft = 0.15f;
		this.forcedRegularSlideTimeLeft = 0.3f;
		this.currentSlideSpeed = this.initialSlideSpeed;
		this.UpdateFollowPositions();
		this.UpdateSlideEffects(SlideSurface.SlideSpeeds.Regular);
		this.slideStartShake.DoShake(this, true);
		Vector3 position4 = this.body.transform.position;
		this.enterAudio.SpawnAndPlayOneShot(position4, null);
		if (this.followLoopSource)
		{
			this.followLoopSource.Play();
		}
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x000D7D3D File Offset: 0x000D5F3D
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.HeroNotInside();
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x000D7D45 File Offset: 0x000D5F45
	private void HeroNotInside()
	{
		if (!this.isHeroInside)
		{
			return;
		}
		this.isHeroInside = false;
		SlideSurface._heroInsideCount--;
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x000D7D63 File Offset: 0x000D5F63
	private bool TryPositionOnSlope(in Vector2 heroPos, out Vector2 newHeroPos)
	{
		return this.TryPositionOnSlope(heroPos, out newHeroPos, false) || (this.isHeroAttached && this.TryPositionOnSlope(heroPos, out newHeroPos, true));
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x000D7D8C File Offset: 0x000D5F8C
	private bool TryPositionOnSlope(in Vector2 heroPos, out Vector2 newHeroPos, bool isBackRay)
	{
		Vector2 vector = heroPos;
		float num = this.heroCollider.size.y * 0.5f + this.heroCollider.offset.y;
		vector.y += num;
		float num2 = 0f;
		if (isBackRay)
		{
			float num3 = Mathf.Sign(this.slopeNormal.x);
			float num4 = this.heroCollider.size.x * 0.5f + 0.001f;
			vector.x += -num4 * num3;
			Vector2 vector2 = new Vector2(this.slopeNormal.y, -this.slopeNormal.x);
			num2 = -(vector2 * (num4 * (1f / Mathf.Abs(vector2.x)))).y;
		}
		Vector2 down = Vector2.down;
		float distance = num + this.heroFeetDistance + 0.7f;
		int num5 = Physics2D.RaycastNonAlloc(vector, down, this.storeHits, distance, 8448);
		if (num5 > 10)
		{
			num5 = 10;
		}
		bool flag = false;
		bool flag2 = false;
		RaycastHit2D raycastHit2D = default(RaycastHit2D);
		float num6 = float.MaxValue;
		int i = 0;
		while (i < num5)
		{
			RaycastHit2D raycastHit2D2 = this.storeHits[i];
			if (raycastHit2D2.collider.gameObject.layer == 13)
			{
				if (raycastHit2D2.collider.isTrigger)
				{
					SlideSurface component = raycastHit2D2.collider.GetComponent<SlideSurface>();
					if (component && !(component != this))
					{
						bool flag3 = true;
						goto IL_185;
					}
				}
			}
			else if (!raycastHit2D2.collider.isTrigger)
			{
				bool flag3 = false;
				goto IL_185;
			}
			IL_1B3:
			i++;
			continue;
			IL_185:
			float num7 = vector.y - raycastHit2D2.point.y;
			if (num7 < num6)
			{
				raycastHit2D = raycastHit2D2;
				num6 = num7;
				bool flag3;
				flag = flag3;
				flag2 = !flag3;
				goto IL_1B3;
			}
			goto IL_1B3;
		}
		if (!flag)
		{
			this.endedOnGround = flag2;
			newHeroPos = heroPos;
			return false;
		}
		float num8 = raycastHit2D.distance - this.heroFeetDistance + num2;
		Vector2 vector3 = heroPos;
		vector3.y -= num8;
		newHeroPos = vector3;
		this.slopeNormal = raycastHit2D.normal;
		this.slopeDot = Mathf.Abs(Vector2.Dot(this.slopeNormal, Vector2.up));
		return true;
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x000D7FD0 File Offset: 0x000D61D0
	private void Detach(bool allowQueueing)
	{
		this.isHeroAttached = false;
		this.HeroNotInside();
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		this.hc.RegainControl();
		this.hc.StartAnimationControl();
		this.hc.AffectedByGravity(true);
		this.heroCollider.enabled = true;
		this.cameraTarget.SetSliding(false);
		SlideSurface.IsHeroSliding = false;
		if (this.endedOnGround)
		{
			this.hc.DoSprintSkid();
		}
		this.UpdateSlideEffects(SlideSurface.SlideSpeeds.None);
		this.exitAudio.SpawnAndPlayOneShot(this.body.transform.position, null);
		if (this.followLoopSource)
		{
			this.followLoopSource.Stop();
		}
		this.jumpGraceTimeLeft = (allowQueueing ? 0.1f : 0f);
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x000D80B8 File Offset: 0x000D62B8
	private void JumpOff()
	{
		if (!this.isHeroAttached)
		{
			if (!this.hc.CanTakeControl())
			{
				return;
			}
			EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
			this.hc.RelinquishControl();
			this.hc.StopAnimationControl();
			this.hc.AffectedByGravity(false);
			this.hc.ResetInputQueues();
		}
		this.animator.Play("Airborne");
		float num = this.ih.inputActions.MoveVector.X;
		if (num == 0f)
		{
			num = Mathf.Sign(this.slopeNormal.x);
		}
		float num2;
		float decay;
		if (num * this.slopeNormal.x < 0f)
		{
			num2 = 15f;
			decay = 2.5f;
			this.hc.SetStartWithTinyJump();
			this.hc.SetDoFullJump();
		}
		else
		{
			num2 = 18f;
			decay = 3.5f;
			this.hc.SetStartWithDownSpikeBounce();
		}
		this.body.linearVelocity = new Vector2(0f, 10f);
		this.Detach(false);
		this.hc.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
		{
			Velocity = new Vector2(num2 * Mathf.Sign(this.slopeNormal.x), 0f),
			Decay = decay
		});
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x000D8204 File Offset: 0x000D6404
	private void UpdateFacing()
	{
		if (this.hc.cState.facingRight)
		{
			if (this.slopeNormal.x < 0f)
			{
				this.hc.FaceLeft();
				return;
			}
		}
		else if (this.slopeNormal.x > 0f)
		{
			this.hc.FaceRight();
		}
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x000D8260 File Offset: 0x000D6460
	private void OnAnimationCompleted(tk2dSpriteAnimator currentAnimator, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == "Slide Start" || clip.name == "Slide Fast End" || clip.name == "Slide Brake End")
		{
			currentAnimator.Play("Slide Normal");
		}
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x000D82B0 File Offset: 0x000D64B0
	private void UpdateSlideEffects(SlideSurface.SlideSpeeds slideSpeed)
	{
		ParticleSystem particleSystem = (slideSpeed > SlideSurface.SlideSpeeds.None) ? this.slideParticles[(int)slideSpeed] : null;
		if (this.currentSlideParticles)
		{
			if (particleSystem == this.currentSlideParticles)
			{
				return;
			}
			this.currentSlideParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		if (particleSystem != null)
		{
			particleSystem.transform.SetPosition2D(this.slideParticlesPosition);
			particleSystem.Clear(true);
			particleSystem.Play(true);
		}
		this.currentSlideParticles = particleSystem;
	}

	// Token: 0x060030D6 RID: 12502 RVA: 0x000D8328 File Offset: 0x000D6528
	private void UpdateFollowPositions()
	{
		Vector3 position = this.body.transform.position;
		this.slideParticlesPosition = position - new Vector2(0f, this.heroFeetDistance);
		if (this.currentSlideParticles)
		{
			this.currentSlideParticles.transform.SetPosition2D(this.slideParticlesPosition);
		}
		if (this.followLoopSource)
		{
			this.followLoopSource.transform.position = position;
		}
	}

	// Token: 0x040033D7 RID: 13271
	private const string SLIDE_START_ANIM = "Slide Start";

	// Token: 0x040033D8 RID: 13272
	private const string SLIDE_NORMAL_ANIM = "Slide Normal";

	// Token: 0x040033D9 RID: 13273
	private const string SLIDE_FAST_ANIM = "Slide Fast";

	// Token: 0x040033DA RID: 13274
	private const string SLIDE_FAST_END_ANIM = "Slide Fast End";

	// Token: 0x040033DB RID: 13275
	private const string SLIDE_BRAKE_ANIM = "Slide Brake";

	// Token: 0x040033DC RID: 13276
	private const string SLIDE_BRAKE_END_ANIM = "Slide Brake End";

	// Token: 0x040033DD RID: 13277
	private const string JUMP_ANIM = "Airborne";

	// Token: 0x040033DE RID: 13278
	private const float RAY_LENGTH = 0.7f;

	// Token: 0x040033DF RID: 13279
	private const int MAX_RAY_HITS = 10;

	// Token: 0x040033E0 RID: 13280
	private const float FORCE_SLIDE_TIME = 0.15f;

	// Token: 0x040033E1 RID: 13281
	private const float FORCE_REGULAR_SLIDE_TIME = 0.3f;

	// Token: 0x040033E2 RID: 13282
	private const float FAST_SPEED_MULTIPLIER = 1.5f;

	// Token: 0x040033E3 RID: 13283
	private const float BRAKE_SPEED_MULTIPLIER = 0.7f;

	// Token: 0x040033E4 RID: 13284
	private const float SLOW_JUMP_AIR_VELOCITY = 15f;

	// Token: 0x040033E5 RID: 13285
	private const float SLOW_JUMP_AIR_VELOCITY_DECAY = 2.5f;

	// Token: 0x040033E6 RID: 13286
	private const float FAST_JUMP_AIR_VELOCITY = 18f;

	// Token: 0x040033E7 RID: 13287
	private const float FAST_JUMP_AIR_VELOCITY_DECAY = 3.5f;

	// Token: 0x040033E8 RID: 13288
	private const float JUMP_GRACE_TIME = 0.1f;

	// Token: 0x040033E9 RID: 13289
	public const PhysLayers SURFACE_LAYER = PhysLayers.HERO_DETECTOR;

	// Token: 0x040033EA RID: 13290
	[SerializeField]
	private float initialSlideSpeed;

	// Token: 0x040033EB RID: 13291
	[SerializeField]
	private float shallowSlideSpeed;

	// Token: 0x040033EC RID: 13292
	[SerializeField]
	private float steepSlideSpeed;

	// Token: 0x040033ED RID: 13293
	[SerializeField]
	private float shallowAcceleration;

	// Token: 0x040033EE RID: 13294
	[SerializeField]
	private float steepAcceleration;

	// Token: 0x040033EF RID: 13295
	[SerializeField]
	private CameraShakeTarget slideStartShake;

	// Token: 0x040033F0 RID: 13296
	[SerializeField]
	private AudioSource followLoopSource;

	// Token: 0x040033F1 RID: 13297
	[SerializeField]
	private AudioEvent enterAudio;

	// Token: 0x040033F2 RID: 13298
	[SerializeField]
	private AudioEvent exitAudio;

	// Token: 0x040033F3 RID: 13299
	[SerializeField]
	[ArrayForEnum(typeof(SlideSurface.SlideSpeeds))]
	private ParticleSystem[] slideParticles;

	// Token: 0x040033F4 RID: 13300
	[SerializeField]
	private bool cameraLeading = true;

	// Token: 0x040033F5 RID: 13301
	private float heroFeetDistance;

	// Token: 0x040033F6 RID: 13302
	private readonly RaycastHit2D[] storeHits = new RaycastHit2D[10];

	// Token: 0x040033F7 RID: 13303
	private float inputDirection;

	// Token: 0x040033F8 RID: 13304
	private float inputAccelerationDirection;

	// Token: 0x040033F9 RID: 13305
	private float slopeSpeedMultiplier;

	// Token: 0x040033FA RID: 13306
	private float forcedSlideTimeLeft;

	// Token: 0x040033FB RID: 13307
	private float forcedRegularSlideTimeLeft;

	// Token: 0x040033FC RID: 13308
	private float jumpGraceTimeLeft;

	// Token: 0x040033FD RID: 13309
	private float slopeDot;

	// Token: 0x040033FE RID: 13310
	private Vector2 slopeNormal;

	// Token: 0x040033FF RID: 13311
	private float currentSlideSpeed;

	// Token: 0x04003400 RID: 13312
	private bool endedOnGround;

	// Token: 0x04003401 RID: 13313
	private ParticleSystem currentSlideParticles;

	// Token: 0x04003402 RID: 13314
	private Vector2 slideParticlesPosition;

	// Token: 0x04003403 RID: 13315
	private bool isHeroInside;

	// Token: 0x04003404 RID: 13316
	private bool isHeroAttached;

	// Token: 0x04003405 RID: 13317
	private HeroController hc;

	// Token: 0x04003406 RID: 13318
	private EnviroRegionListener enviroListener;

	// Token: 0x04003407 RID: 13319
	private Rigidbody2D body;

	// Token: 0x04003408 RID: 13320
	private tk2dSpriteAnimator animator;

	// Token: 0x04003409 RID: 13321
	private BoxCollider2D heroCollider;

	// Token: 0x0400340A RID: 13322
	private Collider2D selfCollider;

	// Token: 0x0400340B RID: 13323
	private InputHandler ih;

	// Token: 0x0400340C RID: 13324
	private CameraTarget cameraTarget;

	// Token: 0x0400340D RID: 13325
	private static readonly List<SlideSurface> _slideSurfaces = new List<SlideSurface>();

	// Token: 0x0400340E RID: 13326
	private static int _heroInsideCount;

	// Token: 0x04003410 RID: 13328
	private bool wasAbove;

	// Token: 0x04003411 RID: 13329
	private bool didSetEnviroOverride;

	// Token: 0x0200185D RID: 6237
	private enum SlideSpeeds
	{
		// Token: 0x040091BA RID: 37306
		None = -1,
		// Token: 0x040091BB RID: 37307
		Regular,
		// Token: 0x040091BC RID: 37308
		Fast,
		// Token: 0x040091BD RID: 37309
		Brake
	}
}
