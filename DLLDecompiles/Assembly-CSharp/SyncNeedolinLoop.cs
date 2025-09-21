using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class SyncNeedolinLoop : MonoBehaviour
{
	// Token: 0x06000999 RID: 2457 RVA: 0x0002BDC1 File Offset: 0x00029FC1
	private void Reset()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x0002BDCF File Offset: 0x00029FCF
	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += this.OnNeedolinStarted;
		HeroPerformanceRegion.StoppedPerforming += this.OnNeedolinStopped;
		if (HeroPerformanceRegion.IsPerforming)
		{
			this.OnNeedolinStarted();
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x0002BE00 File Offset: 0x0002A000
	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= this.OnNeedolinStarted;
		HeroPerformanceRegion.StoppedPerforming -= this.OnNeedolinStopped;
		if (this.playRoutine != null)
		{
			base.StopCoroutine(this.playRoutine);
			this.playRoutine = null;
		}
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x0002BE3F File Offset: 0x0002A03F
	private void OnNeedolinStarted()
	{
		this.isPlaying = true;
		if (this.playRoutine == null)
		{
			this.playRoutine = base.StartCoroutine(this.PlayAudioInSync());
		}
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x0002BE62 File Offset: 0x0002A062
	private void OnNeedolinStopped()
	{
		this.isPlaying = false;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x0002BE6B File Offset: 0x0002A06B
	private IEnumerator PlayAudioInSync()
	{
		HeroController instance = HeroController.instance;
		AudioSource needolinSource = instance.transform.Find("Sounds").Find("Needolin").GetComponent<AudioSource>();
		while (!needolinSource.isPlaying && this.isPlaying)
		{
			yield return null;
		}
		this.source.Play();
		this.source.timeSamples = needolinSource.timeSamples;
		while (this.isPlaying)
		{
			yield return null;
		}
		this.source.Stop();
		this.playRoutine = null;
		yield break;
	}

	// Token: 0x04000937 RID: 2359
	[SerializeField]
	private AudioSource source;

	// Token: 0x04000938 RID: 2360
	private Coroutine playRoutine;

	// Token: 0x04000939 RID: 2361
	private bool isPlaying;
}
