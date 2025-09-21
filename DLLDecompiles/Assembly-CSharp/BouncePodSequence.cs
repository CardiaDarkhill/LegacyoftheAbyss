using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200049D RID: 1181
public class BouncePodSequence : MonoBehaviour
{
	// Token: 0x06002AC3 RID: 10947 RVA: 0x000BA064 File Offset: 0x000B8264
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.audioEventPosition, 0.2f);
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000BA086 File Offset: 0x000B8286
	private void OnValidate()
	{
		if (this.sequences.Count == 0)
		{
			this.sequences.Add(new BouncePodSequence.Sequence
			{
				Pods = new List<BouncePod>()
			});
		}
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000BA0B0 File Offset: 0x000B82B0
	private void Awake()
	{
		this.OnValidate();
		if (this.startSequencePlate)
		{
			this.startSequencePlate.Activated += this.OnPlatePressed;
		}
		foreach (BouncePodSequence.Sequence sequence in this.sequences)
		{
			for (int i = sequence.Pods.Count - 1; i >= 0; i--)
			{
				if (!sequence.Pods[i])
				{
					sequence.Pods.RemoveAt(i);
				}
			}
		}
		this.pods = base.GetComponentsInChildren<BouncePod>(true);
		foreach (BouncePod bouncePod in this.pods)
		{
			bouncePod.BounceHit += this.OnAnonPodHit;
			bouncePod.InertHit += this.OnAnonPodHit;
		}
		for (int k = 0; k < this.sequences.Count; k++)
		{
			List<BouncePod> list = this.sequences[k].Pods;
			for (int l = 0; l < list.Count; l++)
			{
				BouncePod bouncePod2 = list[l];
				int sequenceIndex = k;
				int capturedIndex = l;
				Action value2 = delegate()
				{
					this.OnPodHit(sequenceIndex, capturedIndex);
				};
				bouncePod2.BounceHit += value2;
				bouncePod2.InertHit += value2;
			}
		}
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.currentSequenceIndex;
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				this.currentSequenceIndex = value;
				this.UpdateProgressIndicator(true);
				this.isComplete = (this.currentSequenceIndex >= this.sequences.Count);
				if (this.isComplete)
				{
					if (this.startSequencePlate)
					{
						this.startSequencePlate.ActivateSilent();
					}
					this.Unlock(true);
				}
			};
		}
		this.podHolders = base.GetComponentsInChildren<AnimatorActivatingStates>();
		if (this.rewardPillar)
		{
			this.usingRewardPillar = this.rewardPillar.gameObject.activeInHierarchy;
			this.rewardPillar.gameObject.SetActive(false);
		}
		this.UpdateProgressIndicator(true);
		this.ResetSequence(true, false);
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000BA2C4 File Offset: 0x000B84C4
	private void Update()
	{
		if (this.anonPodWasHit)
		{
			this.ResetSequence(false, false);
		}
		if (this.queuedSuccess)
		{
			this.PodHitSuccess();
		}
		else if (this.queuedFail)
		{
			this.PodHitFail();
		}
		this.queuedSuccess = false;
		this.queuedFail = false;
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000BA302 File Offset: 0x000B8502
	private void OnPlatePressed()
	{
		this.SetCogsRotating(true);
		this.platePressedAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(this.audioEventPosition), null);
		this.StartSequence();
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000BA32F File Offset: 0x000B852F
	private void StartSequence()
	{
		if (this.playSequenceRoutine != null)
		{
			base.StopCoroutine(this.playSequenceRoutine);
		}
		this.playSequenceRoutine = base.StartCoroutine(this.PlaySequence());
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000BA357 File Offset: 0x000B8557
	private IEnumerator PlaySequence()
	{
		if (this.sequenceStartEffects)
		{
			this.sequenceStartEffects.PlayParticleSystems();
		}
		this.sequenceStartShake.DoShake(this, true);
		AnimatorActivatingStates[] array = this.podHolders;
		int j;
		for (j = 0; j < array.Length; j++)
		{
			array[j].SetActive(true, false);
		}
		if (this.sequencePlayDelay > 0f)
		{
			yield return new WaitForSeconds(this.sequencePlayDelay);
		}
		WaitForSeconds wait = new WaitForSeconds(this.sequencePodRingWait);
		for (int i = 0; i < this.sequences[this.currentSequenceIndex].Pods.Count; i = j + 1)
		{
			BouncePod pod = this.sequences[this.currentSequenceIndex].Pods[i];
			if (pod)
			{
				if (i > 0)
				{
					yield return wait;
				}
				pod.Ring(true);
				pod = null;
			}
			j = i;
		}
		this.SetCogsRotating(false);
		this.targetPodIndex = 0;
		this.playSequenceRoutine = null;
		yield break;
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x000BA366 File Offset: 0x000B8566
	private void OnAnonPodHit()
	{
		if (this.isComplete || this.targetPodIndex < 0)
		{
			return;
		}
		this.anonPodWasHit = true;
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x000BA384 File Offset: 0x000B8584
	private void OnPodHit(int sequenceIndex, int hitPodIndex)
	{
		if (this.isComplete || this.targetPodIndex < 0 || sequenceIndex != this.currentSequenceIndex)
		{
			return;
		}
		this.anonPodWasHit = false;
		if (hitPodIndex == this.targetPodIndex)
		{
			this.queuedSuccess = true;
			this.queuedFail = false;
			return;
		}
		if (!this.queuedSuccess)
		{
			this.queuedFail = true;
		}
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x000BA3DC File Offset: 0x000B85DC
	private void PodHitSuccess()
	{
		this.targetPodIndex++;
		this.HitProgressCogTurn();
		if (this.targetPodIndex >= this.sequences[this.currentSequenceIndex].Pods.Count)
		{
			this.currentSequenceIndex++;
			this.UpdateProgressIndicator(false);
			if (this.currentSequenceIndex >= this.sequences.Count)
			{
				this.Unlock(false);
				return;
			}
			this.ResetSequence(false, true);
		}
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000BA458 File Offset: 0x000B8658
	private void PodHitFail()
	{
		this.ResetSequence(false, false);
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x000BA464 File Offset: 0x000B8664
	private void ResetSequence(bool isInstant = false, bool autoStartNextSequence = false)
	{
		this.anonPodWasHit = false;
		this.targetPodIndex = -1;
		if (this.playSequenceRoutine != null)
		{
			base.StopCoroutine(this.playSequenceRoutine);
		}
		if (isInstant)
		{
			if (this.startSequencePlate)
			{
				this.startSequencePlate.Deactivate();
			}
			AnimatorActivatingStates[] array = this.podHolders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false, false);
			}
			return;
		}
		if (!autoStartNextSequence)
		{
			this.PlayCogAnim(BouncePodSequence._turnFailAnim);
		}
		base.StartCoroutine(this.ResetInteractivePartsDelayed(autoStartNextSequence));
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x000BA4EA File Offset: 0x000B86EA
	private IEnumerator ResetInteractivePartsDelayed(bool autoStartNextSequence)
	{
		while (this.indicatorAnimationRoutine != null)
		{
			yield return null;
		}
		if (autoStartNextSequence)
		{
			this.StartSequence();
		}
		else
		{
			AnimatorActivatingStates[] array = this.podHolders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false, false);
			}
			if (this.cogsAnimator)
			{
				yield return null;
				yield return new WaitForSeconds(this.cogsAnimator.GetCurrentAnimatorStateInfo(0).length + this.sequencePlayDelay);
			}
			if (this.startSequencePlate)
			{
				this.startSequencePlate.Deactivate();
			}
		}
		yield break;
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x000BA500 File Offset: 0x000B8700
	private void Unlock(bool isInstant)
	{
		this.isComplete = true;
		if (isInstant)
		{
			if (this.usingRewardPillar)
			{
				this.rewardPillar.gameObject.SetActive(true);
				this.rewardPillar.Play(BouncePodSequence._appearAnim, 0, 1f);
			}
			foreach (UnlockablePropBase unlockablePropBase in this.unlockables)
			{
				if (unlockablePropBase)
				{
					unlockablePropBase.Opened();
				}
			}
			return;
		}
		base.StartCoroutine(this.CompletionSequence());
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000BA57C File Offset: 0x000B877C
	private void UpdateProgressIndicator(bool isInstant)
	{
		if (!this.progressIndicator)
		{
			return;
		}
		float t2 = (float)this.currentSequenceIndex / (float)this.sequences.Count;
		this.targetIndicatorRotation = Quaternion.Euler(0f, 0f, this.progressIndicatorRotationExtents.GetLerpedValue(t2));
		if (this.indicatorAnimationRoutine != null)
		{
			base.StopCoroutine(this.indicatorAnimationRoutine);
			this.indicatorAnimationRoutine = null;
		}
		if (isInstant || this.progressIndicatorRotateDuration <= 0f)
		{
			this.progressIndicator.localRotation = this.targetIndicatorRotation;
			return;
		}
		this.currentIndicatorRotation = this.progressIndicator.localRotation;
		this.SetCogsRotating(true);
		this.indicatorAnimationRoutine = this.StartTimerRoutine(this.progressIndicatorRotateDelay, this.progressIndicatorRotateDuration, delegate(float t)
		{
			t = this.progressIndicatorRotateCurve.Evaluate(t);
			this.progressIndicator.localRotation = Quaternion.LerpUnclamped(this.currentIndicatorRotation, this.targetIndicatorRotation, t);
		}, delegate
		{
			this.progressIndicatorMoveSound.SpawnAndPlayOneShot(base.transform.TransformPoint(this.audioEventPosition), null);
		}, delegate
		{
			this.indicatorAnimationRoutine = null;
		}, false);
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x000BA660 File Offset: 0x000B8860
	private void HitProgressCogTurn()
	{
		if (!this.cogsRotation)
		{
			return;
		}
		this.currentCogHitRotation = this.targetCogHitRotation;
		this.targetCogHitRotation = this.currentCogHitRotation + this.hitCogRotateAmount;
		if (this.cogHitRotationRoutine != null)
		{
			base.StopCoroutine(this.cogHitRotationRoutine);
			this.cogHitRotationRoutine = null;
		}
		if (this.hitCogRotateDuration <= 0f)
		{
			this.cogsRotation.ApplyRotation(this.targetCogHitRotation);
			return;
		}
		this.cogHitRotationRoutine = this.StartTimerRoutine(0f, this.hitCogRotateDuration, delegate(float t)
		{
			t = this.hitCogRotateCurve.Evaluate(t);
			this.cogsRotation.ApplyRotation(Mathf.LerpUnclamped(this.currentCogHitRotation, this.targetCogHitRotation, t));
		}, null, null, false);
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000BA6FC File Offset: 0x000B88FC
	private void PlayCogAnim(int animHash)
	{
		if (!this.cogsAnimator)
		{
			return;
		}
		this.cogsAnimator.Play(animHash);
		if (this.cogsRotation)
		{
			this.cogsRotation.CaptureAnimateRotation();
		}
		this.cogsAnimator.Update(0f);
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000BA74B File Offset: 0x000B894B
	private IEnumerator CompletionSequence()
	{
		while (this.indicatorAnimationRoutine != null)
		{
			yield return null;
		}
		if (this.sequenceStartEffects)
		{
			this.sequenceStartEffects.PlayParticleSystems();
		}
		this.sequenceStartShake.DoShake(this, true);
		this.SetCogsRotating(false);
		yield return new WaitForSeconds(this.unlockBellRingDelay);
		this.unlockBellRingSound.SpawnAndPlayOneShot(base.transform.TransformPoint(this.audioEventPosition), null);
		BouncePod[] array = this.pods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Ring(false);
		}
		yield return new WaitForSeconds(this.sequencePodRingWait);
		AnimatorActivatingStates[] array2 = this.podHolders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetActive(false, false);
		}
		yield return new WaitForSeconds(this.unlockDelay);
		this.SetCogsRotating(true);
		if (this.completionLoopEffects)
		{
			this.completionLoopEffects.PlayParticleSystems();
		}
		this.completionRumble.DoShake(this, true);
		yield return new WaitForSeconds(this.unlockDuration);
		this.SetCogsRotating(false);
		if (this.completionLoopEffects)
		{
			this.completionLoopEffects.StopParticleSystems();
		}
		this.completionRumble.CancelShake();
		this.completionRumbleStopShake.DoShake(this, true);
		if (this.rewardAppearDelay > 0f)
		{
			yield return new WaitForSeconds(this.rewardAppearDelay);
		}
		if (this.usingRewardPillar)
		{
			this.rewardAppearSound.SpawnAndPlayOneShot(this.rewardPillar.transform.position, null);
			this.rewardPillar.gameObject.SetActive(true);
			this.rewardPillar.Play(BouncePodSequence._appearAnim, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.rewardPillar.GetCurrentAnimatorStateInfo(0).length);
		}
		foreach (UnlockablePropBase unlockablePropBase in this.unlockables)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		yield break;
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000BA75C File Offset: 0x000B895C
	private void SetCogsRotating(bool value)
	{
		if (value)
		{
			this.PlayCogAnim(BouncePodSequence._turnStartAnim);
			this.runningSequenceStartAudio = this.sequenceStartAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(this.audioEventPosition), delegate()
			{
				this.runningSequenceStartAudio = null;
			});
			return;
		}
		this.PlayCogAnim(BouncePodSequence._turnStopAnim);
		if (this.runningSequenceStartAudio)
		{
			this.runningSequenceStartAudio.Stop();
			this.sequenceStartStopAudio.SpawnAndPlayOneShot(base.transform.TransformPoint(this.audioEventPosition), null);
		}
	}

	// Token: 0x04002B99 RID: 11161
	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	// Token: 0x04002B9A RID: 11162
	private static readonly int _turnStartAnim = Animator.StringToHash("Turn Start");

	// Token: 0x04002B9B RID: 11163
	private static readonly int _turnStopAnim = Animator.StringToHash("Turn Stop");

	// Token: 0x04002B9C RID: 11164
	private static readonly int _turnFailAnim = Animator.StringToHash("Turn Fail");

	// Token: 0x04002B9D RID: 11165
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x04002B9E RID: 11166
	[SerializeField]
	private TempPressurePlate startSequencePlate;

	// Token: 0x04002B9F RID: 11167
	[SerializeField]
	private float sequencePlayDelay;

	// Token: 0x04002BA0 RID: 11168
	[SerializeField]
	private float sequencePodRingWait;

	// Token: 0x04002BA1 RID: 11169
	[Space]
	[SerializeField]
	private Vector3 audioEventPosition;

	// Token: 0x04002BA2 RID: 11170
	[Space]
	[SerializeField]
	private Transform progressIndicator;

	// Token: 0x04002BA3 RID: 11171
	[SerializeField]
	private MinMaxFloat progressIndicatorRotationExtents;

	// Token: 0x04002BA4 RID: 11172
	[SerializeField]
	private AnimationCurve progressIndicatorRotateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002BA5 RID: 11173
	[SerializeField]
	private float progressIndicatorRotateDuration;

	// Token: 0x04002BA6 RID: 11174
	[SerializeField]
	private float progressIndicatorRotateDelay;

	// Token: 0x04002BA7 RID: 11175
	[SerializeField]
	private AudioEvent progressIndicatorMoveSound;

	// Token: 0x04002BA8 RID: 11176
	[Space]
	[SerializeField]
	private Animator cogsAnimator;

	// Token: 0x04002BA9 RID: 11177
	[SerializeField]
	private CogRotationController cogsRotation;

	// Token: 0x04002BAA RID: 11178
	[SerializeField]
	private float hitCogRotateAmount;

	// Token: 0x04002BAB RID: 11179
	[SerializeField]
	private AnimationCurve hitCogRotateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002BAC RID: 11180
	[SerializeField]
	private float hitCogRotateDuration;

	// Token: 0x04002BAD RID: 11181
	[SerializeField]
	private PlayParticleEffects sequenceStartEffects;

	// Token: 0x04002BAE RID: 11182
	[SerializeField]
	private CameraShakeTarget sequenceStartShake;

	// Token: 0x04002BAF RID: 11183
	[SerializeField]
	private AudioEvent platePressedAudio;

	// Token: 0x04002BB0 RID: 11184
	[SerializeField]
	private AudioEvent sequenceStartAudio;

	// Token: 0x04002BB1 RID: 11185
	[SerializeField]
	private AudioEvent sequenceStartStopAudio;

	// Token: 0x04002BB2 RID: 11186
	[Space]
	[SerializeField]
	private PlayParticleEffects completionLoopEffects;

	// Token: 0x04002BB3 RID: 11187
	[SerializeField]
	private CameraShakeTarget completionRumble;

	// Token: 0x04002BB4 RID: 11188
	[SerializeField]
	private CameraShakeTarget completionRumbleStopShake;

	// Token: 0x04002BB5 RID: 11189
	[SerializeField]
	private float unlockBellRingDelay;

	// Token: 0x04002BB6 RID: 11190
	[SerializeField]
	private AudioEvent unlockBellRingSound;

	// Token: 0x04002BB7 RID: 11191
	[SerializeField]
	private float unlockDelay;

	// Token: 0x04002BB8 RID: 11192
	[SerializeField]
	private float unlockDuration;

	// Token: 0x04002BB9 RID: 11193
	[SerializeField]
	private float rewardAppearDelay;

	// Token: 0x04002BBA RID: 11194
	[Space]
	[SerializeField]
	private List<BouncePodSequence.Sequence> sequences;

	// Token: 0x04002BBB RID: 11195
	[SerializeField]
	private Animator rewardPillar;

	// Token: 0x04002BBC RID: 11196
	[SerializeField]
	private AudioEvent rewardAppearSound;

	// Token: 0x04002BBD RID: 11197
	[SerializeField]
	private UnlockablePropBase[] unlockables;

	// Token: 0x04002BBE RID: 11198
	private AnimatorActivatingStates[] podHolders;

	// Token: 0x04002BBF RID: 11199
	private BouncePod[] pods;

	// Token: 0x04002BC0 RID: 11200
	private int currentSequenceIndex;

	// Token: 0x04002BC1 RID: 11201
	private bool isComplete;

	// Token: 0x04002BC2 RID: 11202
	private bool usingRewardPillar;

	// Token: 0x04002BC3 RID: 11203
	private Coroutine playSequenceRoutine;

	// Token: 0x04002BC4 RID: 11204
	private int targetPodIndex;

	// Token: 0x04002BC5 RID: 11205
	private bool queuedFail;

	// Token: 0x04002BC6 RID: 11206
	private bool queuedSuccess;

	// Token: 0x04002BC7 RID: 11207
	private bool anonPodWasHit;

	// Token: 0x04002BC8 RID: 11208
	private Coroutine indicatorAnimationRoutine;

	// Token: 0x04002BC9 RID: 11209
	private Quaternion currentIndicatorRotation;

	// Token: 0x04002BCA RID: 11210
	private Quaternion targetIndicatorRotation;

	// Token: 0x04002BCB RID: 11211
	private Coroutine cogHitRotationRoutine;

	// Token: 0x04002BCC RID: 11212
	private float currentCogHitRotation;

	// Token: 0x04002BCD RID: 11213
	private float targetCogHitRotation;

	// Token: 0x04002BCE RID: 11214
	private AudioSource runningSequenceStartAudio;

	// Token: 0x020017AF RID: 6063
	[Serializable]
	private class Sequence
	{
		// Token: 0x04008EFF RID: 36607
		public List<BouncePod> Pods;
	}
}
