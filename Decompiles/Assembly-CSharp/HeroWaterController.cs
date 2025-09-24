using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class HeroWaterController : MonoBehaviour
{
	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000460 RID: 1120 RVA: 0x000171CC File Offset: 0x000153CC
	// (set) Token: 0x06000461 RID: 1121 RVA: 0x000171D4 File Offset: 0x000153D4
	public HeroWaterController.States CurrentState { get; private set; }

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000462 RID: 1122 RVA: 0x000171DD File Offset: 0x000153DD
	// (set) Token: 0x06000463 RID: 1123 RVA: 0x000171E5 File Offset: 0x000153E5
	private HeroWaterController.States PreviousState { get; set; }

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000464 RID: 1124 RVA: 0x000171EE File Offset: 0x000153EE
	// (set) Token: 0x06000465 RID: 1125 RVA: 0x000171F6 File Offset: 0x000153F6
	private HeroWaterController.States? PreviousSwimState { get; set; }

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x000171FF File Offset: 0x000153FF
	public bool IsInWater
	{
		get
		{
			return this.isInWater;
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000467 RID: 1127 RVA: 0x00017207 File Offset: 0x00015407
	private float SwimSpeed
	{
		get
		{
			if (!this.hc.IsUsingQuickening)
			{
				return this.swimSpeed;
			}
			return this.quickSwimSpeed;
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000468 RID: 1128 RVA: 0x00017223 File Offset: 0x00015423
	private float SprintSwimSpeed
	{
		get
		{
			if (!this.hc.IsUsingQuickening)
			{
				return this.sprintSwimSpeed;
			}
			return this.quickSprintSwimSpeed;
		}
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001723F File Offset: 0x0001543F
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.dropletFlingSprintingOffset, 0.2f);
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001726B File Offset: 0x0001546B
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		if (this.dashBurstEffect)
		{
			this.dashBurstEffect.SetActive(false);
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x000172A0 File Offset: 0x000154A0
	private void Start()
	{
		this.ih = ManagerSingleton<InputHandler>.Instance;
		this.hc = HeroController.instance;
		this.animCtrl = this.hc.GetComponent<HeroAnimationController>();
		this.vc = this.hc.GetVibrationCtrl();
		this.hasVibrationController = this.vc;
		GameManager.instance.NextSceneWillActivate += this.OnNextSceneLoaded;
		this.hc.OnTakenDamage += this.OnTakenDamage;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "MULTI WOUND CANCEL").ReceivedEvent += this.OnMultiWoundCancel;
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x00017344 File Offset: 0x00015544
	private void OnDestroy()
	{
		if (GameManager.instance)
		{
			GameManager.instance.NextSceneWillActivate -= this.OnNextSceneLoaded;
		}
		if (this.hc)
		{
			this.hc.OnTakenDamage -= this.OnTakenDamage;
		}
		foreach (KeyValuePair<GameObject, OnDestroyEventAnnouncer> keyValuePair in this._flingDestroyEvents)
		{
			GameObject gameObject;
			OnDestroyEventAnnouncer onDestroyEventAnnouncer;
			keyValuePair.Deconstruct(out gameObject, out onDestroyEventAnnouncer);
			onDestroyEventAnnouncer.OnDestroyEvent -= this.OnSpawnedFlingDestroyed;
		}
		this._flingDestroyEvents.Clear();
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00017400 File Offset: 0x00015600
	private void OnMultiWoundCancel()
	{
		this.TryEntry(false);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00017409 File Offset: 0x00015609
	private void TryEntry(bool forced)
	{
		this.monitorTimer = 0f;
		if (this.IsInWater && !forced)
		{
			return;
		}
		SurfaceWaterRegion.TryReentry(this, this.hc);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00017430 File Offset: 0x00015630
	public void EnterWaterRegion(SurfaceWaterRegion surfaceWater)
	{
		this.isInWater = true;
		this.waterColor = surfaceWater.Color;
		this.waterFlowSpeed = surfaceWater.FlowSpeed;
		this.anims = (surfaceWater.UseSpaAnims ? HeroWaterController._spaAnims : HeroWaterController._regularAnims);
		this.waterBounds = surfaceWater.Bounds;
		this.waterSurfaceRotation = surfaceWater.transform.rotation;
		if (this.CurrentState != HeroWaterController.States.Inactive)
		{
			return;
		}
		this.EnteredWater();
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x000174A2 File Offset: 0x000156A2
	public void OnNextSceneLoaded()
	{
		this.ExitWaterRegion(false);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000174AB File Offset: 0x000156AB
	public void ExitWaterRegion()
	{
		this.ExitWaterRegion(true);
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x000174B4 File Offset: 0x000156B4
	private void OnTakenDamage()
	{
		if (this.isInWater)
		{
			this.ExitWaterRegion();
			this.monitorTimer = 0.5f;
			return;
		}
		if (this.monitorTimer > 0f)
		{
			this.monitorTimer = 0.5f;
		}
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000174E8 File Offset: 0x000156E8
	public void ExitWaterRegion(bool vibrate)
	{
		this.isInWater = false;
		if (this.CurrentState == HeroWaterController.States.Inactive)
		{
			return;
		}
		this.TumbleOut(vibrate);
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00017504 File Offset: 0x00015704
	private void EnteredWater()
	{
		if (this.hc.cState.isSprinting && this.ih.inputActions.Dash.IsPressed)
		{
			this.queueStartSprint = true;
		}
		EventRegister.SendEvent(EventRegisterEvents.HeroSurfaceEnter, null);
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		if (this.hc.cState.recoiling)
		{
			this.hc.CancelDamageRecoil();
		}
		this.hc.ResetAirMoves();
		if (this.hc.cState.downSpiking)
		{
			this.hc.FinishDownspike();
		}
		this.hc.RelinquishControl();
		this.hc.StopAnimationControl();
		this.hc.AffectedByGravity(false);
		this.hc.IsSwimming();
		this.hc.SetAllowNailChargingWhileRelinquished(false);
		this.body.linearVelocity = Vector2.zero;
		this.CurrentState = HeroWaterController.States.Entered;
		this.PreviousState = HeroWaterController.States.Inactive;
		this.normalSwimAudio.Play();
		this.fastSwimAudio.Play();
		this.canAnimate = false;
		this.normalSwimAudio.volume = 0f;
		this.fastSwimAudio.volume = 0f;
		this.currentSwimSpeed = this.SwimSpeed;
		this.currentSprintSpeed = this.SprintSwimSpeed;
		this.doSprintDecelerate = false;
		this.isBonkBlocking = false;
		if (this.hasVibrationController)
		{
			this.vc.PlaySwimEnter();
		}
		this.swimBeginRoutine = base.StartCoroutine(this.SwimBegin());
		this.hc.ReduceOdours(20);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00017688 File Offset: 0x00015888
	private void ExitedWater(bool vibrate = true)
	{
		this.hc.RegainControl();
		this.hc.StartAnimationControl();
		this.hc.NotSwimming();
		EventRegister.SendEvent(EventRegisterEvents.HeroSurfaceExit, null);
		if (this.swimBeginRoutine != null)
		{
			base.StopCoroutine(this.swimBeginRoutine);
		}
		this.CurrentState = HeroWaterController.States.Inactive;
		this.PreviousState = HeroWaterController.States.Inactive;
		this.PreviousSwimState = null;
		this.idleTime = new double?(0.0);
		this.resetAnimation = false;
		this.normalSwimAudio.Stop();
		this.fastSwimAudio.Stop();
		this.isSprinting = false;
		if (vibrate && this.hasVibrationController)
		{
			this.vc.PlaySwimExit();
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x00017740 File Offset: 0x00015940
	private IEnumerator SwimBegin()
	{
		this.canJump = false;
		this.isEnterTumbling = false;
		string anim;
		if (Mathf.Abs(this.waterFlowSpeed) > 0.01f)
		{
			anim = "Surface Current In Tumble";
			this.isEnterTumbling = true;
		}
		else
		{
			anim = this.anims.In;
		}
		yield return base.StartCoroutine(this.animator.PlayAnimWait(anim, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationTrigger)));
		if (this.isEnterTumbling)
		{
			this.isEnterTumbling = false;
			yield return base.StartCoroutine(this.animator.PlayAnimWait("Surface Current In Recover", null));
		}
		else if (this.GetSwimDirection() == 0)
		{
			yield return base.StartCoroutine(this.animator.PlayAnimWait(this.anims.InToIdle, null));
			this.animator.Play(this.anims.Idle);
		}
		else
		{
			this.animator.Play(this.anims.IdleToSwim);
		}
		this.canAnimate = true;
		yield break;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001774F File Offset: 0x0001594F
	private void FixedUpdate()
	{
		if (this.dashQueueStepsLeft > 0)
		{
			this.dashQueueStepsLeft--;
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00017768 File Offset: 0x00015968
	private void Update()
	{
		if (this.hc.IsPaused())
		{
			if (this.hasVibrationController)
			{
				this.vc.SetSwimming(false);
			}
			return;
		}
		if (this.recoilCheck || this.monitorTimer > 0f)
		{
			this.monitorTimer -= Time.deltaTime;
			if (this.hc.cState.recoiling)
			{
				this.recoilCheck = true;
			}
			else if (this.recoilCheck)
			{
				this.recoilCheck = false;
				if (this.hc.IsGravityApplied)
				{
					this.TryEntry(true);
				}
			}
			else if (this.monitorTimer <= 0f)
			{
				this.OnMultiWoundCancel();
			}
		}
		if (this.queueStartSprint && !this.ih.inputActions.Dash.IsPressed)
		{
			this.queueStartSprint = false;
		}
		if (this.ih.inputActions.Jump.WasPressed)
		{
			if (!this.hc.cState.dashing && !this.hc.cState.airDashing)
			{
				this.waterEnterJumpQueueTimeLeft = 0.1f;
			}
		}
		else if (this.waterEnterJumpQueueTimeLeft > 0f)
		{
			this.waterEnterJumpQueueTimeLeft -= Time.deltaTime;
		}
		if (this.CurrentState != HeroWaterController.States.Inactive)
		{
			this.resetAnimation = false;
			this.PreviousState = this.CurrentState;
			if (this.waterEnterJumpQueueTimeLeft > 0f && this.TryDoJump())
			{
				return;
			}
			int num = this.GetSwimDirection();
			if (this.isSprinting)
			{
				if (!this.ih.inputActions.Dash.IsPressed && !this.IsInDashBurst())
				{
					this.isSprinting = false;
					this.currentSwimSpeed = this.currentSprintSpeed;
				}
			}
			else
			{
				if (this.queueStartSprint || this.ih.inputActions.Dash.WasPressed)
				{
					this.queueStartSprint = false;
					if (this.CanDash())
					{
						this.isSprinting = true;
					}
					else
					{
						this.dashQueueStepsLeft = this.hc.DASH_QUEUE_STEPS;
					}
				}
				else if (this.ih.inputActions.Dash.IsPressed && this.dashQueueStepsLeft > 0 && this.CanDash())
				{
					this.isSprinting = true;
				}
				if (this.isSprinting)
				{
					this.dashBurstEndTime = Time.timeAsDouble + (double)this.hc.DASH_TIME;
					this.nextDashTime = this.dashBurstEndTime + (double)this.hc.DASH_COOLDOWN;
					this.currentSprintSpeed = (this.hc.cState.facingRight ? this.sprintBurstSpeed : (-this.sprintBurstSpeed));
					this.currentSprintAcceleration = this.sprintBurstAcceleration;
					if (this.swimBeginRoutine != null)
					{
						base.StopCoroutine(this.swimBeginRoutine);
					}
					this.canAnimate = true;
					this.normalSwimAudio.volume = 0f;
					this.fastSwimAudio.volume = 1f;
					if (this.fastSwimStart)
					{
						this.fastSwimAudio.PlayOneShot(this.fastSwimStart);
					}
					this.canJump = true;
					if (this.dashBurstEffect)
					{
						this.dashBurstEffect.SetActive(true);
					}
					this.SpawnFlingsLocal(this.dropletFlingSprintBurst, Vector3.zero);
				}
			}
			if (this.isSprinting)
			{
				this.dashQueueStepsLeft = 0;
				if (num == 0 || this.IsInDashBurst())
				{
					num = (this.hc.cState.facingRight ? 1 : -1);
				}
				this.CurrentState = ((num > 0) ? HeroWaterController.States.SprintRight : HeroWaterController.States.SprintLeft);
				this.idleTime = null;
			}
			else if (num < 0)
			{
				this.CurrentState = HeroWaterController.States.SwimLeft;
				this.idleTime = null;
			}
			else if (num > 0)
			{
				this.CurrentState = HeroWaterController.States.SwimRight;
				this.idleTime = null;
			}
			else
			{
				double value = this.idleTime.GetValueOrDefault();
				if (this.idleTime == null)
				{
					value = Time.timeAsDouble + (double)this.idleTimePadding;
					this.idleTime = new double?(value);
				}
				this.CurrentState = HeroWaterController.States.Idle;
			}
			if (this.doSprintDecelerate && this.CurrentState == HeroWaterController.States.Idle)
			{
				HeroWaterController.States previousState = this.PreviousState;
				if (previousState == HeroWaterController.States.SwimLeft || previousState == HeroWaterController.States.SwimRight)
				{
					this.doSprintDecelerate = false;
				}
			}
			bool flag = this.CurrentState != HeroWaterController.States.Idle;
			float t = this.swimAudioLerpSpeed * Time.deltaTime;
			this.normalSwimAudio.volume = Mathf.Lerp(this.normalSwimAudio.volume, (float)((flag && !this.isSprinting) ? 1 : 0), t);
			this.fastSwimAudio.volume = Mathf.Lerp(this.fastSwimAudio.volume, (float)((flag && this.isSprinting) ? 1 : 0), t);
			if (this.hasVibrationController)
			{
				this.vc.SetSwimAndSprint(flag, this.isSprinting);
			}
			float b = 0f;
			float b2 = 0f;
			float speedMultiplier = 1f;
			switch (this.CurrentState)
			{
			case HeroWaterController.States.Idle:
				if (this.PreviousState != HeroWaterController.States.Idle)
				{
					this.nextAnimation = this.anims.Idle;
				}
				b = 0f;
				break;
			case HeroWaterController.States.SwimLeft:
			{
				if (this.PreviousState != HeroWaterController.States.SwimLeft)
				{
					this.hc.FaceLeft();
					this.PlayIdleToSwim();
				}
				HeroWaterController.States? previousSwimState = this.PreviousSwimState;
				HeroWaterController.States states = HeroWaterController.States.SwimRight;
				if (!(previousSwimState.GetValueOrDefault() == states & previousSwimState != null))
				{
					previousSwimState = this.PreviousSwimState;
					states = HeroWaterController.States.SprintRight;
					if (!(previousSwimState.GetValueOrDefault() == states & previousSwimState != null))
					{
						goto IL_557;
					}
				}
				this.PlaySwimTurn();
				this.doSprintDecelerate = false;
				IL_557:
				b = -this.SwimSpeed;
				speedMultiplier = this.SwimSpeed / this.swimSpeed;
				break;
			}
			case HeroWaterController.States.SwimRight:
			{
				if (this.PreviousState != HeroWaterController.States.SwimRight)
				{
					this.hc.FaceRight();
					this.PlayIdleToSwim();
				}
				HeroWaterController.States? previousSwimState = this.PreviousSwimState;
				HeroWaterController.States states = HeroWaterController.States.SwimLeft;
				if (!(previousSwimState.GetValueOrDefault() == states & previousSwimState != null))
				{
					previousSwimState = this.PreviousSwimState;
					states = HeroWaterController.States.SprintLeft;
					if (!(previousSwimState.GetValueOrDefault() == states & previousSwimState != null))
					{
						goto IL_5DA;
					}
				}
				this.PlaySwimTurn();
				this.doSprintDecelerate = false;
				IL_5DA:
				b = this.SwimSpeed;
				speedMultiplier = this.SwimSpeed / this.swimSpeed;
				break;
			}
			case HeroWaterController.States.SprintLeft:
			{
				if (this.PreviousState != HeroWaterController.States.SprintLeft)
				{
					this.hc.FaceLeft();
					this.PlaySwimDash();
					this.doSprintDecelerate = true;
				}
				HeroWaterController.States? previousSwimState = this.PreviousSwimState;
				HeroWaterController.States states = HeroWaterController.States.SprintRight;
				if (previousSwimState.GetValueOrDefault() == states & previousSwimState != null)
				{
					this.PlaySwimDashTurn();
				}
				b2 = -this.SprintSwimSpeed;
				speedMultiplier = this.SprintSwimSpeed / this.sprintSwimSpeed;
				break;
			}
			case HeroWaterController.States.SprintRight:
			{
				if (this.PreviousState != HeroWaterController.States.SprintRight)
				{
					this.hc.FaceRight();
					this.PlaySwimDash();
					this.doSprintDecelerate = true;
				}
				HeroWaterController.States? previousSwimState = this.PreviousSwimState;
				HeroWaterController.States states = HeroWaterController.States.SprintLeft;
				if (previousSwimState.GetValueOrDefault() == states & previousSwimState != null)
				{
					this.PlaySwimDashTurn();
				}
				b2 = this.SprintSwimSpeed;
				speedMultiplier = this.SprintSwimSpeed / this.sprintSwimSpeed;
				break;
			}
			}
			if (this.CurrentState != HeroWaterController.States.Idle)
			{
				this.PreviousSwimState = new HeroWaterController.States?(this.CurrentState);
			}
			if (this.isSprinting)
			{
				if (!this.IsInDashBurst())
				{
					this.currentSprintSpeed = Mathf.Lerp(this.currentSprintSpeed, b2, this.currentSprintAcceleration * Time.deltaTime);
				}
				this.UpdateMoveVelocity(this.currentSprintSpeed);
				if (Time.timeAsDouble >= this.nextSprintDropletSpawnTime)
				{
					this.nextSprintDropletSpawnTime = Time.timeAsDouble + (double)this.dropletFlingSprintingDelay;
					this.SpawnFlingsLocal(this.dropletFlingSprinting, this.dropletFlingSprintingOffset);
				}
			}
			else
			{
				if (this.doSprintDecelerate)
				{
					this.currentSwimSpeed = Mathf.Lerp(this.currentSwimSpeed, b, this.sprintEndAcceleration * Time.deltaTime);
				}
				else
				{
					this.currentSwimSpeed = b;
				}
				this.UpdateMoveVelocity(this.currentSwimSpeed);
			}
			this.reduceOdourAccumulate += 10f * Time.deltaTime;
			if (this.reduceOdourAccumulate > 1f)
			{
				this.hc.ReduceOdours(Mathf.FloorToInt(this.reduceOdourAccumulate));
				this.reduceOdourAccumulate %= 1f;
			}
			if ((this.idleTime == null || Time.timeAsDouble >= this.idleTime.Value) && this.canAnimate && !string.IsNullOrEmpty(this.nextAnimation))
			{
				if (this.resetAnimation && !this.animator.IsPlaying(this.nextAnimation))
				{
					this.resetAnimation = false;
				}
				if (this.resetAnimation)
				{
					this.animator.PlayFromFrame(0);
				}
				else
				{
					this.PlayAnim(this.nextAnimation, speedMultiplier);
				}
				this.nextAnimation = string.Empty;
			}
			if (this.isSprinting && this.isTouchingWall)
			{
				this.DoSprintBonk();
			}
			if (this.isEnterTumbling && Time.timeAsDouble >= this.nextDropletFlingTumblingTime)
			{
				this.nextDropletFlingTumblingTime = Time.timeAsDouble + (double)this.dropletFlingTumblingDelay.GetRandomValue();
				this.SpawnFlingsLocal(this.dropletFlingTumbling, Vector3.zero);
			}
		}
		else if (this.isInWater && base.transform.position.y - this.previousYPos < 0f && this.entryDelayTime <= Time.timeAsDouble)
		{
			this.EnteredWater();
		}
		this.previousYPos = base.transform.position.y;
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00018064 File Offset: 0x00016264
	private void PlayAnim(string clipName, float speedMultiplier = 1f)
	{
		tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(clipName);
		if (!Mathf.Approximately(speedMultiplier, 1f))
		{
			this.animator.Play(clipByName, 0f, clipByName.fps * speedMultiplier);
			return;
		}
		this.animator.Play(clipByName);
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x000180B4 File Offset: 0x000162B4
	private void UpdateMoveVelocity(float moveSpeed)
	{
		Vector2 v = new Vector2(moveSpeed + this.waterFlowSpeed, 0f);
		Vector3 v2 = this.waterSurfaceRotation * v;
		this.body.linearVelocity = v2;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000180F8 File Offset: 0x000162F8
	private bool IsInDashBurst()
	{
		return Time.timeAsDouble <= this.dashBurstEndTime;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001810A File Offset: 0x0001630A
	private bool CanDash()
	{
		return !this.isEnterTumbling && PlayerData.instance.hasDash && Time.timeAsDouble >= this.nextDashTime;
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00018134 File Offset: 0x00016334
	private void PlayIdleToSwim()
	{
		this.nextAnimation = this.anims.IdleToSwim;
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00018148 File Offset: 0x00016348
	private void PlaySwimTurn()
	{
		if (this.isSprinting)
		{
			this.fastSwimAudio.volume = 1f;
		}
		else
		{
			this.normalSwimAudio.volume = 1f;
		}
		if (this.canAnimate)
		{
			this.nextAnimation = this.anims.TurnToSwim;
			this.resetAnimation = true;
		}
		this.currentSprintAcceleration = this.sprintAcceleration;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x000181AB File Offset: 0x000163AB
	private void PlaySwimDash()
	{
		this.nextAnimation = "Swim Dash";
		this.resetAnimation = true;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x000181BF File Offset: 0x000163BF
	private void PlaySwimDashTurn()
	{
		if (this.isSprinting)
		{
			this.fastSwimAudio.volume = 1f;
		}
		else
		{
			this.normalSwimAudio.volume = 1f;
		}
		this.nextAnimation = "Swim Dash Turn";
		this.resetAnimation = true;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00018200 File Offset: 0x00016400
	private int GetSwimDirection()
	{
		if (this.isBonkBlocking || this.isEnterTumbling)
		{
			return 0;
		}
		int num = 0;
		if (this.ih.inputActions.Left.IsPressed)
		{
			num--;
		}
		if (this.ih.inputActions.Right.IsPressed)
		{
			num++;
		}
		return num;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00018258 File Offset: 0x00016458
	private void SpawnFlingsLocal(FlingUtils.Config config, Vector3 offset)
	{
		if (this.hc.cState.facingRight)
		{
			config.AngleMin = global::Helper.GetReflectedAngle(config.AngleMin, true, false, false);
			config.AngleMax = global::Helper.GetReflectedAngle(config.AngleMax, true, false, false);
		}
		this.flingSpawnTracker.Clear();
		FlingUtils.SpawnAndFling(config, base.transform, offset, this.flingSpawnTracker, -1f);
		this.SetSpawnedGameObjectColorsTemp(this.flingSpawnTracker);
		this.RegisterSpawnedFlingDestroyCallbacks(this.flingSpawnTracker);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000182DC File Offset: 0x000164DC
	private void RegisterSpawnedFlingDestroyCallbacks(List<GameObject> flings)
	{
		for (int i = 0; i < flings.Count; i++)
		{
			GameObject gameObject = flings[i];
			if (!this._flingDestroyEvents.ContainsKey(gameObject))
			{
				OnDestroyEventAnnouncer onDestroyEventAnnouncer = gameObject.AddComponent<OnDestroyEventAnnouncer>();
				onDestroyEventAnnouncer.OnDestroyEvent += this.OnSpawnedFlingDestroyed;
				this._flingDestroyEvents.Add(gameObject, onDestroyEventAnnouncer);
			}
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00018336 File Offset: 0x00016536
	private void OnSpawnedFlingDestroyed(OnDestroyEventAnnouncer announcer)
	{
		announcer.OnDestroyEvent -= this.OnSpawnedFlingDestroyed;
		this.flingSpawnTracker.Remove(announcer.gameObject);
		this._flingDestroyEvents.Remove(announcer.gameObject);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00018370 File Offset: 0x00016570
	private void SetSpawnedGameObjectColorsTemp(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
			if (!sprite)
			{
				break;
			}
			RecycleResetHandler recycleResetHandler = gameObject.GetComponent<RecycleResetHandler>() ?? gameObject.AddComponent<RecycleResetHandler>();
			Color initialColor = sprite.color;
			sprite.color = this.waterColor;
			recycleResetHandler.AddTempAction(delegate()
			{
				sprite.color = initialColor;
			});
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001841C File Offset: 0x0001661C
	private void TranslateIfNecessary()
	{
		Transform[] array = this.jumpRaycastOrigins;
		for (int i = 0; i < array.Length; i++)
		{
			if (global::Helper.Raycast2D(array[i].position, Vector2.up, this.jumpRaycastDistance, 256).collider != null)
			{
				return;
			}
		}
		base.transform.Translate(new Vector3(0f, this.jumpTranslateHeight, 0f));
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00018494 File Offset: 0x00016694
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.isInWater)
		{
			return;
		}
		if (collision.gameObject.layer != 8)
		{
			return;
		}
		float x = base.transform.position.x;
		if (collision.GetSafeContact().Point.x - x > 0f)
		{
			if (!this.hc.cState.facingRight)
			{
				return;
			}
		}
		else if (this.hc.cState.facingRight)
		{
			return;
		}
		this.isTouchingWall = true;
		if (this.isSprinting)
		{
			this.DoSprintBonk();
		}
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001851E File Offset: 0x0001671E
	private void OnCollisionExit2D()
	{
		this.isTouchingWall = false;
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00018528 File Offset: 0x00016728
	private bool TryDoJump()
	{
		if (this.hc.cState.dashing || this.hc.cState.airDashing)
		{
			return false;
		}
		if (!this.canJump)
		{
			if (!this.isEnterTumbling)
			{
				this.isJumpQueued = true;
			}
			return false;
		}
		this.entryDelayTime = Time.timeAsDouble + 0.05000000447034836;
		this.isJumpQueued = false;
		this.waterEnterJumpQueueTimeLeft = 0f;
		Vector2 linearVelocity = this.body.linearVelocity;
		bool flag = this.isSprinting;
		this.animator.Play("Airborne");
		this.body.linearVelocity = new Vector2(0f, 10f);
		this.TranslateIfNecessary();
		this.hc.ResetInputQueues();
		if (this.isSprinting)
		{
			this.hc.SetStartWithFlipJump();
		}
		else
		{
			this.hc.SetStartWithJump();
		}
		this.ExitedWater(true);
		if (flag && Math.Abs(linearVelocity.x) > 0.01f)
		{
			this.hc.AddExtraAirMoveVelocity(new HeroController.DecayingVelocity
			{
				Velocity = new Vector2(linearVelocity.x, 0f),
				Decay = 3f,
				CancelOnTurn = true,
				SkipBehaviour = HeroController.DecayingVelocity.SkipBehaviours.WhileMoving
			});
		}
		return true;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001866C File Offset: 0x0001686C
	private void DoSprintBonk()
	{
		this.canAnimate = false;
		this.animator.Play("Swim Dash Bonk");
		this.isBonkBlocking = true;
		this.isSprinting = false;
		this.currentSwimSpeed = (this.hc.cState.facingRight ? this.sprintBonkSpeed : (-this.sprintBonkSpeed));
		this.sprintBonkActivate.SetAllActive(true);
		this.sprintBonkCameraShake.DoShake(this, true);
		this.sprintBonkAudio.SpawnAndPlayOneShot(base.transform.position, null);
		DeliveryQuestItem.TakeHit();
		this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00018714 File Offset: 0x00016914
	private void OnAnimationTrigger(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		if (clip.name == this.anims.In || clip.name == "Surface Current In Recover")
		{
			this.canJump = true;
			if (this.isJumpQueued)
			{
				this.TryDoJump();
			}
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00018761 File Offset: 0x00016961
	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == "Swim Dash Bonk")
		{
			this.canAnimate = true;
			this.nextAnimation = this.anims.IdleToSwim;
			this.isBonkBlocking = false;
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00018794 File Offset: 0x00016994
	private void TumbleOut(bool vibrate = true)
	{
		this.animCtrl.SetPlayMantleCancel();
		this.hc.SetStartFromMantle();
		this.ExitedWater(vibrate);
		float x = this.hc.transform.position.x;
		float x2 = this.waterBounds.center.x;
		if (x < x2)
		{
			this.hc.FaceLeft();
			this.hc.RecoilLeftLong();
			return;
		}
		this.hc.FaceRight();
		this.hc.RecoilRightLong();
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00018814 File Offset: 0x00016A14
	public void Rejected()
	{
		if (this.entryDelayTime > Time.timeAsDouble)
		{
			return;
		}
		this.hc.RelinquishControl();
		this.hc.StopAnimationControl();
		this.TumbleOut(true);
	}

	// Token: 0x04000402 RID: 1026
	private static readonly HeroWaterController.AnimationGroup _regularAnims = new HeroWaterController.AnimationGroup
	{
		In = "Surface In",
		InToIdle = "Surface InToIdle",
		Idle = "Surface Idle",
		IdleToSwim = "Surface IdleToSwim",
		TurnToSwim = "Surface TurnToSwim"
	};

	// Token: 0x04000403 RID: 1027
	private static readonly HeroWaterController.AnimationGroup _spaAnims = new HeroWaterController.AnimationGroup(HeroWaterController._regularAnims)
	{
		In = "Spa Surface In",
		InToIdle = "Spa Surface InToIdle",
		Idle = "Spa Surface Idle",
		IdleToSwim = "Spa Surface IdleToSwim",
		TurnToSwim = "Spa Surface TurnToSwim"
	};

	// Token: 0x04000407 RID: 1031
	[SerializeField]
	private float swimSpeed = 5f;

	// Token: 0x04000408 RID: 1032
	[SerializeField]
	private float quickSwimSpeed = 6f;

	// Token: 0x04000409 RID: 1033
	[Space]
	[SerializeField]
	private float sprintSwimSpeed = 8f;

	// Token: 0x0400040A RID: 1034
	[SerializeField]
	private float quickSprintSwimSpeed = 9f;

	// Token: 0x0400040B RID: 1035
	[SerializeField]
	private float sprintAcceleration = 5f;

	// Token: 0x0400040C RID: 1036
	[SerializeField]
	private float sprintBurstSpeed = 10f;

	// Token: 0x0400040D RID: 1037
	[SerializeField]
	private float sprintBurstAcceleration = 5f;

	// Token: 0x0400040E RID: 1038
	[SerializeField]
	private float sprintEndAcceleration = 5f;

	// Token: 0x0400040F RID: 1039
	[SerializeField]
	private GameObject dashBurstEffect;

	// Token: 0x04000410 RID: 1040
	[Space]
	[SerializeField]
	private float sprintBonkSpeed = -10f;

	// Token: 0x04000411 RID: 1041
	[SerializeField]
	private GameObject[] sprintBonkActivate;

	// Token: 0x04000412 RID: 1042
	[SerializeField]
	private CameraShakeTarget sprintBonkCameraShake;

	// Token: 0x04000413 RID: 1043
	[SerializeField]
	private AudioEvent sprintBonkAudio;

	// Token: 0x04000414 RID: 1044
	[Space]
	[SerializeField]
	private float idleTimePadding = 0.1f;

	// Token: 0x04000415 RID: 1045
	private double? idleTime;

	// Token: 0x04000416 RID: 1046
	[SerializeField]
	private AudioSource normalSwimAudio;

	// Token: 0x04000417 RID: 1047
	[SerializeField]
	private AudioSource fastSwimAudio;

	// Token: 0x04000418 RID: 1048
	[SerializeField]
	private AudioClip fastSwimStart;

	// Token: 0x04000419 RID: 1049
	[SerializeField]
	private float swimAudioLerpSpeed = 5f;

	// Token: 0x0400041A RID: 1050
	[SerializeField]
	private Transform[] jumpRaycastOrigins;

	// Token: 0x0400041B RID: 1051
	[SerializeField]
	private float jumpRaycastDistance = 1.1f;

	// Token: 0x0400041C RID: 1052
	[SerializeField]
	private float jumpTranslateHeight = 1f;

	// Token: 0x0400041D RID: 1053
	[Space]
	[SerializeField]
	private FlingUtils.Config dropletFlingSprintBurst;

	// Token: 0x0400041E RID: 1054
	[SerializeField]
	private FlingUtils.Config dropletFlingSprinting;

	// Token: 0x0400041F RID: 1055
	[SerializeField]
	private Vector3 dropletFlingSprintingOffset;

	// Token: 0x04000420 RID: 1056
	[SerializeField]
	private float dropletFlingSprintingDelay;

	// Token: 0x04000421 RID: 1057
	[Space]
	[SerializeField]
	private FlingUtils.Config dropletFlingTumbling;

	// Token: 0x04000422 RID: 1058
	[SerializeField]
	private MinMaxFloat dropletFlingTumblingDelay;

	// Token: 0x04000423 RID: 1059
	private Coroutine swimBeginRoutine;

	// Token: 0x04000424 RID: 1060
	private bool canAnimate;

	// Token: 0x04000425 RID: 1061
	private string nextAnimation;

	// Token: 0x04000426 RID: 1062
	private bool resetAnimation;

	// Token: 0x04000427 RID: 1063
	private float waterEnterJumpQueueTimeLeft;

	// Token: 0x04000428 RID: 1064
	private bool canJump;

	// Token: 0x04000429 RID: 1065
	private bool isJumpQueued;

	// Token: 0x0400042A RID: 1066
	private bool isEnterTumbling;

	// Token: 0x0400042B RID: 1067
	private double nextDropletFlingTumblingTime;

	// Token: 0x0400042C RID: 1068
	private float currentSprintSpeed;

	// Token: 0x0400042D RID: 1069
	private float currentSprintAcceleration;

	// Token: 0x0400042E RID: 1070
	private bool isSprinting;

	// Token: 0x0400042F RID: 1071
	private float currentSwimSpeed;

	// Token: 0x04000430 RID: 1072
	private bool doSprintDecelerate;

	// Token: 0x04000431 RID: 1073
	private bool queueStartSprint;

	// Token: 0x04000432 RID: 1074
	private double nextDashTime;

	// Token: 0x04000433 RID: 1075
	private double dashBurstEndTime;

	// Token: 0x04000434 RID: 1076
	private double nextSprintDropletSpawnTime;

	// Token: 0x04000435 RID: 1077
	private int dashQueueStepsLeft;

	// Token: 0x04000436 RID: 1078
	private bool isBonkBlocking;

	// Token: 0x04000437 RID: 1079
	private bool isTouchingWall;

	// Token: 0x04000438 RID: 1080
	private Color waterColor;

	// Token: 0x04000439 RID: 1081
	private float waterFlowSpeed;

	// Token: 0x0400043A RID: 1082
	private Bounds waterBounds;

	// Token: 0x0400043B RID: 1083
	private readonly List<GameObject> flingSpawnTracker = new List<GameObject>();

	// Token: 0x0400043C RID: 1084
	private HeroWaterController.AnimationGroup anims;

	// Token: 0x0400043D RID: 1085
	private Quaternion waterSurfaceRotation;

	// Token: 0x0400043E RID: 1086
	private InputHandler ih;

	// Token: 0x0400043F RID: 1087
	private HeroController hc;

	// Token: 0x04000440 RID: 1088
	private HeroAnimationController animCtrl;

	// Token: 0x04000441 RID: 1089
	private HeroVibrationController vc;

	// Token: 0x04000442 RID: 1090
	private tk2dSpriteAnimator animator;

	// Token: 0x04000443 RID: 1091
	private Rigidbody2D body;

	// Token: 0x04000444 RID: 1092
	private Dictionary<GameObject, OnDestroyEventAnnouncer> _flingDestroyEvents = new Dictionary<GameObject, OnDestroyEventAnnouncer>();

	// Token: 0x04000445 RID: 1093
	private bool isInWater;

	// Token: 0x04000446 RID: 1094
	private float previousYPos;

	// Token: 0x04000447 RID: 1095
	private float reduceOdourAccumulate;

	// Token: 0x04000448 RID: 1096
	private bool hasVibrationController;

	// Token: 0x04000449 RID: 1097
	private float monitorTimer;

	// Token: 0x0400044A RID: 1098
	private bool recoilCheck;

	// Token: 0x0400044B RID: 1099
	private const float RE_ENTER_DELAY = 0.050000004f;

	// Token: 0x0400044C RID: 1100
	private double entryDelayTime;

	// Token: 0x02001407 RID: 5127
	public enum States
	{
		// Token: 0x0400819D RID: 33181
		Inactive,
		// Token: 0x0400819E RID: 33182
		Entered,
		// Token: 0x0400819F RID: 33183
		Idle,
		// Token: 0x040081A0 RID: 33184
		SwimLeft,
		// Token: 0x040081A1 RID: 33185
		SwimRight,
		// Token: 0x040081A2 RID: 33186
		SprintLeft,
		// Token: 0x040081A3 RID: 33187
		SprintRight
	}

	// Token: 0x02001408 RID: 5128
	private class AnimationGroup
	{
		// Token: 0x06008240 RID: 33344 RVA: 0x00264C06 File Offset: 0x00262E06
		public AnimationGroup()
		{
		}

		// Token: 0x06008241 RID: 33345 RVA: 0x00264C10 File Offset: 0x00262E10
		public AnimationGroup(HeroWaterController.AnimationGroup other)
		{
			this.In = other.In;
			this.InToIdle = other.InToIdle;
			this.Idle = other.Idle;
			this.IdleToSwim = other.IdleToSwim;
			this.TurnToSwim = other.TurnToSwim;
		}

		// Token: 0x040081A4 RID: 33188
		public string In;

		// Token: 0x040081A5 RID: 33189
		public string InToIdle;

		// Token: 0x040081A6 RID: 33190
		public string Idle;

		// Token: 0x040081A7 RID: 33191
		public string IdleToSwim;

		// Token: 0x040081A8 RID: 33192
		public string TurnToSwim;
	}
}
