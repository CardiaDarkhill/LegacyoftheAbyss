using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020000FC RID: 252
[Serializable]
public struct AudioEvent
{
	// Token: 0x060007E3 RID: 2019 RVA: 0x0002594C File Offset: 0x00023B4C
	public void Reset()
	{
		this.PitchMin = 0.75f;
		this.PitchMax = 1.25f;
		this.Volume = 1f;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002596F File Offset: 0x00023B6F
	public float SelectPitch()
	{
		if (Mathf.Approximately(this.PitchMin, this.PitchMax))
		{
			return this.PitchMax;
		}
		return Random.Range(this.PitchMin, this.PitchMax);
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002599C File Offset: 0x00023B9C
	public AudioSource SpawnAndPlayOneShot(Vector3 position, Action onRecycled = null)
	{
		return this.SpawnAndPlayOneShot(null, position, onRecycled);
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x000259A7 File Offset: 0x00023BA7
	public AudioSource SpawnAndPlayOneShot(Vector3 position, bool vibrate, Action onRecycled = null)
	{
		return this.SpawnAndPlayOneShot(null, position, vibrate, onRecycled);
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x000259B4 File Offset: 0x00023BB4
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, Action onRecycled = null)
	{
		if (this.Volume < Mathf.Epsilon)
		{
			return null;
		}
		if (this.Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(this.Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = this.Volume;
		audioSource.pitch = this.SelectPitch();
		audioSource.PlayOneShot(this.Clip);
		this.PlayVibration(audioSource);
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00025A58 File Offset: 0x00023C58
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, bool vibrate, Action onRecycled = null)
	{
		if (this.Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(this.Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = this.Volume;
		audioSource.pitch = this.SelectPitch();
		audioSource.PlayOneShot(this.Clip);
		if (vibrate)
		{
			this.PlayVibration(audioSource);
		}
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x00025B00 File Offset: 0x00023D00
	public AudioSource SpawnAndPlayOneShot(AudioSource prefab, Vector3 position, float delay, Action onRecycled = null)
	{
		if (this.Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(this.Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = this.Volume;
		audioSource.pitch = this.SelectPitch();
		if (delay > 0f)
		{
			audioSource.clip = this.Clip;
			audioSource.Play((ulong)(delay * (float)this.Clip.frequency));
			onRecycled = (Action)Delegate.Combine(onRecycled, new Action(delegate()
			{
				audioSource.clip = null;
			}));
		}
		else
		{
			audioSource.PlayOneShot(this.Clip);
		}
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x00025C18 File Offset: 0x00023E18
	public AudioSource SpawnAndPlayLooped(AudioSource prefab, Vector3 position, float delay, Action onRecycled = null)
	{
		if (this.Clip == null)
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
		if (!AudioEventManager.TryPlayAudioClip(this.Clip, prefab, position))
		{
			return null;
		}
		AudioSource audioSource = prefab.Spawn(position);
		audioSource.priority = AudioSourcePriority.SPAWNED_ACTOR_PRIORITY;
		audioSource.volume = this.Volume;
		audioSource.pitch = this.SelectPitch();
		audioSource.loop = true;
		audioSource.clip = this.Clip;
		if (delay > 0f)
		{
			audioSource.Play((ulong)(delay * (float)this.Clip.frequency));
		}
		else
		{
			audioSource.Play();
		}
		onRecycled = (Action)Delegate.Combine(onRecycled, new Action(delegate()
		{
			audioSource.clip = null;
			audioSource.loop = false;
		}));
		RecycleResetHandler.Add(audioSource.gameObject, onRecycled);
		return audioSource;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x00025D34 File Offset: 0x00023F34
	public void PlayVibration(AudioSource audioSource)
	{
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		if (this.vibrationDataAsset)
		{
			VibrationManager.PlayVibrationClipOneShot(this.vibrationDataAsset, null, false, "", false);
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x00025D77 File Offset: 0x00023F77
	public void PlayOnSource(AudioSource source)
	{
		if (!source)
		{
			return;
		}
		source.pitch = this.SelectPitch();
		source.clip = this.Clip;
		source.Play();
	}

	// Token: 0x0400079F RID: 1951
	public AudioClip Clip;

	// Token: 0x040007A0 RID: 1952
	public float PitchMin;

	// Token: 0x040007A1 RID: 1953
	public float PitchMax;

	// Token: 0x040007A2 RID: 1954
	public float Volume;

	// Token: 0x040007A3 RID: 1955
	public VibrationDataAsset vibrationDataAsset;

	// Token: 0x040007A4 RID: 1956
	public static readonly AudioEvent Default = new AudioEvent
	{
		PitchMin = 1f,
		PitchMax = 1f,
		Volume = 1f
	};
}
