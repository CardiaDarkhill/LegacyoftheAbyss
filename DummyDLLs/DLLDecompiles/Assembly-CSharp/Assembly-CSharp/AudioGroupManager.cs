using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000107 RID: 263
public static class AudioGroupManager
{
	// Token: 0x0600082D RID: 2093 RVA: 0x00026AD4 File Offset: 0x00024CD4
	public static void ClearAudioGroups()
	{
		AudioGroupManager.audioGroups.Clear();
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x00026AE0 File Offset: 0x00024CE0
	public static AudioGroupManager.AudioGroup EnsureGroupExists(string groupId, int maxActive = 5, int maxPerFrame = 2)
	{
		AudioGroupManager.AudioGroup audioGroup;
		if (!AudioGroupManager.audioGroups.TryGetValue(groupId, out audioGroup))
		{
			audioGroup = new AudioGroupManager.AudioGroup(maxActive, maxPerFrame);
			AudioGroupManager.audioGroups.Add(groupId, audioGroup);
		}
		return audioGroup;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x00026B14 File Offset: 0x00024D14
	public static bool CanPlay(string groupId, out AudioGroupManager.AudioGroup group)
	{
		if (!AudioGroupManager.audioGroups.TryGetValue(groupId, out group))
		{
			group = AudioGroupManager.EnsureGroupExists(groupId, 5, 2);
		}
		return group.startedThisFrame < group.maxPerFrame && group.activeSources.Count < group.maxActive;
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00026B61 File Offset: 0x00024D61
	public static bool CanPlayLoop(string groupId, out AudioGroupManager.AudioGroup group)
	{
		if (!AudioGroupManager.audioGroups.TryGetValue(groupId, out group))
		{
			group = AudioGroupManager.EnsureGroupExists(groupId, 5, 2);
		}
		return group.activeSources.Count < group.maxActive;
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x00026B94 File Offset: 0x00024D94
	public static bool PlayClip(string groupId, AudioSource source, AudioClip clip)
	{
		AudioGroupManager.AudioGroup audioGroup;
		if (!AudioGroupManager.CanPlay(groupId, out audioGroup))
		{
			return false;
		}
		source.clip = clip;
		source.Play();
		audioGroup.startedThisFrame++;
		audioGroup.activeSources.Add(source);
		return true;
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00026BD8 File Offset: 0x00024DD8
	public static bool PlayOneShotClip(string groupId, AudioSource source, AudioClip clip)
	{
		AudioGroupManager.AudioGroup audioGroup;
		if (!AudioGroupManager.CanPlay(groupId, out audioGroup))
		{
			return false;
		}
		source.PlayOneShot(clip);
		audioGroup.startedThisFrame++;
		audioGroup.activeSources.Add(source);
		return true;
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00026C14 File Offset: 0x00024E14
	public static void PlayLoopClip(string groupId, AudioSource source, AudioClip clip, float volume)
	{
		AudioGroupManager.AudioGroup audioGroup;
		if (AudioGroupManager.CanPlayLoop(groupId, out audioGroup))
		{
			if (clip)
			{
				source.clip = clip;
			}
			source.volume = volume;
			source.Play();
			audioGroup.activeSources.Add(source);
			return;
		}
		audioGroup.clipQueue.Add(new AudioGroupManager.QueuedClip
		{
			source = source,
			clip = clip,
			volume = volume
		});
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x00026C7C File Offset: 0x00024E7C
	public static void RemoveLoopClip(string groupId, AudioSource source)
	{
		AudioGroupManager.AudioGroup audioGroup;
		if (AudioGroupManager.audioGroups.TryGetValue(groupId, out audioGroup))
		{
			audioGroup.activeSources.RemoveAll((AudioSource s) => s == source);
			audioGroup.clipQueue.RemoveAll((AudioGroupManager.QueuedClip q) => q.source == source);
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00026CD8 File Offset: 0x00024ED8
	public static void UpdateAudioGroups()
	{
		foreach (AudioGroupManager.AudioGroup audioGroup in AudioGroupManager.audioGroups.Values)
		{
			audioGroup.startedThisFrame = 0;
			audioGroup.activeSources.RemoveAll((AudioSource source) => source == null || !source.isPlaying);
			int num = 0;
			while (num < audioGroup.clipQueue.Count && audioGroup.activeSources.Count < audioGroup.maxActive)
			{
				AudioGroupManager.QueuedClip queuedClip = audioGroup.clipQueue[num];
				audioGroup.clipQueue.RemoveAt(num);
				if (queuedClip.clip != null)
				{
					queuedClip.source.clip = queuedClip.clip;
				}
				if (!(queuedClip.source.clip == null))
				{
					queuedClip.source.Play();
					queuedClip.source.volume = queuedClip.volume;
					audioGroup.activeSources.Add(queuedClip.source);
				}
			}
		}
	}

	// Token: 0x040007C8 RID: 1992
	private static readonly Dictionary<string, AudioGroupManager.AudioGroup> audioGroups = new Dictionary<string, AudioGroupManager.AudioGroup>();

	// Token: 0x040007C9 RID: 1993
	public const int MAX_PER_FRAME = 2;

	// Token: 0x040007CA RID: 1994
	public const int MAX_ACTIVE = 5;

	// Token: 0x02001457 RID: 5207
	public sealed class AudioGroup
	{
		// Token: 0x06008347 RID: 33607 RVA: 0x00268889 File Offset: 0x00266A89
		public AudioGroup(int maxActive, int maxPerFrame)
		{
			this.maxActive = ((maxActive <= 0) ? 5 : maxActive);
			this.maxPerFrame = ((maxPerFrame <= 0) ? 2 : maxPerFrame);
		}

		// Token: 0x06008348 RID: 33608 RVA: 0x002688C3 File Offset: 0x00266AC3
		public void AddSource(AudioSource source)
		{
			this.startedThisFrame++;
			this.activeSources.Add(source);
		}

		// Token: 0x040082DD RID: 33501
		public int maxActive;

		// Token: 0x040082DE RID: 33502
		public int maxPerFrame;

		// Token: 0x040082DF RID: 33503
		public int startedThisFrame;

		// Token: 0x040082E0 RID: 33504
		public List<AudioSource> activeSources = new List<AudioSource>();

		// Token: 0x040082E1 RID: 33505
		public List<AudioGroupManager.QueuedClip> clipQueue = new List<AudioGroupManager.QueuedClip>();
	}

	// Token: 0x02001458 RID: 5208
	public sealed class QueuedClip
	{
		// Token: 0x040082E2 RID: 33506
		public AudioSource source;

		// Token: 0x040082E3 RID: 33507
		public AudioClip clip;

		// Token: 0x040082E4 RID: 33508
		public float volume;
	}
}
