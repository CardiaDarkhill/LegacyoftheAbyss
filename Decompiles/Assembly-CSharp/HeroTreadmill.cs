using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class HeroTreadmill : MonoBehaviour
{
	// Token: 0x06002DA9 RID: 11689 RVA: 0x000C7A18 File Offset: 0x000C5C18
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(new Vector3(this.speedXRange.Start, 2f, 0f), new Vector3(this.speedXRange.Start, 4f, 0f));
		Gizmos.DrawLine(new Vector3(this.speedXRange.End, 2f, 0f), new Vector3(this.speedXRange.End, 4f, 0f));
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000C7AA7 File Offset: 0x000C5CA7
	private void OnValidate()
	{
		if (this.needleFpsLimit < 0f)
		{
			this.needleFpsLimit = 0f;
		}
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000C7AC1 File Offset: 0x000C5CC1
	private void Awake()
	{
		this.curveAnimators = (this.vectorCurveAnimatorParent ? this.vectorCurveAnimatorParent.GetComponentsInChildren<VectorCurveAnimator>() : new VectorCurveAnimator[0]);
		this.conveyorBelt.CapturedHero += delegate(HeroController hero)
		{
			if (this.capturedHero)
			{
				this.capturedHero.BeforeApplyConveyorSpeed -= this.OnBeforeHeroConveyor;
			}
			if (hero)
			{
				hero.BeforeApplyConveyorSpeed += this.OnBeforeHeroConveyor;
				this.lastCapturedHero = hero;
			}
			else
			{
				this.targetSpeed = 0f;
			}
			this.capturedHero = hero;
		};
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x000C7B00 File Offset: 0x000C5D00
	private void Start()
	{
		this.SetSpeedMultiplier(0f);
		base.StartCoroutine(this.NeedleControlRoutine());
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000C7B1A File Offset: 0x000C5D1A
	private void OnDisable()
	{
		if (this.capturedHero)
		{
			this.capturedHero.BeforeApplyConveyorSpeed -= this.OnBeforeHeroConveyor;
		}
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000C7B40 File Offset: 0x000C5D40
	private void Update()
	{
		if (Math.Abs(this.currentSpeed - this.targetSpeed) < 0.1f)
		{
			this.currentSpeed = this.targetSpeed;
		}
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.targetSpeed, Time.deltaTime * this.speedLerpMultiplier);
		this.SetSpeedMultiplier(this.currentSpeed * this.multiplier);
		bool flag = this.currentSpeed > 0.5f;
		if (flag)
		{
			if (!this.wasMoving)
			{
				this.cogRotationController.ResetNextUpdateTime();
				this.moveParticles.PlayParticles();
				if (this.lastCapturedHero.cState.isSprinting)
				{
					this.startSprintSound.SpawnAndPlayOneShot(this.sprintSource.transform.position, null);
				}
				else
				{
					this.startRunSound.SpawnAndPlayOneShot(this.runSource.transform.position, null);
				}
			}
			if (this.lastCapturedHero.cState.isSprinting)
			{
				if (!this.sprintSource.isPlaying)
				{
					this.sprintSource.Play();
				}
				if (this.runSource.isPlaying)
				{
					this.runSource.Stop();
				}
			}
			else
			{
				if (this.sprintSource.isPlaying)
				{
					this.sprintSource.Stop();
				}
				if (!this.runSource.isPlaying)
				{
					this.runSource.Play();
				}
			}
		}
		else if (this.wasMoving)
		{
			this.moveParticles.StopParticles();
			this.endRunSound.SpawnAndPlayOneShot(this.runSource.transform.position, null);
			this.runSource.Stop();
			this.sprintSource.Stop();
		}
		this.wasMoving = flag;
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x000C7CF0 File Offset: 0x000C5EF0
	private void OnBeforeHeroConveyor(Vector2 heroVelocity)
	{
		if (heroVelocity.x > 0f)
		{
			float x = base.transform.InverseTransformPoint(this.capturedHero.transform.position).x;
			float tbetween = this.speedXRange.GetTBetween(x);
			this.targetSpeed = ((tbetween > 0f) ? this.speedRange.GetLerpUnclampedValue(tbetween) : this.speedRange.Start);
			this.multiplier = heroVelocity.x / this.heroReferenceSpeed;
			return;
		}
		this.targetSpeed = 0f;
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x000C7D80 File Offset: 0x000C5F80
	private void SetSpeedMultiplier(float value)
	{
		if (Math.Abs(value - this.oldSpeedMult) < 0.001f)
		{
			return;
		}
		this.oldSpeedMult = value;
		this.conveyorBelt.SpeedMultiplier = value;
		this.speedControlAnimator.SetFloat(HeroTreadmill._speedAnimatorParam, value);
		this.cogRotationController.RotationMultiplier = value;
		VectorCurveAnimator[] array = this.curveAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpeedMultiplier = value;
		}
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x000C7DF0 File Offset: 0x000C5FF0
	private IEnumerator NeedleControlRoutine()
	{
		this.SetNeedlePosition(0f);
		this.needleJitter.StopJitter();
		float needleT = 0f;
		float needleSpeed = 0f;
		float fallDelayElapsed = 0f;
		float targetT = this.needleRange.GetTBetween(this.needleTarget);
		bool wasAboveTarget = false;
		bool wasNeedleMoving = false;
		for (;;)
		{
			if (this.wasMoving && !this.forceNeedleDrop)
			{
				needleSpeed = Mathf.Lerp(needleSpeed, this.needleRiseSpeed * this.multiplier, Time.deltaTime * this.needleRiseLerpSpeed);
				fallDelayElapsed = 0f;
			}
			else if (needleT <= 0f)
			{
				needleSpeed = 0f;
			}
			else if (fallDelayElapsed >= this.needleFallDelay || this.forceNeedleDrop)
			{
				needleSpeed = Mathf.Lerp(needleSpeed, -this.needleFallSpeed, Time.deltaTime * this.needleFallLerpSpeed);
			}
			else
			{
				needleSpeed = Mathf.Lerp(needleSpeed, 0f, Time.deltaTime * this.needleRiseLerpSpeed);
				fallDelayElapsed += Time.deltaTime;
			}
			bool flag = Math.Abs(needleSpeed) > 0.01f;
			if (flag)
			{
				if (!wasNeedleMoving)
				{
					this.needleJitter.StartJitter();
					this.needleRiseLoop.Play();
				}
			}
			else if (wasNeedleMoving)
			{
				this.needleJitter.StopJitter();
				this.needleRiseLoop.Stop();
			}
			needleT = Mathf.Clamp01(needleT + needleSpeed * Time.deltaTime);
			if (this.needleFpsLimit > 0f)
			{
				if (Time.timeAsDouble > this.needleUpdateTime)
				{
					this.needleUpdateTime = Time.timeAsDouble + (double)(1f / this.needleFpsLimit);
					this.SetNeedlePosition(needleT);
				}
			}
			else
			{
				this.SetNeedlePosition(needleT);
			}
			if (needleT <= 0.01f)
			{
				this.forceNeedleDrop = false;
			}
			bool flag2 = needleT >= targetT;
			if (flag2)
			{
				if (!wasAboveTarget)
				{
					this.needleFsm.SendEvent("NEEDLE ABOVE");
				}
			}
			else if (wasAboveTarget)
			{
				this.needleFsm.SendEvent("NEEDLE BELOW");
			}
			wasAboveTarget = flag2;
			wasNeedleMoving = flag;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000C7E00 File Offset: 0x000C6000
	private void SetNeedlePosition(float value)
	{
		float lerpedValue = this.needleRange.GetLerpedValue(value);
		this.needlePivot.transform.SetLocalRotation2D(lerpedValue);
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000C7E2B File Offset: 0x000C602B
	public void DropNeedle()
	{
		this.forceNeedleDrop = true;
	}

	// Token: 0x04002F84 RID: 12164
	[Header("Treadmill")]
	[SerializeField]
	private ConveyorBelt conveyorBelt;

	// Token: 0x04002F85 RID: 12165
	[SerializeField]
	private Animator speedControlAnimator;

	// Token: 0x04002F86 RID: 12166
	[SerializeField]
	private CogRotationController cogRotationController;

	// Token: 0x04002F87 RID: 12167
	[SerializeField]
	private Transform vectorCurveAnimatorParent;

	// Token: 0x04002F88 RID: 12168
	[SerializeField]
	private ParticleSystemPool moveParticles;

	// Token: 0x04002F89 RID: 12169
	[SerializeField]
	private AudioSource runSource;

	// Token: 0x04002F8A RID: 12170
	[SerializeField]
	private AudioEvent startRunSound;

	// Token: 0x04002F8B RID: 12171
	[SerializeField]
	private AudioSource sprintSource;

	// Token: 0x04002F8C RID: 12172
	[SerializeField]
	private AudioEvent startSprintSound;

	// Token: 0x04002F8D RID: 12173
	[SerializeField]
	private AudioEvent endRunSound;

	// Token: 0x04002F8E RID: 12174
	[Space]
	[SerializeField]
	private MinMaxFloat speedRange;

	// Token: 0x04002F8F RID: 12175
	[SerializeField]
	private MinMaxFloat speedXRange;

	// Token: 0x04002F90 RID: 12176
	[SerializeField]
	private float speedLerpMultiplier;

	// Token: 0x04002F91 RID: 12177
	[Space]
	[SerializeField]
	private float heroReferenceSpeed;

	// Token: 0x04002F92 RID: 12178
	[Header("Gauge")]
	[SerializeField]
	private Transform needlePivot;

	// Token: 0x04002F93 RID: 12179
	[SerializeField]
	private PlayMakerFSM needleFsm;

	// Token: 0x04002F94 RID: 12180
	[SerializeField]
	private AudioSource needleRiseLoop;

	// Token: 0x04002F95 RID: 12181
	[Space]
	[SerializeField]
	private MinMaxFloat needleRange;

	// Token: 0x04002F96 RID: 12182
	[SerializeField]
	private float needleTarget;

	// Token: 0x04002F97 RID: 12183
	[SerializeField]
	private float needleRiseSpeed;

	// Token: 0x04002F98 RID: 12184
	[SerializeField]
	private float needleRiseLerpSpeed;

	// Token: 0x04002F99 RID: 12185
	[SerializeField]
	private float needleFallDelay;

	// Token: 0x04002F9A RID: 12186
	[SerializeField]
	private float needleFallSpeed;

	// Token: 0x04002F9B RID: 12187
	[SerializeField]
	private float needleFallLerpSpeed;

	// Token: 0x04002F9C RID: 12188
	[SerializeField]
	private JitterSelf needleJitter;

	// Token: 0x04002F9D RID: 12189
	[SerializeField]
	private float needleFpsLimit;

	// Token: 0x04002F9E RID: 12190
	private float oldSpeedMult = -1f;

	// Token: 0x04002F9F RID: 12191
	private HeroController capturedHero;

	// Token: 0x04002FA0 RID: 12192
	private HeroController lastCapturedHero;

	// Token: 0x04002FA1 RID: 12193
	private float targetSpeed;

	// Token: 0x04002FA2 RID: 12194
	private float multiplier;

	// Token: 0x04002FA3 RID: 12195
	private float currentSpeed;

	// Token: 0x04002FA4 RID: 12196
	private bool wasMoving;

	// Token: 0x04002FA5 RID: 12197
	private bool forceNeedleDrop;

	// Token: 0x04002FA6 RID: 12198
	private double needleUpdateTime;

	// Token: 0x04002FA7 RID: 12199
	private VectorCurveAnimator[] curveAnimators;

	// Token: 0x04002FA8 RID: 12200
	private static readonly int _speedAnimatorParam = Animator.StringToHash("Speed");
}
