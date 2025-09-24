using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200056F RID: 1391
public class BlackThreadStrand : MonoBehaviour, IInitialisable
{
	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x060031B6 RID: 12726 RVA: 0x000DC97F File Offset: 0x000DAB7F
	private bool CanSpawnCreature
	{
		get
		{
			return this.isVisible && !this.isInactive;
		}
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x000DC994 File Offset: 0x000DAB94
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.heroRadius);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(this.spineSpawnMin, this.spineSpawnMax);
	}

	// Token: 0x060031B8 RID: 12728 RVA: 0x000DC9D0 File Offset: 0x000DABD0
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.hc = HeroController.SilentInstance;
		for (int i = 0; i < this.spinePrefabs.Length; i++)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.spinePrefabs[i].gameObject, 30, i == this.spinePrefabs.Length - 1, false, true);
		}
		this.baseEmissionRates = new float[this.particles.Length];
		for (int j = 0; j < this.particles.Length; j++)
		{
			this.baseEmissionRates[j] = this.particles[j].emission.rateOverTimeMultiplier;
		}
		this.RegisterRageEvents();
		if (this.useProbability)
		{
			this.probabilitySpines = (from prefab in this.spinePrefabs
			select new BlackThreadStrand.ProbabilitySpine(prefab)).ToArray<BlackThreadStrand.ProbabilitySpine>();
		}
		this.isMenuScene = GameManager.instance.IsMenuScene();
		if (this.isMenuScene)
		{
			return true;
		}
		if (this.strandObject != null)
		{
			this.visibilityGroup = this.strandObject.AddComponentIfNotPresent<VisibilityGroup>();
			this.isVisible = this.visibilityGroup.IsVisible;
			this.visibilityGroup.OnVisibilityChanged += this.OnVisibilityChanged;
		}
		return true;
	}

	// Token: 0x060031B9 RID: 12729 RVA: 0x000DCB20 File Offset: 0x000DAD20
	private void RegisterRageEvents()
	{
		if (this.rageStartEvent)
		{
			this.rageStartEvent.ReceivedEvent += this.OnRageStart;
		}
		if (this.rageEndEvent)
		{
			this.rageEndEvent.ReceivedEvent += this.OnRageEnd;
		}
	}

	// Token: 0x060031BA RID: 12730 RVA: 0x000DCB78 File Offset: 0x000DAD78
	private void UnregisterRageEvents()
	{
		if (this.rageStartEvent)
		{
			this.rageStartEvent.ReceivedEvent -= this.OnRageStart;
		}
		if (this.rageEndEvent)
		{
			this.rageEndEvent.ReceivedEvent -= this.OnRageEnd;
		}
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x000DCBCD File Offset: 0x000DADCD
	private void OnRageStart()
	{
		this.forceRage = true;
	}

	// Token: 0x060031BC RID: 12732 RVA: 0x000DCBD6 File Offset: 0x000DADD6
	private void OnRageEnd()
	{
		this.forceRage = false;
	}

	// Token: 0x060031BD RID: 12733 RVA: 0x000DCBE0 File Offset: 0x000DADE0
	private void OnVisibilityChanged(bool visible)
	{
		this.isVisible = visible;
		if (this.isVisible)
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(true);
			}
			if (this.behaviourRoutine == null)
			{
				this.behaviourRoutine = base.StartCoroutine(this.BehaviourRoutine());
				return;
			}
		}
		else
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060031BE RID: 12734 RVA: 0x000DCC5D File Offset: 0x000DAE5D
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		return true;
	}

	// Token: 0x060031BF RID: 12735 RVA: 0x000DCC83 File Offset: 0x000DAE83
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x060031C0 RID: 12736 RVA: 0x000DCC8C File Offset: 0x000DAE8C
	private void OnEnable()
	{
		if (this.hc)
		{
			if (BlackThreadStrand._activeStrands == null)
			{
				BlackThreadStrand._activeStrands = new List<BlackThreadStrand>();
			}
			BlackThreadStrand._activeStrands.Add(this);
			if (!BlackThreadStrand._audioManagerStrand)
			{
				BlackThreadStrand._audioManagerStrand = this;
				this.audioManagerRoutine = base.StartCoroutine(this.SharedAudioControl());
			}
		}
		if (!this.isMenuScene && this.animator)
		{
			this.animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		if (!this.visibilityGroup)
		{
			this.OnVisibilityChanged(true);
		}
	}

	// Token: 0x060031C1 RID: 12737 RVA: 0x000DCD1C File Offset: 0x000DAF1C
	private void OnDisable()
	{
		List<BlackThreadStrand> activeStrands = BlackThreadStrand._activeStrands;
		if (activeStrands != null)
		{
			activeStrands.Remove(this);
		}
		this.StopLoopAudio(true);
		if (this == BlackThreadStrand._audioManagerStrand)
		{
			BlackThreadStrand._audioManagerStrand = null;
			base.StopCoroutine(this.audioManagerRoutine);
			this.audioManagerRoutine = null;
			List<BlackThreadStrand> activeStrands2 = BlackThreadStrand._activeStrands;
			if (activeStrands2 != null && activeStrands2.Count > 0)
			{
				BlackThreadStrand._audioManagerStrand = BlackThreadStrand._activeStrands[0];
				BlackThreadStrand._audioManagerStrand.closestStrandsTemp = this.closestStrandsTemp;
				this.closestStrandsTemp = null;
				BlackThreadStrand._audioManagerStrand.audioManagerRoutine = BlackThreadStrand._audioManagerStrand.StartCoroutine(BlackThreadStrand._audioManagerStrand.SharedAudioControl());
			}
			else
			{
				BlackThreadStrand._activeStrands = null;
			}
		}
		if (!this.visibilityGroup)
		{
			this.OnVisibilityChanged(false);
		}
		if (this.behaviourRoutine != null)
		{
			base.StopCoroutine(this.behaviourRoutine);
			this.behaviourRoutine = null;
		}
	}

	// Token: 0x060031C2 RID: 12738 RVA: 0x000DCDFA File Offset: 0x000DAFFA
	private void OnDestroy()
	{
		this.UnregisterRageEvents();
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x000DCE04 File Offset: 0x000DB004
	private void PositionRageChild()
	{
		if (!this.rageChild)
		{
			return;
		}
		Vector3 closestPosToHero = this.GetClosestPosToHero();
		this.rageChild.SetPosition2D(closestPosToHero);
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x000DCE38 File Offset: 0x000DB038
	private Vector3 GetClosestPosToHero()
	{
		if (!(this.hc != null))
		{
			return base.transform.position;
		}
		Vector3 position = this.hc.transform.position;
		Transform transform = base.transform;
		float num = this.heroRadius * this.heroRadius;
		Vector3 vector = transform.TransformPoint(this.spineSpawnMin);
		Vector3 vector2 = transform.TransformPoint(this.spineSpawnMax) - vector;
		Vector3 lhs = position - vector;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num2 = Mathf.Clamp01(Vector3.Dot(lhs, vector2) / sqrMagnitude);
		float sqrMagnitude2 = (vector + num2 * vector2 - position).sqrMagnitude;
		float num3 = 0f;
		float num4 = 1f;
		bool flag = false;
		if (sqrMagnitude2 < num)
		{
			float num5 = Mathf.Sqrt(num - sqrMagnitude2);
			float num6 = Mathf.Sqrt(sqrMagnitude);
			float num7 = num5 / num6;
			float b = Mathf.Clamp01(num2 - num7);
			float b2 = Mathf.Clamp01(num2 + num7);
			num3 = Mathf.Max(0f, b);
			num4 = Mathf.Min(1f, b2);
			flag = (num3 <= num4);
		}
		float t = flag ? Random.Range(num3, num4) : Random.value;
		Vector3 position2 = Vector3.Lerp(this.spineSpawnMin, this.spineSpawnMax, t);
		return transform.TransformPoint(position2);
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x000DCF9A File Offset: 0x000DB19A
	private IEnumerator BehaviourRoutine()
	{
		Transform trans = base.transform;
		float prevEmissionMult = 0f;
		bool wasWaitingForLoop = false;
		bool hasHero = this.hc != null;
		while (this.CanSpawnCreature)
		{
			bool isNeedolinPlaying = HeroPerformanceRegion.GetAffectedState(trans, true) > HeroPerformanceRegion.AffectedState.None;
			bool waitForLoop = false;
			MinMaxFloat delay;
			int anim;
			float emissionMult;
			if (this.forceRage || isNeedolinPlaying)
			{
				waitForLoop = true;
				delay = this.spineSpawnDelayNeedolin;
				anim = BlackThreadStrand._rageAnim;
				emissionMult = this.emissionMultNeedolin;
				this.PositionRageChild();
				if (!wasWaitingForLoop && this.animator)
				{
					this.animator.Play(BlackThreadStrand._rageStartAnim, 0, 0f);
					yield return null;
					yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
				}
			}
			else if (this.rangeTrigger && this.rangeTrigger.IsInside)
			{
				delay = this.spineSpawnDelayInRange;
				anim = BlackThreadStrand._alertAnim;
				emissionMult = this.emissionMultInRange;
			}
			else
			{
				delay = this.spineSpawnDelay;
				anim = BlackThreadStrand._idleAnim;
				emissionMult = 1f;
			}
			if (this.animator)
			{
				if (waitForLoop)
				{
					this.animator.Play(anim, 0, 0f);
				}
				else
				{
					if (wasWaitingForLoop)
					{
						this.animator.Play(BlackThreadStrand._rageEndAnim, 0, 0f);
						yield return null;
						yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
					}
					this.animator.Play(anim);
				}
			}
			if (Math.Abs(emissionMult - prevEmissionMult) > Mathf.Epsilon)
			{
				for (int i = 0; i < this.particles.Length; i++)
				{
					this.particles[i].emission.rateOverTimeMultiplier = emissionMult * this.baseEmissionRates[i];
				}
			}
			if (waitForLoop && this.animator)
			{
				yield return null;
				yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
			}
			wasWaitingForLoop = waitForLoop;
			float randomValue = delay.GetRandomValue();
			if (randomValue <= Mathf.Epsilon)
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSecondsInterruptable(randomValue, () => this.skipSpawnDelay, false);
				if (!this.forceRage)
				{
					if (!this.CanSpawnCreature)
					{
						break;
					}
					BlackThreadSpine prefab = this.useProbability ? Probability.GetRandomItemByProbabilityFair<BlackThreadStrand.ProbabilitySpine, BlackThreadSpine>(this.probabilitySpines, ref this.probabilities, 2f) : this.spinePrefabs[Random.Range(0, this.spinePrefabs.Length)];
					Vector3 position2;
					if (hasHero)
					{
						Vector2 vector = this.hc.transform.position;
						float num = this.heroRadius * this.heroRadius;
						Vector3 vector2 = trans.TransformPoint(this.spineSpawnMin);
						Vector3 vector3 = trans.TransformPoint(this.spineSpawnMax) - vector2;
						Vector3 lhs = vector - vector2;
						float sqrMagnitude = vector3.sqrMagnitude;
						float num2 = Mathf.Clamp01(Vector3.Dot(lhs, vector3) / sqrMagnitude);
						float sqrMagnitude2 = (vector2 + num2 * vector3 - vector).sqrMagnitude;
						float num3 = 0f;
						float num4 = 1f;
						bool flag = false;
						if (sqrMagnitude2 < num)
						{
							float num5 = Mathf.Sqrt(num - sqrMagnitude2);
							float num6 = Mathf.Sqrt(sqrMagnitude);
							float num7 = num5 / num6;
							float b = Mathf.Clamp01(num2 - num7);
							float b2 = Mathf.Clamp01(num2 + num7);
							num3 = Mathf.Max(0f, b);
							num4 = Mathf.Min(1f, b2);
							flag = (num3 <= num4);
						}
						float t = flag ? Random.Range(num3, num4) : Random.value;
						Vector3 position = Vector3.Lerp(this.spineSpawnMin, this.spineSpawnMax, t);
						position2 = trans.TransformPoint(position);
					}
					else
					{
						Vector3 position3 = Vector3.Lerp(this.spineSpawnMin, this.spineSpawnMax, Random.Range(0f, 1f));
						position2 = trans.TransformPoint(position3);
					}
					BlackThreadSpine blackThreadSpine = prefab.Spawn<BlackThreadSpine>();
					Transform transform = blackThreadSpine.transform;
					transform.position = position2;
					transform.rotation = trans.rotation;
					if (this.setSpineScaleRelative)
					{
						transform.localScale = trans.lossyScale;
						blackThreadSpine.UpdateInitialScale();
					}
					blackThreadSpine.Spawned(isNeedolinPlaying);
					delay = default(MinMaxFloat);
				}
			}
		}
		this.behaviourRoutine = null;
		yield break;
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x000DCFA9 File Offset: 0x000DB1A9
	public void RageForTime(float time)
	{
		this.RageForTime(time, true);
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x000DCFB3 File Offset: 0x000DB1B3
	public void RageForTime(float time, bool skipDelay)
	{
		if (this.rageTimer != null)
		{
			base.StopCoroutine(this.rageTimer);
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.rageTimer = base.StartCoroutine(this.RageRoutine(time, skipDelay));
	}

	// Token: 0x060031C8 RID: 12744 RVA: 0x000DCFE6 File Offset: 0x000DB1E6
	private IEnumerator RageRoutine(float time, bool skipDelay)
	{
		this.forceRage = true;
		this.skipSpawnDelay = skipDelay;
		yield return new WaitForSeconds(time);
		this.forceRage = false;
		this.skipSpawnDelay = false;
		this.rageTimer = null;
		yield break;
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x000DD003 File Offset: 0x000DB203
	public void Deactivate()
	{
		this.isInactive = true;
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x000DD00C File Offset: 0x000DB20C
	private IEnumerator SharedAudioControl()
	{
		yield return null;
		if (this.closestStrandsTemp == null)
		{
			this.closestStrandsTemp = new List<BlackThreadStrand.AudioLoopInfo>(BlackThreadStrand._activeStrands.Count);
		}
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		for (;;)
		{
			Vector3 position = this.hc.transform.position;
			foreach (BlackThreadStrand blackThreadStrand in BlackThreadStrand._activeStrands)
			{
				if (blackThreadStrand.loopAudioSource)
				{
					Vector3 closestPosToHero = blackThreadStrand.GetClosestPosToHero();
					float distance = Vector3.SqrMagnitude(closestPosToHero - position);
					this.closestStrandsTemp.Add(new BlackThreadStrand.AudioLoopInfo
					{
						Strand = blackThreadStrand,
						LoopPosition = closestPosToHero,
						Distance = distance
					});
				}
			}
			this.closestStrandsTemp.Sort((BlackThreadStrand.AudioLoopInfo a, BlackThreadStrand.AudioLoopInfo b) => a.Distance.CompareTo(b.Distance));
			for (int i = 0; i < this.closestStrandsTemp.Count; i++)
			{
				BlackThreadStrand.AudioLoopInfo audioLoopInfo = this.closestStrandsTemp[i];
				if (i < this.activeLoopsCount)
				{
					audioLoopInfo.Strand.loopAudioSource.transform.position = audioLoopInfo.LoopPosition;
					audioLoopInfo.Strand.StartLoopAudio();
				}
				else
				{
					audioLoopInfo.Strand.StopLoopAudio(false);
				}
			}
			this.closestStrandsTemp.Clear();
			yield return wait;
		}
		yield break;
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x000DD01C File Offset: 0x000DB21C
	private void StartLoopAudio()
	{
		if (!this.loopAudioSource || this.loopAudioSource.isPlaying)
		{
			return;
		}
		if (this.loopAudioTable == null)
		{
			return;
		}
		this.loopAudioSource.clip = this.loopAudioTable.SelectClip(false);
		this.loopAudioSource.pitch = this.loopAudioTable.SelectPitch();
		this.loopAudioSource.volume = 0f;
		this.audioFadeTarget = this.loopAudioTable.SelectVolume();
		if (this.audioFadeRoutine != null)
		{
			base.StopCoroutine(this.audioFadeRoutine);
			this.audioFadeRoutine = null;
		}
		this.audioFadeRoutine = this.StartTimerRoutine(0f, this.audioFadeDuration, delegate(float t)
		{
			this.loopAudioSource.volume = t * this.audioFadeTarget;
		}, null, null, false);
		this.loopAudioSource.timeSamples = Random.Range(0, this.loopAudioSource.clip.samples);
		this.loopAudioSource.loop = true;
		this.loopAudioSource.Play();
		this.isFadingOut = false;
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x000DD124 File Offset: 0x000DB324
	private void StopLoopAudio(bool isInstant)
	{
		if (this.isFadingOut)
		{
			return;
		}
		if (!this.loopAudioSource || !this.loopAudioSource.isPlaying)
		{
			return;
		}
		if (isInstant)
		{
			this.loopAudioSource.Stop();
			this.loopAudioSource.clip = null;
			return;
		}
		if (this.audioFadeRoutine != null)
		{
			base.StopCoroutine(this.audioFadeRoutine);
			this.audioFadeRoutine = null;
		}
		this.audioFadeTarget = this.loopAudioSource.volume;
		this.isFadingOut = true;
		this.audioFadeRoutine = this.StartTimerRoutine(0f, this.audioFadeDuration, delegate(float t)
		{
			this.loopAudioSource.volume = (1f - t) * this.audioFadeTarget;
		}, null, delegate
		{
			this.loopAudioSource.Stop();
			this.isFadingOut = false;
		}, false);
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x000DD234 File Offset: 0x000DB434
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400351E RID: 13598
	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	// Token: 0x0400351F RID: 13599
	private static readonly int _alertAnim = Animator.StringToHash("Alert");

	// Token: 0x04003520 RID: 13600
	private static readonly int _rageStartAnim = Animator.StringToHash("Rage Start");

	// Token: 0x04003521 RID: 13601
	private static readonly int _rageAnim = Animator.StringToHash("Rage");

	// Token: 0x04003522 RID: 13602
	private static readonly int _rageEndAnim = Animator.StringToHash("Rage End");

	// Token: 0x04003523 RID: 13603
	[SerializeField]
	private GameObject strandObject;

	// Token: 0x04003524 RID: 13604
	[SerializeField]
	private Animator animator;

	// Token: 0x04003525 RID: 13605
	[SerializeField]
	private TrackTriggerObjects rangeTrigger;

	// Token: 0x04003526 RID: 13606
	[SerializeField]
	private float heroRadius;

	// Token: 0x04003527 RID: 13607
	[SerializeField]
	private BlackThreadSpine[] spinePrefabs;

	// Token: 0x04003528 RID: 13608
	[SerializeField]
	private bool useProbability;

	// Token: 0x04003529 RID: 13609
	[SerializeField]
	private bool setSpineScaleRelative;

	// Token: 0x0400352A RID: 13610
	[SerializeField]
	private MinMaxFloat spineSpawnDelay;

	// Token: 0x0400352B RID: 13611
	[SerializeField]
	private MinMaxFloat spineSpawnDelayInRange;

	// Token: 0x0400352C RID: 13612
	[SerializeField]
	private MinMaxFloat spineSpawnDelayNeedolin;

	// Token: 0x0400352D RID: 13613
	[SerializeField]
	private Vector3 spineSpawnMin;

	// Token: 0x0400352E RID: 13614
	[SerializeField]
	private Vector3 spineSpawnMax;

	// Token: 0x0400352F RID: 13615
	[SerializeField]
	private ParticleSystem[] particles;

	// Token: 0x04003530 RID: 13616
	[SerializeField]
	private float emissionMultInRange;

	// Token: 0x04003531 RID: 13617
	[SerializeField]
	private float emissionMultNeedolin;

	// Token: 0x04003532 RID: 13618
	[Space]
	[SerializeField]
	private EventRegister rageStartEvent;

	// Token: 0x04003533 RID: 13619
	[SerializeField]
	private EventRegister rageEndEvent;

	// Token: 0x04003534 RID: 13620
	[SerializeField]
	private Transform rageChild;

	// Token: 0x04003535 RID: 13621
	[Space]
	[SerializeField]
	private RandomAudioClipTable loopAudioTable;

	// Token: 0x04003536 RID: 13622
	[SerializeField]
	private AudioSource loopAudioSource;

	// Token: 0x04003537 RID: 13623
	[SerializeField]
	private int activeLoopsCount;

	// Token: 0x04003538 RID: 13624
	[SerializeField]
	private float audioFadeDuration;

	// Token: 0x04003539 RID: 13625
	private float[] baseEmissionRates;

	// Token: 0x0400353A RID: 13626
	private bool isMenuScene;

	// Token: 0x0400353B RID: 13627
	private VisibilityGroup visibilityGroup;

	// Token: 0x0400353C RID: 13628
	private bool isVisible;

	// Token: 0x0400353D RID: 13629
	private bool isInactive;

	// Token: 0x0400353E RID: 13630
	private Coroutine behaviourRoutine;

	// Token: 0x0400353F RID: 13631
	private bool forceRage;

	// Token: 0x04003540 RID: 13632
	private bool skipSpawnDelay;

	// Token: 0x04003541 RID: 13633
	private Coroutine rageTimer;

	// Token: 0x04003542 RID: 13634
	private BlackThreadStrand.ProbabilitySpine[] probabilitySpines;

	// Token: 0x04003543 RID: 13635
	private float[] probabilities;

	// Token: 0x04003544 RID: 13636
	private bool hasAwaken;

	// Token: 0x04003545 RID: 13637
	private bool hasStarted;

	// Token: 0x04003546 RID: 13638
	private HeroController hc;

	// Token: 0x04003547 RID: 13639
	private static List<BlackThreadStrand> _activeStrands;

	// Token: 0x04003548 RID: 13640
	private static BlackThreadStrand _audioManagerStrand;

	// Token: 0x04003549 RID: 13641
	private List<BlackThreadStrand.AudioLoopInfo> closestStrandsTemp;

	// Token: 0x0400354A RID: 13642
	private Coroutine audioManagerRoutine;

	// Token: 0x0400354B RID: 13643
	private Coroutine audioFadeRoutine;

	// Token: 0x0400354C RID: 13644
	private float audioFadeTarget;

	// Token: 0x0400354D RID: 13645
	private bool isFadingOut;

	// Token: 0x0200186D RID: 6253
	private class ProbabilitySpine : Probability.ProbabilityBase<BlackThreadSpine>
	{
		// Token: 0x06009126 RID: 37158 RVA: 0x00297A21 File Offset: 0x00295C21
		public ProbabilitySpine(BlackThreadSpine spine)
		{
			this.Item = spine;
		}

		// Token: 0x17000FF6 RID: 4086
		// (get) Token: 0x06009127 RID: 37159 RVA: 0x00297A30 File Offset: 0x00295C30
		public override BlackThreadSpine Item { get; }
	}

	// Token: 0x0200186E RID: 6254
	private struct AudioLoopInfo
	{
		// Token: 0x04009201 RID: 37377
		public BlackThreadStrand Strand;

		// Token: 0x04009202 RID: 37378
		public Vector3 LoopPosition;

		// Token: 0x04009203 RID: 37379
		public float Distance;
	}
}
