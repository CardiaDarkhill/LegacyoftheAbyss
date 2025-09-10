using System;
using UnityEngine;

// Token: 0x02000471 RID: 1137
public class AudioVibrationPlayer : IAudioVibrationPlayer
{
	// Token: 0x060028A9 RID: 10409 RVA: 0x000B2EF3 File Offset: 0x000B10F3
	public VibrationEmission PlayAudioClip(AudioClip audioClip, string tag = null)
	{
		Debug.LogError("Play Audio clip vibration not implemented");
		return null;
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000B2F00 File Offset: 0x000B1100
	public VibrationEmission PlayAudioClip(AudioClip audioClip, AudioSource referenceSource, string tag = null)
	{
		Debug.LogError("Play Audio clip vibration not implemented");
		return null;
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000B2F0D File Offset: 0x000B110D
	public void StopEmissionsWithClip(AudioClip audioClip)
	{
		Debug.LogError("Stop Emissions with clip not implemented");
	}
}
