using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000103 RID: 259
[ActionCategory("Hollow Knight")]
public sealed class PlayAudioEventRandomV2 : PlayAudioEventBase
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000813 RID: 2067 RVA: 0x000265CA File Offset: 0x000247CA
	public AudioClip[] AudioClipsArray
	{
		get
		{
			this.UpdateClipsArray();
			return this.audioClipsArray;
		}
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000265D8 File Offset: 0x000247D8
	private void UpdateClipsArray()
	{
		int contentHash = this.audioClips.Values.GetContentHash<object>();
		if (this.clipHash != contentHash)
		{
			this.clipHash = contentHash;
			if (this.audioClips.Values == null)
			{
				this.audioClipsArray = null;
				return;
			}
			this.audioClipsArray = this.audioClips.Values.SafeCastToArray<AudioClip>();
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000815 RID: 2069 RVA: 0x00026631 File Offset: 0x00024831
	public VibrationDataAsset[] VibrationDataAssetsArray
	{
		get
		{
			this.UpdateVibrationsArray();
			return this.vibrationDataAssetsArray;
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00026640 File Offset: 0x00024840
	private void UpdateVibrationsArray()
	{
		int contentHash = this.vibrations.Values.GetContentHash<object>();
		if (this.vibrationHash != contentHash)
		{
			this.vibrationHash = contentHash;
			if (this.vibrations.Values == null)
			{
				this.vibrationDataAssetsArray = null;
				return;
			}
			this.vibrationDataAssetsArray = this.vibrations.Values.SafeCastToArray<VibrationDataAsset>();
		}
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x00026699 File Offset: 0x00024899
	public override void Awake()
	{
		this.UpdateClipsArray();
		this.UpdateVibrationsArray();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x000266A7 File Offset: 0x000248A7
	public override void Reset()
	{
		base.Reset();
		this.audioClips = null;
		this.vibrations = null;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x000266C0 File Offset: 0x000248C0
	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEventRandom audioEventRandom = new AudioEventRandom
		{
			Clips = this.AudioClipsArray,
			PitchMin = this.pitchMin.Value,
			PitchMax = this.pitchMax.Value,
			Volume = this.volume.Value,
			vibrations = this.VibrationDataAssetsArray
		};
		if (this.audioPlayerPrefab.IsNone)
		{
			return audioEventRandom.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEventRandom.SpawnAndPlayOneShot(this.audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}

	// Token: 0x040007BA RID: 1978
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	// Token: 0x040007BB RID: 1979
	[ArrayEditor(typeof(VibrationDataAsset), "", 0, 0, 65536)]
	public FsmArray vibrations;

	// Token: 0x040007BC RID: 1980
	private int clipHash;

	// Token: 0x040007BD RID: 1981
	private AudioClip[] audioClipsArray;

	// Token: 0x040007BE RID: 1982
	private int vibrationHash;

	// Token: 0x040007BF RID: 1983
	private VibrationDataAsset[] vibrationDataAssetsArray;
}
