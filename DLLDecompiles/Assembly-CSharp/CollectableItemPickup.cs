using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001B2 RID: 434
public class CollectableItemPickup : MonoBehaviour
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x060010DF RID: 4319 RVA: 0x0004FB4E File Offset: 0x0004DD4E
	// (set) Token: 0x060010E0 RID: 4320 RVA: 0x0004FB55 File Offset: 0x0004DD55
	public static bool IsPickupPaused { get; set; }

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x060010E1 RID: 4321 RVA: 0x0004FB5D File Offset: 0x0004DD5D
	private bool IsWaitingForStoppedMoving
	{
		get
		{
			return this.waitForStoppedMoving && this.body && this.interactEvents;
		}
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x060010E2 RID: 4322 RVA: 0x0004FB81 File Offset: 0x0004DD81
	public SavedItem Item
	{
		get
		{
			return this.item;
		}
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x0004FB89 File Offset: 0x0004DD89
	public bool IsPersistenceHandled()
	{
		return (this.item && this.item.IsUnique) || !string.IsNullOrEmpty(this.playerDataBool);
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x060010E4 RID: 4324 RVA: 0x0004FBB5 File Offset: 0x0004DDB5
	// (set) Token: 0x060010E5 RID: 4325 RVA: 0x0004FBBD File Offset: 0x0004DDBD
	public CollectableItemPickup.PickupAnimations PickupAnim
	{
		get
		{
			return this.pickupAnim;
		}
		set
		{
			this.pickupAnim = value;
		}
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x0004FBC8 File Offset: 0x0004DDC8
	private void Awake()
	{
		if (this.persistent)
		{
			if (this.item && this.item.IsUnique)
			{
				Object.Destroy(this.persistent);
				this.persistent = null;
			}
			else
			{
				this.persistent.OnGetSaveState += this.OnGetSaveState;
				this.persistent.OnSetSaveState += this.OnSetSaveState;
			}
		}
		if (this.interactEvents)
		{
			this.interactEvents.Interacted += this.DoPickup;
		}
		this.PickupAnim = this.PickupAnim;
		this.SetPlayerDataBool(this.playerDataBool);
		this.body = base.GetComponent<Rigidbody2D>();
		if (this.body && this.pickupAnim == CollectableItemPickup.PickupAnimations.Stand)
		{
			this.body.bodyType = RigidbodyType2D.Kinematic;
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x0004FCA8 File Offset: 0x0004DEA8
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.Setup();
			if (this.didCancelGravity && this.body)
			{
				this.body.gravityScale = this.previousGravity;
				this.didCancelGravity = false;
				this.body.bodyType = this.bodyType;
			}
		}
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x0004FD01 File Offset: 0x0004DF01
	private void OnDisable()
	{
		this.CancelPickup();
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0004FD0C File Offset: 0x0004DF0C
	private void Start()
	{
		if (!this.ignoreCanExist && !GameManager.instance.CanPickupsExist())
		{
			base.gameObject.Recycle();
			return;
		}
		if (!this.body)
		{
			this.body = base.GetComponentInParent<Rigidbody2D>();
		}
		this.Setup();
		this.hasStarted = true;
		if (this.fling)
		{
			this.FlingSelf();
		}
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0004FD70 File Offset: 0x0004DF70
	private void Update()
	{
		if (this.pickupTrigger)
		{
			bool isInside = this.pickupTrigger.IsInside;
			if (isInside && !this.wasInsidePickupTrigger)
			{
				this.DoPickupInstant();
			}
			this.wasInsidePickupTrigger = isInside;
		}
		if (this.IsWaitingForStoppedMoving && !this.activated)
		{
			if (this.body.linearVelocity.magnitude < 0.1f)
			{
				if (Time.timeAsDouble >= this.canPickupTime && this.interactEvents.IsDisabled)
				{
					this.interactEvents.Activate();
					return;
				}
			}
			else
			{
				if (this.pickupRoutine != null)
				{
					this.CancelPickup();
					return;
				}
				this.ResetPickupDelay();
			}
		}
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0004FE14 File Offset: 0x0004E014
	private void Setup()
	{
		this.CheckActivation();
		this.ResetPickupDelay();
		if (!this.activated && this.spriteRenderer)
		{
			NestedFadeGroup component = this.spriteRenderer.GetComponent<NestedFadeGroup>();
			if (component)
			{
				component.AlphaSelf = 1f;
				return;
			}
			this.spriteRenderer.enabled = true;
		}
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0004FE6E File Offset: 0x0004E06E
	private void ResetPickupDelay()
	{
		this.canPickupTime = Time.timeAsDouble + (double)this.canPickupDelay;
		if (this.IsWaitingForStoppedMoving && !this.interactEvents.IsDisabled)
		{
			this.interactEvents.Deactivate(false);
		}
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0004FEA4 File Offset: 0x0004E0A4
	private void DoPickup()
	{
		if (this.activated || Time.timeAsDouble < this.canPickupTime)
		{
			this.EndInteraction(false);
			return;
		}
		this.pickupRoutine = base.StartCoroutine(this.Pickup());
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0004FED8 File Offset: 0x0004E0D8
	private void DoPickupInstant()
	{
		if (this.activated || Time.timeAsDouble < this.canPickupTime)
		{
			return;
		}
		if (this.DoPickupAction(true))
		{
			this.activated = true;
			return;
		}
		Debug.LogErrorFormat(this, "Couldn't pickup item {0}", new object[]
		{
			this.item ? this.item.name : "null"
		});
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0004FF40 File Offset: 0x0004E140
	private bool DoPickupAction(bool breakIfAtMax)
	{
		bool flag = true;
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			PlayerData.instance.SetVariable(this.playerDataBool, true);
			flag = false;
		}
		bool flag2 = false;
		if (this.item)
		{
			flag2 = !this.item.CanGetMore();
			if (!this.item.TryGet(breakIfAtMax, this.showPopup))
			{
				return false;
			}
		}
		else if (flag)
		{
			Debug.LogError("No collectable item assigned!", this);
			return false;
		}
		if (this.OnPickup != null)
		{
			this.OnPickup.Invoke();
		}
		if (this.OnPickedUp != null)
		{
			this.OnPickedUp.Invoke();
		}
		if (this.spriteRenderer)
		{
			NestedFadeGroup component = this.spriteRenderer.GetComponent<NestedFadeGroup>();
			if (component)
			{
				component.AlphaSelf = 0f;
			}
			else
			{
				this.spriteRenderer.enabled = false;
			}
		}
		if (this.extraEffects)
		{
			this.extraEffects.SetActive(false);
		}
		if (this.pickupEffect)
		{
			this.pickupEffect.SetActive(true);
		}
		if (!this.showPopup)
		{
			CollectableItemHeroReaction.DoReaction(new Vector2(0f, -0.76f), this.smallGetEffect);
		}
		else if (flag2)
		{
			CollectableItemHeroReaction.DoReaction();
		}
		this.StopRBMovement();
		return true;
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00050078 File Offset: 0x0004E278
	private void StopRBMovement()
	{
		if (this.body)
		{
			if (!this.didCancelGravity)
			{
				this.previousGravity = this.body.gravityScale;
				this.body.gravityScale = 0f;
				this.didCancelGravity = true;
				this.bodyType = this.body.bodyType;
				this.body.bodyType = RigidbodyType2D.Static;
			}
			this.body.linearVelocity = Vector2.zero;
		}
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x000500EF File Offset: 0x0004E2EF
	public void CancelPickup()
	{
		if (this.pickupRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.pickupRoutine);
		this.pickupRoutine = null;
		this.EndInteraction(this.activated);
		this.ResetPickupDelay();
		HeroController.instance.StartAnimationControl();
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x00050129 File Offset: 0x0004E329
	private IEnumerator Pickup()
	{
		while (HeroTalkAnimation.IsEndingHurtAnim)
		{
			yield return null;
		}
		HeroController instance = HeroController.instance;
		instance.OnTakenDamage += this.CancelPickup;
		this.subscribedHc = instance;
		instance.StopAnimationControl();
		tk2dSpriteAnimator animator = instance.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController heroAnim = instance.GetComponent<HeroAnimationController>();
		animator.Play((this.pickupAnim == CollectableItemPickup.PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 1") : heroAnim.GetClip("Collect Stand 1"));
		yield return new WaitForSeconds(0.75f);
		if (this.pickupAnim == CollectableItemPickup.PickupAnimations.Normal)
		{
			CollectableItemHeroReaction.NextEffectOffset = CollectableItemPickup.KNEELING_PICKUP_OFFSET;
		}
		bool didPickup = this.DoPickupAction(false);
		animator.Play((this.pickupAnim == CollectableItemPickup.PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 2") : heroAnim.GetClip("Collect Stand 2"));
		if (didPickup)
		{
			FSMUtility.SendEventUpwards(base.gameObject, "COLLECTED");
			this.activated = true;
			yield return new WaitForSeconds(0.5f);
		}
		if (this.pickupAnim == CollectableItemPickup.PickupAnimations.Normal)
		{
			CollectableItemHeroReaction.NextEffectOffset = Vector2.zero;
		}
		if (didPickup && this.pickupAnim != CollectableItemPickup.PickupAnimations.Stand)
		{
			while (CollectableItemPickup.IsPickupPaused)
			{
				yield return null;
			}
		}
		yield return base.StartCoroutine(animator.PlayAnimWait((this.pickupAnim == CollectableItemPickup.PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 3") : heroAnim.GetClip("Collect Stand 3"), null));
		HeroController.instance.StartAnimationControl();
		if (didPickup && this.pickupAnim == CollectableItemPickup.PickupAnimations.Stand)
		{
			while (CollectableItemPickup.IsPickupPaused)
			{
				yield return null;
			}
		}
		this.EndInteraction(didPickup);
		this.pickupRoutine = null;
		this.OnPickupEnd.Invoke();
		yield break;
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00050138 File Offset: 0x0004E338
	private void EndInteraction(bool didPickup)
	{
		if (this.subscribedHc)
		{
			this.subscribedHc.OnTakenDamage -= this.CancelPickup;
			this.subscribedHc = null;
		}
		if (!this.interactEvents)
		{
			return;
		}
		this.interactEvents.EndInteraction();
		if (didPickup)
		{
			this.interactEvents.Deactivate(false);
		}
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00050198 File Offset: 0x0004E398
	public void SetItem(SavedItem newItem, bool keepPersistence = false)
	{
		this.item = newItem;
		if (!keepPersistence && Application.isPlaying && this.persistent)
		{
			Object.Destroy(this.persistent);
		}
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x000501C4 File Offset: 0x0004E3C4
	public void SetPlayerDataBool(string boolName)
	{
		this.playerDataBool = boolName;
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			if (this.persistent)
			{
				this.persistent.OnGetSaveState -= this.OnGetSaveState;
				this.persistent.OnSetSaveState -= this.OnSetSaveState;
				this.persistent = null;
			}
			if (PlayerData.instance.GetVariable(this.playerDataBool))
			{
				if (this.OnPickedUp != null)
				{
					this.OnPickedUp.Invoke();
				}
				if (this.OnPreviouslyPickedUp != null)
				{
					this.OnPreviouslyPickedUp.Invoke();
				}
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x0005026E File Offset: 0x0004E46E
	private void OnGetSaveState(out bool value)
	{
		value = this.activated;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x00050278 File Offset: 0x0004E478
	private void OnSetSaveState(bool value)
	{
		this.activated = value;
		this.CheckActivation();
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x00050288 File Offset: 0x0004E488
	private void CheckActivation()
	{
		if (this.activated)
		{
			if (string.IsNullOrEmpty(this.playerDataBool) && this.persistent == null && (this.item == null || (!this.item.IsUnique && this.item.CanGetMore())))
			{
				this.activated = false;
				return;
			}
		}
		else
		{
			this.activated = (this.item && !this.item.CanGetMore());
		}
		if (this.activated)
		{
			if (this.OnPickedUp != null)
			{
				this.OnPickedUp.Invoke();
			}
			if (this.OnPreviouslyPickedUp != null)
			{
				this.OnPreviouslyPickedUp.Invoke();
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00050346 File Offset: 0x0004E546
	public void SetFling(bool setFling)
	{
		this.fling = setFling;
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0005034F File Offset: 0x0004E54F
	public void SetFlingLeft()
	{
		this.flingDirection = CollectableItemPickup.FlingDirection.Left;
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x00050358 File Offset: 0x0004E558
	public void SetFlingRight()
	{
		this.flingDirection = CollectableItemPickup.FlingDirection.Right;
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x00050364 File Offset: 0x0004E564
	public void FlingSelf()
	{
		if (this.flingDirection == CollectableItemPickup.FlingDirection.Drop)
		{
			this.FlingSelf(new MinMaxFloat(0f, 0f), new MinMaxFloat(0f, 0f));
			return;
		}
		if (this.flingDirection == CollectableItemPickup.FlingDirection.Either)
		{
			this.flingDirection = ((Random.Range(0, 2) > 0) ? CollectableItemPickup.FlingDirection.Right : CollectableItemPickup.FlingDirection.Left);
		}
		if (this.flingDirection == CollectableItemPickup.FlingDirection.AwayFromHero)
		{
			this.flingDirection = ((base.transform.position.x < HeroController.instance.transform.position.x) ? CollectableItemPickup.FlingDirection.Left : CollectableItemPickup.FlingDirection.Right);
		}
		float num = (this.flingDirection == CollectableItemPickup.FlingDirection.Right) ? 80f : 100f;
		this.FlingSelf(new MinMaxFloat(22f, 22f), new MinMaxFloat(num, num));
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x00050428 File Offset: 0x0004E628
	public void FlingSelf(MinMaxFloat speed, MinMaxFloat angle)
	{
		FlingSelf flingSelf = base.GetComponent<FlingSelf>() ?? base.gameObject.AddComponent<FlingSelf>();
		flingSelf.speedMin = speed.Start;
		flingSelf.speedMax = speed.End;
		flingSelf.angleMin = angle.Start;
		flingSelf.angleMax = angle.End;
		ObjectBounce component = base.GetComponent<ObjectBounce>();
		if (component)
		{
			if (this.interactEvents)
			{
				this.interactEvents.Deactivate(false);
				component.StartedMoving += delegate()
				{
					this.interactEvents.Deactivate(false);
				};
				component.StoppedMoving += delegate()
				{
					this.interactEvents.Activate();
				};
			}
			if (this.flingTrail)
			{
				this.flingTrail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				component.StartedMoving += delegate()
				{
					this.flingTrail.Play(true);
				};
				component.StoppedMoving += delegate()
				{
					this.flingTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				};
			}
		}
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00050503 File Offset: 0x0004E703
	public void SetActivation(bool value)
	{
		this.activated = value;
	}

	// Token: 0x04000FFC RID: 4092
	private static readonly Vector2 KNEELING_PICKUP_OFFSET = new Vector2(0f, -0.76f);

	// Token: 0x04000FFD RID: 4093
	[SerializeField]
	private InteractEvents interactEvents;

	// Token: 0x04000FFE RID: 4094
	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", false, false, false)]
	private TrackTriggerObjects pickupTrigger;

	// Token: 0x04000FFF RID: 4095
	[SerializeField]
	private bool waitForStoppedMoving;

	// Token: 0x04001000 RID: 4096
	[SerializeField]
	private float canPickupDelay;

	// Token: 0x04001001 RID: 4097
	[SerializeField]
	private bool ignoreCanExist;

	// Token: 0x04001002 RID: 4098
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsPersistenceHandled", false, true, false)]
	private PersistentBoolItem persistent;

	// Token: 0x04001003 RID: 4099
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04001004 RID: 4100
	[SerializeField]
	private ParticleSystem flingTrail;

	// Token: 0x04001005 RID: 4101
	[Space]
	public UnityEvent OnPickup;

	// Token: 0x04001006 RID: 4102
	public UnityEvent OnPickupEnd;

	// Token: 0x04001007 RID: 4103
	public UnityEvent OnPickedUp;

	// Token: 0x04001008 RID: 4104
	public UnityEvent OnPreviouslyPickedUp;

	// Token: 0x04001009 RID: 4105
	[Space]
	[SerializeField]
	private SavedItem item;

	// Token: 0x0400100A RID: 4106
	[SerializeField]
	private bool showPopup = true;

	// Token: 0x0400100B RID: 4107
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	// Token: 0x0400100C RID: 4108
	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", true, false, false)]
	private CollectableItemPickup.PickupAnimations pickupAnim;

	// Token: 0x0400100D RID: 4109
	[SerializeField]
	private bool fling;

	// Token: 0x0400100E RID: 4110
	[SerializeField]
	[ModifiableProperty]
	[Conditional("fling", true, false, false)]
	private CollectableItemPickup.FlingDirection flingDirection;

	// Token: 0x0400100F RID: 4111
	[SerializeField]
	private bool smallGetEffect;

	// Token: 0x04001010 RID: 4112
	[SerializeField]
	private GameObject extraEffects;

	// Token: 0x04001011 RID: 4113
	[SerializeField]
	private GameObject pickupEffect;

	// Token: 0x04001012 RID: 4114
	private bool activated;

	// Token: 0x04001013 RID: 4115
	private double canPickupTime;

	// Token: 0x04001014 RID: 4116
	private bool hasStarted;

	// Token: 0x04001015 RID: 4117
	private HeroController subscribedHc;

	// Token: 0x04001016 RID: 4118
	private Coroutine pickupRoutine;

	// Token: 0x04001017 RID: 4119
	private bool wasInsidePickupTrigger;

	// Token: 0x04001018 RID: 4120
	private Rigidbody2D body;

	// Token: 0x0400101A RID: 4122
	private bool didCancelGravity;

	// Token: 0x0400101B RID: 4123
	private float previousGravity;

	// Token: 0x0400101C RID: 4124
	private RigidbodyType2D bodyType;

	// Token: 0x020014EF RID: 5359
	public enum PickupAnimations
	{
		// Token: 0x04008542 RID: 34114
		Normal,
		// Token: 0x04008543 RID: 34115
		Stand
	}

	// Token: 0x020014F0 RID: 5360
	private enum FlingDirection
	{
		// Token: 0x04008545 RID: 34117
		Either,
		// Token: 0x04008546 RID: 34118
		Left,
		// Token: 0x04008547 RID: 34119
		Right,
		// Token: 0x04008548 RID: 34120
		Drop,
		// Token: 0x04008549 RID: 34121
		AwayFromHero
	}
}
