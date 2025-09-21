using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000106 RID: 262
public class AudioEventManager : MonoBehaviour
{
	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000821 RID: 2081 RVA: 0x000268BD File Offset: 0x00024ABD
	private static AudioEventManager Instance
	{
		get
		{
			if (!AudioEventManager._instance)
			{
				AudioEventManager._instance = new GameObject("AudioEvent Manager", new Type[]
				{
					typeof(AudioEventManager)
				}).GetComponent<AudioEventManager>();
			}
			return AudioEventManager._instance;
		}
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x000268F7 File Offset: 0x00024AF7
	private void OnEnable()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002690A File Offset: 0x00024B0A
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002691D File Offset: 0x00024B1D
	private void OnDestroy()
	{
		if (AudioEventManager._instance == this)
		{
			AudioEventManager._instance = null;
		}
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00026934 File Offset: 0x00024B34
	private void Update()
	{
		try
		{
			this.temp.AddRange(this.clipReleaseTimesLeft.Keys);
			foreach (AudioClip key in this.temp)
			{
				float num = this.clipReleaseTimesLeft[key];
				num -= Time.unscaledDeltaTime;
				if (num <= 0f)
				{
					this.clipReleaseTimesLeft.Remove(key);
				}
				else
				{
					this.clipReleaseTimesLeft[key] = num;
				}
			}
		}
		finally
		{
			this.temp.Clear();
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x000269E8 File Offset: 0x00024BE8
	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		this.clipReleaseTimesLeft.Clear();
		this.temp.Clear();
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x00026A00 File Offset: 0x00024C00
	private static void RecordAudioClip(AudioClip clip)
	{
		AudioEventManager.Instance.InternalRecordAudioClip(clip);
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x00026A0D File Offset: 0x00024C0D
	private static bool CanPlayAudioClip(AudioClip clip)
	{
		return AudioEventManager.Instance.InternalCanPlayAudioClip(clip);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x00026A1C File Offset: 0x00024C1C
	public static bool TryPlayAudioClip(AudioClip clip, AudioSource prefab, Vector3 position)
	{
		if (!AudioEventManager.CanPlayAudioClip(clip))
		{
			return false;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return false;
		}
		if (prefab.spatialBlend > 0.95f && Vector3.Distance(silentInstance.mainCamera.transform.position, position) > prefab.maxDistance)
		{
			return false;
		}
		AudioEventManager.RecordAudioClip(clip);
		return true;
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x00026A77 File Offset: 0x00024C77
	private void InternalRecordAudioClip(AudioClip clip)
	{
		this.clipReleaseTimesLeft[clip] = Audio.AudioEventFrequencyLimit;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x00026A8C File Offset: 0x00024C8C
	private bool InternalCanPlayAudioClip(AudioClip clip)
	{
		float num;
		return !this.clipReleaseTimesLeft.TryGetValue(clip, out num) || num <= 0f;
	}

	// Token: 0x040007C5 RID: 1989
	private static AudioEventManager _instance;

	// Token: 0x040007C6 RID: 1990
	private readonly Dictionary<AudioClip, float> clipReleaseTimesLeft = new Dictionary<AudioClip, float>();

	// Token: 0x040007C7 RID: 1991
	private readonly List<AudioClip> temp = new List<AudioClip>();
}
