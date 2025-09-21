using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000124 RID: 292
public class MusicRegion : TrackTriggerObjects
{
	// Token: 0x060008F8 RID: 2296 RVA: 0x00029D2C File Offset: 0x00027F2C
	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.layer != 13)
		{
			base.gameObject.layer = 13;
		}
		if (this.enterMusicCue)
		{
			this.enterMusicCue.Preload(base.gameObject);
		}
		if (this.exitMusicCue)
		{
			this.exitMusicCue.Preload(base.gameObject);
		}
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00029D98 File Offset: 0x00027F98
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (isInside)
		{
			if (this.fadeInRoutine != null)
			{
				base.StopCoroutine(this.fadeInRoutine);
			}
			this.fadeInRoutine = base.StartCoroutine(this.FadeIn());
			return;
		}
		if (this.fadeInRoutine != null)
		{
			base.StopCoroutine(this.fadeInRoutine);
		}
		this.FadeOut();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00029DE9 File Offset: 0x00027FE9
	private IEnumerator FadeIn()
	{
		if (this.delay > 0)
		{
			yield return new WaitForSeconds((float)this.delay);
		}
		GameManager instance = GameManager.instance;
		float transitionTime = this.enterTransitionTime;
		if (this.enterMusicSnapshot != null)
		{
			instance.AudioManager.ApplyMusicSnapshot(this.enterMusicSnapshot, 0f, transitionTime);
		}
		if (this.enterMusicCue)
		{
			instance.AudioManager.ApplyMusicCue(this.enterMusicCue, 0f, 0f, false);
		}
		this.fadeInRoutine = null;
		yield break;
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00029DF8 File Offset: 0x00027FF8
	private void FadeOut()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		if (this.exitMusicSnapshot != null)
		{
			silentInstance.AudioManager.ApplyMusicSnapshot(this.exitMusicSnapshot, 0f, this.exitTransitionTime);
		}
		if (this.exitMusicCue)
		{
			silentInstance.AudioManager.ApplyMusicCue(this.exitMusicCue, 0f, 0f, false);
		}
	}

	// Token: 0x040008AF RID: 2223
	[Space]
	public int delay;

	// Token: 0x040008B0 RID: 2224
	public MusicCue enterMusicCue;

	// Token: 0x040008B1 RID: 2225
	public AudioMixerSnapshot enterMusicSnapshot;

	// Token: 0x040008B2 RID: 2226
	public float enterTransitionTime;

	// Token: 0x040008B3 RID: 2227
	[Space]
	public MusicCue exitMusicCue;

	// Token: 0x040008B4 RID: 2228
	public AudioMixerSnapshot exitMusicSnapshot;

	// Token: 0x040008B5 RID: 2229
	public float exitTransitionTime;

	// Token: 0x040008B6 RID: 2230
	private Coroutine fadeInRoutine;
}
