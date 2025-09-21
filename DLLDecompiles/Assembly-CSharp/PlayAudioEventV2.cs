using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000100 RID: 256
[ActionCategory("Hollow Knight")]
public class PlayAudioEventV2 : PlayAudioEventBase
{
	// Token: 0x06000806 RID: 2054 RVA: 0x00026308 File Offset: 0x00024508
	public override void Reset()
	{
		base.Reset();
		this.audioClip = null;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x00026318 File Offset: 0x00024518
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

	// Token: 0x06000808 RID: 2056 RVA: 0x000263AC File Offset: 0x000245AC
	public override void OnExit()
	{
		if (this.stopOnExit.Value && this.spawnedAudioSource != null)
		{
			this.spawnedAudioSource.Stop();
		}
		base.OnExit();
	}

	// Token: 0x040007B3 RID: 1971
	[ObjectType(typeof(AudioClip))]
	public FsmObject audioClip;

	// Token: 0x040007B4 RID: 1972
	public FsmBool stopOnExit;
}
