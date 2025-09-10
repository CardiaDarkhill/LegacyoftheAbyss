using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000493 RID: 1171
public class BellBench : UnlockablePropBase
{
	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x06002A42 RID: 10818 RVA: 0x000B7475 File Offset: 0x000B5675
	public bool IsBenchBroken
	{
		get
		{
			return !string.IsNullOrEmpty(this.fixedPDBool) && !PlayerData.instance.GetVariable(this.fixedPDBool);
		}
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000B749C File Offset: 0x000B569C
	private void Awake()
	{
		if (this.startWorkingPersistent)
		{
			this.isRaiseBlocked = true;
			this.startWorkingPersistent.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					this.isRaiseBlocked = false;
				}
			};
		}
		if (this.brokenAnimator)
		{
			this.brokenAnimator.enabled = false;
		}
		if (this.startWorkingTrigger)
		{
			this.isRaiseBlocked = true;
		}
		this.bellDisturbersImpulse = base.GetComponentsInChildren<Rigidbody2DDisturberImpulse>();
		this.bellDisturbersRumble = base.GetComponentsInChildren<Rigidbody2DDisturber>();
		if (this.startUnlocked && this.tollMachine)
		{
			this.tollMachine.SetActive(false);
		}
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000B753C File Offset: 0x000B573C
	private void Start()
	{
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			this.isHeroInPosition = true;
			this.Setup();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool forceDirect)
		{
			this.isHeroInPosition = true;
			this.Setup();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000B75A8 File Offset: 0x000B57A8
	private void Setup()
	{
		if (this.isSetup)
		{
			return;
		}
		if (!this.frame)
		{
			return;
		}
		this.initialFramePos = this.frame.localPosition;
		if (this.frameRenderer)
		{
			this.initialFrameColor = this.frameRenderer.color;
		}
		this.activateWhenActive.SetAllActive(false);
		this.isSetup = true;
		this.StartBehaviour();
		if (this.startUnlocked)
		{
			this.Opened();
			return;
		}
		this.isActivated = false;
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000B762C File Offset: 0x000B582C
	private void SetStartingBenchState(bool startUp)
	{
		if (!startUp)
		{
			this.frame.SetLocalPosition2D(this.isActivated ? this.downPositionUnlocked : this.downPositionLocked);
			this.realBenchFade.gameObject.SetActive(false);
			this.rosaryMachine.SetFsmBoolIfExists("Raise", false);
			this.rosaryMachine.SendEventSafe("START DOWN");
			if (this.frameRenderer)
			{
				this.frameRenderer.color = (this.isActivated ? this.initialFrameColor : this.frameInactiveColor);
				return;
			}
		}
		else
		{
			this.realBenchFade.AlphaSelf = 1f;
			this.realBenchFade.gameObject.SetActive(true);
			this.rosaryMachine.SetFsmBoolIfExists("Raise", true);
			this.rosaryMachine.SendEventSafe("START UP");
		}
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x000B7703 File Offset: 0x000B5903
	private void StartBehaviour()
	{
		if (this.behaviourRoutine != null)
		{
			base.StopCoroutine(this.behaviourRoutine);
		}
		this.behaviourRoutine = base.StartCoroutine(this.Behaviour());
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x000B772B File Offset: 0x000B592B
	private IEnumerator Behaviour()
	{
		while (!this.isHeroInPosition)
		{
			yield return null;
		}
		yield return null;
		bool startUp;
		if (this.forceStartDown)
		{
			startUp = false;
			this.forceStartDown = false;
		}
		else
		{
			startUp = (this.isActivated && !this.IsBenchBroken);
		}
		this.SetStartingBenchState(startUp);
		this.fakeBenchAnimator.gameObject.SetActive(false);
		this.activateWhenActive.SetAllActive(this.isActivated);
		bool wasActivated = this.isActivated;
		while (!this.isActivated)
		{
			yield return null;
		}
		if (!wasActivated && this.frameRenderer)
		{
			this.StartTimerRoutine(0f, this.frameColorLerpTime, delegate(float t)
			{
				this.frameRenderer.color = Color.Lerp(this.frameInactiveColor, this.initialFrameColor, t);
			}, null, null, false);
		}
		if (this.IsBenchBroken)
		{
			this.frame.SetLocalPosition2D(this.downPositionLocked);
			if (this.brokenAnimator)
			{
				this.brokenAnimator.enabled = true;
				this.brokenAnimator.Play("Rise", 0, wasActivated ? 1f : 0f);
			}
			while (this.IsBenchBroken)
			{
				yield return null;
			}
			this.EndBrokenState();
			yield return new WaitForSeconds(this.fixedRiseDelay);
			this.forceStartDown = true;
			this.StartBehaviour();
			yield break;
		}
		if (startUp)
		{
			this.isRaiseBlocked = false;
		}
		while (this.isRaiseBlocked)
		{
			yield return null;
			if (this.startWorkingTrigger && this.startWorkingTrigger.IsInside)
			{
				this.isRaiseBlocked = false;
			}
		}
		if (!startUp)
		{
			yield return new WaitForSeconds(this.moveUpPause);
			if (this.OnBeginRaise != null)
			{
				this.OnBeginRaise.Invoke();
			}
			this.StartRumble(true);
			this.fakeBenchAnimator.gameObject.SetActive(false);
			yield return base.StartCoroutine(this.MoveFrameTo(this.initialFramePos, this.moveUpSpeed, this.moveUpCurve));
			this.arriveSound.SpawnAndPlayOneShot(base.transform.position, null);
			this.StopRumble(false);
			if (this.cogController != null && this.shakeDuration > 0f)
			{
				base.StartCoroutine(this.CogShake());
			}
			if (this.OnEndRaise != null)
			{
				this.OnEndRaise.Invoke();
			}
			yield return new WaitForSeconds(this.benchRaiseDelay);
			this.benchAppearSound.SpawnAndPlayOneShot(this.fakeBenchAnimator.transform.position, null);
			this.fakeBenchAnimator.gameObject.SetActive(true);
			this.fakeBenchAnimator.Play(this.appearAnim);
			yield return null;
			yield return new WaitForSeconds(this.fakeBenchAnimator.GetCurrentAnimatorStateInfo(0).length);
			this.rosaryMachine.SetFsmBoolIfExists("Raise", true);
			this.realBenchFade.AlphaSelf = 0f;
			this.realBenchFade.gameObject.SetActive(true);
			this.benchActivateSound.SpawnAndPlayOneShot(this.realBenchFade.transform.position, null);
			yield return new WaitForSeconds(this.realBenchFade.FadeTo(1f, this.realBenchFadeInTime, null, false, null));
		}
		else
		{
			this.fakeBenchAnimator.gameObject.SetActive(true);
			this.fakeBenchAnimator.Play(this.silentAppearAnim, 0, 1f);
			this.realBenchFade.AlphaSelf = 1f;
			this.realBenchFade.gameObject.SetActive(true);
		}
		this.activateWhenActive.SetAllActive(true);
		yield break;
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x000B773A File Offset: 0x000B593A
	[ContextMenu("Cog Shake")]
	private void DoCogShake()
	{
		if (this.cogController && this.shakeDuration > 0f)
		{
			base.StartCoroutine(this.CogShake());
		}
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x000B7763 File Offset: 0x000B5963
	private IEnumerator CogShake()
	{
		float t = 1f;
		float inv = 1f / this.shakeDuration;
		float lastShakeAmount = 0f;
		while (t > 0f)
		{
			t -= Time.deltaTime * inv;
			float num = this.shakeIntensity * Mathf.Clamp01(t);
			float num2 = Mathf.PerlinNoise1D(Time.time * this.shakeFrequency) * 2f - 1f;
			float num3 = num * num2;
			float num4 = num3 - lastShakeAmount;
			lastShakeAmount = num3;
			this.cogController.AnimateRotation += num4;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000B7772 File Offset: 0x000B5972
	private void EndBrokenState()
	{
		if (this.brokenAnimator)
		{
			this.brokenAnimator.enabled = false;
		}
		this.StopRumble(true);
		this.StopRumble(false);
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000B779B File Offset: 0x000B599B
	private IEnumerator MoveFrameTo(Vector2 targetPosition, float speed, AnimationCurve curve)
	{
		if (this.cogController)
		{
			this.cogController.CaptureAnimateRotation();
		}
		Vector2 startPosition = this.frame.localPosition;
		float num = Vector2.Distance(startPosition, targetPosition);
		float lerpTime = num / speed;
		for (float elapsed = 0f; elapsed < lerpTime; elapsed += Time.deltaTime)
		{
			float num2 = curve.Evaluate(elapsed / lerpTime);
			this.frame.SetLocalPosition2D(Vector2.Lerp(startPosition, targetPosition, num2));
			if (this.cogController)
			{
				this.cogController.AnimateRotation = num2 * this.cogRotationMultiplier;
			}
			yield return null;
		}
		this.frame.SetLocalPosition2D(targetPosition);
		yield break;
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x000B77BF File Offset: 0x000B59BF
	private bool CanDoEffects()
	{
		return !this.IsBenchBroken || !this.brokenShakeRange || this.brokenShakeRange.IsInside;
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x000B77E8 File Offset: 0x000B59E8
	private void StartRumble(bool isRaising)
	{
		if (this.isRumbling)
		{
			return;
		}
		this.isRumbling = true;
		if (this.CanDoEffects())
		{
			this.cameraRumble.DoShake(this, true);
			if (isRaising)
			{
				this.raiseDust.PlayParticleSystems();
			}
			this.rumbleDust.PlayParticleSystems();
		}
		AudioSource audioSource = isRaising ? this.riseSource : this.lowerSource;
		if (audioSource)
		{
			audioSource.Play();
		}
		if (isRaising)
		{
			if (this.riseVibration)
			{
				if (this.heroVibrationRegion)
				{
					this.riseEmission = this.heroVibrationRegion.PlayVibrationOneShot(this.riseVibration, false, HeroVibrationRegion.VibrationSettings.None, null);
				}
				else
				{
					this.riseEmission = VibrationManager.PlayVibrationClipOneShot(this.riseVibration, null, false, "", false);
				}
			}
		}
		else if (this.lowerVibration)
		{
			if (this.heroVibrationRegion)
			{
				this.lowerEmission = this.heroVibrationRegion.PlayVibrationOneShot(this.lowerVibration, false, HeroVibrationRegion.VibrationSettings.Loop, null);
			}
			else
			{
				this.lowerEmission = VibrationManager.PlayVibrationClipOneShot(this.lowerVibration, null, true, "", false);
			}
		}
		foreach (Rigidbody2DDisturber rigidbody2DDisturber in this.bellDisturbersRumble)
		{
			if (rigidbody2DDisturber.isActiveAndEnabled)
			{
				rigidbody2DDisturber.StartRumble();
			}
		}
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000B794A File Offset: 0x000B5B4A
	public void StartRumbleRaising()
	{
		this.StartRumble(true);
	}

	// Token: 0x06002A50 RID: 10832 RVA: 0x000B7953 File Offset: 0x000B5B53
	public void StartRumbleLowering()
	{
		this.StartRumble(false);
	}

	// Token: 0x06002A51 RID: 10833 RVA: 0x000B795C File Offset: 0x000B5B5C
	private void StopRumble(bool isRaising)
	{
		this.cameraRumble.CancelShake();
		this.raiseDust.StopParticleSystems();
		this.rumbleDust.StopParticleSystems();
		if (isRaising)
		{
			VibrationEmission vibrationEmission = this.riseEmission;
			if (vibrationEmission != null)
			{
				vibrationEmission.Stop();
			}
			this.riseEmission = null;
		}
		else
		{
			if (this.lowerSource)
			{
				this.lowerSource.Stop();
			}
			VibrationEmission vibrationEmission2 = this.lowerEmission;
			if (vibrationEmission2 != null)
			{
				vibrationEmission2.Stop();
			}
			this.lowerEmission = null;
		}
		foreach (Rigidbody2DDisturber rigidbody2DDisturber in this.bellDisturbersRumble)
		{
			if (rigidbody2DDisturber.isActiveAndEnabled)
			{
				rigidbody2DDisturber.StopRumble();
			}
		}
		if (!this.isRumbling)
		{
			return;
		}
		this.isRumbling = false;
		if (this.CanDoEffects())
		{
			this.cameraRumbleEndShake.DoShake(this, true);
			this.raiseStopDust.PlayParticleSystems();
		}
		foreach (Rigidbody2DDisturberImpulse rigidbody2DDisturberImpulse in this.bellDisturbersImpulse)
		{
			if (rigidbody2DDisturberImpulse.isActiveAndEnabled)
			{
				rigidbody2DDisturberImpulse.Impulse();
			}
		}
	}

	// Token: 0x06002A52 RID: 10834 RVA: 0x000B7A58 File Offset: 0x000B5C58
	public void StopRumbleRaising()
	{
		this.StopRumble(true);
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x000B7A61 File Offset: 0x000B5C61
	public void StopRumbleLowering()
	{
		this.StopRumble(false);
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x000B7A6A File Offset: 0x000B5C6A
	public override void Open()
	{
		this.bellToneFSM.SendEventSafe("ACTIVATED");
		this.isActivated = true;
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x000B7A83 File Offset: 0x000B5C83
	public override void Opened()
	{
		this.Setup();
		this.Open();
		this.StartBehaviour();
		if (this.lowerSource)
		{
			this.lowerSource.Stop();
		}
	}

	// Token: 0x04002AB9 RID: 10937
	[SerializeField]
	private Animator fakeBenchAnimator;

	// Token: 0x04002ABA RID: 10938
	[SerializeField]
	private AudioEvent benchAppearSound;

	// Token: 0x04002ABB RID: 10939
	[SerializeField]
	private AudioEvent benchActivateSound;

	// Token: 0x04002ABC RID: 10940
	[SerializeField]
	private NestedFadeGroupBase realBenchFade;

	// Token: 0x04002ABD RID: 10941
	[SerializeField]
	private float realBenchFadeInTime;

	// Token: 0x04002ABE RID: 10942
	[SerializeField]
	private float benchRaiseDelay;

	// Token: 0x04002ABF RID: 10943
	[SerializeField]
	private PlayMakerFSM rosaryMachine;

	// Token: 0x04002AC0 RID: 10944
	[SerializeField]
	private GameObject tollMachine;

	// Token: 0x04002AC1 RID: 10945
	[SerializeField]
	private Transform frame;

	// Token: 0x04002AC2 RID: 10946
	[SerializeField]
	private Vector2 downPositionUnlocked;

	// Token: 0x04002AC3 RID: 10947
	[SerializeField]
	private Vector2 downPositionLocked;

	// Token: 0x04002AC4 RID: 10948
	[SerializeField]
	private float moveUpSpeed;

	// Token: 0x04002AC5 RID: 10949
	[SerializeField]
	private AnimationCurve moveUpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002AC6 RID: 10950
	[SerializeField]
	private float moveUpPause;

	// Token: 0x04002AC7 RID: 10951
	[SerializeField]
	private CogRotationController cogController;

	// Token: 0x04002AC8 RID: 10952
	[SerializeField]
	private float cogRotationMultiplier = 1f;

	// Token: 0x04002AC9 RID: 10953
	[SerializeField]
	private float shakeDuration = 1f;

	// Token: 0x04002ACA RID: 10954
	[SerializeField]
	private float shakeIntensity = 5f;

	// Token: 0x04002ACB RID: 10955
	[SerializeField]
	private float shakeFrequency = 10f;

	// Token: 0x04002ACC RID: 10956
	[SerializeField]
	private CameraShakeTarget cameraRumble;

	// Token: 0x04002ACD RID: 10957
	[SerializeField]
	private CameraShakeTarget cameraRumbleEndShake;

	// Token: 0x04002ACE RID: 10958
	[SerializeField]
	private PlayParticleEffects rumbleDust;

	// Token: 0x04002ACF RID: 10959
	[SerializeField]
	private PlayParticleEffects raiseDust;

	// Token: 0x04002AD0 RID: 10960
	[SerializeField]
	private PlayParticleEffects raiseStopDust;

	// Token: 0x04002AD1 RID: 10961
	[SerializeField]
	private AudioSource riseSource;

	// Token: 0x04002AD2 RID: 10962
	[SerializeField]
	private VibrationDataAsset riseVibration;

	// Token: 0x04002AD3 RID: 10963
	[SerializeField]
	private AudioEvent arriveSound;

	// Token: 0x04002AD4 RID: 10964
	[SerializeField]
	private AudioSource lowerSource;

	// Token: 0x04002AD5 RID: 10965
	[SerializeField]
	private VibrationDataAsset lowerVibration;

	// Token: 0x04002AD6 RID: 10966
	[Space]
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	// Token: 0x04002AD7 RID: 10967
	[SerializeField]
	private SpriteRenderer frameRenderer;

	// Token: 0x04002AD8 RID: 10968
	[SerializeField]
	private Color frameInactiveColor = Color.grey;

	// Token: 0x04002AD9 RID: 10969
	[SerializeField]
	private float frameColorLerpTime = 0.5f;

	// Token: 0x04002ADA RID: 10970
	[SerializeField]
	private PlayMakerFSM bellToneFSM;

	// Token: 0x04002ADB RID: 10971
	[SerializeField]
	private GameObject[] activateWhenActive;

	// Token: 0x04002ADC RID: 10972
	[Space]
	[SerializeField]
	private PersistentBoolItem startWorkingPersistent;

	// Token: 0x04002ADD RID: 10973
	[SerializeField]
	private TrackTriggerObjects startWorkingTrigger;

	// Token: 0x04002ADE RID: 10974
	[SerializeField]
	private bool startUnlocked;

	// Token: 0x04002ADF RID: 10975
	[Header("Broken Options")]
	[SerializeField]
	private Animator brokenAnimator;

	// Token: 0x04002AE0 RID: 10976
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string fixedPDBool;

	// Token: 0x04002AE1 RID: 10977
	[SerializeField]
	private TrackTriggerObjects brokenShakeRange;

	// Token: 0x04002AE2 RID: 10978
	[SerializeField]
	private float fixedRiseDelay;

	// Token: 0x04002AE3 RID: 10979
	[Header("Events")]
	public UnityEvent OnBeginRaise;

	// Token: 0x04002AE4 RID: 10980
	public UnityEvent OnEndRaise;

	// Token: 0x04002AE5 RID: 10981
	private readonly int appearAnim = Animator.StringToHash("Appear");

	// Token: 0x04002AE6 RID: 10982
	private readonly int silentAppearAnim = Animator.StringToHash("Silent Appear");

	// Token: 0x04002AE7 RID: 10983
	private bool isSetup;

	// Token: 0x04002AE8 RID: 10984
	private Vector3 initialFramePos;

	// Token: 0x04002AE9 RID: 10985
	private bool isRaiseBlocked;

	// Token: 0x04002AEA RID: 10986
	private Coroutine behaviourRoutine;

	// Token: 0x04002AEB RID: 10987
	private Coroutine musicRoutine;

	// Token: 0x04002AEC RID: 10988
	private bool isActivated;

	// Token: 0x04002AED RID: 10989
	private bool isHeroInPosition;

	// Token: 0x04002AEE RID: 10990
	private bool isRumbling;

	// Token: 0x04002AEF RID: 10991
	private bool forceStartDown;

	// Token: 0x04002AF0 RID: 10992
	private Color initialFrameColor;

	// Token: 0x04002AF1 RID: 10993
	private Rigidbody2DDisturberImpulse[] bellDisturbersImpulse;

	// Token: 0x04002AF2 RID: 10994
	private Rigidbody2DDisturber[] bellDisturbersRumble;

	// Token: 0x04002AF3 RID: 10995
	private VibrationEmission riseEmission;

	// Token: 0x04002AF4 RID: 10996
	private VibrationEmission lowerEmission;
}
