using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010F RID: 271
public sealed class AudioPreloader : MonoBehaviour
{
	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x0600087C RID: 2172 RVA: 0x00028077 File Offset: 0x00026277
	public static AudioPreloader Instance
	{
		get
		{
			if (!AudioPreloader.initialised)
			{
				AudioPreloader._instance = new GameObject("AudioPreloader").AddComponent<AudioPreloader>();
				if (!AudioPreloader.initialised)
				{
					AudioPreloader.initialised = true;
				}
			}
			return AudioPreloader._instance;
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x000280A6 File Offset: 0x000262A6
	private void Awake()
	{
		if (AudioPreloader._instance == null)
		{
			AudioPreloader._instance = this;
			if (!AudioPreloader.initialised)
			{
				AudioPreloader.initialised = true;
				return;
			}
		}
		else if (AudioPreloader._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x000280E1 File Offset: 0x000262E1
	private void OnDestroy()
	{
		if (AudioPreloader._instance == this)
		{
			AudioPreloader.initialised = false;
			AudioPreloader._instance = null;
			AudioPreloader.preloadedClips.Clear();
		}
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00028106 File Offset: 0x00026306
	private static AudioSource GetAudioSource()
	{
		AudioSource audioSource = AudioPreloader.Instance.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		return audioSource;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0002811E File Offset: 0x0002631E
	public static void PreloadClip(AudioClip clip)
	{
		if (AudioPreloader.preloadedClips.Add(clip))
		{
			AudioPreloader.GetAudioSource().clip = clip;
			clip.LoadAudioData();
		}
	}

	// Token: 0x04000822 RID: 2082
	private static HashSet<AudioClip> preloadedClips = new HashSet<AudioClip>();

	// Token: 0x04000823 RID: 2083
	private static AudioPreloader _instance;

	// Token: 0x04000824 RID: 2084
	private static bool initialised;
}
