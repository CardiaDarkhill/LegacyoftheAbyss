using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200012F RID: 303
[CreateAssetMenu(fileName = "RandomAudioClipTable", menuName = "Hornet/Random Audio Clip Table", order = 1000)]
public class RandomAudioClipTable : ScriptableObject
{
	// Token: 0x170000CA RID: 202
	// (get) Token: 0x06000962 RID: 2402 RVA: 0x0002B358 File Offset: 0x00029558
	public int ClipCount
	{
		get
		{
			RandomAudioClipTable.ProbabilityAudioClip[] array = this.clips;
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x0002B368 File Offset: 0x00029568
	private bool IsUsingPriority()
	{
		return this.type > RandomAudioClipTable.AudioTypes.Normal;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0002B374 File Offset: 0x00029574
	public AudioClip SelectClip(bool forcePlay = false)
	{
		if (!this.CanPlay(forcePlay))
		{
			return null;
		}
		if (!forcePlay && Random.Range(0f, 1f) > this.totalProbability)
		{
			return null;
		}
		AudioClip result = this.SelectRandomClip();
		this.previousClip = result;
		return result;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x0002B3B8 File Offset: 0x000295B8
	public AudioClip SelectClipIgnoreProbability(bool forcePlay = false)
	{
		if (!this.CanPlay(forcePlay))
		{
			return null;
		}
		AudioClip result = this.SelectRandomClip();
		this.previousClip = result;
		return result;
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x0002B3E0 File Offset: 0x000295E0
	private AudioClip SelectRandomClip()
	{
		if (this.ClipCount <= 0)
		{
			return null;
		}
		if (this.disallowRepeatingPrevious)
		{
			if (this.conditions == null || this.conditions.Length != this.clips.Length)
			{
				this.conditions = new bool[this.clips.Length];
			}
			for (int i = 0; i < this.conditions.Length; i++)
			{
				this.conditions[i] = (this.clips[i].Clip != this.previousClip);
			}
		}
		else
		{
			this.conditions = null;
		}
		int num;
		if (Math.Abs(this.unselectedMultiplier - 1f) >= 0.001f)
		{
			return Probability.GetRandomItemByProbabilityFair<RandomAudioClipTable.ProbabilityAudioClip, AudioClip>(this.clips, out num, ref this.currentProbabilities, this.unselectedMultiplier, this.conditions);
		}
		return Probability.GetRandomItemByProbability<RandomAudioClipTable.ProbabilityAudioClip, AudioClip>(this.clips, out num, null, this.conditions);
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0002B4B8 File Offset: 0x000296B8
	public bool CanPlay(bool forcePlay)
	{
		if (!forcePlay)
		{
			if (Time.unscaledTimeAsDouble < this.nextPlayTime)
			{
				return false;
			}
			RandomAudioClipTable.LastPlayedInfo lastPlayedInfo;
			if (RandomAudioClipTable._lastPlayedInfos.TryGetValue(this.type, out lastPlayedInfo) && this.priority < lastPlayedInfo.Priority && Time.unscaledTimeAsDouble < lastPlayedInfo.EndTime)
			{
				return false;
			}
		}
		return this.type != RandomAudioClipTable.AudioTypes.Player || InteractManager.BlockingInteractable || GameManager.instance.gameSettings.playerVoiceEnabled;
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0002B530 File Offset: 0x00029730
	public void ReportPlayed(AudioClip clip, AudioSource spawnedAudioSource)
	{
		this.SetCooldown(this.cooldownDuration);
		if (this.addCooldownTo != null)
		{
			foreach (RandomAudioClipTable randomAudioClipTable in this.addCooldownTo)
			{
				if (!(randomAudioClipTable == null))
				{
					randomAudioClipTable.SetCooldown(this.cooldownDuration);
				}
			}
		}
		if (this.type == RandomAudioClipTable.AudioTypes.Normal)
		{
			return;
		}
		if (!clip || !spawnedAudioSource)
		{
			return;
		}
		RandomAudioClipTable.LastPlayedInfo lastPlayedInfo;
		if (RandomAudioClipTable._lastPlayedInfos.TryGetValue(this.type, out lastPlayedInfo))
		{
			lastPlayedInfo.PlayedOnSource.Stop();
			lastPlayedInfo.PlayedOnSource.Recycle<AudioSource>();
		}
		RandomAudioClipTable._lastPlayedInfos[this.type] = new RandomAudioClipTable.LastPlayedInfo
		{
			Priority = this.priority,
			EndTime = Time.unscaledTimeAsDouble + (double)clip.length,
			PlayedOnSource = spawnedAudioSource
		};
		RecycleResetHandler.Add(spawnedAudioSource.gameObject, delegate()
		{
			RandomAudioClipTable.LastPlayedInfo lastPlayedInfo2;
			if (RandomAudioClipTable._lastPlayedInfos.TryGetValue(this.type, out lastPlayedInfo2) && lastPlayedInfo2.PlayedOnSource == spawnedAudioSource)
			{
				RandomAudioClipTable._lastPlayedInfos.Remove(this.type);
			}
		});
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0002B643 File Offset: 0x00029843
	private void SetCooldown(float cooldown)
	{
		this.nextPlayTime = Time.unscaledTimeAsDouble + (double)cooldown;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0002B654 File Offset: 0x00029854
	public float SelectPitch()
	{
		float num = 0f;
		if (this.pitchEffects.HasFlag(AudioPitchEffects.Quickening))
		{
			HeroController instance = HeroController.instance;
			if (instance && instance.IsUsingQuickening)
			{
				num = 0.05f;
			}
		}
		if (Mathf.Approximately(this.pitchMin, this.pitchMax))
		{
			return this.pitchMax + num;
		}
		return Random.Range(this.pitchMin, this.pitchMax) + num;
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0002B6CA File Offset: 0x000298CA
	public float SelectVolume()
	{
		if (Mathf.Approximately(this.volumeMin, this.volumeMax))
		{
			return this.volumeMax;
		}
		return Random.Range(this.volumeMin, this.volumeMax);
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0002B6F8 File Offset: 0x000298F8
	public void PlayOneShotUnsafe(AudioSource audioSource, float pitchOffset, bool forcePlay = false)
	{
		if (audioSource == null)
		{
			return;
		}
		AudioClip audioClip = this.SelectClip(forcePlay);
		if (audioClip == null)
		{
			return;
		}
		audioSource.pitch = this.SelectPitch() + pitchOffset;
		audioSource.volume = this.SelectVolume();
		audioSource.PlayOneShot(audioClip);
		this.ReportPlayed(audioClip, null);
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0002B74C File Offset: 0x0002994C
	public void PlayOneShotUnsafe(AudioSource audioSource, float pitchOffset, float volumeScale, bool forcePlay = false)
	{
		if (audioSource == null)
		{
			return;
		}
		AudioClip audioClip = this.SelectClip(forcePlay);
		if (audioClip == null)
		{
			return;
		}
		audioSource.pitch = this.SelectPitch() + pitchOffset;
		float num = this.SelectVolume();
		audioSource.PlayOneShot(audioClip, num * volumeScale);
		this.ReportPlayed(audioClip, null);
	}

	// Token: 0x0400090B RID: 2315
	[SerializeField]
	private float pitchMin = 1f;

	// Token: 0x0400090C RID: 2316
	[SerializeField]
	private float pitchMax = 1f;

	// Token: 0x0400090D RID: 2317
	[SerializeField]
	private float volumeMin = 1f;

	// Token: 0x0400090E RID: 2318
	[SerializeField]
	private float volumeMax = 1f;

	// Token: 0x0400090F RID: 2319
	[SerializeField]
	private RandomAudioClipTable.ProbabilityAudioClip[] clips;

	// Token: 0x04000910 RID: 2320
	[SerializeField]
	[Range(0f, 1f)]
	private float totalProbability = 1f;

	// Token: 0x04000911 RID: 2321
	[SerializeField]
	private bool disallowRepeatingPrevious;

	// Token: 0x04000912 RID: 2322
	[SerializeField]
	private float unselectedMultiplier = 1f;

	// Token: 0x04000913 RID: 2323
	[SerializeField]
	private float cooldownDuration;

	// Token: 0x04000914 RID: 2324
	[SerializeField]
	private RandomAudioClipTable[] addCooldownTo;

	// Token: 0x04000915 RID: 2325
	[NonSerialized]
	private AudioClip previousClip;

	// Token: 0x04000916 RID: 2326
	[NonSerialized]
	private float[] currentProbabilities;

	// Token: 0x04000917 RID: 2327
	[NonSerialized]
	private double nextPlayTime;

	// Token: 0x04000918 RID: 2328
	[Space]
	[SerializeField]
	private RandomAudioClipTable.AudioTypes type;

	// Token: 0x04000919 RID: 2329
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingPriority", true, true, false)]
	private int priority;

	// Token: 0x0400091A RID: 2330
	[SerializeField]
	private AudioPitchEffects pitchEffects;

	// Token: 0x0400091B RID: 2331
	private bool[] conditions;

	// Token: 0x0400091C RID: 2332
	private static readonly Dictionary<RandomAudioClipTable.AudioTypes, RandomAudioClipTable.LastPlayedInfo> _lastPlayedInfos = new Dictionary<RandomAudioClipTable.AudioTypes, RandomAudioClipTable.LastPlayedInfo>();

	// Token: 0x0200146B RID: 5227
	[Serializable]
	private class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
	{
		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x0600837B RID: 33659 RVA: 0x0026910B File Offset: 0x0026730B
		public override AudioClip Item
		{
			get
			{
				return this.Clip;
			}
		}

		// Token: 0x04008320 RID: 33568
		public AudioClip Clip;
	}

	// Token: 0x0200146C RID: 5228
	private enum AudioTypes
	{
		// Token: 0x04008322 RID: 33570
		Normal,
		// Token: 0x04008323 RID: 33571
		Player
	}

	// Token: 0x0200146D RID: 5229
	private struct LastPlayedInfo
	{
		// Token: 0x04008324 RID: 33572
		public double EndTime;

		// Token: 0x04008325 RID: 33573
		public int Priority;

		// Token: 0x04008326 RID: 33574
		public AudioSource PlayedOnSource;
	}
}
