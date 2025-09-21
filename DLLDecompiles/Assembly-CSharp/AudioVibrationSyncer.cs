using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200079F RID: 1951
public sealed class AudioVibrationSyncer : ManagerSingleton<AudioVibrationSyncer>
{
	// Token: 0x060044D0 RID: 17616 RVA: 0x0012D195 File Offset: 0x0012B395
	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this)
		{
			AudioVibrationSyncer.isInitialised = true;
		}
	}

	// Token: 0x060044D1 RID: 17617 RVA: 0x0012D1B0 File Offset: 0x0012B3B0
	private void Start()
	{
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this && AudioVibrationSyncer.SYNCED_EMISSIONS.Count == 0)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060044D2 RID: 17618 RVA: 0x0012D1D2 File Offset: 0x0012B3D2
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (ManagerSingleton<AudioVibrationSyncer>.UnsafeInstance == this)
		{
			AudioVibrationSyncer.isInitialised = false;
		}
	}

	// Token: 0x060044D3 RID: 17619 RVA: 0x0012D1F0 File Offset: 0x0012B3F0
	private void Update()
	{
		for (int i = AudioVibrationSyncer.SYNCED_EMISSIONS.Count - 1; i >= 0; i--)
		{
			if (!AudioVibrationSyncer.SYNCED_EMISSIONS[i].Update())
			{
				AudioVibrationSyncer.SYNCED_EMISSIONS.RemoveAt(i);
			}
		}
		if (AudioVibrationSyncer.SYNCED_EMISSIONS.Count == 0)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060044D4 RID: 17620 RVA: 0x0012D244 File Offset: 0x0012B444
	public static VibrationEmission StartSyncedEmission(VibrationDataAsset vibrationDataAsset, AudioSource audioSource, bool isLooping, bool isRealTime)
	{
		return AudioVibrationSyncer.StartSyncedEmission(vibrationDataAsset, audioSource, isLooping, isRealTime, 0.1f);
	}

	// Token: 0x060044D5 RID: 17621 RVA: 0x0012D254 File Offset: 0x0012B454
	private static VibrationEmission StartSyncedEmission(VibrationDataAsset vibrationDataAsset, AudioSource audioSource, bool isLooping, bool isRealTime, float syncThreshold)
	{
		VibrationEmission vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset, null, isLooping, "", isRealTime);
		AudioVibrationSyncer.StartSyncedEmission(vibrationEmission, audioSource, syncThreshold);
		return vibrationEmission;
	}

	// Token: 0x060044D6 RID: 17622 RVA: 0x0012D289 File Offset: 0x0012B489
	public static void StartSyncedEmission(VibrationEmission emission, AudioSource audioSource)
	{
		AudioVibrationSyncer.StartSyncedEmission(emission, audioSource, 0.1f);
	}

	// Token: 0x060044D7 RID: 17623 RVA: 0x0012D297 File Offset: 0x0012B497
	private static void StartSyncedEmission(VibrationEmission emission, AudioSource audioSource, float syncThreshold)
	{
		if (emission == null)
		{
			return;
		}
		if (!emission.IsPlaying)
		{
			return;
		}
		if (audioSource == null)
		{
			return;
		}
		AudioVibrationSyncer.SYNCED_EMISSIONS.Add(new AudioVibrationSyncer.SyncedEmission(emission, audioSource, syncThreshold));
		if (AudioVibrationSyncer.isInitialised)
		{
			ManagerSingleton<AudioVibrationSyncer>.Instance.enabled = true;
		}
	}

	// Token: 0x040045BE RID: 17854
	private static bool isInitialised;

	// Token: 0x040045BF RID: 17855
	private static readonly List<AudioVibrationSyncer.SyncedEmission> SYNCED_EMISSIONS = new List<AudioVibrationSyncer.SyncedEmission>();

	// Token: 0x040045C0 RID: 17856
	private const float syncThreshold = 0.1f;

	// Token: 0x02001A76 RID: 6774
	public sealed class SyncedEmission
	{
		// Token: 0x06009700 RID: 38656 RVA: 0x002A9444 File Offset: 0x002A7644
		public SyncedEmission(VibrationEmission emission, AudioSource audioSource, float syncThreshold)
		{
			this.emission = emission;
			this.audioSource = audioSource;
			this.syncThreshold = syncThreshold;
			float time = audioSource.time;
			emission.SetPlaybackTime(time);
			this.previousTime = time;
		}

		// Token: 0x06009701 RID: 38657 RVA: 0x002A9484 File Offset: 0x002A7684
		public bool Update()
		{
			if (!this.emission.IsPlaying)
			{
				return false;
			}
			float time = this.audioSource.time;
			if (time < this.previousTime)
			{
				this.emission.SetPlaybackTime(time);
			}
			else if (Mathf.Abs(time - this.emission.Time) >= this.syncThreshold)
			{
				this.emission.SetPlaybackTime(time);
			}
			this.previousTime = time;
			return true;
		}

		// Token: 0x0400998F RID: 39311
		public readonly VibrationEmission emission;

		// Token: 0x04009990 RID: 39312
		public readonly AudioSource audioSource;

		// Token: 0x04009991 RID: 39313
		private readonly float syncThreshold;

		// Token: 0x04009992 RID: 39314
		private float previousTime;
	}
}
