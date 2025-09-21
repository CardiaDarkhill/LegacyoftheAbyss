using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200049C RID: 1180
public class BouncePod : ActivatingBase, IHitResponder, IHitResponderOverride
{
	// Token: 0x14000084 RID: 132
	// (add) Token: 0x06002AA1 RID: 10913 RVA: 0x000B8F50 File Offset: 0x000B7150
	// (remove) Token: 0x06002AA2 RID: 10914 RVA: 0x000B8F88 File Offset: 0x000B7188
	public event Action BounceHit;

	// Token: 0x14000085 RID: 133
	// (add) Token: 0x06002AA3 RID: 10915 RVA: 0x000B8FC0 File Offset: 0x000B71C0
	// (remove) Token: 0x06002AA4 RID: 10916 RVA: 0x000B8FF8 File Offset: 0x000B71F8
	public event Action InertHit;

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x06002AA5 RID: 10917 RVA: 0x000B902D File Offset: 0x000B722D
	public override bool IsPaused
	{
		get
		{
			return this.isHarpoonHooked;
		}
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x000B9038 File Offset: 0x000B7238
	protected override void Awake()
	{
		base.Awake();
		this.collider = base.GetComponent<Collider2D>();
		this.heroDamagers = base.GetComponentsInChildren<DamageHero>(true);
		GameObject gameObject = base.gameObject;
		gameObject.AddComponentIfNotPresent<NonBouncer>();
		bool flag = this.harpoonHook != null;
		if (!flag)
		{
			this.harpoonHook = base.GetComponent<HarpoonHook>();
			flag = (this.harpoonHook != null);
			this.allowHitWhileHooked = flag;
		}
		if (flag)
		{
			this.harpoonHook.OnHookStart.AddListener(delegate()
			{
				this.PlayAnim(this.hookAnim, false);
				this.isHarpoonHooked = true;
			});
			this.harpoonHook.OnHookEnd.AddListener(delegate()
			{
				if (!this.isHarpoonHooked)
				{
					return;
				}
				this.isHarpoonHooked = false;
				if (this.hookQueuedHit.Source == null)
				{
					return;
				}
				HitInstance damageInstance = this.hookQueuedHit;
				this.hookQueuedHit = default(HitInstance);
				damageInstance.IsHarpoon = true;
				this.Hit(damageInstance);
			});
			this.harpoonHook.OnHookCancel.AddListener(delegate()
			{
				this.isHarpoonHooked = false;
				this.hookQueuedHit = default(HitInstance);
			});
		}
		EventRegister.GetRegisterGuaranteed(gameObject, "HERO DAMAGED").ReceivedEvent += delegate()
		{
			if (BouncePod._currentBouncer != this)
			{
				return;
			}
			if (this.bouncePullRoutines.Count > 0)
			{
				while (this.bouncePullRoutines.Count > 0)
				{
					base.StopCoroutine(this.bouncePullRoutines.Pop());
				}
			}
			this.CancelBouncePull();
		};
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x000B9114 File Offset: 0x000B7314
	protected override void Start()
	{
		base.Start();
		if (!this.disableReposition)
		{
			this.heroMarker = base.transform.Find("Hero Y");
		}
		this.hasHeroMarker = (this.heroMarker != null);
		this.hasStarted = true;
		this.tinkEffect = base.GetComponent<TinkEffect>();
		if (this.tinkEffect)
		{
			this.tinkEffect.enabled = false;
			this.tinkEffect.OverrideResponder = this;
		}
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x000B918F File Offset: 0x000B738F
	private void OnDestroy()
	{
		if (BouncePod._currentBouncer == this)
		{
			BouncePod._currentBouncer = null;
		}
		this.ClearQueuedHits();
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000B91AC File Offset: 0x000B73AC
	private void Update()
	{
		if (this.inertTimeLeft <= 0f)
		{
			return;
		}
		this.inertTimeLeft -= Time.deltaTime;
		if (this.inertTimeLeft > 0f)
		{
			return;
		}
		if (this.collider)
		{
			this.collider.enabled = true;
		}
		this.PlayAnim(this.inertRecoverAnim, false);
		this.onRecover.Invoke();
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x000B9218 File Offset: 0x000B7418
	private void LateUpdate()
	{
		List<ValueTuple<BouncePod, HitInstance>> list;
		bool flag;
		if (BouncePod._queuedBounceHits.Count > 0)
		{
			list = BouncePod._queuedBounceHits;
			flag = true;
		}
		else
		{
			if (BouncePod._queuedInertHits.Count <= 0)
			{
				return;
			}
			list = BouncePod._queuedInertHits;
			flag = false;
		}
		HeroController instance = HeroController.instance;
		Vector2 a = instance.transform.position;
		a.y -= 1f;
		bool isHarpoon = false;
		BouncePod bouncePod = null;
		float num = float.MaxValue;
		HitInstance hitInstance = default(HitInstance);
		foreach (ValueTuple<BouncePod, HitInstance> valueTuple in list)
		{
			BouncePod item = valueTuple.Item1;
			HitInstance item2 = valueTuple.Item2;
			Vector2 b = item.transform.position;
			float num2 = Vector2.Distance(a, b);
			if (item2.IsHarpoon)
			{
				isHarpoon = true;
			}
			if (num2 <= num)
			{
				num = num2;
				bouncePod = item;
				hitInstance = item2;
			}
		}
		hitInstance.IsHarpoon = isHarpoon;
		if (bouncePod != null)
		{
			if (flag)
			{
				bouncePod.lastHit = hitInstance;
				bouncePod.DoBounce();
				NailSlashTravel component = hitInstance.Source.GetComponent<NailSlashTravel>();
				if (component)
				{
					component.DoBounceEffect(bouncePod.transform.position);
				}
			}
			else
			{
				bouncePod.DoInertHit(instance.cState.facingRight);
			}
		}
		BouncePod._queuedBounceHits.Clear();
		BouncePod._queuedInertHits.Clear();
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x000B9394 File Offset: 0x000B7594
	private void ClearQueuedHits()
	{
		for (int i = BouncePod._queuedBounceHits.Count - 1; i >= 0; i--)
		{
			ValueTuple<BouncePod, HitInstance> valueTuple = BouncePod._queuedBounceHits[i];
			if (!valueTuple.Item1 || valueTuple.Item1 == this)
			{
				BouncePod._queuedBounceHits.RemoveAt(i);
			}
		}
		for (int j = BouncePod._queuedInertHits.Count - 1; j >= 0; j--)
		{
			ValueTuple<BouncePod, HitInstance> valueTuple2 = BouncePod._queuedInertHits[j];
			if (!valueTuple2.Item1 || valueTuple2.Item1 == this)
			{
				BouncePod._queuedInertHits.RemoveAt(j);
			}
		}
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x000B9434 File Offset: 0x000B7634
	public bool WillRespond(HitInstance damageInstance)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (this.isBroken || !base.IsActive)
		{
			return false;
		}
		if (!damageInstance.IsFirstHit)
		{
			return false;
		}
		if (this.inertTimeLeft > 0f)
		{
			return false;
		}
		HeroController instance = HeroController.instance;
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = source ? source.GetComponent<DamageEnemies>() : null;
		if (!(damageEnemies == null))
		{
			bool flag = !damageEnemies.DidHitEnemy;
		}
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.BouncePod);
		bool isEquipped = Gameplay.WarriorCrest.IsEquipped;
		float num = isEquipped ? -1.35f : -1.25f;
		if (instance.transform.position.y < base.transform.position.y + num && hitDirection != HitInstance.HitDirection.Up && isEquipped)
		{
			return false;
		}
		if (source)
		{
			if (damageInstance.AttackType == AttackTypes.Nail && instance.cState.attackCount == BouncePod._lastAttackCount)
			{
				return false;
			}
			if (source.GetComponent<BreakItemsOnContact>())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x000B953C File Offset: 0x000B773C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return IHitResponder.Response.None;
		}
		if (this.isBroken || !base.IsActive)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (this.inertTimeLeft > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.AttackType == AttackTypes.Spikes)
		{
			int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.UseBouncePodDirection ? damageInstance.BouncePodDirection : damageInstance.Direction);
			float hitSign = 1f;
			if (cardinalDirection == 2)
			{
				hitSign = -1f;
			}
			if (!this.isBroken)
			{
				this.PlayAnim(this.hitAnim, false);
			}
			this.SendInertHit(hitSign);
			this.SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		}
		HeroController instance = HeroController.instance;
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = source ? source.GetComponent<DamageEnemies>() : null;
		bool flag = damageEnemies == null || !damageEnemies.DidHitEnemy;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.BouncePod);
		bool isEquipped = Gameplay.WarriorCrest.IsEquipped;
		float num = isEquipped ? -1.35f : -1.25f;
		if (instance.transform.position.y < base.transform.position.y + num && hitDirection != HitInstance.HitDirection.Up)
		{
			if (isEquipped)
			{
				return IHitResponder.Response.None;
			}
			flag = false;
		}
		if (source)
		{
			if (damageInstance.AttackType == AttackTypes.Nail && instance.cState.attackCount == BouncePod._lastAttackCount)
			{
				return IHitResponder.Response.None;
			}
			if (source.GetComponent<BreakItemsOnContact>())
			{
				return IHitResponder.Response.None;
			}
		}
		if (this.isHarpoonHooked)
		{
			damageInstance.IsHarpoon = true;
			this.hookQueuedHit = damageInstance;
			if (this.ignoreHitWhileHooked || !this.allowHitWhileHooked || (flag && this.hasHeroMarker && hitDirection == HitInstance.HitDirection.Down))
			{
				return IHitResponder.Response.None;
			}
		}
		this.lastHit = damageInstance;
		this.hitCount++;
		if (this.hitsToBreak > 0 && this.hitCount >= this.hitsToBreak)
		{
			base.SetActive(false, true);
			this.isBroken = true;
			this.PlayAnim(this.breakAnim, false);
			this.onBreak.Invoke();
		}
		this.lastHitCollider = (source ? source.GetComponent<Collider2D>() : null);
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (!this.isBroken)
			{
				this.PlayAnim(this.hitAnim, false);
			}
			this.SendInertHit(-1f);
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilRight();
			}
			this.SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Right:
			if (!this.isBroken)
			{
				this.PlayAnim(this.hitAnim, false);
			}
			this.SendInertHit(1f);
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilLeft();
			}
			this.SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Up:
			if (!this.isBroken)
			{
				this.PlayAnim(this.hitAnim, false);
			}
			this.SendInertHit((float)(instance.cState.facingRight ? 1 : -1));
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilDown();
			}
			this.SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Down:
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				BouncePod._queuedBounceHits.Add(new ValueTuple<BouncePod, HitInstance>(this, damageInstance));
			}
			else
			{
				BouncePod._queuedInertHits.Add(new ValueTuple<BouncePod, HitInstance>(this, damageInstance));
			}
			return IHitResponder.Response.GenericHit;
		default:
			return IHitResponder.Response.GenericHit;
		}
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x000B988F File Offset: 0x000B7A8F
	private void CancelBouncePull()
	{
		if (this.doingBouncePull)
		{
			this.doingBouncePull = false;
			BounceShared.OnBouncePullInterrupted();
		}
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x000B98A8 File Offset: 0x000B7AA8
	private void DoBounce()
	{
		DamageHero[] array = this.heroDamagers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetCooldown(0.5f);
		}
		this.PlayAnim(this.bounceAnim, false);
		this.SpawnHitEffects();
		this.SendBounceHit();
		HeroController instance = HeroController.instance;
		BouncePod._lastAttackCount = instance.cState.attackCount;
		instance.crestAttacksFSM.SendEvent("BOUNCE CANCEL");
		instance.sprintFSM.SendEvent("BOUNCE CANCEL");
		instance.FinishDownspike();
		instance.StartAnimationControl();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		while (this.bouncePullRoutines.Count > 0)
		{
			base.StopCoroutine(this.bouncePullRoutines.Pop());
		}
		this.CancelBouncePull();
		if (!this.disableReposition && this.heroMarker)
		{
			BouncePod._currentBouncer = this;
			this.bouncePullRoutines.PushIfNotNull(base.StartCoroutine(this.BouncePull(instance)));
			return;
		}
		this.DoBounceOff(instance);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x000B999F File Offset: 0x000B7B9F
	private void DoInertHit(bool facingRight)
	{
		if (!this.isBroken)
		{
			this.PlayAnim(this.hitAnim, false);
		}
		this.SendInertHit((float)(facingRight ? 1 : -1));
		this.SpawnHitEffects();
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000B99CA File Offset: 0x000B7BCA
	private IEnumerator BouncePull(HeroController hc)
	{
		GameCameras.instance.cameraTarget.ShorterDetach();
		this.CancelBouncePull();
		if (this.isHarpoonHooked)
		{
			this.lastHit.IsHarpoon = true;
		}
		this.doingBouncePull = true;
		yield return this.bouncePullRoutines.PushIfNotNullReturn(base.StartCoroutine(BounceShared.BouncePull(base.transform, this.heroMarker.position, hc, this.lastHit)));
		this.doingBouncePull = false;
		if (!hc.cState.dashing && !hc.controlReqlinquished)
		{
			this.DoBounceOff(hc);
		}
		BouncePod._currentBouncer = null;
		yield break;
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000B99E0 File Offset: 0x000B7BE0
	private void DoBounceOff(HeroController hc)
	{
		hc.DownspikeBounce(false, null);
		if (this.bounceSpawnPrefab)
		{
			this.bounceSpawnPrefab.Spawn(base.transform.position);
		}
		if (this.bounceReactBody)
		{
			Vector2 linearVelocity = this.bounceReactVelocity;
			if (hc.cState.facingRight)
			{
				linearVelocity.x *= -1f;
			}
			this.bounceReactBody.linearVelocity = linearVelocity;
		}
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000B9A58 File Offset: 0x000B7C58
	private void SpawnHitEffects()
	{
		this.bounceSounds.SpawnAndPlayOneShot(base.transform.position, null);
		bool flag = this.impactShake.TryShake(this, true);
		if (this.lastHitCollider && this.tinkEffect && this.tinkEffect.TryDoTinkReaction(this.lastHitCollider, !flag, !this.bounceSounds.HasClips()))
		{
			return;
		}
		if (this.impactEffectPrefab)
		{
			float overriddenDirection = this.lastHit.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			this.impactEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, true, false, false) + 180f);
		}
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000B9B1C File Offset: 0x000B7D1C
	private void SendBounceHit()
	{
		if (this.hitInertTime > 0f)
		{
			this.inertTimeLeft = this.hitInertTime;
			if (this.collider)
			{
				this.collider.enabled = false;
			}
		}
		this.onBounceHit.Invoke();
		if (this.BounceHit != null)
		{
			this.BounceHit();
		}
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000B9B7C File Offset: 0x000B7D7C
	private void SendInertHit(float hitSign)
	{
		if (this.hitInertTime > 0f)
		{
			this.inertTimeLeft = this.hitInertTime;
			if (this.collider)
			{
				this.collider.enabled = false;
			}
		}
		this.onInertHit.Invoke();
		if (this.InertHit != null)
		{
			this.InertHit();
		}
		if (this.bounceReactBody)
		{
			Vector2 linearVelocity = this.hitReactVelocity;
			linearVelocity.x *= hitSign;
			this.bounceReactBody.linearVelocity = linearVelocity;
		}
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000B9C08 File Offset: 0x000B7E08
	private void PlayAnim(string animName, bool fromEnd = false)
	{
		if (string.IsNullOrEmpty(animName))
		{
			return;
		}
		if (this.tk2dAnimator)
		{
			tk2dSpriteAnimationClip clipByName = this.tk2dAnimator.GetClipByName(animName);
			this.tk2dAnimator.PlayFromFrame(clipByName, fromEnd ? (clipByName.frames.Length - 1) : 0);
		}
		if (this.mecanimAnimator)
		{
			ActivatingBase.PlayAnim(this, this.mecanimAnimator, animName, fromEnd);
		}
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000B9C70 File Offset: 0x000B7E70
	protected override void OnActiveStateUpdate(bool value, bool isInstant)
	{
		if (this.collider)
		{
			this.collider.enabled = value;
		}
		if (this.mecanimAnimator)
		{
			this.mecanimAnimator.SetBoolIfExists(BouncePod._deactivateAnticProp, false);
		}
		isInstant = (isInstant || !this.hasStarted);
		if (value)
		{
			if (!base.IsActive)
			{
				if (!string.IsNullOrEmpty(this.activateAnim))
				{
					this.PlayAnim(this.activateAnim, isInstant);
				}
			}
			else if (!string.IsNullOrEmpty(this.reactivateAnim))
			{
				this.PlayAnim(this.reactivateAnim, isInstant);
			}
			this.hitCount = 0;
			this.isBroken = false;
			return;
		}
		if (base.IsActive && !string.IsNullOrEmpty(this.deactivateAnim))
		{
			this.PlayAnim(this.deactivateAnim, isInstant);
		}
		this.ClearQueuedHits();
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000B9D40 File Offset: 0x000B7F40
	protected override void OnActivate()
	{
		if (this.tickFadeParent)
		{
			this.tickFadeParent.ForceStop();
			this.tickFadeParent.Group.AlphaSelf = 1f;
		}
		if (this.endTickPt)
		{
			this.endTickPt.Stop(true);
		}
		if (this.activePt)
		{
			this.activePt.Play(true);
		}
		if (this.pulseFadeChild && this.pulseFadeChild.gameObject.activeInHierarchy)
		{
			this.pulseFadeChild.StartAnimation();
		}
		this.onAppear.Invoke();
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000B9DE4 File Offset: 0x000B7FE4
	protected override void OnDeactivateWarning()
	{
		if (this.tickFadeParent)
		{
			this.tickFadeParent.StartAnimation();
		}
		if (this.endTickPt)
		{
			this.endTickPt.Play(true);
		}
		if (this.mecanimAnimator)
		{
			this.mecanimAnimator.SetBoolIfExists(BouncePod._deactivateAnticProp, true);
		}
		if (this.activePt)
		{
			this.activePt.Stop(true);
		}
		if (this.pulseFadeChild)
		{
			this.pulseFadeChild.StopAtCurrentPoint();
			this.pulseFadeChild.Group.FadeTo(1f, 0.5f, null, false, null);
		}
		if (this.deactivateWarningJitter)
		{
			this.deactivateWarningJitter.StartJitter();
		}
		this.onEndWarning.Invoke();
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x000B9EB4 File Offset: 0x000B80B4
	protected override void OnDeactivate()
	{
		if (this.tickFadeParent)
		{
			this.tickFadeParent.StopAtCurrentPoint();
		}
		if (this.endTickPt)
		{
			this.endTickPt.Stop(true);
		}
		if (this.deactivateWarningJitter)
		{
			this.deactivateWarningJitter.StopJitter();
		}
		this.onEnd.Invoke();
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000B9F15 File Offset: 0x000B8115
	public void Ring(bool playSound = true)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.PlayAnim(this.ringAnim, false);
		if (playSound)
		{
			this.bounceSounds.SpawnAndPlayOneShot(base.transform.position, null);
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000B9F48 File Offset: 0x000B8148
	public void SetDisableReposition(bool set)
	{
		this.disableReposition = set;
	}

	// Token: 0x04002B60 RID: 11104
	[SerializeField]
	private string deactivateAnim;

	// Token: 0x04002B61 RID: 11105
	[SerializeField]
	private string activateAnim;

	// Token: 0x04002B62 RID: 11106
	[SerializeField]
	private string reactivateAnim;

	// Token: 0x04002B63 RID: 11107
	[SerializeField]
	private ParticleSystem activePt;

	// Token: 0x04002B64 RID: 11108
	[SerializeField]
	private NestedFadeGroupCurveAnimator pulseFadeChild;

	// Token: 0x04002B65 RID: 11109
	[SerializeField]
	private ParticleSystem endTickPt;

	// Token: 0x04002B66 RID: 11110
	[SerializeField]
	private NestedFadeGroupCurveAnimator tickFadeParent;

	// Token: 0x04002B67 RID: 11111
	[SerializeField]
	private JitterSelf deactivateWarningJitter;

	// Token: 0x04002B68 RID: 11112
	[Space]
	[SerializeField]
	private bool disableReposition;

	// Token: 0x04002B69 RID: 11113
	[SerializeField]
	private string bounceAnim;

	// Token: 0x04002B6A RID: 11114
	[SerializeField]
	private string hitAnim;

	// Token: 0x04002B6B RID: 11115
	[SerializeField]
	private string hookAnim;

	// Token: 0x04002B6C RID: 11116
	[SerializeField]
	private string ringAnim;

	// Token: 0x04002B6D RID: 11117
	[Space]
	[SerializeField]
	private float hitInertTime;

	// Token: 0x04002B6E RID: 11118
	[SerializeField]
	private string inertRecoverAnim;

	// Token: 0x04002B6F RID: 11119
	[Space]
	[SerializeField]
	private int hitsToBreak;

	// Token: 0x04002B70 RID: 11120
	[SerializeField]
	private string breakAnim;

	// Token: 0x04002B71 RID: 11121
	[Space]
	[SerializeField]
	private CameraShakeTarget impactShake;

	// Token: 0x04002B72 RID: 11122
	[SerializeField]
	private GameObject impactEffectPrefab;

	// Token: 0x04002B73 RID: 11123
	[SerializeField]
	private GameObject bounceSpawnPrefab;

	// Token: 0x04002B74 RID: 11124
	[SerializeField]
	[FormerlySerializedAs("animator")]
	private tk2dSpriteAnimator tk2dAnimator;

	// Token: 0x04002B75 RID: 11125
	[SerializeField]
	private Animator mecanimAnimator;

	// Token: 0x04002B76 RID: 11126
	[SerializeField]
	private AudioEventRandom bounceSounds;

	// Token: 0x04002B77 RID: 11127
	[SerializeField]
	private HarpoonHook harpoonHook;

	// Token: 0x04002B78 RID: 11128
	[SerializeField]
	private bool ignoreHitWhileHooked;

	// Token: 0x04002B79 RID: 11129
	[Space]
	[SerializeField]
	private UnityEvent onInertHit;

	// Token: 0x04002B7A RID: 11130
	[SerializeField]
	private UnityEvent onBounceHit;

	// Token: 0x04002B7B RID: 11131
	[SerializeField]
	private UnityEvent onRecover;

	// Token: 0x04002B7C RID: 11132
	[SerializeField]
	private UnityEvent onBreak;

	// Token: 0x04002B7D RID: 11133
	[SerializeField]
	private UnityEvent onAppear;

	// Token: 0x04002B7E RID: 11134
	[SerializeField]
	private UnityEvent onEndWarning;

	// Token: 0x04002B7F RID: 11135
	[SerializeField]
	private UnityEvent onEnd;

	// Token: 0x04002B80 RID: 11136
	[Space]
	[SerializeField]
	private Rigidbody2D bounceReactBody;

	// Token: 0x04002B81 RID: 11137
	[SerializeField]
	private Vector2 bounceReactVelocity;

	// Token: 0x04002B82 RID: 11138
	[SerializeField]
	private Vector2 hitReactVelocity;

	// Token: 0x04002B83 RID: 11139
	private Transform heroMarker;

	// Token: 0x04002B84 RID: 11140
	private bool hasStarted;

	// Token: 0x04002B85 RID: 11141
	private Coroutine cullWaitRoutine;

	// Token: 0x04002B86 RID: 11142
	private Collider2D collider;

	// Token: 0x04002B87 RID: 11143
	private static int _lastAttackCount = -1;

	// Token: 0x04002B88 RID: 11144
	private readonly Stack<Coroutine> bouncePullRoutines = new Stack<Coroutine>();

	// Token: 0x04002B89 RID: 11145
	private static BouncePod _currentBouncer;

	// Token: 0x04002B8A RID: 11146
	private float inertTimeLeft;

	// Token: 0x04002B8B RID: 11147
	private HitInstance lastHit;

	// Token: 0x04002B8C RID: 11148
	private int hitCount;

	// Token: 0x04002B8D RID: 11149
	private bool isBroken;

	// Token: 0x04002B8E RID: 11150
	private DamageHero[] heroDamagers;

	// Token: 0x04002B8F RID: 11151
	private TinkEffect tinkEffect;

	// Token: 0x04002B90 RID: 11152
	private Collider2D lastHitCollider;

	// Token: 0x04002B91 RID: 11153
	private bool isHarpoonHooked;

	// Token: 0x04002B92 RID: 11154
	private HitInstance hookQueuedHit;

	// Token: 0x04002B93 RID: 11155
	private bool doingBouncePull;

	// Token: 0x04002B94 RID: 11156
	[TupleElementNames(new string[]
	{
		"pod",
		"hit"
	})]
	private static readonly List<ValueTuple<BouncePod, HitInstance>> _queuedBounceHits = new List<ValueTuple<BouncePod, HitInstance>>();

	// Token: 0x04002B95 RID: 11157
	[TupleElementNames(new string[]
	{
		"pod",
		"hit"
	})]
	private static readonly List<ValueTuple<BouncePod, HitInstance>> _queuedInertHits = new List<ValueTuple<BouncePod, HitInstance>>();

	// Token: 0x04002B96 RID: 11158
	private static readonly int _deactivateAnticProp = Animator.StringToHash("In Deactivate Antic");

	// Token: 0x04002B97 RID: 11159
	private bool allowHitWhileHooked;

	// Token: 0x04002B98 RID: 11160
	private bool hasHeroMarker;
}
