using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000102 RID: 258
[ActionCategory("Hollow Knight")]
public class PlayAudioEventRandom : PlayAudioEventBase
{
	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600080D RID: 2061 RVA: 0x000264B7 File Offset: 0x000246B7
	public AudioClip[] AudioClipsArray
	{
		get
		{
			this.UpdateClipsArray();
			return this.audioClipsArray;
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x000264C8 File Offset: 0x000246C8
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

	// Token: 0x0600080F RID: 2063 RVA: 0x00026521 File Offset: 0x00024721
	public override void Awake()
	{
		this.UpdateClipsArray();
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x00026529 File Offset: 0x00024729
	public override void Reset()
	{
		base.Reset();
		this.audioClips = null;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00026538 File Offset: 0x00024738
	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEventRandom audioEventRandom = new AudioEventRandom
		{
			Clips = this.AudioClipsArray,
			PitchMin = this.pitchMin.Value,
			PitchMax = this.pitchMax.Value,
			Volume = this.volume.Value
		};
		if (this.audioPlayerPrefab.IsNone)
		{
			return audioEventRandom.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEventRandom.SpawnAndPlayOneShot(this.audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}

	// Token: 0x040007B7 RID: 1975
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	// Token: 0x040007B8 RID: 1976
	private int clipHash;

	// Token: 0x040007B9 RID: 1977
	private AudioClip[] audioClipsArray;
}
