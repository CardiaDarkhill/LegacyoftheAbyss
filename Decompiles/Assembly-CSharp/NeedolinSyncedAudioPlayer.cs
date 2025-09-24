using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public sealed class NeedolinSyncedAudioPlayer : MonoBehaviour
{
	// Token: 0x06000907 RID: 2311 RVA: 0x00029F8C File Offset: 0x0002818C
	private void Awake()
	{
		HeroPerformanceRegion.StartedPerforming += this.OnStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming += this.OnStoppedPerforming;
		this.hasAudioSource = (this.audioSource != null);
		if (!this.hasAudioSource)
		{
			this.audioSource = base.gameObject.GetComponent<AudioSource>();
			this.hasAudioSource = (this.audioSource != null);
		}
		VolumeBlendController component = base.GetComponent<VolumeBlendController>();
		if (component)
		{
			this.volumeModifier = component.GetModifier("NeedolinSyncedAudioPlayer");
			this.volumeModifier.Volume = component.InitialVolume;
		}
		this.hasTarget = (this.syncTargetAudioSource != null);
		base.enabled = false;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x0002A042 File Offset: 0x00028242
	private void OnDestroy()
	{
		HeroPerformanceRegion.StartedPerforming -= this.OnStartedPerforming;
		HeroPerformanceRegion.StoppedPerforming -= this.OnStoppedPerforming;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0002A066 File Offset: 0x00028266
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x0002A084 File Offset: 0x00028284
	private void LateUpdate()
	{
		if (this.state.HasFlag(NeedolinSyncedAudioPlayer.StateFlags.Fading))
		{
			float volume = this.GetVolume();
			float num = Mathf.MoveTowards(volume, this.targetVolume, this.fadeRate * Time.deltaTime);
			this.SetVolume(num);
			if (Mathf.Approximately(volume, num))
			{
				this.state &= ~NeedolinSyncedAudioPlayer.StateFlags.Fading;
			}
		}
		if (this.state.HasFlag(NeedolinSyncedAudioPlayer.StateFlags.Stopping))
		{
			this.stopTimer -= Time.deltaTime;
			if (this.stopTimer <= 0f && !this.state.HasFlag(NeedolinSyncedAudioPlayer.StateFlags.Fading))
			{
				if (this.hasAudioSource)
				{
					this.audioSource.Stop();
				}
				this.state &= ~NeedolinSyncedAudioPlayer.StateFlags.Stopping;
			}
		}
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x0002A157 File Offset: 0x00028357
	private void UpdateEnabledState()
	{
		base.enabled = (this.hasAudioSource && this.state > NeedolinSyncedAudioPlayer.StateFlags.None);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x0002A174 File Offset: 0x00028374
	public void Play()
	{
		this.state &= ~NeedolinSyncedAudioPlayer.StateFlags.Stopping;
		if (this.copyClipOnStart)
		{
			this.CopyClip();
		}
		if (this.syncPlayTimeOnStart)
		{
			this.SyncTime();
		}
		if (this.playOnStart && this.hasAudioSource && !this.audioSource.isPlaying && (!this.hasTarget || this.syncTargetAudioSource.isPlaying))
		{
			this.audioSource.Play();
		}
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0002A1E9 File Offset: 0x000283E9
	public void Stop()
	{
		if (this.hasAudioSource)
		{
			this.audioSource.Stop();
			this.audioSource.clip = null;
		}
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0002A20C File Offset: 0x0002840C
	private void OnStartedPerforming()
	{
		this.state &= ~NeedolinSyncedAudioPlayer.StateFlags.Stopping;
		if (this.copyClipOnStart)
		{
			this.CopyClip();
		}
		if (this.syncPlayTimeOnStart)
		{
			this.SyncTime();
		}
		if (this.playOnStart && this.hasAudioSource && !this.audioSource.isPlaying && (!this.hasTarget || this.syncTargetAudioSource.isPlaying))
		{
			this.audioSource.Play();
		}
		if (this.fadeOnStart)
		{
			this.targetVolume = Mathf.Clamp01(this.startFadeVolume);
			this.fadeRate = this.startFadeRate;
			this.state |= NeedolinSyncedAudioPlayer.StateFlags.Fading;
		}
		this.UpdateEnabledState();
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x0002A2BC File Offset: 0x000284BC
	private void OnStoppedPerforming()
	{
		if (this.stopOnEnd)
		{
			if (this.stopDelay <= 0f && !this.fadeOnEnd)
			{
				this.state = NeedolinSyncedAudioPlayer.StateFlags.None;
				if (this.hasAudioSource)
				{
					this.audioSource.Stop();
				}
				base.enabled = false;
				return;
			}
			this.state |= NeedolinSyncedAudioPlayer.StateFlags.Stopping;
			this.stopTimer = this.stopDelay;
		}
		if (this.fadeOnEnd)
		{
			this.state |= NeedolinSyncedAudioPlayer.StateFlags.Fading;
			this.targetVolume = Mathf.Clamp01(this.endFadeVolume);
			this.fadeRate = this.endFadeRate;
		}
		this.UpdateEnabledState();
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x0002A35C File Offset: 0x0002855C
	private void CopyClip()
	{
		if (this.hasAudioSource && this.hasTarget && this.audioSource.clip != this.syncTargetAudioSource.clip)
		{
			this.audioSource.clip = this.syncTargetAudioSource.clip;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0002A3AC File Offset: 0x000285AC
	private void SyncTime()
	{
		if (this.hasAudioSource && this.hasTarget && this.audioSource.clip == this.syncTargetAudioSource.clip)
		{
			this.audioSource.timeSamples = this.syncTargetAudioSource.timeSamples;
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0002A3FC File Offset: 0x000285FC
	public void SetTarget(AudioSource target)
	{
		this.syncTargetAudioSource = target;
		this.hasTarget = (target != null);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0002A412 File Offset: 0x00028612
	private float GetVolume()
	{
		if (this.volumeModifier != null)
		{
			return this.volumeModifier.Volume;
		}
		if (this.hasAudioSource)
		{
			return this.audioSource.volume;
		}
		return 1f;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x0002A441 File Offset: 0x00028641
	private void SetVolume(float volume)
	{
		if (this.volumeModifier != null)
		{
			this.volumeModifier.Volume = volume;
			return;
		}
		if (this.hasAudioSource)
		{
			this.audioSource.volume = volume;
		}
	}

	// Token: 0x040008BB RID: 2235
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040008BC RID: 2236
	[SerializeField]
	private AudioSource syncTargetAudioSource;

	// Token: 0x040008BD RID: 2237
	[Header("On Needolin Start")]
	[SerializeField]
	private bool syncPlayTimeOnStart;

	// Token: 0x040008BE RID: 2238
	[SerializeField]
	private bool copyClipOnStart;

	// Token: 0x040008BF RID: 2239
	[SerializeField]
	private bool playOnStart;

	// Token: 0x040008C0 RID: 2240
	[Space]
	[SerializeField]
	private bool fadeOnStart;

	// Token: 0x040008C1 RID: 2241
	[SerializeField]
	private float startFadeVolume;

	// Token: 0x040008C2 RID: 2242
	[SerializeField]
	private float startFadeRate = 1f;

	// Token: 0x040008C3 RID: 2243
	[Header("On Needolin End")]
	[SerializeField]
	private bool stopOnEnd;

	// Token: 0x040008C4 RID: 2244
	[SerializeField]
	private float stopDelay;

	// Token: 0x040008C5 RID: 2245
	[Space]
	[SerializeField]
	private bool fadeOnEnd;

	// Token: 0x040008C6 RID: 2246
	[SerializeField]
	private float endFadeVolume;

	// Token: 0x040008C7 RID: 2247
	[SerializeField]
	private float endFadeRate = 1f;

	// Token: 0x040008C8 RID: 2248
	private VolumeModifier volumeModifier;

	// Token: 0x040008C9 RID: 2249
	private bool hasAudioSource;

	// Token: 0x040008CA RID: 2250
	private bool hasTarget;

	// Token: 0x040008CB RID: 2251
	private float targetVolume;

	// Token: 0x040008CC RID: 2252
	private float fadeRate;

	// Token: 0x040008CD RID: 2253
	private float stopTimer;

	// Token: 0x040008CE RID: 2254
	private NeedolinSyncedAudioPlayer.StateFlags state;

	// Token: 0x02001468 RID: 5224
	[Flags]
	private enum StateFlags
	{
		// Token: 0x04008318 RID: 33560
		None = 0,
		// Token: 0x04008319 RID: 33561
		Fading = 1,
		// Token: 0x0400831A RID: 33562
		Stopping = 2
	}
}
