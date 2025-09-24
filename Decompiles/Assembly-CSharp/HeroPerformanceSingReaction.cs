using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200008C RID: 140
public class HeroPerformanceSingReaction : MonoBehaviour
{
	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000426 RID: 1062 RVA: 0x00016347 File Offset: 0x00014547
	// (set) Token: 0x06000427 RID: 1063 RVA: 0x0001634F File Offset: 0x0001454F
	public bool IsForcedSoft { get; set; }

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000428 RID: 1064 RVA: 0x00016358 File Offset: 0x00014558
	// (set) Token: 0x06000429 RID: 1065 RVA: 0x00016360 File Offset: 0x00014560
	public bool IsForced { get; set; }

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600042A RID: 1066 RVA: 0x00016369 File Offset: 0x00014569
	public bool IsForcedAny
	{
		get
		{
			return this.IsForced || this.IsForcedSoft;
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x0600042B RID: 1067 RVA: 0x0001637B File Offset: 0x0001457B
	private bool ShouldSing
	{
		get
		{
			return this.isInside || this.IsForced;
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00016390 File Offset: 0x00014590
	private bool IsUsingLeftAnims()
	{
		HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours lookAnimNpcActivateBehaviours = this.lookAnimNPCActivate;
		return lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.Any || lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceLeft;
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000163B4 File Offset: 0x000145B4
	private bool IsUsingRightAnims()
	{
		HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours lookAnimNpcActivateBehaviours = this.lookAnimNPCActivate;
		return lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.Any || lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceRight;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x000163D8 File Offset: 0x000145D8
	private void Reset()
	{
		this.npcControl = base.GetComponent<NPCControlBase>();
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x000163E8 File Offset: 0x000145E8
	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.idleAnim))
		{
			this.leftAnims.IdleAnim = this.idleAnim;
			this.rightAnims.IdleAnim = this.idleAnim;
			this.idleAnim = null;
		}
		if (!string.IsNullOrEmpty(this.singAnim))
		{
			this.leftAnims.SingAnim = this.singAnim;
			this.rightAnims.SingAnim = this.singAnim;
			this.singAnim = null;
		}
		if (!string.IsNullOrEmpty(this.returnToIdleAnim))
		{
			this.leftAnims.ReturnToIdleAnim = this.returnToIdleAnim;
			this.rightAnims.ReturnToIdleAnim = this.returnToIdleAnim;
			this.returnToIdleAnim = null;
		}
		if (!string.IsNullOrEmpty(this.noiseStartleAnim))
		{
			this.leftAnims.NoiseStartleAnim = this.noiseStartleAnim;
			this.rightAnims.NoiseStartleAnim = this.noiseStartleAnim;
			this.noiseStartleAnim = null;
		}
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x000164CD File Offset: 0x000146CD
	private void Awake()
	{
		this.OnValidate();
		if (!this.npcControl)
		{
			this.npcControl = base.GetComponent<NPCControlBase>();
		}
		this.hasUnityAnimator = this.unityAnimator;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x000164FF File Offset: 0x000146FF
	private void OnEnable()
	{
		ComponentSingleton<HeroPerformanceSingReactionCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		NoiseMaker.NoiseCreated += this.OnNoiseCreated;
		this.StartBehaviour();
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00016530 File Offset: 0x00014730
	private void OnDisable()
	{
		ComponentSingleton<HeroPerformanceSingReactionCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
		NoiseMaker.NoiseCreated -= this.OnNoiseCreated;
		this.StopBehaviour();
		if (this.startleRoutine != null)
		{
			base.StopCoroutine(this.startleRoutine);
			this.startleRoutine = null;
		}
		if (this.disabledNpcControl)
		{
			this.ToggleNpcControl(true);
		}
		this.isInside = false;
		this.IsForced = false;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x000165A2 File Offset: 0x000147A2
	private void StopBehaviour()
	{
		if (this.behaviourRoutine != null)
		{
			base.StopCoroutine(this.behaviourRoutine);
			this.behaviourRoutine = null;
		}
		if (this.needolinTextOwner)
		{
			this.needolinTextOwner.RemoveNeedolinText();
		}
		this.UnregisterAnimationTrigger();
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x000165E0 File Offset: 0x000147E0
	private void OnUpdate()
	{
		bool flag;
		if (this.IsForcedSoft)
		{
			if (!this.wasForcedSoft)
			{
				this.delay = this.startDelay.GetRandomValue();
				this.wasForcedSoft = true;
			}
			flag = true;
			this.affectedState = HeroPerformanceRegion.AffectedState.None;
		}
		else
		{
			this.wasForcedSoft = false;
			HeroPerformanceRegion.AffectedState affectedState = this.affectedState;
			this.affectedState = HeroPerformanceRegion.GetAffectedState(base.transform, this.ignoreNeedolinRange);
			if (this.affectedState != HeroPerformanceRegion.AffectedState.None && affectedState == HeroPerformanceRegion.AffectedState.None)
			{
				this.delay = this.startDelay.GetRandomValue();
			}
			else if (this.affectedState == HeroPerformanceRegion.AffectedState.None && affectedState != HeroPerformanceRegion.AffectedState.None)
			{
				this.delay = this.endDelay.GetRandomValue();
			}
			bool flag2;
			switch (this.affectedState)
			{
			case HeroPerformanceRegion.AffectedState.None:
				flag2 = false;
				break;
			case HeroPerformanceRegion.AffectedState.ActiveInner:
				flag2 = true;
				break;
			case HeroPerformanceRegion.AffectedState.ActiveOuter:
				flag2 = this.reactToOuter;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			flag = flag2;
		}
		if (this.delay <= 0f && flag != this.isInside)
		{
			this.isInside = flag;
		}
		this.delay -= Time.deltaTime;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x000166E5 File Offset: 0x000148E5
	private void StartBehaviour()
	{
		if (this.behaviourRoutine != null)
		{
			base.StopCoroutine(this.behaviourRoutine);
		}
		this.behaviourRoutine = base.StartCoroutine(this.Behaviour());
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0001670D File Offset: 0x0001490D
	private void RegisterAnimationTrigger()
	{
		if (!this.registeredAnimationTrigger && this.animator)
		{
			this.registeredAnimationTrigger = true;
			this.animator.AnimationEventTriggeredEvent += this.OnAnimationEventTriggered;
		}
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00016742 File Offset: 0x00014942
	private void UnregisterAnimationTrigger()
	{
		if (this.registeredAnimationTrigger)
		{
			this.registeredAnimationTrigger = false;
			if (this.animator)
			{
				this.animator.AnimationEventTriggeredEvent -= this.OnAnimationEventTriggered;
			}
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00016777 File Offset: 0x00014977
	private IEnumerator Behaviour()
	{
		HeroPerformanceSingReaction.AnimGroup currentAnimGroup = this.GetCurrentAnimGroup();
		if (!this.lookAnimNPC)
		{
			if (this.animator)
			{
				this.animator.TryPlay(currentAnimGroup.IdleAnim);
			}
			if (this.hasUnityAnimator)
			{
				this.unityAnimator.Play(currentAnimGroup.IdleAnim);
			}
		}
		for (;;)
		{
			if (this.ShouldSing)
			{
				if (this.lookAnimNPC)
				{
					this.lookAnimNPC.DeactivateInstant();
				}
				this.ToggleNpcControl(false);
				for (;;)
				{
					IL_D8:
					currentAnimGroup = this.GetCurrentAnimGroup();
					if (this.animator)
					{
						this.RegisterAnimationTrigger();
						this.animator.TryPlay(currentAnimGroup.SingAnim);
					}
					if (this.hasUnityAnimator)
					{
						this.unityAnimator.Play(currentAnimGroup.SingAnim);
					}
					UnityEvent onSingStarted = this.OnSingStarted;
					if (onSingStarted != null)
					{
						onSingStarted.Invoke();
					}
					if (this.needolinTextOwner && !this.IsForced)
					{
						this.needolinTextOwner.AddNeedolinText();
					}
					if (this.singAudioSource && this.singAudioClipTable)
					{
						this.singAudioSource.clip = this.singAudioClipTable.SelectClip(false);
						this.singAudioSource.pitch = this.singAudioClipTable.SelectPitch();
						this.singAudioSource.volume = this.singAudioClipTable.SelectVolume();
						this.singAudioSource.Play();
					}
					if (this.movementAudioClipTable)
					{
						this.movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
					}
					float singingTime = 0f;
					float singingEndTime = this.singTimeRange.GetRandomValue();
					while (this.ShouldSing)
					{
						yield return null;
						singingTime += Time.deltaTime;
						if (singingEndTime > 0f && singingTime >= singingEndTime)
						{
							break;
						}
					}
					UnityEvent onSingEnding = this.OnSingEnding;
					if (onSingEnding != null)
					{
						onSingEnding.Invoke();
					}
					if (this.needolinTextOwner)
					{
						this.needolinTextOwner.RemoveNeedolinText();
					}
					if (this.singAudioSource)
					{
						this.singAudioSource.Stop();
					}
					if (this.movementAudioClipTable)
					{
						this.movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
					}
					if (this.animator)
					{
						this.UnregisterAnimationTrigger();
						if (!string.IsNullOrEmpty(currentAnimGroup.ReturnToIdleAnim))
						{
							yield return base.StartCoroutine(this.animator.PlayAnimWait(currentAnimGroup.ReturnToIdleAnim, null));
						}
						this.animator.TryPlay(currentAnimGroup.IdleAnim);
					}
					if (this.hasUnityAnimator)
					{
						this.unityAnimator.Play(currentAnimGroup.ReturnToIdleAnim);
						yield return new WaitForEndOfFrame();
						AnimatorStateInfo currentAnimatorStateInfo = this.unityAnimator.GetCurrentAnimatorStateInfo(0);
						if (currentAnimatorStateInfo.IsName(currentAnimGroup.ReturnToIdleAnim))
						{
							yield return new WaitForSeconds(currentAnimatorStateInfo.length);
						}
						this.unityAnimator.Play(currentAnimGroup.IdleAnim);
					}
					UnityEvent onSingEnded = this.OnSingEnded;
					if (onSingEnded != null)
					{
						onSingEnded.Invoke();
					}
					float idleTime = 0f;
					float idleEndTime = this.restTimeRange.GetRandomValue();
					while (this.ShouldSing)
					{
						yield return null;
						idleTime += Time.deltaTime;
						if (idleEndTime > 0f && idleTime >= idleEndTime)
						{
							goto IL_D8;
						}
					}
					break;
				}
				this.ActivateLookAnimNpc();
				yield return new WaitForSeconds(0.5f);
				this.ToggleNpcControl(true);
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00016786 File Offset: 0x00014986
	private void ToggleNpcControl(bool active)
	{
		this.disabledNpcControl = active;
		if (this.npcControl)
		{
			if (active)
			{
				this.npcControl.Activate();
				return;
			}
			this.npcControl.Deactivate(false);
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000167B8 File Offset: 0x000149B8
	private void ActivateLookAnimNpc()
	{
		if (!this.lookAnimNPC)
		{
			return;
		}
		switch (this.lookAnimNPCActivate)
		{
		case HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.Any:
			if (this.leftAnims.IdleAnim != this.rightAnims.IdleAnim)
			{
				this.lookAnimNPC.Activate(this.lookAnimNPC.WasFacingLeft);
			}
			else
			{
				this.lookAnimNPC.Activate();
			}
			this.disabledNpcControl = false;
			return;
		case HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceLeft:
			this.lookAnimNPC.Activate(true);
			this.lookAnimNPC.ResetRestTimer();
			this.lookAnimNPC.ClearTurnDelaySkip();
			this.disabledNpcControl = false;
			return;
		case HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceRight:
			this.lookAnimNPC.Activate(false);
			this.lookAnimNPC.ResetRestTimer();
			this.lookAnimNPC.ClearTurnDelaySkip();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0001688B File Offset: 0x00014A8B
	private void OnAnimationEventTriggered(tk2dSpriteAnimator currentAnimator, tk2dSpriteAnimationClip currentClip, int currentFrame)
	{
		if (currentClip.name == this.leftAnims.SingAnim || currentClip.name == this.rightAnims.SingAnim)
		{
			this.OnSingTrigger.Invoke();
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000168C8 File Offset: 0x00014AC8
	private void OnNoiseCreated(Vector2 _, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffScreen)
	{
		if (intensity < this.minNoiseIntensity)
		{
			return;
		}
		if (!isNoiseInRange(base.transform.position))
		{
			return;
		}
		if (this.startleRoutine != null)
		{
			return;
		}
		if (this.npcControl && InteractManager.BlockingInteractable == this.npcControl)
		{
			return;
		}
		if (Time.timeAsDouble < this.nextRespondTime)
		{
			return;
		}
		if (this.IsForcedAny)
		{
			return;
		}
		this.nextRespondTime = Time.timeAsDouble + (double)this.noiseRespondCooldown;
		this.StopBehaviour();
		this.startleRoutine = base.StartCoroutine(this.Startle());
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00016964 File Offset: 0x00014B64
	private HeroPerformanceSingReaction.AnimGroup GetCurrentAnimGroup()
	{
		HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours lookAnimNpcActivateBehaviours = this.lookAnimNPCActivate;
		if (lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceLeft)
		{
			return this.leftAnims;
		}
		if (lookAnimNpcActivateBehaviours == HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceRight)
		{
			return this.rightAnims;
		}
		if (this.lookAnimNPC)
		{
			if (!this.lookAnimNPC.WasFacingLeft)
			{
				return this.rightAnims;
			}
			return this.leftAnims;
		}
		else
		{
			if (this.lookAnimNPCActivate != HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours.FaceRight)
			{
				return this.leftAnims;
			}
			return this.rightAnims;
		}
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x000169CE File Offset: 0x00014BCE
	private IEnumerator Startle()
	{
		this.ToggleNpcControl(false);
		if (this.lookAnimNPC)
		{
			this.lookAnimNPC.Deactivate();
		}
		float randomValue = this.noiseRespondDelay.GetRandomValue();
		if (randomValue > 0f)
		{
			yield return new WaitForSeconds(randomValue);
		}
		this.OnStartleStarted.Invoke();
		if (this.movementAudioClipTable)
		{
			this.movementAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		if (this.startleAudioClipTable)
		{
			if (this.singAudioSource)
			{
				this.singAudioSource.Stop();
			}
			if (this.startleUsesSingAudioSource && this.singAudioSource)
			{
				this.startleAudioClipTable.PlayOneShot(this.singAudioSource, false);
			}
			else
			{
				this.startleAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
			}
		}
		HeroPerformanceSingReaction.AnimGroup currentAnimGroup = this.GetCurrentAnimGroup();
		bool playStartleAnim = !string.IsNullOrEmpty(currentAnimGroup.NoiseStartleAnim);
		if (playStartleAnim)
		{
			tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(currentAnimGroup.NoiseStartleAnim);
			if (clipByName != null)
			{
				yield return new WaitForSeconds(this.animator.PlayAnimGetTime(clipByName));
			}
			if (this.hasUnityAnimator)
			{
				this.unityAnimator.Play(currentAnimGroup.NoiseStartleAnim);
				yield return new WaitForEndOfFrame();
				AnimatorStateInfo currentAnimatorStateInfo = this.unityAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(currentAnimGroup.NoiseStartleAnim))
				{
					yield return new WaitForSeconds(currentAnimatorStateInfo.length);
				}
			}
			if (!this.startleAnimEndsItself)
			{
				float randomValue2 = this.startleLoopTime.GetRandomValue();
				if (randomValue2 > Mathf.Epsilon)
				{
					yield return new WaitForSeconds(randomValue2);
				}
				clipByName = this.animator.GetClipByName(currentAnimGroup.ReturnToIdleAnim);
				if (clipByName != null)
				{
					yield return new WaitForSeconds(this.animator.PlayAnimGetTime(clipByName));
				}
				if (this.hasUnityAnimator)
				{
					this.unityAnimator.Play(currentAnimGroup.ReturnToIdleAnim);
					yield return new WaitForEndOfFrame();
					AnimatorStateInfo currentAnimatorStateInfo2 = this.unityAnimator.GetCurrentAnimatorStateInfo(0);
					if (currentAnimatorStateInfo2.IsName(currentAnimGroup.ReturnToIdleAnim))
					{
						yield return new WaitForSeconds(currentAnimatorStateInfo2.length);
					}
				}
			}
		}
		this.OnStartleEnded.Invoke();
		if (playStartleAnim)
		{
			this.ActivateLookAnimNpc();
		}
		else if (this.lookAnimNPC)
		{
			if (this.lookAnimNPC.State == LookAnimNPC.AnimState.Disabled)
			{
				this.ActivateLookAnimNpc();
			}
			else if (this.lookAnimNPC.State != LookAnimNPC.AnimState.Resting)
			{
				this.lookAnimNPC.ResetRestTimer();
			}
		}
		this.ToggleNpcControl(true);
		this.startleRoutine = null;
		this.StartBehaviour();
		yield break;
	}

	// Token: 0x040003BC RID: 956
	[SerializeField]
	private NPCControlBase npcControl;

	// Token: 0x040003BD RID: 957
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x040003BE RID: 958
	[SerializeField]
	private Animator unityAnimator;

	// Token: 0x040003BF RID: 959
	[SerializeField]
	private LookAnimNPC lookAnimNPC;

	// Token: 0x040003C0 RID: 960
	[SerializeField]
	[ModifiableProperty]
	[Conditional("lookAnimNPC", true, false, true)]
	private HeroPerformanceSingReaction.LookAnimNpcActivateBehaviours lookAnimNPCActivate;

	// Token: 0x040003C1 RID: 961
	[Space]
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string idleAnim;

	// Token: 0x040003C2 RID: 962
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string singAnim;

	// Token: 0x040003C3 RID: 963
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string returnToIdleAnim;

	// Token: 0x040003C4 RID: 964
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string noiseStartleAnim;

	// Token: 0x040003C5 RID: 965
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingLeftAnims", true, true, true)]
	private HeroPerformanceSingReaction.AnimGroup leftAnims;

	// Token: 0x040003C6 RID: 966
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingRightAnims", true, true, true)]
	private HeroPerformanceSingReaction.AnimGroup rightAnims;

	// Token: 0x040003C7 RID: 967
	[SerializeField]
	private bool startleAnimEndsItself;

	// Token: 0x040003C8 RID: 968
	[SerializeField]
	[ModifiableProperty]
	[Conditional("startleAnimEndsItself", false, false, false)]
	private MinMaxFloat startleLoopTime;

	// Token: 0x040003C9 RID: 969
	[SerializeField]
	private MinMaxFloat startDelay;

	// Token: 0x040003CA RID: 970
	[SerializeField]
	private MinMaxFloat endDelay;

	// Token: 0x040003CB RID: 971
	[SerializeField]
	private bool reactToOuter;

	// Token: 0x040003CC RID: 972
	[SerializeField]
	private bool ignoreNeedolinRange;

	// Token: 0x040003CD RID: 973
	[SerializeField]
	private MinMaxFloat singTimeRange;

	// Token: 0x040003CE RID: 974
	[SerializeField]
	private MinMaxFloat restTimeRange;

	// Token: 0x040003CF RID: 975
	[SerializeField]
	private NoiseMaker.Intensities minNoiseIntensity;

	// Token: 0x040003D0 RID: 976
	[SerializeField]
	private MinMaxFloat noiseRespondDelay;

	// Token: 0x040003D1 RID: 977
	[SerializeField]
	private float noiseRespondCooldown;

	// Token: 0x040003D2 RID: 978
	[SerializeField]
	private NeedolinTextOwner needolinTextOwner;

	// Token: 0x040003D3 RID: 979
	[SerializeField]
	private AudioSource singAudioSource;

	// Token: 0x040003D4 RID: 980
	[SerializeField]
	private RandomAudioClipTable singAudioClipTable;

	// Token: 0x040003D5 RID: 981
	[SerializeField]
	private RandomAudioClipTable startleAudioClipTable;

	// Token: 0x040003D6 RID: 982
	[SerializeField]
	private bool startleUsesSingAudioSource;

	// Token: 0x040003D7 RID: 983
	[SerializeField]
	private RandomAudioClipTable movementAudioClipTable;

	// Token: 0x040003D8 RID: 984
	[Space]
	public UnityEvent OnSingStarted;

	// Token: 0x040003D9 RID: 985
	public UnityEvent OnSingTrigger;

	// Token: 0x040003DA RID: 986
	public UnityEvent OnSingEnding;

	// Token: 0x040003DB RID: 987
	public UnityEvent OnSingEnded;

	// Token: 0x040003DC RID: 988
	public UnityEvent OnStartleStarted;

	// Token: 0x040003DD RID: 989
	public UnityEvent OnStartleEnded;

	// Token: 0x040003DE RID: 990
	private HeroPerformanceRegion.AffectedState affectedState;

	// Token: 0x040003DF RID: 991
	private float delay;

	// Token: 0x040003E0 RID: 992
	private bool isInside;

	// Token: 0x040003E1 RID: 993
	private bool wasForcedSoft;

	// Token: 0x040003E2 RID: 994
	private double nextRespondTime;

	// Token: 0x040003E3 RID: 995
	private Coroutine behaviourRoutine;

	// Token: 0x040003E4 RID: 996
	private Coroutine startleRoutine;

	// Token: 0x040003E5 RID: 997
	private bool disabledNpcControl;

	// Token: 0x040003E6 RID: 998
	private bool hasUnityAnimator;

	// Token: 0x040003E9 RID: 1001
	private bool registeredAnimationTrigger;

	// Token: 0x02001400 RID: 5120
	private enum LookAnimNpcActivateBehaviours
	{
		// Token: 0x04008176 RID: 33142
		Any,
		// Token: 0x04008177 RID: 33143
		FaceLeft,
		// Token: 0x04008178 RID: 33144
		FaceRight
	}

	// Token: 0x02001401 RID: 5121
	[Serializable]
	private class AnimGroup
	{
		// Token: 0x04008179 RID: 33145
		public string IdleAnim;

		// Token: 0x0400817A RID: 33146
		public string SingAnim;

		// Token: 0x0400817B RID: 33147
		public string ReturnToIdleAnim;

		// Token: 0x0400817C RID: 33148
		public string NoiseStartleAnim;
	}
}
