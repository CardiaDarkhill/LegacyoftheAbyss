using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class CameraShakeRegion : MonoBehaviour, ICameraShake
{
	// Token: 0x06000A6C RID: 2668 RVA: 0x0002EECA File Offset: 0x0002D0CA
	private void Awake()
	{
		this.trigger.InsideStateChanged += this.OnInsideStateChanged;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0002EEE3 File Offset: 0x0002D0E3
	private void OnInsideStateChanged(bool isInside)
	{
		this.isInsideTrigger = isInside;
		if (isInside && this.insideRoutine == null)
		{
			base.StartCoroutine(this.InsideRoutine());
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0002EF04 File Offset: 0x0002D104
	private IEnumerator InsideRoutine()
	{
		this.cameraRef.DoShake(this, this, false, true, true);
		for (;;)
		{
			IL_3D:
			float elapsed = 0f;
			float startMultiplier = this.currentMultiplier;
			while (elapsed < this.fadeInDuration)
			{
				this.currentMultiplier = Mathf.Lerp(startMultiplier, 1f, elapsed / this.fadeInDuration);
				if (!this.isInsideTrigger)
				{
					break;
				}
				yield return null;
				elapsed += Time.deltaTime;
			}
			this.currentMultiplier = 1f;
			while (this.isInsideTrigger)
			{
				yield return null;
			}
			startMultiplier = 0f;
			elapsed = this.currentMultiplier;
			while (startMultiplier < this.fadeOutDuration)
			{
				this.currentMultiplier = Mathf.Lerp(elapsed, 0f, startMultiplier / this.fadeOutDuration);
				if (this.isInsideTrigger)
				{
					goto IL_3D;
				}
				yield return null;
				startMultiplier += Time.deltaTime;
			}
			break;
		}
		this.currentMultiplier = 0f;
		this.cameraRef.CancelShake(this);
		this.insideRoutine = null;
		yield break;
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000A6F RID: 2671 RVA: 0x0002EF13 File Offset: 0x0002D113
	public bool CanFinish
	{
		get
		{
			return this.sourceProfile.CanFinish;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000A70 RID: 2672 RVA: 0x0002EF20 File Offset: 0x0002D120
	public int FreezeFrames
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000A71 RID: 2673 RVA: 0x0002EF23 File Offset: 0x0002D123
	public CameraShakeWorldForceIntensities WorldForceOnStart
	{
		get
		{
			return CameraShakeWorldForceIntensities.None;
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x0002EF26 File Offset: 0x0002D126
	public ICameraShakeVibration CameraShakeVibration
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0002EF29 File Offset: 0x0002D129
	public Vector2 GetOffset(float elapsedTime)
	{
		return this.sourceProfile.GetOffset(elapsedTime) * this.currentMultiplier;
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000A74 RID: 2676 RVA: 0x0002EF42 File Offset: 0x0002D142
	public float Magnitude
	{
		get
		{
			return this.sourceProfile.Magnitude;
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0002EF4F File Offset: 0x0002D14F
	public bool IsDone(float elapsedTime)
	{
		return this.sourceProfile.IsDone(elapsedTime);
	}

	// Token: 0x040009EA RID: 2538
	[SerializeField]
	private TrackTriggerObjects trigger;

	// Token: 0x040009EB RID: 2539
	[Space]
	[SerializeField]
	private CameraManagerReference cameraRef;

	// Token: 0x040009EC RID: 2540
	[SerializeField]
	private CameraShakeProfile sourceProfile;

	// Token: 0x040009ED RID: 2541
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x040009EE RID: 2542
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x040009EF RID: 2543
	private Coroutine insideRoutine;

	// Token: 0x040009F0 RID: 2544
	private bool isInsideTrigger;

	// Token: 0x040009F1 RID: 2545
	private float currentMultiplier;
}
