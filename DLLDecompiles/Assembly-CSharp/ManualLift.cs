using System;
using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x02000519 RID: 1305
public class ManualLift : MonoBehaviour, HeroPlatformStick.IMoveHooks, HeroPlatformStick.ITouchHooks
{
	// Token: 0x06002EEE RID: 12014 RVA: 0x000CEC88 File Offset: 0x000CCE88
	private void OnDrawGizmos()
	{
		float z = base.transform.position.z;
		Vector3 vector = this.leftTargetPos.ToVector3(z);
		Vector3 vector2 = this.rightTargetPos.ToVector3(z);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawWireSphere(vector, 0.3f);
		Gizmos.DrawWireSphere(vector2, 0.3f);
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x000CECDC File Offset: 0x000CCEDC
	private void Awake()
	{
		ManualLift.UnlockSide unlockSide = this.unlockFromSide;
		if (unlockSide != ManualLift.UnlockSide.Left)
		{
			if (unlockSide != ManualLift.UnlockSide.Right)
			{
				this.isUnlocked = true;
			}
			else if (this.callPlateRight)
			{
				this.callPlateRight.Activated += this.OnUnlockPlateActivated;
			}
		}
		else if (this.callPlateLeft)
		{
			this.callPlateLeft.Activated += this.OnUnlockPlateActivated;
		}
		if (this.buttonLeft)
		{
			SimpleButton simpleButton = this.buttonLeft;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, new Action<bool>(delegate(bool isDepressed)
			{
				this.isLeftPressed = isDepressed;
				this.UpdateDirection(true);
			}));
		}
		if (this.buttonRight)
		{
			SimpleButton simpleButton2 = this.buttonRight;
			simpleButton2.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton2.DepressedChange, new Action<bool>(delegate(bool isDepressed)
			{
				this.isRightPressed = isDepressed;
				this.UpdateDirection(true);
			}));
		}
		if (this.movingDamager)
		{
			this.movingDamager.gameObject.SetActive(false);
		}
		if (this.callPlateLeft)
		{
			this.callPlateLeft.Activated += delegate()
			{
				this.OnCallPlateActivated(-1);
			};
		}
		if (this.callPlateRight)
		{
			this.callPlateRight.Activated += delegate()
			{
				this.OnCallPlateActivated(1);
			};
		}
		if (this.unlockPersistent)
		{
			this.unlockPersistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isUnlocked;
			};
			this.unlockPersistent.OnSetSaveState += delegate(bool value)
			{
				this.isUnlocked = value;
				this.unlockAnimator.enabled = true;
				this.unlockAnimator.Play("Unlock", 0, this.isUnlocked ? 1f : 0f);
				this.unlockAnimator.Update(0f);
				this.unlockAnimator.enabled = false;
				if (this.isUnlocked)
				{
					this.OnUnlockPlateActivated();
				}
				this.UpdateButtons();
				this.SetStartPos();
			};
		}
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x000CEE58 File Offset: 0x000CD058
	private void Start()
	{
		if (this.tiltPlatFsm)
		{
			this.tiltPlatXFloat = this.tiltPlatFsm.FsmVariables.FindFsmFloat("Self X");
		}
		HeroController instance = HeroController.instance;
		if (instance && !instance.isHeroInPosition)
		{
			HeroController.HeroInPosition temp = null;
			temp = delegate(bool _)
			{
				this.SetStartPos();
				HeroController.instance.heroInPosition -= temp;
			};
			HeroController.instance.heroInPosition += temp;
		}
		else
		{
			this.SetStartPos();
		}
		this.UpdateButtons();
		this.UpdatePlates();
		if (this.unlockAnimator)
		{
			this.unlockAnimator.enabled = true;
			this.unlockAnimator.Update(0f);
			this.unlockAnimator.enabled = false;
		}
		if (this.supportTimeline)
		{
			this.supportTimeline.Evaluate();
		}
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x000CEF3C File Offset: 0x000CD13C
	private void Update()
	{
		bool flag = false;
		float num = this.targetTDirection;
		if (this.moveDelayLeft > 0f)
		{
			this.moveDelayLeft -= Time.deltaTime;
			if (this.moveDelayLeft > 0f)
			{
				num = 0f;
				flag = true;
			}
		}
		float num2;
		if (num == 0f)
		{
			num2 = 0f;
		}
		else
		{
			num2 = this.moveSpeed * num;
		}
		float num3 = (Mathf.Abs(num2) >= Mathf.Abs(this.currentVelocity) || (num2 > 0f && this.currentVelocity <= 0f) || (num2 < 0f && this.currentVelocity >= 0f)) ? this.acceleration : this.deceleration;
		this.currentVelocity = Mathf.Lerp(this.currentVelocity, num2, Time.deltaTime * num3);
		float num4 = this.currentVelocity / this.moveSpeed;
		if (this.supportTimeline)
		{
			this.supportTimeline.time += (double)(Time.deltaTime * num4 * this.supportTimelineSpeed);
			if (this.supportTimeline.time < 0.0)
			{
				this.supportTimeline.time = this.supportTimeline.duration;
			}
			else if (this.supportTimeline.time > this.supportTimeline.duration)
			{
				this.supportTimeline.time = 0.0;
			}
			this.supportTimeline.Evaluate();
		}
		if (this.movingAudioLoop)
		{
			if (Mathf.Abs(num4) > 0.01f)
			{
				if (!this.movingAudioLoop.isPlaying && this.previousInputDirection != 0)
				{
					this.movingAudioLoop.Play();
				}
				float time = Mathf.Abs(num4);
				this.movingAudioLoop.volume = this.movingAudioVolumeCurve.Evaluate(time);
				this.movingAudioLoop.pitch = this.movingAudioPitchCurve.Evaluate(time);
			}
			else if (this.movingAudioLoop.isPlaying)
			{
				this.movingAudioLoop.Stop();
			}
		}
		if (Mathf.Abs(this.currentVelocity) <= 0.001f)
		{
			if (this.wasMoving)
			{
				this.OnStoppedMoving.Invoke();
				this.wasMoving = false;
			}
			return;
		}
		if (!this.wasMoving)
		{
			this.OnStartedMoving.Invoke();
			this.wasMoving = true;
		}
		this.currentPosT = Mathf.Clamp01(this.currentPosT + this.speedFactor * this.currentVelocity * Time.deltaTime);
		this.UpdatePosition();
		if (flag)
		{
			return;
		}
		if ((this.currentPosT <= 0f && num < 0f) || (this.currentPosT >= 1f && num > 0f))
		{
			this.StopMoving(true);
		}
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x000CF1E4 File Offset: 0x000CD3E4
	private void SetStartPos()
	{
		HeroController instance = HeroController.instance;
		if (this.isUnlocked)
		{
			Vector3 position = instance.transform.position;
			this.currentPosT = ((Vector2.Distance(position, this.leftTargetPos) < Vector2.Distance(position, this.rightTargetPos)) ? 0f : 1f);
		}
		else
		{
			ManualLift.UnlockSide unlockSide = this.unlockFromSide;
			if (unlockSide != ManualLift.UnlockSide.Left)
			{
				if (unlockSide != ManualLift.UnlockSide.Right)
				{
					Debug.LogError("Should not be in this state!");
				}
				else
				{
					this.currentPosT = 1f;
				}
			}
			else
			{
				this.currentPosT = 0f;
			}
		}
		this.UpdatePosition();
		this.UpdatePlates();
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x000CF284 File Offset: 0x000CD484
	private void UpdateDirection(bool overrideCall)
	{
		if (!this.isUnlocked)
		{
			return;
		}
		if (this.calledDirection != 0 && overrideCall)
		{
			this.calledDirection = 0;
		}
		int num;
		if (this.calledDirection != 0)
		{
			num = this.calledDirection;
		}
		else
		{
			num = 0;
			if (this.isLeftPressed)
			{
				num--;
			}
			if (this.isRightPressed)
			{
				num++;
			}
		}
		if (num != this.previousInputDirection)
		{
			this.moveDelayLeft = this.moveDelay;
			if (num != 0)
			{
				this.takeOffAudio.SpawnAndPlayOneShot(this.movingAudioLoop.transform.position, null);
			}
		}
		this.previousInputDirection = num;
		if (num == 0)
		{
			this.StopMoving(false);
			return;
		}
		this.targetTDirection = (float)((num < 0) ? -1 : 1);
		this.speedFactor = 1f / Vector2.Distance(this.leftTargetPos, this.rightTargetPos);
		if (this.movingDamager)
		{
			GameObject gameObject = this.movingDamager.gameObject;
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			Vector2 normalized = ((num > 0) ? (this.rightTargetPos - this.leftTargetPos) : (this.leftTargetPos - this.rightTargetPos)).normalized;
			float direction = Vector2.Angle(Vector2.right, normalized);
			this.movingDamager.direction = direction;
		}
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x000CF3C0 File Offset: 0x000CD5C0
	private void StopMoving(bool hardStop)
	{
		this.targetTDirection = 0f;
		this.calledDirection = 0;
		if (hardStop)
		{
			float num = Mathf.Abs(this.currentVelocity) / this.moveSpeed;
			if (num > 0.8f)
			{
				this.endImpactShakeBig.DoShake(this, true);
				if (this.currentPosT < 0.5f)
				{
					if (!string.IsNullOrEmpty(this.bigImpactLeftEvent))
					{
						EventRegister.SendEvent(this.bigImpactLeftEvent, null);
					}
				}
				else if (!string.IsNullOrEmpty(this.bigImpactRightEvent))
				{
					EventRegister.SendEvent(this.bigImpactRightEvent, null);
				}
			}
			else if (num > 0.3f)
			{
				this.endImpactShakeSmall.DoShake(this, true);
				if (this.currentPosT < 0.5f)
				{
					if (!string.IsNullOrEmpty(this.smallImpactLeftEvent))
					{
						EventRegister.SendEvent(this.smallImpactLeftEvent, null);
					}
				}
				else if (!string.IsNullOrEmpty(this.smallImpactRightEvent))
				{
					EventRegister.SendEvent(this.smallImpactRightEvent, null);
				}
			}
			this.currentVelocity = 0f;
			this.movingAudioLoop.Stop();
			this.arriveAudio.SpawnAndPlayOneShot(this.movingAudioLoop.transform.position, null);
			this.OnStoppedMoving.Invoke();
			this.wasMoving = false;
		}
		if (this.movingDamager)
		{
			this.movingDamager.gameObject.SetActive(false);
		}
		if (this.heroSquasherLeft)
		{
			this.heroSquasherLeft.SetActive(false);
		}
		if (this.heroSquasherRight)
		{
			this.heroSquasherRight.SetActive(false);
		}
		this.UpdatePlates();
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x000CF544 File Offset: 0x000CD744
	private void UpdatePosition()
	{
		Vector2 position = Vector2.Lerp(this.leftTargetPos, this.rightTargetPos, this.currentPosT);
		this.moveTransform.SetPosition2D(position);
		if (this.tiltPlatXFloat != null)
		{
			this.tiltPlatXFloat.Value = this.moveTransform.position.x;
		}
		Vector3 localPosition = this.moveTransform.localPosition;
		bool flag = Mathf.Abs(this.currentVelocity) > 0.01f;
		if (this.heroSquasherLeft)
		{
			this.heroSquasherLeft.SetActive(flag && localPosition.x <= this.heroSquashXLeft);
		}
		if (this.heroSquasherRight)
		{
			this.heroSquasherRight.SetActive(flag && localPosition.x >= this.heroSquashXRight);
		}
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x000CF618 File Offset: 0x000CD818
	private void OnUnlockPlateActivated()
	{
		ManualLift.UnlockSide unlockSide = this.unlockFromSide;
		if (unlockSide != ManualLift.UnlockSide.Left)
		{
			if (unlockSide != ManualLift.UnlockSide.Right)
			{
				Debug.LogError("Could not unsubscribe", this);
			}
			else if (this.callPlateRight)
			{
				this.callPlateRight.Activated -= this.OnUnlockPlateActivated;
			}
		}
		else if (this.callPlateLeft)
		{
			this.callPlateLeft.Activated -= this.OnUnlockPlateActivated;
		}
		if (this.isUnlocked)
		{
			return;
		}
		if (this.unlockRoutine == null)
		{
			this.unlockRoutine = base.StartCoroutine(this.Unlock());
		}
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x000CF6B0 File Offset: 0x000CD8B0
	private IEnumerator Unlock()
	{
		this.isUnlocked = true;
		if (this.unlockAnimator)
		{
			this.unlockAnimator.enabled = true;
			this.unlockAnimator.Play("Unlock", 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.unlockAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.OnUnlock.Invoke();
		this.UpdateButtons();
		this.UpdatePlates();
		yield break;
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x000CF6BF File Offset: 0x000CD8BF
	private void OnCallPlateActivated(int direction)
	{
		if (!this.isUnlocked)
		{
			return;
		}
		this.calledDirection = direction;
		this.UpdateDirection(false);
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x000CF6D8 File Offset: 0x000CD8D8
	private void UpdatePlates()
	{
		if (this.isUnlocked)
		{
			if (this.callPlateRight)
			{
				if (this.currentPosT >= 0.99f)
				{
					this.callPlateRight.ActivateSilent();
				}
				else
				{
					this.callPlateRight.Deactivate();
				}
			}
			if (this.callPlateLeft)
			{
				if (this.currentPosT <= 0.01f)
				{
					this.callPlateLeft.ActivateSilent();
					return;
				}
				this.callPlateLeft.Deactivate();
				return;
			}
		}
		else
		{
			ManualLift.UnlockSide unlockSide = this.unlockFromSide;
			if (unlockSide != ManualLift.UnlockSide.Left)
			{
				if (unlockSide != ManualLift.UnlockSide.Right)
				{
					return;
				}
				if (this.callPlateLeft)
				{
					this.callPlateLeft.ActivateSilent();
				}
			}
			else if (this.callPlateRight)
			{
				this.callPlateRight.ActivateSilent();
				return;
			}
		}
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x000CF794 File Offset: 0x000CD994
	private void UpdateButtons()
	{
		if (this.buttonLeft)
		{
			this.buttonLeft.SetLocked(!this.isUnlocked);
		}
		if (this.buttonRight)
		{
			this.buttonRight.SetLocked(!this.isUnlocked);
		}
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x000CF7E4 File Offset: 0x000CD9E4
	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		this.OnStartedMoving.AddListener(delegate()
		{
			onStartMove();
		});
		this.OnStoppedMoving.AddListener(delegate()
		{
			onStopMove();
		});
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x000CF834 File Offset: 0x000CDA34
	public void AddTouchHooks(Action onStartTouching, Action onStopTouching)
	{
		bool wasTouching = false;
		if (this.buttonLeft)
		{
			SimpleButton simpleButton = this.buttonLeft;
			simpleButton.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton.DepressedChange, new Action<bool>(delegate(bool _)
			{
				base.<AddTouchHooks>g__CheckTouching|0();
			}));
		}
		if (this.buttonRight)
		{
			SimpleButton simpleButton2 = this.buttonRight;
			simpleButton2.DepressedChange = (Action<bool>)Delegate.Combine(simpleButton2.DepressedChange, new Action<bool>(delegate(bool _)
			{
				base.<AddTouchHooks>g__CheckTouching|0();
			}));
		}
	}

	// Token: 0x04003177 RID: 12663
	[SerializeField]
	private SimpleButton buttonLeft;

	// Token: 0x04003178 RID: 12664
	[SerializeField]
	private SimpleButton buttonRight;

	// Token: 0x04003179 RID: 12665
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform moveTransform;

	// Token: 0x0400317A RID: 12666
	[SerializeField]
	private float moveSpeed;

	// Token: 0x0400317B RID: 12667
	[SerializeField]
	private float acceleration;

	// Token: 0x0400317C RID: 12668
	[SerializeField]
	private float deceleration;

	// Token: 0x0400317D RID: 12669
	[SerializeField]
	private float moveDelay;

	// Token: 0x0400317E RID: 12670
	[SerializeField]
	private PlayMakerFSM tiltPlatFsm;

	// Token: 0x0400317F RID: 12671
	[Space]
	[SerializeField]
	private DamageEnemies movingDamager;

	// Token: 0x04003180 RID: 12672
	[SerializeField]
	private GameObject heroSquasherLeft;

	// Token: 0x04003181 RID: 12673
	[SerializeField]
	private float heroSquashXLeft;

	// Token: 0x04003182 RID: 12674
	[SerializeField]
	private GameObject heroSquasherRight;

	// Token: 0x04003183 RID: 12675
	[SerializeField]
	private float heroSquashXRight;

	// Token: 0x04003184 RID: 12676
	[Space]
	[SerializeField]
	private CameraShakeTarget endImpactShakeSmall;

	// Token: 0x04003185 RID: 12677
	[SerializeField]
	private CameraShakeTarget endImpactShakeBig;

	// Token: 0x04003186 RID: 12678
	[SerializeField]
	private PlayableDirector supportTimeline;

	// Token: 0x04003187 RID: 12679
	[SerializeField]
	private float supportTimelineSpeed;

	// Token: 0x04003188 RID: 12680
	[Space]
	[SerializeField]
	private AudioSource movingAudioLoop;

	// Token: 0x04003189 RID: 12681
	[SerializeField]
	private AnimationCurve movingAudioPitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400318A RID: 12682
	[SerializeField]
	private AnimationCurve movingAudioVolumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400318B RID: 12683
	[SerializeField]
	private AudioEvent takeOffAudio;

	// Token: 0x0400318C RID: 12684
	[SerializeField]
	private AudioEvent arriveAudio;

	// Token: 0x0400318D RID: 12685
	[Space]
	[SerializeField]
	private Vector2 leftTargetPos;

	// Token: 0x0400318E RID: 12686
	[SerializeField]
	private Vector2 rightTargetPos;

	// Token: 0x0400318F RID: 12687
	[SerializeField]
	private TempPressurePlate callPlateLeft;

	// Token: 0x04003190 RID: 12688
	[SerializeField]
	private TempPressurePlate callPlateRight;

	// Token: 0x04003191 RID: 12689
	[SerializeField]
	private ManualLift.UnlockSide unlockFromSide;

	// Token: 0x04003192 RID: 12690
	[SerializeField]
	private PersistentBoolItem unlockPersistent;

	// Token: 0x04003193 RID: 12691
	[SerializeField]
	private Animator unlockAnimator;

	// Token: 0x04003194 RID: 12692
	[Space]
	[SerializeField]
	private string smallImpactLeftEvent;

	// Token: 0x04003195 RID: 12693
	[SerializeField]
	private string bigImpactLeftEvent;

	// Token: 0x04003196 RID: 12694
	[SerializeField]
	private string smallImpactRightEvent;

	// Token: 0x04003197 RID: 12695
	[SerializeField]
	private string bigImpactRightEvent;

	// Token: 0x04003198 RID: 12696
	[Space]
	public UnityEvent OnUnlock;

	// Token: 0x04003199 RID: 12697
	[Space]
	public UnityEvent OnStartedMoving;

	// Token: 0x0400319A RID: 12698
	public UnityEvent OnStoppedMoving;

	// Token: 0x0400319B RID: 12699
	private FsmFloat tiltPlatXFloat;

	// Token: 0x0400319C RID: 12700
	private bool isLeftPressed;

	// Token: 0x0400319D RID: 12701
	private bool isRightPressed;

	// Token: 0x0400319E RID: 12702
	private int calledDirection;

	// Token: 0x0400319F RID: 12703
	private int previousInputDirection;

	// Token: 0x040031A0 RID: 12704
	private float moveDelayLeft;

	// Token: 0x040031A1 RID: 12705
	private bool wasMoving;

	// Token: 0x040031A2 RID: 12706
	private float currentVelocity;

	// Token: 0x040031A3 RID: 12707
	private float currentPosT;

	// Token: 0x040031A4 RID: 12708
	private float targetTDirection;

	// Token: 0x040031A5 RID: 12709
	private float speedFactor;

	// Token: 0x040031A6 RID: 12710
	private bool isUnlocked;

	// Token: 0x040031A7 RID: 12711
	private Coroutine unlockRoutine;

	// Token: 0x02001823 RID: 6179
	private enum UnlockSide
	{
		// Token: 0x040090E0 RID: 37088
		None,
		// Token: 0x040090E1 RID: 37089
		Left,
		// Token: 0x040090E2 RID: 37090
		Right
	}
}
