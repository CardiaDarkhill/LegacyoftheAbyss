using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class PlayAudioEventRandomBool : PlayAudioEventBase
{
	// Token: 0x0600081B RID: 2075 RVA: 0x0002675F File Offset: 0x0002495F
	public override void Reset()
	{
		base.Reset();
		this.audioClips = null;
		this.doPlay = true;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002677C File Offset: 0x0002497C
	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		if (!this.doPlay.Value)
		{
			return null;
		}
		AudioEventRandom audioEventRandom = new AudioEventRandom
		{
			Clips = this.audioClips.Values.Cast<AudioClip>().ToArray<AudioClip>(),
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

	// Token: 0x040007C0 RID: 1984
	[ArrayEditor(typeof(AudioClip), "", 0, 0, 65536)]
	public FsmArray audioClips;

	// Token: 0x040007C1 RID: 1985
	public FsmBool doPlay;
}
