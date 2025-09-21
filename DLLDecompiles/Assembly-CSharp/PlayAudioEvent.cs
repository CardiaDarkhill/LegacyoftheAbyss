using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020000FF RID: 255
[ActionCategory("Hollow Knight")]
public class PlayAudioEvent : PlayAudioEventBase
{
	// Token: 0x06000803 RID: 2051 RVA: 0x0002625D File Offset: 0x0002445D
	public override void Reset()
	{
		base.Reset();
		this.audioClip = null;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002626C File Offset: 0x0002446C
	protected override AudioSource SpawnAudioEvent(Vector3 position, Action onRecycle)
	{
		AudioEvent audioEvent = new AudioEvent
		{
			Clip = (this.audioClip.Value as AudioClip),
			PitchMin = this.pitchMin.Value,
			PitchMax = this.pitchMax.Value,
			Volume = this.volume.Value
		};
		if (this.audioPlayerPrefab.IsNone)
		{
			return audioEvent.SpawnAndPlayOneShot(position, onRecycle);
		}
		return audioEvent.SpawnAndPlayOneShot(this.audioPlayerPrefab.Value as AudioSource, position, onRecycle);
	}

	// Token: 0x040007B2 RID: 1970
	[ObjectType(typeof(AudioClip))]
	public FsmObject audioClip;
}
