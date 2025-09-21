using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class HeroTalkAnimation : MonoBehaviour
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000441 RID: 1089 RVA: 0x000169ED File Offset: 0x00014BED
	// (set) Token: 0x06000442 RID: 1090 RVA: 0x00016A1A File Offset: 0x00014C1A
	public static RandomAudioClipTable TalkAudioTable
	{
		get
		{
			if (HeroTalkAnimation.overrideTalkAudioTable != null)
			{
				return HeroTalkAnimation.overrideTalkAudioTable;
			}
			if (!HeroTalkAnimation._instance)
			{
				return null;
			}
			return HeroTalkAnimation._instance.talkAudioTable;
		}
		set
		{
			HeroTalkAnimation.SetTalkTableOverride(value);
		}
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00016A23 File Offset: 0x00014C23
	public static int SetTalkTableOverride(RandomAudioClipTable randomAudioClipTable)
	{
		HeroTalkAnimation.overrideTalkAudioTable = randomAudioClipTable;
		return ++HeroTalkAnimation.talkOverrideID;
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00016A38 File Offset: 0x00014C38
	public static bool RemoveTalkTableOverride(int ID)
	{
		if (ID == HeroTalkAnimation.talkOverrideID)
		{
			HeroTalkAnimation.overrideTalkAudioTable = null;
			return true;
		}
		return false;
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000445 RID: 1093 RVA: 0x00016A4B File Offset: 0x00014C4B
	public static bool IsEndingHurtAnim
	{
		get
		{
			return HeroTalkAnimation._instance && HeroTalkAnimation._instance.endHurtAnimWait != null;
		}
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00016A68 File Offset: 0x00014C68
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<HeroTalkAnimation.AnimationGroup>(ref this.animationGroups, typeof(HeroTalkAnimation.AnimationTypes));
		ArrayForEnumAttribute.EnsureArraySize<HeroTalkAnimation.AnimationGroup>(ref this.hurtAnimationGroups, typeof(HeroTalkAnimation.AnimationTypes));
		ArrayForEnumAttribute.EnsureArraySize<HeroTalkAnimation.AnimationGroup>(ref this.windyHurtAnimationGroups, typeof(HeroTalkAnimation.AnimationTypes));
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00016AB4 File Offset: 0x00014CB4
	private void Awake()
	{
		this.OnValidate();
		this.hc = base.GetComponent<HeroController>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.heroAnim = base.GetComponent<HeroAnimationController>();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00016AE0 File Offset: 0x00014CE0
	private void Start()
	{
		HeroTalkAnimation._instance = this;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00016AE8 File Offset: 0x00014CE8
	private void OnDestroy()
	{
		if (HeroTalkAnimation._instance == this)
		{
			HeroTalkAnimation._instance = null;
		}
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00016AFD File Offset: 0x00014CFD
	public static void EnterConversation(NPCControlBase sourceNpc)
	{
		if (!HeroTalkAnimation._instance)
		{
			return;
		}
		HeroTalkAnimation._instance.InternalEnterConversation(sourceNpc);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00016B17 File Offset: 0x00014D17
	public static void ExitConversation()
	{
		if (!HeroTalkAnimation._instance)
		{
			return;
		}
		HeroTalkAnimation._instance.InternalExitConversation();
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00016B30 File Offset: 0x00014D30
	public static void SetTalking(bool setTalking, bool setAnimating)
	{
		if (!HeroTalkAnimation._instance)
		{
			return;
		}
		HeroTalkAnimation._instance.InternalSetTalking(setTalking, setAnimating);
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00016B4B File Offset: 0x00014D4B
	public static void SetBlocked(bool value)
	{
		if (!HeroTalkAnimation._instance)
		{
			return;
		}
		HeroTalkAnimation._instance.isBlocked = value;
		if (value)
		{
			HeroTalkAnimation._instance.hasControl = false;
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00016B74 File Offset: 0x00014D74
	public static void SetWatchTarget(Transform target, Action onEndedFacingForward)
	{
		if (!HeroTalkAnimation._instance)
		{
			return;
		}
		HeroTalkAnimation._instance.onEndedFacingForward = onEndedFacingForward;
		if (target)
		{
			if (HeroTalkAnimation._instance.hc.HasAnimationControl)
			{
				HeroTalkAnimation._instance.TakeAnimationControl();
			}
			HeroTalkAnimation._instance.watchTarget = target;
			if (HeroTalkAnimation._instance.watchTargetRoutine == null)
			{
				HeroTalkAnimation._instance.watchTargetRoutine = HeroTalkAnimation._instance.StartCoroutine(HeroTalkAnimation._instance.KeepFacingTarget());
				return;
			}
		}
		else
		{
			HeroTalkAnimation._instance.watchTarget = null;
			if (HeroTalkAnimation._instance.watchTargetRoutine == null && onEndedFacingForward != null)
			{
				onEndedFacingForward();
			}
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00016C12 File Offset: 0x00014E12
	private void SkipWaiting()
	{
		if (this.onEndHurtAnimCompletion != null)
		{
			this.onEndHurtAnimCompletion();
			this.onEndHurtAnimCompletion = null;
		}
		if (this.endHurtAnimWait != null)
		{
			this.endHurtAnimWait.Cancel();
			this.endHurtAnimWait = null;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000450 RID: 1104 RVA: 0x00016C48 File Offset: 0x00014E48
	private bool IsHurtAnimOverridden
	{
		get
		{
			return this.sourceAnimNpc != null && this.sourceAnimNpc.OverrideHeroHurtAnim;
		}
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00016C68 File Offset: 0x00014E68
	private void InternalEnterConversation(NPCControlBase sourceNpc)
	{
		this.wasTalking = false;
		this.sourceAnimNpc = sourceNpc;
		if (!this.hc.AnimCtrl.IsHurt() || !this.hc.AnimCtrl.IsPlayingHurtAnim || !this.IsHurtAnimOverridden)
		{
			if (this.sourceAnimNpc.HeroAnimation != HeroTalkAnimation.AnimationTypes.LookForward)
			{
				this.SkipWaiting();
				this.TakeAnimationControl();
				this.InternalSetTalking(false, true);
			}
			return;
		}
		if (this.endHurtAnimWait != null)
		{
			return;
		}
		this.onEndHurtAnimCompletion = delegate()
		{
			this.InternalSetTalking(false, true);
		};
		this.EndHurtAnim();
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00016CF2 File Offset: 0x00014EF2
	private void TakeAnimationControl()
	{
		this.hc.StopAnimationControl();
		this.hasControl = true;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00016D08 File Offset: 0x00014F08
	private void EndHurtAnim()
	{
		this.TakeAnimationControl();
		tk2dSpriteAnimationClip clip2 = this.heroAnim.GetClip("Hurt To Idle");
		this.animator.Play(clip2);
		this.endHurtAnimWait = new WaitForTk2dAnimatorClipFinish(this.animator, delegate(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
		{
			if (clip.name == "Hurt To Idle")
			{
				tk2dSpriteAnimationClip clip3 = this.heroAnim.GetClip("Idle");
				anim.Play(clip3);
			}
			this.endHurtAnimWait = null;
			if (this.onEndHurtAnimCompletion != null)
			{
				this.onEndHurtAnimCompletion();
				this.onEndHurtAnimCompletion = null;
			}
		});
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00016D55 File Offset: 0x00014F55
	private void InternalExitConversation()
	{
		this.SkipWaiting();
		this.isBlocked = false;
		if (!this.hasControl)
		{
			return;
		}
		this.hasControl = false;
		if (!this.hc.HasAnimationControl)
		{
			this.hc.StartAnimationControl();
		}
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00016D8C File Offset: 0x00014F8C
	private void InternalSetTalking(bool setTalking, bool setAnimating)
	{
		if (setTalking)
		{
			if (setAnimating)
			{
				if (this.wasTalking && this.linesSinceLastSpeak < 1)
				{
					this.linesSinceLastSpeak++;
				}
				else
				{
					this.linesSinceLastSpeak = 0;
					NPCSpeakingAudio.PlayVoice(HeroTalkAnimation.TalkAudioTable, base.transform.position);
				}
			}
		}
		else
		{
			this.linesSinceLastSpeak = 0;
		}
		this.wasTalking = setTalking;
		this.wasAnimating = setAnimating;
		if ((this.sourceAnimNpc && this.sourceAnimNpc.HeroAnimation == HeroTalkAnimation.AnimationTypes.Custom) || this.isBlocked)
		{
			return;
		}
		if (setAnimating)
		{
			this.hasControl = true;
		}
		if (!this.hasControl || this.isTurning || this.endHurtAnimWait != null)
		{
			return;
		}
		if (this.hc.HasAnimationControl && setAnimating)
		{
			this.TakeAnimationControl();
		}
		if (!this.hc.HasAnimationControl)
		{
			this.PlayCorrectAnimation(setTalking, false);
		}
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00016E64 File Offset: 0x00015064
	private HeroTalkAnimation.AnimationGroup GetAnimGroup(HeroTalkAnimation.AnimationTypes animationType)
	{
		if (!this.hc.AnimCtrl.IsHurt())
		{
			return this.animationGroups[(int)animationType];
		}
		if (this.hc.cState.inWindRegion || this.hc.cState.inUpdraft)
		{
			return this.windyHurtAnimationGroups[(int)animationType];
		}
		return this.hurtAnimationGroups[(int)animationType];
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00016ED0 File Offset: 0x000150D0
	private HeroTalkAnimation.AnimationGroup GetCurrentAnimGroup()
	{
		if (this.hc.AnimCtrl.IsHurt() && !this.IsHurtAnimOverridden)
		{
			if (!this.sourceAnimNpc || this.sourceAnimNpc.HeroAnimation < HeroTalkAnimation.AnimationTypes.LookForward)
			{
				return this.hurtAnimationGroups[0];
			}
			HeroTalkAnimation.AnimationTypes heroAnimation = this.sourceAnimNpc.HeroAnimation;
			if (heroAnimation != HeroTalkAnimation.AnimationTypes.Custom && heroAnimation != HeroTalkAnimation.AnimationTypes.Kneeling)
			{
				if (this.hc.cState.inWindRegion || this.hc.cState.inUpdraft)
				{
					return this.windyHurtAnimationGroups[(int)heroAnimation];
				}
				return this.hurtAnimationGroups[(int)heroAnimation];
			}
		}
		if (!this.sourceAnimNpc || this.sourceAnimNpc.HeroAnimation < HeroTalkAnimation.AnimationTypes.LookForward)
		{
			return this.animationGroups[0];
		}
		return this.animationGroups[(int)this.sourceAnimNpc.HeroAnimation];
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00016FB2 File Offset: 0x000151B2
	private IEnumerator KeepFacingTarget()
	{
		bool setAnimating = !this.wasAnimating;
		for (;;)
		{
			if (this.endHurtAnimWait == null)
			{
				float num;
				if (this.watchTarget)
				{
					float watchTargetDirection = this.GetWatchTargetDirection();
					num = (float)(this.hc.cState.facingRight ? 1 : -1) * watchTargetDirection;
				}
				else
				{
					num = 1f;
				}
				if (num < 0f)
				{
					if (!this.isFacingBackward)
					{
						this.isTurning = true;
						HeroTalkAnimation.AnimationGroup currentAnimGroup = this.GetCurrentAnimGroup();
						yield return new WaitForSeconds(this.animator.PlayAnimGetTime(this.heroAnim.GetClip(currentAnimGroup.TurnBackwardAnim)));
						this.isFacingBackward = true;
					}
				}
				else if (this.isFacingBackward)
				{
					this.isTurning = true;
					HeroTalkAnimation.AnimationGroup currentAnimGroup2 = this.GetCurrentAnimGroup();
					yield return new WaitForSeconds(this.animator.PlayAnimGetTime(this.heroAnim.GetClip(currentAnimGroup2.TurnForwardAnim)));
					this.isFacingBackward = false;
				}
				if (this.isTurning || setAnimating != this.wasAnimating)
				{
					this.isTurning = false;
					setAnimating = this.wasAnimating;
					this.PlayCorrectAnimation(this.wasTalking, true);
				}
				if (!this.watchTarget && !this.isFacingBackward)
				{
					break;
				}
				yield return null;
			}
			else
			{
				if (!this.watchTarget)
				{
					this.SkipWaiting();
				}
				yield return null;
			}
		}
		this.isFacingBackward = false;
		this.watchTargetRoutine = null;
		Action action = this.onEndedFacingForward;
		if (action != null)
		{
			action();
		}
		yield break;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00016FC4 File Offset: 0x000151C4
	private float GetWatchTargetDirection()
	{
		Vector2 vector = base.transform.position;
		return Mathf.Sign(this.watchTarget.position.x - vector.x);
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00017004 File Offset: 0x00015204
	private void PlayCorrectAnimation(bool isTalking, bool skipToLoop)
	{
		HeroTalkAnimation.AnimationGroup currentAnimGroup = this.GetCurrentAnimGroup();
		tk2dSpriteAnimationClip clip = this.heroAnim.GetClip(this.isFacingBackward ? currentAnimGroup.TalkBackwardAnimSafe : currentAnimGroup.TalkAnim);
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip;
		if (isTalking)
		{
			tk2dSpriteAnimationClip = clip;
		}
		else
		{
			tk2dSpriteAnimationClip = this.heroAnim.GetClip(this.isFacingBackward ? currentAnimGroup.ListenBackwardAnimSafe : currentAnimGroup.ListenAnim);
			if (tk2dSpriteAnimationClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && this.animator.IsPlaying(clip) && tk2dSpriteAnimationClip != clip)
			{
				skipToLoop = true;
			}
		}
		if (skipToLoop)
		{
			this.animator.PlayFromFrame(tk2dSpriteAnimationClip, tk2dSpriteAnimationClip.loopStart);
			return;
		}
		this.animator.Play(tk2dSpriteAnimationClip);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x000170A8 File Offset: 0x000152A8
	public void PlayCorrectLookAnimation(HeroTalkAnimation.AnimationTypes animType, bool skipToLoop)
	{
		HeroTalkAnimation.AnimationGroup currentAnimGroup = this.GetCurrentAnimGroup();
		HeroTalkAnimation.AnimationGroup animGroup = this.GetAnimGroup(animType);
		tk2dSpriteAnimationClip clip = this.heroAnim.GetClip(this.isFacingBackward ? currentAnimGroup.TalkBackwardAnimSafe : currentAnimGroup.TalkAnim);
		tk2dSpriteAnimationClip clip2 = this.heroAnim.GetClip(this.isFacingBackward ? animGroup.ListenBackwardAnimSafe : animGroup.ListenAnim);
		if (clip2.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && this.animator.IsPlaying(clip))
		{
			skipToLoop = true;
		}
		if (skipToLoop)
		{
			this.animator.PlayFromFrame(clip2, clip2.loopStart);
			return;
		}
		this.animator.Play(clip2);
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00017146 File Offset: 0x00015346
	public static void PlayLookAnimation(HeroTalkAnimation.AnimationTypes animType, bool skipToLoop)
	{
		if (HeroTalkAnimation._instance == null)
		{
			return;
		}
		HeroTalkAnimation._instance.PlayCorrectLookAnimation(animType, skipToLoop);
	}

	// Token: 0x040003EA RID: 1002
	[SerializeField]
	[ArrayForEnum(typeof(HeroTalkAnimation.AnimationTypes))]
	private HeroTalkAnimation.AnimationGroup[] animationGroups;

	// Token: 0x040003EB RID: 1003
	[SerializeField]
	private HeroTalkAnimation.AnimationGroup hurtAnims;

	// Token: 0x040003EC RID: 1004
	[SerializeField]
	[ArrayForEnum(typeof(HeroTalkAnimation.AnimationTypes))]
	private HeroTalkAnimation.AnimationGroup[] hurtAnimationGroups;

	// Token: 0x040003ED RID: 1005
	[SerializeField]
	[ArrayForEnum(typeof(HeroTalkAnimation.AnimationTypes))]
	private HeroTalkAnimation.AnimationGroup[] windyHurtAnimationGroups;

	// Token: 0x040003EE RID: 1006
	[SerializeField]
	private RandomAudioClipTable talkAudioTable;

	// Token: 0x040003EF RID: 1007
	private bool wasTalking;

	// Token: 0x040003F0 RID: 1008
	private bool wasAnimating;

	// Token: 0x040003F1 RID: 1009
	private int linesSinceLastSpeak;

	// Token: 0x040003F2 RID: 1010
	private NPCControlBase sourceAnimNpc;

	// Token: 0x040003F3 RID: 1011
	private bool hasControl;

	// Token: 0x040003F4 RID: 1012
	private WaitForTk2dAnimatorClipFinish endHurtAnimWait;

	// Token: 0x040003F5 RID: 1013
	private Action onEndHurtAnimCompletion;

	// Token: 0x040003F6 RID: 1014
	private tk2dSpriteAnimator animator;

	// Token: 0x040003F7 RID: 1015
	private HeroAnimationController heroAnim;

	// Token: 0x040003F8 RID: 1016
	private HeroController hc;

	// Token: 0x040003F9 RID: 1017
	private bool isBlocked;

	// Token: 0x040003FA RID: 1018
	private Transform watchTarget;

	// Token: 0x040003FB RID: 1019
	private Coroutine watchTargetRoutine;

	// Token: 0x040003FC RID: 1020
	private bool isTurning;

	// Token: 0x040003FD RID: 1021
	private bool isFacingBackward;

	// Token: 0x040003FE RID: 1022
	private Action onEndedFacingForward;

	// Token: 0x040003FF RID: 1023
	private static HeroTalkAnimation _instance;

	// Token: 0x04000400 RID: 1024
	private static RandomAudioClipTable overrideTalkAudioTable;

	// Token: 0x04000401 RID: 1025
	private static int talkOverrideID;

	// Token: 0x02001404 RID: 5124
	public enum AnimationTypes
	{
		// Token: 0x0400818B RID: 33163
		Custom = -1,
		// Token: 0x0400818C RID: 33164
		LookForward,
		// Token: 0x0400818D RID: 33165
		LookUp,
		// Token: 0x0400818E RID: 33166
		LookDown,
		// Token: 0x0400818F RID: 33167
		LookHalfUp,
		// Token: 0x04008190 RID: 33168
		Kneeling,
		// Token: 0x04008191 RID: 33169
		LookHalfDown
	}

	// Token: 0x02001405 RID: 5125
	[Serializable]
	private struct AnimationGroup
	{
		// Token: 0x17000C7B RID: 3195
		// (get) Token: 0x06008238 RID: 33336 RVA: 0x002649AC File Offset: 0x00262BAC
		public string TalkBackwardAnimSafe
		{
			get
			{
				if (!string.IsNullOrEmpty(this.TalkBackwardAnim))
				{
					return this.TalkBackwardAnim;
				}
				return this.TalkAnim;
			}
		}

		// Token: 0x17000C7C RID: 3196
		// (get) Token: 0x06008239 RID: 33337 RVA: 0x002649C8 File Offset: 0x00262BC8
		public string ListenBackwardAnimSafe
		{
			get
			{
				if (!string.IsNullOrEmpty(this.ListenBackwardAnim))
				{
					return this.ListenBackwardAnim;
				}
				return this.ListenAnim;
			}
		}

		// Token: 0x04008192 RID: 33170
		public string TalkAnim;

		// Token: 0x04008193 RID: 33171
		public string TalkBackwardAnim;

		// Token: 0x04008194 RID: 33172
		public string ListenAnim;

		// Token: 0x04008195 RID: 33173
		public string ListenBackwardAnim;

		// Token: 0x04008196 RID: 33174
		public string TurnBackwardAnim;

		// Token: 0x04008197 RID: 33175
		public string TurnForwardAnim;
	}
}
