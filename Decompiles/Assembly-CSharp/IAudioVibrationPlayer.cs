using System;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public interface IAudioVibrationPlayer
{
	// Token: 0x060028A6 RID: 10406
	VibrationEmission PlayAudioClip(AudioClip audioClip, string tag = null);

	// Token: 0x060028A7 RID: 10407
	VibrationEmission PlayAudioClip(AudioClip audioClip, AudioSource referenceSource, string tag = null);

	// Token: 0x060028A8 RID: 10408
	void StopEmissionsWithClip(AudioClip audioClip);
}
