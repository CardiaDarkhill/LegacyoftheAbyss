using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000101 RID: 257
[ActionCategory("Hollow Knight")]
public class PlayAudioEventDelayed : PlayAudioEventBase
{
	// Token: 0x0600080A RID: 2058 RVA: 0x000263E2 File Offset: 0x000245E2
	public override void Reset()
	{
		base.Reset();
		this.audioClip = null;
		this.delay = 0f;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x00026404 File Offset: 0x00024604
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
			return audioEvent.SpawnAndPlayOneShot(null, position, this.delay.Value, onRecycle);
		}
		return audioEvent.SpawnAndPlayOneShot(this.audioPlayerPrefab.Value as AudioSource, position, this.delay.Value, onRecycle);
	}

	// Token: 0x040007B5 RID: 1973
	[ObjectType(typeof(AudioClip))]
	public FsmObject audioClip;

	// Token: 0x040007B6 RID: 1974
	public FsmFloat delay;
}
