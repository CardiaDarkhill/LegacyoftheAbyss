using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class WeaverLift : MonoBehaviour
{
	// Token: 0x17000567 RID: 1383
	// (get) Token: 0x0600329D RID: 12957 RVA: 0x000E14DD File Offset: 0x000DF6DD
	public bool IsAvailable
	{
		get
		{
			return !this.isUnlockedTarget || this.isUnlockedTarget.GetCurrentValue();
		}
	}

	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x0600329E RID: 12958 RVA: 0x000E14F9 File Offset: 0x000DF6F9
	public bool HasDirections
	{
		get
		{
			return this.liftUp && this.liftUp.IsAvailable && this.liftDown && this.liftDown.IsAvailable;
		}
	}

	// Token: 0x0600329F RID: 12959 RVA: 0x000E1530 File Offset: 0x000DF730
	private void Awake()
	{
		if (this.isUnlockedTarget)
		{
			this.isUnlockedTarget.OnSetSaveState += delegate(bool value)
			{
				this.SetActive(value, true);
			};
			if (this.firstActivateRange)
			{
				this.firstActivateRange.OnTriggerEntered += delegate(Collider2D _, GameObject _)
				{
					if (!this.isActive && this.isUnlockedTarget.GetCurrentValue())
					{
						this.SetActive(true, false);
					}
				};
			}
		}
		else
		{
			this.SetActive(true, true);
		}
		if (this.preTeleportActivate)
		{
			this.preTeleportActivate.SetActive(false);
		}
		if (this.arriveActivate)
		{
			this.arriveActivate.SetActive(false);
		}
		if (this.chargeUpWindRegion)
		{
			this.chargeUpWindRegion.SetActive(false);
		}
		if (this.shaftGlowsUp)
		{
			this.shaftGlowsUp.AlphaSelf = 0f;
		}
		if (this.shaftGlowsDown)
		{
			this.shaftGlowsDown.AlphaSelf = 0f;
		}
		if (this.chargeActivate)
		{
			this.chargeActivate.SetActive(false);
		}
		this.heroTeleportObject.SetActive(false);
		this.mainAnimatorCapture = this.mainAnimator.GetComponent<CaptureAnimationEvent>();
	}

	// Token: 0x060032A0 RID: 12960 RVA: 0x000E164C File Offset: 0x000DF84C
	private void Start()
	{
		if (this.liftUp)
		{
			if (this.liftUp.isUnlockedTarget)
			{
				this.liftUp.isUnlockedTarget.PreSetup();
			}
			this.teleportDirectionUp = this.liftUp.IsAvailable;
		}
	}

	// Token: 0x060032A1 RID: 12961 RVA: 0x000E1699 File Offset: 0x000DF899
	private void OnDestroy()
	{
		if (WeaverLift._isTeleportingLift == this)
		{
			WeaverLift._isTeleportingLift = null;
		}
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x000E16AE File Offset: 0x000DF8AE
	private void SetActive(bool value, bool isInstant)
	{
		this.isActive = value;
		this.mainAnimator.SetBool(WeaverLift._isActiveAnimParam, value);
		if (isInstant)
		{
			this.mainAnimator.Play(value ? WeaverLift._activeAnim : WeaverLift._inactiveAnim, 0, 1f);
		}
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x000E16EC File Offset: 0x000DF8EC
	public void BeginTransport()
	{
		if (WeaverLift._isTeleportingLift != null)
		{
			return;
		}
		if (!this.IsAvailable)
		{
			return;
		}
		this.chargePlateShake.DoShake(this, true);
		if (this.chargeActivate)
		{
			this.chargeActivate.SetActive(true);
		}
		if (this.chargeUpRoutine != null)
		{
			base.StopCoroutine(this.chargeUpRoutine);
		}
		this.chargeUpRoutine = base.StartCoroutine(this.ChargeUp());
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x000E175C File Offset: 0x000DF95C
	public void CancelTransport()
	{
		if (this.teleportRoutine != null)
		{
			return;
		}
		if (WeaverLift._isTeleportingLift == this)
		{
			WeaverLift._isTeleportingLift = null;
		}
		if (this.chargeUpRoutine != null)
		{
			base.StopCoroutine(this.chargeUpRoutine);
		}
		this.ChargeUpStopped();
		this.mainAnimator.SetBool(WeaverLift._isChargingAnimParam, false);
		if (this.chargeActivate)
		{
			this.chargeActivate.SetActive(false);
		}
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x000E17C9 File Offset: 0x000DF9C9
	private IEnumerator ChargeUp()
	{
		this.mainAnimator.SetBool(WeaverLift._isChargingAnimParam, true);
		if (this.mainAnimatorCapture)
		{
			this.mainAnimatorCapture.EventFiredTemp += this.ChargeUpStarted;
		}
		else
		{
			this.ChargeUpStarted();
		}
		if (this.chargeUpParticles)
		{
			this.chargeUpParticles.PlayParticles();
		}
		yield return new WaitForEndOfFrame();
		AnimatorStateInfo currentAnimatorStateInfo = this.mainAnimator.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.length == 0f)
		{
			yield return new WaitForEndOfFrame();
			currentAnimatorStateInfo = this.mainAnimator.GetCurrentAnimatorStateInfo(0);
		}
		yield return new WaitForSeconds(currentAnimatorStateInfo.length);
		this.chargeUpRoutine = null;
		this.Teleport();
		yield break;
	}

	// Token: 0x060032A6 RID: 12966 RVA: 0x000E17D8 File Offset: 0x000DF9D8
	private void ChargeUpStarted()
	{
		this.chargeRumble.DoShake(this, true);
		if (this.chargeUpWindRegion)
		{
			this.chargeUpWindRegion.SetActive(true);
		}
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x000E1800 File Offset: 0x000DFA00
	private void ChargeUpStopped()
	{
		if (this.mainAnimatorCapture)
		{
			this.mainAnimatorCapture.ClearTempEvent();
		}
		this.chargeRumble.CancelShake();
		if (this.chargeUpParticles)
		{
			this.chargeUpParticles.StopParticles();
		}
		if (this.chargeUpWindRegion)
		{
			this.chargeUpWindRegion.SetActive(false);
		}
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x000E1864 File Offset: 0x000DFA64
	private void Teleport()
	{
		WeaverLift weaverLift;
		NestedFadeGroupBase shaftGlows;
		if (this.HasDirections)
		{
			if (this.teleportDirectionUp)
			{
				weaverLift = this.liftUp;
				shaftGlows = this.shaftGlowsUp;
			}
			else
			{
				weaverLift = this.liftDown;
				shaftGlows = this.shaftGlowsDown;
			}
		}
		else if (this.liftUp && this.liftUp.IsAvailable)
		{
			weaverLift = this.liftUp;
			shaftGlows = this.shaftGlowsUp;
		}
		else if (this.liftDown && this.liftDown.IsAvailable)
		{
			weaverLift = this.liftDown;
			shaftGlows = this.shaftGlowsDown;
		}
		else
		{
			weaverLift = null;
			shaftGlows = null;
		}
		if (weaverLift == null)
		{
			Debug.LogError("No target!", this);
			return;
		}
		if (!weaverLift.heroTeleportPoint)
		{
			Debug.LogError("Target has no teleport point!", weaverLift);
			return;
		}
		this.teleportRoutine = base.StartCoroutine(this.TeleportRoutine(weaverLift, shaftGlows));
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x000E193B File Offset: 0x000DFB3B
	private IEnumerator TeleportRoutine(WeaverLift target, NestedFadeGroupBase shaftGlows)
	{
		WeaverLift._isTeleportingLift = this;
		HeroController hc = HeroController.instance;
		tk2dSpriteAnimator heroAnimator = hc.GetComponent<tk2dSpriteAnimator>();
		SpriteFlash heroFlash = hc.GetComponent<SpriteFlash>();
		CameraController cam = GameCameras.instance.cameraController;
		MeshRenderer heroRenderer = hc.GetComponent<MeshRenderer>();
		this.heroTeleportObject.SetActive(true);
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		hc.RelinquishControl();
		hc.StopAnimationControl();
		hc.AffectedByGravity(false);
		hc.cState.invulnerable = true;
		hc.Body.linearVelocity = Vector2.zero;
		heroRenderer.enabled = false;
		CameraController.CameraMode prevCamMode = cam.mode;
		cam.SetMode(CameraController.CameraMode.PANNING);
		Vector2 initialHeroPos = hc.transform.position;
		Vector2 vector = target.heroTeleportPoint.position;
		Vector2 targetHeroPos = hc.FindGroundPoint(vector, false);
		if (vector.y - initialHeroPos.y > 0f)
		{
			tk2dSpriteAnimationClip clipByName = heroAnimator.GetClipByName("Updraft Rise");
			heroAnimator.PlayFromFrame(clipByName, clipByName.loopStart);
		}
		else
		{
			heroAnimator.Play("Fall");
		}
		if (this.preTeleportActivate)
		{
			this.preTeleportActivate.SetActive(true);
		}
		if (shaftGlows)
		{
			shaftGlows.FadeTo(1f, this.shaftGlowsFadeTime, null, false, null);
		}
		yield return new WaitForSeconds(this.teleportWait);
		if (target.arriveActivate)
		{
			target.arriveActivate.SetActive(false);
		}
		Transform heroTrans = hc.transform;
		heroTrans.SetPositionX(targetHeroPos.x);
		initialHeroPos.x = targetHeroPos.x;
		MinMaxFloat cameraYRange = new MinMaxFloat(this.cameraYPos, target.cameraYPos);
		for (float elapsed = 0f; elapsed < this.teleportDuration; elapsed += Time.deltaTime)
		{
			float t = elapsed / this.teleportDuration;
			Vector2 position = Vector2.Lerp(initialHeroPos, targetHeroPos, t);
			heroTrans.SetPosition2D(position);
			this.heroTeleportObject.transform.SetPosition2D(position);
			cam.SnapToY(cameraYRange.GetLerpedValue(t));
			yield return null;
		}
		hc.transform.SetPosition2D(targetHeroPos);
		heroRenderer.enabled = true;
		this.heroTeleportObject.SetActive(false);
		this.mainAnimator.SetBool(WeaverLift._isChargingAnimParam, false);
		if (this.preTeleportActivate)
		{
			this.preTeleportActivate.SetActive(false);
		}
		target.mainAnimator.SetBool(WeaverLift._isChargingAnimParam, true);
		target.mainAnimator.Play(WeaverLift._chargeUpAnim, 0, 1f);
		if (target.arriveActivate)
		{
			target.arriveActivate.SetActive(true);
		}
		tk2dSpriteAnimationClip clipByName2 = heroAnimator.GetClipByName("Collect Normal 2");
		heroAnimator.PlayFromFrame(clipByName2, clipByName2.frames.Length - 1);
		heroFlash.flashWhiteLong();
		if (shaftGlows)
		{
			shaftGlows.FadeTo(0f, this.shaftGlowsFadeTime, null, false, null);
		}
		cam.camTarget.PositionToStart();
		cam.SnapToY(cameraYRange.End);
		cam.SetMode(prevCamMode);
		yield return new WaitForSeconds(target.arriveWait);
		target.CancelTransport();
		this.ChargeUpStopped();
		yield return new WaitForSeconds(heroAnimator.PlayAnimGetTime("Collect Normal 3"));
		hc.RegainControl();
		hc.StartAnimationControl();
		hc.AffectedByGravity(true);
		hc.cState.invulnerable = false;
		this.teleportRoutine = null;
		if (WeaverLift._isTeleportingLift == this)
		{
			WeaverLift._isTeleportingLift = null;
		}
		yield break;
	}

	// Token: 0x04003672 RID: 13938
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	// Token: 0x04003673 RID: 13939
	private static readonly int _isVisibleAnimParam = Animator.StringToHash("IsVisible");

	// Token: 0x04003674 RID: 13940
	private static readonly int _swapAnimParam = Animator.StringToHash("Swap");

	// Token: 0x04003675 RID: 13941
	private static readonly int _isChargingAnimParam = Animator.StringToHash("IsCharging");

	// Token: 0x04003676 RID: 13942
	private static readonly int _chargeUpAnim = Animator.StringToHash("Charge Up");

	// Token: 0x04003677 RID: 13943
	private static readonly int _isActiveAnimParam = Animator.StringToHash("IsActive");

	// Token: 0x04003678 RID: 13944
	private static readonly int _activeAnim = Animator.StringToHash("Active");

	// Token: 0x04003679 RID: 13945
	private static readonly int _inactiveAnim = Animator.StringToHash("Inactive");

	// Token: 0x0400367A RID: 13946
	[SerializeField]
	private WeaverLift liftUp;

	// Token: 0x0400367B RID: 13947
	[SerializeField]
	private WeaverLift liftDown;

	// Token: 0x0400367C RID: 13948
	[SerializeField]
	private NestedFadeGroupBase shaftGlowsUp;

	// Token: 0x0400367D RID: 13949
	[SerializeField]
	private NestedFadeGroupBase shaftGlowsDown;

	// Token: 0x0400367E RID: 13950
	[SerializeField]
	private float shaftGlowsFadeTime;

	// Token: 0x0400367F RID: 13951
	[SerializeField]
	private PersistentBoolItem isUnlockedTarget;

	// Token: 0x04003680 RID: 13952
	[Space]
	[SerializeField]
	private Animator mainAnimator;

	// Token: 0x04003681 RID: 13953
	[SerializeField]
	private CameraShakeTarget chargePlateShake;

	// Token: 0x04003682 RID: 13954
	[SerializeField]
	private CameraShakeTarget chargeRumble;

	// Token: 0x04003683 RID: 13955
	[SerializeField]
	private ParticleSystemPool chargeUpParticles;

	// Token: 0x04003684 RID: 13956
	[SerializeField]
	private GameObject chargeUpWindRegion;

	// Token: 0x04003685 RID: 13957
	[SerializeField]
	private GameObject chargeActivate;

	// Token: 0x04003686 RID: 13958
	[SerializeField]
	private TriggerEnterEvent firstActivateRange;

	// Token: 0x04003687 RID: 13959
	[SerializeField]
	private GameObject preTeleportActivate;

	// Token: 0x04003688 RID: 13960
	[SerializeField]
	private float teleportWait;

	// Token: 0x04003689 RID: 13961
	[SerializeField]
	private GameObject arriveActivate;

	// Token: 0x0400368A RID: 13962
	[SerializeField]
	private float arriveWait;

	// Token: 0x0400368B RID: 13963
	[Space]
	[SerializeField]
	private Transform heroTeleportPoint;

	// Token: 0x0400368C RID: 13964
	[SerializeField]
	private float teleportDuration;

	// Token: 0x0400368D RID: 13965
	[SerializeField]
	private GameObject heroTeleportObject;

	// Token: 0x0400368E RID: 13966
	[SerializeField]
	private float cameraYPos;

	// Token: 0x0400368F RID: 13967
	private CaptureAnimationEvent mainAnimatorCapture;

	// Token: 0x04003690 RID: 13968
	private bool isActive;

	// Token: 0x04003691 RID: 13969
	private Coroutine chargeUpRoutine;

	// Token: 0x04003692 RID: 13970
	private Coroutine teleportRoutine;

	// Token: 0x04003693 RID: 13971
	private bool teleportDirectionUp;

	// Token: 0x04003694 RID: 13972
	private static WeaverLift _isTeleportingLift;
}
