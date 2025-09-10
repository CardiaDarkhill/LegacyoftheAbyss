using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020000FD RID: 253
[Serializable]
public struct AudioEventRandom
{
	// Token: 0x060007EE RID: 2030 RVA: 0x00025DDF File Offset: 0x00023FDF
	public void Reset()
	{
		this.PitchMin = 0.75f;
		this.PitchMax = 1.25f;
		this.Volume = 1f;
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x00025E02 File Offset: 0x00024002
	public float SelectPitch()
	{
		if (Mathf.Approximately(this.PitchMin, this.PitchMax))
		{
			return this.PitchMax;
		}
		return Random.Range(this.PitchMin, this.PitchMax);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x00025E2F File Offset: 0x0002402F
	public bool HasClips()
	{
		return this.Clips != null && this.Clips.Length != 0;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00025E45 File Offset: 0x00024045
	public AudioClip GetClip()
	{
		if (!this.HasClips())
		{
			return null;
		}
		return this.Clips[Random.Range(0, this.Clips.Length)];
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00025E66 File Offset: 0x00024066
	public AudioSource SpawnAndPlayOneShot(Vector3 position, Action onRecycled = null)
	{
		return this.SpawnAndPlayOneShot(null, position, onRecycled);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00025E71 File Offset: 0x00024071
	public AudioSource SpawnAndPlayOneShot(Vector3 position, bool vibrate, Action onRecycled = null)
	{
		return this.SpawnAndPlayOneShot(null, position, vibrate, onRecycled);
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00025E7D File Offset: 0x0002407D
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, float volume)
	{
		return this.SpawnAndPlayOneShotInternal(prefab, position, volume, true);
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00025E8C File Offset: 0x0002408C
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action onRecycled = null)
	{
		AudioSource audioSource = this.SpawnAndPlayOneShotInternal(prefab, position, 1f, true);
		if (audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x00025EC0 File Offset: 0x000240C0
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, bool vibrate, Action onRecycled = null)
	{
		AudioSource audioSource = this.SpawnAndPlayOneShotInternal(prefab, position, 1f, vibrate);
		if (audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		}
		return audioSource;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x00025EF8 File Offset: 0x000240F8
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action<AudioSource> onRecycled)
	{
		AudioSource audioSource = this.SpawnAndPlayOneShotInternal(prefab, position, 1f, true);
		if (audioSource && onRecycled != null)
		{
			RecycleResetHandler.Add(audioSource.gameObject, delegate()
			{
				onRecycled(audioSource);
			});
		}
		return audioSource;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00025F60 File Offset: 0x00024160
	private AudioSource SpawnAndPlayOneShotInternal(AudioSource prefab, Vector3 position, float volume, bool vibrate = true)
	{
		if (this.Clips == null || this.Clips.Length == 0)
		{
			return null;
		}
		int num = Random.Range(0, this.Clips.Length);
		AudioClip audioClip = this.Clips[num];
		if (audioClip == null)
		{
			return null;
		}
		if (this.Volume < Mathf.Epsilon)
		{
			return null;
		}
		if (prefab == null)
		{
			prefab = Audio.DefaultAudioSourcePrefab;
			if (prefab == null)
			{
				return null;
			}
		}
		if (!AudioEventManager.TryPlayAudioClip(audioClip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = this.Volume * volume;
		audioSource.pitch = this.SelectPitch();
		audioSource.PlayOneShot(audioClip);
		if (vibrate)
		{
			this.PlayVibration(num, audioSource);
		}
		return audioSource;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0002601A File Offset: 0x0002421A
	public void PlayVibrationRandom()
	{
		if (this.vibrations != null && this.vibrations.Length != 0)
		{
			this.PlayVibration(Random.Range(0, this.vibrations.Length), null);
		}
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00026044 File Offset: 0x00024244
	public void PlayVibration(AudioClip clip, AudioSource source)
	{
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		if (this.Clips.Length == 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < this.Clips.Length; i++)
		{
			if (this.Clips[i] == clip)
			{
				num = i;
				break;
			}
		}
		if (this.vibrations != null && this.vibrations.Length != 0)
		{
			VibrationManager.PlayVibrationClipOneShot(this.vibrations[num % this.vibrations.Length], null, false, "", false);
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x000260CC File Offset: 0x000242CC
	public void PlayVibration(int index, AudioSource source)
	{
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		if (this.vibrations != null && this.vibrations.Length != 0)
		{
			VibrationManager.PlayVibrationClipOneShot(this.vibrations[index % this.vibrations.Length], null, false, "", false);
		}
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0002611E File Offset: 0x0002431E
	public void PlayOnSource(AudioSource source)
	{
		if (!source)
		{
			return;
		}
		source.pitch = this.SelectPitch();
		source.clip = this.GetClip();
		source.Play();
	}

	// Token: 0x040007A5 RID: 1957
	public AudioClip[] Clips;

	// Token: 0x040007A6 RID: 1958
	public float PitchMin;

	// Token: 0x040007A7 RID: 1959
	public float PitchMax;

	// Token: 0x040007A8 RID: 1960
	public float Volume;

	// Token: 0x040007A9 RID: 1961
	[Space]
	public VibrationDataAsset[] vibrations;
}
