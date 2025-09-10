using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000139 RID: 313
public sealed class TimeScaledAudio : MonoBehaviour
{
	// Token: 0x060009A0 RID: 2464 RVA: 0x0002BE82 File Offset: 0x0002A082
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				base.enabled = false;
				return;
			}
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0002BEB4 File Offset: 0x0002A0B4
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0002BED0 File Offset: 0x0002A0D0
	private void OnEnable()
	{
		this.RegisterEvents();
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x0002BED8 File Offset: 0x0002A0D8
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x0002BEE0 File Offset: 0x0002A0E0
	private void RegisterEvents()
	{
		if (!this.registeredEvents)
		{
			this.registeredEvents = true;
			TimeManager.OnTimeScaleUpdated += this.OnTimeScaleUpdated;
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x0002BF02 File Offset: 0x0002A102
	private void UnregisterEvents()
	{
		if (this.registeredEvents)
		{
			this.registeredEvents = false;
			TimeManager.OnTimeScaleUpdated -= this.OnTimeScaleUpdated;
		}
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x0002BF24 File Offset: 0x0002A124
	private void OnTimeScaleUpdated(float timeScale)
	{
		this.audioSource.pitch = this.pitchLimits.GetClampedBetween(timeScale);
	}

	// Token: 0x0400093A RID: 2362
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400093B RID: 2363
	[SerializeField]
	private MinMaxFloat pitchLimits = new MinMaxFloat(0f, 1f);

	// Token: 0x0400093C RID: 2364
	private bool registeredEvents;
}
