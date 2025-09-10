using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000130 RID: 304
public static class RandomAudioClipTableExtensions
{
	// Token: 0x06000970 RID: 2416 RVA: 0x0002B801 File Offset: 0x00029A01
	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, bool forcePlay = false)
	{
		if (table == null)
		{
			return;
		}
		table.PlayOneShotUnsafe(audioSource, 0f, forcePlay);
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0002B81A File Offset: 0x00029A1A
	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, float pitchOffset, bool forcePlay = false)
	{
		if (table == null)
		{
			return;
		}
		table.PlayOneShotUnsafe(audioSource, pitchOffset, forcePlay);
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0002B82F File Offset: 0x00029A2F
	public static void PlayOneShot(this RandomAudioClipTable table, AudioSource audioSource, float pitchOffset, float volumeScale, bool forcePlay = false)
	{
		if (table == null)
		{
			return;
		}
		table.PlayOneShotUnsafe(audioSource, pitchOffset, volumeScale, forcePlay);
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0002B846 File Offset: 0x00029A46
	public static AudioSource SpawnAndPlayOneShot(this RandomAudioClipTable table, Vector3 position, bool forcePlay = false)
	{
		return table.SpawnAndPlayOneShot(Audio.DefaultAudioSourcePrefab, position, forcePlay, 1f, null);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0002B85C File Offset: 0x00029A5C
	public static AudioSource SpawnAndPlayOneShot(this RandomAudioClipTable table, AudioSource prefab, Vector3 position, bool forcePlay = false, float volume = 1f, Action onRecycled = null)
	{
		if (table == null)
		{
			return null;
		}
		if (prefab == null)
		{
			prefab = Audio.DefaultAudioSourcePrefab;
		}
		if (prefab == null)
		{
			return null;
		}
		AudioClip audioClip = table.SelectClip(forcePlay);
		if (audioClip == null)
		{
			return null;
		}
		if (!AudioEventManager.TryPlayAudioClip(audioClip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn<AudioSource>();
		audioSource.transform.position = position;
		audioSource.pitch = table.SelectPitch();
		audioSource.volume = table.SelectVolume() * volume;
		audioSource.PlayOneShot(audioClip);
		table.ReportPlayed(audioClip, audioSource);
		if (audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0002B905 File Offset: 0x00029B05
	public static void SpawnAndPlayOneShot2D(this RandomAudioClipTable table, Vector3 position, bool forcePlay = false)
	{
		table.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, position, forcePlay, 1f, null);
	}
}
