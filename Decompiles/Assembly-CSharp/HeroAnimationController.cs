using System;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class HeroAnimationController : MonoBehaviour, IHeroAnimationController
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003D4 RID: 980 RVA: 0x0001317A File Offset: 0x0001137A
	// (set) Token: 0x060003D5 RID: 981 RVA: 0x00013182 File Offset: 0x00011382
	public ActorStates actorState { get; private set; }

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003D6 RID: 982 RVA: 0x0001318B File Offset: 0x0001138B
	// (set) Token: 0x060003D7 RID: 983 RVA: 0x00013193 File Offset: 0x00011393
	public ActorStates prevActorState { get; private set; }

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x0001319C File Offset: 0x0001139C
	// (set) Token: 0x060003D9 RID: 985 RVA: 0x000131A4 File Offset: 0x000113A4
	public ActorStates stateBeforeControl { get; private set; }

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x060003DA RID: 986 RVA: 0x000131AD File Offset: 0x000113AD
	// (set) Token: 0x060003DB RID: 987 RVA: 0x000131B5 File Offset: 0x000113B5
	public bool controlEnabled { get; private set; }

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060003DC RID: 988 RVA: 0x000131BE File Offset: 0x000113BE
	// (set) Token: 0x060003DD RID: 989 RVA: 0x000131C6 File Offset: 0x000113C6
	public bool IsPlayingUpdraftAnim { get; private set; }

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060003DE RID: 990 RVA: 0x000131CF File Offset: 0x000113CF
	// (set) Token: 0x060003DF RID: 991 RVA: 0x000131D7 File Offset: 0x000113D7
	public bool IsPlayingWindyAnim { get; private set; }

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060003E0 RID: 992 RVA: 0x000131E0 File Offset: 0x000113E0
	// (set) Token: 0x060003E1 RID: 993 RVA: 0x000131E8 File Offset: 0x000113E8
	public bool IsPlayingHurtAnim { get; private set; }

	// Token: 0x060003E2 RID: 994 RVA: 0x000131F4 File Offset: 0x000113F4
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.heroCtrl = base.GetComponent<HeroController>();
		this.audioCtrl = base.GetComponent<HeroAudioController>();
		this.cState = this.heroCtrl.cState;
		this.clearBackflipSpawnedAudio = delegate()
		{
			this.backflipSpawnedAudio = null;
		};
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00013254 File Offset: 0x00011454
	private void Start()
	{
		this.pd = PlayerData.instance;
		this.ResetAll();
		this.actorState = this.heroCtrl.hero_state;
		if (this.controlEnabled)
		{
			if (this.heroCtrl.hero_state == ActorStates.airborne)
			{
				this.PlayFromFrame("Airborne", 7, false);
			}
			else
			{
				this.PlayIdle();
			}
		}
		else
		{
			this.animator.Stop();
		}
		if (this.windyAnimLib != null)
		{
			foreach (tk2dSpriteAnimationClip tk2dSpriteAnimationClip in this.windyAnimLib.clips)
			{
				if (tk2dSpriteAnimationClip != null && !tk2dSpriteAnimationClip.Empty && !(tk2dSpriteAnimationClip.frames[0].spriteCollection == null))
				{
					tk2dSpriteCollectionData inst = tk2dSpriteAnimationClip.frames[0].spriteCollection.inst;
				}
			}
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += this.UpdateToolEquipFlags;
		this.UpdateToolEquipFlags();
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00013341 File Offset: 0x00011541
	private void UpdateToolEquipFlags()
	{
		this.isCursed = Gameplay.CursedCrest.IsEquipped;
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00013354 File Offset: 0x00011554
	private void Update()
	{
		if (this.controlEnabled && !this.waitingToEnter)
		{
			this.UpdateAnimation();
		}
		else if (this.cState.facingRight)
		{
			this.wasFacingRight = true;
		}
		else
		{
			this.wasFacingRight = false;
		}
		if (this.pd.betaEnd)
		{
			this.PlayRun();
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x000133A9 File Offset: 0x000115A9
	public void SetHeroControllerConfig(HeroControllerConfig config)
	{
		this.config = config;
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x000133B4 File Offset: 0x000115B4
	public void ResetAll()
	{
		this.playRunToIdle = false;
		this.playDashToIdle = false;
		this.playLanding = false;
		this.playSlashLand = false;
		this.playSilkChargeEnd = false;
		this.controlEnabled = true;
		this.isPlayingSlashLand = false;
		this.wasFacingRight = this.cState.facingRight;
		this.wasPlayingAirRecovery = false;
		this.ResetIdleLook();
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00013410 File Offset: 0x00011610
	public void ResetDownspikeBounce()
	{
		this.playedDownSpikeBounce = false;
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x00013419 File Offset: 0x00011619
	public void ResetIdleLook()
	{
		this.playingIdleRest = false;
		this.nextIdleLookTime = Random.Range(10f, 15f);
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00013437 File Offset: 0x00011637
	public void ResetPlays()
	{
		this.playLanding = false;
		this.playRunToIdle = false;
		this.playDashToIdle = false;
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x00013450 File Offset: 0x00011650
	public void UpdateState(ActorStates newState)
	{
		if (!this.controlEnabled)
		{
			return;
		}
		if (newState == this.actorState)
		{
			return;
		}
		if (this.actorState == ActorStates.airborne && newState == ActorStates.idle && !this.playLanding)
		{
			if (this.cState.attacking)
			{
				this.playSlashLand = true;
				this.canceledSlash = true;
			}
			else
			{
				this.playLanding = true;
			}
			this.playMantleCancel = false;
			this.playBackflip = false;
			this.playSuperJumpFall = false;
			this.playDashUpperRecovery = false;
			this.cState.mantleRecovery = false;
			this.cState.downSpikeRecovery = false;
		}
		if (this.actorState == ActorStates.airborne && newState == ActorStates.running)
		{
			this.didJustLand = true;
			if (this.cState.attacking)
			{
				this.skipIdleToRun = true;
				this.playSlashLand = true;
				this.canceledSlash = true;
			}
			else
			{
				this.skipIdleToRun = false;
				this.idleToRunShort = false;
			}
		}
		ActorStates actorState = this.actorState;
		if ((actorState == ActorStates.idle || actorState == ActorStates.running) && newState == ActorStates.airborne)
		{
			this.playSlashLand = false;
			this.playSlashEnd = false;
			this.canceledSlash = true;
			this.playSprintToRun = false;
			this.playDashToIdle = false;
			this.playRunToIdle = false;
			this.playLanding = false;
		}
		if (this.actorState == ActorStates.idle && newState != ActorStates.idle)
		{
			this.playSlashEnd = false;
		}
		if (this.actorState == ActorStates.running && newState == ActorStates.idle && !this.playRunToIdle && !this.cState.attacking && !this.cState.downSpikeRecovery && !this.cState.isToolThrowing)
		{
			this.SetPlayRunToIdle();
			this.cState.mantleRecovery = false;
			this.playMantleCancel = false;
			this.playBackflip = false;
		}
		actorState = this.actorState;
		if ((actorState == ActorStates.idle || actorState == ActorStates.running) && newState != ActorStates.idle && newState != ActorStates.running)
		{
			this.cState.mantleRecovery = false;
			this.playMantleCancel = false;
			this.playBackflip = false;
		}
		if (newState == ActorStates.hard_landing)
		{
			this.playSlashLand = false;
			this.playSlashEnd = false;
		}
		if (newState == ActorStates.idle)
		{
			this.nextIdleLookTime = Random.Range(4f, 10f);
		}
		this.prevActorState = this.actorState;
		this.actorState = newState;
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00013646 File Offset: 0x00011846
	public void PlayClip(string clipName)
	{
		if (!this.controlEnabled)
		{
			return;
		}
		this.PlayClipForced(clipName);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00013658 File Offset: 0x00011858
	public void PlayClipForced(string clipName)
	{
		this.animEventsTriggered = 0;
		if (!(clipName == "Exit Door To Idle"))
		{
			if (clipName == "Wake Up Ground")
			{
				this.animator.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
			}
		}
		else
		{
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
		}
		this.Play(clipName, 1f);
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x000136C4 File Offset: 0x000118C4
	private void UpdateAnimation()
	{
		this.IsPlayingUpdraftAnim = false;
		this.IsPlayingWindyAnim = false;
		if (this.playLanding)
		{
			this.PlayLand();
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			this.playLanding = false;
		}
		if (this.playRunToIdle)
		{
			if (this.cState.inWalkZone)
			{
				this.Play("Walk To Idle", 1f);
			}
			else
			{
				this.Play("Run To Idle", 1f);
			}
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			this.playRunToIdle = false;
		}
		if (this.playBackDashToIdleEnd)
		{
			this.Play("Backdash Land 2", 1f);
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			this.playBackDashToIdleEnd = false;
		}
		if (this.playDashToIdle)
		{
			this.Play("Dash To Idle", 1f);
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			this.playDashToIdle = false;
		}
		if (this.playSuperJumpFall)
		{
			this.Play("Super Jump Fall", 1f);
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			this.playSuperJumpFall = false;
		}
		if (this.playDashUpperRecovery)
		{
			if (this.CanPlayDashUpperRecovery())
			{
				this.Play("Dash Upper Recovery", 1f);
				this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			}
			this.playDashUpperRecovery = false;
		}
		if (this.playSilkChargeEnd)
		{
			this.Play("Silk Charge End", 1f);
			this.playSilkChargeEnd = false;
		}
		if (this.actorState == ActorStates.no_input)
		{
			if (this.cState.recoilFrozen)
			{
				this.Play("Stun", 1f);
			}
			else if (this.cState.recoiling)
			{
				this.Play("Recoil", 1f);
			}
			else if (this.cState.transitioning)
			{
				if (this.cState.onGround)
				{
					if (this.heroCtrl.transitionState == HeroTransitionState.EXITING_SCENE)
					{
						if (!this.UpdateCheckIsPlayingRun() && !this.animator.IsPlaying("Dash") && !this.animator.IsPlaying("Idle To Run") && !this.animator.IsPlaying("Idle To Run Short") && !this.animator.IsPlaying("Land To Run") && !this.animator.IsPlaying("Idle To Run Weak"))
						{
							this.PlayRun();
						}
					}
					else if (this.heroCtrl.transitionState == HeroTransitionState.ENTERING_SCENE && !this.UpdateCheckIsPlayingRun())
					{
						this.PlayRun();
					}
				}
				else
				{
					HeroTransitionState transitionState = this.heroCtrl.transitionState;
					if (transitionState - HeroTransitionState.EXITING_SCENE > 1)
					{
						if (transitionState == HeroTransitionState.ENTERING_SCENE)
						{
							if (!this.setEntryAnim)
							{
								GatePosition gatePosition = this.heroCtrl.gatePosition;
								if (gatePosition != GatePosition.top)
								{
									if (gatePosition == GatePosition.bottom)
									{
										this.PlayFromFrame("Airborne", 3, true);
									}
								}
								else if (this.heroCtrl.dashingDown)
								{
									this.Play("Dash Down", 1f);
								}
								else
								{
									this.PlayFromFrame("Airborne", 7, true);
								}
								this.setEntryAnim = true;
							}
						}
					}
					else if (!this.animator.IsPlaying("Airborne") && !this.animator.IsPlaying("Dash Down") && !this.animator.IsPlaying("Shadow Dash Down"))
					{
						this.PlayFromFrame("Airborne", 7, false);
					}
				}
			}
			if (!this.wasJumping && this.cState.jumping)
			{
				this.wantsToJump = true;
			}
		}
		else if (this.setEntryAnim)
		{
			this.setEntryAnim = false;
			if (!this.wasJumping && this.cState.jumping)
			{
				this.wantsToJump = true;
			}
		}
		else if (this.cState.dashing)
		{
			if (this.heroCtrl.dashingDown)
			{
				if (this.cState.shadowDashing)
				{
					this.Play("Shadow Dash Down", 1f);
				}
				else
				{
					this.Play("Dash Down", 1f);
				}
			}
			else if (this.cState.shadowDashing)
			{
				this.Play("Shadow Dash", 1f);
			}
			else if (this.cState.airDashing)
			{
				this.Play("Air Dash", 1f);
			}
			else
			{
				this.Play("Dash", 1f);
			}
		}
		else if (this.cState.backDashing)
		{
			this.Play("Back Dash", 1f);
		}
		else if (this.playSlashLand && this.cState.attackCount == this.previousAttackCount && this.cState.toolThrowCount == this.previousToolThrowCount && !this.cState.jumping)
		{
			this.PlaySlashLand();
			this.isPlayingSlashLand = true;
		}
		else if (this.cState.downSpikeAntic)
		{
			this.playSlashEnd = false;
			this.Play("DownSpike Antic", 1f);
		}
		else if (this.cState.downSpikeBouncing)
		{
			this.playSlashEnd = false;
			if (!this.playedDownSpikeBounce)
			{
				if (this.cState.downSpikeBouncingShort)
				{
					this.Play("DownSpikeBounce 1", 1f);
					this.cState.downSpikeBouncingShort = false;
				}
				else
				{
					this.Play(Probability.GetRandomItemByProbabilityFair<HeroAnimationController.ProbabilityString, string>(this.downspikeAnims, ref this.downspikeAnimProbabilities, 2f), 1f);
				}
				this.playedDownSpikeBounce = true;
			}
		}
		else if (this.cState.downSpiking)
		{
			this.playSlashEnd = false;
			this.Play("DownSpike", 1f);
		}
		else if (this.cState.attacking && ((!this.canceledSlash && !this.playSlashEnd && !this.playSlashLand && !this.IsPlayingAirRecovery()) || this.cState.attackCount != this.previousAttackCount))
		{
			this.canceledSlash = false;
			this.playSlashLand = false;
			this.playSlashEnd = false;
			this.playMantleCancel = false;
			this.playBackflip = false;
			this.cState.mantleRecovery = false;
			float speedMultiplier = this.heroCtrl.IsUsingQuickening ? this.heroCtrl.Config.QuickAttackSpeedMult : 1f;
			if (this.cState.upAttacking)
			{
				this.Play(this.cState.altAttack ? "UpSlashAlt" : "UpSlash", speedMultiplier);
			}
			else if (this.cState.downAttacking)
			{
				this.Play(this.cState.altAttack ? "DownSlashAlt" : "DownSlash", speedMultiplier);
			}
			else if (this.cState.wallSliding)
			{
				this.Play("Wall Slash", speedMultiplier);
			}
			else
			{
				this.Play(this.cState.altAttack ? "SlashAlt" : "Slash", speedMultiplier);
			}
		}
		else if (this.cState.isToolThrowing && (!this.canceledSlash || this.cState.toolThrowCount != this.previousToolThrowCount))
		{
			string text = null;
			if (this.cState.wallSliding)
			{
				text = "ToolThrow Wall";
			}
			else if (this.cState.throwingToolVertical == 0)
			{
				text = ((this.cState.toolThrowCount % 2 == 0) ? "ToolThrowAlt Q" : "ToolThrow Q");
			}
			else if (this.cState.throwingToolVertical > 0)
			{
				text = "ToolThrow Up";
			}
			else
			{
				Debug.LogError("ToolThrow Down anim not implemented");
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (this.cState.toolThrowCount != this.previousToolThrowCount)
				{
					this.PlayFromFrame(text, 0, true);
				}
				else
				{
					this.Play(text, 1f);
				}
			}
			this.previousToolThrowCount = this.cState.toolThrowCount;
			this.canceledSlash = false;
			this.playSlashLand = false;
		}
		else if (this.cState.shuttleCock)
		{
			this.Play("Shuttlecock", 1f);
		}
		else if (this.cState.floating)
		{
			this.Play("Float", 1f);
		}
		else if (this.cState.wallClinging)
		{
			this.PlayFromFrame("Wall Cling", this.wasWallClinging ? 2 : 0, false);
			this.playSlashEnd = false;
			this.playMantleCancel = false;
			this.playBackflip = false;
		}
		else if (this.cState.wallSliding)
		{
			this.PlayFromFrame("Wall Slide", this.wasWallSliding ? 2 : 0, false);
			this.playSlashEnd = false;
			this.playMantleCancel = false;
			this.playBackflip = false;
		}
		else if (this.cState.downSpikeRecovery && !this.playedDownSpikeBounce && !this.cState.doubleJumping)
		{
			this.Play(this.cState.onGround ? "Downspike Recovery Land" : "Downspike Recovery", 1f);
		}
		else if (this.cState.casting)
		{
			this.Play("Fireball", 1f);
		}
		else if (this.actorState == ActorStates.idle)
		{
			if (this.cState.lookingUpAnim && !this.animator.IsPlaying("LookUp") && !this.animator.IsPlaying("LookUp Updraft") && !this.animator.IsPlaying("LookUp Windy"))
			{
				this.PlayLookUp();
			}
			else if (this.CanPlayLookDown())
			{
				this.PlayLookDown();
			}
			else if (!this.cState.lookingUpAnim && !this.cState.lookingDownAnim && this.CanPlayIdle())
			{
				this.PlayIdle();
			}
		}
		else if (this.actorState == ActorStates.running)
		{
			if (!this.IsPlayingTurn() && !this.animator.IsPlaying("TurnWalk"))
			{
				if (this.cState.inWalkZone)
				{
					if (this.didJustLand)
					{
						this.Play("Land To Walk", 1f);
					}
					else if (!this.wasInWalkZone)
					{
						if (this.IsHurt())
						{
							this.Play("Run To Walk Weak", 1f);
						}
						else
						{
							this.Play("Run To Walk", 1f);
						}
					}
					else if (!this.animator.IsPlaying("Run To Walk") && !this.animator.IsPlaying("Land To Walk") && !this.animator.IsPlaying("Run To Walk Weak"))
					{
						if (this.IsHurt())
						{
							string text2 = "Weak Walk Faster";
							if (!this.animator.IsPlaying(text2))
							{
								this.skipIdleToRun = false;
								if (this.animator.CurrentClip.name == "Run To Walk")
								{
									this.PlayFromFrame(text2, 1, false);
								}
								else
								{
									this.Play(text2, 1f);
								}
							}
						}
						else
						{
							string text3 = this.heroCtrl.IsUsingQuickening ? "Walk Q" : "Walk";
							if (!this.animator.IsPlaying(text3))
							{
								this.skipIdleToRun = false;
								if (this.animator.CurrentClip.name == "Run To Walk")
								{
									this.PlayFromFrame(text3, 1, false);
								}
								else
								{
									this.Play(text3, 1f);
								}
							}
						}
					}
				}
				else if (!this.animator.IsPlaying("Mantle Land"))
				{
					this.PlayRun();
				}
			}
		}
		else if (this.actorState == ActorStates.airborne && !this.animator.IsPlaying("Slash") && !this.animator.IsPlaying("SlashAlt"))
		{
			string text4 = this.wallJumpedFromScramble ? "Walljump Somersault" : "Walljump";
			this.isPlayingSlashLand = false;
			this.playSlashEnd = false;
			this.playSlashLand = false;
			if (this.heroCtrl.wallLocked || (!this.cState.doubleJumping && this.wallJumpedFromScramble && this.animator.IsPlaying(text4)))
			{
				if (this.justWallJumped)
				{
					this.PlayFromFrame(text4, 0, true);
				}
				else
				{
					this.Play(text4, 1f);
				}
				this.playSlashEnd = false;
			}
			else if (this.cState.doubleJumping || this.playingDoubleJump)
			{
				if (!this.animator.IsPlaying("Double Jump"))
				{
					this.Play("Double Jump", 1f);
					this.playingDoubleJump = true;
				}
				else if (this.canForceDoubleJump)
				{
					this.PlayFromFrame("Double Jump", 0, true);
				}
				this.canForceDoubleJump = false;
			}
			else if (this.playingDownDashEnd)
			{
				if (!this.animator.IsPlaying("Dash Down End"))
				{
					this.Play("Dash Down End", 1f);
					this.playingDownDashEnd = true;
				}
			}
			else if (this.cState.jumping)
			{
				this.playSlashLand = false;
				this.playSlashEnd = false;
				if (!this.TryPlayMantleCancelJump())
				{
					this.PlayFromFrame("Airborne", 0, !this.wasJumping || this.wantsToJump);
				}
			}
			else if (this.cState.falling || this.startWithFallAnim)
			{
				if (!this.TryPlayMantleCancelJump() && !this.animator.IsPlaying("Super Jump Fall") && !this.animator.IsPlaying("Silk Charge End"))
				{
					bool flag = this.IsPlayingAirRecovery();
					if (!this.animator.IsPlaying("Airborne") && !flag)
					{
						this.PlayFromFrame("Airborne", this.wasPlayingAirRecovery ? 9 : 7, false);
					}
					this.wasPlayingAirRecovery = flag;
					if (this.startWithFallAnim)
					{
						this.startWithFallAnim = false;
					}
				}
			}
			else if (!this.TryPlayMantleCancelJump() && !this.animator.IsPlaying("Super Jump Fall") && !this.animator.IsPlaying("Silk Charge End") && !this.animator.IsPlaying("Airborne") && !this.IsPlayingAirRecovery())
			{
				this.PlayFromFrame("Airborne", 3, false);
			}
		}
		else if (this.actorState == ActorStates.dash_landing)
		{
			this.Play("Dash Down Land", 1f);
		}
		else if (this.actorState == ActorStates.hard_landing)
		{
			this.Play("HardLand", 1f);
		}
		if (this.cState.facingRight)
		{
			if (!this.wasFacingRight && this.cState.onGround && this.CanPlayTurn())
			{
				this.PlayTurn();
			}
			this.wasFacingRight = true;
		}
		else
		{
			if (this.wasFacingRight && this.cState.onGround && this.CanPlayTurn())
			{
				this.PlayTurn();
			}
			this.wasFacingRight = false;
		}
		if (!this.cState.downSpikeBouncing && !this.cState.downSpikeRecovery && this.playedDownSpikeBounce)
		{
			this.playedDownSpikeBounce = false;
		}
		this.previousAttackCount = this.cState.attackCount;
		this.ResetPlays();
		this.wasJumping = this.cState.jumping;
		this.wasWallSliding = this.cState.wallSliding;
		this.wasWallClinging = this.cState.wallClinging;
		this.justWallJumped = false;
		this.wasInWalkZone = this.cState.inWalkZone;
		this.didJustLand = false;
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x00014610 File Offset: 0x00012810
	public bool IsPlayingTurn()
	{
		return this.animator.IsPlaying("Turn");
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x00014624 File Offset: 0x00012824
	private bool TryPlayMantleCancelJump()
	{
		if (this.cState.mantleRecovery && (this.playMantleCancel || this.playBackflip))
		{
			if (!this.cState.onGround)
			{
				if (this.selectedMantleCancelJumpAnim == null)
				{
					if (this.playBackflip)
					{
						this.selectedMantleCancelJumpAnim = "Sprint Backflip";
					}
					else
					{
						this.selectedMantleCancelJumpAnim = ((this.cState.facingRight == this.wasFacingRightWhenStopped) ? "Mantle Cancel To Jump" : "Mantle Cancel To Jump Backwards");
					}
				}
				this.playSlashLand = false;
				this.Play(this.selectedMantleCancelJumpAnim, 1f);
				this.checkMantleCancel = true;
			}
			else
			{
				this.cState.mantleRecovery = false;
				this.selectedMantleCancelJumpAnim = null;
				this.playMantleCancel = false;
				this.playBackflip = false;
			}
			return true;
		}
		return false;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x000146E8 File Offset: 0x000128E8
	private void PlaySlashLand()
	{
		this.Play((this.actorState == ActorStates.running) ? (this.cState.altAttack ? "Slash Land Run Alt" : "Slash Land Run") : "Slash Land", 1f);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00014720 File Offset: 0x00012920
	private bool IsPlayingAirRecovery()
	{
		foreach (HeroAnimationController.ProbabilityString probabilityString in this.downspikeAnims)
		{
			if (this.animator.IsPlaying(probabilityString.Item))
			{
				return true;
			}
		}
		return this.animator.IsPlaying("Dash Upper Recovery");
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00014770 File Offset: 0x00012970
	private bool CanPlayIdle()
	{
		return !this.isPlayingSlashLand && !this.animator.IsPlaying("Land") && !this.animator.IsPlaying("Land Q") && !this.animator.IsPlaying("Run To Idle") && !this.animator.IsPlaying("Walk To Idle") && !this.animator.IsPlaying("Dash To Idle") && !this.animator.IsPlaying("Backdash Land") && !this.animator.IsPlaying("Backdash Land 2") && !this.animator.IsPlaying("LookUpEnd") && !this.animator.IsPlaying("LookUpEnd Windy") && !this.animator.IsPlaying("Look Up Half End") && !this.animator.IsPlaying("LookDown Slight End") && !this.animator.IsPlaying("LookDownEnd") && !this.animator.IsPlaying("LookDownEnd Windy") && !this.animator.IsPlaying("Exit Door To Idle") && !this.animator.IsPlaying("Wake Up Ground") && !this.animator.IsPlaying("Hazard Respawn") && !this.animator.IsPlaying("Hurt Look Up Windy End") && !this.animator.IsPlaying("Hurt Look Up End") && !this.animator.IsPlaying("Hurt Look Down Windy End") && !this.IsPlayingAirRecovery();
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00014910 File Offset: 0x00012B10
	private bool CanPlayLookDown()
	{
		return this.cState.lookingDownAnim && !this.animator.IsPlaying("LookDown") && !this.animator.IsPlaying("LookDown Updraft") && !this.animator.IsPlaying("LookDown Windy") && !this.IsPlayingAirRecovery();
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0001496B File Offset: 0x00012B6B
	private bool CanPlayTurn()
	{
		return !this.animator.IsPlaying("Wake Up Ground") && !this.animator.IsPlaying("Hazard Respawn") && !this.wasWallSliding;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x0001499C File Offset: 0x00012B9C
	private bool CanPlayDashUpperRecovery()
	{
		tk2dSpriteAnimationClip currentClip = this.animator.CurrentClip;
		return currentClip != null && (currentClip.name == "Umbrella Deflate" || currentClip.name == "Dash Upper");
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x000149E0 File Offset: 0x00012BE0
	private void PlayTurn()
	{
		this.playSlashLand = false;
		if (this.animator.IsPlaying("Turn") || this.animator.IsPlaying("TurnWalk"))
		{
			return;
		}
		this.Play(this.cState.inWalkZone ? "TurnWalk" : "Turn", 1f);
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00014A3D File Offset: 0x00012C3D
	private void PlayLand()
	{
		this.Play((this.IsHurt() || this.IsInRage()) ? "Land Q" : "Land", 1f);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00014A68 File Offset: 0x00012C68
	private void AnimationEventTriggered(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frame)
	{
		if (clip.name == "Wake Up Ground")
		{
			int num = this.animEventsTriggered;
			AudioEvent audioEvent;
			if (num != 0)
			{
				if (num != 1)
				{
					return;
				}
				if (this.heroCtrl.Config.ForceBareInventory)
				{
					return;
				}
				audioEvent = this.wakeUpGround2;
			}
			else
			{
				audioEvent = (this.heroCtrl.Config.ForceBareInventory ? this.wakeUpGroundCloakless : this.wakeUpGround1);
			}
			audioEvent.SpawnAndPlayOneShot(base.transform.position, null);
			this.animEventsTriggered++;
			return;
		}
		if (clip.frames[frame].eventInfo == "Footstep")
		{
			if (this.meshRenderer.enabled)
			{
				this.audioCtrl.PlayFootstep();
				return;
			}
		}
		else if (clip.name == "Sprint Backflip")
		{
			this.backflipSpawnedAudio = this.backflipSpin.SpawnAndPlayLooped(null, base.transform.position, 0f, this.clearBackflipSpawnedAudio);
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00014B64 File Offset: 0x00012D64
	private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		if (this.isPlayingSlashLand)
		{
			this.isPlayingSlashLand = false;
			this.playSlashEnd = true;
			this.canceledSlash = true;
			this.skipIdleToRun = true;
			if (this.cState.onGround)
			{
				this.playRunToIdle = true;
			}
		}
		string name = clip.name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num > 2041425783U)
		{
			if (num <= 3424407707U)
			{
				if (num <= 2958486913U)
				{
					if (num <= 2482372231U)
					{
						if (num != 2355424947U)
						{
							if (num != 2482372231U)
							{
								return;
							}
							if (!(name == "Backdash To Idle"))
							{
								return;
							}
						}
						else
						{
							if (!(name == "Idle To Run Weak"))
							{
								return;
							}
							goto IL_4A0;
						}
					}
					else if (num != 2642932242U)
					{
						if (num != 2958486913U)
						{
							return;
						}
						if (!(name == "Super Jump Fall"))
						{
							return;
						}
						this.PlayFromFrame("Airborne", 8, false);
						return;
					}
					else
					{
						if (!(name == "Slash Land Run"))
						{
							return;
						}
						goto IL_4F1;
					}
				}
				else if (num <= 3216127775U)
				{
					if (num != 3146028055U)
					{
						if (num != 3216127775U)
						{
							return;
						}
						if (!(name == "Land Q"))
						{
							return;
						}
					}
					else
					{
						if (!(name == "Dash To Run"))
						{
							return;
						}
						this.PlayFromFrame(this.GetRunAnim(), 2, false);
						return;
					}
				}
				else if (num != 3301589315U)
				{
					if (num != 3360930981U)
					{
						if (num != 3424407707U)
						{
							return;
						}
						if (!(name == "Idle Rest"))
						{
							return;
						}
						this.ResetIdleLook();
						return;
					}
					else
					{
						if (!(name == "Mantle Cancel To Jump Backwards"))
						{
							return;
						}
						goto IL_4F9;
					}
				}
				else
				{
					if (!(name == "Sprint Backflip"))
					{
						return;
					}
					goto IL_4F9;
				}
			}
			else if (num <= 3876503808U)
			{
				if (num <= 3709345263U)
				{
					if (num != 3669758097U)
					{
						if (num != 3709345263U)
						{
							return;
						}
						if (!(name == "Run To Idle"))
						{
							return;
						}
					}
					else if (!(name == "Walk To Idle"))
					{
						return;
					}
				}
				else if (num != 3762484447U)
				{
					if (num != 3876503808U)
					{
						return;
					}
					if (!(name == "Land To Run"))
					{
						return;
					}
					goto IL_4A0;
				}
				else
				{
					if (!(name == "Slash Land"))
					{
						return;
					}
					goto IL_4F1;
				}
			}
			else if (num <= 4029933209U)
			{
				if (num != 3994452668U)
				{
					if (num != 4029933209U)
					{
						return;
					}
					if (!(name == "UpSlash"))
					{
						return;
					}
					goto IL_4B9;
				}
				else if (!(name == "Land"))
				{
					return;
				}
			}
			else if (num != 4051213062U)
			{
				if (num != 4140371785U)
				{
					if (num != 4290210248U)
					{
						return;
					}
					if (!(name == "Dash To Idle"))
					{
						return;
					}
				}
				else
				{
					if (!(name == "SlashAlt"))
					{
						return;
					}
					goto IL_4B9;
				}
			}
			else if (!(name == "Exit Door To Idle"))
			{
				return;
			}
			this.PlayIdle();
			return;
		}
		if (num <= 393897062U)
		{
			if (num <= 294540514U)
			{
				if (num <= 250109124U)
				{
					if (num != 169357623U)
					{
						if (num != 250109124U)
						{
							return;
						}
						if (!(name == "Double Jump"))
						{
							return;
						}
						this.playingDoubleJump = false;
						return;
					}
					else
					{
						if (!(name == "Mantle Cancel To Jump"))
						{
							return;
						}
						goto IL_4F9;
					}
				}
				else if (num != 260547933U)
				{
					if (num != 294540514U)
					{
						return;
					}
					if (!(name == "Mantle Land To Idle"))
					{
						return;
					}
					goto IL_4F9;
				}
				else
				{
					if (!(name == "Slash Land Run Alt"))
					{
						return;
					}
					goto IL_4F1;
				}
			}
			else if (num <= 314774315U)
			{
				if (num != 310717262U)
				{
					if (num != 314774315U)
					{
						return;
					}
					if (!(name == "ToolThrowAlt Q"))
					{
						return;
					}
					goto IL_4B9;
				}
				else
				{
					if (!(name == "ToolThrow Up"))
					{
						return;
					}
					goto IL_4B9;
				}
			}
			else if (num != 325523764U)
			{
				if (num != 393897062U)
				{
					return;
				}
				if (!(name == "Dash Down End"))
				{
					return;
				}
				this.playingDownDashEnd = false;
				return;
			}
			else if (!(name == "Sprint Skid To Run"))
			{
				return;
			}
		}
		else if (num <= 898653561U)
		{
			if (num <= 751824102U)
			{
				if (num != 464120318U)
				{
					if (num != 751824102U)
					{
						return;
					}
					if (!(name == "Slash To Run"))
					{
						return;
					}
				}
				else
				{
					if (!(name == "ToolThrow Q"))
					{
						return;
					}
					goto IL_4B9;
				}
			}
			else if (num != 762071205U)
			{
				if (num != 898653561U)
				{
					return;
				}
				if (!(name == "Downspike Recovery Land"))
				{
					return;
				}
				this.SetPlayRunToIdle();
				this.idleToRunShort = true;
				return;
			}
			else
			{
				if (!(name == "Mantle Land To Run"))
				{
					return;
				}
				this.cState.mantleRecovery = false;
				this.skipIdleToRun = true;
				return;
			}
		}
		else if (num <= 1289709245U)
		{
			if (num != 1081430095U)
			{
				if (num != 1289709245U)
				{
					return;
				}
				if (!(name == "Idle To Run"))
				{
					return;
				}
			}
			else if (!(name == "Idle To Run Short"))
			{
				return;
			}
		}
		else if (num != 1898928778U)
		{
			if (num != 1932569431U)
			{
				if (num != 2041425783U)
				{
					return;
				}
				if (!(name == "Sprint To Run"))
				{
					return;
				}
			}
			else
			{
				if (!(name == "Rage Idle End"))
				{
					return;
				}
				this.PlayFromFrame("Run To Idle", 1, false);
				return;
			}
		}
		else
		{
			if (!(name == "Slash"))
			{
				return;
			}
			goto IL_4B9;
		}
		IL_4A0:
		this.Play(this.GetRunAnim(), 1f);
		return;
		IL_4B9:
		if (this.canceledSlash)
		{
			return;
		}
		this.playSlashEnd = true;
		this.canceledSlash = true;
		this.skipIdleToRun = true;
		if (this.cState.onGround)
		{
			this.playRunToIdle = true;
			return;
		}
		return;
		IL_4F1:
		this.playSlashLand = false;
		return;
		IL_4F9:
		this.cState.mantleRecovery = false;
		this.selectedMantleCancelJumpAnim = null;
		this.playBackflip = false;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x000150E1 File Offset: 0x000132E1
	public bool IsHurt()
	{
		return (this.pd.health == 1 && this.pd.healthBlue < 1) || this.isCursed || this.cState.isMaggoted || this.cState.fakeHurt;
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00015121 File Offset: 0x00013321
	private bool IsInRage()
	{
		return this.heroCtrl.WarriorState.IsInRageMode;
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x00015134 File Offset: 0x00013334
	public bool CurrentClipNameContains(string clipName)
	{
		if (string.IsNullOrEmpty(clipName))
		{
			return false;
		}
		tk2dSpriteAnimationClip currentClip = this.animator.CurrentClip;
		return currentClip != null && currentClip.name.Contains(clipName);
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00015168 File Offset: 0x00013368
	public void PlayIdle()
	{
		if (this.cState.mantleRecovery)
		{
			if (this.cState.onGround)
			{
				this.Play("Mantle Land To Idle", 1f);
			}
			else
			{
				this.cState.mantleRecovery = false;
			}
		}
		else if (this.IsHurt())
		{
			if (this.animator.IsPlaying("Hurt Look Up") || this.animator.IsPlaying("Hurt Look Up Windy") || this.CurrentClipNameContains("Hurt Listen Up"))
			{
				if (this.cState.inUpdraft)
				{
					this.Play("Hurt Look Up Windy End", 1f);
					this.IsPlayingUpdraftAnim = true;
				}
				else if (this.cState.inWindRegion)
				{
					this.Play("Hurt Look Up Windy End", 1f);
					this.IsPlayingWindyAnim = true;
				}
				else
				{
					this.Play("Hurt Look Up End", 1f);
				}
			}
			else if (this.animator.IsPlaying("Hurt Look Down Windy"))
			{
				if (this.cState.inUpdraft)
				{
					this.Play("Hurt Look Down Windy End", 1f);
					this.IsPlayingUpdraftAnim = true;
				}
				else if (this.cState.inWindRegion)
				{
					this.Play("Hurt Look Down Windy End", 1f);
					this.IsPlayingWindyAnim = true;
				}
				else
				{
					this.PlayFromLoopPoint("Idle Hurt Windy", false);
				}
			}
			else if (this.cState.inUpdraft)
			{
				if (this.animator.IsPlaying("Idle Hurt Listen Windy") || this.animator.IsPlaying("Idle Hurt Talk Windy") || this.CurrentClipNameContains("Hurt Look") || this.animator.IsPlaying("Hurt Listen Down Windy"))
				{
					this.PlayFromLoopPoint("Idle Hurt Windy", false);
				}
				else
				{
					this.Play("Idle Hurt Windy", 1f);
				}
				this.IsPlayingUpdraftAnim = true;
			}
			else if (this.cState.inWindRegion)
			{
				if (this.animator.IsPlaying("Idle Hurt Listen Windy") || this.animator.IsPlaying("Idle Hurt Talk Windy") || this.CurrentClipNameContains("Hurt Look") || this.animator.IsPlaying("Hurt Listen Down Windy"))
				{
					this.PlayFromLoopPoint("Idle Hurt Windy", false);
				}
				else
				{
					this.Play("Idle Hurt Windy", 1f);
				}
				this.IsPlayingWindyAnim = true;
			}
			else if (this.animator.IsPlaying("Idle Hurt Listen") || this.animator.IsPlaying("Idle Hurt Talk") || this.CurrentClipNameContains("Hurt Look") || this.animator.IsPlaying("Hurt Listen Down") || this.CurrentClipNameContains("Weak Walk"))
			{
				this.PlayFromLoopPoint("Idle Hurt", false);
			}
			else
			{
				this.Play("Idle Hurt", 1f);
			}
			this.IsPlayingHurtAnim = true;
		}
		else if (this.IsInRage())
		{
			this.Play("Rage Idle", 1f);
		}
		else if (this.animator.IsPlaying("Rage Idle") || this.animator.IsPlaying("Rage Idle End"))
		{
			this.Play("Rage Idle End", 1f);
			this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
		}
		else if (this.animator.IsPlaying("LookUp") || this.animator.IsPlaying("LookUp Updraft") || this.animator.IsPlaying("LookUp Windy"))
		{
			if (this.cState.inUpdraft)
			{
				this.Play("LookUpEnd Updraft", 1f);
				this.IsPlayingUpdraftAnim = true;
			}
			else if (this.cState.inWindRegion)
			{
				this.Play("LookUpEnd Windy", 1f);
				this.IsPlayingWindyAnim = true;
			}
			else
			{
				this.Play("LookUpEnd", 1f);
			}
		}
		else if (this.animator.IsPlaying("LookDown") || this.animator.IsPlaying("LookDown Updraft") || this.animator.IsPlaying("LookDown Windy"))
		{
			if (this.cState.inUpdraft)
			{
				this.Play("LookDownEnd Updraft", 1f);
				this.IsPlayingUpdraftAnim = true;
			}
			else if (this.cState.inWindRegion)
			{
				this.Play("LookDownEnd Windy", 1f);
				this.IsPlayingWindyAnim = true;
			}
			else
			{
				this.Play("LookDownEnd", 1f);
			}
		}
		else if (this.animator.IsPlaying("Look Up Half"))
		{
			this.Play("Look Up Half End", 1f);
		}
		else if (this.animator.IsPlaying("LookDown Slight"))
		{
			this.Play("LookDown Slight End", 1f);
		}
		else if (this.animator.IsPlaying("Downspike Recovery Land"))
		{
			this.Play("Run To Idle", 1f);
		}
		else if (this.cState.inUpdraft)
		{
			this.Play("Idle Updraft", 1f);
			this.IsPlayingUpdraftAnim = true;
		}
		else if (this.cState.inWindRegion)
		{
			this.Play("Idle Windy", 1f);
			this.IsPlayingWindyAnim = true;
		}
		else
		{
			if (this.heroCtrl.controlReqlinquished || !this.meshRenderer.enabled)
			{
				this.ResetIdleLook();
			}
			else
			{
				this.nextIdleLookTime -= Time.deltaTime;
			}
			if (this.nextIdleLookTime <= 0f)
			{
				this.Play("Idle Rest", 1f);
				this.playingIdleRest = true;
				this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			}
			else
			{
				this.Play("Idle", 1f);
			}
		}
		this.skipIdleToRun = false;
		this.idleToRunShort = false;
		this.playSprintToRun = false;
		this.didSkid = false;
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x00015744 File Offset: 0x00013944
	public void PlayLookUp()
	{
		this.cState.mantleRecovery = false;
		if (this.IsHurt())
		{
			if (this.cState.inUpdraft)
			{
				this.Play("Hurt Look Up Windy", 1f);
				this.IsPlayingUpdraftAnim = true;
			}
			else if (this.cState.inWindRegion)
			{
				this.Play("Hurt Look Up Windy", 1f);
				this.IsPlayingWindyAnim = true;
			}
			else
			{
				this.Play("Hurt Look Up", 1f);
			}
			this.IsPlayingHurtAnim = true;
		}
		else if (this.cState.inUpdraft)
		{
			this.Play("LookUp Updraft", 1f);
			this.IsPlayingUpdraftAnim = true;
		}
		else if (this.cState.inWindRegion)
		{
			this.Play("LookUp Windy", 1f);
			this.IsPlayingWindyAnim = true;
		}
		else
		{
			this.Play("LookUp", 1f);
		}
		this.ResetIdleLook();
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0001582C File Offset: 0x00013A2C
	public void PlayLookDown()
	{
		this.cState.mantleRecovery = false;
		if (this.IsHurt())
		{
			if (this.cState.inUpdraft)
			{
				this.Play("Hurt Look Down Windy", 1f);
				this.IsPlayingUpdraftAnim = true;
			}
			else if (this.cState.inWindRegion)
			{
				this.Play("Hurt Look Down Windy", 1f);
				this.IsPlayingWindyAnim = true;
			}
			else
			{
				this.Play("Hurt Look Down", 1f);
			}
			this.IsPlayingHurtAnim = true;
		}
		else if (this.cState.inUpdraft)
		{
			this.Play("LookDown Updraft", 1f);
			this.IsPlayingUpdraftAnim = true;
		}
		else if (this.cState.inWindRegion)
		{
			this.Play("LookDown Windy", 1f);
			this.IsPlayingWindyAnim = true;
		}
		else
		{
			this.Play("LookDown", 1f);
		}
		this.ResetIdleLook();
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00015914 File Offset: 0x00013B14
	private void PlayRun()
	{
		if (this.cState.mantleRecovery)
		{
			if (this.cState.onGround)
			{
				this.Play("Mantle Land To Run", 1f);
			}
			else
			{
				this.cState.mantleRecovery = false;
			}
		}
		else
		{
			if (this.isPlayingSlashLand)
			{
				return;
			}
			if (this.playSlashEnd)
			{
				this.Play("Slash To Run", 1f);
			}
			else if (this.playSprintToRun)
			{
				this.Play(this.didSkid ? "Sprint Skid To Run" : "Sprint To Run", 1f);
			}
			else if (this.animator.IsPlaying("Dash To Idle") || this.animator.IsPlaying("Dash To Run"))
			{
				this.Play("Dash To Run", 1f);
			}
			else if (this.skipIdleToRun)
			{
				this.Play(this.GetRunAnim(), 1f);
			}
			else if (this.didJustLand)
			{
				this.Play("Land To Run", 1f);
			}
			else if (!this.UpdateCheckIsPlayingRun() && !this.animator.IsPlaying("Idle To Run") && !this.animator.IsPlaying("Idle To Run Short") && !this.animator.IsPlaying("Sprint To Run") && !this.animator.IsPlaying("Sprint Skid To Run") && !this.animator.IsPlaying("Land To Run") && !this.animator.IsPlaying("Slash To Run") && !this.animator.IsPlaying("Idle To Run Weak"))
			{
				if (this.wasInWalkZone && this.IsHurt())
				{
					this.Play((this.idleToRunShort || this.animator.IsPlaying("Downspike Recovery Land")) ? "Idle To Run Short" : "Idle To Run Weak", 1f);
				}
				else if (this.CurrentClipNameContains("Land"))
				{
					this.Play("Land To Run", 1f);
				}
				else
				{
					this.Play((this.idleToRunShort || this.animator.IsPlaying("Downspike Recovery Land")) ? "Idle To Run Short" : "Idle To Run", 1f);
				}
			}
		}
		this.skipIdleToRun = false;
		this.idleToRunShort = false;
		this.playSprintToRun = false;
		this.playSlashEnd = false;
		this.didSkid = false;
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00015B7B File Offset: 0x00013D7B
	private string GetRunAnim()
	{
		if (!this.heroCtrl.IsUsingQuickening)
		{
			return "Run";
		}
		return "Run Q";
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00015B98 File Offset: 0x00013D98
	private bool UpdateCheckIsPlayingRun()
	{
		if (!this.animator.IsPlaying("Run") && !this.animator.IsPlaying("Run Q"))
		{
			return false;
		}
		string name = this.animator.CurrentClip.name;
		string runAnim = this.GetRunAnim();
		if (name != runAnim)
		{
			this.Play(runAnim, 1f);
		}
		return true;
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00015BF8 File Offset: 0x00013DF8
	private void Play(string clipName, float speedMultiplier = 1f)
	{
		if (clipName == this.animator.CurrentClip.name)
		{
			return;
		}
		if (this.playingIdleRest)
		{
			this.ResetIdleLook();
		}
		this.ResetPlaying();
		tk2dSpriteAnimationClip clip = this.GetClip(clipName);
		if (!Mathf.Approximately(speedMultiplier, 1f))
		{
			this.animator.Play(clip, 0f, clip.fps * speedMultiplier);
		}
		else
		{
			this.animator.Play(clip);
		}
		this.animator.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
		this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
		if (this.isPlayingSlashLand)
		{
			this.isPlayingSlashLand = false;
			this.playSlashLand = false;
		}
		if (this.checkMantleCancel)
		{
			this.playMantleCancel = false;
			this.playBackflip = false;
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00015CC8 File Offset: 0x00013EC8
	private void PlayFromFrame(string clipName, int frame, bool force = false)
	{
		if (clipName != this.animator.CurrentClip.name || force)
		{
			this.ResetPlaying();
			this.animator.PlayFromFrame(this.GetClip(clipName), frame);
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00015D00 File Offset: 0x00013F00
	private void PlayFromLoopPoint(string clipName, bool force = false)
	{
		if (clipName != this.animator.CurrentClip.name || force)
		{
			this.ResetPlaying();
			tk2dSpriteAnimationClip clip = this.GetClip(clipName);
			this.animator.PlayFromFrame(clip, clip.loopStart);
		}
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x00015D48 File Offset: 0x00013F48
	public void RefreshAnimationEvents()
	{
		if (this.cState.isSprinting)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered));
			tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
			tk2dSpriteAnimator2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered));
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00015DB0 File Offset: 0x00013FB0
	public tk2dSpriteAnimationClip GetClip(string clipName)
	{
		if (this.config)
		{
			tk2dSpriteAnimationClip animationClip = this.config.GetAnimationClip(clipName);
			if (animationClip != null)
			{
				return animationClip;
			}
		}
		if (this.heroCtrl.cState.inWindRegion || this.heroCtrl.cState.inUpdraft)
		{
			tk2dSpriteAnimationClip clipByName = this.windyAnimLib.GetClipByName(clipName);
			if (clipByName != null)
			{
				return clipByName;
			}
		}
		tk2dSpriteAnimationClip clipByName2 = this.animator.GetClipByName(clipName);
		if (clipByName2 == null)
		{
			Debug.LogError(string.Format("Could not resolve animation clip: {0}", clipName), this);
		}
		return clipByName2;
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00015E32 File Offset: 0x00014032
	public void ResetPlaying()
	{
		this.playingDoubleJump = false;
		this.playingDownDashEnd = false;
		this.IsPlayingHurtAnim = false;
		this.playingIdleRest = false;
		if (this.backflipSpawnedAudio)
		{
			this.backflipSpawnedAudio.Stop();
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00015E68 File Offset: 0x00014068
	public void AllowDoubleJumpReEntry()
	{
		this.canForceDoubleJump = true;
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00015E74 File Offset: 0x00014074
	public void StopControl()
	{
		if (this.controlEnabled)
		{
			this.controlEnabled = false;
			this.stateBeforeControl = this.actorState;
			this.cState.mantleRecovery = false;
			this.selectedMantleCancelJumpAnim = null;
			this.playBackflip = false;
			this.wasFacingRightWhenStopped = this.cState.facingRight;
			this.ResetPlaying();
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00015ECD File Offset: 0x000140CD
	public void StartControl()
	{
		this.actorState = this.heroCtrl.hero_state;
		this.ResetAll();
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00015EE6 File Offset: 0x000140E6
	public void StartControlRunning()
	{
		this.StartControl();
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00015EEE File Offset: 0x000140EE
	public void StartControlWithoutSettingState()
	{
		this.controlEnabled = true;
		if (this.stateBeforeControl == ActorStates.running && this.actorState == ActorStates.running)
		{
			this.actorState = ActorStates.idle;
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00015F10 File Offset: 0x00014110
	public void StartControlToIdle()
	{
		this.StartControlToIdle(false);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00015F1C File Offset: 0x0001411C
	public void StartControlToIdle(bool forcePlay)
	{
		this.actorState = this.heroCtrl.hero_state;
		this.controlEnabled = true;
		tk2dSpriteAnimationClip currentClip = this.animator.CurrentClip;
		if (currentClip == null || forcePlay || (currentClip.name != "Idle" && (!currentClip.name.Contains("land", StringComparison.InvariantCultureIgnoreCase) || currentClip.name == "Mantle Land To Run" || currentClip.name == "Downspike Recovery Land")))
		{
			this.SetPlayRunToIdle();
		}
		this.UpdateAnimation();
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00015FAC File Offset: 0x000141AC
	public void StopAttack()
	{
		if (this.animator.IsPlaying("UpSlash") || this.animator.IsPlaying("UpSlashAlt") || this.animator.IsPlaying("DownSlash") || this.animator.IsPlaying("DownSlashAlt"))
		{
			this.animator.Stop();
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0001600C File Offset: 0x0001420C
	public void StopToolThrow()
	{
		if (this.animator.IsPlaying("ToolThrowAlt Q") || this.animator.IsPlaying("ToolThrow Q") || this.animator.IsPlaying("ToolThrow Up"))
		{
			this.animator.Stop();
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x0001605A File Offset: 0x0001425A
	public float GetCurrentClipDuration()
	{
		return (float)this.animator.CurrentClip.frames.Length / this.animator.CurrentClip.fps;
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00016080 File Offset: 0x00014280
	public float GetClipDuration(string clipName)
	{
		if (this.animator == null)
		{
			this.animator = base.GetComponent<tk2dSpriteAnimator>();
		}
		tk2dSpriteAnimationClip clip = this.GetClip(clipName);
		if (clip == null)
		{
			Debug.LogError("HeroAnim: Could not find animation clip with the name " + clipName);
			return -1f;
		}
		return (float)clip.frames.Length / clip.fps;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000160D8 File Offset: 0x000142D8
	public bool IsTurnBlocked()
	{
		return this.animator.IsPlaying("DownSpikeBounce 2") || this.animator.IsPlaying("DownSpike Antic") || this.animator.IsPlaying("DownSpike") || this.playedDownSpikeBounce || this.animator.IsPlaying("Downspike Recovery Land") || this.animator.CurrentClip.name.Equals("Harpoon Catch") || this.animator.IsPlaying("Recoil Twirl") || this.animator.IsPlaying("Slash_Charged") || this.animator.IsPlaying("Super Jump Fall") || this.animator.IsPlaying("Dash Upper Recovery") || this.animator.IsPlaying("Sprint Backflip");
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x000161B7 File Offset: 0x000143B7
	public void FinishedDash()
	{
		this.playDashToIdle = true;
		if (this.actorState == ActorStates.running && !this.heroCtrl.dashingDown)
		{
			this.skipIdleToRun = true;
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x000161E0 File Offset: 0x000143E0
	public void FinishedSprint(bool didSkid)
	{
		this.didSkid = didSkid;
		if (didSkid)
		{
			if (this.actorState == ActorStates.running)
			{
				this.playSprintToRun = true;
				return;
			}
			this.playDashToIdle = true;
			return;
		}
		else
		{
			if (this.actorState == ActorStates.running)
			{
				this.playSprintToRun = true;
				return;
			}
			if (this.actorState == ActorStates.idle && !this.playLanding)
			{
				this.playRunToIdle = true;
			}
			return;
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00016239 File Offset: 0x00014439
	public void SetPlayMantleCancel()
	{
		this.checkMantleCancel = false;
		this.playMantleCancel = true;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00016249 File Offset: 0x00014449
	public void SetPlayBackflip()
	{
		this.checkMantleCancel = false;
		this.playBackflip = true;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00016259 File Offset: 0x00014459
	public void SetPlaySuperJumpFall()
	{
		this.playSuperJumpFall = true;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00016262 File Offset: 0x00014462
	public void SetPlayDashUpperRecovery()
	{
		this.playDashUpperRecovery = true;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x0001626B File Offset: 0x0001446B
	public void SetPlayRunToIdle()
	{
		if (this.IsHurt())
		{
			this.PlayIdle();
			return;
		}
		this.playRunToIdle = true;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00016283 File Offset: 0x00014483
	public void SetWallJumped()
	{
		this.justWallJumped = true;
		this.UpdateWallScramble();
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00016292 File Offset: 0x00014492
	public void UpdateWallScramble()
	{
		this.wallJumpedFromScramble = this.cState.wallScrambling;
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x000162A5 File Offset: 0x000144A5
	public void SetDownDashEnded()
	{
		this.playingDownDashEnd = true;
		this.playDashToIdle = false;
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x000162B5 File Offset: 0x000144B5
	public void SetPlaySilkChargeEnd()
	{
		this.playSilkChargeEnd = true;
	}

	// Token: 0x0400037A RID: 890
	public tk2dSpriteAnimator animator;

	// Token: 0x0400037B RID: 891
	private MeshRenderer meshRenderer;

	// Token: 0x0400037C RID: 892
	private HeroController heroCtrl;

	// Token: 0x0400037D RID: 893
	private HeroControllerStates cState;

	// Token: 0x0400037E RID: 894
	private PlayerData pd;

	// Token: 0x0400037F RID: 895
	private HeroAudioController audioCtrl;

	// Token: 0x04000380 RID: 896
	[SerializeField]
	private tk2dSpriteAnimation windyAnimLib;

	// Token: 0x04000382 RID: 898
	[HideInInspector]
	public bool playLanding;

	// Token: 0x04000383 RID: 899
	private bool _playRunToIdle;

	// Token: 0x04000384 RID: 900
	private bool playRunToIdle;

	// Token: 0x04000385 RID: 901
	private bool playDashToIdle;

	// Token: 0x04000386 RID: 902
	private bool playBackDashToIdleEnd;

	// Token: 0x04000387 RID: 903
	private bool playedDownSpikeBounce;

	// Token: 0x04000388 RID: 904
	private bool justWallJumped;

	// Token: 0x04000389 RID: 905
	private bool wallJumpedFromScramble;

	// Token: 0x0400038A RID: 906
	private bool isPlayingSlashLand;

	// Token: 0x0400038B RID: 907
	private bool playSlashLand;

	// Token: 0x0400038C RID: 908
	private bool playSlashEnd;

	// Token: 0x0400038D RID: 909
	private int previousAttackCount;

	// Token: 0x0400038E RID: 910
	private bool canceledSlash;

	// Token: 0x0400038F RID: 911
	private bool playSilkChargeEnd;

	// Token: 0x04000390 RID: 912
	public bool skipIdleToRun;

	// Token: 0x04000391 RID: 913
	public bool idleToRunShort;

	// Token: 0x04000392 RID: 914
	public bool startWithFallAnim;

	// Token: 0x04000393 RID: 915
	[Space]
	[SerializeField]
	private AudioEvent wakeUpGround1;

	// Token: 0x04000394 RID: 916
	[SerializeField]
	private AudioEvent wakeUpGround2;

	// Token: 0x04000395 RID: 917
	[SerializeField]
	private AudioEvent wakeUpGroundCloakless;

	// Token: 0x04000396 RID: 918
	[SerializeField]
	private AudioEvent backflipSpin;

	// Token: 0x04000397 RID: 919
	private AudioSource backflipSpawnedAudio;

	// Token: 0x04000398 RID: 920
	private Action clearBackflipSpawnedAudio;

	// Token: 0x04000399 RID: 921
	private bool playSprintToRun;

	// Token: 0x0400039A RID: 922
	private bool didSkid;

	// Token: 0x0400039B RID: 923
	private bool wasFacingRight;

	// Token: 0x0400039C RID: 924
	[HideInInspector]
	public bool setEntryAnim;

	// Token: 0x0400039D RID: 925
	private int previousToolThrowCount;

	// Token: 0x0400039E RID: 926
	private int animEventsTriggered;

	// Token: 0x0400039F RID: 927
	private bool attackComplete;

	// Token: 0x040003A0 RID: 928
	private readonly HeroAnimationController.ProbabilityString[] downspikeAnims = new HeroAnimationController.ProbabilityString[]
	{
		new HeroAnimationController.ProbabilityString("DownSpikeBounce 1", 1f),
		new HeroAnimationController.ProbabilityString("DownSpikeBounce 2", 1f),
		new HeroAnimationController.ProbabilityString("Recoil Twirl", 1f)
	};

	// Token: 0x040003A1 RID: 929
	private float[] downspikeAnimProbabilities;

	// Token: 0x040003A2 RID: 930
	private bool wasPlayingAirRecovery;

	// Token: 0x040003A3 RID: 931
	private bool wasJumping;

	// Token: 0x040003A4 RID: 932
	private bool wantsToJump;

	// Token: 0x040003A5 RID: 933
	private bool wasWallSliding;

	// Token: 0x040003A6 RID: 934
	private bool wasWallClinging;

	// Token: 0x040003A7 RID: 935
	private bool wasInWalkZone;

	// Token: 0x040003A8 RID: 936
	private bool didJustLand;

	// Token: 0x040003A9 RID: 937
	private bool playingDoubleJump;

	// Token: 0x040003AA RID: 938
	private bool canForceDoubleJump;

	// Token: 0x040003AB RID: 939
	private bool playingDownDashEnd;

	// Token: 0x040003AC RID: 940
	private bool wasFacingRightWhenStopped;

	// Token: 0x040003AD RID: 941
	private string selectedMantleCancelJumpAnim;

	// Token: 0x040003AE RID: 942
	private bool playMantleCancel;

	// Token: 0x040003AF RID: 943
	private bool checkMantleCancel;

	// Token: 0x040003B0 RID: 944
	private bool playBackflip;

	// Token: 0x040003B1 RID: 945
	private bool playSuperJumpFall;

	// Token: 0x040003B2 RID: 946
	private bool playDashUpperRecovery;

	// Token: 0x040003B3 RID: 947
	private float nextIdleLookTime;

	// Token: 0x040003B4 RID: 948
	private bool playingIdleRest;

	// Token: 0x040003B5 RID: 949
	private HeroControllerConfig config;

	// Token: 0x040003B6 RID: 950
	private bool isCursed;

	// Token: 0x040003BA RID: 954
	public bool waitingToEnter;

	// Token: 0x040003BB RID: 955
	public const string HURT_IDLE_ANIM = "Idle Hurt";

	// Token: 0x020013FF RID: 5119
	private class ProbabilityString : Probability.ProbabilityBase<string>
	{
		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x06008229 RID: 33321 RVA: 0x0026412F File Offset: 0x0026232F
		public override string Item
		{
			get
			{
				return this.item;
			}
		}

		// Token: 0x0600822A RID: 33322 RVA: 0x00264137 File Offset: 0x00262337
		public ProbabilityString(string item, float probability)
		{
			this.item = item;
			this.Probability = probability;
		}

		// Token: 0x04008174 RID: 33140
		private string item;
	}
}
